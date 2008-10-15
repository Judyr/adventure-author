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
using form = NWN2Toolset.NWN2ToolsetMainForm;

namespace AdventureAuthor.Scripts
{
	/// <summary>
	/// Generates script objects, descriptions for scripts, lists of tags and other useful functions.
	/// </summary>
	public static class ScriptHelper
	{		
		#region Constants
		
		private static string ownerName = "[OWNER]";		
					
		#endregion
		
		#region Fields		
		
		private static ZIPResourceRepository nwn2ScriptsRepository = 
			(ZIPResourceRepository)ResourceManager.Instance.GetRepositoryByName(
				Path.Combine(ResourceManager.Instance.BaseDirectory,@"Data\Scripts.zip"));
		public static ZIPResourceRepository NWN2ScriptsRepository {
			get {
				return nwn2ScriptsRepository;
			}
		}
		
		
		private static DirectoryResourceRepository overrideRepository = 
			((NWN2ResourceManager)ResourceManager.Instance).OverrideDirectory;
		public static DirectoryResourceRepository OverrideRepository {
			get { 
				return overrideRepository; 
			}
		}
			
					
		public static DirectoryResourceRepository CurrentModuleRepository {
			get {
				return new DirectoryResourceRepository(ModuleHelper.GetCurrentModulePath());
			}
		}
		
		#endregion
		
		#region Creating scripts			
		
