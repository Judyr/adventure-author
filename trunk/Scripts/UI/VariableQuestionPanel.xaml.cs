using System;
using System.Collections.Generic;
using System.Windows.Controls;
using AdventureAuthor.Core;
using AdventureAuthor.Setup;
using AdventureAuthor.Utils;
using AdventureAuthor.Variables.UI;
using NWN2Toolset.NWN2.Data;

namespace AdventureAuthor.Scripts.UI
{	
	/// <summary>
	/// Ask a question which will be answered by a string value corresponding to the name of a game variable. 
	/// </summary>
    public partial class VariableQuestionPanel : UserControl, IQuestionPanel
    {
    	/// <summary>
    	/// The type of variable which the user can select from (e.g. int, string).
    	/// </summary>
    	private NWN2ScriptVariableType type;
    	        
    	
    	/// <summary>
        /// Create a question panel which will be answered by a string value corresponding to the name of a game variable.
        /// </summary>
        /// <param name="question">The question to ask the user</param>
        /// <param name="variableType">The type of variable which the user can select from (e.g. int, string)</param>
    	public VariableQuestionPanel(string question, NWN2ScriptVariableType type)
        {
    		if (type != NWN2ScriptVariableType.Int && type != NWN2ScriptVariableType.String) {
    			throw new ArgumentException("Variables other than strings and ints are not currently supported.");
    		}
    		    		
            InitializeComponent();
            QuestionLabel.Text = question;
            this.type = type;
            PopulateVariableList();
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
        
        
        /// <summary>
        /// Populate the list of variables the user can answer from.
        /// </summary>
        private void PopulateVariableList()
        {
            SortedList<string,string> variables = new SortedList<string,string>();            
            foreach (NWN2ScriptVariable variable in Adventure.CurrentAdventure.Module.ModuleInfo.Variables) {
            	if (variable.VariableType == type && !variables.ContainsKey(variable.Name)) {
            		variables.Add(variable.Name,null);
            	}
            }            
            AnswerBox.ItemsSource = variables.Keys;  
        }
        
        
        #region Event handlers
        
        /// <summary>
        /// Launch the variable manager window, to allow users to add/delete/edit variables before setting/getting them through scripting.
        /// </summary>
        private void OnClick_LaunchVariableManagerButton(object sender, EventArgs ea)
        {
        	Toolset.LaunchVariableManager();
            PopulateVariableList(); // variables may have changed
        }
        
        #endregion
    }
}
