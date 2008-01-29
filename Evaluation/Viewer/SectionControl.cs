using System;
using System.Windows;
using System.Windows.Media;

using AdventureAuthor.Utils;

namespace AdventureAuthor.Evaluation.Viewer
{
    public partial class SectionControl : OptionalWorksheetPartControl
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
    	    	
        
        public SectionControl(Section section, bool designerMode) 	
        {
        	if (section == null) {
        		section = new Section();
        	}
        	
            InitializeComponent();            
            SectionTitleTextBox.Text = section.Title;
            SectionTitleTextBox.TextChanged += delegate { OnChanged(new EventArgs()); };            
            
        	foreach (Question question in section.Questions) {
        		if (WorksheetViewer.DesignerMode || question.Include) {	
		        	AddQuestion(question);
        		}
        	}
                   
            if (designerMode) { // show 'Active?' control, and assume that control is Active to begin with
            	ActivateCheckBox.Visibility = Visibility.Visible;
	    		if (section.Include) {
	    			Activate();
	    		}
	    		else {
	    			Deactivate(false);
	    		}
            }
            else { // hide 'Active?' control
            	Enable();
            }
        }  
			        
			
        public QuestionControl AddQuestion(Question question)
        {
        	if (question == null) {
        		throw new ArgumentNullException("Can't add a null question field.");
        	}
        	
        	QuestionControl control = (QuestionControl)question.GetControl(WorksheetViewer.DesignerMode);
        	AddQuestionControl(control);
        	return control;
        } 
        
        
        private void AddQuestionControl(QuestionControl control)
        {
        	if (control == null) {
        		throw new ArgumentNullException("Can't add a null question field.");
        	}   
        	
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
        
        
        private void OnChecked(object sender, EventArgs e)
        {
        	Activate();
        }
        
        
        private void OnUnchecked(object sender, EventArgs e)
        {
        	Deactivate(false);
        }
        
        
        private void OnClick_AddQuestion(object sender, EventArgs e)
        {
        	if (!WorksheetViewer.DesignerMode) {
        		throw new InvalidOperationException("Should not have been possible to call Add Question " +
        		                                    "when not in designer mode.");
        	}
        	
        	Question question = new Question("Enter your question here...");
        	question.Answers.Add(new Rating());
        	question.Answers.Add(new Comment());
        	question.Answers.Add(new Evidence());
        	QuestionControl control = new QuestionControl(question,true);
        	AddQuestionControl(control);
        	control.QuestionTitle.Focus();
        	control.QuestionTitle.SelectAll();        	
        }
        
        
    	protected override void PerformEnable()
    	{    		
    		// DON'T enable the title - otherwise it's editable outside of designer mode:
    		Tools.PreventEditingOfTextBox(SectionTitleTextBox);
    		SectionTitleTextBox.Opacity = 1.0f;
    		ActivatableControl.EnableElement(ActivateCheckBox);
    		EnableChildren();
    	}
    	
    	
    	protected void EnableChildren()
    	{
    		foreach (UIElement element in QuestionsPanel.Children) {
    			OptionalWorksheetPartControl part = element as OptionalWorksheetPartControl;
    			if (part != null) {
    				part.Enable();
    			}
    		}
    	}
    	
    	
    	protected override void PerformActivate()
    	{	    		
    		SectionTitleTextBox.Opacity = 1.0f;
    		SectionTitleTextBox.IsEnabled = true;
    		Tools.AllowEditingOfTextBox(SectionTitleTextBox);
    		ActivatableControl.EnableElement(ActivateCheckBox);
    		ActivatableControl.EnableElement(AddQuestionButton);
    		ActivateChildren();
    		if ((bool)!ActivateCheckBox.IsChecked) {
    			ActivateCheckBox.IsChecked = true;
    		}
    	}
    	
    	
    	protected void ActivateChildren()
    	{
    		foreach (UIElement element in QuestionsPanel.Children) {
    			OptionalWorksheetPartControl part = element as OptionalWorksheetPartControl;
    			if (part != null) {
    				part.Activate();
    			}
    		}
    	}
    	
        
    	protected override void PerformDeactivate(bool preventReactivation)
    	{
    		ActivatableControl.DeactivateElement(SectionTitleTextBox);
    		ActivatableControl.DeactivateElement(AddQuestionButton);
    		DeactivateChildren();
    		if ((bool)ActivateCheckBox.IsChecked) {
    			ActivateCheckBox.IsChecked = false;
    		}
    		if (preventReactivation) {
    			ActivatableControl.DeactivateElement(ActivateCheckBox);
    		}
    	}
    	
    	
    	protected void DeactivateChildren()
    	{
    		foreach (UIElement element in QuestionsPanel.Children) {
    			OptionalWorksheetPartControl part = element as OptionalWorksheetPartControl;
    			if (part != null) {
    				part.Deactivate(true);
    			}
    		}
    	}
        
        
        protected override OptionalWorksheetPart GetWorksheetPartObject()
        {
        	Section section = new Section(SectionTitleTextBox.Text);   			
   			foreach (UIElement element in QuestionsPanel.Children) {
   				QuestionControl qc = element as QuestionControl;
   				if (qc != null) {
   					Question question = (Question)qc.GetWorksheetPart();
   					section.Questions.Add(question);
   				}
   			}   			
   			return section;
        }
    }
}