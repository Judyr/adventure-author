using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.IO;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Evaluation
{
    public partial class SectionControl : CardPartControl
    {     	
    	#region Fields
    	
    	private string previousValue = String.Empty;
        
    	#endregion
    	
    	#region Events
    	
    	public event EventHandler<CardPartControlEventArgs> Deleting;  
    	
		protected virtual void OnDeleting(CardPartControlEventArgs e)
		{
			EventHandler<CardPartControlEventArgs> handler = Deleting;
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
        	properName = "section";
        	
        	if (section == null) {
        		section = new Section();
        	}  	
        	
        	previousValue = section.Title;
        	
            InitializeComponent();   
            
            SectionTitleTextBox.SetText(section.Title);
            
            SectionTitleTextBox.TextEdited += delegate(object sender, TextEditedEventArgs e) {
            	Log.WriteAction(LogAction.edited,"section",e.NewValue);
            };            
            SectionTitleTextBox.TextChanged += delegate { 
            	OnChanged(new EventArgs()); 
            };
            
            SetInitialActiveStatus(section); 
            
        	foreach (Question question in section.Questions) {
        		if (CardViewer.Instance.EvaluationMode == Mode.Designer || question.Include) {	
		        	AddQuestionField(question);
        		}
        	}
                           
            if (CardViewer.Instance.EvaluationMode == Mode.Designer) {
            	AddQuestionButton.Visibility = Visibility.Visible;
            	DeleteSectionButton.Visibility = Visibility.Visible;
            	MoveSectionDownButton.Visibility = Visibility.Visible;
            	MoveSectionUpButton.Visibility = Visibility.Visible;
            }
            
            string imagePath = Path.Combine(EvaluationPreferences.Instance.InstallDirectory,"delete.png");
            Tools.SetXAMLButtonImage(DeleteSectionButton,imagePath,"delete");
            imagePath = Path.Combine(EvaluationPreferences.Instance.InstallDirectory,"07.png");
            Tools.SetXAMLButtonImage(MoveSectionDownButton,imagePath,"down");
            imagePath = Path.Combine(EvaluationPreferences.Instance.InstallDirectory,"08.png");
            Tools.SetXAMLButtonImage(MoveSectionUpButton,imagePath,"up");
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
    		control.Deleting += new EventHandler<CardPartControlEventArgs>(questionControl_Deleting);
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
    		CardViewer.MoveWithin(e.Control,QuestionsPanel.Children,e.MoveUp);
    		SetBackgrounds();
    	}
    	    	
    	
    	private void questionControl_Deleting(object sender, CardPartControlEventArgs e)
    	{
    		CardViewer.DeleteFrom(e.Control,QuestionsPanel.Children);
    		SetBackgrounds();
    	}
        
        
        private void OnClick_DeleteSection(object sender, EventArgs e)
        {
        	if (CardViewer.Instance.EvaluationMode != Mode.Designer) {
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
        		Log.WriteAction(LogAction.deleted,"section");
        		OnDeleting(new CardPartControlEventArgs(this));
        	}
        }        
        
        
        private void OnClick_MoveUp(object sender, EventArgs e)
        {
        	Log.WriteAction(LogAction.moved,"section","up");
        	OnMoving(new MovingEventArgs(this,true));
        }
        
        
        private void OnClick_MoveDown(object sender, EventArgs e)
        {
        	Log.WriteAction(LogAction.moved,"section","down");
        	OnMoving(new MovingEventArgs(this,false));
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
        	if (CardViewer.Instance.EvaluationMode != Mode.Designer) {
        		throw new InvalidOperationException("Should not have been possible to call Add Question " +
        		                                    "when not in designer mode.");
        	}
        	
        	AddNewQuestion();
        }
        
        
    	protected override void PerformEnable()
    	{    		
    		// DON'T enable the title - otherwise it's editable outside of designer mode:
    		SectionTitleTextBox.IsEditable = false; //Tools.PreventEditingOfTextBox(SectionTitleTextBox);
    		SectionTitleTextBox.Opacity = 1.0f;
    		ActivatableControl.EnableElement(ActivateCheckBox);
    		EnableChildren();
    	}
    	
    	
    	protected void EnableChildren()
    	{
    		foreach (UIElement element in QuestionsPanel.Children) {
    			CardPartControl part = element as CardPartControl;
    			if (part != null) {
    				part.Enable();
    			}
    		}
    	}
    	    	
    	
    	protected override void PerformActivate()
    	{	    		
    		SectionTitleTextBox.Opacity = 1.0f;
    		SectionTitleTextBox.IsEnabled = true;
    		SectionTitleTextBox.IsEditable = true;
    		ActivatableControl.EnableElement(ActivateCheckBox);
    		ActivatableControl.EnableElement(AddQuestionButton);
    		ActivatableControl.EnableElement(MoveSectionDownButton);
    		ActivatableControl.EnableElement(MoveSectionUpButton);
    		ActivatableControl.EnableElement(DeleteSectionButton);    		
    		ActivateChildren();
    		if ((bool)!ActivateCheckBox.IsChecked) {
    			ActivateCheckBox.IsChecked = true;
    		}
    		ActivateCheckBox.ToolTip = "Click to deactivate this section.\n(Won't appear to users filling out this Comment Card.)";
    	}
    	
    	
    	protected void ActivateChildren()
    	{
    		foreach (UIElement element in QuestionsPanel.Children) {
    			CardPartControl part = element as CardPartControl;
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
    		ActivateCheckBox.ToolTip = "Click to activate this section.\n(Will appear to users filling out this Comment Card.)";
    	}
    	
    	
    	protected void DeactivateChildren()
    	{
    		foreach (UIElement element in QuestionsPanel.Children) {
    			CardPartControl part = element as CardPartControl;
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
        
        
        protected override CardPart GetCardPartObject()
        {
        	Section section = new Section(SectionTitleTextBox.Text);   			
   			foreach (UIElement element in QuestionsPanel.Children) {
   				QuestionControl qc = element as QuestionControl;
   				if (qc != null) {
   					Question question = (Question)qc.GetCardPart();
   					section.Questions.Add(question);
   				}
   			}   			
   			return section;
        }	
    }
}