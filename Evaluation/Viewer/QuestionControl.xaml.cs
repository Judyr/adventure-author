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

namespace AdventureAuthor.Evaluation.Viewer
{
    /// <summary>
    /// Interaction logic for QuestionControl.xaml
    /// </summary>

    public partial class QuestionControl : UserControl
    {    	
        public QuestionControl(string question)
        {
            InitializeComponent();
            QuestionText.Text = question;
        }

        
        public void AddAnswerField(IAnswerControl control)
        {
        	if (control == null) {
        		throw new ArgumentNullException("Can't add a null answer field.");
        	}            	
        	if (!(control is UIElement)) {
        		throw new ArgumentException("The answer control must be a UIElement.");
        	}
        	AnswersPanel.Children.Add((UIElement)control);
        }
			
			
        public void AddAnswerField(Answer answer)
        {
        	if (answer == null) {
        		throw new ArgumentNullException("Can't add a null answer field.");
        	}
        	IAnswerControl control = answer.GetAnswerControl();        	
        	if (!(control is UIElement)) {
        		throw new ArgumentException("The answer control must be a UIElement.");
        	}
        	AnswersPanel.Children.Add((UIElement)control);
        }
        
        
        public Question GetQuestion()
        {
        	Question question = new Question(QuestionText.Text);
        	foreach (UIElement element in AnswersPanel.Children) {
        		IAnswerControl ac = element as IAnswerControl;
        		if (ac != null) {
        			question.Answers.Add(ac.GetAnswer());
        		}
        	}
        	return question;
        }
    }
}