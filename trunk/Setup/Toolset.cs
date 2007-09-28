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
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using AdventureAuthor.Conversations.UI;
using AdventureAuthor.Core;
using AdventureAuthor.Core.UI;
using AdventureAuthor.Utils;
using AdventureAuthor.Variables.UI;
using Crownwood.DotNetMagic.Controls;
using Crownwood.DotNetMagic.Docking;
using NWN2Toolset;
using NWN2Toolset.NWN2.IO;
using NWN2Toolset.NWN2.UI;
using NWN2Toolset.NWN2.Views;
using NWN2Toolset.Plugins;
using OEIShared.Utils;
using TD.SandBar;
using form = NWN2Toolset.NWN2ToolsetMainForm;

namespace AdventureAuthor.Setup
{		
	public static class Toolset
	{
		#region Global variables
		
		private static DockingManager dockingManager = null;		
		private static Crownwood.DotNetMagic.Controls.TabbedGroups tabbedGroupsCollection = null; // owns the TabGroupLeaf which holds resource viewers		
		private static Content chapterListContent = null;
		private static Zone leftZone = null;
		
		#endregion Global variables	
			
		/// <summary>
		/// Clears the user interface after an Adventure has been closed.
		/// </summary>
		public static void Clear()
		{
			tabbedGroupsCollection.RootSequence.Clear();
			form.App.BlueprintView.Module = null; 	  // stop displaying custom blueprints specific to the old module
			form.App.VerifyOutput.ClearVerifyOutput();  
			// Close all Property Grids - not implemented.
			form.App.CreateNewPropertyPanel(null, null, false, "Properties"); // create a fresh toolset Properties panel
		    UpdateChapterList();
			UpdateTitleBar();
		}
					
		public static void UpdateTitleBar()
		{
			form.App.Text = "Adventure Author";
			if (NWN2ToolsetMainForm.App.Module != null && NWN2ToolsetMainForm.App.Module.LocationType == ModuleLocationType.Directory) {
				NWN2ToolsetMainForm.App.Text += ": " + NWN2ToolsetMainForm.App.Module.Name;
			}
		}	
				
		public static void UpdateChapterList()
		{
			// Clear the current list of chapters:
			ListView listView = (ListView)chapterListContent.Control;
			listView.Clear();
			
			if (Adventure.CurrentAdventure == null) {
				// Disable the chapter list context menu:
				foreach (MenuItem mi in chapterListContent.Control.ContextMenu.MenuItems) {
					mi.Enabled = false;
				}				
			}
			else {
				ListViewItem scratchpad = listView.Items.Add(Adventure.NAME_OF_SCRATCHPAD_AREA);
				scratchpad.ForeColor = Color.Maroon;			
				foreach (Chapter chapter in Adventure.CurrentAdventure.Chapters.Values) {
					listView.Items.Add(chapter.Name);					
				}				
				
				// Enable the chapter list context menu:
				foreach (MenuItem mi in chapterListContent.Control.ContextMenu.MenuItems) {
					if (mi.Text == "Add") {
						mi.Enabled = true;
						break;
					}
				}		
			}
		}
			
		/// <summary>
		/// If a Properties or Verify window is displayed, hide it again.
		/// </summary>
		private static void HideContent(Content c, EventArgs ea)
		{
			if (c.Control is NWN2PropertyGrid) {
				dockingManager.HideContent(c);
			}
			if (form.App.VerifyContent.Visible) {
				dockingManager.HideContent(form.App.VerifyContent);
			}			
		}
		
