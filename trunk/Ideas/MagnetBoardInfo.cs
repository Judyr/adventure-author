/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 14/02/2008
 * Time: 17:13
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Windows.Media;
using AdventureAuthor.Ideas;
using AdventureAuthor.Utils;
using System.Windows.Controls;
using System.Windows;

namespace AdventureAuthor.Ideas
{
	/// <summary>
	/// Description of MagnetBoardInfo.
	/// </summary>
	[Serializable]
	public class MagnetBoardInfo : ISerializableData
	{    	
		[XmlAttribute]
		private Color surfaceColour;
		public Color SurfaceColour {
			get { return surfaceColour; }
			set { surfaceColour = value; }
		}
		
		
    	[XmlArray]
    	private List<MagnetInfo> magnetInfos = new List<MagnetInfo>();       	
		public List<MagnetInfo> Magnets {
			get { return magnetInfos; }
			set { magnetInfos = value; }
		}
    	
    	
    	/// <summary>
    	/// Constructor for serialization.
    	/// </summary>
    	private MagnetBoardInfo()
    	{
    		
    	}
    	
    	
		public MagnetBoardInfo(MagnetBoardControl magnetBoardControl)
		{
			surfaceColour = magnetBoardControl.SurfaceColour;
			foreach (MagnetControl magnet in magnetBoardControl.GetMagnets()) {
				MagnetInfo magnetInfo = (MagnetInfo)magnet.GetSerializable();
				magnetInfos.Add(magnetInfo);
			}
		}
		
		
		/// <summary>
		/// Get a control based on the data in this object.
		/// </summary>
		/// <returns>A control based on this information</returns>
		public UnserializableControl GetControl()
		{
			return new MagnetBoardControl(this);
		}
	}
}
