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
using System.IO;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using AdventureAuthor.Conversations.UI.Graph;
using AdventureAuthor.Core;
using AdventureAuthor.Setup;
using AdventureAuthor.Scripts;
using AdventureAuthor.Utils;
using NWN2Toolset;
using NWN2Toolset.Plugins;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.ConversationData;

namespace AdventureAuthor.Conversations.UI.Controls
{
	/// <summary>
	/// A control representing a line of dialogue and its speaker. It also hosts controls to represent any 
	/// actions, conditions or sounds attached to this line of dialogue.</remarks>
	/// </summary>
    public abstract partial class LineControl : UserControl
    {    	
    	#region Constants
    	
    	protected const string AddScriptHeader = "Add script";
    	protected const string EditScriptHeader = "Edit script";
    	
    	#endregion
    	
    	#region Fields
    	
    	/// <summary>
    	/// The line of conversation this control represents.
    	/// </summary>
    	protected NWN2ConversationConnector nwn2Line;
		public NWN2ConversationConnector Nwn2Line {
			get { return nwn2Line; }
		}
    	
    	
    	/// <summary>
    	/// A control representing all the conditions on the line (conditions are co-dependent, so they share a control). May be null.
    	/// </summary>
    	protected ConditionControl conditionalControl = null;
    	
    	
    	/// <summary>
    	/// A control representing a sound file that will be played on this line. May be null.
    	/// </summary>
    	protected SoundControl soundControl = null;
    	
    	
    	/// <summary>
    	/// A list of controls, each representing a single action on the line (actions are independent, so they don't share). May be null.
    	/// </summary>
    	protected List<ActionControl> actionControls = null;
    	
    	#endregion Fields
    	
    	#region Events
    	        
        public event EventHandler<WordCountEventArgs> WordsTyped;
		protected virtual void OnWordsTyped(WordCountEventArgs e)
		{
			EventHandler<WordCountEventArgs> handler = WordsTyped;
			if (handler != null) {
				handler(this,e);
			}
		}
		
    	#endregion
    	
    	#region Constructor
    	    	
