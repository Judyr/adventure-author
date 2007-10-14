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
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using AdventureAuthor.Conversations.UI.Graph;
using AdventureAuthor.Core;
using AdventureAuthor.Utils;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.ConversationData;

namespace AdventureAuthor.Conversations.UI.Controls
{
	/// <summary>
	/// A control representing a line of dialogue and its speaker.
	/// <remarks>Can also host controls to represent any actions, conditions or sounds attached to this line of dialogue.</remarks>
	/// </summary>
    public partial class LineControl : UserControl
    {    	
    	#region Fields
    	
    	/// <summary>
    	/// The line of conversation this control represents.
    	/// </summary>
    	private NWN2ConversationConnector nwn2Line;
		public NWN2ConversationConnector Nwn2Line {
			get { return nwn2Line; }
		}
    	
    	/// <summary>
    	/// True if the line this control represents is part of a branch, i.e. either this line or another could be spoken next.
    	/// </summary>
    	private bool isPartOfBranch;    	
		public bool IsPartOfBranch {
			get { return isPartOfBranch; }
			set { isPartOfBranch = value; }
		}    
    	
    	/// <summary>
    	/// A control representing all the conditions on the line (conditions are co-dependent, so they share a control). May be null.
    	/// </summary>
    	private ConditionControl conditionalControl;	
    	
    	/// <summary>
    	/// A control representing a sound file that will be played on this line. May be null.
    	/// </summary>
    	private SoundControl soundControl;
    	
    	/// <summary>
    	/// A list of controls, each representing a single action on the line (actions are independent, so they don't share). May be null.
    	/// </summary>
    	private List<ActionControl> actionControls;	
    	
    	#endregion Fields
    	
    	#region Events
    	
    	
    	
    	#endregion
    	
    	#region Constructor
    	
