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
using System.Windows.Forms;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Core.UI
{
	/// <summary>
	/// Description of CreateChapter_Form.
	/// </summary>
	public partial class CreateChapter_Form : Form
	{
		public CreateChapter_Form()
		{
			Log.Write(Log.WizardAction.Started,Log.WizardSubject.CreateChapterWizard);
			InitializeComponent();
		}
		
		void CreateChapter_Cancel(object sender, EventArgs e)
		{
			this.Close();
			Log.Write(Log.WizardAction.Cancelled,Log.WizardSubject.CreateChapterWizard);			
		}
		
		void CreateChapter_OK(object sender, EventArgs e)
		{
			string name = this.textBox_ChapterName.Text;
			string intro = this.textBox_IntroductionToChapter.Text;
			bool exterior = this.radioButton_Outdoors.Checked;
			int width = 8;
			int height = 8;
			
			try {
				Adventure.CurrentAdventure.AddChapter(name,intro,exterior,width,height);
			}
			catch (ArgumentException ae) {
				Say.Error(ae);
				return;
			}
			
			this.Close();
			Log.Write(Log.WizardAction.Completed,Log.WizardSubject.CreateChapterWizard);
		}
	}
}
