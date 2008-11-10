/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 24/01/2008
 * Time: 18:20
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Xml.Serialization;
using AdventureAuthor.Evaluation;

namespace AdventureAuthor.Evaluation
{
	public abstract class CardPart
	{
		[XmlAttribute]
		protected bool include = true;
		public bool Include {
			get { return include; }
			set { include = value; }
		}
		
		
		public abstract CardPartControl GetControl();
		
		
		public abstract bool IsBlank();		
		
		
		public abstract void Clear();
	}
}
