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
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Media;
using AdventureAuthor.Core;
using AdventureAuthor.UI.Controls;
using AdventureAuthor.UI.Forms;
using AdventureAuthor.Utils;
using Microsoft.Win32;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.ConversationData;
using form = NWN2Toolset.NWN2ToolsetMainForm;
using Netron.Diagramming.Core;
using Netron.Diagramming.Win;
using System.Windows.Input;

namespace AdventureAuthor.UI.Windows
{
    /// <summary>
    /// Interaction logic for ConversationWriterWindow.xaml
    /// </summary>

    public sealed partial class ConversationWriterWindow : Window
    {    
    	#region Constructors 
    	
		public ConversationWriterWindow()
		{
			try {
				InitializeComponent();
			}
			catch (Exception e) {
				Say.Error(e);
			}
		}
		
		#endregion
    	
    	#region Fields    	
    	
    	/// <summary>
    	/// Used to host a .NET 2.0 Windows Forms object, specifically the conversation graph.
    	/// </summary>
    	private WindowsFormsHost host;
    	
    	private static ConversationWriterWindow instance;    	
		public static ConversationWriterWindow Instance {
			get { return instance; }
			set { instance = (ConversationWriterWindow)value; }
		}
    	
    	/// <summary>
    	/// Deprecated.
    	/// </summary>
    	private GraphWindow expandedGraphViewer;    	
		public GraphWindow ExpandedGraphViewer {
			get { return expandedGraphViewer; }
		}    	    	
    	
    	/// <summary>
    	/// The pages that make up the currently-open conversation, starting with a root page.
    	/// A new page is taken when the conversation reaches a branch point (i.e. the player or
    	/// an NPC has a choice of what to say next.)
    	/// </summary>
    	private List<ConversationPage> pages;
    	public List<ConversationPage> Pages {
			get { return pages; }
		}
    	
    	/// <summary>
    	/// The page that is currently selected in the graph and displayed in the main window.
    	/// </summary>
    	private ConversationPage currentPage;    	
		public ConversationPage CurrentPage {
			get { return currentPage; }
		}
    	
    	/// <summary>
    	/// The previously viewed page, if there is one, to allow the user to click Back.
    	/// </summary>
    	private ConversationPage previousPage;    	
		public ConversationPage PreviousPage {
			get { return previousPage; }
		}    	
    	
    	/// <summary>
    	/// The currently selected line control.
    	/// </summary>
    	private LineControl currentControl;
		public LineControl CurrentControl {
			get { return currentControl; }
			set { currentControl = value; }
		}
    	    	
    	/// <summary>
    	/// The filename of the conversation the user opened.
    	/// </summary>
    	private string originalFilename;    	
		public string OriginalFilename {
			get { return originalFilename; }
		}
    	
    	/// <summary>
    	/// The filename of the working copy of the original conversation file that we are actually operating on.
    	/// </summary>
    	private string workingFilename;    	
		public string WorkingFilename {
			get { return workingFilename; }
		}    	
    	
    	#endregion Fields
    	        
        #region UI
        
        
//        private void TEMPDROPHANDLER(object sender, DragEventArgs e)
//        {
//        	
//        	
//        	if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
//        		string[] fileNames = e.Data.GetData(DataFormats.FileDrop, true) as string[];
//        		Conversation.CurrentConversation.AddSpeaker(fileNames[0]);
//        		
//        	}
//        	e.Handled = true;
//        }
           

