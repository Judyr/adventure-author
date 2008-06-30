using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Evaluation
{
    public partial class QuestionControl : CardPartControl
    {    	
    	#region Fields
    	
    	private string previousValue;
    	
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
    	
    	
    	
    	
    	
        public QuestionControl(Question question)
        {
        	properName = "question";
        	
        	if (question == null) {
        		question = new Question();
        	}
        	
        	previousValue = question.Text;
        	
            InitializeComponent(); 
            this.QuestionTitle.SetText(question.Text);
            
            QuestionTitle.TextEdited += delegate(object sender, TextEditedEventArgs e) {
            	Log.WriteAction(LogAction.edited,"question",e.NewValue);
            };            
            QuestionTitle.TextChanged += delegate { 
            	OnChanged(new EventArgs()); 
            };
            
            SetInitialActiveStatus(question);
            
		    foreach (Answer answer in question.Answers) {
		    	if (CardViewer.Instance.EvaluationMode == Mode.Designer || answer.Include) {
		    		AddAnswerField(answer);
		    	}
		    }
            
            foreach (Reply reply in question.Replies) {
            	if (CardViewer.Instance.EvaluationMode != Mode.Designer) {
            		AddReplyField(reply);
            	}
            }
                 
            if (CardViewer.Instance.EvaluationMode == Mode.Designer) {
            	ControlPanel.Visibility = Visibility.Visible;
            	ControlPanel.Width = 80;
            	ControlPanel.MaxWidth = 80;
            	QuestionAndControlsColumn.MaxWidth = 500;
            	QuestionAndControlsColumn.Width = new GridLength(500);
            }
            else {
            	ControlPanel.Visibility = Visibility.Collapsed;
            	ControlPanel.Width = 0;
            	ControlPanel.MaxWidth = 0;
            	QuestionAndControlsColumn.MaxWidth = 350;
            	QuestionAndControlsColumn.Width = new GridLength(350);
            }
            	
            if (CardViewer.Instance.EvaluationMode == Mode.User_Discuss) {
            	AddReplyButton.Visibility = Visibility.Visible;
            }
            else {
            	AddReplyButton.Visibility = Visibility.Collapsed;
            }
            
            string imagePath;
            imagePath = Path.Combine(EvaluationPreferences.Instance.InstallDirectory,"delete.png");
            Tools.SetXAMLButtonImage(DeleteQuestionButton,imagePath,"delete");
            imagePath = Path.Combine(EvaluationPreferences.Instance.InstallDirectory,"07.png");
            Tools.SetXAMLButtonImage(MoveDownButton,imagePath,"down");
            imagePath = Path.Combine(EvaluationPreferences.Instance.InstallDirectory,"08.png");
            Tools.SetXAMLButtonImage(MoveUpButton,imagePath,"up");
            imagePath = Path.Combine(EvaluationPreferences.Instance.InstallDirectory,"29.png");
            Tools.SetXAMLButtonImage(AddReplyButton,imagePath,"reply");
        }

        
        private void AddAnswerField(CardPartControl control)
        {
        	if (control == null) {
        		throw new ArgumentNullException("Can't add a null answer field.");
        	}            	
        	AnswersPanel.Children.Add(control);
        	control.Changed += delegate { OnChanged(new EventArgs()); };
        	control.Activated += delegate { OnChanged(new EventArgs()); };
        	control.Deactivated += delegate { OnChanged(new EventArgs()); };
        	OnChanged(new EventArgs());
        }
			        
			
        public void AddAnswerField(Answer answer)
        {
        	if (answer == null) {
        		throw new ArgumentNullException("Can't add a null answer field.");
        	}
        	
        	CardPartControl control = answer.GetControl();
        	AddAnswerField(control);
        }

        
        private void replyControl_Deleted(object sender, CardPartControlEventArgs e)
        {
        	if (e.Control == null) {
        		throw new ArgumentNullException("Tried to delete a null reply.");
        	}
        	else if (!RepliesPanel.Children.Contains(e.Control)) {
        		throw new ArgumentNullException("Tried to delete a reply that did not exist in this question.");
        	}
        	
        	RepliesPanel.Children.Remove((UIElement)e.Control);
        	OnChanged(new EventArgs());
        }
        
        
        private void AddReplyField(ReplyControl control)
        {
        	if (control == null) {
        		throw new ArgumentNullException("Can't add a null reply field.");
        	}            	
        	RepliesPanel.Children.Add(control);
        	control.Changed += delegate { OnChanged(new EventArgs()); };
        	control.Deleted += new EventHandler<CardPartControlEventArgs>(replyControl_Deleted);
        	control.Edited += delegate(object sender, EventArgs e) {
        		ShowReplyDialog((ReplyControl)sender);
        	};
        	OnChanged(new EventArgs());
        }
			        
			
        public void AddReplyField(Reply reply)
        {
        	if (reply == null) {
        		throw new ArgumentNullException("Can't add a null reply field.");
        	}
        	
        	ReplyControl control = (ReplyControl)reply.GetControl();
        	AddReplyField(control);
        }
        
        
//        private void OnChecked(object sender, EventArgs e)
//        {
//    		Log.WriteAction(LogAction.activated,"question");
//        	Activate();
//        }
//        
//        
//        private void OnUnchecked(object sender, EventArgs e)
//        {
//    		Log.WriteAction(LogAction.deactivated,"question");
//        	Deactivate(false);
//        }
        
        
        private void OnClick_DeleteQuestion(object sender, EventArgs e)
        {
        	if (CardViewer.Instance.EvaluationMode != Mode.Designer) {
        		throw new InvalidOperationException("Should not have been possible to try to delete a question.");
        	}
        	
        	MessageBoxResult result = MessageBox.Show("Are you sure you want to permanently delete this question?",
        	                                          "Delete question?",
        	                                          MessageBoxButton.OKCancel,
        	                                          MessageBoxImage.Warning,
        	                                          MessageBoxResult.Cancel,
        	                                          MessageBoxOptions.None);
        	if (result == MessageBoxResult.OK) {
        		Log.WriteAction(LogAction.deleted,"question");
        		OnDeleting(new CardPartControlEventArgs(this));
        	}
        }    
        
        
        private void OnClick_MoveUp(object sender, EventArgs e)
        {
        	Log.WriteAction(LogAction.moved,"question","up");
        	OnMoving(new MovingEventArgs(this,true));
        }
        
        
        private void OnClick_MoveDown(object sender, EventArgs e)
        {
        	Log.WriteAction(LogAction.moved,"question","down");
        	OnMoving(new MovingEventArgs(this,false));
        }

        
        private void OnClick_AddReply(object sender, EventArgs e)
        {
        	ShowReplyDialog();
        }
        
        
        private void ShowReplyDialog()
        {
        	ShowReplyDialog(null);
        }
        
        
        private void ShowReplyDialog(ReplyControl replyControl)
        {
        	AddReplyWindow window;
        	if (replyControl != null) { // editing an existing reply
        		window = new AddReplyWindow(replyControl.Reply);
        		window.ReplyEdited += delegate(object sender, CardPartEventArgs e) { 
        			Reply reply = (Reply)e.Part;
        			Log.WriteAction(LogAction.edited,"reply",reply.ToString());
        			replyControl.Reply = reply;
        		};
        	}
        	else { // adding a new reply
        		window = new AddReplyWindow();
        		window.ReplyEdited += delegate(object sender, CardPartEventArgs e) { 
        			Reply reply = (Reply)e.Part;
        			Log.WriteAction(LogAction.added,"reply",reply.ToString());
        			AddReplyField(reply);
        		};
        	}
        	Brush background = Background;
        	Background = Brushes.LightSalmon;
        	window.ShowDialog();
        	Background = background;
        }
        
        
    	protected override void PerformEnable()
    	{    		
    		QuestionTitle.IsEditable = false;//    		Tools.PreventEditingOfTextBox(QuestionTitle);
    		QuestionTitle.Opacity = 1.0f;
    		ActivatableControl.EnableElement(ActivateCheckBox);
    		EnableChildren();
    	}
    	
    	
    	protected void EnableChildren()
    	{
    		foreach (UIElement element in AnswersPanel.Children) {
    			CardPartControl part = element as CardPartControl;
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
    		ActivateCheckBox.ToolTip = "Click to deactivate this question.\n(Won't appear to users filling out this Comment Card.)";
    	}
    	
    	
    	protected void ActivateChildren()
    	{
    		foreach (UIElement element in AnswersPanel.Children) {
    			CardPartControl part = element as CardPartControl;
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
    		ActivateCheckBox.ToolTip = "Click to activate this question.\n(Will appear to users filling out this Comment Card.)";
    	}
    	
    	
    	protected void DeactivateChildren()
    	{
    		foreach (UIElement element in AnswersPanel.Children) {
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
        	Question question = new Question(QuestionTitle.Text);
        	foreach (UIElement element in AnswersPanel.Children) {
        		CardPartControl control = element as CardPartControl;
        		if (control != null) {      
        			Answer answer = (Answer)control.GetCardPart();
        			question.Answers.Add(answer);        			
        		}
        	}
        	foreach (UIElement element in RepliesPanel.Children) {
        		CardPartControl control = element as CardPartControl;
        		if (control != null) {      
        			Reply reply = (Reply)control.GetCardPart();
        			question.Replies.Add(reply);        			
        		}
        	}
        	return question;
        }
    }
}