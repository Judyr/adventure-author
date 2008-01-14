/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 14/01/2008
 * Time: 13:30
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using AdventureAuthor.Evaluation;

namespace AdventureAuthor.Evaluation
{
	/// <summary>
	/// Description of Section.
	/// </summary>
	[XmlRoot]
	public class Section
	{
		[XmlAttribute]
		private string title;		
		public string Title {
			get { return title; }
			set { title = value; }
		}
		
		
		[XmlArray]
		private List<Question> questions;
		public List<Question> Questions {
			get { return questions; }
		}
		
		
		public Section(string title)
		{
			this.title = title;
			questions = new List<Question>();
		}
	}
}
