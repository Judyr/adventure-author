using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Evaluation
{
    public partial class CommentControl : CardPartControl
    {    	
    	private string previousValue;
    	
    	
    	public CommentControl(Comment comment)
        {    		
        	properName = "comment";
        	
        	if (comment == null) {
    			comment = new Comment();
    		}
        	
        	previousValue = comment.Value;
    		
            InitializeComponent();          
            CommentTextBox.SetTextWithoutRaisingEvent(comment.Value);
            SetInitialActiveStatus(comment);
            
            CommentTextBox.TextEdited += delegate(object sender, TextEditedEventArgs e) {
            	Log.WriteAction(LogAction.edited,"comment",e.NewValue);
            };            
            CommentTextBox.TextChanged += delegate { 
            	OnChanged(new EventArgs()); 
            };
            
            ToolTip = "Write your answer here.";
        }
        
        
//        private void OnChecked(object sender, EventArgs e)
//        {
//    		Log.WriteAction(LogAction.activated,"comment");
//        	Activate();
//        }
//        
//        
//        private void OnUnchecked(object sender, EventArgs e)
//        {
//    		Log.WriteAction(LogAction.deactivated,"comment");
//        	Deactivate(false);
//        }

        
    	protected override void PerformEnable()
    	{    		
    		ActivatableControl.EnableElement(CommentPanel);
    	}
    	
    	
    	protected override void PerformActivate()
    	{	
    		ActivatableControl.ActivateElement(CommentPanel);
    		ActivatableControl.EnableElement(ActivateCheckBox);
    		if ((bool)!ActivateCheckBox.IsChecked) {
    			ActivateCheckBox.IsChecked = true;
    		}
    		ActivateCheckBox.ToolTip = "Click to hide this answer field.";
    	}
    	
        
    	protected override void PerformDeactivate(bool parentIsDeactivated)
    	{
    		ActivatableControl.DeactivateElement(CommentPanel);
    		if ((bool)ActivateCheckBox.IsChecked) {
    			ActivateCheckBox.IsChecked = false;
    		}
    		if (parentIsDeactivated) {
    			ActivatableControl.DeactivateElement(ActivateCheckBox);
    		}
    		ActivateCheckBox.ToolTip = "Click to show this answer field.";
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
			return new Comment(CommentTextBox.Text);
		}
    }
}