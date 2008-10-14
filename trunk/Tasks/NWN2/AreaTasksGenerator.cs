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
		#region Constants
		
		private const string BROKENTRANSITIONS = "Check for broken area transitions";
		private const string ALLAREASREACHABLE = "Check that all areas can be reached";
		private const string BROKENTRANSITIONSDESCRIPTION = "Check for any area transitions which lead to nowhere, " +
			"lead to themselves, or have the wrong transition type selected.";
		private const string ALLAREASREACHABLEDESCRIPTION = "Check that the module has a player start location, " +
			"and that the player can reach all the areas in the module via working area transitions.";
		
		#endregion
		
		#region Fields
		
		/// <summary>
		/// All of the various criteria which can be applied to generate tasks.
		/// </summary>
		private Dictionary<string,Criterion> criteria;
		
		
		/// <summary>
		/// True to generate a task for each area transition which has been
		/// improperly set up; false to ignore this factor.
		/// </summary>
		private bool checkForBrokenAreaTransitions;
		public bool CheckForBrokenAreaTransitions {
			get { return criteria[BROKENTRANSITIONS].Include; }
			set { criteria[BROKENTRANSITIONS].Include = value; }
		}
		
		
		/// <summary>
		/// True to generate a task if a start location
		/// has not been placed, or if there are areas which are inaccessible from the start location; 
		/// false to ignore this factor.
		/// </summary>
		private bool checkThatAllAreasCanBeReached;
		public bool CheckThatAllAreasCanBeReached {
			get { return criteria[ALLAREASREACHABLE].Include; }
			set { criteria[ALLAREASREACHABLE].Include = value; }
		}
		
		Dictionary<string,Area> areas;		
		Dictionary<string,NWN2DoorInstance> exitDoors = new Dictionary<string,NWN2DoorInstance>();
		Dictionary<string,NWN2WaypointInstance> exitWaypoints = new Dictionary<string,NWN2WaypointInstance>();
		
		#endregion
		
		#region Constructors
		
		/// <summary>		
		/// Construct an AreaTasksGenerator object which will generate tasks for the My Tasks
		/// application based on perceived problems with the areas in the current module.
		/// </summary>
		public AreaTasksGenerator() : this(true,true)
		{			
		}
		
		
		/// <summary>		
		/// Construct an AreaTasksGenerator object which will generate tasks for the My Tasks
		/// application based on perceived problems with the areas in the current module.
		/// </summary>
		/// <param name="checkForBrokenAreaTransitions">True to generate a task for each area transition which has been
		/// improperly set up; false to ignore this factor.</param>
		/// <param name="checkThatAllAreasCanBeReached">True to generate a task if a start location
		/// has not been placed, or if there are areas which are inaccessible from the start location; false to ignore
		/// this factor.</param>
		public AreaTasksGenerator(bool checkForBrokenAreaTransitions,
		                          bool checkThatAllAreasCanBeReached)
		{
			Criterion brokenTransitions = new Criterion(BROKENTRANSITIONS,
					                                    BROKENTRANSITIONSDESCRIPTION,
					                                    checkForBrokenAreaTransitions,
					                                    null);
			Criterion allAreasReachable = new Criterion(ALLAREASREACHABLE,
					                                    ALLAREASREACHABLEDESCRIPTION,
					                                    checkThatAllAreasCanBeReached,
					                                    null);
			
			this.criteria = new Dictionary<string,Criterion>{{BROKENTRANSITIONS,brokenTransitions},
													   		 {ALLAREASREACHABLE,allAreasReachable}};
		}
		
		#endregion
		
		#region Methods
		
