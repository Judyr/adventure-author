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

namespace AdventureAuthor.Evaluation
{
	/// <summary>
	/// Description of WorksheetPreferences.
	/// </summary>
	[Serializable]
	[XmlRoot]
	public class WorksheetPreferences : INotifyPropertyChanged
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
		private static WorksheetPreferences instance;
		[XmlIgnore]
		public static WorksheetPreferences Instance {
			get {
				if (instance == null) {
					instance = new WorksheetPreferences();
				}
				return instance;
			}
			set {
				instance = value;
			}
		}
		
		
		/// <summary>
		/// The path of the worksheets installation. 
		/// </summary>
		private string installDirectory;
		[XmlElement]
		public string InstallDirectory {
			get { return installDirectory; }
			set { 
				if (installDirectory != value) {
					installDirectory = value;
					NotifyPropertyChanged("InstallDirectory");
				}
			}
		}
		
		
		/// <summary>
		/// The path where saved worksheets are kept for this user account.
		/// </summary>
		private string savedWorksheetsDirectory;
		[XmlElement]
		public string SavedWorksheetsDirectory {
			get { return savedWorksheetsDirectory; }
			set { 
				if (savedWorksheetsDirectory != value) {
					savedWorksheetsDirectory = value;
					NotifyPropertyChanged("SavedWorksheetsDirectory");
				}
			}
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
		
		static WorksheetPreferences()
		{
			instance = null;
		}
 
		
		public WorksheetPreferences()
		{			
			// default preferences - should only be used if there's no preferences file found:
			this.ImageViewer = ImageApp.Default;
			this.InstallDirectory = @"C:\Program Files\Heriot-Watt University\Worksheets";
			string myDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			this.SavedWorksheetsDirectory = Path.Combine(myDocumentsPath,"Worksheets");
			
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
