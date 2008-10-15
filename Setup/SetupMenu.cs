using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using System.Text;

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
		private static MenuButtonItem openRecentModule = null;
		private static List<string> recentModulesList = null;
		
		
		/// <summary>
		/// The number of filenames to remember and display in the 'Open recent modules' menu.
		/// </summary>
		public const int NUMBER_OF_RECENT_MODULES = 4;	
		
		
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
			MenuButtonItem saveModuleAs = new MenuButtonItem("Save module As");
			saveModuleAs.Activate += delegate { SaveModuleAsDialog(); };
			MenuButtonItem bakeModule = new MenuButtonItem("Bake module");
			bakeModule.Activate += delegate { BakeModuleDialog(); };
//			MenuButtonItem runModule = new MenuButtonItem("Run module");
//			runModule.Activate += delegate { RunModuleDialog(); };
			MenuButtonItem closeModule = new MenuButtonItem("Close module");
			closeModule.Activate += delegate { CloseModuleDialog(); };
			
			openRecentModule = new MenuButtonItem("Recently opened modules");
			openRecentModule.BeginGroup = true;
			SetUpRecentlyOpenedModulesMenu();
							
			MenuButtonItem newArea = new MenuButtonItem("Create new area");
			newArea.BeginGroup = true;
			newArea.Activate += delegate { NewAreaDialog(); };
			
			/*
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
			
			MenuButtonItem extractWordCount = new MenuButtonItem("Extract word count");
			extractWordCount.Activate += delegate { 
				NWN2Utils.WriteTotalWordCountForAllModulesToFile();
				Say.Information("Finished.");
			};
			*/	
			
			MenuButtonItem exitAdventureAuthor = new MenuButtonItem("Exit");
			exitAdventureAuthor.BeginGroup = true;
			exitAdventureAuthor.Activate += delegate { form.App.Close(); };
			
			/*
			MenuButtonItem extractText = new MenuButtonItem("Extract text");
			extractText.Activate += delegate {		
				FileInfo file = new FileInfo(@"C:\To burn\narrativevehicles.txt");
				using (StreamWriter writer = file.CreateText())
				{
					writer.AutoFlush = true;
					DirectoryInfo allKids = new DirectoryInfo(@"C:\To burn");								
					foreach (DirectoryInfo kid in allKids.GetDirectories()) {
						string path = Path.Combine(kid.FullName,"modules");	
						DirectoryInfo modulesDirectory = new DirectoryInfo(path);
						DirectoryInfo[] modules = modulesDirectory.GetDirectories();
						foreach (DirectoryInfo module in modules) {
							if (module.Name.StartsWith("temp")) {
								continue;
							}
							try {
								form.App.Module.OpenModuleDirectory(module.FullName);
								string narrativeVehicleText = NWN2Utils.GetNarrativeVehicleText(form.App.Module,false,true);
								if (narrativeVehicleText != String.Empty) {
									writer.WriteLine(narrativeVehicleText);
								}								
							}
							catch (Exception e) {
								writer.WriteLine("Error: " + e.ToString());
							}
						}
					}
					Say.Information("Finished.");
				}
			};
			*/
			
