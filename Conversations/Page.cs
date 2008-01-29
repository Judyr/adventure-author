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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using NWN2Toolset.NWN2.Data.ConversationData;
using AdventureAuthor.Conversations.UI;
using AdventureAuthor.Conversations.UI.Controls;
using AdventureAuthor.Conversations.UI.Graph;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Conversations
{
    public partial class Page
    {
    	#region Fields
    	
    	private Page parent;		
		public Page Parent {
			get { return parent; }
		}		
		
		private List<Page> children;		
		public List<Page> Children {
			get { return children; }
		}
		    	    	
		private NWN2ConversationConnector leadLine; // the line in the parent Page that leads to this Page
		public NWN2ConversationConnector LeadLine {
			get { return leadLine; }
		}
		
    	private List<LineControl> lineControls;  // should become List<Line>?	
		public List<LineControl> LineControls {
			get { return lineControls; }
		}  
    	
    	private LeadingLine leadLineControl;    	
		public LeadingLine LeadLineControl {
			get { return leadLineControl; }
			set { leadLineControl = value; }
		}
    	
    	public bool IsEndPage {
    		get { return children.Count == 0; }
    	}
    	
    	#endregion Fields
    	
    	
		public Page(NWN2ConversationConnector leadsFrom, Page parent)
		{
			this.leadLine = leadsFrom;
			this.parent = parent;
			this.children = new List<Page>();
			this.lineControls = new List<LineControl>();
			this.leadLineControl = null;
			if (parent != null) {
				parent.children.Add(this);
			}
		}	
		
		
		public override string ToString()
		{
			if (leadLine == null) {
				return "Root";
			}
			else if (Conversation.GetStringFromOEIString(leadLine.Line.Text).Length == 0) {
				return "[Continue]";
			}
			else {
				return Tools.Truncate(Conversation.GetStringFromOEIString(leadLine.Line.Text),30);
			}
		}
    }
}
