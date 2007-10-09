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
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using NWN2Toolset.NWN2.Data.ConversationData;
using OEIShared.IO;
using OEIShared.Utils;
using AdventureAuthor.Core;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Conversations.UI
{
    /// <summary>
    /// Interaction logic for SoundWindow.xaml
    /// </summary>

    public partial class SoundWindow : Window
    {
    	private NWN2ConversationConnector line;
    	
        public SoundWindow(NWN2ConversationConnector line)
        {
            IResourceRepository moduleRepos = ResourceManager.Instance.GetRepositoryByName(Adventure.CurrentAdventure.ModulePath);
            if (moduleRepos == null) {
            	Say.Error("Couldn't locate the module as a resource repository.");
            	return;
            }        	
            
        	this.line = line;
        	
            InitializeComponent();
       
			ushort WAV = 4;
			OEIGenericCollectionWithEvents<IResourceEntry> WAVfiles = moduleRepos.FindResourcesByType(WAV);
			MissingResourceEntry blank = new MissingResourceEntry(new OEIResRef("<no sound>"));
			WAVfiles.Insert(0,blank);
			
			SoundBox.ItemsSource = WAVfiles;
			    	
			// Note that there is an issue with lines having a non-null Sound property (FullName == ".RES") even when they don't have a sound:
			if (line.Sound != null && line.Sound.FullName != ".RES") { // 	
				CurrentSoundTextBox.Text = "Current sound: " + line.Sound.FullName;
			}
			else {
				CurrentSoundTextBox.Text = "No sound currently selected";
			}
        }
        
        private void OnClick_OK(object sender, EventArgs ea)
        {
        	IResourceEntry wav = SoundBox.SelectedItem as IResourceEntry;
        	if (wav == null) {
        		Say.Warning("You didn't select a sound file to play.");
        		return;
        	}
        	
        	if (wav.ResRef.Value == "<no sound>") {
        		line.Sound = null;
        	}
        	else {
        		line.Sound = wav;
        	}
	    	WriterWindow.Instance.RefreshPageViewOnly();
        	Close();
        }
        
        private void OnClick_Cancel(object sender, EventArgs ea)
        {
        	Close();
        }
    }
}
