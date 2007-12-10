/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 29/11/2007
 * Time: 12:28
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Notebook.Worksheets
{
	public enum PointType
	{
		Good,
		Bad
	}
	
	
	public struct Point
	{		
		[XmlText]
		private string message;
		public string Message {
			get { return message; }
			set { message = value; }
		}
		
		[XmlAttribute]
		private PointType type;		
		public PointType Type {
			get { return type; }
			set { type = value; }
		}
				
		
		public Point(string message, PointType type)
		{
			this.type = type;
			this.message = message;
		}
	}
	
	
//	public struct Comment
//	{		
//		[XmlAttribute]
//		private string author;		
//		public string Author {
//			get { return author; }
//			set { author = value; }
//		}
//		
//		[XmlText]
//		private string message;			
//		public string Message {
//			get { return message; }
//			set { message = value; }
//		}
//		
//		
//		public Comment(string author, string message)
//		{
//			this.author = author;
//			this.message = message;
//		}
//	}
	
	
	[XmlRoot]
	public class Feedback
	{
		private string playtester;	
		private string designer;	
		private string game;
		private List<Point> points;	
		private SerializableDictionary<string,int> ratings;
		private string response;
//		private List<Comment> comments;	
		
		[XmlElement]	
		public string Playtester {
			get { return playtester; }
			set { playtester = value; }
		}
		
		[XmlElement]	
		public string Designer {
			get { return designer; }
			set { designer = value; }
		}
		
		[XmlElement]		
		public string Game {
			get { return game; }
			set { game = value; }
		}
		
		[XmlArray]	
		public List<Point> Points {
			get { return points; }
			set { points = value; }
		}
		
		[XmlElement]		
		public SerializableDictionary<string,int> Ratings {
			get { return ratings; }
			set { ratings = value; }
		}
		
		[XmlElement]
		public string Response {
			get { return response; }
			set { value = response; }
		}
		
//		[XmlArray]		
//		public List<Comment> Comments {
//			get { return comments; }
//			set { comments = value; }
//		}
		
		
		/// <summary>
		/// For the purposes of serialization.
		/// </summary>
		public Feedback()
		{
			this.playtester = String.Empty;
			this.designer = String.Empty;
			this.game = String.Empty;
			this.points = new List<Point>(2);
			this.ratings = new SerializableDictionary<string,int>();
			this.ratings.Add("Story",0);
			this.ratings.Add("Gameplay",0);
			this.ratings.Add("World",0);
			this.ratings.Add("Overall",0);
//			this.comments = new List<Comment>(1);
			this.response = String.Empty;
		}	
	}
}
