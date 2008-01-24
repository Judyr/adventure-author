/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 24/01/2008
 * Time: 11:33
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace AdventureAuthor.Evaluation.Viewer
{
	/// <summary>
	/// Description of AnswerControl.
	/// </summary>
	public abstract class AnswerControl : ActivatableControl
	{
		#region Events
		
		public event EventHandler AnswerChanged;
		
		protected virtual void OnAnswerChanged(EventArgs e)
		{
			EventHandler handler = AnswerChanged;
			if (handler != null) {
				handler(this,e);
			}
		}
		
		#endregion
		
		#region Constructors
		
		protected AnswerControl()
		{
		}
		
		#endregion
			
		#region Methods
			
		protected abstract Answer GetAnswerObject();
		
		public Answer GetAnswer()
		{
			Answer answer = GetAnswerObject();
			if (answer == null) {
				throw new InvalidOperationException("GetAnswerObject() returned a null reference.");
			}        	
        	switch (ActivationStatus) {
        		case ControlStatus.Active:
        			answer.Include = true;
        			break;
        		case ControlStatus.Inactive:
        			answer.Include = false;
        			break;
        		case ControlStatus.NA:
        			answer.Include = true;
        			break;
        	}
			return answer;
		}
		
		#endregion
	}
}
