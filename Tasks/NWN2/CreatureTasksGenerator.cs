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
using OEIShared.IO;
using form = NWN2Toolset.NWN2ToolsetMainForm;

namespace AdventureAuthor.Tasks.NWN2
{
	/// <summary>
	/// Check the current module for any instances of creatures which are hostile
	/// but have conversations attached, and create a task about each one.
	/// </summary>
	public class CreatureTasksGenerator : ITaskGenerator
	{
		#region Constants
		
		private const string HOSTILESPEAKERS = "Check for hostile speakers";
		private const string MISSINGCONVERSATIONS = "Check for missing conversations";
		private const string HOSTILESPEAKERSDESCRIPTION = "Check for creatures which have " +
			"conversations attached, but which are hostile and will fight the player instead of speaking.";
		private const string MISSINGCONVERSATIONSDESCRIPTION = "Check for creatures which have " +
			"have conversations where the conversation file is missing, or the filename has been entered wrongly.";
		
		#endregion
		
		#region Properties and fields
				
		/// <summary>
		/// All of the various criteria which can be applied to generate tasks.
		/// </summary>
		private Dictionary<string,Criterion> criteria;
		
		
		/// <summary>
		/// True to generate a task for each hostile creature
		/// which has a conversation; false to ignore this factor.
		/// </summary>
		private bool checkForHostileSpeakers;
		public bool CheckForHostileSpeakers {
			get { return criteria[HOSTILESPEAKERS].Include; }
			set { criteria[HOSTILESPEAKERS].Include = value; }
		}
		
		
		/// <summary>
		/// True to generate a task for each creature who has a conversation
		/// with an unrecognised name; false to ignore this factor.
		/// </summary>
		private bool checkForUnrecognisedConversations;
		public bool CheckForUnrecognisedConversations {
			get { return criteria[MISSINGCONVERSATIONS].Include; }
			set { criteria[MISSINGCONVERSATIONS].Include = value; }
		}
		
		#endregion
		
		#region Constructors
		
		/// <summary>
		/// Construct an CreatureTasksGenerator object which will generate tasks for the My Tasks
		/// application based on perceived problems with the creatures in the current module.
		/// </summary>
		/// <param name="checkForHostileSpeakers">True to generate 
		/// a task for each hostile creature which has a conversation; 
		/// false to ignore this factor.</param>
		/// <param name="checkForUnrecognisedConversations">True to generate 
		/// a task for each creature who has a conversation
		/// with an unrecognised name; false to ignore this factor.</param>
		public CreatureTasksGenerator(bool checkForHostileSpeakers,
		                              bool checkForUnrecognisedConversations)
		{
			Criterion hostileSpeakers = new Criterion(HOSTILESPEAKERS,
					                                  HOSTILESPEAKERSDESCRIPTION,
					                                  checkForHostileSpeakers,
					                                  null);
			Criterion missingConversations = new Criterion(MISSINGCONVERSATIONS,
					                       		           MISSINGCONVERSATIONSDESCRIPTION,
					                            	       checkForUnrecognisedConversations,
					                                	   null);
					
			this.criteria = new Dictionary<string,Criterion>{{HOSTILESPEAKERS,hostileSpeakers},
													   		 {MISSINGCONVERSATIONS,missingConversations}};
		}
		
		
		/// <summary>
		/// Construct an CreatureTasksGenerator object which will generate tasks for the My Tasks
		/// application based on perceived problems with the creatures in the current module.
		/// </summary>
		public CreatureTasksGenerator() : this(true,true)
		{
			
		}
		
		#endregion
		
		#region Methods
		
		public List<Task> GetTasks()
		{
			if (!ModuleHelper.ModuleIsOpen()) {
				throw new InvalidOperationException("No module is open to generate tasks.");
			}
			
			List<Task> tasks = new List<Task>();
			
			foreach (NWN2GameArea area in form.App.Module.Areas.Values) {
				area.Demand();
				
				foreach (NWN2CreatureInstance creature in area.Creatures) {
										
					if (CheckForUnrecognisedConversations) {
						// If there's no conversation (or an invalid filename), Conversation is a 
						// MissingResourceEntry, otherwise it's a DirectoryResourceEntry.
						if (creature.Conversation.ToString() != String.Empty &&
						    creature.Conversation.GetType() == typeof(MissingResourceEntry)) {
							Task task = new Task(creature.Name + " (in area '" + area.Name + "') is supposed " +
							                     "to have a conversation called '" + creature.Conversation.ToString() +
							                     "', but there aren't any conversations with that name.",
												 "Bugs",
												 TaskOrigin.SoftwareSuggestion);
							tasks.Add(task);
						}
					}
					
					if (CheckForHostileSpeakers) {
						if (creature.Conversation.ToString() != String.Empty && creature.FactionID == 1) {
							Task task = new Task(creature.Name + " (in area '" + area.Name + "') has a " +
												 "conversation, but their faction is Hostile, so they'll " +
												 "attack instead of speaking.",
												 "Bugs",
												 TaskOrigin.SoftwareSuggestion);
							tasks.Add(task);
						}
					}
				}
				
				area.Release();
			}		
			
			return tasks;
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
}
