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
	public class SwitchableTextBox : ReportingTextBox
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
		        	Cursor = Cursors.IBeam;
				}
				else {
					IsEditable = true;
		        	Background = Brushes.Transparent;
		        	BorderBrush = Brushes.Transparent;
					IsReadOnly = true;
					Cursor = Cursors.Arrow;
				}
			}
		}
		
		
		public SwitchableTextBox() : this(true)
		{
			AllowDrop = true;
		}
		
		
		public SwitchableTextBox(bool isEditable) : base()
		{
			IsEditable = isEditable;
		}
	}
}
