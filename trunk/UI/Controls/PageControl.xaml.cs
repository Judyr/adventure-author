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
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NWN2Toolset.NWN2.Data.ConversationData;
using AdventureAuthor.UI.Controls;

namespace AdventureAuthor.UI.Controls
{
    /// <summary>
    /// Interaction logic for PageControl.xaml
    /// </summary>

    public partial class PageControl : Button
    {
    	#region Fields
    	
    	private static DependencyProperty isSelectedProperty = DependencyProperty.Register("IsSelected",typeof(bool),typeof(PageControl));
    	public bool IsSelected {
    		get { return (bool)GetValue(isSelectedProperty); }
    		set { SetValue(isSelectedProperty, value); }
    	}
    	
    	private static DependencyProperty isPartOfPathProperty = DependencyProperty.Register("IsPartOfPath",typeof(bool),typeof(PageControl));
    	public bool IsPartOfPath {
    		get { return (bool)GetValue(isPartOfPathProperty); }
    		set { SetValue(isPartOfPathProperty, value); }
    	}
    	
    	private PageControl parentPage;		
		public PageControl ParentPage {
			get { return parentPage; }
		}		
		
		private NWN2ConversationConnector leadInLine; // the line in the parent Page that leads to this Page
		public NWN2ConversationConnector LeadInLine {
			get { return leadInLine; }
		}
		
		private List<PageControl> children;		
		public List<PageControl> Children {
			get { return children; }
		}
		
		private int? numberOfEndNodes;		
		public Nullable<int> NumberOfEndNodes {
			get { return numberOfEndNodes; }
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
    	
		public PageControl(NWN2ConversationConnector leadsFrom, PageControl parent)
		{
			InitializeComponent();
			this.leadInLine = leadsFrom;
			this.parentPage = parent;
			this.children = new List<PageControl>();
			this.numberOfEndNodes = null;
			this.lineControls = new List<LineControl>();
			this.finalControl = null;
			Canvas.SetZIndex(this,2);
		}
		
		public int CalculateNumberOfEndNodes()
		{
			if (numberOfEndNodes != null) {
				return (int)numberOfEndNodes; // already calculated, so just return the value
			}		
			else {
				int count = 0;		
				foreach (PageControl child in children) {
					if (child.IsEndNode()) {
						count++;
					}
					else {
						count+= child.CalculateNumberOfEndNodes();
					}
				}				
				numberOfEndNodes = (int?)count;
				return (int)numberOfEndNodes;
			}
		}
		
		public bool IsEndNode()
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
