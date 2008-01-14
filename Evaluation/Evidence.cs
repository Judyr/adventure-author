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
	/// Description of Evidence.
	/// </summary>
	public class Evidence : Answer
	{
		public Evidence(string location)
		{
			this.Value = location;
		}
	}
}
