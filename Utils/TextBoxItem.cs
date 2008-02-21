/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 20/02/2008
 * Time: 17:03
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using System.Windows.Forms;
using TD.SandBar;

namespace AdventureAuthor.Utils
{
	/// <summary>
	/// For use with the Sandbar library - hosts a textbox in order to add it to a ToolBar.
	/// </summary>
	public class TextBoxItem : ControlContainerItem
	{
		public TextBoxItem() : base(new TextBox())
		{
		}
	}
}
