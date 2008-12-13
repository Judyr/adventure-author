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
using System.Windows.Threading;
using System.ComponentModel;
using AdventureAuthor.Core;
using AdventureAuthor.Conversations.UI;
using AdventureAuthor.Variables.UI;
using AdventureAuthor.Achievements.UI;
using AdventureAuthor.Utils;
using System.Resources;

namespace AdventureAuthor.Setup
{	
	/// <summary>
	/// A floating window with links to all of the main Adventure
	/// Author applications, and an 'Adventure Author speaks' panel
	/// which provides helpful messages to the user.
	/// </summary>
	/// <remarks>Cannot be closed until CanClose
	/// is set to True (False by default).</remarks>
    public partial class AdventureAuthorRibbon : Window
    {
    	#region Properties and fields
    	
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
    	
    	#endregion
    	
    	#region Constructors
    	
        public AdventureAuthorRibbon()
        {       
            InitializeComponent(); 
            
            UserMessagePanel.DefaultMessage = new HyperlinkMessage("Adventure Author is developed " + 
                                                                   "by Heriot-Watt University.");
                        
			ModuleHelper.ModuleOpened += delegate { 
            	lock (padlock) {
					conversationWriterButton.IsEnabled = true;
					variableManagerButton.IsEnabled = true;
            	}
            };
            
			ModuleHelper.ModuleClosed += delegate {  
            	lock (padlock) {
					conversationWriterButton.IsEnabled = false;
					variableManagerButton.IsEnabled = false;
            	}
            };
        }
        
        #endregion
        
        #region Methods       
        
        public void ForceClose()
        {
        	lock (padlock) {
	        	CanClose = true;
	        	Close();
        	}
        }
        
        #endregion
        
        #region Event handlers
        
        private void CancelClose(object sender, CancelEventArgs e)
        {
        	e.Cancel = !CanClose;
        }
        
        
        public delegate void RunApplicationDelegate();
        
        public static void RunConversationWriter()
        {
        	
        	(new Button()).Dispatcher.Invoke(DispatcherPriority.Normal,
        	                  new RunApplicationDelegate(RunConversationWriter2));
        }
        private static void RunConversationWriter2()
        {
			Toolset.LaunchConversationWriter(true);	
			Tools.BringToFront(WriterWindow.Instance);
        }
        
        public void RunConversationWriter(object sender, RoutedEventArgs e)
        {
        	RunConversationWriter();
        }
        
        
        public void RunVariableManager(object sender, RoutedEventArgs e)
        {
			Toolset.LaunchVariableManager();
			Tools.BringToFront(VariablesWindow.Instance);
        }
        
        
        public void RunFridgeMagnets(object sender, RoutedEventArgs e)
        {
        }
        
        
        public void RunMyTasks(object sender, RoutedEventArgs e)
        {
        	
        }
        
        
        public void RunCommentCards(object sender, RoutedEventArgs e)
        {
        	
        }
        
        
        public void RunMyAchievements(object sender, RoutedEventArgs e)
        {
			Toolset.LaunchMyAchievements();
			Tools.BringToFront(ProfileWindow.Instance);
        }	
        
        #endregion
    }
}