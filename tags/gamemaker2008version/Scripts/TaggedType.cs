/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 08/03/2008
 * Time: 08:33
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace AdventureAuthor.Scripts
{
	/// <summary>
	/// Game artifacts which are identified by tags. 
	/// </summary>
	/// <remarks>Not all of these are objects. Includes a special AnyObject tag.</remarks>
	public enum TaggedType { 
		AnyObject, 
		Creature, 
		Door, 
		Encounter, 
		Item, 
		JournalCategory, // journal does not exist as an object
		Light, 
		Placeable,
		PlacedEffect, 
		Sound, 
		StaticCamera, 
		Store, 
		Tree, 
		Trigger, 
		Waypoint 
	};
}
