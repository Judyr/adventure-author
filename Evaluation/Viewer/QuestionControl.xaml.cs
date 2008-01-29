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
    public partial class QuestionControl : OptionalWorksheetPartControl
    {    	
        public QuestionControl(Question question, bool designerMode)
        {
        	if (question == null) {
        		question = new Question();
        	}
        	
            InitializeComponent();            
            QuestionTitle.Text = question.Text;    
            QuestionTitle.TextChanged += delegate { OnChanged(new EventArgs()); };
            
		    foreach (Answer answer in question.Answers) {
		    	if (WorksheetViewer.DesignerMode || answer.Include) {
		    		AddAnswerField(answer);
		    	}
		    }        	
                   
            if (designerMode) { // show 'Active?' control, and assume that control is Active to begin with
            	ActivateCheckBox.Visibility = Visibility.Visible;
	    		if (question.Include) {
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

        
        private void AddAnswerField(OptionalWorksheetPartControl control)
        {
        	if (control == null) {
        		throw new ArgumentNullException("Can't add a null answer field.");
        	}            	
        	AnswersPanel.Children.Add(control);
        }
			        
			
        public void AddAnswerField(Answer answer)
        {
        	if (answer == null) {
        		throw new ArgumentNullException("Can't add a null answer field.");
        	}
        	
        	OptionalWorksheetPartControl control = answer.GetControl(WorksheetViewer.DesignerMode);
        	AddAnswerField(control);
        }
        
        
        private void OnChecked(object sender, EventArgs e)
        {
        	Activate();
        }
        
        
        private void OnUnchecked(object sender, EventArgs e)
        {
        	Deactivate(false);
        }

        
    	protected override void PerformEnable()
    	{    		
    		Tools.PreventEditingOfTextBox(QuestionTitle);
    		QuestionTitle.Opacity = 1.0f;
    		ActivatableControl.EnableElement(ActivateCheckBox);
    		EnableChildren();
    	}
    	
    	
    	protected void EnableChildren()
    	{
    		foreach (UIElement element in AnswersPanel.Children) {
    			OptionalWorksheetPartControl part = element as OptionalWorksheetPartControl;
    			if (part != null) {
    				part.Enable();
    			}
    		}
    	}
    	
    	
    	protected override void PerformActivate()
    	{	
    		ActivatableControl.EnableElement(QuestionTitle);
    		QuestionTitle.IsReadOnly = false;
    		ActivatableControl.EnableElement(ActivateCheckBox);
    		ActivateChildren();
    		if ((bool)!ActivateCheckBox.IsChecked) {
    			ActivateCheckBox.IsChecked = true;
    		}
    	}
    	
    	
    	protected void ActivateChildren()
    	{
    		foreach (UIElement element in AnswersPanel.Children) {
    			OptionalWorksheetPartControl part = element as OptionalWorksheetPartControl;
    			if (part != null) {
    				part.Activate();
    			}
    		}
    	}
    	
        
    	protected override void PerformDeactivate(bool preventReactivation)
    	{
    		ActivatableControl.DeactivateElement(QuestionTitle);
    		QuestionTitle.IsReadOnly = true;
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
    		foreach (UIElement element in AnswersPanel.Children) {
    			OptionalWorksheetPartControl part = element as OptionalWorksheetPartControl;
    			if (part != null) {
    				part.Deactivate(true);
    			}
    		}
    	}
    	        
    	
        protected override OptionalWorksheetPart GetWorksheetPartObject()
        {
        	Question question = new Question(QuestionTitle.Text);
        	foreach (UIElement element in AnswersPanel.Children) {
        		OptionalWorksheetPartControl control = element as OptionalWorksheetPartControl;
        		if (control != null) {      
        			Answer answer = (Answer)control.GetWorksheetPart();
        			question.Answers.Add(answer);
        		}
        	}
        	return question;
        }
    }
}