		internal void CreateButtonForSpeaker(Speaker speaker)
		{
			Button button = new Button();
			button.Margin = new Thickness(2);
			TextBlock textBlock = new TextBlock();
			TextBlock tb0 = new TextBlock();
			TextBlock tb1 = new TextBlock();
			TextBlock tb2 = new TextBlock();
			tb0.FontSize = 26;
			tb1.FontSize = 26;
			tb2.FontSize = 26;
			tb0.Foreground = Brushes.Black;
			tb1.Foreground = speaker.Colour;
			tb2.Foreground = Brushes.Black;
			if (speaker.Name.Length > 0 && UsefulTools.StartsWithVowel(speaker.Name)) {
				tb0.Text = "Add an ";
			}
			else {
				tb0.Text = "Add a ";
			}
			tb1.Text = speaker.Name.ToUpper();
			tb2.Text = " line";
			textBlock.Inlines.Add(tb0);
			textBlock.Inlines.Add(tb1);
			textBlock.Inlines.Add(tb2);
			button.Content = textBlock;
			button.Click += delegate 
			{ 
				if (currentPage != null) {					
					NWN2ConversationConnector parentLine;
					LineControl selectedLine = currentControl as LineControl;
					if (selectedLine != null && !selectedLine.IsPartOfBranch) {
						parentLine = selectedLine.Nwn2Line; // add a new line after the current one
					}
					else if (currentPage.LineControls.Count > 0) { // add a line to the end of the page
						parentLine = currentPage.LineControls[currentPage.LineControls.Count-1].Nwn2Line;
					}
					else { // add a line to the start of the page if there are no other lines
						parentLine = currentPage.LeadInLine; // may be null (for root)
					}
					NWN2ConversationConnector newLine = Conversation.CurrentConversation.AddLine(parentLine,speaker.Tag,true);
					
					DisplayPage(currentPage);
					RefreshDisplay(false);
					
					// TODO: Doesn't work: (but does if you launch a message box before .Focus(), 
					// something about taking focus away from screen elements maybe?)
					LineControl newLineControl = GetControlForLine(newLine);
//					MessageBox.Show("try now");
					
					newLineControl.Focus();
					newLineControl.Dialogue.Focus();					
				}
			};
			SpeakersButtonsPanel.Children.Add(button);
		}	
		
		private LineControl GetControlForLine(NWN2ConversationConnector line)
		{
			if (line == null) {
				return null;
			}
			
			foreach (LineControl c in currentPage.LineControls) {
				if (c.Nwn2Line == line) {
					return c;
				}
			}
			
			return null;			
		}
								
		private void DisplayLine(NWN2ConversationConnector line)
		{
			LineControl lineControl = new LineControl(line,false);
			this.currentPage.LineControls.Add(lineControl);
			this.LinesPanel.Children.Add(lineControl);
		}	
			
		private void DisplayBranch(NWN2ConversationConnectorCollection possibleLines) 
		{
			if (possibleLines == null) {
				Say.Error("Tried to create a branch from a null collection of lines.");
				return;
			}
			if (possibleLines.Count < 2) {
				throw new ArgumentException("Tried to create a branch with less than 2 lines.");
			}
			NWN2ConversationConnectorType speakerType = possibleLines[0].Type;
			
			// Create a LineControl for each line of dialogue in the branch:
			foreach (NWN2ConversationConnector line in possibleLines) {
				if (line.Type != speakerType) {
					throw new ArgumentException("Tried to create a branch with lines by both PC and NPC.");
				}
			}				
			BranchControl branchControl = new BranchControl(possibleLines);
			this.LinesPanel.Children.Add(branchControl);
		}
		
		private void DisplayEndOfConversation()
		{				
			EndConversationControl endOfConversation = new EndConversationControl();
			LinesPanel.Children.Add(endOfConversation);
		}
		
		public void DisplayPage(ConversationPage page)
		{
			// Save any changes that have been made to on-screen controls, since we're about to replace them:
			Conversation.CurrentConversation.SaveToWorkingCopy();
			
			// Clear the current page:
			previousPage = currentPage;
			currentPage = page;		
			currentControl = null;		
			LinesPanel.Children.Clear();
			currentPage.LineControls.Clear();
			
			// Activate the page node in the graph, and deselect the current page node if one is selected:
//			MainGraphViewer.RefreshSelectedNode();
			if (ExpandedGraphViewer != null) {
				ExpandedGraphViewer.RefreshSelectedNode();
			}
					
			// Check whether we are starting from the root:
			NWN2ConversationConnectorCollection possibleNextLines;
			if (currentPage.LeadInLine == null) { // root
				possibleNextLines = Conversation.CurrentConversation.NwnConv.StartingList;
			}
			else {
				possibleNextLines = currentPage.LeadInLine.Line.Children;
			}
			
			// Display each line of dialogue until the page branches or the conversation ends:
			while (possibleNextLines.Count == 1) {
				NWN2ConversationConnector currentLine = possibleNextLines[0];
				if (!Conversation.IsFiller(currentLine)) {
					DisplayLine(currentLine);
				}
				possibleNextLines = currentLine.Line.Children;
			}
				
			// Display the choice, check or end of conversation that ends this page:
			if (possibleNextLines.Count == 0) {
				DisplayEndOfConversation();
			}
			else {
				DisplayBranch(possibleNextLines);
			}
		}
		
