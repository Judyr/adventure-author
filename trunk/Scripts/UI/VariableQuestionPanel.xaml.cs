using System;
using System.Collections.Generic;
using System.Windows.Controls;
using AdventureAuthor.Core;
using AdventureAuthor.Utils;
using AdventureAuthor.Variables.UI;
using NWN2Toolset.NWN2.Data;

namespace AdventureAuthor.Scripts.UI
{
    /// <summary>
    /// Interaction logic for TagQuestionPanel.xaml
    /// </summary>

    public partial class VariableQuestionPanel : UserControl, IQuestionPanel
    {
    	public VariableQuestionPanel(string question, NWN2ScriptVariableType variableType)
        {
    		if (variableType != NWN2ScriptVariableType.Int && variableType != NWN2ScriptVariableType.String) {
    			Say.Error("Variables other than strings and ints are not currently supported.");
    		}
    		
            InitializeComponent();
            QuestionLabel.Text = question;
            List<string> variables = new List<string>();
            
            foreach (NWN2ScriptVariable variable in Adventure.CurrentAdventure.Module.ModuleInfo.Variables) {
            	if (variable.VariableType == variableType) {
            		variables.Add(variable.Name);
            	}
            }
            
            AnswerBox.ItemsSource = variables;
        }
        
        public object Answer
        {
			get { 
        		return AnswerBox.SelectedItem;
        	}
		}
        
        private void OnClick_LaunchVariableManagerButton(object sender, EventArgs ea)
        {
        	// TODO: Always check that there is only one Variable Manager open, and if it is already open, simply bring it to the front.
        	
        	VariablesWindow window = new VariablesWindow();
        	window.ShowDialog();
        }
    }
}
