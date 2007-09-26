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
using System.Collections.Generic;
using System.IO;
using System.Windows;
using AdventureAuthor.Core;
using AdventureAuthor.Utils;
using form = NWN2Toolset.NWN2ToolsetMainForm;

namespace AdventureAuthor.Core.UI
{
    /// <summary>
    /// Interaction logic for OpenAdventureWindow.xaml
    /// </summary>

    public partial class OpenAdventureWindow : Window
    {
        public OpenAdventureWindow()
        {
        	try {
	        	string[] adventureData = Directory.GetFiles(Adventure.SerializedDir,"*.xml",SearchOption.TopDirectoryOnly);
	        	string[] moduleData = Directory.GetDirectories(form.ModulesDirectory,"*", SearchOption.TopDirectoryOnly);
				List<string> adventures = new List<string>(adventureData.Length);				
	        	foreach (string s in adventureData) {
	        		string name = Path.GetFileNameWithoutExtension(s);
	        		foreach (string p in moduleData) {
	        			if (name == Path.GetFileNameWithoutExtension(p)) {
	        				adventures.Add(name);
	        				break;
	        			}
	        		}
	        	}
				this.Resources.Add("adventurenames",adventures);			
	            InitializeComponent();
        	}
        	catch (DirectoryNotFoundException e) {
        		Say.Error("Could not find one of the directories needed to store Adventure/module data.",e);
        	}
        }

        private void OnClickCancel(object sender, EventArgs ea)
        {
        	this.Close();
        }
        
        private void OnClickOK(object sender, EventArgs ea)
        {
        	if (adventuresList.SelectedItem == null) {
        		Say.Warning("Select an adventure, and then click OK.");
        	}
        	else if (Adventure.Open((string)adventuresList.SelectedItem) == null) {
        		Say.Error("Could not find a well-formed Adventure with the name '" + adventuresList.SelectedItem + "'.");
        	}
        	else {
        		this.Close();
        	}
        }
    }
}
