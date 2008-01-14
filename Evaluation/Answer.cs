/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 14/01/2008
 * Time: 12:56
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace AdventureAuthor.Evaluation
{
	public abstract class Answer
	{
		private string val;		
		public string Value {
			get { return val; }
			set { val = value; }
		}
	}
}
