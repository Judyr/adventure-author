/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 14/03/2008
 * Time: 12:43
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace AdventureAuthor.Utils
{
	/// <summary>
	/// Description of TextEditedEventArgs.
	/// </summary>
	public class TextEditedEventArgs : EventArgs
	{
		private string oldValue;
		public string OldValue {
			get { return oldValue; }
		}
		
		
		private string newValue;
		public string NewValue {
			get { return newValue; }
		}
		
		
		public TextEditedEventArgs(string oldValue, string newValue)
		{
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
	}
}
