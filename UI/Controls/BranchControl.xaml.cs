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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NWN2Toolset.NWN2.Data.ConversationData;
using AdventureAuthor.Core;
using AdventureAuthor.UI.Windows;
using AdventureAuthor.Utils;

namespace AdventureAuthor.UI.Controls
{
    /// <summary>
    /// Interaction logic for BranchControl.xaml
    /// </summary>

    public partial class BranchControl : UserControl
    {    	
    	public enum BranchControlType { Choice, Check }
    	
    	private BranchControlType branchType;
		public BranchControlType BranchType {
			get { return branchType; }
			set { branchType = value; }
		}
    	
     	private List<LineControl> lineControls;    	
		public List<LineControl> LineControls {
			get { return lineControls; }
			set { lineControls = value; }
		}
        
        public BranchControl(NWN2ConversationConnectorCollection lines)
        {
        	InitializeComponent();
            this.lineControls = new List<LineControl>(2);
        	
        	if (lines == null) {
        		throw new ArgumentNullException("lines","Tried to form a branch with a null collection of lines.");
        	}
        	else if (lines.Count < 2) {
        		throw new ArgumentException("Tried to form a branch with less than 2 lines.");
        	}
        	else {
        		if (lines[0].Type == NWN2ConversationConnectorType.Reply) {
        			this.branchType = BranchControlType.Choice;
        		}
        		else {
        			this.branchType = BranchControlType.Check;
        		}
        	}
        
        	foreach (NWN2ConversationConnector line in lines) {
            	if (Conversation.IsFiller(line)) {
            		throw new ArgumentException("Tried to form a branch with a filler line.");
            	}
            	
				LineControl lineControl = new LineControl(line,true);				
				lineControl.Dialogue.FontStyle = FontStyles.Italic;
				lineControl.SpeakerLabel.FontStyle = FontStyles.Italic;
       			lineControl.Dialogue.Foreground = Brushes.Black;
               			
       			// Set appearance of control based on whether this is a choice (PC) or a check (NPC):
       			if (line.Type == NWN2ConversationConnectorType.Reply) {
       				if (branchType == BranchControlType.Choice) { // PC chooses what to say
						lineControl.SpeakerLabel.Text = Conversation.PLAYER_NAME.ToUpper();
						lineControl.SpeakerLabel.Foreground = Conversation.BRANCH_COLOUR;
       				}
       				else {
       					throw new ArgumentException("Tried to form a Check using an PC line - Checks should only contain NPC lines.");
       				}
       			}
       			else {
       				if (branchType == BranchControlType.Choice) {
       					throw new ArgumentException("Tried to form a Choice using an NPC line - Choices should only contain PC lines.");
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
        
        public void OnClick_AddOption(object sender, EventArgs ea)
        {
        	NWN2ConversationConnector parentLine = this.lineControls[0].Nwn2Line.Parent;
        	NWN2ConversationConnector newLine = Conversation.CurrentConversation.AddLineToBranch(parentLine);
				
			ConversationWriterWindow.Instance.RefreshDisplay(true);
					
			if (newLine != null) {
				foreach (LineControl lc in OptionsPanel.Children) {
					Say.Debug("Checking a LineControl for the line to focus on.");
					if (lc.Nwn2Line == newLine) {
						Say.Debug("Found the line to focus on.");
						lc.Focus(); // TODO also doesn't work
					}
				}
			}
        }
    }
}
