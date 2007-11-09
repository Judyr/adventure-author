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
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.TypedCollections;
using NWN2Toolset.NWN2.Data.Instances;
using NWN2Toolset.NWN2.Views;
using System.Xml.Serialization;
using form = NWN2Toolset.NWN2ToolsetMainForm;
using AdventureAuthor.Core;
using AdventureAuthor.Utils;
using OEIShared.Utils;

namespace AdventureAuthor.Core
{
	/// <summary>
	/// Description of GameArea.
	/// </summary>
	public abstract class GameArea
	{				
		#region Static methods
		
		/// <summary>
		/// Returns a Size with width and height between the minimum and maximum valid area lengths.
		/// </summary>
		/// <param name="width">The specified area width</param>
		/// <param name="height">The specified area height</param>
		/// <returns>A valid area size (which may contain the values that were originally passed)</returns>
		public static Size GetValidSize(Size size)
		{
			int width = size.Width;
			int height = size.Height;
			
			int w,h; // new width and height
			
			if (width < Adventure.MIN_AREA_LENGTH) {
				w = Adventure.MIN_AREA_LENGTH;
			}
			else if (width > Adventure.MAX_AREA_LENGTH) {
				w = Adventure.MAX_AREA_LENGTH;
			}
			else {
				w = width;
			}
			
			if (height < Adventure.MIN_AREA_LENGTH) {
				h = Adventure.MIN_AREA_LENGTH;
			}
			else if (height > Adventure.MAX_AREA_LENGTH) {
				h = Adventure.MAX_AREA_LENGTH;
			}
			else {
				h = height;
			}
			
			return new Size(w,h);
		}
		
		public static string GetBackupDirname(string name)
		{
			return Path.Combine(Adventure.BackupDir,Adventure.CurrentUser.Name+"_"+name+"_"
			                    +DateTime.Now.Day+"_"+DateTime.Now.Month+"_"+DateTime.Now.Year
			                    +"___"+DateTime.Now.Ticks.ToString());
		}
						
		#endregion Static methods
					
		[XmlIgnore]
		public abstract NWN2GameArea Area {	get; set; }
		
		internal void LogChanges()
		{
			foreach (NWN2InstanceCollection instances in this.Area.AllInstances) {
				instances.Inserted += new OEICollectionWithEvents.ChangeHandler(AddedGameObject);
				instances.Removed += new OEICollectionWithEvents.ChangeHandler(DeletedGameObject);
			}
		}
		
		[XmlIgnore]
		public abstract Adventure OwningAdventure { get; set; }
				
		public abstract NWN2WaypointInstance GetWaypointToRunFrom();
						
		public abstract bool Open();	
		
		public void Close()
		{
			INWN2Viewer viewer = form.App.GetViewerForResource(this.Area);
				
			if (viewer != null) {	
				if (!Adventure.BeQuiet) {
					// Show a dialog asking the user if they want to save:
					DialogResult result = MessageBox.Show("Do you want to save changes to the Adventure before closing this window?",
					                    				  "Save", 
					                    				  MessageBoxButtons.YesNoCancel);
					if (result == DialogResult.Cancel) {
						return;
					}
					else if (result == DialogResult.Yes) {
						Adventure.CurrentAdventure.Save();
					}
				}					
				form.App.CloseViewer(viewer,true); // don't need to save here
				viewer.Dispose();	
			}
		}
	
		/// <summary>
		/// Check if an object with the given tag exists in this chapter.
		/// </summary>
		/// <param name="tag">The tag to search for</param>
		/// <returns>True if an object with this tag exists in this chapter; false otherwise.</returns>
		public bool ContainsObject(string tag)
		{		
			int numberOfTagsFound = 0;			
			foreach (NWN2InstanceCollection coll in this.Area.AllInstances) {
				foreach (INWN2Instance instance in coll) {
					if (instance.Name == tag) {	// INWN2Instance.Name is the object's tag.
						numberOfTagsFound++;
					}
				}				
			}
			
			if (numberOfTagsFound == 0) {
				return false;
			}
			else if (numberOfTagsFound > 1) {
				throw new ArgumentException("The tag was not unique - more than one object in this area has the tag '" + tag + "'.");
			}
			else {
				return true;
			}
		}
		
		/// <summary>
		/// Check if an object exists in this chapter which has the given tag, and can take part in conversations.
		/// </summary>
		/// <param name="tag">The tag to search for</param>
		/// <returns>True if an object with this tag exists in this chapter and can speak; false otherwise.</returns>		
		public bool ContainsSpeakingObject(string tag)
		{
			// Creatures, doors and placeables can all take part in conversations and own their own conversations:
			foreach (NWN2CreatureInstance creature in this.Area.Creatures) {
				if (creature.Tag == tag) {
					return true; // note that creature must always be alive
				}
			}
			foreach (NWN2DoorInstance door in this.Area.Doors) {
				if (door.Tag == tag) {
					return true;
				}
			}
			foreach (NWN2PlaceableInstance placeable in this.Area.Placeables) {
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
		public bool ContainsAnimatableObject(string tag)
		{
			// Currently looks like there might be no good way to tell whether this is true without lots of manual work.
			// All the animations are under Data/lod_merged*.zip, and different creatures have different sets of animations.
			// Many seem to have a few (e.g. bats and badgers seem to have around 10), while humans have almost all.
			// Might just have to warn players that animations are most likely to work on humanoid characters.
			
			foreach (NWN2CreatureInstance creature in this.Area.Creatures) {
				if (creature.Tag == tag) {
					return true;
				}
			}	
			return false;
		}
		
		
		private void AddedGameObject(OEICollectionWithEvents cList, int index, object value)
		{
			INWN2Instance instance = value as INWN2Instance;
			if (instance != null) {
				Log.WriteAction(Log.Action.added,Log.GetNWN2TypeName(value),instance.Name);
			}
			else {
				Log.WriteAction(Log.Action.added,Log.GetNWN2TypeName(value));
			}
		}

		
		private void DeletedGameObject(OEICollectionWithEvents cList, int index, object value)
		{
			INWN2Instance instance = value as INWN2Instance;
			if (instance != null) {
				Log.WriteAction(Log.Action.deleted,Log.GetNWN2TypeName(value),instance.Name);
			}
			else {
				Log.WriteAction(Log.Action.deleted,Log.GetNWN2TypeName(value));
			}
		}	
	}
}
