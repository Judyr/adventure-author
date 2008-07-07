using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using AdventureAuthor.Evaluation;
using AdventureAuthor.Utils;
using Microsoft.Win32;

namespace AdventureAuthor.Evaluation
{
	/// <summary>
	/// A window which provides a number of criteria for evaluating a module
	/// and elicits a response from the user as to how well they have been met.
	/// </summary>
    public partial class CardViewer : Window
    {    	
    	#region Constants
    	    	
    	public const string DEFAULT_TITLE = "Comment Cards";
    	public const string NEW_SECTION_NAME = "New section";
    	public const string NEW_QUESTION_NAME = "New question";
    	public const string NEW_CARD_NAME = "New Comment Card";
    	
    	#endregion
    	    	
    	#region Fields
    	
    	/// <summary>
    	/// The single instance of the Comment Card viewer window.
    	/// <remarks>Pseudo-Singleton pattern, but I haven't really implemented this.</remarks>
    	/// </summary>
    	private static CardViewer instance;    	
		public static CardViewer Instance {
			get { return instance; }
			set { instance = value; }
		}   
    	
    	
    	private Mode evaluationMode;    	
		public Mode EvaluationMode {
			get { return evaluationMode; }
			internal set {
				evaluationMode = value;	
				Log.WriteAction(LogAction.mode,"evaluation_" + evaluationMode);
			}
		}
    	
    	
    	private Card originalCard;
    	
    	
    	private string filename;    	
		public string Filename {
			get { return filename; }
			set { 
				filename = value;
				UpdateTitleBar();
			}
		}    	
    	
    	
    	/// <summary>
    	/// True if fields have been changed since the last save; false otherwise.
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
    	
    	public CardViewer()
    	{
    		InitializeComponent();    		    		
    		
    		CardViewer.Instance = this;
    			
    		ShowSelectModeScreen();
    		Title = DEFAULT_TITLE;
    		
    		MinWidth = AdventureAuthor.Utils.Tools.MINIMUMWINDOWWIDTH;
			MinHeight = AdventureAuthor.Utils.Tools.MINIMUMWINDOWHEIGHT;
				
    		Log.WriteAction(LogAction.launched,"evaluation");   
    		
    		Closing += new CancelEventHandler(viewerClosing);
    		Changed += new EventHandler(changeMade);
            
    		EvaluationPreferences.Instance.PropertyChanged += new PropertyChangedEventHandler(userPreferencesPropertyChanged);
            updateImageViewerSelectionMenu();
            
            try {       	
    			Tools.EnsureDirectoryExists(EvaluationPreferences.LocalAppDataDirectory);
            	Tools.EnsureDirectoryExists(EvaluationPreferences.Instance.SavedCommentCardsDirectory);     
            }
            catch (Exception e) {
            	Say.Error("A problem was encountered when trying to create a folder for saved Comment Cards.",e);
            }   
    	}

    	
    	private void userPreferencesPropertyChanged(object sender, PropertyChangedEventArgs e)
    	{
    		if (e.PropertyName == "ImageViewer") {
    			updateImageViewerSelectionMenu();
    		}
    	}
    	

    	private void viewerClosing(object sender, CancelEventArgs ea)
    	{	
			if (!CloseCardDialog()) {
				ea.Cancel = true;
			}
    		else {
    			try {
	    			// Serialize the user's preferences:
	    			Serialization.Serialize(EvaluationPreferences.DefaultPreferencesPath,EvaluationPreferences.Instance);
    			}
    			catch (Exception e) {
    				Say.Error("Something went wrong when trying to save your preferences - the choices " +
    				          "you have made may not have been saved.",e);
    			}
    			
    			Log.WriteAction(LogAction.exited,"evaluation");
    		}
    	}

    	
    	private void updateImageViewerSelectionMenu()
    	{
    		switch (EvaluationPreferences.Instance.ImageViewer) {
    			case ImageApp.Default:
    				if (!UseDefaultMenuItem.IsChecked) {
	    				UseDefaultMenuItem.IsChecked = true;
	    				UseDefaultMenuItem.IsCheckable = false;
    				}
    				if (UsePaintMenuItem.IsChecked) {
	    				UsePaintMenuItem.IsChecked = false;
	    				UsePaintMenuItem.IsCheckable = true;
    				}
    				break;
    			case ImageApp.MicrosoftPaint:
    				if (UseDefaultMenuItem.IsChecked) {
	    				UseDefaultMenuItem.IsChecked = false;
	    				UseDefaultMenuItem.IsCheckable = true;
    				}
    				if (!UsePaintMenuItem.IsChecked) {    					
	    				UsePaintMenuItem.IsChecked = true;
	    				UsePaintMenuItem.IsCheckable = false;
    				}
    				break;
    		}
    	}
    	
