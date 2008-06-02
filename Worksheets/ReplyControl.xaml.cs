using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Evaluation
{
    public partial class ReplyControl : OptionalWorksheetPartControl
    {    	
    	#region Constants
    	
    	private static readonly Brush TEACHER_BRUSH = Brushes.Crimson;
    	private static readonly Brush PLAYTESTER_BRUSH = Brushes.Black;
    	private static readonly Brush DESIGNER_BRUSH = Brushes.DarkSlateBlue;
    	private static readonly Brush SOMEONEELSE_BRUSH = Brushes.Black;
    	
    	#endregion
    	
    	#region Fields
    	
    	private Role authorType;
    	
    	private string authorName;
    	
    	private Reply reply;
		public Reply Reply {
			get { return reply; }
			set { 
				reply = value; 
				Open(reply);
			}
		}
    	
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
    	
    	
    	public event EventHandler Edited;    	
    	protected virtual void OnEdited(EventArgs e)
    	{
    		EventHandler handler = Edited;
    		if (handler != null) {
    			handler(this,e);
    		}
    	}
    	
    	#endregion
    	
        public ReplyControl(Reply reply)
        {
            InitializeComponent();
            if (WorksheetViewer.Instance.EvaluationMode == Mode.Complete) {
            	DeleteButton.Visibility = Visibility.Collapsed;
            	EditButton.Visibility = Visibility.Collapsed;
            }            
            Open(reply);
            
            string imagePath = Path.Combine(WorksheetPreferences.Instance.InstallDirectory,"delete.png");
            Tools.SetXAMLButtonImage(DeleteButton,imagePath,"delete");
            DeleteButton.ToolTip = "Delete this comment\n(teachers only)";
            EditButton.ToolTip = "Edit this comment\n(teachers only)";
        }
        
        
        private void Open(Reply reply)
        {
        	authorName = reply.Replier;
            authorType = reply.ReplierType;
            replyAuthorTextBlock.Text = reply.Replier;
            if (reply.ReplierType != Role.Other) {
            	replyAuthorTextBlock.Text = reply.Replier + " (" + reply.ReplierType.ToString().ToLower() + ")";
            }
            else {
            	replyAuthorTextBlock.Text = reply.Replier;
            }
            
            switch (reply.ReplierType) {
            	case Role.Designer:
            		replyAuthorTextBlock.Foreground = DESIGNER_BRUSH;        		
            		break;
            	case Role.Teacher:
            		replyAuthorTextBlock.Foreground = TEACHER_BRUSH;
            		break;
            	case Role.Playtester:
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
        	if (User.IdentifyTeacherOrDemandPassword()) {
        		Log.WriteAction(LogAction.deleted,"reply",GetWorksheetPart().ToString());
        		OnDeleted(new OptionalWorksheetPartControlEventArgs(this));
        	}
        }
        
        
        private void OnClick_Edit(object sender, RoutedEventArgs e)
        {
        	if (User.IdentifyTeacherOrDemandPassword()) {
        		OnEdited(new EventArgs());
        	}
        }
		
        
        #region Redundant
        
    	protected override void PerformEnable()
    	{    		
    	}
    	
    	    	
    	protected override void PerformActivate()
    	{	
    	}
    	
        
    	protected override void PerformDeactivate(bool parentIsDeactivated)
    	{
    	}
    	
    	
		public override void ShowActivationControls()
		{
		}
		
		
		public override void HideActivationControls()
		{
		}
		
		#endregion
    	
		
		public void ShowEditControls()
		{
			EditButton.Visibility = Visibility.Visible;
			DeleteButton.Visibility = Visibility.Visible;
		}
		
		
		public void HideEditControls()
		{
			EditButton.Visibility = Visibility.Collapsed;
			DeleteButton.Visibility = Visibility.Collapsed;
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