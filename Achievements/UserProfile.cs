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
		[XmlArray]
		private List<Award> awards;
		public List<Award> Awards {
			get { return awards; }
			set { awards = value; }
		}
		
		#endregion
		
		#region Constructors
		
		/// <summary>
		/// For deserialisation.
		/// </summary>
		private UserProfile()
		{
			awards = new List<Award>();
		}
		
		
		/// <summary>
		/// Create a user profile to represent information about the current user 
		/// relating to their use of Adventure Author.
		/// </summary>
		/// <param name="awards">The awards given to this user.</param>
		public UserProfile(List<Award> awards)
		{
			this.awards = awards;
		}
		
		#endregion
	
		#region Methods
		
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
		
		#endregion
	}
}
