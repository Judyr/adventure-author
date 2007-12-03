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
using Netron.Diagramming.Win;
using Netron.Diagramming.Win.AdventureAuthor;

namespace AdventureAuthor.Notebook
{
	public partial class MindMapForm : System.Windows.Forms.Form
	{
		//private MindMapControl diagramControl;
		private DiagramControl diagramControl;
		
		/// <summary>
		/// Create a new instance of a form that holds a graph control.
		/// </summary>
		/// <param name="topLevel">True if this form has no parent; false otherwise. Necessary to allow it to be added to a WindowsFormHost.</param>
		public MindMapForm(bool topLevel)
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
				Open(currentlayout);
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
			if (diagramControl != null) {
				origin = diagramControl.Origin;
				magnification = diagramControl.Magnification;
				diagramControl.Dispose();
			}
			else {
				origin = new Point(0,0);
				magnification = new SizeF(100F,100F);
			}
			
			// Create a new GraphControl object:	
			diagramControl = new DiagramControl();//MindMapControl();
				
	        diagramControl.Dock = System.Windows.Forms.DockStyle.None;
	        diagramControl.Location = new System.Drawing.Point(-GraphControl.RULER_OFFSET,-GraphControl.RULER_OFFSET); // hide the ruler space
	        diagramControl.Size = new Size(this.Width+GraphControl.RULER_OFFSET,this.Height+GraphControl.RULER_OFFSET);
	        
	        diagramControl.Origin = origin;
	        diagramControl.Magnification = magnification;
	        
	        this.Controls.Add(this.diagramControl);
			
			
			
//			// If a GraphControl object already exists, remember its origin and magnification:
//			Point origin;
//			SizeF magnification;
//			if (graphControl != null) {
//				origin = graphControl.Origin;
//				magnification = graphControl.Magnification;
//				graphControl.Dispose();
//			}
//			else {
//				origin = new Point(0,0);
//				magnification = new SizeF(100F,100F);
//			}
//			
//			// Create a new GraphControl object:	
//	        graphControl = new GraphControl();
//				
//	        graphControl.Controller.AddTool(new AdventureAuthor.Conversations.UI.Graph.GraphTool("Graph Tool")); // the tool which governs clicks on the graph
//	        graphControl.Dock = System.Windows.Forms.DockStyle.None;
//	        graphControl.Location = new System.Drawing.Point(-GraphControl.RULER_OFFSET,-GraphControl.RULER_OFFSET); // hide the ruler space
//	        graphControl.Size = new Size(this.Width+GraphControl.RULER_OFFSET,this.Height+GraphControl.RULER_OFFSET);
//	        
//	        graphControl.Origin = origin;
//	        graphControl.Magnification = magnification;
//	        graphControl.MIN_ZOOM = 1;
//	        
//	        this.Controls.Add(this.graphControl);
		}
		
	
		/// <summary>
		/// Open a conversation as a conversation tree.
		/// </summary>
		/// <param name="pages">A list of pages of the conversation; there must be at least one page in the collection</param>
		public void Open(LayoutType layout)
		{
			total = 0;
			
			this.ClearGraph();
			
            ((System.ComponentModel.ISupportInitialize)(this.diagramControl)).BeginInit();
            SuspendLayout();	
            
            rootshape = new SimpleEllipse();
            diagramControl.AddShape(rootshape);
            
            // inexplicably required to get the root drawn if that's all there is:
            diagramControl.AddConnection(rootshape.Connectors[0],rootshape.Connectors[0]); 
            diagramControl.SetLayoutRoot(rootshape);
            DrawChildren(rootshape);
            diagramControl.Layout(layout);
                        
            ((System.ComponentModel.ISupportInitialize)(diagramControl)).EndInit();
            ResumeLayout(false);
            diagramControl.Invalidate();
		}
		
		private static SimpleEllipse rootshape = null;
		
		
		private void SetLayout(LayoutType layout)
		{
			SuspendLayout();
			diagramControl.Layout(layout);			
			ResumeLayout();
			if (rootshape != null) {
				//diagramControl.CentreOnShape(rootshape);
			}
			diagramControl.Invalidate();
			layoutlabel.Text = layout.ToString();
		}
		
		
		private static int total = 0;
		
		
		/// <summary>
		/// Recursively draw the children of this node, and edges to connect them to the parent node.
		/// </summary>
		/// <param name="children">A list of pages that are the children of the page represented by parentNode</param>
		/// <param name="parentNode">The node to draw children for</param>
		private void DrawChildren(SimpleEllipse root)
		{
	        if (total > 16) {
	          	return;
	        }
			
			Random random = new Random();
			int randomNumber = random.Next(0,4);
			
			for (int i = 0; i < randomNumber; i++) {
				SimpleEllipse newshape = new SimpleEllipse();
				diagramControl.AddShape(newshape);
				IConnection connection = diagramControl.AddConnection(root.Connectors[2],newshape.Connectors[0]);	
	            diagramControl.Controller.Model.SendToBack(connection);
	            total++;
	            DrawChildren(newshape);
			}
		}
		
//		
//		/// <summary>
//		/// Get the node in this graph that represents the given page.
//		/// </summary>
//		/// <param name="page">The page that the returned node represents</param>
//		/// <returns>A node that represents the given page if one exists; null otherwise</returns>
//		internal Node GetNode(Page page)
//		{
//			foreach (IDiagramEntity entity in graphControl.Controller.Model.Paintables) {
//				Node pageNode = entity as Node;
//				if (pageNode != null && pageNode.Page == page) {
//					return pageNode;
//				}
//			}
//			return null;
//		}
		
		
		protected override void OnClosed(EventArgs e)
		{
//			if (TopLevel) {
//				Log.WriteAction(Log.Action.exited,"expandedgraph");
//			}
			base.OnClosed(e);
		}
		
		private static LayoutType currentlayout = LayoutType.Balloon;
		
		void Button1Click(object sender, EventArgs e)
		{
			switch (currentlayout) {
				case LayoutType.Balloon:
					currentlayout = LayoutType.ClassicTree;
					break;
				case LayoutType.ClassicTree:
					currentlayout = LayoutType.ForceDirected;
					break;
				case LayoutType.ForceDirected:
					currentlayout = LayoutType.FruchtermanRheingold;
					break;
				case LayoutType.FruchtermanRheingold:
					currentlayout = LayoutType.RadialTree;
					break;
				case LayoutType.RadialTree:
					currentlayout = LayoutType.Balloon;
					break;
				default:
					currentlayout = LayoutType.ClassicTree;
					break;
			}
			
			SetLayout(currentlayout);
		}
		
		void Button2Click(object sender, EventArgs e)
		{
			SuspendLayout();
			SimpleEllipse newshape = new SimpleEllipse();
			newshape.Text = "Imogen Heap";
			diagramControl.AddShape(newshape);
			total++;
			
			Random rnd = new Random();
			int shapeid = rnd.Next(0,Math.Max(total-1,0));
			IDiagramEntity entity = diagramControl.Controller.Model.Paintables[shapeid];
			IShape oldshape = entity as IShape;
			if (oldshape == null) {
				Say.Error("couldn't cast to IShape");
				return;
			}
			
			IConnection connection = diagramControl.AddConnection(oldshape.Connectors[0],newshape.Connectors[0]);
			
			ResumeLayout();
			diagramControl.Invalidate();
			

		}
	}
}









