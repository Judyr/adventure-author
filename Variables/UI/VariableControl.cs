﻿using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Forms;
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
        	if (variable.VariableType != NWN2ScriptVariableType.String && variable.VariableType != NWN2ScriptVariableType.Int) {
            	throw new ArgumentException("Currently you can only add string and integer variables - " 
            	                            + var.VariableType.ToString() + " is invalid.");     
        	}
        	
        	var = variable;
        	
            InitializeComponent();
            
            VariableNameTextBox.Text = var.Name;
            VariableTypeTextBox.Text = "(" + var.VariableType.ToString() + ")";
            VariableValueTextBox.Text = var.ValueString;
        }             
        
        
        private void OnClick_Delete(object sender, EventArgs ea)
        {
        	string warning = "Any actions/conditions which reference this variable will also be deleted. This operation cannot " +
        	                 "be undone. Are you sure you want to delete this variable?";
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