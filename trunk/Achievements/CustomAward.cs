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
using System.Drawing;
using System.IO;
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
	/// A custom award, in which the name, description and image
	/// are entered/chosen by a user.
	/// </summary>
	/// <remarks>This award does not have requirements to fulfill -
	/// it should be awarded to the user upon creation. The
	/// CriteriaMet and GetCriteriaType methods will raise
	/// NotImplementedExceptions if called.</remarks>
	[Serializable]
	public class CustomAward : Award
	{				
		#region Properties and fields
		
		[XmlElement]
		private string imagePath;
		public string ImagePath {
			get { return imagePath; }
			set { 
				imagePath = value; 			
				try {
					if (File.Exists(imagePath)) {
						this.picture = new Bitmap(imagePath);
					}
				}
				catch (Exception) {
					this.picture = null;
				}
			}
		}
		
		#endregion
		
		#region Constructors
		
		/// <summary>
		/// For deserialisation.
		/// </summary>
		protected CustomAward()
		{
			// TODO: Do I need to explicitly replicate the ImagePath setter code here,
			// or will the constructor call it itself? 
		}
		
		
		/// <summary>
		/// A custom award, in which the name, description and image
		/// are entered/chosen by a user.
		/// </summary>
		/// <remarks>This award does not have requirements to fulfill -
		/// it should be awarded to the user upon creation.</remarks>
		public CustomAward(string name, string description, uint designerPoints, string imagePath) : this()
		{
			this.name = name;
			this.description = description;
			this.designerPoints = designerPoints;
			
			ImagePath = imagePath; // sets the picture as well, if it can find it at imagePath.
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
			throw new NotImplementedException();
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
			throw new NotImplementedException();
		}
		
		#endregion
	}
}
