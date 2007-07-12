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
 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using NWN2Toolset.NWN2.Data.ConversationData;
using AdventureAuthor.ConversationWriter;

namespace AdventureAuthor.UI.Windows
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
        		NWN2ConversationConnector parent;
        		if (ConversationWriterWindow.Instance.CurrentPage.LineControls.Count > 0) {
        			parent = ConversationWriterWindow.Instance.CurrentPage.LineControls[ConversationWriterWindow.Instance.CurrentPage.LineControls.Count-1].Nwn2Line;
				}
				else {
        			parent = ConversationWriterWindow.Instance.CurrentPage.LeadInLine; // LeadInline may be null i.e. root
				}	
        		string speakerTag = ((Speaker)SpeakersList.SelectedItem).Tag;   
				ConversationWriterWindow.Instance.MakeBranchAtEndOfPage(parent,speakerTag);
				this.Close();
        	}
        }

        private void OnClickCancel(object sender, EventArgs ea)
        {
        	this.Close();
        }
    }
}
