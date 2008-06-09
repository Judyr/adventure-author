/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 04/02/2008
 * Time: 11:01
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace AdventureAuthor.Ideas
{
	/// <summary>
	/// Description of BoardObject.
	/// </summary>
	[Serializable]
	public abstract class BoardObject : UnserializableControl
	{				
		[XmlElement]
		public abstract double X {
			get; set;
		}
		
		
		[XmlElement]
		public abstract double Y {
			get; set;
		}
	}
}
