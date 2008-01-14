using System;
using System.IO;
using System.Windows;
using AdventureAuthor.Utils;
using NWN2Toolset.NWN2.Data;
using AdventureAuthor.Evaluation.UI;
using Microsoft.Win32;

namespace AdventureAuthor.Evaluation.UI
{
	/// <summary>
	/// A window which provides a number of criteria for evaluating a module
	/// and elicits a response from the user as to how well they have been met.
	/// </summary>
    public partial class WorksheetWindow : Window
    {    	
    	#region Constructors    	   	
    	
    	public WorksheetWindow(Worksheet worksheet)
    	{
    		InitializeComponent();
    		Open(worksheet);
    		Serialization.Serialize(@"C:\adventureauthorlogs\LALALAL.xml",worksheet);
    	}
    	
    	#endregion
    	
    	#region Methods
    	
    	public void Open(Worksheet worksheet)
    	{
    		Title = worksheet.Title;
    		DateField.Text = worksheet.Date;
    		NameField.Text = worksheet.Name;
    		
    		EvaluationSectionsPanel.Children.Clear();
    		
    		foreach (Section section in worksheet.Sections) {
    			SectionControl sectionControl = new SectionControl(section);
    			EvaluationSectionsPanel.Children.Add(sectionControl);
    		}
    	}  
    	
    	public void Open(string filename)
    	{
    		if (!File.Exists(filename)) {
    			Say.Error(filename + " could not be found.");
    			return;
    		}
    		
    		object o = AdventureAuthor.Utils.Serialization.Deserialize(filename,typeof(Worksheet));
    		Worksheet worksheet = (Worksheet)o;
    		Open(worksheet);
    	}
    	
    	#endregion
    	
    	#region Event handlers
    	
    	private void OnClick_Open(object sender, EventArgs e)
    	{
			OpenFileDialog openFile = new OpenFileDialog();
			openFile.ValidateNames = true;
			openFile.Filter = "xml files (*.xml)|*.xml";
			openFile.Title = "Select a worksheet file";
			openFile.Multiselect = false;
			//openFile.InitialDirectory = form.App.Module.Repository.DirectoryName;
			openFile.RestoreDirectory = false;
			openFile.ShowDialog();
			
			Open(openFile.FileName);
		}
    	
    	private void OnClick_Save(object sender, EventArgs e)
    	{
    		
    	}
    	
    	#endregion
    }
}
