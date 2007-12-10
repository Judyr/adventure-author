using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Xml.Serialization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AdventureAuthor.Notebook.Worksheets;
using AdventureAuthor.Utils;
using AdventureAuthor.Setup.UI;
using Microsoft.Win32;
using form = NWN2Toolset.NWN2ToolsetMainForm;

namespace AdventureAuthor.Notebook.Worksheets.UI
{
    /// <summary>
    /// Interaction logic for FeedbackWorksheet.xaml
    /// </summary>

    public partial class FeedbackWorksheet : Window
    {
    	// TODO dirty check to see a) if a worksheet is open and b) if it needs saving
    	// TODO bind to feedback object to make this happen
    	// TODO tooltips should display a bit better
    	// TODO finish all the fields of this worksheet
    	
    	
    	private string worksheetPath;
		public string WorksheetPath {
			get { return worksheetPath; }
		}
    	
    	
    	private const string XMLFILTER = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
    									 
    	
        public FeedbackWorksheet()
        {
        	worksheetPath = String.Empty;
            InitializeComponent();
            
            Point point = new Point("Main character was cool",PointType.Good);
            Point point2 = new Point("Villain was really scary, had a cool backstory as well",PointType.Good);
            Point point3 = new Point("Gameplay was a bit samey, lots of killing",PointType.Bad);
            Point point4 = new Point("Couldn't get to the second area, transition didn't work, had to do in toolset",PointType.Bad);
            Point point5 = new Point("Cliched setting",PointType.Bad);
            	
            PointsPanel.Children.Add(new PointControl(point));
            PointsPanel.Children.Add(new PointControl(point2));
            PointsPanel.Children.Add(new PointControl(point3));
            PointsPanel.Children.Add(new PointControl(point4));
            PointsPanel.Children.Add(new PointControl(point5));
            
            
            
            
            
//            StoryRating.Tip = new FriendlyToolTip("Rate the story",
//                                                      "A game has a good story if you care about the characters, " +
//                                                      "and are interested in the plot and what happens next.");
//            GameplayRating.Tip = new FriendlyToolTip("Rate the gameplay",
//                                                         "A game has good gameplay if it is enjoyable to play. This could mean " +
//                                                         "it had exciting combat, fun exploration and treasure hunts, " + 
//                                                         "brain-bending riddles and clever puzzles.",
//                                                         "Remember that game balance is important. The game should be challenging, " + 
//                                                         "not easy enough that it's boring, or difficult enough that it's " +
//                                                         "frustrating.");
//            WorldRating.Tip = new FriendlyToolTip("Rate the world",
//                                                      "A game has a good game world if the world is well designed. This could mean " +
//                                                      "it has carefully crafted landscapes and cities, and creates atmosphere " + 
//                                                      "through the use of music, sound effects and lighting.");
//            OverallRating.Tip = new FriendlyToolTip("Rate the game overall",
//                                                        "A good game should have an interesting story, a carefully designed world, " +
//                                                        "and most importantly, some fun gameplay.");
        }
        
        
        public FeedbackWorksheet(string path) : this()
        {
        	try {        		
        		Open(path);
        	}
        	catch (Exception e) {
        		Say.Error("Failed to open existing feedback worksheet.",e);
        		New();
        	}
        }

        
        #region Event handlers
        
