using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Samples.DragDrop;
using AdventureAuthor.Conversations.UI.Controls;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Conversations.UI
{
    public class LineDragSourceAdvisor : IDragSourceAdvisor
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
        	Say.Debug("GetDataObject");
			DataObject data = new DataObject();
        	data.SetData("nwn2line",((LineControl)draggedElt).Nwn2Line);
        	Say.Debug("a");
        	data.SetData("lineOriginatedFromChoice",(object)draggedElt is BranchLine);
        	Say.Debug("b");
            data.SetData("point", offsetPoint);
			return data;
		}

    	public void FinishDrag(UIElement draggedElt, DragDropEffects finalEffects)
		{
        	
		}


		public bool IsDraggable(UIElement dragElt)
		{
			Say.Debug("IsDraggable - " + dragElt.GetType().ToString());
			bool isDraggable = dragElt is Line || dragElt is BranchLine;
			Say.Debug(isDraggable.ToString());
			return (isDraggable);
		}
	}
}
