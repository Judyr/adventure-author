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
using AdventureAuthor.Core;
using AdventureAuthor.Utils;
using Orbifold.Unfold.ShapeLibrary;
using Orbifold.Unfold.Core;

namespace AdventureAuthor.UI.Windows
{
    /// <summary>
    /// Interaction logic for GraphWindow.xaml
    /// </summary>

    public partial class GraphWindow : UserControl
    {    	
        public GraphWindow()
        {
            InitializeComponent(); 
            
           // Orbifold.Unfold.Core.
            
           // Orbifold.Unfold.Core.
           
           SimpleEllipse ellipse = new SimpleEllipse();
           ellipse.Text = "my ellipse";
           ellipse.ShapeColor = Colors.ForestGreen;
           DefaultConnection connection = new DefaultConnection();
           connection.EndPoint = new Point(60,60);
                     
           ellipse.BindConnection(connection,Connection.StartProperty, ConnectorLocation.Bottom);
     	   
           
           SimpleRectangle rectangle = new SimpleRectangle();
           rectangle.Text = "my rect";
           rectangle.ShapeColor = Colors.SeaGreen;
           rectangle.BindConnection(connection,Connection.EndProperty, ConnectorLocation.Top);
           myCanvas.Children.Add(ellipse);
           myCanvas.Children.Add(rectangle);
           myCanvas.Children.Add(connection);
        }

        public void RefreshGraph()
        {
        	// Recreate graph
        	RefreshSelectedNode();
        }
        
        public void RefreshSelectedNode()
        {
        	ConversationPage page = ConversationWriterWindow.Instance.CurrentPage;
        	
        	if (page == null) {
        		return;
        	}
        	
        	try {
//	        	Node node = viewer.Graph.FindNode(ConversationWriterWindow.Instance.Pages.IndexOf(page).ToString());   	    		
//			    if (currentNode != null) {
//			    	Unhighlight(currentNode);	    			
//			    }		     	
//				Highlight(node);	
//				currentNode = node;
//				viewer.Graph.FixAttributes();
//				viewer.Refresh();
        	}
        	catch (NullReferenceException e) {
        		Say.Error("Couldn't find a node to highlight in this graph.",e);
        	}
        }
                
		private void Highlight()
		{			
//			foreach (Edge e in node.InEdges) {
//				HighlightAsOnRoute(e.SourceNode);
//			}
		}
    
		private void HighlightAsOnRoute()
		{
//			foreach (Edge e in node.InEdges) {
//				HighlightAsOnRoute(e.SourceNode);
//			}			
		}
		
		private void Unhighlight()
		{
//		    foreach (Edge e in node.InEdges) {
//		    	Unhighlight(e.SourceNode);
//		    }
		}
		
		public static ConversationPage GetPageForNode()
		{
			try {
				return null;// ConversationWriterWindow.Instance.Pages[int.Parse(node.Id)];
			}
			catch (NullReferenceException) {
				return null;
			}
		}
		
		private void CreateGraph(List<ConversationPage> pages)
		{
			//Graph graph = new Graph("Conversation Tree");
			
//			if (pages != null) {			
//				for (int i = 0; i < pages.Count; i++) {
//					CPage page = pages[i];
//					Node node = graph.FindNode(i.ToString());
//					if (node == null) {
//						node = graph.AddNode(i.ToString());
//						node.Attr.AddStyle(Microsoft.Glee.Drawing.Style.Filled);
//						node.Attr.Fontsize = 20;
//					}
//					foreach (CPage child in page.Children) {
//						Node node2 = graph.AddNode(pages.IndexOf(child).ToString());
//						node2.Attr.AddStyle(Microsoft.Glee.Drawing.Style.Filled);
//						node2.Attr.Fontsize = 20;
//						graph.AddEdge(i.ToString(),pages.IndexOf(child).ToString());
//					}
//				}
//			}
			
			//return graph;
		}
    }
}
