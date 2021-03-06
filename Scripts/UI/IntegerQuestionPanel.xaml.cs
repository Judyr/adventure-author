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
using AdventureAuthor.Utils;

namespace AdventureAuthor.Scripts.UI
{
	/// <summary>
	/// Ask a question which will be answered by an integer value.
	/// </summary>
    public partial class IntegerQuestionPanel : ParameterPanel
    {
    	/// <summary>
    	/// The minimum allowed value for this answer
    	/// </summary>
    	int? min;
    	
    	/// <summary>
    	/// The maximum allowed value for this answer
    	/// </summary>
    	int? max;
    	
    	    	
    	/// <summary>
    	/// Create a question panel which will be answered by an int value.
    	/// </summary>
    	/// <param name="question">The question to ask the user</param>
        public IntegerQuestionPanel(string question)
        {
            InitializeComponent();
        	QuestionLabel.Text = question;
        }
        
        
    	/// <summary>
    	/// Create a question panel which will be answered by an int value.
    	/// </summary>
    	/// <param name="question">The question to ask the user</param>
    	/// <param name="min">The minimum allowed value for this answer</param>
    	/// <param name="max">The maximum allowed value for this answer</param>
        public IntegerQuestionPanel(string question, int? min, int? max) : this(question)
        {
        	this.min = min;
        	this.max = max;
        }
        
                     
    	/// <summary>
    	/// Create a question panel which will be answered by an int value.
    	/// </summary>
    	/// <param name="question">The question to ask the user</param>
    	/// <param name="min">The minimum allowed value for this answer</param>
    	/// <param name="max">The maximum allowed value for this answer</param>
        /// <param name="defaultValue">The default value of this answer on loading the window</param>
        public IntegerQuestionPanel(string question, int? min, int? max, int defaultValue) : this(question,min,max)
        {
        	AnswerBox.Text = defaultValue.ToString();
        }
        
        
        /// <summary>
        /// Returns an object representing an answer to the question posed by this panel - the type of object depends on the type of question.
        /// </summary>
        public override object Answer
        {
			get { 
        		try {
        			int result = int.Parse(AnswerBox.Text);
        			if (min != null && result < min || max != null && result > max) {
        				if (min != null && max != null) {
        					Say.Warning("Enter a number between " + min + " and " + max + ".");
        				}
        				else if (min != null) {
        					Say.Warning("Enter a number equal to or higher than " + min + ".");
        				}
        				else {
        					Say.Warning("Enter a number equal to or lower than " + max + ".");
        				}
        				return null;
        			}
        			else {
        				return result;
        			}        			
        		}
        		catch (FormatException) {
        			if (AnswerBox.Text == String.Empty) {
        				Say.Error("You didn't enter a number.");
        			}
	        		else {
	        			Say.Error(AnswerBox.Text + " is not a valid number.");
        			}
        			return null;
        		}
        		catch (ArgumentNullException) {
        			Say.Error("You didn't enter a number.");
        			return null;
        		}
        		catch (OverflowException) {
        			Say.Error("Your number was too big! Try entering a smaller number.");
        			return null;
        		}
        	}
		}
    }
}