		private static void CreateChapterList()
		{			
			ListView listView = new ListView();
			listView.Width = 200;
			listView.MinimumSize = new Size(500,500);
			listView.GridLines = true;
			listView.BackColor = Color.AliceBlue;
			ColumnHeader ch = listView.Columns.Add("Name",200);
			
			listView.MultiSelect = false;
			listView.Font = Adventure.ADVENTURE_AUTHOR_FONT;
			listView.Alignment = ListViewAlignment.Default;
			listView.DoubleClick += new EventHandler(ChapterList_Open);
			chapterListContent = new Content(dockingManager,listView,"Chapters");		
			chapterListContent.HideButton = false; // lock interface
			chapterListContent.CloseButton = false; // lock interface
			
			ContextMenu menu = new ContextMenu();
					
			MenuItem viewItem = new MenuItem("Open");
			MenuItem addItem = new MenuItem("Add");
			MenuItem deleteItem = new MenuItem("Delete");
			
			viewItem.Enabled = false;
			addItem.Enabled = true;
			deleteItem.Enabled = false;
			
			addItem.Click += delegate { NewChapterDialog(); }; 
			deleteItem.Click += new EventHandler(ChapterList_Delete);
			viewItem.Click += new EventHandler(ChapterList_Open);			
					
			menu.MenuItems.Add(viewItem);
			menu.MenuItems.Add(addItem);
			menu.MenuItems.Add(deleteItem);
			
			listView.SelectedIndexChanged += new EventHandler(ChapterList_SelectedIndexChanged);			
			chapterListContent.Control.ContextMenu = menu;			
			
			dockingManager.Contents.Add(chapterListContent);				
			dockingManager.AddContentToZone(chapterListContent,leftZone,0);
			dockingManager.ShowContent(chapterListContent);	
		}
			
		/// <summary>
		/// Enable context menu options based on whether a Chapter, Scratchpad or nothing is currently selected.
		/// </summary>
		private static void ChapterList_SelectedIndexChanged(object sender, EventArgs ea)
		{	
			ListView list = (ListView)sender;
			
			// If there is no resource currently selected..:
			if (list.SelectedIndices.Count == 0) {
				foreach (MenuItem m in list.ContextMenu.MenuItems) {
					if (m.Text == "Add") { // only allow the user to Add a new Chapter
						m.Enabled = true;
					}
					else {
						m.Enabled = false;
					}
				}
			}
			// If the scratchpad has been selected..:
			else if (list.SelectedItems[0].Text == Adventure.NAME_OF_SCRATCHPAD_AREA) {
				foreach (MenuItem m in list.ContextMenu.MenuItems) {
					if (m.Text == "Add" | m.Text == "Open") { // only allow the user to Add or Open
						m.Enabled = true;
					}
					else {
						m.Enabled = false;
					}
				}					
			}
			// If an area has been selected..:
			else {
				foreach (MenuItem m in list.ContextMenu.MenuItems) {
					m.Enabled = true; // allow the user to Add, Delete or Open
				}						
			}
		}
		
		private static void SetupFileMenu(MenuBarItem fileMenu)
		{
			fileMenu.Items.Clear();
							
			MenuButtonItem newAdventure = new MenuButtonItem("New");
			newAdventure.Activate += delegate { NewAdventureDialog(); };
			MenuButtonItem openAdventure = new MenuButtonItem("Open");
			openAdventure.Activate += delegate { OpenAdventureDialog(); };
			MenuButtonItem saveAdventure = new MenuButtonItem("Save");
			saveAdventure.Activate += delegate { SaveAdventureDialog(); };
			MenuButtonItem saveAdventureAs = new MenuButtonItem("Save As");
			saveAdventureAs.Activate += delegate { SaveAdventureAsDialog(); };
			MenuButtonItem runAdventure = new MenuButtonItem("Run");
			runAdventure.Activate += delegate { RunAdventureDialog(); };
			MenuButtonItem closeAdventure = new MenuButtonItem("Close");
			closeAdventure.Activate += delegate { CloseAdventureDialog(); };
							
			MenuButtonItem newChapter = new MenuButtonItem("New Chapter");
			newChapter.Activate += delegate { NewChapterDialog(); };
			MenuButtonItem newConversation = new MenuButtonItem("Conversation Writer");
			newConversation.Activate += delegate { LaunchConversationWriter(); };
			MenuButtonItem newScript = new MenuButtonItem("Variables Manager");
			newScript.Activate += delegate {LaunchVariableManager(); };
			
			MenuButtonItem changeUser = new MenuButtonItem("Change User");
			changeUser.Activate += delegate { Say.Error("Not implemented yet."); };
			MenuButtonItem exitAdventureAuthor = new MenuButtonItem("Exit");
			exitAdventureAuthor.Activate += delegate { ExitToolsetDialog(); };
						
			fileMenu.Items.AddRange( new MenuButtonItem[] {
			                           	newAdventure,
			                           	openAdventure,
			                           	saveAdventure,
			                           	saveAdventureAs,
			                           	runAdventure,
			                           	closeAdventure,
			                           	newChapter,
			                           	newConversation,
			                           	newScript,
			                           	changeUser,
			                           	exitAdventureAuthor
			                           });			
		}
		
