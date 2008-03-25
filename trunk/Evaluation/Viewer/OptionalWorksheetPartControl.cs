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
using AdventureAuthor.Utils;

namespace AdventureAuthor.Evaluation.Viewer
{
	public abstract class OptionalWorksheetPartControl : ActivatableControl
	{		
		#region Fields
		
		/// <summary>
		/// The name of the control type, for logging purposes.
		/// </summary>
		protected string properName;
		
		#endregion
		
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
		
		public abstract void ShowActivationControls();
		public abstract void HideActivationControls();			
		protected abstract OptionalWorksheetPart GetWorksheetPartObject();
		
		public OptionalWorksheetPart GetWorksheetPart()
		{
			OptionalWorksheetPart part = GetWorksheetPartObject();
			if (part == null) {
				throw new InvalidOperationException("GetWorksheetPart() returned a null reference.");
			}      
			
			// This worksheet part should only be included if its control has been set to 'active':
			part.Include = this.isActive;
			return part;
		}		
		
				
		protected void SetInitialActiveStatus(OptionalWorksheetPart representedWorksheetPart)
		{          
            if (WorksheetViewer.Instance.EvaluationMode == Mode.Design) { // show 'Active?' control, and assume that control is Active to begin with
				ShowActivationControls();
	    		if (representedWorksheetPart.Include) {
					Activate();
	    		}
	    		else {
	    			Deactivate(false);
	    		}
            }
            else { // hide 'Active?' control
            	Enable();
            }
		}        
		
        
		public override string ToString()
		{
			return GetWorksheetPartObject().ToString();
		}
		
		
        protected virtual void onActivatorChecked(object sender, EventArgs e)
        {
        	Activate();
        }
        
        
        protected virtual void onActivatorUnchecked(object sender, EventArgs e)
        {    		
        	Deactivate(false);
        }
        
        
        /// <summary>
        /// Log whether the control has been activated or deactivated when the
        /// activator checkbox is clicked. Note that basing this on Checked and
        /// Unchecked would register the activation/deactivation of child controls
        /// also, but we only want the one the user clicked.
        /// </summary>
        protected virtual void onActivatorClicked(object sender, EventArgs e)
        {
        	// Log what it's about to become (deactivated) rather than what it currently is,
        	// since the current activation status won't be updated yet.
        	if (IsActive) { 
        		Log.WriteAction(LogAction.deactivated,properName);
        	}
        	else {
        		Log.WriteAction(LogAction.activated,properName);
        	}
        }
		
		#endregion
	}
}
