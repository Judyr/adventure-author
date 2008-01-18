using System;
using System.IO;
using System.ComponentModel;
using System.Windows;
using AdventureAuthor.Utils;
using NWN2Toolset.NWN2.Data;
using AdventureAuthor.Evaluation.UI;
using AdventureAuthor.Scripts.UI;
using Microsoft.Win32;

namespace AdventureAuthor.Evaluation.UI
{
	/// <summary>
	/// A window which provides a number of criteria for evaluating a module
	/// and elicits a response from the user as to how well they have been met.
	/// </summary>
    public partial class WorksheetWindow : Window
    {    	
    	#region Constants
    	
    	private string DEFAULT_TITLE = "Evaluation";
    	private const string XML_FILTER = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
    	
    	#endregion
    	    	
    	#region Fields
    	
    	private Worksheet originalWorksheet;
    	
    	private string filename;    	
		public string Filename {
			get { return filename; }
			set { filename = value; }
		}
    	
    	/// <summary>
    	/// True if the worksheet fields have been changed since the last save; false otherwise.
    	/// </summary>
    	private bool dirty = false;    	
		internal bool Dirty {
			get { return dirty; }
			set { 
				dirty = value; 
				if (originalWorksheet != null) {
					if (dirty) {
						Title = originalWorksheet.Title + "*";
					}
					else {
						Title = originalWorksheet.Title;
					}
				}
			}
		}
    	
    	#endregion
    	    	    	    	
    	#region Constructors    	   	
    	
    	public WorksheetWindow()
    	{
    		InitializeComponent();
    		this.Closing += new CancelEventHandler(WorksheetWindow_Closing);
    		EvaluationOptions.ChangedDefaultImageApplication += 
    			new EventHandler(EvaluationOptions_ChangedDefaultImageApplication);
    	}

    	private void WorksheetWindow_Closing(object sender, CancelEventArgs e)
    	{	
			if (!CloseWorksheetDialog()) {
				e.Cancel = true;
			}
    	}

    	
    	private void EvaluationOptions_ChangedDefaultImageApplication(object sender, EventArgs e)
    	{
    		switch (EvaluationOptions.ApplicationToOpenImages) {
    			case EvaluationOptions.ImageApps.Default:
    				UseDefaultMenuItem.IsChecked = true;
    				UsePaintMenuItem.IsChecked = false;
    				break;
    			case EvaluationOptions.ImageApps.MicrosoftPaint:
    				UseDefaultMenuItem.IsChecked = false;
    				UsePaintMenuItem.IsChecked = true;
    				break;
    		}
    	}
    	
    	
    	public WorksheetWindow(Worksheet worksheet) : this()
    	{
    		Open(worksheet);
    	}
    	
    	
    	public WorksheetWindow(string filename) : this()
    	{
    		Open(filename);
    	}
    	
    	#endregion
    	    	
    	#region Methods
    	
