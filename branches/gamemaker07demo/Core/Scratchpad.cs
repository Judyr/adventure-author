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
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Serialization;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.Instances;
using form = NWN2Toolset.NWN2ToolsetMainForm;
using AdventureAuthor.UI;
using AdventureAuthor.Core;

namespace AdventureAuthor.Core
{
	/// <summary>
	/// Description of Scratchpad.
	/// </summary>
	[Serializable]
	public class Scratchpad : GameArea
	{
		#region Fields
		
		private NWN2GameArea area;
		private Adventure owningAdventure;
				
		[XmlElement]
		public string Name {
			get { return Adventure.NAME_OF_SCRATCHPAD_AREA;; }
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
		
		private Scratchpad()
		{			
		}
		
		public Scratchpad(Adventure adventure)
		{
			this.area = new NWN2GameArea(this.Name,
				                         adventure.Module.Repository.DirectoryName,
				                         adventure.Module.Repository);	
			this.area.HasTerrain = true;
			Size size = new Size(Adventure.MAX_AREA_LENGTH,Adventure.MAX_AREA_LENGTH);
			this.area.Size = GameArea.GetValidSize(size);
			this.owningAdventure = adventure;
		}
		
		#endregion Constructors
		
		#region Methods
		
		public override bool Open()		
		{
			if (this.area == null) {
				throw new InvalidOperationException("This chapter has no area.");
			}
			else if (Adventure.CurrentAdventure != this.owningAdventure) {
				throw new InvalidOperationException("Tried to operate on a closed Adventure.");
			}
			else if (!(Adventure.CurrentAdventure.Module.Areas.Contains(this.Area))) { // if module is mysteriously missing area
				throw new InvalidOperationException("The area you tried to open is not contained within this module.");
			}
			else {			
				if (form.App.GetViewerForResource(this.OwningAdventure.Scratch.Area) == null) { // if Scratchpad is not already open
					int numberOfAreasOpen = form.App.GetAllAreaViewers().Count;
					if (numberOfAreasOpen < Adventure.MAX_NUMBER_OF_OPEN_AREAS) { // if there's room for at least one more area viewer
						this.area.Demand();
						form.App.ShowResource(this.area); // open the Scratchpad
						return true;							
					}
					else {
						throw new InvalidOperationException("It was not possible to open the Scratchpad, as there were already 3 " +
						                                    "areas open - this should not have been possible.");
					}
				}
				else {
					return true; // otherwise return true since Scratchpad is already open
				}
			}
		}	
				
		public override NWN2WaypointInstance GetWaypointToRunFrom()
		{
			throw new NotImplementedException();
		}
			
		public override string ToString()
		{
			return "Scratchpad in " + this.owningAdventure;
		}
		#endregion Methods
	}
}