		/// <summary>
		/// Dispose the [X] control and perform other modifications to the resource viewers window.
		/// </summary>
		private static void SetupResourceViewersLeaf()
		{
			if (tabbedGroupsCollection.ActiveLeaf != null) {
				// Set font size:
				tabbedGroupsCollection.ActiveLeaf.TabControl.Font = Adventure.ADVENTURE_AUTHOR_FONT;
									
				// Dispose the [<], [>] and [X] controls:
				List<Control> controls = GetControls(tabbedGroupsCollection.ActiveLeaf.TabControl);
				for (int i = 0; i < 3; i++) {
					controls[i].Dispose(); //innocent
				}		
			}
		}
				
		
		/// <summary>
		/// Performs a myriad of modifications to the user interface at launch.
		/// </summary>
		internal static void SetupUI()
		{
			// Nullify every original context menu
			foreach (Control c in GetControls(form.App)) {
				c.ContextMenu = null;
				c.ContextMenuStrip = null;
			}	
			
			// Set preferences:
			NWN2ToolsetGeneralPreferences general = NWN2ToolsetPreferences.Instance.General;
			NWN2ToolsetGraphicsPreferences gfx = NWN2ToolsetPreferences.Instance.Graphics;			
			
			general.Autosave = false;			

			gfx.AmbientSound = false;			
			gfx.Bloom = true;
			gfx.DisplaySurfaceMesh = false;
			gfx.FarPlane = 1000.0F;
			gfx.Fog = false;
			gfx.UseAreaFarPlane = false;
			
			// Get every field of NWN2ToolsetMainForm.App:
			FieldInfo[] fields = form.App.GetType().GetFields(BindingFlags.Public |
															  BindingFlags.NonPublic |
			                                                  BindingFlags.Instance |
			                                                  BindingFlags.Static);				
			
			//TODO: Uncomment this if you want to get the original NWN2 interface:
//					DialogResult d = MessageBox.Show("Show original NWN2 layout?","Show all windows?",MessageBoxButtons.YesNo);
					DialogResult d = DialogResult.No;
					
			// Iterate through NWN2ToolsetMainForm fields and apply UI modifications:
			foreach (FieldInfo fi in fields) {		
				
				// Hide everything except for the area contents and blueprints, and add a chapter list:
				if (fi.FieldType == typeof(DockingManager)) {
					dockingManager = fi.GetValue(form.App) as DockingManager;
					
					//TODO: try iterating through dockingManager object, copying every field
					//except for Event ContextMenu, and then assigning it to the toolset (using a reflected method)
					
					// Lock the interface:
//					dockingManager.AllowFloating = false;
//					dockingManager.AllowRedocking = false;
//					dockingManager.AllowResize = false;
			
					// NB: Trying to hide specific objects that are already hidden (or show those
					// that are showing) seems to lead to an InvalidOperationException.
					dockingManager.ShowAllContents();
					
					// Hide everything except for the area contents and blueprints list:
//					foreach (Content c in dockingManager.Contents) {
//												
//						// Lock the interface:
//						c.HideButton = false;
//						c.CloseButton = false;
//						
//						if (d == DialogResult.Yes) {
//							dockingManager.ShowContent(c);
//						}
//						
//						if (c.Control is NWN2AreaContentsView) {
//							c.FullTitle = "Chapter Contents";
//							if (!c.Visible) {
//								dockingManager.ShowContent(c);
//								leftZone = c.ParentWindowContent.ParentZone;
//							}			
//						}
//						else if (c.Control is NWN2BlueprintView) {
//							if (!c.Visible) {
//								dockingManager.ShowContent(c);
//							}
//						}
//						else if (c.Control is NWN2TerrainEditorForm) {
//							if (!c.Visible) {
//								dockingManager.ShowContent(c);
//							}
//						}
//						else if (c.Control is NWN2TileView) {
//							if (!c.Visible) {
//								dockingManager.ShowContent(c);
//							}
//						}
//					}					
				
					// If the Properties or Verify window is displayed, hide it again immediately:
					dockingManager.ContentShown += new DockingManager.ContentHandler(HideContent);
				}
				
				// Hide the resource viewer controls:
				else if (fi.FieldType == typeof(TabbedGroups)) {
					tabbedGroupsCollection = (TabbedGroups)fi.GetValue(form.App);
					
					// Hide the close [X] control, change the appearance:
					SetupResourceViewersLeaf();					
					
					// When all resource viewers are closed, the resource viewers window (ActiveLeaf) is disposed 
					// and then created again later on; so on ActiveLeafChanged, reapply UI modifications:
					tabbedGroupsCollection.ActiveLeafChanged += delegate { SetupResourceViewersLeaf(); };
				}
				
				// Get rid of the graphics preferences toolbar:
				else if (fi.FieldType == typeof(GraphicsPreferencesToolBar)) {
					((GraphicsPreferencesToolBar)fi.GetValue(form.App)).Dispose();
				}
				
				// Prevent the object manipulation toolbar from being modified or moved:
				else if (fi.FieldType == typeof(TD.SandBar.ToolBar)) {
					TD.SandBar.ToolBar toolBar = fi.GetValue(form.App) as TD.SandBar.ToolBar;
					toolBar.AddRemoveButtonsVisible = false;
					toolBar.Movable = false;					
				}
								
				// Get rid of the "Filters:" label on the object manipulation toolbar:
				else if (fi.FieldType == typeof(LabelItem)) {
					LabelItem labelItem = fi.GetValue(form.App) as LabelItem;
					if (labelItem.Text == "Filters:") {
						labelItem.Dispose();
					}
				}
				
				// Get rid of "Show/Hide" and "Selection" menus on the object manipulation toolbar:
				else if (fi.FieldType == typeof(DropDownMenuItem)) {
					DropDownMenuItem dropDownMenuItem = fi.GetValue(form.App) as DropDownMenuItem;
					if (dropDownMenuItem.Text == "Show/Hide" ||
					    dropDownMenuItem.Text == "Selection") {
						dropDownMenuItem.Dispose();
					}
				}
				
				// Get rid of "Snap", "Paint Spawn Point", "Create Transition..." 
				// and "Drag Selection" buttons on the object manipulation toolbar:
				else if (fi.FieldType == typeof(ButtonItem)) {;
					ButtonItem buttonItem = (ButtonItem)fi.GetValue(form.App);	
					
					string[] dispose = new string[] {"Snap",
													 "Paint Spawn Point",
													 "Create Transition...",
													 "Drag Selection"};
													 
					foreach (string text in dispose) {
						if (buttonItem.Text == text) {
							buttonItem.Dispose();					
						}						
					}									
					if (buttonItem != null) {
						buttonItem.Font = Adventure.ADVENTURE_AUTHOR_FONT;
					}
				}				
				
				// Get rid of various menu items:
				else if (fi.FieldType == typeof(MenuButtonItem)) {
//					MenuButtonItem menuButtonItem = (MenuButtonItem)fi.GetValue(form.App);	
//					
//					string[] dispose = new string[] {"Create Transition...",
//													 "Paint Spawn Point",
//													 "Module &Properties",
//													 "&Journal",
//													 "&2DA File...",
//													 "Mode"
//													 };
//					foreach (string text in dispose) {
//						if (menuButtonItem.Text == text) {
//							menuButtonItem.Dispose();
//							continue;
//						}						
//					}
				}
				// Deal with MenuBarItems:
				else if (fi.FieldType == typeof(MenuBarItem)) {
					MenuBarItem menuBarItem = (MenuBarItem)fi.GetValue(form.App);
										
					if (menuBarItem.Text == "&File") {	
						if (d == DialogResult.No) {
							SetupFileMenu(menuBarItem);
						}
					}
//					else if (menuBarItem.Text == "&Edit") {
//						
//					}
//					else if (menuBarItem.Text == "&View") {
//						
//					}
//					else if (menuBarItem.Text == "&Window") {
//						
//					}
//					else if (menuBarItem.Text == "&Plugins") {
//						
//					}
//					else if (menuBarItem.Text == "&Help") {
//						
//					}
				}
				else if (fi.FieldType == typeof(ToolBarContainer)) {
					try {
						// Contains a MenuBar, a GraphicsPreferencesToolbar and a ToolBar, plus the ToolBar i'm currently creating below.
						
						ToolBarContainer tbc = (ToolBarContainer)fi.GetValue(form.App);
						if (tbc.Name == "topSandBarDock") {
							TD.SandBar.ToolBar tb = new TD.SandBar.ToolBar();	
							tb.AddRemoveButtonsVisible = false;
							tb.AllowMerge = true;
							
							ButtonItem cw = new ButtonItem();
							Bitmap b = new Bitmap(Path.Combine(Adventure.AdventureAuthorDir,"cwimage.bmp"));
							cw.Image = b;	
							cw.Text = "Conversation Writer";
							cw.Activate += delegate { LaunchConversationWriter(); };
							tb.Items.Add(cw);
							
							ButtonItem vm = new ButtonItem();
							vm.Text = "Variable Manager";
							vm.Activate += delegate { LaunchVariableManager(); };
							tb.Items.Add(vm);
							
							tbc.Controls.Add(tb);
						}
					} catch (Exception e) {
						Say.Error(e.ToString());
					}
				}
			}
			
			// Create and show a chapter list window:
			CreateChapterList();
			UpdateChapterList();
			
			// Update title bar:
			UpdateTitleBar();
			
			
//			
//								dockingManager.Dispose();
//					
//					DockingManager myDockingManager 
//						= new DockingManager(chapterListContent,
//						                     Crownwood.DotNetMagic.Common.VisualStyle.Office2007Black);
////					
////					myDockingManager.Contents = dockingManager.Contents;
////					myDockingManager.Factory = dockingManager.Factory;
////					myDockingManager.FeedbackStyle = dockingManager.FeedbackStyle;
////					myDockingManager.InnerControl = dockingManager.InnerControl;
////					myDockingManager.InnerMinimum = dockingManager.InnerMinimum;
////					myDockingManager.OuterControl = dockingManager.OuterControl;
//					
//					fi.SetValue(form.App,myDockingManager);
//					                                                     
//					dockingManager = fi.GetValue(form.App) as DockingManager;
			
			
			
		}
		
