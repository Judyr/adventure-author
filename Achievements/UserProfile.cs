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
using System.ComponentModel;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Serialization;
using System.Windows.Threading;
using System.Diagnostics;
using System.Collections.ObjectModel;
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
		/// Monitors which track certain aspects of user activity
		/// and give awards based on them.
		/// </summary>
		/// <remarks>These are not serialized, and should be
		/// set up when the user profile is read from disk.</remarks>
		protected ObservableCollection<AchievementMonitor> monitors;
		[XmlIgnore]
		public ObservableCollection<AchievementMonitor> Monitors {
			get { return monitors; }
		}
		
		
		/// <summary>
		/// The awards that this user has received as a result
		/// of their toolset activity.
		/// </summary>
		protected ObservableCollection<Award> awards;
		[XmlArray]
		public ObservableCollection<Award> Awards {
			get { return awards; }
			set { awards = value; }
		}
		
		
		/// <summary>
		/// Information that is being tracked for this user in order
		/// to grant them awards.
		/// </summary>
		protected SerializableDictionary<string,object> trackedInfo;
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
		public uint MyTasksActivity {
			get { 
				return (uint)GetValue(MyTasksMonitor.SUBJECT);
			}
			set {
				SetValue(MyTasksMonitor.SUBJECT,value);
			}
		}
		
		
		/// <summary>
		/// The total activity of the user with the Fridge Magnets application.
		/// </summary>
		[XmlIgnore]
		public uint FridgeMagnetsActivity {
			get { 
				return (uint)GetValue(FridgeMagnetsMonitor.SUBJECT);
			}
			set {
				SetValue(FridgeMagnetsMonitor.SUBJECT,value);
			}
		}
		
		
		/// <summary>
		/// A loose UI element used to get a reference to the UI thread dispatcher.
		/// </summary>
		[XmlIgnore]
		private System.Windows.Controls.Label UIControl = new System.Windows.Controls.Label();
		
		#endregion
		
		#region Events
		
		public event EventHandler<AwardEventArgs> AwardReceived;
		protected virtual void OnAwardReceived(AwardEventArgs e)
		{
			EventHandler<AwardEventArgs> handler = AwardReceived;
			if (handler != null) {
				handler(this, e);
			}
		}
		
		#endregion
		
		#region Constructors
		
		/// <summary>
		/// For deserialisation.
		/// </summary>
		protected UserProfile()
		{
			awards = new ObservableCollection<Award>();
			trackedInfo = new SerializableDictionary<string,object>();
			monitors = new ObservableCollection<AchievementMonitor>();
			SetupDefaultValues();
			
			// Watch for custom awards which are granted without being
			// sent by a particular AchievementMonitor:
			AchievementMonitor.AwardGrantedDirectly += GiveAward;
		}
		
		
		/// <summary>
		/// Create a user profile to represent information about the current user 
		/// relating to their use of Adventure Author.
		/// </summary>
		/// <param name="awards">The awards given to this user.</param>
		public UserProfile(ObservableCollection<Award> awards)
		{
			if (awards == null) {
				throw new ArgumentNullException("awards","Parameter 'awards' cannot be null.");
			}
			
			Awards = awards;
			trackedInfo = new SerializableDictionary<string,object>();
			monitors = new ObservableCollection<AchievementMonitor>();
			SetupDefaultValues();
			
			// Watch for custom awards which are granted without being
			// sent by a particular AchievementMonitor:
			AchievementMonitor.AwardGrantedDirectly += GiveAward;
		}
		
		#endregion
	
		#region Methods
		
		/// <summary>
		/// Set up default values for tracked information.
		/// </summary>
		protected void SetupDefaultValues()
		{
			WordCount = 0;
			MyTasksActivity = 0;
			FridgeMagnetsActivity = 0;
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
		/// When a monitor indicates that an award should be granted,
		/// add that award to the user's award collection. 
		/// </summary>
		/// <param name="award">The award to give.</param>
		public void GiveAward(Award award)
		{
			if (!HasAward(award)) {
				Awards.Add(award);
				OnAwardReceived(new AwardEventArgs(award));
			}
		}				
		
		
		/// <summary>
		/// When a monitor indicates that an award should be granted,
		/// add that award to the user's award collection. 
		/// </summary>
		/// <param name="award">The award to give.</param>
		public delegate void GiveAwardDelegate(Award award);	
		
		
		/// <summary>
		/// Add a monitoring class to track user activity
		/// and grant awards.
		/// </summary>
		/// <param name="monitor">The monitor to add.</param>
		public void AddMonitor(AchievementMonitor monitor)
		{
			Monitors.Add(monitor);
			monitor.AwardGranted += GiveAward;
		}

		
		/// <summary>
		/// When a monitor indicates that an award should be granted,
		/// add that award to the user's award collection. 
		/// </summary>
		private void GiveAward(object sender, AwardEventArgs e)
		{
			// ObservableCollection<T> must have its contents changed via
			// the UI thread, so use a control to get the UI thread's Dispatcher:
			UIControl.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
					                         new GiveAwardDelegate(GiveAward),
					                         e.Award);			
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
		
		
		/// <summary>
		/// Serialize the user profile to disk.
		/// </summary>
		/// <param name="path">The path to serialize this object to.</param>
		public void Serialize(string path)
		{
			UpdateRecords();
			
			try {
				Serialization.Serialize(path,this);
			}
			catch (Exception e) {
				Say.Error("Failed to save the user profile.",e);
			}	
		}
		
		
		/// <summary>
		/// Record the current values of all information being
		/// tracked by monitor classes.
		/// </summary>
		public void UpdateRecords()
		{			
			foreach (AchievementMonitor monitor in monitors) {
				string name = monitor.GetSubjectName();
				object val = monitor.GetSubjectValue();
				if (name != null && name != String.Empty) {
					SetValue(name,val);
				}				
			}
		}
		
		#endregion
	}
}
