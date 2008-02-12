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
using AdventureAuthor.Evaluation.Viewer;

namespace AdventureAuthor.Evaluation
{
	public abstract class OptionalWorksheetPart
	{
		[XmlAttribute]
		protected bool include = true;
		public bool Include {
			get { return include; }
			set { include = value; }
		}
		
		
		public abstract OptionalWorksheetPartControl GetControl();
		
		
		public abstract bool IsBlank();		
		
		
		public abstract void Clear();
	}
}
