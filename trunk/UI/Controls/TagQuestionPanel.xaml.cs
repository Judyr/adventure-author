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
    /// <summary>
    /// Interaction logic for TagQuestionPanel.xaml
    /// </summary>

    public partial class TagQuestionPanel : UserControl, IQuestionPanel
    {
        public TagQuestionPanel(string question, TagHelper.TagType[] tagTypes)
        {
            InitializeComponent();
            QuestionLabel.Content = question;
            
            List<string> tags = new List<string>();
            foreach (TagHelper.TagType type in tagTypes) {
            	tags.AddRange(TagHelper.GetTags(type));
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