//			MenuButtonItem extractAllConversations = new MenuButtonItem("Extract conversations");
//			extractAllConversations.Activate += delegate { 
//				Conversations.Conversation.ExportAllConversationsInDirectoryToTextFile(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop),"To burn"),
//				                                                                       Conversations.ExportFormat.TreeFormat,
//				                                                                       SearchOption.AllDirectories);
//				Say.Information("done");
//			};
									
			fileMenu.Items.AddRange( new MenuButtonItem[] {
			                           	newModule,
			                           	openModule,
			                           	saveModule,
			                           	saveModuleAs,
			                           	bakeModule,
			                           	closeModule,
			                           	openRecentModule,
			                           	newArea,
			                           	exitAdventureAuthor,
			                           });	
			
			return fileMenu;
		}
				
				
		private static void SetUpRecentlyOpenedModulesMenu()
		{	
			// Check whether a list of recent modules has already been saved - if not, create a new one:
			string recentModulesListPath = AdventureAuthorPluginPreferences.RecentlyOpenedModulesPath;
			if (File.Exists(recentModulesListPath)) {
				try {
					object obj = Serialization.Deserialize(recentModulesListPath,typeof(List<string>));
					recentModulesList = (List<string>)obj;					
				}
				catch (Exception e) {
					System.Diagnostics.Debug.WriteLine("The file at location " + recentModulesListPath +
					          " is not a valid 'recent modules list' file.");
					recentModulesList = new List<string>();
				}
			}
			else {
				recentModulesList = new List<string>();
			}
			
			
			// Serialise the list of recently opened modules on closing:
			form.App.Closing += delegate 
			{  
				if (recentModulesList != null) {
					Serialization.Serialize(AdventureAuthorPluginPreferences.RecentlyOpenedModulesPath,recentModulesList);
				}
			};
			
			
			// Whenever a module is successfully opened, add that module's name to the top of the list:
			ModuleHelper.ModuleOpened += delegate 
			{
				if (recentModulesList.Contains(form.App.Module.Name)) {
					if (recentModulesList.IndexOf(form.App.Module.Name) == 0) {
						return; // module was already at the top of the list, so don't do anything
					}
					recentModulesList.Remove(form.App.Module.Name);
				}				
				recentModulesList.Insert(0,form.App.Module.Name);
				
				UpdateRecentModulesMenu();
			};
			
			
			// Finally, set up the 'Recently opened modules' menu for the first time:
			UpdateRecentModulesMenu();
		}
		
				
		public static void UpdateRecentModulesMenu()
		{
			if (openRecentModule != null && recentModulesList != null) {
				
				// Only store and display a certain number of recent modules (currently 4):
				if (recentModulesList.Count > NUMBER_OF_RECENT_MODULES) {
					recentModulesList.RemoveRange(NUMBER_OF_RECENT_MODULES,recentModulesList.Count-NUMBER_OF_RECENT_MODULES);
//					for (int i = NUMBER_OF_RECENT_MODULES; i < recentModulesList.Count; i++) {
//						recentModulesList.RemoveAt(i);
//					}
				}
				
				openRecentModule.Items.Clear();
				foreach (string filename in recentModulesList) {
					MenuButtonItem moduleMenuItem = new MenuButtonItem(filename);
					moduleMenuItem.Activate += new EventHandler(AttemptToOpenRecentModule);
					openRecentModule.Items.Add(moduleMenuItem);					
				}
				
				// Only enable the menu if there are recent modules to choose from:
				openRecentModule.Enabled = openRecentModule.Items.Count > 0;
			}
			else {
				System.Diagnostics.Debug.WriteLine("Tried to update the 'Open recent module' " + 
				                                   "menu when either the menu item or the " +
				                                   "list had not been constructed/deserialised yet.");
			}
		}

		
		private static void AttemptToOpenRecentModule(object sender, EventArgs e)
		{
			MenuButtonItem moduleMenuItem = (MenuButtonItem)sender;
			string filename = moduleMenuItem.Text;
			string modulePath = Path.Combine(ModuleHelper.ModulesDirectory,filename);
			
			// Check that the module exists before you try to open it:
			if (!Directory.Exists(modulePath)) {				
				Say.Warning("The module '" + filename + "' could not be found.");
				if (recentModulesList.Contains(filename)) {
					recentModulesList.Remove(filename);
					UpdateRecentModulesMenu();
				}
			}
			else {
				try {
					ModuleHelper.Open(filename,AdventureAuthorPluginPreferences.Instance.OpenScratchpadByDefault); 
				}
				catch (Exception ex) {			
					Say.Error("The module '" + filename + "' could not be opened.",ex);
					if (recentModulesList.Contains(filename)) {
						recentModulesList.Remove(filename);
						UpdateRecentModulesMenu();
					}
					ModuleHelper.CloseModule();
					UpdateTitleBar();
				}
			}			
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
							
//			ButtonItem ideasButton = new ButtonItem();
//			ideasButton.Activate += delegate { 
				// Process.Run(etc.) - not Instance.Show() anymore
//			};
//			ideasButton.ToolTipText = "Record and review your ideas";
//			SetSandbarButtonImage(ideasButton,"litbulb.png","Ideas");
//			aaToolbar.Items.Add(ideasButton);
							
//			ButtonItem addIdeaButton = new ButtonItem();
//			addIdeaButton.Activate += delegate { 
//				EditMagnetWindow window = new EditMagnetWindow();
//				window.MagnetCreated += delegate(object sender, MagnetEventArgs e) { 
//					OnMagnetSubmitted(new MagnetEventArgs(e.Magnet));
//					Log.WriteAction(LogAction.added,"idea",e.Magnet.ToString() + " ... added from main toolset");
//				};
//				window.ShowDialog();
//			};
//			addIdeaButton.ToolTipText = "Quickly add a new idea to your Magnet Box";
//			addIdeaButton.Text = "+";
//			aaToolbar.Items.Add(addIdeaButton);
							
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
						
//			ButtonItem evaluationButton = new ButtonItem();
//			evaluationButton.Activate += delegate { 
//				if (CardViewer.Instance == null || !CardViewer.Instance.IsLoaded) {					
//					SelectModeWindow selectModeWindow = new SelectModeWindow();
//					bool cancelled = !((bool)selectModeWindow.ShowDialog());
//					if (cancelled) {
//						return;	
//					}
//				}
//				BringToFront(CardViewer.Instance);
//			};
//			SetSandbarButtonImage(evaluationButton,"clipboard.png","Comment Cards");
//			evaluationButton.ToolTipText = "Answer questions to evaluate a game";
//			aaToolbar.Items.Add(evaluationButton);
														
			ButtonItem achievementsButton = new ButtonItem();
			achievementsButton.Activate += delegate { };
			SetSandbarButtonImage(achievementsButton,"crown.png","Achievements");
			achievementsButton.Enabled = false;
			achievementsButton.Visible = false;
			achievementsButton.ToolTipText = "View your achievements";
			aaToolbar.Items.Add(achievementsButton);
														
//			ButtonItem checkTransitions = new ButtonItem();
//			SetSandbarButtonImage(checkTransitions,"clipboard.png","Check transitions");
//			checkTransitions.Enabled = true;
//			checkTransitions.Visible = true;
//			checkTransitions.ToolTipText = "Check transitions";
//			checkTransitions.Activate += delegate 
//			{ 
//				// Temp test:
//				if (ModuleHelper.ModuleIsOpen()) {
//					bool checkBrokenAreaTransitions;
//					bool checkAllReachable;
//					checkBrokenAreaTransitions = System.Windows.Forms.MessageBox.Show("Check for broken area transitions?",
//					                                                                  String.Empty,
//					                                                                  System.Windows.Forms.MessageBoxButtons.YesNo)
//						== System.Windows.Forms.DialogResult.Yes;
//					checkAllReachable = System.Windows.Forms.MessageBox.Show("Check that all areas are reachable?",
//					                                                                  String.Empty,
//					                                                                  System.Windows.Forms.MessageBoxButtons.YesNo)
//						== System.Windows.Forms.DialogResult.Yes;
//					
//					List<AdventureAuthor.Tasks.Task> tasks = (new AdventureAuthor.Tasks.NWN2.AreaTasksGenerator(checkBrokenAreaTransitions,
//					                                                                                           checkAllReachable)).GetTasks();
//					tasks.AddRange((new AdventureAuthor.Tasks.NWN2.CreatureTasksGenerator()).GetTasks());
//					StringBuilder message = new StringBuilder("..To do..\n\n");
//					foreach (AdventureAuthor.Tasks.Task task in tasks) {
//						message.Append(task.Description + "\n\n");
//					}			
//					Say.Information(message.ToString());					
//				}
//				else {
//					Say.Information("No module is open.");
//				}
//			};
//			aaToolbar.Items.Add(checkTransitions);
			
			
			
			
			ModuleHelper.ModuleOpened += delegate {  
				conversationButton.Enabled = true;
				variableButton.Enabled = true;
				//ideasButton.Enabled = true;
				//analysisButton.Enabled = true;
//				evaluationButton.Enabled = true;
				//achievementsButton.Enabled = true;
			};
			
			ModuleHelper.ModuleClosed += delegate {  
				conversationButton.Enabled = false;
				variableButton.Enabled = false;
				//ideasButton.Enabled = true;
				//analysisButton.Enabled = false;
//				evaluationButton.Enabled = true;
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