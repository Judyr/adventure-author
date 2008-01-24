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
using AdventureAuthor.Evaluation.Viewer;

namespace AdventureAuthor.Evaluation.Viewer
{
    public partial class CommentBox : AnswerControl
    {    	
        public CommentBox()
        {
            InitializeComponent();
            CommentTextBox.TextChanged += delegate { OnAnswerChanged(new EventArgs()); };
        }
        
        
        public CommentBox(string text) : this()
        {
        	CommentTextBox.Text = text;
        }
        
        
        public CommentBox(Comment comment) : this(comment.Value)
        {        	
        }

        
    	protected override void Enable()
    	{    		
//    		if (!StarsPanel.IsEnabled) {
//    			StarsPanel.IsEnabled = true;
//    		}
//    		StarsPanel.Opacity = 1.0f;
    	}
    	
    	
    	protected override void Activate()
    	{	
//    		if (StarsPanel.IsEnabled) {
//    			StarsPanel.IsEnabled = false;
//    		}
//    		if ((bool)!ActivateCheckBox.IsChecked) {
//    			ActivateCheckBox.IsChecked = true;
//    		}
//    		StarsPanel.Opacity = 1.0f;
    	}
    	
        
    	protected override void Deactivate()
    	{
//    		if (StarsPanel.IsEnabled) {
//    			StarsPanel.IsEnabled = false;
//    		}
//    		if ((bool)ActivateCheckBox.IsChecked) {
//    			ActivateCheckBox.IsChecked = false;
//    		}
//    		StarsPanel.Opacity = 0.2f;
    	}
    	
        
        protected override Answer GetAnswerObject()
        {
			return new Comment(CommentTextBox.Text);
		}
    }
}