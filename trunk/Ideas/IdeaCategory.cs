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
	public enum IdeaCategory {
		Quests,
		Plot,
		Items,
		Characters,
		Setting,
		Other, // user stated that this idea did not relate to any of the given categories
		Resources // user has submitted an object or a blueprint as an 'idea' to be used
	}
}
