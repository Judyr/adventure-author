using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Samples.DragDrop;
using AdventureAuthor.Conversations.UI.Controls;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Conversations.UI
{
    public class DefaultDragSourceAdvisor : IDragSourceAdvisor
    {
		private static DataFormat SupportedFormat = DataFormats.GetDataFormat("ABCFormat");
		private UIElement _sourceUI;

		public DragDropEffects SupportedEffects
		{
			get { return DragDropEffects.Copy | DragDropEffects.Move; }
		}

		public UIElement SourceUI
		{
			get
			{
				return _sourceUI;
			}
			set
			{
				_sourceUI = value;
			}
		}

        public DataObject GetDataObject(UIElement draggedElt, Point offsetPoint)
		{
//        	string serializedObject;
        	Say.Debug("GetDataObject");
        	Line line = draggedElt as Line;
        	if (line == null) {
        		throw new System.Exception("Dragged element was not line, was: " + line.GetType().ToString() + ". Returning.");
        	}
//        	else {
//        		if (line.Nwn2Line.Actions.Count > 0) {
//        			Say.Debug("This line has action controls on it.");
//        			Button btn = new Button();
//        			btn.Height = 80;
//        			btn.Width = 400;
//        			btn.Content = "I AM A BUTTON PLEASE LISTEN TO ME";
//        			btn.Background = System.Windows.Media.Brushes.Red;
//        			serializedObject = XamlWriter.Save(btn);
//        		}
//        		else {
//        			Say.Debug("Line has no actions.");
//        			serializedObject = XamlWriter.Save(draggedElt);
//        		}
//        	}
        	
        	
//			string serializedObject = XamlWriter.Save(draggedElt);
        	Say.Debug("bang");
        	
			DataObject data = new DataObject();
			
        	Say.Debug("bang");
        	
//            data.SetData(SupportedFormat.Name, serializedObject);
            data.SetData("nwn2line",line.Nwn2Line);
            
        	Say.Debug("bang");
        	
            data.SetData("point", offsetPoint);
            
            Say.Debug("returning DataObject");
            
			return data;
		}

    	public void FinishDrag(UIElement draggedElt, DragDropEffects finalEffects)
		{
        	Say.Debug("FinishDrag");
			if ((finalEffects & DragDropEffects.Move) == DragDropEffects.Move)
			{
				StackPanel panel = SourceUI as StackPanel;
				if (panel != null)
				{
					panel.Children.Remove(draggedElt);
				}
			}
		}


		public bool IsDraggable(UIElement dragElt)
		{
			Say.Debug("IsDraggable - " + dragElt.GetType().ToString());
			bool isDraggable = dragElt is LineControl || dragElt is Line || dragElt is BranchLine;
			Say.Debug(isDraggable.ToString());
			return (isDraggable);
		}
	}
}
