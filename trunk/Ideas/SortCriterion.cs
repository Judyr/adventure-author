/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 18/02/2008
 * Time: 17:36
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace AdventureAuthor.Ideas
{
	/// <summary>
	/// The criterion to sort the magnet list by (e.g. sort alphabetically, or
	/// sort into those magnets which have been used and those which haven't.)
	/// </summary>
	public enum SortCriterion {
		Alphabetically,
		Category,
		Created,
		Used,
		Stars
	}
}
