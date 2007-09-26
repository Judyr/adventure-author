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
using Netron.Diagramming.Core;
using AdventureAuthor.Utils;
using AdventureAuthor.Conversations;
using AdventureAuthor.UI.Windows;
using AdventureAuthor.Core;
using Netron.Diagramming.Core.AdventureAuthor;

namespace AdventureAuthor.Conversations
{
	/// <summary>
	/// Description of PageNode.
	/// </summary>
	public class PageNode : SimpleRectangle, IToolTipped
	{
		/// <summary>
		/// The page of the conversation that this node represents.
		/// </summary>
		private ConversationPage page;
		public ConversationPage Page {
			get { return page; }
		}
		
		/// <summary>
		/// The node that leads to this node - null if this is the root.
		/// </summary>
		private PageNode parentNode;
		
		/// <summary>
		/// The edge that connects this node and its parent node - null if this is the root.
		/// </summary>
		private IConnection parentEdge = null;
		public IConnection ParentEdge {
			get { return parentEdge; }
			set { parentEdge = value; }
		}
		
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
					return Conversation.OEIExoLocStringToString(page.LeadInLine.Text);
				}
			}
		}
		
		public PageNode(ConversationPage page, PageNode parentNode) : base()
		{
			this.page = page;
			this.Resizable = false;
			this.parentNode = parentNode;
			
			if (parentNode == null) { // root
				this.Text = "Start";
			}
			else {
				this.Text = UsefulTools.Truncate(Conversation.OEIExoLocStringToString(page.LeadInLine.Text),30) + "...";
			}
		}
		
		/// <summary>
		/// Highlight the route to this node, up to the root of the conversation tree.
		/// <remarks>Requires an Invalidate() call on the parent form afterwards.</remarks>
		/// </summary>
		public void ShowRoute()
		{
			if (parentNode != null && parentEdge == null) {
				throw new ArgumentException("No information was provided about the edge that links the node to its parent node.");
			}
			
			if (!IsSelected) { // highlight this node unless it is selected, in which it is the route's endpoint
				
				// TODO
				
				this.PaintStyle = new GradientPaintStyle(Color.LightBlue,Color.AntiqueWhite,0.0f);
			}
			if (parentEdge != null) { // continue to highlight up the tree until you reach the root, which has no parent
				// highlight the edge:
				parentEdge.PaintStyle = new GradientPaintStyle(Color.Red,Color.Maroon,0.0f);
				
				// continue up the tree:
				try {
					parentNode.ShowRoute();
				}
				catch (NullReferenceException e) {
					Say.Error("Failed to highlight route: node '" + page.ToString() +
					          "' has a parent edge but no parent node, which is invalid.",e);
				}
			}
		}
	}
}
