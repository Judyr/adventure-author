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
using System.IO;
using System.Text;
using System.Diagnostics;
using AdventureAuthor.Core;

namespace AdventureAuthor.Utils
{
	public static class Log
	{	
		// TODO out of date:
		// How to read log messages:
			// GetNumber() // hour
			// GetChar(":")
			// GetNumber() // minute
			// GetChar(":")			
			// GetNumber() // second
			// GetChar(":")
			// GetString() //action
			// if (GetChar("_")) {
			//    GetString() // subject (might not always be present)
			// }
		
		private static StreamWriter writer = null;
				
		public enum UIAction {
			Clicked,
			DoubleClicked,
			RightClicked,
			PressedKey
		}
		
		public enum WizardAction {
			Started,
			Completed,
			Cancelled
			// Deferred?
		}
		
		public enum EffectiveAction {
			Launched,
			Opened,
			Closed,
			Added,
			Edited,
			Renamed,
			Deleted,
			Selected,
			Viewed
		}
		
		
		public static void StartRecording()
		{			
			// Filename:
			// Date_Username.log
			// e.g. 05_05_07_JackStuart.log
			// e.g. 05_05_07_JackStuart2.log
			// Place in AdventureAuthor/logs
			
			string filename = UsefulTools.GetDateStamp() + "_" + Adventure.CurrentUser.Name;
			string logpath = Path.Combine(Adventure.LogDir,filename+".log");
			
			// If the filename is already taken, add numbering ("<filename>2.log", "<filename>3.log" etc.):
			int count = 1;
			while (File.Exists(logpath)) {
				count++;
				string newfilename = filename + count.ToString();
				logpath = Path.Combine(Adventure.LogDir,newfilename+".log");									
			}			
			FileInfo f = new FileInfo(logpath);			
			Stream s = f.Open(FileMode.Create);
			writer = new StreamWriter(s);
			writer.AutoFlush = true;
		}
		
		public static void StopRecording()
		{
			if (writer != null) {
				writer.Flush();
				writer.Close();
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
			string message = UsefulTools.GetTimeStamp(false) + ": >" + logMessage;
			writer.WriteLine(message);
			writer.Flush();
		}			
		

		/// <summary>
		/// Writes a log message in the form: '16:24:15: Clicked AddSpeaker_button'
		/// </summary>
		/// <remarks>Timestamp:UIAction ElementName_elementtype -optionalextrainfo</remarks>
		/// <remarks>Completed indicates the user clicked OK to complete a wizard; 
		/// Cancelled indicates they clicked Cancel to cancel a wizard.</remarks>
		/// <param name="interaction">The interaction with the user interface, e.g. Clicked, PressedKey, Completed, Cancelled</param>
		/// <param name="element">The UI element the interaction involved, in the form Name_SpecificElement.
		/// e.g. AddChoice_button, AddSpeaker_button, AddSpeaker_menuitem</param>
		public static void WriteUIAction(UIAction interaction, string element)
		{
			WriteUIAction(interaction,element,null);
		}		
		
		
		/// <summary>
		/// Writes a log message in the form: '16:24:15: Clicked AddSpeaker_button -WOLF'
		/// </summary>
		/// <remarks>Timestamp:UIAction ElementName_elementtype -optionalextrainfo</remarks>
		/// <param name="interaction">The interaction with the user interface, e.g. Clicked, PressedKey, RightClicked, DoubleClicked</param>
		/// <param name="element">The UI element the interaction involved, in the form Name_SpecificElement.
		/// e.g. AddChoice_button, AddSpeaker_button, AddSpeaker_menuitem</param>
		/// <param name="extraInfo">A string containing any extra applicable information in a non-standard format</param>
		public static void WriteUIAction(UIAction interaction, string element, string extraInfo)
		{
			string message;
			string subjectmsg = element == null ? "<Subject not logged>" : element;			
			
			if (extraInfo != null) {
				message = UsefulTools.GetTimeStamp(false) + ": " + interaction.ToString() + " " + subjectmsg + " -" + extraInfo;
			}
			else {
				message = UsefulTools.GetTimeStamp(false) + ": " + interaction.ToString() + " " + subjectmsg;
			}
				
			writer.WriteLine(message);
			writer.Flush();
		}
				
		
		/// <summary>
		/// Writes a log message in the form: '16:24:15:StartedWizard NewCharacterWizard'
		/// </summary>
		/// <remarks>Timestamp:WizardAction wizard -optionalextrainfo</remarks>
		/// <param name="action">The user's action involving a wizard, i.e. Starting, Completing or Cancelling it</param>
		/// <param name="wizard">The wizard in question, e.g. NewConversationWizard, AddSpeakerWizard</param>
		public static void WriteWizardAction(WizardAction action, string wizard)
		{
			WriteWizardAction(action,wizard,null);
		}		
		
		
		/// <summary>
		/// Writes a log message in the form: '16:24:15:StartedWizard NewCharacterWizard -8th character created'
		/// </summary>
		/// <remarks>Timestamp:WizardAction wizard -optionalextrainfo</remarks>
		/// <param name="action">The user's action involving a wizard, i.e. Starting, Completing or Cancelling it</param>
		/// <param name="wizard">The wizard in question, e.g. NewConversationWizard, AddSpeakerWizard</param>
		/// <param name="extraInfo">A string containing any extra applicable information in a non-standard format</param>
		public static void WriteWizardAction(WizardAction action, string wizard, string extraInfo)
		{
			string message;
			string subjectmsg = wizard == null ? "<Subject not logged>" : wizard;			
			
			if (extraInfo != null) {
				message = UsefulTools.GetTimeStamp(false) + ": " + action.ToString() + " " + subjectmsg + " -" + extraInfo;
			}
			else {
				message = UsefulTools.GetTimeStamp(false) + ": " + action.ToString() + " " + subjectmsg;
			}
				
			writer.WriteLine(message);
			writer.Flush();
		}
		
		

		/// <summary>
		/// Writes a log message in the form: '16:24:15:Added Choice'
		/// </summary>
		/// <remarks>Timestamp:EffectiveAction subject -optionalextrainfo</remarks>
		/// <param name="action">The user's effective action, e.g. Opened, Added, Edited, Deleted</param>
		/// <param name="subject">The subject of the effective action, e.g. Line, BranchLine, Choice, Speaker, Sound</param>
		public static void WriteEffectiveAction(EffectiveAction action, string subject)
		{
			WriteEffectiveAction(action,subject,null);
		}		
		
		
		/// <summary>
		/// Writes a log message in the form: '16:24:15:Added Choice -PLAYER'
		/// </summary>
		/// <remarks>Timestamp:EffectiveAction subject -optionalextrainfo</remarks>
		/// <param name="action">The user's effective action, e.g. Opened, Added, Edited, Deleted</param>
		/// <param name="subject">The subject of the effective action, e.g. Line, BranchLine, Choice, Speaker, Sound</param>
		/// <param name="extraInfo">A string containing any extra applicable information in a non-standard format</param>
		public static void WriteEffectiveAction(EffectiveAction action, string subject, string extraInfo)
		{
			string message;
			string subjectmsg = subject == null ? "<Subject not logged>" : subject;			
			
			if (extraInfo != null) {
				message = UsefulTools.GetTimeStamp(false) + ": " + action.ToString() + " " + subjectmsg + " -" + extraInfo;
			}
			else {
				message = UsefulTools.GetTimeStamp(false) + ": " + action.ToString() + " " + subjectmsg;
			}
				
			writer.WriteLine(message);
			writer.Flush();
		}
	}
}
