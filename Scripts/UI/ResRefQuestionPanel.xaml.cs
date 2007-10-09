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
	/// Ask a question which will be answered by a string value corresponding to the resref of a game object blueprint. 
	/// </summary>
    public partial class ResRefQuestionPanel : UserControl, IQuestionPanel
    {    	
    	/// <summary>
    	/// Create a question panel which will be answered by a string value corresponding to the resref of a game object blueprint.
    	/// </summary>
    	/// <param name="question">The question to ask the user</param>
        /// <param name="objectTypes">The (multiple) types of object to populate the resref list from</param>
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
