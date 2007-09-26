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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using AdventureAuthor.Conversations;
using AdventureAuthor.Core;
using AdventureAuthor.Utils;
using Netron.Diagramming.Core;
using Netron.Diagramming.Win.AdventureAuthor;

namespace AdventureAuthor.Conversations.UI.Graph
{
	public partial class GraphForm : Form
	{
		public GraphControl graphControl;
		
		public GraphForm()
		{
			try {
				TopLevel = false; // to allow it to be added to a WindowsFormHost
				InitializeComponent();
				
	            graphControl = new GraphControl();
	            ((System.ComponentModel.ISupportInitialize)(this.graphControl)).BeginInit();
	            SuspendLayout();			
				
	            graphControl.Controller.AddTool(new GraphTool("Graph Tool"));
	            graphControl.AllowDrop = false;
	            graphControl.AutoScroll = true;
	            graphControl.BackColor = System.Drawing.Color.LightBlue;
	            graphControl.BackgroundType = Netron.Diagramming.Core.CanvasBackgroundTypes.Gradient;
	            graphControl.Dock = System.Windows.Forms.DockStyle.Fill;
	            graphControl.EnableAddConnection = false;
	           	Netron.Diagramming.Core.Layout.LayoutSettings.TreeLayout.TreeOrientation = TreeOrientation.TopBottom;
	            graphControl.Location = new System.Drawing.Point(0, 0);
	            graphControl.Magnification = new System.Drawing.SizeF(100F, 100F);
	            graphControl.Name = "diagramControl";
	            graphControl.Size = new Size(500,500);
	//            Point p = new Point(-1200,0);
	//			graphControl.Origin = p;
	            graphControl.Origin = new System.Drawing.Point(0, 0);
	            graphControl.ShowPage = false;
	            graphControl.ShowRulers = false;
	            graphControl.TabIndex = 0;
	            graphControl.Text = "Conversation Graph";
	            
	            graphControl.Click += delegate { 
	            	Point p = new Point(graphControl.Location.X - 10, graphControl.Location.Y);
	            	graphControl.Origin = p;
	            	graphControl.Invalidate();
	            };
	            
	            this.Controls.Add(this.graphControl);
	            ((System.ComponentModel.ISupportInitialize)(graphControl)).EndInit();
	            ResumeLayout(false);
			}
			catch (Exception e) {
				Say.Error(e);
			}
		}
		
	
		public void Open(List<Page> pages)
		{
			if (pages == null || pages.Count == 0) {
				return;
			}
			
			Say.Debug("Creating a graph with " + pages.Count + " pages.");
			
			Clear();
			
            ((System.ComponentModel.ISupportInitialize)(this.graphControl)).BeginInit();
            SuspendLayout();	
            
            Node root = new Node(pages[0],null);
            graphControl.AddShape(root);
            graphControl.Invalidate(); // in case this is the only node we add
            graphControl.SetLayoutRoot(root);
            if (pages.Count > 1) {
            	DrawChildren(pages[0].Children,root);
            }
            
            graphControl.Layout(LayoutType.ClassicTree);
//            root.Move(new Point(root.X,root.Y+50));
            graphControl.Invalidate();
            
            ((System.ComponentModel.ISupportInitialize)(graphControl)).EndInit();
            ResumeLayout(false);
//            root.Move(new Point(root.X+70,root.Y));
            graphControl.Invalidate();
            Say.Debug("Finished creating graph.");
		}
		
		
		private void DrawChildren(List<Page> children, Node parentNode)
		{
			if (children == null || children.Count == 0) {
				return;
			}
			
			foreach (Page child in children) {
				if (child.LeadInLine == null) {
					throw new ArgumentException("Found another root.");
				}
				
	            Node childNode = new Node(child,parentNode);         	
	            graphControl.AddShape(childNode);	
	            IConnection connection = graphControl.AddConnection(parentNode.Connectors[2],childNode.Connectors[0]);	
	            childNode.ParentEdge = connection;
	            
	            DrawChildren(child.Children,childNode);
			}
		}
		
		public void Clear()
		{
			Selection.Clear();
			graphControl.Controller.Model.Clear();
			graphControl.Invalidate();
		}
		
		internal void SelectNode(Node node)
		{
			if (node != null) {
				node.IsSelected = true;
				node.Hovered = true;
				node.Invalidate();
			}
			else {
				Say.Error("Tried to select a null node.");
			}
		}
		
		internal Node GetNode(Page page)
		{
			foreach (IDiagramEntity entity in graphControl.Controller.Model.Paintables) {
				Say.Debug("Checking entity " + entity.ToString() + ".");
				Node pageNode = entity as Node;
				if (pageNode == null) {
					Say.Debug("Wasn't a page node.");
				}
				else {
					Say.Debug("Was a page node.");
				}
				if (pageNode != null && pageNode.Page == page) {
					Say.Debug("Found the page node I was looking for.");
					return pageNode;
				}
			}
			return null;
		}
	}
}
