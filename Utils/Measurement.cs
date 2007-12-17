/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 12/12/2007
 * Time: 12:25
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using Microsoft.DirectX;

namespace AdventureAuthor.Utils
{
	/// <summary>
	/// Provides methods to aid with measurement in game areas.
	/// </summary>
	public static class Measurement
	{
		/// <summary>
		/// Find the distance between two 3D vectors.
		/// </summary>
		/// <param name="v1">The first vector</param>
		/// <param name="v2">The second vector</param>
		/// <returns>The distance between the two vectors</returns>
		public static double GetDistanceBetween(Vector3 v1, Vector3 v2)
		{			
			double b = GetDifferenceBetween((double)v1.X,(double)v2.X);
			double c = GetDifferenceBetween((double)v1.Y,(double)v2.Y);			
			return Math.Sqrt((b*b) + (c*c));
		}
		
		
		/// <summary>
		/// Find the difference between two numbers.
		/// </summary>
		/// <param name="a">The first number</param>
		/// <param name="b">The second number</param>
		/// <returns>The difference between the two numbers</returns>
		private static double GetDifferenceBetween(double a, double b)
		{
			if (a > b) {
				return a - b;
			}
			else {
				return b - a;
			}
		}
	}
}
