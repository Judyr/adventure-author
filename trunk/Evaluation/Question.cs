/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 14/01/2008
 * Time: 11:46
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using AdventureAuthor.Evaluation;
using AdventureAuthor.Evaluation.Viewer;

namespace AdventureAuthor.Evaluation
{
	[Serializable]
	[XmlRoot]
	[XmlInclude(typeof(Rating)), XmlInclude(typeof(Evidence)), XmlInclude(typeof(Comment))]
	public class Question : IExcludable
	{			
		[XmlAttribute]
		private bool include = true;
		public bool Include {
			get { return include; }
			set { include = value; }
		}
		
		
		/// <summary>
		/// The text of this question.
		/// </summary>
		[XmlAttribute]
		private string text;		
		public string Text {
			get { return text; }
			set { text = value; }
		}
		
		
		/// <summary>
		/// Answers to this question.
		/// </summary>
		/// <remarks>A worksheet question may require multiple answers
		/// e.g. a star rating, a comment, and the URL of supporting evidence</remarks>
		[XmlArray]
		private List<Answer> answers;
		public List<Answer> Answers {
			get { return answers; }
		}
		
		
		public Question()
		{			
			answers = new List<Answer>(1);	
		}
		
		
		public Question(string text) : this()
		{
			this.text = text;
		}
	}
}
