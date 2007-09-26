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

namespace AdventureAuthor.Scripts.UI
{
    public partial class FloatQuestionPanel : UserControl, IQuestionPanel
    {
    	int? min, max;
    	
        public FloatQuestionPanel(string question)
        {
            InitializeComponent();
        	QuestionLabel.Text = question;
        }

        public FloatQuestionPanel(string question, int? min, int? max) : this(question)
        {
        	this.min = min;
        	this.max = max;
        }
        
        public object Answer
        {
			get { 
        		try {
        			float floatresult = float.Parse(AnswerBox.Text);
        			int result = (int)floatresult;
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
        				return (float)result; // return a float with nothing after the decimal point
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
