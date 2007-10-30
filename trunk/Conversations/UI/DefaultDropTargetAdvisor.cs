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

namespace AdventureAuthor.Conversations.UI
{
	public class DefaultDropTargetAdvisor : IDropTargetAdvisor
	{
		private static DataFormat SupportedFormat = DataFormats.GetDataFormat("ABCFormat");
		private UIElement _targetUI;

		public bool IsValidDataObject(IDataObject obj)
		{
        	Say.Debug("IsValidDataObject");
        	bool isValid = obj.GetDataPresent(SupportedFormat.Name);
        	Say.Debug(isValid.ToString());
			return isValid;
		}

		private static int tempcount = 0;
		
		public void OnDropCompleted(IDataObject obj, Point dropPoint)
		{
			tempcount++;
        	Say.Debug("OnDropCompleted");
			UIElement elt = ExtractElement(obj);

			(TargetUI as StackPanel).Children.Add(elt);
			
			
			Conversation.CurrentConversation.AddSpeaker("rambo" + tempcount);
			Say.Debug(tempcount.ToString() + "th run of OnDropCompleted.");
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
        	Say.Debug("ExtractElement");
			string xamlString = obj.GetData("ABCFormat") as string;
			XmlReader reader = XmlReader.Create(new StringReader(xamlString));
			UIElement elt = XamlReader.Load(reader) as UIElement;

			return elt;
		}
	}
}
