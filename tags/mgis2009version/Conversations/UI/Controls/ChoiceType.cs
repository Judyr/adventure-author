/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 06/03/2008
 * Time: 16:17
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace AdventureAuthor.Conversations.UI.Controls
{
	/// <summary>
	/// Describes the type of a choice point in a conversation - either Player (the player chooses
	/// what to say next from a menu of branches) or NPC (a non-player character performs conditional
	/// checks and decides what to say next based on their outcomes)
	/// </summary>
	public enum ChoiceType { 
		Player, 
		NPC 
	}    	
}
