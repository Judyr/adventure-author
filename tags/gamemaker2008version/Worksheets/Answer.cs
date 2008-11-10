/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 14/01/2008
 * Time: 12:56
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Xml.Serialization;
using System.Windows;
using AdventureAuthor.Evaluation;

namespace AdventureAuthor.Evaluation
{
	[XmlRoot]
	public abstract class Answer : CardPart
	{	
		[XmlElement]
		protected string val;		
		public string Value {
			get { return val; }
			set { val = value; }
		}
		
		
		public override void Clear()
		{
			val = String.Empty;
		}
	}
}
