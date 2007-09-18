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
using System.Collections.Generic;
using System.Windows.Controls;
using AdventureAuthor.Utils;
using NWN2Toolset.NWN2.Data;
using OEIShared.IO.GFF;

namespace AdventureAuthor.Scripts
{
	/// <summary>
	/// ~35 script actions.
	/// </summary>
	public static class Actions
	{			
		/* Actions to add:
		 * ga_effect
		 * ga_journal
		 * the 10 music scripts to start/stop battle and background music
		 * ga_party_add
		 * ga_remove_comp (should work on both henchmen and companions)
		 * ga_set_wwp_controller
		 * ga_sound_object_play
		 * ga_sound_object_stop
		 * ga_sound_object_setposition
		 * ga_sound_object_setvolume
		 * Change NPC faction
         * Make a Message Box as in Emily's game ("messagebox script.nss")
		 * */
		
		/// <summary>
		/// Add a target creature to your party as a henchman.
		/// </summary>
		/// <description>ga_henchman_add</description>
		/// <param name="sTarget">Tag of the creature you want to add</param>
		/// <param name="sMaster">The creature you are adding the henchman to</param>
		/// <param name="bOverrideBehavior">if set to 1, sTarget's event handlers (scripts) will be replaced with henchman scripts</param>
		/// <returns></returns>
		public static NWN2ScriptFunctor AddHenchman(string sTarget, string sMaster, int bOverrideBehavior)
		{
			int bForce = 1; // always add henchman even if the player has max henchmen
			return ScriptHelper.GetScriptFunctor("ga_henchman_add",new object[]{sTarget,bForce,sMaster,bOverrideBehavior},ScriptHelper.ScriptOrigin.NWN2);
		}		
				
		/// <summary>
		/// Add an existing NPC to the roster, so that they can then be added to the player's party.
		/// </summary>
		/// <remarks>The roster is the pool of NPCs from which you can add companions to the party</remarks>
		/// <param name="sRosterName">The roster name of the companion, to refer to when adding and removing them from the party</param>
		/// <param name="sTarget">The existing creature to add to the party roster</param>
		public static NWN2ScriptFunctor AddObjectToRoster(string sRosterName, string sTarget)
		{
			// TODO: It may be sensible to only allow work with either henchmen or party members, but not both. 
			return ScriptHelper.GetScriptFunctor("ga_roster_add_object",new object[]{sRosterName,sTarget},ScriptHelper.ScriptOrigin.NWN2);
		}
		
		/// <summary>
		/// Add a blueprint to the roster, so that an object created from that blueprint can then be added to the player's party.
		/// </summary>
		/// <remarks>The roster is the pool of NPCs from which you can add companions to the party</remarks>
		/// <param name="sRosterName">The roster name of the companion, to refer to when adding and removing them from the party</param>
		/// <param name="sTarget">The name of the blueprint/template to add to the party roster</param>
		public static NWN2ScriptFunctor AddBlueprintToRoster(string sRosterName, string sTarget)
		{
			// TODO: It may be sensible to only allow work with either henchmen or party members, but not both. 
			return ScriptHelper.GetScriptFunctor("ga_roster_add_blueprint",new object[]{sRosterName,sTarget},ScriptHelper.ScriptOrigin.NWN2);
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
			return ScriptHelper.GetScriptFunctor("ga_time_advance",new object[]{nHour,nMinute,nSecond,nMillisecond},ScriptHelper.ScriptOrigin.NWN2);
		}
		
		/// <summary>
		/// Make the specified NPC attack the PC.
		/// </summary>
		/// <description>ga_attack</description>
		/// <param name="sAttacker">Tag of attacker</param>
		/// <returns></returns>
		public static NWN2ScriptFunctor Attack(string sAttacker)
		{
			return ScriptHelper.GetScriptFunctor("ga_attack",new object[]{sAttacker},ScriptHelper.ScriptOrigin.NWN2);
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
			return ScriptHelper.GetScriptFunctor("ga_attack_target",new object[]{sAttacker,sTarget},ScriptHelper.ScriptOrigin.NWN2);
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
			return ScriptHelper.GetScriptFunctor("ga_door_close",new object[]{sTag,nLock},ScriptHelper.ScriptOrigin.NWN2);
		}
				       
