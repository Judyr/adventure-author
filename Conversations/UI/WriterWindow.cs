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
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Media;
using System.Windows.Documents;
using AdventureAuthor.Conversations.UI.Controls;
using AdventureAuthor.Conversations.UI.Graph;
using AdventureAuthor.Core;
using AdventureAuthor.Scripts;
using AdventureAuthor.Utils;
using Microsoft.Win32;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.ConversationData;
using NWN2Toolset.NWN2.IO;
using form = NWN2Toolset.NWN2ToolsetMainForm;
using System.Windows.Input;
using OEIShared.Utils;

namespace AdventureAuthor.Conversations.UI
{
    /// <summary>
    /// Interaction logic for ConversationWriterWindow.xaml
    /// </summary>

    public sealed partial class WriterWindow : Window
    {       
    	#region Fields    	
    	
    	/// <summary>
    	/// The single instance of the Writer window.
    	/// <remarks>Pseudo-Singleton pattern, but I haven't really implemented this.</remarks>
    	/// </summary>
    	private static WriterWindow instance;    	
		public static WriterWindow Instance {
			get { return instance; }
			set { instance = (WriterWindow)value; }
		}    	
    	
    	
    	/// <summary>
    	/// A form containing a navigatable graph representation of the conversation, displayed on the main window.
    	/// </summary>	
		public GraphForm MainGraph {
			get { return this.mainGraph; }
		}
    	
		
		/// <summary>
		/// A form containing a navigatable graph representation of the conversation, 
		/// displayed via an Expand Graph button on the main window.
		/// </summary>
		private GraphForm expandedGraph = null;	
		public GraphForm ExpandedGraph {
			get { return expandedGraph; }
		}
    	
		
    	/// <summary>
    	/// The pages that make up the currently-open conversation, starting with a root page.
    	/// A new page is taken when the conversation reaches a branch point (i.e. the player or
    	/// an NPC has a choice of what to say next.)
    	/// </summary>
    	private List<Page> pages;
    	public List<Page> Pages {
			get { return pages; }
		}
    	
    	
    	/// <summary>
    	/// The page that is currently selected in the graph and displayed in the main window.
    	/// </summary>
    	private Page currentPage;    	
		public Page CurrentPage {
			get { return currentPage; }
		}
    	
    	
    	/// <summary>
    	/// The previously viewed page, if there is one, to allow the user to click Back.
    	/// </summary>
    	private Page previousPage;    	
		public Page PreviousPage {
			get { return previousPage; }
		}    	
    	
    	
    	/// <summary>
    	/// The currently selected line control.
    	/// </summary>
    	private LineControl selectedLineControl;
		public LineControl SelectedLineControl {
			get { return selectedLineControl; }
			set { selectedLineControl = value; 
				if (selectedLineControl == null) Say.Debug("Set CurrentControl to null."); 
				else Say.Debug("Set CurrentControl to value: " + selectedLineControl.Nwn2Line.ToString());}
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
    	
    	
//    	private static Brush DRAGOVERBRUSH = Brushes.SandyBrown;
//    	private static Brush SELECTEDBRUSH = Brushes.Wheat;
//    	private static Brush UNSELECTEDBRUSH = Brushes.LightYellow;
//    	private static Brush BRANCHSELECTEDBRUSH = Brushes.Wheat;
//    	private static Brush BRANCHUNSELECTEDBRUSH = Brushes.AliceBlue;
    	
    	#endregion Fields
    	  						
		#region Events
		
		public event EventHandler ViewedPage;
		
		private void OnViewedPage(EventArgs e)
		{
			Say.Debug("Conversation.ViewedPage event raised.");
			EventHandler handler = ViewedPage;
			if (handler != null) {
				handler(this,e);
			}
		}
		
		#endregion
		    	      	
    	#region Constructors 
    	
    	/// <summary>
    	/// Open a new conversation writer window, to create and edit conversations for game characters.
    	/// </summary>
    	/// <param name="launchNewOpenConversationDialog">True to launch a dialog upon loading that
    	/// asks the user if they want to create a new conversation or open an existing conversation;
    	/// false to do nothing.</param>
		public WriterWindow(bool launchNewOpenConversationDialog)
		{
			InitializeComponent();
			
    		Loaded += delegate
    		{  
				Log.WriteAction(LogAction.launched,"conversationwriter");
				
				if (launchNewOpenConversationDialog) {
					NewOpenConversationWindow win = new NewOpenConversationWindow();
					win.ShowDialog();
				}
				
				mainGraph.MinimumSize = mainGraph.Size;
				mainGraph.MaximumSize = mainGraph.MinimumSize;
    		};
    		
            Closing += delegate { 
            	try {
            		Log.WriteAction(LogAction.exited,"conversationwriter");
            	}
            	catch (Exception) { 
            		// already disposed because toolset is closing
            	}
            };
    		
//    		this.PreviewDrop += delegate(object sender, DragEventArgs e) { 
//    			Say.Information("Source: " + e.Source + "\n\nOriginal source: " + e.OriginalSource);
//    		};

			MinWidth = AdventureAuthor.Utils.Tools.MINIMUMWINDOWWIDTH;
			MinHeight = AdventureAuthor.Utils.Tools.MINIMUMWINDOWHEIGHT;
		}
		
		
		public WriterWindow(string filename) : this(false)
		{
			Loaded += delegate 
			{
				try {
					Open(filename);
				}
				catch (ApplicationException e) {
					Say.Error(e);
				}
			};
		}
		
		#endregion
		
        #region Page view
          
        /// <summary>
		/// Refresh the page view display for the current page, to take account of changes to the page.
		/// </summary>
		private void RefreshPageViewOnly()
		{
			// Save any changes to the currently selected line before clearing the page, but NOT
			// if it's a filler line - this can happen when the selected line is 'deleted' and the software
			// chooses to turn it into an invisible filler line rather than actually deleting it. In these
			// circumstances the LineControl needs to be ignored until the page is cleared and repopulated,
			// at which point the LineControl will not be recreated. Otherwise, it will save the text of the
			// LineControl to what is now a filler line, causing problems. 
			if (SelectedLineControl != null && !Conversation.IsFiller(SelectedLineControl.Nwn2Line)) {
				SelectedLineControl.FlushChangesToText();
				SelectedLineControl = null;
			}			
			LinesPanel.Children.Clear();
			currentPage.LineControls.Clear();
								
			// Show the (disabled) first line of the previous page at the top of the page view:
			if (currentPage.LeadLine != null) {
				LeadingLine lineControl = ShowLeadingLine(currentPage.LeadLine); 
				AddHandlersForDropping(lineControl); // not draggable
			}
			
			// Display each line of dialogue until the page branches or the conversation ends:
			NWN2ConversationConnectorCollection possibleNextLines = Conversation.CurrentConversation.GetChildren(currentPage.LeadLine);
			while (possibleNextLines.Count == 1) {
				NWN2ConversationConnector currentLine = possibleNextLines[0];
				if (!Conversation.IsFiller(currentLine)) {
					Line lineControl = ShowLine(currentLine);
					AddHandlersForDragging(lineControl);
					AddHandlersForDropping(lineControl);
				}
				possibleNextLines = currentLine.Line.Children;
			}
				
			// Display the choice, check or end of conversation that ends this page:
			if (possibleNextLines.Count == 0) {
				ShowEndOfConversation(); // not draggable or droppable
			}
			else {
				ChoiceControl choiceControl = ShowChoice(possibleNextLines);
				foreach (LineControl lineControl in choiceControl.LineControls) {					
					AddHandlersForDragging(lineControl);
					AddHandlersForDropping(lineControl);
				}
			}
		}		
		
		
		/// <summary>
		/// Add the line of dialogue that leads to this page to the current page view, unless this is the root page.
		/// </summary>
		/// <param name="line">The leading line to add</param>
		private LeadingLine ShowLeadingLine(NWN2ConversationConnector line)
		{
			LeadingLine leadingLine = new LeadingLine(line);
			LinesPanel.Children.Insert(0,leadingLine);
			return leadingLine;
		}
		
		
		/// <summary>
		/// Add a line of dialogue to the current page view.
		/// </summary>
		/// <param name="line">The line of dialogue to add</param>
		private Line ShowLine(NWN2ConversationConnector line)
		{
			Line lineControl = new Line(line);
			this.currentPage.LineControls.Add(lineControl);
			this.LinesPanel.Children.Add(lineControl);
			return lineControl;
		}				
	
		
		/// <summary>
		/// Add a branch to the current page view.
		/// </summary>
		/// <param name="possibleLines">The lines which make up the branch</param>
		private ChoiceControl ShowChoice(NWN2ConversationConnectorCollection possibleLines) 
		{
			if (possibleLines == null) {
				throw new ArgumentNullException("Tried to create a choice from a null collection of lines.");
			}
			if (possibleLines.Count < 2) {
				throw new ArgumentException("Tried to create a choice with less than 2 lines.");
			}
			NWN2ConversationConnectorType speakerType = possibleLines[0].Type;
			
			// Create a LineControl for each line of dialogue in the branch:
			foreach (NWN2ConversationConnector line in possibleLines) {
				if (!Conversation.AreSameSpeakerType(line.Type,speakerType)) {					
					throw new ArgumentException("Tried to create a choice with lines by both PC and NPC.");
				}
			}				
			ChoiceControl choiceControl = new ChoiceControl(possibleLines);
			this.LinesPanel.Children.Add(choiceControl);
			return choiceControl;
		}		
		
		
		/// <summary>
		/// Add an end of conversation notice to the current page view.
		/// </summary>
		private EndConversationControl ShowEndOfConversation()
		{				
			EndConversationControl endOfConversation = new EndConversationControl();
			LinesPanel.Children.Add(endOfConversation);
			return endOfConversation;
		}
		
		
		private void AddHandlersForDragging(LineControl lineControl)
		{
			lineControl.MouseLeftButtonDown += lineControl_PreviewMouseLeftButtonDown;
			lineControl.MouseMove += lineControl_PreviewMouseMove;
		}
		
		
		private void AddHandlersForDropping(LineControl lineControl)
		{			
			lineControl.PreviewDrop += lineControl_OnDrop;
			
			// too horrible, do properly:
//			lineControl.DragEnter += delegate(object sender, DragEventArgs e) { 
//				LineControl lineControl = (LineControl)sender;
//				lineControl.Background = DRAGOVERBRUSH;
//			};
//			
//			lineControl.DragLeave += delegate(object sender, DragEventArgs e) { 
//				LineControl lineControl = (LineControl)sender;
//				if (SelectedLineControl == lineControl) {
//					if (lineControl is BranchLine) {
//						lineControl.Background = BRANCHSELECTEDBRUSH;
//					}
//					else {
//						lineControl.Background = SELECTEDBRUSH;
//					}
//				}
//				else {
//					if (lineControl is BranchLine) {
//						lineControl.Background = BRANCHUNSELECTEDBRUSH;
//					}
//					else {
//						lineControl.Background = UNSELECTEDBRUSH;
//					}
//				}
//			};
		}
						
        #endregion
        
        #region Graph view   
				
		/// <summary>
		/// Refresh both the graph view and the page view to account for changes to the conversation.
		/// </summary>
		private void RefreshBothViews()
		{					
			// The structure of the conversation graph has changed, so recreate the entire graph and reset the display:			
			Page newVersionOfCurrentPage = null;
			Page newVersionOfPreviousPage = null;
						
			pages = CreatePageTree(Conversation.CurrentConversation);
			foreach (Page p in pages) {					
				if (p.LeadLine == currentPage.LeadLine) {
					newVersionOfCurrentPage = p;						
					break;
				}
			}
							
			if (previousPage != null) {
				foreach (Page p in pages) {
					if (p.LeadLine == previousPage.LeadLine) {
						newVersionOfPreviousPage = p;
						break;
					}
				}
			}								
				
			MainGraph.Open(pages);
			if (ExpandedGraph != null) {
				ExpandedGraph.Open(pages);
			}
				
			// If the currently viewed page still exists after recreating the graph, display it again; otherwise, display root.
			// Note that this acts to refresh the page view, and so no separate call to RefreshPageViewOnly() is necessary.
			if (newVersionOfCurrentPage != null) {
				currentPage = newVersionOfCurrentPage;
				previousPage = newVersionOfPreviousPage;
				DisplayPage(newVersionOfCurrentPage);
			}
			else {
				previousPage = null;
				currentPage = null;
				DisplayPage(pages[0]);
				CentreGraph(false);
			}	
		}
		        

		/// <summary>
		/// Centre the conversation graph(s) around the node representing the current page.
		/// </summary>
		/// <param name="resetZoom">True to reset the zoom level to default; false otherwise</param>
		public void CentreGraph(bool resetZoom)
		{
			if (resetZoom) {
				MainGraph.GraphControl.Magnification = new System.Drawing.SizeF(100F,100F);
				if (ExpandedGraph != null) {
					ExpandedGraph.GraphControl.Magnification = new System.Drawing.SizeF(100F,100F);
				}
			}
			
			MainGraph.GraphControl.CentreOnShape(MainGraph.GetNode(CurrentPage));
			if (ExpandedGraph != null) {
				ExpandedGraph.GraphControl.CentreOnShape(ExpandedGraph.GetNode(CurrentPage));
			}
		}		
		        
        #endregion
        
        #region Writer window        
   
        
        #endregion

        
        private void SetUI_ConversationIsOpen()
        {
        	// main interface:
			buttonsPanel.IsEnabled = true;
			ExpandGraphButton.IsEnabled = true;
			GoToStartButton.IsEnabled = true;
			
        	// file menu:
        	saveMenuItem.IsEnabled = true;
        	exportMenu.IsEnabled = true;
        	closeMenuItem.IsEnabled = true;
        	optionsMenu.IsEnabled = true;        	
        	replaceSpeakerMenuItem.IsEnabled =  Conversation.CurrentConversation.Speakers.Count > 1;       		
        }
        
        
        private void SetUI_ConversationIsNotOpen()
        {        	
        	// main interface:
			buttonsPanel.IsEnabled = false;
			ExpandGraphButton.IsEnabled = false;
			GoToStartButton.IsEnabled = false;
			
        	// file menu:
        	saveMenuItem.IsEnabled = false;
        	exportMenu.IsEnabled = false;
        	closeMenuItem.IsEnabled = false;
        	optionsMenu.IsEnabled = false;
        	replaceSpeakerMenuItem.IsEnabled = false;
        }
        
		
		/// <summary>
		/// Display a page of the conversation, in the page view and in the graph view.
		/// </summary>
		/// <param name="page">The page to display</param>
		public void DisplayPage(Page page)
		{
			// Update references to the currently and previously viewed pages:
			if (currentPage != page) {
				previousPage = currentPage;
				currentPage = page;				
			}
			
			// Draw the new current page:
			RefreshPageViewOnly();
			PageScroll.ScrollToTop();
			
			// Select the node and its route in the graph:
			Node mainNode = MainGraph.GetNode(currentPage);
			MainGraph.GraphControl.SelectNode(mainNode);
			if (ExpandedGraph != null) {
				Node expandedNode = ExpandedGraph.GetNode(currentPage);
				ExpandedGraph.GraphControl.SelectNode(expandedNode);
			}
			
			OnViewedPage(new EventArgs());
		}
			
				
		/// <summary>
		/// Creates and returns a tree of Pages from a conversation
		/// </summary>
		/// <param name="conversation">The conversation to traverse to create pages for</param>
		/// <returns>a list of all Pages in the tree, the first entry being the root Page which owns all other Pages</returns>
		private List<Page> CreatePageTree(Conversation conversation)
		{
			if (conversation.NwnConv == null) {
				throw new ArgumentNullException("The conversation has no NWN2GameConversation object to build pages for.");
			}
			
			List<Page> pages = new List<Page>(1);			
			Page rootPage = new Page(null,null);
			pages.Add(rootPage);
			
			// Identify every child of the root page:
			if (conversation.NwnConv.StartingList.Count == 0) {
				// do nothing; the conversation is empty, so just return the root page
			}
			else if (conversation.NwnConv.StartingList.Count > 1) {
				foreach (NWN2ConversationConnector line in conversation.NwnConv.StartingList) {
					Page page = new Page(line,rootPage);
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
					Page page = new Page(line,rootPage);
					pages.Add(page);
				}
			}
			
			// Identify each Page that is a child of rootPage's children:
			foreach (Page child in rootPage.Children) {
				pages.AddRange(CreatePages(child));
			}
						
			return pages;
		}
		
		
		/// <summary>
		/// A recursive method called from CreatePageTree, to create pages for the children of a given page.
		/// </summary>
		/// <param name="parentPage">The page to create child pages for</param>
		/// <returns>A list of created child pages</returns>
		private List<Page> CreatePages(Page parentPage)
		{
			List<Page> pages = new List<Page>();
			List<Page> pages2 = new List<Page>();
			
			NWN2ConversationConnector skippableLine = parentPage.LeadLine;			
			while (skippableLine.Line.Children.Count == 1) {
				skippableLine = skippableLine.Line.Children[0];
			}
			
			foreach (NWN2ConversationConnector line in skippableLine.Line.Children) {
				Page child = new Page(line,parentPage);
				pages.Add(child);				
			}
			
			foreach (Page child in pages) {
				pages2.AddRange(CreatePages(child));
			}
			
			pages.AddRange(pages2);
			return pages;
		}
		
				
		#region Event handlers: graph view
				
		private void OnClick_ExpandGraph(object sender, EventArgs ea)
		{
			if (Conversation.CurrentConversation != null) {
				Log.WriteAction(LogAction.launched,"expandedgraph");
				expandedGraph = new GraphForm(true);
				expandedGraph.Open(pages);
				DisplayPage(currentPage);
				expandedGraph.GraphControl.CentreOnShape(expandedGraph.GetNode(pages[0])); // display from root for clarity
				expandedGraph.ShowDialog();
			}
		}
		
		
		/// <summary>
		/// On clicking 'Go to Start', display the root page and centre the graph on it.
		/// </summary>
		private void OnClick_GoToStart(object sender, EventArgs ea)
		{
			DisplayPage(pages[0]);
			CentreGraph(true);
	      	Log.WriteAction(LogAction.viewed,"page","clicked 'go to start'");
		}
		
		#endregion
		
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
		
		#region Opening a conversation
		
		/// <summary>
		/// Open an existing conversation.
		/// </summary>
		/// <param name="filename">The filename of the conversation.</param>
		/// <remarks>The conversation file must be located in the directory of the current Adventure.</remarks>
		public void Open(string filename)
		{
			if (form.App.Module == null || form.App.Module.LocationType != ModuleLocationType.Directory) {
				Say.Error("Open an Adventure first.");
				return;
			}
			
			// Close current conversation, if one is open:
			if (!CloseConversationDialog()) {
				return;
			}
			
			Log.WriteAction(LogAction.opened,"conversation",Path.GetFileName(filename));
			
			try {
				Open(filename,false);
			}
			catch (ApplicationException e) {
				Say.Error(e);
			}
		}
				
		
		/// <summary>
		/// Create and open a new conversation.
		/// </summary>
		/// <param name="filename">The filename of the conversation to create.</param>
		public void CreateOpen(string filename)
		{
			if (form.App.Module == null || form.App.Module.LocationType != ModuleLocationType.Directory) {
				Say.Error("Open an Adventure first.");
				return;
			}
			else if (!ModuleHelper.IsValidName(filename)) {
				Say.Error("'" + filename + "' is not a valid name for a conversation.");
				return;
			}
			
			// Close current conversation, if one is open:
			if (!CloseConversationDialog()) {
				return;
			}
			
			Log.WriteAction(LogAction.added,"conversation",Path.GetFileName(filename));
			Log.WriteAction(LogAction.opened,"conversation",Path.GetFileName(filename));
			
			try {
				Open(filename,true);
			}
			catch (ApplicationException e) {
				Say.Error(e);
			}
		}
		
		
		private void Open(string name, bool createAsNew)
		{				
			NWN2GameConversation conv = null;
			try {
				this.originalFilename = name;	
				
				if (createAsNew) { // if the original conversation doesn't exist, create it:
					conv = new NWN2GameConversation(originalFilename,
					                         		form.App.Module.Repository.DirectoryName,
						                     		form.App.Module.Repository);
					form.App.Module.AddResource(conv); // necessary?
				}
					
				// Create and open a working copy of the original conversation:
				this.workingFilename = CreateWorkingCopy(originalFilename);
				conv = new NWN2GameConversation(workingFilename,
				                                form.App.Module.Repository.DirectoryName,
					                            form.App.Module.Repository);		
				
				conv.Demand();				
				Conversation.CurrentConversation = new Conversation(conv);
				
				// Add event handlers:
				Conversation.CurrentConversation.Changed += 
					new EventHandler<ConversationChangedEventArgs>(WriterWindow_OnChanged);
				Conversation.CurrentConversation.Saved += 
					new EventHandler<EventArgs>(WriterWindow_OnSaved);
				Conversation.CurrentConversation.SpeakerAdded += 
					new EventHandler<SpeakerAddedEventArgs>(WriterWindow_OnSpeakerAdded);
			}
			catch (Exception e) {			
				if (conv != null) {
					conv.Release();
				}
				string originalPath = System.IO.Path.Combine(form.App.Module.Repository.DirectoryName,originalFilename+".dlg");
				string workingPath = System.IO.Path.Combine(form.App.Module.Repository.DirectoryName,workingFilename+".dlg");
				if (createAsNew && File.Exists(originalPath)) {
					File.Delete(originalPath);
				}
				if (File.Exists(workingPath)) {
					File.Delete(workingPath);
				}	
				throw new ApplicationException("Failed to open this conversation.",e);
			}
						
			// Build a graph based on the list of Pages:
			pages = CreatePageTree(Conversation.CurrentConversation);
			MainGraph.Open(pages);
			if (ExpandedGraph != null) {
				ExpandedGraph.Open(pages);
			}
			
			try {
			// Allocate speakers unique colours and create a button for each:				
			Conversation.CurrentConversation.AddSpeaker(String.Empty); // player has no tag, so pass String.Empty
			foreach (NWN2ConversationConnector line in Conversation.CurrentConversation.NwnConv.AllConnectors) {
				if (line.Speaker.Length > 0) {
					if (Conversation.CurrentConversation.GetSpeaker(line.Speaker) == null) { // note that line.Speaker is just a tag, not a Speaker object
						Conversation.CurrentConversation.AddSpeaker(line.Speaker);					
					}
				}					
			}
			}
			catch (Exception e) {
				Say.Error(e);
			}
			
			SetUI_ConversationIsOpen();
			
			// Display the conversation from the root page:
			DisplayPage(pages[0]);	
			CentreGraph(true);
			SetTitleBar();
		}		
		
		
		/// <summary>
		/// Create a working copy of a conversation file with a unique temporary name.
		/// </summary>
		/// <param name="originalFilename">The name of the conversation file to create a working copy of</param>
		/// <returns>The filename (without extension) of the working copy.</returns>
		private string CreateWorkingCopy(string originalFilename)
		{				
			string tempname, temppath;
			Random random = new Random();
			do {
				tempname = "~tmp" + random.Next();
				temppath = Path.Combine(form.App.Module.Repository.DirectoryName,tempname+".dlg");
			}
			while (File.Exists(temppath));		
							
			string originalPath = Path.Combine(form.App.Module.Repository.DirectoryName,originalFilename+".dlg");
			File.Copy(originalPath,temppath);
			return tempname;			
		}	
		
		#endregion
		
		private void OnClick_Open(object sender, EventArgs ea)
		{
			OpenDialog();
		}
		
		
		internal void OpenDialog()
		{
			OpenFileDialog openFile = new OpenFileDialog();
			openFile.ValidateNames = true;
			openFile.Filter = "dlg files (*.dlg)|*.dlg";
			openFile.Title = "Select a conversation file";
			openFile.Multiselect = false;
			openFile.InitialDirectory = form.App.Module.Repository.DirectoryName;
			openFile.RestoreDirectory = true;
			
			if ((bool)openFile.ShowDialog()) {
				if (openFile.SafeFileName.StartsWith("~tmp")) {
					Say.Information(openFile.SafeFileName + " is a temporary file, and cannot be opened.");
					return;
				}
				
				Open(Path.GetFileNameWithoutExtension(openFile.FileName));
			}			
		}
				
		
		
		private void OnClick_Save(object sender, EventArgs ea)
		{	
			if (Conversation.CurrentConversation != null) {
				Conversation.CurrentConversation.SaveToOriginal();
			}			
		}
						
		
		private void OnClick_ExportTextFile_PageFormat(object sender, EventArgs e)
		{
			ExportDialog(ExportFormat.PageFormat);
		}
				
		
		private void OnClick_ExportTextFile_TreeFormat(object sender, EventArgs e)
		{
			ExportDialog(ExportFormat.TreeFormat);
		}
		
		
		private void ExportDialog(ExportFormat format)
		{
			if (Conversation.CurrentConversation != null) {	
				if (Conversation.CurrentConversation.IsDirty) {
					MessageBoxResult result = MessageBox.Show("Save?", 
					                                          "Save changes to this conversation?", 
					                                          MessageBoxButton.YesNoCancel);
					if (result == MessageBoxResult.Yes) {		
						Conversation.CurrentConversation.SaveToOriginal();
					}
					else if (result == MessageBoxResult.Cancel) {
						return;
					}
				}
				
	    		SaveFileDialog saveFileDialog = new SaveFileDialog();
	    		saveFileDialog.AddExtension = true;
	    		saveFileDialog.CheckPathExists = true;
	    		saveFileDialog.DefaultExt = Filters.TXT;
	    		saveFileDialog.Filter = Filters.TXT;
	  			saveFileDialog.ValidateNames = true;
	  			saveFileDialog.Title = "Select location to export conversation to";
	  			
	  			// get the default filename from the conversation filename:
	  			try {
	  				saveFileDialog.FileName = Path.Combine(saveFileDialog.InitialDirectory,originalFilename + ".txt");
	  			}
	  			catch (Exception e) {
	  				Say.Error(e);
	  			};
	  			
	  			bool ok = (bool)saveFileDialog.ShowDialog();  				
	  			if (ok) {	  				
	  				string filename = saveFileDialog.FileName;
	  				try {	  					
	  					Conversation.CurrentConversation.ExportToTextFile(filename,format);
	  					Log.WriteMessage("exported conversation to " + filename + " (format: " + format + ")");
	  					Process.Start(filename);
	  				}
	  				catch (IOException e) {
	  					Say.Error("Failed to export conversation.",e);
	  				}
	  			}
			}	
		}
		
		
		private void OnClick_Close(object sender, EventArgs ea)
		{
			CloseConversationDialog();
			SetTitleBar();
		}
		
		
		private void OnClick_Exit(object sender, EventArgs ea)
		{
			Close(); // close window
		}
		
		
		private void OnClick_AddRemoveSpeakers(object sender, EventArgs ea)
		{
			if (Conversation.CurrentConversation != null) {
				AddSpeakerWindow window = new AddSpeakerWindow();
				window.ShowDialog();
			}
		}
		
		
		private void OnClick_ReplaceSpeaker(object sender, EventArgs e)
		{
			if (Conversation.CurrentConversation != null) {
				ReplaceSpeakerWindow window = new ReplaceSpeakerWindow();
				window.ShowDialog();
			}
		}
		
		
		private void OnClick_CreateChoiceAtEndOfPage(object sender, EventArgs ea)
		{
			if (Conversation.CurrentConversation != null) {
				if (!CurrentPage.IsEndPage) {
					Say.Information("This page already ends in a branch. Try again on a page that ends with 'END OF CONVERSATION'.");
				}
				else {
					ChooseSpeaker form = new ChooseSpeaker();
					form.ShowDialog();
				}
			}
		}			
		
		
		/// <summary>
		/// If there's a conversation still open, ask if the user wants to save it before closing. 
		/// If they cancel, cancel the closing event.
		/// </summary>
		private void OnClosing(object sender, CancelEventArgs ea)
		{
			if (!CloseConversationDialog()) {
				ea.Cancel = true;
			}
			else {
				string path = form.App.Module.Repository.DirectoryName;
				DirectoryInfo di = new DirectoryInfo(path);
				FileInfo[] tempFiles = di.GetFiles("~tmp*.dlg");
				foreach (FileInfo file in tempFiles) {
					file.Delete();
				}
			}
		}
		
		
		/// <summary>
		/// Check to see if there are old temporary conversation files lying about, and if so delete them. 
		/// </summary>
		/// <remarks>These are usually deleted when they're done with, but will remain if there's a crash.</remarks>
		private void OnClosed(object sender, EventArgs ea)
		{
//			string path = form.App.Module.Repository.DirectoryName;
//			DirectoryInfo di = new DirectoryInfo(path);
//			FileInfo[] tempFiles = di.GetFiles("~tmp*.dlg");
//			foreach (FileInfo file in tempFiles) {
//				file.Delete();
//			}
		}
		
		
		/// <summary>
		/// Closes the current conversation; if appropriate, asks whether the user wants to save first.
		/// </summary>
		/// <returns>False if the close operation was cancelled; true otherwise</returns>
		private bool CloseConversationDialog()
		{
			if (Conversation.CurrentConversation != null) {
				// Make sure you get any changes to the current line:
				if (SelectedLineControl != null && !Conversation.IsFiller(SelectedLineControl.Nwn2Line)) {
					SelectedLineControl.FlushChangesToText();
				}	
				if (!ModuleHelper.BeQuiet && Conversation.CurrentConversation.IsDirty) {	
					MessageBoxResult result = MessageBox.Show("Save?", "Save changes to this conversation?", MessageBoxButton.YesNoCancel);
					if (result == MessageBoxResult.Cancel) {
						return false;
					}
					else if (result == MessageBoxResult.Yes) {
						Conversation.CurrentConversation.SaveToOriginal();
					}
				}	
				
				Log.WriteAction(LogAction.closed,"conversation",this.originalFilename);				
				CloseConversation();
			}
			else {
				Say.Debug("CurrentConversation is null.");
			}
			return true;
		}
			
		
		
		// TODO: Write an OnClosing method for form.App, attach it, and see if it gets called by ShutdownToolset
		
		
		/// <summary>
		/// Close the current conversation.
		/// </summary>
		private void CloseConversation()
		{
			try {
				string workingFilePath = 
					System.IO.Path.Combine(form.App.Module.Repository.DirectoryName,workingFilename+".dlg");
				if (File.Exists(workingFilePath)) {
					File.Delete(workingFilePath);
				}
				this.workingFilename = null;
				this.originalFilename = null;
				currentPage = null;
				if (pages != null) {
					pages.Clear();
				}
				
				SelectedLineControl = null;
				Conversation.CurrentConversation = null;
				SetTitleBar();
								
				MainGraph.ClearGraph();					
				if (ExpandedGraph != null) {
					ExpandedGraph.ClearGraph();
				}
								
				Button addSpeakersButton = (Button)FindName("AddSpeakersButton");
				speakersButtonsPanel.Children.Clear();
				speakersButtonsPanel.Children.Add(addSpeakersButton);
			
				SetUI_ConversationIsNotOpen();
				
				this.LinesPanel.Children.Clear();	
			}
			catch (Exception e) {
				Say.Error(e);
			}
		}
		

		private void SetTitleBar()
		{
			if (Conversation.CurrentConversation == null) {
				this.Title = "Conversation Writer";
			}
			else if (!Conversation.CurrentConversation.IsDirty) {
				this.Title = this.OriginalFilename + " - Conversation Writer";
			}
			else {
				this.Title = this.OriginalFilename + "* - Conversation Writer";
			}
		}
				
			
		/// <summary>
		/// Refresh the page and graph views as needed, and update the title bar to reflect that the working copy has changes.
		/// </summary>
		private void WriterWindow_OnChanged(object sender, ConversationChangedEventArgs e)
		{
			if (e.GraphOutOfDate) {
				RefreshBothViews();
			}
			else {
				RefreshPageViewOnly();
			}		
			
			SetTitleBar();
		}
		
		
		/// <summary>
		/// Update the title bar to reflect that the working copy is up to date.
		/// </summary>
		private void WriterWindow_OnSaved(object sender, EventArgs e)
		{
			SetTitleBar();
		}
		
		
		/// <summary>
		/// Create a button on the interface that allows user to add lines of dialogue by the newly added speaker.
		/// </summary>
		private void WriterWindow_OnSpeakerAdded(object sender, SpeakerAddedEventArgs e)
		{
			SpeakerButton button = new SpeakerButton(e.Speaker);
			
			button.Click += delegate 
			{ 
				if (currentPage != null) {	
					NWN2ConversationConnector parentLine;
					if (SelectedLineControl != null && !(SelectedLineControl is BranchLine)) {
						parentLine = SelectedLineControl.Nwn2Line; // add a new line after the current one
						Say.Debug("Found a selected line that was not part of a branch - make this the parent line.");
					}
					else if (currentPage.LineControls.Count > 0) { // add a line to the end of the page
						parentLine = currentPage.LineControls[currentPage.LineControls.Count-1].Nwn2Line;
						Say.Debug("Found no selected lines. Use the last LineControl on the page.");
					}
					else { // add a line to the start of the page if there are no other lines
						parentLine = currentPage.LeadLine; // may be null (for root)
						Say.Debug("Found no LineControls at all. Use the lead in line of the current page.");
					}
					
					NWN2ConversationConnector newLine = Conversation.CurrentConversation.AddLine(parentLine,e.Speaker.Tag);
										
					FocusOn(newLine);
				}
			};
			
			speakersButtonsPanel.Children.Add(button);
			
			// disable Replace Speaker until there is at least one NPC speaker.
			// (don't have to disable this again, as there's currently no way to remove speakers)
			if (!replaceSpeakerMenuItem.IsEnabled && !e.Speaker.IsPlayer()) {
				replaceSpeakerMenuItem.IsEnabled = true;
			}
		}	
		
		
		/// <summary>
		/// Bring a line into view and focus on it.
		/// </summary>
		/// <param name="line">The line of dialogue</param>
		internal void FocusOn(NWN2ConversationConnector line)
		{
			if (line != null) {
				LineControl lineControl = GetLineControl(line);
				if (lineControl != null) {
					if (lineControl.ActualHeight == 0) {
						// TODO: following logic does not account for very large choice points, in which case you want to
						// want to bring the branch into view if it's not on the screen, or you don't want to scroll
						// to bottom if that means the last line on the page will go out of view (because of a huge choice point)
						if (lineControl is BranchLine) {
							PageScroll.ScrollToBottom();
						}
						else if (currentPage.LineControls.Count > 0 && currentPage.LineControls[currentPage.LineControls.Count-1] == lineControl) {
							PageScroll.ScrollToBottom();
						}
						else {
							lineControl.BringIntoView();								
						}
					}
					
					// TODO: Doesn't work: (but does if you launch a message box before .Focus(), 
					// something about taking focus away from screen elements maybe?)
						
					//MessageBox.Show("try now");
					
					// TODO - if Focus is called, it works (but we want the textbox to be enabled.) If Dialogue.Focus() is called, it doesn't 
					// work, even though from debug statements I can see that SelectLine() is ran all the way through to the end.
					
					//Focus();
					
					
					FocusManager.SetFocusedElement(this,lineControl);//.Dialogue);
					
					//Dialogue.Focus();
					
					//TextBox dialogue = (TextBox)FindName("Dialogue");
					//dialogue.Focus();
				}
			}
			else {
				throw new ArgumentNullException("Tried to focus on a null line.");
			}
		}
		
		
		/// <summary>
		/// Get the line control on the current page view that represents a given line of conversation, if one exists.
		/// </summary>
		/// <param name="line">The line to fetch a control for</param>
		/// <returns>A LineControl object representing this line if one exists; null otherwise</returns>
		private LineControl GetLineControl(NWN2ConversationConnector line)
		{
			if (line == null) {
				return null;
			}
			
			foreach (LineControl c in currentPage.LineControls) {
				if (c.Nwn2Line == line) {
					return c;
				}
			}
						
			foreach (Control control in LinesPanel.Children) {
				ChoiceControl branch = control as ChoiceControl;
				if (branch != null) {
					foreach (LineControl c in branch.LineControls) {
						if (c.Nwn2Line == line) {
							return c;
						}
					}						
				}
			}			
			
			return null;			
		}
		
        #region Drag-drop

        bool mouseDownOnDraggable = false;
                
        
        private void lineControl_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDownOnDraggable && e.LeftButton == MouseButtonState.Pressed && !IsDragging)
            {
                Point position = e.GetPosition(null);

                if (Math.Abs(position.X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(position.Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                     StartDragInProcAdorner(e); 
                }
            }   
        }
        
        
        /// <summary>
        /// Check whether an element which has been clicked is valid for dragging.
        /// </summary>
        /// <param name="e">Event arguments from the preview mouse left button down event</param>
        /// <returns>True if the event source can be dragged; false otherwise</returns>
        private bool IsDraggable(MouseButtonEventArgs e)
        {        	
        	if (!(e.Source is BranchLine || e.Source is Line)) { // only these are draggable
        		return false;
        	}
        	
        	LineControl lineControl = (LineControl)e.Source;        	
//        	if (e.OriginalSource is EditableTextBlock && ((EditableTextBlock)e.OriginalSource).IsEditable) {
//        		return false; // don't drag if the user is actually selecting text
//        	}
        	if (e.OriginalSource is SwitchableTextBox && ((SwitchableTextBox)e.OriginalSource).IsEditable) {
        		return false; // don't drag if the user is actually selecting text
        	}
        	
        	return true;
        }

        
        private void lineControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        	_startPoint = e.GetPosition(null);       	
        	mouseDownOnDraggable = IsDraggable(e);
        }
        

