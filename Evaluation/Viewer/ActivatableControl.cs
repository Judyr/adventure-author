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

namespace AdventureAuthor.Evaluation.Viewer
{
	/// <summary>
	/// Description of ActivatableControl.
	/// </summary>
	public abstract class ActivatableControl : UserControl
	{
		#region Fields
		
		public enum ControlStatus {
			NA,
			Active,
			Inactive
		}
		
		private ControlStatus activationStatus;
		public ControlStatus ActivationStatus {
			get {
				return activationStatus;
			}
			set {				
				switch (value) {
					case ControlStatus.NA:
						Enable();
						break;
					case ControlStatus.Active:
						Activate();
						OnActivated(new ActivationEventArgs(this));
						break;
					case ControlStatus.Inactive:
						Deactivate();
						OnDeactivated(new ActivationEventArgs(this));
						break;
					default:
						throw new ArgumentException(value.ToString() + " was not known.");
				}
				
				activationStatus = value;
			}
		}
		
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
		
		protected abstract void Enable();
		protected abstract void Activate();		
		protected abstract void Deactivate();
		
		
		protected static void Enable(UIElement element)
		{		
    		if (!element.IsEnabled) {
    			element.IsEnabled = true;
    		}
    		element.Opacity = 1.0f;
		}
		
		
		protected static void Activate(UIElement element)
		{
    		if (element.IsEnabled) {
    			element.IsEnabled = false;
    		}
    		element.Opacity = 1.0f;
		}
		
		
		protected static void Deactivate(UIElement element)
		{
    		if (element.IsEnabled) {
    			element.IsEnabled = false;
    		}
    		element.Opacity = 0.2f;
		}    	
		
		#endregion
	}
}
