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
using AdventureAuthor.Conversations.UI.Controls;

namespace AdventureAuthor.Utils
{
	/// <summary>
	/// Description of SwitchableTextBox.
	/// </summary>
	public class SwitchableTextBox : ReportingTextBox
	{
		public event EventHandler EditableChanged;
		protected virtual void OnEditableChanged(EventArgs e)
		{
			EventHandler handler = EditableChanged;
			if (handler != null) {
				handler(this, e);
			}
		}
		
		
		private bool isEditable;		
		public bool IsEditable {
			get { return isEditable; }
			set { 
				isEditable = value;
				if (isEditable) {
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
		
		
		public SwitchableTextBox() : this(true)
		{			
		}
		
		
		/// <summary>
		/// Allow LineControls to be dragged over and dropped on this textbox.
		/// </summary>
		protected override void OnDragOver(DragEventArgs e)
		{
			if (e.Data.GetDataPresent(typeof(LineControl)) ||
			    e.Data.GetDataPresent(typeof(Line)) ||
			    e.Data.GetDataPresent(typeof(BranchLine))) 
			{
				return;
			}
			
			base.OnDragOver(e);
		}
		
		
		/// <summary>
		/// Allow LineControls to be dragged over and dropped on this textbox.
		/// </summary>
		protected override void OnDrop(DragEventArgs e)
		{
			if (e.Data.GetDataPresent(typeof(LineControl)) ||
			    e.Data.GetDataPresent(typeof(Line)) ||
			    e.Data.GetDataPresent(typeof(BranchLine))) 
			{
				return;
			}
			
			base.OnDragOver(e);
		}
		
		
		public SwitchableTextBox(bool isEditable) : base()
		{		
			IsEditable = isEditable;
			AllowDrop = true;
		}
	}
}
