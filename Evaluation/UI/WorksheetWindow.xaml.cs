using System;
using System.IO;
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
    	
    	
    	#region Constants
    	
    	private const string XMLFILTER = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
    	
    	#endregion
    	
    	
    	#region Constructors    	   	
    	
    	public WorksheetWindow()
    	{
    		InitializeComponent();
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
	    			DateField.TextChanged += delegate { worksheet_AnswerChanged(); };
	    			NameField.TextChanged += delegate { worksheet_AnswerChanged(); };
	    			NameLabel.Visibility = Visibility.Visible;
	    			DateLabel.Visibility = Visibility.Visible;
		    		NameField.Visibility = Visibility.Visible;
		    		DateField.Visibility = Visibility.Visible;
	    		}
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
    	
    	
    	public void Save()
    	{
    		if (originalWorksheet == null || filename == null) {
    			Say.Debug("Save failed: Should not have called Save without setting a filename or opening a worksheet.");
    		}
    		else {
	    		AdventureAuthor.Utils.Serialization.Serialize(filename,GetWorksheet());
	    		if (Dirty) {
	    			Dirty = false;
	    		}
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
    	
    	
    	public Worksheet GetWorksheet()
    	{
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
    		openFileDialog.DefaultExt = XMLFILTER;
    		openFileDialog.Filter = XMLFILTER;
			openFileDialog.Title = "Select a worksheet file";
			openFileDialog.Multiselect = false;
			//openFileDialog.InitialDirectory = form.App.Module.Repository.DirectoryName;
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
    		SaveFileDialog saveFileDialog = new SaveFileDialog();
    		saveFileDialog.AddExtension = true;
    		saveFileDialog.CheckPathExists = true;
    		saveFileDialog.DefaultExt = XMLFILTER;
    		saveFileDialog.Filter = XMLFILTER;
  			saveFileDialog.ValidateNames = true;
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
    		// Get a filename to save to if there isn't one already:
    		if (filename == null) { 
    			return SaveAsDialog();
    		}
    		else {
	    		try {
	    			Save();
	    			return true;
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
    	
    	
    	private void OnClick_Exit(object sender, EventArgs e)
    	{
    		if (CloseWorksheetDialog()) {
    			Close();
    		}    		
    	}
    	
    	
    	private void worksheet_AnswerChanged()
    	{
    		if (!Dirty) {
    			Dirty = true;
    		}
    	}
    	
    	#endregion
    }
}
