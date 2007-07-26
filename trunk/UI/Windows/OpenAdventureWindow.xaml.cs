using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AdventureAuthor.Core;
using form = NWN2Toolset.NWN2ToolsetMainForm;
using AdventureAuthor.Utils;

namespace AdventureAuthor.UI.Windows
{
    /// <summary>
    /// Interaction logic for OpenAdventureWindow.xaml
    /// </summary>

    public partial class OpenAdventureWindow : Window
    {
        public OpenAdventureWindow()
        {
        	try {
	        	string[] adventureData = Directory.GetFiles(Adventure.SerializedDir,"*.xml",SearchOption.TopDirectoryOnly);
	        	string[] moduleData = Directory.GetDirectories(form.ModulesDirectory,"*",SearchOption.TopDirectoryOnly);
				List<string> adventures = new List<string>(adventureData.Length);
				foreach (string name in adventureData) {
					foreach (string name2 in moduleData) {
						if (name == name2) {
							adventures.Add(name);
							break;
						}
					}
				}
				this.Resources.Add("adventurenames",adventuresList);			
	            InitializeComponent();
        	}
        	catch (DirectoryNotFoundException e) {
        		Say.Error("Could not find one of the directories needed to store Adventure/module data.",e);
        	}
        }

        private void OnClickCancel(object sender, EventArgs ea)
        {
        	this.Close();
        }
        
        private void OnClickOK(object sender, EventArgs ea)
        {
        	if (Adventure.Open((string)adventuresList.SelectedItem) == null) {
        		Say.Error("Could not find a well-formed Adventure with the name '" + adventuresList.SelectedItem + "'.");
        	}
        	else {
        		this.Close();
        	}
        }
    }
}
