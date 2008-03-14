/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 14/03/2008
 * Time: 12:27
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AdventureAuthor.Utils
{
	/// <summary>
	/// A textbox which raises an event when the user has finished editing it,
	/// i.e. when it has both lost keyboard focus and had its value has changed from
	/// what it previously was. 
	/// </summary>
	public class ReportingTextBox : TextBox
	{
		#region Fields
		
		protected string oldValue;
		
		#endregion
		
		#region Events
		
		public event EventHandler<TextEditedEventArgs> TextEdited;
		protected virtual void OnTextEdited(TextEditedEventArgs e)
		{
			EventHandler<TextEditedEventArgs> handler = TextEdited;
			if (handler != null) {
				handler(this,e);
			}
		}
		
		#endregion
		
		#region Constructors
		
		public ReportingTextBox() : base()
		{
			oldValue = Text;
			LostKeyboardFocus += new KeyboardFocusChangedEventHandler(lostKeyboardFocus);
		}

		#endregion
		
		#region Methods
		
		public void SetText(string text)
		{
			Text = text;
			oldValue = Text;
		}
		
		#endregion
		
		#region Event handlers
		
		protected void lostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			if (oldValue != Text) {
				OnTextEdited(new TextEditedEventArgs(oldValue,Text));
				oldValue = Text;
			}
		}
		
		#endregion
	}
}
