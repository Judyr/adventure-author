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
using System.Collections.Generic;
using AdventureAuthor.Setup;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Achievements
{
	/// <summary>
	/// Tracks the total 'narrative' word count of the user - 
	/// i.e. any words they have typed into the Conversation Writer,
	/// or into the First Name, Last Name, Localized Description or
	/// Localized Description (when identified) fields of an object -
	/// and grants awards when the total reaches certain levels.
	/// </summary>
	public class WordCountMonitor : AchievementMonitor
	{
		#region Properties and fields
			
		/// <summary>
		/// The total 'narrative' word count of the user - a rough total
		/// of all the words they have typed into the Conversation Writer,
		/// and into the First Name, Last Name, Localized Description and
		/// Localized Description (when identified) fields of an object.
		/// </summary>
		protected uint wordCount;
		public uint WordCount {
			get { 
				lock (padlock) {
					return wordCount;
				}
			}
			set { 
				lock (padlock) {
					wordCount = value;
					OnSubjectChanged(new PropertyChangedEventArgs("WordCount"));
				}
			}
		}
		
		#endregion
		
		#region Constructors	
		
		/// <summary>
		/// Construct an object which tracks the total 'narrative' word 
		/// count of the user and grants awards when the total reaches 
		/// certain levels.
		/// </summary>
		public WordCountMonitor() : base()
		{
			Toolset.WordsTyped += UpdateWordCount;
		}
		
		
		/// <summary>
		/// Construct an object which tracks the total 'narrative' word 
		/// count of the user and grants awards when the total reaches 
		/// certain levels.
		/// </summary>
		/// <param name="award">An award to be given out based on word count.</param>
		public WordCountMonitor(Award award) : this(new List<Award>{award})
		{			
		}
		
		
		/// <summary>
		/// Construct an object which tracks the total 'narrative' word 
		/// count of the user and grants awards when the total reaches 
		/// certain levels.
		/// </summary>
		/// <param name="awards">Awards to be given out based on word count.</param>
		public WordCountMonitor(List<Award> awards) : this()
		{
			foreach (Award award in awards) {
				AddAward(award);
			}
		}
		
		#endregion
		
		#region Methods
				
		/// <summary>
		/// Get the current recorded information on the monitored subject.
		/// </summary>
		/// <returns>The information being tracked about the monitored subject.</returns>
		public override object GetCurrentInformationOnSubject()
		{
			return WordCount;
		}
		
		
		/// <summary>
		/// Get the type of information being recorded about the monitored subject.
		/// </summary>
		/// <returns>The Type of information being recorded about the monitored subject.</returns>
		/// <remarks>Any award being added to this AchievementMonitor must return a value
		/// for GetCriteriaType() which is identical to the value returned by this method.</remarks>
		public override Type GetTrackedInformationType()
		{
			return typeof(uint);
		}
		
		
		/// <summary>
		/// Update the total word count for this user.
		/// </summary>
		protected void UpdateWordCount(object sender, WordCountEventArgs e)
		{
			try {
				WordCount += e.Words;
			}
			catch (OverflowException) {
				WordCount = uint.MaxValue;
			}
		}
		
		#endregion
	}
}
