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
using NWN2Toolset.NWN2.Data;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Scripts.UI
{
    /// <summary>
    /// Interaction logic for StringQuestionPanel.xaml
    /// </summary>

    public partial class IntegerAsStringQuestionPanel : ParameterPanel
    {
    	private CheckType checkType;
    	
		public IntegerAsStringQuestionPanel(string question, CheckType type)
        {
            InitializeComponent();
        	QuestionLabel.Text = question;
        	checkType = type;
        }
        
		public enum CheckType { 
			CheckForSetting, 
			CheckForGetting 
		}
		
        public override object Answer
        {
        	get {    
        		string s = AnswerBox.Text;
        		
        		switch (checkType) {
        			case CheckType.CheckForSetting:
        				if (s == String.Empty) {
		        			
		        		}
        		
        				
        				break;
        			case CheckType.CheckForGetting:
        				
        				
        				break;
        			default:
        				throw new NullReferenceException("IntegerAsStringQuestionPanel hasn't been given a CheckType.");
        		}
        		
        		
        		return null;
        			/* Valid setting entries:
        			"5"     (Set to 5)
					"=-2"   (Set to -2)
					"+3"    (Add 3)
					"+"     (Add 1)
					"++"    (Add 1)
					"-4"    (Subtract 4)
					"-"     (Subtract 1)
					"--"    (Subtract 1)
					
					Valid checking entries:
					  Default is an = check, but you can also specify <,>,or !
					  So an sCheck of "<50" returns TRUE if sVariable is less than 50,
					  an sCheck of "9" or "=9" returns TRUE if sVariable is equal to 9
					  and an sCheck of "!0" returns TRUE if sVariable is not equal to 0.
					*/
					
//					string[] numbers = new string[] {"0","1","2","3","4","5","6","7","8","9"};
//					string[] operations = new string[] {"+","-","=-"};
//					
//					string check = AnswerBox.Text;
//					
//					bool 
//					foreach (string number in numbers) {
//						if (check.StartsWith(number
//					}
//        			
        			
        			
        			
        			// if it doesn't equal <5, >5, 5, -5, ++, +, etc., say an error and return null. TODO
        		
        			
        	}
        }
    }
}
