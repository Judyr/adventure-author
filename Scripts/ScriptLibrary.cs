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
using System.IO;
using System.Text;
using AdventureAuthor.Core;
using AdventureAuthor.Utils;
using AdventureAuthor.Scripts;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.TypedCollections;
using NWN2Toolset.NWN2.UI;
using OEIShared.IO;
using OEIShared.Utils;

namespace AdventureAuthor.Scripts
{
	/// <summary>
	/// Description of ScriptLibrary
	/// </summary>
	public static class ScriptLibrary
	{		
		private static string ownerName = "[OWNER]";
			
		
		/// <summary>
		/// Returns a script functor given a script name and an array of parameter arguments.
		/// </summary>
		/// <param name="scriptName">The name/resref of the script, e.g. ga_attack (no file extension)</param>
		/// <param name="args">The arguments to pass into the script method.</param>
		/// <returns>Returns a script functor, or null if failed.</returns>
		public static NWN2ScriptFunctor GetScriptFunctor(string scriptName, object[] args)
		{
			try {
				NWN2GameScript script = GetScript(scriptName);
				if (script == null) {
					Say.Error("Couldn't find a script named '" + scriptName + "'.");
					return null;
				}		
								
				NWN2ScriptFunctor scriptFunctor = new NWN2ScriptFunctor();
				scriptFunctor.Script = script.Resource;
				
				foreach (object o in args) { // float, int, string or tag(?)
					NWN2ScriptParameter param = new NWN2ScriptParameter();					
					if (o is string) {
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
					scriptFunctor.Parameters.Add(param);
				}				
				return scriptFunctor;
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
		/// <returns>Returns the compiled script, or null if no script was found.</returns>
		private static NWN2GameScript GetScript(string name)
		{
			try {
				string scriptsPath = Path.Combine(ResourceManager.Instance.BaseDirectory,@"Data\Scripts.zip");
				ResourceRepository scripts = (ResourceRepository)ResourceManager.Instance.GetRepositoryByName(scriptsPath);
				ushort ncs = BWResourceTypes.GetResourceType("ncs");
				OEIResRef resRef = new OEIResRef(name);
				IResourceEntry entry = scripts.FindResource(resRef,ncs);
				NWN2GameScript scrp = new NWN2GameScript(entry);	
				return scrp;
			}
			catch (NullReferenceException e) {
				Say.Error("Was unable to retrieve script named '" + name + "'.",e);
				return null;
			}
		}
		
		public static string GetDescription(NWN2ScriptFunctor action)
		{
			switch (action.Script.ResRef.Value) {
				case "ga_attack":
					string sAttacker = action.Parameters[0].ValueString;
					return GetOwnerIfBlank(sAttacker) + " ATTACKS THE PLAYER.";					
					
				case "ga_attack_target":
					string sAttacker1 = action.Parameters[0].ValueString;
					string sTarget = action.Parameters[0].ValueString;
					return GetOwnerIfBlank(sAttacker1) + " ATTACKS " + sTarget + ".";
										
				case "ga_create_obj":
					string sLocationTag = action.Parameters[2].ValueString;
					string sNewTag = action.Parameters[4].ValueString;
					float fDelay = action.Parameters[5].ValueFloat;
					
					StringBuilder s0 = new StringBuilder();
					if (fDelay > 0.0f) {
						s0.Append(fDelay.ToString() + " SECONDS LATER, ");
					}
					s0.Append("'" + sNewTag + "' APPEARS AT LOCATION '" + sLocationTag + "'.");
					return s0.ToString();
					
				case "ga_death":
					string deathtag = action.Parameters[0].ValueString;
					int deathinstance = action.Parameters[1].ValueInt;
					if (deathinstance > 0) {
						return "EVERYTHING CALLED '" + deathtag + "' IS KILLED.";
					}
					else {
						return deathtag.ToUpper() + " IS KILLED.";
					}
					
				case "ga_destroy":
					string sTagString = action.Parameters[0].ValueString;
					int iInstance = action.Parameters[1].ValueInt;
					float fDelay1 = action.Parameters[2].ValueFloat;
					
					StringBuilder s1 = new StringBuilder();
					if (fDelay1 > 0.0f) {
						s1.Append(fDelay1.ToString() + " SECONDS LATER, ");
					}				
					if (sTagString == String.Empty) {
						s1.Append(ownerName + " IS REMOVED FROM THE AREA.");
					}
					else {
						if (iInstance == -1) { 
							s1.Append("EVERYTHING NAMED ");
						}
						s1.Append(GetOwnerIfBlank(sTagString) + " IS REMOVED FROM THE AREA.");
					}
					return s1.ToString();					
					
				case "ga_destroy_item":
					string destroyitemtag = action.Parameters[0].ValueString;
					int destroyiteminstances = action.Parameters[1].ValueInt;
					if (destroyiteminstances == -1) {
						return "EVERY ITEM CALLED '" + destroyitemtag + "' IS REMOVED FROM INVENTORY.";
					}
					else if (destroyiteminstances == 1) {
						return destroyitemtag + " IS REMOVED FROM INVENTORY.";
					}
					else {
						return destroyiteminstances.ToString() + " ITEMS CALLED '" + destroyitemtag + "' ARE REMOVED FROM INVENTORY.";
					}
					
				case "ga_destroy_party_henchmen":
					return "ALL THE PLAYER'S HENCHMEN ARE REMOVED FROM THE AREA.";
					
				case "ga_door_close":
					string sTag = action.Parameters[0].ValueString;
					return "DOOR '" + sTag + "' IS CLOSED.";
										
				case "ga_door_open":
					string sTag1 = action.Parameters[0].ValueString;
					return "DOOR '" + sTag1 + "' IS OPENED.";
					
				case "ga_lock":
					string locktag = action.Parameters[0].ValueString;
					int lockstatus = action.Parameters[1].ValueInt;
					if (lockstatus == 1) {
						return "DOOR '" + locktag + "' IS LOCKED.";
					}
					else {
						return "DOOR '" + locktag + "' IS UNLOCKED.";
					}
					
				case "ga_give_gold":
					int nGP = action.Parameters[0].ValueInt;
					return "PLAYER RECEIVES " + nGP + " PIECES OF GOLD.";					
					
				case "ga_give_item":
					string sTemplate1 = action.Parameters[0].ValueString;
					int nQuantity = action.Parameters[1].ValueInt;
					if (nQuantity != 1) {
						return "PLAYER RECEIVES " + nQuantity + " COPIES OF '" + sTemplate1 + ".";
					}
					else {
						return "PLAYER RECEIVES A " + sTemplate1 + ".";
					}
					
				case "ga_henchman_add": 
					string sTarget1 = action.Parameters[0].ValueString;
					string sMaster = action.Parameters[2].ValueString;
					return GetOwnerIfBlank(sTarget1) + " BECOMES A HENCHMAN OF " + GetPlayerIfBlank(sMaster) + ".";
						
				case "ga_henchman_remove":
					string henchman = action.Parameters[0].ValueString;
					return henchman.ToUpper() + " IS REMOVED AS A HENCHMAN.";
					
				case "ga_jump":
					string jumpdestination = action.Parameters[0].ValueString;
					string thingthatjumps = action.Parameters[1].ValueString;
					float timetowait = action.Parameters[2].ValueFloat;					
					if (timetowait != 0.0) {
						return timetowait.ToString() + " SECONDS LATER, '" + 
							thingthatjumps.ToUpper() + "' JUMPS TO '" + jumpdestination.ToUpper() + "'.";
					}
					else {
						return "'" + thingthatjumps.ToUpper() + "' JUMPS TO '" + jumpdestination.ToUpper() + "'.";
					}
					
				case "ga_jump_players":
					string locationtojumpto = action.Parameters[0].ValueString;
					return "PLAYER JUMPS TO LOCATION '" + locationtojumpto + "'.";
					
				case "ga_take_gold":
					int goldtotake = action.Parameters[0].ValueInt;
					return "PLAYER LOSES " + goldtotake + " PIECES OF GOLD.";
					
				case "ga_take_item":
					string tagofitem = action.Parameters[0].ValueString;
					int numberofitems = action.Parameters[0].ValueInt;
					if (numberofitems == 1) {
						return "PLAYER LOSES '" + tagofitem + "'.";
					}
					else if (numberofitems == -1) {
						return "PLAYER LOSES ALL ITEMS WITH TAG '" + tagofitem + "'.";
					}
					else {
						return "PLAYER LOSES " + numberofitems + " ITEMS WITH TAG '" + tagofitem + "'.";
					}
					
				default:
					return "No description available (" + action.Script.ResRef.Value + ")";
			}
		}
		
		public static string GetDescription(NWN2ConditionalFunctorCollection conditions)
		{
			return "IF A CERTAIN CONDITION IS FULFILLED";
		}
		
		private static string GetOwnerIfBlank(string tag)
		{
			if (tag == String.Empty) {
				return ownerName;
			}
			else {
				return tag.ToUpper();
			}
		}
		
		private static string GetPlayerIfBlank(string tag)
		{
			if (tag == String.Empty) {
				return "PLAYER";
			}
			else {
				return tag.ToUpper();
			}
		}
	}
}


