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
	/// Tracks the user's level of activity with the Fridge Magnets
	/// application (represented by an abstract
	/// number which increases with Fridge Magnets usage) and grants 
	/// awards when the total reaches certain levels.
	/// </summary>
	public class FridgeMagnetsMonitor : AchievementMonitor
	{
		#region Constants
				
		/// <summary>
		/// The subject being monitored.
		/// </summary>
		public const string SUBJECT = "Fridge Magnets activity";
		
		#endregion
		
		#region Properties and fields
				
		protected uint userActivity;	
		/// <summary>
		/// A number which roughly represents the extent to which the user
		/// has used the Fridge Magnets application - a higher number indicates
		/// a greater amount of time/engagement with the application.
		/// </summary>
		/// <remarks>Each action taken by the user in Fridge Magnets adds some
		/// points to this total, with some actions counting more than others
		/// (e.g. adding a magnet counts for more than deleting one.)</remarks>
		public uint UserActivity {
			get { 
				lock (padlock) {
					return userActivity;
				}
			}
			set { 
				lock (padlock) {
					userActivity = value;
					OnSubjectChanged(new PropertyChangedEventArgs("UserActivity"));
				}
			}
		}
		
		#endregion
		
		#region Constructors
				
		/// Construct an object which tracks the user's use
		/// of the Fridge Magnets application and grants awards when it
		/// has reached certain levels.
		public FridgeMagnetsMonitor() : this(0)
		{
		}
		
		
		/// Construct an object which tracks the user's use
		/// of the Fridge Magnets application and grants awards when it
		/// has reached certain levels.
		/// <param name="userActivityToDate">The initial user activity level.</param>
		public FridgeMagnetsMonitor(uint userActivityToDate) : base()
		{
			this.userActivity = userActivityToDate;
			PipeCommunication.MessageReceived += TallyUserActivity;
		}

		#endregion
		
		#region Methods
		
		/// <summary>
		/// For every logged message from Fridge Magnets, analyse the message to
		/// determine which user action it represents and increase the
		/// tally of 'user activity points' accordingly.
		/// </summary>
		private void TallyUserActivity(object sender, MessageReceivedEventArgs e)
		{
			if (e.Source == PipeCommunication.FRIDGEMAGNETSLOG) {				
				ushort points = GetActivityPoints(e.Message);			
				AddToUserActivity(points);
			}
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
		private ushort GetActivityPoints(string message)
		{
			// Assign points based on how 'valuable' the action is:
			ushort points;			
						
			// Don't give the big points to ideas which are simply blueprints sent from the toolset:
			if ((message.Contains("added idea") && !message.Contains("added from blueprints")) || 
			     message.Contains("edited idea")) {
				points = 7;
			}	
			// Exclude anything that racks up points too quickly or which
			// could be repeatedly performed without conscious thought
			// (e.g. mindlessly clicking Show/Hide):
			else if (message.Contains("hid ideacategory") || message.Contains("show ideacategory") ||
			         message.Contains("set magnetboardcolour")) {
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
		
		
		/// <summary>
		/// Get the type of information being recorded about the monitored subject.
		/// </summary>
		/// <returns>The Type of information being recorded about the monitored subject.</returns>
		/// <remarks>Any award being added to this AchievementMonitor must return a value
		/// for GetCriteriaType() which is identical to the value returned by this method.</remarks>
		public override Type GetSubjectType()
		{
			return typeof(uint);
		}
				
		
		/// <summary>
		/// Get the current recorded information on the monitored subject.
		/// </summary>
		/// <returns>The information being tracked about the monitored subject.</returns>
		public override object GetSubjectValue()
		{
			return UserActivity;
		}
		
		
		/// <summary>
		/// Add to the user activity level total. 
		/// </summary>
		/// <param name="userActivity">The number of 'points' to add to the user activity level total.</param>
		public void AddToUserActivity(uint userActivity)
		{
			if (UserActivity < uint.MaxValue && UserActivity + userActivity > uint.MaxValue) {
				UserActivity = uint.MaxValue;
			}
			else {
				UserActivity += userActivity;
			}
		}
		
		#endregion
	}
}
