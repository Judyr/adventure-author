using System;
using System.Windows.Controls;
using System.Windows.Forms;
using NWN2Toolset.NWN2.Data;
using AdventureAuthor.Core;

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
        	if (variable.VariableType != NWN2ScriptVariableType.String && variable.VariableType != NWN2ScriptVariableType.Int) {
            	throw new ArgumentException("Currently you can only add string and integer variables - " 
            	                            + var.VariableType.ToString() + " is invalid.");     
        	}
        	
        	var = variable;
        	
            InitializeComponent();
            
            VariableNameTextBox.Text = var.Name;
            VariableValueTextBox.Text = var.ValueString;
        }
        
        private void OnClick_Delete(object sender, EventArgs ea)
        {
        	if (MessageBox.Show("Delete this variable?","Delete", MessageBoxButtons.OKCancel) == DialogResult.OK) {
	            NWN2ScriptVarTable variables = Adventure.CurrentAdventure.Module.ModuleInfo.Variables;
	            variables.Remove(var);
	            ((System.Windows.Controls.ListBox)Parent).Items.Remove(this);
        	}
        }

        private void OnClick_Edit(object sender, EventArgs ea)
        {
        	CreateEditVariableWindow window = new CreateEditVariableWindow(ref var);
        	window.ShowDialog();
        	
        }
    }
}
