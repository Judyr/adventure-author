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
using System.Text;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using AdventureAuthor.Conversations.UI;
using AdventureAuthor.Core;
using AdventureAuthor.Core.UI;
using AdventureAuthor.Utils;
using AdventureAuthor.Variables.UI;
using crown = Crownwood.DotNetMagic.Controls;
using Crownwood.DotNetMagic.Docking;
using Crownwood.DotNetMagic.Common;
using NWN2Toolset;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.Factions;
using NWN2Toolset.NWN2.Data.Journal;
using NWN2Toolset.NWN2.Data.ConversationData;
using NWN2Toolset.NWN2.Data.Instances;
using NWN2Toolset.NWN2.Data.Templates;
using NWN2Toolset.NWN2.Data.Blueprints;
using NWN2Toolset.NWN2.IO;
using NWN2Toolset.NWN2.UI;
using NWN2Toolset.NWN2.Views;
using NWN2Toolset.Plugins;
using GlacialComponents.Controls.GlacialTreeList;
using OEIShared.Utils;
using OEIShared.IO.TwoDA;
using TD.SandBar;
using form = NWN2Toolset.NWN2ToolsetMainForm;
using win = System.Windows;

namespace AdventureAuthor.Setup
{		
	public static class Toolset
	{
		#region Global variables
		
		private static DockingManager dockingManager = null;		
		private static Crownwood.DotNetMagic.Controls.TabbedGroups tabbedGroupsCollection = null; 
		// owns the TabGroupLeaf which holds resource viewers
		private static Content chapterListContent = null;
		private static Zone leftZone = null;
		private static NWN2AreaContentsView areaContentsView = null;
		private static Dictionary<string,Dictionary<INWN2Object,GTLTreeNode>> dictionaries 
			= new Dictionary<string,Dictionary<INWN2Object,GTLTreeNode>>(14);
														   
		
		#endregion Global variables	
			

		
		private static bool temp2(Control c)
		{
			if (c == null) {
				return false;
			}
			
			
				ContextMenu menu = c.ContextMenu;
				
				if (menu != null) {
					foreach (MenuItem mi in menu.MenuItems) {
						if (mi.Text == "Properties (new window)") {
							Say.Debug(mi.ToString());
							Say.Debug("Owned by: " + c.ToString() + " of size " + menu.MenuItems.Count + ", menu is named " + menu.Name);
							c.ContextMenu = null;
//							mi.Click += delegate { Log.WriteMessage("RARG I WUR CLICKED (context menu)"); };
							return true;
						}
					}
				}
				
				return false;
		}
		
		
		
		private static MouseMode? previousMouseMode = null;
		
		
		
		/// <summary>
		/// Performs a myriad of modifications to the user interface at launch.
		/// </summary>
		internal static void SetupUI()
		{
			// Nullify every original context menu
//			foreach (Control c in GetControls(form.App)) {
//				if (c.ContextMenu != null) {
//					c.ContextMenu.MenuItems.Clear();
//				}
//				if (c.ContextMenuStrip != null) {
//					c.ContextMenuStrip.Items.Clear();
//				}
//				
//				
////				temp2(c);
////				if (temp2(c)) {
////					// found it
////					
////				}
//				
//			
////				c.ContextMenu = null;
////				c.ContextMenuStrip = null;
//			}
			
			//
			
			NWN2AreaViewer.MouseModeChanged += delegate
			{
				if (previousMouseMode != NWN2AreaViewer.MouseMode) {
					Log.WriteMessage("entered mode " + NWN2AreaViewer.MouseMode);
					previousMouseMode = NWN2AreaViewer.MouseMode;
				}
			};
			
			
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
					
//					List<Content> contents = new List<Content>(5);
//					
//					foreach (Content c in dockingManager.Contents) {
//						if (c.FullTitle == "Conversations") {
//							contents.Add(c);
//						}
//						else if (c.FullTitle == "Campaign Conversations") {
//							contents.Add(c);
//						}
//						else if (c.FullTitle == "Campaign Scripts") {
//							contents.Add(c);
//						}
//						else if (c.FullTitle == "Search Results") {
//							contents.Add(c);
//						}
//						else if (c.Control is NWN2VerifyOutputControl) {
//							contents.Add(c);
//						}
//					}		
//					
//					foreach (Content c in contents) {
//						if (c.Visible) {
//							dockingManager.HideContent(c);
//						}
//					}
					
															
//					// Lock the interface:
//					foreach (Content c in dockingManager.Contents) {	
//						c.HideButton = false;
//						c.CloseButton = false;
//					}					
				
					// Certain windows should be automatically hidden if the toolset ever tries to display them:
					dockingManager.ContentShown += new DockingManager.ContentHandler(HideContent);
				}
				
