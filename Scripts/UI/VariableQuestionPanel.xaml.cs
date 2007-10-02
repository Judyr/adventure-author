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
    public partial class VariableQuestionPanel : UserControl, IQuestionPanel
    {
    	private NWN2ScriptVariableType type;
    	
    	public VariableQuestionPanel(string question, NWN2ScriptVariableType type)
        {
    		if (type != NWN2ScriptVariableType.Int && type != NWN2ScriptVariableType.String) {
    			Say.Error("Variables other than strings and ints are not currently supported.");
    		}
    		    		
            InitializeComponent();
            QuestionLabel.Text = question;
            this.type = type;
            RefreshVariableList();
        }
        
        public object Answer
        {
			get { 
        		return AnswerBox.SelectedItem;
        	}
		}
        
        private void RefreshVariableList()
        {
            List<string> variables = new List<string>();            
            foreach (NWN2ScriptVariable variable in Adventure.CurrentAdventure.Module.ModuleInfo.Variables) {
            	if (variable.VariableType == type) {
            		variables.Add(variable.Name);
            	}
            }            
            AnswerBox.ItemsSource = variables;  
        }
        
        private void OnClick_LaunchVariableManagerButton(object sender, EventArgs ea)
        {
        	Toolset.LaunchVariableManager();
            RefreshVariableList(); // variables may have changed
        }
    }
}
