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
using System.Windows.Shapes;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Evaluation.Viewer
{
    /// <summary>
    /// Interaction logic for AddReplyWindow.xaml
    /// </summary>

    public partial class AddReplyWindow : Window
    {
    	#region Events
    	
    	public EventHandler<ReplyAddedEventArgs> ReplyAdded;
    	
    	protected virtual void OnReplyAdded(ReplyAddedEventArgs e)
    	{
    		EventHandler<ReplyAddedEventArgs> handler = ReplyAdded;
    		if (handler != null) {
    			handler(this,e);
    		}
    	}
    	
    	#endregion
    	
        public AddReplyWindow()
        {
            InitializeComponent();
            RespondantRoleComboBox.Items.Add("a teacher");
            RespondantRoleComboBox.Items.Add("a playtester");
            RespondantRoleComboBox.Items.Add("the game's designer");
            RespondantRoleComboBox.Items.Add("someone else");
        }

        
        private void OnClick_OK(object sender, EventArgs e)
        {
        	if (NameTextBox.Text == String.Empty) {
        		Say.Warning("You forgot to enter your name!");
        	}
        	else if (RespondantRoleComboBox.SelectedItem == null) {
        		Say.Warning("Please select one of: \"I am... a teacher/a playtester/the game's designer/someone else.\"");        		
        	}
        	else if (ReplyTextBox.Text == String.Empty) {
        		Say.Warning("You forgot to enter your reply!");
        	}
        	else {        	
	        	Reply reply = new Reply();
	        	reply.Replier = NameTextBox.Text;
	        	reply.Text = ReplyTextBox.Text;
	        	if (RespondantRoleComboBox.Text == "a teacher") {
	        		reply.ReplierType = Reply.ReplierRole.Teacher;
	        	}
	        	else if (RespondantRoleComboBox.Text == "a playtester") {
	        		reply.ReplierType = Reply.ReplierRole.Playtester;
	        	}
	        	else if (RespondantRoleComboBox.Text == "the game's designer") {
	        		reply.ReplierType = Reply.ReplierRole.Designer;
	        	}
	        	else if (RespondantRoleComboBox.Text == "someone else") {
	        		reply.ReplierType = Reply.ReplierRole.Other;
	        	}
	        	
	        	OnReplyAdded(new ReplyAddedEventArgs(reply));
	        	Close();
        	}
        }

        
        private void OnClick_Cancel(object sender, EventArgs e)
        {
        	Close();
        }
    }
}