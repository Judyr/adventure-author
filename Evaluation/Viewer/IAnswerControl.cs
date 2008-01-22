/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 15/01/2008
 * Time: 11:28
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Windows.Controls;
using AdventureAuthor.Evaluation;

namespace AdventureAuthor.Evaluation.Viewer
{
	/// <summary>
	/// A panel that asks a question of the user, and can return the user's answer. 
	/// </summary>
	public interface IAnswerControl
	{
		/// <summary>
		/// Get the answer to the question posed by the panel, based on user input.
		/// </summary>
		Answer GetAnswer();
		
			
		event EventHandler AnswerChanged;
	}
}
