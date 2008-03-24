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
		#region INWN2Plugin
		
		public MenuButtonItem PluginMenuItem {
			get { return null; }
		}
		
		
		public AdventureAuthorPluginPreferences Options {
			get { return (AdventureAuthorPluginPreferences)Preferences; }
		}	
		
		
		public object Preferences {
			get { return (object)AdventureAuthorPluginPreferences.Instance; }
			set { AdventureAuthorPluginPreferences.Instance = (AdventureAuthorPluginPreferences)value; }
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
		/// Called when the plugin starts (after being clicked in the menu)
		/// </summary>
		/// <param name="cHost"></param>
		public void Startup(INWN2PluginHost cHost)
		{
			
		}		
				
		
		/// <summary>
		/// Called when the toolset starts
		/// </summary>
		/// <param name="cHost"></param>
		public void Load(INWN2PluginHost cHost)
		{
			try {
				//form.App.Closing += OnNeverwinterNights2Closing;
				
				// Check directories:				
				if (!Directory.Exists(ModuleHelper.AdventureAuthorInstallDirectory)) {					
					Say.Error("Adventure Author files were not found at the expected location " + 
					          "(" + ModuleHelper.AdventureAuthorInstallDirectory + ").\n\n" +
					          "You may find that the software no longer runs correctly. " +
					          "If this is the case, try reinstalling Adventure Author.");
				}
				try {
					if (!Directory.Exists(ModuleHelper.PublicUserDirectory)) {
						Directory.CreateDirectory(ModuleHelper.PublicUserDirectory);	
					}	
					if (!Directory.Exists(ModuleHelper.PrivateUserDirectory)) {
						Directory.CreateDirectory(ModuleHelper.PrivateUserDirectory);	
					}	
					if (!Directory.Exists(ModuleHelper.DebugDirectory)) {
						Directory.CreateDirectory(ModuleHelper.DebugDirectory);	
					}	
					if (!Directory.Exists(ModuleHelper.UserLogDirectory)) {
						Directory.CreateDirectory(ModuleHelper.UserLogDirectory);	
					}	
					if (!Directory.Exists(ModuleHelper.WorksheetsDirectory)) {
						Directory.CreateDirectory(ModuleHelper.WorksheetsDirectory);	
					}	
					if (!Directory.Exists(ModuleHelper.MagnetBoardsDirectory)) {
						Directory.CreateDirectory(ModuleHelper.MagnetBoardsDirectory);
						// missing magnet list will be created by MagnetBoardViewer if required
					}	
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
				MagnetBoardViewer magnets = new MagnetBoardViewer();
							
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
		/// Called when the toolset closes
		/// </summary>
		/// <param name="cHost"></param>
		public void Unload(INWN2PluginHost cHost)
		{					
//			if (WriterWindow.Instance != null) {
//				WriterWindow.Instance.Close();
//			}
//			if (VariablesWindow.Instance != null) {
//				VariablesWindow.Instance.Close();
//			}
//			if (MagnetBoardViewer.Instance != null) {
//				MagnetBoardViewer.Instance.Close();
//			}
//			// TODO if analysis window is not null
//			// TODO if evaluation window is not null
//			
//			Log.WriteAction(LogAction.exited,"toolset");
//			LogWriter.StopRecording();
//			DebugWriter.StopRecording();
		}
		
		
		private object padlock = new object();
		
		
		/// <summary>
		/// Called when the plugin finishes
		/// </summary>
		public void Shutdown(INWN2PluginHost cHost)
		{
			lock (padlock) {
				if (WriterWindow.Instance != null) {
					WriterWindow.Instance.Close();
				}
				if (VariablesWindow.Instance != null) {
					VariablesWindow.Instance.Close();
				}
				if (MagnetBoardViewer.Instance != null) {
					MagnetBoardViewer.Instance.Close();
				}
				// TODO if analysis window is not null
				// TODO if evaluation window is not null
			}
			
			Log.WriteAction(LogAction.exited,"toolset");
			LogWriter.StopRecording();
			DebugWriter.StopRecording();
		}
		
		#endregion INWN2Plugin
		
				
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
			catch (Exception) {
				//MessageBox.Show("Failed to delete temp modules on loading.\n\n\n" + ex.ToString());
			}
		}	
	}
}
