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
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using AdventureAuthor.Conversations;
using AdventureAuthor.Conversations.UI;
using AdventureAuthor.Core;
using AdventureAuthor.Core.UI;
using AdventureAuthor.Utils;
using AdventureAuthor.Variables.UI;
using AdventureAuthor.Ideas;
using Crownwood.DotNetMagic.Common;
using Crownwood.DotNetMagic.Docking;
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
using crown = Crownwood.DotNetMagic.Controls;
using form = NWN2Toolset.NWN2ToolsetMainForm;

namespace AdventureAuthor.Setup
{		
	public static partial class Toolset
	{
		#region Global variables
		
		/// <summary>
		/// The docking manager controls all docked controls on the main interface screen
		/// </summary>
		private static DockingManager dockingManager = null;
		
		/// <summary>
		/// Owns the TabGroupLeaf which holds resource viewers
		/// </summary>
		private static Crownwood.DotNetMagic.Controls.TabbedGroups tabbedGroupsCollection = null; 
		
		/// <summary>
		/// The area contents view control, which lists every object in the current game area
		/// </summary>
		private static NWN2AreaContentsView areaContentsView = null;
		
		/// <summary>
		/// The blueprints view control, which lists every available blueprint
		/// </summary>
		private static NWN2BlueprintView blueprintView = null;
		
		/// <summary>
		/// A list of all the game object dictionaries, one for each type of object (e.g. creature, door, placeable etc.)
		/// </summary>
		private static Dictionary<string,Dictionary<INWN2Object,GTLTreeNode>> dictionaries 
			= new Dictionary<string,Dictionary<INWN2Object,GTLTreeNode>>(14);	
		
		/// <summary>
		/// The mouse mode the user was previously in (e.g. Select Objects) - so that we only log genuine changes to the mouse mode
		/// </summary>
		private static MouseMode? previousMouseMode = null;	
		
		#endregion Global variables	
			
			
		#region Events
		
