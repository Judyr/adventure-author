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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using NWN2Toolset.NWN2.Data.ConversationData;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Conversations.UI.Controls
{
	/// <summary>
	/// Description of LeadingLine.
	/// </summary>
	public class LeadingLine : LineControl
	{
		public LeadingLine(NWN2ConversationConnector line) : base(line)
		{
        	// Disable and grey out everything:
        	Focusable = false;
        	DeleteLineButton.IsEnabled = false;
        	DeleteLineButton.Visibility = Visibility.Hidden;  
        	SpeakerLabel.IsEnabled = false;
        	SpeakerLabel.Foreground = Brushes.Gray;
        	Dialogue.Foreground = Brushes.Gray;
        	Dialogue.Background = Brushes.LightYellow;
        	Dialogue.BorderBrush = Brushes.Transparent;         	
        	Dialogue.IsEnabled = false;
        	Dialogue.ContextMenu = null;
        	foreach (ActionControl actionControl in actionControls) {
        		actionControl.Foreground = Brushes.Gray;
        	}
        	if (conditionalControl != null) {
        		conditionalControl.Foreground = Brushes.Gray;
        	} 
        	if (soundControl != null) {
        		soundControl.Foreground = Brushes.Gray;
        	}
        	
        	this.MouseDoubleClick += new MouseButtonEventHandler(OnMouseDoubleClick_GoToPage);
		}  		
		
		
		protected override void SetupContextMenu()
		{
			MenuItem MenuItem_GoToPage = (MenuItem)FindName("MenuItem_GoToPage");
			MenuItem MenuItem_AddAction = (MenuItem)FindName("MenuItem_AddAction");
			MenuItem MenuItem_AddCondition = (MenuItem)FindName("MenuItem_AddCondition");
			MenuItem MenuItem_DeleteAction = (MenuItem)FindName("MenuItem_DeleteAction");
			MenuItem MenuItem_DeleteCondition = (MenuItem)FindName("MenuItem_DeleteCondition");
			MenuItem MenuItem_DeleteLine = (MenuItem)FindName("MenuItem_DeleteLine");
			MenuItem MenuItem_MakeIntoChoice = (MenuItem)FindName("MenuItem_MakeIntoChoice");
			
			MenuItem_GoToPage.Header = "Go up to page";
			MenuItem_GoToPage.IsEnabled = true;
			MenuItem_GoToPage.Visibility = Visibility.Visible;
			
			MenuItem_AddAction.IsEnabled = false;
			MenuItem_AddAction.Visibility = Visibility.Collapsed;
			
			MenuItem_AddCondition.IsEnabled = false;
			MenuItem_AddCondition.Visibility = Visibility.Collapsed;
			
			MenuItem_DeleteAction.IsEnabled = false;
			MenuItem_DeleteAction.Visibility = Visibility.Collapsed;
			
			MenuItem_DeleteCondition.IsEnabled = false;
			MenuItem_DeleteCondition.Visibility = Visibility.Collapsed;
			
			MenuItem_DeleteLine.IsEnabled = false;
			MenuItem_DeleteLine.Visibility = Visibility.Collapsed;
			
			MenuItem_MakeIntoChoice.IsEnabled = false;
			MenuItem_MakeIntoChoice.Visibility = Visibility.Collapsed;
		}        
		
		
		/// <summary>
		/// Display the page that is the parent of this line (and hence this entire page).
		/// </summary>
		protected override void OnClick_GoToPage(object sender, EventArgs ea)
        {
        	foreach (Page page in WriterWindow.Instance.Pages) {
				if (page.Children.Contains(WriterWindow.Instance.CurrentPage)) {
        			WriterWindow.Instance.DisplayPage(page);        
        			WriterWindow.Instance.CentreGraph(false);
        			WriterWindow.Instance.FocusOn(this.nwn2Line);
	        		Log.WriteAction(LogAction.viewed,"page");	
        			return;
				}
        	}        	
        } 
        
        
        private void OnMouseDoubleClick_GoToPage(object sender, MouseButtonEventArgs e)
        {
        	OnClick_GoToPage(this,e);
        }
	}
}
