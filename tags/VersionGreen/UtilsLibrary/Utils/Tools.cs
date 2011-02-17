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
using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;

namespace AdventureAuthor.Utils
{
	/// <summary>
	/// Useful functions for Adventure Author.
	/// </summary>
	public static class Tools
	{		
		public const double MINIMUMWINDOWWIDTH = 640;
		public const double MINIMUMWINDOWHEIGHT = 480;
			
		
		/// <summary>
		/// Bring a window to the front and make sure it is not minimised.
		/// </summary>
		/// <param name="window">The window to bring to the front.</param>
		public static void BringToFront(Window window)
		{
			if (window.WindowState == WindowState.Minimized) {
				window.WindowState = WindowState.Normal;
			}
			window.Activate(); // bring to front
		}
		
		
		/// <summary>
		/// Strip the new line character from the end of a string if it appears.
		/// </summary>
		/// <param name="str">The string to strip the new line from.</param>
		/// <returns>A version of the string without a new line character at the end.</returns>
		public static string StripNewLineFromEnd(string str)
		{
			if (str.EndsWith(System.Environment.NewLine)) {
				str = str.Substring(0,str.Length-Environment.NewLine.Length);
			}
			return str;
		}
		
		  
		/// <summary>
		/// Check whether a character can form part of an English language word.
		/// </summary>
		/// <param name="c">The character to check</param>
		/// <returns>True if this character can form part of a word, false otherwise.</returns>
        public static bool IsLetter(char c)
        {
        	return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c == '\'');
        }  
		
		
		public static string SplitCamelCase(string input)
		{
			return System.Text.RegularExpressions.Regex.Replace(input,
															    "([A-Z])",
															    " $1",
															    System.Text.RegularExpressions.RegexOptions.Compiled).Trim();
		}
		
		
		public static void DisplayAboutWindow(string message)
		{
			Say.Information(message);
		}		
		
		
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
					if (lastSpace > 0 && IsPunctuation(text[lastSpace-1])) { // don't end on a punctuation mark if possible
						return text.Substring(0,lastSpace-1);
					}
					else {
						return text.Substring(0,lastSpace);
					}
				}
				else {
					return text.Substring(0,maxLength);
				}
			}
			else {
				return text;
			}
		}
		
		
		/// <summary>
		/// Check whether a character is a punctuation mark.
		/// </summary>
		/// <param name="text">The character to check</param>
		/// <returns>True if the character is one of .,:;()!?&-, false otherwise</returns>
		private static bool IsPunctuation(char character)
		{
			char[] punctuation = new char[] {'.',',',':',';','(',')','!','?','&','-'};
			foreach (char c in punctuation) {
				if (character == c) {
					return true;
				}
			}
			return false;
		}
		
		
		/// <summary>
		/// Get a string representing the current date.
		/// </summary>
		/// <returns>A string representing the date in days, months and years</returns>
		public static string GetDateStamp(bool forNWN2ModuleFilename)
		{
			DateTime now = DateTime.Now;
			string divider;
			if (forNWN2ModuleFilename) {
				divider = "-";
			}
			else {
				divider = ".";
			}
			return now.Day + divider + now.Month + divider + now.Year;
		}
		
		
		/// <summary>
		/// Get a string representing the current time.
		/// </summary>
		/// <param name="filenameFriendly">True if the string should only contain valid characters for filenames, false otherwise</param>
		/// <returns>A string representing the current time in hours, minutes and seconds</returns>
		public static string GetTimeStamp(bool filenameFriendly)
		{
			if (!filenameFriendly) {
				return GetNWN2StyleTimeStamp();
			}
			
			DateTime d = DateTime.Now;
			StringBuilder timestamp = new StringBuilder();
			string divider;
			if (filenameFriendly) {
				divider = "";
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
//			string second = d.Second.ToString();
//			if (second.Length == 1) {
//				timestamp.Append("0" + second + divider);
//			}
//			else {
//				timestamp.Append(second + divider);
//			}
				
			return timestamp.ToString();
		}
		
		
		public static string GetNWN2StyleTimeStamp()
		{
			// [Thu Nov 08 13:51:20]
			DateTime d = DateTime.Now;			
			return "[" + d.DayOfWeek.ToString() + " " + GetMonth(d.Month) + " " + Pad(d.Day.ToString(),2) + " " + 
				Pad(d.Hour.ToString(),2) + ":" + Pad(d.Minute.ToString(),2) + ":" + Pad(d.Second.ToString(),2) + "]";
		}
		
		
		private static string Pad(string text, int desiredLength)
		{
			int diff = desiredLength - text.Length;
			for (int i = 0; i < diff; i++) {
				text = "0" + text;
			}
			return text;
		}
		
		
		private static string GetMonth(int month)
		{
			switch (month) {
				case 1:
					return "Jan";
				case 2: 
					return "Feb";
				case 3:
					return "Mar";
				case 4: 
					return "Apr";
				case 5:
					return "May";
				case 6: 
					return "Jun";
				case 7:
					return "Jul";
				case 8: 
					return "Aug";
				case 9:
					return "Sep";
				case 10: 
					return "Oct";
				case 11:
					return "Nov";
				case 12: 
					return "Dec";
				default:
					throw new ArgumentException("Not valid month number");
			}
		}
						
		
		/// <summary>
		/// Check that a given directory exists; if it doesn't, create it.
		/// </summary>
		/// <param name="directory">The directory to check for/create</param>
		public static void EnsureDirectoryExists(string directory)
		{
			if (!Directory.Exists(directory)) {
				Directory.CreateDirectory(directory);	
			}	
		}
        
        
        public static BitmapSource GetBitmapSource(Bitmap bitmap)
        {
        	if (bitmap == null) throw new ArgumentNullException("bitmap");
        	
        	return Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(),
		        	                                     IntPtr.Zero,
		        	                                     Int32Rect.Empty,
		        	                                     BitmapSizeOptions.FromEmptyOptions());
        }
	}
}
