/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 07/02/2008
 * Time: 15:58
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using AdventureAuthor.Evaluation.Viewer;

namespace AdventureAuthor.Evaluation
{
	/// <summary>
	/// Description of Response.
	/// </summary>
	[Serializable]
	public class Reply : OptionalWorksheetPart
	{
		public enum ReplierRole {
			Teacher,
			Playtester,
			Designer,
			Other
		}
		
		
		private string replier;			
		public string Replier {
			get { return replier; }
			set { replier = value; }
		}
		
		
		private ReplierRole replierType;		
		public ReplierRole ReplierType {
			get { return replierType; }
			set { replierType = value; }
		}
		
		
		private string text;		
		public string Text {
			get { return text; }
			set { text = value; }
		}
		
		
		public Reply()
		{
		}
		
		
		public override bool IsBlank()
		{
			return (text == null || text == String.Empty) &&
				   (replier == null || text == String.Empty);
		}
		
		
		public override OptionalWorksheetPartControl GetControl(bool designerMode)
		{
			ReplyControl replyControl = new ReplyControl(this,designerMode);
			return replyControl;
		}
	}
}
