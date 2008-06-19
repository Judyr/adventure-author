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
		
		
		// TODO refactor all the redundant code below
		
		
		/// <summary>
		/// Writes a log message in the form: '16:24:15:message'
		/// </summary>
		/// <remarks>'>' character indicates this message does not follow the usual format</remarks>
		/// <param name="logMessage">The message to log. For unique user actions.</param>
		public static void WriteMessage(string logMessage)
		{
			string message = Tools.GetTimeStamp(false) + " >" + logMessage;
			OnMessage(new LogEventArgs(message));
		}			
		

//		
//		/// <summary>
//		/// Writes a log message in the form: '16:24:15:StartedWizard NewCharacterWizard'
//		/// </summary>
//		/// <remarks>Timestamp:WizardAction wizard -optionalextrainfo</remarks>
//		/// <param name="action">The user's action involving a wizard/dialog, i.e. Starting, Completing or Aborting it</param>
//		/// <param name="dialog">The wizard/dialog in question, e.g. NewConversationWizard, AddSpeakerWizard, DeleteActionDialog</param>
//		public static void WriteDialogAction(DialogAction action, string wizard)
//		{
//			WriteDialogAction(action,wizard,null);
//		}		
//		
//		
//		/// <summary>
//		/// Writes a log message in the form: '16:24:15:StartedWizard NewCharacterWizard -8th character created'
//		/// </summary>
//		/// <remarks>Timestamp:WizardAction wizard -optionalextrainfo</remarks>
//		/// <param name="action">The user's action involving a wizard/dialog, i.e. Starting, Completing or Aborting it</param>
//		/// <param name="dialog">The wizard/dialog in question, e.g. NewConversationWizard, AddSpeakerWizard, DeleteActionDialog</param>
//		/// <param name="extraInfo">A string containing any extra applicable information in a non-standard format</param>
//		public static void WriteDialogAction(DialogAction action, string dialog, string extraInfo)
//		{
//			string message;
//			string subjectmsg = dialog == null ? "<Subject not logged>" : dialog;			
//			
//			if (extraInfo != null) {
//				message = UsefulTools.GetTimeStamp(false) + ": " + action.ToString() + " " + subjectmsg + " -" + extraInfo;
//			}
//			else {
//				message = UsefulTools.GetTimeStamp(false) + ": " + action.ToString() + " " + subjectmsg;
//			}
//				
//			writer.WriteLine(message);
//			writer.Flush();
//		}
		
		

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
			string message;
			string subjectmsg = subject == null ? "<Subject not logged>" : subject;			
			
			if (extraInfo != null && extraInfo != String.Empty) {
				message = Tools.GetTimeStamp(false) + " " + action.ToString() + " " + subjectmsg + " -" + extraInfo;
			}
			else {
				message = Tools.GetTimeStamp(false) + " " + action.ToString() + " " + subjectmsg;
			}
				
			OnMessage(new LogEventArgs(message));
		}
	}
}