/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 07/02/2008
 * Time: 20:33
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace AdventureAuthor.Evaluation.Viewer
{
	/// <summary>
	/// Description of ReplyAddedEventArgs.
	/// </summary>
	public class ReplyAddedEventArgs : EventArgs
	{
		private Reply reply;		
		public Reply Reply {
			get { return reply; }
		}
		
		
		public ReplyAddedEventArgs(Reply reply)
		{
			this.reply = reply;
		}
	}
}