		public static event EventHandler<IdeaEventArgs> IdeaSubmitted;		
		private static void OnIdeaSubmitted(IdeaEventArgs e)
		{
			EventHandler<IdeaEventArgs> handler = IdeaSubmitted;
			if (handler != null) {
				handler(null, e);
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
					Log.WriteMessage("entered mode " + NWN2AreaViewer.MouseMode);
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
			
			GraphicsPreferencesToolBar graphicsToolbar = null;
			TD.SandBar.ToolBar objectsToolbar = null;
					
			// Iterate through NWN2ToolsetMainForm fields and apply UI modifications:
			foreach (FieldInfo fi in fields) {	
								
				// Hide everything except for the area contents and blueprints, and add a chapter list:
				
				if (fi.FieldType == typeof(DockingManager)) {
					dockingManager = (DockingManager)fi.GetValue(form.App);					
					
					// Add 'add idea' content:			
//					ElementHost host = new ElementHost();
//					AdventureAuthor.Ideas.LightweightIdeaControl ideacontrol 
//						= new AdventureAuthor.Ideas.LightweightIdeaControl();
//					ideacontrol.Width = 300;
//					ideacontrol.Height = 200;
//					host.Child = ideacontrol;
//					Content content = new Content(dockingManager,host,"Record an idea");
//					ideacontrol.IdeaSubmitted += delegate(object sender, IdeaEventArgs e) { OnIdeaSubmitted(e); };					
//					dockingManager.Contents.Add(content);	
//					dockingManager.AddContentWithState(content,State.Floating);
//					//dockingManager.AddContentToZone(content,null,0);
//					dockingManager.ShowContent(content);	
					
					//TODO: try iterating through dockingManager object, copying every field
					//except for Event ContextMenu, and then assigning it to the toolset (using a reflected method)
					
					// Lock the interface:
					dockingManager.AllowFloating = false;
//					dockingManager.AllowRedocking = false;
//					dockingManager.AllowResize = false;
			
					// NB: Trying to hide specific objects that are already hidden (or show those
					// that are showing) seems to lead to an InvalidOperationException.
					dockingManager.ShowAllContents();					
					
					List<Content> contents = new List<Content>(5);
					
					foreach (Content c in dockingManager.Contents) {
						if (c.FullTitle == "Conversations") {
							contents.Add(c);
						}
						else if (c.FullTitle == "Campaign Conversations") {
							contents.Add(c);
						}
						else if (c.FullTitle == "Campaign Scripts") {
							contents.Add(c);
						}
						else if (c.FullTitle == "Search Results") {
							contents.Add(c);
						}
						else if (c.Control is NWN2VerifyOutputControl) {
							contents.Add(c);
						}
						
						// Lock the interface:
						//c.HideButton = false;
						//c.CloseButton = false;
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
									MenuItem addAsIdea = new MenuItem("Add this to my Ideas");
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
								Log.WriteAction(Log.Action.selected,"blueprint",message.ToString());
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
								Log.WriteAction(Log.Action.selected,key,e.TreeNode.Text);
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
//						MenuItem addAsIdea = new MenuItem("Add this to my Ideas");
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
				
				// Prevent the object manipulation toolbar from being modified or moved:
				else if (fi.FieldType == typeof(TD.SandBar.ToolBar)) {
					objectsToolbar = (TD.SandBar.ToolBar)fi.GetValue(form.App);
					objectsToolbar.AddRemoveButtonsVisible = false;
					objectsToolbar.Movable = false;					
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
							TD.SandBar.ToolBar aaToolbar = new TD.SandBar.ToolBar();
							SetupAdventureAuthorToolBar(aaToolbar);
							tbc.Controls.Add(aaToolbar);
							TD.SandBar.ToolBar ideaToolbar = new TD.SandBar.ToolBar();
							SetupAddIdeaToolBar(ideaToolbar);
							tbc.Controls.Add(ideaToolbar);
						}
					} 
					catch (Exception e) {
						Say.Error(e.ToString());
					}
				}
			}
						
//			if (objectsToolbar != null && graphicsToolbar != null) {
//				MenuBarItem graphicsOptionsItem = new MenuBarItem("Graphics Options");
//				foreach (ToolbarItemBase item in graphicsToolbar.Items) {
//					graphicsOptionsItem.Items.Add(item);
//				}
//				objectsToolbar.Items.Add(graphicsOptionsItem);
//				graphicsToolbar.Dispose();				
//			}
//			else {
//				Say.Warning("Failed to modify the UI toolbars.");
//			}
			
			ModuleHelper.ModuleSaved += new EventHandler(ModuleHelper_ModuleSaved);
			ModuleHelper.ModuleOpened += new EventHandler(ModuleHelper_ModuleOpened);
			ModuleHelper.ModuleClosed += new EventHandler(ModuleHelper_ModuleClosed);
			
			UpdateTitleBar();			
		}
		
		
		private static string GetTagOrResref(INWN2Object obj)
		{
			return "tag: " + obj.Tag;
		}
		
		
		private static string GetTagOrResref(INWN2Blueprint blueprint)
		{
			return "resref: " + blueprint.TemplateResRef.Value;
		}
		
		
		private static string GetDescriptionForMagnet(INWN2Blueprint blueprint)
		{
			string text;
			
			if (blueprint is NWN2CreatureTemplate) {
				NWN2CreatureTemplate creature = (NWN2CreatureTemplate)blueprint;
				text = creature.FirstName + " " + creature.LastName + 
					"\nCreature" + "\nresref: " + blueprint.TemplateResRef.Value;
			}
			else {
				text = blueprint.Comment + "\n" + blueprint.ObjectType.ToString() + "\nresref: " +
				blueprint.TemplateResRef.Value;				
			}
			
			return text;
			
//			else if (blueprint is NWN2DoorTemplate) {
//				NWN2DoorTemplate door = (NWN2DoorTemplate)blueprint;
//				text = door.Name + "\nDoor" + "\nresref: " + blueprint.TemplateResRef.Value;
//			}
//			else if (blueprint is NWN2EncounterTemplate) {
//				NWN2EncounterTemplate encounter = (NWN2DoorTemplate)blueprint;
//				text = encounter.Name + "\nEncounter" + "\nresref: " + blueprint.TemplateResRef.Value;
//			}
//			else if (blueprint is NWN2DoorTemplate) {
//				NWN2DoorTemplate door = (NWN2DoorTemplate)blueprint;
//				text = door.Name + "\nDoor" + "\nresref: " + blueprint.TemplateResRef.Value;
//			}
//			else if (blueprint is NWN2DoorTemplate) {
//				NWN2DoorTemplate door = (NWN2DoorTemplate)blueprint;
//				text = door.Name + "\nDoor" + "\nresref: " + blueprint.TemplateResRef.Value;
//			}
//			else if (blueprint is NWN2DoorTemplate) {
//				NWN2DoorTemplate door = (NWN2DoorTemplate)blueprint;
//				text = door.Name + "\nDoor" + "\nresref: " + blueprint.TemplateResRef.Value;
//			}
//			else if (blueprint is NWN2DoorTemplate) {
//				NWN2DoorTemplate door = (NWN2DoorTemplate)blueprint;
//				text = door.Name + "\nDoor" + "\nresref: " + blueprint.TemplateResRef.Value;
//			}
//			else if (blueprint is NWN2DoorTemplate) {
//				NWN2DoorTemplate door = (NWN2DoorTemplate)blueprint;
//				text = door.Name + "\nDoor" + "\nresref: " + blueprint.TemplateResRef.Value;
//			}
//								
//					
//				
//			
//			return text + "\n" + GetTagOrResref(
//					string text2 = blueprint.Name + "\n" + blueprint.ObjectType.ToString() + "\n(Resref '" +
//								  blueprint.TemplateResRef.Value + "')";
//					
//					
//			
//			// if blueprint;
//			return text + "\nresref: " + blueprint.TemplateResRef.Value;
//			// else;
//			//return text + "\ntag: " + blueprint.Tag;
		}

		
		private static void BlueprintSentToIdeas(object sender, EventArgs e)
		{
			if (blueprintView != null) {
				if (blueprintView.Selection.Length > 0) {					
					INWN2Blueprint blueprint = (INWN2Blueprint)blueprintView.Selection[0]; // multiselect should be off
					NWN2GlobalBlueprintManager.Instance.LoadBlueprint(blueprint.Resource); // fetch from disk
					string text = GetDescriptionForMagnet(blueprint);
					Idea idea = new Idea(text,IdeaCategory.Toolset);
					OnIdeaSubmitted(new IdeaEventArgs(idea));
				}
			}
			else {
				Say.Error("Should not have been able to call BlueprintSentToIdeas.");
			}
		}

		
		private static void ModuleHelper_ModuleClosed(object sender, EventArgs e)
		{
			if (!ModuleHelper.ModuleIsOpen()) {
				Clear();
				
			}
			UpdateTitleBar();
		}

		
		private static void ModuleHelper_ModuleOpened(object sender, EventArgs e)
		{
			if (!ModuleHelper.ModuleIsOpen()) {
				
			}
			UpdateTitleBar();
		}
		
		
		private static void ModuleHelper_ModuleSaved(object sender, EventArgs e)
		{
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
					
					NWN2AreaViewer areaViewer = viewer as NWN2AreaViewer;	
					
//					foreach (Control c in GetControls(areaViewer)) {
//						Say.Debug(c.ToString());
//						if (c.ContextMenu != null && c.ContextMenu.MenuItems.Count > 0) {
//							Say.Debug("Context menu:");
//							foreach (MenuItem menuItem in c.ContextMenu.MenuItems) {
//								Say.Debug(" - " + menuItem.Text);
//							}
//						}
//						if (c.ContextMenuStrip != null && c.ContextMenuStrip.Items.Count > 0) {
//							Say.Debug("Context strip:");
//							foreach (ToolStripItem menuItem in c.ContextMenuStrip.Items) {
//								Say.Debug(" - " + menuItem.Text);
//							}
//						}
//						Say.Debug("");
//					}
					
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
		/// Clears the user interface after a module has been closed.
		/// </summary>
		private static void Clear()
		{
			tabbedGroupsCollection.RootSequence.Clear();
			form.App.BlueprintView.Module = null; 	  // stop displaying custom blueprints specific to the old module
			form.App.VerifyOutput.ClearVerifyOutput();  
			// Close all Property Grids - not implemented. TODO iterate through DockingManager.Contents to find
			form.App.CreateNewPropertyPanel(null, null, false, "Properties"); // create a fresh toolset Properties panel
		}
					
		
		private static void UpdateTitleBar()
		{
			form.App.Text = "Adventure Author";
			if (form.App.Module != null && form.App.Module.LocationType == ModuleLocationType.Directory) {
				form.App.Text += ": " + form.App.Module.Name;
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
				WatchForChanges((NWN2PropertyGrid)c.Control);
			}
		}
		
		
		private static void WatchForChanges(NWN2PropertyGrid grid)
		{
			grid.ValueChanged += delegate(object sender, NWN2PropertyValueChangedEventArgs e) 
			{ 
				Log.WritePropertyChange(e); 
			};
			grid.PreviewStateChanged += delegate 
			{ 
				Log.WriteMessage("Preview state changed on property grid (??)"); 
			};
		}							
		
				
		/// <summary>
		/// Dispose the [X] control and perform other modifications to the resource viewers window.
		/// </summary>
		private static void SetupResourceViewersLeaf()
		{
			if (tabbedGroupsCollection.ActiveLeaf != null) {
				// Set font size:
				tabbedGroupsCollection.ActiveLeaf.TabControl.Font = ModuleHelper.ADVENTURE_AUTHOR_FONT;
									
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
	        		if (!Directory.Exists(modulePath)) {
	        			throw new DirectoryNotFoundException("Could not find directory " + modulePath + ".");
	        		}
	        		else if (!File.Exists(moduleIFOPath)) {
	        			throw new FileNotFoundException(modulePath + " is not a valid NWN2 module. (Missing module.IFO)");
	        		}
					else {
						bool opened = ModuleHelper.Open(moduleName);	
						if (!opened || !ModuleHelper.ModuleIsOpen()) {
							Say.Error("Failed to open module.");
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
				if (dialog.ShowDialog(form.App) == DialogResult.OK) {					
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
				if (!ModuleHelper.BeQuiet) {
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
			
			//CreateChapter_Form chapterForm = new CreateChapter_Form();
			//chapterForm.ShowDialog(form.App);
			
			NWN2NewAreaWizard wizard = new NWN2NewAreaWizard(form.App.Module.GetTemporaryName(ModuleResourceType.Area));
			wizard.cWizardCompleteHandler += delegate(string sName, Size cSize, bool bInterior) 
			{  
				AreaHelper.CreateArea(sName,!bInterior,cSize.Width,cSize.Height);
			};
			wizard.ShowDialog(form.App);
		}
		
		
		/// <summary>
		/// Bring up the Conversation Writer window.
		/// </summary>
		public static void LaunchConversationWriter() // Works
		{
			if (!ModuleHelper.ModuleIsOpen()) {
				Say.Debug("Tried to run conversation writer when no module was open.");
				return;
			}
//			
//			LaunchConversationWriterApplication();
//			return;
			
			try {
				if (WriterWindow.Instance == null || !WriterWindow.Instance.IsLoaded) {
					WriterWindow.Instance = new WriterWindow();
					ElementHost.EnableModelessKeyboardInterop(WriterWindow.Instance);
					WriterWindow.Instance.Show();
					
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
			if (!ModuleHelper.ModuleIsOpen()) {
				Say.Debug("Tried to run variable manager when no module was open.");
				return;
			}
			
			try {
				if (VariablesWindow.Instance == null || !VariablesWindow.Instance.IsLoaded) {
					VariablesWindow.Instance = new VariablesWindow();
					ElementHost.EnableModelessKeyboardInterop(VariablesWindow.Instance);
					VariablesWindow.Instance.ShowDialog();
				}
			}
			catch (Exception e) {
				Say.Error(e);
			}
		}		
		
		
		private static void DeleteAreaDialog(NWN2GameArea area)
		{			
			DialogResult d = MessageBox.Show("Are you sure you want to delete '" + area.Name + "'?",
			                                 "Delete chapter?", 
			                                 MessageBoxButtons.YesNo,
			                                 MessageBoxIcon.Question,
			                                 MessageBoxDefaultButton.Button2);
			
			if (d == DialogResult.Yes) {
				ModuleHelper.DeleteArea(area);
			}
		}
					
		
		private static void ExitToolsetDialog()
		{
			if (!ModuleHelper.BeQuiet && ModuleHelper.ModuleIsOpen() && !CloseModuleDialog()) {
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
