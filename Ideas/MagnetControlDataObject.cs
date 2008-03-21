/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 21/02/2008
 * Time: 13:59
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Windows;

namespace AdventureAuthor.Ideas
{
	/// <summary>
	/// Description of MagnetControlDataObject.
	/// </summary>
	public struct MagnetControlDataObject
	{
		public MagnetControl Magnet;
		public Size ActualRotatedSize; // actual size of magnet, accounting for rotation if appropriate
		public double MagnetHeight;
		public double MagnetWidth;
	}
}
