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
using NWN2Toolset.NWN2.Data.Blueprints;
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
    	public enum ObjectType { Any, Creature, Door, Encounter, Item, Light, Placeable, 
							  PlacedEffect, Sound, StaticCamera, Store, Tree, Trigger, Waypoint };   
		
		public static List<String> GetResRefs(ObjectType type)
		{
			if (Adventure.CurrentAdventure == null) {
				return null;
			}
			
			List<string> resrefs = new List<string>();
						
			switch (type) {
				case ObjectType.Any:
//					foreach (NWN2InstanceCollection coll in Adventure.CurrentAdventure.Module.BlueprintCollecti) {
//						tags.AddRange(GetTags(coll));TODO
//					}
//					break;
				case ObjectType.Creature:
					resrefs.AddRange(GetResRefs(NWN2GlobalBlueprintManager.Instance.Creatures));
					break;
				case ObjectType.Door:
					resrefs.AddRange(GetResRefs(NWN2GlobalBlueprintManager.Instance.Doors));
					break;
				case ObjectType.Encounter:
					resrefs.AddRange(GetResRefs(NWN2GlobalBlueprintManager.Instance.Encounters));
					break;
				case ObjectType.Item:
					resrefs.AddRange(GetResRefs(NWN2GlobalBlueprintManager.Instance.Items));
					break;
				case ObjectType.Light:
					resrefs.AddRange(GetResRefs(NWN2GlobalBlueprintManager.Instance.Lights));
					break;
				case ObjectType.Placeable:
					resrefs.AddRange(GetResRefs(NWN2GlobalBlueprintManager.Instance.Placeables));
					break;
				case ObjectType.PlacedEffect:
					resrefs.AddRange(GetResRefs(NWN2GlobalBlueprintManager.Instance.PlacedEffects));
					break;
				case ObjectType.Sound:
					resrefs.AddRange(GetResRefs(NWN2GlobalBlueprintManager.Instance.Sounds));
					break;
				case ObjectType.StaticCamera:
					resrefs.AddRange(GetResRefs(NWN2GlobalBlueprintManager.Instance.StaticCameras));
					break;
				case ObjectType.Store:
					resrefs.AddRange(GetResRefs(NWN2GlobalBlueprintManager.Instance.Stores));
					break;
				case ObjectType.Tree:
					resrefs.AddRange(GetResRefs(NWN2GlobalBlueprintManager.Instance.Trees));
					break;
				case ObjectType.Trigger:
					resrefs.AddRange(GetResRefs(NWN2GlobalBlueprintManager.Instance.Triggers));
					break;
				case ObjectType.Waypoint:
					resrefs.AddRange(GetResRefs(NWN2GlobalBlueprintManager.Instance.Waypoints));
					break;
				default:
					return null;
			}
			return resrefs;
		}	
				
		public static List<string> GetResRefs(NWN2BlueprintCollection blueprints)
		{
			List<string> resrefs = new List<string>(blueprints.Count);
			foreach (INWN2Blueprint blueprint in blueprints) {
				string resref = blueprint.TemplateResRef.Value;				
				if (resref != String.Empty && !resrefs.Contains(resref)) {
					resrefs.Add(resref);
				}
			}
			return resrefs;
		}
		
		public static List<String> GetTags(ObjectType type)
		{
			if (Adventure.CurrentAdventure == null) {
				return null;
			}
			
			List<string> tags = new List<string>();
						
			foreach (NWN2GameArea area in Adventure.CurrentAdventure.Module.Areas.Values) {
				switch (type) {
					case ObjectType.Any:
						foreach (NWN2InstanceCollection coll in area.AllInstances) {
							tags.AddRange(GetTags(coll));
						}
						break;
					case ObjectType.Creature:
						tags.AddRange(GetTags(area.Creatures));
						break;
					case ObjectType.Door:
						tags.AddRange(GetTags(area.Doors));
						break;
					case ObjectType.Encounter:
						tags.AddRange(GetTags(area.Encounters));
						break;
					case ObjectType.Item:
						tags.AddRange(GetTags(area.Items));
						break;
					case ObjectType.Light:
						tags.AddRange(GetTags(area.Lights));
						break;
					case ObjectType.Placeable:
						tags.AddRange(GetTags(area.Placeables));
						break;
					case ObjectType.PlacedEffect:
						tags.AddRange(GetTags(area.PlacedEffects));
						break;
					case ObjectType.Sound:
						tags.AddRange(GetTags(area.Sounds));
						break;
					case ObjectType.StaticCamera:
						tags.AddRange(GetTags(area.StaticCameras));
						break;
					case ObjectType.Store:
						tags.AddRange(GetTags(area.Stores));
						break;
					case ObjectType.Tree:
						tags.AddRange(GetTags(area.Trees));
						break;
					case ObjectType.Trigger:
						tags.AddRange(GetTags(area.Triggers));
						break;
					case ObjectType.Waypoint:
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
				if (tag != String.Empty && !tags.Contains(tag)) {
					tags.Add(tag);
				}
			}
			return tags;
		}
	}
}
