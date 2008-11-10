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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using AdventureAuthor;
using AdventureAuthor.Core;
using AdventureAuthor.Utils;
using AdventureAuthor.Scripts;

namespace AdventureAuthor.Conversations.UI
{
    /// <summary>
    /// Interaction logic for AddSpeakerWindow.xaml
    /// </summary>

    public partial class AddSpeakerWindow : Window
    {
        public AddSpeakerWindow()
        {
            InitializeComponent();
            
            // Populate list of possible speakers with all creatures in module except for those already in the conversation:
            SortedList<string,string> allSpeakers = ScriptHelper.GetTags(TaggedType.Creature);
            foreach (Speaker speaker in Conversation.CurrentConversation.Speakers) {
            	if (allSpeakers.ContainsKey(speaker.Tag)) {
            		allSpeakers.Remove(speaker.Tag);
            	}
            }
            
            AnswerBox.ItemsSource = allSpeakers.Keys;
        }

        private void OnClick_AddSpeaker(object sender, EventArgs ea)
        {
        	if (AnswerBox.SelectedItem == null) {
        		Say.Error("You didn't select a speaker.");
        		return;
        	}
        	else if (Conversation.CurrentConversation.GetSpeaker((string)AnswerBox.SelectedItem) != null) {
        	    Say.Error(AnswerBox.SelectedValue + " is already a part of this conversation."); 
        	    return; 	          
        	}
        	else {
         		Conversation.CurrentConversation.AddSpeaker((string)AnswerBox.SelectedItem);	
        		Close(); 
        	}
        	
        	// TODO let them add [OWNER]
        }
        
        private void OnClick_Cancel(object sender, EventArgs ea)
        {
        	Close();
        }
    }
}
