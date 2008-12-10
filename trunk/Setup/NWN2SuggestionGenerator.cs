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
using AdventureAuthor.Utils;

namespace AdventureAuthor.Setup
{
	/// <summary>
	/// Provides a number of pre-defined messages to the user relating to 
	/// interesting Neverwinter Nights 2 blueprints, resources and general
	/// features that they could use in their games. 
	/// </summary>
	public class NWN2SuggestionGenerator : IMessageGenerator
	{
		#region Properties and fields
		
		/// <summary>
		/// Pre-defined suggestions to give the user.
		/// </summary>
		private List<string> suggestions = new List<string>();
		public List<string> Suggestions {
			get { return suggestions; }
		}
		
		
		private object padlock = new object();
		
		#endregion
		
		#region Constructors
		
		public NWN2SuggestionGenerator()
		{
			Populate();
		}
		
		#endregion
		
		#region Methods
		
		/// <summary>
		/// Populate the suggestions list.
		/// </summary>
		protected virtual void Populate()
		{
			suggestions.Add("Stuck for ideas? Why not add a creepy graveyard area to your game? In the blueprints " +
			                "menus you can find skeletons, vampires, graves, crypts and other spooky stuff.");
			suggestions.Add("Want your player to be surprised when an enemy attacks? Add an Encounter to your " +
			                "area and the enemies will appear out of nowhere!");
			suggestions.Add("There's a wide selection of animal blueprints you can choose from, including badgers, " +
			                "deer, rabbits, cats and bears.");
		}
		
		
		/// <summary>
		/// Get a message to display to the user.
		/// </summary>
		/// <returns>The message to be displayed</returns>
		public string GetMessage()
		{
			Random random = new Random();
			lock (padlock) {
				if (suggestions.Count == 0) {
					return String.Empty;
				}
				int index = random.Next(0,suggestions.Count);				
				return suggestions[index];
			}
		}
		
		#endregion
	}
}
