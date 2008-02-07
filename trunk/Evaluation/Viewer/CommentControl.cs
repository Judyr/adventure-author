using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;

namespace AdventureAuthor.Evaluation.Viewer
{
    public partial class CommentControl : OptionalWorksheetPartControl
    {    	
    	public CommentControl(Comment comment, bool designerMode)
        {    		
        	if (comment == null) {
    			comment = new Comment();
    		}
    		
            InitializeComponent();          
            CommentTextBox.Text = comment.Value;
            SetInitialActiveStatus(comment);
            
            CommentTextBox.TextChanged += delegate { OnChanged(new EventArgs()); };
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
    		ActivatableControl.EnableElement(CommentPanel);
    	}
    	
    	
    	protected override void PerformActivate()
    	{	
    		ActivatableControl.ActivateElement(CommentPanel);
    		ActivatableControl.EnableElement(ActivateCheckBox);
    		if ((bool)!ActivateCheckBox.IsChecked) {
    			ActivateCheckBox.IsChecked = true;
    		}
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
			return new Comment(CommentTextBox.Text);
		}
    }
}