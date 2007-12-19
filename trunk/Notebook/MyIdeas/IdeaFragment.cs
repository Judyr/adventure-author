/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 18/12/2007
 * Time: 02:07
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace AdventureAuthor.Notebook.MyIdeas
{
	[SerializableAttribute]
	public class IdeaFragment
	{
		[XmlElement]
		private string text;		
		public string Text {
			get { return text; }
			set { text = value; }
		}
		
		
		[XmlArray]
		private List<string> tags;		
		public List<string> Tags {
			get { return tags; }
			set { tags = value; }
		}
		
		
		public IdeaFragment(string text)
		{
			this.text = text;
		}
	}
}