    	/// <summary>
    	/// Create a new LineControl.
    	/// </summary>
    	/// <param name="line">The line of conversation to represent</param>
    	/// <param name="lineIsPartOfBranch">True if the line is part of a branch, false otherwise</param>
        protected LineControl(NWN2ConversationConnector line)
        {      
        	this.nwn2Line = line;
        	if (this.nwn2Line.Text.Strings.Count == 0) {
        		this.nwn2Line.Text = Conversation.GetOEIStringFromString(String.Empty);
        	}
        	
        	this.Resources.Add("LineText",this.nwn2Line.Text.Strings[0]);
        	InitializeComponent();
        	this.DataContext = this;
        	        		
        	// Check if there are conditions for this line to be spoken, 
        	// and if so represent *all* of them with a single control.
        	// (Note that this should only be the case on BranchLine
        	// or LeadingLine instances, not on Line instances.)
        	if (line.Conditions.Count > 0) {
        		conditionalControl = new ConditionControl(this);
        		Grid.SetRow(conditionalControl,0);
        		Grid.SetColumn(conditionalControl,0);
        		Grid.SetColumnSpan(conditionalControl,4);
        		LineControlGrid.Children.Add(conditionalControl);
        	}   
        	
        	// Set the appearance of the control based on who is speaking:
			Speaker speaker = Conversation.CurrentConversation.GetSpeaker(nwn2Line.Speaker);
			if (speaker != null) { 
				SpeakerLabel.Text = speaker.Name.ToUpper();
			}
			else { // if we can't identify the speaker - but this should never happen
				SpeakerLabel.Text = "???";
			}
      	        	       
        	
        	// Check if there are actions to occur when this line is spoken, 
        	// and if so represent *each* of them with a control:
        	if (line.Actions.Count > 0) {
        		bool lineHasSeveralActions = line.Actions.Count > 1;
        		actionControls = new List<ActionControl>(line.Actions.Count);
        		foreach (NWN2ScriptFunctor action in line.Actions) {
        			ActionControl actionControl = new ActionControl(action,this);
        			actionControls.Add(actionControl);
        			actionControl.moveUpMenuItem.IsEnabled = lineHasSeveralActions;
        			actionControl.moveDownMenuItem.IsEnabled = lineHasSeveralActions;
        			ActionsPanel.Children.Add(actionControl);
        		}
        	}   
        	else {
        		actionControls = new List<ActionControl>();
        	}
        	
        	// Check if there is a sound attached to this line, and if so represent it with a control. 
        	// Note that there is an issue with lines having a non-null Sound property (FullName == ".RES") 
        	// even when they don't have a sound:
        	if (line.Sound != null && line.Sound.FullName != ".RES") { 	
        		soundControl = new SoundControl(this);
        		Grid.SetRow(soundControl,2);
        		Grid.SetColumn(soundControl,0);
        		Grid.SetColumnSpan(soundControl,4);
        		LineControlGrid.Children.Add(soundControl);
        	}
        	
        	SetupContextMenu();

        	WriterWindow.Instance.ViewedPage += new EventHandler(LineControl_OnViewedPage);
        	
        	Dialogue.LostFocus += new RoutedEventHandler(Dialogue_LostFocus);
        	Dialogue.GotFocus += new RoutedEventHandler(Dialogue_GotFocus);
        	
        	/*
        	 * Disabled My Achievements.
        	Dialogue.TextChanged += new TextChangedEventHandler(GetNarrativeWordsTyped);
        	 */
        	
        	// Update the context menu:
        	ContextMenuOpening += delegate(object sender, ContextMenuEventArgs e)
        	{  
        		ContextMenu cm = Resources["LineControlContextMenu"] as ContextMenu;
        		foreach (MenuItem mi in cm.Items) {
        			
        			if (mi.Name == "MenuItem_AddAction") {        				
		        		if (line.Actions.Count > 0) mi.Header = EditScriptHeader;
		        		else mi.Header = AddScriptHeader;
		        		return;
        			}
        		}
        	};
        }
        

