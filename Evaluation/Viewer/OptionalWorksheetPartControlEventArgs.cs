/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 11/02/2008
 * Time: 14:28
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace AdventureAuthor.Evaluation.Viewer
{
	/// <summary>
	/// Description of OptionalWorksheetPartControlEventArgs.
	/// </summary>
	public class OptionalWorksheetPartControlEventArgs : EventArgs
	{
		protected OptionalWorksheetPartControl control;		
		public OptionalWorksheetPartControl Control {
			get { return control; }
		}
		
		
		/// <summary>
		/// The owner of the control to be moved.
		/// </summary>
		protected OptionalWorksheetPartControl parent;		
		public OptionalWorksheetPartControl Parent {
			get { return parent; }
		}
		
		
		public OptionalWorksheetPartControlEventArgs(OptionalWorksheetPartControl control)
		{
			this.control = control;
		}
	}
}
