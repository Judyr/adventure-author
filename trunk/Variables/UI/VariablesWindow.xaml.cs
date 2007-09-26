using System;
using System.Windows;
using AdventureAuthor.Core;
using NWN2Toolset.NWN2.Data;

namespace AdventureAuthor.Variables.UI
{
    /// <summary>
    /// Interaction logic for VariablesWindow.xaml
    /// </summary>

    public partial class VariablesWindow : Window
    {

        public VariablesWindow()
        {
            InitializeComponent();
            
            NWN2ScriptVarTable variables = Adventure.CurrentAdventure.Module.ModuleInfo.Variables;
            foreach (NWN2ScriptVariable var in variables) {
            	if (var.VariableType == NWN2ScriptVariableType.String) {
            		StringControl varControl = new StringControl(var);
            		StringVariablesList.Items.Add(varControl);
            	}
            	else if (var.VariableType == NWN2ScriptVariableType.Int) {
            		// todo
            	}
            	else {
            		// todo
            		// also this should be a switch statement rather than string of if/else
            	}
            }
        }
        
        public void OnClick_AddVariable(object sender, EventArgs ea)
        {
        	//todo
        }

    }
}
