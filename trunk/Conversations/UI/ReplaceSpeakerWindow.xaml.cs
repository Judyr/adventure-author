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
using System.Windows;
using System.Collections.Generic;
using NWN2Toolset.NWN2.Data.ConversationData;
using AdventureAuthor.Core;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Conversations.UI
{
    public partial class ReplaceSpeakerWindow : Window
    {
        public ReplaceSpeakerWindow()
        {
        	List<Speaker> speakers = new List<Speaker>(Conversation.CurrentConversation.Speakers.Count - 1);
        	foreach (Speaker speaker in Conversation.CurrentConversation.Speakers) {
        		if (!speaker.IsPlayer()) {
        			speakers.Add(speaker);
        		}
        	}
        	this.Resources.Add("speakers",speakers);
            InitializeComponent();
        }
        

        private void OnClickOK(object sender, EventArgs ea)
        {
        	if (Conversation.CurrentConversation == null) {
        		throw new InvalidOperationException("There was no conversation open.");
        	}
        	
        	Speaker speaker = (Speaker)SpeakersList.SelectedItem;
        	string newTag = newTagTextBox.Text;
        	
        	if (SpeakersList.SelectedItem == null) {
        		Say.Information("Select a speaker to replace.");
        	}
        	else if (newTag == String.Empty) {
        		Say.Information("Enter a new tag for the speaker.");
        	}
        	else if (newTag.ToLower() == Conversation.PLAYER_NAME.ToLower()) {
        		Say.Information("It is not possible to replace a speaker with the player.");
        	}
        	else if (newTag.ToLower() == speaker.Tag) {
        		Say.Information(newTag + " is already the name of this speaker.");
        	}
        	else if (Conversation.CurrentConversation.GetSpeaker(newTag) != null) {
        		Say.Information(newTag + " is already a speaker in this conversation.");
        	}
        	else {
//        		MessageBoxResult result = MessageBox.Show("This speaker may also appear in actions and conditions " +
//							        		                "in this conversation. Do you want to replace the speaker " +
//							        		                "in these actions/conditions as well?",
//							        		                "Replace speaker in actions/conditions?",
//							        		                MessageBoxButton.YesNoCancel,
//							        		                MessageBoxImage.Question,
//							        		                MessageBoxResult.Yes,
//							        		                MessageBoxOptions.None);
//        		if (result != MessageBoxResult.Cancel) {				
					// Rename the speaker:
					bool includingScripts = true;// bool includingScripts = result == MessageBoxResult.Yes;
					Conversation.CurrentConversation.RenameSpeaker(speaker,newTag,includingScripts);
							
					// Rename the button representing the speaker:
					foreach (UIElement element in WriterWindow.Instance.speakersButtonsPanel.Children) {
						if (element is SpeakerButton) {
							SpeakerButton button = (SpeakerButton)element;
							if (button.Speaker == speaker) {
								button.UpdateSpeakerName();
							}
						}
					}
					
        			Close();
//        		}
        	}
        }

        
        private void OnClickCancel(object sender, EventArgs ea)
        {
        	Close();
        }
    }
}
