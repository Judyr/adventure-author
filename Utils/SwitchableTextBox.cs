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

namespace AdventureAuthor.Utils
{
	/// <summary>
	/// Description of SwitchableTextBox.
	/// </summary>
	public class SwitchableTextBox : System.Windows.Controls.TextBox
	{
		private bool isEditable;		
		public bool IsEditable {
			get { return isEditable; }
			set { 
				isEditable = value;
				if (isEditable) {
		        	Background = Brushes.White;
		        	BorderBrush = Brushes.Black;
		        	IsReadOnly = false;
		        	Focusable = true;
		        	cursor = Cursors.IBeam;
				}
				else {
		        	Background = Brushes.Transparent;
		        	BorderBrush = Brushes.Transparent;
					IsReadOnly = true;
					Focusable = false;
					cursor = Cursors.Arrow;
				}
			}
		}
		
		
		private Cursor cursor;
		
		
		public SwitchableTextBox() : this(true)
		{
		}
		
		
		public SwitchableTextBox(bool isEditable) : base()
		{
			IsEditable = isEditable;
			MouseEnter += new MouseEventHandler(SwitchableTextBox_MouseEnter);
		}
		

		/// <summary>
		/// When not in editable form, the cursor will stay as the default arrow instead of the text IBeam.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SwitchableTextBox_MouseEnter(object sender, MouseEventArgs e)
		{
			if (Cursor != cursor) {
				Cursor = cursor;
			}
		}
	}
}
