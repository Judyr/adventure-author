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
using System.IO;
using System.Drawing;
using System.Xml.Serialization;
using System.Windows.Forms;
using AdventureAuthor.Utils;
using NWN2Toolset.Data;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.Instances;
using NWN2Toolset.NWN2.Data.TypedCollections;
using NWN2Toolset.NWN2.IO;
using NWN2Toolset.NWN2.Views;
using AdventureAuthor.Scripts;
using OEIShared.Utils;
using OEIShared.IO;
using OEIShared.OEIMath;
using Microsoft.DirectX;
using form = NWN2Toolset.NWN2ToolsetMainForm;

namespace AdventureAuthor.Core
{
	public static class AreaHelper
	{
		public static NWN2GameArea CreateArea(NWN2GameModule module, string name)
		{
			return CreateArea(module,name,true,ModuleHelper.MIN_AREA_LENGTH,ModuleHelper.MIN_AREA_LENGTH);
		}
		
		
		public static NWN2GameArea CreateArea(NWN2GameModule module, string name, bool exterior)
		{
			return CreateArea(module,name,exterior,ModuleHelper.MIN_AREA_LENGTH,ModuleHelper.MIN_AREA_LENGTH);		
		}
				
		
		public static NWN2GameArea CreateArea(NWN2GameModule module, string name, bool exterior, int width, int height)
		{
//			if (!ModuleHelper.ModuleIsOpen()) {
//				throw new InvalidOperationException("No game module is open to operate on.");
//			}
			
			// Create a new area object:
			NWN2GameArea area = new NWN2GameArea(name,
			                                     module.Repository.DirectoryName,
			                                     module.Repository);
			area.HasTerrain = exterior;
			area.Size = GetValidAreaSize(width,height);
						
			// Apply default scripts for logging changes:
			try {
				ScriptHelper.ApplyDefaultScripts(area);
			}
			catch (IOException e) {
				Say.Error("Could not find Adventure Author logging scripts to assign to this resource.",e);
			}
			
			// Log when objects in this area are added and deleted:
			ApplyLogging(area);
			
			form.App.Module.AddResource(area);
						
			area.OEISerialize();
			
			return area;
		}
		
		
		internal static void ApplyLogging(NWN2GameArea area)
		{
			// Add event handlers for logging when objects are added and deleted:
			foreach (NWN2InstanceCollection instances in area.AllInstances) {
				instances.Inserted += new OEICollectionWithEvents.ChangeHandler(LogAddedGameObject);
				instances.Removed += new OEICollectionWithEvents.ChangeHandler(LogDeletedGameObject);
			}
		}
		
		
		public static void PlaceStartLocationAtCentre(NWN2GameArea area)
		{
			NWN2GameModule module = (NWN2GameModule)area.Module;
			module.ModuleInfo.EntryArea = area.AreaResource;
			module.ModuleInfo.EntryOrientation = new RHQuaternion(0,0,0,0);
			BoundingBox3 areaBounds = area.GetBoundsOfArea();
			
			module.ModuleInfo.EntryPosition = new Vector3(80 + (areaBounds.Length/2),
			                                              80 + (areaBounds.Height/2),
			                                              0);
		}
		
		
		/// <summary>
		/// Open an area in the toolset.
		/// </summary>
		/// <param name="area">The name of the area to open</param>
		/// <returns>The area that was opened</returns>
		public static NWN2GameArea Open(string name)		
		{	
			if (!ModuleHelper.ModuleIsOpen()) {
				throw new InvalidOperationException("No game module is open to operate on.");
			}			
			
			NWN2GameArea area = form.App.Module.Areas[name];
			if (area == null) {
				throw new FileNotFoundException("Couldn't find an area named '" + name + "' in this module.");
			}
			else if (form.App.GetViewerForResource(area) == null) { // if the area is not already open
				form.App.ShowResource(area);
			}		
			return area;
		}	
			
				
		public static void Close(NWN2GameArea area)
		{
			INWN2Viewer viewer = form.App.GetViewerForResource(area);
				
			if (viewer != null) {	
				if (!ModuleHelper.BeQuiet) {
					// Show a dialog asking the user if they want to save:
					DialogResult result = MessageBox.Show("Save changes to this area?",
					                    				  "Save", 
					                    				  MessageBoxButtons.YesNoCancel);
					if (result == DialogResult.Cancel) {
						return;
					}
					else if (result == DialogResult.Yes) {
						ModuleHelper.Save();
					}
				}					
				form.App.CloseViewer(viewer,true); // don't need to save here
				viewer.Dispose();	
				
				if (form.App.AreaContents.Area == area) {
					form.App.AreaContents.Area = null;
					form.App.AreaContents.Refresh();
				}	
			}
		}		
		
		
		public static NWN2WaypointInstance GetWaypointToRunFrom()
		{
			throw new NotImplementedException(); // TODO
		}
		
				
		private static void LogAddedGameObject(OEICollectionWithEvents cList, int index, object value)
		{
			INWN2Instance instance = value as INWN2Instance;
			if (instance != null) {
				Log.WriteAction(LogAction.added,Log.GetNWN2TypeName(value),instance.Name);
				try {
					ScriptHelper.ApplyDefaultScripts(instance);
				}
				catch (IOException e) {
					Say.Error("Could not find Adventure Author logging scripts to assign to this resource.",e);
				}
			}
			else {
				Log.WriteAction(LogAction.added,Log.GetNWN2TypeName(value));
			}
		}
		
				
		private static void LogDeletedGameObject(OEICollectionWithEvents cList, int index, object value)
		{
			INWN2Instance instance = value as INWN2Instance;
			if (instance != null) {
				Log.WriteAction(LogAction.deleted,Log.GetNWN2TypeName(value),instance.Name);
			}
			else {
				Log.WriteAction(LogAction.deleted,Log.GetNWN2TypeName(value));
			}
		}
		
		
		