//		public List<Task> GetTasks(List<Criteria> chosenCriteria)
//		{
//			foreach (Criterion chosenCriterion in chosenCriteria) {
//				if (this.criteria.ContainsKey(chosenCriterion.Description)) {
//					this.criteria[chosenCriterion.Description].Include = chosenCriterion.Include;
//				}
//			}
//			GetTasks();
//		}
		
		
		public List<Task> GetTasks()
		{			
			if (!ModuleHelper.ModuleIsOpen()) {
				throw new InvalidOperationException("No module is open to generate tasks.");
			}
			
			List<Task> tasks = new List<Task>();
			
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
					                     TaskOrigin.SoftwareSuggestion);
					tasks.Add(task);
				}
				else {
					// Remove '.are' suffix and set to lowercase:
					string filename = form.App.Module.ModuleInfo.EntryArea.FullName;
					startingArea = areas[filename.Substring(0,filename.Length-4).ToLower()];
				}		
			}
			
			
			// In the first pass, collect all of the entrance doors and triggers, 
			// as well as collecting the *tags* of the exit doors and waypoints.
			
			Dictionary<string,INWN2Instance> entrances = new Dictionary<string,INWN2Instance>();
			List<string> exitTags = new List<string>();
			
			foreach (NWN2GameArea area in form.App.Module.Areas.Values) {
				area.Demand();					
				foreach (NWN2DoorInstance door in area.Doors) {
					if (door.LinkedTo != null && door.LinkedTo != String.Empty) {
						if (CheckForBrokenAreaTransitions && door.LinkedTo == door.Tag) {
							Task task = new Task("The door '" + door.Tag + "' (in area '" +
										         door.Area.Name + "') has an area transition " + 
										         "which leads to itself!",
										         "Bugs",
										         TaskOrigin.SoftwareSuggestion);
							tasks.Add(task);
						}
						else {
							entrances.Add(door.Tag,door);
							if (!exitTags.Contains(door.LinkedTo)) {
								exitTags.Add(door.LinkedTo);								
							}
						}
					}
				}				
				foreach (NWN2TriggerInstance trigger in area.Triggers) {
					if (trigger.LinkedTo != null && trigger.LinkedTo != String.Empty) {
						if (CheckForBrokenAreaTransitions && trigger.LinkedTo == trigger.Tag) {
							Task task = new Task("The trigger '" + trigger.Tag + "' (in area '" +
										         trigger.Area.Name + "') has an area transition " + 
										         "which leads to itself!",
										         "Bugs",
										         TaskOrigin.SoftwareSuggestion);
							tasks.Add(task);
						}
						else {
							entrances.Add(trigger.Tag,trigger);
							if (!exitTags.Contains(trigger.LinkedTo)) {
								exitTags.Add(trigger.LinkedTo);								
							}
						}
					}
				}				
				area.Release();
			}			
			
			
			// In the second pass, collect the exit doors/waypoints, including
			// any which were named but were not identified (i.e. user selected 'No transition'
			// but typed in a LinkedTo tag anyway.)
			foreach (NWN2GameArea area in form.App.Module.Areas.Values) {
				area.Demand();					
				foreach (NWN2DoorInstance door in area.Doors) {
					if (exitTags.Contains(door.Tag) && !exitDoors.ContainsKey(door.Tag)) {
						exitDoors.Add(door.Tag,door);
					}
				}							
				foreach (NWN2WaypointInstance waypoint in area.Waypoints) {
					if (exitTags.Contains(waypoint.Tag) && !exitWaypoints.ContainsKey(waypoint.Tag)) {
						exitWaypoints.Add(waypoint.Tag,waypoint);
					}
				}				
				area.Release();
			}
			
			
			// Then build a graph of areas linked to other areas:				
			foreach (INWN2Instance entrance in entrances.Values) {
				Task task = LinkAreas(entrance);
				if (task != null) { // LinkAreas() returns a Task if it finds a broken area transition
					tasks.Add(task);
				}
			}	
			
			
			// Navigate the graph via area transitions, and mark off every area that you reach:
			if (CheckThatAllAreasCanBeReached && startingArea != null) {
				int areasReached = TryToReachAllAreas(startingArea);
			
				if (areasReached < areas.Count) {
					StringBuilder description = new StringBuilder("Some areas in this module (");
					foreach (Area area in areas.Values) {
						if (!area.Reached) {
							description.Append("'" + area.Name + "', ");
						}
					}
					description.Remove(description.Length-2,2); // remove last comma and space
					description.Append(") can't be reached from the start location. Add area transitions " +
					                  "(or fix broken ones!) to make sure the player can get to all the areas.");
					Task task = new Task(description.ToString(),
					                     "Bugs",
					                     TaskOrigin.SoftwareSuggestion);
					tasks.Add(task);
				}
			}
			
			return tasks;
		}
		
		
		private int TryToReachAllAreas(Area area)
		{
			area.Reached = true;
			int reached = 1;
			foreach (Area linkedArea in area.LeadsTo) {
				if (!linkedArea.Reached) {
					reached += TryToReachAllAreas(linkedArea);
				}
			}
			return reached;
		}
		
		
		private Task LinkAreas(INWN2Instance entrance)
		{		
			string LinkedTo;
			DoorTransitionType LinkedToType;
			string entranceType;
			if (entrance is NWN2DoorInstance) {
				NWN2DoorInstance door = (NWN2DoorInstance)entrance;
				LinkedTo = door.LinkedTo;
				LinkedToType = door.LinkedToType;
				entranceType = "door";
			}
			else if (entrance is NWN2TriggerInstance) {
				NWN2TriggerInstance trigger = (NWN2TriggerInstance)entrance;
				LinkedTo = trigger.LinkedTo;
				LinkedToType = trigger.LinkedToType;
				entranceType = "trigger";
			}
			else {
				throw new ArgumentException("entrance must be a NWN2DoorInstance or a NWN2TriggerInstance.","entrance");
			}
			
			// If you can find the door or waypoint that the transition is 
			// supposed to link to, add a link from the area that the 
			// entrance is in to the area that the exit is in:
			bool foundExitDoor = exitDoors.ContainsKey(LinkedTo);
			bool foundExitWaypoint = exitWaypoints.ContainsKey(LinkedTo);
						
			INWN2Instance exit = null;
			if (LinkedToType == DoorTransitionType.LinkToDoor && foundExitDoor) {
				exit = exitDoors[LinkedTo];
			}
			else if (LinkedToType == DoorTransitionType.LinkToWaypoint && foundExitWaypoint) {
				exit = exitWaypoints[LinkedTo];
			}
			
			if (exit != null) {
				areas[entrance.Area.Name].LeadsTo.Add(areas[exit.Area.Name]);
			}
			// Otherwise, if you're checking for broken area transitions, check whether
			// there is a valid exit with the given tag but the user has failed to
			// set LinkedToType properly. If there is, generate a task about it, and if
			// there isn't, generate a task stating that there is no door or waypoint with that tag:
			else if (CheckForBrokenAreaTransitions) {				
				Task task = null;
				if (LinkedToType != DoorTransitionType.LinkToWaypoint && foundExitWaypoint) {
					// there is a WAYPOINT called that..
					task = new Task("The " + entranceType + " '" + entrance.Name + "' (in area '" +
							        entrance.Area.Name + "') has an area transition which links to " + 
							        "waypoint '" + LinkedTo + "' - but its property 'Link Object Type' " + 
							        " needs to be set to 'Transition to a waypoint', or it won't work.",
							        "Bugs",
							        TaskOrigin.SoftwareSuggestion);
				}
				else if (LinkedToType != DoorTransitionType.LinkToDoor && foundExitDoor) {
					// there is a DOOR called that..
					task = new Task("The " + entranceType + " '" + entrance.Name + "' (in area '" +
							        entrance.Area.Name + "') has an area transition which links to " + 
							        "door '" + LinkedTo + "' - but its property 'Link Object Type' " + 
							        " needs to be set to 'Transition to a door', or it won't work.",
							        "Bugs",
							        TaskOrigin.SoftwareSuggestion);
				}
				else {
					// there isn't anything called that..
					task = new Task("The " + entranceType + " '" + entrance.Name + "' (in area '" +
							        entrance.Area.Name + "') has an area transition which links to '" +
							        LinkedTo + "' - but there is no door or waypoint with that tag.",
							        "Bugs",
							        TaskOrigin.SoftwareSuggestion);
				}
				return task;
			}
			return null; //only return a Task if something is broken!
		}
			
			
		public List<Criterion> GetCriteria()
		{
			List<Criterion> availableCriteria = new List<Criterion>(criteria.Keys.Count);
			foreach (Criterion criterion in criteria.Values) {
				availableCriteria.Add(criterion);
			}
			return availableCriteria;
		}
			
		#endregion
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
