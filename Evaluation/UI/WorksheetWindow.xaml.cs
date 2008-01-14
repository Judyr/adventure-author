using System;
using System.Windows;
using AdventureAuthor.Utils;
using NWN2Toolset.NWN2.Data;
using AdventureAuthor.Evaluation.UI;

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
    	}
    	
    	#endregion
    	
    	
    	public void Open(Worksheet worksheet)
    	{
    		Title = worksheet.Title;
    		DateField.Text = worksheet.Date;
    		NameField.Text = worksheet.Name;
    		
    		foreach (Section section in worksheet.Sections) {
    			SectionControl sectionControl = new SectionControl(section);
    			EvaluationSectionsPanel.Children.Add(sectionControl);
    		}
    	}    	
    }
}
