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
		Praise,
		Criticism
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
	
	
	public struct Comment
	{		
		[XmlAttribute]
		private string author;		
		public string Author {
			get { return author; }
			set { author = value; }
		}
		
		[XmlText]
		private string message;			
		public string Message {
			get { return message; }
			set { message = value; }
		}
		
		
		public Comment(string author, string message)
		{
			this.author = author;
			this.message = message;
		}
	}
	
	
	[XmlRoot]
	public class Feedback
	{
		private string playtester;	
		private string designer;	
		private string game;
		private List<Point> points;	
		private SerializableDictionary<string,int?> ratings;
		private List<Comment> comments;	
		
		[XmlAttribute]	
		public string Playtester {
			get { return playtester; }
			set { playtester = value; }
		}
		
		[XmlAttribute]	
		public string Designer {
			get { return designer; }
			set { designer = value; }
		}
		
		[XmlAttribute]		
		public string Game {
			get { return game; }
			set { game = value; }
		}
		
		[XmlArray]	
		public List<Point> Points {
			get { return points; }
			set { points = value; }
		}
		
		[XmlArray]		
		public SerializableDictionary<string,int?> Ratings {
			get { return ratings; }
			set { ratings = value; }
		}
		
		[XmlArray]		
		public List<Comment> Comments {
			get { return comments; }
			set { comments = value; }
		}
		
		
		/// <summary>
		/// For the purposes of serialization.
		/// </summary>
		private Feedback()
		{
			
		}
		
		
		public Feedback(string playtester, string designer, string game)
		{
			this.playtester = playtester;
			this.designer = designer;
			this.game = game;
			this.points = new List<Point>(2);
			this.ratings = new SerializableDictionary<string,int?>();
			this.ratings.Add("Story",null);
			this.ratings.Add("Gameplay",null);
			this.ratings.Add("Design",null);
			this.ratings.Add("Overall",null);
			this.comments = new List<Comment>(1);
		}	
	}
}
