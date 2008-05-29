using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.IO;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.ConversationData;
using NWN2Toolset.NWN2.Data.TypedCollections;
using AdventureAuthor.Core;
using AdventureAuthor.Variables;
using AdventureAuthor.Utils;
using OEIShared.IO;
using OEIShared.Utils;

namespace AdventureAuthor.Variables.UI
{
    public partial class VariableControl : System.Windows.Controls.UserControl
    {
    	private NWN2ScriptVariable var;    	
		public NWN2ScriptVariable Var {
			get { return var; }
		}
    	
    	
        public VariableControl(NWN2ScriptVariable variable)
        {
        	if (variable.VariableType != NWN2ScriptVariableType.String && 
        	    variable.VariableType != NWN2ScriptVariableType.Int) {
            	throw new ArgumentException("Currently you can only add string and integer variables - " 
            	                            + var.VariableType.ToString() + " is invalid.");     
        	}
        	
        	var = variable;
        	
            InitializeComponent();    
            
            DeleteButton.Content = ResourceHelper.GetImage(Path.Combine(ModuleHelper.ImagesDir,"delete.png")); // NB: was commented out - why?
            
            VariableNameTextBox.Text = var.Name;
            VariableTypeTextBox.Text = GetVariableType(var);
            VariableValueTextBox.Text = var.ValueString;
        }             
        
        
        private string GetVariableType(NWN2ScriptVariable var)
        {
            switch (var.VariableType) {
            	case NWN2ScriptVariableType.Int:
            		return "Number";
            	case NWN2ScriptVariableType.String:
            		return "Word(s)";
            	case NWN2ScriptVariableType.Float:
            		return "Decimal"; // not currently used
            	default: 
            		return var.ToString();
            }
        }
        
        
        private void OnClick_Delete(object sender, EventArgs ea)
        {
//        	string warning = "Any actions/conditions which reference this variable will also be deleted. This operation cannot " +
//        	                 "be undone. Are you sure you want to delete this variable?";

        	string warning = "Are you sure you want to delete this variable?";
        	if (MessageBox.Show(warning,"Delete", MessageBoxButtons.YesNo) == DialogResult.Yes) {
        		VariableManager.Delete(var,true);
        	} 
        }
        
        
        private void OnClick_Edit(object sender, EventArgs ea)
        {
        	try {
	        	CreateEditVariableWindow window = new CreateEditVariableWindow(ref var,true);
	        	window.ShowDialog();
	        	VariablesWindow.Instance.RefreshVariablesList();
        	}
        	catch (ArgumentNullException ane) {
        		Say.Error("Was unable to restore the original starting value of variable " + var.Name + ".",ane);
        	}
        	catch (ArgumentException ae) {
        		Say.Error("Tried to add a variable of an invalid type.",ae);
        	}
        }
    }
}
