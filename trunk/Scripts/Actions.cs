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
		/* Actions to add:
		 * ga_align_good_evil
		 * ga_align_law_chaos?
		 * ga_effect
		 * ga_give_feat?
		 * ga_give_xp?
		 * ga_journal
		 * the 10 music scripts to start/stop battle and background music
		 * ga_party_add
		 * ga_party_face_target
		 * ga_play_animation? (if we can't access this functionality more easily in the same way Node does)
		 * ga_remove_comp (should work on both henchmen and companions)
		 * ga_remove_feat?
		 * ga_roster_add_blueprint
		 * ga_roster_add_object
		 * ga_set_wwp_controller
		 * ga_sound_object_play
		 * ga_sound_object_stop
		 * ga_sound_object_setposition
		 * ga_sound_object_setvolume
		 * */
		
		public enum FadeColour { Black = 0, White = 16777215, Red = 16711680 };
		
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
		/// Advance time by a given amount
		/// </summary>
		/// <param name="nHour">Hours to advance</param>
		/// <param name="nMinute">Minutes to advance</param>
		/// <param name="nSecond">Seconds to advance</param>
		/// <param name="nMillisecond">Milliseconds to advance</param>
		public static NWN2ScriptFunctor AdvanceTimeBy(int nHour, int nMinute, int nSecond, int nMillisecond)
		{
			return ScriptLibrary.GetScriptFunctor("ga_time_advance",new object[]{nHour,nMinute,nSecond,nMillisecond});
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
		/// Make a creature walk/run to a location
		/// </summary>
		/// <param name="sCreatureTag">The tag of the creature to move</param>
		/// <param name="sWPTag">The tag of the location the creature will move to</param>
		/// <param name="bRun">Set to 0 for walk, 1 for run</param>
		public static NWN2ScriptFunctor CreatureMoves(string sCreatureTag, string sWPTag, int bRun)
		{
			// parameters are in a different order from the function call, to be consistent with CreatureMovesAndExits:
			return ScriptLibrary.GetScriptFunctor("ga_move",new object[]{sWPTag,bRun,sCreatureTag});
		}
		
		/// <summary>
		/// Make a creature walk/run to a location and then vanish (exit stage left)
		/// </summary>
		/// <param name="sCreatureTag">The tag of the creature to move/vanish</param>
		/// <param name="sWPTag">The tag of the location the creature will move to and then disappear at</param>
		/// <param name="bRun">Set to 0 for walk, 1 for run</param>
		public static NWN2ScriptFunctor CreatureMovesAndDisappears(string sCreatureTag, string sWPTag, int bRun)
		{
			return ScriptLibrary.GetScriptFunctor("ga_force_exit",new object[]{sCreatureTag,sWPTag,bRun});
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
		/// Destroy all the henchmen in the party. If you just want to remove them, use RemoveHenchman().
		/// </summary>
		/// <description>ga_destroy_party_henchmen</description>
		/// <returns></returns>
		public static NWN2ScriptFunctor DestroyAllHenchmen()
		{
			return ScriptLibrary.GetScriptFunctor("ga_destroy_party_henchmen",null);
		}
		
		/// <summary>
		/// If the screen has faded to black (or another colour), fade out from that colour over a number of seconds.
		/// </summary>
		/// <param name="fSpeed">Number of seconds to fade in over (0.0 for instantly).</param>
		public static NWN2ScriptFunctor FadeIn(float fSpeed)
		{
			return ScriptLibrary.GetScriptFunctor("ga_fade_from_black",new object[]{fSpeed});
		}
		
		/// <summary>
		/// Fade the screen to black (or another colour).
		/// </summary>
		/// <param name="fSpeed">Number of seconds to fade out over (0.0 for instantly).</param>
		/// <param name="fFailsafe">Maximum length of time to allow fade-out for before automatically fading back in 
		/// (passing 0.0 results in the default value of 15 seconds)</param>
		/// <param name="fadeColour">The colour to fade to - either black, white or red.</param>
		public static NWN2ScriptFunctor FadeOut(float fSpeed, float fFailsafe, FadeColour fadeColour)
		{
			return ScriptLibrary.GetScriptFunctor("ga_fade_to_black",new object[]{fSpeed,fFailsafe,(int)fadeColour});
		}
		
		/// <summary>
		/// Instantly make screen black (useful if you want to then 'fade in')
		/// </summary>
		public static NWN2ScriptFunctor FadeOutInstantly()
		{
			return ScriptLibrary.GetScriptFunctor("ga_blackout",null);
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
		/// Heal the player and his companions.
		/// </summary>
		/// <param name="nHealPercent">The percentage damage to heal, from 0 to 100, 100 being the most.</param>
		public static NWN2ScriptFunctor HealPC(int nHealPercent)
		{
			int bAllPartyMembers = 1; // not useful
			return ScriptLibrary.GetScriptFunctor("ga_heal_pc",new object[]{nHealPercent,bAllPartyMembers});
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
		/// Play a sound file on an object.
		/// </summary>
		/// <param name="sSound">The filename (without extension or directory) of the sound to play</param>
		/// <param name="sTarget">The object to play the sound on - it is recommended to only play sounds on creature objects</param>
		/// <param name="fDelay">The delay before playing the sound, in seconds</param>
		/// <returns></returns>
		public static NWN2ScriptFunctor PlaySound(string sSound, string sTarget, float fDelay)
		{
			return ScriptLibrary.GetScriptFunctor("ga_play_sound",new object[]{sSound,sTarget,fDelay});
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
		/// Set the value of a global float variable
		/// </summary>
		/// <param name="sVariable">The variable to set the value of</param>
		/// <param name="sChange">The operation to perform on the variable, e.g.:
		/// "5.1"    (Set to 5.1)
		///	"=-2.1"  (Set to -2.1)
		///	"+3.5"   (Add 3.5)
		///	"+"    (Add 1.0)
		///	"++"   (Add 1.0)
		///	"-4.8"   (Subtract 4.8)
		///	"-"    (Subtract 1.0)
		///	"--"   (Subtract 1.0)</param>
		/// <returns></returns>
		public static NWN2ScriptFunctor SetGlobalFloat(string sVariable, string sChange)
		{
			return ScriptLibrary.GetScriptFunctor("ga_global_float",new object[]{sVariable,sChange});
		}
		
		/// <summary>
		/// Set the value of a global int variable
		/// </summary>
		/// <param name="sVariable">The variable to set the value of</param>
		/// <param name="sChange">The operation to perform on the variable, e.g.:
		/// "5"    (Set to 5)
		///	"=-2"  (Set to -2)
		///	"+3"   (Add 3)
		///	"+"    (Add 1)
		///	"++"   (Add 1)
		///	"-4"   (Subtract 4)
		///	"-"    (Subtract 1)
		///	"--"   (Subtract 1)</param>
		public static NWN2ScriptFunctor SetGlobalInt(string sVariable, string sChange)
		{
			return ScriptLibrary.GetScriptFunctor("ga_global_int",new object[]{sVariable,sChange});
		}
						
		/// <summary>
		/// Set the value of a global string variable
		/// </summary>
		/// <param name="sVariable">The variable to set the value of</param>
		/// <param name="sChange">The new value of the variable</param>
		/// <returns></returns>
		public static NWN2ScriptFunctor SetGlobalString(string sVariable, string sChange)
		{
			return ScriptLibrary.GetScriptFunctor("ga_global_string",new object[]{sVariable,sChange});
		}		
		
		/// <summary>
		/// Set a creature to be immortal or not immortal
		/// </summary>
		/// <param name="sTarget">Tag of target creature, if blank use OWNER</param>
		/// <param name="bImmortal">0 for FALSE, else for TRUE</param>
		public static NWN2ScriptFunctor SetImmortal(string sTarget, int bImmortal)
		{
			return ScriptLibrary.GetScriptFunctor("ga_setimmortal",new object[]{sTarget,bImmortal});
		}
				
		/// <summary>
		/// Advance time to a particular time of day
		/// </summary>
		/// <param name="nHour">Hour</param>
		/// <param name="nMinute">Minute</param>
		/// <param name="nSecond">Second</param>
		/// <param name="nMillisecond">Milliseconds</param>
		public static NWN2ScriptFunctor SetTime(int nHour, int nMinute, int nSecond, int nMillisecond)
		{
			return ScriptLibrary.GetScriptFunctor("ga_time_set",new object[]{nHour,nMinute,nSecond,nMillisecond});
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
