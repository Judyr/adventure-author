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
	public static class UserLog
	{	
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
		
		public enum Action {
			Added,
			Deleted,
			Edited,
			Opened,
			Renamed
		}
		
		public enum Subject {
			Adventure,
			Chapter,
			Character,
			MinorCharacter,
			Creature,
			CharacterBlueprint,
			MinorCharacterBlueprint,
			CreatureBlueprint,
			Hero,
			Conversation,
			Script,
			Variable,
			Entrance,
			Exit,
			Transition,
			Item,
			ItemBlueprint,
			Idea
		}
		
		public enum WizardAction {
			Started,
			Completed,
			Cancelled,
			Deferred
		}
		
		public enum WizardSubject {
			CreateChapterWizard,
			EditChapterWizard,
			CreateCharacterWizard,
			EditCharacterWizard,
			CreateCreatureWizard,
			EditCreatureWizard,
			CreateHeroWizard,
			EditHeroWizard,
			CreateConversationWizard,
			EditConversationWizard
		}
		
		public enum SpecialAction {
			RanFromStart,
			RanFromChapter,
			AcceptedAdvice,
			RejectedAdvice,
			AttachedIdea,
			DetachedIdea,
			DonatedIdea,
			RequestedIdea,
			CategorisedIdea,
			OpenedAdventure
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
	
		/// <summary>
		/// Writes a log message in the form: '16:24:15:Modified_Character'
		/// </summary>
		/// <param name="action">The action taken by the user</param>
		/// <param name="subject">The subject the action was performed upon</param>
		public static void Write(Action action, Subject subject)
		{
			string message = UsefulTools.GetTimeStamp(false) + action.ToString() + "_" + subject.ToString();
			writer.WriteLine(message);
			writer.Flush();
		}
				
		/// <summary>
		/// Writes a log message in the form: '16:24:23:Started_CreateCreatureWizard'
		/// </summary>
		/// <param name="action">The action taken by the user</param>
		/// <param name="subject">The subject the action was performed upon</param>
		public static void Write(WizardAction action, WizardSubject subject)
		{		
			string message = UsefulTools.GetTimeStamp(false) + action.ToString() + "_" + subject.ToString();
			writer.WriteLine(message);
			writer.Flush();
		}
		
		/// <summary>
		/// Writes a log message in the form: '16:24:47:RanFromChapter'
		/// </summary>
		/// <param name="action">The action taken by the user</param>
		public static void Write(SpecialAction action)
		{			
			writer.WriteLine(UsefulTools.GetTimeStamp(false) + action.ToString());
			writer.Flush();
		}
		
		/// <summary>
		/// Writes a log message in the form: '16:24:47:(message)'
		/// </summary>
		/// <param name="message">The message to log</param>
		public static void Write(string message)
		{
			writer.WriteLine(UsefulTools.GetTimeStamp(false) + message);
			writer.Flush();
		}
	}
}
