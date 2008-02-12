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
		private string replier;			
		public string Replier {
			get { return replier; }
			set { replier = value; }
		}
		
		
		private Role replierType;		
		public Role ReplierType {
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
		
		
		public override void Clear()
		{
			text = String.Empty;
			replier = String.Empty;
		}
		
		
		public override OptionalWorksheetPartControl GetControl()
		{
			ReplyControl replyControl = new ReplyControl(this);
			return replyControl;
		}
	}
}
