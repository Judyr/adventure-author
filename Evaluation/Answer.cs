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
using AdventureAuthor.Evaluation.UI;

namespace AdventureAuthor.Evaluation
{
	public abstract class Answer
	{
		[XmlElement]
		private string val;		
		public string Value {
			get { return val; }
			set { val = value; }
		}
		
		
		public abstract IAnswerControl GetAnswerControl();
	}
}
