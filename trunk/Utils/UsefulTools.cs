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
using System.Text;

namespace AdventureAuthor.Utils
{
	/// <summary>
	/// Useful functions for Adventure Author.
	/// </summary>
	public static class UsefulTools
	{	
		// TODO make the string methods into Automatic Methods on string
		
		/// <summary>
        /// Returns true if a given word begins with a vowel (ignoring case).
        /// </summary>
        /// <param name="word">The word to check.</param>
        /// <returns>True if the word begins with a vowel, false otherwise.</returns>
        public static bool StartsWithVowel(string word)
        {
        	if (word == null || word.Length == 0) {
        		return false;
        	}
        	char c = word.ToLower()[0];
        	return c == 'a' || c == 'e' || c == 'i' || c == 'o' || c == 'u'; 
        }
		
        /// <summary>
        /// Truncates a line of text to within a given length, if possible stopping at the end of whole word.
        /// </summary>
        /// <param name="text">The text to truncate.</param>
        /// <param name="maxLength">The maximum length of the returned line.</param>
        /// <returns>The original line if its length is less than maxLength, otherwise a truncated version of the line - 
        /// to the end of a word if possible (in which case it will probably be shorter than maxLength), or to the
        /// maximum possible length of the line if not.</returns>
		public static string Truncate(string text, int maxLength)
		{
			if (text.Length > maxLength) {
				int lastSpace = text.LastIndexOf(' ',maxLength-10);
				if (lastSpace != -1) {
					return text.Substring(0,lastSpace);
				}
				else {
					return text.Substring(0,maxLength);
				}
			}
			else {
				return text;
			}
		}
		
		public static string GetDateStamp()
		{
			DateTime now = DateTime.Now;
			return now.Day + "_" + now.Month + "_" + now.Year;
		}
		
		public static string GetTimeStamp(bool forFilename)
		{
			DateTime d = DateTime.Now;		
			StringBuilder timestamp = new StringBuilder();
			string divider;
			if (forFilename) {
				divider = "_";
			}
			else {
				divider = ":";
			}
			
			string hour = d.Hour.ToString();
			if (hour.Length == 1) {
				timestamp.Append("0" + hour + divider);
			}
			else {
				timestamp.Append(hour + divider);
			}			
			string minute = d.Minute.ToString();
			if (minute.Length == 1) {
				timestamp.Append("0" + minute + divider);
			}
			else {
				timestamp.Append(minute + divider);
			}						
			string second = d.Second.ToString();
			if (second.Length == 1) {
				timestamp.Append("0" + second + divider);
			}
			else {
				timestamp.Append(second + divider);
			}
				
			return timestamp.ToString();
		}
	}
}
