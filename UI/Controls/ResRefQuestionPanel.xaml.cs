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
using AdventureAuthor.Scripts;

namespace AdventureAuthor.UI.Controls
{
    public partial class ResRefQuestionPanel : UserControl, IQuestionPanel
    {
        public ResRefQuestionPanel(string question, ScriptHelper.ObjectType[] objectTypes)
        {
            InitializeComponent();
            QuestionLabel.Text = question;
            
            List<string> resrefs = new List<string>();
            foreach (ScriptHelper.ObjectType type in objectTypes) {
            	resrefs.AddRange(ScriptHelper.GetResRefs(type));
            }
            AnswerBox.ItemsSource = resrefs;
        }
        
        public object Answer
        {
			get { 
        		return AnswerBox.SelectedItem;
        	}
		}
    }
}
