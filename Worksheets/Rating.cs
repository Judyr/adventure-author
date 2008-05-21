/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 14/01/2008
 * Time: 11:49
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Windows;
using System.Xml.Serialization;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Evaluation
{
	[Serializable]
	[XmlRoot]
	public class Rating : Answer
	{		
		[XmlAttribute]
		private int max;		
		public int Max {
			get { return max; }
		}
		
		
		public Rating() : this(5)
		{	
		}
		
		
		public Rating(int max)
		{
			this.max = max;
			this.Value = "0";
		}
		
		
		public Rating(int max, int val) : this(max)
		{
			if (val > max) {
				throw new ArgumentException(val.ToString() + " is not in the range 1 to " + max + ".");
			}
			
			this.Value = val.ToString();
		}
		
		
		public override OptionalWorksheetPartControl GetControl()
		{
			RatingControl rating = new RatingControl(this);			
			return rating;
		}
		
		
		public override bool IsBlank()
		{
			return Value == null || Value == String.Empty || Value == "0";
		}
		
		
		public override string ToString()
		{
			if (IsBlank()) {
				return "Star rating:";
			}
			else {
				return "Star rating: " + Value + " out of " + max;
			}
		}
	}
}
