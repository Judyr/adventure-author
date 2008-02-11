﻿using System;
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
    	#region Constants
    	
    	private string[] REPLIER_TYPES = new string[4] { "a teacher", 
												    	 "a playtester", 
												   		 "the game's designer", 
												   		 "someone else" };
    	
    	#endregion
    	
    	#region Events
    	
    	public EventHandler<OptionalWorksheetPartEventArgs> ReplyAdded;
    	
    	protected virtual void OnReplyAdded(OptionalWorksheetPartEventArgs e)
    	{
    		EventHandler<OptionalWorksheetPartEventArgs> handler = ReplyAdded;
    		if (handler != null) {
    			handler(this,e);
    		}
    	}
    	
    	#endregion
    	
        public AddReplyWindow()
        {
            InitializeComponent();
            foreach (string role in REPLIER_TYPES) {
            	RespondantRoleComboBox.Items.Add(role);
            }
        }
        
        
        public AddReplyWindow(Reply editingReply) : this()
        {
        	NameTextBox.Text = editingReply.Replier;
        	ReplyTextBox.Text = editingReply.Text;
        	switch (editingReply.ReplierType) {
        		case Reply.ReplierRole.Designer:
        			RespondantRoleComboBox.SelectedItem = "the game's designer";
        			break;
        		case Reply.ReplierRole.Playtester:
        			RespondantRoleComboBox.SelectedItem = "a playtester";
        			break;
        		case Reply.ReplierRole.Teacher:
        			RespondantRoleComboBox.SelectedItem = "a teacher";
        			break;
        		case Reply.ReplierRole.Other:
        			RespondantRoleComboBox.SelectedItem = "someone else";
        			break;
        	}
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
	        	
	        	// Check that the user is a teacher before allowing them to add a teacher response:
	        	if (reply.ReplierType == Reply.ReplierRole.Teacher && !Tools.TeacherHasSignedIn(false)) {
	        		return;
	        	}
	        	
	        	OnReplyAdded(new OptionalWorksheetPartEventArgs(reply));
	        	Close();
        	}
        }

        
        private void OnClick_Cancel(object sender, EventArgs e)
        {
        	Close();
        }
    }
}