		/// <summary>
		/// Create an object.
		/// </summary>
		/// <description>ga_create_obj</description>
		/// <param name="sTemplate">Template to create object from</param>
		/// <param name="sLocationTag">Tag of the waypoint at which to create the object</param>
		/// <param name="bUseAppearAnimation">Set to 1 to make an animation play when the object appears</param>
		/// <param name="sNewTag">Tag of the newly created object</param>
		/// <param name="fDelay">Delay, in seconds, before creating the object</param>
		/// <returns></returns>
		public static NWN2ScriptFunctor CreateCreature(string sTemplate, string sLocationTag, 
		                                             int bUseAppearAnimation, string sNewTag, float fDelay)
		{		
			return ScriptHelper.GetScriptFunctor("ga_create_obj",
			                                      new object[]{"C",sTemplate,sLocationTag,bUseAppearAnimation,sNewTag,fDelay},ScriptHelper.ScriptOrigin.NWN2);
		}
		
		/// <summary>
		/// Create an object.
		/// </summary>
		/// <description>ga_create_obj</description>
		/// <param name="sTemplate">Template to create object from</param>
		/// <param name="sLocationTag">Tag of the waypoint at which to create the object</param>
		/// <param name="bUseAppearAnimation">Set to 1 to make an animation play when the object appears</param>
		/// <param name="sNewTag">Tag of the newly created object</param>
		/// <param name="fDelay">Delay, in seconds, before creating the object</param>
		/// <returns></returns>
		public static NWN2ScriptFunctor CreateItem(string sTemplate, string sLocationTag, 
		                                             int bUseAppearAnimation, string sNewTag, float fDelay)
		{		
			return ScriptHelper.GetScriptFunctor("ga_create_obj",
			                                      new object[]{"I",sTemplate,sLocationTag,bUseAppearAnimation,sNewTag,fDelay},ScriptHelper.ScriptOrigin.NWN2);
		}					       
		/// <summary>
		/// Create an object.
		/// </summary>
		/// <description>ga_create_obj</description>
		/// <param name="sTemplate">Template to create object from</param>
		/// <param name="sLocationTag">Tag of the waypoint at which to create the object</param>
		/// <param name="bUseAppearAnimation">Set to 1 to make an animation play when the object appears</param>
		/// <param name="sNewTag">Tag of the newly created object</param>
		/// <param name="fDelay">Delay, in seconds, before creating the object</param>
		/// <returns></returns>
		public static NWN2ScriptFunctor CreatePlaceable(string sTemplate, string sLocationTag, 
		                                             int bUseAppearAnimation, string sNewTag, float fDelay)
		{		
			return ScriptHelper.GetScriptFunctor("ga_create_obj",
			                                      new object[]{"P",sTemplate,sLocationTag,bUseAppearAnimation,sNewTag,fDelay},ScriptHelper.ScriptOrigin.NWN2);
		}					       
		/// <summary>
		/// Create an object.
		/// </summary>
		/// <description>ga_create_obj</description>
		/// <param name="sTemplate">Template to create object from</param>
		/// <param name="sLocationTag">Tag of the waypoint at which to create the object</param>
		/// <param name="bUseAppearAnimation">Set to 1 to make an animation play when the object appears</param>
		/// <param name="sNewTag">Tag of the newly created object</param>
		/// <param name="fDelay">Delay, in seconds, before creating the object</param>
		/// <returns></returns>
		public static NWN2ScriptFunctor CreateStore(string sTemplate, string sLocationTag, 
		                                             int bUseAppearAnimation, string sNewTag, float fDelay)
		{		
			return ScriptHelper.GetScriptFunctor("ga_create_obj",
			                                      new object[]{"S",sTemplate,sLocationTag,bUseAppearAnimation,sNewTag,fDelay},ScriptHelper.ScriptOrigin.NWN2);
		}					       
		/// <summary>
		/// Create an object.
		/// </summary>
		/// <description>ga_create_obj</description>
		/// <param name="sTemplate">Template to create object from</param>
		/// <param name="sLocationTag">Tag of the waypoint at which to create the object</param>
		/// <param name="bUseAppearAnimation">Set to 1 to make an animation play when the object appears</param>
		/// <param name="sNewTag">Tag of the newly created object</param>
		/// <param name="fDelay">Delay, in seconds, before creating the object</param>
		/// <returns></returns>
		public static NWN2ScriptFunctor CreateWaypoint(string sTemplate, string sLocationTag, 
		                                             int bUseAppearAnimation, string sNewTag, float fDelay)
		{		
			return ScriptHelper.GetScriptFunctor("ga_create_obj",
			                                      new object[]{"W",sTemplate,sLocationTag,bUseAppearAnimation,sNewTag,fDelay},ScriptHelper.ScriptOrigin.NWN2);
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
			return ScriptHelper.GetScriptFunctor("ga_move",new object[]{sWPTag,bRun,sCreatureTag},ScriptHelper.ScriptOrigin.NWN2);
		}
		
