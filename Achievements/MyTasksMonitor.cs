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
using System.ComponentModel;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Achievements
{
	/// <summary>
	/// Tracks the user's level of activity with the My Tasks
	/// application (represented by an abstract
	/// number which increases with My Tasks usage) and grants 
	/// awards when the total reaches certain levels.
	/// </summary>
	public class MyTasksMonitor : UserActivityMonitor
	{
		#region Constants
				
		/// <summary>
		/// The subject being monitored.
		/// </summary>
		public const string SUBJECT = "My Tasks activity";
		
		#endregion
		
		#region Constructors
				
		/// Construct an object which tracks the user's use
		/// of the My Tasks application and grants awards when it
		/// has reached certain levels.
		public MyTasksMonitor() : base()
		{
		}
		
		
		/// Construct an object which tracks the user's use
		/// of the My Tasks application and grants awards when it
		/// has reached certain levels.
		/// <param name="userActivityToDate">The initial user activity level.</param>
		public MyTasksMonitor(uint userActivityToDate) : base(userActivityToDate)
		{
		}

		#endregion
		
		#region Methods
				
		/// <summary>
		/// Get the name of the pipe to listen to in order to receive
		/// messages about user activity in the monitored application.
		/// </summary>
		/// <returns>A string with the name of the pipe to listen to.</returns>
		protected override string GetPipeName()
		{
			return PipeCommunication.MYTASKSLOG;
		}
		
		
		/// <summary>
		/// Analyse a log message to determine what user action it 
		/// represents, and return a value representing the number
		/// of points that should be awarded for that action.
		/// </summary>
		/// <param name="message">The log message to parse.</param>
		/// <returns>A value representing the number
		/// of points that should be awarded for that action.</returns>
		/// <remarks>Any message passed in may return some number 
		/// of points - if it can't be identified as representing
		/// a specific user action it will be taken as a 
		/// miscellaneous action and awarded accordingly.</remarks>
		protected override ushort GetActivityPoints(string message)
		{
			// Assign points based on how 'valuable' the action is:
			ushort points;
			
			// Only give points once they've actually written a task,
			// not simply for adding a blank one:
			if (message.Contains("edited task")) {	
				points = 7;
			}
			else if (message.Contains("completed task")) {
				points = 3;
			}
			// Exclude anything that racks up points too quickly or which
			// could be repeatedly performed without conscious thought
			// (e.g. mindlessly clicking Show/Hide):
			else if (message.Contains("mytasks_")) { // i.e. ignore all changes to filter mode
				points = 0;
			}
			else {
				points = 1;
			}
			
			return points;
		}
		
		
		/// <summary>
		/// Gets the name of the monitored subject.
		/// </summary>
		/// <returns>The name of the monitored subject e.g. 'Word count'.</returns>
		public override string GetSubjectName()
		{
			return SUBJECT;
		}
		
		#endregion
	}
}