    	public void Open(Worksheet worksheet)
    	{
    		CloseWorksheetDialog();
    		
    		try {
	    		originalWorksheet = worksheet;
	    		Title = worksheet.Title;
	    		DateField.Text = worksheet.Date;
	    		NameField.Text = worksheet.Name;
	    		    		
	    		foreach (Section section in worksheet.Sections) {
	    			SectionControl sectionControl = new SectionControl(section);
	    			EvaluationSectionsPanel.Children.Add(sectionControl);
	    			
	    			// Mark the worksheet as 'dirty' (different from saved copy) if an answer changes:
	    			foreach (QuestionControl questionControl in sectionControl.QuestionsPanel.Children) {
	    				foreach (IAnswerControl answerControl in questionControl.AnswersPanel.Children) {
	    					answerControl.AnswerChanged += delegate { worksheet_AnswerChanged(); };
	    				}
	    			}
	    		}
	    		
	    		// 'Seal off' the very end of the worksheet with a border:
	    		SectionControl finalSectionControl = 
	    			(SectionControl)EvaluationSectionsPanel.Children[EvaluationSectionsPanel.Children.Count-1];
	    		finalSectionControl.SectionBorder.BorderThickness = new Thickness(2);
	    			    		
	    		DateField.TextChanged += delegate { worksheet_AnswerChanged(); };
	    		NameField.TextChanged += delegate { worksheet_AnswerChanged(); };
	    		NameLabel.Visibility = Visibility.Visible;
	    		DateLabel.Visibility = Visibility.Visible;
		    	NameField.Visibility = Visibility.Visible;
		    	DateField.Visibility = Visibility.Visible;
		    	
		    	Dirty = false; // ignore the TextChanged event caused by DateField/NameField being created
    		}
    		catch (Exception e) {
    			Say.Error("Was unable to open worksheet.",e);
    			CloseWorksheet();
    		}
    	} 

    	    	
    	public void Open(string filename)
    	{
    		if (!File.Exists(filename)) {
    			Say.Error(filename + " could not be found.");
    			return;
    		}
    			
    		try {
	    		object o = AdventureAuthor.Utils.Serialization.Deserialize(filename,typeof(Worksheet));
	    		Worksheet worksheet = (Worksheet)o;
    			CloseWorksheetDialog();    		
	    		Open(worksheet);
	    		this.filename = filename;
    		}
    		catch (Exception e) {
    			Say.Error("The selected file was not a valid worksheet.",e);
    			CloseWorksheet();
    			this.filename = null;
    		}
    	}
    	
    	
    	public bool Save()
    	{
    		if (originalWorksheet == null || filename == null) {
    			Say.Debug("Save failed: Should not have called Save without setting a filename or opening a worksheet.");
    			return false;
    		}
    		else {
	    		AdventureAuthor.Utils.Serialization.Serialize(filename,GetWorksheet());
	    		if (Dirty) {
	    			Dirty = false;
	    		}
	    		return true;
    		}
    	}
    	    	
    	
    	public void CloseWorksheet()
    	{
    		filename = null;
    		originalWorksheet = null;
    		
    		NameField.Clear();
    		DateField.Clear();
    		Title = DEFAULT_TITLE;
	    	NameLabel.Visibility = Visibility.Hidden;
	    	DateLabel.Visibility = Visibility.Hidden;
		    NameField.Visibility = Visibility.Hidden;
		    DateField.Visibility = Visibility.Hidden;
    		EvaluationSectionsPanel.Children.Clear();
    		
    		dirty = false;
    	}
    	
    	
    	/// <summary>
    	/// Construct a worksheet object (including the user's entries)
    	/// </summary>
    	/// <returns>A worksheet object, or null if no worksheet is open</returns>
    	public Worksheet GetWorksheet()
    	{
    		if (originalWorksheet == null) {
    			return null;
    		}
    		
    		Worksheet ws = new Worksheet(originalWorksheet.Title,NameField.Text,DateField.Text);    		
    		foreach (SectionControl sc in EvaluationSectionsPanel.Children) {
    			ws.Sections.Add(sc.GetSection());
    		}
    		return ws;
    	}
    	
    	#endregion
    	    	
    	#region Event handlers
    	
