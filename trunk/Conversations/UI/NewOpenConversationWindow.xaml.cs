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
using AdventureAuthor.Conversations.UI;


namespace AdventureAuthor.Conversations.UI
{
    /// <summary>
    /// Interaction logic for NewOpenConversationWindow.xaml
    /// </summary>

    public partial class NewOpenConversationWindow : Window
    {

        public NewOpenConversationWindow()
        {
            InitializeComponent();
        }

        
        private void OnClick_StartNewConversation(object sender, EventArgs e)
        {
			NewConversation form = new NewConversation();
			form.ShowDialog();
			this.Close();
        }
        
        
        private void OnClick_OpenExistingConversation(object sender, EventArgs e)
        {
        	WriterWindow.Instance.OpenDialog();
        	this.Close();
        }
    }
}