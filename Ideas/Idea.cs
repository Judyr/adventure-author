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
	[Serializable]
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
		
		
		[XmlAttribute]
		private IdeaCategory category;			
		public IdeaCategory Category {
			get { return category; }
			set { category = value; }
		}
		
		
		/// <summary>
		/// All the values of the enum IdeaCategory in array form.
		/// </summary>
		public static readonly IdeaCategory[] IDEA_CATEGORIES = (IdeaCategory[])Enum.GetValues(typeof(IdeaCategory));
		
		#endregion
		
		#region Constructors
		
		/// <summary>
		/// For serialization.
		/// </summary>
		public Idea()
		{
			
		}
		
		public Idea(string text) : this(text,IdeaCategory.Unassigned) 
		{
		}
		
		
		public Idea(string text, IdeaCategory category) : this(text,category,String.Empty) 
		{
		}
		
		
		public Idea(string text, IdeaCategory category, string author) : this(text,category,author,DateTime.Now)
		{			
		}
		
		
		public Idea(string text, IdeaCategory category, string author, DateTime created)
		{
			this.text = text;
			this.category = category;
			this.author = author;
			this.created = created;
		}
				
		#endregion
		
		#region Methods
		
		public int CompareTo(object obj)
		{
			Idea idea = obj as Idea;
			if (idea == null || author != idea.author || text != idea.text || 
			    created != idea.created || category != idea.category) 
			{
				return 0;
			}
			else 
			{
				return 1;
			}
		}
		
		#endregion
	}
}
