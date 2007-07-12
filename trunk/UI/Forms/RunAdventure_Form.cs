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
using System.Drawing;
using System.Windows.Forms;
using AdventureAuthor.AdventureData;

namespace AdventureAuthor.UI.Forms
{
	/// <summary>
	/// Description of RunAdventure_Form.
	/// </summary>
	public partial class RunAdventure_Form : Form
	{
		public RunAdventure_Form()
		{
			InitializeComponent();
			//TODO: Populate combobox with all waypoints in module
		}
		
		void Button_CancelRunAdventureClick(object sender, EventArgs e)
		{
			this.Close();
		}
		
		void Button_RunAdventureClick(object sender, EventArgs e)
		{
			string waypoint;
			if (this.radioButton_RunFromWaypoint.Checked) {
				waypoint = this.comboBox_WaypointsToRunAdventureFrom.SelectedText;
			}
			else {
				waypoint = string.Empty;
			}			
			bool canLaunchDebugWindow = this.checkBox_CanLaunchDebugWindow.Checked;
			bool playerIsInvincible = this.checkBox_PlayerIsInvincible.Checked;
			
			Adventure.CurrentAdventure.Run(waypoint,canLaunchDebugWindow,playerIsInvincible);
		}
		
		void RadioButton_RunFromWaypointCheckedChanged(object sender, EventArgs e)
		{
			this.comboBox_WaypointsToRunAdventureFrom.Enabled = true;
		}
		
		void RadioButton_RunFromStartCheckedChanged(object sender, EventArgs e)
		{
			this.comboBox_WaypointsToRunAdventureFrom.Enabled = false;
		}
	}
}
