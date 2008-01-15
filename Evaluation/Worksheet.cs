
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using AdventureAuthor.Evaluation;

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
		
		/// <summary>
		/// Create a blank copy of this worksheet in which all the fields are blank.
		/// </summary>
		/// <returns>Returns a blank copy of an existing worksheet, in which all the fields are blank</returns>
		public Worksheet GetBlankCopy()
		{
			Worksheet worksheet = (Worksheet)this.MemberwiseClone();
			worksheet.ClearFields();
			return worksheet;
		}
		
		
		private void ClearFields()
		{
			Name = String.Empty;
			Date = String.Empty;
			foreach (Section section in sections) {
				foreach (Question question in section.Questions) {
					foreach (Answer answer in question.Answers) {
						answer.Value = String.Empty;
					}
				}
			}
		}
		
		#endregion
	}
}
