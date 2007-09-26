using System;
using System.Windows.Forms;
using Netron.Diagramming.Core;

namespace AdventureAuthor.Conversations.UI.Graph
{
    /// <summary>
    /// This tool implement the action of hitting an entity on the canvas. 
    /// </summary>
    public class GraphTool : AbstractTool, IMouseListener
    {
        public GraphTool(string name) : base(name)
        {
        	
        }
        
        /// <summary>
        /// Called when the tool is activated.
        /// </summary>
        protected override void OnActivateTool()
        {
        	
        }

        /// <summary>
        /// Handles the mouse down event
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        public bool MouseDown(MouseEventArgs e)
        {
        	if (e == null) {
        		throw new ArgumentNullException("The argument object is 'null'");
        	}
        	
            if (e.Button == MouseButtons.Left && Enabled && !IsSuspended)
            {        
            	Selection.CollectEntitiesAt(e.Location);
            	if (Selection.SelectedItems.Count > 0) {
	            	if (Selection.SelectedItems[0] is Node) {
            			Page page = ((Node)Selection.SelectedItems[0]).Page;
            			ConversationWriterWindow.Instance.DisplayPage(page);
	            	}
	            	else if (Selection.SelectedItems[0] is IConnection) {
            			Selection.Clear();
            			Selection.Invalidate();
	            	}
	            	else if (Selection.SelectedItems[0] is IConnector) {
            			Selection.Clear();
            			Selection.Invalidate();
	            	}
            	}
            	
            	
                              
//                if (Selection.SelectedItems.Count > 0) {
//                    IMouseListener listener = Selection.SelectedItems[0].GetService(typeof(IMouseListener)) as IMouseListener;
//                    if(listener != null) {
//                    	if (listener.MouseDown(e)) {
//                            return true;
//                        }
//                    }
//                }               
            }
            return false;
        }

        /// <summary>
        /// Handles the mouse move event
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        public void MouseMove(MouseEventArgs e)
        {
      
        }
      
        public void MouseUp(MouseEventArgs e)
        {
            if (IsActive)
            {
                DeactivateTool();
            }
        }
    }

}
