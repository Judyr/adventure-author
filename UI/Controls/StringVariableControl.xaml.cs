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
using System.Windows.Navigation;
using System.Windows.Shapes;
using NWN2Toolset.NWN2.Data;

namespace AdventureAuthor.UI.Controls
{
    /// <summary>
    /// Interaction logic for StringVariableControl.xaml
    /// </summary>

    public partial class StringVariableControl : UserControl
    {
        public StringVariableControl(NWN2ScriptVariable var)
        {
        	if (var.VariableType != NWN2ScriptVariableType.String) {
        		throw new ArgumentException("StringVariableControl will not accept a variable of type " + var.GetType().ToString() + ".");
        	}
        	
            InitializeComponent();
            
            // TODO: a two-way binding for Name and Value
            
            // temp:
            
            VariableNameTextBox.Text = var.Name;
            VariableValueTextBox.Text = var.ValueString;
        }
        
        private void OnClick_Delete(object sender, EventArgs ea)
        {
        	//todo
        }

    }
}
