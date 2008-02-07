/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 04/02/2008
 * Time: 11:01
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Windows.Controls;

namespace AdventureAuthor.Ideas
{
	/// <summary>
	/// Description of BoardObject.
	/// </summary>
	public abstract class BoardObject : UserControl
	{				
		public abstract double X {
			get; set;
		}
		
		public abstract double Y {
			get; set;
		}
		
		public abstract double Angle {
			get; set;
		}
		
		public abstract IDragBehaviour DragBehaviour {
			get; set;
		}
		
		public abstract IDropBehaviour DropBehaviour {
			get; set;
		}
	}
}
