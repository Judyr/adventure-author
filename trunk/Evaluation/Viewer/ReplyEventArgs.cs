/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 07/02/2008
 * Time: 20:33
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace AdventureAuthor.Evaluation
{
	/// <summary>
	/// Description of ReplyAddedEventArgs.
	/// </summary>
	public class OptionalWorksheetPartEventArgs : EventArgs
	{
		private OptionalWorksheetPart part;		
		public OptionalWorksheetPart Part {
			get { return part; }
		}
				
		
		public OptionalWorksheetPartEventArgs(OptionalWorksheetPart part)
		{
			this.part = part;
		}
	}
}
