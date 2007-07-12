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
 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.IO;
using System.Diagnostics;
using AdventureAuthor.AdventureData;

namespace AdventureAuthor
{
	public static class Log
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
			
			DateTime now = DateTime.Now;
			
			string filename = GetDate() + "_" + Adventure.CurrentUser.Name;
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
			try	{
				string message = GetTime() + action.ToString() + "_" + subject.ToString();
				writer.WriteLine(message);
				writer.Flush();
			}
			catch (NullReferenceException e) {
				Debug.WriteLine("Log writer was not open.");
				Debug.WriteLine(e.ToString());
			}
		}
				
		/// <summary>
		/// Writes a log message in the form: '16:24:23:Started_CreateCreatureWizard'
		/// </summary>
		/// <param name="action">The action taken by the user</param>
		/// <param name="subject">The subject the action was performed upon</param>
		public static void Write(WizardAction action, WizardSubject subject)
		{			
			try	{
				string message = GetTime() + action.ToString() + "_" + subject.ToString();
				writer.WriteLine(message);
				writer.Flush();
			}
			catch (NullReferenceException e) {
				Debug.WriteLine("Log writer was not open.");
				Debug.WriteLine(e.ToString());
			}
		}
		
		/// <summary>
		/// Writes a log message in the form: '16:24:47:RanFromChapter'
		/// </summary>
		/// <param name="action">The action taken by the user</param>
		public static void Write(SpecialAction action)
		{			
			try	{
				string message = GetTime() + action.ToString();
				writer.WriteLine(message);
				writer.Flush();
			}
			catch (NullReferenceException e) {
				Debug.WriteLine("Log writer was not open.");
				Debug.WriteLine(e.ToString());
			}
		}
		
		public static String GetDate()
		{
			DateTime now = DateTime.Now;
			return now.Day + "_" + now.Month + "_" + now.Year;
		}
		
		public static String GetTime()
		{
			DateTime d = DateTime.Now;		
			string hour = d.Hour.ToString();
			string minute = d.Minute.ToString();
			string second = d.Second.ToString();
			if (hour.Length == 1) {
				hour = "0" + hour;
			}
			if (minute.Length == 1) {
				minute = "0" + minute;
			}
			if (second.Length == 1) {
				second = "0" + second;
			}			
			
			return hour + ":" + minute + ":" + second + ":";
		}
	}
}
