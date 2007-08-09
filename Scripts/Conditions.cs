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
using NWN2Toolset.NWN2.Data;

namespace AdventureAuthor.Scripts
{
	/// <summary>
	/// Description of Conditions
	/// </summary>
	public static class Conditions
	{	
		/// <summary>
		/// Check to see if a creature is dead
		/// </summary>
		/// <param name="sCreatureTag">The tag of the creature to check</param>
		public static NWN2ConditionalFunctor CreatureIsDead(string sCreatureTag)
		{
			return ScriptLibrary.GetConditionalFunctor("gc_dead",new object[]{sCreatureTag});
		}
		
		/// <summary>
		/// Check to see if a door or container is currently open
		/// </summary>
		/// <param name="sObjectTag">The tag of the object to check</param>
		public static NWN2ConditionalFunctor DoorOrContainerIsOpen(string sObjectTag)
		{
			return ScriptLibrary.GetConditionalFunctor("gc_is_open",new object[]{sObjectTag});
		}
		
		/// <summary>
		/// Check to see if a hostile character is within a certain range of the player
		/// </summary>
		/// <param name="fRadius">The range in metres to check</param>
		public static NWN2ConditionalFunctor EnemyIsNearPlayer(float fRange)
		{
			int bLineOfSight = 0; // not useful
			return ScriptLibrary.GetConditionalFunctor("gc_is_enemy_near",new object[]{fRange,bLineOfSight});
		}

		/// <summary>
		/// Checks the value of a global float
		/// </summary>
		/// <param name="sVariable">The variable to check</param>
		/// <param name="sCheck">The value to check for, can use operators e.g. <5.0, >10.0, !0.0</param>
		public static NWN2ConditionalFunctor GlobalFloatHasValue(string sVariable, string sCheck)
		{
			return ScriptLibrary.GetConditionalFunctor("gc_global_float",new object[]{sVariable,sCheck});
		}
		
		/// <summary>
		/// Checks the value of a global int
		/// </summary>
		/// <param name="sVariable">The variable to check</param>
		/// <param name="sCheck">The value to check for, can use operators e.g. <5, >10, !0</param>
		public static NWN2ConditionalFunctor GlobalIntHasValue(string sVariable, string sCheck)
		{
			return ScriptLibrary.GetConditionalFunctor("gc_global_int",new object[]{sVariable,sCheck});
		}
		
		/// <summary>
		/// Checks the value of a global string
		/// </summary>
		/// <param name="sVariable">The variable to check</param>
		/// <param name="sCheck">The value to check for</param>
		public static NWN2ConditionalFunctor GlobalStringHasValue(string sVariable, string sCheck)
		{
			return ScriptLibrary.GetConditionalFunctor("gc_global_string",new object[]{sVariable,sCheck});
		}
		
		/// <summary>
		/// Checks the value of a journal entry
		/// </summary>
		/// <param name="sQuestTag">The journal quest to check</param>
		/// <param name="sCheck">The value to check for, can use operators e.g. <5, >10, !0</param>
		public static NWN2ConditionalFunctor JournalEntryHasValue(string sQuestTag, string sCheck)
		{
			return ScriptLibrary.GetConditionalFunctor("gc_journal_entry",new object[]{sQuestTag,sCheck});
		}
			
		/// <summary>
		/// Check the distance between an object and the player
		/// </summary>
		/// <param name="sTag">The object to check</param>
		/// <param name="sCheck">The distance in metres to check for, can use operators e.g. <5.0, >10.0</param>
		public static NWN2ConditionalFunctor ObjectIsNearPlayer(string sTag, string sCheck)
		{
			return ScriptLibrary.GetConditionalFunctor("gc_distance_pc",new object[]{sTag,sCheck});
		}			
			
		/// <summary>
		/// Check the distance between two objects
		/// </summary>
		/// <param name="sTagA">The first object to check</param>
		/// <param name="sTagB">The second object to check</param>
		/// <param name="sCheck">The distance in metres to check for, can use operators e.g. <5.0, >10.0</param>
		public static NWN2ConditionalFunctor ObjectIsNearObject(string sTagA, string sTagB, string sCheck)
		{
			return ScriptLibrary.GetConditionalFunctor("gc_distance",new object[]{sTagA,sTagB,sCheck});
		}
		
