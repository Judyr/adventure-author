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
using System.Windows.Controls;

namespace AdventureAuthor.Achievements
{
	/// <summary>
	/// An award granted to the user based on some aspect of their activity.
	/// </summary>
	public abstract class Award
	{
		#region Constants
		
		/// <summary>
		/// A typical default amount of Designer Points for an award to carry.
		/// </summary>
		/// <remarks>This is intended for 'real' awards - 'tutorial' awards (granted
		/// when the user carries out basic toolset activities for the first time)
		/// are expected to carry 0 points.</remarks>
		public const uint DEFAULTDESIGNERPOINTSVALUE = 1000;
		
		#endregion
		
		#region Properties and fields
		
		/// <summary>
		/// The name of this award.
		/// </summary>
		protected string name;
		public string Name {
			get { return name; }
		}
		
		
		/// <summary>
		/// A brief description of this award and the reasons
		/// why it is granted.
		/// </summary>
		protected string description;
		public string Description {
			get { return description; }
		}
		
				
		/// <summary>
		/// An image which can be used to represent this award.
		/// </summary>
		protected Image image;
		public Image Image {
			get { return image; }
		}
		
		
		/// <summary>
		/// The number of Designer Points to allocate to the user
		/// on receiving this award. The user will be given a rank
		/// based on the total number of Designer Points they have
		/// accumulated.
		/// </summary>
		/// <remarks>A greater number indicates that the award was
		/// more difficult to achieve - a typical number of DP to award
		/// can be taken from DEFAULTDESIGNERPOINTSVALUE. 
		/// Currently it is expected that 'tutorial' awards will
		/// carry 0 points and all other awards will carry 
		/// DEFAULTDESIGNERPOINTSVALUE points, but this could
		/// be changed in future.
		/// </remarks>
		protected uint designerPoints;
		public uint DesignerPoints {
			get { return designerPoints; }
		}		
		
		#endregion
		
		#region Methods
		
		/// <summary>
		/// Check whether the value passed (representing tracked information about
		/// some aspect of user activity) matches or exceeds the minimum criteria 
		/// for the award to be granted.
		/// </summary>
		/// <param name="dataToCheckAgainstCriteria">The data to check against
		/// the minimum criteria for this award. The type of this data
		/// should match the value of GetCriteriaType().</param>
		/// <returns>True if the criteria for granting this award have
		/// been met; false otherwise.</returns>
		public abstract bool CriteriaMet(object dataToCheckAgainstCriteria);
		
		
		/// <summary>
		/// Get the type of information which is used to judge whether the criteria
		/// of this award have been met.
		/// </summary>
		/// <returns>The Type of information used to judge whether the criteria of
		/// this award have been met.</returns>
		/// <remarks>Any AchievementMonitor which this award is added to must return a value
		/// for GetTrackedInformationType() which is identical to the value returned by this method.</remarks>
		public abstract Type GetCriteriaType();
		
		#endregion
	}
}
