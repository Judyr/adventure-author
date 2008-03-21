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
    public partial class ResRefQuestionPanel : ParameterPanel
    {    	
    	/// <summary>
    	/// Create a question panel which will be answered by a string value corresponding to the resref of a game object blueprint.
    	/// </summary>
    	/// <param name="question">The question to ask the user</param>
        /// <param name="objectTypes">The (multiple) types of object to populate the resref list from</param>
        public ResRefQuestionPanel(string question, TaggedType[] objectTypes)
        {
            InitializeComponent();
            QuestionLabel.Text = question;
            
            SortedList<string,string> resrefs = new SortedList<string,string>();
            foreach (TaggedType type in objectTypes) {
            	foreach (string resref in ScriptHelper.GetResRefs(type).Keys) {
            		if (!resrefs.ContainsKey(resref)) {
            			resrefs.Add(resref,null);
            		}
            	}
            }
            AnswerBox.ItemsSource = resrefs.Keys;
        }
        
        
        /// <summary>
        /// Returns an object representing an answer to the question posed by this panel - the type of object depends on the type of question.
        /// </summary>
        public override object Answer
        {
			get { 
        		return AnswerBox.SelectedItem;
        	}
		}
    }
}
