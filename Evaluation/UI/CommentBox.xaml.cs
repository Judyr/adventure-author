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
using System.Windows.Navigation;
using System.Windows.Shapes;
using AdventureAuthor.Evaluation.UI;

namespace AdventureAuthor.Evaluation.UI
{
    public partial class CommentBox : IAnswerControl
    {
        public CommentBox()
        {
            InitializeComponent();
        }
        
        
        public CommentBox(string text) : this()
        {
        	CommentTextBox.Text = text;
        }
        
        
        public CommentBox(Comment comment) : this(comment.Value)
        {        	
        }

        
        public Answer GetAnswer()
        {
			return new Comment(CommentTextBox.Text);
		}
    }
}