        private void OnClick_New_FeedbackWorksheet(object sender, EventArgs e)
        {
        	New();
        }
        
        
        private void OnClick_Open(object sender, EventArgs e)
        {
        	OpenFileDialog open = new OpenFileDialog();
        	open.DefaultExt = XMLFILTER;
        	open.Filter = XMLFILTER;
        	open.InitialDirectory = form.ModulesDirectory;
        	open.Multiselect = false;
        	open.RestoreDirectory = false;
        	open.Title = "Open a worksheet";
        	
        	open.FileOk += delegate 
        	{  
        		try {
        			Open(open.FileName);
        		}
        		catch (IOException ioe) {
        			Say.Error(ioe);
        		}
        	};
        	
        	open.ShowDialog(this);
        }
        
        
        private void OnClick_Save(object sender, EventArgs e)
        {
        	try {
        		Save();
        	}
        	catch (IOException ioe) {
        		Say.Error(ioe);
        	}
        }
        
        
        private void OnClick_SaveAs(object sender, EventArgs e)
        {
        	SaveFileDialog save = new SaveFileDialog();
        	save.CreatePrompt = true;
        	save.DefaultExt = XMLFILTER;
        	save.Filter = XMLFILTER;
        	save.InitialDirectory = form.ModulesDirectory;
        	save.RestoreDirectory = false;
        	save.Title = "Save worksheet as";
        	save.ValidateNames = true;        	
        	
        	save.FileOk += delegate
        	{  
        		try {
        			Save(save.FileName);
        		}
        		catch (IOException ioe) {
        			Say.Error(ioe);
        		}
        	};
        	
        	save.ShowDialog(this);
        }
        
        
        private void OnClick_Close(object sender, EventArgs e)
        {
        	MessageBoxResult result = SaveFirst();
        	if (result != MessageBoxResult.Cancel) {
        		if (result == MessageBoxResult.Yes) {
        			Save();
        		}
        		New();
        	}
        }
        
        
        private void OnClick_Exit(object sender, EventArgs e)
        {
        	MessageBoxResult result = SaveFirst();
        	if (result != MessageBoxResult.Cancel) {
        		if (result == MessageBoxResult.Yes) {
        			Save();
        		}
        		Close();
        	}        	
        }
        
        
        #endregion
                
        private void New()
        {
        	this.worksheetPath = String.Empty;
        	Title = "Worksheet editor";
        	Feedback feedback = new Feedback();
        	Open(feedback);
        }
        
        
        private void Open(string path)
        {
        	try {
				object o = Serialization.Deserialize(path,typeof(Feedback));
				if (o is Feedback) {
					Feedback f = (Feedback)o;	
					Open(f);
					worksheetPath = path;
					Title = "Worksheet editor: " + Path.GetFileNameWithoutExtension(worksheetPath);					
				}
				// else if (o is...)
				else {
					Say.Warning("File at location " + path + " is not a valid worksheet file, and cannot be opened.");
				}
        	}
        	catch (Exception e) {
        		throw new IOException("Couldn't open a worksheet at path " + path + "\n\n" + e);
        	}
        }
        
        
        private void Open(Feedback feedback)
        {
        	if (feedback == null) {
        		throw new ArgumentNullException("Cannot open a null feedback.");
        	}
        	
        	Playtester.Text = feedback.Playtester;
        	Designer.Text = feedback.Designer;
        	Game.Text = feedback.Game;
        	
//        	StoryRating.Rating = feedback.Ratings["Story"];
//        	GameplayRating.Rating = feedback.Ratings["Gameplay"];
//        	WorldRating.Rating = feedback.Ratings["World"];
//        	OverallRating.Rating = feedback.Ratings["Overall"];
        	
        	
        }
        
        
        private void Save()
        {
        	Save(worksheetPath);
        }
        
        
        private void Save(string path)
        {
			Feedback feedback = new Feedback();
			
        	feedback.Playtester = Playtester.Text;
        	feedback.Designer = Designer.Text;
        	feedback.Game = Game.Text;	
			
//			feedback.Ratings["Story"] = StoryRating.Rating;
//			feedback.Ratings["Gameplay"] = GameplayRating.Rating;
//			feedback.Ratings["World"] = WorldRating.Rating;
//			feedback.Ratings["Overall"] = OverallRating.Rating;
			
			try {
				Serialization.Serialize(path,feedback);
			}
			catch (Exception e) {
        		throw new IOException("Couldn't save worksheet to path " + path + "\n\n" + e);
			}
		}
        
        
        private MessageBoxResult SaveFirst()
        {
        	return MessageBox.Show("Save changes before closing?","Save?",MessageBoxButton.YesNoCancel);
        }
    }
}