		/// <summary>
		/// Creates and returns a tree of Pages from a conversation
		/// </summary>
		/// <param name="conversation">The conversation to traverse to create pages for</param>
		/// <returns>a list of all Pages in the tree, the first entry being the root Page which owns all other Pages</returns>
		private List<ConversationPage> CreatePages(Conversation conversation)
		{
			if (conversation.NwnConv == null) {
				throw new ArgumentNullException("The conversation has no NWN2GameConversation object to build pages for.");
			}
			
			List<ConversationPage> pages = new List<ConversationPage>(1);			
			ConversationPage rootPage = new ConversationPage(null,null);
			pages.Add(rootPage);
			
			// Identify every child of the root page:
			if (conversation.NwnConv.StartingList.Count == 0) {
				// do nothing; the conversation is empty, so just return the root page
			}
			else if (conversation.NwnConv.StartingList.Count > 1) {
				foreach (NWN2ConversationConnector line in conversation.NwnConv.StartingList) {
					ConversationPage page = new ConversationPage(line,rootPage);
					pages.Add(page);
				}
			}
			else {
				// Look for the first branch or conversation end:
				NWN2ConversationConnector skippableLine = conversation.NwnConv.StartingList[0];
				while (skippableLine.Line.Children.Count == 1) {
					skippableLine = skippableLine.Line.Children[0];
				}
				foreach (NWN2ConversationConnector line in skippableLine.Line.Children) {
					ConversationPage page = new ConversationPage(line,rootPage);
					pages.Add(page);
				}
			}
			
			// Identify each Page that is a child of rootPage's children:
			foreach (ConversationPage child in rootPage.Children) {
				pages.AddRange(CreatePages(child));
			}
						
			return pages;
		}
		
		private List<ConversationPage> CreatePages(ConversationPage parentPage)
		{
			List<ConversationPage> pages = new List<ConversationPage>();
			List<ConversationPage> pages2 = new List<ConversationPage>();
			
			NWN2ConversationConnector skippableLine = parentPage.LeadInLine;			
			while (skippableLine.Line.Children.Count == 1) {
				skippableLine = skippableLine.Line.Children[0];
			}
			
			foreach (NWN2ConversationConnector line in skippableLine.Line.Children) {
				ConversationPage child = new ConversationPage(line,parentPage);
				pages.Add(child);				
			}
			
			foreach (ConversationPage child in pages) {
				pages2.AddRange(CreatePages(child));
			}
			
			pages.AddRange(pages2);
			return pages;
		}
		
		public void RemoveLineControl(NWN2ConversationConnector line)
		{			
			LineControl current = CurrentControl as LineControl;
			if (current != null && current.Nwn2Line == line) {
				CurrentControl = null;
			}
			
			foreach (Control control in LinesPanel.Children) {
				LineControl lineControl = control as LineControl;
				BranchControl branchControl = control as BranchControl;
				
				if (lineControl != null && lineControl.Nwn2Line == line) {
					CurrentPage.LineControls.Remove(lineControl);
					return;
				}
				else if (branchControl != null) {
					LineControl toRemove = null;
					foreach (LineControl lc in branchControl.LineControls) {
						if (lc.Nwn2Line == line) {
							toRemove = lc;
							break;
						}
					}
					if (toRemove != null) {
						branchControl.LineControls.Remove(toRemove);
						return;
					}
				}
			}
		}
				
		public void RefreshDisplay(bool conversationStructureChanged)
		{						
			// If the structure of the page tree has changed, recreate the entire tree and reset the display:
			if (conversationStructureChanged) {
				ConversationPage newVersionOfCurrentPage = null;
				ConversationPage newVersionOfPreviousPage = null;
						
				pages = CreatePages(Conversation.CurrentConversation);
				foreach (ConversationPage p in pages) {					
					if (p.LeadInLine == currentPage.LeadInLine) {
						newVersionOfCurrentPage = p;
						break;
					}
				}
				
				if (previousPage != null) {
					foreach (ConversationPage p in pages) {
						if (p.LeadInLine == previousPage.LeadInLine) {
							newVersionOfPreviousPage = p;
							break;
						}
					}
				}
										
				// If the currently viewed page still exists after recreating the page tree, display it again; otherwise, display root:				
				if (newVersionOfCurrentPage != null) {
					DisplayPage(newVersionOfCurrentPage);
					previousPage = newVersionOfPreviousPage;
				}
				else {
					DisplayPage(pages[0]);
					previousPage = null;
				}					
				

				ConversationWriterWindow.instance.MainGraph.Open(pages);
				if (ExpandedGraphViewer != null) {
					ExpandedGraphViewer.RefreshGraph();
				}
			}
			else {
				DisplayPage(currentPage);
			}
		}
		
