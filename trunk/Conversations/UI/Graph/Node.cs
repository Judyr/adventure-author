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
using AdventureAuthor.Utils;
using Netron.Diagramming.Core;
using Netron.Diagramming.Core.AdventureAuthor;
using Netron.Diagramming.Win.AdventureAuthor;

namespace AdventureAuthor.Conversations.UI.Graph
{
	public class Node : SimpleRectangle, IPageNode, IHoverListener
	{
		/// <summary>
		/// The page of the conversation that this node represents.
		/// </summary>
		private Page page;
		public Page Page {
			get { return page; }
		}
		
		/// <summary>
		/// The node that leads to this node - null if this is the root.
		/// </summary>
		private Node parentNode;
		
		/// <summary>
		/// The edge that connects this node and its parent node - null if this is the root.
		/// </summary>
		private IConnection parentEdge = null;
		public IConnection ParentEdge {
			get { return parentEdge; }
			set { parentEdge = value; }
		}        
		
		/// <summary>
        /// the services of this shape
        /// </summary>
        private Dictionary<Type, IInteraction> mServices;
		
		/// <summary>
		/// The text to display on a tool tip if the mouse is hovering over this node.
		/// <remarks>This is necessary because the class does not derive from Control - rather
		/// it tells the parent control what to display on its tooltip.</remarks>
		/// </summary>
		public string ToolTipText {
			get {
				if (parentNode == null) { // root
					return "Start";
				}
				else {
					return Conversation.GetStringFromOEIString(page.LeadLine.Text);
				}
			}
		}
		
		
		/// <summary>
		/// Create a new node
		/// </summary>
		/// <param name="page">The page the node represents</param>
		/// <param name="parentNode">The parent node of this node</param>
		public Node(Page page, Node parentNode) : base()
		{
			this.Resizable = false;
			this.PaintStyle = GraphControl.PAINT_NOT_ON_ROUTE;
			
			this.page = page;
			this.parentNode = parentNode;
			
			SetLabel();					
			
            mServices = new Dictionary<Type, IInteraction>();
            mServices[typeof(IHoverListener)] = this;
		}
		
		
		private void SetLabel()
		{
			if (this.parentNode == null) { // root
				this.Text = "Start";
			}
			else {			
				string newText = Conversation.GetStringFromOEIString(this.page.LeadLine.Text);
				if (newText.Length == 0) {
					this.Text = "...";
				}
				else {
					string shorttext = Tools.Truncate(newText,30);
					if (shorttext.Length < newText.Length) {
						this.Text = shorttext + "...";
					}
					else {
						this.Text = newText;
					}
				}	
			}
		}
		
		
		/// <summary>
		/// Get the route between the selected node and the root, in the form of a list of nodes and edges.
		/// </summary>
		/// <returns>A list of INode objects representing the route between the selected node and the root. Empty if called on root.</returns>
		public List<IDiagramEntity> GetRoute()
		{
			if (parentNode != null && parentEdge == null) {
				throw new ArgumentException("No information was provided about the edge that links the node to its parent node.");
			}
			
			List<IDiagramEntity> route = new List<IDiagramEntity>();			
			
			Node node = this;	
			
			while (node != null) {
				route.Add(node);
				if (node.parentEdge != null) {
					route.Add(node.parentEdge);
				}
				node = node.parentNode;
			}
			
			return route;
		}
		
		void IHoverListener.MouseHover(MouseEventArgs e)
		{
			Say.Debug("Hovered: " + this.Page.ToString());
		}
		
		void IHoverListener.MouseEnter(MouseEventArgs e)
		{
			Say.Debug("Entered: " + this.Page.ToString());
		}
		
		void IHoverListener.MouseLeave(MouseEventArgs e)
		{
			Say.Debug("Left: " + this.Page.ToString());
		}
	}
}
