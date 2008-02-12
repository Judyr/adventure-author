using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.IO;
using AdventureAuthor.Core;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Evaluation.Viewer
{
    public partial class SectionControl : OptionalWorksheetPartControl
    {     	
    	#region Events
    	
    	public event EventHandler<OptionalWorksheetPartControlEventArgs> Deleting;  
    	
		protected virtual void OnDeleting(OptionalWorksheetPartControlEventArgs e)
		{
			EventHandler<OptionalWorksheetPartControlEventArgs> handler = Deleting;
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
    	    	
        
        public SectionControl(Section section) 	
        {
        	if (section == null) {
        		section = new Section();
        	}  	
        	
            InitializeComponent();   
            
            SectionTitleTextBox.Text = section.Title;
            SectionTitleTextBox.TextChanged += delegate { OnChanged(new EventArgs()); };  
            SetInitialActiveStatus(section); 
            
        	foreach (Question question in section.Questions) {
        		if (WorksheetViewer.EvaluationMode == Mode.Design || question.Include) {	
		        	AddQuestionField(question);
        		}
        	}
                           
            if (WorksheetViewer.EvaluationMode == Mode.Design) {
            	AddQuestionButton.Visibility = Visibility.Visible;
            	DeleteSectionButton.Visibility = Visibility.Visible;
            	MoveSectionDownButton.Visibility = Visibility.Visible;
            	MoveSectionUpButton.Visibility = Visibility.Visible;
            }
            
            Tools.SetButtonImage(DeleteSectionButton,"01.png","delete");
            Tools.SetButtonImage(MoveSectionDownButton,"07.png","down");
            Tools.SetButtonImage(MoveSectionUpButton,"08.png","up");
        }  
			        
			
        public QuestionControl AddQuestionField(Question question)
        {
        	if (question == null) {
        		throw new ArgumentNullException("Can't add a null question field.");
        	}
        	
        	QuestionControl control = (QuestionControl)question.GetControl();
        	AddQuestionField(control);
        	return control;
        } 
        
        
        private void AddQuestionField(QuestionControl control)
        {
        	if (control == null) {
        		throw new ArgumentNullException("Can't add a null question field.");
        	}   
        	
        	if (QuestionsPanel.Children.Count % 2 != 0) {
        		control.Background = (Brush)Resources["Stripe1Brush"];
        	}
        	else {
        		control.Background = (Brush)Resources["Stripe2Brush"];
        	}         	
        	QuestionsPanel.Children.Add(control);    	
    		control.Deleting += new EventHandler<OptionalWorksheetPartControlEventArgs>(questionControl_Deleting);
    		control.Moving += new EventHandler<MovingEventArgs>(questionControl_Moving);
    		control.Activated += delegate { OnChanged(new EventArgs()); };
    		control.Deactivated += delegate { OnChanged(new EventArgs()); };
    		control.Changed += delegate { OnChanged(new EventArgs()); };
        	control.BringIntoView();
        	OnChanged(new EventArgs());
        }
        
        
        private void SetBackgrounds()
        {
        	bool even = false;
        	foreach (QuestionControl questionControl in QuestionsPanel.Children) {
        		if (even) {
        			questionControl.Background = (Brush)Resources["Stripe1Brush"];
	        	}
	        	else {
	        		questionControl.Background = (Brush)Resources["Stripe2Brush"];
	        	}     
        		even = !even;
        	}	
        }
    	
    	
    	private void questionControl_Moving(object sender, MovingEventArgs e)
    	{
    		WorksheetViewer.MoveWithin(e.Control,QuestionsPanel.Children,e.MoveUp);
    		SetBackgrounds();
    	}
    	    	
    	
    	private void questionControl_Deleting(object sender, OptionalWorksheetPartControlEventArgs e)
    	{
    		WorksheetViewer.DeleteFrom(e.Control,QuestionsPanel.Children);
    		SetBackgrounds();
    	}
        
        
        private void OnClick_DeleteSection(object sender, EventArgs e)
        {
        	if (WorksheetViewer.EvaluationMode != Mode.Design) {
        		throw new InvalidOperationException("Should not have been possible to try to delete a section.");
        	}
        	
        	string message;
        	if (QuestionsPanel.Children.Count == 0) {
        		message = "Are you sure you want to delete this section?";
        	}
        	else {
        		message = "This section contains " + QuestionsPanel.Children.Count +
        	              " questions - are you sure you want to permanently delete it?";
        	}        	
        	MessageBoxResult result = MessageBox.Show(message,
        	                                          "Delete section?",
        	                                          MessageBoxButton.OKCancel,
        	                                          MessageBoxImage.Warning,
        	                                          MessageBoxResult.Cancel,
        	                                          MessageBoxOptions.None);
        	if (result == MessageBoxResult.OK) {
        		OnDeleting(new OptionalWorksheetPartControlEventArgs(this));
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
        
        
        private void OnChecked(object sender, EventArgs e)
        {
        	Activate();
        }
        
        
        private void OnUnchecked(object sender, EventArgs e)
        {
        	Deactivate(false);
        }
        
        
        internal void AddNewQuestion()
        {        	
        	Question question = new Question("New question");
        	question.Answers.Add(new Rating());
        	question.Answers.Add(new Comment());
        	question.Answers.Add(new Evidence());
        	QuestionControl control = new QuestionControl(question);
        	AddQuestionField(control);
        	OnChanged(new EventArgs());
        }
        
        
        private void OnClick_AddQuestion(object sender, EventArgs e)
        {
        	if (WorksheetViewer.EvaluationMode != Mode.Design) {
        		throw new InvalidOperationException("Should not have been possible to call Add Question " +
        		                                    "when not in designer mode.");
        	}
        	
        	AddNewQuestion();
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
    		ActivatableControl.EnableElement(MoveSectionDownButton);
    		ActivatableControl.EnableElement(MoveSectionUpButton);
    		ActivatableControl.EnableElement(DeleteSectionButton);    		
    		ActivateChildren();
    		if ((bool)!ActivateCheckBox.IsChecked) {
    			ActivateCheckBox.IsChecked = true;
    		}
    		ActivateCheckBox.ToolTip = "Click to deactivate this section\n(will not appear in worksheet)";
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
    	
        
    	protected override void PerformDeactivate(bool parentIsDeactivated)
    	{
    		ActivatableControl.DeactivateElement(SectionTitleTextBox);
    		ActivatableControl.DeactivateElement(AddQuestionButton);
    		ActivatableControl.DeactivateElement(MoveSectionDownButton);
    		ActivatableControl.DeactivateElement(MoveSectionUpButton);
    		ActivatableControl.DeactivateElement(DeleteSectionButton);
    		DeactivateChildren();
    		if ((bool)ActivateCheckBox.IsChecked) {
    			ActivateCheckBox.IsChecked = false;
    		}
    		if (parentIsDeactivated) {
    			ActivatableControl.DeactivateElement(ActivateCheckBox);
    		}
    		ActivateCheckBox.ToolTip = "Click to activate this section\n(will appear in worksheet)";
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
    	
		public override void ShowActivationControls()
		{
			ActivateCheckBox.Visibility = Visibility.Visible;
		}
		
		public override void HideActivationControls()
		{
			ActivateCheckBox.Visibility = Visibility.Collapsed;
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