        private void Dialogue_GotFocus(object sender, RoutedEventArgs e)
        {
        	Say.Debug("BEGIN Dialogue_GotFocus (" + Dialogue.Text + ")");
        	SelectLine();
        	e.Handled = true;
        	Say.Debug("END Dialogue_GotFocus (" + Dialogue.Text + ")");
        }
        
        
        private void AddScript(object sender, RoutedEventArgs e)
        {
        	// If there already is a script on this line, open it (or warn that you have to edit the existing one, whatever's easier.)
        	// Otherwise, create a new one.
        	
        	// Calling: 'public void UseConversationLineAsTrigger(NWN2ConversationConnector line, NWN2GameConversation conversation)'
        	
        	try {
        	
	        	foreach (INWN2Plugin plugin in NWN2ToolsetMainForm.PluginHost.Plugins) {
	        		
	        		if (plugin.Name == "Flip") {
	        			
	        			MethodInfo mi = plugin.GetType().GetMethod("UseConversationLineAsTrigger",BindingFlags.Public | BindingFlags.Instance);
	        			
	        			object[] parameters = new object[] { nwn2Line, Conversation.CurrentConversation.NwnConv };
	        			
	        			if (mi != null) {
	        				mi.Invoke(plugin,parameters);
	        				return;
	        			}
	        			
	        			else throw new MethodAccessException("Couldn't find method UseConversationLineAsTrigger.");
	        		}
	        	}
        		
        		AddNWScriptActionDialog(); // if Flip has not been found
        	
        	}
        	
        	catch (Exception x) {
        		Say.Error("Couldn't add a script to this line of dialogue due to an error.",x);
        	}
        }
        
        
        private void AddCondition(object sender, RoutedEventArgs e)
        {
        	// If there already is a script on this line, open it (or warn that you have to edit the existing one, whatever's easier.)
        	// Otherwise, create a new one.
        	
        	// Calling: 'public void AddConditionToConversationLine(NWN2ConversationConnector line, NWN2GameConversation conversation)'
        	
        	try {
        	
	        	foreach (INWN2Plugin plugin in NWN2ToolsetMainForm.PluginHost.Plugins) {
	        		
	        		if (plugin.Name == "Flip") {
	        			
	        			MethodInfo mi = plugin.GetType().GetMethod("AddConditionToConversationLine",BindingFlags.Public | BindingFlags.Instance);
	        			
	        			object[] parameters = new object[] { nwn2Line, Conversation.CurrentConversation.NwnConv };
	        			
	        			if (mi != null) {
	        				mi.Invoke(plugin,parameters);
	        				return;
	        			}
	        			
	        			else throw new MethodAccessException("Couldn't find method AddConditionToConversationLine.");
	        		}
	        	}
        		
        		AddNWScriptConditionDialog(); // if Flip has not been found
        	
        	}
        	
        	catch (Exception x) {
        		Say.Error("Couldn't add a script to this line of dialogue due to an error.",x);
        	}
        }
            	
    	
    	private void AddNWScriptActionDialog()
    	{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.ValidateNames = true;
    		openFileDialog.DefaultExt = Filters.COMPILEDSCRIPTS;
    		openFileDialog.Filter = Filters.COMPILEDSCRIPTS;
			openFileDialog.Title = "Select a script to run";
			openFileDialog.Multiselect = false;
			openFileDialog.RestoreDirectory = true;
			openFileDialog.InitialDirectory = ModuleHelper.GetCurrentModulePath();			
									
  			bool ok = (bool)openFileDialog.ShowDialog();  				
  			if (ok) {
  				try {
	  				string filename = openFileDialog.FileName;
	  				FileInfo fileInfo = new FileInfo(filename);  			  				
	  				NWN2ScriptFunctor action = ScriptHelper.GetScriptFunctor(Path.GetFileNameWithoutExtension(filename),
								  				                             null, // currently don't handle scripts taking parameters
								  				                             fileInfo.DirectoryName); 	
		    		Conversation.CurrentConversation.AddAction(nwn2Line,action);
  				}
  				catch (Exception x) {
  					Say.Error("Could not add script.",x);
  				}
  			}    		
    	}
            	
    	
    	private void AddNWScriptConditionDialog()
    	{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.ValidateNames = true;
    		openFileDialog.DefaultExt = Filters.COMPILEDSCRIPTS;
    		openFileDialog.Filter = Filters.COMPILEDSCRIPTS;
			openFileDialog.Title = "Select a script to use as condition";
			openFileDialog.Multiselect = false;
			openFileDialog.RestoreDirectory = true;
			openFileDialog.InitialDirectory = ModuleHelper.GetCurrentModulePath();			
									
  			bool ok = (bool)openFileDialog.ShowDialog();  				
  			if (ok) {
  				try {
	  				string filename = openFileDialog.FileName;
	  				FileInfo fileInfo = new FileInfo(filename);  	
	  				
	  				NWN2ConditionalFunctor condition = ScriptHelper.GetConditionalFunctor(Path.GetFileNameWithoutExtension(filename),
	  				                                                                      null, // currently don't handle scripts taking parameters
	  				                                                                      fileInfo.DirectoryName);
	  				
	  				Conversation.CurrentConversation.AddCondition(nwn2Line,condition);
  				}
  				catch (Exception x) {
  					Say.Error("Could not add script as condition.",x);
  				}
  			}    		
    	}
        

