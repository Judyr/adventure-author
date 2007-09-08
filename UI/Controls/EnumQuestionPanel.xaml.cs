using System;
using System.Collections.Generic;
using System.Reflection;
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

namespace AdventureAuthor.UI.Controls
{
    /// <summary>
    /// Interaction logic for EnumQuestionPanel.xaml
    /// </summary>

    public partial class EnumQuestionPanel : UserControl, IQuestionPanel
    {
    	private Type enumType;
    	
        public EnumQuestionPanel(string question, Type enumType)
        {
        	if (!enumType.IsEnum) {
        		throw new ArgumentException("Tried to create an EnumQuestionPanel with a type that was not an enum (type was " +
        		                            enumType.GetType().ToString() + ").");
        	}
            InitializeComponent();
            QuestionLabel.Text = question;  
            this.enumType = enumType;
            
            string[] enumNames = BindingFlags.GetNames(enumType);            
            AnswerBox.ItemsSource = enumNames;
        }
        
        public object Answer
        {
			get {   
        		return Enum.Parse(enumType,(string)AnswerBox.SelectedItem,true);
        	}
		}
    }
}
