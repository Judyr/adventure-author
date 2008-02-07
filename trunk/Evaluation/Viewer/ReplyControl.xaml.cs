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

namespace AdventureAuthor.Evaluation.Viewer
{
    public partial class ReplyControl : UserControl
    {
        public ReplyControl()
        {
            InitializeComponent();
        }

        
        private void OnClick_ReplyToThis(object sender, RoutedEventArgs e)
        {
        	// OnReply(new ReplyEventArgs(this));
        }
    }
}