    	private void OnClick_Open(object sender, EventArgs e)
    	{    		
    		OpenDialog();
    	}
    	
    	
    	private void OpenDialog()
    	{    				
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.ValidateNames = true;
    		openFileDialog.DefaultExt = XML_FILTER;
    		openFileDialog.Filter = XML_FILTER;
			openFileDialog.Title = "Select a worksheet file to open";
			openFileDialog.Multiselect = false;
			openFileDialog.RestoreDirectory = false;	
			
  			bool ok = (bool)openFileDialog.ShowDialog();  				
  			if (!ok) {
  				return;
  			}  				
  			else {
  				Open(openFileDialog.FileName);
  			}
    	}
    	
    	
    	private void OnClick_Save(object sender, EventArgs e)
    	{
    		SaveDialog();
    	}
    	
    	
    	private void OnClick_SaveAs(object sender, EventArgs e)
    	{
    		SaveAsDialog();
    	}
    	
    	
    	private bool SaveAsDialog()
    	{
    		if (originalWorksheet == null) {
    			return false;
    		}
    		
    		SaveFileDialog saveFileDialog = new SaveFileDialog();
    		saveFileDialog.AddExtension = true;
    		saveFileDialog.CheckPathExists = true;
    		saveFileDialog.DefaultExt = XML_FILTER;
    		saveFileDialog.Filter = XML_FILTER;
  			saveFileDialog.ValidateNames = true;
  			saveFileDialog.Title = "Select location to save worksheet to";
  			bool ok = (bool)saveFileDialog.ShowDialog();  				
  			if (ok) {
	  			filename = saveFileDialog.FileName;
	  			Save();
	  			return true;
  			}
  			else {
  				return false;
  			}
    	}
    	
    	
    	private bool SaveDialog() 
    	{    		
    		if (originalWorksheet == null) {
    			return false;
    		}
    		else if (filename == null) { // get a filename to save to if there isn't one already    		
    			return SaveAsDialog();
    		}
    		else {
	    		try {
	    			return Save();
	    		}
	    		catch (Exception e) {
	    			Say.Error("Failed to save worksheet.",e);
	    			return false;
	    		}
    		}
    	}
    	    	
    	
    	private void OnClick_Close(object sender, EventArgs e)
    	{
    		CloseWorksheetDialog();
    	}
    	
    	
    	private bool CloseWorksheetDialog()
    	{
    		if (dirty) {
    			MessageBoxResult result = 
    				MessageBox.Show("Save changes to this worksheet?","Save changes?",MessageBoxButton.YesNoCancel);
    			switch (result) {
    				case MessageBoxResult.Cancel:
    					return false;
    				case MessageBoxResult.Yes:
    					bool cancelled = !SaveDialog(); // user can save, proceed without saving or cancel
    					if (cancelled) {
    						return false;
    					}
    					break;
    				default:
    					break;
    			}    			
    		}
    		
    		CloseWorksheet();
    		return true;
    	}
    	
    	
    	private void OnClick_MakeBlankCopy(object sender, EventArgs e)
    	{
  			Worksheet original = GetWorksheet();
  			if (original == null) {
  				return;
  			}
	    	Worksheet blankCopy = original.GetBlankCopy(); 
	    	
    		SaveFileDialog saveFileDialog = new SaveFileDialog();
    		saveFileDialog.AddExtension = true;
    		saveFileDialog.CheckPathExists = true;
    		saveFileDialog.DefaultExt = XML_FILTER;
    		saveFileDialog.Filter = XML_FILTER;
  			saveFileDialog.ValidateNames = true;
  			saveFileDialog.Title = "Select location to save blank copy to";
  			bool ok = (bool)saveFileDialog.ShowDialog();  				
  			if (ok) {
  				string filename = saveFileDialog.FileName;  
	    		AdventureAuthor.Utils.Serialization.Serialize(filename,blankCopy);
	    		Say.Information("Created blank copy at " + filename);
  			}
    	}    	
    	
    	
    	private void OnClick_Exit(object sender, EventArgs e)
    	{
    		Close();
    	}
    	
    	
    	private void worksheet_AnswerChanged()
    	{
    		if (!Dirty) {
    			Dirty = true;
    		}
    	}
    	
    	
    	private void OnChecked_UsePaint(object sender, EventArgs e)
    	{
    		EvaluationOptions.ApplicationToOpenImages = EvaluationOptions.ImageApps.MicrosoftPaint;
    	}
    	
    	
    	private void OnChecked_UseDefault(object sender, EventArgs e)
    	{
    		EvaluationOptions.ApplicationToOpenImages = EvaluationOptions.ImageApps.Default;
    	}
    	
    	#endregion
    }
}
