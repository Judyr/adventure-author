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
using System.Text;
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
	public class AreaTasksGenerator : ITaskGenerator
	{
		/// <summary>
		/// True to generate a task for each area transition which has been
		/// improperly set up; false to ignore this factor.
		/// </summary>
		private bool checkForBrokenAreaTransitions;
		public bool CheckForBrokenAreaTransitions {
			get { return checkForBrokenAreaTransitions; }
			set { checkForBrokenAreaTransitions = value; }
		}
		
		
		/// <summary>
		/// True to generate a task if a start location
		/// has not been placed, or if there are areas which are inaccessible from the start location; 
		/// false to ignore this factor.
		/// </summary>
		private bool checkThatAllAreasCanBeReached;
		public bool CheckThatAllAreasCanBeReached {
			get { return checkThatAllAreasCanBeReached; }
			set { checkThatAllAreasCanBeReached = value; }
		}
		
		
		List<Task> tasks;
		
		Dictionary<string,Area> areas;		
		Dictionary<string,NWN2DoorInstance> entranceDoors = new Dictionary<string,NWN2DoorInstance>();
		Dictionary<string,NWN2TriggerInstance> entranceTriggers = new Dictionary<string,NWN2TriggerInstance>();
		Dictionary<string,NWN2DoorInstance> exitDoors = new Dictionary<string,NWN2DoorInstance>();
		Dictionary<string,NWN2WaypointInstance> exitWaypoints = new Dictionary<string,NWN2WaypointInstance>();
		
		private int areasReached = 0;
		
		
		public AreaTasksGenerator() : this(true,true)
		{			
		}
		
		
		/// <summary>
		/// Construct an AreaTasksGenerator object which will generate tasks for the My Tasks
		/// application based on perceived problems with the current module.
		/// </summary>
		/// <param name="checkForBrokenAreaTransitions">True to generate a task for each area transition which has been
		/// improperly set up; false to ignore this factor.</param>
		/// <param name="checkThatAllAreasCanBeReached">True to generate a task if a start location
		/// has not been placed, or if there are areas which are inaccessible from the start location; false to ignore
		/// this factor.</param>
		public AreaTasksGenerator(bool checkForBrokenAreaTransitions,
		                          bool checkThatAllAreasCanBeReached)
		{
			this.checkForBrokenAreaTransitions = checkForBrokenAreaTransitions;
			this.checkThatAllAreasCanBeReached = checkThatAllAreasCanBeReached;
		}
		
		
		public List<Task> GetTasks()
		{			
			if (!ModuleHelper.ModuleIsOpen()) {
				throw new InvalidOperationException("No module is open to generate tasks.");
			}
			
			tasks = new List<Task>();
			
			if (!CheckForBrokenAreaTransitions && !CheckThatAllAreasCanBeReached) {
				return tasks;
			}
								
			areas = new Dictionary<string, Area>(form.App.Module.Areas.Count);
			foreach (NWN2GameArea nwn2area in form.App.Module.Areas.Values) {
				Area areaInfo = new Area(nwn2area.Name.ToLower());
				areas.Add(areaInfo.Name,areaInfo);
			}
			
			// Check whether the module has a start location. If it does, note the 
			// start area - otherwise, add a 'place start location' task.
			Area startingArea = null;
			if (CheckThatAllAreasCanBeReached) {
				if (form.App.Module.ModuleInfo.EntryArea == null ||
				    !form.App.Module.ModuleInfo.EntryArea.FullName.ToLower().EndsWith(".are"))
				{				
					Task task = new Task("The module needs to have a player start location added.",
					                     "Bugs",
					                     TaskOrigin.FixingPossibleError.ToString());
					tasks.Add(task);
				}
				else {
					// Remove '.are' suffix and set to lowercase:
					string filename = form.App.Module.ModuleInfo.EntryArea.FullName;
					startingArea = areas[filename.Substring(0,filename.Length-4).ToLower()];
				}		
			}
			
			
			// In the first pass, collect all of the entrance doors and triggers, 
			// as well as collecting the *names* of the exit doors and waypoints.
			List<string> doorsNamedAsExits = new List<string>();
			List<string> waypointsNamedAsExits = new List<string>();
			
			foreach (NWN2GameArea area in form.App.Module.Areas.Values) {
				area.Demand();	
				
				foreach (NWN2DoorInstance door in area.Doors) {					
					string namedExit = door.LinkedTo;
					if (namedExit != null && namedExit != String.Empty) {						
						entranceDoors.Add(door.Name,door);
						
						switch (door.LinkedToType) {								
							case DoorTransitionType.LinkToDoor:
								doorsNamedAsExits.Add(namedExit);
								break;
								
							case DoorTransitionType.LinkToWaypoint:
								waypointsNamedAsExits.Add(namedExit);
								break;
								
							case DoorTransitionType.NoLink:
								if (CheckForBrokenAreaTransitions) {
									Task task = new Task("The door '" + door.Name + 
									                     "' (in area '" + door.Area.Name + "') has an area transition, but " +
									                     "it won't work unless the property 'Link Object Type' is set to " +
									                     "either 'Transition to a waypoint' or 'Transition to a door'.",
									                     "Bugs",
									                     TaskOrigin.FixingError.ToString());
									tasks.Add(task);							
								}
								break;
						}
					}
				}
				
				foreach (NWN2TriggerInstance trigger in area.Triggers) {					
					string namedExit = trigger.LinkedTo;
					if (namedExit != null && namedExit != String.Empty) {						
						entranceTriggers.Add(trigger.Name,trigger);
						
						switch (trigger.LinkedToType) {								
							case DoorTransitionType.LinkToDoor:
								doorsNamedAsExits.Add(namedExit);
								break;
								
							case DoorTransitionType.LinkToWaypoint:
								waypointsNamedAsExits.Add(namedExit);
								break;
								
							case DoorTransitionType.NoLink:
								if (CheckForBrokenAreaTransitions) {
									Task task = new Task("The trigger '" + trigger.Name + 
									                     "' (in area '" + trigger.Area.Name + "') has an area transition, but " +
									                     "it won't work unless the property 'Link Object Type' is set to " +
									                     "either 'Transition to a waypoint' or 'Transition to a door'.",
									                     "Bugs",
									                     TaskOrigin.FixingError.ToString());
									tasks.Add(task);							
								}
								break;
						}
					}
				}
				
				area.Release();
			}
			
			
			// In the second pass, collect the exit doors and waypoints:			
			foreach (NWN2GameArea area in form.App.Module.Areas.Values) {
				area.Demand();	
				
				foreach (NWN2DoorInstance door in area.Doors) {
					if (doorsNamedAsExits.Contains(door.Name)) {
						if (!exitDoors.ContainsValue(door)) {
							exitDoors.Add(door.Name,door);
						}
						doorsNamedAsExits.Remove(door.Name);
						if (doorsNamedAsExits.Count == 0) {
							break;
						}
					}					
				}
				
				foreach (NWN2WaypointInstance waypoint in area.Waypoints) {
					if (waypointsNamedAsExits.Contains(waypoint.Name)) {
						if (!exitWaypoints.ContainsValue(waypoint)) {
							exitWaypoints.Add(waypoint.Name,waypoint);
						}
						waypointsNamedAsExits.Remove(waypoint.Name);
						if (waypointsNamedAsExits.Count == 0) {
							break;
						}
					}					
				}
				
				area.Release();
			}
			
			
			// Then build a graph of areas linked to other areas:				
			foreach (NWN2DoorInstance entrance in entranceDoors.Values) {	
				LinkAreas(entrance.Name,
				          "door",
						  entrance.Area.Name.ToLower(),
				          entrance.LinkedTo,
				          entrance.LinkedToType);
			}			
			foreach (NWN2TriggerInstance entrance in entranceTriggers.Values) {		
				LinkAreas(entrance.Name,
				          "trigger",
						  entrance.Area.Name.ToLower(),
				          entrance.LinkedTo,
				          entrance.LinkedToType);
			}
			
			
			// Check whether all of the nodes in the graph are reachable:
			if (CheckThatAllAreasCanBeReached && startingArea != null) {
				TryToReachAllAreas(startingArea);
			
				if (areasReached < areas.Count) {
					StringBuilder description = new StringBuilder("Some areas in this module (");
					foreach (Area area in areas.Values) {
						if (!area.Reached) {
							description.Append("'" + area.Name + "', ");
						}
					}
					description.Remove(description.Length-2,2); // remove last comma and space
					description.Append(" can't be reached from the start location. Add area transitions " +
					                  "to make sure the player can get to all the areas.");
					Task task = new Task(description.ToString(),
					                     "Bugs",
					                     TaskOrigin.FixingPossibleError.ToString());
					tasks.Add(task);
				}
			}
			
			return tasks;
		}
		
		
		private void TryToReachAllAreas(Area area)
		{
			area.Reached = true;
			areasReached++;
			foreach (Area linkedArea in area.LeadsTo) {
				if (!linkedArea.Reached) {
					TryToReachAllAreas(linkedArea);
				}
			}
		}
		
		
		private void LinkAreas(string entranceName, string entranceType, string entranceArea, string exitName, DoorTransitionType LinkedToType)
		{		
			bool foundExitDoor = exitDoors.ContainsKey(exitName);
			bool foundExitWaypoint = exitWaypoints.ContainsKey(exitName);
				
			switch (LinkedToType) {
				case DoorTransitionType.LinkToDoor:
					if (foundExitDoor) {
						NWN2DoorInstance exit = exitDoors[exitName];
						string exitAreaName = exit.Area.Name.ToLower();
						areas[entranceArea].LeadsTo.Add(areas[exitAreaName]);
					}
					else if (CheckForBrokenAreaTransitions) {
						if (foundExitWaypoint) {
							Task task = new Task("The area transition on " + entranceType + " '" + entranceName + "' " +
							                     "(in area '" + entranceArea + "') is linked to a waypoint - it won't work unless " +
							                     "the property 'Link Object Type' is set to 'Transition to a waypoint'.",
							                     "Bugs",
							                     TaskOrigin.FixingError.ToString());
							tasks.Add(task);
						}
						else {		
							Task task = new Task("The " + entranceType + " '" + entranceName + "' (in area '" +
							                     entranceArea + "') has an area transition which links to '" +
							                     exitName + "' - but there is no door or waypoint with that tag.",
							                     "Bugs",
							                     TaskOrigin.FixingError.ToString());
							tasks.Add(task);
						}
					}
					break;
					
				case DoorTransitionType.LinkToWaypoint:
					if (foundExitWaypoint) {
						NWN2WaypointInstance exit = exitWaypoints[exitName];
						string exitAreaName = exit.Area.Name.ToLower();
						areas[entranceArea].LeadsTo.Add(areas[exitAreaName]);
					}
					else if (CheckForBrokenAreaTransitions) {
						if (foundExitDoor) {	
							Task task = new Task("The area transition on " + entranceType + " '" + entranceName + "' " +
							                     "(in area '" + entranceArea + "') is linked to a door - it won't work unless " +
							                     "the property 'Link Object Type' is set to 'Transition to a door'.",
							                     "Bugs",
							                     TaskOrigin.FixingError.ToString());
							tasks.Add(task);
						}
						else {							
							Task task = new Task("The " + entranceType + " '" + entranceName + "' (in area '" +
							                     entranceArea + "') has an area transition which links to '" +
							                     exitName + "' - but there is no door or waypoint with that tag.",
							                     "Bugs",
							                     TaskOrigin.FixingError.ToString());
							tasks.Add(task);
						}
					}
					break;
			}
		}
	}	
	

	/// <summary>
	/// Represents information about a NWN2GameArea object.
	/// </summary>
	public class Area
	{
		public string Name;
		public List<Area> LeadsTo;
		public bool Reached;
		
		public Area(string name)
		{
			this.Name = name;
			this.LeadsTo = new List<Area>();
			this.Reached = false;
		}
	}
}
