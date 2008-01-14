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
using AdventureAuthor.Scripts;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Conversations.UI.Controls
{
	/// <summary>
	/// A control representing the condition(s) attached to a particular line of dialogue.
	/// </summary>
    public partial class ConditionControl : UserControl
    {
    	/// <summary>
    	/// The control representing the line of dialogue this condition is attached to.
    	/// </summary>
    	private LineControl attachedTo;    	
		public LineControl AttachedTo {
			get { return attachedTo; }
			set { attachedTo = value; }
		}
    	
    	
    	/// <summary>
    	/// Create a new ConditionControl.
    	/// </summary>
    	/// <param name="attachedTo">The line that this condition is attached to</param>
    	/// <remarks>Not necessary to specify a condition because due to issues with generating a natural language
    	/// description of the boolean logic of multiple conditions, one ConditionControl represents all the conditions
    	/// on a line. Furthermore, adding more than one condition to a single line is currently blocked.</remarks>
        internal ConditionControl(LineControl attachedTo)
        {
        	InitializeComponent();
            this.attachedTo = attachedTo;
            this.Description.Text = ScriptHelper.GetDescriptionForCondition(attachedTo.Nwn2Line.Conditions);
        }

                
        /// <summary>
        /// Launch a window to edit the parameters of an existing condition. 
        /// </summary>
        private void OnClick_EditConditions(object sender, EventArgs ea)
        {
        	Say.Information("Not implemented yet.");
        	
//        	Conversation.CurrentConversation.ReplaceCondition(AttachedTo.Nwn2Line,AttachedTo.Nwn2Line.Conditions[0],null);
        }
        
        
        /// <summary>
        /// Delete this condition from the line it's attached to.
        /// </summary>
        private void OnClick_DeleteConditions(object sender, EventArgs ea)
        {
	 		MessageBoxResult result = MessageBox.Show("Delete this condition?","Delete?", MessageBoxButton.YesNo);
			if (result == MessageBoxResult.Yes) {
	 			Conversation.CurrentConversation.DeleteAllConditions(AttachedTo.Nwn2Line);
			}	 		
        }
        
        
        /// <summary>
        /// Give focus to the parent LineControl.
        /// </summary>
        private void OnMouseDown(object sender, EventArgs ea)
        {
        	this.attachedTo.Focus();        	
        }
    }
}
