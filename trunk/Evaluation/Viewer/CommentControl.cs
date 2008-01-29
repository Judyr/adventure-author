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
            
            if (designerMode) { // show 'Active?' control
            	ActivateCheckBox.Visibility = Visibility.Visible;
	    		if (comment.Include) {
            		Activate();
	    		}
	    		else {
            		Deactivate(false);
	    		}
            }
            else { // hide 'Active?' control
            	Enable();
            }
            
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
    	
        
    	protected override void PerformDeactivate(bool preventReactivation)
    	{
    		ActivatableControl.DeactivateElement(CommentPanel);
    		if ((bool)ActivateCheckBox.IsChecked) {
    			ActivateCheckBox.IsChecked = false;
    		}
    		if (preventReactivation) {
    			ActivatableControl.DeactivateElement(ActivateCheckBox);
    		}
    	}
    	
        
        protected override OptionalWorksheetPart GetWorksheetPartObject()
        {
			return new Comment(CommentTextBox.Text);
		}
    }
}