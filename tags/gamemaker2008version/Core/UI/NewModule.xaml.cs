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
using System.IO;
using AdventureAuthor.Utils;
using AdventureAuthor.Setup;
using form = NWN2Toolset.NWN2ToolsetMainForm;

namespace AdventureAuthor.Core.UI
{
    /// <summary>
    /// Interaction logic for NewModule.xaml
    /// </summary>

    public partial class NewModule : Window
    {

        public NewModule()
        {
            InitializeComponent();
            NameTextBox.Focus();
        }
                
        
		private void OnClick_Cancel(object sender, EventArgs e)
		{
			this.Close(); 
		}
		
		
		private void OnClick_OK(object sender, EventArgs e)
		{
			try {
				if (NameTextBox.Text.StartsWith("temp")) {
					Say.Warning("Module name cannot begin with 'temp' - choose another name.");
				}
				else if (!ModuleHelper.IsValidName(NameTextBox.Text)) {
					Say.Warning("The name '" + NameTextBox.Text + "' is invalid. Module names " + 
					          "must not contain the following characters ('<', '>', ':', '\', '\"', '/', '|', '.') " +
					          "and must be between 1 and 32 characters in length.");
				}
				else if (!ModuleHelper.IsAvailableModuleName(NameTextBox.Text)) {
					Say.Warning("A module called '" + NameTextBox.Text + "' already " +
					          "exists - choose another name.");
				}
				else {
					if (form.App.Module != null && !Toolset.CloseModuleDialog()) {
						return;
					}
					
					string name = NameTextBox.Text;
					ModuleHelper.CreateAndOpenModule(name);
					this.Close();
				}
			}
			catch (IOException ioe) {
				Say.Error(ioe);
				ModuleHelper.Close();
			}
		}
    }
}