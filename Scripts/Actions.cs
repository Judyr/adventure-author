/*
 *   This file is part of Adventure Author.
 *
 *   Adventure Author is copyright Heriot-Watt University 2006-2008.
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
using NWN2Toolset.NWN2.Data;
using AdventureAuthor.Utils;
using form = NWN2Toolset.NWN2ToolsetMainForm;

namespace AdventureAuthor.Scripts
{
	/// <summary>
	/// ~35 script actions.
	/// </summary>
	public static class Actions
	{			
		/* Notes on scripts:
		 * 
		 * Deleted ObjectIsNearby, ObjectIsNearObject, PlayerAnimation, because they didn't work. I think I've made
		 * the player animate before though, so should investigate fixing this.
		 * 
		 * EnemyIsNearby works in 'metres'. Game metres look about half as long as a real metre (at a guess). With the 
		 * standard fog settings, the point where you can just start to see an enemy on the horizon is (very roughly)
		 * 100 metres. Again very roughly, a big area may be 200 metres across (less sure of this). 
		 * 
		 * Alignment works, but there is a big 'neutral' area in the middle, so you need several changes to alignment
		 * in the same direction before a condition check will notice it on your character.
		 * 
		 * AttackTarget doesn't really work - attacks the faction of the target instead, which is almost always Commoner,
		 * and that essentially means anyone since Defenders will help Commoners.
		 * 
		 * 
		 * 
		 */ 
		
		
		/* Actions to add:
		 * ga_effect
		 * the 10 music scripts to start/stop battle and background music
		 * ga_set_wwp_controller
		 * Custom script for AttackPlayer that will take the creature out of the player's party first (?)
		 * */
		
		/// <summary>
		/// Add a target creature as the player's henchman.
		/// </summary>
		/// <description>ga_henchman_add</description>
		/// <param name="sTarget">Tag of the creature you want to add</param>
		/// <param name="bOverrideBehavior">if set to 1, sTarget's event handlers (scripts) will be replaced with henchman scripts</param>
		/// <returns></returns>
		public static NWN2ScriptFunctor AddHenchmanForPlayer(string sTarget, int bOverrideBehavior)
		{
			int bForce = 1; // always add henchman even if the player has max henchmen
			string sMaster = String.Empty; // add to player
			return ScriptHelper.GetScriptFunctor("ga_henchman_add",new object[]{sTarget,bForce,sMaster,bOverrideBehavior},ScriptOrigin.Original);
		}				
		
		/// <summary>
		/// Add a target creature as the henchman of another creature.
		/// </summary>
		/// <description>ga_henchman_add</description>
		/// <param name="sTarget">Tag of the creature you want to add</param>
		/// <param name="sMaster">The creature you are adding the henchman to</param>
		/// <param name="bOverrideBehavior">if set to 1, sTarget's event handlers (scripts) will be replaced with henchman scripts</param>
		/// <returns></returns>
		public static NWN2ScriptFunctor AddHenchmanForCreature(string sTarget, string sMaster, int bOverrideBehavior)
		{
			int bForce = 1; // always add henchman even if the player has max henchmen
			return ScriptHelper.GetScriptFunctor("ga_henchman_add",new object[]{sTarget,bForce,sMaster,bOverrideBehavior},ScriptOrigin.Original);
		}	
				
		/// <summary>
		/// Advance time by a given amount
		/// </summary>
		/// <param name="nHour">Hours to advance</param>
		/// <param name="nMinute">Minutes to advance</param>
		/// <param name="nSecond">Seconds to advance</param>
		/// <param name="nMillisecond">Milliseconds to advance</param>
		public static NWN2ScriptFunctor AdvanceTimeBy(int nHour)
		{
			int nMinute = 0; // not useful and works strangely
			int nSecond = 0; // not useful
			int nMillisecond = 0; // not useful
			return ScriptHelper.GetScriptFunctor("ga_time_advance",new object[]{nHour,nMinute,nSecond,nMillisecond},ScriptOrigin.Original);
		}
		
		/// <summary>
		/// Make the specified NPC attack the PC.
		/// </summary>
		/// <description>ga_attack</description>
		/// <param name="sAttacker">Tag of attacker</param>
		/// <returns></returns>
		public static NWN2ScriptFunctor Attack(string sAttacker)
		{
			return ScriptHelper.GetScriptFunctor("ga_attack",new object[]{sAttacker},ScriptOrigin.Original);
		}
		
		/// <summary>
		/// Make the specified NPC attack the specified target
		/// </summary>
		/// <description>ga_attack_target</description>
		/// <param name="sAttacker">Tag of attacker</param>
		/// <param name="sTarget">Tag of target</param>
		/// <returns></returns>
		private static NWN2ScriptFunctor AttackTarget(string sAttacker, string sTarget)
		{
			return ScriptHelper.GetScriptFunctor("ga_attack_target",new object[]{sAttacker,sTarget},ScriptOrigin.Original);
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
			return ScriptHelper.GetScriptFunctor("ga_door_close",new object[]{sTag,nLock},ScriptOrigin.Original);
		}
				       
		/// <summary>
		/// Create a creature.
		/// </summary>
		/// <description>ga_create_obj</description>
		/// <param name="sTemplate">Template to create object from</param>
		/// <param name="sNewTag">Tag of the newly created object</param>
		/// <param name="sLocationTag">Tag of the waypoint at which to create the object</param>
		/// <param name="bUseAppearAnimation">Set to 1 to make an animation play when the object appears</param>
		/// <param name="fDelay">Delay, in seconds, before creating the object</param>
		public static NWN2ScriptFunctor CreateCreature(string sTemplate, string sNewTag, string sLocationTag, 
		                                             int bUseAppearAnimation, float fDelay)
		{		
			return ScriptHelper.GetScriptFunctor("ga_create_obj",
			                                      new object[]{"C",sTemplate,sLocationTag,bUseAppearAnimation,sNewTag,fDelay},ScriptOrigin.Original);
		}
		
		/// <summary>
		/// Create an item.
		/// </summary>
		/// <description>ga_create_obj</description>
		/// <param name="sTemplate">Template to create object from</param>
		/// <param name="sNewTag">Tag of the newly created object</param>
		/// <param name="sLocationTag">Tag of the waypoint at which to create the object</param>
		/// <param name="bUseAppearAnimation">Set to 1 to make an animation play when the object appears</param>
		/// <param name="fDelay">Delay, in seconds, before creating the object</param>
		public static NWN2ScriptFunctor CreateItem(string sTemplate, string sNewTag, string sLocationTag, 
		                                             int bUseAppearAnimation, float fDelay)			
		{		
			return ScriptHelper.GetScriptFunctor("ga_create_obj",
			                                      new object[]{"I",sTemplate,sLocationTag,bUseAppearAnimation,sNewTag,fDelay},ScriptOrigin.Original);
		}					       
		/// <summary>
		/// Create a placeable.
		/// </summary>
		/// <description>ga_create_obj</description>
		/// <param name="sTemplate">Template to create object from</param>
		/// <param name="sNewTag">Tag of the newly created object</param>
		/// <param name="sLocationTag">Tag of the waypoint at which to create the object</param>
		/// <param name="bUseAppearAnimation">Set to 1 to make an animation play when the object appears</param>
		/// <param name="fDelay">Delay, in seconds, before creating the object</param>
		public static NWN2ScriptFunctor CreatePlaceable(string sTemplate, string sNewTag, string sLocationTag, 
		                                             int bUseAppearAnimation, float fDelay)
		{		
			return ScriptHelper.GetScriptFunctor("ga_create_obj",
			                                      new object[]{"P",sTemplate,sLocationTag,bUseAppearAnimation,sNewTag,fDelay},ScriptOrigin.Original);
		}					       
		/// <summary>
		/// Create a store.
		/// </summary>
		/// <description>ga_create_obj</description>
		/// <param name="sTemplate">Template to create object from</param>
		/// <param name="sNewTag">Tag of the newly created object</param>
		/// <param name="sLocationTag">Tag of the waypoint at which to create the object</param>
		/// <param name="bUseAppearAnimation">Set to 1 to make an animation play when the object appears</param>
		/// <param name="fDelay">Delay, in seconds, before creating the object</param>
		public static NWN2ScriptFunctor CreateStore(string sTemplate, string sNewTag, string sLocationTag, 
		                                             int bUseAppearAnimation, float fDelay)
		{		
			return ScriptHelper.GetScriptFunctor("ga_create_obj",
			                                      new object[]{"S",sTemplate,sLocationTag,bUseAppearAnimation,sNewTag,fDelay},ScriptOrigin.Original);
		}					       
		/// <summary>
		/// Create a waypoint.
		/// </summary>
		/// <description>ga_create_obj</description>
		/// <param name="sTemplate">Template to create object from</param>
		/// <param name="sNewTag">Tag of the newly created object</param>
		/// <param name="sLocationTag">Tag of the waypoint at which to create the object</param>
		/// <param name="bUseAppearAnimation">Set to 1 to make an animation play when the object appears</param>
		/// <param name="fDelay">Delay, in seconds, before creating the object</param>
		public static NWN2ScriptFunctor CreateWaypoint(string sTemplate, string sNewTag, string sLocationTag, 
		                                             int bUseAppearAnimation, float fDelay)
		{		
			return ScriptHelper.GetScriptFunctor("ga_create_obj",
			                                      new object[]{"W",sTemplate,sLocationTag,bUseAppearAnimation,sNewTag,fDelay},ScriptOrigin.Original);
		}			
						
		/// <summary>
		/// Make a creature walk/run to a location, and optionally vanish.
		/// </summary>
		/// <param name="sCreatureTag">The tag of the creature to make walk/run</param>
		/// <param name="sWPTag">The tag of the location the creature will move to (and then optionally disappear at)</param>
		/// <param name="bRun">Set to 0 for walk, 1 for run</param>
		/// <param name="disappear">Set to 1 to make the creature vanish once they reach their location</param>
		public static NWN2ScriptFunctor CreatureMoves(string sCreatureTag, string sWPTag, int bRun, int disappear)
		{
			if (disappear == 1) {
				return ScriptHelper.GetScriptFunctor("ga_force_exit",new object[]{sCreatureTag,sWPTag,bRun},ScriptOrigin.Original);
			}
			else {
				// parameters are in a different order from the function call, to be consistent with ga_force_exit:
				return ScriptHelper.GetScriptFunctor("ga_move",new object[]{sWPTag,bRun,sCreatureTag},ScriptOrigin.Original);
			}
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
			return ScriptHelper.GetScriptFunctor("ga_destroy",new object[]{sTagString,iInstance,fDelay},ScriptOrigin.Original);
		}		
		
		/// <summary>
		/// Display a message in a message box. Should probably be used on the last line of a conversation.
		/// </summary>
		/// <param name="message">The message to display.</param>
		public static NWN2ScriptFunctor DisplayMessage(string message)
		{
			return ScriptHelper.GetScriptFunctor("ga_display_message",new object[]{message},ScriptOrigin.Override);
		}
		
		/// <summary>
		/// Destroy all the henchmen in the party. If you just want to remove them, use RemoveHenchman().
		/// </summary>
		/// <description>ga_destroy_party_henchmen</description>
		public static NWN2ScriptFunctor DestroyAllHenchmen()
		{
			return ScriptHelper.GetScriptFunctor("ga_destroy_party_henchmen",null,ScriptOrigin.Original);
		}
		
		/// <summary>
		/// End the game, play a movie, and return the player to the main menu. 
		/// </summary>
		/// <param name="sEndMovie">The end movie to play</param>
		public static NWN2ScriptFunctor EndGame(string sEndMovie)
		{
			return ScriptHelper.GetScriptFunctor("ga_end_game",new object[]{sEndMovie},ScriptOrigin.Original);
		}
		
		/// <summary>
		/// If the screen has faded to black (or another colour), fade out from that colour over a number of seconds.
		/// </summary>
		/// <param name="fSpeed">Number of seconds to fade in over (0.0 for instantly).</param>
		public static NWN2ScriptFunctor FadeIn(float fSpeed)
		{
			return ScriptHelper.GetScriptFunctor("ga_fade_from_black",new object[]{fSpeed},ScriptOrigin.Original);
		}
		
		/// <summary>
		/// Fade the screen to black (or another colour).
		/// </summary>
		/// <param name="fadeColour">The colour to fade to - either black, white or red.</param>
		/// <param name="fSpeed">Number of seconds to fade out over (0.0 for instantly).</param>
		/// <param name="fFailsafe">Maximum length of time to allow fade-out for before automatically fading back in 
		/// (passing 0.0 results in the default value of 15 seconds)</param>
		public static NWN2ScriptFunctor FadeOut(NWN2Colour fadeColour, float fSpeed, float fFailsafe)
		{
			return ScriptHelper.GetScriptFunctor("ga_fade_to_black",new object[]{fSpeed,fFailsafe,(int)fadeColour},ScriptOrigin.Original);
		}
		
		/// <summary>
		/// Give the player a feat. 
		/// </summary>
		/// <remarks>A feat is a special ability, often relating to combat.</remarks>
		/// <param name="feat">The feat to give.</param>
		public static NWN2ScriptFunctor GivePlayerFeat(Feat feat)
		{
			string sTarget = String.Empty; // player
			int nFeat = (int)feat;
			int bCheckReq = 0; // not useful
			int bAllPartyMembers = 0; // not useful
			return ScriptHelper.GetScriptFunctor("ga_give_feat",new object[]{sTarget,nFeat,bCheckReq,bAllPartyMembers},ScriptOrigin.Original);
		}
		
		/// <summary>
		/// Give a creature a feat. 
		/// </summary>
		/// <remarks>A feat is a special ability, often relating to combat.</remarks>
		/// <param name="sTarget">The creature to give the feat to. If blank, assign to the player.</param>
		/// <param name="feat">The feat to give.</param>
		public static NWN2ScriptFunctor GiveCreatureFeat(string sTarget, Feat feat)
		{
			int nFeat = (int)feat;
			int bCheckReq = 0; // not useful
			int bAllPartyMembers = 0; // not useful
			return ScriptHelper.GetScriptFunctor("ga_give_feat",new object[]{sTarget,nFeat,bCheckReq,bAllPartyMembers},ScriptOrigin.Original);
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
    		return ScriptHelper.GetScriptFunctor("ga_give_gold",new object[]{nGP,bAllPartyMembers},ScriptOrigin.Original);
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
			return ScriptHelper.GetScriptFunctor("ga_give_item",new object[]{sTemplate,nQuantity,bAllPartyMembers},ScriptOrigin.Original);
		}			
		
		/// <summary>
		/// Give the player some experience.
		/// </summary>
		/// <remarks>Note that we may decide to hide 'experience points' from the player/designer altogether in future.</remarks>
		/// <param name="nXP">The number of experience points to award the player</param>
		public static NWN2ScriptFunctor GiveExperience(int nXP)
		{
			int bAllPartyMembers = 0; // not useful
			return ScriptHelper.GetScriptFunctor("ga_give_xp",new object[]{nXP,bAllPartyMembers},ScriptOrigin.Original);
		}
					
		/// <summary>
		/// Heal the player.
		/// </summary>
		/// <param name="nHealPercent">The percentage damage to heal, from 0 to 100, 100 being the most.</param>
		public static NWN2ScriptFunctor HealPC(int nHealPercent)
		{
			int bAllPartyMembers = 1; // not useful
			return ScriptHelper.GetScriptFunctor("ga_heal_pc",new object[]{nHealPercent,bAllPartyMembers},ScriptOrigin.Original);
		}		
		
		/// <summary>
		/// Make a creature join the Commoner faction.
		/// </summary>
		/// <param name="sTarget">The target whose faction will change.</param>
		public static NWN2ScriptFunctor CreatureJoinsCommonerFaction(string sTarget)
		{
			string sTargetFaction = "$COMMONER";
			return ScriptHelper.GetScriptFunctor("ga_faction_join",new object[]{sTarget,sTargetFaction},ScriptOrigin.Original);
		}			
		
		/// <summary>
		/// Make a creature join the Defender faction.
		/// </summary>
		/// <param name="sTarget">The target whose faction will change.</param>
		public static NWN2ScriptFunctor CreatureJoinsDefenderFaction(string sTarget)
		{
			string sTargetFaction = "$DEFENDER";
			return ScriptHelper.GetScriptFunctor("ga_faction_join",new object[]{sTarget,sTargetFaction},ScriptOrigin.Original);
		}	
		
		/// <summary>
		/// Make a creature join the Hostile faction.
		/// </summary>
		/// <param name="sTarget">The target whose faction will change.</param>
		public static NWN2ScriptFunctor CreatureJoinsHostileFaction(string sTarget)
		{
			string sTargetFaction = "$HOSTILE";
			return ScriptHelper.GetScriptFunctor("ga_faction_join",new object[]{sTarget,sTargetFaction},ScriptOrigin.Original);
		}	
		
		/// <summary>
		/// Jump a creature to a waypoint or another object.
		/// </summary>
		/// <description>ga_jump</description>
		/// <param name="sTarget">Tag of creature to jump</param>
		/// <param name="sDestination">Tag of waypoint or object to jump to</param>
		/// <param name="fDelay">Delay before jumping</param>
		public static NWN2ScriptFunctor TeleportCreature(string sTarget, string sDestination, float fDelay)
		{
			// NB: Identical to TeleportObject, but it felt more natural to have teleport creature and teleport object
			// separate in the UI, so I've replicated that here for consistency's sake.
    		return ScriptHelper.GetScriptFunctor("ga_jump",new object[]{sDestination,sTarget,fDelay},ScriptOrigin.Original);
		}
	
		/// <summary>
		/// Jump an object to a waypoint or another object.
		/// </summary>
		/// <description>ga_jump</description>
		/// <param name="sTarget">Tag of object to jump</param>
		/// <param name="sDestination">Tag of waypoint or object to jump to</param>
		/// <param name="fDelay">Delay before jumping</param>
		public static NWN2ScriptFunctor TeleportObject(string sTarget, string sDestination, float fDelay)
		{
    		return ScriptHelper.GetScriptFunctor("ga_jump",new object[]{sDestination,sTarget,fDelay},ScriptOrigin.Original);
		}
		
		/// <summary>
		/// Jump the player to a waypoint or object.
		/// </summary>
		/// <description>ga_jump_players</description>
		/// <param name="sDestTag">Tag of waypoint or object to jump to</param>
		public static NWN2ScriptFunctor TeleportPlayer(string sDestTag)
		{
			int bWholeParty = 1; // not useful
			int bOnlyThisArea = 0; // deprecated parameter
    		return ScriptHelper.GetScriptFunctor("ga_jump_players",new object[]{sDestTag,bWholeParty,bOnlyThisArea},ScriptOrigin.Original);
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
    		return ScriptHelper.GetScriptFunctor("ga_death",new object[]{sTag,iInstance},ScriptOrigin.Original);
		}	
		
		/// <summary>
		/// Open a door.
		/// </summary>
		/// <description>ga_door_open</description>
		/// <param name="sTag">Tag of door to open</param>
		public static NWN2ScriptFunctor OpenDoor(string sTag)
		{
			return ScriptHelper.GetScriptFunctor("ga_door_open",new object[]{sTag},ScriptOrigin.Original);
		}
				
		/// <summary>
		/// Open an existing store.
		/// </summary>
		/// <param name="sTag">The tag of the store to open.</param>
		/// <param name="nMarkUp">The percentage amount to increase all prices by (from regular prices).</param>
		/// <param name="nMarkDown">The percentage amount to reduce all prices by (from regular prices).</param>
		public static NWN2ScriptFunctor OpenStore(string sTag, int nMarkUp, int nMarkDown)
		{
			return ScriptHelper.GetScriptFunctor("ga_open_store",new object[]{sTag,nMarkUp,nMarkDown},ScriptOrigin.Original);
		}
				
		/// <summary>
		/// Play an animation on a line of dialogue.
		/// </summary>
		/// <param name="sTarget">Tag of creature to play animation - default is conversation owner.</param>
		/// <param name="animation">The animation to play.</param>
		/// <param name="fDelayUntilStart">Number of seconds to wait before starting to play the animation.</param>
		public static NWN2ScriptFunctor CreatureAnimation(string sTarget, Animation animation, float fDelayUntilStart)
		{
			float fDuration = 6.0f; // not used for one-time animations, simpler to give it a fixed value for the looping ones too
			float fSpeed = 1.0f; // simpler to leave this out of the equation, for now anyway
			int iAnim = (int)animation;
			return ScriptHelper.GetScriptFunctor("ga_play_animation",new object[]{sTarget,iAnim,fSpeed,fDuration,fDelayUntilStart},ScriptOrigin.Original);
		}
		
		/// <summary>
		/// Play a sound file on an object.
		/// </summary>
		/// <param name="sSound">The filename (without extension or directory) of the sound to play</param>
		/// <param name="sTarget">The object to play the sound on - it is recommended to only play sounds on creature objects</param>
		/// <param name="fDelay">The delay before playing the sound, in seconds</param>
		/// <remarks>Don't use this from a conversation as it will complicate matters, but may want the ability to play a sound
		/// on a creature/object from an OnEntry(etc.) script later on.</remarks>
		public static NWN2ScriptFunctor PlaySound(string sSound, string sTarget, float fDelay)
		{
			return ScriptHelper.GetScriptFunctor("ga_play_sound",new object[]{sSound,sTarget,fDelay},ScriptOrigin.Original);
		}
		
		/// <summary>
		/// Remove a feat from the player.
		/// </summary>
		/// <param name="feat">The feat to remove.</param>
		public static NWN2ScriptFunctor RemovePlayerFeat(Feat feat)
		{
			string sTarget = String.Empty; // player
			int bAllPartyMembers = 0; // not useful
			int nFeat = (int)feat;
			return ScriptHelper.GetScriptFunctor("ga_remove_feat",new object[]{sTarget,nFeat,bAllPartyMembers},ScriptOrigin.Original);
		}	
		
		/// <summary>
		/// Remove a feat from a creature.
		/// </summary>
		/// <param name="sTarget">The creature to remove the feat from.</param>
		/// <param name="feat">The feat to remove.</param>
		public static NWN2ScriptFunctor RemoveCreatureFeat(string sTarget, Feat feat)
		{
			int bAllPartyMembers = 0; // not useful
			int nFeat = (int)feat;
			return ScriptHelper.GetScriptFunctor("ga_remove_feat",new object[]{sTarget,nFeat,bAllPartyMembers},ScriptOrigin.Original);
		}	
		
		/// <summary>
		/// Remove a target henchman from the party.
		/// </summary>
		/// <description>ga_henchman_remove</description>
		/// <param name="sTarget">Tag of the creature you want to remove</param>
		public static NWN2ScriptFunctor RemoveHenchman(string sTarget)
		{
			string sOptionalMasterTag = String.Empty; // deprecated parameter
			return ScriptHelper.GetScriptFunctor("ga_henchman_remove",new object[]{sTarget,sOptionalMasterTag},ScriptOrigin.Original);
		}
		
		/// <summary>
		/// Take an item from the player.
		/// </summary>
		/// <description>ga_destroy_item</description>
		/// <param name="sItemTag">Tag of the item to remove</param>
		/// <returns></returns>
		public static NWN2ScriptFunctor RemoveItem(string sItemTag)
		{
			int nQuantity = -1; // destroy all items with this tag. Think we should hide the functionality of objects sharing the same tag.
			int bPCFaction = 1; // not useful
			return ScriptHelper.GetScriptFunctor("ga_destroy_item",new object[]{sItemTag,nQuantity,bPCFaction},ScriptOrigin.Original);
		}
		
		/// <summary>
		/// Lock a door
		/// </summary>
		/// <description>ga_lock</description>
		/// <param name="sDoorTag">The door to lock</param>
		/// <returns></returns>
		public static NWN2ScriptFunctor LockDoor(string sDoorTag)
		{
			int bLock = 1; // lock
			return ScriptHelper.GetScriptFunctor("ga_lock",new object[]{sDoorTag,bLock},ScriptOrigin.Original);
		}		
		
		/// <summary>
		/// Unlock a door
		/// </summary>
		/// <description>ga_lock</description>
		/// <param name="sDoorTag">The door to unlock</param>
		/// <returns></returns>
		public static NWN2ScriptFunctor UnlockDoor(string sDoorTag)
		{
			int bLock = 0; // unlock
			return ScriptHelper.GetScriptFunctor("ga_lock",new object[]{sDoorTag,bLock},ScriptOrigin.Original);
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
			return ScriptHelper.GetScriptFunctor("ga_module_float",new object[]{sVariable,sChange},ScriptOrigin.Override);
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
			return ScriptHelper.GetScriptFunctor("ga_module_int",new object[]{sVariable,sChange},ScriptOrigin.Override);
		}
						
		/// <summary>
		/// Set the value of a global string variable
		/// </summary>
		/// <param name="sVariable">The variable to set the value of</param>
		/// <param name="sChange">The new value of the variable</param>
		/// <returns></returns>
		public static NWN2ScriptFunctor SetString(string sVariable, string sChange)
		{
			return ScriptHelper.GetScriptFunctor("ga_module_string",new object[]{sVariable,sChange},ScriptOrigin.Override);
		}		
		
		/// <summary>
		/// Set a creature to be immortal
		/// </summary>
		/// <param name="sTarget">Tag of target creature</param>
		public static NWN2ScriptFunctor CreatureBecomesImmortal(string sTarget)
		{
			int bImmortal = 1; // unkillable
			return ScriptHelper.GetScriptFunctor("ga_setimmortal",new object[]{sTarget,bImmortal},ScriptOrigin.Original);
		}	
		
		/// <summary>
		/// Set a creature to be killable
		/// </summary>
		/// <param name="sTarget">Tag of target creature</param>
		public static NWN2ScriptFunctor CreatureBecomesMortal(string sTarget)
		{
			int bImmortal = 0; // killable
			return ScriptHelper.GetScriptFunctor("ga_setimmortal",new object[]{sTarget,bImmortal},ScriptOrigin.Original);
		}
				
		/// <summary>
		/// Advance time to a particular time of day
		/// </summary>
		/// <param name="nHour">Hour</param>
		/// <param name="nMinute">Minute</param>
		/// <param name="nSecond">Second</param>
		/// <param name="nMillisecond">Milliseconds</param>
		public static NWN2ScriptFunctor SetTime(int nHour)
		{
			int nMinute = 0; // not useful and works strangely
			int nSecond = 0; // not useful
			int nMillisecond = 0; // not useful
			return ScriptHelper.GetScriptFunctor("ga_time_set",new object[]{nHour,nMinute,nSecond,nMillisecond},ScriptOrigin.Original);
		}	
		
		/// <summary>
		/// Shift the player's alignment towards good (because he has committed a good act).
		/// </summary>
		/// <remarks>The player's good/evil alignment is 100 if he is completely good, and 0 if he is completely evil.</remarks>
		public static NWN2ScriptFunctor PlayerBecomesMoreGood()
		{
			int degreeOfChange = 3; // simpler to take this out of the equation (goes from 1 to 3 for good, -1 to -3 for evil)
			int axis = 0; // adjust on the Good/Evil axis
			return ScriptHelper.GetScriptFunctor("ga_alignment",new object[]{degreeOfChange,axis},ScriptOrigin.Original);
		}
		
		/// <summary>
		/// Shift the player's alignment towards evil (because he has committed a evil act).
		/// </summary>
		/// <remarks>The player's good/evil alignment is 100 if he is completely good, and 0 if he is completely evil.</remarks>
		public static NWN2ScriptFunctor PlayerBecomesMoreEvil()
		{
			int degreeOfChange = -3; // simpler to take this out of the equation (goes from 1 to 3 for good, -1 to -3 for evil)
			int axis = 0; // adjust on the Good/Evil axis
			return ScriptHelper.GetScriptFunctor("ga_alignment",new object[]{degreeOfChange,axis},ScriptOrigin.Original);
		}		
		
		/// <summary>
		/// Shift the player's alignment towards lawf (because he has committed a lawful act).
		/// </summary>
		/// <remarks>The player's lawful/chaotic alignment is 100 if he is completely lawful, and 0 if he is completely chaotic.</remarks>
		public static NWN2ScriptFunctor PlayerBecomesMoreLawful()
		{
			int degreeOfChange = 3; // simpler to take this out of the equation (goes from 1 to 3 for lawful, -1 to -3 for chaotic)
			int axis = 1; // adjust on the Law/Chaos axis
			return ScriptHelper.GetScriptFunctor("ga_alignment",new object[]{degreeOfChange,axis},ScriptOrigin.Original);
		}
		
		/// <summary>
		/// Shift the player's alignment towards chaos (because he has committed a chaotic act).
		/// </summary>
		/// <remarks>The player's lawful/chaotic alignment is 100 if he is completely lawful, and 0 if he is completely chaotic.</remarks>
		public static NWN2ScriptFunctor PlayerBecomesMoreChaotic()
		{
			int degreeOfChange = -3; // simpler to take this out of the equation (goes from 1 to 3 for lawful, -1 to -3 for chaotic)
			int axis = 1; // adjust on the Law/Chaos axis
			return ScriptHelper.GetScriptFunctor("ga_alignment",new object[]{degreeOfChange,axis},ScriptOrigin.Original);
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
			return ScriptHelper.GetScriptFunctor("ga_take_gold",new object[]{nGold,bAllPartyMembers},ScriptOrigin.Original);
		}	
	}
}
