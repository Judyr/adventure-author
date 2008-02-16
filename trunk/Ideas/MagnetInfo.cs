/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 14/02/2008
 * Time: 17:09
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Xml.Serialization;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Ideas
{
	/// <summary>
	/// Description of MagnetInfo.
	/// </summary>
	[Serializable]
	public class MagnetInfo : ISerializableData
	{
		[XmlElement]
		public double X;
    	
    	[XmlElement]
		public double Y;
		
		[XmlElement]	
		public Idea Idea;
		
		[XmlElement]
		public double Angle;
		
		
		/// <summary>
		/// Constructor for serialization.
		/// </summary>
		private MagnetInfo()
		{
			
		}
		
		
		public MagnetInfo(MagnetControl magnetControl)
		{
			X = magnetControl.X;
			Y = magnetControl.Y;
			Idea = magnetControl.Idea;
			Angle = magnetControl.Angle;
		}
		
		
		public UnserializableControl GetControl()
		{
			return new MagnetControl(this);
		}
	}
}
