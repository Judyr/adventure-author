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
using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Xml;
using System.Xml.Serialization;
using AdventureAuthor.Conversations;
using AdventureAuthor.Conversations.UI;
using AdventureAuthor.Core;
using AdventureAuthor.Core.UI;
using AdventureAuthor.Tasks;
using AdventureAuthor.Tasks.NWN2;
using AdventureAuthor.Utils;
using AdventureAuthor.Variables.UI;
using AdventureAuthor.Ideas;
using AdventureAuthor.Evaluation;
using AdventureAuthor.Analysis;
using AdventureAuthor.Analysis.UI;
using Crownwood.DotNetMagic.Common;
using Crownwood.DotNetMagic.Docking;
using Crownwood.DotNetMagic.Controls;
using GlacialComponents.Controls.GlacialTreeList;
using NWN2Toolset;
using NWN2Toolset.Data;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.Blueprints;
using NWN2Toolset.NWN2.Data.Factions;
using NWN2Toolset.NWN2.Data.Journal;
using NWN2Toolset.NWN2.Data.Templates;
using NWN2Toolset.NWN2.IO;
using NWN2Toolset.NWN2.UI;
using NWN2Toolset.NWN2.Views;
using NWN2Toolset.NWN2.Wizards;
using NWN2Toolset.NWN2.Data.Instances;
using NWN2Toolset.Plugins;
using OEIShared.IO.TwoDA;
using OEIShared.Utils;
using TD.SandBar;
using System.IO;
using System.IO.Pipes;
using crown = Crownwood.DotNetMagic.Controls;
using form = NWN2Toolset.NWN2ToolsetMainForm;
using AdventureAuthor.Ideas;

namespace AdventureAuthor.Setup
{		
	public static partial class Toolset
	{
		#region Properties and fields
		
		/// <summary>
		/// The Adventure Author plugin. 
		/// </summary>
		private static AdventureAuthorPlugin plugin = null;
		public static AdventureAuthorPlugin Plugin {
			get { return plugin; }
			internal set { plugin = value; }
		}
		
		
		/// <summary>
		/// The docking manager controls all docked controls on the main interface screen
		/// </summary>
		private static DockingManager dockingManager = null;
		internal static DockingManager DockingManager {
			get { return dockingManager; }
		}
		
		
		/// <summary>
		/// Owns the TabGroupLeaf which holds resource viewers
		/// </summary>
		private static TabbedGroups tabbedGroupsCollection = null; 
		internal static TabbedGroups TabbedGroupsCollection {
			get { return tabbedGroupsCollection; }
		}
		
		
		/// <summary>
		/// The area contents view control, which lists every object in the current game area
		/// </summary>
		private static NWN2AreaContentsView areaContentsView = null;
		
		
		/// <summary>
		/// The blueprints view control, which lists every available blueprint
		/// </summary>
		private static NWN2BlueprintView blueprintView = null;
		
		
		private static NWN2ModuleAreaList areaList = null;
		private static NWN2ModuleConversationList conversationList = null;
		private static NWN2ModuleScriptList scriptList = null;
			
		
		private static GraphicsPreferencesToolBar graphicsToolbar = null;
		private static TD.SandBar.ToolBar objectsToolbar = null;
		private static MenuBarItem fileMenu = null;
		
		
		/// <summary>
		/// A list of all the game object dictionaries, one for each type of object (e.g. creature, door, placeable etc.)
		/// </summary>
		private static Dictionary<string,Dictionary<INWN2Object,GTLTreeNode>> dictionaries 
			= new Dictionary<string,Dictionary<INWN2Object,GTLTreeNode>>(14);	
		
		
		/// <summary>
		/// The mouse mode the user was previously in (e.g. Select Objects) - so that we only log genuine changes to the mouse mode
		/// </summary>
		private static MouseMode? previousMouseMode = null;	
		
		#endregion
			
			
		#region Events
		
		public static event EventHandler<MagnetEventArgs> MagnetSubmitted;		
		private static void OnMagnetSubmitted(MagnetEventArgs e)
		{
			EventHandler<MagnetEventArgs> handler = MagnetSubmitted;
			if (handler != null) {
				handler(null,e);
			}
		}
		
		#endregion
		
		
		