		/// <summary>
		/// Make a creature walk/run to a location and then vanish (exit stage left)
		/// </summary>
		/// <param name="sCreatureTag">The tag of the creature to move/vanish</param>
		/// <param name="sWPTag">The tag of the location the creature will move to and then disappear at</param>
		/// <param name="bRun">Set to 0 for walk, 1 for run</param>
		public static NWN2ScriptFunctor CreatureMovesAndDisappears(string sCreatureTag, string sWPTag, int bRun)
		{
			return ScriptHelper.GetScriptFunctor("ga_force_exit",new object[]{sCreatureTag,sWPTag,bRun},ScriptHelper.ScriptOrigin.NWN2);
		}
		
		/// <summary>
		/// Destroy an object.
		/// </summary>
		/// <description>ga_destroy</description>
		/// <param name="sTagString">The tag(s) of the object(s) to destroy. You can pass multiple 
		/// tags, seperated by commas (NO SPACES) to destroy multiple objects (ie. "Object1,Object2,Object3")</param>
		/// <param name="fDelay">The delay before destroying the object(s)</param>
		public static NWN2ScriptFunctor Destroy(string sTagString, float fDelay)
		{
			int iInstance = -1; // not useful
			return ScriptHelper.GetScriptFunctor("ga_destroy",new object[]{sTagString,iInstance,fDelay},ScriptHelper.ScriptOrigin.NWN2);
		}		
		
		/// <summary>
		/// Display a message in a message box. Should probably be used on the last line of a conversation.
		/// </summary>
		/// <param name="message">The message to display.</param>
		public static NWN2ScriptFunctor DisplayMessage(string message)
		{
			return ScriptHelper.GetScriptFunctor("aa_display_messagebox",new object[]{message},ScriptHelper.ScriptOrigin.AdventureAuthor);
		}
		
		/// <summary>
		/// Destroy all the henchmen in the party. If you just want to remove them, use RemoveHenchman().
		/// </summary>
		/// <description>ga_destroy_party_henchmen</description>
		/// <returns></returns>
		public static NWN2ScriptFunctor DestroyAllHenchmen()
		{
			return ScriptHelper.GetScriptFunctor("ga_destroy_party_henchmen",null,ScriptHelper.ScriptOrigin.NWN2);
		}
		
		/// <summary>
		/// End the game, play a movie, and return the player to the main menu. 
		/// </summary>
		/// <param name="sEndMovie">The end movie to play</param>
		public static NWN2ScriptFunctor EndGame(string sEndMovie)
		{
			return ScriptHelper.GetScriptFunctor("ga_end_game",new object[]{sEndMovie},ScriptHelper.ScriptOrigin.NWN2);
		}
		    
    	/// <summary>
    	/// Make the party turn to face a particular object.
    	/// </summary>
    	/// <param name="sFacer">The tag of any member of the party faction. Can also be a GetTarget() constant (?).</param>
    	/// <param name="sTarget">Tag of object that the party will orient towards.</param>
    	public static NWN2ScriptFunctor FaceParty(string sFacer, string sTarget)
    	{
    		int bLockOrientation = 0; // not useful
    		return ScriptHelper.GetScriptFunctor("ga_party_face_target",new object[]{sFacer,sTarget,bLockOrientation},ScriptHelper.ScriptOrigin.NWN2);
    	}		
		
		/// <summary>
		/// If the screen has faded to black (or another colour), fade out from that colour over a number of seconds.
		/// </summary>
		/// <param name="fSpeed">Number of seconds to fade in over (0.0 for instantly).</param>
		public static NWN2ScriptFunctor FadeIn(float fSpeed)
		{
			return ScriptHelper.GetScriptFunctor("ga_fade_from_black",new object[]{fSpeed},ScriptHelper.ScriptOrigin.NWN2);
		}
		
