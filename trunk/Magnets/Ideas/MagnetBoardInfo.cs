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
using System.Windows.Controls;
using System.Windows;

namespace AdventureAuthor.Ideas
{
	/// <summary>
	/// Description of MagnetBoardInfo.
	/// </summary>
	[Serializable]
	[XmlRoot("MagnetBoard")]
	public class MagnetBoardInfo : ISerializableData
	{    	          
    	[XmlAttribute("Version")]
        /// <summary>
    	/// The version of Fridge Magnets this object was created with.
    	/// </summary>
        private string version;
		public string Version {
			get { return version; }
			set { version = value; }
		}
        
        
		[XmlAttribute("Colour")]
		private Color surfaceColour;
		public Color SurfaceColour {
			get { return surfaceColour; }
			set { surfaceColour = value; }
		}
		
		
		[XmlArray("Magnets")]
		[XmlArrayItemAttribute("Magnet")]
    	private List<MagnetControlInfo> magnetInfos = new List<MagnetControlInfo>();       	
		public List<MagnetControlInfo> Magnets {
			get { return magnetInfos; }
			set { magnetInfos = value; }
		}
                

    	[XmlElement("ID")]
    	private Guid id;
		public Guid ID {
			get { return id; }
			set { id = value; }
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
				MagnetControlInfo magnetInfo = (MagnetControlInfo)magnet.GetSerializable();
				magnetInfos.Add(magnetInfo);
			}
			version = magnetBoardControl.Version;
			id = magnetBoardControl.ID;
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
