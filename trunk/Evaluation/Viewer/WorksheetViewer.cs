using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using AdventureAuthor.Utils;
using NWN2Toolset.NWN2.Data;
using AdventureAuthor.Evaluation.Viewer;
using AdventureAuthor.Scripts.UI;
using Microsoft.Win32;

namespace AdventureAuthor.Evaluation.Viewer
{
	/// <summary>
	/// A window which provides a number of criteria for evaluating a module
	/// and elicits a response from the user as to how well they have been met.
	/// </summary>
    public partial class WorksheetViewer : Window
    {    	
    	#region Constants
    	
    	private string DEFAULT_TITLE = "Evaluation";
    	internal const string XML_FILTER = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
    	
    	#endregion
    	    	
    	#region Fields
    	
    	private static bool designerMode;
    	public static bool DesignerMode {
    		get { return designerMode; }
    	}
    	
    	private Worksheet originalWorksheet;
    	
    	private string filename;    	
		public string Filename {
			get { return filename; }
			set { 
				filename = value;
				if (filename == null) {
					FileNameTextBlock.Text = "<null>";
				}
				else {
					FileNameTextBlock.Text = filename;
				}
				Log.WriteMessage(FileNameTextBlock.Text);
			}
		}
    	
    	/// <summary>
    	/// True if the worksheet fields have been changed since the last save; false otherwise.
    	/// </summary>
    	private bool dirty = false;    	
		internal bool Dirty {
			get { return dirty; }
			set { 
				if (dirty != value) {
					if (Filename == null || Filename == String.Empty) {
						Title = "temp";
					}
					else {
						Title = Filename;
					}					
					if (value) {
						Title += "*";
					}
				}
				
				dirty = value; 
			}
		}
    	
    	#endregion
    	    	    	    	
    	#region Constructors    	   	
    	
