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
using AdventureAuthor.Conversations;
using AdventureAuthor.Core;
using AdventureAuthor.Utils;
using Netron.Diagramming.Core;
using Netron.Diagramming.Core.Layout;
using Netron.Diagramming.Win.AdventureAuthor;

namespace AdventureAuthor.Conversations.UI.Graph
{
	public partial class GraphForm : System.Windows.Forms.Form
	{
		private GraphControl graphControl;		
		public GraphControl GraphControl {
			get { return graphControl; }
		}
		
		
		public GraphForm(bool topLevel)
		{
			try {
				TopLevel = topLevel; // allows it to be added to a WindowsFormHost
				InitializeComponent();
				
	            graphControl = new GraphControl();
	            ((System.ComponentModel.ISupportInitialize)(this.graphControl)).BeginInit();
	            SuspendLayout();			
				
	            if (topLevel) { // TODO this is getting overriden by the later assignment, do it properly after the Clear thing
	            	graphControl.Size = new Size(800,600);
	            }
	            else {
	            	graphControl.Size = new Size(400,400);
	            }
	            
	            graphControl.Controller.AddTool(new GraphTool("Graph Tool"));
	            graphControl.AllowDrop = false;
	            graphControl.AutoScroll = true;
	            // can only set colours on CanvasBackgroundTypes.FlatColour - could use an image however:
	            graphControl.BackgroundType = CanvasBackgroundTypes.Gradient; 
	            graphControl.Dock = System.Windows.Forms.DockStyle.None;
	            graphControl.EnableAddConnection = false;
	           	LayoutSettings.TreeLayout.TreeOrientation = TreeOrientation.TopBottom;
	            graphControl.Location = new System.Drawing.Point(0, 0);
	            graphControl.Magnification = new System.Drawing.SizeF(100F, 100F);
	            graphControl.Name = "diagramControl";
	            graphControl.Size = this.Size;
	            
//	            if (topLevel) {
//		            this.MaximumSize = new Size(1200,900);
//		            this.MinimumSize = new Size(1200,900);
//	            }
//	            else {
//		            this.MaximumSize = new Size(400,400);
//		            this.MinimumSize = new Size(400,400);
//	            }
	            
	            this.Controls.Add(this.graphControl);
	            
	            ((System.ComponentModel.ISupportInitialize)(graphControl)).EndInit();
	            ResumeLayout(false);
			}
			catch (Exception e) {
				Say.Error(e);
			}
		}
		
		
		public void Clear() // a test to see if I can just recreate the entire GraphControl everywhere I'm calling GraphControl.Clear()
		{
			graphControl.Dispose();
	            graphControl = new GraphControl();
	            ((System.ComponentModel.ISupportInitialize)(this.graphControl)).BeginInit();
	            SuspendLayout();			
				
	            if (this.Parent == null) { // TODO this is getting overriden by the later assignment, do it properly after the Clear thing
	            	graphControl.Size = new Size(800,600);
	            }
	            else {
	            	graphControl.Size = new Size(400,400);
	            }
	            
	            graphControl.Controller.AddTool(new GraphTool("Graph Tool"));
	            graphControl.AllowDrop = false;
	            graphControl.AutoScroll = true;
	            // can only set colours on CanvasBackgroundTypes.FlatColour - could use an image however:
	            graphControl.BackgroundType = CanvasBackgroundTypes.Gradient; 
	            graphControl.Dock = System.Windows.Forms.DockStyle.None;
	            graphControl.EnableAddConnection = false;
	           	LayoutSettings.TreeLayout.TreeOrientation = TreeOrientation.TopBottom;
	            graphControl.Location = new System.Drawing.Point(0, 0);
	            graphControl.Magnification = new System.Drawing.SizeF(100F, 100F);
	            graphControl.Name = "diagramControl";
	            graphControl.Size = this.Size;
	            
//	            if (topLevel) {
//		            this.MaximumSize = new Size(1200,900);
//		            this.MinimumSize = new Size(1200,900);
//	            }
//	            else {
//		            this.MaximumSize = new Size(400,400);
//		            this.MinimumSize = new Size(400,400);
//	            }
	            
	            this.Controls.Add(this.graphControl);
	            
	            ((System.ComponentModel.ISupportInitialize)(graphControl)).EndInit();
	            ResumeLayout(false);
			
		}
		
	
		public void Open(List<Page> pages)
		{
			if (pages == null || pages.Count == 0) {
				Say.Error("Tried to open a null page collection, or page collection was empty.");
				return;
			}
			
			this.Clear();
//			graphControl.Clear();
//			graphControl.Controller.AddTool(new GraphTool("Graph Tool"));
			
            ((System.ComponentModel.ISupportInitialize)(this.graphControl)).BeginInit();
            SuspendLayout();	
            
            Node root = new Node(pages[0],null);
            graphControl.AddShape(root);
            
            // inexplicably required to get the root drawn if that's all there is:
            graphControl.AddConnection(root.Connectors[0],root.Connectors[0]); 
            graphControl.SetLayoutRoot(root);
            DrawChildren(pages[0].Children,root);            
            graphControl.Layout(LayoutType.ClassicTree);
                        
            ((System.ComponentModel.ISupportInitialize)(graphControl)).EndInit();
            ResumeLayout(false);
            graphControl.CentreOnShape(root);
            graphControl.Invalidate();
		}
		
		
		private void DrawChildren(List<Page> children, Node parentNode)
		{
			if (children == null) {
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
		
		
		internal Node GetNode(Page page)
		{
			foreach (IDiagramEntity entity in graphControl.Controller.Model.Paintables) {
				Node pageNode = entity as Node;
				if (pageNode != null && pageNode.Page == page) {
					return pageNode;
				}
			}
			return null;
		}
	}
}