		#endregion UI
					
		#region Event handlers
		
		private static int offset = 0;
		
		private void OnClick_ExpandGraph(object sender, EventArgs ea)
		{
			DiagramControl d = (DiagramControl)host.Child;
								offset += 40;
					Say.Information("Trying to add shapes.");
					ClassShape shape = new ClassShape();
					shape.Height = 30;
					shape.Width =30;
					shape.X = offset;
					shape.Y = offset;
					shape.Text = "ARRG";
					d.AddShape(shape);
					host.Child.Invalidate();
			
					
//			expandedGraphViewer = new GraphWindow();
//			Window window = new Window();
//			window.Height = 900;
//			window.Width = 900;
//			window.Content = this.expandedGraphViewer;
//			expandedGraphViewer.RefreshGraph();
//			window.ShowDialog();
		}
		
		private void OnClick_New(object sender, EventArgs ea)
		{
			NewConversation form = new NewConversation();
			form.ShowDialog();
						
			// TODO: Launch a dialog box with a list of all characters in the module, and ask the user to 
			// pick up to 3 characters to talk to the player. There will be an "Add Special Speaker" button
			// which will allow the player to pick any tagged object to speak in the conversation as well
			// (so for instance, doors and treasure chests can talk to the player as well). 
			// Could also have a special Narrator speaker? (invisible object)
		}
			
		public bool OpenConversation(string name, bool createAsNew)
		{	
			if (Adventure.CurrentAdventure == null) {
				Say.Error("Open an Adventure first.");
				return false;
			}
			
			// Close current conversation, if one is open:
			if (!CloseConversationDialog()) {
				return false;
			}
			
			// Open new conversation:
			NWN2GameConversation conv = null;
			try {
				this.originalFilename = name;	
				
				if (createAsNew) { // if the original conversation doesn't exist, create it:
					conv = new NWN2GameConversation(originalFilename,
					                         		Adventure.CurrentAdventure.Module.Repository.DirectoryName,
						                     		Adventure.CurrentAdventure.Module.Repository);
					form.App.Module.AddResource(conv);
				}
					
				// Create and open a working copy of the original conversation:
				this.workingFilename = MakeWorkingCopy(originalFilename);
				conv = new NWN2GameConversation(workingFilename,
				                                Adventure.CurrentAdventure.Module.Repository.DirectoryName,
					                            Adventure.CurrentAdventure.Module.Repository);		
				
				conv.Demand();				
				Conversation.CurrentConversation = new Conversation(conv);
			}
			catch (Exception e) {			
				if (conv != null) {
					conv.Release();
				}
				string originalPath = System.IO.Path.Combine(Adventure.CurrentAdventure.Module.Repository.DirectoryName,originalFilename+".dlg");
				string workingPath = System.IO.Path.Combine(Adventure.CurrentAdventure.Module.Repository.DirectoryName,workingFilename+".dlg");
				if (createAsNew && File.Exists(originalPath)) {
					File.Delete(originalPath);
				}
				if (File.Exists(workingPath)) {
					File.Delete(workingPath);
				}	
				Say.Error("Failed to open conversation.",e);
			}
						
			// Build a graph based on the list of Pages:
			pages = CreatePages(Conversation.CurrentConversation);
			MainGraph.Open(pages);
			if (ExpandedGraphViewer != null) {
				ExpandedGraphViewer.RefreshGraph();
			}
			
			// Allocate speakers unique colours and create a button for each:				
			Conversation.CurrentConversation.AddSpeaker(String.Empty); // player has no tag, so pass String.Empty
			foreach (NWN2ConversationConnector line in Conversation.CurrentConversation.NwnConv.AllConnectors) {
				if (line.Speaker.Length > 0) {
					if (Conversation.CurrentConversation.GetSpeaker(line.Speaker) == null) { // note that line.Speaker is just a tag, not a Speaker object
						Conversation.CurrentConversation.AddSpeaker(line.Speaker);					
					}
				}					
			}
					
			ButtonsPanel.IsEnabled = true;
			
			// Display the conversation from the root page:
			DisplayPage(pages[0]);			
			this.Title += ": " + this.originalFilename;
			
			return true;
		}		
		
