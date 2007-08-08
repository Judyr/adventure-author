/*
 *   This file is part of Adventure Author.
 *
 *   Adventure Author is copyright Heriot-Watt University 2006-2007.
 *
 *   This copyright and licence apply to all source code, compiled code,
 *   documentation, graphics and auxiliary files, except where otherwise stated.
 *
 *   Adventure Author is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 *   Adventure Author is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 * 
 *   Adventure Author is a plugin for Atari's Neverwinter Nights 2, a COMMERCIAL
 *   product. Permission is given to link this GPL-covered plug-in with the 
 *   non-free main program. 
 *
 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using AdventureAuthor.Utils;
using NWN2Toolset.NWN2.Data;

namespace AdventureAuthor.Scripts
{
	/// <summary>
	/// Description of Actions
	/// </summary>
	public static class Actions
	{	
		// TODO: up to ga_effect in list of action scripts
		
		/// <summary>
		/// Add a target creature to your party as a henchman.
		/// </summary>
		/// <description>ga_henchman_add</description>
		/// <param name="sTarget"> tag of the creature you want to add</param>
		/// <param name="bForce">if set to 1, sTarget will be added even if player already has maximum henchman</param>
		/// <param name="sMaster">The creature you are adding the henchman to</param>
		/// <param name="bOverrideBehavior">if set to 1, sTarget's event handlers (scripts) will be replaced with henchman scripts</param>
		/// <returns></returns>
		public static NWN2ScriptFunctor AddHenchman(string sTarget, int bForce, string sMaster, int bOverrideBehavior)
		{
			return ScriptLibrary.GetScriptFunctor("ga_henchman_add",new object[]{sTarget,bForce,sMaster,bOverrideBehavior});
		}		
		
		/// <summary>
		/// Make the specified NPC attack the PC.
		/// </summary>
		/// <description>ga_attack</description>
		/// <param name="sAttacker">Tag of attacker</param>
		/// <returns></returns>
		public static NWN2ScriptFunctor Attack(string sAttacker)
		{
			return ScriptLibrary.GetScriptFunctor("ga_attack",new object[]{sAttacker});
		}
		
		/// <summary>
		/// Make the specified NPC attack the specified target
		/// </summary>
		/// <description>ga_attack_target</description>
		/// <param name="sAttacker">Tag of attacker</param>
		/// <param name="sTarget">Tag of target</param>
		/// <returns></returns>
		public static NWN2ScriptFunctor AttackTarget(string sAttacker, string sTarget)
		{
			return ScriptLibrary.GetScriptFunctor("ga_attack_target",new object[]{sAttacker,sTarget});
		}
		
		/// <summary>
		/// Close a door.
		/// </summary>
		/// <description>ga_door_close</description>
		/// <param name="sTag">Tag of door to close</param>
		/// <returns></returns>
		public static NWN2ScriptFunctor CloseDoor(string sTag)
		{
			int nLock = 0; // not useful - just call SetDoorLock after this
			return ScriptLibrary.GetScriptFunctor("ga_door_close",new object[]{sTag,nLock});
		}
				       
		/// <summary>
		/// Create an object.
		/// </summary>
		/// <description>ga_create_obj</description>
		/// <param name="sObjectType">C for creature, P for placeable, I for item, W for waypoint or S for store</param>
		/// <param name="sTemplate">Template to create object from</param>
		/// <param name="sLocationTag">Tag of the waypoint at which to create the object</param>
		/// <param name="bUseAppearAnimation">Set to 1 to make an animation play when the object appears</param>
		/// <param name="sNewTag">Tag of the newly created object</param>
		/// <param name="fDelay">Delay, in seconds, before creating the object</param>
		/// <returns></returns>
		public static NWN2ScriptFunctor CreateObject(string sObjectType, string sTemplate, string sLocationTag, 
		                                             int bUseAppearAnimation, string sNewTag, float fDelay)
		{
			return ScriptLibrary.GetScriptFunctor("ga_create_obj",
			                                      new object[]{sObjectType,sTemplate,sLocationTag,bUseAppearAnimation,sNewTag,fDelay});
		}
		
		/// <summary>
		/// Destroy an object.
		/// </summary>
		/// <description>ga_destroy</description>
		/// <param name="sTagString">The tag(s) of the object(s) to destroy. You can pass multiple 
		/// tags, seperated by commas (NO SPACES) to destroy multiple objects (ie. "Object1,Object2,Object3")</param>
		/// <param name="iInstance">The instance of the object to destroy. Pass -1 to destroy all instances, pass 0 to destroy the first instance</param>
		/// <param name="fDelay">The delay before destroying the object(s)</param>
		/// <returns></returns>
		public static NWN2ScriptFunctor Destroy(string sTagString, int iInstance, float fDelay)
		{
			return ScriptLibrary.GetScriptFunctor("ga_destroy",new object[]{sTagString,iInstance,fDelay});
		}				
		
		/// <summary>
		/// Destroys all the henchmen in the party. If you just want to remove them, use RemoveHenchman().
		/// </summary>
		/// <description>ga_destroy_party_henchmen</description>
		/// <returns></returns>
		public static NWN2ScriptFunctor DestroyAllHenchmen()
		{
			return ScriptLibrary.GetScriptFunctor("ga_destroy_party_henchmen",new object[]{});
		}
		
    	/// <summary>
    	/// Give the player some gold.
    	/// </summary>
    	/// <description>ga_give_gold</description>
    	/// <param name="nGP">The amount of gold coins given to the PC</param>
    	/// <returns></returns>
    	public static NWN2ScriptFunctor GiveGold(int nGP)
		{
    		int bAllPartyMembers = 0; // not useful
    		return ScriptLibrary.GetScriptFunctor("ga_give_gold",new object[]{nGP,bAllPartyMembers});
		}		     
		
		/// <summary>
		/// Create an item on PC(s).
		/// </summary>
		/// <description>ga_give_item</description>
		/// <param name="sTemplate">Item blueprint ResRef</param>
		/// <param name="nQuantity">Stack size for item</param>
		/// <param name="bAllPartyMembers">If set to 1, create item on all player characters in party (MP)</param>
		/// <returns></returns>
		public static NWN2ScriptFunctor GiveItem(string sTemplate, int nQuantity)
		{
			int bAllPartyMembers = 0; // not useful
			return ScriptLibrary.GetScriptFunctor("ga_give_item",new object[]{sTemplate,nQuantity,bAllPartyMembers});
		}	
		
		/// <summary>
		/// Jump an object to a waypoint or another object.
		/// </summary>
		/// <description>ga_jump</description>
		/// <param name="sDestination">Tag of waypoint or object to jump to</param>
		/// <param name="sTarget">Tag of object to jump</param>
		/// <param name="fDelay">Delay before jumping</param>
		/// <returns></returns>
		public static NWN2ScriptFunctor JumpObject(string sDestination, string sTarget, float fDelay)
		{
    		return ScriptLibrary.GetScriptFunctor("ga_jump",new object[]{sDestination,sTarget,fDelay});
		}
		
		/// <summary>
		/// Jump the PC party to a waypoint or object.
		/// </summary>
		/// <description>ga_jump_players</description>
		/// <param name="sDestTag">Tag of waypoint or object to jump to</param>
		/// <returns></returns>
		public static NWN2ScriptFunctor JumpPlayer(string sDestTag)
		{
			int bWholeParty = 1; // not useful
			int bOnlyThisArea = 0; // deprecated parameter
    		return ScriptLibrary.GetScriptFunctor("ga_jump_players",new object[]{sDestTag,bWholeParty,bOnlyThisArea});
		}				
		
		/// <summary>
		/// Make an object appear dead
		/// </summary>
		/// <description>ga_death</description>
		/// <param name="sTag">The tag(s) of the object(s) to make dead (multiple tags can be separated by commas with no spaces)</param>
		/// <param name="iInstance">The instance of the object(s) to make dead. Pass -1 to make all instances dead</param>
		/// <returns></returns>
		public static NWN2ScriptFunctor Kill(string sTag, int iInstance)
		{
    		return ScriptLibrary.GetScriptFunctor("ga_death",new object[]{sTag,iInstance});
		}	
		
		/// <summary>
		/// Open a door.
		/// </summary>
		/// <description>ga_door_open</description>
		/// <param name="sTag">Tag of door to open</param>
		/// <returns></returns>
		public static NWN2ScriptFunctor OpenDoor(string sTag)
		{
			return ScriptLibrary.GetScriptFunctor("ga_door_open",new object[]{sTag});
		}
		
		/// <summary>
		/// Remove a target henchman from the party.
		/// </summary>
		/// <description>ga_henchman_remove</description>
		/// <param name="sTarget">tag of the creature you want to remove</param>
		/// <returns></returns>
		public static NWN2ScriptFunctor RemoveHenchman(string sTarget)
		{
			string sOptionalMasterTag = String.Empty; // deprecated parameter
			return ScriptLibrary.GetScriptFunctor("ga_henchman_remove",new object[]{sTarget,sOptionalMasterTag});
		}
		
		/// <summary>
		/// Take an item from the player.
		/// </summary>
		/// <description>ga_destroy_item</description>
		/// <param name="sItemTag">Tag of the item to remove</param>
		/// <param name="nQuantity">The number of items to destroy. -1 is all of the Player's items of that tag</param>
		/// <returns></returns>
		public static NWN2ScriptFunctor RemoveItem(string sItemTag, int nQuantity)
		{
			int bPCFaction = 1; // not useful
			return ScriptLibrary.GetScriptFunctor("ga_destroy_item",new object[]{sItemTag,nQuantity,bPCFaction});
		}
		
		/// <summary>
		/// Change the lock status of a door
		/// </summary>
		/// <description>ga_lock</description>
		/// <param name="sDoorTag">The door to lock/unlock</param>
		/// <param name="bLock">True to lock the door, false to unlock the door</param>
		/// <returns></returns>
		public static NWN2ScriptFunctor SetDoorLock(string sDoorTag, int bLock)
		{
			return ScriptLibrary.GetScriptFunctor("ga_lock",new object[]{sDoorTag,bLock});
		}
				
		/// <summary>
		/// Take gold from the player
		/// </summary>
		/// <description>ga_take_gold</description>
		/// <param name="nGold">The amount of gold to take</param>
		/// <returns></returns>
		public static NWN2ScriptFunctor TakeGold(int nGold)
		{
			int bAllPartyMembers = 0; // not useful
			return ScriptLibrary.GetScriptFunctor("ga_take_gold",new object[]{nGold,bAllPartyMembers});
		}		
		
		/// <summary>
		/// Take item(s) from the player
		/// </summary>
		/// <description>ga_take_item</description>
		/// <param name="sItemTag">This is the string name of the item's tag</param>
		/// <param name="nQuantity">The number of items to take, set to -1 to take all of the Player's items of that tag</param>
		/// <returns></returns>
		public static NWN2ScriptFunctor TakeItem(string sItemTag, int nQuantity)
		{
			int bAllPartyMembers = 0; // not useful
			return ScriptLibrary.GetScriptFunctor("ga_take_item",new object[]{sItemTag,nQuantity,bAllPartyMembers});
		}	
	}
}
