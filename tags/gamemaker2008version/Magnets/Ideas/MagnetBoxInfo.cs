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
using System.Windows.Controls;
using System.Windows;

namespace AdventureAuthor.Ideas
{
	/// <summary>
	/// Description of MagnetListInfo.
	/// </summary>
	[Serializable]
	[XmlRoot("MagnetBox")]
	public class MagnetBoxInfo : ISerializableData
	{    	   	
    	[XmlElement("ID")]
    	private Guid id;
		public Guid ID {
			get { return id; }
			set { id = value; }
		}   
        
        
    	[XmlAttribute("Version")]
        /// <summary>
    	/// The version of Fridge Magnets this object was created with.
    	/// </summary>
        private string version;
		public string Version {
			get { return version; }
			set { version = value; }
		}
    	    	
    	
		[XmlArray("Magnets")]
		[XmlArrayItemAttribute("Magnet")]
    	private List<MagnetControlInfo> magnetInfos = new List<MagnetControlInfo>();       	
		public List<MagnetControlInfo> Magnets {
			get { return magnetInfos; }
			set { magnetInfos = value; }
		}
    	
    	
    	/// <summary>
    	/// Constructor for serialization.
    	/// </summary>
    	private MagnetBoxInfo()
    	{
    		
    	}
    	
    	
		public MagnetBoxInfo(MagnetBox magnetBox)
		{
			foreach (MagnetControl magnet in magnetBox.GetMagnets(false)) {
				MagnetControlInfo magnetInfo = (MagnetControlInfo)magnet.GetSerializable();
				magnetInfos.Add(magnetInfo);
			}
			this.id = magnetBox.ID;
           	this.version = magnetBox.Version;
		}
		
		
		/// <summary>
		/// Get a control based on the data in this object.
		/// </summary>
		/// <returns>A control based on this information</returns>
		/// <remarks>Save automatically is set to false since there is no filename to save to</remarks>
		public UnserializableControl GetControl()
		{
			MagnetBox magnetList = new MagnetBox(this);
			return magnetList;
		}
	}
}
