/*
 *   This file is part of Adventure Author.
 *
 *   Adventure Author is copyright Heriot-Watt University 2006-2007.
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
		
		
		public object Preferences {
			get { return null; }
			set {}
		}
		
		
		public string Name {
			get { return "Adventure Author"; }
		}
		
		
		public string DisplayName {
			get { return "Adventure Author"; }
		}
		
		
		public string MenuName {
			get { return "Adventure Author"; }
		}
		
		
		private static string modulesDirectory = form.ModulesDirectory;
		
		
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
				// Check directories:
				if (!Directory.Exists(ModuleHelper.AdventureAuthorDir)) {
					throw new DirectoryNotFoundException("Adventure Author installation directory at " + ModuleHelper.AdventureAuthorDir + 
					                                     "was missing.");
				}					
				
				// Delete temp modules created during previous sessions:
				ClearTempModules();
				
				// Start recording debug messages and user actions:
				//LogWindow logWindow = new LogWindow();
				//logWindow.Show();
				DebugWriter.StartRecording();
				LogWriter.StartRecording();
				
				Log.WriteAction(Log.Action.launched,"toolset");
			}
			catch (DirectoryNotFoundException e) {
				MessageBox.Show("Required Adventure Author files were not found at the expected location - software may " +
				                "not function correctly. \n\n" + e.ToString());
				//CloseToolset();
			}
					
			// Instantiate windows now to speed things up later on:
			WriterWindow.Instance = new WriterWindow();
							
			// Set up the Adventure Author toolset:
			Toolset.SetupUI();
		}	
		
		
		/// <summary>
		/// Called when the toolset closes
		/// </summary>
		/// <param name="cHost"></param>
		public void Unload(INWN2PluginHost cHost)
		{		
			Log.WriteAction(Log.Action.exited,"toolset");
			LogWriter.StopRecording();
			DebugWriter.StopRecording();
		}
		
		
		/// <summary>
		/// Called when the plugin finishes
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void Shutdown(INWN2PluginHost cHost)
		{
			
		}
		
		#endregion INWN2Plugin
		
				
		/// <summary>
		/// Delete any temp modules in the modules directory.
		/// </summary>
		private static void ClearTempModules()
		{
			try {
				string[] temppaths = Directory.GetDirectories(modulesDirectory,"temp*");
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
						
	
		private static void CloseToolset()
		{
			throw new NotImplementedException();
		}
	}
}
