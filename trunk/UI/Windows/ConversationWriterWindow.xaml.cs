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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.IO;
using AdventureAuthor;
using AdventureAuthor.AdventureData;
using AdventureAuthor.ConversationWriter;
using AdventureAuthor.UI.Controls;
using Microsoft.Win32;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.ConversationData;
using OEIShared.Utils;
using form = NWN2Toolset.NWN2ToolsetMainForm;

namespace AdventureAuthor.UI.Windows
{
    /// <summary>
    /// Interaction logic for ConversationWriterWindow.xaml
    /// </summary>

    public sealed partial class ConversationWriterWindow : Window
    {    
    	private static ConversationWriterWindow instance;    	
		public static ConversationWriterWindow Instance {
			get { return instance; }
			set { instance = (ConversationWriterWindow)value; }
		}
    	
		public ConversationWriterWindow()
		{
        	GraphScale = 0.5;
        	InitializeComponent();     	
		}
		
    	#region Fields
    	
    	private List<PageControl> pages;
    	public List<PageControl> Pages {
			get { return pages; }
		}
    	
    	private PageControl currentPage;    	
		public PageControl CurrentPage {
			get { return currentPage; }
		}
    	
    	private UserControl currentlySelectedControl;
		public UserControl CurrentlySelectedControl {
			get { return currentlySelectedControl; }
			set { currentlySelectedControl = value; }
		}
    	    	
    	private string originalFilename;    	
		public string OriginalFilename {
			get { return originalFilename; }
		}
    	
    	private string workingFilename;    	
		public string WorkingFilename {
			get { return workingFilename; }
		}
    	
    	private static DependencyProperty graphScaleProperty = 
    		DependencyProperty.Register("GraphScale",typeof(double),typeof(ConversationWriterWindow));
    	public double GraphScale
    	{
    		get { return (double)GetValue(graphScaleProperty); }
    		set { SetValue(graphScaleProperty,value); }
    	}
    	
    	#endregion Fields
    	        
        #region UI
        
		public void CreateButtonForSpeaker(string displayName, string tag, Brush colour)
		{
			Button button = new Button();
			button.Margin = new Thickness(2);
			TextBlock textBlock = new TextBlock();
			TextBlock tb1 = new TextBlock();
			TextBlock tb2 = new TextBlock();
			tb1.FontSize = 26;
			tb2.FontSize = 26;
			tb1.Foreground = colour;
			tb2.Foreground = Brushes.Black;
			tb1.Text = displayName.ToUpper();
			tb2.Text = " speaks";
			textBlock.Inlines.Add(tb1);
			textBlock.Inlines.Add(tb2);
			button.Content = textBlock;
			button.Click += delegate 
			{ 
				if (currentPage != null) {					
					NWN2ConversationConnector parentLine;
					LineControl selectedLine = currentlySelectedControl as LineControl;
					if (selectedLine != null && !selectedLine.IsPartOfBranch) {
						parentLine = selectedLine.Nwn2Line; // add a new line after the current one
					}
					else if (currentPage.LineControls.Count > 0) { // add a line to the end of the page
						parentLine = currentPage.LineControls[currentPage.LineControls.Count-1].Nwn2Line;
					}
					else { // add a line to the start of the page if there are no other lines
						parentLine = currentPage.LeadInLine; // may be null (for root)
					}
					NWN2ConversationConnector newLine = Conversation.CurrentConversation.AddLine(parentLine,tag,true);
					
					Conversation.CurrentConversation.SaveToWorkingCopy();
					DisplayPage(currentPage);
					RefreshDisplay(false);
					
					// TODO: Doesn't work: (but does if you launch a message box before .Focus(), something about taking focus away from screen elements maybe?)
					if (newLine != null) {
						foreach (LineControl lc in this.currentPage.LineControls) {
							if (lc.Nwn2Line == newLine) {
								lc.Dialogue.Focus();
							}
						}
					}				
					// TODO: Deal with ignoring links
				}
			};
			this.SpeakersButtonsPanel.Children.Add(button);
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
				throw new BadBranchException("Tried to create a branch from a null collection of lines.");
			}
			if (possibleLines.Count < 2) {
				throw new BadBranchException("Tried to create a branch with less than 2 lines.");
			}
			NWN2ConversationConnectorType speakerType = possibleLines[0].Type;
			
