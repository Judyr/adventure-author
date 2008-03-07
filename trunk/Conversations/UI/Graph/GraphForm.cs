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
		
		
		private bool standAlone;		
		public bool StandAlone {
			get { return standAlone; }
		}
		
		
		/// <summary>
		/// Create a new instance of a form that holds a graph control.
		/// </summary>
		/// <param name="standAlone">True if this form has no parent and is intended
		/// for use as a standalone form; false otherwise. 
		/// <remarks>It is necessary to set standAlone to true in order to
		/// add this form to a WindowsFormsHost</remarks>
		public GraphForm(bool standAlone)
		{
			this.standAlone = standAlone;
			this.TopLevel = standAlone;
			InitializeComponent();
			this.SetStyle(ControlStyles.AllPaintingInWmPaint |
						  ControlStyles.UserPaint |
						  ControlStyles.DoubleBuffer,true);
			
	        if (standAlone) {								
				// open in kiosk mode (takes up the entire screen):
				closeGraphFormButton.Visible = true;
				MaximizeBox = false;
				MinimizeBox = false;
				TopMost = true;
				FormBorderStyle = System.Windows.Forms.FormBorderStyle.None; 
				WindowState = System.Windows.Forms.FormWindowState.Maximized;		
				MinimumSize = new Size(4000,4000);
			}
			this.SizeGripStyle = SizeGripStyle.Hide;
			
			ClearGraph(); 
			
			// Close the expanded graph if you hit the escape
			// key (the Exit button initially has focus):
	        if (standAlone) {
				closeGraphFormButton.PreviewKeyDown += delegate(object sender, PreviewKeyDownEventArgs e) 
				{  
					if (e.KeyCode == Keys.Escape) {
						Close();
					}
				};
	        }
		}
		
		
		/// <summary>
		/// Create a new instance of a form that holds a graph control. 
		/// </summary>
		/// <remarks>Intended for use in XAML (setting TopLevel to false allows 
		/// it to be hosted within a WindowsFormsHost) within a WindowsFormsHost
		/// (TopLevel is set to false).</remarks>
		public GraphForm() : this(false)
		{			
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
	        
	        if (standAlone) {
	        	int height = Height + GraphControl.RULER_OFFSET + 80; // account for missing title bar
	        		//SystemInformation.CaptionHeight + SystemInformation.BorderSize.Height; // doesn't seem right?
	        	int width = Width + GraphControl.RULER_OFFSET + SystemInformation.BorderSize.Width;
	        	graphControl.MinimumSize = new Size(width,height);
	        }
	        else {
	        	graphControl.MinimumSize = new Size(Width + GraphControl.RULER_OFFSET,
	        	                                    Height + GraphControl.RULER_OFFSET);
	        }
	        
	        graphControl.Origin = origin;
	        graphControl.Magnification = magnification;
	        
	        if (standAlone) { // close the expanded graph if you hit the escape key
				graphControl.KeyDown += delegate(object sender, KeyEventArgs e) 
				{  
					if (e.KeyCode == Keys.Escape) {
						Close();
					}
				};
	        }
	            
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
				if (child.LeadLine == null) {
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
				Log.WriteAction(Log.Action.exited,"expandedgraph");
			}
			base.OnClosed(e);
		}
		
		
		private void OnClick_CloseGraphForm(object sender, EventArgs e)
		{
			if (standAlone) { // this should never get called in stand-alone mode anyway, but just for safety
				Close();
			}
		}
	}
}
