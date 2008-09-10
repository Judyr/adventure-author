/*
 *   This file is part of Adventure Author.
 *
 *   Adventure Author is copyright Heriot-Watt University 2006-2008.
 *
 *   This copyright and licence apply to all source code, compiled code,
 *   documentation, graphics and auxiliary files, except where otherwise stated.
 *
 *   Adventure Author is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 *   Adventure Author is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 * 
 *   Adventure Author is a plugin for Atari's Neverwinter Nights 2, a COMMERCIAL
 *   product. Permission is given to link this GPL-covered plug-in with the 
 *   non-free main program. 
 *
 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Reflection;
using AdventureAuthor.Utils;
using Microsoft.Win32;

namespace AdventureAuthor.Tasks
{
	/// <summary>
	/// Description of MagnetPreferences.
	/// </summary>
	[Serializable]
	[XmlRoot]
	public class MyTasksPreferences : INotifyPropertyChanged
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
		private static MyTasksPreferences instance;
		[XmlIgnore]
		public static MyTasksPreferences Instance {
			get {
				if (instance == null) {
	            	// Look for a saved preferences file in the default path - if it's not
	            	// there, use a generic set of preferences:
					if (File.Exists(defaultPreferencesPath) &&
					    Serialization.IsSerialisedObjectOfType(defaultPreferencesPath,typeof(MyTasksPreferences))) 
					{
						object o = Serialization.Deserialize(defaultPreferencesPath,typeof(MyTasksPreferences));
						MyTasksPreferences.Instance = (MyTasksPreferences)o;
					}
					else {
	            		ObservableCollection<string> tags = new ObservableCollection<string>{"Gameplay",
	            																			 "Bugs",
	            																			 "Area design",
	            																			 "Story",
	            																			 "Dialogue"};
	            		instance = new MyTasksPreferences(tags);
					}
				}
				return instance;
			}
			set {
				instance = value;
			}
		}		
		
		
		/// <summary>
		/// The path of the installation. 
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
		/// The path where user-accessible files are kept for this user account.
		/// </summary>
		private string userFilesDirectory;
		[XmlElement]
		public string UserFilesDirectory {
			get { return userFilesDirectory; }
			set { 
				if (userFilesDirectory != value) {
					userFilesDirectory = value;
					NotifyPropertyChanged("UserFilesDirectory");
				}
			}
		}
		
		
		/// <summary>
		/// The path to the file which is currently open in the application.
		/// </summary>
		private string activeFilePath;
		[XmlElement]
		public string ActiveFilePath {
			get { return activeFilePath; }
			set { 
				if (activeFilePath != value) {
					activeFilePath = value;
					NotifyPropertyChanged("ActiveFilePath");
				}
			}
		}
		
		
		/// <summary>
		/// The path to the last My Tasks file that was opened.
		/// </summary>
		private string previousFilePath;
		[XmlElement]
		public string PreviousFilePath {
			get { return previousFilePath; }
			set { 
				if (previousFilePath != value) {
					previousFilePath = value;
					NotifyPropertyChanged("PreviousFilePath");
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
		
		
		private ObservableCollection<string> preDefinedTags;
		[XmlArray]
		public ObservableCollection<string> PreDefinedTags {
			get { return preDefinedTags; }
			set { 
				if (preDefinedTags != value) {
					preDefinedTags = value;
					NotifyPropertyChanged("PreDefinedTags");
				}
			}
		}
		
		#endregion

		#region Constructors			
		
		static MyTasksPreferences()
		{
			instance = null;				
			localAppDataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"My Tasks");
			defaultPreferencesPath = Path.Combine(localAppDataDirectory,"MyTasksPreferences.xml");
		}
		
		
		/// <summary>
		/// For deserialisation only.
		/// </summary>
		private MyTasksPreferences()
		{			
		}
		
		
		/// <summary>
		/// Construct a MyTaskPreferences object with mostly default values.
		/// </summary>
		/// <param name="tags">The pre-defined tags to offer the user</param>
		/// <remarks>Caused problems when I used a single parameterless constructor
		/// for both construction and deserialisation (tag collection kept getting
		/// added to itself, not sure why) so I created this one - the tags
		/// parameter is fairly arbitrary but it needed a different method signature, so.</remarks>
		private MyTasksPreferences(ObservableCollection<string> tags)
		{			
			string myDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			this.userFilesDirectory = Path.Combine(myDocumentsPath,"My Tasks");
			this.activeFilePath = null;
			this.previousFilePath = null;
			
			// TODO get the install directory from the registry:			
			this.installDirectory = @"C:\Program Files\Heriot-Watt University\My Tasks";
			
			preDefinedTags = tags;
			
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

