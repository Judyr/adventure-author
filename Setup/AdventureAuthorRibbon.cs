using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;

namespace AdventureAuthor.Setup
{	
	/// <summary>
	/// A floating ribbon with links to all of the main Adventure
	/// Author applications, and an 'Adventure Author speaks' panel
	/// which provides helpful messages to the user.
	/// </summary>
    public partial class AdventureAuthorRibbon : Window
    {
    	private bool canClose = false;
		public bool CanClose {
			get { return canClose; }
			set { canClose = value; }
		}
    	
    	
    	public MessagePanel UserMessagePanel {
    		get {
    			return userMessagePanel;
    		}
    	}
    	
    	
    	private object padlock = new object();
    	
    	
        public AdventureAuthorRibbon()
        {
            InitializeComponent(); 
            UserMessagePanel.DefaultMessage = "Adventure Author is developed by Heriot-Watt University.";
        }
        

        private void CancelClose(object sender, CancelEventArgs e)
        {
        	e.Cancel = !CanClose;
        }
        
        
        public void ForceClose()
        {
        	lock (padlock) {
	        	CanClose = true;
	        	Close();
        	}
        }
    }
}