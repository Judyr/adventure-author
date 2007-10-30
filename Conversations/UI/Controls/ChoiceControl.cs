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
using System.Windows.Media;
using AdventureAuthor.Core;
using AdventureAuthor.Utils;
using NWN2Toolset.NWN2.Data.ConversationData;

namespace AdventureAuthor.Conversations.UI.Controls
{
    /// <summary>
    /// Interaction logic for BranchControl.xaml
    /// </summary>

    public partial class ChoiceControl : UserControl
    {    	
    	public enum ChoiceType { Player, NPC }
    	
    	private ChoiceType typeOfSpeaker;
		public ChoiceType TypeOfSpeaker {
			get { return typeOfSpeaker; }
			set { typeOfSpeaker = value; }
		}
    	
     	private List<LineControl> lineControls;    	
		public List<LineControl> LineControls {
			get { return lineControls; }
			set { lineControls = value; }
		}
        
        public ChoiceControl(NWN2ConversationConnectorCollection lines)
        {
        	InitializeComponent();
            this.lineControls = new List<LineControl>(2);
        	
        	if (lines == null) {
        		throw new ArgumentNullException("Tried to form a choice with a null collection of lines.");
        	}
        	else if (lines.Count < 2) {
        		throw new ArgumentException("Tried to form a choice with less than 2 lines.");
        	}
        	else {
        		if (lines[0].Type == NWN2ConversationConnectorType.Reply) {
        			this.typeOfSpeaker = ChoiceType.Player;
        		}
        		else {
        			this.typeOfSpeaker = ChoiceType.NPC;
        		}
        	}
        
        	foreach (NWN2ConversationConnector line in lines) {
            	if (Conversation.IsFiller(line)) {
            		throw new ArgumentException("Tried to form a choice with a filler line.");
            	}
            	
            	BranchLine lineControl = new BranchLine(line);
				lineControl.Dialogue.FontStyle = FontStyles.Italic;
				lineControl.SpeakerLabel.FontStyle = FontStyles.Italic;
       			lineControl.Dialogue.Foreground = Brushes.Black;
               			
       			// Set appearance of control based on whether this is a choice (PC) or a check (NPC):
       			if (line.Type == NWN2ConversationConnectorType.Reply) {
       				if (typeOfSpeaker == ChoiceType.Player) { // PC chooses what to say
						lineControl.SpeakerLabel.Text = Conversation.PLAYER_NAME.ToUpper();
						lineControl.SpeakerLabel.Foreground = Conversation.BRANCH_COLOUR;
       				}
       				else {
       					throw new ArgumentException("Tried to form a choice with lines from more than one speaker.");
       				}
       			}
       			else {
       				if (typeOfSpeaker == ChoiceType.Player) {
       					throw new ArgumentException("Tried to form a choice with lines from more than one speaker.");
       				}
       				else { // NPC checks what to say based on conditions
	  					Speaker speaker = Conversation.CurrentConversation.GetSpeaker(line.Speaker);
						if (speaker == null) {
							lineControl.SpeakerLabel.Text = "???";
						}
						else {
							lineControl.SpeakerLabel.Text = speaker.Name.ToUpper();
						}
						lineControl.SpeakerLabel.Foreground = Conversation.BRANCH_COLOUR;     					
       				}
       			}       			
      
        		this.OptionsPanel.Children.Add(lineControl);
        		this.lineControls.Add(lineControl); 
        	}
        }	
        
        
        public void OnClick_AddBranch(object sender, EventArgs ea)
        {
        	NWN2ConversationConnector parentLine = this.lineControls[0].Nwn2Line.Parent;
        	NWN2ConversationConnector newLine = Conversation.CurrentConversation.AddLineToChoice(parentLine);
				
        	WriterWindow.Instance.FocusOn(newLine);
        }	
        
        
        public void OnClick_DeleteEntireChoice(object sender, EventArgs ea)
        {        	
        	NWN2ConversationConnector parentLine = this.lineControls[0].Nwn2Line.Parent;
        	NWN2ConversationConnectorCollection children;
        	if (parentLine == null) {
        		children = Conversation.CurrentConversation.NwnConv.StartingList;
        	}
        	else {
        		children = parentLine.Line.Children;
        	}
        	
        	if (!Adventure.BeQuiet) { 	
        		Conversation.DataFromConversation casualties = Conversation.CurrentConversation.GetWordLinePageCounts(children);
        		
        		StringBuilder warning = new StringBuilder();
        		
        		if (children.Count == 2) {
        			warning.Append("Are you sure you want to delete both branches of this choice");
        		}
        		else {
        			warning.Append("Are you sure you want to delete all " + children.Count + " branches of this choice");
        		}
        		
        		if (casualties.words > 0) {
        			warning.Append(", including " + casualties.words + " word(s) of dialogue?");
        		}
        		else {
        			warning.Append("?");
        		}
        		
        		if (!(bool)Say.Question(warning.ToString(),"Delete?",System.Windows.Forms.MessageBoxButtons.YesNo)) {
        			return;
        		}
        	}
        	
        	Conversation.CurrentConversation.DeleteEntireChoice(parentLine);
        }
        
        
        private void OnDrop(object sender, DragEventArgs e)
        {
        	Say.Debug("Dropped on " + this.typeOfSpeaker.ToString() + " choice control.");
        }
    }
}
