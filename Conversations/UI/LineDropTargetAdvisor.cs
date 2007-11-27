using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Xml;
using Samples.DragDrop;
using AdventureAuthor.Conversations.UI.Controls;
using AdventureAuthor.Utils;
using NWN2Toolset.NWN2.Data.ConversationData;

namespace AdventureAuthor.Conversations.UI
{
	public class LineDropTargetAdvisor : IDropTargetAdvisor
	{
		private static DataFormat SupportedFormat = DataFormats.GetDataFormat("ABCFormat");
		private UIElement _targetUI;

		public bool IsValidDataObject(IDataObject obj)
		{
        	bool isValid = obj.GetDataPresent("nwn2line");
			return isValid;
		}

		
		public void OnDropCompleted(IDataObject obj, Point dropPoint)
		{
			Say.Debug("OnDropCompleted");
			NWN2ConversationConnector droppedLine = obj.GetData("nwn2line") as NWN2ConversationConnector;
			NWN2ConversationConnector targetLine = ((LineControl)TargetUI).Nwn2Line;
			bool lineOriginatedFromChoice = (bool)obj.GetData("lineOriginatedFromChoice");
						
			if (TargetUI is BranchLine) {
				if (lineOriginatedFromChoice) { // if the dragged line came from the page's choice
					Conversation.CurrentConversation.MoveLineWithinChoice(droppedLine,targetLine);
				}
				else {
					Conversation.CurrentConversation.MoveLineIntoChoice(droppedLine,targetLine.Parent);
				}
			}
			else if (!lineOriginatedFromChoice && (TargetUI is Line || TargetUI is LeadingLine)) {
				Conversation.CurrentConversation.MoveLine(droppedLine,targetLine);
			}
			else {
				throw new InvalidOperationException("This drop advisor should not be attached to anything other than a " + 
				                                    "Line, BranchLine or LeadingLine (all subclasses of LineControl). " + 
				                                    "Instead found TargetUI to be " + TargetUI.GetType().ToString() + ".");
			}
		}

		
		public UIElement TargetUI
		{
			get
			{
				return _targetUI;
			}
			set
			{
				_targetUI = value;
			}
		}
		
		
		public UIElement GetVisualFeedback(IDataObject obj)
		{
        	Say.Debug("GetVisualFeedback");
        	
        	Say.Debug(ExtractElement(obj).GetType().ToString());
        	UIElement elt = ExtractElement(obj);
			Button b = elt as Button;
			if (b != null) {
				b.Width = 100;
				b.Height = 60;
				b.Content = "HELLO LOOKIT";
				b.Opacity = 0.5;
				b.IsHitTestVisible = false;
	
				DoubleAnimation anim = new DoubleAnimation(0.75, new Duration(TimeSpan.FromMilliseconds(500)));
				anim.From = 0.25;
				anim.AutoReverse = true;
				anim.RepeatBehavior = RepeatBehavior.Forever;
				b.BeginAnimation(UIElement.OpacityProperty, anim);
				
				return elt;
			}
			else {
				throw new Exception("No visual feedback.");
			}
        	
        	
//			elt.Width = 350;
//			elt.Height = 60;
//			elt.Opacity = 0.5;
//			elt.IsHitTestVisible = false;
//			Say.Debug("plunk");
//
//			DoubleAnimation anim = new DoubleAnimation(0.75, new Duration(TimeSpan.FromMilliseconds(500)));
//			Say.Debug("plunk");
//			anim.From = 0.25;
//			anim.AutoReverse = true;
//			anim.RepeatBehavior = RepeatBehavior.Forever;
//			Say.Debug("plunk");
//			elt.BeginAnimation(UIElement.OpacityProperty, anim);
//
//			Say.Debug("Dunned GetVisualFeedback");
//			
//			return elt;
			
		}

		
        public Point GetOffsetPoint(IDataObject obj)
        {
        	Say.Debug("GetOffsetPoint");
            Point p = (Point)obj.GetData("point");
            return p;
        }

        
		private UIElement ExtractElement(IDataObject obj)
		{
			return new Button();
			
			
//        	Say.Debug("ExtractElement");
//			string xamlString = obj.GetData("ABCFormat") as string;
//			XmlReader reader = XmlReader.Create(new StringReader(xamlString));
//			UIElement elt = XamlReader.Load(reader) as UIElement;
//
//			return elt;
		}
	}
}

