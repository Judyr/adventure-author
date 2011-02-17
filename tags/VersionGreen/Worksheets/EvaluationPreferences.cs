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

namespace AdventureAuthor.Evaluation
{
	/// <summary>
	/// Description of EvaluationPreferences.
	/// </summary>
	[Serializable]
	[XmlRoot]
	public class EvaluationPreferences : INotifyPropertyChanged
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
		private static EvaluationPreferences instance;
		[XmlIgnore]
		public static EvaluationPreferences Instance {
			get {
				if (instance == null) {
	            	// Look for a saved preferences file in the default path - if it's not
	            	// there, use a generic set of preferences:
					if (File.Exists(defaultPreferencesPath) &&
					    Serialization.IsSerialisedObjectOfType(defaultPreferencesPath,typeof(EvaluationPreferences))) 
					{
						object o = Serialization.Deserialize(defaultPreferencesPath,typeof(EvaluationPreferences));
						EvaluationPreferences.Instance = (EvaluationPreferences)o;
					}
					else {
						instance = new EvaluationPreferences();
					}
				}
				return instance;
			}
			set {
				instance = value;
			}
		}		
		
		
		/// <summary>
		/// The path of the Comment Cards installation. 
		/// </summary>
		private string installDirectory;
		[XmlElement]
		public string InstallDirectory {
			get { return installDirectory; }
		}
		
		
		/// <summary>
		/// The path where saved Comment Cards are kept for this user account.
		/// </summary>
		private string savedCommentCardsDirectory;
		[XmlElement]
		public string SavedCommentCardsDirectory {
			get { return savedCommentCardsDirectory; }
			set { 
				if (savedCommentCardsDirectory != value) {
					savedCommentCardsDirectory = value;
					NotifyPropertyChanged("SavedCommentCardsDirectory");
				}
			}
		}
		
		
		/// <summary>
		/// The folder where local application data is kept for this application.
		/// </summary>
		private static string localAppDataDirectory;
		[XmlElement]
		public static string LocalAppDataDirectory {
			get { return localAppDataDirectory; }
		}
		
		
		/// <summary>
		/// The path where user preferences are kept.
		/// </summary>
		private static string defaultPreferencesPath;
		[XmlElement]
		public static string DefaultPreferencesPath {
			get { return defaultPreferencesPath; }
		}
		
		
		/// <summary>
		/// The default application to open images in.
		/// </summary>
		private ImageApp imageViewer;
		[XmlElement]
		public ImageApp ImageViewer {
			get { return imageViewer; }
			set { 
				if (imageViewer != value) {
					imageViewer = value;
					NotifyPropertyChanged("ImageViewer");
				}
			}
		}
		
		#endregion

		#region Constructors			
		
		static EvaluationPreferences()
		{
			instance = null;	
			
			string genericLocalApplicationDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);			
			localAppDataDirectory = Path.Combine(genericLocalApplicationDataDirectory,"Comment Cards");
			defaultPreferencesPath = Path.Combine(localAppDataDirectory,"CommentCardsPreferences.xml");
		}
 
		
		public EvaluationPreferences()
		{			
			// default preferences - should only be used if there's no preferences file found:
			this.ImageViewer = ImageApp.Default;
			string myDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			this.SavedCommentCardsDirectory = Path.Combine(myDocumentsPath,"Comment Cards");		
									
			installDirectory = String.Empty; // deprecated	
			
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
