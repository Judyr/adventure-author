
using System;
using AdventureAuthor.Analysis;
using AdventureAuthor.Core;
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
			newArea.Activate += delegate { NewAreaDialog(); };
			MenuButtonItem newConversation = new MenuButtonItem("Conversation writer");
			newConversation.Activate += delegate { LaunchConversationWriter(); };
			MenuButtonItem variableManager = new MenuButtonItem("Variable manager");
			variableManager.Activate += delegate { LaunchVariableManager(); };
			
//			MenuButtonItem changeUser = new MenuButtonItem("Change user");
//			changeUser.Activate += delegate { Say.Error("Not implemented yet."); };
			MenuButtonItem exitAdventureAuthor = new MenuButtonItem("Exit");
			exitAdventureAuthor.Activate += delegate { ExitToolsetDialog(); };
			
			MenuButtonItem misc = new MenuButtonItem("Miscellaneous");
			MenuButtonItem difficulty = new MenuButtonItem("Analyse hostiles");
			difficulty.Activate += delegate { 
				if (ModuleHelper.ModuleIsOpen() && form.App.Module.Areas.Count > 0) {
					CombatMap combatMap = new CombatMap(); 
					combatMap.Show(); 
				}
			};
			misc.Items.Add(difficulty);
			
			MenuButtonItem colourPicker = new MenuButtonItem("Colour picker");
			colourPicker.Activate += delegate { 
				RGBPicker picker = new RGBPicker();
				picker.ShowDialog();
			};
			misc.Items.Add(colourPicker);
			
			MenuButtonItem evaluation = new MenuButtonItem("Evaluation");
			evaluation.Activate += delegate { 
				// if (user.HasAdminRights) {
						SelectModeWindow selectModeWindow = new SelectModeWindow();
						selectModeWindow.ShowDialog();
				// }
				// else {
				// 		WorksheetViewer(WorksheetViewer.Mode.PupilMode);
				// }
			};
			
			MenuButtonItem magnets = new MenuButtonItem("Magnets");
			magnets.Activate += delegate { 
				MagnetBoardViewer mbv = new MagnetBoardViewer();
				mbv.ShowDialog();
			};
			
			MenuButtonItem logWindow = new MenuButtonItem("Display log output");
			logWindow.Activate += delegate { LogWindow window = new LogWindow(); window.Show(); };
			
			newArea.BeginGroup = true;
			exitAdventureAuthor.BeginGroup = true;
			misc.BeginGroup = true;
						
			fileMenu.Items.AddRange( new MenuButtonItem[] {
			                           	newModule,
			                           	openModule,
			                           	saveModule,
			                           	bakeModule,
			                           	runModule,
			                           	closeModule,
			                           	newArea,
			                           	newConversation,
			                           	variableManager,
//			                           	changeUser,
			                           	exitAdventureAuthor,
			                           	logWindow,
			                           	misc,
			                           	evaluation,
			                           	magnets
			                           });	
			
			return fileMenu;
		}
		
		
		public static ToolBar SetupAdventureAuthorToolBar(ToolBar toolbar)
		{
			aaToolbar = toolbar;
			aaToolbar.AddRemoveButtonsVisible = false;
			aaToolbar.AllowMerge = true;							
			aaToolbar.Text = "Adventure Author applications";			
							
			ButtonItem conversationButton = new ButtonItem();							
			conversationButton.Activate += delegate { LaunchConversationWriter(); };
			conversationButton.ToolTipText = "Write interactive conversations for game characters";
			Tools.SetSandbarButtonImage(conversationButton,"speechbubblesblue.png","Conversations");
			aaToolbar.Items.Add(conversationButton);
							
			ButtonItem variableButton = new ButtonItem();
			variableButton.Activate += delegate { LaunchVariableManager(); };
			variableButton.ToolTipText = "Manage game variables";
			Tools.SetSandbarButtonImage(variableButton,"gear.png","Variables");
			aaToolbar.Items.Add(variableButton);
							
			ButtonItem ideasButton = new ButtonItem();
			ideasButton.Activate += delegate {
				MagnetBoardViewer mbv = new MagnetBoardViewer();
				mbv.ShowDialog();
			};
			ideasButton.ToolTipText = "Record and review your ideas";
			Tools.SetSandbarButtonImage(ideasButton,"litbulb.png","Ideas");
			aaToolbar.Items.Add(ideasButton);
							
			ButtonItem analysisButton = new ButtonItem();
			analysisButton.Activate += delegate { };
			Tools.SetSandbarButtonImage(analysisButton,"verticalbarchart.png","Analysis");
			analysisButton.Enabled = false;
			aaToolbar.Items.Add(analysisButton);
						
			ButtonItem evaluationButton = new ButtonItem();
			evaluationButton.Activate += delegate { 
			// if (user.HasAdminRights) {
					SelectModeWindow selectModeWindow = new SelectModeWindow();
					selectModeWindow.ShowDialog();
			// }
			// else {
			// 		WorksheetViewer(WorksheetViewer.Mode.PupilMode);
			// } 
			};
			Tools.SetSandbarButtonImage(evaluationButton,"clipboard.png","Evaluation");
			aaToolbar.Items.Add(evaluationButton);
														
			ButtonItem achievementsButton = new ButtonItem();
			achievementsButton.Activate += delegate { };
			Tools.SetSandbarButtonImage(achievementsButton,"crown.png","Achievements");
			achievementsButton.Enabled = false;
			aaToolbar.Items.Add(achievementsButton);
			
			return aaToolbar;
		}
	}
}
