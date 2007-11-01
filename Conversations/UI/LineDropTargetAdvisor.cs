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
//        	Say.Debug("IsValidDataObject");
        	bool isValid = obj.GetDataPresent("nwn2line");
//        	bool isValid = obj.GetDataPresent(SupportedFormat.Name);
			return isValid;
		}

		
		public void OnDropCompleted(IDataObject obj, Point dropPoint)
		{
			NWN2ConversationConnector line = obj.GetData("nwn2line") as NWN2ConversationConnector;
			
			if (TargetUI is BranchLine) {
				Conversation.CurrentConversation.MoveLineIntoChoice(line,((LineControl)TargetUI).Nwn2Line.Parent);
			}
			else if (TargetUI is Line || TargetUI is LeadingLine) {
				Conversation.CurrentConversation.MoveLine(line,((LineControl)TargetUI).Nwn2Line);
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
			Line l = elt as Line;
			if (b != null) {
				b.Width = 100;
				b.Height = 60;
				b.Content = "HELLO LOOKIT";
				b.Opacity = 0.5;
				b.IsHitTestVisible = false;
				Say.Debug("bplunk");
	
				DoubleAnimation anim = new DoubleAnimation(0.75, new Duration(TimeSpan.FromMilliseconds(500)));
				Say.Debug("bplunk");
				anim.From = 0.25;
				anim.AutoReverse = true;
				anim.RepeatBehavior = RepeatBehavior.Forever;
				Say.Debug("bplunk");
				b.BeginAnimation(UIElement.OpacityProperty, anim);
	
				Say.Debug("bDunned GetVisualFeedback");
				
				return elt;
			}
			else if (l != null) {
				l.Width = 350;
				l.Height = 260;
				l.Opacity = 0.5;
				l.IsHitTestVisible = false;
				Say.Debug("lplunk");
	
				DoubleAnimation anim = new DoubleAnimation(0.75, new Duration(TimeSpan.FromMilliseconds(500)));
				Say.Debug("lplunk");
				anim.From = 0.25;
				anim.AutoReverse = true;
				anim.RepeatBehavior = RepeatBehavior.Forever;
				Say.Debug("lplunk");
				elt.BeginAnimation(UIElement.OpacityProperty, anim);
	
				Say.Debug("lDunned GetVisualFeedback");
				
				return elt;
			}
			else {
				Say.Debug("throw new Exception(Couldn't get either a button or a line for visual feedback.);");
				throw new Exception("Couldn't get either a button or a line for visual feedback.");
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
			
			
        	Say.Debug("ExtractElement");
			string xamlString = obj.GetData("ABCFormat") as string;
			XmlReader reader = XmlReader.Create(new StringReader(xamlString));
			UIElement elt = XamlReader.Load(reader) as UIElement;

			return elt;
		}
	}
}

