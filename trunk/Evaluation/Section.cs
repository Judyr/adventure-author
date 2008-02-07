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
using AdventureAuthor.Evaluation.Viewer;

namespace AdventureAuthor.Evaluation
{
	/// <summary>
	/// Description of Section.
	/// </summary>
	[Serializable]
	[XmlRoot]
	public class Section : OptionalWorksheetPart
	{					
		[XmlAttribute]
		protected string title;		
		public string Title {
			get { return title; }
			set { title = value; }
		}	
		
		
		[XmlArray]
		private List<Question> questions;
		public List<Question> Questions {
			get { return questions; }
		}
		
		
		public Section()
		{
			questions = new List<Question>();			
		}
		
		
		public Section(string title) : this()
		{			
			Title = title;
		}
						
		
		public override OptionalWorksheetPartControl GetControl(bool designerMode)
		{
			return new SectionControl(this,designerMode);
		}
		
		
		public override bool IsBlank()
		{
			foreach (Question question in questions) {
				if (!question.IsBlank()) {
					return false;
				}
			}
			return true;
		}
		
		
		public override string ToString()
		{
			return title;
		}
	}
}