		/// <summary>
		/// Check to see if the PC has more than a certain amount of gold
		/// </summary>
		/// <param name="nGold">The amount of gold you want to make sure the player has</param>
		public static NWN2ConditionalFunctor PlayerHasGold(int nGold)
		{
			int nMP = 0; // not useful
			return ScriptLibrary.GetConditionalFunctor("gc_check_gold",new object[]{nGold,nMP});
		}
		
		/// <summary>
		/// Check whether the player has an item with a given tag
		/// </summary>
		/// <param name="sItemTag">Tag of the item to check for</param>
		public static NWN2ConditionalFunctor PlayerHasItem(string sItem)
		{
			int bCheckParty	= 1; // not useful 
			return ScriptLibrary.GetConditionalFunctor("gc_check_item",new object[]{sItem,bCheckParty});
		}
			
		/// <summary>
		/// Check whether a creature is a henchman of the player
		/// </summary>
		/// <param name="sTarget">The tag of the creature to check</param>
		public static NWN2ConditionalFunctor PlayerHasHenchman(string sTarget)
		{
			string sMaster = String.Empty; // not useful
			return ScriptLibrary.GetConditionalFunctor("gc_henchman",new object[]{sTarget,sMaster});
		}
		
		/// <summary>
		/// Check whether the creature is a companion of the player (i.e. is in the player's party)
		/// </summary>
		/// <remarks>**Not sure whether this also checks for henchmen or not</remarks>
		/// <param name="sTarget">The tag of the creature to check</param>
		/// <returns></returns>
		public static NWN2ConditionalFunctor PlayerHasCompanion(string sTarget)
		{
			return ScriptLibrary.GetConditionalFunctor("gc_is_in_party",new object[]{sTarget});
		}
		
		/// <summary>
		/// Check whether the player has a certain number of roster companions (i.e. party members)
		/// </summary>
		/// <param name="sCheck">The value to check for, can use operators e.g. <5, >10, !0</param>
		public static NWN2ConditionalFunctor PlayerHasNumberOfRosterCompanions(string sCheck)
		{
			return ScriptLibrary.GetConditionalFunctor("gc_num_comps",new object[]{sCheck});
		}
		
		/// <summary>
		/// Check whether the player has a certain number of items with a given tag
		/// </summary>
		/// <description>gc_item_count</description>
		/// <param name="sItemTag">Tag of the item to count</param>
		/// <param name="sCheck">Number of items to check for, for example:
		/// "<5" Less than 5
		/// ">1" Greater than 1
		/// "=9" Equal to 9
		/// "!0" Not equal to 0
		/// </param>
		public static NWN2ConditionalFunctor PlayerHasNumberOfItems(string sItemTag, string sCheck)
		{
			int bPCOnly = 0; // not useful		
			return ScriptLibrary.GetConditionalFunctor("gc_item_count",new object[]{sItemTag,sCheck,bPCOnly});
		}						
		
		/// <summary>
		/// Check whether the player is chaotic
		/// </summary>
		public static NWN2ConditionalFunctor PlayerIsChaotic()
		{
			return ScriptLibrary.GetConditionalFunctor("gc_align_chaotic",null);
		}	
		
		/// <summary>
		/// Check whether the player is evil
		/// </summary>
		public static NWN2ConditionalFunctor PlayerIsEvil()
		{
			return ScriptLibrary.GetConditionalFunctor("gc_align_evil",null);
		}	
		
		/// <summary>
		/// Check whether the player is female
		/// </summary>
		public static NWN2ConditionalFunctor PlayerIsFemale()
		{
			return ScriptLibrary.GetConditionalFunctor("gc_is_female",null);
		}
		
		/// <summary>
		/// Check whether the player is good
		/// </summary>
		public static NWN2ConditionalFunctor PlayerIsGood()
		{
			return ScriptLibrary.GetConditionalFunctor("gc_align_good",null);
		}		
				
		/// <summary>
		/// Check whether the player is lawful
		/// </summary>
		public static NWN2ConditionalFunctor PlayerIsLawful()
		{
			return ScriptLibrary.GetConditionalFunctor("gc_align_lawful",null);
		}			
		
		/// <summary>
		/// Check whether the player is male
		/// </summary>
		public static NWN2ConditionalFunctor PlayerIsMale()
		{
			return ScriptLibrary.GetConditionalFunctor("gc_is_male",null);
		}	
	}
}
