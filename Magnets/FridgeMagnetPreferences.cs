/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 20/05/2008
 * Time: 15:02
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.IO;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Reflection;
using AdventureAuthor.Utils;
using Microsoft.Win32;

namespace AdventureAuthor.Ideas
{
	/// <summary>
	/// Description of MagnetPreferences.
	/// </summary>
	[Serializable]
	[XmlRoot]
	public class FridgeMagnetPreferences : INotifyPropertyChanged
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
		private static FridgeMagnetPreferences instance;
		[XmlIgnore]
		public static FridgeMagnetPreferences Instance {
			get {
				if (instance == null) {
	            	// Look for a saved preferences file in the default path - if it's not
	            	// there, use a generic set of preferences:
					if (File.Exists(defaultFridgeMagnetPreferencesPath) &&
					    Serialization.IsSerialisedObjectOfType(defaultFridgeMagnetPreferencesPath,typeof(FridgeMagnetPreferences))) 
					{
						object o = Serialization.Deserialize(defaultFridgeMagnetPreferencesPath,typeof(FridgeMagnetPreferences));
						FridgeMagnetPreferences.Instance = (FridgeMagnetPreferences)o;
					}
					else {
						instance = new FridgeMagnetPreferences();
					}
				}
				return instance;
			}
			set {
				instance = value;
			}
		}		
		
		
		/// <summary>
		/// The path of the magnets installation. 
		/// </summary>
		private string installDirectory;
		[XmlElement]
		public string InstallDirectory {
			get { return installDirectory; }
		}
		
		
		/// <summary>
		/// The path where user-accessible Fridge Magnets files are kept for this user account.
		/// </summary>
		private string userFridgeMagnetsDirectory;
		[XmlElement]
		public string UserFridgeMagnetsDirectory {
			get { return userFridgeMagnetsDirectory; }
			set { 
				if (userFridgeMagnetsDirectory != value) {
					userFridgeMagnetsDirectory = value;
					NotifyPropertyChanged("UserFridgeMagnetsDirectory");
				}
			}
		}
		
		
		/// <summary>
		/// The path where saved magnet board files are kept for this user account.
		/// </summary>
		private string savedMagnetBoardsDirectory;
		[XmlElement]
		public string SavedMagnetBoardsDirectory {
			get { return savedMagnetBoardsDirectory; }
			set { 
				if (savedMagnetBoardsDirectory != value) {
					savedMagnetBoardsDirectory = value;
					NotifyPropertyChanged("SavedMagnetBoardsDirectory");
				}
			}
		}
		
		
		/// <summary>
		/// The path where saved magnet box files are kept for this user account.
		/// </summary>
		private string savedMagnetBoxesDirectory;
		[XmlElement]
		public string SavedMagnetBoxesDirectory {
			get { return savedMagnetBoxesDirectory; }
			set { 
				if (savedMagnetBoxesDirectory != value) {
					savedMagnetBoxesDirectory = value;
					NotifyPropertyChanged("SavedMagnetBoxesDirectory");
				}
			}
		}
		
		
		/// <summary>
		/// The path to the magnet box the user is currently using.
		/// </summary>
		private string activeMagnetBoxPath;
		[XmlElement]
		public string ActiveMagnetBoxPath {
			get { return activeMagnetBoxPath; }
		}
		
		
		/// <summary>
		/// The folder where local application data is kept for this application.
		/// </summary>
		private static string localAppDataForMagnetsDirectory;
		[XmlElement]
		public static string LocalAppDataForMagnetsDirectory {
			get { return localAppDataForMagnetsDirectory; }
		}
		
		
		/// <summary>
		/// The path where user preferences are kept.
		/// </summary>
		private static string defaultFridgeMagnetPreferencesPath;
		[XmlElement]
		public static string DefaultFridgeMagnetPreferencesPath {
			get { return defaultFridgeMagnetPreferencesPath; }
		}
		
		
		// Magnets:
			
		/// <summary>
		/// Whether or not magnets in the Magnet Box will appear perfectly straight or slightly skewed.
		/// </summary>
		private bool useWonkyMagnets;
		[XmlElement]
		[Description("Whether magnets in the Magnet Box will appear perfectly straight " +
		             "or slightly skewed."), Category("Ideas"), Browsable(true)]
		public bool UseWonkyMagnets {
			get { return useWonkyMagnets; }
			set { 
				if (useWonkyMagnets != value) {
					useWonkyMagnets = value;
					NotifyPropertyChanged("UseWonkyMagnets");
				}
			}
		}	
		
		
		/// <summary>
		/// Whether or not the Magnet Box will appear at the right-hand side of the screen, or at the
		/// bottom of the screen.
		/// </summary>
		private bool magnetBoxAppearsAtSide;
		[XmlElement]
		[Description("Whether the Magnet Box will appear at the right-hand side of the screen, or at the " +
					 "bottom of the screen."), Category("Ideas"), Browsable(true)]
		public bool MagnetBoxAppearsAtSide {
			get { return magnetBoxAppearsAtSide; }
			set { 
				if (magnetBoxAppearsAtSide != value) {
					magnetBoxAppearsAtSide = value;
					NotifyPropertyChanged("MagnetBoxAppearsAtSide");
				}
			}
		}
		
		#endregion

		#region Constructors			
		
		static FridgeMagnetPreferences()
		{
			instance = null;	
			
			string localAppDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);			
			localAppDataForMagnetsDirectory = Path.Combine(localAppDataDirectory,"Fridge Magnets");
			defaultFridgeMagnetPreferencesPath = Path.Combine(localAppDataForMagnetsDirectory,"FridgeMagnetsPreferences.xml");
		}
 
		
		public FridgeMagnetPreferences()
		{			
			// default preferences - should only be used if there's no preferences file found:
			string myDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			userFridgeMagnetsDirectory = Path.Combine(myDocumentsPath,"Fridge Magnets");
			savedMagnetBoardsDirectory = Path.Combine(userFridgeMagnetsDirectory,"Magnet boards");
			savedMagnetBoxesDirectory = Path.Combine(userFridgeMagnetsDirectory,"Magnet boxes");
			activeMagnetBoxPath = Path.Combine(savedMagnetBoxesDirectory,"MyMagnetBox.box");
			useWonkyMagnets = true;
			magnetBoxAppearsAtSide = true;
						
			// TODO get the install directory from the registry:
			installDirectory = @"C:\Program Files\Heriot-Watt University\Fridge Magnets";			
			
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
