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
using System.Collections.ObjectModel;
using AdventureAuthor.Core;
using AdventureAuthor.Setup;
using AdventureAuthor.Tasks;
using AdventureAuthor.Utils;
using NWN2Toolset;
using NWN2Toolset.NWN2;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.Blueprints;
using NWN2Toolset.NWN2.Data.Instances;
using NWN2Toolset.NWN2.Data.Templates;
using NWN2Toolset.NWN2.Views;
using form = NWN2Toolset.NWN2ToolsetMainForm;

namespace AdventureAuthor.Tasks.NWN2
{
	/// <summary>
	/// Check the current module for any areas which are not accessible via
	/// an area transition, player start location or (non-custom) teleportation script.
	/// </summary>
	/// <remarks>It's difficult to confirm that an area isn't accessible - the designer
	/// could use custom scripts etc. - so areas are described as 'apparently' inaccessible.
	/// </remarks>
	public class CheckAllAreasAreReachableAndStartLocationIsPlaced : ITaskGenerator
	{
		public CheckAllAreasAreReachableAndStartLocationIsPlaced()
		{
		}
		
		
		public List<Task> GetTasks()
		{			
			if (!ModuleHelper.ModuleIsOpen()) {
				throw new InvalidOperationException("No module is open to generate tasks.");
			}
			
			List<Task> tasks = new List<Task>();
			
			// Collect the names of all areas, and remove any that seem to be accessible:
			List<string> areaNames = new List<string>(form.App.Module.Areas.Count);			
			foreach (string areaName in form.App.Module.Areas.Keys) {
				areaNames.Add(areaName.ToLower());
			}			
			
			// Check whether the module has a start location. If it does, remove the start
			// area from the list of possibly inaccessible areas; if it doesn't, create a
			// task about placing a start location.
			if (form.App.Module.ModuleInfo.EntryArea == null ||
			    !form.App.Module.ModuleInfo.EntryArea.FullName.ToLower().EndsWith(".are"))
			{
				Task task = new Task("Remember to put down a start location for this module.",
				                     new ObservableCollection<string>{"Bugs"},
				                     TaskOrigin.FixingPossibleError.ToString(),
				                     TaskState.NotCompleted);
				tasks.Add(task);
			}
			else {
				string entryAreaName = form.App.Module.ModuleInfo.EntryArea.FullName.ToLower();
				if (entryAreaName.EndsWith(".are")) {
					entryAreaName = entryAreaName.Substring(0,entryAreaName.Length-4);
					if (areaNames.Contains(entryAreaName)) {
						areaNames.Remove(entryAreaName);
					}
				}		
			}		
			
			// Check every door and trigger in every area for transitions:
			foreach (NWN2GameArea area in form.App.Module.Areas.Values) {
				area.Demand();					
				
//				foreach (NWN2DoorInstance door in area.Doors) {			
//					NWN2DoorTemplate doorTemplate = (NWN2DoorTemplate)door.Template;
//					if (doorTemplate.LinkedTo != null && doorTemplate.LinkedTo != String.Empty) {
//						if (areaNames.Contains(doorTemplate.LinkedTo)) {
//							areaNames.Remove(doorTemplate.LinkedTo);
//							if (areaNames.Count == 0) { 
//								break;
//							}
//						}
//					}
//				}
//				foreach (NWN2TriggerInstance trigger in area.Triggers) {
//					NWN2TriggerTemplate triggerTemplate = (NWN2TriggerTemplate)trigger.Template;
//					if (triggerTemplate.LinkedTo != null && triggerTemplate.LinkedTo != String.Empty) {
//						if (areaNames.Contains(triggerTemplate.LinkedTo)) {
//							areaNames.Remove(triggerTemplate.LinkedTo);
//							if (areaNames.Count == 0) {
//								break;
//							}
//						}
//					}
//				}
				
				area.Release();
			}		
			
			// Any area names which remain may not be accessible - create a task about them:
			foreach (string inaccessibleAreaName in areaNames) {
				Task task = new Task("Area '" + inaccessibleAreaName + "' doesn't seem to be reachable by the player.");
				task.Origin = TaskOrigin.FixingPossibleError.ToString();
				tasks.Add(task);
			}
			
			return tasks;
			
			
			
			/*
			 * 1. Need to check whether transition is set up properly. Check LinkedToType,
			 * and whether the other end of the transition exists and is of the correct type.
			 * 
			 * 3. door.Template isn't the NWN2DoorTemplate as I thought, but a ZipResourceEntry?
			 * This means the door and trigger checking code doesn't work.
			 * 
			 * */
		}
	}
}
