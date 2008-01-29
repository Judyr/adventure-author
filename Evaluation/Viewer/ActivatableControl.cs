/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 23/01/2008
 * Time: 14:04
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;

namespace AdventureAuthor.Evaluation.Viewer
{
	/// <summary>
	/// Description of ActivatableControl.
	/// </summary>
	public abstract class ActivatableControl : UserControl
	{
		#region Fields
		
		protected bool active;
				
		#endregion
		
		#region Events
		
		public event EventHandler<ActivationEventArgs> Activated;
		
		protected virtual void OnActivated(ActivationEventArgs e)
		{
			EventHandler<ActivationEventArgs> handler = Activated;
			if (handler != null) {
				handler(this,e);
			}
		}
		
		public event EventHandler<ActivationEventArgs> Deactivated;
		
		protected virtual void OnDeactivated(ActivationEventArgs e)
		{
			EventHandler<ActivationEventArgs> handler = Deactivated;
			if (handler != null) {
				handler(this,e);
			}
		}
		
		#endregion
		
		#region Constructors
		
		protected ActivatableControl()
		{
		}
		
		#endregion
		
		#region Methods
				
		public void Enable()
		{
			PerformEnable();
			active = true; 
		}
		
		public void Activate()
		{
			PerformActivate();
			active = true;
			OnActivated(new ActivationEventArgs(this));
		}
		
		public void Deactivate(bool preventReactivation)
		{
			PerformDeactivate(preventReactivation);
			active = false;
			OnDeactivated(new ActivationEventArgs(this));
		}
		
		protected abstract void PerformEnable();
		protected abstract void PerformActivate();
		/// <summary>
		/// Deactivate this control.
		/// </summary>
		/// <param name="preventReactivation">True if the controls that allow activation and deactivation
		/// should also be deactivated; false otherwise</param>
		protected abstract void PerformDeactivate(bool preventReactivation);
		
		
		internal static void EnableElement(UIElement element)
		{		
			if (element != null) {				
	    		if (!element.IsEnabled) {
	    			element.IsEnabled = true;
	    		}
	    		element.Opacity = 1.0f;
			}
		}
		
		
		internal static void ActivateElement(UIElement element)
		{
			if (element != null) {	
	    		if (element.IsEnabled) {
	    			element.IsEnabled = false;
	    		}
	    		element.Opacity = 1.0f;
			}
		}
		
		
		internal static void DeactivateElement(UIElement element)
		{
			if (element != null) {	
	    		if (element.IsEnabled) {
	    			element.IsEnabled = false;
	    		}
	    		element.Opacity = 0.2f;
			}
		}
		
		#endregion
	}
}
