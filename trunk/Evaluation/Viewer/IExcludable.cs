/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 21/01/2008
 * Time: 15:53
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace AdventureAuthor.Evaluation.Viewer
{
	/// <summary>
	/// Description of IExcludable.
	/// </summary>
	public interface IExcludable
	{
		bool Include {
			get; set;
		}
	}
}
