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
using AdventureAuthor.Core;
using AdventureAuthor.Setup;
using AdventureAuthor.Tasks;
using NWN2Toolset;
using NWN2Toolset.NWN2;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.Blueprints;
using NWN2Toolset.NWN2.Data.Instances;
using form = NWN2Toolset.NWN2ToolsetMainForm;

namespace AdventureAuthor.Tasks.NWN2
{
	/// <summary>
	/// Check the current module for any instances of creatures which are hostile
	/// but have conversations attached, and create a task about each one.
	/// </summary>
	public class CreatureTasksGenerator : ITaskGenerator
	{
		public CreatureTasksGenerator()
		{
		}
		
		
		public List<Task> GetTasks()
		{
			if (!ModuleHelper.ModuleIsOpen()) {
				throw new InvalidOperationException("No module is open to generate tasks.");
			}
			
			List<Task> tasks = new List<Task>();
			
			foreach (NWN2GameArea area in form.App.Module.Areas.Values) {
				area.Demand();
				List<NWN2CreatureInstance> hostileSpeakers = NWN2Utils.GetHostileSpeakers(area);
				foreach (NWN2CreatureInstance creature in hostileSpeakers) {
					Task task = new Task();
					task.Origin = TaskOrigin.FixingError.ToString();
					task.Description = creature.Name + " (in area '" + area.Name + "') has a " +
						"conversation, but their faction is Hostile, so they'll attack instead of speaking.";
					tasks.Add(task);
				}
				area.Release();
			}		
			
			return tasks;
		}
	}
}