    	public WorksheetViewer(bool UseDesignerMode)
    	{
    		InitializeComponent();
    		this.Closing += new CancelEventHandler(WorksheetWindow_Closing);
    		EvaluationOptions.ChangedDefaultImageApplication += 
    			new EventHandler(EvaluationOptions_ChangedDefaultImageApplication);    		    			
    		designerMode = UseDesignerMode;
    		
    		// Edit worksheet titles in designer mode only; fill in name and date in viewer mode only
    		TitleField.IsEnabled = designerMode;
    		NameField.IsEnabled = !designerMode;
    		DateField.IsEnabled = !designerMode;    		
    		
    		if (designerMode) {
    			SaveBlankMenuItem.Visibility = Visibility.Collapsed;
    			NewMenuItem.Visibility = Visibility.Visible;
    			OptionsMenu.Visibility = Visibility.Collapsed;
    			EditMenu.Visibility = Visibility.Visible;
    		}
    		else {
    			SaveBlankMenuItem.Visibility = Visibility.Visible;
    			NewMenuItem.Visibility = Visibility.Collapsed;
    			OptionsMenu.Visibility = Visibility.Visible;
    			EditMenu.Visibility = Visibility.Collapsed;
    		}
    	}
    	
    	
    	public WorksheetViewer() : this(false)
    	{
    		
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
    	
    	#endregion
    	    	
    	#region Methods
    	
    	/// <summary>
    	/// Open a worksheet.
    	/// </summary>
    	/// <param name="worksheet">The worksheet to open</param>
    	/// <param name="sourceFilename">The filename this worksheet was deserialized from,
    	/// or null if the worksheet was created in code</param>
    	private void Open(Worksheet worksheet, string sourceFilename)
    	{
    		CloseWorksheetDialog();
    		
    		if (DesignerMode && !worksheet.IsBlank()) {
    			MessageBoxResult result = MessageBox.Show("The worksheet you are trying to open has been partially " +
    			                                          "filled in, and cannot be re-designed.\n\n" +
    			                                          "Would you like to create a blank copy to work on?",
    			                                          "Create blank copy?",
    			                                          MessageBoxButton.OKCancel);
    			if (result == MessageBoxResult.OK) {
    				worksheet = worksheet.GetBlankCopy();
    				Filename = null;
    			}
    			else {
    				return;
    			}    			
    		}
    		else {
    			Filename = sourceFilename;
    		}
    		
    		try {
	    		originalWorksheet = worksheet;
	    		TitleField.Text = worksheet.Title;
	    		DateField.Text = worksheet.Date;
	    		NameField.Text = worksheet.Name;
	    		    		
	    		foreach (Section section in worksheet.Sections) {	    			
	    			if (DesignerMode || section.Include) {
			    		SectionControl sectionControl = new SectionControl(section,WorksheetViewer.DesignerMode);
			    		AddSectionControl(sectionControl);
			    		WatchForChanges(sectionControl);
			    		
			    		// Mark the worksheet as 'dirty' (different from saved copy) if an answer changes:
			    		foreach (QuestionControl questionControl in sectionControl.QuestionsPanel.Children) {
			    			WatchForChanges(questionControl);
			    			foreach (OptionalWorksheetPartControl answerControl in questionControl.AnswersPanel.Children) {
			    				WatchForChanges(answerControl);
			    			}
			    		}
	    			}
	    		}
	    		
	    		// 'Seal off' the very end of the worksheet with a border:
	    		if (SectionsPanel.Children.Count > 0) {
		    		SectionControl finalSectionControl = 
		    			(SectionControl)SectionsPanel.Children[SectionsPanel.Children.Count-1];
	    			finalSectionControl.SectionBorder.BorderThickness = new Thickness(2);
	    		}
	    		
	    		DateField.TextChanged += delegate { worksheet_Changed(); };
	    		NameField.TextChanged += delegate { worksheet_Changed(); };
	    		TitleField.TextChanged += delegate { worksheet_Changed(); };
	    		TitleLabel.Visibility = Visibility.Visible;
	    		NameLabel.Visibility = Visibility.Visible;
	    		DateLabel.Visibility = Visibility.Visible;
	    		TitleField.Visibility = Visibility.Visible;
		    	NameField.Visibility = Visibility.Visible;
		    	DateField.Visibility = Visibility.Visible;
		    	
		    	if (Filename == null || Filename == String.Empty) {
		    		Dirty = true;  // set Dirty to true if we don't have a filename to save to yet...
		    	}
		    	else {
		    		Dirty = false; // ...otherwise expressly set Dirty to false, so as to ignore the  
		    					   // meaningless TextChanged event when the Title, Name and Date fields are created
		    	}
    		}
    		catch (Exception e) {
    			Say.Error("Was unable to open worksheet.",e);
    			CloseWorksheet();
    		}
    	} 

    	
    	private void sectionControl_Deleting(object sender, DeletingEventArgs e)
    	{
    		if (SectionsPanel.Children.Contains(e.SectionControl)) {
    			SectionsPanel.Children.Remove(e.SectionControl);
    		}
    		else {
    			throw new InvalidOperationException("Received instruction to delete a section control " + 
    			                                    "( " + e.SectionControl.SectionTitleTextBox.Text + 
    			                                    ") that was not a part of this worksheet.");
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
	    		Open(worksheet,filename);
    		}
    		catch (Exception e) {
    			Say.Error("The selected file was not a valid worksheet.",e);
    			CloseWorksheet();
    			this.Filename = null;
    		}
    	}
    	
    	
    	public SectionControl AddSection(Section section)
    	{
    		if (section == null) {
        		throw new ArgumentNullException("Can't add a null section field.");
        	}
        	
        	SectionControl control = (SectionControl)section.GetControl(WorksheetViewer.DesignerMode);
        	AddSectionControl(control);
        	return control;
    	}
    	
    	
    	private void AddSectionControl(SectionControl sectionControl)
    	{
    		foreach (SectionControl sc in SectionsPanel.Children) {
    			if (sc.SectionTitleTextBox.Text == sectionControl.SectionTitleTextBox.Text) {
    				throw new ArgumentException("Cannot add more than one section with the title " 
    				                            + sc.SectionTitleTextBox.Text + ".");
    			}
    		}
    		SectionsPanel.Children.Add(sectionControl);
    	}
    	
    	
    	public bool Save()
    	{
    		if (originalWorksheet == null || Filename == null) {
    			Say.Debug("Save failed: Should not have called Save without setting a filename or opening a worksheet.");
    			return false;
    		}
    		else {
	    		AdventureAuthor.Utils.Serialization.Serialize(Filename,GetWorksheet());
	    		if (Dirty) {
	    			Dirty = false;
	    		}
	    		return true;
    		}
    	}
    	    	
    	
    	public void CloseWorksheet()
    	{
    		Filename = null;
    		originalWorksheet = null;
    		
    		TitleField.Clear();
    		NameField.Clear();
    		DateField.Clear();
    		Title = DEFAULT_TITLE;
    		TitleLabel.Visibility = Visibility.Hidden;
	    	NameLabel.Visibility = Visibility.Hidden;
	    	DateLabel.Visibility = Visibility.Hidden;
		    TitleField.Visibility = Visibility.Hidden;
	    	NameField.Visibility = Visibility.Hidden;
		    DateField.Visibility = Visibility.Hidden;
    		SectionsPanel.Children.Clear();
    		
    		dirty = false;
    	}
    	
    	
    	/// <summary>
    	/// Construct a worksheet object (including the user's entries)
    	/// </summary>
    	/// <returns>A worksheet object, or null if no worksheet is open</returns>
    	public Worksheet GetWorksheet()
    	{
    		Worksheet ws;
    		if (DesignerMode) {
	    		ws = new Worksheet(TitleField.Text,NameField.Text,DateField.Text); 
		    	foreach (SectionControl sc in SectionsPanel.Children) {
	    			Section section = (Section)sc.GetWorksheetPart();
		    		ws.Sections.Add(section);
		    	}
	    		return ws;
    		}
    		else {
    			if (originalWorksheet == null) {
    				throw new ArgumentException("Could not find object representing the opened worksheet.");
    			}    			
    			ws = originalWorksheet.GetCopy();
    			if (ws.Title != TitleField.Text) {
    				ws.Title = TitleField.Text;
    			}
    			if (ws.Name != NameField.Text) {
    				ws.Name = NameField.Text;
    			}
    			if (ws.Date != DateField.Text) {
    				ws.Date = DateField.Text;
    			}
    			
    			foreach (SectionControl sc in SectionsPanel.Children) {
    				foreach (QuestionControl qc in sc.QuestionsPanel.Children) {
    					Question question = originalWorksheet.GetQuestion(qc.QuestionTitle.Text,sc.SectionTitleTextBox.Text);
    					Question question2 = (Question)qc.GetWorksheetPart();
    					if (question != null && question2 != null) {
    						if (question.Text != question2.Text) {
    							question.Text = question2.Text;
    						}
    						foreach (Answer answer2 in question2.Answers) {
    							foreach (Answer answer in question.Answers) {
    								if (answer2.GetType() == answer.GetType()) {
    									answer.Value = answer2.Value;
    									continue;
    								}
    							}
    						}
    					}
    				}
    			}
    		}
    		return ws;
    	}
    	    	
    	#endregion
    	    	
    	#region Event handlers
    	
    	private void OnClick_New(object sender, EventArgs e)
    	{
    		Open(new Worksheet(),null);
    	}
    	
    	
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
    	
    	
    	private void WatchForChanges(SectionControl control)
    	{
    		control.Deleting += new EventHandler<DeletingEventArgs>(sectionControl_Deleting);
    		WatchForChanges((OptionalWorksheetPartControl)control);
    	}
    	
    	
    	private void WatchForChanges(OptionalWorksheetPartControl control)
    	{
    		control.Activated += delegate { worksheet_Changed(); };
    		control.Deactivated += delegate { worksheet_Changed(); };
    		control.Changed += delegate { worksheet_Changed(); };
    	}
    	
    	
    	private bool SaveAsDialog()
    	{
    		SaveFileDialog saveFileDialog = new SaveFileDialog();
    		saveFileDialog.AddExtension = true;
    		saveFileDialog.CheckPathExists = true;
    		saveFileDialog.DefaultExt = XML_FILTER;
    		saveFileDialog.Filter = XML_FILTER;
  			saveFileDialog.ValidateNames = true;
  			saveFileDialog.Title = "Select location to save worksheet to";
  			bool ok = (bool)saveFileDialog.ShowDialog();  				
  			if (ok) {
  				Filename = saveFileDialog.FileName;
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
    		else if (Filename == null) { // get a filename to save to if there isn't one already    		
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
    	
    	
    	private void worksheet_Changed()
    	{
    		if (!Dirty) {
    			Dirty = true;
    		}
    	}
        
        
        private void OnClick_AddSection(object sender, EventArgs e)
        {
        	if (!WorksheetViewer.DesignerMode) {
        		throw new InvalidOperationException("Should not have been possible to call Add Section " +
        		                                    "when not in designer mode.");
        	}
        	
        	Section section = new Section("Enter the section title here...");
        	SectionControl control = new SectionControl(section,true);
        	AddSectionControl(control);
        	control.SectionTitleTextBox.Focus();
        	control.SectionTitleTextBox.SelectAll();        	
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
