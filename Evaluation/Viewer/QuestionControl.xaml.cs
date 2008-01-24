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
            QuestionTitle.Text = question;
        }

        
        private void AddAnswerField(AnswerControl control)
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
        	
        	AnswerControl answerControl = answer.GetAnswerControl(WorksheetViewer.DesignerMode);
        	
//        	IAnswerControl answerControl;
//        	if (!WorksheetViewer.DesignerMode) {
//        		answerControl = answer.GetAnswerControl();
//        	}
//        	else {
//        		ActivatedAnswerControl aac = new ActivatedAnswerControl(answer.GetAnswerControl(),answer.Include);
//        		answerControl = aac;
//        	}        	
        	AddAnswerField(answerControl);
        }
        
        
        public Question GetQuestion()
        {
        	Question question = new Question(QuestionTitle.Text);
        	foreach (UIElement element in AnswersPanel.Children) {
        		AnswerControl ac = element as AnswerControl;
        		if (ac != null) {      
        			Answer answer = ac.GetAnswer();        		
        			question.Answers.Add(answer);
        		}
        	}
        	return question;
        }
    }
}