				// Hide the resource viewer controls:
				else if (fi.FieldType == typeof(crown.TabbedGroups)) {
					tabbedGroupsCollection = (crown.TabbedGroups)fi.GetValue(form.App);
					tabbedGroupsCollection.ActiveLeaf.TabPages.Inserted += new CollectionChange(ResourceViewerOpened);					
					tabbedGroupsCollection.ActiveLeaf.TabPages.Removing += new CollectionChange(ResourceViewerClosed);
										
					// Hide the close [X] control, change the appearance:
					SetupResourceViewersLeaf();					
					
					// When all resource viewers are closed, the resource viewers window (ActiveLeaf) is disposed 
					// and then created again later on; so on ActiveLeafChanged, we need to redo some stuff:
					tabbedGroupsCollection.ActiveLeafChanged += delegate 
					{ 
						SetupResourceViewersLeaf(); 
						tabbedGroupsCollection.ActiveLeaf.TabPages.Inserted += new CollectionChange(ResourceViewerOpened);					
						tabbedGroupsCollection.ActiveLeaf.TabPages.Removing += new CollectionChange(ResourceViewerClosed);
					};
				}
				
				else if (fi.FieldType == typeof(NWN2PropertyGrid)) {
					NWN2PropertyGrid grid = (NWN2PropertyGrid)fi.GetValue(form.App);
					grid.ValueChanged += delegate(object sender, NWN2PropertyValueChangedEventArgs e) { Log.WritePropertyChange(e); };
					grid.PreviewStateChanged += delegate { Log.WriteMessage("Preview state changed on property grid (??)"); };
				}
				
				else if (fi.FieldType == typeof(NWN2TerrainEditorForm)) {
					FieldInfo[] terrainEditorFields = typeof(NWN2TerrainEditorForm).GetFields(BindingFlags.NonPublic |
					                                                                          BindingFlags.Public |
					                                                                          BindingFlags.Instance);
					foreach (FieldInfo terrainEditorField in terrainEditorFields) {
						object o = terrainEditorField.GetValue((NWN2TerrainEditorForm)fi.GetValue(form.App));
						Control c = o as Control;
						if (c != null) {
							c.Click += delegate { 
								string extraInfo;
								if (c.Text != null && c.Text != String.Empty) {
									extraInfo = "(clicked " + c.Text + ")";
								}
								else {
									extraInfo = String.Empty;
								}
								Log.WriteAction(Log.Action.set,"terraineditorsettings",extraInfo);
							};
						}
					}
				}
				
				else if (fi.FieldType == typeof(NWN2TileView)) {
					NWN2TileView tileView = (NWN2TileView)fi.GetValue(form.App);
					
					foreach (FieldInfo f in typeof(NWN2TileView).GetFields(BindingFlags.NonPublic | BindingFlags.Instance)) {
						if (f.FieldType == typeof(NWN2TileTreeView)) {
							NWN2TileTreeView tileTreeView = (NWN2TileTreeView)f.GetValue(tileView);
							tileTreeView.SelectedIndexChanged += delegate(object source, GTSelectionChangedEventArgs e) 
							{  
								if (e.NewValue) {
									Log.WriteAction(Log.Action.selected,"tile",e.TreeNode.Text);
								}
							};
						}
						else if (f.FieldType == typeof(NWN2MetaTileTreeView)) {
							NWN2MetaTileTreeView metaTileTreeView = (NWN2MetaTileTreeView)f.GetValue(tileView);
							metaTileTreeView.SelectedIndexChanged += delegate(object source, GTSelectionChangedEventArgs e)
							{  
								if (e.NewValue) {
									Log.WriteAction(Log.Action.selected,"metatile",e.TreeNode.Text);
								}
							};
						}
					}
				}
				
