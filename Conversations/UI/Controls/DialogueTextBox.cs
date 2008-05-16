/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 16/05/2008
 * Time: 09:12
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Windows;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Conversations.UI.Controls
{
	/// <summary>
	/// Description of DialogueTextBox.
	/// </summary>
	public class DialogueTextBox : SwitchableTextBox
	{
		public DialogueTextBox() : base() {}
		
		
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
	}
}
