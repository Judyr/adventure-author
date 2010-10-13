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
	/// Monitors a particular aspect of user activity and tracks
	/// information about it in the form of an abstract 'user activity' score.
	/// </summary>
	public abstract class UserActivityMonitor : AchievementMonitor
	{		
		#region Properties and fields
						
		protected uint userActivity;	
		/// <summary>
		/// A number which roughly represents the extent to which the user
		/// has used the application - a higher number indicates
		/// a greater amount of time/engagement with the application.
		/// </summary>
		/// <remarks>Each action taken by the user in adds some
		/// points to this total, with some actions counting for 
		/// more than others.</remarks>
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
		/// of an application and grants awards when it
		/// has reached certain levels.
		public UserActivityMonitor() : this(0)
		{
		}
		
		
		/// Construct an object which tracks the user's use
		/// of an application and grants awards when it
		/// has reached certain levels.
		/// <param name="userActivityToDate">The initial user activity level.</param>
		public UserActivityMonitor(uint userActivityToDate) : base()
		{
			this.userActivity = userActivityToDate;
			PipeCommunication.MessageReceived += TallyUserActivity;
		}

		#endregion
		
		#region Methods
		
		/// <summary>
		/// Get the name of the pipe to listen to in order to receive
		/// messages about user activity in the monitored application.
		/// </summary>
		/// <returns>A string with the name of the pipe to listen to.</returns>
		protected abstract string GetPipeName();
		
		
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
		protected abstract ushort GetActivityPoints(string message);
		
		
		/// <summary>
		/// For every logged message from the application, analyse the message to
		/// determine which user action it represents and increase the
		/// tally of 'user activity points' accordingly.
		/// </summary>
		protected void TallyUserActivity(object sender, MessageReceivedEventArgs e)
		{
			if (e.Source == GetPipeName()) {
				uint points = GetActivityPoints(e.Message);
				AddToUserActivity(points);
			}
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
		
		#endregion
	}
}