		private void OnClick_Open(object sender, EventArgs ea)
		{
			OpenFileDialog openFile = new OpenFileDialog();
			openFile.Filter = "dlg files (*.dlg)|*.dlg|All files (*.*)|*.*";
			openFile.Title = "Select a conversation file";
			openFile.Multiselect = false;
			openFile.InitialDirectory = form.App.Module.Repository.DirectoryName;
			openFile.RestoreDirectory = true;
			
			if ((bool)openFile.ShowDialog()) {
				OpenConversation(System.IO.Path.GetFileNameWithoutExtension(openFile.FileName),false);
			}			
		}
		
		private string MakeWorkingCopy(string originalFilenameWithoutExtension)
		{
			string tempPath, tempFileName;
			Random randomNumberGenerator = new Random();
			do {
				tempFileName = originalFilenameWithoutExtension + "_" + randomNumberGenerator.Next();
				tempPath = System.IO.Path.Combine(form.App.Module.Repository.DirectoryName,tempFileName+".dlg");
			}
			while (File.Exists(tempPath));				
				
			string originalPath = System.IO.Path.Combine(Adventure.CurrentAdventure.Module.Repository.DirectoryName,originalFilenameWithoutExtension+".dlg");
			File.Copy(originalPath,tempPath);
			return tempFileName;			
		}		
		
		private void OnClick_Save(object sender, EventArgs ea)
		{	
			if (Conversation.CurrentConversation != null) {
				Conversation.CurrentConversation.SaveToOriginal();
				Say.Information("Saved.");
			}			
		}
		
		private void OnClick_Close(object sender, EventArgs ea)
		{
			CloseConversationDialog();
		}
		
		private void OnClick_Exit(object sender, EventArgs ea)
		{
			Close();
		}
		
		private void OnClick_AddRemoveSpeakers(object sender, EventArgs ea)
		{
			if (Conversation.CurrentConversation != null) {
				AddRemoveSpeakersWindow window = new AddRemoveSpeakersWindow();
				window.ShowDialog();
			}
			else {
				Say.Information("You have to have a conversation open to be able to add speakers.");
			}
		}
		
		private void OnClick_CreateBranchAtEndOfPage(object sender, EventArgs ea)
		{
			if (Conversation.CurrentConversation != null) {
				if (!CurrentPage.IsEndPage()) {
					Say.Information("This page already ends in a branch. Try again on a page that ends with 'END OF CONVERSATION'.");
				}
				else {
					ChooseSpeaker form = new ChooseSpeaker();
					form.ShowDialog();
				}
			}
		}			
		
		private void OnClick_MakeSelectedLineIntoBranch(object sender, EventArgs ea)
		{
			LineControl lc = CurrentControl as LineControl;
			if (lc == null) {
				Say.Information("You must first click on the line that you want to make into a branch.");
			}
			else if (lc.IsPartOfBranch) {
				Say.Information("The line you have selected is already part of a branch.");
			}
			else {
				MakeLineIntoBranch(lc.Nwn2Line);
			}
		}
		
		//TODO: Move to Conversation
		internal void MakeLineIntoBranch(NWN2ConversationConnector memberOfBranch)
		{
			Conversation.CurrentConversation.InsertNewLineWithoutReparenting(memberOfBranch.Parent,memberOfBranch.Speaker);
			DisplayPage(CurrentPage);
			RefreshDisplay(true);			
		}
		
