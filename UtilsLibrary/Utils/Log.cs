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
using System.IO;
using System.Text;
using System.Diagnostics;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Utils
{
	public static class Log
	{	
		public static event EventHandler<LogEventArgs> Message;		
		private static void OnMessage(LogEventArgs e)
		{
			EventHandler<LogEventArgs> handler = Message;
			if (handler != null) {
				handler(null,e);
			}
		}
		
		
		/// <summary>
		/// Writes a log message in the form: '16:24:15:message'
		/// </summary>
		/// <remarks>'>' character indicates this message does not follow the usual format</remarks>
		/// <param name="logMessage">The message to log. For unique user actions.</param>
		/// <param name="includeTimeStamp">True to add a time stamp to the start of this message, false otherwise.</param>
		public static void WriteMessage(string logMessage, bool includeTimeStamp)
		{
			if (includeTimeStamp) {
				logMessage = Tools.GetTimeStamp(false) + " >" + logMessage;
			}			
			OnMessage(new LogEventArgs(logMessage));
		}			
		
		
		/// <summary>
		/// Writes a log message in the form: '16:24:15:message'
		/// </summary>
		/// <remarks>'>' character indicates this message does not follow the usual format</remarks>
		/// <param name="logMessage">The message to log. For unique user actions.</param>
		public static void WriteMessage(string logMessage)
		{
			WriteMessage(logMessage,true);
		}				
		

		/// <summary>
		/// Writes a log message in the form: '16:24:15:Added Choice'
		/// </summary>
		/// <remarks>Timestamp:EffectiveAction subject -optionalextrainfo</remarks>
		/// <param name="action">The user's effective action, e.g. Opened, Added, Edited, Deleted</param>
		/// <param name="subject">The subject of the effective action, e.g. Line, BranchLine, Choice, Speaker, Sound</param>
		public static void WriteAction(LogAction action, string subject)
		{
			WriteAction(action,subject,null);
		}	
		
		
		/// <summary>
		/// Writes a log message in the form: '16:24:15:Added Choice -PLAYER'
		/// </summary>
		/// <remarks>Timestamp:EffectiveAction subject -optionalextrainfo</remarks>
		/// <param name="action">The user's effective action, e.g. Opened, Added, Edited, Deleted</param>
		/// <param name="subject">The subject of the effective action, e.g. Line, BranchLine, Choice, Speaker, Sound</param>
		/// <param name="extraInfo">A string containing any extra applicable information in a non-standard format</param>
		public static void WriteAction(LogAction action, string subject, string extraInfo)
		{
			WriteAction(action,subject,extraInfo,true);
		}
		
		
		/// <summary>
		/// Writes a log message in the form: '16:24:15:Added Choice -PLAYER'
		/// </summary>
		/// <remarks>Timestamp:EffectiveAction subject -optionalextrainfo</remarks>
		/// <param name="action">The user's effective action, e.g. Opened, Added, Edited, Deleted</param>
		/// <param name="subject">The subject of the effective action, e.g. Line, BranchLine, Choice, Speaker, Sound</param>
		/// <param name="extraInfo">A string containing any extra applicable information in a non-standard format</param>
		/// <param name="includeTimeStamp">True to add a time stamp to the start of this message, false otherwise.</param>
		public static void WriteAction(LogAction action, string subject, string extraInfo, bool includeTimeStamp)
		{
			string message;
			string subjectmsg = subject == null ? "<Subject not logged>" : subject;	
			
			if (extraInfo != null && extraInfo != String.Empty) {
				message = action.ToString() + " " + subjectmsg + " -" + extraInfo;
			}
			else {
				message = action.ToString() + " " + subjectmsg;
			}
			if (includeTimeStamp) {
				message = Tools.GetTimeStamp(false) + " " + message;
			}
				
			OnMessage(new LogEventArgs(message));
		}
	}
}
