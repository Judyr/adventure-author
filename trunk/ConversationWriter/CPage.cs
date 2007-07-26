
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
 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using AdventureAuthor.ConversationWriter;
using AdventureAuthor.UI.Controls;
using AdventureAuthor.UI.Windows;
using NWN2Toolset.NWN2.Data.ConversationData;

namespace AdventureAuthor.ConversationWriter
{
    /// <summary>
    /// Interaction logic for CPage.xaml
    /// </summary>

    public partial class CPage
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
    	
    	private CPage parentPage;		
		public CPage ParentPage {
			get { return parentPage; }
		}		
		
		private List<CPage> children;		
		public List<CPage> Children {
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
    	
    	private UserControl finalControl;    	
		public UserControl FinalControl {
			get { return finalControl; }
			set { finalControl = value; }
		}
    	
    	#endregion Fields
    	
		public CPage(NWN2ConversationConnector leadsFrom, CPage parent)
		{
			this.leadInLine = leadsFrom;
			this.parentPage = parent;
			this.children = new List<CPage>();
			this.lineControls = new List<LineControl>();
			this.finalControl = null;
			if (parent != null) {
				parent.children.Add(this);
			}			
		}		

		public bool IsEndPage()
		{
			return children.Count == 0;
		}
		
		private void OnClickNode(object sender, EventArgs ea)
		{
			Conversation.CurrentConversation.SaveToWorkingCopy();
			ConversationWriterWindow.Instance.DisplayPage(this);
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
