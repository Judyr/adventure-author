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
using AdventureAuthor.Core;
using AdventureAuthor.Conversations;

namespace AdventureAuthor.UI.Forms
{
	/// <summary>
	/// Description of ConversationGraph.
	/// </summary>
	public partial class ConversationGraph : Form
	{
		private DiagramControl diagramControl;
		
		public ConversationGraph()
		{
			TopLevel = false; // to allow it to be added to a WindowsFormHost
			InitializeComponent();
			
            diagramControl = new DiagramControl();
            ((System.ComponentModel.ISupportInitialize)(this.diagramControl)).BeginInit();
            SuspendLayout();			

            diagramControl.AllowDrop = false;
            diagramControl.AutoScroll = true;
            diagramControl.BackColor = System.Drawing.Color.LightBlue;
            diagramControl.BackgroundType = Netron.Diagramming.Core.CanvasBackgroundTypes.Gradient;
            diagramControl.Dock = System.Windows.Forms.DockStyle.Fill;
            diagramControl.EnableAddConnection = true;
           	Netron.Diagramming.Core.Layout.LayoutSettings.TreeLayout.TreeOrientation = TreeOrientation.TopBottom;
            diagramControl.Location = new System.Drawing.Point(0, 0);
            diagramControl.Magnification = new System.Drawing.SizeF(100F, 100F);
            diagramControl.Name = "diagramControl";
            diagramControl.Size = new Size(500,500);
//            Point p = new Point(-1200,0);
//			diagramControl.Origin = p;
            diagramControl.Origin = new System.Drawing.Point(0, 0);
            
            
            
            diagramControl.ShowPage = true;
            diagramControl.ShowRulers = false;
            diagramControl.TabIndex = 0;
            diagramControl.Text = "Conversation Graph";
            
            diagramControl.Click += delegate { 
            	Point p = new Point(diagramControl.Location.X - 10, diagramControl.Location.Y);
            	diagramControl.Origin = p;
            	diagramControl.Invalidate();
            };
            
            
            this.Controls.Add(this.diagramControl);
            ((System.ComponentModel.ISupportInitialize)(diagramControl)).EndInit();
            ResumeLayout(false);
		}
		
		/*
		 * if this doesn't work:
		 * 
		 * 
		 * 
		 */
		
		
		private void DrawChildren(List<ConversationPage> children, PageNode parentNode)
		{
			if (children == null || children.Count == 0) {
				return;
			}
			
			foreach (ConversationPage child in children) {
				if (child.LeadInLine == null) {
					throw new ArgumentException("Found another root.");
				}
	            PageNode childNode = new PageNode();
            	childNode.Text = UsefulTools.Truncate(Conversation.OEIExoLocStringToString(child.LeadInLine.Text),30);            	
	            diagramControl.AddShape(childNode);	
	            diagramControl.AddConnection(parentNode.Connectors[0],childNode.Connectors[0]);
	            DrawChildren(child.Children,childNode);
			}
		}
	
		public void Open(List<ConversationPage> pages)
		{
			if (pages == null || pages.Count == 0) {
				return;
			}
            ((System.ComponentModel.ISupportInitialize)(this.diagramControl)).BeginInit();
            SuspendLayout();			            
            PageNode root = new PageNode();
            root.Text = "Root";
            diagramControl.AddShape(root);
            diagramControl.SetLayoutRoot(root);
            DrawChildren(pages[0].Children,root);
            diagramControl.Layout(LayoutType.ClassicTree);
            root.Move(new Point(root.X+150,root.Y+15));
            diagramControl.Invalidate();
            ((System.ComponentModel.ISupportInitialize)(diagramControl)).EndInit();
            ResumeLayout(false);
            root.Move(new Point(root.X+150,root.Y+15));
            diagramControl.Invalidate();
		}
		
		public void Clear()
		{
			diagramControl.Document.Model.Clear();
			diagramControl.Invalidate();
		}
	}
}
