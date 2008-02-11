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
using AdventureAuthor.Utils;

namespace AdventureAuthor.Evaluation.Viewer
{
    public partial class ReplyControl : OptionalWorksheetPartControl
    {    	
    	#region Constants
    	
    	private static readonly Brush TEACHER_BRUSH = Brushes.Crimson;
    	private static readonly Brush PLAYTESTER_BRUSH = Brushes.DarkMagenta;
    	private static readonly Brush DESIGNER_BRUSH = Brushes.DarkSlateBlue;
    	private static readonly Brush SOMEONEELSE_BRUSH = Brushes.Black;
    	
    	#endregion
    	
    	#region Fields
    	
    	private Reply.ReplierRole authorType;
    	
    	private string authorName;
    	
    	#endregion
    	
    	#region Events
    	
    	public event EventHandler<OptionalWorksheetPartControlEventArgs> Deleted;
    	
    	protected virtual void OnDeleted(OptionalWorksheetPartControlEventArgs e)
    	{
    		EventHandler<OptionalWorksheetPartControlEventArgs> handler = Deleted;
    		if (handler != null) {
    			handler(this,e);
    		}
    	}
    	
    	#endregion
    	
        public ReplyControl(Reply reply, bool designerMode)
        {
        	// TODO use designerMode variable
        	
            InitializeComponent();
            Open(reply);
        }
        
        
        private void Open(Reply reply)
        {
        	authorName = reply.Replier;
            authorType = reply.ReplierType;
            replyAuthorTextBlock.Text = reply.Replier;
            if (reply.ReplierType != Reply.ReplierRole.Other) {
            	replyAuthorTextBlock.Text = reply.Replier + " (" + reply.ReplierType.ToString().ToLower() + ")";
            }
            else {
            	replyAuthorTextBlock.Text = reply.Replier;
            }
            
            switch (reply.ReplierType) {
            	case Reply.ReplierRole.Designer:
            		replyAuthorTextBlock.Foreground = DESIGNER_BRUSH;        		
            		break;
            	case Reply.ReplierRole.Teacher:
            		replyAuthorTextBlock.Foreground = TEACHER_BRUSH;
            		break;
            	case Reply.ReplierRole.Playtester:
            		replyAuthorTextBlock.Foreground = PLAYTESTER_BRUSH;
            		break;
            	default:
            		replyAuthorTextBlock.Foreground = SOMEONEELSE_BRUSH;
            		break;
            }
            replyBodyTextBlock.Text = reply.Text;
        	OnChanged(new EventArgs());
        }
        
        
        private void OnClick_Delete(object sender, RoutedEventArgs e)
        {
        	if (Tools.TeacherHasSignedIn(false)) {
        		OnDeleted(new OptionalWorksheetPartControlEventArgs(this));
        	}
        }
        
        
        private void OnClick_Edit(object sender, RoutedEventArgs e)
        {
        	if (Tools.TeacherHasSignedIn(false)) {
        		AddReplyWindow window = new AddReplyWindow((Reply)GetWorksheetPart());
        		window.ReplyAdded += new EventHandler<OptionalWorksheetPartEventArgs>(ReplyEdited);
        		window.ShowDialog();
        	}
        }

        
        private void ReplyEdited(object sender, OptionalWorksheetPartEventArgs e)
        {
        	Reply reply = (Reply)e.Part;
        	Open(reply);
        }
		
        
    	protected override void PerformEnable()
    	{    		
//    		ActivatableControl.EnableElement(ButtonsPanel);
    	}
    	
    	    	
    	protected override void PerformActivate()
    	{	
//    		ActivatableControl.ActivateElement(ButtonsPanel);
//    		ActivatableControl.EnableElement(ActivateCheckBox);
//    		if ((bool)!ActivateCheckBox.IsChecked) {
//    			ActivateCheckBox.IsChecked = true;
//    		}
    	}
    	
        
    	protected override void PerformDeactivate(bool parentIsDeactivated)
    	{
//    		ActivatableControl.DeactivateElement(ButtonsPanel);
//    		if ((bool)ActivateCheckBox.IsChecked) {
//    			ActivateCheckBox.IsChecked = false;
//    		}
//    		if (parentIsDeactivated) {
//    			ActivatableControl.DeactivateElement(ActivateCheckBox);
//    		}
    	}
    	
		protected override void ShowActivationControls()
		{
//			ActivateCheckBox.Visibility = Visibility.Visible;
		}
		
		protected override void HideActivationControls()
		{
//			ActivateCheckBox.Visibility = Visibility.Collapsed;
		}
    	
        
        protected override OptionalWorksheetPart GetWorksheetPartObject()
		{
        	Reply reply = new Reply();
        	reply.Replier = authorName;
        	reply.ReplierType = authorType;
        	reply.Text = replyBodyTextBlock.Text;
        	return reply;
		}
    }
}