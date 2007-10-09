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
            	bool centreGraph = e.Clicks == 2 ? true : false; // only centre on double-click
            	Selection.CollectEntitiesAt(e.Location);
            	foreach (IDiagramEntity entity in Selection.SelectedItems) {
            		Node node = entity as Node;
            		if (node != null) {
            			WriterWindow.Instance.DisplayPage(node.Page);
            			if (centreGraph) {
            				WriterWindow.Instance.CentreGraph(false);
            			}
            		}
            		entity.IsSelected = false; // doesn't help
            	}
            	Selection.Clear();
            	Selection.Invalidate();
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
        	
        }
    }

}