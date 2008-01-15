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
	/// Ask a question which will be answered by a string value.
	/// </summary>
    public partial class StringQuestionPanel : UserControl, IParameterPanel
    {    	
    	/// <summary>
    	/// Create a question panel which will be answered by a string value.
    	/// </summary>
    	/// <param name="question">The question to ask the user</param>
		public StringQuestionPanel(string question)
        {
            InitializeComponent();
        	QuestionLabel.Text = question;
        }
		
		
    	/// <summary>
    	/// Create a question panel which will be answered by a string value.
    	/// </summary>
    	/// <param name="question">The question to ask the user</param>
        /// <param name="defaultValue">The default value of this answer on loading the window</param>
        public StringQuestionPanel(string question, string defaultValue) : this(question)
        {
        	AnswerBox.Text = defaultValue;
        }
        
		
        /// <summary>
        /// Returns an object representing an answer to the question posed by this panel - the type of object depends on the type of question.
        /// </summary>
        public object Answer
        {
        	get {
        		return AnswerBox.Text;
        	}
        }
    }
}
