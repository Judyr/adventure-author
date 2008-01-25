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
            
            if (designerMode) { // show 'Active?' control, and assume that control is Active to begin with
            	ActivateCheckBox.Visibility = Visibility.Visible;
	    		if (comment.Include) {
	    			ActivationStatus = ControlStatus.Active;
	    		}
	    		else {
	    			ActivationStatus = ControlStatus.Inactive;
	    		}
            }
            else { // hide 'Active?' control, and set status to Not Applicable
        		ActivationStatus = ControlStatus.NA;
            }
            
            CommentTextBox.TextChanged += delegate { OnChanged(new EventArgs()); };
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
    		ActivatableControl.Enable(CommentPanel);
    	}
    	
    	
    	protected override void Activate()
    	{	
    		ActivatableControl.Activate(CommentPanel);
    		if ((bool)!ActivateCheckBox.IsChecked) {
    			ActivateCheckBox.IsChecked = true;
    		}
    	}
    	
        
    	protected override void Deactivate()
    	{
    		ActivatableControl.Deactivate(CommentPanel);
    		if ((bool)ActivateCheckBox.IsChecked) {
    			ActivateCheckBox.IsChecked = false;
    		}
    	}
    	
        
        protected override OptionalWorksheetPart GetWorksheetPartObject()
        {
			return new Comment(CommentTextBox.Text);
		}
        
        
        protected override List<Control> GetActivationControls()
        {
        	List<Control> activationControls = new List<Control>(1);
        	activationControls.Add(ActivateCheckBox);
        	return activationControls;
        }
    }
}