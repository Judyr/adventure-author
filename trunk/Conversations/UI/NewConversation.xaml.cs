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
using System.IO;
using System.Windows;
using AdventureAuthor;
using AdventureAuthor.Core;
using AdventureAuthor.Utils;
using form = NWN2Toolset.NWN2ToolsetMainForm;

namespace AdventureAuthor.Conversations.UI
{
    /// <summary>
    /// Interaction logic for NewConversation.xaml
    /// </summary>
    public partial class NewConversation : Window
    {
        public NewConversation()
        {
            InitializeComponent();
        }

        private void OnClickOK(object sender, EventArgs ea)
        {        	
        	string name = ConversationName.Text;
        	if (File.Exists(Path.Combine(Path.Combine(form.ModulesDirectory,Adventure.CurrentAdventure.Module.Repository.DirectoryName),name+".dlg"))) {
        		Say.Warning("A conversation called '" + name + "' already exists in the Adventure '" + 
        		            Adventure.CurrentAdventure.Name + "' - try another name.");
        	}
        	else if (!Adventure.IsValidName(name)) {
				Say.Information("The name '" + name + "' is invalid. Adventure names " + 
				        	    "must not contain the following characters ('<', '>', ':', '\', '\"', '/', '|', '.') " +
				          		"and must be between 1 and 32 characters in length. Try another name.");
        	}
        	else {        		
        		ConversationWriterWindow.Instance.OpenConversation(name,true);
        		this.Close();
        	}
        }
        
        private void OnClickCancel(object sender, EventArgs ea)
        {
        	this.Close();
        }
    }
}
