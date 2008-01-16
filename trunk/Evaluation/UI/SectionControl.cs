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
using AdventureAuthor.Evaluation;
using AdventureAuthor.Evaluation.UI;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Evaluation.UI
{
    public partial class SectionControl : UserControl
    {
        public SectionControl(string title)
        {
            InitializeComponent();
            SectionTitle.Content = title;
        }   
        
        
        public SectionControl(Section section) : this(section.Title)
        {
        	foreach (Question question in section.Questions) {
        		QuestionControl qc = new QuestionControl(question.Text);         		
        		foreach (Answer answer in question.Answers) {
        			qc.AddAnswerField(answer.GetAnswerControl());
        		}        	
        		AddQuestionControl(qc);
        	}
        }
        
        
        public Section GetSection()
        {
        	bool reportedError = false;
   			Section section;
   			try {
   				section = new Section((string)SectionTitle.Content);
   			}
   			catch (Exception) {
   				if (!reportedError) { // only report this once
   					Say.Error("Invalid section title(s) in worksheet.");
   					reportedError = true;
   				}
   				section = new Section("<Unknown section>");
   			}
   			
   			foreach (UIElement element in QuestionsPanel.Children) {
   				QuestionControl qc = element as QuestionControl;
   				if (qc != null) {
   					section.Questions.Add(qc.GetQuestion());
   				}
   			}
   			
   			return section;
        }
        
        
        private void AddQuestionControl(QuestionControl control)
        {
        	if (QuestionsPanel.Children.Count % 2 == 0) {
        		control.Background = (Brush)Resources["Stripe1Brush"];
        	}
        	else {
        		control.Background = (Brush)Resources["Stripe2Brush"];
        	}
        	QuestionsPanel.Children.Add(control);
        }
    }
}