		/// <summary>
		/// Fade the screen to black (or another colour).
		/// </summary>
		/// <param name="fSpeed">Number of seconds to fade out over (0.0 for instantly).</param>
		/// <param name="fFailsafe">Maximum length of time to allow fade-out for before automatically fading back in 
		/// (passing 0.0 results in the default value of 15 seconds)</param>
		/// <param name="fadeColour">The colour to fade to - either black, white or red.</param>
		public static NWN2ScriptFunctor FadeOut(float fSpeed, float fFailsafe, ScriptHelper.FadeColour fadeColour)
		{
			return ScriptHelper.GetScriptFunctor("ga_fade_to_black",new object[]{fSpeed,fFailsafe,(int)fadeColour},ScriptHelper.ScriptOrigin.NWN2);
		}
		
		/// <summary>
		/// Instantly make screen black (useful if you want to then 'fade in')
		/// </summary>
		public static NWN2ScriptFunctor FadeOutInstantly()
		{
			return ScriptHelper.GetScriptFunctor("ga_blackout",null,ScriptHelper.ScriptOrigin.NWN2);
		}
		
		/// <summary>
		/// Give the player or a creature a feat. 
		/// </summary>
		/// <remarks>A feat is a special ability, often relating to combat.</remarks>
		/// <param name="sTarget">The creature to give the feat to. If blank, assign to the player.</param>
		/// <param name="nFeat">The feat to give.</param>
		public static NWN2ScriptFunctor GiveFeat(string sTarget, ScriptHelper.Feat feat)
		{
			int nFeat = (int)feat;
			int bCheckReq = 0; // not useful
			int bAllPartyMembers = 0; // not useful
			return ScriptHelper.GetScriptFunctor("ga_give_feat",new object[]{sTarget,nFeat,bCheckReq,bAllPartyMembers},ScriptHelper.ScriptOrigin.NWN2);
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
    		return ScriptHelper.GetScriptFunctor("ga_give_gold",new object[]{nGP,bAllPartyMembers},ScriptHelper.ScriptOrigin.NWN2);
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
			return ScriptHelper.GetScriptFunctor("ga_give_item",new object[]{sTemplate,nQuantity,bAllPartyMembers},ScriptHelper.ScriptOrigin.NWN2);
		}			
		
		/// <summary>
		/// Give the player some experience.
		/// </summary>
		/// <remarks>Note that we may decide to hide 'experience points' from the player/designer altogether in future.</remarks>
		/// <param name="nXP">The number of experience points to award the player</param>
		public static NWN2ScriptFunctor GiveExperience(int nXP)
		{
			int bAllPartyMembers = 0; // not useful
			return ScriptHelper.GetScriptFunctor("ga_give_xp",new object[]{nXP,bAllPartyMembers},ScriptHelper.ScriptOrigin.NWN2);
		}
					
		/// <summary>
		/// Heal the player and his companions.
		/// </summary>
		/// <param name="nHealPercent">The percentage damage to heal, from 0 to 100, 100 being the most.</param>
		public static NWN2ScriptFunctor HealPC(int nHealPercent)
		{
			int bAllPartyMembers = 1; // not useful
			return ScriptHelper.GetScriptFunctor("ga_heal_pc",new object[]{nHealPercent,bAllPartyMembers},ScriptHelper.ScriptOrigin.NWN2);
		}
			
		/// <summary>
		/// Make a creature join a new faction.
		/// </summary>
		/// <param name="sTarget">The target whose faction will change.</param>
		/// <param name="sTargetFaction">Either one of the 4 standard factions $COMMONER, $DEFENDER, $HOSTILE, $MERCHANT or
		/// a target who's faction is to be joined (must be a creature)</param>
		public static NWN2ScriptFunctor JoinFaction(string sTarget, string sTargetFaction)
		{
			return ScriptHelper.GetScriptFunctor("ga_faction_join",new object[]{sTarget,sTargetFaction},ScriptHelper.ScriptOrigin.NWN2);
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
    		return ScriptHelper.GetScriptFunctor("ga_jump",new object[]{sDestination,sTarget,fDelay},ScriptHelper.ScriptOrigin.NWN2);
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
    		return ScriptHelper.GetScriptFunctor("ga_jump_players",new object[]{sDestTag,bWholeParty,bOnlyThisArea},ScriptHelper.ScriptOrigin.NWN2);
		}				
		
		/// <summary>
		/// Make an object appear dead
		/// </summary>
		/// <description>ga_death</description>
		/// <param name="sTag">The tag(s) of the object(s) to make dead (multiple tags can be separated by commas with no spaces)</param>
		/// <returns></returns>
		public static NWN2ScriptFunctor Kill(string sTag)
		{
			int iInstance = -1; // not useful
    		return ScriptHelper.GetScriptFunctor("ga_death",new object[]{sTag,iInstance},ScriptHelper.ScriptOrigin.NWN2);
		}	
		
