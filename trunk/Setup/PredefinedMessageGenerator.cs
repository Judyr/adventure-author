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
using AdventureAuthor.Utils;

namespace AdventureAuthor.Setup
{
	/// <summary>
	/// Reads messages from a text file and provides them
	/// to the user on request.
	/// </summary>
	public class PredefinedMessageGenerator : IMessageGenerator
	{
		#region Properties and fields
		
		/// <summary>
		/// Pre-defined suggestions to give the user.
		/// </summary>
		private List<string> suggestions = new List<string>();
		public List<string> Suggestions {
			get { return suggestions; }
		}
		
		
		/// <summary>
		/// Used for locking.
		/// </summary>
		private object padlock = new object();
		
		#endregion
		
		#region Constructors
		
		/// <summary>
		/// Reads messages from a text file and provides them
		/// to the user on request.
		/// </summary>
		/// <param name="path">The path of the plain text file to read from.</param>
		public PredefinedMessageGenerator(string path)
		{
			Populate(path);
		}
		
		#endregion
		
		#region Methods
		
		/// <summary>
		/// Populate the suggestions list from a text file.
		/// </summary>
		/// <param name="path">The path of the plain text file to draw suggestions from.</param>
		protected virtual void Populate(string path)
		{
			if (!File.Exists(path)) {
				throw new ArgumentException("path","No file exists at '" + path + "'.");
			}
			
			try {
				suggestions = new List<string>(File.ReadAllLines(path));
			}
			catch (Exception e) {
				Say.Error("Failed to get suggestions from file '" + path + "'.",e);
			}
		}
		
		
		/// <summary>
		/// Get a message to display to the user.
		/// </summary>
		/// <returns>The message to be displayed, or null if
		/// no message could be generated.</returns>
		public HyperlinkMessage GetMessage()
		{
			Random random = new Random();
			lock (padlock) {
				if (suggestions.Count == 0) {
					return null;
				}
				int index = random.Next(0,suggestions.Count);				
				string suggestion = suggestions[index];
				return new HyperlinkMessage(suggestion);
			}
		}
		
		#endregion
	}
}
