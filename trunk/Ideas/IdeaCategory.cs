/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 14/02/2008
 * Time: 12:06
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace AdventureAuthor.Ideas
{
	/// <summary>
	/// The general subject domain that an idea relates to.
	/// </summary>
	public enum IdeaCategory {
		Plot,
		Quests,
		Characters,
		Dialogue,
		Setting,
		Items,
		Other, // user stated that this idea did not relate to any of the given categories
		Toolset // user has submitted an object or a blueprint as an 'idea' to be used
	}
}
