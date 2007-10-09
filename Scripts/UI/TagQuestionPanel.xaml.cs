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
	/// Ask a question which will be answered by a string value corresponding to the tag of a game object.
	/// </summary>
    public partial class TagQuestionPanel : UserControl, IQuestionPanel
    {    	
    	/// <summary>
    	/// Create a question panel which will be answered by a string value corresponding to the tag of a game object.
    	/// </summary>
    	/// <param name="question">The question to ask the user</param>
        /// <param name="objectTypes">The (multiple) types of object to populate the tag list from</param>
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
        
                
        /// <summary>
        /// Returns an object representing an answer to the question posed by this panel - the type of object depends on the type of question.
        /// </summary>
        public object Answer
        {
			get { 
        		return AnswerBox.SelectedItem;
        	}
		}
    }
}
