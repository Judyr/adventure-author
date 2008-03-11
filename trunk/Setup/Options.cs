/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 10/03/2008
 * Time: 21:52
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Xml.Serialization;
using AdventureAuthor.Utils;
using AdventureAuthor.Evaluation.Viewer;
using NWN2Toolset;
using OEILocalization;

namespace AdventureAuthor.Setup
{
	[Serializable]
	[XmlRoot]
	public class Options
	{		
		#region Events
			
		public event EventHandler ChangedDefaultImageApplication;		
		private void OnChangedDefaultImageApplication(EventArgs e) 
		{
			EventHandler handler = ChangedDefaultImageApplication;
			if (handler != null) {
				handler(this,e);
			}
		}
		
		#endregion
		
		#region Fields
		
		private bool openScratchpadByDefault;	
		[XmlElement]
		[OEIDescription(typeof(object), "NWN2ToolsetPreferences_OpenScratchpadByDefault"), 
		 OEICategory(typeof(object), "General")]
		public bool OpenScratchpadByDefault {
			get { return openScratchpadByDefault; }
			set { 
				openScratchpadByDefault = value; 
				Log.WriteAction(LogAction.set,"OpenScratchpadByDefault",value.ToString());
			}
		}
				
		
		private bool lockInterface;
		[XmlElement]
		[OEIDescription(typeof(object), "NWN2ToolsetPreferences_LockInterface"), 
		 OEICategory(typeof(object), "General")]
		public bool LockInterface {
			get { return lockInterface; }
			set { 
				lockInterface = value; 
				if (lockInterface) {
					Toolset.LockInterface();
				}
				else {
					Toolset.UnlockInterface();
				}
				Log.WriteAction(LogAction.set,"LockInterface",value.ToString());
			}
		}
		
		
			
		private ImageApp imageViewer;
		[XmlElement]
		[OEIDescription(typeof(object), "NWN2ToolsetPreferences_ImageViewer"), 
		 OEICategory(typeof(object), "Evaluation")]
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
				OnChangedDefaultImageApplication(new EventArgs());
			}
		}
		
				
		private Uri nwn2InstallationDirectory;		
		[XmlElement]
		[OEIDescription(typeof(object), "NWN2ToolsetPreferences_NWN2InstallationDirectory"), 
		 OEICategory(typeof(object), "General")]
		public Uri NWN2InstallationDirectory {
			get { return nwn2InstallationDirectory; }
			set { 
				nwn2InstallationDirectory = value; 
				Log.WriteAction(LogAction.set,"NWN2InstallationDirectory",value.ToString());
			}
		}		
		
		#endregion
		
		#region Constructors		
		
		public Options()
		{
			lockInterface = false;
			openScratchpadByDefault = false;
			imageViewer = ImageApp.Default;
			nwn2InstallationDirectory = new Uri(@"C:\Program Files\Atari\Neverwinter Nights 2");
			//TODO
			// using a Uri doesn't seem to give you a browse button automatically
			// might be unable to get access to categorising options
			// more options could go in this class
			// AdventureAuthor.Setup contents could all go in Core
		}
		
		
		public Options(string filename)
		{
			throw new NotImplementedException();
		}
		
		#endregion
	}
}
