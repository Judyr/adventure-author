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

namespace AdventureAuthor.Evaluation
{
	/// <summary>
	/// Description of Evidence.
	/// </summary>
	[Serializable]
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
		
		
		public override OptionalWorksheetPartControl GetControl()
		{
			return new EvidenceControl(this);
		}
		
		
		public override bool IsBlank()
		{
			return Value == null || Value == String.Empty;
		}
		
		
		public override string ToString()
		{
			return "Supporting evidence: " + Value;
		}
	}
}
