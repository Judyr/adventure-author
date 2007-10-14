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
using Netron.Diagramming.Core.Layout;
using Netron.Diagramming.Win.AdventureAuthor;

namespace AdventureAuthor.Conversations.UI.Graph
{
	public partial class GraphForm : System.Windows.Forms.Form
	{
		/// <summary>
		/// The graph control held by this form.
		/// </summary>
		private GraphControl graphControl;		
		public GraphControl GraphControl {
			get { return graphControl; }
		}
		
		/// <summary>
		/// Create a new instance of a form that holds a graph control.
		/// </summary>
		/// <param name="topLevel">True if this form has no parent; false otherwise. Necessary to allow it to be added to a WindowsFormHost.</param>
		public GraphForm(bool topLevel)
		{
			try {
				this.TopLevel = topLevel;
				InitializeComponent();
				this.SetStyle(ControlStyles.AllPaintingInWmPaint |
							  ControlStyles.UserPaint |
							  ControlStyles.DoubleBuffer,true);
				
	            if (topLevel) {
		            this.MaximumSize = new Size(1200,900);
				}
				else {
					this.MaximumSize = new Size(400,420);
	            }
		        this.MinimumSize = this.MaximumSize;
				this.SizeGripStyle = SizeGripStyle.Hide;
				
				ClearGraph();
			}
			catch (Exception e) {
				Say.Error(e);
			}
		}
		
		
		/// <summary>
		/// Create a new instance of the graph, effectively clearing the graph display.
		/// </summary>
		public void ClearGraph()
		{
			// If a GraphControl object already exists, remember its origin and magnification:
			Point origin;
			SizeF magnification;
			if (graphControl != null) {
				origin = graphControl.Origin;
				magnification = graphControl.Magnification;
				graphControl.Dispose();
			}
			else {
				origin = new Point(0,0);
				magnification = new SizeF(100F,100F);
			}
			
			// Create a new GraphControl object:	
	        graphControl = new GraphControl();
				
	        graphControl.Controller.AddTool(new GraphTool("Graph Tool")); // the tool which governs clicks on the graph
	        graphControl.Dock = System.Windows.Forms.DockStyle.None;
	        graphControl.Location = new System.Drawing.Point(-GraphControl.RULER_OFFSET,-GraphControl.RULER_OFFSET); // hide the ruler space
	        graphControl.Size = new Size(this.Width+GraphControl.RULER_OFFSET,this.Height+GraphControl.RULER_OFFSET);
	        
	        graphControl.Origin = origin;
	        graphControl.Magnification = magnification;
	            
	        this.Controls.Add(this.graphControl);
		}
		
	
		/// <summary>
		/// Open a conversation as a conversation tree.
		/// </summary>
		/// <param name="pages">A list of pages of the conversation; there must be at least one page in the collection</param>
		public void Open(List<Page> pages)
		{
			if (pages == null || pages.Count == 0) {
				Say.Error("Tried to open a null page collection, or page collection was empty.");
				return;
			}
			
			this.ClearGraph();
			
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
            graphControl.Invalidate();
		}
		
		
		/// <summary>
		/// Recursively draw the children of this node, and edges to connect them to the parent node.
		/// </summary>
		/// <param name="children">A list of pages that are the children of the page represented by parentNode</param>
		/// <param name="parentNode">The node to draw children for</param>
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
	            graphControl.Controller.Model.SendToBack(connection);
	            childNode.ParentEdge = connection;	            
	            
	            DrawChildren(child.Children,childNode);
			}
		}
		
		
		/// <summary>
		/// Get the node in this graph that represents the given page.
		/// </summary>
		/// <param name="page">The page that the returned node represents</param>
		/// <returns>A node that represents the given page if one exists; null otherwise</returns>
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
		
		
		protected override void OnClosed(EventArgs e)
		{
			if (TopLevel) {
				Log.WriteEffectiveAction(Log.EffectiveAction.exited,"expandedgraph");
			}
			base.OnClosed(e);
		}
	}
}
