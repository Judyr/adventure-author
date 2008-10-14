/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 10/03/2008
 * Time: 21:52
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Reflection;
using System.Xml.Serialization;
using AdventureAuthor.Utils;
using AdventureAuthor.Evaluation;
using NWN2Toolset;
using OEILocalization;

namespace AdventureAuthor.Setup
{
	/// <summary>
	/// Representing the various options that can be set by the user when using the toolset.
	/// </summary>
	[Serializable]
	[XmlRoot]
	public class AdventureAuthorPluginPreferences : INotifyPropertyChanged
	{		
		#region Events	
		
		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void NotifyPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) {
				PropertyChanged(this,new PropertyChangedEventArgs(propertyName));
			}
		}
		
		#endregion
				
		#region Fields
		
		/// <summary>
		/// The single instance of this class.
		/// </summary>
		private static AdventureAuthorPluginPreferences instance;
		[XmlIgnore]
		public static AdventureAuthorPluginPreferences Instance {
			get {
				if (instance == null) {
					instance = new AdventureAuthorPluginPreferences(true,true);
				}
				return instance;
			}
			set {
				instance = value;
			}
		}
		
		
		// General:
		
		/// <summary>
		/// Whether or not the scratchpad area should automatically load when you open a module.
		/// </summary>
		private bool openScratchpadByDefault;	
		[XmlElement]		
		[Description("Whether or not the scratchpad area should automatically load when you open a module."), 
		 Category("General"), Browsable(true)]
		public bool OpenScratchpadByDefault {
			get { return openScratchpadByDefault; }
			set { 
				if (openScratchpadByDefault != value) {
					openScratchpadByDefault = value; 
					NotifyPropertyChanged("OpenScratchpadByDefault");
				}
			}
		}
				
		
		/// <summary>
		/// Whether or not the interface is unlocked (controls can be moved about the screen, resized
		/// and closed) or locked (controls are frozen in position.)
		/// </summary>
		private bool lockInterface;
		[XmlElement]
		[Description("True to allow interface controls to be docked, resized or closed."), 
		 Category("General"), Browsable(true)]
		public bool LockInterface {
			get { return lockInterface; }
			set { 
				if (lockInterface != value) {
					lockInterface = value;
					NotifyPropertyChanged("LockInterface");
				}
			}
		}		
		
		
		// Conversations:
						
//		/// <summary>
//		/// The font size of conversation dialogue and speaker names. 
//		/// </summary>
//		private double dialogueFontSize;
//		[XmlElement]
//		[Description("The font size of conversation dialogue and speaker names."), 
//		 Category("Conversations"), Browsable(true)]
//		public double DialogueFontSize {
//			get { return dialogueFontSize; }
//			set { 
//				if (dialogueFontSize != value) {
//					dialogueFontSize = value;
//					NotifyPropertyChanged("DialogueFontSize");
//				}
//			}
//		}
		
		
		/// <summary>
		/// The folder where local application data is kept for this application.
		/// </summary>
		private static string localAppDataDirectory;
		[XmlElement]
		[Description("The folder where local application data is kept for this application."), 
		 Category("General"), Browsable(false)]
		public static string LocalAppDataDirectory {
			get { return localAppDataDirectory; }
		}
		
		
		/// <summary>
		/// The folder where the serialised list of recently opened module filenames is kept.
		/// </summary>
		private static string recentlyOpenedModulesPath;
		[XmlElement]
		[Description("The folder where the serialised list of recently opened module filenames is kept."), 
		 Category("General"), Browsable(false)]
		public static string RecentlyOpenedModulesPath {
			get { return recentlyOpenedModulesPath; }
		}
		
		#endregion
		
		#region Constructors			
		
		static AdventureAuthorPluginPreferences()
		{
			instance = null;
			
			localAppDataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"Adventure Author");
			recentlyOpenedModulesPath = Path.Combine(localAppDataDirectory,"RecentlyOpenedModules.xml");
		}
 
		
		/// <summary>
		/// For deserialisation.
		/// </summary>
		/// <remarks>Don't use for anything other than deserialisation, as it
		/// seems to cause problems when you have List properties (if you construct a list
		/// here it seems to get 'added to itself' when called for deserialisation.)</remarks>
		private AdventureAuthorPluginPreferences()
		{
			
		}
		
		
		public AdventureAuthorPluginPreferences(bool lockInterface, bool openScratchpadByDefault)
		{			
			// default preferences - will only be used if there's no preferences file found:
			this.LockInterface = lockInterface;
			this.OpenScratchpadByDefault = openScratchpadByDefault;
			
			PropertyChanged += new PropertyChangedEventHandler(logPropertyChange);
		}
		
		#endregion
		
		#region Event handlers
		
		private void logPropertyChange(object sender, PropertyChangedEventArgs e)
		{
			try {
				PropertyInfo property = GetType().GetProperty(e.PropertyName);
				if (property != null) {
					object val = property.GetValue(this,null);
					if (val == null) {
						Log.WriteAction(LogAction.set,e.PropertyName);
					}
					else {
						Log.WriteAction(LogAction.set,e.PropertyName,val.ToString());
					}
				}
			}
			catch (Exception ex) {
				Say.Debug("Failed to log a property change to property " + e.PropertyName + ":\n" + ex.ToString());
			}
		}
		
		#endregion
	}
}
