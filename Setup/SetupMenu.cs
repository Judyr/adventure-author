
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
		private static void SetupFileMenu(MenuBarItem fileMenu)
		{
			fileMenu.Items.Clear();
							
			MenuButtonItem newAdventure = new MenuButtonItem("New module");
			newAdventure.Activate += delegate { NewModuleDialog(); };
			MenuButtonItem openAdventure = new MenuButtonItem("Open module");
			openAdventure.Activate += delegate { OpenModuleDialog(); };
			MenuButtonItem saveAdventure = new MenuButtonItem("Save module");
			saveAdventure.Activate += delegate { SaveModuleDialog(); };
//			MenuButtonItem saveAdventureAs = new MenuButtonItem("Save As");
//			saveAdventureAs.Activate += delegate { SaveAdventureAsDialog(); };
			MenuButtonItem bakeAdventure = new MenuButtonItem("Bake module");
			bakeAdventure.Activate += delegate { BakeModuleDialog(); };
			MenuButtonItem runAdventure = new MenuButtonItem("Run module");
			runAdventure.Activate += delegate { RunModuleDialog(); };
			MenuButtonItem closeAdventure = new MenuButtonItem("Close module");
			closeAdventure.Activate += delegate { CloseModuleDialog(); };
							
			MenuButtonItem newChapter = new MenuButtonItem("New area");
			newChapter.Activate += delegate { NewAreaDialog(); };
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
			
			newChapter.BeginGroup = true;
			exitAdventureAuthor.BeginGroup = true;
			misc.BeginGroup = true;
						
			fileMenu.Items.AddRange( new MenuButtonItem[] {
			                           	newAdventure,
			                           	openAdventure,
			                           	saveAdventure,
//			                           	saveAdventureAs,
			                           	bakeAdventure,
			                           	runAdventure,
			                           	closeAdventure,
			                           	newChapter,
			                           	newConversation,
			                           	variableManager,
//			                           	changeUser,
			                           	exitAdventureAuthor,
			                           	logWindow,
			                           	misc,
			                           	evaluation,
			                           	magnets
			                           });			
		}	
	}
}
