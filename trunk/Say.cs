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
using System.Windows.Forms;
using AdventureAuthor.AdventureData;

namespace AdventureAuthor
{
	public static class Say
	{		
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
			
			// TODO: Create a pretty form with readable text (base text size on user age settings?) instead of a message box.
			// TODO: Log a message about this error.
		}
		
		public static void Hint(string message)
		{
			MessageBox.Show("HINT: \n\n " + message);
			
			// TODO: Create a pretty form with readable text (base text size on user age settings?) instead of a message box.
		}
		
		public static void Information(string message)
		{
			if (!Adventure.BeQuiet) {
				MessageBox.Show(message);
			}
			
			// TODO: Create a pretty form with readable text (base text size on user age settings?) instead of a message box.
		}
		
		public static bool Question(string message, MessageBoxButtons buttons)
		{
			// TODO: Fix or delete (doesn't take account of the difference between No and Cancel)
			if (buttons == MessageBoxButtons.AbortRetryIgnore || buttons == MessageBoxButtons.RetryCancel) {
				throw new NotImplementedException();
			}
			
			if (!Adventure.BeQuiet) {
				DialogResult result = MessageBox.Show(message,String.Empty,buttons);
				if (result == DialogResult.Yes || result == DialogResult.OK) {
					return true;
				}
				else {
					return false;
				}
			}
			else {
				return true;
			}
			// TODO: Create a pretty form with readable text (base text size on user age settings?) instead of a message box.
			// TODO: Log a message about this error.
		}
		
		public static void Warning(string message)
		{
			if (!Adventure.BeQuiet) {
				MessageBox.Show("WARNING: \n\n " + message);
			}
			
			// TODO: Create a pretty form with readable text (base text size on user age settings?) instead of a message box.
			// TODO: Log a message about this warning.
		}
	}
}
