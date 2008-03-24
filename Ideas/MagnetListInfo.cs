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
using AdventureAuthor.Ideas;
using AdventureAuthor.Utils;
using System.Windows.Controls;
using System.Windows;

namespace AdventureAuthor.Ideas
{
	/// <summary>
	/// Description of MagnetListInfo.
	/// </summary>
	[Serializable]
	[XmlRoot("MagnetBox")]
	public class MagnetListInfo : ISerializableData
	{    	
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
    	private MagnetListInfo()
    	{
    		
    	}
    	
    	
		public MagnetListInfo(MagnetList magnetList)
		{
			foreach (MagnetControl magnet in magnetList.GetMagnets(false)) {
				MagnetControlInfo magnetInfo = (MagnetControlInfo)magnet.GetSerializable();
				magnetInfos.Add(magnetInfo);
			}
		}
		
		
		/// <summary>
		/// Get a control based on the data in this object.
		/// </summary>
		/// <returns>A control based on this information</returns>
		/// <remarks>Save automatically is set to false since there is no filename to save to</remarks>
		public UnserializableControl GetControl()
		{
			MagnetList magnetList = new MagnetList(this);
			return magnetList;
		}
	}
}
