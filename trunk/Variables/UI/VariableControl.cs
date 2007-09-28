using System;
using System.Windows.Controls;
using System.Windows.Forms;
using NWN2Toolset.NWN2.Data;
using AdventureAuthor.Core;
using AdventureAuthor.Utils;

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
            VariableValueTextBox.Text = "Value:  " + var.ValueString;
        }
        
        private void OnClick_Delete(object sender, EventArgs ea)
        {
        	if (MessageBox.Show("Delete this variable?","Delete", MessageBoxButtons.OKCancel) == DialogResult.OK) {
        		VariablesWindow.Instance.DeleteVariable(var);
        	}
        }

        private void OnClick_Edit(object sender, EventArgs ea)
        {
        	try {
	        	CreateEditVariableWindow window = new CreateEditVariableWindow(ref var,true);
	        	window.ShowDialog();
	        	VariablesWindow.Instance.Refresh();
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
