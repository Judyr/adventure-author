/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 18/12/2007
 * Time: 02:07
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace AdventureAuthor.Ideas
{
	[Serializable]
	[XmlRoot]
	public class Idea : IComparable, ICloneable
	{
		#region Constants
		
		public const int MAX_IDEA_LENGTH = 200;
		
		#endregion
		
		#region Fields
		
		[XmlElement]
		private string text;		
		public string Text {
			get { return text; }
			set { text = value; }
		}
		
		
		[XmlAttribute]
		private string creator;
		public string Creator {
			get { return creator; }
			set { creator = value; }
		}		
				
		
		[XmlAttribute]
		private DateTime created;		
		public DateTime Created {
			get { return created; }
			set { created = value; }
		}
		
		
		[XmlAttribute]
		private IdeaCategory category;			
		public IdeaCategory Category {
			get { return category; }
			set { category = value; }
		}
		
		
		[XmlAttribute]
		private bool isStarred;
		public bool IsStarred	{
			get { return isStarred; }
			set { isStarred = value; }
		}
		
		
		/// <summary>
		/// All the values of the enum IdeaCategory in array form.
		/// </summary>
		public static readonly IdeaCategory[] IDEA_CATEGORIES = (IdeaCategory[])Enum.GetValues(typeof(IdeaCategory));
		
		
		/// <summary>
		/// Sort according to the text of the idea, in ascending alphabetical order.
		/// </summary>
		public static IComparer SortAlphabeticallyAscending {
			get {
				return new sortAlphabeticallyAscendingComparer();
			}
		}
		
		
		/// <summary>
		/// Sort according to the text of the idea, in descending alphabetical order.
		/// </summary>
		public static IComparer SortAlphabeticallyDescending {
			get {
				return new sortAlphabeticallyDescendingComparer();
			}
		}
		
		#endregion
		
		#region Constructors
		
		/// <summary>
		/// For serialization.
		/// </summary>
		public Idea()
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
			this.creator = author;
			this.created = created;
		}
				
		#endregion
	
		#region Methods
		
		public override string ToString()
		{
			return text + " (" + category + ")";
		}
		
		        
		public object Clone()
		{
			return MemberwiseClone();
		}
		
		
		public override bool Equals(object obj)
		{
			Idea idea = obj as Idea;
			if (idea == null) {
				return false;
			}
			else {
				return text == idea.text && 
					creator == idea.creator && 
					category == idea.category && 
					created == idea.created;// &&
					//isStarred == idea.isStarred;
			}
		}
		
		
		public override int GetHashCode()
		{
			return text.GetHashCode();
		}
		
		
		public int CompareTo(object obj)
		{
			Idea idea = (Idea)obj;
			int answer = String.Compare(text,((Idea)obj).text);
			return answer;
		}
		
		#endregion
		
		#region IComparer classes
		
		/// <summary>
		/// Sort according to the text of the idea, in ascending alphabetical order.
		/// </summary>
		private class sortAlphabeticallyAscendingComparer : IComparer
		{			
			public int Compare(object x, object y)
			{
				Idea idea1 = (Idea)x;
				Idea idea2 = (Idea)y;				
				return String.Compare(idea1.text,idea2.text);
			}
		}
		
		
		/// <summary>
		/// Sort according to the text of the idea, in descending alphabetical order.
		/// </summary>
		private class sortAlphabeticallyDescendingComparer : IComparer
		{			
			public int Compare(object x, object y)
			{
				Idea idea1 = (Idea)x;
				Idea idea2 = (Idea)y;				
				return String.Compare(idea2.text,idea1.text);
			}
		}
		
		#endregion
	}
}
