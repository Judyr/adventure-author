using System;
using System.Windows;
using AdventureAuthor.Core;
using AdventureAuthor.Utils;
using NWN2Toolset.NWN2.Data;

namespace AdventureAuthor.Variables.UI
{
    public partial class VariablesWindow : Window
    {    	
    	/// <summary>
    	/// The single instance of the Variable Manager window.
    	/// <remarks>Pseudo-Singleton pattern, but I haven't really implemented this.</remarks>
    	/// </summary>
    	private static VariablesWindow instance;    	
		public static VariablesWindow Instance {
			get { return instance; }
			set { instance = (VariablesWindow)value; }
		}   
    	
        public VariablesWindow()
        {
            InitializeComponent();
            Refresh();
        }
        
        public void OnClick_AddVariable(object sender, EventArgs ea)
        {
        	NWN2ScriptVariable var = new NWN2ScriptVariable();
        	bool? result = null;
        	
        	try {
	        	CreateEditVariableWindow window = new CreateEditVariableWindow(ref var,false);
	        	result = window.ShowDialog();
        	}
        	catch (ArgumentException ae) {
        		Say.Error("Tried to add a variable of an invalid type.",ae);
        	}
        	
        	if (result.HasValue && result.Value) { // returned true, so add the new variable
	        	Adventure.CurrentAdventure.Module.ModuleInfo.Variables.Add(var);
	        	Refresh();   		
        	}
        }
        
        internal void DeleteVariable(NWN2ScriptVariable var)
        {
			NWN2ScriptVarTable variables = Adventure.CurrentAdventure.Module.ModuleInfo.Variables;
			variables.Remove(var);
			Refresh();
        }
        
        internal void Refresh()
        {        	
            NWN2ScriptVarTable variables = Adventure.CurrentAdventure.Module.ModuleInfo.Variables;
            VariablesList.Items.Clear();
            foreach (NWN2ScriptVariable var in variables) {
            	VariableControl varControl = new VariableControl(var);
            	VariablesList.Items.Add(varControl);
            }
        }
    }
}
