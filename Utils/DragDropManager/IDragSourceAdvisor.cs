using System.Windows;

namespace Samples.DragDrop
{
    public interface IDragSourceAdvisor
    {
		UIElement SourceUI
		{
			get;
			set;
		}

		DragDropEffects SupportedEffects
		{
			get;
		}
    	
		DataObject GetDataObject(UIElement draggedElt, Point offsetPoint);
		void FinishDrag(UIElement draggedElt, DragDropEffects finalEffects);
		bool IsDraggable(UIElement dragElt);
    }
}
