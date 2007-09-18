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
using System.Windows.Forms;
using System.Collections.Generic;
using Netron.Diagramming.Core;
using Netron.Diagramming.Win;
using AdventureAuthor.Utils;

namespace AdventureAuthor.UI.Forms
{
	/// <summary>
	/// Description of ConversationGraph.
	/// </summary>
	public partial class ConversationGraph : Form
	{
		private static int offset = 0;
		private DiagramControl diagramControl1;
		
		public ConversationGraph()
		{
			TopLevel = false; // to allow it to be added to a WindowsFormHost
			InitializeComponent();
			
            this.diagramControl1 = new Netron.Diagramming.Win.DiagramControl();
            ((System.ComponentModel.ISupportInitialize)(this.diagramControl1)).BeginInit();
            this.SuspendLayout();
            // 
            // diagramControl1
            // 
            ClassShape shape = new ClassShape();
            shape.Enabled = false;
            shape.Height = 500; 
            shape.Width =200;
            shape.X = 80;
            shape.Y = 90;
            shape.Text = "Iam an text.";
            
            ClassShape shape2 = new ClassShape();
            shape2.Height = 50; 
            shape2.Width =300;
            shape2.X = 0;
            shape2.Y = 0;
            shape2.Text = "I am some text";
            
            SimpleRectangle rect = new SimpleRectangle();
            rect.Height = 40;
            rect.Width = 250;
            rect.Name = "my reeeect";
            rect.OnEntitySelect += delegate { Say.Information("Selected " + rect.Name + "."); };
            rect.Text = "listen to me !";
            rect.X = 100;
            rect.Y = 120;
            Say.Information(rect.EntityName);
            
            this.diagramControl1.AddShape(shape);
            this.diagramControl1.AllowDrop = true;
            this.diagramControl1.AutoScroll = true;
            this.diagramControl1.BackColor = System.Drawing.Color.Silver;
            this.diagramControl1.BackgroundType = Netron.Diagramming.Core.CanvasBackgroundTypes.FlatColor;
            this.diagramControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.diagramControl1.EnableAddConnection = true;
            this.diagramControl1.Location = new System.Drawing.Point(0, 0);
            this.diagramControl1.Magnification = new System.Drawing.SizeF(100F, 100F);
            this.diagramControl1.Name = "diagramControl1";
            this.diagramControl1.Origin = new System.Drawing.Point(0, 0);
            this.diagramControl1.ShowPage = false;
            this.diagramControl1.ShowRulers = false;
            this.diagramControl1.Size = new System.Drawing.Size(862, 755);
            this.diagramControl1.TabIndex = 0;
            this.diagramControl1.Text = "diagramControl1";
            this.diagramControl1.AddShape(shape2);
            this.diagramControl1.AddShape(rect);
            // 
            // Form1
            // 
            this.Controls.Add(this.diagramControl1);
            ((System.ComponentModel.ISupportInitialize)(this.diagramControl1)).EndInit();
            this.ResumeLayout(false);
			
			
		}
		
		
			
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			

 			
		}
		
		protected override void OnClick(EventArgs e)
		{
			
			foreach (Control c in this.Controls) {
				if (c is DiagramControl) {
					offset += 40;
					Say.Information("Trying to add shapes.");
					DiagramControl d = (DiagramControl)c;
					ClassShape shape = new ClassShape();
					shape.Height = 30;
					shape.Width =30;
					shape.X = offset;
					shape.Y = offset;
					shape.Text = "ARRG";
					d.AddShape(shape);
					d.Invalidate();
					this.Invalidate();
				}
			}
		}
		
	}
}
