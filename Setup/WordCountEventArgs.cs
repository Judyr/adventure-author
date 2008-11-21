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

namespace AdventureAuthor.Setup
{
	/// <summary>
	/// Arguments to accompany the event of the user typing a word or words in
	/// a narrative or dialogue context.
	/// </summary>
	public class WordCountEventArgs : EventArgs
	{
		/// <summary>
		/// The number of words the user typed.
		/// </summary>
		private int words;
		public int Words {
			get { return words; }
		}
		
		
		/// <summary>
		/// Create a new WordCountEventArgs.
		/// </summary>
		/// <param name="words"> The number of words the user typed.</param>
		public WordCountEventArgs(int words)
		{
			this.words = words;
		}
	}
}
