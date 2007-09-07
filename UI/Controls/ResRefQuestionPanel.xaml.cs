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

namespace AdventureAuthor.UI.Controls
{
    public partial class ResRefQuestionPanel : UserControl, IQuestionPanel
    {
        public ResRefQuestionPanel(string question, TagHelper.ObjectType[] objectTypes)
        {
            InitializeComponent();
            QuestionLabel.Text = question;
            
            List<string> resrefs = new List<string>();
            foreach (TagHelper.ObjectType type in objectTypes) {
            	resrefs.AddRange(TagHelper.GetResRefs(type));
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