        private void Dialogue_LostFocus(object sender, RoutedEventArgs e)
        {
        	Say.Debug("BEGIN Dialogue_LostFocus (" + Dialogue.Text + ")");
        	Dialogue.SelectionLength = 0; // otherwise when you come back to it text is already highlighted
        	FlushChangesToText();
        	
        	DeselectLine();
        	
        	try {
	        	// Move the text cursor, which should have the effect of hiding it
	        	// (dealing with ugly persistent cursors across textboxes when we only really want one):
	        	if (Dialogue.SelectionStart == 0 && Dialogue.Text.Length > 1) {
	        		Dialogue.SelectionStart = 1;
	        	}
	        	else {
	        		Dialogue.SelectionStart = 0;
	        	}
        	}
        	catch (Exception x) {
        		Say.Debug("Exception on changing SelectionStart of Dialogue.");
        	}
        	
        	

        	
        	e.Handled = true;
        	Say.Debug("END Dialogue_LostFocus (" + Dialogue.Text + ")");
        }
        
        protected void OnLineControlGotFocus(object sender, RoutedEventArgs rea)
        {      	        	
        	Say.Debug("BEGIN OnLineControlGotFocus (" + Dialogue.Text + ")");
        	SelectLine();
        	Say.Debug("END OnLineControlGotFocus (" + Dialogue.Text + ")");
        }
        
        
        protected void OnLineControlLostFocus(object sender, RoutedEventArgs rea)
        {
        	Say.Debug("BEGIN OnLineControlLostFocus (" + Dialogue.Text + ")");
        	DeselectLine();
        	Say.Debug("END OnLineControlLostFocus (" + Dialogue.Text + ")");
        }
        
        
        protected virtual void SetupContextMenu()
        {        
        }
        
        
        protected virtual void OnClick_GoToPage(object sender, EventArgs e)
        {        	
        }
        
        
        private void LineControl_OnViewedPage(object sender, EventArgs e)
        {
        	MenuItem MenuItem_GoBack = (MenuItem)FindName("MenuItem_GoBack");
        	
        	if (WriterWindow.Instance.PreviousPage == null) {
        		if (MenuItem_GoBack.IsEnabled) {
        			MenuItem_GoBack.IsEnabled = false;
        		}
        	}
        	else {
        		if (!MenuItem_GoBack.IsEnabled) {
        			MenuItem_GoBack.IsEnabled = true;
        		}
        	}
        }
                
        #endregion Constructor             
        
        #region Event handlers
        
//        /// <summary>
//        /// Save changes made to dialogue, if necessary updating node labels on the graph.
//        /// </summary>
//        protected void OnDialogueLostFocus2(object sender, RoutedEventArgs e)
//        {
//        	Say.Debug("BEGIN OnDialogueLostFocus2 (" + Dialogue.Text + ")");
//        	FlushChangesToText();
//        	Say.Debug("END OnDialogueLostFocus2 (" + Dialogue.Text + ")");
//        }




        bool lastCharacterWasLetter = false;
        
