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
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using AdventureAuthor.Core;
using AdventureAuthor.UI;
using AdventureAuthor.Utils;

namespace AdventureAuthor.UI.Forms
{
	/// <summary>
	/// Description of NewAdventure_Form.
	/// </summary>
	public partial class NewAdventure_Form : Form
	{
		public NewAdventure_Form()
		{
			InitializeComponent();			
			label_CurrentUser.Text = "Current user:  " + Adventure.CurrentUser.Name;
		}
		
		void Button_Cancel_NewAdventureClick(object sender, EventArgs e)
		{
			this.Close(); 
		}
		
		void Button_OK_NewAdventureClick(object sender, EventArgs e)
		{
			try {
				if (!Adventure.IsValidName(textBox_NameOfNewAdventure.Text)) {
					Say.Error("The name '" + textBox_NameOfNewAdventure.Text + "' is invalid. Adventure names " + 
					          "must not contain the following characters ('<', '>', ':', '\', '\"', '/', '|', '.') " +
					          "and must be between 1 and 32 characters in length.");
				}
				else if (!Adventure.IsAvailableAdventureName(textBox_NameOfNewAdventure.Text)) {
					Say.Error("An Adventure called '" + textBox_NameOfNewAdventure.Text + "' already " +
					          "exists: choose another name.");
				}
				else {
					if (Adventure.CurrentAdventure != null && !Toolset.CloseAdventureDialog()) {
						return;
					}
					
					new Adventure(textBox_NameOfNewAdventure.Text);
					Adventure adventure = Adventure.Open(textBox_NameOfNewAdventure.Text);
					if (adventure == null) {
						Say.Error("Failed to open Adventure.");
						return;
					}
					else {
						Adventure.CurrentAdventure.Scratch.Open();
						this.Close();
					};
				}
			}
			catch (IOException ioe) {
				Say.Error(ioe);
			}
		}
	}
}
