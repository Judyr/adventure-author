﻿/*
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
using System.IO;
using System.Text;
using AdventureAuthor.Core;
using AdventureAuthor.Utils;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.Blueprints;
using NWN2Toolset.NWN2.Data.Instances;
using NWN2Toolset.NWN2.Data.Journal;
using NWN2Toolset.NWN2.Data.Templates;
using NWN2Toolset.NWN2.Data.TypedCollections;
using NWN2Toolset.NWN2.IO;
using OEIShared.IO;
using OEIShared.Utils;

namespace AdventureAuthor.Scripts
{
	/// <summary>
	/// Generates script objects, descriptions for scripts, lists of tags and other useful functions.
	/// </summary>
	public static class ScriptHelper
	{		
		//TODO: write a script to give player all usable item feats and massive lore and automatically attach it upon the creation
		//of a new adventure
				
		#region Constants
		
		private static string ownerName = "[OWNER]";		
		
		public enum Origin {
			NWN2, // included with game - located at Neverwinter Nights 2/Data/Scripts.zip on the main drive
			AdventureAuthor // included with Adventure Author - located at Neverwinter Nights 2/Override on the main drive
		}
		
		public enum FadeColour { 
			Black = 0, 
			White = 16777215, 
			Blue = 255,
			Red = 16711680,
			Yellow = 16776960,
			Green = 32768,
			Orange = 16753920,
			Purple = 8388736,
			Pink = 16761035,
			Gold = 16766720,
			Silver = 12632256,
			Brown = 10824234,
			Grey = 8421504,
		};
		
		public enum Feat { 
			Cleave = 6, 
			Knockdown = 23, 
			PowerAttack = 28 
		};
		
		public enum Animation { 
			// Looping:
			Pause = 0,
			Pause2 = 1,
			Listen = 2,
			Meditate = 3, 
			Worship = 4, 
			LookFar = 5,
//			SitInChair = 6,
			SitCrossLegged = 7,
			TalkNormal = 8,
			TalkPleading = 9,
			TalkForceful = 10,
			TalkLaughing = 11,
			GetLow = 12,
			GetMid = 13,
			PauseTired = 14,
			PauseDrunk = 15,
			DeadFront = 16,
			DeadBack = 17,
			Conjure1 = 18,
			Conjure2 = 19,
			Spasm = 20,
//			Custom1 = 21,
//			Custom2 = 22,
			Cast1 = 23,
			Prone = 24,
			Kneel = 25,			
			Dance1 = 26, 
			Dance2 = 27, 
			Dance3 = 28,
			PlayGuitar = 29,
			IdleGuitar = 30,
			PlayFlute = 31,
			IdleFlute = 32,
			PlayDrum = 33,
			IdleDrum = 34,
			Cook1 = 35,
			Cook2 = 36,
			Craft = 37,
			Forge = 38,
			BoxCarry = 39,
			BoxIdle = 40,
			BoxHurried = 41,
			Lookown = 42,
			LookUp = 43,
			LookLeft = 44,
			LookRight = 45,
			Shoveling = 46,
			Injured = 47,
			
			// One-time:
			TurnHeadLeft = 100,
			TurnHeadRight = 101,
			PauseScratchHead = 102,
			PauseBored = 103,
			Salute = 104,
			Bow = 105,
			Steal = 106,
			Greeting = 107,
			Taunt = 108,
			Victory1 = 109,
			Victory2 = 110,
			Victory3 = 111,
			Read = 112,
			Drink = 113,
			DodgeToSide = 114,
			Duck = 115,
//			Spasm = 116, // seems to be repeated
			Collapse = 117,
			LieDown = 118,
			StandUp = 119,
			Activate = 120,
			UseItem = 121,
			KneelFidget = 122,
			KneelTalk = 123,
			KneelDamage = 124,
			KneelDeath = 125,
			Sing = 126,
			FidgetWithGuitar = 127,
			FidgetWithFlute = 128,
			FidgetWithDrum = 129,
			Wildshape = 130,
			Search = 131,
			Intimidate = 132,
			Chuckle = 133			
		};
		
		public enum Movie { //.bik files in Neverwinter Nights 2/Movies
			AtariLogo,
			Credits,
			Intro,
			Legal,
			NvidiaLogo,
			OEIlogo,
			WOTCLogo
		}
		
		public enum TaggedType { 
			AnyObject, 
			Creature, 
			Door, 
			Encounter, 
			Item, 
			JournalCategory, // note that journal does not exist as an object
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
						
		public enum VariableType {
			String,
			Int
		}
		
		#endregion
		
		#region Creating scripts
		
		/// <summary>
		/// Returns a conditional functor given a script name and an array of parameter arguments.
		/// </summary>
		/// <param name="scriptName">The name/resref of the script, e.g. gc_item_count (no file extension)</param>
		/// <param name="args">The arguments to pass into the script method.</param>
		/// <returns>Returns a conditional functor, or null if failed.</returns>
		public static NWN2ConditionalFunctor GetConditionalFunctor(string scriptName, object[] args, Origin origin)
		{
			NWN2ConditionalFunctor conditionalFunctor = GetFunctor(scriptName,args,origin);
			
			// TODO: Do stuff with AND, OR and NOT here.
			
			return conditionalFunctor;
		}	
		
		/// <summary>
		/// Returns a script functor given a script name and an array of parameter arguments.
		/// </summary>
		/// <param name="scriptName">The name/resref of the script, e.g. ga_attack (no file extension)</param>
		/// <param name="args">The arguments to pass into the script method.</param>
		/// <returns>Returns a script functor, or null if failed.</returns>
		public static NWN2ScriptFunctor GetScriptFunctor(string scriptName, object[] args, Origin origin)
		{
			NWN2ScriptFunctor scriptFunctor = (NWN2ScriptFunctor)GetFunctor(scriptName,args,origin);
			return scriptFunctor;
		}
		
		/// <summary>
		/// Returns a script functor given a script name and an array of parameter arguments.
		/// </summary>
		/// <param name="scriptName">The name/resref of the script, e.g. ga_attack (no file extension)</param>
		/// <param name="args">The arguments to pass into the script method.</param>
		/// <returns>Returns a script functor, or null if failed.</returns>
		private static NWN2ConditionalFunctor GetFunctor(string scriptName, object[] args, Origin origin)
		{
			try {
				NWN2GameScript script = GetScript(scriptName, origin);
				if (script == null) {
					Say.Error("Couldn't find a script named '" + scriptName + "'.");
					return null;
				}		
								
				NWN2ConditionalFunctor functor = new NWN2ConditionalFunctor();
				functor.Script = script.Resource;
				
				if (args != null) {
					foreach (object o in args) { // float, int, string or tag(?)
						NWN2ScriptParameter param = new NWN2ScriptParameter();
						if (o == null) {
							Say.Error("Was passed a null parameter for the script - returning.");
							return null;
						}
						else if (o is string) {
							param.ParameterType = NWN2ScriptParameterType.String;
							param.ValueString = (string)o;
						}
						else if (o is int) {
							param.ParameterType = NWN2ScriptParameterType.Int;
							param.ValueInt = (int)o;				
						}
						else if (o is float) {
							param.ParameterType = NWN2ScriptParameterType.Float;
							param.ValueFloat = (float)o;
						}						
						else {
							throw new ArgumentException("Was passed an argument of type " + o.GetType().ToString() + 
							                            " which is not valid as a script parameter.");
						}	
						functor.Parameters.Add(param);
					}		
				}
				return functor;
			}
			catch (ArgumentException e) {
				Say.Error(e);
				return null;
			}
		}	
		
		/// <summary>
		/// Retrieves a compiled (.NCS) script of a given name/resref.
		/// </summary>
		/// <param name="name">The name/resref of the script, e.g. ga_attack (no file extension)</param>
		/// <param name="origin">The origin of the script; i.e. a NWN2 game script, or an Adventure Author script</param>
		/// <returns>Returns the compiled script, or null if no script was found.</returns>
		private static NWN2GameScript GetScript(string name, Origin origin)
		{
			try {	
				IResourceRepository scripts;
				if (origin == Origin.NWN2) {
					string path = Path.Combine(ResourceManager.Instance.BaseDirectory,@"Data\Scripts.zip");
					scripts = ResourceManager.Instance.GetRepositoryByName(path);
				}
				else if (origin == Origin.AdventureAuthor) {
					scripts = ((NWN2ResourceManager)ResourceManager.Instance).OverrideDirectory; // NOT UserOverrideDirectory
				}
				else {
					throw new ArgumentException("Invalid ScriptOrigin enum passed.");
				}
								
				ushort ncs = BWResourceTypes.GetResourceType("ncs");
				OEIResRef resRef = new OEIResRef(name);
				IResourceEntry entry = scripts.FindResource(resRef,ncs);
				NWN2GameScript script = new NWN2GameScript(entry);
				return script;	
			}
			catch (NullReferenceException e) {
				Say.Error("Was unable to retrieve script named '" + name + "'.",e);
				return null;
			}
			catch (ArgumentException e) {
				Say.Error(e);
				return null;
			}
		}
		
		public static bool WriteScript(NWN2ScriptFunctor scriptFunctor, string filename)
		{
			List<string> parameterNames = new List<string>(scriptFunctor.Parameters.Count);
			
			// TODO: Big issue, can I actually access stuff inside the Scripts.zip file from code? Would it take forever?
			// can I use scriptFunctor.Script.GetStream(..)?
			
			return false;
		}
		
		
		
		
//		List<string> parameters = new List<string>(parameterscount);
//
//0. Copy NWN2ScriptFunctor.Script to its new location, given the filename 'filename'. 
//1. Is this an action or a conditional?
//2a. If it's an action, search for "void main" and retrieve the rest of that line.
//2b. If it's a conditional, search for "int StartingConditional" and retrieve the rest of that line.
//3. Either replace everything after this point with "()", or replace the whole line with "void main()"/"int StartingConditional()", whichever's easiest.
//4. While you haven't reached the end of the retrieved line
//	- Fetch a word
//	- If the word is 'string' 'int' or 'float' {
//		- Fetch the next word
//		- parameters.Add(new Parameter(firstfetched,secondfetched));
//	}
//}
//5. Read through the file from the beginning.
//6. While you haven't reached the end of the file
//{
//	Fetch a word. 
//	If the word is one of those contained in parameters {
//		Delete the word.
//		int parameterIndex = the point in parameters that the word came.
//		Fetch the NWN2ScriptParameter param at functor.Parameters[parameterIndex].
//		switch (param.ParameterType)
//		{
//			case ParameterType.String:
//				Write "\"" + param.ValueString + "\"";
//				break;
//
//			case ParameterType.Tag:
//				Write "\"" + param.ValueString + "\"";
//				break;
//
//			case ParameterType.Int:
//				Write param.ValueInt.ToString();
//				break;
//
//			case ParameterType.Float:
//				Write param.ValueFloat.ToString() + "f";
//
//			default:
//				Say.Error("Invalid parameter data.");
//				return;
//		}
//	}
//}
		
		
		
		
		
		#endregion 
		
		#region Describing scripts
		
		public static string GetDescriptionForAction(NWN2ScriptFunctor action)
		{
			if (action == null) {
				return "Was given a blank action.";
			}
			
			switch (action.Script.ResRef.Value) {					
					
				case "ga_alignment":					
					if (action.Parameters[0].ValueInt > 0) {
						if (action.Parameters[1].ValueInt == 0) {
							return "PLAYER PERFORMS A NOBLE ACT.";
						}
						else {
							return "PLAYER PERFORMS A LAWFUL ACT.";
						}
					}
					else if (action.Parameters[0].ValueInt < 0) {
						if (action.Parameters[1].ValueInt == 0) {
							return "PLAYER PERFORMS AN EVIL ACT.";
						}
						else {
							return "PLAYER PERFORMS A CHAOTIC ACT.";
						}						
					}
					else {
						return "NO CHANGE TO ALIGNMENT.";
					}
					
				case "ga_attack":
					string sAttacker = action.Parameters[0].ValueString;
					return GetOwnerIfBlank(sAttacker) + " ATTACKS THE PLAYER.";					
					
				case "ga_attack_target":
					string sAttacker1 = action.Parameters[0].ValueString;
					string sTarget = action.Parameters[1].ValueString;
					return GetOwnerIfBlank(sAttacker1) + " ATTACKS " + sTarget + ".";
					
				case "ga_blackout":
					return "THE SCREEN TURNS BLACK.";
										
				case "ga_create_obj":
					string sLocationTag = action.Parameters[2].ValueString;
					string sNewTag = action.Parameters[4].ValueString;
					float fDelay = action.Parameters[5].ValueFloat;
					
					StringBuilder s0 = new StringBuilder();
					if (fDelay > 0.0f) {
						s0.Append(fDelay.ToString() + " SECONDS LATER, ");
					}
					s0.Append("" + sNewTag + " APPEARS AT LOCATION " + sLocationTag + ".");
					return s0.ToString();
					
				case "ga_death":
					return action.Parameters[0].ValueString + " IS KILLED.";
					
				case "ga_destroy":
					string sTagString = action.Parameters[0].ValueString;
					int iInstance = action.Parameters[1].ValueInt;
					float fDelay1 = action.Parameters[2].ValueFloat;
					
					StringBuilder s1 = new StringBuilder();
					if (fDelay1 > 0.0f) {
						s1.Append(fDelay1.ToString() + " SECONDS LATER, ");
					}				
					s1.Append(GetOwnerIfBlank(sTagString) + " IS REMOVED FROM THE AREA.");
					return s1.ToString();					
					
				case "ga_destroy_item":
					string destroyitemtag = action.Parameters[0].ValueString;
//					int destroyiteminstances = action.Parameters[1].ValueInt;
//					if (destroyiteminstances == -1) {
//						return "EVERY ITEM CALLED " + destroyitemtag + " IS REMOVED FROM INVENTORY.";
//					}
//					else if (destroyiteminstances == 1) {
						return destroyitemtag + " IS REMOVED FROM THE PLAYER'S INVENTORY.";
//					}
//					else {
//						return destroyiteminstances.ToString() + " ITEMS CALLED " + destroyitemtag + " ARE REMOVED FROM INVENTORY.";
//					}
					
				case "ga_destroy_party_henchmen":
					return "ALL THE PLAYER'S ALLIES ARE REMOVED FROM THE AREA.";

				case "ga_display_message":
					string message = Utils.UsefulTools.Truncate(action.Parameters[0].ValueString,60);					
					if (message.Length < action.Parameters[0].ValueString.Length) {
						return "DISPLAY MESSAGE \"" + message + "...\"";
					}
					else {
						return "DISPLAY MESSAGE \"" + message + "\"";
					}
					
				case "ga_door_close":
					string sTag = action.Parameters[0].ValueString;
					return "DOOR " + sTag + " BECOMES CLOSED.";
										
				case "ga_door_open":
					string sTag1 = action.Parameters[0].ValueString;
					return "DOOR " + sTag1 + " BECOMES OPEN.";
						
				case "ga_end_game":
					if (action.Parameters[0].ValueString != String.Empty) {
						return "GAME OVER - PLAY END MOVIE " + action.Parameters[0].ValueString + ".";
					}
					else {
						return "GAME OVER.";
					}
					
				case "ga_faction_join":
					switch (action.Parameters[1].ValueString) {
						case "$COMMONER":
							return action.Parameters[0].ValueString + " BECOMES A COMMONER.";
						case "$DEFENDER":
							return action.Parameters[0].ValueString + " BECOMES A DEFENDER.";
						case "$HOSTILE":
							return action.Parameters[0].ValueString + " BECOMES A HOSTILE.";
						case "$MERCHANT":
							return action.Parameters[0].ValueString + " BECOMES A MERCHANT.";
						default:
							return action.Parameters[0].ValueString + " JOINS " + action.Parameters[1].ValueString + "'S FACTION.";
					}
					
				case "ga_fade_from_black":
					if (action.Parameters[0].ValueFloat == 0.0f) {
						return "FADE IN (INSTANTLY).";
					}
					else {
						return "FADE IN (OVER " + action.Parameters[0].ValueFloat + " SECONDS).";
					}
					
				case "ga_fade_to_black":
					StringBuilder sb = new StringBuilder("FADE ");
					string colour = Enum.GetName(typeof(ScriptHelper.FadeColour),action.Parameters[2].ValueInt);
					if (colour == null) {
						sb.Append("OUT (");
					}
					else {
						sb.Append("TO " + colour.ToUpper() + " (");
					}
					if (action.Parameters[0].ValueFloat == 0.0f) {
						sb.Append("INSTANTLY).");
					}
					else {
						sb.Append("OVER " + action.Parameters[0].ValueFloat + " SECONDS).");
					}					
					return sb.ToString();
					
				case "ga_force_exit":
					string moves;
					if (action.Parameters[2].ValueInt == 1) {
						moves = "RUNS";
					}
					else {
						moves = "WALKS";
					}
					
					return action.Parameters[0].ValueString + " " + moves + " TO " + action.Parameters[1].ValueString
						+ " AND DISAPPEARS.";
					
				case "ga_give_feat":
					string featgiven = Enum.GetName(typeof(ScriptHelper.Feat),action.Parameters[1].ValueInt);
					if (featgiven == null) {
						return GetPlayerIfBlank(action.Parameters[0].ValueString) + 
							" RECEIVES SPECIAL ABILITY " + action.Parameters[1].ValueInt + ".";
					}
					else {
						return GetPlayerIfBlank(action.Parameters[0].ValueString) + " RECEIVES THE " + featgiven + " SPECIAL ABILITY.";
					}
					
				case "ga_give_gold":
					int nGP = action.Parameters[0].ValueInt;
					return "PLAYER RECEIVES " + nGP + " PIECES OF GOLD.";					
					
				case "ga_give_item":
					string sTemplate1 = action.Parameters[0].ValueString;
					int nQuantity = action.Parameters[1].ValueInt;
					if (nQuantity != 1) {
						return "PLAYER RECEIVES " + nQuantity + " COPIES OF " + sTemplate1 + ".";
					}
					else {
						return "PLAYER RECEIVES A " + sTemplate1 + ".";
					}
					
				case "ga_give_xp":
					return "PLAYER RECEIVES " + action.Parameters[0].ValueInt + " EXPERIENCE POINTS.";
					
				case "ga_module_float":
					return "SET FLOAT VARIABLE " + action.Parameters[0].ValueString + " TO VALUE " + action.Parameters[1].ValueString;
					
				case "ga_module_int":
					return "SET INTEGER VARIABLE " + action.Parameters[0].ValueString + " TO VALUE " + action.Parameters[1].ValueString;
						
				case "ga_module_string":
					return "SET STRING VARIABLE " + action.Parameters[0].ValueString + " TO VALUE " + action.Parameters[1].ValueString;
					
				case "ga_heal_pc":
					if (action.Parameters[0].ValueInt == 100) {
						return "THE PLAYER HAS ALL OF THEIR WOUNDS HEALED.";
					}
					else {
						return "THE PLAYER HAS " + action.Parameters[0].ValueInt + "% OF THEIR WOUNDS HEALED.";
					}
					
				case "ga_henchman_add": 
					string sTarget1 = action.Parameters[0].ValueString;
					string sMaster = action.Parameters[2].ValueString;
					return GetOwnerIfBlank(sTarget1) + " BECOMES AN ALLY OF " + GetPlayerIfBlank(sMaster) + ".";
						
				case "ga_henchman_remove":
					return action.Parameters[0].ValueString + " STOPS BEING THE PLAYER'S ALLY.";
					
				case "ga_jump":
					string jumpdestination = action.Parameters[0].ValueString;
					string thingthatjumps = action.Parameters[1].ValueString;
					float timetowait = action.Parameters[2].ValueFloat;					
					if (timetowait != 0.0) {
						return timetowait.ToString() + " SECONDS LATER, " + thingthatjumps + " TELEPORTS TO " + jumpdestination + ".";
					}
					else {
						return "" + thingthatjumps + " TELEPORTS TO " + jumpdestination + ".";
					}
					
				case "ga_jump_players":
					string locationtojumpto = action.Parameters[0].ValueString;
					return "PLAYER TELEPORTS TO LOCATION " + locationtojumpto + ".";
					
				case "ga_lock":
					string locktag = action.Parameters[0].ValueString;
					int lockstatus = action.Parameters[1].ValueInt;
					if (lockstatus == 1) {
						return "DOOR " + locktag + " BECOMES LOCKED.";
					}
					else {
						return "DOOR " + locktag + " BECOMES UNLOCKED.";
					}
					
				case "ga_move":
					string moves2;
					if (action.Parameters[1].ValueInt == 1) {
						moves2 = "RUNS";
					}
					else {
						moves2 = "WALKS";
					}					
					return action.Parameters[2].ValueString + " " + moves2 + " TO " + action.Parameters[0].ValueString;
						
				case "ga_open_store":
					switch (action.Parameters[1].ValueInt) {
						case 0:
							switch (action.Parameters[2].ValueInt) {
								case 0:
									return "OPEN STORE " + action.Parameters[0].ValueString + ".";
								default:
									return "OPEN STORE " + action.Parameters[0].ValueString + " WITH PRICES REDUCED BY " +
										action.Parameters[2].ValueInt + " PERCENT.";
							}
						default:
							switch (action.Parameters[2].ValueInt) {
								case 0:
									return "OPEN STORE " + action.Parameters[0].ValueString + " WITH PRICES INCREASED BY " +
										action.Parameters[1].ValueInt + " PERCENT.";
								default:
									int overall = action.Parameters[1].ValueInt - action.Parameters[2].ValueInt;
									if (overall == 0) {
										return "OPEN STORE " + action.Parameters[0].ValueString + ".";
									}
									else if (overall > 0) {
										return "OPEN STORE " + action.Parameters[0].ValueString + " WITH PRICES INCREASED BY " +
											overall + " PERCENT.";
									}
									else {
										return "OPEN STORE " + action.Parameters[0].ValueString + " WITH PRICES REDUCED BY " +
											(0-overall) + " PERCENT.";
									}
							}
					}
										
				case "ga_play_animation":
					string animName = Enum.GetName(typeof(ScriptHelper.Animation),action.Parameters[1].ValueInt);
					if (animName == null) {
						animName = "unidentified animation";
					}
					StringBuilder ga_play_animation = new StringBuilder();
					if (action.Parameters[4].ValueFloat != 0.0f) {
						ga_play_animation.Append(action.Parameters[4].ValueFloat.ToString() + " SECONDS LATER, ");
					}
					ga_play_animation.Append(GetPlayerIfBlank(action.Parameters[0].ValueString) + " PLAYS ANIMATION " + animName);
					if (action.Parameters[2].ValueFloat > 1.0f) {
						ga_play_animation.Append(" QUICKLY");
					}
					else if (action.Parameters[2].ValueFloat < 1.0f) {
						ga_play_animation.Append(" SLOWLY");
					}			
					ga_play_animation.Append(".");
					return ga_play_animation.ToString();
					
				case "ga_play_sound":
					StringBuilder ga_play_sound = new StringBuilder();
					if (action.Parameters[2].ValueFloat != 0.0f) {
						ga_play_sound.Append(action.Parameters[2].ValueFloat.ToString() + " SECONDS LATER, ");
					}
					ga_play_sound.Append("THE SOUND " + action.Parameters[0].ValueString + 
					                     " PLAYS AT " + action.Parameters[1].ValueString + ".");
					return ga_play_sound.ToString();
					
				case "ga_remove_feat":
					string featremoved = Enum.GetName(typeof(ScriptHelper.Feat),action.Parameters[1].ValueInt);
					if (featremoved == null) {
						return GetPlayerIfBlank(action.Parameters[0].ValueString) + 
							" LOSES SPECIAL ABILITY " + action.Parameters[1].ValueInt + ".";
					}
					else {
						return GetPlayerIfBlank(action.Parameters[0].ValueString) + " LOSES SPECIAL ABILITY " + featremoved + ".";
					}
										
				case "ga_setimmortal":
					if (action.Parameters[1].ValueInt == 1) {
						return action.Parameters[0].ValueString + " BECOMES IMMORTAL.";
					}
					else {
						return action.Parameters[0].ValueString + " BECOMES MORTAL.";
					}
					
				case "ga_take_gold":
					int goldtotake = action.Parameters[0].ValueInt;
					return "PLAYER LOSES " + goldtotake + " PIECES OF GOLD.";
					
				case "ga_take_item":
					string tagofitem = action.Parameters[0].ValueString;
					int numberofitems = action.Parameters[0].ValueInt;
					if (numberofitems == 1) {
						return "PLAYER LOSES " + tagofitem + ".";
					}
					else if (numberofitems == -1) {
						return "PLAYER LOSES ALL ITEMS WITH TAG " + tagofitem + ".";
					}
					else {
						return "PLAYER LOSES " + numberofitems + " ITEMS WITH TAG " + tagofitem + ".";
					}
					
				case "ga_time_advance":
					return action.Parameters[0].ValueInt.ToString() + " HOURS PASS.";
//					if (action.Parameters[0].ValueInt > 0 || action.Parameters[1].ValueInt > 0) {						
//						return action.Parameters[0].ValueInt.ToString() + " HOURS AND " + action.Parameters[1].ValueInt + " MINUTES PASS.";
//					}
//					else {
//						return action.Parameters[2].ValueInt.ToString() + " SECONDS AND " + action.Parameters[3].ValueInt + " MILLISECONDS PASS.";
//					}
					
				case "ga_time_set":
					StringBuilder ga_time_set = new StringBuilder("TIME PASSES. THE TIME IS NOW ");
					
					if (action.Parameters[0].ValueInt == 0) {
						ga_time_set.Append(" MIDNIGHT.");
					}
					else if (action.Parameters[0].ValueInt == 12) {
						ga_time_set.Append(" NOON.");
					}
					else if (action.Parameters[0].ValueInt < 12) {
						ga_time_set.Append(action.Parameters[0].ValueInt.ToString() + "AM.");
					}
					else if (action.Parameters[0].ValueInt > 12) {
						ga_time_set.Append(action.Parameters[0].ValueInt.ToString() + "PM.");
					}                                   
					
					return ga_time_set.ToString();
					
				default:
					return "SCRIPT: " + action.ToString();				
			}
		}
			 	
		
		public static string GetDescriptionForCondition(NWN2ConditionalFunctorCollection conditions)
		{
			if (conditions == null || conditions.Count == 0) {
				return "Was given a blank set of conditions.";
			}
			if (conditions.Count > 1) {
				Say.Error("Tried to describe a line with more than one condition attached to it - only the first will be described.");
			}
			
			NWN2ConditionalFunctor condition = conditions[0];
			return GetDescriptionForCondition(condition);
		}
		
		public static string GetDescriptionForCondition(NWN2ConditionalFunctor condition)
		{
			if (condition == null) {
				return "Was given a blank condition.";
			}			
			
			switch (condition.Script.ResRef.Value) {
				case "gc_align_chaotic":
					return "IF THE PLAYER'S ALIGNMENT IS CHAOTIC";
					
				case "gc_align_evil":
					return "IF THE PLAYER IS EVIL";
					
				case "gc_align_good":
					return "IF THE PLAYER IS GOOD";
					
				case "gc_align_lawful":
					return "IF THE PLAYER'S ALIGNMENT IS LAWFUL";
					
				case "gc_check_gold":
					return "IF THE PLAYER HAS AT LEAST " + condition.Parameters[0].ValueInt + " GOLD PIECES";
					
				case "gc_check_item":
					return "IF THE PLAYER HAS THE ITEM " + condition.Parameters[0].ValueString;
					
				case "gc_dead":
					return "IF " + condition.Parameters[0].ValueString + " IS DEAD";					
										
				case "gc_distance_pc":
					return "IF THE DISTANCE BETWEEN THE PLAYER AND " + condition.Parameters[0].ValueString + 
						" IN METRES IS " + condition.Parameters[1].ValueString;						
					
				case "gc_distance":
					return "IF THE DISTANCE BETWEEN " + condition.Parameters[0].ValueString + " AND " + condition.Parameters[1].ValueString +
						" IN METRES IS " + condition.Parameters[2].ValueString;	
					
				case "gc_equipped":
					return "IF THE PLAYER HAS EQUIPPED ITEM " + condition.Parameters[0].ValueString + ".";
					
				case "gc_module_float":
					return "IF FLOAT VARIABLE " + condition.Parameters[0].ValueString + " IS OF VALUE " + condition.Parameters[1].ValueString;
					
				case "gc_module_int":
					return "IF INTEGER VARIABLE " + condition.Parameters[0].ValueString + " IS OF VALUE " + condition.Parameters[1].ValueString;
						
				case "gc_module_string":
					return "IF STRING VARIABLE " + condition.Parameters[0].ValueString + " IS OF VALUE " + condition.Parameters[1].ValueString;
					
				case "gc_henchman":
					if (condition.Parameters[1].ValueString != String.Empty) {
						return "IF " + condition.Parameters[0].ValueString + " IS AN ALLY OF " + condition.Parameters[1].ValueString;
					}
					else {
						return "IF " + condition.Parameters[0].ValueString + " IS THE PLAYER'S ALLY";
					}
					
				case "gc_is_enemy_near":
					return "IF A HOSTILE CREATURE IS WITHIN " + condition.Parameters[0].ValueFloat + " METRES OF THE PLAYER";
					
				case "gc_is_female":
					return "IF THE PLAYER IS FEMALE";
					
				case "gc_is_male":
					return "IF THE PLAYER IS MALE";
					
				case "gc_is_open": // not currently used
					return "IF " + condition.Parameters[0].ValueString + " IS OPEN";
					
				case "gc_item_count": // not currently used
					return "IF THE PLAYER HAS " + condition.Parameters[1].ValueString + 
						   " COPIES OF ITEM " + condition.Parameters[0].ValueString + "";
					
				default:
					return "SCRIPT: " + condition.ToString();
			}
		}
		
		private static string GetOwnerIfBlank(string tag)
		{
			if (tag == String.Empty) {
				return ownerName;
			}
			else {
				return tag;
			}
		}
		
		private static string GetPlayerIfBlank(string tag)
		{
			if (tag == String.Empty) {
				return "PLAYER";
			}
			else {
				return tag;
			}
		}
		
		#endregion 
		
		#region Tags and blueprints
				
		public static SortedList<string,string> GetResRefs(TaggedType type)
		{
			if (Adventure.CurrentAdventure == null) {
				return null;
			}
						
			switch (type) {
				case TaggedType.AnyObject:
					SortedList<string,string> resrefs = new SortedList<string,string>();
					foreach (NWN2BlueprintCollection blueprintCollection in NWN2GlobalBlueprintManager.Instance.BlueprintCollections) {
						Merge(ref resrefs,GetResRefs(blueprintCollection));
					}
					return resrefs;			
				case TaggedType.Creature:
					return GetResRefs(NWN2GlobalBlueprintManager.Instance.Creatures);
				case TaggedType.Door:
					return GetResRefs(NWN2GlobalBlueprintManager.Instance.Doors);
				case TaggedType.Encounter:
					return GetResRefs(NWN2GlobalBlueprintManager.Instance.Encounters);
				case TaggedType.Item:
					return GetResRefs(NWN2GlobalBlueprintManager.Instance.Items);
				case TaggedType.JournalCategory:
					throw new ArgumentException("Journal categories are not created from a blueprint, and so do not have resrefs.");
				case TaggedType.Light:
					return GetResRefs(NWN2GlobalBlueprintManager.Instance.Lights);
				case TaggedType.Placeable:
					return GetResRefs(NWN2GlobalBlueprintManager.Instance.Placeables);
				case TaggedType.PlacedEffect:
					return GetResRefs(NWN2GlobalBlueprintManager.Instance.PlacedEffects);
				case TaggedType.Sound:
					return GetResRefs(NWN2GlobalBlueprintManager.Instance.Sounds);
				case TaggedType.StaticCamera:
					return GetResRefs(NWN2GlobalBlueprintManager.Instance.StaticCameras);
				case TaggedType.Store:
					return GetResRefs(NWN2GlobalBlueprintManager.Instance.Stores);
				case TaggedType.Tree:
					return GetResRefs(NWN2GlobalBlueprintManager.Instance.Trees);
				case TaggedType.Trigger:
					return GetResRefs(NWN2GlobalBlueprintManager.Instance.Triggers);
				case TaggedType.Waypoint:
					return GetResRefs(NWN2GlobalBlueprintManager.Instance.Waypoints);
				default:
					return null;
			}
		}	
				
		
		private static SortedList<string,string> GetResRefs(NWN2BlueprintCollection blueprints)
		{
			SortedList<string,string> resrefs = new SortedList<string,string>(blueprints.Count);
			
			foreach (INWN2Blueprint blueprint in blueprints) {
				string resref = blueprint.TemplateResRef.Value;
				string description = null; 
				if (resref != String.Empty) {
					try {
						resrefs.Add(resref,description);
					}
					catch (ArgumentException) {	}
				}
			}
			return resrefs;
		}
		
		
		private static SortedList<string,string> Merge(ref SortedList<string,string> a, SortedList<string,string> b)
		{
			foreach (string key in b.Keys) {
				try {
					a.Add(key,b[key]);
				}
				catch (ArgumentException) { }
			}
			return a;
		}
		
		
		public static SortedList<string,string> GetTags(TaggedType type)
		{
			if (Adventure.CurrentAdventure == null) {
				return null;
			}
			
			SortedList<string,string> tags = new SortedList<string,string>();
			
			foreach (NWN2GameArea area in Adventure.CurrentAdventure.Module.Areas.Values) {
				switch (type) {
					case TaggedType.AnyObject: // excludes journal					
						foreach (NWN2InstanceCollection coll in area.AllInstances) {
							Merge(ref tags,GetTags(coll));
						}
						break;
					case TaggedType.Creature:
						Merge(ref tags,GetTags(area.Creatures));
						break;
					case TaggedType.Door:
						Merge(ref tags,GetTags(area.Doors));
						break;
					case TaggedType.Encounter:
						Merge(ref tags,GetTags(area.Encounters));
						break;
					case TaggedType.Item:
						Merge(ref tags,GetTags(area.Items));
						break;
					case TaggedType.JournalCategory:
						Merge(ref tags,GetTags(Adventure.CurrentAdventure.Module.Journal.Categories));
						break;
					case TaggedType.Light:
						Merge(ref tags,GetTags(area.Lights));
						break;
					case TaggedType.Placeable:
						Merge(ref tags,GetTags(area.Placeables));
						break;
					case TaggedType.PlacedEffect:
						Merge(ref tags,GetTags(area.PlacedEffects));
						break;
					case TaggedType.Sound:
						Merge(ref tags,GetTags(area.Sounds));
						break;
					case TaggedType.StaticCamera:
						Merge(ref tags,GetTags(area.StaticCameras));
						break;
					case TaggedType.Store:
						Merge(ref tags,GetTags(area.Stores));
						break;
					case TaggedType.Tree:
						Merge(ref tags,GetTags(area.Trees));
						break;
					case TaggedType.Trigger:
						Merge(ref tags,GetTags(area.Triggers));
						break;
					case TaggedType.Waypoint:
						Merge(ref tags,GetTags(area.Waypoints));
						break;
					default:
						throw new ArgumentException("Invalid TaggedType value.");
				}
			}
			return tags;
		}
		
		
		
		private static SortedList<string,string> GetTags(NWN2InstanceCollection instances)
		{
			SortedList<string,string> tags = new SortedList<string,string>(instances.Count);			
			foreach (INWN2Instance instance in instances) {				
				string tag = instance.Name;
				string description = null; 
				if (tag != String.Empty) {
					try {
						tags.Add(tag,description);
					}
					catch (ArgumentException) {						
					}
				}
			}
			return tags;
		}
		
				
		private static SortedList<string,string> GetTags(NWN2JournalCategoryCollection journalCategories)
		{
			SortedList<string,string> tags = new SortedList<string,string>(journalCategories.Count);			
			foreach (NWN2JournalCategory category in journalCategories) {				
				string tag = category.Tag;
				string description = null; 
				if (tag != String.Empty) {
					try {
						tags.Add(tag,description);
					}
					catch (ArgumentException) {						
					}
				}
			}
			return tags;
		}
		#endregion
		
	}
}