				else if (fi.FieldType == typeof(NWN2BlueprintView)) {
					NWN2BlueprintView blueprintView = (NWN2BlueprintView)fi.GetValue(form.App);
					blueprintView.SelectionChanged += delegate(object sender, BlueprintSelectionChangedEventArgs e) 
					{  
						if (e.Selection.Length > 0 && e.Selection != e.OldSelection) {							
							StringBuilder message = new StringBuilder("selection: ");
							foreach (object o in e.Selection) {
								INWN2Blueprint blueprint = (INWN2Blueprint)o;
								message.Append(blueprint.Name + " (" + blueprint.ObjectType + ") ");
							}							
							Log.WriteAction(Log.Action.selected,"blueprint",message.ToString());
						}
					};
				}
				
				else if (fi.FieldType == typeof(NWN2AreaContentsView)) {
					areaContentsView = (NWN2AreaContentsView)fi.GetValue(form.App);
					
					// All this is necessary to identify the object type of the thing you're selecting:
					FieldInfo[] paletteFields = typeof(NWN2PaletteTreeView).GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
					FieldInfo dictionaryField = null;
					foreach (FieldInfo f in paletteFields) {
						if (f.FieldType == typeof(Dictionary<INWN2Object,GTLTreeNode>)) {
							dictionaryField = f;
							break;
						}
					}
					
					Dictionary<string,NWN2PaletteTreeView> paletteTreeViews = new Dictionary<string,NWN2PaletteTreeView>(14);
					paletteTreeViews.Add("waypoint",(NWN2PaletteTreeView)areaContentsView.Controls["panelWaypoints"].Controls["treeListWaypoints"]);
					paletteTreeViews.Add("trigger",(NWN2PaletteTreeView)areaContentsView.Controls["panelTriggers"].Controls["treeListTriggers"]);
					paletteTreeViews.Add("store",(NWN2PaletteTreeView)areaContentsView.Controls["panelStores"].Controls["treeListStores"]);
					paletteTreeViews.Add("camera",(NWN2PaletteTreeView)areaContentsView.Controls["panelStaticCameras"].Controls["treeListStaticCameras"]);
					paletteTreeViews.Add("sound",(NWN2PaletteTreeView)areaContentsView.Controls["panelSounds"].Controls["treeListSounds"]);
					paletteTreeViews.Add("placeable",(NWN2PaletteTreeView)areaContentsView.Controls["panelPlaceables"].Controls["treeListPlaceables"]);
					paletteTreeViews.Add("item",(NWN2PaletteTreeView)areaContentsView.Controls["panelItems"].Controls["treeListItems"]);
					paletteTreeViews.Add("encounter",(NWN2PaletteTreeView)areaContentsView.Controls["panelEncounters"].Controls["treeListEncounters"]);
					paletteTreeViews.Add("door",(NWN2PaletteTreeView)areaContentsView.Controls["panelDoors"].Controls["treeListDoors"]);
					paletteTreeViews.Add("creature",(NWN2PaletteTreeView)areaContentsView.Controls["panelCreatures"].Controls["treeListCreatures"]);
					paletteTreeViews.Add("tree",(NWN2PaletteTreeView)areaContentsView.Controls["panelTrees"].Controls["treeListTrees"]);
					paletteTreeViews.Add("light",(NWN2PaletteTreeView)areaContentsView.Controls["panelLights"].Controls["treeListLights"]);
					paletteTreeViews.Add("placedeffect",(NWN2PaletteTreeView)areaContentsView.Controls["panelPlacedEffects"].Controls["treeListPlacedEffects"]);
					paletteTreeViews.Add("environmentobject",(NWN2PaletteTreeView)areaContentsView.Controls["panelEnvironmentObjects"].Controls["treeListEnvironmentObjects"]);
					
					foreach (string k in paletteTreeViews.Keys) {
						string key = k;
						NWN2PaletteTreeView view = paletteTreeViews[key];						
						dictionaries.Add(key,(Dictionary<INWN2Object,GTLTreeNode>)dictionaryField.GetValue(view));
						
						view.SelectedIndexChanged += delegate(object source, GTSelectionChangedEventArgs e) 
						{  
							if (e.NewValue && dictionaries[key].ContainsValue(e.TreeNode)) {
								Log.WriteAction(Log.Action.selected,key,e.TreeNode.Text);
							}
						};
					}
				}
												
