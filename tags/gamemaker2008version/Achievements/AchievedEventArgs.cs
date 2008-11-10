/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 23/12/2007
 * Time: 14:52
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using AdventureAuthor.Achievements;

namespace AdventureAuthor.Achievements
{
	/// <summary>
	/// Arguments to accompany the event of an achievement being unlocked.
	/// </summary>
	public class AchievedEventArgs : EventArgs
	{
		/// <summary>
		/// The achievement which has been unlocked.
		/// </summary>
		public IAchievement achievement;		
		public IAchievement Achievement {
			get { return achievement; }
		}
		
		
		/// <summary>
		/// Create a new AchievedEventArgs.
		/// </summary>
		/// <param name="achievement">The achievement which has been unlocked</param>
		public AchievedEventArgs(IAchievement achievement)
		{
			this.achievement = achievement;
		}
	}
}
