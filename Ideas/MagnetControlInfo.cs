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
	[XmlRoot("Magnet")]
	[XmlInclude(typeof(BlueprintMagnetControlInfo))]
	public class MagnetControlInfo : ISerializableData
	{
		[XmlAttribute]
		public double X;
    	
    	[XmlAttribute]
		public double Y;
		
		[XmlAttribute]
		public double Angle;	
		
		[XmlElement]	
		public Idea Idea;
			
		
		/// <summary>
		/// Constructor for serialization.
		/// </summary>
		protected MagnetControlInfo()
		{
			
		}
		
		
		public MagnetControlInfo(MagnetControl magnetControl) : this()
		{
			X = magnetControl.X;
			Y = magnetControl.Y;
			Idea = magnetControl.Idea;
			Angle = magnetControl.Angle;
		}
		
		
		public virtual UnserializableControl GetControl()
		{
			return new MagnetControl(this);
		}
	}
}
