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
using System.Windows.Navigation;
using System.Windows.Shapes;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Utils
{
    public partial class TeacherPasswordDialog : Window
    {
    	private const string TEACHER_PASSWORD = "iamapassword";
    	
    	
    	private bool receivedCorrectPassword = false;    	
		public bool ReceivedCorrectPassword {
			get { return receivedCorrectPassword; }
		}
    	
    	
        public TeacherPasswordDialog()
        {
            InitializeComponent();
            TeacherPasswordBox.Password = TEACHER_PASSWORD;
            TeacherPasswordBox.Focus();
            TeacherPasswordBox.SelectAll();
        }

        
        private void OnClick_OK(object sender, RoutedEventArgs e)
        {
        	if (TeacherPasswordBox.Password == TEACHER_PASSWORD) {
        		Log.WriteMessage("Passed teacher password check");
        		receivedCorrectPassword = true;
        		Close();
        	}
        	else {
        		Log.WriteMessage("Failed teacher password check");
        		receivedCorrectPassword = false;
        		Say.Warning("Password was incorrect.");
        		TeacherPasswordBox.Password = String.Empty;
        		TeacherPasswordBox.Focus();
        	}
        }

        
        private void OnClick_Cancel(object sender, RoutedEventArgs e)
        {
        	Log.WriteMessage("Cancelled teacher password check");
        	receivedCorrectPassword = false;
        	Close();
        }
    }
}