﻿/*
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
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Media;
using AdventureAuthor;
using AdventureAuthor.Core;
using NWN2Toolset.NWN2.Data.ConversationData;
using AdventureAuthor.UI.Windows;
using AdventureAuthor.Utils;

namespace AdventureAuthor.UI.Controls
{
    /// <summary>
    /// Interaction logic for LineControl.xaml
    /// </summary>

    public partial class LineControl : UserControl
    {    	
    	private NWN2ConversationConnector nwn2Line;
		public NWN2ConversationConnector Nwn2Line {
			get { return nwn2Line; }
		}
    	
    	private bool isPartOfBranch;    	
		public bool IsPartOfBranch {
			get { return isPartOfBranch; }
			set { isPartOfBranch = value; }
		}    
    	
    	private ActionControl textAppearsWhenControl;    	
		public ActionControl TextAppearsWhenControl {
			get { return textAppearsWhenControl; }
		}
    	
    	private const int MAX_LENGTH_OF_LINE_TO_DISPLAY = 70;
				
        public LineControl(NWN2ConversationConnector line, bool lineIsPartOfBranch)
        {        
        	this.nwn2Line = line;
        	this.isPartOfBranch = lineIsPartOfBranch;
        	
        	if (this.nwn2Line.Text.Strings.Count == 0) {
        		this.nwn2Line.Text = Conversation.StringToOEIExoLocString(String.Empty);
        		// TODO: Could just do this to an entire conversation upon opening it to avoid problems?
        	}
        	
        	this.Resources.Add("LineText",this.nwn2Line.Text.Strings[0]);
        	InitializeComponent();               	
        	
        	// Set the appearance of the control based on who is speaking:
			Speaker speaker = Conversation.CurrentConversation.GetSpeaker(nwn2Line.Speaker);
			if (speaker != null) { 
				SpeakerLabel.Text = speaker.DisplayName.ToUpper();
				SpeakerLabel.Foreground = speaker.Colour;
			}
			else { // if we can't identify the speaker - but this should never happen
				SpeakerLabel.Text = "???";
				SpeakerLabel.Foreground = Brushes.Black;
			}

        	// Save the changes made to dialogue:
        	Dialogue.LostFocus += delegate
        	{
        		if (Conversation.CurrentConversation != null) {
	        		this.nwn2Line.Line.Text.Strings[0].Value = Dialogue.Text;
	        		Conversation.CurrentConversation.SaveToWorkingCopy();
	        		// Note that the better way to do this would be for the Binding from Dialogue.Text to "LineText"
	        		// to be OneWayToSource or TwoWay (although no reason why the source itself would change, but better to
	        		// be safe) and remove this delegate - however for some reason the binding doesn't seem to take.
	        		// If have time do it properly, but this works for now.
        		}
        	};            
        	
        	// If there are conditions on this line appearing, represent them with an ActionControl:
        	if (line.Conditions.Count > 0) {
        		if (!lineIsPartOfBranch) {
        			Say.Error("Conditional check appeared on a line that was not part of a branch.");
        		}
        		ActionControl actionControl = new ActionControl(this);
        		this.textAppearsWhenControl = actionControl;
        		this.LineControlGrid.Children.Add(actionControl);
        		Grid.SetRow(this.textAppearsWhenControl,0);
        		Grid.SetColumn(this.textAppearsWhenControl,0);
        		Grid.SetColumnSpan(this.textAppearsWhenControl,3);
        	}   
        	else {
        		this.textAppearsWhenControl = null;
        	}
        }
                
        private void OnMouseDown(object sender, EventArgs ea)
        {
        	Focus();        	
        }
        
        private void OnGotFocus(object sender, EventArgs ea)
        {
        	ConversationWriterWindow.Instance.CurrentlySelectedControl = this;
        	SelectLine();
        }
        
        private void OnLostFocus(object sender, EventArgs ea)
        {
        	DeselectLine();
        }
        
        private void OnMouseDoubleClick(object sender, EventArgs ea)
        {
        	// If this line is part of a branch, double-clicking it should display the page it leads to:
        	if (this.isPartOfBranch) {
        		foreach (ConversationPage p in ConversationWriterWindow.Instance.Pages) {
        			if (p.LeadInLine == this.nwn2Line) {
        				ConversationWriterWindow.Instance.DisplayPage(p);
        				return;
        			}
        		}
        	}
        }
        
        private void SelectLine()
        {
        	this.Dialogue.Background = Brushes.White;
        	this.Dialogue.BorderBrush = Brushes.Black;
        	this.SoundButton.IsEnabled = true;
        	this.SoundButton.Opacity = 1.0;
        	this.AnimationButton.IsEnabled = true;
        	this.AnimationButton.Opacity = 1.0;
        	this.DeleteLineButton.IsEnabled = true;
        	this.DeleteLineButton.Opacity = 1.0;
        	if (this.textAppearsWhenControl != null) {
	        	this.textAppearsWhenControl.EditActionButton.IsEnabled = true;
	        	this.textAppearsWhenControl.EditActionButton.Opacity = 1.0;    
        	}
        }
        
        private void DeselectLine()
        {
        	this.Dialogue.Background = Brushes.Transparent;	
        	this.Dialogue.BorderBrush = Brushes.Transparent;
        	this.SoundButton.IsEnabled = false;
        	this.SoundButton.Opacity = 0.0;
        	this.AnimationButton.IsEnabled = false;
        	this.AnimationButton.Opacity = 0.0;
        	this.DeleteLineButton.IsEnabled = false;
        	this.DeleteLineButton.Opacity = 0.0;
        	if (this.textAppearsWhenControl != null) {
	        	this.textAppearsWhenControl.EditActionButton.IsEnabled = false;
	        	this.textAppearsWhenControl.EditActionButton.Opacity = 0.0;    
        	} 
        }
        
        private void OnSoundClick(object sender, EventArgs ea)
        {        
        	SoundWindow window = new SoundWindow();
        	window.ShowDialog();
        }
        
        private void OnAnimationClick(object sender, EventArgs ea)
        {
        	AnimationWindow window = new AnimationWindow();
        	window.ShowDialog();
        }
        
        private void OnDeleteClick(object sender, EventArgs ea)
        { 	
        	if (isPartOfBranch) {
        		if (!Adventure.BeQuiet) {   
        			// Check what effects this deletion will have on the conversation:
        			Conversation.DataFromConversation casualties = Conversation.GetWordLinePageCounts(Conversation.CurrentConversation,this.nwn2Line);
        			string onlyRemainingLineInBranch = null; // if there is only one possible line remaining, then the branch will be replaced by that line
        			if (this.nwn2Line.Parent == null) {
        				if (Conversation.CurrentConversation.NwnConv.StartingList.Count == 2) {
        					if (Conversation.CurrentConversation.NwnConv.StartingList[0] == this.nwn2Line) {
        						onlyRemainingLineInBranch = Conversation.OEIExoLocStringToString(Conversation.CurrentConversation.NwnConv.StartingList[1].Text);
        					}
        					else {
        						onlyRemainingLineInBranch = Conversation.OEIExoLocStringToString(Conversation.CurrentConversation.NwnConv.StartingList[0].Text);
        					}
        				}
        			}
        			else if (this.nwn2Line.Parent.Line.Children.Count == 2) {
        				if (this.nwn2Line.Parent.Line.Children[0] == this.nwn2Line) {
        					onlyRemainingLineInBranch = Conversation.OEIExoLocStringToString(this.nwn2Line.Parent.Line.Children[1].Text);
        				}
        				else {
        					onlyRemainingLineInBranch = Conversation.OEIExoLocStringToString(this.nwn2Line.Parent.Line.Children[0].Text);
        				}
        			}        			    
        			
        			// Inform the user and ask them to confirm their choice:
        			if (onlyRemainingLineInBranch == null && casualties.words == 0) {
        				Conversation.CurrentConversation.DeleteLineFromBranch(this.nwn2Line); // if there's no real effect, then just delete the line
        			}
        			else {       
        				string warning = String.Empty; 
        				string shortLine = String.Empty;
        				if (onlyRemainingLineInBranch != null) {        					
        					if (onlyRemainingLineInBranch.Length < MAX_LENGTH_OF_LINE_TO_DISPLAY) {
        						warning += "This branch will be replaced by the only other possible choice, which is the line '" + onlyRemainingLineInBranch + "'\n\n";
        					}
        					else {
        						shortLine = onlyRemainingLineInBranch.Substring(0,MAX_LENGTH_OF_LINE_TO_DISPLAY);
        						// TODO: Make it display up to the end of a word. Tried this before and got weird errors using .LastIndexOf, everything was
        						// out of range, not sure why so just left it.
        						
//        						int maxlength = Math.Min(onlyRemainingLineInBranch.Length,MAX_LENGTH_OF_LINE_TO_DISPLAY);
//        						MessageBox.Show("Other line length is " + onlyRemainingLineInBranch.Length.ToString() + " and max length is " + MAX_LENGTH_OF_LINE_TO_DISPLAY + 
//        						                " so use the smallest, which is " + maxlength + ".");
//        						MessageBox.Show(onlyRemainingLineInBranch);
//        						
//        						try {
//        								int lastSpace = onlyRemainingLineInBranch.LastIndexOf('i',0);
//        								MessageBox.Show(lastSpace.ToString());
////	        						if (lastSpace != -1 && lastSpace < MAX_LENGTH_OF_LINE_TO_DISPLAY && lastSpace > MAX_LENGTH_OF_LINE_TO_DISPLAY - 15) { // if there's a nice cut-off point for the sentence
////	        							shortLine = onlyRemainingLineInBranch.Substring(0,lastSpace);
////	        						}
////	        						else {
////	        							shortLine = onlyRemainingLineInBranch.Substring(0,maxlength);
////	        						}
//        						}
//        						catch (ArgumentOutOfRangeException are) {
//        							Say.Error("Died " + are.ToString());
//        						}
        						
        						warning += "This branch will be replaced by the only other possible choice, which is the line beginning '" + shortLine + "...'\n\n";
        					}
        				}
        				if (casualties.pages > 1) {
        					warning += "Deleting this line will also delete the " + casualties.pages + " pages it can lead to. ";
        				}
        				warning += "A total of " + casualties.lines + " lines and " + casualties.words + " words will be deleted. " +
        						   "Are you sure you want to delete this line?";
        				
	 		        	MessageBoxResult result = MessageBox.Show(warning,"Delete?", MessageBoxButton.YesNo);
			        	if (result == MessageBoxResult.Yes) {
			        		Conversation.CurrentConversation.DeleteLineFromBranch(this.nwn2Line);
			        	}       			
        			}
        		}
        		else {
        			Conversation.CurrentConversation.DeleteLineFromBranch(this.nwn2Line);
        		}
        	}
        	else {
	        	// Only ask to confirm deletion if the line is not blank:
	        	if (!Adventure.BeQuiet && this.nwn2Line.Text.Strings.Count > 0 && this.nwn2Line.Text.Strings[0].Value.Length > 0) {
		        	MessageBoxResult result = MessageBox.Show("Delete?","Are you sure?", MessageBoxButton.YesNo);
		        	if (result == MessageBoxResult.Yes) {
		        		Conversation.CurrentConversation.DeleteLine(this.nwn2Line);
		        	}
	        	}
	        	else { 
	        		Conversation.CurrentConversation.DeleteLine(this.nwn2Line);
	        	}        		
        	}
        }
        
        /// <summary>
        /// Called when a drag-drop operation is started.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ea"></param>
        private void OnDragStarted(object sender, EventArgs ea)
        {
        	//this.Background = Brushes.Pink;
        }
        
        private void OnDragCompleted(object sender, EventArgs ea) 
        {
        	//this.Background = Brushes.LightYellow;
        }
        
        private void OnDragDelta(object sender, EventArgs ea)
        {
//        	if (DateTime.Now.Second % 2 == 0) {
//        		this.Background = Brushes.Red;
//        	}
//        	else {
//        		this.Background = Brushes.Blue;
//        	}
        }
    }
}