		//TODO: Move to Conversation
		internal void MakeBranchAtEndOfPage(string speakerTag)
		{
			if (!CurrentPage.IsEndPage()) {
				throw new InvalidOperationException("Tried to add a branch at the end of a page that already had one.");
			}
			
        	NWN2ConversationConnector parent;
        	NWN2ConversationConnectorCollection children;
        	if (ConversationWriterWindow.Instance.CurrentPage.LineControls.Count > 0) {
        		parent = ConversationWriterWindow.Instance.CurrentPage.LineControls[ConversationWriterWindow.Instance.CurrentPage.LineControls.Count-1].Nwn2Line;
			}
			else {
        		parent = ConversationWriterWindow.Instance.CurrentPage.LeadInLine; // LeadInLine may be null i.e. root
			}				
			
			if (parent == null) {
				children = Conversation.CurrentConversation.NwnConv.StartingList;
			}
			else {
				children = parent.Line.Children;
			}			
			
        	try {
				if (children.Count > 0) { // shouldn't happen as we are at the end of the page
					NWN2ConversationConnectorCollection childrenToRemove = new NWN2ConversationConnectorCollection();
					foreach (NWN2ConversationConnector child in children) {
						if (child.Comment != Conversation.FILLER) {
							throw new InvalidOperationException("The line at the end of the page had non-filler lines as children.");
						}		
						childrenToRemove.Add(child);
					}
					foreach (NWN2ConversationConnector child in childrenToRemove) {
						Conversation.CurrentConversation.NwnConv.RemoveNode(child);
					}
	        		Say.Error("The line Adventure Author took to be the end of the page had filler lines as children - this shouldn't have happened.");
				}			
				
				// Insert the new lines, including a filler line if necessary:
				NWN2ConversationConnectorType newLineType;
				if (speakerTag == String.Empty) {
					newLineType = NWN2ConversationConnectorType.Reply;
				}
				else if (parent == null) {
					newLineType = NWN2ConversationConnectorType.StartingEntry;
				}
				else {
					newLineType = NWN2ConversationConnectorType.Entry;
				}	
				
				if (Conversation.CanFollow(parent,newLineType)) {
					Conversation.CurrentConversation.InsertNewLineWithoutReparenting(parent,speakerTag);
					Conversation.CurrentConversation.InsertNewLineWithoutReparenting(parent,speakerTag);
				}
				else {					
					NWN2ConversationConnector fillerLine = Conversation.CurrentConversation.InsertFillerLineWithoutReparenting(parent);
					Conversation.CurrentConversation.InsertNewLineWithoutReparenting(fillerLine,speakerTag);
					Conversation.CurrentConversation.InsertNewLineWithoutReparenting(fillerLine,speakerTag);	
				}			
								
//				Conversation.CurrentConversation.SaveToWorkingCopy();
				DisplayPage(CurrentPage);
				RefreshDisplay(true);
        	}
        	catch (InvalidOperationException) {
        		Say.Error("The line at the end of the page had non-filler lines as children - failed to create a branch.");
        	}
		}		
		
		private void OnClick_AddAction(object sender, EventArgs ea)
		{
			
		}
		
		private void OnClosing(object sender, EventArgs ea)
		{
			if (!CloseConversationDialog()) {
				// TODO: Abort closing of window.
			}
		}
		
		/// <summary>
		/// Closes the current conversation; if appropriate, asks whether the user wants to save first.
		/// </summary>
		/// <returns>false if the close operation was cancelled, true otherwise</returns>
		private bool CloseConversationDialog()
		{
			if (Conversation.CurrentConversation != null) {
				if (!Adventure.BeQuiet && Conversation.CurrentConversation.IsDirty) {	
					MessageBoxResult result = MessageBox.Show("Save?", "Save changes to this conversation?", MessageBoxButton.YesNoCancel);
					if (result == MessageBoxResult.Cancel) {
						return false;
					}
					else if (result == MessageBoxResult.Yes) {
						Conversation.CurrentConversation.SaveToOriginal();
					}
				}				
				CloseConversation();
			}
			return true;
		}
			
		#endregion Event handlers
		
		
		// TODO: Highlight current node, and path through graph
		// TODO: ArrowControl, which has IsSelected and IsPartOfPath triggers like CPage
		// TODO: Write an OnClosing method for form.App, attach it, and see if it gets called by ShutdownToolset
		
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="withoutSaving"></param>
		/// <returns>false if aborted, true otherwise</returns>
		private void CloseConversation()
		{
			string workingFilePath = System.IO.Path.Combine(Adventure.CurrentAdventure.Module.Repository.DirectoryName,this.workingFilename+".dlg");
			File.Delete(workingFilePath);
			this.workingFilename = null;
			this.originalFilename = null;
			this.Title = "Conversation Writer";
			currentPage = null;
			if (pages != null) {
				pages.Clear();
			}
			currentControl = null;
			Conversation.CurrentConversation = null;
			
			MainGraph.Clear();
			if (ExpandedGraphViewer != null) {
				ExpandedGraphViewer.RefreshGraph();
			}
			
			Button addSpeakersButton = (Button)FindName("AddSpeakersButton");
			SpeakersButtonsPanel.Children.Clear();
			SpeakersButtonsPanel.Children.Add(addSpeakersButton);
		
			this.ButtonsPanel.IsEnabled = false;
			this.LinesPanel.Children.Clear();
		}
		
		private void OnLoaded(object sender, EventArgs ea)
		{
			host = new WindowsFormsHost();
			MainGraph = new ConversationGraph();
			host.Child = MainGraph;
			Grid.SetRow(host,0);
			Grid.SetColumn(host,0);
			GraphGrid.Children.Add(host);
		}
		
		public ConversationGraph MainGraph;
    }
}
		
			
			
			
			



