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
using NWN2Toolset.NWN2.Data;

namespace AdventureAuthor.Scripts
{
	/// <summary>
	/// Description of Conditions
	/// </summary>
	public static class Conditions
	{	
		/// <summary>
		/// Check whether the player has an item with a given tag
		/// </summary>
		/// <param name="sItemTag">Tag of the item to check for</param>
		/// <returns></returns>
		public static NWN2ScriptFunctor PlayerHasItem(string sItemTag)
		{
			string sCheck = "!0"; // check that the player has at least one item with this tag
			int bPCOnly = 0; // not useful		
			return ScriptLibrary.GetScriptFunctor("gc_item_count",new object[]{sItemTag,sCheck,bPCOnly});
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
		/// <returns></returns>
		public static NWN2ScriptFunctor PlayerHasNumberOfItems(string sItemTag, string sCheck)
		{
			int bPCOnly = 0; // not useful		
			return ScriptLibrary.GetScriptFunctor("gc_item_count",new object[]{sItemTag,sCheck,bPCOnly});
		}			
	}
}
