using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Evaluation.Viewer
{
    public partial class CommentControl : OptionalWorksheetPartControl
    {    	
    	private string previousValue;
    	
    	
    	public CommentControl(Comment comment)
        {    		
        	properName = "Comment";
        	
        	if (comment == null) {
    			comment = new Comment();
    		}
        	
        	previousValue = comment.Value;
    		
            InitializeComponent();          
            CommentTextBox.SetText(comment.Value);
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
    		ActivateCheckBox.ToolTip = "Click to deactivate this answer field\n(will not appear in worksheet)";
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
    		ActivateCheckBox.ToolTip = "Click to activate this answer field\n(will appear in worksheet)";
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
			return new Comment(CommentTextBox.Text);
		}
    }
}