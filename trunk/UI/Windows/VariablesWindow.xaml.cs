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
using System.Windows.Shapes;
using AdventureAuthor.Core;
using AdventureAuthor.UI.Controls;
using NWN2Toolset.NWN2.Data;

namespace AdventureAuthor.UI.Windows
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
            		StringVariableControl varControl = new StringVariableControl(var);
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