		/// <summary>
		/// Open a door.
		/// </summary>
		/// <description>ga_door_open</description>
		/// <param name="sTag">Tag of door to open</param>
		public static NWN2ScriptFunctor OpenDoor(string sTag)
		{
			return ScriptHelper.GetScriptFunctor("ga_door_open",new object[]{sTag},ScriptHelper.ScriptOrigin.NWN2);
		}
				
		/// <summary>
		/// Open an existing store.
		/// </summary>
		/// <param name="sTag">The tag of the store to open.</param>
		/// <param name="nMarkUp">The percentage amount to increase all prices by (from regular prices).</param>
		/// <param name="nMarkDown">The percentage amount to reduce all prices by (from regular prices).</param>
		public static NWN2ScriptFunctor OpenStore(string sTag, int nMarkUp, int nMarkDown)
		{
			return ScriptHelper.GetScriptFunctor("ga_open_store",new object[]{sTag,nMarkUp,nMarkDown},ScriptHelper.ScriptOrigin.NWN2);
		}
		
		/// <summary>
		/// Play an animation once on a line of dialogue.
		/// </summary>
		/// <param name="sTarget">Tag of creature to play animation - default is conversation owner.</param>
		/// <param name="animation">The animation to play.</param>
		/// <param name="fSpeed">Speed animation plays at. (Default is probably 1.0?)</param>
		/// <param name="fDelayUntilStart">Number of seconds to wait before starting to play the animation.</param>
		public static NWN2ScriptFunctor PlayAnimationOnce(string sTarget, ScriptHelper.OneTimeAnimation animation, float fSpeed, float fDelayUntilStart)
		{
			float fDuration = 30; // not useful
			int iAnim = (int)animation;
			return ScriptHelper.GetScriptFunctor("ga_play_animation",new object[]{sTarget,iAnim,fSpeed,fDuration,fDelayUntilStart},ScriptHelper.ScriptOrigin.NWN2);
		}
		
		/// <summary>
		/// Play an animation repeatedly on a line of dialogue.
		/// </summary>
		/// <param name="sTarget">Tag of creature to play animation - default is conversation owner.</param>
		/// <param name="animation">The animation to play.</param>
		/// <param name="fSpeed">Speed animation plays at. (Default is probably 1.0?)</param>
		/// <param name="fDuration">Number of seconds to play this animation for.</param>
		/// <param name="fDelayUntilStart">Number of seconds to wait before starting to play the animation.</param>
		public static NWN2ScriptFunctor PlayAnimationRepeatedly(string sTarget, ScriptHelper.LoopingAnimation animation, 
		                                                        float fSpeed, float fDuration, float fDelayUntilStart)
		{
			int iAnim = (int)animation;
			return ScriptHelper.GetScriptFunctor("ga_play_animation",new object[]{sTarget,iAnim,fSpeed,fDuration,fDelayUntilStart},ScriptHelper.ScriptOrigin.NWN2);
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
			return ScriptHelper.GetScriptFunctor("ga_play_sound",new object[]{sSound,sTarget,fDelay},ScriptHelper.ScriptOrigin.NWN2);
		}
		
