using System;
using System.Windows.Forms.Integration;
using AdventureAuthor.Analysis;
using AdventureAuthor.Core;
using AdventureAuthor.Conversations.UI;
using AdventureAuthor.Evaluation.Viewer;
using AdventureAuthor.Ideas;
using AdventureAuthor.Utils;
using TD.SandBar;
using form = NWN2Toolset.NWN2ToolsetMainForm;

namespace AdventureAuthor.Setup
{
	/// <summary>
	/// Description of SetupMenu.
	/// </summary>
	public static partial class Toolset
	{			
		private static MenuBarItem mainFileMenu;
		private static ToolBar aaToolbar;
		private static ToolBar addIdeaToolbar;
		
		private static MenuBarItem SetupFileMenu(MenuBarItem fileMenu)
		{
			mainFileMenu = fileMenu;
			fileMenu.Items.Clear();							
			MenuButtonItem newModule = new MenuButtonItem("New module");
			newModule.Activate += delegate { NewModuleDialog(); };
			MenuButtonItem openModule = new MenuButtonItem("Open module");
			openModule.Activate += delegate { OpenModuleDialog(); };
			MenuButtonItem saveModule = new MenuButtonItem("Save module");
			saveModule.Activate += delegate { SaveModuleDialog(); };
//			MenuButtonItem saveAdventureAs = new MenuButtonItem("Save As");
//			saveAdventureAs.Activate += delegate { SaveAdventureAsDialog(); };
			MenuButtonItem bakeModule = new MenuButtonItem("Bake module");
			bakeModule.Activate += delegate { BakeModuleDialog(); };
			MenuButtonItem runModule = new MenuButtonItem("Run module");
			runModule.Activate += delegate { RunModuleDialog(); };
			MenuButtonItem closeModule = new MenuButtonItem("Close module");
			closeModule.Activate += delegate { CloseModuleDialog(); };
							
			MenuButtonItem newArea = new MenuButtonItem("New area");
			newArea.BeginGroup = true;
			newArea.Activate += delegate { NewAreaDialog(); };
			
			MenuButtonItem programmerFunctions = new MenuButtonItem("Programmer functions");
			programmerFunctions.BeginGroup = true;
			
			MenuButtonItem colourPicker = new MenuButtonItem("Colour picker");
			colourPicker.Activate += delegate { 
				RGBPicker picker = new RGBPicker();
				ElementHost.EnableModelessKeyboardInterop(picker);
				picker.ShowDialog();
			};
			programmerFunctions.Items.Add(colourPicker);
			
			MenuButtonItem logWindow = new MenuButtonItem("Display log output");
			logWindow.Activate += delegate { LogWindow window = new LogWindow(); window.Show(); };
			programmerFunctions.Items.Add(logWindow);	
						
			MenuButtonItem exitAdventureAuthor = new MenuButtonItem("Exit");
			exitAdventureAuthor.BeginGroup = true;
			exitAdventureAuthor.Activate += delegate { ExitToolsetDialog(); };
									
			fileMenu.Items.AddRange( new MenuButtonItem[] {
			                           	newModule,
			                           	openModule,
			                           	saveModule,
			                           	bakeModule,
			                           	runModule,
			                           	closeModule,
			                           	newArea,
			                           	programmerFunctions,
			                           	exitAdventureAuthor,
			                           });	
			
			return fileMenu;
		}
		
		
		public static ToolBar SetupAdventureAuthorToolBar(ToolBar toolbar)
		{
			aaToolbar = toolbar;
			aaToolbar.AddRemoveButtonsVisible = false;
			aaToolbar.AllowMerge = false;							
			aaToolbar.Text = "Adventure Author applications";			
							
			ButtonItem conversationButton = new ButtonItem();							
			conversationButton.Activate += delegate { 
				LaunchConversationWriter(true);				
				if (WriterWindow.Instance.WindowState == System.Windows.WindowState.Minimized) {
					WriterWindow.Instance.WindowState = System.Windows.WindowState.Normal;
				}
				WriterWindow.Instance.Activate(); // bring to front
			};
			conversationButton.ToolTipText = "Write interactive conversations for game characters";
			conversationButton.Enabled = false;
			Tools.SetSandbarButtonImage(conversationButton,"speechbubblesblue.png","Conversations");
			
			aaToolbar.Items.Add(conversationButton);
							
			ButtonItem variableButton = new ButtonItem();
			variableButton.Activate += delegate { LaunchVariableManager(); };
			variableButton.ToolTipText = "Manage game variables";
			variableButton.Enabled = false;
			Tools.SetSandbarButtonImage(variableButton,"gear.png","Variables");
			aaToolbar.Items.Add(variableButton);
							
			ButtonItem ideasButton = new ButtonItem();
			ideasButton.Activate += delegate { LaunchMagnetBoardViewer(); };
			ideasButton.ToolTipText = "Record and review your ideas";
			Tools.SetSandbarButtonImage(ideasButton,"litbulb.png","Ideas");
			aaToolbar.Items.Add(ideasButton);
							
			ButtonItem analysisButton = new ButtonItem();
			analysisButton.Activate += delegate { 
				if (form.App.Module.Areas.Count > 0) {
					CombatMap combatMap = new CombatMap(); 
					combatMap.Show(); 
				}
				else {
					Say.Information("There are no areas to display in map view.");
				}
			};
			Tools.SetSandbarButtonImage(analysisButton,"verticalbarchart.png","Analysis");
			analysisButton.Enabled = false;
			aaToolbar.Items.Add(analysisButton);
						
			ButtonItem evaluationButton = new ButtonItem();
			evaluationButton.Activate += delegate { 
				SelectModeWindow selectModeWindow = new SelectModeWindow();
				selectModeWindow.ShowDialog();
			};
			Tools.SetSandbarButtonImage(evaluationButton,"clipboard.png","Evaluation");
			aaToolbar.Items.Add(evaluationButton);
														
			ButtonItem achievementsButton = new ButtonItem();
			achievementsButton.Activate += delegate { };
			Tools.SetSandbarButtonImage(achievementsButton,"crown.png","Achievements");
			achievementsButton.Enabled = false;
			aaToolbar.Items.Add(achievementsButton);
			
			ModuleHelper.ModuleOpened += delegate {  
				conversationButton.Enabled = true;
				variableButton.Enabled = true;
				ideasButton.Enabled = true;
				analysisButton.Enabled = true;
				evaluationButton.Enabled = true;
				//achievementsButton.Enabled = true;
			};
			
			ModuleHelper.ModuleClosed += delegate {  
				conversationButton.Enabled = false;
				variableButton.Enabled = false;
				ideasButton.Enabled = true;
				analysisButton.Enabled = false;
				evaluationButton.Enabled = true;
				//achievementsButton.Enabled = true;
			};
        		
			// TODO: 'Ideas' should flash to indicate an idea has 'gone in' - or go from unlit to lit bulb?
			
			return aaToolbar;
		}
		
		
		public static void SetupAddIdeaToolBar(ToolBar toolbar)
		{
			toolbar.AddRemoveButtonsVisible = false;
			toolbar.Text = "Add an idea";
			toolbar.Margin = new System.Windows.Forms.Padding(3);
			toolbar.BackColor = System.Drawing.Color.LightBlue;
			
			LabelItem label = new LabelItem();
			label.Text = "Add an idea: ";
			
			TextBoxItem ideaEntryBox = new TextBoxItem();			
			ideaEntryBox.ToolTipText = "Type in your idea here";
			ideaEntryBox.MinimumControlWidth = 150;
			System.Windows.Forms.TextBox textBox = (System.Windows.Forms.TextBox)ideaEntryBox.ContainedControl;
			textBox.MaxLength = Idea.MAX_IDEA_LENGTH;
			
			ComboBoxItem ideaCategoryBox = new ComboBoxItem();
			ideaCategoryBox.ToolTipText = "Select an idea category";
			ideaCategoryBox.MinimumControlWidth = 100;
			ideaCategoryBox.ComboBox.SelectedValue = IdeaCategory.Other;
			ideaCategoryBox.ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			foreach (IdeaCategory cat in Idea.IDEA_CATEGORIES) {
				ideaCategoryBox.Items.Add(cat);
			}
							
			ButtonItem addButton = new ButtonItem();	
			addButton.Text = "Add";
			addButton.Activate += delegate { 
				string text = ((System.Windows.Forms.TextBox)ideaEntryBox.ContainedControl).Text;
				if (text == String.Empty) {
					return;
				}
				IdeaCategory category;
				object selected = ((System.Windows.Forms.ComboBox)ideaCategoryBox.ContainedControl).SelectedItem;
				if (selected == null) {
					category = IdeaCategory.Other;
				}
				else {
					category = (IdeaCategory)selected;
				}
				Idea idea = new Idea(text,category,User.GetCurrentUser());
				OnIdeaSubmitted(new IdeaEventArgs(idea));
				((System.Windows.Forms.TextBox)ideaEntryBox.ContainedControl).Text = String.Empty;
			};
			addButton.ToolTipText = "Add this idea to your ideas screen";
			
			ButtonItem clearButton = new ButtonItem();	
			clearButton.Text = "Clear";
			clearButton.Activate += delegate { 
				((System.Windows.Forms.TextBox)ideaEntryBox.ContainedControl).Text = String.Empty;
			};
			clearButton.ToolTipText = "Clear this idea";					
			
			toolbar.Items.Add(label);
			toolbar.Items.Add(ideaEntryBox);
			toolbar.Items.Add(ideaCategoryBox);
			toolbar.Items.Add(addButton);
			toolbar.Items.Add(clearButton);
		}
	}
}