    	#endregion
    	    	
    	#region Methods
    	
    	/// <summary>
    	/// Open a Comment Card.
    	/// </summary>
    	/// <param name="card">The Comment Card to open</param>
    	/// <param name="sourceFilename">The filename this Comment Card was deserialized from,
    	/// or null if the Comment Card was created in code</param>
    	private void Open(Card card, string sourceFilename)
    	{
    		CloseCardDialog();	    					
    		    		
    		// Check whether there are duplicated question or section titles - if there are, then
    		// prevent the user from opening the Comment Card unless they're in design mode (it's 
    		// relatively easy to fix when the whole Comment Card is displayed, but more of a pain
    		// when some parts of the Comment Card are inactive and do not have on-screen controls.)
    		if (EvaluationMode != Mode.Designer) {    			
    			string warning = "This Comment Card contains duplicate sections or questions " +
						    	 "and cannot be opened. Try opening and then saving the Comment Card " +
						    	 "in designer mode - doing so will rename the duplicate sections and fix this problem.";    			
    			List<string> sectionTitles = new List<string>(card.Sections.Count);
	    		foreach (Section section in card.Sections) {
    				if (sectionTitles.Contains(section.Title)) {
	    				Say.Error(warning);
	    				CloseCard();
    					return;
	    			}
	    			else {
	    				sectionTitles.Add(section.Title);
	    			}
    				List<string> questionTitles = new List<string>(section.Questions.Count);
    				foreach (Question question in section.Questions) {
	    				if (questionTitles.Contains(question.Text)) {
		    				Say.Error(warning);
	    					CloseCard();
	    					return;
		    			}
		    			else {
		    				questionTitles.Add(question.Text);
		    			}
    				}
	    		}
    		}    		
    		
    		bool createdBlankCopyOfCard;
    		if (EvaluationMode == Mode.Designer && !card.IsBlank()) {    
    			createdBlankCopyOfCard = true;
    			card = card.GetBlankCopy();
    			Filename = null;  			
    		}
    		else {
    			createdBlankCopyOfCard = false;
    			Filename = sourceFilename;
    		}
    		
    		Log.WriteAction(LogAction.opened,"commentcard",Path.GetFileName(filename));
    		
	    	originalCard = card;
	    	TitleField.SetText(card.Title);
	    	DateField.SetText(card.Date);
	    	DesignerNameField.SetText(card.DesignerName);
	    	EvaluatorNameField.SetText(card.EvaluatorName);
	    	    		
	    	foreach (Section section in card.Sections) {	    			
	    		if (EvaluationMode == Mode.Designer || section.Include) {
	    			try {	    					
		    			SectionControl sectionControl = AddSection(section);
		    			if (sectionControl != null) {					    		
				    		foreach (QuestionControl questionControl in sectionControl.QuestionsPanel.Children) {
				    			if (EvaluationMode == Mode.Designer && !sectionControl.IsActive) {
				    				questionControl.Deactivate(true);
				    			}
		    					else if (EvaluationMode == Mode.User_Discuss) {
		    						questionControl.Activate();
		    						questionControl.HideActivationControls();
		    					}
				    			foreach (CardPartControl answerControl in questionControl.AnswersPanel.Children) {
				    				if (EvaluationMode == Mode.Designer && !questionControl.IsActive) {
				    					answerControl.Deactivate(true);
				    				}
			    					else if (EvaluationMode == Mode.User_Discuss) {
			    						if (answerControl is EvidenceControl) {
			    							EvidenceControl ec = (EvidenceControl)answerControl;
			    							ec.ViewLink.IsEnabled = true;
			    							ec.SelectLink.IsEnabled = false;
			    							ec.ClearLink.IsEnabled = false;
			    						}
			    						else {
			    							// activating a control should usually make the Comment Card dirty,
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
	    				CloseCard();
	    				Say.Error(e);
	    				return;
	    			}
	    		}
	    	}
	    		    	
            DateField.TextEdited += delegate(object sender, TextEditedEventArgs e) {
            	Log.WriteAction(LogAction.edited,"commentcarddate",e.NewValue);
            };
            DesignerNameField.TextEdited += delegate(object sender, TextEditedEventArgs e) {
            	Log.WriteAction(LogAction.edited,"commentcarddesignername",e.NewValue);
            };
            EvaluatorNameField.TextEdited += delegate(object sender, TextEditedEventArgs e) {
            	Log.WriteAction(LogAction.edited,"commentcardevaluatorname",e.NewValue);
            };
            TitleField.TextEdited += delegate(object sender, TextEditedEventArgs e) {
            	Log.WriteAction(LogAction.edited,"commentcardtitle",e.NewValue);
            };
	    	
	    	DateField.TextChanged += delegate { 
	    		OnChanged(new EventArgs()); 
	    	};
	    	DesignerNameField.TextChanged += delegate { 
	    		OnChanged(new EventArgs()); 
	    	};
	    	EvaluatorNameField.TextChanged += delegate { 
	    		OnChanged(new EventArgs()); 
	    	};
	    	TitleField.TextChanged += delegate { 
	    		OnChanged(new EventArgs()); 
	    	};
	    			    
		    if (EvaluationMode == Mode.Designer) {
		    	DesignerModeControls.Visibility = Visibility.Visible;
		    	UserModeControls.Visibility = Visibility.Collapsed;
		    }
		    else {
		    	DesignerModeControls.Visibility = Visibility.Collapsed;
		    	UserModeControls.Visibility = Visibility.Visible;
		    }
		  
		    SaveMenuItem.IsEnabled = true;
		    SaveAsMenuItem.IsEnabled = true;
		    SaveBlankMenuItem.IsEnabled = true;
		    ExportMenuItem.IsEnabled = true;
		    CloseMenuItem.IsEnabled = true;
		    EditMenu.IsEnabled = true;
		    OptionsMenu.IsEnabled = true;
		    
		    Dirty = false; // never require user to save a file that has had no changes made, even a blank file
		    
		    Scroller.ScrollToTop();
		    
		    ShowMainScreen();
		    		
		    // this comes at the end so you don't see the Select Mode screen in background:
		    if (createdBlankCopyOfCard) { 
    			string message = "Someone has already written on this Comment Card - " +
    				"once you're finished, remember to save under a different filename, " + 
    				"or they'll lose their work.";
    			Say.Information(message);
		    }
    	} 
    	
    	    	
    	public void Open(string filename)
    	{
    		if (!File.Exists(filename)) {
    			Say.Error(filename + " could not be found.");
    			return;
    		}
    			
    		try {
	    		object o = AdventureAuthor.Utils.Serialization.Deserialize(filename,typeof(Card));
	    		Card card = (Card)o;
	    		Open(card,filename);
    		}
    		catch (Exception e) {
    			Say.Error("Was unable to open Comment Card.",e);
    			CloseCard();
    			this.Filename = null;
    		}
    	}
    	
    	
    	private string GetTitle(CardPartControl control)
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
    	
    	
    	private void SetTitle(CardPartControl control, string title)
    	{
    		if (control is SectionControl) {
    			((SectionControl)control).SectionTitleTextBox.SetText(title);
    		}
    		else if (control is QuestionControl) {
    			((QuestionControl)control).QuestionTitle.SetText(title);
    		}
    		else {
    			throw new ArgumentException("Can only set titles of sections and questions, not " +
    			                            control.GetType().ToString() + ".");
    		}
    	}
    	
    	
		private void ValidateTitles(UIElementCollection controls)
    	{
			List<string> titles = new List<string>(controls.Count);
    		List<CardPartControl> controlsWithDuplicateTitles 
    			= new List<CardPartControl>(controls.Count);
    		foreach (CardPartControl control in controls) {
    			string title = GetTitle(control);    			
    			if (titles.Contains(title)) {
    				controlsWithDuplicateTitles.Add(control);
    			}
    			else {
    				titles.Add(title);
    			}
    		}
    		
    		foreach (CardPartControl control in controlsWithDuplicateTitles) {
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
    			Log.WriteAction(LogAction.added,"section");
	    		SectionControl sectionControl = AddSection(new Section(NEW_SECTION_NAME));
	    		Log.WriteAction(LogAction.added,"question");
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
    		if (EvaluationMode != Mode.Designer) { // check that this section actually has some active questions to display
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
    		control.Deleting += new EventHandler<CardPartControlEventArgs>(sectionControl_Deleting);
    		control.Moving += new EventHandler<MovingEventArgs>(sectionControl_Moving);
    		control.Activated += delegate { 
    			OnChanged(new EventArgs());
    		};
    		control.Deactivated += delegate { 
    			OnChanged(new EventArgs()); 
    		};
    		control.Changed += delegate { 
    			OnChanged(new EventArgs()); 
    		};
    		control.BringIntoView();
    		OnChanged(new EventArgs());
    	}
        
    	    	
    	private bool Save()
    	{
    		if (originalCard == null) {
    			Say.Error("Save failed: Should not have called Save without opening a Comment Card.");
    			return false;
    		}
    		else if (filename == null) {
    			Say.Error("Save failed: Should not have called Save without giving a filename.");
    			return false;
    		}
    		else {
    			Log.WriteAction(LogAction.saved,"commentcard",Path.GetFileName(Filename));
	    		AdventureAuthor.Utils.Serialization.Serialize(Filename,GetCard());
	    		if (Dirty) {
	    			Dirty = false;
	    		}
	    		return true;
    		}
    	}
    	    	
    	
    	public void CloseCard()
    	{
    		if (filename != null && filename != String.Empty) {
    			Log.WriteAction(LogAction.closed,"commentcard",Path.GetFileName(Filename));
    		}
    		
    		Filename = null;
    		originalCard = null;
    		
    		TitleField.Clear();
    		DesignerNameField.Clear();
    		EvaluatorNameField.Clear();
    		DateField.Clear();
    		
    		Title = DEFAULT_TITLE;
		    
    		SectionsPanel.Children.Clear();
    		
    		Dirty = false;
    		
    		ShowSelectModeScreen();
    	}
    	
    	
    	/// <summary>
    	/// Construct a Comment Card object (including the user's entries)
    	/// </summary>
    	/// <returns>A Comment Card object, or null if no Comment Card is open</returns>
    	public Card GetCard()
    	{
    		Card ws = null;
    		
    		switch (EvaluationMode) {
    		
	    		// In design mode, everything in the Comment Card is displayed on the screen, even the parts
	    		// which are inactive, so we can just save everything that's represented by a control.
	    		
    			case Mode.Designer:
    				
		    		ws = new Card(TitleField.Text,DesignerNameField.Text,EvaluatorNameField.Text,DateField.Text);	    		
		    		ValidateTitles(); // ensure all section and question names are unique
			    	foreach (SectionControl sc in SectionsPanel.Children) {
		    			ws.Sections.Add((Section)sc.GetCardPart());
			    	}
		    		break;
		    		
	    		// When not in design mode, some Comment Card parts are excluded - i.e. they're not displayed
	    		// on the UI, but we still want to remember their existence. As a result we can't just
	    		// save only the questions and answers on the UI, as we will end up losing the ones that
	    		// have been excluded. Instead, we find the original Comment Card part that each control corresponds 
	    		// to, and if the control has a new value, we save it over the original value. The exception
	    		// is replies, since these cannot be excluded, so we don't have to check them against an original
	    		// value.
		    		
		    	default:
	    		
	    			if (originalCard == null) {
	    				throw new ArgumentException("Could not find object representing the opened Comment Card.");
	    			}    			
	    			ws = originalCard.GetCopy();
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
	    					Question question2 = (Question)qc.GetCardPart();
	    					
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
    			fn = "Comment Cards";
			}
			else {
    			fn = Path.GetFileName(filename);
	    		if (dirty) {
	    			fn += "*";
	    		}
    			fn += " - Comment Cards";
			}
    		
    		Title = fn;
    	}
    	    
    	
    	private void SwitchBetweenUserModes(Mode mode)
	    {
    		try {
	    		switch (mode) {
	    			case Mode.User_Complete:
	    				DateField.IsEditable = true;
	    				DesignerNameField.IsEditable = true;
	    				EvaluatorNameField.IsEditable = true;
	    				textBlockModeExplanation.Text = "Answer each question you see. There are three ways to answer questions:\n" +
	    					"* If you see stars, choose a rating of between 1 and 5 stars.\n" +    				
	    					"* If you see a white box, click on it, and type in your answer.\n" +
	    					"* If you see an Attach Evidence button, attach a screenshot that supports your answer.";	
	    				break;
	    			case Mode.User_Discuss:
	    				DateField.IsEditable = false;
	    				DesignerNameField.IsEditable = false;
	    				EvaluatorNameField.IsEditable = false;
	    				textBlockModeExplanation.Text = "Now that all of your answers have been filled in, " +
	    					"your friends and teachers can add their own comments. Click on the speech bubble button " +
	    					"next to a question to add your own.\n\n" + 
	    					"While discussing this Comment Card, you won't be able to change your answers. If you want to change " +
	    					"them again, just click 'I want to write my answers' again.";
	    				break;
	    			case Mode.Designer:
	    				throw new InvalidOperationException("Cannot switch to another mode when in design mode.");
	    		}
	    		
	    		foreach (SectionControl sc in SectionsPanel.Children) {
	    			foreach (QuestionControl qc in sc.QuestionsPanel.Children) {
	    				foreach (CardPartControl ac in qc.AnswersPanel.Children) {
	    					if (mode == Mode.User_Complete) {
	    						ac.Enable();    						
	    					}
	    					else if (mode == Mode.User_Discuss) {
	    						if (ac is EvidenceControl) {
	    							EvidenceControl ec = (EvidenceControl)ac;
	    							ec.ViewLink.IsEnabled = true;
	    							ec.SelectLink.IsEnabled = false;
	    							ec.ClearLink.IsEnabled = false;
	    						}
	    						else {
	    							// activating a control should usually make the Comment Card dirty,
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
	    					if (mode == Mode.User_Complete) {
	    						rc.HideEditControls();
	    					}
	    					else if (mode == Mode.User_Discuss) {
	    						rc.ShowEditControls();
	    					}
	    				}
	    				if (mode == Mode.User_Complete) {
	    					qc.AddReplyButton.Visibility = Visibility.Collapsed;
	    				}
	    				else if (mode == Mode.User_Discuss) {
	    					qc.AddReplyButton.Visibility = Visibility.Visible;
	    				}
	    			}
	    		}
	    		
	    		EvaluationMode = mode;
	    		UpdateTitleBar();  
    		}
    		catch (Exception e) {
    			Say.Error("Something went wrong.",e);
    		}
    	}    	
    	
    	
    	private void ShowMainScreen()
    	{
    		this.SelectModePanel.Visibility = Visibility.Collapsed;
    		this.SelectModePanel.IsEnabled = false;
    		this.MainCommentCardsPanel.Visibility = Visibility.Visible;
    		this.MainCommentCardsPanel.IsEnabled = true;
    	}  	
    	
    	
    	private void ShowSelectModeScreen()
    	{
    		this.SelectModePanel.Visibility = Visibility.Visible;
    		this.SelectModePanel.IsEnabled = true;
    		this.MainCommentCardsPanel.Visibility = Visibility.Collapsed;
    		this.MainCommentCardsPanel.IsEnabled = false;
    	}
    	
    	
    	private void LaunchForDesigner()
    	{
    		EvaluationMode = Mode.Designer;
    		
    		// Enable/disable menus:
    		NewMenuItem.Visibility = Visibility.Visible;
    		OptionsMenu.Visibility = Visibility.Collapsed;
    		EditMenu.Visibility = Visibility.Visible;
    		SaveBlankMenuItem.Visibility = Visibility.Collapsed;
    		ExportMenuItem.Visibility = Visibility.Collapsed;
    		
    		// Enable/disable fields:
    		TitleField.IsEditable = true;
    		EvaluatorNameField.IsEditable = false;
    		DesignerNameField.IsEditable = false;
    		DateField.IsEditable = false;
    		
    		OpenNewCard();
    		Dirty = false;
    		ShowMainScreen();
    	}
    	
    	
    	private void LaunchForUser()
    	{
    		EvaluationMode = Mode.User_Complete;
    		
    		// Enable/disable menus:
    		NewMenuItem.Visibility = Visibility.Collapsed;
    		OptionsMenu.Visibility = Visibility.Visible;
    		EditMenu.Visibility = Visibility.Collapsed;
    		SaveBlankMenuItem.Visibility = Visibility.Visible;
    		ExportMenuItem.Visibility = Visibility.Visible;
    		
    		// Enable/disable fields: 	
    		TitleField.IsEditable = false;
    		EvaluatorNameField.IsEditable = true;
    		DesignerNameField.IsEditable = true;
    		DateField.IsEditable = true;
    		
    		// Shouldn't matter for Designer mode:
    		radioButtonIWantToWriteMyAnswers.IsChecked = true;
    		radioButtonIWantToDiscussMyAnswers.IsChecked = false;
    		
    		if (OpenDialog()) {
    			ShowMainScreen();
    		};    		
    	}
    	
    	#endregion
    	    	
    	#region Event handlers
    	
    	private void enterAsDesigner(object sender, EventArgs e)
    	{
    		LaunchForDesigner();
    	}
    	
    	
    	private void enterAsUser(object sender, EventArgs e)
    	{
    		LaunchForUser();
    	}
    	
    	
    	private void OnClick_New(object sender, EventArgs e)
    	{    		
    		OpenNewCard();
    	}
    	
    	
    	private void OpenNewCard()
    	{	
    		try {
    			Log.WriteAction(LogAction.added,"commentcard");
    			Card card = new Card();
    			card.Title = NEW_CARD_NAME;
    			Open(card,null);
    			AddNewSection();
    		}
    		catch (Exception ex) {
    			Say.Error("Was unable to create Comment Card.",ex);
    			CloseCard();
    		}
    	}
    	
    	
    	private void OnClick_Open(object sender, EventArgs e)
    	{    		
    		OpenDialog();
    	}
    	
    	
    	/// <summary>
    	/// 
    	/// </summary>
    	/// <returns>False if the user cancelled the dialog; true otherwise. Note that returning true
    	/// does not indicate that the Open operation was successful.</returns>
    	private bool OpenDialog()
    	{    				
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.ValidateNames = true;
    		openFileDialog.DefaultExt = Filters.COMMENTCARDS_ALL;
    		openFileDialog.Filter = Filters.COMMENTCARDS_ALL;
			openFileDialog.Title = "Select a Comment Card to open";
			openFileDialog.Multiselect = false;
			openFileDialog.RestoreDirectory = false;
			if (Directory.Exists(EvaluationPreferences.Instance.SavedCommentCardsDirectory)) {
				openFileDialog.InitialDirectory = EvaluationPreferences.Instance.SavedCommentCardsDirectory;
			}	
			else { // if you give it an invalid InitialDirectory, the dialog will (silently) refuse to load
				openFileDialog.InitialDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
			}
				
  			bool ok = (bool)openFileDialog.ShowDialog();  				
  			if (ok) {
  				Open(openFileDialog.FileName);
  			}
  			return ok;
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
    		saveFileDialog.DefaultExt = Filters.TXT_ALL;
    		saveFileDialog.Filter = Filters.TXT_ALL;
  			saveFileDialog.ValidateNames = true;
  			saveFileDialog.Title = "Select location to export Comment Card to";  	  			
  			saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
			
	  		// try to get the default filename from the filename:
	  		string suggestedFileName;
	  		if (filename == null || filename == String.Empty) {
	  			suggestedFileName = "Untitled.txt";
	  		}
	  		else {
	  			try {
	  				suggestedFileName = Path.GetFileNameWithoutExtension(filename) + ".txt";
		  		}
		  		catch (Exception e) {
		  			Say.Debug("Failed to get a suggested filename for Comment Card - " + e);
		  			suggestedFileName = "Untitled.txt";
		  		};
	  		}
	  		saveFileDialog.FileName = Path.Combine(saveFileDialog.InitialDirectory,suggestedFileName);
  			
  			bool ok = (bool)saveFileDialog.ShowDialog();  				
  			if (ok) {
  				string exportFilename = saveFileDialog.FileName;
  				Log.WriteAction(LogAction.exported,"commentcard",Path.GetFileName(exportFilename));  			
  				try {
  					ExportToTextFile(exportFilename);
  					Process.Start(exportFilename);
  				}
  				catch (IOException e) {
  					Say.Error("Failed to export Comment Card.",e);
  				}
  			}
    	}
    	
    	

		public void ExportToTextFile(string filename)
		{
			FileInfo fi = new FileInfo(filename);
			StreamWriter sw = fi.CreateText();
			sw.AutoFlush = false;
			
			Card card = GetCard();
			sw.WriteLine("Comment Card:\t" + card.Title);
			sw.WriteLine("Game designer:\t" + card.DesignerName);
			sw.WriteLine("Evaluator:\t" + card.EvaluatorName);
			sw.WriteLine("Filled in on:\t" + card.Date);
			sw.WriteLine();
			sw.WriteLine();
						
			foreach (Section section in card.Sections) {
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
							sw.WriteLine("Replies:");
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
  			   	    	    	
    	
    	private void sectionControl_Deleting(object sender, CardPartControlEventArgs e)
    	{
    		DeleteFrom(e.Control,SectionsPanel.Children);
    	}
    	
    	
    	internal static void DeleteFrom(CardPartControl deleting, UIElementCollection siblings)
    	{
    		if (siblings.Contains(deleting)) {
    			siblings.Remove(deleting);
    			OnChanged(new EventArgs());
    		}
    		else {
    			throw new InvalidOperationException("Received instruction to delete a control " + 
    			                                    "( " + deleting.ToString() +
    			                                    ") that was not a part of this Comment Card.");
    		}
    	}
    	
    	
    	private void sectionControl_Moving(object sender, MovingEventArgs e)
    	{
    		MoveWithin(e.Control,SectionsPanel.Children,e.MoveUp);
    	}
    	
    	
    	internal static void MoveWithin(CardPartControl moving, UIElementCollection siblings, bool moveUp)
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
    			                                    ") that was not a part of this Comment Card.");
    		}
    	}
    	
    	
    	private bool SaveAsDialog()
    	{
    		SaveFileDialog saveFileDialog = new SaveFileDialog();
    		saveFileDialog.AddExtension = true;
    		saveFileDialog.CheckPathExists = true;
    		saveFileDialog.DefaultExt = Filters.COMMENTCARDS_ALL;
    		saveFileDialog.Filter = Filters.COMMENTCARDS_ALL;
  			saveFileDialog.ValidateNames = true;
  			saveFileDialog.Title = "Select location to save Comment Card to";
			saveFileDialog.RestoreDirectory = false;
  			if (Directory.Exists(EvaluationPreferences.Instance.SavedCommentCardsDirectory)) {
  				saveFileDialog.InitialDirectory = EvaluationPreferences.Instance.SavedCommentCardsDirectory;
  			}
			
//			TODO: Would like to fix the following, so that it suggests a filename based on your card title,
//			but have had problems stripping out invalid characters - and if you don't, leaving punctuation in
//			somehow concatenates the file filter onto the filename, while leaving a ':' in causes a crash.
//			
//			string invalidChars = "<|>|!|:|.";
//			//string invalidChars = "<|>|:|.|?|!|\\|/|\"";			
//			if (TitleField.Text != null && TitleField.Text != String.Empty) {
//				string suggestedTitle = TitleField.Text;
//				Say.Information(suggestedTitle + "\n\nreplacing: " + invalidChars);
//				suggestedTitle = Regex.Replace(suggestedTitle,invalidChars,"");
//				
//				Say.Information("wound up with: " + suggestedTitle);
//				saveFileDialog.FileName = Path.Combine(saveFileDialog.InitialDirectory,suggestedTitle);
//			}			
  				
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
    		if (originalCard == null) {
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
	    			Say.Error("Failed to save Comment Card.",e);
	    			return false;
	    		}
    		}
    	}
    	    	
    	
    	private void OnClick_Close(object sender, EventArgs e)
    	{
    		CloseCardDialog();
    	}
    		
    		
    	private void OnClick_SwitchUserMode(object sender, EventArgs e)
    	{
    		CloseCardDialog();
    	}
    	
    	
    	private bool CloseCardDialog()
    	{
    		if (dirty) {
    			MessageBoxResult result = 
    				MessageBox.Show("Save changes to this Comment Card?","Save changes?",MessageBoxButton.YesNoCancel);
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
    		
    		CloseCard();
    		return true;
    	}
    	
    	
    	private void OnClick_MakeBlankCopy(object sender, EventArgs e)
    	{
  			Card original = GetCard();
  			if (original == null) {
  				return;
  			}
	    	Card blankCopy = original.GetBlankCopy(); 
	    	
    		SaveFileDialog saveFileDialog = new SaveFileDialog();
    		saveFileDialog.AddExtension = true;
    		saveFileDialog.CheckPathExists = true;
    		saveFileDialog.DefaultExt = Filters.COMMENTCARDS_ALL;
    		saveFileDialog.Filter = Filters.COMMENTCARDS_ALL;
  			saveFileDialog.ValidateNames = true;
  			saveFileDialog.Title = "Select location to save blank copy to";  			
			if (Directory.Exists(EvaluationPreferences.Instance.SavedCommentCardsDirectory)) {
				saveFileDialog.InitialDirectory = EvaluationPreferences.Instance.SavedCommentCardsDirectory;
			}	
  			
  			bool ok = (bool)saveFileDialog.ShowDialog();  				
  			if (ok) {
  				string filename = saveFileDialog.FileName;  
  				Log.WriteAction(LogAction.saved,"commentcard","blank copy -" + Path.GetFileName(filename));
	    		AdventureAuthor.Utils.Serialization.Serialize(filename,blankCopy);
	    		Say.Information("Created blank copy at " + filename);
  			}
    	}    	
    	
    	
    	private void OnClick_Exit(object sender, EventArgs e)
    	{
    		Close();
    	}
    	
    	
    	private void changeMade(object sender, EventArgs e)
    	{
    		if (!Dirty) {
    			Dirty = true;
    		}
    	}
        
        
        private void OnClick_AddSection(object sender, EventArgs e)
        {
        	if (EvaluationMode != Mode.Designer) {
        		throw new InvalidOperationException("Should not have been possible to click this " +
        		                                    "when not in designer mode.");
        	}  			
        	else if (GetCard() == null) {
  				return;
  			}
        	
        	AddNewSection();
        }
    	
    	
    	private void OnChecked_UsePaint(object sender, EventArgs e)
    	{
    		EvaluationPreferences.Instance.ImageViewer = ImageApp.MicrosoftPaint;
    	}
    	
    	
    	private void OnChecked_UseDefault(object sender, EventArgs e)
    	{
    		EvaluationPreferences.Instance.ImageViewer = ImageApp.Default;    		
    	}
    	
    	
    	private void OnChecked_OpenHelpFile(object sender, EventArgs e)
    	{
    		string filename = Path.Combine(EvaluationPreferences.Instance.InstallDirectory,"Readme.rtf");    		
    		if (File.Exists(filename)) {
    			Process.Start(filename);
    		}
    		else {
    			Say.Warning("Couldn't find help file (" + filename + ").");
    		}
    	}
    	
    	
    	private void OnClick_DisplayAboutWindow(object sender, EventArgs e)
    	{
    		Say.Information("Comment Cards (version 0.2)\n\n" +
    		                "by Keiron Nicholson, Dr. Judy Robertson, Cathrin Howells\n" +
    		                "Heriot-Watt University\n\n" +
    		                "Email: adventure.author@googlemail.com\n" + 
    		                "Web: http://judyrobertson.typepad.com/adventure_author/about-adventure-author.html");
    	}
    	
    	
    	private void startedDiscussingAnswers(object sender, EventArgs e)
    	{
    		SwitchBetweenUserModes(Mode.User_Discuss);
    	}
    	
    	
    	private void startedWritingAnswers(object sender, EventArgs e)
    	{
    		SwitchBetweenUserModes(Mode.User_Complete);
    	}
    	
    	#endregion
    }
}