		/// <summary>
		/// Returns all controls that are owned by a given control
		/// </summary>
		/// <param name="parent">The control which owns the set of controls to search through</param>
		/// <returns>A list of controls of the specified type</returns>			
		private static List<Control> GetControls(Control parent)
		{
			List<Control> found = new List<Control>();			
			foreach (Control c in parent.Controls) {
				found.Add(c);
				found.AddRange(GetControls(c));
			}			
			return found;
		}
		
		/// <summary>
		/// Returns all controls of a particular type that are owned by a given control
		/// </summary>
		/// <param name="type">The type of control to return</param>
		/// <param name="parent">The control which owns the set of controls to search through</param>
		/// <returns>A list of controls of the specified type</returns>			
		private static List<Control> GetControls(Control parent, Type type)
		{
			List<Control> found = new List<Control>();			
			foreach (Control c in parent.Controls) {
				Type t = c.GetType();
				if (t.Equals(type))	{
					found.Add(c);
				}
				found.AddRange(GetControls(c,type));
			}			
			return found;
		}
						
		#region Event handlers
				
		private static void NewAdventureDialog()
		{
			if (Adventure.CurrentUser == null) {
				Say.Error("Log in to be able to create new adventures.");
			}
			else {
				NewAdventure_Form newAdventureForm = new NewAdventure_Form();
				newAdventureForm.ShowDialog(form.App);
			}
		}
		
