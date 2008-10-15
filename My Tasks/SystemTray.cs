/*
 *   This file is part of Adventure Author.
 *
 *   Adventure Author is copyright Heriot-Watt University 2006-2008.
 *
 *   This copyright and licence apply to all source code, compiled code,
 *   documentation, graphics and auxiliary files, except where otherwise stated.
 *
 *   Adventure Author is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 *   Adventure Author is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 * 
 *   Adventure Author is a plugin for Atari's Neverwinter Nights 2, a COMMERCIAL
 *   product. Permission is given to link this GPL-covered plug-in with the 
 *   non-free main program. 
 *
 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Reflection;
using System.Resources;
using System.Drawing;
using System.Windows;
using System.ComponentModel;
using System.Windows.Forms;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Tasks
{
	/// <summary>
	/// Description of SystemTray.
	/// </summary>
	public partial class MyTasksWindow : System.Windows.Window
	{
		/// <summary>
		/// Place an icon representing this application in the system tray.
		/// </summary>
		private void LaunchInSystemTray(object sender, RoutedEventArgs e)
		{
			ResourceManager manager = new ResourceManager("AdventureAuthor.Tasks.Icons",Assembly.GetExecutingAssembly());
			Icon icon = (Icon)manager.GetObject("textfile");		
			
			trayIcon = new NotifyIcon();
			trayIcon.Icon = icon;
			trayIcon.Text = "Click to view your Top Task, or right-click for options.";
			
			trayIcon.MouseClick += ShowBalloonTipOnLeftClick;
			
			ContextMenuStrip menu = new ContextMenuStrip();		
			
			ToolStripMenuItem openMyTasks = new ToolStripMenuItem("Open My Tasks");
			openMyTasks.Font = new Font(openMyTasks.Font.FontFamily,
			                            openMyTasks.Font.Size,
			                            System.Drawing.FontStyle.Bold);
			openMyTasks.Click += ShowMainWindow;			
			menu.Items.Add(openMyTasks);		
			
			ToolStripMenuItem showTopTask = new ToolStripMenuItem("Show Top Task");
			showTopTask.Click += delegate { ShowBalloonTip(); };
			menu.Items.Add(showTopTask);
			
			menu.Items.Add(new ToolStripSeparator());
			
			ToolStripMenuItem exit = new ToolStripMenuItem("Exit");
			exit.Click += OnClick_Exit;
			menu.Items.Add(exit);
			
			trayIcon.ContextMenuStrip = menu;
			
			trayIcon.Visible = true;
		}


		/// <summary>
		/// On left-click show a balloon tip over this application's system tray icon which
		/// informs the user of the topmost uncompleted task in their list.
		/// </summary>
		private void ShowBalloonTipOnLeftClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left) {
				ShowBalloonTip();
			}
		}
				

		/// <summary>
		/// Show a balloon tip over this application's system tray icon which
		/// informs the user of the topmost uncompleted task in their list.
		/// </summary>
		private void ShowBalloonTip()
		{
			if (trayIcon != null) {	
				if (pad.Tasks != null) {
					foreach (Task task in pad.Tasks) {
						if (task.State == TaskState.NotCompleted || task.State == TaskState.InProgress) {
							ShowBalloonTipWithoutFadeIn(12000,"Top Task",task.Description,ToolTipIcon.None);
							return;
						}
					}
					
					if (pad.Tasks.Count == 0) {
						ShowBalloonTipWithoutFadeIn(12000,null,"You haven't added any tasks yet. Once you have,\n" +
						                            		   "clicking here will display your Top Task.",ToolTipIcon.None);
					}
					else {
						ShowBalloonTipWithoutFadeIn(12000,null,"All tasks completed!",ToolTipIcon.None);
					}					
				}
			}
		}
		
    	
    	/// <summary>
    	/// Show the main window - regardless of whether it is currently hidden,
    	/// minimised or out of view.
    	/// </summary>
    	private void ShowMainWindow(object sender, EventArgs e)
    	{
    		if (!IsVisible) { // if the window has been 'closed' (actually hidden):
    			Show();
    			WindowState = System.Windows.WindowState.Maximized;
    		}    			    		
    		if (WindowState == System.Windows.WindowState.Minimized) {
    			WindowState = System.Windows.WindowState.Normal;
    		}
    		if (!IsActive) {
    			Activate(); // bring the window into view
    		}    		
    	}
		
    	
    	/// <summary>
    	/// Provides access to the HideWindow method, allowing the main window to be hidden instead of closed.
    	/// </summary>
		public delegate void HideWindowDelegate();	
		
		
		/// <summary>
		/// Hide the window.
		/// </summary>
		/// <remarks>Intended for use with HideWindowDelegate.</remarks>
		private void HideWindow()
		{
			Hide();
		}
    	
    	    	
    	/// <summary>
    	/// Check that the user wants to close, and save their preferences if they do.
    	/// </summary>
    	private void SavePreferencesWhenClosing(object sender, CancelEventArgs e)
    	{
    		if (!CloseDialog()) { // return if the user cancels
    			e.Cancel = true;
    			return;
    		}
			
    		try {
	    		// Serialize the user's preferences:
	    		Serialization.Serialize(MyTasksPreferences.DefaultPreferencesPath,
	    		                        MyTasksPreferences.Instance);
    		}
    		catch (Exception ex) {
    			Say.Error("Something went wrong when trying to save your preferences file.",ex);
    		}
    	}	 
		
		
    	/// <summary>
    	/// Immediately displays a balloon tip over the system tray icon, avoiding the standard fade-in.
    	/// </summary>
    	/// <param name="timeout">The time period, in milliseconds, the balloon tip should display.</param>
    	/// <param name="tipTitle">The title to display on the balloon tip.</param>
    	/// <param name="tipText">The text to display on the balloon tip.</param>
    	/// <param name="tipIcon">One of the System.Windows.Forms.ToolTipIcon values.</param>
		private void ShowBalloonTipWithoutFadeIn(int timeout, string tipTitle, string tipText, ToolTipIcon tipIcon)
		{
			if (trayIcon != null) {
				trayIcon.ShowBalloonTip(timeout,tipTitle,tipText,tipIcon);
				trayIcon.ShowBalloonTip(timeout,tipTitle,tipText,tipIcon);
				// Horrible, but avoids the fade-in.
			}
		}
	}
}
