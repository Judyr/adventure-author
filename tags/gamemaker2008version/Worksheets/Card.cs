
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using AdventureAuthor.Evaluation;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Evaluation
{
	[Serializable]
	[XmlRoot(ElementName="Worksheet")]
	public class Card
	{
		[XmlAttribute]
		private string title;		
		public string Title {
			get { return title; }
			set { title = value; }
		}
		
		
		[XmlAttribute]
		private string designerName;		
		public string DesignerName {
			get { return designerName; }
			set { designerName = value; }
		}
		
		
		[XmlAttribute]
		private string evaluatorName;		
		public string EvaluatorName {
			get { return evaluatorName; }
			set { evaluatorName = value; }
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
		public Card()
		{			
			this.sections = new List<Section>(1);
		}
		
		
		public Card(string title, string designerName, string evaluatorName, string date) : this()
		{
			this.title = title;
			this.designerName = designerName;
			this.evaluatorName = evaluatorName;
			this.date = date;
		}
	
		
		#region Methods
		
		public Section GetSection(string sectionName)
		{
			foreach (Section section in sections) {
				if (section.Title == sectionName) {
					return section;
				}
			}
			return null;
		}
		
		
		public Question GetQuestion(string questionName, string sectionName)
		{
			Section section = GetSection(sectionName);
			if (section == null) {
				return null;
			}
			
			foreach (Question question in section.Questions) {
				if (question.Text == questionName) {
					return question;
				}
			}
			return null;
		}
		
		
		/// <summary>
		/// Create a copy of this card in which all the fields are blank.
		/// </summary>
		/// <returns>Returns a blank copy of an existing card, in which all the fields are blank</returns>
		public Card GetBlankCopy()
		{
			Card card = GetCopy();
			card.Clear();
			return card;
		}
		
		
		public void Clear()
		{
			EvaluatorName = String.Empty;
			DesignerName = String.Empty;
			Date = String.Empty;			
			foreach (Section section in sections) {
				section.Clear();
			}
		}
		
		
		public Card GetCopy()
		{
			return (Card)MemberwiseClone();
		}
		
		
		/// <summary>
		/// Check whether a card has had any of its fields filled in.
		/// </summary>
		/// <returns>True if the fields of this card are entirely blank; false otherwise</returns>
		public bool IsBlank()
		{
			if ((EvaluatorName != null && EvaluatorName != String.Empty) || 
			    (Date != null && Date != String.Empty) ||
			    (DesignerName != null && DesignerName != String.Empty))
			{
				return false;
			}			
			
			foreach (Section section in sections) {
				if (!section.IsBlank()) {
					return false;
				}
			}
			return true;		
		}
		
		#endregion
	}
}
