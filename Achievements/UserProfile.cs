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
using System.Xml;
using System.Xml.Serialization;
using AdventureAuthor.Setup;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Achievements
{
	/// <summary>
	/// Represents information about the current user relating to their
	/// use of Adventure Author.
	/// </summary>
	[Serializable]
	public class UserProfile
	{		
		#region Properties and fields
		
		/// <summary>
		/// The awards that this user has received as a result
		/// of their toolset activity.
		/// </summary>
		private List<Award> awards;
		[XmlArray]
		public List<Award> Awards {
			get { return awards; }
			set { awards = value; }
		}
		
		
		/// <summary>
		/// Information that is being tracked for this user in order
		/// to grant them awards.
		/// </summary>
		private SerializableDictionary<string,object> trackedInfo;
		public SerializableDictionary<string,object> TrackedInfo {
			get { return trackedInfo; }
			set { trackedInfo = value; }
		}
		
		
		/// <summary>
		/// The total 'narrative' word count of the user.
		/// </summary>
		[XmlIgnore]
		public uint WordCount {
			get { 
				return (uint)GetValue(WordCountMonitor.SUBJECT);
			}
			set {
				SetValue(WordCountMonitor.SUBJECT,value);
			}
		}
		
		
		/// <summary>
		/// The total activity of the user with the My Tasks application.
		/// </summary>
		[XmlIgnore]
		public uint MyTasksUserActivity {
			get { 
				return (uint)GetValue(MyTasksMonitor.SUBJECT);
			}
			set {
				SetValue(MyTasksMonitor.SUBJECT,value);
			}
		}
		
		#endregion
		
		#region Constructors
		
		/// <summary>
		/// For deserialisation.
		/// </summary>
		private UserProfile()
		{
			awards = new List<Award>();
			trackedInfo = new SerializableDictionary<string,object>();
		}
		
		
		/// <summary>
		/// Create a user profile to represent information about the current user 
		/// relating to their use of Adventure Author.
		/// </summary>
		/// <param name="awards">The awards given to this user.</param>
		public UserProfile(List<Award> awards)
		{
			this.awards = awards;
			trackedInfo = new SerializableDictionary<string,object>();
			SetupDefaultValues();
		}
		
		#endregion
	
		#region Methods
		
		/// <summary>
		/// Set up default values for tracked information.
		/// </summary>
		private void SetupDefaultValues()
		{
			WordCount = 0;
			MyTasksUserActivity = 0;
		}
		
		
		/// <summary>
		/// Check whether this user has already received a given award.
		/// </summary>
		/// <param name="award">The award the user may or may not have already received.</param>
		/// <returns>True if the user has already received this award; false otherwise.</returns>
		public bool HasAward(Award award)
		{
			foreach (Award ownedAward in awards) {
				if (ownedAward.GetID() == award.GetID()) {
					return true;
				}
			}
			return false;
		}
		
		
		/// <summary>
		/// Retrieve the value of a piece of tracked information.
		/// </summary>
		/// <param name="infoName">The name that the information
		/// has been stored under, e.g. 'Word Count'.</param>
		/// <returns>The value of the tracked information, or null
		/// if no information with that name has been tracked.</returns>
		public object GetValue(string infoName)
		{
			if (!trackedInfo.ContainsKey(infoName)) {
				return null;
			}
			else {
				return trackedInfo[infoName];
			}
		}
		
		
		/// <summary>
		/// Set the value of a piece of tracked information.
		/// </summary>
		/// <param name="infoName">The name that the information
		/// is being stored under, e.g. 'Word Count'.</param>
		public void SetValue(string infoName, object val)
		{
			if (!trackedInfo.ContainsKey(infoName)) {
				trackedInfo.Add(infoName,val);
			}
			else {
				trackedInfo[infoName] = val;
			}
		}
		
		#endregion
	}
}
