using System;
using System.Windows.Forms;
using Netron.Diagramming.Core;
using AdventureAuthor.Utils;

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
        	try {
	        	if (e == null) {
	        		throw new ArgumentNullException("The argument object is 'null'");
	        	}
	        	
	            if (e.Button == MouseButtons.Left && Enabled && !IsSuspended)
	            {    
	            	bool centreGraph = e.Clicks == 2 ? true : false; // only centre on double-click
	            	Selection.CollectEntitiesAt(e.Location);
	            	foreach (IDiagramEntity entity in Selection.SelectedItems) { 
	            		// TODO Raised Exception (collection was modified, cannot enumerate)
	            		//, but don't know where it was modified? doesn't seem to have been by me. Only raised on double click 
	            		// seemingly
	            		Node node = entity as Node;
	            		if (node != null) {
	            			//Log.WriteAction(LogAction.selected,"pagenode");
	            			WriterWindow.Instance.DisplayPage(node.Page);	            			
	            			if (centreGraph) {	            			
	            				WriterWindow.Instance.CentreGraph(false);
	            			}
	       					Log.WriteAction(LogAction.viewed,"page","page beginning with: " + node.Page);
	            		}
	            		entity.IsSelected = false; // doesn't help
	            	}
	            	Selection.Clear();
	            	Selection.Invalidate();
	            }
	            return false;
        	}
        	catch (Exception ex) {
        		Say.Error("Something went wrong while the user was clicking on the graph.",ex);
        		return false;
        	}
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
