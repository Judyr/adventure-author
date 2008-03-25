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
using System.Windows;
using System.Collections.Generic;
using System.Windows.Forms;
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
		
		#endregion
		
		#region Methods
		
		/// <summary>
		/// Called when the toolset starts.
		/// </summary>
		/// <param name="cHost"></param>
		public void Load(INWN2PluginHost cHost)
		{
			try {
				// Check directories:				
				if (!Directory.Exists(ModuleHelper.AdventureAuthorInstallDirectory)) {					
					Say.Error("Adventure Author files were not found at the expected location " + 
					          "(" + ModuleHelper.AdventureAuthorInstallDirectory + ").\n\n" +
					          "You may find that the software no longer runs correctly. " +
					          "If this is the case, try reinstalling Adventure Author.");
				}
				try {
					EnsureDirectoryExists(ModuleHelper.PublicUserDirectory);
					EnsureDirectoryExists(ModuleHelper.PrivateUserDirectory);
					EnsureDirectoryExists(ModuleHelper.DebugDirectory);
					EnsureDirectoryExists(ModuleHelper.UserLogDirectory);
					EnsureDirectoryExists(ModuleHelper.WorksheetsDirectory);
					EnsureDirectoryExists(ModuleHelper.MagnetBoardsDirectory);
				} 
				catch (Exception e) {
					Say.Error("Was unable to create an Adventure Author app data directory for this user. " +
					          "Some features of Adventure Author will not work correctly until this is resolved.",e);
				}
								
				Toolset.Plugin = this;
				
				// Delete temp modules created during previous sessions:
				ClearTempModules();
				
				// Start recording debug messages and user actions:
				DebugWriter.StartRecording();
				LogWriter.StartRecording();
				
				// Create an instance of the magnets board on loading, so that it's
				// ready to receive ideas submitted from the main GUI:
				MagnetBoardViewer magnetBoardViewer = new MagnetBoardViewer();
				sessionWindows.Add(magnetBoardViewer);
							
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
		/// <param name="cHost"></param>
		public void Unload(INWN2PluginHost cHost)
		{			
			lock (padlock) {
				CloseSessionWindows();
				CloseModuleWindows();
			}
			
			Log.WriteAction(LogAction.exited,"toolset");
			LogWriter.StopRecording();
			DebugWriter.StopRecording();
		}
				
		
		/// <summary>
		/// Check that a given directory exists; if it doesn't, create it.
		/// </summary>
		/// <param name="directory">The directory to check for/create</param>
		private void EnsureDirectoryExists(string directory)
		{
			if (!Directory.Exists(directory)) {
				Directory.CreateDirectory(directory);	
			}	
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
					Say.Debug("closing window: " + window.Title + " (" + window.GetType() + ")");
					window.Close();
				}
			}
		}
		
		#endregion
	}
}