		/// <summary>
		/// Each time the text is changed, check whether the user has just
		/// entered a keystroke which indicates the end of a word - if so,
		/// raise an event indicating that the user just typed a 'narrative' word.
		/// </summary>
        private void GetNarrativeWordsTyped(object sender, TextChangedEventArgs e)
        {
        	TextBox textBox = (TextBox)sender;
        	bool currentCharacterIsPartOfWord;
        	
        	foreach (TextChange change in e.Changes) {        		
        		// If the user typed a single character...
        		if (change.AddedLength == 1 && change.RemovedLength == 0) {
        			char current = textBox.Text[change.Offset];
        			bool currentCharacterIsLetter = Tools.IsLetter(current);
        			// ...and it ended a word, then raise a OnWordsTyped event:
        			if (lastCharacterWasLetter && !currentCharacterIsLetter) {
        				OnWordsTyped(new WordCountEventArgs(1));
        			}
        			lastCharacterWasLetter = currentCharacterIsLetter;
        		}
        		else {
        			lastCharacterWasLetter = false;
        		}
        	}
        }   
        
        
        /// <summary>
        /// Hit delete to delete this line. Hit return to update the node labels on the graph.
        /// </summary>
        protected void OnKeyDown(object sender, KeyEventArgs e)
        {
			if (e.Key == Key.Delete) {
        		OnClick_Delete(sender,e);
		    }
        	if (e.Key == Key.Return) {
        		FlushChangesToText();
        	}
        }
        
        
        /// <summary>
        /// Save changes made to dialogue, so they are not lost when the page view is next refreshed.
        /// </summary>
        internal void FlushChangesToText()
        {
        	Say.Debug("FlushChangesToText");
        	if (Conversation.GetStringFromOEIString(nwn2Line.Line.Text) != Dialogue.Text) {
        		Conversation.CurrentConversation.SetText(nwn2Line,Dialogue.Text);
        	}
        }
                
        
        protected void OnClick_GoBack(object sender, EventArgs ea)
        {
        	if (Conversation.CurrentConversation != null && WriterWindow.Instance.PreviousPage != null) {
        		WriterWindow.Instance.DisplayPage(WriterWindow.Instance.PreviousPage);
        		WriterWindow.Instance.CentreGraph(false);
	       		Log.WriteAction(LogAction.viewed,"page","page beginning with: " + WriterWindow.Instance.CurrentPage);
        	}
        }
        
        
		protected void OnClick_SetCamera(object sender, EventArgs ea)
        {
        	
        }
		
		
		protected void OnClick_SetSound(object sender, EventArgs ea)
        {                	
        	SoundWindow window = new SoundWindow(nwn2Line);
        	window.ShowDialog();
        }		
        
		
        
