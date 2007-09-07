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

namespace AdventureAuthor.UI.Controls
{
    /// <summary>
    /// Interaction logic for StringQuestionPanel.xaml
    /// </summary>

    public partial class StringQuestionPanel : UserControl, IQuestionPanel
    {
        public StringQuestionPanel(string question)
        {
            InitializeComponent();
        	QuestionLabel.Text = question;
        }

        public object Answer
        {
        	get {
        		return AnswerBox.Text;
        	}
        }
    }
}
