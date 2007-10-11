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
using System.Windows.Forms;
using AdventureAuthor.Core;

namespace AdventureAuthor.Utils
{
	// TODO: Create pretty forms with readable text (base text size on user age settings?) instead of message boxes.
	public static class Say
	{		
		public static void Debug(string message) {
			if (Adventure.Debug) {
				DebugLog.Write(message);
			}
		}
		
		public static void Error(string message) {
			Error(message,null);
		}
		
		public static void Error(string message, Exception e)
		{
			if (!Adventure.BeQuiet) {
				if (e != null) {
					message += "\n\n" + e.ToString();
				}
				
				MessageBox.Show("ERROR: \n\n " + message);
			}
						
			StringBuilder logMessage = new StringBuilder("- error: " + message);
			if (e != null) {
				logMessage.Append("   > " + e.ToString());
			}
			Debug(logMessage.ToString());
		}
		
		public static void Error(Exception e)
		{
			Error(String.Empty,e);
		}
		
		public static void Hint(string message)
		{
			if (!Adventure.BeQuiet) {
				MessageBox.Show("HINT: \n\n " + message);
			}
		}
		
		public static void Information(string message)
		{
			if (!Adventure.BeQuiet) {
				MessageBox.Show(message);
			}
		}
		
		public static bool? Question(string text, MessageBoxButtons buttons)
		{
			return Say.Question(text, String.Empty, buttons);
		}
		
		public static bool? Question(string message, string caption, MessageBoxButtons buttons)
		{
			if (buttons == MessageBoxButtons.AbortRetryIgnore || buttons == MessageBoxButtons.RetryCancel) {
				throw new NotImplementedException();
			}
			
			if (!Adventure.BeQuiet) {
				DialogResult result = MessageBox.Show(message,caption,buttons);
				if (result == DialogResult.Cancel) {
					return null;
				}
				else if (result == DialogResult.Yes || result == DialogResult.OK) {
					return true;
				}
				else {
					return false;
				}
			}
			else {
				return true;
			}
		}
		
		public static void Warning(string message)
		{
			if (!Adventure.BeQuiet) {
				MessageBox.Show("WARNING: \n\n " + message);
			}
			
			UserLog.Write("- warned: " + message);
		}
	}
}