        // TODO override
		protected void OnClick_MakeIntoBranch(object sender, EventArgs ea)
		{
			if (this is BranchLine) {
				Say.Information("The line you have selected is already part of a choice.");
			}
			else {
				Conversation.CurrentConversation.MakeLineIntoChoice(Nwn2Line);
				WriterWindow.Instance.PageScroll.ScrollToBottom();
			}
		}
		
		
		// TODO override
        protected void OnClick_Delete(object sender, EventArgs ea)
        { 	
        	if (!IsVisible) { // in case we have already deleted this line - should have lost focus, but doesn't seem to
        		return;
        	}        		
        	
        	if (this is BranchLine) {
	       		if (!Say.BeQuiet) {   
        			Conversation.DataFromConversation casualties = Conversation.GetWordLinePageCounts(nwn2Line);			
        			if (casualties.Words == 0 && nwn2Line.Actions.Count == 0) { // if there's no real effect, then just delete the line
        				Conversation.CurrentConversation.DeleteLineFromChoice(nwn2Line);
        				WriterWindow.Instance.PageScroll.ScrollToBottom();
        			}
        			else { // if there are consequences, remind the user of them and ask them to confirm		
        				string warning = String.Empty; 
        				if (Conversation.CurrentConversation.GetChildren(nwn2Line.Parent).Count == 2) {
        					warning += "Deleting this line will remove the whole choice, changing the shape of the conversation tree. ";
        				}  										
        				if (casualties.Pages > 1) {
        					warning += "This will also delete " + casualties.Pages + " page(s) and " + casualties.Words + 
        						" word(s) of conversation.\n\n";
        				}
        				else if (casualties.Pages == 1 && casualties.Lines > 1) {
        					warning += "This will also delete " + casualties.Words + " word(s) of conversation on the next page.\n\n";
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
//	        	if (!Say.BeQuiet && ((nwn2Line.Text.Strings.Count > 0 && nwn2Line.Text.Strings[0].Value.Length > 0) || nwn2Line.Actions.Count > 0)) {
		        	MessageBoxResult result = MessageBox.Show("Delete?","Are you sure?", MessageBoxButton.YesNo);
		        	if (result == MessageBoxResult.Yes) {
		        		Conversation.CurrentConversation.DeleteLine(this.nwn2Line);
		        	}
//	        	}
//	        	else { 
//	        		Conversation.CurrentConversation.DeleteLine(this.nwn2Line);
//	        	}        		
        	}
        }        
       
        
        protected void GoUp(object sender, EventArgs ea)
        {
        	if (WriterWindow.Instance.CurrentPage.Parent != null) {
        		WriterWindow.Instance.DisplayPage(WriterWindow.Instance.CurrentPage.Parent);
        		WriterWindow.Instance.PageScroll.ScrollToBottom(); // to ensure choice is visible
        		WriterWindow.Instance.CentreGraph(false);
        		Log.WriteAction(LogAction.viewed,"page","page beginning with: " + WriterWindow.Instance.CurrentPage);
        	}
        }
        
        
        protected void OnMouseDown(object sender, MouseEventArgs ea)
        {        	
        	Say.Debug("BEGIN OnMouseDown");
			Focus();
        	Say.Debug("END OnMouseDown");
        }        
        
        
        
        
        #endregion
        
        #region Selecting lines
        
        protected void SelectLine()
        {
        	try {
	//        	Say.Debug("BEGIN SelectLine (" + Dialogue.Text + ")");
	        	WriterWindow.Instance.SelectedLineControl = this;
	        	
	        	Background = Brushes.Wheat;
				Dialogue.IsEditable = true;
	
	        	SwitchOn(DeleteLineButton);
	//        	if (conditionalControl != null) {
	//        		SwitchOn(conditionalControl.EditConditionsButton);
	//        		SwitchOn(conditionalControl.DeleteConditionsButton);
	//        	} 
	//        	foreach (ActionControl actionControl in actionControls) {
	//        		SwitchOn(actionControl.EditActionButton);
	//        		SwitchOn(actionControl.DeleteActionButton);
	//        	}
	        	if (soundControl != null) {
	        		SwitchOn(soundControl.PlaySoundButton);
	        	}
	//        	Say.Debug("END SelectLine (" + Dialogue.Text + ")");
        	}
        	catch (Exception ex) {
        		Say.Error("Something went wrong when deselecting a line.",ex);
        	}
        }
        
        
        // TODO override
        protected void DeselectLine()
        {
        	try {
	//        	Say.Debug("BEGIN DeselectLine (" + Dialogue.Text + ")");
	        	if (this is BranchLine) {
	        		Background = Brushes.AliceBlue;
	        	}
	        	else {
	        		Background = Brushes.LightYellow;
	        	}
				Dialogue.IsEditable = false;
	        	
	        	SwitchOff(DeleteLineButton);
	//        	if (conditionalControl != null) {
	//        		SwitchOff(conditionalControl.EditConditionsButton);
	//        		SwitchOff(conditionalControl.DeleteConditionsButton);
	//        	} 
	//        	foreach (ActionControl actionControl in actionControls) {
	//        		SwitchOff(actionControl.EditActionButton);
	//        		SwitchOff(actionControl.DeleteActionButton);
	//        	}
	        	if (soundControl != null) {
	        		SwitchOff(soundControl.PlaySoundButton);
	        	}
	//        	Say.Debug("END DeselectLine (" + Dialogue.Text + ")");
        	}
        	catch (Exception ex) {
        		Say.Error("Something went wrong when deselecting a line.",ex);
        	}
        }
        
        
        protected void SwitchOn(Control c)
        {
        	c.IsEnabled = true;
        	c.Visibility = Visibility.Visible;
        }
        
        
        protected void SwitchOff(Control c)
        {
        	c.IsEnabled = false;
        	c.Visibility = Visibility.Hidden;
        }
        
        #endregion
        
		public override string ToString()
		{
			return "LineControl: " + SpeakerLabel.Text + " - " + Tools.Truncate(Dialogue.Text,60) + "...";
		}
    }
}
