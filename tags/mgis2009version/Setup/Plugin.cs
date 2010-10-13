/*
 *   This file is part of Adventure Author.
 *
 *   Adventure Author is copyright Heriot-Watt University 2006-2008.
 *
 *   This copyright and licence apply to all source code, compiled code,
 *   documentation, graphics and auxiliary files, except where otherwise stated.
 *
 *   Adventure Author is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 *   Adventure Author is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 * 
 *   Adventure Author is a plugin for Atari's Neverwinter Nights 2, a COMMERCIAL
 *   product. Permission is given to link this GPL-covered plug-in with the 
 *   non-free main program. 
 *
 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.IO;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Forms;
using AdventureAuthor.Achievements;
using AdventureAuthor.Conversations.UI;
using AdventureAuthor.Core;
using AdventureAuthor.Variables.UI;
using AdventureAuthor.Utils;
using AdventureAuthor.Ideas;
using NWN2Toolset.Plugins;
using TD.SandBar;
using form = NWN2Toolset.NWN2ToolsetMainForm;

namespace AdventureAuthor.Setup
{
	/// <summary>
	/// Plugin for the NWN2Toolset modification Adventure Author.
	/// </summary>
	public class AdventureAuthorPlugin : INWN2Plugin
	{	
		#region Fields	
		
		/// <summary>
		/// A padlock object for locking purposes.
		/// </summary>
		private object padlock = new object();
		
		
		/// <summary>
		/// Not used.
		/// </summary>
		public MenuButtonItem PluginMenuItem {
			get { return null; }
		}
		
		
		/// <summary>
		/// The user preference settings relating to this plugin.
		/// </summary>
		public object Preferences {
			get { return (object)AdventureAuthorPluginPreferences.Instance; }
			set { AdventureAuthorPluginPreferences.Instance = (AdventureAuthorPluginPreferences)value; }
		}
		
		
		/// <summary>
		/// The user preference settings related to this plugin.
		/// </summary>
		/// <remarks>This is identical to the Preferences property, 
		/// but no cast is required to use it.</remarks>
		public AdventureAuthorPluginPreferences Options {
			get { return (AdventureAuthorPluginPreferences)Preferences; }
			set { AdventureAuthorPluginPreferences.Instance = value; }
		}	
		
		
		public string Name {
			get { return "AdventureAuthor"; }
		}
		
		
		public string DisplayName {
			get { return "Adventure Author"; }
		}
		
		
		public string MenuName {
			get { return "Adventure Author"; }
		}
		
		
		/// <summary>
		/// Windows which should be shut down when the module is closed.
		/// </summary>
		private List<Window> moduleWindows = new List<Window>(4);
		public List<Window> ModuleWindows	{
			get { return moduleWindows; }
		}
		
		
		/// <summary>
		/// Windows which should be shut down when the toolset is closed.
		/// </summary>
		private List<Window> sessionWindows = new List<Window>(1);
		public List<Window> SessionWindows {
			get { return sessionWindows; }
		}
		
		
		/// <summary>
		/// An information profile about the current user.
		/// </summary>
		private UserProfile profile;
		public UserProfile Profile {
			get { return profile; }
		}
		
		#endregion
		
		#region Methods
		
		/// <summary>
		/// Called when the toolset starts.
		/// </summary>
		/// <param name="cHost"></param>
		public void Load(INWN2PluginHost cHost)
		{
			try {
				DebugWriter.DebugDirectory = ModuleHelper.DebugDirectory;		
				Tools.EnsureDirectoryExists(AdventureAuthorPluginPreferences.LocalAppDataDirectory);
				Tools.EnsureDirectoryExists(ModuleHelper.PublicUserDirectory);
				Tools.EnsureDirectoryExists(ModuleHelper.DebugDirectory);
								
				Toolset.Plugin = this;
				
				// Delete temp modules created during previous sessions:
				ClearTempModules();
				
				// Start recording debug messages and user actions:
				DebugWriter.StartRecording();
				LogWriter.StartRecording("toolset");
				
				SetupUserProfile();
				SetupMyAchievements();
							
				// Modify the main user interface:
				Toolset.SetupUI();
				
				Log.WriteAction(LogAction.launched,"toolset");
			}
			catch (Exception e) {
				Say.Error("A problem occurred when trying to set up the toolset. " + 
				          "You may experience problems which require you to reinstall Adventure Author.",e);
			}
		}	
		
		
		/// <summary>
		/// Called when the plugin starts (after being clicked in the menu).
		/// </summary>
		/// <param name="cHost"></param>
		public void Startup(INWN2PluginHost cHost)
		{
			
		}	
		
		
		/// <summary>
		/// Called when the plugin finishes
		/// </summary>
		public void Shutdown(INWN2PluginHost cHost)
		{
			
		}	
						
		
		/// <summary>
		/// Called when the toolset closes - ensure that every window Adventure Author has
		/// opened is closed again, otherwise the toolset crashes.
		/// </summary>
		public void Unload(INWN2PluginHost cHost)
		{			
			lock (padlock) {
				CloseSessionWindows();
				CloseModuleWindows();
			}
			
			if (profile != null) {
				profile.Serialize(AdventureAuthorPluginPreferences.UserProfilePath);
			}
			
			Log.WriteAction(LogAction.exited,"toolset");
			LogWriter.StopRecording();
			DebugWriter.StopRecording();
		}
		
				
		/// <summary>
		/// Delete any temp modules in the modules directory.
		/// </summary>
		private static void ClearTempModules()
		{
			try {
				string[] temppaths = Directory.GetDirectories(form.ModulesDirectory,"temp*");
				foreach (string path in temppaths) {
					if (Directory.Exists(path)) {
						DirectoryInfo dir = new DirectoryInfo(path);
						dir.Delete(true);
					}
				}
			}
			catch (Exception e) {
				//Say.Debug("Failed to delete temp modules.\n" + e.ToString());
			}
		}	
		
		
		/// <summary>
		/// Close all Adventure Author windows.
		/// </summary>
		public void CloseSessionWindows()
		{
			CloseWindows(sessionWindows);
		}
		
		
		/// <summary>
		/// Close all Adventure Author windows that relate to the module that's currently open.
		/// </summary>
		public void CloseModuleWindows()
		{
			CloseWindows(moduleWindows);
		}
		
		
		/// <summary>
		/// Close a list of windows.
		/// </summary>
		/// <param name="windows">The list of windows to close</param>
		private void CloseWindows(List<Window> windows)
		{
			foreach (Window window in windows) {
				if (window != null) {
					window.Close();
				}
			}
		}
		
		
		/// <summary>
		/// Check whether a user profile already exists, and if not, create a new one.
		/// </summary>
		private void SetupUserProfile()
		{
			if (File.Exists(AdventureAuthorPluginPreferences.UserProfilePath)) {
				try {
					object obj = Serialization.Deserialize(AdventureAuthorPluginPreferences.UserProfilePath,
					                                       typeof(UserProfile));
					profile = (UserProfile)obj;
				}
				catch (Exception e) {
					System.Diagnostics.Debug.WriteLine("The file at location " + 
					                                   AdventureAuthorPluginPreferences.UserProfilePath +
					          						   " is not a valid user profile file.");
					profile = new UserProfile(new ObservableCollection<Award>());
				}
			}
			else {
				profile = new UserProfile(new ObservableCollection<Award>());
			}
		}
		
		
		/// <summary>
		/// Set up the My Achievements system, monitoring user activity and granting
		/// awards when their criteria are met.
		/// </summary>
		private void SetupMyAchievements()
		{			
			// Listen for logged messages from other applications:
			PipeCommunication.ThreadedListen(PipeCommunication.MYTASKSLOG);
			PipeCommunication.ThreadedListen(PipeCommunication.FRIDGEMAGNETSLOG);
			PipeCommunication.ThreadedListen(PipeCommunication.COMMENTCARDSLOG);
			
			WordCountMonitor wordCountMonitor = new WordCountMonitor(Profile.WordCount);
			wordCountMonitor.AddAward(new WordsmithAward("Wordsmith (Bronze)",WordsmithAward.BRONZEWORDCOUNT),true);
			wordCountMonitor.AddAward(new WordsmithAward("Wordsmith (Silver)",WordsmithAward.SILVERWORDCOUNT),true);
			wordCountMonitor.AddAward(new WordsmithAward("Wordsmith (Gold)",WordsmithAward.GOLDWORDCOUNT),true);
			wordCountMonitor.AddAward(new WordsmithAward("Wordsmith (Emerald)",WordsmithAward.EMERALDWORDCOUNT),true);
			wordCountMonitor.AddAward(new WordsmithAward("Wordsmith (Sapphire)",WordsmithAward.SAPPHIREWORDCOUNT),true);
			wordCountMonitor.AddAward(new WordsmithAward("Wordsmith (Ruby)",WordsmithAward.RUBYWORDCOUNT),true);
			wordCountMonitor.AddAward(new WordsmithAward("Wordsmith (Diamond)",WordsmithAward.DIAMONDWORDCOUNT),true);
			profile.AddMonitor(wordCountMonitor);
			
			MyTasksMonitor myTasksMonitor = new MyTasksMonitor(Profile.MyTasksActivity);
			myTasksMonitor.AddAward(new ManagerAward("Manager (Bronze)",ManagerAward.BRONZEUSAGE),true);
			myTasksMonitor.AddAward(new ManagerAward("Manager (Silver)",ManagerAward.SILVERUSAGE),true);
			myTasksMonitor.AddAward(new ManagerAward("Manager (Gold)",ManagerAward.GOLDUSAGE),true);
			myTasksMonitor.AddAward(new ManagerAward("Manager (Emerald)",ManagerAward.EMERALDUSAGE),true);
			myTasksMonitor.AddAward(new ManagerAward("Manager (Sapphire)",ManagerAward.SAPPHIREUSAGE),true);
			myTasksMonitor.AddAward(new ManagerAward("Manager (Ruby)",ManagerAward.RUBYUSAGE),true);
			myTasksMonitor.AddAward(new ManagerAward("Manager (Diamond)",ManagerAward.DIAMONDUSAGE),true);
			profile.AddMonitor(myTasksMonitor);
			
			FridgeMagnetsMonitor fridgeMagnetsMonitor = new FridgeMagnetsMonitor(Profile.FridgeMagnetsActivity);
			fridgeMagnetsMonitor.AddAward(new ImagineerAward("Imagineer (Bronze)",ImagineerAward.BRONZEUSAGE),true);
			fridgeMagnetsMonitor.AddAward(new ImagineerAward("Imagineer (Silver)",ImagineerAward.SILVERUSAGE),true);
			fridgeMagnetsMonitor.AddAward(new ImagineerAward("Imagineer (Gold)",ImagineerAward.GOLDUSAGE),true);
			fridgeMagnetsMonitor.AddAward(new ImagineerAward("Imagineer (Emerald)",ImagineerAward.EMERALDUSAGE),true);
			fridgeMagnetsMonitor.AddAward(new ImagineerAward("Imagineer (Sapphire)",ImagineerAward.SAPPHIREUSAGE),true);
			fridgeMagnetsMonitor.AddAward(new ImagineerAward("Imagineer (Ruby)",ImagineerAward.RUBYUSAGE),true);
			fridgeMagnetsMonitor.AddAward(new ImagineerAward("Imagineer (Diamond)",ImagineerAward.DIAMONDUSAGE),true);
			profile.AddMonitor(fridgeMagnetsMonitor);
		}
		
		#endregion
	}
}
