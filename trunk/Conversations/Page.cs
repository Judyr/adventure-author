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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using NWN2Toolset.NWN2.Data.ConversationData;
using AdventureAuthor.Conversations.UI.Controls;

namespace AdventureAuthor.Conversations
{
    public partial class Page
    {
    	#region Fields
    	
    	private bool isSelected; 
		public bool IsSelected {
			get { return isSelected; }
			set { isSelected = value; }
		}
    	
    	private bool isInRoute;    	
		public bool IsInRoute {
			get { return isInRoute; }
			set { isInRoute = value; }
		}
    	
    	private Page parentPage;		
		public Page ParentPage {
			get { return parentPage; }
		}		
		
		private List<Page> children;		
		public List<Page> Children {
			get { return children; }
		}
		    	    	
		private NWN2ConversationConnector leadInLine; // the line in the parent Page that leads to this Page
		public NWN2ConversationConnector LeadInLine {
			get { return leadInLine; }
		}
		
    	private List<LineControl> lineControls;  	
		public List<LineControl> LineControls {
			get { return lineControls; }
		}
    	
    	#endregion Fields
    	
		public Page(NWN2ConversationConnector leadsFrom, Page parent)
		{
			this.leadInLine = leadsFrom;
			this.parentPage = parent;
			this.children = new List<Page>();
			this.lineControls = new List<LineControl>();
			if (parent != null) {
				parent.children.Add(this);
			}			
		}	

		public bool IsEndPage()
		{
			return children.Count == 0;
		}
		
		public override string ToString()
		{
			if (leadInLine == null) {
				return "Root";
			}
			else if (leadInLine.Line.Text.Strings.Count == 0) {
				return "[Continue]";
			}
			else {
				return leadInLine.Line.Text.Strings[0].Value;
			}
		}
    }
}
