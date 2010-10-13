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
namespace AdventureAuthor.Conversations.UI.Graph
{
	partial class GraphForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.closeGraphFormButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// closeGraphFormButton
			// 
			this.closeGraphFormButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.closeGraphFormButton.BackColor = System.Drawing.SystemColors.ButtonHighlight;
			this.closeGraphFormButton.Font = new System.Drawing.Font("Arial", 11F);
			this.closeGraphFormButton.ForeColor = System.Drawing.Color.Maroon;
			this.closeGraphFormButton.Location = new System.Drawing.Point(599, 12);
			this.closeGraphFormButton.Name = "closeGraphFormButton";
			this.closeGraphFormButton.Size = new System.Drawing.Size(64, 29);
			this.closeGraphFormButton.TabIndex = 0;
			this.closeGraphFormButton.Text = "Exit";
			this.closeGraphFormButton.UseVisualStyleBackColor = false;
			this.closeGraphFormButton.Visible = false;
			this.closeGraphFormButton.Click += new System.EventHandler(this.OnClick_CloseGraphForm);
			// 
			// GraphForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(675, 549);
			this.Controls.Add(this.closeGraphFormButton);
			this.Name = "GraphForm";
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Button closeGraphFormButton;
	}
}
