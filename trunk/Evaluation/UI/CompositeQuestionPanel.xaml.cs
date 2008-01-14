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
using AdventureAuthor.Scripts.UI;

namespace AdventureAuthor.Evaluation.UI
{
    /// <summary>
    /// A question panel which displays a given question, and some number of
    /// fields for sub-questions which collectively answer the main question.
    /// </summary>
    public partial class CompositeQuestionPanel : UserControl, IQuestionPanel
    {
        public CompositeQuestionPanel(string question)
        {
            InitializeComponent();
            QuestionLabel.Text = question;
        }

        
        public void AddSubQuestion(IQuestionPanel questionPanel)
        {
        	if (!(questionPanel is UIElement)) {
        		throw new ArgumentException("Cannot add a question panel which is not a " +
        		                            "UI element.");
        	}
        	SubQuestionsPanel.Children.Add((UIElement)questionPanel);
        }
        
    	
        /// <summary>
        /// Answers the question posed by this panel.
        /// </summary>
		public object Answer {
			get {
        		List<object> answers = new List<object>(SubQuestionsPanel.Children.Count);
        		foreach (UIElement element in SubQuestionsPanel.Children) {
        			IQuestionPanel subquestion = element as IQuestionPanel;
        			if (subquestion != null) {
        				answers.Add(subquestion.Answer);
        			}
        		}
        		return answers;
			}
		}
    }
}