				// Get rid of the graphics preferences toolbar:
				else if (fi.FieldType == typeof(GraphicsPreferencesToolBar)) {
//					((GraphicsPreferencesToolBar)fi.GetValue(form.App)).Dispose();
				}
				
				// Prevent the object manipulation toolbar from being modified or moved:
				else if (fi.FieldType == typeof(TD.SandBar.ToolBar)) {
					TD.SandBar.ToolBar toolBar = fi.GetValue(form.App) as TD.SandBar.ToolBar;
					toolBar.AddRemoveButtonsVisible = false;
					toolBar.Movable = false;					
				}
								
				// Get rid of the "Filters:" label on the object manipulation toolbar:
				else if (fi.FieldType == typeof(LabelItem)) {
//					LabelItem labelItem = fi.GetValue(form.App) as LabelItem;
//					if (labelItem.Text == "Filters:") {
//						labelItem.Dispose();
//					}
				}
				
				// Get rid of "Show/Hide" and "Selection" menus on the object manipulation toolbar:
				else if (fi.FieldType == typeof(DropDownMenuItem)) {
//					DropDownMenuItem dropDownMenuItem = fi.GetValue(form.App) as DropDownMenuItem;
//					if (dropDownMenuItem.Text == "Show/Hide" ||
//					    dropDownMenuItem.Text == "Selection") {
//						dropDownMenuItem.Dispose();
//					}
				}
				
				// Get rid of "Snap", "Paint Spawn Point", "Create Transition..." 
				// and "Drag Selection" buttons on the object manipulation toolbar:
				else if (fi.FieldType == typeof(ButtonItem)) {
//					ButtonItem buttonItem = (ButtonItem)fi.GetValue(form.App);	
					
//
//					string[] dispose = new string[] {"Snap",
//													 "Paint Spawn Point",
//													 "Create Transition...",
//													 "Drag Selection"};
//													 
//					foreach (string text in dispose) {
//						if (buttonItem.Text == text) {
//							buttonItem.Dispose();					
//						}						
//					}									
//					if (buttonItem != null) {
//						buttonItem.Font = Adventure.ADVENTURE_AUTHOR_FONT;
//					}
				}				
				
				// Get rid of various menu items:
				else if (fi.FieldType == typeof(MenuButtonItem)) {
//					MenuButtonItem menuButtonItem = (MenuButtonItem)fi.GetValue(form.App);	
					
					
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
						SetupFileMenu(menuBarItem);
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
//					try {
//						// Contains a MenuBar, a GraphicsPreferencesToolbar and a ToolBar, plus the ToolBar i'm currently creating below.
//						
//						ToolBarContainer tbc = (ToolBarContainer)fi.GetValue(form.App);
//						if (tbc.Name == "topSandBarDock") {
//							TD.SandBar.ToolBar tb = new TD.SandBar.ToolBar();	
//							tb.AddRemoveButtonsVisible = false;
//							tb.AllowMerge = true;
//							
//							ButtonItem cw = new ButtonItem();
//							Bitmap b = new Bitmap(Path.Combine(Adventure.ImagesDir,"conversationwriter.bmp"));
//							cw.Image = b;	
//							cw.Text = "Conversation Writer";
//							cw.Activate += delegate { LaunchConversationWriter(); };
//							tb.Items.Add(cw);
//							
//							ButtonItem vm = new ButtonItem();
//							vm.Text = "Variable Manager";
//							vm.Activate += delegate { LaunchVariableManager(); };
//							tb.Items.Add(vm);
//							
//							tbc.Controls.Add(tb);
//						}
//					} catch (Exception e) {
//						Say.Error(e.ToString());
//					}
				}
			}
			
