
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using AdventureAuthor.Evaluation;
using AdventureAuthor.Utils;

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
		/// Create a blank copy of this worksheet in which all the fields are blank.
		/// </summary>
		/// <returns>Returns a blank copy of an existing worksheet, in which all the fields are blank</returns>
		public Worksheet GetBlankCopy()
		{
			Worksheet worksheet = GetCopy();
			worksheet.Clear();
			return worksheet;
		}
		
		
		public void Clear()
		{
			Name = String.Empty;
			Date = String.Empty;			
			foreach (Section section in sections) {
				section.Clear();
			}
		}
		
		
		public Worksheet GetCopy()
		{
			return (Worksheet)MemberwiseClone();
		}
		
		
		/// <summary>
		/// Check whether a worksheet has had any of its fields filled in.
		/// </summary>
		/// <returns>True if the fields of this worksheet are entirely blank; false otherwise</returns>
		public bool IsBlank()
		{
			if ((Name != null && Name != String.Empty) || (Date != null && Date != String.Empty)) {
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