		private static void OpenAdventureDialog()
		{
			if (Adventure.CurrentAdventure != null && !CloseAdventureDialog()) {
				return; // if they change their mind when prompted to close the current adventure
			}
			
			OpenAdventureWindow window = new OpenAdventureWindow();
			window.ShowDialog();
		}
		
		private static void SaveAdventureDialog()
		{
			if (Adventure.CurrentAdventure == null) {
				Say.Error("No Adventure is open to be saved.");
				return;
			}
			
			try {
				Adventure.CurrentAdventure.Save();
			}
			catch (InvalidOperationException e) {
				Say.Error(e);
			}
		}
		
		private static void SaveAdventureAsDialog()
		{		
			if (Adventure.CurrentAdventure == null) {
				Say.Error("No Adventure is open to be saved.");
				return;
			}
			
			try {
				NWN2SaveDirectoryDialog dialog = new NWN2SaveDirectoryDialog();
				if (dialog.ShowDialog(form.App) == DialogResult.OK) {					
					Adventure.CurrentAdventure.SaveAs(dialog.DirectoryName);
				}
			}
			catch (InvalidOperationException e) {
				Say.Error(e);
			}
		}
		
		private static void RunAdventureDialog()
		{
			if (Adventure.CurrentAdventure == null) {
				Say.Error("No Adventure is open to be run.");
			}
			
			RunAdventure_Form runAdventureForm = new RunAdventure_Form();
			runAdventureForm.ShowDialog(form.App);
		}		
		
