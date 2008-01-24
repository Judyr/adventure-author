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
            
		    foreach (Answer answer in question.Answers) {
		    	if (WorksheetViewer.DesignerMode || answer.Include) {
		    		AddAnswerField(answer);
		    	}
		    }        	
                   
            if (designerMode) { // show 'Active?' control, and assume that control is Active to begin with
            	ActivateCheckBox.Visibility = Visibility.Visible;
	    		if (question.Include) {
	    			ActivationStatus = ControlStatus.Active;
	    		}
	    		else {
	    			ActivationStatus = ControlStatus.Inactive;
	    		}
            }
            else { // hide 'Active?' control, and set status to Not Applicable
        		ActivationStatus = ControlStatus.NA;
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
        	ActivationStatus = ControlStatus.Active;
        }
        
        
        private void OnUnchecked(object sender, EventArgs e)
        {
        	ActivationStatus = ControlStatus.Inactive;
        }

        
    	protected override void Enable()
    	{    		
    		ActivatableControl.Enable(QuestionTitle);
    		ActivatableControl.Enable(AnswersPanel);
    	}
    	
    	
    	protected override void Activate()
    	{	
    		ActivatableControl.Activate(QuestionTitle);
    		ActivatableControl.Activate(AnswersPanel);
    		if ((bool)!ActivateCheckBox.IsChecked) {
    			ActivateCheckBox.IsChecked = true;
    		}
    	}
    	
        
    	protected override void Deactivate()
    	{
    		ActivatableControl.Deactivate(QuestionTitle);
    		ActivatableControl.Deactivate(AnswersPanel);
    		if ((bool)ActivateCheckBox.IsChecked) {
    			ActivateCheckBox.IsChecked = false;
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
        
        
        protected override List<Control> GetActivationControls()
        {
        	List<Control> activationControls = new List<Control>(1);
        	activationControls.Add(ActivateCheckBox);
        	return activationControls;
        }
    }
}