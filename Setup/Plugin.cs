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
using System.Windows.Forms;
using AdventureAuthor.Conversations.UI;
using AdventureAuthor.Core;
using AdventureAuthor.Variables.UI;
using AdventureAuthor.Utils;
using NWN2Toolset.Plugins;
using TD.SandBar;

namespace AdventureAuthor.Setup
{
	/// <summary>
	/// Plugin for the NWN2Toolset modification Adventure Author.
	/// </summary>
	public class Plugin : INWN2Plugin
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
		
		public void Startup(INWN2PluginHost cHost)
		/// <summary>
		/// Called when the plugin starts (after being clicked in the menu)
		/// </summary>
		/// <param name="cHost"></param>
		{
			
		}		
				
		public void Load(INWN2PluginHost cHost)
		/// <summary>
		/// Called when the toolset starts
		/// </summary>
		/// <param name="cHost"></param>
		{
			if (!DirectoriesExist() && !(bool)Say.Question("Continue loading Neverwinter Nights 2 toolset?",MessageBoxButtons.YesNo)) {
				CloseToolset();
			}
			else {
				// Instantiate windows now to speed things up later on:
				WriterWindow.Instance = new WriterWindow();
				
				// Start recording debug messages and user actions:
				DebugLog.StartRecording();
				Log.StartRecording();	
				
				// Set up the Adventure Author toolset:
				Toolset.SetupUI();
			}
		}	
		
		public void Unload(INWN2PluginHost cHost)
		/// <summary>
		/// Called when the toolset closes
		/// </summary>
		/// <param name="cHost"></param>
		{		
			Log.StopRecording();
			DebugLog.StopRecording();
		}
		
		public void Shutdown(INWN2PluginHost cHost)
		/// <summary>
		/// Called when the plugin finishes
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		{
			
		}
		
		#endregion INWN2Plugin
		
		/// <summary>
		/// Check that necessary Adventure Author directories already exist - if not, return false.
		/// </summary>
		/// <returns></returns>
		private static bool DirectoriesExist()
		{
			if (!Directory.Exists(Adventure.AdventureAuthorDir)) {
				Say.Error("Adventure Author directory could not be found - expected " + Adventure.AdventureAuthorDir + " to exist." +
				          "Adventure Author will not function correctly.");
				return false;				
			}
			else {
				if (!Directory.Exists(Adventure.BackupDir)) {
					Say.Error("Backup directory could not be found - expected " + Adventure.BackupDir + " to exist." +
				               "Adventure Author will not function correctly.");
					return false;
				}
				else if (!Directory.Exists(Adventure.LogDir)) {
					Say.Error("Log directory could not be found - expected " + Adventure.LogDir + " to exist." +
				              "Adventure Author will not function correctly.");
					return false;
				}		
				else {
					return true;
				}
			}
		}
	
		private static void CloseToolset()
		{
			throw new NotImplementedException();
		}
	}
}
