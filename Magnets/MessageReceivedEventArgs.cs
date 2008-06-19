/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 13/06/2008
 * Time: 10:09
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace AdventureAuthor.Ideas
{
	/// <summary>
	/// Description of MessageReceivedEventArgs.
	/// </summary>
	public class MessageReceivedEventArgs : EventArgs
	{
		private string message;
		public string Message {
			get { return message; }
		}
		
		
		public MessageReceivedEventArgs(string message)
		{
			this.message = message;
		}
	}
}
