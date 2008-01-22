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
using AdventureAuthor.Evaluation.Viewer;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Evaluation.Viewer
{
    public partial class SectionControl : UserControl
    {
    	public string SectionTitle {
    		get {    			
	   			return SectionTitleTextBlock.Text;
    		}
    		set {
    			SectionTitleTextBlock.Text = value;
    		}
    	}
    	
    	
        public SectionControl(string title)
        {
            InitializeComponent();
            SectionTitle = title;
        }   
        
        
        public SectionControl(Section section) : this(section.Title)
        {
        	foreach (Question question in section.Questions) {
        		if (WorksheetViewer.DesignerMode || question.Include) {	    			
		        	QuestionControl qc = new QuestionControl(question.Text);         		
		        	foreach (Answer answer in question.Answers) {
		        		if (WorksheetViewer.DesignerMode || answer.Include) {
		        			qc.AddAnswerField(answer);
		        		}
		        	}        	
		        	AddQuestionControl(qc);
        		}
        	}
        }
        
        
        public Section GetSection()
        {
        	Section section = new Section(SectionTitle);   			
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