			// Create and show a chapter list window:
			CreateChapterList();
			UpdateChapterList();
			
			// Update title bar:
			UpdateTitleBar();
		}

		
		private static void ResourceViewerOpened(int index, object value)
		{
			// by the time the Removing event is called on the tabbed pages collection, the resource viewer has already been
			// disposed, which means we have the title of the tab page but not the resource type - setting the unused page.Text
			// property to the string name of the resource type means we have this information when we log the closing operation
			
			crown.TabPage page = (crown.TabPage)value;						
			INWN2Viewer viewer = page.Control as INWN2Viewer;
			if (viewer != null && viewer.ViewedResource != null) {
				if (viewer.ViewedResource is NWN2GameArea) {
					NWN2GameArea area = (NWN2GameArea)viewer.ViewedResource;
					Log.WriteAction(Log.Action.opened,"area",area.Name);
					page.Text = "area";
					
					// now handled through area contents viewer events, which picks up selection events from the area viewer as well:
					
//					NWN2AreaViewer areaViewer = viewer as NWN2AreaViewer;	
//					areaViewer.ElectronPanel.SelectionChanged += delegate 
//					{
//						if (areaViewer.SelectedInstances.Count > 0) {							
//							StringBuilder message = new StringBuilder("selection: ");
//							foreach (object o in areaViewer.SelectedInstances) {
//								INWN2Instance instance = (INWN2Instance)o;
//								message.Append(instance.Name + " (" + instance.ObjectType + ") ");
//							}							
//							Log.WriteAction(Log.Action.selected,"object",message.ToString());
//						}
//					};
				}
				else if (viewer.ViewedResource is NWN2GameConversation) {
					NWN2GameConversation conv = (NWN2GameConversation)viewer.ViewedResource;
					Say.Debug("Tried to open the original conversation editor - closing it again.");
					tabbedGroupsCollection.ActiveLeaf.TabPages.Remove(page); // old conversation editor so close	
					LaunchConversationWriter();
					WriterWindow.Instance.Open(conv.Name);
				}
				else if (viewer.ViewedResource is NWN2FactionData) {
					NWN2FactionData factions = (NWN2FactionData)viewer.ViewedResource;
					Log.WriteAction(Log.Action.opened,"factions");
					page.Text = "faction";
				}
				else if (viewer.ViewedResource is NWN2Journal) {
					//NWN2Journal journal = (NWN2Journal)viewer.ViewedResource;
					Log.WriteAction(Log.Action.opened,"journal");
					page.Text = "journal";
				}
				else if (viewer.ViewedResource is TwoDAFile) {
					TwoDAFile twodafile = (TwoDAFile)viewer.ViewedResource;
					Log.WriteAction(Log.Action.opened,"2dafile",twodafile.Name);
					page.Text = "2dafile";
				}
				else if (viewer.ViewedResource is NWN2GameScript) {
					NWN2GameScript script = (NWN2GameScript)viewer.ViewedResource;
					Log.WriteAction(Log.Action.opened,"script",script.Name);
					page.Text = "script";
				}
				else {
					string type = viewer.ViewedResource.GetType().ToString();
					Log.WriteAction(Log.Action.opened,type);
					page.Text = type;
				}
			}
		}
		
		
		private static void ResourceViewerClosed(int index, object value)
		{
			crown.TabPage page = (crown.TabPage)value;
			if (page.Text == null || page.Text == String.Empty) {
				Log.WriteAction(Log.Action.closed,"<resource>",page.Title);
			}
			else if (page.Text == "2dafile" || page.Text == "script" || page.Text == "area") {
				Log.WriteAction(Log.Action.closed,page.Text,page.Title);
			}
			else {
				Log.WriteAction(Log.Action.closed,page.Text);
			}
		}
		
		
		
		
		
		
		
		/// <summary>
		/// Clears the user interface after an Adventure has been closed.
		/// </summary>
		internal static void Clear()
		{
			tabbedGroupsCollection.RootSequence.Clear();
			form.App.BlueprintView.Module = null; 	  // stop displaying custom blueprints specific to the old module
			form.App.VerifyOutput.ClearVerifyOutput();  
			// Close all Property Grids - not implemented.
			form.App.CreateNewPropertyPanel(null, null, false, "Properties"); // create a fresh toolset Properties panel
		    UpdateChapterList();
			UpdateTitleBar();
		}
					
		internal static void UpdateTitleBar()
		{
			form.App.Text = "Adventure Author";
			if (NWN2ToolsetMainForm.App.Module != null && NWN2ToolsetMainForm.App.Module.LocationType == ModuleLocationType.Directory) {
				NWN2ToolsetMainForm.App.Text += ": " + NWN2ToolsetMainForm.App.Module.Name;
			}
		}	
				
		internal static void UpdateChapterList()
		{
			return;
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
		/// If a Verify window or the original conversation editor is displayed, hide it again.
		/// </summary>
		private static void HideContent(Content c, EventArgs ea)
		{
			if (c.Control is NWN2ConversationViewer) {
				dockingManager.HideContent(c);
			}
			if (form.App.VerifyContent.Visible) {
				dockingManager.HideContent(form.App.VerifyContent);
			}	
		}
		
		private static void CreateChapterList()
		{			
			return;
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
			return;
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
							
			MenuButtonItem newAdventure = new MenuButtonItem("New adventure");
			newAdventure.Activate += delegate { NewAdventureDialog(); };
			MenuButtonItem openAdventure = new MenuButtonItem("Open adventure");
			openAdventure.Activate += delegate { OpenAdventureDialog(); };
			MenuButtonItem saveAdventure = new MenuButtonItem("Save adventure");
			saveAdventure.Activate += delegate { SaveAdventureDialog(); };
//			MenuButtonItem saveAdventureAs = new MenuButtonItem("Save As");
//			saveAdventureAs.Activate += delegate { SaveAdventureAsDialog(); };
			MenuButtonItem runAdventure = new MenuButtonItem("Run adventure");
			runAdventure.Activate += delegate { RunAdventureDialog(); };
			MenuButtonItem closeAdventure = new MenuButtonItem("Close adventure");
			closeAdventure.Activate += delegate { CloseAdventureDialog(); };
							
			MenuButtonItem newChapter = new MenuButtonItem("New area");
			newChapter.Activate += delegate { NewChapterDialog(); };
			MenuButtonItem newConversation = new MenuButtonItem("Conversation writer");
			newConversation.Activate += delegate { LaunchConversationWriter(); };
			MenuButtonItem variableManager = new MenuButtonItem("Variable manager");
			variableManager.Activate += delegate { LaunchVariableManager(); };
			
//			MenuButtonItem changeUser = new MenuButtonItem("Change user");
//			changeUser.Activate += delegate { Say.Error("Not implemented yet."); };
			MenuButtonItem exitAdventureAuthor = new MenuButtonItem("Exit");
			exitAdventureAuthor.Activate += delegate { ExitToolsetDialog(); };
			MenuButtonItem logWindow = new MenuButtonItem("Display log output");
			logWindow.Activate += delegate { LogWindow window = new LogWindow(); window.Show(); };
			
			newChapter.BeginGroup = true;
			exitAdventureAuthor.BeginGroup = true;
			logWindow.BeginGroup = true;
						
			fileMenu.Items.AddRange( new MenuButtonItem[] {
			                           	newAdventure,
			                           	openAdventure,
			                           	saveAdventure,
//			                           	saveAdventureAs,
			                           	runAdventure,
			                           	closeAdventure,
			                           	newChapter,
			                           	newConversation,
			                           	variableManager,
//			                           	changeUser,
			                           	exitAdventureAuthor,
			                           	logWindow
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
			
			FolderBrowserDialog openFolder = new FolderBrowserDialog();
			openFolder.Description = "Choose a module (directory format only)";
			openFolder.RootFolder = System.Environment.SpecialFolder.MyDocuments;
			openFolder.ShowNewFolderButton = false;
			DialogResult result = openFolder.ShowDialog(form.App);
			if (result == DialogResult.OK) {
				try {
					string modulePath = openFolder.SelectedPath;
					string moduleName = Path.GetFileNameWithoutExtension(modulePath);
					string moduleIFOPath = Path.Combine(modulePath,"MODULE.IFO");
					string moduleAAPath = Path.Combine(modulePath,moduleName+".xml");
	        		if (!Directory.Exists(modulePath)) {
	        			throw new DirectoryNotFoundException("Could not find directory " + modulePath + ".");
	        		}
	        		else if (!File.Exists(moduleIFOPath)) {
	        			throw new FileNotFoundException(modulePath + " is not a valid NWN2 module. (Missing module.IFO)");
	        		}
					else if (!File.Exists(moduleAAPath)) {
						throw new FileNotFoundException(modulePath + " is not a valid Adventure Author module. (Missing modulename.XML)");
					}
					else {
						Adventure.Open(moduleName);
					}
				}
				catch (DirectoryNotFoundException e) {
					Say.Error(e);
				}
				catch (FileNotFoundException e) {
					Say.Error(e);
				}
			}			
		}
		
		
		private static void SaveAdventureDialog()
		{
			if (Adventure.CurrentAdventure == null) {
				Say.Error("Open an adventure first.");
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
				Say.Error("Open an adventure first.");
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
				Say.Error("Open an adventure first.");
				return;
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
				Say.Error("Open an adventure first.");
				return;
			}
			
			CreateChapter_Form chapterForm = new CreateChapter_Form();
			chapterForm.ShowDialog(form.App);
		}
		
		
//		public static void LaunchConversationWriterApplication()
//		{
//			AdventureAuthor.Conversations.UI.App.Main();
//		}
		
		
		/// <summary>
		/// Bring up the Conversation Writer window.
		/// <remarks>Note that a bug occurs where you can't enter text into LineControls if you use Show() rather than ShowDialog().</remarks>
		/// </summary>
		public static void LaunchConversationWriter() // Works
		{
			if (Adventure.CurrentAdventure == null) {
				Say.Error("Open an adventure first.");
				return;
			}
//			
//			LaunchConversationWriterApplication();
//			return;
			
			try {
				if (WriterWindow.Instance == null || !WriterWindow.Instance.IsLoaded) {
					WriterWindow.Instance = new WriterWindow();
					WriterWindow.Instance.ShowDialog();
					
//					win.Application.Current.Activated  += delegate { Say.Debug("Application.Current Activated. " + win.Application.Current.ToString()); };
//					win.Application.Current.Deactivated += delegate { Say.Debug("Application.Current Deactivated. " + win.Application.Current.ToString()); };
//					win.Application.Current.Exit += delegate { Say.Debug("Application.Current Exit. " + win.Application.Current.ToString()); };
//					win.Application.Current.LoadCompleted += delegate { Say.Debug("Application.Current LoadCompleted. " + win.Application.Current.ToString()); };
//					win.Application.Current.Startup += delegate { Say.Debug("Application.Current Startup. " + win.Application.Current.ToString()); };
				}
				
			}
			catch (Exception e) {
				Say.Error(e);
			}
		}	
		
		
		/// <summary>
		/// Bring up the Variable Manager window.
		/// </summary>
		public static void LaunchVariableManager() // Works
		{
			if (Adventure.CurrentAdventure == null) {
				Say.Error("Open an adventure first.");
				return;
			}
			
			try {
				if (VariablesWindow.Instance == null || !VariablesWindow.Instance.IsLoaded) {
					VariablesWindow.Instance = new VariablesWindow();
					VariablesWindow.Instance.ShowDialog();
				}
			}
			catch (Exception e) {
				Say.Error(e);
			}
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
