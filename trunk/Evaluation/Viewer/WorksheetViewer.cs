using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
    	    	
    	private string DEFAULT_TITLE = "Evaluation";
    	
    	#endregion
    	    	
    	#region Fields
    	
    	private static Mode evaluationMode;    	
		public static Mode EvaluationMode {
			get { return evaluationMode; }
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
    	
    	#region Events
    	
    	public static event EventHandler Changed;    	
    	protected static void OnChanged(EventArgs e)
    	{
    		EventHandler handler = Changed;
    		if (handler != null) {
    			handler(null,e);
    		}
    	}
    	
    	#endregion
    	    	    	    	
    	#region Constructors    	   	
    	
    	public WorksheetViewer(Mode mode)
    	{
    		InitializeComponent();
    		this.Closing += new CancelEventHandler(WorksheetClosing);
    		Changed += new EventHandler(WorksheetChanged);
    		EvaluationOptions.ChangedDefaultImageApplication += 
    			new EventHandler(EvaluationOptions_ChangedDefaultImageApplication);    		    			
    		evaluationMode = mode;
    		
    		// Edit worksheet titles in design mode only; fill in name and date in complete/discuss mode only
    		switch (EvaluationMode) {
    				
    			case Mode.Design:
	    			TitleField.IsEnabled = true;
		    		DesignerNameField.IsEnabled = false;
		    		EvaluatorNameField.IsEnabled = false;
		    		DateField.IsEnabled = false;
		    		
		    		switchModeButton.Visibility = Visibility.Collapsed;
		    		addSectionButton.Visibility = Visibility.Collapsed; // because nothing is open yet
		    		
	    			SaveBlankMenuItem.Visibility = Visibility.Collapsed;
	    			NewMenuItem.Visibility = Visibility.Visible;
	    			OptionsMenu.Visibility = Visibility.Collapsed;
	    			
	    			EditMenu.Visibility = Visibility.Visible;
    				break;
    				
    			case Mode.Complete:
	    			TitleField.IsEnabled = false;
		    		DesignerNameField.IsEnabled = true;
		    		EvaluatorNameField.IsEnabled = true;
		    		DateField.IsEnabled = true;
		    		
		    		switchModeButton.Visibility = Visibility.Visible;
		    		addSectionButton.Visibility = Visibility.Collapsed;
		    		
	    			SaveBlankMenuItem.Visibility = Visibility.Visible;
	    			NewMenuItem.Visibility = Visibility.Collapsed;
	    			OptionsMenu.Visibility = Visibility.Visible;
	    			
	    			EditMenu.Visibility = Visibility.Collapsed;	    		
    				SwitchModesMenuItem.Header = "Switch to Discuss mode";
    				switchModeButton.Content = "Switch to Discuss mode";
    				OpenDialog();
		    		break;
    				
    			case Mode.Discuss:
	    			TitleField.IsEnabled = false;
		    		DesignerNameField.IsEnabled = false;
		    		EvaluatorNameField.IsEnabled = false;
		    		DateField.IsEnabled = false;
		    		
		    		switchModeButton.Visibility = Visibility.Visible;
		    		addSectionButton.Visibility = Visibility.Collapsed;
		    		
	    			SaveBlankMenuItem.Visibility = Visibility.Visible;
	    			NewMenuItem.Visibility = Visibility.Collapsed;
	    			OptionsMenu.Visibility = Visibility.Visible;
	    			
	    			EditMenu.Visibility = Visibility.Collapsed; 		
    				SwitchModesMenuItem.Header = "Switch to Complete mode";
    				switchModeButton.Content = "Switch to Complete mode";
    				OpenDialog();
		    		break;
    		}
    	}
    	

    	private void WorksheetClosing(object sender, CancelEventArgs e)
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
    		    		
    		// Check whether there are duplicated question or section titles - if there are, then
    		// prevent the user from opening the worksheet unless they're in design mode (it's 
    		// relatively easy to fix when the whole worksheet is displayed, but more of a pain
    		// when some parts of the worksheet are inactive and do not have on-screen controls.)
    		if (EvaluationMode != Mode.Design) {    			
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
    		
    		if (EvaluationMode == Mode.Design && !worksheet.IsBlank()) {
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
	    	DesignerNameField.Text = worksheet.DesignerName;
	    	EvaluatorNameField.Text = worksheet.EvaluatorName;
	    	    		
	    	foreach (Section section in worksheet.Sections) {	    			
	    		if (EvaluationMode == Mode.Design || section.Include) {
	    			try {	    					
		    			SectionControl sectionControl = AddSection(section);
		    			if (sectionControl != null) {					    		
				    		foreach (QuestionControl questionControl in sectionControl.QuestionsPanel.Children) {
				    			if (EvaluationMode == Mode.Design && !sectionControl.IsActive) {
				    				questionControl.Deactivate(true);
				    			}
		    					else if (EvaluationMode == Mode.Discuss) {
		    						questionControl.Activate();
		    						questionControl.HideActivationControls();
		    					}
				    			foreach (OptionalWorksheetPartControl answerControl in questionControl.AnswersPanel.Children) {
				    				if (EvaluationMode == Mode.Design && !questionControl.IsActive) {
				    					answerControl.Deactivate(true);
				    				}
			    					else if (EvaluationMode == Mode.Discuss) {
			    						if (answerControl is EvidenceControl) {
			    							EvidenceControl ec = (EvidenceControl)answerControl;
			    							ec.ViewLink.IsEnabled = true;
			    							ec.SelectLink.IsEnabled = false;
			    							ec.ClearLink.IsEnabled = false;
			    						}
			    						else {
			    							// activating a control should usually make the worksheet dirty,
			    							// but not in this case:
			    							bool isDirty = Dirty;
				    						answerControl.Activate();    						
				    						answerControl.HideActivationControls();
				    						if (isDirty == false) {
				    							Dirty = false;
				    						}
			    						}
			    					}
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
	    		    	
	    	DateField.TextChanged += delegate { OnChanged(new EventArgs()); };
	    	DesignerNameField.TextChanged += delegate { OnChanged(new EventArgs()); };
	    	EvaluatorNameField.TextChanged += delegate { OnChanged(new EventArgs()); };
	    	TitleField.TextChanged += delegate { OnChanged(new EventArgs()); };
	    	
	    	TitleLabel.Visibility = Visibility.Visible;
	    	DesignerNameLabel.Visibility = Visibility.Visible;
	    	EvaluatorNameLabel.Visibility = Visibility.Visible;
	    	DateLabel.Visibility = Visibility.Visible;
	    	
	    	TitleField.Visibility = Visibility.Visible;
		    DesignerNameField.Visibility = Visibility.Visible;
		    EvaluatorNameField.Visibility = Visibility.Visible;
		    DateField.Visibility = Visibility.Visible;
		    
		    BorderClosingRectangle.Visibility = Visibility.Visible;
		    
		    switch (EvaluationMode) {
		    	case Mode.Design:
		    		//addSectionButton.Visibility = Visibility.Visible; currently disabled cos it makes UI look crap
		    		switchModeButton.Visibility = Visibility.Collapsed;
		    		break;
		    	case Mode.Complete:
		    		addSectionButton.Visibility = Visibility.Collapsed;
		    		switchModeButton.Content = "Switch to Discuss mode";
		    		switchModeButton.Visibility = Visibility.Visible;
		    		break;
		    	case Mode.Discuss:
		    		addSectionButton.Visibility = Visibility.Collapsed;
		    		switchModeButton.Content = "Switch to Complete mode";
		    		switchModeButton.Visibility = Visibility.Visible;
		    		break;
		    }
		  
		    SaveMenuItem.IsEnabled = true;
		    SaveAsMenuItem.IsEnabled = true;
		    SaveBlankMenuItem.IsEnabled = true;
		    ExportMenuItem.IsEnabled = true;
		    CloseMenuItem.IsEnabled = true;
		    EditMenu.IsEnabled = true;
		    OptionsMenu.IsEnabled = true;
		    
		    if (Filename == null || Filename == String.Empty) {
		    	Dirty = true;  // set Dirty to true if we don't have a filename to save to yet...
		    }
		    else {
		    	Dirty = false; // ...otherwise expressly set Dirty to false, so as to ignore the  
		    				   // meaningless TextChanged event when the Title, Name and Date fields are created
		    }    
		    
		    Scroller.ScrollToTop();
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
    	
    	
    	public void AddNewSection()
    	{
    		try {
	    		SectionControl sectionControl = AddSection(new Section("New section"));
	    		sectionControl.AddNewQuestion();
	    		OnChanged(new EventArgs());
    		}
    		catch (Exception e) {
    			Say.Error("Failed to add new section.",e);
    		}
    	}
    	
    	
    	public SectionControl AddSection(Section section)
    	{
    		if (section == null) {
        		throw new ArgumentNullException("Can't add a null section field.");
        	}
    		if (EvaluationMode != Mode.Design) { // check that this section actually has some active questions to display
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
    		        	
        	SectionControl sc = (SectionControl)section.GetControl();
        	AddSectionField(sc);
    		return sc;
    	}
    	
    	
    	private void AddSectionField(SectionControl control)
    	{
        	if (control == null) {
        		throw new ArgumentNullException("Can't add a null section field.");
        	}   
    		SectionsPanel.Children.Add(control);
    		control.Deleting += new EventHandler<OptionalWorksheetPartControlEventArgs>(sectionControl_Deleting);
    		control.Moving += new EventHandler<MovingEventArgs>(sectionControl_Moving);
    		control.Activated += delegate { OnChanged(new EventArgs()); };
    		control.Deactivated += delegate { OnChanged(new EventArgs()); };
    		control.Changed += delegate { OnChanged(new EventArgs()); };
    		control.BringIntoView();
    		OnChanged(new EventArgs());
    	}
        
    	    	
    	private bool Save()
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
    		DesignerNameField.Clear();
    		EvaluatorNameField.Clear();
    		DateField.Clear();
    		
    		Title = DEFAULT_TITLE;
    		
    		TitleLabel.Visibility = Visibility.Hidden;
	    	DesignerNameLabel.Visibility = Visibility.Hidden;
	    	EvaluatorNameLabel.Visibility = Visibility.Hidden;
	    	DateLabel.Visibility = Visibility.Hidden;
	    	addSectionButton.Visibility = Visibility.Collapsed;
	    	switchModeButton.Visibility = Visibility.Collapsed;
	    	
		    TitleField.Visibility = Visibility.Hidden;
	    	DesignerNameField.Visibility = Visibility.Hidden;
	    	EvaluatorNameField.Visibility = Visibility.Hidden;
		    DateField.Visibility = Visibility.Hidden;
		    
		   	BorderClosingRectangle.Visibility = Visibility.Collapsed;
		   	
		    SaveMenuItem.IsEnabled = false;
		    SaveAsMenuItem.IsEnabled = false;
		    SaveBlankMenuItem.IsEnabled = false;
		    CloseMenuItem.IsEnabled = false;
		    ExportMenuItem.IsEnabled = false;
		    EditMenu.IsEnabled = false;
		    OptionsMenu.IsEnabled = false;
		    
    		SectionsPanel.Children.Clear();
    		
    		Dirty = false;
    	}
    	
    	
    	/// <summary>
    	/// Construct a worksheet object (including the user's entries)
    	/// </summary>
    	/// <returns>A worksheet object, or null if no worksheet is open</returns>
    	public Worksheet GetWorksheet()
    	{
    		Worksheet ws = null;
    		
    		switch (EvaluationMode) {
    		
	    		// In design mode, everything in the worksheet is displayed on the screen, even the parts
	    		// which are inactive, so we can just save everything that's represented by a control.
	    		
    			case Mode.Design:
    				
		    		ws = new Worksheet(TitleField.Text,DesignerNameField.Text,EvaluatorNameField.Text,DateField.Text);	    		
		    		ValidateTitles(); // ensure all section and question names are unique
			    	foreach (SectionControl sc in SectionsPanel.Children) {
		    			ws.Sections.Add((Section)sc.GetWorksheetPart());
			    	}
		    		break;
		    		
	    		// When not in design mode, some worksheet parts are excluded - i.e. they're not displayed
	    		// on the UI, but we still want to remember their existence. As a result we can't just
	    		// save only the questions and answers on the UI, as we will end up losing the ones that
	    		// have been excluded. Instead, we find the original worksheet part that each control corresponds 
	    		// to, and if the control has a new value, we save it over the original value. The exception
	    		// is replies, since these cannot be excluded, so we don't have to check them against an original
	    		// value.
		    		
		    	default:
	    		
	    			if (originalWorksheet == null) {
	    				throw new ArgumentException("Could not find object representing the opened worksheet.");
	    			}    			
	    			ws = originalWorksheet.GetCopy();
	    			if (ws.Title != TitleField.Text) {
	    				ws.Title = TitleField.Text;
	    			}
	    			if (ws.EvaluatorName != EvaluatorNameField.Text) {
	    				ws.EvaluatorName = EvaluatorNameField.Text;
	    			}
	    			if (ws.DesignerName != DesignerNameField.Text) {
	    				ws.DesignerName = DesignerNameField.Text;
	    			}
	    			if (ws.Date != DateField.Text) {
	    				ws.Date = DateField.Text;
	    			}
	    			
	    			foreach (SectionControl sc in SectionsPanel.Children) {
	    				foreach (QuestionControl qc in sc.QuestionsPanel.Children) {
	    					Question question = ws.GetQuestion(qc.QuestionTitle.Text,sc.SectionTitleTextBox.Text);
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
	    						question.Replies = question2.Replies;
	    					}
	    				}
	    			}
	    			
	    			break;
    		}    				
    				
    		return ws;
    	}
    	
    	
    	private void UpdateTitleBar()
    	{
    		string fn;
			if (filename == null || filename == String.Empty) {
    			fn = "Untitled*";
			}
			else {
    			fn = Path.GetFileName(filename);
	    		if (dirty) {
	    			fn += "*";
	    		}
    			fn += " - Evaluation";
			}
    		if (EvaluationMode == Mode.Design) {
    			fn += " (Designer Mode)";
    		}
    		
    		Title = fn;
    	}
    	    
    	
    	private void SwitchTo(Mode mode)
    	{
    		switch (mode) {
    			case Mode.Complete:
    				DateField.IsEnabled = true;
    				DesignerNameField.IsEnabled = true;
    				EvaluatorNameField.IsEnabled = true;
    				SwitchModesMenuItem.Header = "Switch to Discuss mode";
    				switchModeButton.Content = "Switch to Discuss mode";
    				break;
    			case Mode.Discuss:
    				DateField.IsEnabled = false;
    				DesignerNameField.IsEnabled = false;
    				EvaluatorNameField.IsEnabled = false;
    				SwitchModesMenuItem.Header = "Switch to Complete mode";
    				switchModeButton.Content = "Switch to Complete mode";
    				break;
    			case Mode.Design:
    				throw new InvalidOperationException("Cannot switch to another mode when in design mode.");
    		}
    		
    		foreach (SectionControl sc in SectionsPanel.Children) {
    			foreach (QuestionControl qc in sc.QuestionsPanel.Children) {
    				foreach (OptionalWorksheetPartControl ac in qc.AnswersPanel.Children) {
    					if (mode == Mode.Complete) {
    						ac.Enable();    						
    					}
    					else if (mode == Mode.Discuss) {
    						if (ac is EvidenceControl) {
    							EvidenceControl ec = (EvidenceControl)ac;
    							ec.ViewLink.IsEnabled = true;
    							ec.SelectLink.IsEnabled = false;
    							ec.ClearLink.IsEnabled = false;
    						}
    						else {
    							// activating a control should usually make the worksheet dirty,
    							// but not in this case:
    							bool isDirty = Dirty;
	    						ac.Activate();    						
	    						ac.HideActivationControls();
	    						if (isDirty == false) {
	    							Dirty = false;
	    						}
    						}
    					}
    				}
    				foreach (ReplyControl rc in qc.RepliesPanel.Children) {
    					if (mode == Mode.Complete) {
    						rc.HideEditControls();
    					}
    					else if (mode == Mode.Discuss) {
    						rc.ShowEditControls();
    					}
    				}
    				if (mode == Mode.Complete) {
    					qc.AddReplyButton.Visibility = Visibility.Collapsed;
    				}
    				else if (mode == Mode.Discuss) {
    					qc.AddReplyButton.Visibility = Visibility.Visible;
    				}
    			}
    		}
    		
    		evaluationMode = mode;
    		UpdateTitleBar();    		
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
    		openFileDialog.DefaultExt = Filters.XML;
    		openFileDialog.Filter = Filters.XML;
			openFileDialog.Title = "Select a worksheet file to open";
			openFileDialog.Multiselect = false;
			openFileDialog.RestoreDirectory = false;	
			
  			bool ok = (bool)openFileDialog.ShowDialog();  				
  			if (ok) {
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
    	
    	
    	private void OnClick_Export(object sender, EventArgs ea)
    	{    	
    	    SaveFileDialog saveFileDialog = new SaveFileDialog();
    		saveFileDialog.AddExtension = true;
    		saveFileDialog.CheckPathExists = true;
    		saveFileDialog.DefaultExt = Filters.TXT;
    		saveFileDialog.Filter = Filters.TXT;
  			saveFileDialog.ValidateNames = true;
  			saveFileDialog.Title = "Select location to export worksheet to";
  			bool ok = (bool)saveFileDialog.ShowDialog();  				
  			if (ok) {
  				string exportFilename = saveFileDialog.FileName;
  				try {
  					ExportToTextFile(exportFilename);
  					Process.Start(exportFilename);
  				}
  				catch (IOException e) {
  					Say.Error("Failed to export worksheet.",e);
  				}
  			}
    	}
    	
    	

		public void ExportToTextFile(string filename)
		{
			FileInfo fi = new FileInfo(filename);
			StreamWriter sw = fi.CreateText();
			sw.AutoFlush = false;
			
			Worksheet worksheet = GetWorksheet();
			sw.WriteLine("Worksheet:\t" + worksheet.Title);
			sw.WriteLine("Game designer:\t" + worksheet.DesignerName);
			sw.WriteLine("Evaluator:\t" + worksheet.EvaluatorName);
			sw.WriteLine("Filled in on:\t" + worksheet.Date);
			sw.WriteLine();
			sw.WriteLine();
						
			foreach (Section section in worksheet.Sections) {
				// Check that a section has not been excluded, either because it has been excluded explicitly
				// or because it has no questions which are to be included:
				if (!section.Include) {
					continue;
				}
				else {
					bool sectionHasQuestions = false;
					foreach (Question question in section.Questions) {
						if (question.Include) {
							sectionHasQuestions = true;
							break;
						}
					}
					if (!sectionHasQuestions) {
						continue;
					}
				}
				
				int questionCount = 0;
				sw.WriteLine(">>>>> " + section.Title + " <<<<<");
				sw.WriteLine();
				foreach (Question question in section.Questions) {
					if (question.Include) {
						questionCount++;
						sw.WriteLine("Q" + questionCount + ": " + question.Text);
						sw.WriteLine();
						sw.WriteLine("Answer(s):");
						foreach (Answer answer in question.Answers) {
							if (answer.Include) {
								sw.WriteLine("- " + answer.ToString());
							}
						}
						if (question.Replies.Count > 0) {
							sw.WriteLine();
							sw.WriteLine("Comments:");
							foreach (Reply reply in question.Replies) {
								sw.WriteLine("- " + reply.ToString());
							}
						}
						sw.WriteLine();
						sw.WriteLine();
					}
				}
			}
						
			sw.Flush();
			sw.Close();
			sw.Dispose();
		}		
  			   	    	    	
    	
    	private void sectionControl_Deleting(object sender, OptionalWorksheetPartControlEventArgs e)
    	{
    		DeleteFrom(e.Control,SectionsPanel.Children);
    	}
    	
    	
    	internal static void DeleteFrom(OptionalWorksheetPartControl deleting, UIElementCollection siblings)
    	{
    		if (siblings.Contains(deleting)) {
    			siblings.Remove(deleting);
    			OnChanged(new EventArgs());
    		}
    		else {
    			throw new InvalidOperationException("Received instruction to delete a control " + 
    			                                    "( " + deleting.ToString() +
    			                                    ") that was not a part of this worksheet.");
    		}
    	}
    	
    	
    	private void sectionControl_Moving(object sender, MovingEventArgs e)
    	{
    		MoveWithin(e.Control,SectionsPanel.Children,e.MoveUp);
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
    				OnChanged(new EventArgs());
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
    		saveFileDialog.DefaultExt = Filters.XML;
    		saveFileDialog.Filter = Filters.XML;
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
    		saveFileDialog.DefaultExt = Filters.XML;
    		saveFileDialog.Filter = Filters.XML;
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
    	
    	
    	private void WorksheetChanged(object sender, EventArgs e)
    	{
    		if (!Dirty) {
    			Dirty = true;
    		}
    	}
        
        
        private void OnClick_AddSection(object sender, EventArgs e)
        {
        	if (EvaluationMode != Mode.Design) {
        		throw new InvalidOperationException("Should not have been possible to click this " +
        		                                    "when not in designer mode.");
        	}  			
        	else if (GetWorksheet() == null) {
  				return;
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
    	
    	
    	private void OnClick_SwitchMode(object sender, EventArgs e)
    	{
    		switch (EvaluationMode) {
    			case Mode.Design:
    				throw new InvalidOperationException("Cannot switch to another mode from design mode.");
    			case Mode.Complete:
    				SwitchTo(Mode.Discuss);
    				break;
    			case Mode.Discuss:
    				SwitchTo(Mode.Complete);
    				break;
    		}
    	}
    	
    	#endregion
    }
}
