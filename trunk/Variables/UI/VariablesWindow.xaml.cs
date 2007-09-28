using System;
using System.Windows;
using AdventureAuthor.Core;
using AdventureAuthor.Utils;
using NWN2Toolset.NWN2.Data;

namespace AdventureAuthor.Variables.UI
{
    public partial class VariablesWindow : Window
    {
        public VariablesWindow()
        {
            InitializeComponent();
            Populate();
        }
        
        public void OnClick_AddVariable(object sender, EventArgs ea)
        {
        	NWN2ScriptVariable var = null;
        	CreateEditVariableWindow window = new CreateEditVariableWindow(ref var);
        	window.ShowDialog();
        	if (var != null) {
        		Say.Debug("Adding new variable: " + var.ToString());
        		Adventure.CurrentAdventure.Module.ModuleInfo.Variables.Add(var);
        		Populate();
        		Say.Debug("New variable count: " + Adventure.CurrentAdventure.Module.ModuleInfo.Variables.Count.ToString());
        	}
        	else {
        		Say.Debug("No new variable returned.");
        	}
        }
        
        internal void Populate()
        {
            NWN2ScriptVarTable variables = Adventure.CurrentAdventure.Module.ModuleInfo.Variables;
            foreach (NWN2ScriptVariable var in variables) {
            	VariableControl varControl = new VariableControl(var);
            	VariablesList.Items.Add(varControl);
            }
        }
    }
}