		/// <summary>
		/// Returns a Size with width and height between the minimum and maximum valid area lengths.
		/// </summary>
		/// <param name="width">The specified area width</param>
		/// <param name="height">The specified area height</param>
		/// <returns>A valid area size (which may contain the values that were originally passed)</returns>
		private static Size GetValidAreaSize(int width, int height)
		{
			int w,h; // new width and height
			
			if (width < ModuleHelper.MIN_AREA_LENGTH) {
				w = ModuleHelper.MIN_AREA_LENGTH;
			}
			else if (width > ModuleHelper.MAX_AREA_LENGTH) {
				w = ModuleHelper.MAX_AREA_LENGTH;
			}
			else {
				w = width;
			}
			
			if (height < ModuleHelper.MIN_AREA_LENGTH) {
				h = ModuleHelper.MIN_AREA_LENGTH;
			}
			else if (height > ModuleHelper.MAX_AREA_LENGTH) {
				h = ModuleHelper.MAX_AREA_LENGTH;
			}
			else {
				h = height;
			}
			
			return new Size(w,h);
		}
	
		
		/// <summary>
		/// Check if an object with the given tag exists in this area.
		/// </summary>
		/// <param name="tag">The tag to search for</param>
		/// <returns>True if an object with this tag exists in this area; false otherwise.</returns>
		public static bool ContainsObject(NWN2GameArea area, string tag)
		{					
			foreach (NWN2InstanceCollection coll in area.AllInstances) {
				foreach (INWN2Instance instance in coll) {
					if (instance.Name == tag) {	// INWN2Instance.Name is the object's tag.
						return true;
					}
				}				
			}
			return false;
		}
		
		
		/// <summary>
		/// Check if an object exists in this area which has the given tag, and can take part in conversations.
		/// </summary>
		/// <param name="tag">The tag to search for</param>
		/// <returns>True if an object with this tag exists in this area and can speak; false otherwise.</returns>		
		public static bool ContainsSpeakingObject(NWN2GameArea area, string tag)
		{
			// Creatures, doors and placeables can all take part in conversations and own their own conversations:
			foreach (NWN2CreatureInstance creature in area.Creatures) {
				if (creature.Tag == tag) {
					return true; // note that creature must be alive for the conversation to take place
				}
			}
			foreach (NWN2DoorInstance door in area.Doors) {
				if (door.Tag == tag) {
					return true;
				}
			}
			foreach (NWN2PlaceableInstance placeable in area.Placeables) {
				if (placeable.Tag == tag) {
					return true;
				}
			}
			return false;
			
			// Notes on other speaking instances:
			// Items *can* speak in-game, but only if they are not being carried. Since almost all items have the appearance
			// of a bag, this probably isn't worth allowing.
			// Encounters, Stores, Triggers, Trees, Lights and Sounds are (seemingly intermittently) allowed to speak
			// but only through the player - that is the conversation does not end, but the player speaks their line. Disallow.
			
			// If two objects have the same tag, it seems to just pick one to be the speaker. (At random?)
		}
		
		
		/// <summary>
		/// Check if an object exists in this chapter which has the given tag, and can be animated.
		/// </summary>
		/// <param name="tag">The tag to search for</param>
		/// <returns>True if an object with this tag exists in this chapter and can be animated; false otherwise.</returns>			
		public static bool ContainsAnimatableObject(NWN2GameArea area, string tag)
		{
			// Currently looks like there might be no good way to tell whether this is true without lots of manual work.
			// All the animations are under Data/lod_merged*.zip, and different creatures have different sets of animations.
			// Many seem to have a few (e.g. bats and badgers seem to have around 10), while humans have almost all.
			// Might just have to warn players that animations are most likely to work on humanoid characters.
			
			foreach (NWN2CreatureInstance creature in area.Creatures) {
				if (creature.Tag == tag) {
					return true;
				}
			}	
			return false;
		}
	
	}
}
