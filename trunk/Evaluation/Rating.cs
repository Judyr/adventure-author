/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 14/01/2008
 * Time: 11:49
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using AdventureAuthor.Evaluation;

namespace AdventureAuthor.Evaluation
{
	public class Rating : Answer
	{
		private int min;		
		public int Min {
			get { return min; }
		}
		
		
		private int max;		
		public int Max {
			get { return max; }
		}
		
		
		public Rating(int val, int min, int max)
		{
			if (val < min || val > max) {
				throw new ArgumentException(val.ToString() + " is not in the range " + 
				                            min + " to " + max + ".");
			}
			
			this.Value = val.ToString();
			this.min = min;
			this.max = max;
		}
	}
}
