
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using AdventureAuthor.Evaluation;

namespace AdventureAuthor.Evaluation
{
	[Serializable]
	[XmlRoot]
	public class Worksheet
	{
		[XmlAttribute]
		private string title;		
		public string Title {
			get { return title; }
			set { title = value; }
		}
		
		
		[XmlAttribute]
		private string name;		
		public string Name {
			get { return name; }
			set { name = value; }
		}
		
		
		[XmlAttribute]
		private string date;		
		public string Date {
			get { return date; }
			set { date = value; }
		}
		
		
		[XmlArray]
		private List<Section> sections;		
		public List<Section> Sections {
			get { return sections; }
		}
		
				
		/// <summary>
		/// Default constructor for the purposes of serialization.
		/// </summary>
		public Worksheet()
		{			
			this.sections = new List<Section>(1);
		}
		
		
		public Worksheet(string title, string name, string date) : this()
		{
			this.title = title;
			this.name = name;
			this.date = date;
		}
	
		
		#region Methods
		
//        private void Open(string path)
//        {
//        	try {
//				object o = Serialization.Deserialize(path,typeof(Feedback));
//				if (o is Feedback) {
//					Feedback f = (Feedback)o;	
//					Open(f);
//					worksheetPath = path;
//					Title = "Worksheet editor: " + Path.GetFileNameWithoutExtension(worksheetPath);					
//				}
//				// else if (o is...)
//				else {
//					Say.Warning("File at location " + path + " is not a valid worksheet file, and cannot be opened.");
//				}
//        	}
//        	catch (Exception e) {
//        		throw new IOException("Couldn't open a worksheet at path " + path + "\n\n" + e);
//        	}
//        }
        
        
//        private void Open(Feedback feedback)
//        {
//        	if (feedback == null) {
//        		throw new ArgumentNullException("Cannot open a null feedback.");
//        	}
//        	
//        	Playtester.Text = feedback.Playtester;
//        	Designer.Text = feedback.Designer;
//        	Game.Text = feedback.Game;
//        	
//        	StoryRating.Rating = feedback.Ratings["Story"];
//        	GameplayRating.Rating = feedback.Ratings["Gameplay"];
//        	WorldRating.Rating = feedback.Ratings["World"];
//        	OverallRating.Rating = feedback.Ratings["Overall"];
//        	
//        	PointsPanel.Children.Clear();
//        	foreach (Point p in feedback.Points) {
//				PointControl pc = new PointControl(p);
//				pc.Owner = this;
//				PointsPanel.Children.Add(pc);
//        	}
//        	
//        }
        
        
//        private void Save()
//        {
//        	Save(worksheetPath);
//        }
//        
//        
//        private void Save(string path)
//        {
//			Feedback feedback = new Feedback();
//			
//        	feedback.Playtester = Playtester.Text;
//        	feedback.Designer = Designer.Text;
//        	feedback.Game = Game.Text;	
//			
//			feedback.Ratings["Story"] = StoryRating.Rating;
//			feedback.Ratings["Gameplay"] = GameplayRating.Rating;
//			feedback.Ratings["World"] = WorldRating.Rating;
//			feedback.Ratings["Overall"] = OverallRating.Rating;
//        	
//        	foreach (PointControl pc in PointsPanel.Children) {
//        		Point p = new Point(pc.PointTextBox.Text,pc.RepresentedPoint.Type);
//        		feedback.Points.Add(p);
//        	}
//			
//			try {
//				Serialization.Serialize(path,feedback);
//			}
//			catch (Exception e) {
//        		throw new IOException("Couldn't save worksheet to path " + path + "\n\n" + e);
//			}
//		}
        
        #endregion
	}
}
