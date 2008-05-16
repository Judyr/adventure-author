/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 16/05/2008
 * Time: 15:30
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.IO;
using TD.SandBar;
using AdventureAuthor.Core;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Setup
{
	/// <summary>
	/// Description of UI.
	/// </summary>
	public static class UITools
	{		
		internal static void SetXAMLButtonImage(System.Windows.Controls.Button button, string filename, string alternateText)
		{
	       	try {
				string path = Path.Combine(ModuleHelper.ImagesDir,filename);
				button.Content = ResourceHelper.GetImage(path);
	       	}
	       	catch (Exception e) {
	       		Say.Debug("Couldn't assign image for interface button: " + e);
	       		button.Content = alternateText;
	       	}
		}
		
			
		internal static void SetSandbarButtonImage(ButtonItem button, string filename, string buttonText)
		{		
			string path = Path.Combine(ModuleHelper.ImagesDir,filename);
			button.Image = ResourceHelper.GetBitmap(path);
	        button.Text = buttonText;
	        button.BeginGroup = true;
		}
	}
}
