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

namespace AdventureAuthor.Evaluation.Viewer
{
    public partial class ReplyControl : OptionalWorksheetPartControl
    {    	
    	#region Constants
    	
    	private static readonly Brush TEACHER_BRUSH = Brushes.Crimson;
    	private static readonly Brush PLAYTESTER_BRUSH = Brushes.Black;
    	private static readonly Brush DESIGNER_BRUSH = Brushes.DarkSlateBlue;
    	private static readonly Brush SOMEONEELSE_BRUSH = Brushes.Black;
    	
    	#endregion
    	
        public ReplyControl(Reply reply)
        {
            InitializeComponent();
            if (reply.ReplierType == Reply.ReplierRole.Other) {
            	replyAuthorTextBlock.Text = reply.Replier;
            }
            else {
            	replyAuthorTextBlock.Text = reply.Replier + " (" + reply.ReplierType.ToString().ToLower() + ")";
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
        	throw new Exception();
//			return new Evidence(filename);
		}
    }
}