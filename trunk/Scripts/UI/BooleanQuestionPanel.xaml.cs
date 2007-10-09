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

namespace AdventureAuthor.Scripts.UI
{
	/// <summary>
	/// Ask a question which will be answered by a boolean value.
	/// </summary>
    public partial class BooleanQuestionPanel : UserControl, IQuestionPanel
    {    	
    	/// <summary>
    	/// Create a question panel which will be answered by a boolean value.
    	/// </summary>
    	/// <param name="question">The question to ask the user</param>
        public BooleanQuestionPanel(string question)
        {
        	InitializeComponent();
        	QuestionLabel.Text = question;
        	YesButton.Content = "Yes";
        	NoButton.Content = "No";
        }
        
        
    	/// <summary>
    	/// Create a question panel which will be answered by a boolean value.
    	/// </summary>
    	/// <param name="question">The question to ask the user</param>    	
    	/// <param name="trueText">The text to display instead of "Yes" for the 'true' answer</param>
    	/// <param name="falseText">The text to display instead of "No" for the 'false' answer</param>
        public BooleanQuestionPanel(string question, string trueText, string falseText)
        {
        	InitializeComponent();
        	QuestionLabel.Text = question;
        	if (trueText != null) {
        		YesButton.Content = trueText;
        	}
        	else {
        		YesButton.Content = "Yes";
        	}
        	if (falseText != null) {
        		NoButton.Content = falseText;
        	}
        	else {
        		NoButton.Content = "No";
        	}
        }
        
        
    	/// <summary>
    	/// Create a question panel which will be answered by a boolean value.
    	/// </summary>
    	/// <param name="question">The question to ask the user</param>    	
    	/// <param name="trueText">The text to display instead of "Yes" for the 'true' answer</param>
    	/// <param name="falseText">The text to display instead of "No" for the 'false' answer</param>
        /// <param name="defaultValue">The default value of this answer on loading the window</param>
        public BooleanQuestionPanel(string question, string trueText, string falseText, bool defaultval) : this(question,trueText,falseText)
        {
        	if (defaultval == true) {
        		YesButton.IsChecked = true;
        	}
        	else {
        		NoButton.IsChecked = true;
        	}
        }
        
                
        /// <summary>
        /// Returns an object representing an answer to the question posed by this panel - the type of object depends on the type of question.
        /// </summary>
        public object Answer  // have to convert bool into an int for use with scripts
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
