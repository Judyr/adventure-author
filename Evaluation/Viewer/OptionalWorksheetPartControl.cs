/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 24/01/2008
 * Time: 15:36
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Windows.Controls;
using System.Collections.Generic;

namespace AdventureAuthor.Evaluation.Viewer
{
	public abstract class OptionalWorksheetPartControl : ActivatableControl
	{
		#region Events
		
		public event EventHandler Changed;
		
		protected virtual void OnChanged(EventArgs e)
		{
			EventHandler handler = Changed;
			if (handler != null) {
				handler(this,e);
			}
		}
		
		#endregion
		
		#region Constructors
		
		protected OptionalWorksheetPartControl()
		{
		}
		
		#endregion
			
		#region Methods
			
		protected abstract OptionalWorksheetPart GetWorksheetPartObject();
		
		public OptionalWorksheetPart GetWorksheetPart()
		{
			OptionalWorksheetPart part = GetWorksheetPartObject();
			if (part == null) {
				throw new InvalidOperationException("GetWorksheetPart() returned a null reference.");
			}      
			
			SetIncludeStatus(part);
			return part;
		}
		
		protected abstract List<Control> GetActivationControls();
		
		#endregion
	}
}
