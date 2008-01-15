/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 14/01/2008
 * Time: 13:15
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Xml.Serialization;
using System.Windows;
using AdventureAuthor.Evaluation;
using AdventureAuthor.Evaluation.UI;

namespace AdventureAuthor.Evaluation
{
	/// <summary>
	/// Description of Comment.
	/// </summary>
	[Serializable]
	[XmlRoot]
	public class Comment : Answer
	{
		public Comment()
		{			
		}
		
		
		public Comment(string comment)
		{
			this.Value = comment;
		}
		
		
		public override IAnswerControl GetAnswerControl()
		{
			return new CommentBox(this);
		}
	}
}
