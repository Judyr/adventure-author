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
 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.ConversationData;
using AdventureAuthor.Core;
using AdventureAuthor.UI.Controls;
using AdventureAuthor.Utils;

namespace AdventureAuthor.UI.Controls
{
    /// <summary>
    /// Interaction logic for ActionControl.xaml
    /// </summary>

    public partial class ActionControl : UserControl
    {
    	private LineControl owner;    	
		public LineControl Owner {
			get { return owner; }
			set { owner = value; }
		}
    	
        internal ActionControl(LineControl owner)
        {
            InitializeComponent();
            this.owner = owner;
            this.ActionDescriptionTextBlock.Text = Conversation.GetDescriptionOfConditions(owner.Nwn2Line);
        }

        private void OnClick_EditAction(object sender, EventArgs ea)
        {
        	Say.Information("Not implemented. yet.");
        	// TODO: On returning an OK result from ScriptCards/ScriptWizard, Conversation.CurrentConversation.Dirty = true;
        }
        
        private void OnMouseDown(object sender, EventArgs ea)
        {
        	this.owner.Focus();        	
        }
    }
}