    	/// <summary>
    	/// Create a new LineControl.
    	/// </summary>
    	/// <param name="line">The line of conversation to represent</param>
    	/// <param name="lineIsPartOfBranch">True if the line is part of a branch, false otherwise</param>
        public LineControl(NWN2ConversationConnector line, bool isPartOfBranch)
        {        
        	this.nwn2Line = line;
        	this.isPartOfBranch = isPartOfBranch;
        	
        	if (this.nwn2Line.Text.Strings.Count == 0) {
        		this.nwn2Line.Text = Conversation.GetOEIStringFromString(String.Empty);
        		// TODO: Could just do this to an entire conversation upon opening it to avoid problems?
        	}
        	
        	this.Resources.Add("LineText",this.nwn2Line.Text.Strings[0]);
        	InitializeComponent();
            
        	// Set the image on the delete button:
            Image image = new Image();
            ImageSourceConverter s = new ImageSourceConverter();
            image.Source = (ImageSource)s.ConvertFromString(Path.Combine(Adventure.ImagesDir,"delete.jpg"));            
            DeleteLineButton.Content = image;            	
        	
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
        	
        	// Check if there are conditions for this line to be spoken, and if so represent *all* of them with a single control:
        	if (line.Conditions.Count > 0) {
        		if (!isPartOfBranch) {
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
        	
        	// Check if there is a sound attached to this line, and if so represent it with a control. 
        	// Note that there is an issue with lines having a non-null Sound property (FullName == ".RES") even when they don't have a sound:        	
        	if (line.Sound != null && line.Sound.FullName != ".RES") { 	
        		soundControl = new SoundControl(this);
        		Grid.SetRow(soundControl,3);
        		Grid.SetColumn(soundControl,0);
        		Grid.SetColumnSpan(soundControl,3);
        		LineControlGrid.Children.Add(soundControl);
        	}
        	
        	// Fine-tune context menu based on current circumstances:
        	if (WriterWindow.Instance.PreviousPage == null) {
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

        	
        	
        	
        	this.Dialogue.LostFocus += new RoutedEventHandler(OnDialogueLostFocus2);

			Dialogue.LostFocus += new RoutedEventHandler(OnDialogueLostFocus);
			Dialogue.GotFocus += new RoutedEventHandler(OnDialogueGotFocus);
			
			
        	this.KeyDown += new KeyEventHandler(OnKeyDown);
			
			
			
//			Dialogue.LostFocus += new RoutedEventHandler(New_DialogueLostFocus);
//			Dialogue.GotFocus += new RoutedEventHandler(New_DialogueGotFocus);
//			this.LostFocus += delegate { DeselectLine(); };
        }
        
        #endregion Constructor
                
        #region Event handlers
        
        /// <summary>
        /// Save changes made to dialogue, if necessary updating node labels on the graph.
        /// </summary>
        private void OnDialogueLostFocus2(object sender, RoutedEventArgs e)
        {
        	SaveChangesToText();
        }
        
        
        /// <summary>
        /// Hit delete to delete this line. Hit return to update the node labels on the graph.
        /// </summary>
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
			if (e.Key == Key.Delete) {
        		OnClick_Delete(sender,e);
		    }
        	if (e.Key == Key.Return) {
        		SaveChangesToText();
        	}
        }
        
        
        /// <summary>
        /// Save changes made to dialogue, so they are not lost when the page view is next refreshed.
        /// </summary>
        internal void SaveChangesToText()
        {
        	if (Conversation.GetStringFromOEIString(nwn2Line.Line.Text) != Dialogue.Text) {
        		Conversation.CurrentConversation.SetText(nwn2Line,Dialogue.Text);
        	}
        }
                
        
        private void OnClick_GoBack(object sender, EventArgs ea)
        {
        	if (Conversation.CurrentConversation != null && WriterWindow.Instance.PreviousPage != null) {
        		WriterWindow.Instance.DisplayPage(WriterWindow.Instance.PreviousPage);
        		WriterWindow.Instance.CentreGraph(false);
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
        		foreach (Page page in WriterWindow.Instance.Pages) {
        			if (page.LeadInLine == this.nwn2Line) {
        				WriterWindow.Instance.DisplayPage(page);
        				WriterWindow.Instance.CentreGraph(false);
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
				Conversation.CurrentConversation.MakeLineIntoChoice(Nwn2Line);
				WriterWindow.Instance.PageScroll.ScrollToBottom();
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
        				Conversation.CurrentConversation.DeleteLineFromChoice(nwn2Line);
        				WriterWindow.Instance.PageScroll.ScrollToBottom();
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
			        		Conversation.CurrentConversation.DeleteLineFromChoice(nwn2Line);
        					WriterWindow.Instance.PageScroll.ScrollToBottom();
			        	}       			
        			}
        		}
        		else {
        			Conversation.CurrentConversation.DeleteLineFromChoice(nwn2Line);
        			WriterWindow.Instance.PageScroll.ScrollToBottom();
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
        	if (WriterWindow.Instance.CurrentPage.Parent != null) {
        		WriterWindow.Instance.DisplayPage(WriterWindow.Instance.CurrentPage.Parent);
        		WriterWindow.Instance.CentreGraph(false);
        	}
        }
        
        #region Drag-drop
        
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
        
        #endregion
        
        private void OnMouseDown(object sender, MouseEventArgs ea)
        {        	
			Focus();
        }        
        
        
        
        
        #endregion
        
        // Tests:
        
        
//        
//		protected override void OnGotFocus(RoutedEventArgs e)
//		{
//			SelectLine();
//			base.OnGotFocus(e);
//		}
//		
////		protected override void OnLostFocus(RoutedEventArgs e)
////		{	
////			base.OnLostFocus(e);
////			if (!DeleteLineButton.IsFocused) {
////				DeselectLine();
////			}
////		}
//		
//		private void New_DialogueGotFocus(object sender, RoutedEventArgs e)
//		{
//			SelectLine();
//		}
//       
//		private void New_DialogueLostFocus(object sender, RoutedEventArgs e)
//		{
//			DeselectLine();
//		}
        
        
        
        
        
        
        
//		protected override void OnGotFocus(RoutedEventArgs e)
//		{
//			Dialogue.Focus();
//		}
//		
//		protected override void OnLostFocus(RoutedEventArgs e)
//		{
//			base.OnLostFocus(e);
//		}
//		
//		private void New_DialogueGotFocus(object sender, RoutedEventArgs e)
//		{
//			SelectLine();
//			e.Handled = true;			
//		}
//       
//		private void New_DialogueLostFocus(object sender, RoutedEventArgs e)
//		{
//			DeselectLine();
//			e.Handled = false;
//		}
        
        
        
        
        
        
        
        
        
        private void OnLineControlGotFocus(object sender, RoutedEventArgs rea)
        {      	        	
        	Log.WriteEffectiveAction(Log.EffectiveAction.selected,"line");
        	Say.Debug("OnLineControlGotFocus(): " + this.ToString());
        	SelectLine();
        }
        
        
        private void OnLineControlLostFocus(object sender, RoutedEventArgs rea)
        {
        	Say.Debug("OnLineControlLostFocus(): " + this.ToString());
        	DeselectLine();
        }
        
        private void OnDialogueGotFocus(object sender, RoutedEventArgs rea)
        {      	        	
        	Say.Debug("OnDialogueGotFocus(): " + this.ToString());
        	SelectLine();
        }
        
        
        private void OnDialogueLostFocus(object sender, RoutedEventArgs rea)
        {
        	Say.Debug("OnDialogueLostFocus(): " + this.ToString());
        	DeselectLine();
        }
        
   
        
        #region Selecting lines
        
        private void SelectLine()
        {
//        	Say.Debug("SelectLine(): " + this.ToString());
        	WriterWindow.Instance.SelectedLineControl = this;
//        	Say.Debug("about to set background");
        	Background = Brushes.Wheat;
        	Dialogue.Background = Brushes.White;
        	Dialogue.BorderBrush = Brushes.Black;
//        	Say.Debug("just set background. about to switch on all the buttons");
        	SwitchOn(DeleteLineButton);
        	if (conditionalControl != null) {
//        		SwitchOn(conditionalControl.EditConditionsButton);
        		SwitchOn(conditionalControl.DeleteConditionsButton);
        	} 
        	foreach (ActionControl actionControl in actionControls) {
//        		SwitchOn(actionControl.EditActionButton);
        		SwitchOn(actionControl.DeleteActionButton);
        	}
        	if (soundControl != null) {
        		SwitchOn(soundControl.PlaySoundButton);
        	}
//        	Say.Debug("Switched on all the buttons.");
        }
        
        
        private void DeselectLine()
        {
//        	Say.Debug("DeselectLine(): " + this.ToString());
        	if (isPartOfBranch) {
        		Background = Brushes.AliceBlue;
        	}
        	else {
        		Background = Brushes.LightYellow;
        	}
        	Dialogue.Background = Brushes.Transparent;	
        	Dialogue.BorderBrush = Brushes.Transparent;
        	SwitchOff(DeleteLineButton);
        	if (conditionalControl != null) {
//        		SwitchOff(conditionalControl.EditConditionsButton);
        		SwitchOff(conditionalControl.DeleteConditionsButton);
        	} 
        	foreach (ActionControl actionControl in actionControls) {
//        		SwitchOff(actionControl.EditActionButton);
        		SwitchOff(actionControl.DeleteActionButton);
        	}
        	if (soundControl != null) {
        		SwitchOff(soundControl.PlaySoundButton);
        	}
        }
        
        
        private void SwitchOn(Control c)
        {
        	c.IsEnabled = true;
        	c.Opacity = 1.0;
        }
        
        
        private void SwitchOff(Control c)
        {
        	c.IsEnabled = false;
        	c.Opacity = 0.0;
        }
        
        #endregion
        
		public override string ToString()
		{
			return "LineControl: " + SpeakerLabel.Text + " - " + UsefulTools.Truncate(Dialogue.Text,60) + "...";
		}
    }
}
