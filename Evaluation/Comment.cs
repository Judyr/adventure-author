/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 14/01/2008
 * Time: 13:15
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using AdventureAuthor.Evaluation;

namespace AdventureAuthor.Evaluation
{
	/// <summary>
	/// Description of Comment.
	/// </summary>
	public class Comment : Answer
	{
		public Comment(string comment)
		{
			this.Value = comment;
		}
	}
}