		/// <summary>
		/// Remove a feat from a player or creature.
		/// </summary>
		/// <param name="sTarget">The creature to remove the feat from. If blank, remove from the player.</param>
		/// <param name="feat">The feat to remove.</param>
		public static NWN2ScriptFunctor RemoveFeat(string sTarget, ScriptHelper.Feat feat)
		{
			int bAllPartyMembers = 0; // not useful
			int nFeat = (int)feat;
			return ScriptHelper.GetScriptFunctor("ga_remove_feat",new object[]{sTarget,nFeat,bAllPartyMembers},ScriptHelper.ScriptOrigin.NWN2);
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
			return ScriptHelper.GetScriptFunctor("ga_henchman_remove",new object[]{sTarget,sOptionalMasterTag},ScriptHelper.ScriptOrigin.NWN2);
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
			return ScriptHelper.GetScriptFunctor("ga_destroy_item",new object[]{sItemTag,nQuantity,bPCFaction},ScriptHelper.ScriptOrigin.NWN2);
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
			return ScriptHelper.GetScriptFunctor("ga_lock",new object[]{sDoorTag,bLock},ScriptHelper.ScriptOrigin.NWN2);
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
		public static NWN2ScriptFunctor SetFloat(string sVariable, string sChange)
		{
			return ScriptHelper.GetScriptFunctor("ga_global_float",new object[]{sVariable,sChange},ScriptHelper.ScriptOrigin.NWN2);
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
		public static NWN2ScriptFunctor SetInt(string sVariable, string sChange)
		{
			return ScriptHelper.GetScriptFunctor("ga_global_int",new object[]{sVariable,sChange},ScriptHelper.ScriptOrigin.NWN2);
		}
						
		/// <summary>
		/// Set the value of a global string variable
		/// </summary>
		/// <param name="sVariable">The variable to set the value of</param>
		/// <param name="sChange">The new value of the variable</param>
		/// <returns></returns>
		public static NWN2ScriptFunctor SetString(string sVariable, string sChange)
		{
			return ScriptHelper.GetScriptFunctor("ga_global_string",new object[]{sVariable,sChange},ScriptHelper.ScriptOrigin.NWN2);
		}		
		
		/// <summary>
		/// Set a creature to be immortal or not immortal
		/// </summary>
		/// <param name="sTarget">Tag of target creature, if blank use OWNER</param>
		/// <param name="bImmortal">0 for FALSE, else for TRUE</param>
		public static NWN2ScriptFunctor SetImmortal(string sTarget, int bImmortal)
		{
			return ScriptHelper.GetScriptFunctor("ga_setimmortal",new object[]{sTarget,bImmortal},ScriptHelper.ScriptOrigin.NWN2);
		}
				
		/// <summary>
		/// Advance time to a particular time of day
		/// </summary>
		/// <param name="nHour">Hour</param>
		/// <param name="nMinute">Minute</param>
		/// <param name="nSecond">Second</param>
		/// <param name="nMillisecond">Milliseconds</param>
		public static NWN2ScriptFunctor SetTime(int nHour, int nMinute)
		{
			int nSecond = 0; // not useful
			int nMillisecond = 0; // not useful
			return ScriptHelper.GetScriptFunctor("ga_time_set",new object[]{nHour,nMinute,nSecond,nMillisecond},ScriptHelper.ScriptOrigin.NWN2);
		}	
		
		/// <summary>
		/// Shift the player's alignment towards good or evil (because he has committed a good or evil act).
		/// </summary>
		/// <param name="amountToChangeBy">The amount to change alignment by, from 3 (a very good act) to -3 (a very evil act).</param>
		/// <remarks>The player's good/evil alignment is 100 if he is completely good, and 0 if he is completely evil.</remarks>
		public static NWN2ScriptFunctor PlayerBecomesMoreGoodOrMoreEvil(int degreeOfChange)
		{
			int axis = 0; // adjust on the Good/Evil axis
			return ScriptHelper.GetScriptFunctor("ga_alignment",new object[]{degreeOfChange,axis},ScriptHelper.ScriptOrigin.NWN2);
		}
		
		/// <summary>
		/// Shift the player's alignment towards law or chaos (because he has committed a lawful or chaotic act).
		/// </summary>
		/// <param name="amountToChangeBy">The amount to change alignment by, from 3 (a very lawful act) to -3 (a very chaotic act).</param>
		/// <remarks>The player's law/chaos alignment is 100 if he is completely lawful, and 0 if he is completely chaotic.</remarks>
		/// <remarks>A lawful act is one in which you keep a vow, obey the law, obey orders etc. A chaotic act is one in which you break
		/// a promise, break the law or simply act unexpectedly. They don't equate to good and evil - for example a cruel king or contract
		/// killer would be Lawful Evil, while somebody who robs the rich to feed the poor would be Chaotic Good.</remarks>
		public static NWN2ScriptFunctor PlayerBecomesMoreLawfulOrMoreChaotic(int degreeOfChange)
		{
			int axis = 1; // adjust on the Law/Chaos axis
			return ScriptHelper.GetScriptFunctor("ga_alignment",new object[]{degreeOfChange,axis},ScriptHelper.ScriptOrigin.NWN2);			
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
			return ScriptHelper.GetScriptFunctor("ga_take_gold",new object[]{nGold,bAllPartyMembers},ScriptHelper.ScriptOrigin.NWN2);
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
			return ScriptHelper.GetScriptFunctor("ga_take_item",new object[]{sItemTag,nQuantity,bAllPartyMembers},ScriptHelper.ScriptOrigin.NWN2);
		}	
	}
}
