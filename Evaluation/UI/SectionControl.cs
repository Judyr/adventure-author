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
using AdventureAuthor.Evaluation;
using AdventureAuthor.Evaluation.UI;
using AdventureAuthor.Scripts.UI;

namespace AdventureAuthor.Evaluation.UI
{
    public partial class SectionControl : UserControl
    {
        public SectionControl(string title)
        {
            InitializeComponent();
            SectionTitle.Content = title;
        }   
        
        
        public SectionControl(Section section) : this(section.Title)
        {
        	foreach (Question question in section.Questions) {
        		CompositeQuestionPanel panel = new CompositeQuestionPanel(question.Text);
        		foreach (Answer answer in question.Answers) {
        			panel.AddSubQuestion(answer.GetUIControl());
        		}        	
        		AddField(panel);
        	}
        }
                               
        
        public void AddField(IQuestionPanel field)
        {           	
        	ContentControl element = field as ContentControl;
        	if (element == null) {
        		throw new ArgumentException("Field must be a ContentControl.");
        	}
        	if (QuestionsPanel.Children.Count % 2 == 0) {
        		element.Background = Brushes.LightGreen;
        	}
        	else {
        		element.Background = Brushes.LimeGreen;
        	}
        	QuestionsPanel.Children.Add(element);
        }
    }
}