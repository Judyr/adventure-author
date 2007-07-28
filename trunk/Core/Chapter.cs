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
using System.Diagnostics;
using System.Drawing;
using System.Xml.Serialization;
using NWN2Toolset.Data;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.Instances;
using form = NWN2Toolset.NWN2ToolsetMainForm;
using AdventureAuthor.UI;
using AdventureAuthor.Core;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Core
{
	/// <summary>
	/// Description of Chapter.
	/// </summary>	
	[Serializable]
	[XmlRoot]	
	public class Chapter : GameArea
	{
		// TODO: Decouple Chapter from Adventure. Make the Chapter constructors
		// take an object of type Adventure, and then call AddChapter on that
		// object.
		
		#region Fields
			
		private string name;
		private string introduction;		
		private NWN2GameArea area;	
		private Adventure owningAdventure;
		
		[XmlElement]
		public string Name {
			get { return name; }
			set { name = value; }
		}		
						
		[XmlElement]
		public string Introduction {
			get { return introduction; }
			set { introduction = value; }
		}
		
		[XmlIgnore]
		public override NWN2GameArea Area {
			get { return area; }
			set { area = value; }
		}
		
		[XmlIgnore]
		public override Adventure OwningAdventure {
			get { return owningAdventure; }
			set { owningAdventure = value; }
		}
		
		#endregion Fields
		
		#region Constructors
		/// <summary>
		/// For serialization purposes - DO NOT DELETE.
		/// </summary>
		private Chapter()
		{
		}
		
		/// <summary>
		/// Base constructor, for use by AddChapter only.
		/// </summary>
		/// <param name="adventure"></param>
		/// <param name="name"></param>
		internal Chapter(Adventure adventure, string name) // base constructor
		{
			this.owningAdventure = adventure;
			this.name = name; // name is set both here..
			this.area = new NWN2GameArea(name, //..and here
				                         adventure.Module.Repository.DirectoryName,
				                         adventure.Module.Repository);
			this.introduction = String.Empty;
		}
				
		/// <summary>
		/// For use by AddChapter only.
		/// </summary>
		/// <param name="adventure"></param>
		/// <param name="name"></param>
		internal Chapter(Adventure adventure, 
		                 string name, 
		                 string introduction, 
		                 bool exterior, 
		                 Size size) : this(adventure,name) // call base constructor
		{
			this.owningAdventure = adventure;
			this.name = name;
			this.introduction = introduction;
			this.area.HasTerrain = exterior;	
			this.area.Size = GameArea.GetValidSize(size);
		}
	
		#endregion Constructors
	
		#region Methods
		
		/// <summary>
		/// Opens the chapter in the toolset.
		/// </summary>
		/// <returns>True if the chapter was successfully opened, false otherwise.</returns>
		public override bool Open()		
		{	
			try {
				if (this.area == null) {
					throw new InvalidOperationException("This chapter has no area.");
				}
				else if (form.App.GetViewerForResource(this.Area) != null) { // if the area is already open
					return true; 
				}
				else if (Adventure.CurrentAdventure != this.owningAdventure) {
					throw new InvalidOperationException("Tried to operate on a closed Adventure.");
				}
				else if (!(Adventure.CurrentAdventure.Module.Areas.Contains(this.Area))) { // if module is mysteriously missing area
					throw new InvalidOperationException("The area you tried to open is not contained within this module.");
				}
				else {
					int numberOfAreasOpen = form.App.GetAllAreaViewers().Count;
					switch (numberOfAreasOpen) {
						case Adventure.MAX_NUMBER_OF_OPEN_AREAS: 	// case 3:
							Say.Error("You can only have 2 chapters open at once.");
							return false; // no more room for area viewers
						case Adventure.MAX_NUMBER_OF_OPEN_AREAS - 1: // case 2:
							if (form.App.GetViewerForResource(this.OwningAdventure.Scratch.Area) == null) { // if Scratchpad is not open
								Say.Error("You can only have 2 chapters open at once.");
								return false; // have room for 1 more area, but will leave it free for the Scratchpad
							}
							else {
								break;
							}
						default:
							break;
					}
									
					// Display area in toolset:
					form.App.ShowResource(this.Area); 
					
					
//					
//								try{
//				UI.Say("here1");
//			FieldInfo contextMenu = UI.dockingManager.GetType().GetField("ContextMenu",BindingFlags.NonPublic |
//			                                                         BindingFlags.Instance);
//				UI.Say("here2");
//			DockingManager.ContextMenuHandler cmh 
//				= contextMenu.GetValue(UI.dockingManager) as DockingManager.ContextMenuHandler;
//			if (cmh == null) UI.Say("Null");
//			else UI.Say("Not null");
//			UI.dockingManager.ContextMenu -= cmh;
//			ContextMenuStrip cms = new ContextMenuStrip();
//			cms.Items.Add("Riproar");
//			cms.Items.Add("Mercury Tilt Switch area  rubbish band");
//			DockingManager.ContextMenuHandler cm = new DockingManager.ContextMenuHandler(cms);
//			UI.dockingManager.ContextMenu -= cm;
//				UI.Say("here4");
//			}
//			catch (Exception e) {
//				UI.Say(e.ToString());
//			}
//					
					
					
//					ContextMenu menu = new ContextMenu();
//					MenuItem m = new MenuItem("A song in silence");
//					m.Click += delegate { MessageBox.Show("clicked"); };
//					menu.MenuItems.Add(m);
					
					
					
					// THoughts:
					// 1) Pretty sure that context menus belong to DockingManager.ContextMenu
					// which is an event handler. I can get this object but it returns null.
					// Either this means I can't really get it because it's an event handler
					// or it's not really the DockingManager that owns them - think it is tho.
					// 2) is it possible to create a new DockingManager, copy over the stuff
					// we want from the original (i.e. almost all of it), and assign the
					// toolset's dockingmanager to be our new object? Mental but possibly possible.
					
					
//					menu.MenuItems.Add(new MenuItem("Like I'm used to"));
//					
//					ContextMenuStrip menu2 = new ContextMenuStrip();
//					menu2.Items.Add("Bird of wales probably aren't very good");
//					menu2.Items.Add("I think");		
//					
//					object o = UI.dockingManager.GetType().InvokeMember("ContextMenu",
//					                                                        BindingFlags.GetField |
//					                                                        BindingFlags.Instance |
//					                                                        BindingFlags.NonPublic,
//					                                                        null,
//					                                                        UI.dockingManager,
//					                                                        null);
//					
//					UI.Say(o.GetType().ToString());
//					DockingManager.ContextMenuHandler cmh = o as DockingManager.ContextMenuHandler;
//					if (cmh == null) {
//						UI.Say("got contextmenuhandler");
//					} 
//					else {
//						UI.Say("contextmenuhandler was null");
//					}
//					try{
//					UI.dockingManager.ContextMenu -= cmh;
//					} catch (Exception e) { UI.Say(e.ToString());}
//					                                         
//					
////					UI.dockingManager.ContextMenu 
////						+= new Crownwood.DotNetMagic.Docking.DockingManager.ContextMenuHandler(menu2);
//					foreach (Crownwood.DotNetMagic.Docking.Content c in UI.dockingManager.Contents) {
//						foreach (Control cc in IO.GetControls(c.Control)) {
//							cc.ContextMenu = null;
//							cc.ContextMenuStrip = null;
//						}
//					}
//					
//		
					
					// Display any Chapter data also..?:
					// ...
					
					return true;
				}
			}
			catch (Exception e) { 
				throw e;
			}
		}	
			
		public override NWN2WaypointInstance GetWaypointToRunFrom()
		{
			throw new NotImplementedException(); // TODO
		}
		
		public bool Rename(string name)
		{
			try	{
				NWN2GameModule mod = (NWN2GameModule)this.area.Module;	
				
				if (!Adventure.IsValidName(name)) {
					Say.Error("Chapter name must be 32 characters or under.");
					return false;
				}
				else if (mod.IsNameOccupied(ModuleResourceType.Area,name)) {
					Say.Error("Chapter name '" + name + "' is taken - choose something else.");
					return false;
				}
				else {
					this.name = name;
					this.area.Name = name;
					return true;
				}
			}
			catch (Exception e)	{
				throw e;
			}
		}		
				
		public override string ToString()
		{
			return this.name;
		}	
		

		
		#endregion Methods
	}
}