		internal static bool CloseAdventureDialog()
		{
			if (Adventure.CurrentAdventure != null) {
				if (!Adventure.BeQuiet) {
					if (Adventure.CurrentAdventure != null) {
						switch (MessageBox.Show("Do you want to save the current Adventure?", 
						                        "Save?",
						                        MessageBoxButtons.YesNoCancel, 
						                        MessageBoxIcon.Exclamation)) {
							case DialogResult.Cancel:
								return false;		
								
							case DialogResult.Yes:
								Adventure.CurrentAdventure.Save();
								break;
						}
					}
				}
				Adventure.CurrentAdventure.Close();
			}
			return true;
		}
		
		private static void NewChapterDialog()
		{
			if (Adventure.CurrentAdventure == null) {
				Say.Error("Open or create an Adventure before trying to add a new Chapter.");
				return;
			}
			
			CreateChapter_Form chapterForm = new CreateChapter_Form();
			chapterForm.ShowDialog(form.App);
		}
		
		public static void LaunchConversationWriter()
		{
			if (Adventure.CurrentAdventure == null) {
				Say.Error("Open or create an Adventure before trying to add a new conversation.");
				return;
			}
			
			try {
				if (ConversationWriterWindow.Instance == null || !ConversationWriterWindow.Instance.IsLoaded) {
					ConversationWriterWindow.Instance = new ConversationWriterWindow();
				}
				ConversationWriterWindow.Instance.ShowDialog();
			}
			catch (ExecutionEngineException e) {
				Say.Error("ExecutionEngineException was thrown when running the ConversationWriterWindow.",e);
			}
		}
		
