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
    	#region Events
    	
    	public event EventHandler<DeletingEventArgs> Deleting;  
    	
		protected virtual void OnDeleting(DeletingEventArgs e)
		{
			EventHandler<DeletingEventArgs> handler = Deleting;
			if (handler != null) {
				handler(this,e);
			}
		}
		
    	public event EventHandler<MovingEventArgs> Moving;  
    	
		protected virtual void OnMoving(MovingEventArgs e)
		{
			EventHandler<MovingEventArgs> handler = Moving;
			if (handler != null) {
				handler(this,e);
			}
		}  
    	
    	#endregion
    	
    	
        public QuestionControl(Question question, bool designerMode)
        {
        	if (question == null) {
        		question = new Question();
        	}
        	
            InitializeComponent();            
            QuestionTitle.Text = question.Text;    
            QuestionTitle.TextChanged += delegate { OnChanged(new EventArgs()); };
            SetInitialActiveStatus(question);
            
		    foreach (Answer answer in question.Answers) {
		    	if (WorksheetViewer.DesignerMode || answer.Include) {
		    		AddAnswerField(answer);
		    	}
		    }
                 
            if (designerMode) {
            	ControlPanel.Visibility = Visibility.Visible;
            	ControlPanel.Width = 120;
            	ControlPanel.MaxWidth = 120;
            	QuestionAndControlsColumn.MaxWidth = 500;
            	QuestionAndControlsColumn.Width = new GridLength(500);
            }
            else {
            	ControlPanel.Visibility = Visibility.Collapsed;
            	ControlPanel.Width = 0;
            	ControlPanel.MaxWidth=0;
            	QuestionAndControlsColumn.MaxWidth = 350;
            	QuestionAndControlsColumn.Width = new GridLength(350);
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
        
        
        private void OnClick_DeleteQuestion(object sender, EventArgs e)
        {
        	if (!WorksheetViewer.DesignerMode) {
        		throw new InvalidOperationException("Should not have been possible to try to delete a question.");
        	}
        	
        	MessageBoxResult result = MessageBox.Show("Are you sure you want to permanently delete this question?",
        	                                          "Delete question?",
        	                                          MessageBoxButton.OKCancel,
        	                                          MessageBoxImage.Warning,
        	                                          MessageBoxResult.Cancel,
        	                                          MessageBoxOptions.None);
        	if (result == MessageBoxResult.OK) {
        		OnDeleting(new DeletingEventArgs(this));
        	}
        }    
        
        
        private void OnClick_MoveUp(object sender, EventArgs e)
        {
        	OnMoving(new MovingEventArgs(this,true));
        }
        
        
        private void OnClick_MoveDown(object sender, EventArgs e)
        {
        	OnMoving(new MovingEventArgs(this,false));
        }

        
        private void OnClick_AddReply(object sender, EventArgs e)
        {
        	AddReplyWindow window = new AddReplyWindow();
        	window.ReplyAdded += new EventHandler<ReplyAddedEventArgs>(ReplyAdded);
        	window.ShowDialog();        	
        }

        
        private void ReplyAdded(object sender, ReplyAddedEventArgs e)
        {
        	ReplyControl replyControl = new ReplyControl(e.Reply);
        	RepliesPanel.Children.Add(replyControl);
//        	if (AddReplyButton.Visibility == Visibility.Visible) {
//        		AddReplyButton.Visibility == Visibility.Collapsed);
//        	}
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
    		ActivatableControl.EnableElement(ActivateCheckBox);
    		ActivatableControl.EnableElement(MoveUpButton);
    		ActivatableControl.EnableElement(MoveDownButton);
    		ActivatableControl.EnableElement(DeleteQuestionButton);
    		ActivateChildren();
    		QuestionTitle.IsReadOnly = false;
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
    	
        
    	protected override void PerformDeactivate(bool parentIsDeactivated)
    	{
    		ActivatableControl.DeactivateElement(QuestionTitle);
    		ActivatableControl.DeactivateElement(MoveUpButton);
    		ActivatableControl.DeactivateElement(MoveDownButton);
    		ActivatableControl.DeactivateElement(DeleteQuestionButton);
    		QuestionTitle.IsReadOnly = true;
    		DeactivateChildren();
    		if ((bool)ActivateCheckBox.IsChecked) {
    			ActivateCheckBox.IsChecked = false;
    		}
    		if (parentIsDeactivated) {
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
    	
		protected override void ShowActivationControls()
		{
			ActivateCheckBox.Visibility = Visibility.Visible;
		}
		
		protected override void HideActivationControls()
		{
			ActivateCheckBox.Visibility = Visibility.Collapsed;
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