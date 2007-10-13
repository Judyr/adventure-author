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
using System.Windows;
using NWN2Toolset.NWN2.Data.ConversationData;
using AdventureAuthor.Core;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Conversations.UI
{
    /// <summary>
    /// Interaction logic for ChooseSpeaker.xaml
    /// </summary>

    public partial class ChooseSpeaker : Window
    {
        public ChooseSpeaker()
        {
        	this.Resources.Add("speakers",Conversation.CurrentConversation.Speakers);
            InitializeComponent();
        }

        private void OnClickOK(object sender, EventArgs ea)
        {
        	if (SpeakersList.SelectedItem == null) {
        		Say.Information("Select a speaker.");
        	}
        	else {
        		string speakerTag = ((Speaker)SpeakersList.SelectedItem).Tag;   
				Conversation.CurrentConversation.AddChoice(speakerTag);
				WriterWindow.Instance.PageScroll.ScrollToBottom();
				this.Close();
        	}
        }

        private void OnClickCancel(object sender, EventArgs ea)
        {
        	this.Close();
        }
    }
}
