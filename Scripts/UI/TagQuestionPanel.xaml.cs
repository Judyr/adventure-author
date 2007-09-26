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

namespace AdventureAuthor.Scripts.UI
{
    /// <summary>
    /// Interaction logic for TagQuestionPanel.xaml
    /// </summary>

    public partial class TagQuestionPanel : UserControl, IQuestionPanel
    {
        public TagQuestionPanel(string question, ScriptHelper.ObjectType[] objectTypes)
        {
            InitializeComponent();
            QuestionLabel.Text = question;
            
            List<string> tags = new List<string>();
            foreach (ScriptHelper.ObjectType type in objectTypes) {
            	tags.AddRange(ScriptHelper.GetTags(type));
            }
            AnswerBox.ItemsSource = tags;
        }
        
        public object Answer
        {
			get { 
        		return AnswerBox.SelectedItem;
        	}
		}
    }
}
