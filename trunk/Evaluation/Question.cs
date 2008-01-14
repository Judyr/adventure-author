/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 14/01/2008
 * Time: 11:46
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;
using AdventureAuthor.Evaluation;

namespace AdventureAuthor.Evaluation
{
	public class Question
	{
		/// <summary>
		/// The text of this question.
		/// </summary>
		private string text;		
		public string Text {
			get { return text; }
			set { text = value; }
		}
		
		
		/// <summary>
		/// Answers to this question.
		/// </summary>
		/// <remarks>A worksheet question may require multiple answers
		/// e.g. a star rating, a comment, and the URL of supporting evidence</remarks>
		private List<Answer> answers = new List<Answer>(1);		
		public List<Answer> Answers {
			get { return answers; }
		}
		
	}
}
