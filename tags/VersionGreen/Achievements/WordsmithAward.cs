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
	/// An award granted to the user based on their total 'narrative'
	/// word count to date.
	/// </summary>
	/// <remarks>
	/// The total 'narrative' word count of the user is a rough total
	/// of all the words they have typed into the Conversation Writer,
	/// and into the First Name, Last Name, Localized Description and
	/// Localized Description (when identified) fields of an object.
	/// </remarks>
	[Serializable]
	public class WordsmithAward : Award
	{		
		#region Constants
		
//		public const uint BRONZEWORDCOUNT = 250;
//		public const uint SILVERWORDCOUNT = 500;
//		public const uint GOLDWORDCOUNT = 1000;
//		public const uint EMERALDWORDCOUNT = 1500;
//		public const uint SAPPHIREWORDCOUNT = 2000;
//		public const uint RUBYWORDCOUNT = 2500;
//		public const uint DIAMONDWORDCOUNT = 3000;
		
		public const uint BRONZEWORDCOUNT = 2;//50;
		public const uint SILVERWORDCOUNT = 5;//00;
		public const uint GOLDWORDCOUNT = 10;//00;
		public const uint EMERALDWORDCOUNT = 15;//00;
		public const uint SAPPHIREWORDCOUNT = 20;//00;
		public const uint RUBYWORDCOUNT = 25;//00;
		public const uint DIAMONDWORDCOUNT = 30;//00;
		
		#endregion
		
		#region Properties and fields
		
		/// <summary>
		/// The total word count which the user must have typed in order
		/// to receive this award.
		/// </summary>
		private uint requiredWordCount;
		[XmlElement]
		public uint RequiredWordCount {
			get { return requiredWordCount; }
			set { requiredWordCount = value; }
		}
		
		#endregion
				
		#region Constructors
		
		/// <summary>
		/// For deserialisation.
		/// </summary>
		protected WordsmithAward()
		{					
			ResourceManager manager = new ResourceManager("AdventureAuthor.Achievements.UI.awardimages",
			                                              Assembly.GetExecutingAssembly());
			this.picture = (System.Drawing.Bitmap)manager.GetObject("edit");	
		}
		
		
		/// <summary>
		/// An award granted to the user based on their total 'narrative'
		/// word count to date.
		/// </summary>
		/// <remarks>
		/// The total 'narrative' word count of the user is a rough total
		/// of all the words they have typed into the Conversation Writer,
		/// and into the First Name, Last Name, Localized Description and
		/// Localized Description (when identified) fields of an object.
		/// </remarks>
		public WordsmithAward(string name, uint requiredWordCount) : this()
		{
			this.name = name;
			this.requiredWordCount = requiredWordCount;
			this.description = "Awarded when the user has written at least " +
				requiredWordCount + " words of narrative text.";
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
		/// <param name="wordCount">The total 'narrative' word count of the user to
		/// date.</param>
		/// <returns>True if the criteria for granting this award have
		/// been met; false otherwise.</returns>
		public bool CriteriaMet(uint wordCount)
		{
			return wordCount >= requiredWordCount;
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
