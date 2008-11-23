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
	/// Monitors a particular aspect of user activity and grants awards
	/// based on that activity once the specific criteria of a particular
	/// award have been met.
	/// </summary>
	public abstract class AchievementMonitor
	{
		#region Properties and fields
		
		/// <summary>
		/// The awards which this class is responsible for awarding to the user.
		/// </summary>
		protected List<Award> awards = new List<Award>(4);
		public List<Award> Awards {
			get { return awards; }
		}
		
		
		/// <summary>
		/// An object for locking access to the awards list.
		/// </summary>
		protected object padlock = new object();
		
		#endregion
		
		#region Events
						
		/// <summary>
		/// Raised when there has been a change in the subject being monitored.
		/// </summary>
		public event PropertyChangedEventHandler SubjectChanged;
		protected virtual void OnSubjectChanged(PropertyChangedEventArgs e)
		{
			PropertyChangedEventHandler handler = SubjectChanged;
			if (handler != null) {
				handler(this,e);
			}
		}
		
		
		/// <summary>
		/// Raised when the criteria for granting a particular award have been met.
		/// </summary>
		public event EventHandler<AwardGrantedEventArgs> AwardGranted;
		protected virtual void OnAwardGranted(AwardGrantedEventArgs e)
		{
			EventHandler<AwardGrantedEventArgs> handler = AwardGranted;
			if (handler != null) {
				handler(this,e);
			}
		}
		
		#endregion
		
		#region Constructors
		
		/// <summary>
		/// Construct an object which will monitor a particular aspect of 
		/// user activity and grant awards based on that activity once the 
		/// specific criteria of a particular award have been met.
		/// </summary>
		public AchievementMonitor()
		{
			SubjectChanged += CheckAgainstAwardsCriteria;
		}
		
		#endregion
		
		#region Methods	
		
		/// <summary>
		/// Get the current recorded information on the monitored subject.
		/// </summary>
		/// <returns>The information being tracked about the monitored subject.</returns>
		public abstract object GetCurrentInformationOnSubject();	
		
		
		/// <summary>
		/// Get the type of information being recorded about the monitored subject.
		/// </summary>
		/// <returns>The Type of information being recorded about the monitored subject.</returns>
		/// <remarks>Any award being added to this AchievementMonitor must return a value
		/// for GetCriteriaType() which is identical to the value returned by this method.</remarks>
		public abstract Type GetTrackedInformationType();

		
		/// <summary>
		/// Check whether the criteria of each award have been met, and if so
		/// remove that award from consideration and raise an event indicating 
		/// that it has been granted to the user.
		/// </summary>
		protected void CheckAgainstAwardsCriteria(object sender, PropertyChangedEventArgs e)
		{
			object information = GetCurrentInformationOnSubject();
			List<Award> awardsToGrant = new List<Award>();
			List<Award> awardsToRemove = new List<Award>();				
				
			foreach (Award award in awards) {
				try {
					if (award.CriteriaMet(information)) {
						awardsToGrant.Add(award);
						awardsToRemove.Add(award);
					}
				}
				catch (ArgumentException) {
					awardsToRemove.Add(award);
					Say.Error("The method CriteriaMet() on award '" + award.Name + "' must " + 
					          "be passed a value of type " + award.GetCriteriaType() + ". The " +
					          "award is inappropriate for this AchievementMonitor, and will be removed.");
				}
			}
			
			lock (padlock) {
				foreach (Award award in awardsToRemove) {
					if (awards.Contains(award)) {
						awards.Remove(award);
					}
				}
			}
			
			foreach (Award award in awardsToGrant) {
				OnAwardGranted(new AwardGrantedEventArgs(award));
			}
		}
		
		
		/// <summary>
		/// Add to the list of awards being considered.
		/// </summary>
		/// <param name="award">The award to add.</param>
		/// <param name="checkUserDoesNotHave">True to only add if the user does
		/// not have this award already.</param>
		public void AddAward(Award award, bool checkUserDoesNotHave)
		{
			if (award == null) {
				throw new ArgumentNullException("award","'award' cannot be null.");
			}
			
			Type criteriaType = award.GetCriteriaType();
			Type subjectType = GetTrackedInformationType();
			if (criteriaType != subjectType) {
				throw new ArgumentException("Only awards which return a value of " + subjectType + " from " + 
				                            "method GetCriteriaType() can be added to this AchievementMonitor (" +
				                            "returned type " + criteriaType + ").");
			}
			
			if (checkUserDoesNotHave && Toolset.Plugin.Profile.HasAward(award)) {
				return;
			}
			
			lock (padlock) {
				if (Awards.Contains(award)) {
					throw new InvalidOperationException("award was already added to this AchievementMonitor.");
				}
				else {
					awards.Add(award);
				}
			}
		}
		
		
		/// <summary>
		/// Add to the list of awards being considered.
		/// </summary>
		/// <param name="award">The award to add.</param>
		public void AddAward(Award award)
		{	
			AddAward(award,true);
		}
		
		#endregion
	}
}
