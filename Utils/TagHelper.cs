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
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.Instances;
using NWN2Toolset.NWN2.Data.Templates;
using NWN2Toolset.NWN2.Data.TypedCollections;
using AdventureAuthor.Core;

namespace AdventureAuthor.Utils
{
	/// <summary>
	/// Description of TagHelper.
	/// </summary>
	public static class TagHelper
	{		
    	public enum TagType { Any, Creature, Door, Encounter, Item, Light, Placeable, 
							  PlacedEffect, Sound, StaticCamera, Store, Tree, Trigger, Waypoint };   
		
		public static List<String> GetTags(TagType type)
		{
			if (Adventure.CurrentAdventure == null) {
				return null;
			}
			
			List<string> tags = new List<string>();
						
			foreach (NWN2GameArea area in Adventure.CurrentAdventure.Module.Areas.Values) {
				switch (type) {
					case TagType.Any:
						foreach (NWN2InstanceCollection coll in area.AllInstances) {
							tags.AddRange(GetTags(coll));
						}
						break;
					case TagType.Creature:
						tags.AddRange(GetTags(area.Creatures));
						break;
					case TagType.Door:
						tags.AddRange(GetTags(area.Doors));
						break;
					case TagType.Encounter:
						tags.AddRange(GetTags(area.Encounters));
						break;
					case TagType.Item:
						tags.AddRange(GetTags(area.Items));
						break;
					case TagType.Light:
						tags.AddRange(GetTags(area.Lights));
						break;
					case TagType.Placeable:
						tags.AddRange(GetTags(area.Placeables));
						break;
					case TagType.PlacedEffect:
						tags.AddRange(GetTags(area.PlacedEffects));
						break;
					case TagType.Sound:
						tags.AddRange(GetTags(area.Sounds));
						break;
					case TagType.StaticCamera:
						tags.AddRange(GetTags(area.StaticCameras));
						break;
					case TagType.Store:
						tags.AddRange(GetTags(area.Stores));
						break;
					case TagType.Tree:
						tags.AddRange(GetTags(area.Trees));
						break;
					case TagType.Trigger:
						tags.AddRange(GetTags(area.Triggers));
						break;
					case TagType.Waypoint:
						tags.AddRange(GetTags(area.Waypoints));
						break;
					default:
						return null;
				}
			}			
			return tags;
		}
		
		public static List<string> GetTags(NWN2InstanceCollection instances)
		{
			List<string> tags = new List<string>(instances.Count);
			foreach (INWN2Instance instance in instances) {
				string tag = ((INWN2Object)instance).Tag;
				if (!tags.Contains(tag)) {
					tags.Add(tag);
				}
			}
			return tags;
		}
	}
}