        private void DragSource_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            e.UseDefaultCursors = false;
            e.Handled = true;
        }


        private LineControlDragAdorner _adorner = null;
        private AdornerLayer _layer;



        private FrameworkElement _dragScope;
        public FrameworkElement DragScope {
            get { return _dragScope; }
            set { _dragScope = value; }
        }


        private void StartDragInProcAdorner(MouseEventArgs e)
        {
            // Let's define our DragScope .. In this case it is every thing inside our main window .. 
            //DragScope = Application.Current.MainWindow.Content as FrameworkElement;
            DragScope = this.Content as FrameworkElement;
            System.Diagnostics.Debug.Assert(DragScope != null);

            // We enable Drag & Drop in our scope ...  We are not implementing Drop, so it is OK, but this allows us to get DragOver 
            bool previousDrop = DragScope.AllowDrop;
            DragScope.AllowDrop = true;            

            // Let's wire our usual events.. 
            // GiveFeedback just tells it to use no standard cursors..  

            GiveFeedbackEventHandler feedbackhandler = new GiveFeedbackEventHandler(DragSource_GiveFeedback);
            //this.DragSource.GiveFeedback += feedbackhandler;
            this.GiveFeedback += feedbackhandler;

            // The DragOver event ... 
            DragEventHandler draghandler = new DragEventHandler(Window1_DragOver);
            DragScope.PreviewDragOver += draghandler; 

            // Drag Leave is optional, but write up explains why I like it .. 
            DragEventHandler dragleavehandler = new DragEventHandler(DragScope_DragLeave);
            DragScope.DragLeave += dragleavehandler; 

            // QueryContinue Drag goes with drag leave... 
            QueryContinueDragEventHandler queryhandler = new QueryContinueDragEventHandler(DragScope_QueryContinueDrag);
            DragScope.QueryContinueDrag += queryhandler; 

            //Here we create our adorner..
            
            
            _adorner = new LineControlDragAdorner(DragScope,(LineControl)e.Source, true, 0.5);
            _layer = AdornerLayer.GetAdornerLayer(DragScope as Visual);
            _layer.Add(_adorner);


            IsDragging = true;
            _dragHasLeftScope = false; 
            //Finally lets drag drop 
            
            // Construct data object:
            DataObject dataObject = new DataObject(e.Source.GetType(),e.Source);
        	
        	
            DragDropEffects de = DragDrop.DoDragDrop(this, dataObject, DragDropEffects.Move);
            
             // Clean up our mess :) 
            DragScope.AllowDrop = previousDrop;
            AdornerLayer.GetAdornerLayer(DragScope).Remove(_adorner);
            _adorner = null;

            this.GiveFeedback -= feedbackhandler;
            DragScope.DragLeave -= dragleavehandler;
            DragScope.QueryContinueDrag -= queryhandler;
            DragScope.PreviewDragOver -= draghandler;  

            mouseDownOnDraggable = false;
            
            IsDragging = false;
        }
        