		/// <summary>
		/// Returns a conditional functor given a script name and an array of parameter arguments.
		/// </summary>
		/// <param name="scriptName">The name/resref of the script, e.g. gc_item_count (no file extension)</param>
		/// <param name="args">The arguments to pass into the script method.</param>
		/// <returns>Returns a conditional functor, or null if failed.</returns>
		public static NWN2ConditionalFunctor GetConditionalFunctor(string scriptName, object[] args, 
		                                                           string directoryPath)
		{
			DirectoryResourceRepository repository = new DirectoryResourceRepository(directoryPath);
			return GetConditionalFunctor(scriptName,args,repository);
		}
		
		
		/// <summary>
		/// Returns a script functor given a script name and an array of parameter arguments.
		/// </summary>
		/// <param name="scriptName">The name/resref of the script, e.g. ga_attack (no file extension)</param>
		/// <param name="args">The arguments to pass into the script method.</param>
		/// <returns>Returns a script functor, or null if failed.</returns>
		public static NWN2ScriptFunctor GetScriptFunctor(string scriptName, object[] args, 
		                                                 string directoryPath)
		{
			DirectoryResourceRepository repository = new DirectoryResourceRepository(directoryPath);
			return GetScriptFunctor(scriptName,args,repository);
		}
		
		
		/// <summary>
		/// Returns a conditional functor given a script name and an array of parameter arguments.
		/// </summary>
		/// <param name="scriptName">The name/resref of the script, e.g. gc_item_count (no file extension)</param>
		/// <param name="args">The arguments to pass into the script method.</param>
		/// <returns>Returns a conditional functor, or null if failed.</returns>
		public static NWN2ConditionalFunctor GetConditionalFunctor(string scriptName, object[] args, 
		                                                           IResourceRepository repository)
		{
			NWN2ConditionalFunctor conditionalFunctor = GetFunctor(scriptName,args,repository);
			
			// TODO: Do stuff with AND, OR and NOT here.
			return conditionalFunctor;
		}
		
		
		/// <summary>
		/// Returns a script functor given a script name and an array of parameter arguments.
		/// </summary>
		/// <param name="scriptName">The name/resref of the script, e.g. ga_attack (no file extension)</param>
		/// <param name="args">The arguments to pass into the script method.</param>
		/// <returns>Returns a script functor, or null if failed.</returns>
		public static NWN2ScriptFunctor GetScriptFunctor(string scriptName, object[] args, 
		                                                 IResourceRepository repository)
		{
			NWN2ScriptFunctor scriptFunctor = (NWN2ScriptFunctor)GetFunctor(scriptName,args,repository);
			return scriptFunctor;
		}
						
		
		/// <summary>
		/// Returns a script functor given a script name and an array of parameter arguments.
		/// </summary>
		/// <param name="scriptName">The name/resref of the script, e.g. ga_attack (no file extension)</param>
		/// <param name="args">The arguments to pass into the script method.</param>
		/// <returns>Returns a script functor, or null if failed.</returns>
		private static NWN2ConditionalFunctor GetFunctor(string scriptName, object[] args, 
		                                                 IResourceRepository repository)
		{
			try {
				IResourceEntry resource = GetScriptResource(scriptName, repository);
				NWN2GameScript script = new NWN2GameScript(resource);
				if (resource == null || script == null) {
					throw new IOException("Couldn't find a script named '" + scriptName + "'.");
				}		
								
				NWN2ConditionalFunctor functor = new NWN2ConditionalFunctor();
				functor.Script = script.Resource;
				
				// Set up parameters:
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
		/// Retrieves a compiled (.NCS) script of a given name/resref as a resource.
		/// </summary>
		/// <param name="name">The name/resref of the script, e.g. ga_attack (no file extension)</param>
		/// <param name="repository">The directory resource repository object representing the directory
		/// which contains this script.</param>
		/// <remarks>Use the static IResourceRepository objects in the ScriptHelper class
		/// to help you locate scripts.</remarks>
		/// <returns>Returns the compiled script resource, or null if no script was found.</returns>
		public static IResourceEntry GetScriptResource(string name, IResourceRepository repository)
		{
			try {									
				ushort ncs = BWResourceTypes.GetResourceType("ncs");
				OEIResRef resRef = new OEIResRef(name);
				return repository.FindResource(resRef,ncs);
			}
			catch (Exception e) {
				throw new IOException("Was unable to retrieve script named '" + name + "'.",e);
			}
		}
		
		
		public static bool WriteScript(NWN2ScriptFunctor scriptFunctor, string filename)
		{
			List<string> parameterNames = new List<string>(scriptFunctor.Parameters.Count);
			
			// TODO: Big issue, can I actually access stuff inside the Scripts.zip file from code? Would it take forever?
			// can I use scriptFunctor.Script.GetStream(..)?
			
			return false;
		}
		
		
		/// <summary>
		/// Apply default scripts for logging game events to a module.
		/// </summary>
		/// <param name="instance">The game module</param>
		/// <exception cref="IOException">thrown if a script resource couldn't be found</exception>
		internal static void ApplyDefaultScripts(NWN2GameModule module)
		{
			if (module == null) {
				throw new ArgumentNullException("module","Module to attach scripts to cannot be null.");
			}
			
			DirectoryResourceRepository repository = OverrideRepository;
			
			module.ModuleInfo.OnAcquireItem = ScriptHelper.GetScriptResource("module_onacquireitem",repository);
			module.ModuleInfo.OnActivateItem = ScriptHelper.GetScriptResource("module_onactivateitem",repository);
			module.ModuleInfo.OnClientLeave = ScriptHelper.GetScriptResource("module_onclientexit",repository);
			module.ModuleInfo.OnPCLoaded = ScriptHelper.GetScriptResource("module_onpcloaded",repository);
			module.ModuleInfo.OnPlayerDeath = ScriptHelper.GetScriptResource("module_onplayerdeath",repository);
			module.ModuleInfo.OnPlayerEquipItem = ScriptHelper.GetScriptResource("module_onplayerequipitem",repository);
			module.ModuleInfo.OnPlayerLevelUp = ScriptHelper.GetScriptResource("module_onplayerlevelup",repository);
			//module.ModuleInfo.OnPlayerRespawn = ScriptHelper.GetScriptResource("module_onplayerrespawn",loc); doesn't work
			module.ModuleInfo.OnPlayerRest = ScriptHelper.GetScriptResource("module_onplayerrest",repository);
			module.ModuleInfo.OnPlayerUnequipItem = ScriptHelper.GetScriptResource("module_onplayerunequipitem",repository);
			module.ModuleInfo.OnUnacquireItem = ScriptHelper.GetScriptResource("module_onunacquireitem",repository);
			
			// allow player to use (almost) all items by default:
			module.ModuleInfo.OnClientEnter = ScriptHelper.GetScriptResource("playercanuseallitems",repository);
		}
		
		
		/// <summary>
		/// Apply default scripts for logging game events to an area.
		/// </summary>
		/// <param name="instance">The game area</param>
		/// <exception cref="IOException">thrown if a script resource couldn't be found</exception>
		internal static void ApplyDefaultScripts(NWN2GameArea area)
		{
			if (area == null) {
				throw new ArgumentNullException("area","Area to attach scripts to cannot be null.");
			}
			
			DirectoryResourceRepository repository = OverrideRepository;
			
			area.OnEnterScript = ScriptHelper.GetScriptResource("area_trigger_onenter",repository);
		}
		
		
		/// <summary>
		/// Apply default scripts for logging game events to a game object.
		/// </summary>
		/// <param name="instance">The newly created game object</param>
		/// <exception cref="IOException">thrown if a script resource couldn't be found</exception>
		internal static void ApplyDefaultScripts(INWN2Instance instance)
		{
			if (instance == null) {
				throw new ArgumentNullException("instance","Instance to attach scripts to cannot be null.");
			}
			
			DirectoryResourceRepository repository = OverrideRepository;
			
			if (instance is NWN2CreatureInstance) {
				NWN2CreatureInstance creature = (NWN2CreatureInstance)instance;
				creature.OnConversation = ScriptHelper.GetScriptResource("creature_onconversation",repository);
				creature.OnDeath = ScriptHelper.GetScriptResource("creature_ondeath",repository);
				creature.OnPhysicalAttacked = ScriptHelper.GetScriptResource("creature_onphysicallyattacked",repository);
				creature.OnSpellCastAt = ScriptHelper.GetScriptResource("creature_onspellcastat",repository);
				//creature.FactionID = 2;
				// So that when a creature has a conversation, it was definitely added by the user:
				//creature.Conversation = null;//new OEIShared.IO.MissingResourceEntry();
			}
			else if (instance is NWN2DoorInstance) {
				NWN2DoorInstance door = (NWN2DoorInstance)instance; 
				door.OnOpen = ScriptHelper.GetScriptResource("onopen",repository);
				door.OnClosed = ScriptHelper.GetScriptResource("onclosed",repository);
				door.OnDisarm = ScriptHelper.GetScriptResource("ondisarm",repository);
				door.OnLock = ScriptHelper.GetScriptResource("onlock",repository);
				door.OnUnlock = ScriptHelper.GetScriptResource("onunlock",repository);
				door.OnConversation = ScriptHelper.GetScriptResource("door_placeable_onconversation",repository);
				door.OnDeath = ScriptHelper.GetScriptResource("door_placeable_ondeath",repository);
				door.OnTrapTriggered = ScriptHelper.GetScriptResource("ontraptriggered",repository);
				door.OnUsed = ScriptHelper.GetScriptResource("door_placeable_onused",repository);
			}
			else if (instance is NWN2EncounterInstance) {
				NWN2EncounterInstance encounter = (NWN2EncounterInstance)instance;
				encounter.OnEntered = ScriptHelper.GetScriptResource("encounter_onentered",repository);
				encounter.OnExhausted = ScriptHelper.GetScriptResource("encounter_onexhausted",repository);
			}
			else if (instance is NWN2PlaceableInstance) {
				NWN2PlaceableInstance placeable = (NWN2PlaceableInstance)instance;
				placeable.OnClosed = ScriptHelper.GetScriptResource("onclosed",repository);
				placeable.OnConversation = ScriptHelper.GetScriptResource("door_placeable_onconversation",repository);
				placeable.OnDeath = ScriptHelper.GetScriptResource("door_placeable_ondeath",repository);
				placeable.OnDisarm = ScriptHelper.GetScriptResource("ondisarm",repository);
				placeable.OnLeftClick = ScriptHelper.GetScriptResource("onleftclick",repository);
				placeable.OnLock = ScriptHelper.GetScriptResource("onlock",repository);
				placeable.OnOpen = ScriptHelper.GetScriptResource("onopened",repository);
				placeable.OnTrapTriggered = ScriptHelper.GetScriptResource("ontraptriggered",repository);
				placeable.OnUnlock = ScriptHelper.GetScriptResource("onunlock",repository);
				placeable.OnUsed = ScriptHelper.GetScriptResource("door_placeable_onused",repository);
			}
			else if (instance is NWN2StoreInstance) {
				NWN2StoreInstance store = (NWN2StoreInstance)instance;
				store.OnOpenStore = ScriptHelper.GetScriptResource("store_onopenstore",repository);
				store.OnCloseStore = ScriptHelper.GetScriptResource("store_onclosestore",repository);
			}
			else if (instance is NWN2TriggerInstance) {
				NWN2TriggerInstance trigger = (NWN2TriggerInstance)instance;
				trigger.OnEnter = ScriptHelper.GetScriptResource("area_trigger_onenter",repository);
				trigger.OnExit = ScriptHelper.GetScriptResource("trigger_onexit",repository);
				trigger.OnTrapTriggered = ScriptHelper.GetScriptResource("ontraptriggered",repository);
				trigger.OnDisarm = ScriptHelper.GetScriptResource("ondisarm",repository);
			}
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
					string message = Utils.Tools.Truncate(action.Parameters[0].ValueString,60);					
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
					string colour = Enum.GetName(typeof(NWN2Colour),action.Parameters[2].ValueInt);
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
					string featgiven = Enum.GetName(typeof(Feat),action.Parameters[1].ValueInt);
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
					return "SET DECIMAL NUMBER VARIABLE " + action.Parameters[0].ValueString + " TO VALUE " + action.Parameters[1].ValueString;
					
				case "ga_module_int":
					return "SET NUMBER VARIABLE " + action.Parameters[0].ValueString + " TO VALUE " + action.Parameters[1].ValueString;
						
				case "ga_module_string":
					return "SET WORD(S) VARIABLE " + action.Parameters[0].ValueString + " TO VALUE " + action.Parameters[1].ValueString;
					
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
					string animName = Enum.GetName(typeof(Animation),action.Parameters[1].ValueInt);
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
					string featremoved = Enum.GetName(typeof(Feat),action.Parameters[1].ValueInt);
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
					return "RUN SCRIPT: '" + Path.GetFileNameWithoutExtension(action.Script.FullName) + "'";
			}
		}
			 	
		
		public static string GetDescriptionForCondition(NWN2ConditionalFunctorCollection conditions)
		{
			if (conditions == null || conditions.Count == 0) {
				return "Was given a blank set of conditions.";
			}
			if (conditions.Count > 1) {
				Say.Warning("Tried to describe a line with more than one condition attached to it - only the first will be described.");
			}
			
			NWN2ConditionalFunctor condition = conditions[0];
			return GetDescriptionForCondition(condition);
		}
		
		public static string GetDescriptionForCondition(NWN2ConditionalFunctor condition)
		{
			if (condition == null) {
				return "Was given a blank condition.";
			}			
			
			string description;
			
			switch (condition.Script.ResRef.Value) {
				case "gc_align_chaotic":
					description = "THE PLAYER'S ALIGNMENT IS CHAOTIC";
					break;
					
				case "gc_align_evil":
					description = "THE PLAYER IS EVIL";
					break;
					
				case "gc_align_good":
					description = "THE PLAYER IS GOOD";
					break;
					
				case "gc_align_lawful":
					description = "THE PLAYER'S ALIGNMENT IS LAWFUL";
					break;
					
				case "gc_check_gold":
					description = "THE PLAYER HAS AT LEAST " + condition.Parameters[0].ValueInt + " GOLD PIECES";
					break;
					
				case "gc_check_item":
					description = "THE PLAYER HAS THE ITEM " + condition.Parameters[0].ValueString + "";
					break;
					
				case "gc_dead":
					description = "" + condition.Parameters[0].ValueString + " IS DEAD";	
					break;			
										
				case "gc_distance_pc":
					description = "THE DISTANCE BETWEEN THE PLAYER AND " + condition.Parameters[0].ValueString + 
						" IN METRES IS " + condition.Parameters[1].ValueString + "";	
					break;			
					
				case "gc_distance":
					description = "THE DISTANCE BETWEEN " + condition.Parameters[0].ValueString + " AND " + condition.Parameters[1].ValueString +
						" IN METRES IS " + condition.Parameters[2].ValueString + "";
					break;
					
				case "gc_equipped":
					description = "THE PLAYER HAS EQUIPPED ITEM " + condition.Parameters[0].ValueString + "";
					break;
					
				case "gc_module_float":
					description = "DECIMAL NUMBER VARIABLE " + condition.Parameters[0].ValueString + " IS OF VALUE " + 
						condition.Parameters[1].ValueString + "";
					break;
					
				case "gc_module_int":
					description = "NUMBER VARIABLE " + condition.Parameters[0].ValueString + " IS OF VALUE " + 
						condition.Parameters[1].ValueString + "";
					break;
						
				case "gc_module_string":
					description = "WORD(S) VARIABLE " + condition.Parameters[0].ValueString + " IS OF VALUE " + 
						condition.Parameters[1].ValueString + "";
					break;
					
				case "gc_henchman":
					if (condition.Parameters[1].ValueString != String.Empty) {
						description = "" + condition.Parameters[0].ValueString + " IS AN ALLY OF " + 
							condition.Parameters[1].ValueString + "";
					}
					else {
						description = "" + condition.Parameters[0].ValueString + " IS THE PLAYER'S ALLY";
					}
					break;
					
				case "gc_is_enemy_near":
					description = "A HOSTILE CREATURE IS WITHIN " + condition.Parameters[0].ValueFloat + 
						" METRES OF THE PLAYER";
					break;
					
				case "gc_is_female":
					description = "THE PLAYER IS FEMALE";
					break;
					
				case "gc_is_male":
					description = "THE PLAYER IS MALE";
					break;
					
				case "gc_is_open": // not currently used
					description = "" + condition.Parameters[0].ValueString + " IS OPEN";
					break;
					
				case "gc_item_count": // not currently used
					description = "THE PLAYER HAS " + condition.Parameters[1].ValueString + 
						   " COPIES OF ITEM " + condition.Parameters[0].ValueString + "";
					break;
					
				default:
					description = "(SCRIPT '" + Path.GetFileNameWithoutExtension(condition.Script.FullName)
								+ "' RETURNS TRUE)";
					break;
			}
			
			return description;
		}
		
		internal static string GetOwnerIfBlank(string tag)
		{
			if (tag == String.Empty) {
				return ownerName;
			}
			else {
				return tag;
			}
		}
		
		internal static string GetPlayerIfBlank(string tag)
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
				
		/// <summary>
		/// If this script functor features any variable which has a value equal to oldTag,
		/// give it the value of newTag instead.
		/// </summary>
		/// <param name="scriptFunctor">The script to check the variables of</param>
		/// <param name="oldTag">The tag to replace</param>
		/// <param name="newTag">The replacement tag</param>
		public static void ReplaceTag(NWN2ScriptFunctor script, string oldTag, string newTag)
		{
			if (newTag == null) {
				throw new ArgumentNullException(newTag);
			}
			else if (oldTag == null) {
				throw new ArgumentNullException(oldTag);
			}
			
			foreach (NWN2ScriptParameter param in script.Parameters) {							
				switch (param.ParameterType) {
					case NWN2ScriptParameterType.String:
						if (param.ValueString == oldTag) {
							param.ValueString = newTag;
							continue;
						}									
						break;
					case NWN2ScriptParameterType.Tag:
						if (param.ValueTag == oldTag) {
							param.ValueTag = newTag;
							continue;
						}									
						break;
				}
			}
		}
		
		
		public static SortedList<string,string> GetResRefs(TaggedType type)
		{
			if (!ModuleHelper.ModuleIsOpen()) {
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
			if (!ModuleHelper.ModuleIsOpen()) {
				return null;
			}
					
			SortedList<string,string> tags = new SortedList<string,string>();
			
			foreach (NWN2GameArea area in form.App.Module.Areas.Values) {
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
						Merge(ref tags,GetTags(form.App.Module.Journal.Categories));
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


