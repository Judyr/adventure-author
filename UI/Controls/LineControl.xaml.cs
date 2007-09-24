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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using AdventureAuthor.Core;
using AdventureAuthor.UI.Windows;
using AdventureAuthor.Utils;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.ConversationData;

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
    	
    	private ConditionControl conditionalControl; // conditions are co-dependent, e.g. "IF (x) AND (y) THEN...", so they share a control 	
		public ConditionControl ConditionalControl {
			get { return conditionalControl; }
		}
    	
    	private List<ActionControl> actionControls; // actions are independent, e.g. "HIRO GIVES THE PLAYER 100 GOLD. 
		public List<ActionControl> ActionControls { // HIRO JOINS THE PLAYER'S PARTY", so they each have their own control.
			get { return actionControls; }
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
				SpeakerLabel.Text = speaker.Name.ToUpper();
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
        	
        	// Check if there are conditions for this line to be spoken, and if so represent *all* of them with a single control:
        	if (line.Conditions.Count > 0) {
        		if (!lineIsPartOfBranch) {
        			Say.Error("Conditional check appeared on a line that was not part of a branch.");
        		}
        		conditionalControl = new ConditionControl(this);
        		Grid.SetRow(conditionalControl,0);
        		Grid.SetColumn(conditionalControl,0);
        		Grid.SetColumnSpan(conditionalControl,3);
        		LineControlGrid.Children.Add(conditionalControl);
        	}   
        	else {
        		conditionalControl = null;
        	}        	        	       
        	
        	// Check if there are actions to occur when this line is spoken, and if so represent *each* of them with a control:
        	if (line.Actions.Count > 0) {
        		actionControls = new List<ActionControl>(line.Actions.Count);
        		foreach (NWN2ScriptFunctor action in line.Actions) {
        			ActionControl actionControl = new ActionControl(action,this);
        			actionControls.Add(actionControl);
        			ActionsPanel.Children.Add(actionControl);
        		}
        	}   
        	else {
        		actionControls = new List<ActionControl>();
        	}
        	
        	// Fine-tune context menu based on current circumstances:
        	if (ConversationWriterWindow.Instance.PreviousPage == null) {
        		MenuItem MenuItem_GoBack = (MenuItem)FindName("MenuItem_GoBack");
        		MenuItem_GoBack.IsEnabled = false;
        	}
        	if (!isPartOfBranch) {
        		MenuItem MenuItem_AddCondition = (MenuItem)FindName("MenuItem_AddCondition");        		
        		MenuItem MenuItem_GoToPage = (MenuItem)FindName("MenuItem_GoToPage");
        		MenuItem_AddCondition.IsEnabled = false;
        		MenuItem_AddCondition.Visibility = Visibility.Collapsed;
        		MenuItem_GoToPage.IsEnabled = false;
        		MenuItem_GoToPage.Visibility = Visibility.Collapsed;
        	}
        	else {
        		MenuItem MenuItem_MakeIntoChoice = (MenuItem)FindName("MenuItem_MakeIntoChoice");
        		MenuItem_MakeIntoChoice.IsEnabled = false;
        		MenuItem_MakeIntoChoice.Visibility = Visibility.Collapsed;
        	}        	

        	this.KeyDown += delegate(object sender, System.Windows.Input.KeyEventArgs e) 
			{         	
				if (e.Key == Key.Delete) {
        			OnClick_Delete(sender,e);
		       	}
			};
        	
        	
        	
        	
        	
        	// TEMP: TODO        	
        	if (nwn2Line.Sound != null) {
        		SoundButton.Height = 30;
        		SoundButton.Width = 30;
        	}
        }
                
        #region LineControl event handlers
        
        private void OnClick_GoBack(object sender, EventArgs ea)
        {
        	if (Conversation.CurrentConversation != null && ConversationWriterWindow.Instance.PreviousPage != null) {
        		ConversationWriterWindow.Instance.DisplayPage(ConversationWriterWindow.Instance.PreviousPage);
        	}
        }
        
		private void OnClick_SetCamera(object sender, EventArgs ea)
        {
        	
        }
		
		private void OnClick_SetSound(object sender, EventArgs ea)
        {                	
        	SoundWindow window = new SoundWindow(nwn2Line);
        	window.ShowDialog();
        }		
        
        private void OnClick_GoToPage(object sender, EventArgs ea)
        {
        	// If this line is part of a branch, double-clicking it should display the page it leads to:
        	if (this.isPartOfBranch) {
        		foreach (ConversationPage page in ConversationWriterWindow.Instance.Pages) {
        			if (page.LeadInLine == this.nwn2Line) {
        				ConversationWriterWindow.Instance.DisplayPage(page);
        				return;
        			}
        		}
        	}
        }        
        
		private void OnClick_MakeIntoBranch(object sender, EventArgs ea)
		{
			if (isPartOfBranch) {
				Say.Information("The line you have selected is already part of a choice.");
			}
			else {
				ConversationWriterWindow.Instance.MakeLineIntoBranch(Nwn2Line); //TODO: move the make branch functions to Conversation
			}
		}
		
        private void OnClick_Delete(object sender, EventArgs ea)
        { 	
        	if (!IsVisible) { // in case we have already deleted this line - should have lost focus, but doesn't seem to
        		return;
        	}        		
        	
        	if (isPartOfBranch) {
	       		if (!Adventure.BeQuiet) {   
        			Conversation.DataFromConversation casualties = Conversation.CurrentConversation.GetWordLinePageCounts(nwn2Line);			
        			if (casualties.words == 0 && nwn2Line.Actions.Count == 0) { // if there's no real effect, then just delete the line
        				Conversation.CurrentConversation.DeleteLineFromBranch(nwn2Line);
        			}
        			else { // if there are consequences, remind the user of them and ask them to confirm		
        				string warning = String.Empty;  
		       			if (nwn2Line.Parent == null) {
		        			if (Conversation.CurrentConversation.NwnConv.StartingList.Count == 2) {
		        				warning += "Deleting this line will remove the whole branch, changing the shape of the conversation tree. ";
		        			}
		        		}
		        		else if (nwn2Line.Parent.Line.Children.Count == 2){
		        			warning += "Deleting this line will remove the whole branch, changing the shape of the conversation tree. ";
		        		}   										
        				if (casualties.pages > 1) {
        					warning += "This will also delete " + casualties.pages + " page(s) and " + casualties.words + 
        						" word(s) of conversation.\n\n";
        				}
        				else if (casualties.pages == 1 && casualties.lines > 1) {
        					warning += "This will also delete " + casualties.words + " word(s) of conversation on the next page.\n\n";
        				}        				
        				warning += "Are you sure?";
        				
	 		        	MessageBoxResult result = MessageBox.Show(warning,"Delete?", MessageBoxButton.YesNo);
			        	if (result == MessageBoxResult.Yes) {
			        		Conversation.CurrentConversation.DeleteLineFromBranch(nwn2Line);
			        	}       			
        			}
        		}
        		else {
        			Conversation.CurrentConversation.DeleteLineFromBranch(nwn2Line);
        		}
        	}
        	else {
	        	// Only ask to confirm deletion if the line is not blank, or has an action associated with it:
	        	if (!Adventure.BeQuiet && ((nwn2Line.Text.Strings.Count > 0 && nwn2Line.Text.Strings[0].Value.Length > 0) || nwn2Line.Actions.Count > 0)) {
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
        
       
        
        private void GoUp(object sender, EventArgs ea)
        {
        	if (ConversationWriterWindow.Instance.CurrentPage.ParentPage != null) {
        		ConversationWriterWindow.Instance.DisplayPage(ConversationWriterWindow.Instance.CurrentPage.ParentPage);
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
        
        private void OnMouseDown(object sender, MouseEventArgs ea)
        {        	
			Focus();
        }        
        
        private void OnGotFocus(object sender, EventArgs ea)
        {      	
        	SelectLine();
        	Say.Debug("OnGotFocus called from " + sender.ToString());  
        	
        }
        
        private void OnLostFocus(object sender, EventArgs ea)
        {
        	DeselectLine();
        	Say.Debug("OnLostFocus called from " + sender.ToString());
        }
        
        #endregion
        

        
        #region Selecting lines
        
        private void SelectLine()
        {
        	ConversationWriterWindow.Instance.CurrentControl = this;
        	this.Background = Brushes.Wheat;
        	this.Dialogue.Background = Brushes.White;
        	this.Dialogue.BorderBrush = Brushes.Black;
        	this.SoundButton.IsEnabled = true;
        	this.SoundButton.Opacity = 1.0;
        	this.unusedButton.IsEnabled = true;
        	this.unusedButton.Opacity = 1.0;
        	this.DeleteLineButton.IsEnabled = true;
        	this.DeleteLineButton.Opacity = 1.0;
        	if (this.conditionalControl != null) {
	        	this.conditionalControl.EditConditionsButton.IsEnabled = true;
	        	this.conditionalControl.EditConditionsButton.Opacity = 1.0;    
        	}
        }
        
        private void DeselectLine()
        {
        	ConversationWriterWindow.Instance.CurrentControl = null;
        	this.Background = Brushes.LightYellow;
        	this.Dialogue.Background = Brushes.Transparent;	
        	this.Dialogue.BorderBrush = Brushes.Transparent;
        	this.SoundButton.IsEnabled = false;
        	this.SoundButton.Opacity = 0.0;
        	this.unusedButton.IsEnabled = false;
        	this.unusedButton.Opacity = 0.0;
        	this.DeleteLineButton.IsEnabled = false;
        	this.DeleteLineButton.Opacity = 0.0;
        	if (this.conditionalControl != null) {
	        	this.conditionalControl.EditConditionsButton.IsEnabled = false;
	        	this.conditionalControl.EditConditionsButton.Opacity = 0.0;    
        	} 
        }
        
        #endregion
        
		public override string ToString()
		{
			return "LineControl: " + SpeakerLabel.Text + " - " + UsefulTools.Truncate(Dialogue.Text,60);
		}
    }
}
