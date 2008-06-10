/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 01/04/2008
 * Time: 21:08
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Xml.Serialization;

namespace AdventureAuthor.Ideas
{
	/// <summary>
	/// Description of BlueprintMagnetControlInfo.
	/// </summary>
	[Serializable]
	[XmlRoot("BlueprintMagnet")]
	public class BlueprintMagnetControlInfo : MagnetControlInfo
	{
		[XmlElement]
		public string BlueprintIdentifier;
		
    	
		/// <summary>
		/// Constructor for serialization.
		/// </summary>
		protected BlueprintMagnetControlInfo() : base()
		{
			
		}
		
		
		public BlueprintMagnetControlInfo(BlueprintMagnetControl blueprintMagnetControl) : base(blueprintMagnetControl)
		{
			BlueprintIdentifier = blueprintMagnetControl.BlueprintIdentifier;
		}
		
		
		public override UnserializableControl GetControl()
		{
			return new BlueprintMagnetControl(this);
		}
	}
}