		public static void LaunchVariableManager()
		{
			if (VariablesWindow.Instance == null || !VariablesWindow.Instance.IsLoaded) {
				VariablesWindow.Instance = new VariablesWindow();
			}
			VariablesWindow.Instance.ShowDialog();
		}
		
		private static void DeleteChapterDialog(Chapter chapterToDelete)
		{			
			DialogResult d = MessageBox.Show("Are you sure you want to delete '" + chapterToDelete.Name + "'?",
			                                 "Delete chapter?", 
			                                 MessageBoxButtons.YesNo,
			                                 MessageBoxIcon.Question,
			                                 MessageBoxDefaultButton.Button2);
			
			if (d == DialogResult.Yes) {
				Adventure.CurrentAdventure.DeleteChapter(chapterToDelete);
			}
		}
				
		private static void ChapterList_Open(object sender, EventArgs ea)
		{	
			UserLog.Write(UserLog.Action.Opened,UserLog.Subject.Chapter);			
			
			try {
				string name = ((ListView)chapterListContent.Control).SelectedItems[0].Text;
				
				Chapter chapter = Adventure.CurrentAdventure.Chapters[name];
				if (chapter != null) {
					chapter.Open();
				}
				else if (name == Adventure.NAME_OF_SCRATCHPAD_AREA) {
					Adventure.CurrentAdventure.Scratch.Open();
				}
				else {	
					throw new KeyNotFoundException("'" + name + "' was not found in this Adventure.");				
				}
			}
			catch (KeyNotFoundException e) {
				Say.Error(e.Message,e);
			}
		}		
				
		private static void ChapterList_Delete(object sender, EventArgs ea)
		{	
			UserLog.Write(UserLog.Action.Deleted,UserLog.Subject.Chapter);
			
			string name = ((ListView)chapterListContent.Control).SelectedItems[0].Text;
			Chapter chapter = Adventure.CurrentAdventure.Chapters[name];
			if (chapter != null) {
				DeleteChapterDialog(chapter);
			}
		}			
		
		private static void ExitToolsetDialog()
		{
			if (!Adventure.BeQuiet && Adventure.CurrentAdventure != null && !CloseAdventureDialog()) {
				return; // cancel shutdown if they change their mind when asked to save the current adventure
			}
			
			ShutdownToolset();
			
			// TODO: should be adding event handling stuff to the OnClose(?) event, and then raising that event from here.
		}
		
		#endregion Event handlers
		
		private static void ShutdownToolset()
		{	
			IScriptCompiler scriptCompiler = null;
			int? threadID = null;
			NWN2PluginHost host = null;
			
			FieldInfo[] fields = form.App.GetType().GetFields(BindingFlags.NonPublic |
			                                                  BindingFlags.Instance |
			                                                  BindingFlags.Static);
			
			// Get the script compiler and thread ID through reflection 
			// (there is only one field of either type):
			foreach (FieldInfo fi in fields) {
				if (fi.FieldType == typeof(IScriptCompiler)) {
					scriptCompiler = fi.GetValue(form.App) as IScriptCompiler;
				}
				else if (fi.FieldType == typeof(Int32)) {
					threadID = (Int32)fi.GetValue(form.App);
				}
				else if (fi.FieldType == typeof(NWN2PluginHost)) {
					host = fi.GetValue(form.App) as NWN2PluginHost;
				}
			}	
				
			if (Thread.CurrentThread.ManagedThreadId == threadID) {
				form.App.Module.CloseModule();
				
				if (scriptCompiler != null) {
					scriptCompiler.Dispose();
					scriptCompiler = null;
				}
				//NWN2NetDisplayManager.Instance.Shutdown();
				host.UnloadPlugins();
				OEIShared.IO.TalkTable.TalkTable.Instance.Dispose();
				form.App.Module.Dispose(true);
				host.ShutdownPlugins();
				
				//Application.Exit();
				form.App.Close();
				form.App.Dispose();
			}
		}
	}
}
