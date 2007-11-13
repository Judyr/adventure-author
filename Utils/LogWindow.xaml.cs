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
using AdventureAuthor.Utils;

namespace AdventureAuthor.Utils
{
    public partial class LogWindow : Window
    {
    	private List<Label> debugmsgs = new List<Label>(1000);
    	
    	
        public LogWindow()
        {
            InitializeComponent();
            Log.Message += new EventHandler<LogEventArgs>(Log_Message);
            DebugLog.Message += new EventHandler<DebugLogEventArgs>(DebugLog_Message);
        }

        
        private void DebugLog_Message(object sender, DebugLogEventArgs e)
        {			
//        	object loggable = null;
//			if (e.Message != null) {
//				loggable = e.Message;
//			}
//			else if (e.ThrownException != null) {
//				loggable = e.ThrownException;
//			}
//			else {
//				loggable = "Empty debug message.";
//			}
//			string msg = UsefulTools.GetTimeStamp(false) + ": " + loggable;
//			
//        	Label message = new Label();
//        	message.Content = msg;
//        	message.Visibility = Visibility.Collapsed;
//        	debugmsgs.Add(message);
//        	MessagesList.Items.Add(msg);        	
//        	MessagesScroller.ScrollToBottom();
        }
        

        private void Log_Message(object sender, LogEventArgs e)
        {
        	Label message = new Label();
        	message.Content = e.Message;
        	message.Foreground = Brushes.Red;
        	
        	MessagesList.Items.Add(e.Message);
        	MessagesScroller.ScrollToBottom();
        }
        
        
        private void OnClick_GoToTop(object sender, EventArgs e)
        {
        	MessagesScroller.ScrollToTop();
        }

        
        private void OnClick_GoToBottom(object sender, EventArgs e)
        {
        	MessagesScroller.ScrollToBottom();
        }
		
        
		private void ShowDebugCheckboxUnchecked(object sender, RoutedEventArgs e)
		{
			foreach (Label l in debugmsgs) {
				l.Visibility = Visibility.Collapsed;
			}
		}
		
		
		private void ShowDebugCheckboxChecked(object sender, RoutedEventArgs e)
		{
			foreach (Label l in debugmsgs) {
				l.Visibility = Visibility.Visible;
			}
		}
    }
}