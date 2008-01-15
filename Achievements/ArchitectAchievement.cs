/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 23/12/2007
 * Time: 02:57
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;
using NWN2Toolset.NWN2.Data;
using form = NWN2Toolset.NWN2ToolsetMainForm;
using AdventureAuthor.Achievements;
using AdventureAuthor.Core;

namespace AdventureAuthor.Achievements
{
	/// <summary>
	/// Description of WordCountAchievement.
	/// </summary>
	public class ArchitectAchievement : IAchievement
	{		
		/// <summary>
		/// Raised when the requirements for this achievement have been fulfilled. 
		/// </summary>
		public event EventHandler<AchievedEventArgs> Achieved;
		
		
		/// <summary>
		/// Create a new achievement based on the number of words the user has written in a story-telling context.
		/// </summary>
		/// <param name="requiredWords">The number of words the user has to write to unlock this achievement</param>
		public ArchitectAchievement()
		{
			// ModuleHelper.AreaAdded += new EventHandler(OnCriterionChanged);
		}
		
		
		/// <summary>
		/// Check whether the criteria for unlocking this achievement have been met.
		/// </summary>
		/// <returns>True if the criteria for unlocking this achievement have been met; false otherwise</returns>
		public bool CriteriaMet()
		{
			if (!ModuleHelper.ModuleIsOpen()) {
				throw new InvalidOperationException("Module must be open to evaluate achievements.");
			}
			
			bool hasInterior = false;
			bool hasExterior = false;
			
			foreach (NWN2GameArea area in form.App.Module.Areas) {
				if (area.HasTerrain) {
					hasExterior = true;
				}
				else {
					hasInterior = true;
				}
			}
			
			return hasInterior && hasExterior;
		}
		
		
		public void OnCriterionChanged(EventArgs e)
		{
			if (CriteriaMet()) {
				OnAchieved(new AchievedEventArgs(this));
			}
		}
		
		
		protected virtual void OnAchieved(AchievedEventArgs e)
		{
			EventHandler<AchievedEventArgs> handler = Achieved;
			if (handler != null) {
				handler(this,e);
			}
			
			// Stop watching criteria:
			// ModuleHelper.AreaAdded -= new EventHandler(OnCriterionChanged); 
		}	
		
		
		/// <summary>
		/// The name of this achievement.
		/// </summary>
		public string GetName()
		{
			return "Architect";
		}

		
		/// <summary>
		/// A description of the criteria for unlocking this achievement.
		/// </summary>
		public string GetDescription()
		{
			return "Designer has constructed an interior area and an exterior area."; 
		}
						
		
		public override string ToString()
		{
			return GetName() + " (" + GetDescription() + ")";
		}
	}
}
