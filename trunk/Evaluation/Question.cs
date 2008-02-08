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
	public class Question : OptionalWorksheetPart
	{				
		/// <summary>
		/// The text of this question.
		/// </summary>
		[XmlAttribute]
		protected string text;		
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
		
		
		/// <summary>
		/// A series of replies made to this question once it has been answered.
		/// </summary>
		/// <remarks>Usually the initial comment will relate directly to the answers for this question,
		/// for example a teacher marking a pupil's answers, or a designer responding to a playtester's
		/// criticisms. Subsequent comments may directly reply to previous comments, or simply to the
		/// question in general.</remarks>
		[XmlElement]
		private List<Reply> replies;			
		public List<Reply> Replies {
			get { return replies; }
		}		
		
		
		public Question()
		{			
			answers = new List<Answer>(1);
			replies = new List<Reply>(0);
		}
		
		
		public Question(string text) : this()
		{
			this.text = text;
		}
		
		
		public override OptionalWorksheetPartControl GetControl(bool designerMode)
		{
			return new QuestionControl(this,designerMode);
		}
		
		
		public override bool IsBlank()
		{
			foreach (Answer answer in answers) {
				if (!answer.IsBlank()) {
					return false;
				}
			}
			return true;
		}
		
		
		public override string ToString()
		{
			return text;
		}
	}
}
