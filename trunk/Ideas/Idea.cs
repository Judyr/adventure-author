/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 18/12/2007
 * Time: 02:07
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace AdventureAuthor.Ideas
{
	[SerializableAttribute]
	public class Idea : IComparable
	{
		#region Fields
		
		[XmlElement]
		private string text;		
		public string Text {
			get { return text; }
			set { text = value; }
		}
		
		
		[XmlAttribute]
		private string author; // TODO take user ID instead
		public string Author {
			get { return author; }
			set { author = value; }
		}		
		
		
		private DateTime created;		
		public DateTime Created {
			get { return created; }
		}
		
		
		[XmlArray]
		private List<string> categories;		
		public List<string> Categories {
			get { return categories; }
			set { categories = value; }
		}
		
		#endregion
		
		#region Constructors
		
		/// <summary>
		/// For serialization.
		/// </summary>
		private Idea()
		{
			
		}
		
		
		public Idea(string text, string author) : this (text,author,DateTime.Now)
		{
		}
		
		
		public Idea(string text, string author, DateTime created)
		{
			this.text = text;
			this.author = author;
			this.created = created;
		}
		
		#endregion
		
		#region Methods
		
		public int CompareTo(object obj)
		{
			Idea idea = obj as Idea;
			if (idea == null || 
			    author != idea.author || 
			    text != idea.text ||
			    created != idea.created ||
			    categories != idea.categories) 
			{
				return 0;
			}
			else {
				return 1;
			}
		}
		
		#endregion
	}
}
