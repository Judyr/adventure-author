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
using System.Resources;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Xml.Serialization;

namespace AdventureAuthor.Achievements
{
	/// <summary>
	/// An award granted to the user based on the extent
	/// to which they have used Fridge Magnets.
	/// </summary>
	[Serializable]
	public class ImagineerAward : Award
	{		
		#region Constants
	
//		public const uint BRONZEUSAGE = 250;
//		public const uint SILVERUSAGE = 500;
//		public const uint GOLDUSAGE = 1000;
//		public const uint EMERALDUSAGE = 1500;
//		public const uint SAPPHIREUSAGE = 2000;
//		public const uint RUBYUSAGE = 2500;
//		public const uint DIAMONDUSAGE = 3000;
		
		public const uint BRONZEUSAGE = 2;//50;
		public const uint SILVERUSAGE = 5;//00;
		public const uint GOLDUSAGE = 10;//00;
		public const uint EMERALDUSAGE = 15;//00;
		public const uint SAPPHIREUSAGE = 20;//00;
		public const uint RUBYUSAGE = 25;//00;
		public const uint DIAMONDUSAGE = 30;//00;
		
		#endregion
		
		#region Properties and fields
		
		/// <summary>
		/// The level of user activity (represented by an abstract
		/// number of 'points') that must have been reached in 
		/// order for the user to gain this award.
		/// </summary>
		private uint requiredUserActivity;
		[XmlElement]
		public uint RequiredUserActivity {
			get { return requiredUserActivity; }
			set { requiredUserActivity = value; }
		}
		
		#endregion
				
		#region Constructors
		
		/// <summary>
		/// For deserialisation.
		/// </summary>
		protected ImagineerAward()
		{		
			ResourceManager manager = new ResourceManager("AdventureAuthor.Achievements.UI.awardimages",
			                                              Assembly.GetExecutingAssembly());
			this.picture = (System.Drawing.Bitmap)manager.GetObject("wizard");		
		}
		
		
		/// <summary>
		/// An award granted to the user based on their level of activity
		/// with the Fridge Magnets application to date.
		/// </summary>
		public ImagineerAward(string name, uint requiredUserActivityLevel) : this()
		{
			this.name = name;
			this.requiredUserActivity = requiredUserActivityLevel;
			this.description = "Awarded when the user has spent a period " +
				"of time working productively with Fridge Magnets.";
			this.designerPoints = DEFAULTDESIGNERPOINTSVALUE;	
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
		public override bool CriteriaMet(object dataToCheckAgainstCriteria)
		{
			if (dataToCheckAgainstCriteria is uint) {
				return CriteriaMet((uint)dataToCheckAgainstCriteria);
			}
			else {
				throw new ArgumentException("Must pass a uint value to check criteria for this award.");
			}
		}
		
		
		/// <summary>
		/// Check whether the value passed (representing tracked information about
		/// some aspect of user activity) matches or exceeds the minimum criteria 
		/// for the award to be granted.
		/// </summary>
		/// <param name="userActivity">The total user activity level of the user 
		/// with My Tasks to date.</param>
		/// <returns>True if the criteria for granting this award have
		/// been met; false otherwise.</returns>
		public bool CriteriaMet(uint userActivity)
		{
			return userActivity >= requiredUserActivity;
		}
		
				
		/// <summary>
		/// Get the type of information which is used to judge whether the criteria
		/// of this award have been met.
		/// </summary>
		/// <returns>The Type of information used to judge whether the criteria of
		/// this award have been met.</returns>
		/// <remarks>Any AchievementMonitor which this award is added to must return a value
		/// for GetTrackedInformationType() which is identical to the value returned by this method.</remarks>
		public override Type GetCriteriaType()
		{
			return typeof(uint);
		}
		
		#endregion
	}
}
