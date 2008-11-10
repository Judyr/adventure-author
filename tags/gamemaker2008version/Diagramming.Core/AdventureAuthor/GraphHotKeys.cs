using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Netron.Diagramming.Core.AdventureAuthor
{
    /// <summary>
    /// This class implementing the <see cref="IKeyboardListener"/> collects all the hotkeys.
    /// </summary>
    class GraphHotKeys : IKeyboardListener
    {
        private IController mController;
        public IController Controller {
            get { return mController; }
        }

        public GraphHotKeys(IController controller)
        {
            mController = controller;
        }
        
        public void KeyUp(System.Windows.Forms.KeyEventArgs e)
        {
        	
        }
        
        public void KeyDown(System.Windows.Forms.KeyEventArgs e)
        {  
        	/*
        	The up, down, left, right and numpad keys don't seem to be picked up, which is a shame since they're the only
			ones which could really be useful (for navigating around the graph control). Other keys (alphabet letters at 
			least) do, but we don't really need them for anything. It'd be good to be able to navigate with the arrow
			keys but not really important. 
			*/

            switch (e.KeyCode)
            {
            	case System.Windows.Forms.Keys.Z:
                    break;
                default:
                    break;
            }
        }
        
        public void KeyPress(System.Windows.Forms.KeyPressEventArgs e)
        {
        	
        }
    }
}
