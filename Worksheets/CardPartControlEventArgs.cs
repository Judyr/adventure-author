/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 11/02/2008
 * Time: 14:28
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace AdventureAuthor.Evaluation
{
	/// <summary>
	/// Description of CardPartControlEventArgs.
	/// </summary>
	public class CardPartControlEventArgs : EventArgs
	{
		protected CardPartControl control;		
		public CardPartControl Control {
			get { return control; }
		}
		
		
		/// <summary>
		/// The owner of the control raising this event.
		/// </summary>
		protected CardPartControl parent;		
		public CardPartControl Parent {
			get { return parent; }
		}
		
		
		public CardPartControlEventArgs(CardPartControl control)
		{
			this.control = control;
		}
	}
}
