﻿/*
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
using System.Windows.Media;

namespace AdventureAuthor.Conversations
{
	/// <summary>
	/// A speaker in a conversation.
	/// </summary>
	public class Speaker
	{		
		/// <summary>
		/// The tag this speaker receives as a game object.
		/// </summary>
		private string tag;		
		public string Tag {
			get { return tag; }
			set { 
				if (tag == String.Empty) {
					throw new InvalidOperationException("Cannot give a speaker an empty tag, " +
					                                    "as this would clash with the definition " + 
					                                    "of the Player speaker.");
				}
				tag = value; 
			}
		}
		
		
		/// <summary>
		/// The name of this speaker.
		/// </summary>
		/// <remarks>Cannot be set - either returns the tag name, or the name of
		/// the player if there is a blank tag.</remarks>
		public string Name {
			get { 
				if (tag == String.Empty) {
					return Conversation.PLAYER_NAME;
				}
				else {
					return tag;
				}
			}
		}
		
		
		/// <summary>
		/// The colour that will represent this speaker on interface controls.
		/// </summary>
		private Brush colour;				
		public Brush Colour {
			get { return colour; }
			set { colour = value; }
		}
				
		
		public Speaker(string tag)
		{
			this.tag = tag;
			this.colour = Brushes.Black;
		}
		
		
		public override string ToString()
		{
			return Name;
		}
		
		
		/// <summary>
		/// Check whether this speaker represents the player.
		/// </summary>
		/// <returns>True if this speaker represents the player; false if it represents some NPC</returns>
		public bool IsPlayer()
		{
			return tag == String.Empty;
		}
	}
}
