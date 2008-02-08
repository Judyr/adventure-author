using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using NWN2Toolset.NWN2.Data;
using AdventureAuthor.Evaluation.Viewer;
using AdventureAuthor.Utils;
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
    	
//    	public enum Mode {
//    		DesignWorksheet,
//    		CompleteWorksheet,
//    		DiscussWorksheet
//    	}
    	
    	private string DEFAULT_TITLE = "Evaluation";
    	internal const string XML_FILTER = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
    	
    	#endregion
    	    	
    	#region Fields
    	
//    	private static Mode appMode;    	
//		public static Mode AppMode {
//			get { return appMode; }
//		}
    	
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
				UpdateTitleBar();
			}
		}    	
    	
    	/// <summary>
    	/// True if the worksheet fields have been changed since the last save; false otherwise.
    	/// </summary>
    	private bool dirty = false;    	
		internal bool Dirty {
			get { return dirty; }
			set {
				bool updateTitleBar = (dirty != value); // update title bar if dirty status changes
				dirty = value;
				if (updateTitleBar) {
					UpdateTitleBar();
				}
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
    		}
    		else {
    			SaveBlankMenuItem.Visibility = Visibility.Visible;
    			NewMenuItem.Visibility = Visibility.Collapsed;
    			OptionsMenu.Visibility = Visibility.Visible;
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
    		    		
    		if (!DesignerMode) {    			
    			string warning = "This worksheet contains duplicate sections or questions " +
						    	 "and cannot be opened. Try opening and then saving the worksheet " +
						    	 "in designer mode to fix this problem.";    			
    			List<string> sectionTitles = new List<string>(worksheet.Sections.Count);
	    		foreach (Section section in worksheet.Sections) {
    				if (sectionTitles.Contains(section.Title)) {
	    				Say.Error(warning);
	    				CloseWorksheet();
    					return;
	    			}
	    			else {
	    				sectionTitles.Add(section.Title);
	    			}
    				List<string> questionTitles = new List<string>(section.Questions.Count);
    				foreach (Question question in section.Questions) {
	    				if (questionTitles.Contains(question.Text)) {
		    				Say.Error(warning);
	    					CloseWorksheet();
	    					return;
		    			}
		    			else {
		    				questionTitles.Add(question.Text);
		    			}
    				}
	    		}
    		}    		
    		
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
    		
	    	originalWorksheet = worksheet;
	    	TitleField.Text = worksheet.Title;
	    	DateField.Text = worksheet.Date;
	    	NameField.Text = worksheet.Name;
	    	    		
	    	foreach (Section section in worksheet.Sections) {	    			
	    		if (DesignerMode || section.Include) {
	    			try {	    					
		    			SectionControl sectionControl = AddSection(section);
		    			if (sectionControl != null) {					    		
				    		// Mark the worksheet as 'dirty' (different from saved copy) if an answer changes:
				    		foreach (QuestionControl questionControl in sectionControl.QuestionsPanel.Children) {
				    			if (DesignerMode && !sectionControl.IsActive) {
				    				questionControl.Deactivate(true);
				    			}
				    			WatchForChanges(questionControl);
				    			foreach (OptionalWorksheetPartControl answerControl in questionControl.AnswersPanel.Children) {
				    				if (DesignerMode && !questionControl.IsActive) {
				    					answerControl.Deactivate(true);
				    				}
				    				WatchForChanges(answerControl);
				    			}
				    		}
		    			}
	    			}
	    			catch (InvalidDataException e) {
	    				CloseWorksheet();
	    				Say.Error(e);
	    				return;
	    			}
	    		}
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
		    BorderClosingRectangle.Visibility = Visibility.Visible;
		    
		    if (DesignerMode) {
		    	EditMenu.Visibility = Visibility.Visible;
		    }
		    
		    if (Filename == null || Filename == String.Empty) {
		    	Dirty = true;  // set Dirty to true if we don't have a filename to save to yet...
		    }
		    else {
		    	Dirty = false; // ...otherwise expressly set Dirty to false, so as to ignore the  
		    				   // meaningless TextChanged event when the Title, Name and Date fields are created
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
    			Say.Error("Was unable to open worksheet.",e);
    			CloseWorksheet();
    			this.Filename = null;
    		}
    	}
    	
    	
    	private string GetTitle(OptionalWorksheetPartControl control)
    	{
    		if (control is SectionControl) {
    			return ((SectionControl)control).SectionTitleTextBox.Text;
    		}
    		else if (control is QuestionControl) {
    			return ((QuestionControl)control).QuestionTitle.Text;
    		}
    		else {
    			throw new ArgumentException("Can only get titles of sections and questions, not " +
    			                            control.GetType().ToString() + ".");
    		}
    	}
    	
    	
    	private void SetTitle(OptionalWorksheetPartControl control, string title)
    	{
    		if (control is SectionControl) {
    			((SectionControl)control).SectionTitleTextBox.Text = title;
    		}
    		else if (control is QuestionControl) {
    			((QuestionControl)control).QuestionTitle.Text = title;
    		}
    		else {
    			throw new ArgumentException("Can only set titles of sections and questions, not " +
    			                            control.GetType().ToString() + ".");
    		}
    	}
    	
    	
		private void ValidateTitles(UIElementCollection controls)
    	{
			List<string> titles = new List<string>(controls.Count);
    		List<OptionalWorksheetPartControl> controlsWithDuplicateTitles 
    			= new List<OptionalWorksheetPartControl>(controls.Count);
    		foreach (OptionalWorksheetPartControl control in controls) {
    			string title = GetTitle(control);    			
    			if (titles.Contains(title)) {
    				controlsWithDuplicateTitles.Add(control);
    			}
    			else {
    				titles.Add(title);
    			}
    		}
    		
    		foreach (OptionalWorksheetPartControl control in controlsWithDuplicateTitles) {
    			string originalTitle = GetTitle(control);
    			string newTitle = originalTitle;
    			int count = 1;
    			while (titles.Contains(newTitle)) {
    				count++;
    				newTitle = originalTitle + " (" + count + ")";
    			}
    			SetTitle(control,newTitle);
    			titles.Add(newTitle);    			
    		}
    	}  
		
		
		private void ValidateTitles()
		{
			ValidateTitles(SectionsPanel.Children);
			foreach (SectionControl sectionControl in SectionsPanel.Children) {
				ValidateTitles(sectionControl.QuestionsPanel.Children);
			}
		}
    	
    	
//		private void ValidateSectionTitles()
//    	{
//			List<string> titles = new List<string>(SectionsPanel.Children.Count);
//    		List<SectionControl> sectionsWithDuplicateTitles = new List<SectionControl>(SectionsPanel.Children.Count);
//    		foreach (SectionControl sectionControl in SectionsPanel.Children) {
//    			string title = sectionControl.SectionTitleTextBox.Text;
//    			if (titles.Contains(title)) {
//    				sectionsWithDuplicateTitles.Add(sectionControl);
//    			}
//    			else {
//    				titles.Add(title);
//    			}
//    		}
//    		
//    		foreach (SectionControl sectionControl in sectionsWithDuplicateTitles) {
//    			string originalTitle = sectionControl.SectionTitleTextBox.Text;
//    			string newTitle = originalTitle;
//    			int count = 1;
//    			while (titles.Contains(newTitle)) {
//    				count++;
//    				newTitle = originalTitle + " (" + count + ")";
//    			}
//    			sectionControl.SectionTitleTextBox.Text = newTitle;
//    			titles.Add(newTitle);    			
//    		}
//    	}   
    	
    	
    	public void AddNewSection()
    	{
    		try {
	    		SectionControl sectionControl = AddSection(new Section("New section"));
	    		sectionControl.AddNewQuestion();
	    		worksheet_Changed();
    		}
    		catch (Exception e) {
    			Say.Error("Failed to add new section.",e);
    		}
    	}
    	
    	
    	private SectionControl AddSection(Section section)
    	{
    		if (section == null) {
        		throw new ArgumentNullException("Can't add a null section field.");
        	}
    		if (!DesignerMode) { // check that this section actually has some active questions to display
	    		bool hasQuestions = false;
	    		foreach (Question question in section.Questions) {
	    			if (question.Include) {
	    				hasQuestions = true;
	    				break;
	    			}
	    		}
	    		if (!hasQuestions) {
	    			Say.Debug("Section '" + section.Title + "' had no questions - did not display.");
	    			return null;
	    		}
    		}
    		        	
        	SectionControl sc = (SectionControl)section.GetControl(WorksheetViewer.DesignerMode);
    		SectionsPanel.Children.Add(sc);
    		sc.BringIntoView(); // bring the new section into view
    		WatchForChanges(sc);
    		return sc;
    	}
    	    	
    	    	
    	public bool Save()
    	{
    		if (originalWorksheet == null) {
    			Say.Error("Save failed: Should not have called Save without opening a worksheet.");
    			return false;
    		}
    		else if (filename == null) {
    			Say.Error("Save failed: Should not have called Save without giving a filename.");
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
		    EditMenu.Visibility = Visibility.Collapsed;
		   	BorderClosingRectangle.Visibility = Visibility.Collapsed;
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
	    		ValidateTitles(); // ensure all section and question names are unique
		    	foreach (SectionControl sc in SectionsPanel.Children) {
	    			ws.Sections.Add((Section)sc.GetWorksheetPart());
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
    	
    	
    	private void UpdateTitleBar()
    	{
    		string fn;
			if (filename == null || filename == String.Empty) {
    			fn = "Untitled";
			}
			else {
    			fn = Path.GetFileName(filename);
	    		if (dirty) {
	    			fn += "*";
	    		}
			}
    		if (DesignerMode) {
    			fn += " - Evaluation (Designer mode)";
    		}
    		else {
    			fn += " - Evaluation";
    		}    		
    		Title = fn;
    	}
    	    	
    	#endregion
    	    	
    	#region Event handlers
    	
    	private void OnClick_New(object sender, EventArgs e)
    	{    		
    		try {
    			Open(new Worksheet(),null);
    			AddNewSection();
    		}
    		catch (Exception ex) {
    			Say.Error("Was unable to open worksheet.",ex);
    			CloseWorksheet();
    		}
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
    		control.Moving += new EventHandler<MovingEventArgs>(sectionControl_Moving);
    		WatchForChanges((OptionalWorksheetPartControl)control);
    	}
    	
    	
    	private void WatchForChanges(OptionalWorksheetPartControl control)
    	{
    		control.Activated += delegate { worksheet_Changed(); };
    		control.Deactivated += delegate { worksheet_Changed(); };
    		control.Changed += delegate { worksheet_Changed(); };
    	}
    	
    	
    	private void sectionControl_Deleting(object sender, DeletingEventArgs e)
    	{
    		DeleteFrom(e.DeletingControl,SectionsPanel.Children);
    	}
    	
    	
    	private void DeleteFrom(OptionalWorksheetPartControl deleting, UIElementCollection siblings)
    	{
    		if (siblings.Contains(deleting)) {
    			siblings.Remove(deleting);
    			worksheet_Changed(); // TODO would be better to raise a WorksheetControl.Changed event
    		}
    		else {
    			throw new InvalidOperationException("Received instruction to delete a control " + 
    			                                    "( " + deleting.ToString() +
    			                                    ") that was not a part of this worksheet.");
    		}
    	}
    	
    	
    	private void sectionControl_Moving(object sender, MovingEventArgs e)
    	{
    		MoveWithin(e.MovingControl,SectionsPanel.Children,e.MoveUp);
    		worksheet_Changed(); // TODO would be better to raise a WorksheetControl.Changed event
    	}
    	
    	
    	internal static void MoveWithin(OptionalWorksheetPartControl moving, UIElementCollection siblings, bool moveUp)
    	{    		
    		if (siblings.Contains(moving)) {
    			if (siblings.Count > 1) {   
    				int currentIndex = siblings.IndexOf(moving);
    				int newIndex = currentIndex;
    				if (moveUp) {		
    					newIndex--;
    					if (newIndex < 0) {
    						return; // as low as it can go, so do nothing
    					}    	
    				}
    				else {
    					newIndex++;
    					if (newIndex == siblings.Count) {
    						return; // as high as it can go, so do nothing
    					}		
    				}    				
    				siblings.RemoveAt(currentIndex);
    				siblings.Insert(newIndex,moving);
    				moving.BringIntoView();    				
    			}
    		}
    		else {
    			throw new InvalidOperationException("Received instruction to move a control " + 
    			                                    "( " + moving +
    			                                    ") that was not a part of this worksheet.");
    		}
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
        		throw new InvalidOperationException("Should not have been possible to click this " +
        		                                    "when not in designer mode.");
        	}
        	
        	AddNewSection();
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