        private bool _dragHasLeftScope = false; 
        private void DragScope_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (this._dragHasLeftScope)
            {
                e.Action = DragAction.Cancel;
                e.Handled = true; 
            }
            
        }


        private void DragScope_DragLeave(object sender, DragEventArgs e)
        {
            if (e.OriginalSource == DragScope)
            {
                Point p = e.GetPosition(DragScope);
                Rect r = VisualTreeHelper.GetContentBounds(DragScope);
                if (!r.Contains(p))
                {
                    this._dragHasLeftScope = true;
                    e.Handled = true;
                }
            }

        } 




        private void Window1_DragOver(object sender, DragEventArgs args)
        {
            if (_adorner != null)
            {
                _adorner.LeftOffset = args.GetPosition(DragScope).X /* - _startPoint.X */ ;
                _adorner.TopOffset = args.GetPosition(DragScope).Y /* - _startPoint.Y */ ;
            }
        }

    

        private Point _startPoint;
        private bool _isDragging;

        public bool IsDragging
        {
            get { return _isDragging; }
            set { _isDragging = value; }
        } 
        
        
        private void lineControl_OnDrop(object sender, DragEventArgs e)
        {        	        
        	LineControl dropTarget = (LineControl)sender;
        	        	
        	LineControl dragSource;        	
        	if (e.Data.GetDataPresent(typeof(Line))) {
        		dragSource = (Line)e.Data.GetData(typeof(Line));
        	}
        	else if (e.Data.GetDataPresent(typeof(BranchLine))) {
        		dragSource = (BranchLine)e.Data.GetData(typeof(BranchLine));
        	}
        	else {
        		return;
        	}
        	
        	if (dragSource is BranchLine) {
        		if (dropTarget is BranchLine) { // re-ordering branches within a choice
	        		int dropTargetIndex = dropTarget.Nwn2Line.Parent.Line.Children.IndexOf(dropTarget.Nwn2Line);
	        		Conversation.CurrentConversation.MoveLineWithinChoice(dragSource.Nwn2Line,dropTargetIndex);
        		}
        	}
        	else if (dragSource is Line) {
        		if (dropTarget is Line) { // re-ordering lines on a page
        			Conversation.CurrentConversation.MoveLine(dragSource.Nwn2Line,dropTarget.Nwn2Line);
        		}
        		else if (dropTarget is LeadingLine) { // re-ordering lines on a page
        			Conversation.CurrentConversation.MoveLine(dragSource.Nwn2Line,dropTarget.Nwn2Line);
        		}
        		else if (dropTarget is BranchLine) { // making an ordinary line into a branch
        			Conversation.CurrentConversation.MoveLineIntoChoice(dragSource.Nwn2Line,dropTarget.Nwn2Line.Parent);
        		}
        	}
        	
        	e.Handled = true;
        }
        
        #endregion 
    }
}
		
			
			
			
			



