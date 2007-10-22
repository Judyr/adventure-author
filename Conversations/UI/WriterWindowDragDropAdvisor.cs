///*
// *   This file is part of Adventure Author.
// *
// *   Adventure Author is copyright Heriot-Watt University 2006-2007.
// *
// *   This copyright and licence apply to all source code, compiled code,
// *   documentation, graphics and auxiliary files, except where otherwise stated.
// *
// *   Adventure Author is free software; you can redistribute it and/or modify
// *   it under the terms of the GNU General Public License as published by
// *   the Free Software Foundation; either version 2 of the License, or
// *   (at your option) any later version.
// *
// *   Adventure Author is distributed in the hope that it will be useful,
// *   but WITHOUT ANY WARRANTY; without even the implied warranty of
// *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// *   GNU General Public License for more details.
// * 
// *   Adventure Author is a plugin for Atari's Neverwinter Nights 2, a COMMERCIAL
// *   product. Permission is given to link this GPL-covered plug-in with the 
// *   non-free main program. 
// *
// *   You should have received a copy of the GNU General Public License
// *   along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */
//
//using System;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Markup;
//using System.Windows.Media;
//using System.Windows.Shapes;
//using System.Xml;
//using System.IO;
//using Samples.DragDrop;
//using NWN2Toolset.NWN2.Data.ConversationData;
//using AdventureAuthor.Conversations.UI.Controls;
//using AdventureAuthor.Utils;
//
//namespace AdventureAuthor.Conversations.UI
//{
//	/// <summary>
//	/// Description of WriterWindowDragDropAdvisor.
//	/// </summary>
//	public class WriterWindowDragDropAdvisor : IDragSourceAdvisor, IDropTargetAdvisor
//	{		
//		#region IDropSourceAdvisor members
//		
//		private UIElement _sourceAndTargetElt;		
//		UIElement IDragSourceAdvisor.SourceUI {
//			get {
//				return _sourceAndTargetElt;
//			}
//			set {
//				_sourceAndTargetElt = value;
//			}
//		}
//		
//		DragDropEffects IDragSourceAdvisor.SupportedEffects {
//			get {
//				return DragDropEffects.Move; 
//				// is this the list of effects you want to happen - and you're telling the drop target about them?
//			}
//		}
//		
//		DataObject IDragSourceAdvisor.GetDataObject(UIElement draggedElt, Point offsetPoint)
//		{
//			Say.Debug("GetDataObject()");
//			LineControl lineControl = draggedElt as LineControl;
//			if (lineControl != null) {
//				string serializedElt = XamlWriter.Save(draggedElt);		
//				
//				DataObject dataObject = new DataObject();
//				dataObject.SetData("XamlLineControl",serializedElt);
//				dataObject.SetData("Nwn2Line",lineControl.Nwn2Line); 
//				dataObject.SetData("point",offsetPoint);
//            
//				if (lineControl is AdventureAuthor.Conversations.UI.Controls.Line) {
//					Say.Debug("Dragged a line.");					
//				}
//				else if (lineControl is AdventureAuthor.Conversations.UI.Controls.BranchLine) {
//					Say.Debug("Dragged a branch line.");						
//				}
//				return dataObject;
//			}		
//			else {
//				return null;
//			}
//		}
//		
//		void IDragSourceAdvisor.FinishDrag(UIElement draggedElt, DragDropEffects finalEffects)
//		{
//			Say.Debug("FinishDrag()");
//			if ((finalEffects & DragDropEffects.Move) == DragDropEffects.Move)
//			{
//				WriterWindow.Instance.LinesPanel.Children.Remove(draggedElt);
//			}
//		}
//		
//		bool IDragSourceAdvisor.IsDraggable(UIElement dragElt)
//		{
//			Say.Debug("IsDraggable()");
//			bool draggable = dragElt is LineControl || dragElt is AdventureAuthor.Conversations.UI.Controls.Line || dragElt is BranchLine;
//			Say.Debug("Returned "+ draggable.ToString());
//			return draggable;
//		}
//		
//		#endregion
//		
//		#region IDropTargetAdvisor members
//		
//		UIElement IDropTargetAdvisor.TargetUI {
//			get {
//				return _sourceAndTargetElt;
//			}
//			set {
//				_sourceAndTargetElt = value;
//			}
//		}
//		
//		bool IDropTargetAdvisor.IsValidDataObject(IDataObject obj)
//		{
//			Say.Debug("IsValidDataObject()");
//			return obj.GetDataPresent("Nwn2Line");
//		}
//		
//		void IDropTargetAdvisor.OnDropCompleted(IDataObject obj, Point dropPoint)
//		{
//			Say.Debug("OnDropCompleted()");
//			StackPanel stackPanel = _sourceAndTargetElt as StackPanel;
//			
//			UIElement elt = ExtractElement(obj);
//			stackPanel.Children.Add(elt);
//		}
//
//		
//		UIElement IDropTargetAdvisor.GetVisualFeedback(IDataObject obj)
//		{
//			Say.Debug("GetVisualFeedback()");
//            UIElement elt = ExtractElement(obj);
//
//			Type t = elt.GetType();
//			
//			Rectangle rect = new Rectangle();
//			rect.Width = (double)t.GetProperty("Width").GetValue(elt, null);
//			rect.Height = (double)t.GetProperty("Height").GetValue(elt, null);
//			rect.Fill = new VisualBrush(elt);
//			rect.Opacity = 0.5;
//			rect.IsHitTestVisible = false;
//
//			return rect;
//		}
//
//        public Point GetOffsetPoint(IDataObject obj)
//        {
//			Say.Debug("GetOffsetPoint()");
//            Point p = (Point)obj.GetData("point");
//            return p;
//        }
//
//		#endregion
//		
//		private UIElement ExtractElement(IDataObject obj)
//		{            
//			Say.Debug("ExtractElement()");
//			string xamlString = obj.GetData("XamlLineControl") as string;
//			XmlReader reader = XmlReader.Create(new StringReader(xamlString));
//			UIElement elt = XamlReader.Load(reader) as UIElement;			
//			return elt;
//		}
//	}
//}
