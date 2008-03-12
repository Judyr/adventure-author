/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 10/03/2008
 * Time: 21:52
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.IO;
using System.ComponentModel;
using System.Xml.Serialization;
using AdventureAuthor.Utils;
using AdventureAuthor.Evaluation.Viewer;
using NWN2Toolset;
using OEILocalization;

namespace AdventureAuthor.Setup
{
	/// <summary>
	/// Representing the various options that can be set by the user when using the toolset.
	/// </summary>
	[Serializable]
	[XmlRoot]
	public class AdventureAuthorPluginPreferences
	{		
		#region Events			
		
		public event EventHandler OpenScratchpadByDefaultChanged;		
		private void OnOpenScratchpadByDefaultChanged(EventArgs e) 
		{
			EventHandler handler = OpenScratchpadByDefaultChanged;
			if (handler != null) {
				handler(this,e);
			}
		}
		
					
		public event EventHandler LockedInterface;		
		private void OnLockedInterface(EventArgs e) 
		{
			EventHandler handler = LockedInterface;
			if (handler != null) {
				handler(this,e);
			}
		}
			
		
		public event EventHandler UnlockedInterface;		
		private void OnUnlockedInterface(EventArgs e) 
		{
			EventHandler handler = UnlockedInterface;
			if (handler != null) {
				handler(this,e);
			}
		}
		
		
		public event EventHandler DefaultImageViewerChanged;		
		private void OnDefaultImageViewerChanged(EventArgs e) 
		{
			EventHandler handler = DefaultImageViewerChanged;
			if (handler != null) {
				handler(this,e);
			}
		}
		
		
		public event EventHandler Nwn2InstallationDirectoryChanged;
		private void OnNwn2InstallationDirectoryChanged(EventArgs e) 
		{
			EventHandler handler = Nwn2InstallationDirectoryChanged;
			if (handler != null) {
				handler(this,e);
			}
		}			
		
		#endregion
				
		#region Fields
		
		/// <summary>
		/// The single instance of this class.
		/// </summary>
		private static AdventureAuthorPluginPreferences instance;
		public static AdventureAuthorPluginPreferences Instance {
			get {
				if (instance == null) {
					instance = new AdventureAuthorPluginPreferences();
				}
				return instance;
			}
			set {
				instance = value;
			}
		}
		
		
		/// <summary>
		/// True to open the scratchpad area (if one exists) automatically upon
		/// opening a module; otherwise false.
		/// </summary>
		[XmlElement]		
		[Description("True to open the scratchpad area (if one exists) automatically upon" + 
		 			 "opening a module; otherwise false."), Category("General"), Browsable(true)]
		private bool openScratchpadByDefault;	
		public bool OpenScratchpadByDefault {
			get { return openScratchpadByDefault; }
			set { 
				openScratchpadByDefault = value; 
				Log.WriteAction(LogAction.set,"OpenScratchpadByDefault",value.ToString());
				OnOpenScratchpadByDefaultChanged(new EventArgs());
			}
		}
				
		
		/// <summary>
		/// True if the interface is locked (i.e. controls cannot be docked,
		/// resized or closed); false if it is unlocked (can be altered). 
		/// </summary>
		[XmlElement]
		[Description("True to allow interface controls to be docked, resized or closed."), Category("General"), Browsable(true)]
		private bool lockInterface;
		public bool LockInterface {
			get { return lockInterface; }
			set { 
				lockInterface = value;
				if (lockInterface) {
					OnLockedInterface(new EventArgs());
				}
				else {
					OnUnlockedInterface(new EventArgs());
				}
				Log.WriteAction(LogAction.set,"LockInterface",value.ToString());
			}
		}
						
			
		/// <summary>
		/// The default application to open images in (usually when viewing a piece of evidence
		/// in the Evaluation application).
		/// </summary>
		[XmlElement]
		[Description("The default application to open images in (usually when viewing a piece of evidence" + 
					 "in the Evaluation application)."), Category("General"), Browsable(true)]
		private ImageApp imageViewer;
		public ImageApp ImageViewer {
			get { return imageViewer; }
			set { 
				imageViewer = value; 
				switch (imageViewer) {
					case ImageApp.Default:
	    				Log.WriteMessage("checked 'View images in default application'");
						break;
					case ImageApp.MicrosoftPaint:
						Log.WriteMessage("checked 'View images in Microsoft Paint'");
						break;
				}    				
				OnDefaultImageViewerChanged(new EventArgs());
			}
		}
		
				
		/// <summary>
		/// The location of the Neverwinter Nights 2 main install.
		/// </summary>
		[XmlElement]
		[Description("The location of the Neverwinter Nights 2 main install."), Category("General"), Browsable(true)]
		private string nwn2InstallationDirectory;		
		public string NWN2InstallationDirectory {
			get { return nwn2InstallationDirectory; }
			set { 
				nwn2InstallationDirectory = value;				
				Log.WriteAction(LogAction.set,"NWN2InstallationDirectory",value.ToString());
				OnNwn2InstallationDirectoryChanged(new EventArgs());
			}
		}		
		
		#endregion
		
			// more options could go in this class
			// AdventureAuthor.Setup contents could all go in Core
		
		#region Constructors			
		
		static AdventureAuthorPluginPreferences()
		{
			instance = null;
		}
 
		
		public AdventureAuthorPluginPreferences()
		{			
		}
		
		#endregion
	}
}
