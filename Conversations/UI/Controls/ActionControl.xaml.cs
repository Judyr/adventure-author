/*
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
using AdventureAuthor.Scripts;
using AdventureAuthor.Utils;
using NWN2Toolset.NWN2.Data;

namespace AdventureAuthor.Conversations.UI.Controls
{
	/// <summary>
	/// A control representing the action attached to a particular line of dialogue.
	/// </summary>
    public partial class ActionControl : UserControl
    {
    	/// <summary>
    	/// The control representing the line of dialogue this action is attached to.
    	/// </summary>
    	private LineControl attachedTo;    	
		public LineControl AttachedTo {
			get { return attachedTo; }
			set { attachedTo = value; }
		}
    	
    	/// <summary>
    	/// The action to represent.
    	/// </summary>
    	private NWN2ScriptFunctor action;    	
		public NWN2ScriptFunctor Action {
			get { return action; }
			set { action = value; }
		}
    	
    	
    	/// <summary>
    	/// Create a new ActionControl.
    	/// </summary>
    	/// <param name="action">The action to represent</param>
    	/// <param name="attachedTo">The control representing the line of dialogue this action is attached to</param>
        internal ActionControl(NWN2ScriptFunctor action, LineControl attachedTo)
        {
            InitializeComponent();
            this.action = action;
            this.attachedTo = attachedTo;
            this.Description.Text = ScriptHelper.GetDescriptionForAction(action);
        }

        
        /// <summary>
        /// Launch a window to edit the parameters of an existing action. 
        /// </summary>
        private void OnClick_EditAction(object sender, EventArgs ea)
        {
        	Say.Information("Not implemented yet.");
        	
//        	Log.WriteUIAction(Log.UIAction.Clicked,"EditAction_button","not implemented yet");
        	
//        	Conversation.CurrentConversation.ReplaceAction(AttachedTo.Nwn2Line,Action,null);
        }
        
        
        /// <summary>
        /// Delete this action from the line it's attached to.
        /// </summary>
        private void OnClick_DeleteAction(object sender, EventArgs ea)
        {
//        	Log.WriteDialogAction(Log.DialogAction.Started,"DeleteActionDialog");
	 		MessageBoxResult result = MessageBox.Show("Delete this action?","Delete?", MessageBoxButton.YesNo);
			if (result == MessageBoxResult.Yes) {
//	 			Log.WriteDialogAction(Log.DialogAction.Completed,"DeleteActionDialog");
	 			Conversation.CurrentConversation.DeleteAction(AttachedTo.Nwn2Line,Action);
	 			
			}	
	 		else {
//	 			Log.WriteDialogAction(Log.DialogAction.Aborted,"DeleteActionDialog");
	 		}
        }
        
        
        /// <summary>
        /// Give focus to the parent LineControl.
        /// </summary>
        private void OnMouseDown(object sender, EventArgs ea)
        {
        	this.attachedTo.Focus();    
        	Log.WriteAction(Log.Action.selected,"line");
        }
    }
}
