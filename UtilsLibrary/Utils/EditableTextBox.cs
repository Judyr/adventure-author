/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 16/02/2008
 * Time: 21:11
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Utils
{
	/// <summary>
	/// XAML-friendly version of SwitchableTextBox - made a new class so as not to disrupt the other applications.
	/// Should switch them all over to use this at some point (never will).
	/// </summary>
	public class EditableTextBox : ReportingTextBox
	{
		public event EventHandler EditableChanged;
		protected virtual void OnEditableChanged(EventArgs e)
		{
			EventHandler handler = EditableChanged;
			if (handler != null) {
				handler(this, e);
			}
		}
		
		
		public static readonly DependencyProperty IsEditableProperty = DependencyProperty.Register("IsEditable",
		                                                                        typeof(bool),
		                                                                        typeof(EditableTextBox));
		                                                                        
		
			
		public bool IsEditable {
			get { 
				return (bool)this.GetValue(IsEditableProperty);
			}
			set { 
				this.SetValue(IsEditableProperty,value);
				if (value) {
		        	Background = Brushes.White;
		        	BorderBrush = Brushes.Black;
		        	IsReadOnly = false;
		        	Cursor = Cursors.IBeam;
				}
				else {
		        	Background = Brushes.Transparent;
		        	BorderBrush = Brushes.Transparent;
					IsReadOnly = true;
					Cursor = Cursors.Arrow;
				}
				OnEditableChanged(new EventArgs());
			}
		}
		
		
		public EditableTextBox() : this(true)
		{			
		}
		
		
		public EditableTextBox(bool editable) : base()
		{		
			IsEditable = editable;
			AllowDrop = true;
		}
	}
}
