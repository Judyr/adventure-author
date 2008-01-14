/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 14/01/2008
 * Time: 13:15
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Xml.Serialization;
using System.Windows;
using AdventureAuthor.Evaluation;
using AdventureAuthor.Evaluation.UI;
using AdventureAuthor.Scripts.UI;

namespace AdventureAuthor.Evaluation
{
	/// <summary>
	/// Description of Evidence.
	/// </summary>
	[XmlRoot]
	public class Evidence : Answer
	{
		public Evidence()
		{			
		}
		
		
		public Evidence(string location)
		{
			this.Value = location;
		}
		
		
		public override IQuestionPanel GetUIControl()
		{
			return new EvidenceButton(this);
		}
	}
}
