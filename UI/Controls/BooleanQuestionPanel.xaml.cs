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

namespace AdventureAuthor.UI.Controls
{
    /// <summary>
    /// Interaction logic for BooleanQuestionPanel.xaml
    /// </summary>

    public partial class BooleanQuestionPanel : UserControl, IQuestionPanel
    {
        public BooleanQuestionPanel(string question)
        {
        	InitializeComponent();
        	QuestionLabel.Content = question;
        	YesButton.Content = "Yes";
        	YesButton.Content = "No";
        }
        
        public BooleanQuestionPanel(string question, string trueText, string falseText)
        {
        	InitializeComponent();
        	QuestionLabel.Content = question;  
        	YesButton.Content = trueText;
        	YesButton.Content = falseText;
        }
        
        public object Answer    // have to convert bool into an int for use with scripts
        {
			get { 
        		if ((bool)YesButton.IsChecked) {
        			return 1;
        		}
        		else if ((bool)NoButton.IsChecked) {
        			return 0;
        		}
        		else {
        			return null;
        		}
        	}
		}
    }
}
