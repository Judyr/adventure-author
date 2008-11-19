/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 23/12/2007
 * Time: 02:57
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using AdventureAuthor.Achievements;

namespace AdventureAuthor.Achievements
{
	/// <summary>
	/// Description of WordCountAchievement.
	/// </summary>
	public class WordCountAchievement : IAchievement
	{
		/// <summary>
		/// The number of words the user has to write to unlock this achievement.
		/// </summary>
		private int requiredWords;		
		public int RequiredWords {
			get { return requiredWords; }
		}
		
		
		/// <summary>
		/// Raised when the requirements for this achievement have been fulfilled. 
		/// </summary>
		public event EventHandler<AchievedEventArgs> Achieved;
		
		
		/// <summary>
		/// Create a new achievement based on the number of words the user has written in a story-telling context.
		/// </summary>
		/// <param name="requiredWords">The number of words the user has to write to unlock this achievement</param>
		public WordCountAchievement(int requiredWords)
		{
			this.requiredWords = requiredWords;
			// User.CurrentUser.WordCountChanged += new EventHandler(OnCriterionChanged);
		}
		
		
		/// <summary>
		/// Check whether the criteria for unlocking this achievement have been met.
		/// </summary>
		/// <returns>True if the criteria for unlocking this achievement have been met; false otherwise</returns>
		public bool CriteriaMet()
		{
			throw new NotImplementedException();
			// return User.CurrentUser.WordCount >= requiredWords;
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
			
			// Stop watching word count:
			// User.CurrentUser.WordCountChanged -= new EventHandler(OnCriterionChanged);
		}	
		
		
		/// <summary>
		/// The name of this achievement.
		/// </summary>
		public string GetName()
		{
			return "Wordsmith (Bronze)";
		}

		
		/// <summary>
		/// A description of the criteria for unlocking this achievement.
		/// </summary>
		public string GetDescription()
		{
			return "Designer has written at least " + requiredWords + " words of dialogue and description."; 
		}
						
		
		public override string ToString()
		{
			return GetName() + " (" + GetDescription() + ")";
		}
	}
}