		/// <summary>
		/// Performs a myriad of modifications to the user interface at launch.
		/// </summary>
		internal static void SetupUI()
		{
			#region Mouse mode changes
			
			NWN2AreaViewer.MouseModeChanged += delegate
			{
				if (previousMouseMode != NWN2AreaViewer.MouseMode) {
					Log.WriteAction(LogAction.mode,"toolset_" + NWN2AreaViewer.MouseMode);
					previousMouseMode = NWN2AreaViewer.MouseMode;
				}
			};
			
			#endregion
			
			#region Setting general/graphics preferences
			
			// Set preferences:
			NWN2ToolsetGeneralPreferences general = NWN2ToolsetPreferences.Instance.General;
			NWN2ToolsetGraphicsPreferences gfx = NWN2ToolsetPreferences.Instance.Graphics;			
			
			general.Autosave = false;			
			general.AllowPlugins = PluginSecurityPreference.All;
			
			gfx.AmbientSound = false;			
			gfx.Bloom = true;
			gfx.DisplaySurfaceMesh = false;
			gfx.FarPlane = 1000.0F;
			gfx.Fog = false;
			gfx.UseAreaFarPlane = false;
			
			#endregion
			
						
			// Get every field of NWN2ToolsetMainForm.App:
			FieldInfo[] fields = form.App.GetType().GetFields(BindingFlags.Public |
															  BindingFlags.NonPublic |
			                                                  BindingFlags.Instance |
			                                                  BindingFlags.Static);	
			
					
			// Iterate through NWN2ToolsetMainForm fields and apply UI modifications:
			foreach (FieldInfo fi in fields) {	
				
				if (fi.FieldType == typeof(DockingManager)) {
					dockingManager = (DockingManager)fi.GetValue(form.App);					
										
					// NB: Trying to hide specific objects that are already hidden (or show those
					// that are showing) seems to lead to an InvalidOperationException.
					dockingManager.ShowAllContents();					
					
					List<Content> contents = new List<Content>(5);
					
					foreach (Content c in dockingManager.Contents) {
						if (c.Control is NWN2ModuleAreaList) {
							areaList = (NWN2ModuleAreaList)c.Control;
						}
						else if (c.Control is NWN2ModuleConversationList) {
							contents.Add(c);
//							if (c.FullTitle == "Conversations") {
//								conversationList = (NWN2ModuleConversationList)c.Control;
//							}
//							else { // hide "Campaign Conversations"
//								contents.Add(c);
//							}
						}
						else if (c.Control is NWN2ModuleScriptList) {
							if (c.FullTitle == "Scripts") {
								scriptList = (NWN2ModuleScriptList)c.Control;
							}
							else { // hide "Campaign Scripts"
								contents.Add(c);
							}
						}
						else if (c.FullTitle == "Search Results") {
							contents.Add(c);
						}
						else if (c.Control is NWN2VerifyOutputControl) {
							contents.Add(c);
						}					
					}		
					
					foreach (Content c in contents) {
						if (c.Visible) {
							dockingManager.HideContent(c);
						}
					}			
				
					// Certain windows should be automatically hidden if the toolset ever tries to display them:
					dockingManager.ContentShown += new DockingManager.ContentHandler(OnContentShown);
				}
				
				// Hide the resource viewer controls:
				else if (fi.FieldType == typeof(crown.TabbedGroups)) {
					tabbedGroupsCollection = (crown.TabbedGroups)fi.GetValue(form.App);
					tabbedGroupsCollection.ActiveLeaf.TabPages.Inserted += new CollectionChange(ResourceViewerOpened);					
					tabbedGroupsCollection.ActiveLeaf.TabPages.Removing += new CollectionChange(ResourceViewerClosed);
										
					// Hide the close [X] control, change the appearance:
					//SetupResourceViewersLeaf(); //no longer necessary		
					
					// When all resource viewers are closed, the resource viewers window (ActiveLeaf) is disposed 
					// and then created again later on; so on ActiveLeafChanged, we need to redo some stuff:
					tabbedGroupsCollection.ActiveLeafChanged += delegate 
					{ 
						//SetupResourceViewersLeaf(); //no longer necessary	
						tabbedGroupsCollection.ActiveLeaf.TabPages.Inserted += new CollectionChange(ResourceViewerOpened);					
						tabbedGroupsCollection.ActiveLeaf.TabPages.Removing += new CollectionChange(ResourceViewerClosed);
					};
				}
				
				else if (fi.FieldType == typeof(NWN2PropertyGrid)) {
					NWN2PropertyGrid grid = (NWN2PropertyGrid)fi.GetValue(form.App);
					WatchForChanges(grid);
				}
				
				// Replace the 'Add' item on the area list context menu, to block users from working with temporary modules:
				else if (fi.FieldType == typeof(NWN2ModuleAreaList)) {
					NWN2ModuleAreaList areaList = (NWN2ModuleAreaList)fi.GetValue(form.App);
					foreach (Control c in areaList.Controls) {
						if (c is ListView && c.ContextMenu != null) {
							MenuItem removable = null;							
							foreach (MenuItem mi in c.ContextMenu.MenuItems) {
								if (mi.Text == "Add") {
									removable = mi;
								}
							}
							
							if (removable != null) {
								c.ContextMenu.MenuItems.Remove(removable);
								MenuItem item = new MenuItem("Add");
								item.Click += delegate { NewAreaDialog(); };
								c.ContextMenu.MenuItems.Add(1,item);								
								c.ContextMenu.Popup += delegate 
								{  
									ListView listView = (ListView)c;
									foreach (MenuItem mi in c.ContextMenu.MenuItems) {
										if (mi.Text == "Add") {
											if (listView.SelectedIndices.Count == 0) {
												mi.Enabled = false;
											}
											else {
												mi.Enabled = true;
											}
											return;
										}
									}
								};
							}
						}
					}					
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
								Log.WriteAction(LogAction.set,"terraineditorsettings",extraInfo);
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
									Log.WriteAction(LogAction.selected,"tile",e.TreeNode.Text);
								}
							};
						}
						else if (f.FieldType == typeof(NWN2MetaTileTreeView)) {
							NWN2MetaTileTreeView metaTileTreeView = (NWN2MetaTileTreeView)f.GetValue(tileView);
							metaTileTreeView.SelectedIndexChanged += delegate(object source, GTSelectionChangedEventArgs e)
							{  
								if (e.NewValue) {
									Log.WriteAction(LogAction.selected,"metatile",e.TreeNode.Text);
								}
							};
						}
					}
				}
				
				else if (fi.FieldType == typeof(NWN2BlueprintView)) {
					try {
						blueprintView = (NWN2BlueprintView)fi.GetValue(form.App);						
						blueprintView.MultiSelect = false;
						
						// Users can send blueprints to their ideas list. NB: For some reason adding this
						// to each NWN2PaletteTreeView you find will lead to all of them having many
						// copies of this MenuItem, so just add it to the first one and break:
						List<Control> controls = GetControls(blueprintView);
						foreach (Control c in controls) {
							if (c is NWN2PaletteTreeView) {
								NWN2PaletteTreeView pt = (NWN2PaletteTreeView)c;								
								if (c.ContextMenu != null && c.ContextMenu.MenuItems.Count > 0) {	
									// NB: Unable to correctly enable and disable this menu item depending on 
									// whether an actual blueprint (as opposed to a blueprint category like
									// Animals) has been selected, because the SelectionChanged event doesn't
									// seem to fire on right-click, only left-click, so it's inconsistent.
									// However, it does send the correct selection when you right-click to
									// select it, as well as when you left-click to select it.
									MenuItem separator = new MenuItem("-");
									MenuItem addAsIdea = new MenuItem("Add to Magnet Box");
									addAsIdea.Click += new EventHandler(BlueprintSentToIdeas);
									c.ContextMenu.MenuItems.Add(separator);
									c.ContextMenu.MenuItems.Add(addAsIdea);
									break;
								}								
							}
						}
						
						blueprintView.SelectionChanged += delegate(object sender, BlueprintSelectionChangedEventArgs e) 
						{  
							if (e.Selection.Length > 0 && e.Selection != e.OldSelection) {							
								StringBuilder message = new StringBuilder("selection: ");
								foreach (object o in e.Selection) {
									INWN2Blueprint blueprint = (INWN2Blueprint)o;
									message.Append(blueprint.Name + " (" + blueprint.ObjectType + ") ");
								}							
								Log.WriteAction(LogAction.selected,"blueprint",message.ToString());
							}
						};
					}
					catch (Exception e) {
						Say.Error(e);
					}
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
								Log.WriteAction(LogAction.selected,key,e.TreeNode.Text);
							}
						};
					}
										
					// TODO: Fully implement this below (sending instances as Ideas)
					
					
					// Users can send objects to their ideas list. NB: For some reason adding this
					// to each NWN2PaletteTreeView you find will lead to all of them having many
					// copies of this MenuItem, so just add it to the first one and break:
//					NWN2PaletteTreeView pt = paletteTreeViews["creature"];
//					if (pt.ContextMenu != null && pt.ContextMenu.MenuItems.Count > 0) {
//						MenuItem separator = new MenuItem("-");
//						MenuItem addAsIdea = new MenuItem("Add to Magnet Box");
//						addAsIdea.Click += delegate { 
//							if (areaContentsView.AreaViewer != null) {
//								foreach (
//								
//								foreach (NWN2tem instance in areaContentsView.AreaViewer.SelectedInstances) {
//									string text = instance.Name + "\n" + instance.ObjectType.ToString() + "\n(Tag '" +
//										instance.ObjectID.ToString();									
//									Idea idea = new Idea(text,IdeaCategory.Resources);
//									OnIdeaSubmitted(new IdeaEventArgs(idea));									
//								}
//							}
//						};
//						pt.ContextMenu.MenuItems.Add(separator);
//						pt.ContextMenu.MenuItems.Add(addAsIdea);
//					}						
				}
												
				// Get rid of the graphics preferences toolbar:
				else if (fi.FieldType == typeof(GraphicsPreferencesToolBar)) {
					graphicsToolbar = (GraphicsPreferencesToolBar)fi.GetValue(form.App);
					graphicsToolbar.Dispose();
				}
				
				else if (fi.FieldType == typeof(TD.SandBar.ToolBar)) {
					TD.SandBar.ToolBar toolbar = (TD.SandBar.ToolBar)fi.GetValue(form.App);
					if (toolbar.Text == "Object Manipulation") {
						objectsToolbar = toolbar;
					}
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
						fileMenu = menuBarItem;
						SaveModuleAsDialog(menuBarItem);
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
				
				//TODO - this is apparently the correct way to do it but nothing appears
//				else if (fi.FieldType == typeof(TD.SandBar.SandBarManager)) {
//					Say.Information("Found SandBarManager");
//					try {
//						SandBarManager manager = (SandBarManager)fi.GetValue(form.App);
//						
//						TD.SandBar.ToolBar aaToolbar = new TD.SandBar.ToolBar();
//						SetupAdventureAuthorToolBar(aaToolbar);
//						
//						TD.SandBar.ToolBar ideaToolbar = new TD.SandBar.ToolBar();
//						SetupAddIdeaToolBar(ideaToolbar);
//						
//						manager.AddToolbar(aaToolbar);
//						manager.AddToolbar(ideaToolbar);manager.
//					}
//					catch (Exception e) {
//						Say.Error(e);
//					}
//				}
				
				else if (fi.FieldType == typeof(ToolBarContainer)) {					
					try {
						// Contains a MenuBar, a GraphicsPreferencesToolbar and a ToolBar, 
						// plus the ToolBar i'm currently creating below.						
						ToolBarContainer tbc = (ToolBarContainer)fi.GetValue(form.App);
						if (tbc.Name == "topSandBarDock") {
							aaToolbar = new TD.SandBar.ToolBar();
							SetupAdventureAuthorToolBar(aaToolbar);
							tbc.Controls.Add(aaToolbar);
						}
					} 
					catch (Exception e) {
						Say.Error(e.ToString());
					}
				}
			}
			
			ModuleHelper.ModuleSaved += new EventHandler(ModuleHelper_ModuleSaved);
			ModuleHelper.ModuleOpened += new EventHandler(ModuleHelper_ModuleOpened);
			ModuleHelper.ModuleClosed += new EventHandler(ModuleHelper_ModuleClosed);
			
			// If a MagnetSubmitted event is raised, that magnet should now be
			// sent to the Magnets application (if it can be found):
			MagnetSubmitted += delegate(object sender, MagnetEventArgs e) 
			{  
				SendMagnet(e.Magnet);
			};
			
			// Listen for My Tasks requesting task suggestions:
			StartListeningForMessages();
			
			// Set up the interface for initial use - update the title bar, and disable
			// parts of the interface which require an open module to be useful.
			UpdateTitleBar();		
			SetUI_ModuleNotOpen();
			
			// Lock or unlock the interface depending on user settings, and handle
			// locking/unlocking the interface when user settings change in future.
            Plugin.Options.PropertyChanged += UpdateLockedStatus;            
			SetInterfaceLock(Plugin.Options.LockInterface);
			areaContentsView.BringToFront();
			areaContentsView.Focus();
		}
				
		
		private const string NWN2TOMYTASKS = "mytasksinpipe";
		private const string MYTASKSTONWN2 = "mytasksoutpipe";
		private const string REQUESTAVAILABLECRITERIA = "Send available criteria for task generation.";	
		private const string REQUESTALLTASKS = "Send all tasks, without filtering by criteria.";	
		private const string CRITERIAFOLLOW = "<<<Criteria follow>>>"; 
		
		
    	private static void StartListeningForMessages()
    	{   
    		ThreadStart threadStart = new ThreadStart(ListenForMessagesFromMyTasks);
    		Thread thread = new Thread(threadStart);
    		thread.IsBackground = true; // will not prevent the application from closing down
			thread.Priority = ThreadPriority.BelowNormal;
    		thread.Start();
    	}
    	
		
		private static void ListenForMessagesFromMyTasks()
    	{
    		using (NamedPipeClientStream client = new NamedPipeClientStream(".",MYTASKSTONWN2,PipeDirection.In))
    		{
    			try {
    				client.Connect();
    				
	    			using (StreamReader reader = new StreamReader(client))
	    			{
	    				string message;
	    				while ((message = reader.ReadToEnd()) != null) {
	    					if (message.Length == 0) {	    						
	    						break;
	    					}
	    					
	    					if (message.StartsWith(REQUESTALLTASKS)) {
	    						
	    						List<Task> tasks = GetTasks();	    						
	    						
	    						XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Task>));
	    						StringWriter stringWriter = new StringWriter();
	    						xmlSerializer.Serialize(stringWriter,tasks);
	    						string serialisedTasks = stringWriter.GetStringBuilder().ToString();
	    						
	    						SendMessageToMyTasks(serialisedTasks);
		    				}
	    					else if (message.Length > 0) {
	    						Say.Information("NWN2 received:\n" + message);
		    				}	    						
	    				}
	    			}
	    		}
    			catch (Exception e) {
    				Say.Error("Failed to connect.",e);
    			}    			
			}    		
			ListenForMessagesFromMyTasks();
    	}
		
		
		private static void SendRandomMessageToMyTasks(object sender, EventArgs e)
		{
			ParameterizedThreadStart threadStart = new ParameterizedThreadStart(SendMessageToMyTasks);
			Thread thread = new Thread(threadStart);
			thread.Priority = ThreadPriority.BelowNormal;
			thread.Start("Do that thing");
		}
		
		
		private static void SendMessageToMyTasks(object message)
		{
			try {
				using (NamedPipeServerStream server = new NamedPipeServerStream(NWN2TOMYTASKS,PipeDirection.Out)) {
					server.WaitForConnection();
	    			using (StreamWriter writer = new StreamWriter(server))
	    			{
	    				writer.Write(message);
	    				writer.Flush();
	    			}
	    		}
			}
			catch (Exception e) {
				Say.Error(e);
			}
		}
		
		
		private static List<Criterion> FetchAvailableCriteriaForTaskGeneration()
		{
			List<Criterion> criteria = new List<Criterion>();
			criteria.AddRange(new AreaTasksGenerator().GetCriteria());
			criteria.AddRange(new CreatureTasksGenerator().GetCriteria());
			return criteria;
		}
		
		
		private static List<Task> GetTasks()
		{
			List<Task> tasks = new List<Task>();
			
			if (ModuleHelper.ModuleIsOpen()) {
				AreaTasksGenerator areaGen = new AreaTasksGenerator(true,true);
				tasks.AddRange(areaGen.GetTasks());
				
				CreatureTasksGenerator creatureGen = new CreatureTasksGenerator(true,true);
				tasks.AddRange(creatureGen.GetTasks());				
			}
			
			return tasks;
		}
		
		
		private static void SendMagnet(object obj)
		{
			using (NamedPipeClientStream client = new NamedPipeClientStream(".","magnets",PipeDirection.Out))
			{
				try { // doesn't seem to require longer than this
					client.Connect(50);
				}
				catch (TimeoutException) {
					try { // but have a second attempt in case the computer is going slow?
						client.Connect(500);
					}
					catch (TimeoutException) {
						Say.Warning("You need to run Fridge Magnets before you can do this.");
						return;
					}
				}
				
				using (StreamWriter writer = new StreamWriter(client))
				{
					writer.AutoFlush = true;
					writer.WriteLine(obj);
				}				
			}
		}
		
		
		/// <summary>
		/// Check whether the user has set the interface to be locked or unlocked,
		/// and lock/unlock the interface accordingly.
		/// </summary>
		private static void UpdateLockedStatus(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "LockInterface") {
				SetInterfaceLock(Plugin.Options.LockInterface);
			}
		}

		
		/// <summary>
		/// Create an idea representing the user-selected blueprint, and send it to the Magnet Box.
		/// </summary>
		private static void BlueprintSentToIdeas(object sender, EventArgs e)
		{
			if (blueprintView != null) {
				if (blueprintView.Selection.Length > 0) {					
					INWN2Blueprint blueprint = (INWN2Blueprint)blueprintView.Selection[0]; // multiselect should be off	
					
					// Currently experiencing a problem where BlueprintMagnetControl cannot construct
					// because it can't find the MagnetControl resource:
//					BlueprintMagnetControl magnet = new BlueprintMagnetControl(blueprint);	
//					ISerializableData magnetInfo = magnet.GetSerializable();
//					SendMagnet(magnetInfo);
					//OnMagnetSubmitted(new MagnetEventArgs(magnet));
					
					blueprint.OEIUnserialize(blueprint.Resource.GetStream(false)); // fetch from disk
					string text = BlueprintMagnetControl.GetTextForBlueprintMagnet(blueprint);					
					SendMagnet(text);
					blueprint.Resource.Release();
					
					Log.WriteAction(LogAction.added,"idea",text + " ... added from blueprints");
				}
			}
			else {
				Say.Error("Should not have been able to call BlueprintSentToIdeas.");
			}
		}

		
		/// <summary>
		/// Clear the interface of the remnants of the old module, disable any controls
		/// which are tied to modules and update the title bar.
		/// </summary>
		private static void ModuleHelper_ModuleClosed(object sender, EventArgs e)
		{
//			if (!ModuleHelper.ModuleIsOpen()) {
//				Clear();				
//			}
			UpdateTitleBar();
			SetUI_ModuleNotOpen();
		}

			
		/// <summary>
		/// Enable the interface controls which relate to modules and update the title bar.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private static void ModuleHelper_ModuleOpened(object sender, EventArgs e)
		{
			UpdateTitleBar();
			SetUI_ModuleOpen();
		}
		
		
		/// <summary>
		/// Update the title bar.
		/// </summary>
		private static void ModuleHelper_ModuleSaved(object sender, EventArgs e)
		{
			UpdateTitleBar();	
		}		
		
		
		/// <summary>
		/// Disable parts of the interface which should only be used when a module is open.
		/// </summary>
		private static void SetUI_ModuleNotOpen()
		{
			areaList.Enabled = false;
			//conversationList.Enabled = false;
			scriptList.Enabled = false;		
		}
		
		
		/// <summary>
		/// Enable parts of the interface which should only be used when a module is open.
		/// </summary>
		private static void SetUI_ModuleOpen()
		{
			areaList.Enabled = true;
			//conversationList.Enabled = true;
			scriptList.Enabled = true;
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
					Log.WriteAction(LogAction.opened,"area",area.Name);
					page.Text = "area";
					
					NWN2AreaViewer areaViewer = viewer as NWN2AreaViewer;	
					
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
//							Log.WriteAction(LogAction.selected,"object",message.ToString());
//						}
//					};
				}
				else if (viewer is NWN2ConversationViewer) {
					NWN2GameConversation conv = (NWN2GameConversation)viewer.ViewedResource;
										
					//viewer.Close(false);
//					if (viewer != null) {
//						viewer.Dispose();
//					}
					
					
					try {
						TabPageCollection tabPages = tabbedGroupsCollection.ActiveLeaf.TabPages;
					
						tabPages.Remove(page); // close the original conversation editor
//						if (tabPages == null) {
//							tabPages = new TabPageCollection();
//						}
						//tabbedGroupsCollection.ActiveTabPage = null;
					}
					catch (Exception e) {
						Say.Error("Error when closing the original conversation editor.",e);
					}
						
						
					
					
					if (conv.Name.StartsWith("~tmp")) {
						return; // temp file
					}
					
					LaunchConversationWriter(false); // delay bringing it into view
					// Unlike when you Open the same conversation again from within the conversation writer
					// (usually if you have screwed something up and don't want to save) double-clicking
					// the conversation that is already open probably just means you want to bring it into view:
					if (Conversation.CurrentConversation == null || WriterWindow.Instance.OriginalFilename != conv.Name) {
						WriterWindow.Instance.Open(conv.Name);
					}						
					Toolset.BringToFront(WriterWindow.Instance);
				}
				else if (viewer.ViewedResource is NWN2FactionData) {
					NWN2FactionData factions = (NWN2FactionData)viewer.ViewedResource;
					Log.WriteAction(LogAction.opened,"factions");
					page.Text = "faction";
				}
				else if (viewer.ViewedResource is NWN2Journal) {
					//NWN2Journal journal = (NWN2Journal)viewer.ViewedResource;
					Log.WriteAction(LogAction.opened,"journal");
					page.Text = "journal";
				}
				else if (viewer.ViewedResource is TwoDAFile) {
					TwoDAFile twodafile = (TwoDAFile)viewer.ViewedResource;
					Log.WriteAction(LogAction.opened,"2dafile",twodafile.Name);
					page.Text = "2dafile";
				}
				else if (viewer.ViewedResource is NWN2GameScript) {
					NWN2GameScript script = (NWN2GameScript)viewer.ViewedResource;
					Log.WriteAction(LogAction.opened,"script",script.Name);
					page.Text = "script";
				}
				else {
					string type = viewer.ViewedResource.GetType().ToString();
					Log.WriteAction(LogAction.opened,type);
					page.Text = type;
				}
			}
		}
		
		
		private static void ResourceViewerClosed(int index, object value)
		{
			try {
				crown.TabPage page = (crown.TabPage)value;
				if (page.Text == null || page.Text == String.Empty) {
					Log.WriteAction(LogAction.closed,"<resource>",page.Title);
				}
				else if (page.Text == "2dafile" || page.Text == "script" || page.Text == "area") {
					Log.WriteAction(LogAction.closed,page.Text,page.Title);
				}
				else {
					Log.WriteAction(LogAction.closed,page.Text);
				}
			}
			catch (Exception e) {
				Say.Error("Error in resourceviewerclosed",e);
			}
		}
		
				
