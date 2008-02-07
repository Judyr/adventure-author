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
    	
		
    	public event EventHandler<MovingEventArgs> Moving;  
    	
		protected virtual void OnMoving(MovingEventArgs e)
		{
			EventHandler<MovingEventArgs> handler = Moving;
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
            SetInitialActiveStatus(section); 
            
        	foreach (Question question in section.Questions) {
        		if (designerMode || question.Include) {	
		        	AddQuestion(question);
        		}
        	}
                           
            if (designerMode) {
            	AddQuestionButton.Visibility = Visibility.Visible;
            	DeleteSectionButton.Visibility = Visibility.Visible;
            	MoveSectionDownButton.Visibility = Visibility.Visible;
            	MoveSectionUpButton.Visibility = Visibility.Visible;
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
        	
        	if (QuestionsPanel.Children.Count % 2 != 0) {
        		control.Background = (Brush)Resources["Stripe1Brush"];
        	}
        	else {
        		control.Background = (Brush)Resources["Stripe2Brush"];
        	}         	
        	QuestionsPanel.Children.Add(control);    	
    		control.Deleting += new EventHandler<DeletingEventArgs>(questionControl_Deleting);
    		control.Moving += new EventHandler<MovingEventArgs>(questionControl_Moving);
        	control.BringIntoView();
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
    		WorksheetViewer.MoveWithin(e.MovingControl,QuestionsPanel.Children,e.MoveUp);
    		SetBackgrounds();
    		OnChanged(new EventArgs());
    	}
    	    	
    	
    	private void questionControl_Deleting(object sender, DeletingEventArgs e)
    	{
    		if (QuestionsPanel.Children.Contains(e.DeletingControl)) {
    			QuestionsPanel.Children.Remove(e.DeletingControl);
    			SetBackgrounds();
        		OnChanged(new EventArgs());
    		}
    		else {
    			throw new InvalidOperationException("Received instruction to delete a control " + 
    			                                    "( " + e.DeletingControl.ToString() +
    			                                    ") that was not a part of this worksheet.");
    		}
    	}
        
        
        private void OnClick_DeleteSection(object sender, EventArgs e)
        {
        	if (!WorksheetViewer.DesignerMode) {
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
        	QuestionControl control = new QuestionControl(question,true);
        	AddQuestionControl(control);
        	OnChanged(new EventArgs());
        }
        
        
        private void OnClick_AddQuestion(object sender, EventArgs e)
        {
        	if (!WorksheetViewer.DesignerMode) {
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