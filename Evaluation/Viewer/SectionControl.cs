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
    	#region Events
    	
    	public event EventHandler<DeletingEventArgs> Deleting;  
    	
		protected virtual void OnDeleting(DeletingEventArgs e)
		{
			EventHandler<DeletingEventArgs> handler = Deleting;
			if (handler != null) {
				handler(this,e);
			}
		}
    	
    	#endregion
    	
    	
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
            if (!WorksheetViewer.DesignerMode) {
            	DeleteSectionButton.Visibility = Visibility.Collapsed;
            	//ActiveCheckBox.Visibility = Visibility.Collapsed;
            }
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
   					Question question = qc.GetQuestion();
   					section.Questions.Add(question);
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
        
        
        private void OnClick_DeleteSection(object sender, EventArgs e)
        {
        	if (!WorksheetViewer.DesignerMode) {
        		throw new InvalidOperationException("Should not have been possible to try to delete a section.");
        	}
        	
        	MessageBoxResult result = MessageBox.Show("This section contains " + QuestionsPanel.Children.Count +
        	                                          " questions - are you sure you want to permanently delete it?",
        	                                          "Delete section?",
        	                                          MessageBoxButton.OKCancel,
        	                                          MessageBoxImage.Warning,
        	                                          MessageBoxResult.Cancel,
        	                                          MessageBoxOptions.None);
        	if (result == MessageBoxResult.OK) {
        		OnDeleting(new DeletingEventArgs(this));
        	}
        }
    }
}