			// Create a LineControl for each line of dialogue in the branch:
			foreach (NWN2ConversationConnector line in possibleLines) {
				if (line.Type != speakerType) {
					throw new BadBranchException("Tried to create a branch with lines by both PC and NPC.");
				}
			}				
			BranchControl branchControl = new BranchControl(possibleLines);
			this.LinesPanel.Children.Add(branchControl);
			this.currentPage.FinalControl = branchControl;
		}
		
		private void DisplayEndOfConversation()
		{				
			EndConversationControl endOfConversation = new EndConversationControl();
			LinesPanel.Children.Add(endOfConversation);
			this.currentPage.FinalControl = endOfConversation;
		}
		
		public void DisplayPage(PageControl page)
		{			
			// Activate the page node in the graph, and deselect the current page node if one is selected:
			if (currentPage != null) {
				currentPage.IsSelected = false;
			}	
			page.IsSelected = true;
			
			// Clear the current page:	
			currentPage = page;		
			currentlySelectedControl = null;		
			LinesPanel.Children.Clear();
			currentPage.LineControls.Clear();
			currentPage.FinalControl = null;
			
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
		private List<PageControl> CreatePages(Conversation conversation)
		{
			List<PageControl> pages = new List<PageControl>(1);			
			PageControl rootPage = new PageControl(null,null);
			pages.Add(rootPage);
			
			// Identify every child of the root page:
			if (conversation.NwnConv.StartingList.Count == 0) {
				// do nothing; the conversation is empty, so just return the root page
			}
			else if (conversation.NwnConv.StartingList.Count > 1) {
				foreach (NWN2ConversationConnector line in conversation.NwnConv.StartingList) {
					PageControl page = new PageControl(line,rootPage);
					rootPage.Children.Add(page);
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
					PageControl page = new PageControl(line,rootPage);
					rootPage.Children.Add(page); // TODO: Refactor so that this happens on Page construction
					pages.Add(page);
				}
			}
			
			// Identify each Page that is a child of rootPage's children:
			foreach (PageControl child in rootPage.Children) {
				pages.AddRange(CreatePages(child));
			}
			
			rootPage.CalculateNumberOfEndNodes();			
			
			return pages;
		}
		
		private List<PageControl> CreatePages(PageControl parentPage)
		{
			List<PageControl> pages = new List<PageControl>();
			List<PageControl> pages2 = new List<PageControl>();
			
			NWN2ConversationConnector skippableLine = parentPage.LeadInLine;			
			while (skippableLine.Line.Children.Count == 1) {
				skippableLine = skippableLine.Line.Children[0];
			}
			
			foreach (NWN2ConversationConnector line in skippableLine.Line.Children) {
				PageControl child = new PageControl(line,parentPage);
				parentPage.Children.Add(child);
				pages.Add(child);				
			}
			
			foreach (PageControl child in pages) {
				pages2.AddRange(CreatePages(child));
			}
			
			pages.AddRange(pages2);
			return pages;
		}
		
		public void RemoveLineControl(NWN2ConversationConnector line)
		{
			foreach (Control control in LinesPanel.Children) {
				LineControl lineControl = control as LineControl;
				BranchControl branchControl = control as BranchControl;
				
				if (lineControl != null && lineControl.Nwn2Line == line) {
					CurrentPage.LineControls.Remove(lineControl);					
				}
				else if (branchControl != null) {
					LineControl toRemove = null;
					foreach (LineControl lc in branchControl.LineControls) {
						if (lc.Nwn2Line == line) {
							toRemove = lc;
							break;
						}
					}
					branchControl.LineControls.Remove(toRemove);
				}
			}
		}
				
		public void RefreshDisplay(bool conversationStructureChanged)
		{
			// If the structure of the page tree has changed, recreate the entire tree and reset the display:
			if (conversationStructureChanged) {
				NWN2ConversationConnector parentLineOfCurrentPage = currentPage.LeadInLine;
				PageControl newVersionOfCurrentPage = null;
						
				GraphCanvas.Children.Clear();	
				pages = CreatePages(Conversation.CurrentConversation);
				DrawConversationGraph();
								
				foreach (PageControl p in this.pages) {					
					if (p.LeadInLine == parentLineOfCurrentPage) {
						newVersionOfCurrentPage = p;
					}
				}			
						
				// If the currently viewed page still exists after recreating the page tree, display it again; otherwise, display root:
				Conversation.CurrentConversation.SaveToWorkingCopy();
				if (newVersionOfCurrentPage != null) {
					DisplayPage(newVersionOfCurrentPage);
				}
				else {
					DisplayPage(pages[0]);
				}	
			}
			else {
				Conversation.CurrentConversation.SaveToWorkingCopy();
				DisplayPage(currentPage);
			}
		}
		
		#endregion UI
					
		#region Event handlers
		
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
			
		public void OpenConversation(string name, bool createAsNew)
		{	
			// Checks:
			if (Adventure.CurrentAdventure == null) {
				Say.Error("Open an Adventure first.");
				return;
			}
			
			// TODO: these checks should now happen when you attach a conversation to a character, since we are currently not able to store OwningChapter or Owner:
//			else if (conversation.OwningChapter == null) {
//				Warning.Say("Warning - this conversation has not been assigned to a Chapter, so Adventure Author can't tell " +
//							"whether the conversation will work properly.");
//			}
//			else {
//				foreach (Speaker speaker in conversation.Speakers) {
//					if (!conversation.OwningChapter.ContainsSpeakingObject(speaker.Tag)) {
//						if (!conversation.OwningChapter.ContainsObject(speaker.Tag)) {
//							Warning.Say("Warning - Adventure Author couldn't find an object with tag " + speaker.Tag +
//								        ". The conversation will not work properly.");
//						}
//						else {
//							Warning.Say("Warning - the object with tag " + speaker.Tag + " exists, but may not be able to speak. " +
//							            "The conversation may not work as expected.");
//						}
//					}
//				}
//			}		
			
			// Close current conversation, if one is open:
			if (Conversation.CurrentConversation != null) {
				if (!CloseConversation(false)) { // try to close current conversation, return if Close is cancelled
					return;
				}
			}
			
			// Open new conversation:	
			NWN2GameConversation conv;
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
			this.pages = CreatePages(Conversation.CurrentConversation);
			
			// Build a graph based on the list of Pages:
			DrawConversationGraph();			
						
			// Allocate speakers unique colours and create a button for each:				
			Conversation.CurrentConversation.AddSpeaker(String.Empty,Conversation.PLAYER_NAME); // player has no tag, so pass String.Empty
			foreach (NWN2ConversationConnector line in Conversation.CurrentConversation.NwnConv.AllConnectors) {
				if (line.Speaker.Length > 0) {
					if (Conversation.CurrentConversation.GetSpeaker(line.Speaker) == null) { // note that line.Speaker is just a tag, not a Speaker object
						Speaker speaker = Conversation.CurrentConversation.AddSpeaker(line.Speaker);						
					}
				}					
			}
			foreach (Speaker speaker in Conversation.CurrentConversation.Speakers) {
				CreateButtonForSpeaker(speaker.DisplayName,speaker.Tag,speaker.Colour);
			}
						
			foreach (Button b in OtherActionsButtonsPanel.Children) {
				b.IsEnabled = true;
			}
			
			// Display the conversation from the root page:
			DisplayPage(pages[0]);
			
			this.Title += ": " + this.originalFilename;
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
			Random randomNumberGenerator = new Random();
			int randomNumber = randomNumberGenerator.Next();
			string tempFileNameWithoutExtension = originalFilenameWithoutExtension + "_" + randomNumber;
			string tempFileName = tempFileNameWithoutExtension + ".dlg";
			string tempPath = System.IO.Path.Combine(form.App.Module.Repository.DirectoryName,tempFileName);
			string originalPath = System.IO.Path.Combine(Adventure.CurrentAdventure.Module.Repository.DirectoryName,originalFilenameWithoutExtension+".dlg");
			File.Copy(originalPath,tempPath);
			return tempFileNameWithoutExtension;
		}		
		
		private void OnClick_Save(object sender, EventArgs ea)
		{	
			if (Conversation.CurrentConversation != null) {
				Conversation.CurrentConversation.SaveToOriginal();
			}			
			Say.Information("Saved.");
		}
		
		private void OnClick_Close(object sender, EventArgs ea)
		{
			if (Conversation.CurrentConversation != null) {
				CloseConversation(false);
			}
		}
		
		private void OnClick_Exit(object sender, EventArgs ea)
		{
			if (Conversation.CurrentConversation != null) {
				CloseConversation(false);
			}
			this.Close(); // picked up by a CancelEventHandler
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
				if (!CurrentPage.IsEndNode()) {
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
			LineControl lc = CurrentlySelectedControl as LineControl;
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
		
		private void MakeLineIntoBranch(NWN2ConversationConnector memberOfBranch)
		{
			Conversation.CurrentConversation.InsertNewLine(memberOfBranch.Parent,memberOfBranch.Speaker);
			Conversation.CurrentConversation.SaveToWorkingCopy();
			DisplayPage(CurrentPage);
			RefreshDisplay(true);			
		}
		
		internal void MakeBranchAtEndOfPage(NWN2ConversationConnector parentOfBranch, string speakerTag)
		{
			if (!CurrentPage.IsEndNode()) {
				throw new BadBranchException("Tried to add a branch at the end of a page that already had one.");
			}
			
			NWN2ConversationConnectorCollection children;
			if (parentOfBranch == null) {
				children = Conversation.CurrentConversation.NwnConv.StartingList;
			}
			else {
				children = parentOfBranch.Line.Children;
			}			
			
			if (children.Count > 0) { // shouldn't happen as we are at the end of the page
				NWN2ConversationConnectorCollection childrenToRemove = new NWN2ConversationConnectorCollection();
				foreach (NWN2ConversationConnector child in children) {
					if (child.Comment != Conversation.FILLER) {
						throw new Exception(); // if the children aren't filler lines, something has gone particularly wrong
					}		
					childrenToRemove.Add(child);
				}
				foreach (NWN2ConversationConnector child in childrenToRemove) {
					Conversation.CurrentConversation.NwnConv.RemoveNode(child);
				}
			}			
			
			// Insert the new lines, including a filler line if necessary:
			NWN2ConversationConnectorType newLineType;
			if (speakerTag == String.Empty) {
				newLineType = NWN2ConversationConnectorType.Reply;
			}
			else if (parentOfBranch == null) {
				newLineType = NWN2ConversationConnectorType.StartingEntry;
			}
			else {
				newLineType = NWN2ConversationConnectorType.Entry;
			}	
			
			if (Conversation.CanFollow(parentOfBranch,newLineType)) {
				Conversation.CurrentConversation.InsertNewLine(parentOfBranch,speakerTag);
				Conversation.CurrentConversation.InsertNewLine(parentOfBranch,speakerTag);
			}
			else {					
				NWN2ConversationConnector fillerLine = Conversation.CurrentConversation.InsertFillerLine(parentOfBranch);
				Conversation.CurrentConversation.InsertNewLine(fillerLine,speakerTag);
				Conversation.CurrentConversation.InsertNewLine(fillerLine,speakerTag);	
			}			
							
			Conversation.CurrentConversation.SaveToWorkingCopy();
			DisplayPage(CurrentPage);
			RefreshDisplay(true);			
		}		
		
		private void OnClick_AddAction(object sender, EventArgs ea)
		{
			
		}
		
		private void OnClosing(object sender, EventArgs ea)
		{
			if (Conversation.CurrentConversation != null) {
				CloseConversation(false);
			}	
		}
		
		
		#endregion Event handlers
		
		
		
//		private void deprecatedAddBranch(bool isChoice)
//		{
//			if (currentPage != null) {		
//				string speakerTag;
//				NWN2ConversationConnector parentLine = null;
//				if (ConversationWriterWindow.Instance.CurrentPage.IsEndNode()) { // if the page ends with END OF CONVERSATION
//					if (ConversationWriterWindow.Instance.CurrentPage.LineControls.Count > 0) { // add a line to the end of the page
//						parentLine = ConversationWriterWindow.Instance.CurrentPage.LineControls[ConversationWriterWindow.Instance.CurrentPage.LineControls.Count-1].Nwn2Line;
//					}
//					else { // add a line to the start of the page if there are no other lines
//						parentLine = ConversationWriterWindow.Instance.CurrentPage.LeadInLine; // may be null (for root)
//					}
//				}
//				else { // if the page ends with a choice/check already
//					if (ConversationWriterWindow.Instance.CurrentlySelectedControl == null) {
//						Say.Information("This page already ends with a choice or check. " + 
//						           "To add another one, first click on the line you would like it to follow, then click this button again.");
//						return;
//					}
//					else {
//						LineControl selected = ConversationWriterWindow.Instance.CurrentlySelectedControl as LineControl;
//						if (selected == null) {
//							// do nothing (?) this case would be if the BranchControl itself were selected
//							return;
//						}
//						else if (selected.IsPartOfBranch) {
//							Say.Information("You can't add a new choice/check inside an existing choice/check.");
//							return;
//						}
//						else {
//							parentLine = selected.Nwn2Line;
//						}
//					}
//				}
//				
//				if (isChoice) {
//					speakerTag = String.Empty;
//				}
//				else {
//					// TODO: Launch a dialog box asking to select one of the current speakers of the conversation to partake in Check.
//					// Temp:
//					speakerTag = Conversation.CurrentConversation.Speakers[Conversation.CurrentConversation.Speakers.Count-1].Tag;
//				}
//				
//				//Conversation.CurrentConversation.AddBranch(parentLine,speakerTag);
//										
//				Conversation.CurrentConversation.SaveToWorkingCopy();
//				DisplayPage(currentPage);
//				RefreshDisplay(true);
//			}
//		}
		
		private void DrawConversationGraph()
		{			
			PageControl root = pages[0];
			Point rootPoint = new Point(GraphCanvas.ActualWidth/2,Conversation.HEIGHT_FROM_TOP_OF_WINDOW);
			CreateNode(root,rootPoint);	
			CreateNodesForChildren(root);	
			GraphCanvas.ClipToBounds = true;	
		}
		
		private void CreateNodesForChildren(PageControl parent)
		{
			if (parent.Children.Count == 0) {
				return;
			}
			
			Point parentPoint = GetCentrePointOfNode(parent);
			double widthToDrawChildrenOver = (parent.CalculateNumberOfEndNodes() - 1) * Conversation.MINIMUM_WIDTH_BETWEEN_CHILDREN;
			double widthBetweenEachChild = widthToDrawChildrenOver / (parent.Children.Count - 1);
			double startingXCoordinate = parentPoint.X - (widthToDrawChildrenOver / 2);
			
			for (int i = 0; i < parent.Children.Count; i++) {
				PageControl child = parent.Children[i];
				double x = startingXCoordinate + (i * widthBetweenEachChild);
				double y = parentPoint.Y + Conversation.HEIGHT_BETWEEN_LEVELS;
				Point childPoint = new Point(x,y);
				PageControl childButton = CreateNode(child,childPoint);	
				DrawArrow(parent,childButton);
				CreateNodesForChildren(child);
			}
		}					
		
		private PageControl CreateNode(PageControl page, Point point)
		{
			page.Click += delegate {  // TODO: move to XAML
				Conversation.CurrentConversation.SaveToWorkingCopy();
				DisplayPage(page);
			};
			
			Canvas.SetLeft(page,point.X-(Conversation.UNSELECTED_NODE_OUTER_DIAMETER / 2));			              
			Canvas.SetTop(page,point.Y-(Conversation.UNSELECTED_NODE_OUTER_DIAMETER / 2));
			GraphCanvas.Children.Add(page);
			return page;
		}
				
		private Point GetCentrePointOfNode(PageControl page)
		{
			double left = Canvas.GetLeft(page);
			double top = Canvas.GetTop(page);
			
			Point point = new Point(left + Conversation.UNSELECTED_NODE_OUTER_DIAMETER / 2, top + Conversation.UNSELECTED_NODE_OUTER_DIAMETER / 2);
			return point;
		}
		
		
		private void DrawArrow(PageControl parent, PageControl child)
		{
			Point start = GetCentrePointOfNode(parent);
			Point end = GetCentrePointOfNode(child);
			
			Line myLine = new Line();
			GraphCanvas.Children.Add(myLine);
			myLine.Stroke = Brushes.Black;
			myLine.StrokeThickness = 3;
			myLine.X1 = start.X;
			myLine.Y1 = start.Y;
			myLine.X2 = end.X;
			myLine.Y2 = end.Y;
			Canvas.SetZIndex(myLine,1);
			
			// unfinished: two divergent lines to make an arrow shape
//			Line myLine2 = new Line();
//			GraphCanvas.Children.Add(myLine2);
//			myLine2.Stroke = Brushes.Black;
//			myLine2.StrokeThickness = 3;
//			myLine2.X1 = end.X;
//			myLine2.Y1 = end.Y;
//			myLine2.X2 = 
//			myLine2.Y2 = 
//			Canvas.SetZIndex(myLine2,-1);
//			
//			Line myLine3 = new Line();
//			GraphCanvas.Children.Add(myLine3);
//			myLine3.Stroke = Brushes.Black;
//			myLine3.StrokeThickness = 3;
//			myLine3.X1 = end.X;
//			myLine3.Y1 = end.Y;
//			myLine3.X2 =
//			myLine3.Y2 = 
//			Canvas.SetZIndex(myLine3,-1);
		}
		
		private double AngleBetweenPoints(Point q, Point r)		
		{
			double px1 = q.X;
			double py1 = q.Y;
			double px2 = r.X;
			double py2 = r.Y;				
			
			// Negate X and Y values:		
			double pxRes = px2 - px1;		
			double pyRes = py2 - py1;		
			double angle = 0.0;
			
			// Calculate the angle:		
			if (pxRes == 0.0) {		
				if (pxRes == 0.0) {		
					angle = 0.0;
				}
				else if (pyRes > 0.0) {
					angle = System.Math.PI / 2.0;
				}
				else {
					angle = System.Math.PI * 3.0 / 2.0;
				}
			}		
			else if (pyRes == 0.0) {		
				if (pxRes > 0.0) {
					angle = 0.0;
				}
				else {
					angle = System.Math.PI;
				}
			}		
			else {		
				if (pxRes < 0.0) {
					angle = System.Math.Atan(pyRes / pxRes) + System.Math.PI;
				}
				else if (pyRes < 0.0) {
					angle = System.Math.Atan(pyRes / pxRes) + (2 * System.Math.PI);				
				}
				else {
					angle = System.Math.Atan(pyRes / pxRes);
				}
			}
			
			// Convert to degrees:		
			angle = angle * 180 / System.Math.PI;
			return angle;		
		}
		
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="withoutSaving"></param>
		/// <returns>false if aborted, true otherwise</returns>
		public bool CloseConversation(bool withoutSaving)
		{
			if (!withoutSaving && !Adventure.BeQuiet) {
				MessageBoxResult result = MessageBox.Show("Save?", "Save this conversation?", MessageBoxButton.YesNoCancel);
				if (result == MessageBoxResult.Cancel) {
					return false;
				}
				else if (result == MessageBoxResult.Yes) {
					Conversation.CurrentConversation.SaveToOriginal();
				}
			}
						
			string workingFilePath = System.IO.Path.Combine(Adventure.CurrentAdventure.Module.Repository.DirectoryName,this.workingFilename+".dlg");
			File.Delete(workingFilePath);
			this.workingFilename = null;
			this.originalFilename = null;
			this.Title = "Conversation Writer";
			
			this.GraphCanvas.Children.Clear();
			this.SpeakersButtonsPanel.Children.Clear();
			foreach (Button b in OtherActionsButtonsPanel.Children) {
				b.IsEnabled = false;
			}
			this.LinesPanel.Children.Clear();
			
			currentPage = null;
			if (pages != null) {
				pages.Clear();
			}
			currentlySelectedControl = null;
			Conversation.CurrentConversation = null;
			
			return true;
		}
    }
}
		
			
			
			
			