//		/// <summary>
//		/// Clears the user interface after a module has been closed.
//		/// </summary>
//		private static void Clear()
//		{
//			tabbedGroupsCollection.RootSequence.Clear();
//			form.App.BlueprintView.Module = null; 	  // stop displaying custom blueprints specific to the old module
//			form.App.VerifyOutput.ClearVerifyOutput();  
//			// Close all Property Grids - not implemented. TODO iterate through DockingManager.Contents to find
//			form.App.CreateNewPropertyPanel(null, null, false, "Properties"); // create a fresh toolset Properties panel
//		}
					
		
		private static void UpdateTitleBar()
		{
			form.App.Text = "Adventure Author";
			if (form.App.Module != null && form.App.Module.LocationType == ModuleLocationType.Directory) {
				form.App.Text += ": " + form.App.Module.FileName;
			}
		}	
		
			
		/// <summary>
		/// If a Verify window or the original conversation editor is displayed, hide it again.
		/// </summary>
		private static void OnContentShown(Content c, EventArgs ea)
		{
			if (c.Control is NWN2ConversationViewer) {
				dockingManager.HideContent(c);
			}
			if (form.App.VerifyContent.Visible) {
				dockingManager.HideContent(form.App.VerifyContent);
			}
			if (c.Control is NWN2PropertyGrid) {
				WatchForChanges((NWN2PropertyGrid)c.Control); // TODO does this work for newly opened property grids?
			}
		}
		
		
		private static void WatchForChanges(NWN2PropertyGrid grid)
		{
			grid.ValueChanged += delegate(object sender, NWN2PropertyValueChangedEventArgs e) 
			{ 
				NWN2Utils.WritePropertyChangeToLog(e);
			};
			grid.PreviewStateChanged += delegate 
			{ 
				Log.WriteMessage("Preview state changed on property grid (??)"); 
			};
		}							
		
				
		/// <summary>
		/// Dispose the [X] control and perform other modifications to the resource viewers window.
		/// </summary>
		/// <remarks>Deprecated.</remarks>
		private static void SetupResourceViewersLeaf()
		{
			if (tabbedGroupsCollection.ActiveLeaf != null) {
				// Dispose the [<], [>] and [X] controls:
				List<Control> controls = GetControls(tabbedGroupsCollection.ActiveLeaf.TabControl);
				for (int i = 0; i < 3; i++) {
					controls[i].Dispose(); 
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
				
		private static void NewModuleDialog()
		{
			NewModule form = new NewModule();
			form.ShowDialog();
		}
		
		
		private static void OpenModuleDialog()
		{
			if (ModuleHelper.ModuleIsOpen() && !CloseModuleDialog()) {
				return; // if they change their mind when prompted to close the current module
			}
			
			FolderBrowserDialog openFolder = new FolderBrowserDialog();
			openFolder.Description = "Choose a module to open. \n\nRemember, each module is stored as " + 
				"a folder, not as a file.";
			
			// Expand modules folder (don't use openFolder.RootFolder as it only accepts SpecialFolder constants):
			DirectoryInfo modulesDirectory = new DirectoryInfo(form.ModulesDirectory);
			DirectoryInfo[] modules = modulesDirectory.GetDirectories("*",SearchOption.TopDirectoryOnly);
			if (modules.Length > 0) {
				openFolder.SelectedPath = modules[0].FullName;
			}
			else {
				openFolder.SelectedPath = form.ModulesDirectory; 
			}
			
			openFolder.ShowNewFolderButton = false;
			DialogResult result = openFolder.ShowDialog(form.App);
			if (result == DialogResult.OK) {
				try {
					string modulePath = openFolder.SelectedPath;
					string moduleName = Path.GetFileNameWithoutExtension(modulePath);
					string moduleIFOPath = Path.Combine(modulePath,"MODULE.IFO");
	        		if (!Directory.Exists(modulePath)) {
	        			throw new DirectoryNotFoundException("Could not find directory " + modulePath + ".");
	        		}
	        		else if (!File.Exists(moduleIFOPath)) {
	        			throw new FileNotFoundException(modulePath + " is not a valid NWN2 module. (Missing module.IFO)");
	        		}
					else {
						bool opened = ModuleHelper.Open(moduleName);	
						if (!opened || !ModuleHelper.ModuleIsOpen()) {
							Say.Error("Something went wrong - the module was not opened.");
						}
						else {							
							if (Toolset.Plugin.Options.OpenScratchpadByDefault && 
							    form.App.Module.Areas[ModuleHelper.NAME_OF_SCRATCHPAD_AREA] != null) 
							{
								try {
									AreaHelper.Open(ModuleHelper.NAME_OF_SCRATCHPAD_AREA);
								}
								catch (FileNotFoundException) { 
									Say.Debug("Tried to open " + ModuleHelper.NAME_OF_SCRATCHPAD_AREA  + 
									          " in module '" + moduleName + "', but there was no such area.");
								}
							}
						}
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
		
		
		private static void SaveModuleDialog()
		{
			if (!ModuleHelper.ModuleIsOpen()) {
				Say.Debug("Tried to save when no module was open.");
				return;
			}
			// This resource will be an area (resourceType 2012) if a start location has been
			// assigned, but just an anonymous resource file (.RES) if not - often, checking whether
			// an object is just a .RES file seems to be the equivalent of checking whether it's
			// null (since it often isn't null where you'd expect it to be.)
//	Commented out in favour of placing a start location automatically at the centre of the scratchpad.
//	Note that the start location will not appear until the second time you open the module.
// TODO also note if uncommenting this code: it will not check for INVALID start locations,
// which will pass this check but then raise the Obsidian dialog box as well.
//			if (form.App.Module.ModuleInfo.EntryArea.ResourceType == 0) {
//				Say.Warning("You can't save your module until you choose a Start Location.\n\n" +
//				            "Open an area, click the Set Start Location button, and then click on a clear " +
//				            "patch of ground to place the start location. Then try saving again.");
//				return;
//			}
			
			try {
				ModuleHelper.Save();
			}
			catch (InvalidOperationException e) {
				Say.Error(e);
			}
		}
		
		
		private static void SaveModuleAsDialog()
		{		
			if (!ModuleHelper.ModuleIsOpen()) {
				Say.Debug("Tried to save as when no module was open.");
				return;
			}
			
			try {
				NWN2SaveDirectoryDialog dialog = new NWN2SaveDirectoryDialog();
				string suggestedName;
				int startOfUserName = form.App.Module.FileName.LastIndexOf(User.GetCurrentUserName());
				if (startOfUserName > 0) {
					suggestedName = form.App.Module.FileName.Substring(0,startOfUserName);
				}
				else {
					suggestedName = form.App.Module.FileName + " ";
				}
				suggestedName += User.GetCurrentUserName() + " " + 
								 Tools.GetDateStamp(true) + " " +
								 Tools.GetTimeStamp(true);
				
				dialog.DirectoryName = suggestedName;
				
				if (dialog.ShowDialog(form.App) == DialogResult.OK) {	
					Say.Information("Saving your module under a new name can take up to a minute, \n" +
					                "during which time the toolset will stop responding.\n\n" +
					                "You'll see a message when your module has finished saving.");
					ModuleHelper.SaveAs(dialog.DirectoryName);
				}
			}
			catch (InvalidOperationException e) {
				Say.Error(e);
			}
		}
		
		
		private static void BakeModuleDialog()
		{
			if (!ModuleHelper.ModuleIsOpen()) {
				Say.Debug("Tried to bake when no module was open.");
				return;
			}
			
			ModuleHelper.Bake();
		}	
		
		
		private static void RunModuleDialog()
		{
			if (!ModuleHelper.ModuleIsOpen()) {
				Say.Debug("Tried to run when no module was open.");
				return;
			}
			
			
//			RunAdventure_Form runAdventureForm = new RunAdventure_Form();
//			runAdventureForm.ShowDialog(form.App);
			
			ModuleHelper.Run(String.Empty,false,false);
		}		
		
		
		internal static bool CloseModuleDialog()
		{
			if (ModuleHelper.ModuleIsOpen()) {
				if (!Say.BeQuiet) {
					switch (MessageBox.Show("Do you want to save the current module?", 
						                    "Save?",
						                    MessageBoxButtons.YesNoCancel, 
						                    MessageBoxIcon.Exclamation)) {
						case DialogResult.Cancel:
							return false;		
							
						case DialogResult.Yes:
							ModuleHelper.Save();
							break;
					}
				}
				ModuleHelper.Close();
			}
			return true;
		}
		
		private static void NewAreaDialog()
		{
			if (!ModuleHelper.ModuleIsOpen()) {
				Say.Debug("Tried to add a new area when no module was open.");
				return;
			}
			
			NWN2NewAreaWizard wizard = new NWN2NewAreaWizard(form.App.Module.GetTemporaryName(ModuleResourceType.Area));
			wizard.cWizardCompleteHandler += delegate(string sName, Size cSize, bool bInterior) 
			{  
				AreaHelper.CreateArea(form.App.Module,sName,!bInterior,cSize.Width,cSize.Height);
			};
			wizard.ShowDialog(form.App);
		}
		
		
		/// <summary>
		/// Bring up the Conversation Writer window.
		/// </summary>
		public static void LaunchConversationWriter(bool withDialog) 
		{
			if (!ModuleHelper.ModuleIsOpen()) {
				Say.Debug("Tried to run conversation writer when no module was open.");
				return;
			}
			
			try {
				if (WriterWindow.Instance == null || !WriterWindow.Instance.IsLoaded) {
					WriterWindow.Instance = new WriterWindow(withDialog);
					Plugin.ModuleWindows.Add(WriterWindow.Instance);
				}
				ElementHost.EnableModelessKeyboardInterop(WriterWindow.Instance);
				WriterWindow.Instance.Show();
			}
			catch (Exception e) {
				Say.Error("Could not open conversation writer.",e);
			}
		}	
		
		
		/// <summary>
		/// Bring up the Variable Manager window.
		/// </summary>
		public static void LaunchVariableManager()
		{
			if (!ModuleHelper.ModuleIsOpen()) {
				Say.Debug("Tried to run variable manager when no module was open.");
				return;
			}
			
			try {
				if (VariablesWindow.Instance == null || !VariablesWindow.Instance.IsLoaded) {
					VariablesWindow.Instance = new VariablesWindow();
					Plugin.ModuleWindows.Add(VariablesWindow.Instance);
				}
				//ElementHost.EnableModelessKeyboardInterop(VariablesWindow.Instance);
				VariablesWindow.Instance.ShowDialog();
			}
			catch (Exception e) {
				Say.Error("Could not open variable manager.",e);
			}
		}	
		
		
	
		
		
		/// <summary>
		/// Bring up an Analysis window.
		/// </summary>
		public static CombatMap LaunchAnalysis()
		{
			if (!ModuleHelper.ModuleIsOpen()) {
				Say.Debug("Tried to run analysis window when no module was open.");
				return null;
			}
			
			try {
				CombatMap combatMap = new CombatMap();
				ElementHost.EnableModelessKeyboardInterop(combatMap);	  
				Plugin.ModuleWindows.Add(combatMap);
				combatMap.Show();
				return combatMap;
			}
			catch (Exception e) {
				Say.Error("Could not open analysis window.",e);
				return null;
			}
		}	
		
		
		
		public static void BringToFront(System.Windows.Window window)
		{
			if (window.WindowState == System.Windows.WindowState.Minimized) {
				window.WindowState = System.Windows.WindowState.Normal;
			}
			window.Activate(); // bring to front
		}
		
		
		private static void DeleteAreaDialog(NWN2GameArea area)
		{			
			DialogResult d = MessageBox.Show("Are you sure you want to delete '" + area.Name + "'?",
			                                 "Delete area?", 
			                                 MessageBoxButtons.YesNo,
			                                 MessageBoxIcon.Question,
			                                 MessageBoxDefaultButton.Button2);
			
			if (d == DialogResult.Yes) {
				ModuleHelper.DeleteArea(area);
			}
		}
		
		#endregion Event handlers	
		
		/// <summary>
		/// Set the locked status of the interface.
		/// </summary>
		/// <param name="locked">True to lock the interface; false to unlock it</param>
		internal static void SetInterfaceLock(bool locked)
		{
			try {				 
				dockingManager.AllowFloating = !locked;
				dockingManager.AllowRedocking = !locked;
				dockingManager.AllowResize = !locked;
											
				if (graphicsToolbar != null) { 
					SetToolbarLock(graphicsToolbar,locked);
				}						
				if (objectsToolbar != null) { 
					SetToolbarLock(objectsToolbar,locked);
				}					
				if (fileMenu != null && fileMenu.ToolBar != null) { 
					SetToolbarLock(fileMenu.ToolBar,locked);
				}
				if (aaToolbar != null) {
					SetToolbarLock(aaToolbar,locked);
				}		
				if (addIdeaToolbar != null) {
					SetToolbarLock(addIdeaToolbar,locked);
				}
										
				FieldInfo[] fields = form.App.GetType().GetFields(BindingFlags.Public |
																  BindingFlags.NonPublic |
				                                                  BindingFlags.Instance);
				
				foreach (FieldInfo fi in fields) {								
					if (fi.FieldType == typeof(Content)) {
						Content c = (Content)fi.GetValue(form.App);
						c.HideButton = !locked;
						c.CloseButton = !locked;
					}
				}
			}
			catch (Exception e) {
				Say.Error("Could not lock/unlock the interface.",e);
			}
		}
		
		
		private static void SetToolbarLock(TD.SandBar.ToolBar toolbar, bool locked)
		{
			toolbar.AddRemoveButtonsVisible = !locked;
			toolbar.Closable = !locked;
			toolbar.Movable = !locked;
			toolbar.Tearable = !locked;
			toolbar.Resizable = !locked;
		}
	}
}
