/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 23/12/2007
 * Time: 02:53
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using AdventureAuthor.Achievements;

namespace AdventureAuthor.Achievements
{
	/// <summary>
	/// Description of IAchievement.
	/// </summary>
	public interface IAchievement
	{
		event EventHandler<AchievedEventArgs> Achieved;
		string GetName();
		string GetDescription();	
		void OnCriterionChanged(EventArgs e);	
		bool CriteriaMet();	
	}
}