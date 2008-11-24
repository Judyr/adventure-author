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
	/// Arguments to accompany the event of a message being sent to the
	/// toolset via a named pipe.
	/// </summary>
	public class MessageReceivedEventArgs : EventArgs
	{
		/// <summary>
		/// The message that was received.
		/// </summary>
		private string message;
		public string Message {
			get { return message; }
		}
		
		
		/// <summary>
		/// The name of the pipe this message was sent via.
		/// </summary>
		private string source;
		public string Source {
			get { return source; }
		}
		

		/// <summary>
		/// The time the message was received.
		/// </summary>
		private DateTime received;
		public DateTime Received {
			get { return received; }
		}
		
		
		/// <summary>
		/// Create a set of arguments to accompany the event of a
		/// message being sent to the toolset via a named pipe.
		/// </summary>
		/// <param name="message">The message that was received.</param>
		/// <param name="source">The name of the pipe this message was sent via.</param>
		/// <param name="received">The time the message was received.</param>
		public MessageReceivedEventArgs(string message, string source, DateTime received)
		{
			this.message = message;
			this.source = source;
			this.received = received;
		}
	}
}
