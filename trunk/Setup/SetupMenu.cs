using System;
using System.IO;
using System.Windows.Forms.Integration;
using AdventureAuthor.Analysis;
using AdventureAuthor.Core;
using AdventureAuthor.Conversations.UI;
using AdventureAuthor.Evaluation;
using AdventureAuthor.Ideas;
using AdventureAuthor.Utils;
using AdventureAuthor.Variables.UI;
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
		
		
		private static MenuBarItem SaveModuleAsDialog(MenuBarItem fileMenu)
		{
			mainFileMenu = fileMenu;
			fileMenu.Items.Clear();							
			MenuButtonItem newModule = new MenuButtonItem("New module");
			newModule.Activate += delegate { NewModuleDialog(); };
			MenuButtonItem openModule = new MenuButtonItem("Open module");
			openModule.Activate += delegate { OpenModuleDialog(); };
			MenuButtonItem saveModule = new MenuButtonItem("Save module");
			saveModule.Activate += delegate { SaveModuleDialog(); };
			MenuButtonItem saveModuleAs = new MenuButtonItem("Save module As");
			saveModuleAs.Activate += delegate { SaveModuleAsDialog(); };
			MenuButtonItem bakeModule = new MenuButtonItem("Bake module");
			bakeModule.Activate += delegate { BakeModuleDialog(); };
//			MenuButtonItem runModule = new MenuButtonItem("Run module");
//			runModule.Activate += delegate { RunModuleDialog(); };
			MenuButtonItem closeModule = new MenuButtonItem("Close module");
			closeModule.Activate += delegate { CloseModuleDialog(); };
							
			MenuButtonItem newArea = new MenuButtonItem("Create new area");
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
			
			MenuButtonItem selectTagWindow = new MenuButtonItem("Select tag window");
			selectTagWindow.Activate += delegate { 
				AdventureAuthor.Scripts.UI.SelectTagQuestionPanel panel =
					new AdventureAuthor.Scripts.UI.SelectTagQuestionPanel();
				System.Windows.Window window = new System.Windows.Window();
				window.Content = panel;
				window.Show(); 
			
			};
			programmerFunctions.Items.Add(selectTagWindow);	
						
			MenuButtonItem exitAdventureAuthor = new MenuButtonItem("Exit");
			exitAdventureAuthor.BeginGroup = true;
			exitAdventureAuthor.Activate += delegate { form.App.Close(); };
			
			MenuButtonItem testingMessageQueue = new MenuButtonItem("Testing messages");
			testingMessageQueue.Activate += delegate { 
				System.Messaging.MessageQueue queue = null;
				
				string queueName = System.IO.Path.Combine(System.Environment.MachineName,"otheronefollows");
			
				if (System.Messaging.MessageQueue.Exists(queueName))
				queue = new System.Messaging.MessageQueue(queueName);
				else
				queue = System.Messaging.MessageQueue.Create(queueName, false);
				
				
				
				System.Messaging.Message[] messages = queue.GetAllMessages();
				
				foreach (System.Messaging.Message message in messages)
				{
					Say.Information(message.Label + "\n\n" + message.Body.ToString());
				}
			};
									
			fileMenu.Items.AddRange( new MenuButtonItem[] {
			                           	newModule,
			                           	openModule,
			                           	saveModule,
			                           	saveModuleAs,
			                           	bakeModule,
//			                           	runModule,
			                           	closeModule,
			                           	newArea,
//			                           	programmerFunctions,
			                           	exitAdventureAuthor,
			                           	testingMessageQueue
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
				BringToFront(WriterWindow.Instance);
			};
			conversationButton.ToolTipText = "Write interactive conversations for game characters";
			conversationButton.Enabled = false;
			SetSandbarButtonImage(conversationButton,"speechbubblesblue.png","Conversations");
			
			aaToolbar.Items.Add(conversationButton);
							
			ButtonItem variableButton = new ButtonItem();
			variableButton.Activate += delegate { 
				LaunchVariableManager();
				BringToFront(VariablesWindow.Instance);
			};
			variableButton.ToolTipText = "Manage game variables";
			variableButton.Enabled = false;
			SetSandbarButtonImage(variableButton,"gear.png","Variables");
			aaToolbar.Items.Add(variableButton);
							
			ButtonItem ideasButton = new ButtonItem();
			ideasButton.Activate += delegate { 
				MagnetBoardViewer.Instance.Show();//LaunchMagnetBoardViewer();
				BringToFront(MagnetBoardViewer.Instance);
			};
			ideasButton.ToolTipText = "Record and review your ideas";
			SetSandbarButtonImage(ideasButton,"litbulb.png","Ideas");
			aaToolbar.Items.Add(ideasButton);
							
			ButtonItem addIdeaButton = new ButtonItem();
			addIdeaButton.Activate += delegate { 
				EditMagnetWindow window = new EditMagnetWindow();
				window.MagnetCreated += delegate(object sender, MagnetEventArgs e) { 
					OnMagnetSubmitted(new MagnetEventArgs(e.Magnet));
					Log.WriteAction(LogAction.added,"idea",e.Magnet.ToString() + " ... added from main toolset");
				};
				window.ShowDialog();
			};
			addIdeaButton.ToolTipText = "Quickly add a new idea to your Magnet Box";
			addIdeaButton.Text = "+";
			aaToolbar.Items.Add(addIdeaButton);
							
			ButtonItem analysisButton = new ButtonItem();
			analysisButton.Activate += delegate { 
				if (form.App.Module.Areas.Count > 0) {
					CombatMap analysisWindow = LaunchAnalysis();
					if (analysisWindow != null) {
						BringToFront(analysisWindow);
					}
				}
				else {
					Say.Information("There are no areas to display in map view.");
				}
			};
			SetSandbarButtonImage(analysisButton,"verticalbarchart.png","Analysis");
			analysisButton.ToolTipText = "Read a useful analysis of your game";
			analysisButton.Enabled = false;
			analysisButton.Visible = false;
			aaToolbar.Items.Add(analysisButton);
						
			ButtonItem evaluationButton = new ButtonItem();
			evaluationButton.Activate += delegate { 
				if (WorksheetViewer.Instance == null || !WorksheetViewer.Instance.IsLoaded) {					
					SelectModeWindow selectModeWindow = new SelectModeWindow();
					bool cancelled = !((bool)selectModeWindow.ShowDialog());
					if (cancelled) {
						return;	
					}
				}
				BringToFront(WorksheetViewer.Instance);
			};
			SetSandbarButtonImage(evaluationButton,"clipboard.png","Evaluation");
			evaluationButton.ToolTipText = "Answer questions to evaluate a game";
			aaToolbar.Items.Add(evaluationButton);
														
			ButtonItem achievementsButton = new ButtonItem();
			achievementsButton.Activate += delegate { };
			SetSandbarButtonImage(achievementsButton,"crown.png","Achievements");
			achievementsButton.Enabled = false;
			achievementsButton.Visible = false;
			achievementsButton.ToolTipText = "View your achievements";
			aaToolbar.Items.Add(achievementsButton);
			
			ModuleHelper.ModuleOpened += delegate {  
				conversationButton.Enabled = true;
				variableButton.Enabled = true;
				ideasButton.Enabled = true;
				//analysisButton.Enabled = true;
				evaluationButton.Enabled = true;
				//achievementsButton.Enabled = true;
			};
			
			ModuleHelper.ModuleClosed += delegate {  
				conversationButton.Enabled = false;
				variableButton.Enabled = false;
				ideasButton.Enabled = true;
				//analysisButton.Enabled = false;
				evaluationButton.Enabled = true;
				//achievementsButton.Enabled = true;
			};
        	
			aaToolbar.AddRemoveButtonsVisible = false;
			return aaToolbar;
		}
		
		
		private static void SetSandbarButtonImage(ButtonItem button, string filename, string buttonText)
		{		
			string path = Path.Combine(ModuleHelper.ImagesDir,filename);
			button.Image = ResourceHelper.GetBitmap(path);
	        button.Text = buttonText;
	        button.BeginGroup = true;
		}
	}
}