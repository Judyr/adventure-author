﻿/*
 *   This file is part of Adventure Author.
 *
 *   Adventure Author is copyright Heriot-Watt University 2006-2007.
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

namespace AdventureAuthor.Conversations.UI.Controls
{
	/// <summary>
	/// Description of Branch.
	/// </summary>
	public class BranchLine : LineControl
	{
		public BranchLine(NWN2ConversationConnector line) : base(line)
		{        	
        	// Check if there are conditions for this line to be spoken, and if so represent *all* of them with a single control:
        	if (line.Conditions.Count > 0) {
        		conditionalControl = new ConditionControl(this);
        		Grid.SetRow(conditionalControl,0);
        		Grid.SetColumn(conditionalControl,0);
        		Grid.SetColumnSpan(conditionalControl,3);
        		LineControlGrid.Children.Add(conditionalControl);
        	}   
			
        	
			
			
			// TODO: make into less of a mess
			Dialogue.LostFocus += new RoutedEventHandler(OnDialogueLostFocus2);
			Dialogue.LostFocus += new RoutedEventHandler(OnDialogueLostFocus);
			Dialogue.GotFocus += new RoutedEventHandler(OnDialogueGotFocus);
        	this.KeyDown += new KeyEventHandler(OnKeyDown);
        	
        	this.MouseDoubleClick += new MouseButtonEventHandler(OnMouseDoubleClick_GoToPage);
        	
        	       	
        	
			Speaker speaker = Conversation.CurrentConversation.GetSpeaker(nwn2Line.Speaker);
			if (speaker != null) { 
				SpeakerLabel.Foreground = speaker.Colour;
			}
			else { // if we can't identify the speaker - but this should never happen
				SpeakerLabel.Foreground = Brushes.Black;
			}
		}
		
		
		protected override void SetupContextMenu()
		{
			MenuItem MenuItem_GoToPage = (MenuItem)FindName("MenuItem_GoToPage");
			MenuItem MenuItem_AddAction = (MenuItem)FindName("MenuItem_AddAction");
			MenuItem MenuItem_AddCondition = (MenuItem)FindName("MenuItem_AddCondition");
			MenuItem MenuItem_SetCamera = (MenuItem)FindName("MenuItem_SetCamera");
			MenuItem MenuItem_DeleteLine = (MenuItem)FindName("MenuItem_DeleteLine");
			MenuItem MenuItem_MakeIntoChoice = (MenuItem)FindName("MenuItem_MakeIntoChoice");
			
			MenuItem_GoToPage.Header = "Go down to page";
			MenuItem_GoToPage.IsEnabled = true;
			MenuItem_GoToPage.Visibility = Visibility.Visible;
			
			MenuItem_AddAction.IsEnabled = true;
			MenuItem_AddAction.Visibility = Visibility.Visible;
			
			MenuItem_AddCondition.IsEnabled = true;
			MenuItem_AddAction.Visibility = Visibility.Visible;
			
			MenuItem_SetCamera.IsEnabled = false;
			MenuItem_SetCamera.Visibility = Visibility.Visible;
			
			MenuItem_DeleteLine.IsEnabled = true;
			MenuItem_DeleteLine.Visibility = Visibility.Visible;
			
			MenuItem_MakeIntoChoice.IsEnabled = false;
			MenuItem_MakeIntoChoice.Visibility = Visibility.Collapsed;
		}
		
				
		/// <summary>
		/// Display the page that this line leads to.
		/// </summary>
        protected override void OnClick_GoToPage(object sender, EventArgs ea)
        {
        	foreach (Page page in WriterWindow.Instance.Pages) {
        		if (page.LeadInLine == this.nwn2Line) {
        			WriterWindow.Instance.DisplayPage(page);
        			WriterWindow.Instance.CentreGraph(false);
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
