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

namespace AdventureAuthor.Conversations
{
	/// <summary>
	/// Arguments to accompany the event of some change being made to a conversation.
	/// </summary>
	public class ConversationChangedEventArgs : EventArgs
	{
		/// <summary>
		/// Whether or not the change will require the conversation graph to be redrawn.
		/// </summary>
		private bool graphOutOfDate;		
		public bool GraphOutOfDate {
			get { return graphOutOfDate; }
		}
		
		
		/// <summary>
		/// Create a new ConversationChangedEventArgs.
		/// </summary>
		/// <param name="graphOutOfDate">True if the change to the conversation will require the graph to be redrawn, either
		/// due to a structural change or due to node labels being outdated; false otherwise</param>
		/// <remarks>Note that the page view will always be out of date after any change,
		/// but the graph view will only be out of date after specific changes.</remarks>
		public ConversationChangedEventArgs(bool graphOutOfDate)
		{
			this.graphOutOfDate = graphOutOfDate;
		}
	}
}
