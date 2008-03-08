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
    public partial class TagQuestionPanel : UserControl, IParameterPanel
    {    	
    	/// <summary>
    	/// Create a question panel which will be answered by a string value corresponding to the tag of a game object.
    	/// </summary>
    	/// <param name="question">The question to ask the user</param>
        /// <param name="objectTypes">The (multiple) types of object to populate the tag list from</param>
        public TagQuestionPanel(string question, TaggedType[] objectTypes)
        {
            InitializeComponent();
            QuestionLabel.Text = question;            
            
            SortedList<string,string> tags = new SortedList<string,string>();
            foreach (TaggedType type in objectTypes) {
            	foreach (string tag in ScriptHelper.GetTags(type).Keys) {
            		if (!tags.ContainsKey(tag)) {
            			tags.Add(tag,null);
            		}
            	}
            }
            AnswerBox.ItemsSource = tags.Keys;
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
