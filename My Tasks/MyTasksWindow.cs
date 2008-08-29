using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Reflection;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AdventureAuthor.Utils;
using Microsoft.Win32;

namespace AdventureAuthor.Tasks
{
	/// <summary>
	/// The main user interface window for the My Tasks Application.
	/// </summary>
	public partial class MyTasksWindow : Window
	{
		#region Constants
		
		public const string APPLICATION_NAME = "My Tasks";
		
		#endregion
		
		#region Properties and fields
    	
    	/// <summary>
    	/// True if the file has changed since the last save; false otherwise.
    	/// </summary>
    	private bool dirty = false;    	
		public bool Dirty {
			get { return dirty; }
			set {
				dirty = value;
				OnChanged(new EventArgs());
			}
		}
		
		#endregion
		
		#region Events
		
		private event EventHandler Changed;		
		protected virtual void OnChanged(EventArgs e) {
			EventHandler handler = Changed;
			if (handler != null) {
				handler(this,e);
			}
		}
		
		#endregion
		
		#region Constructors
		
		/// <summary>
		/// Create the main user interface window for the My Tasks application.
		/// </summary>
		public MyTasksWindow()
		{			
			InitializeComponent();  
	                		
			try {
				Tools.EnsureDirectoryExists(MyTasksPreferences.LocalAppDataDirectory);
				Tools.EnsureDirectoryExists(MyTasksPreferences.Instance.UserFilesDirectory);
			}
			catch (Exception e) {
	    		Say.Debug("Failed to create directory for user:\n"+e);
			}  
			
			// Set up event handlers:
			Closing += HandleApplicationClosing;
			MyTasksPreferences.Instance.PropertyChanged += HandlePreferencesChanged;
			this.Changed += UpdateTitleBar;
			
			// If you can find the last file that was opened, attempt to open it. Otherwise, open a blank task list.
			string previousFilePath = MyTasksPreferences.Instance.PreviousFilePath;
			if (previousFilePath != null && File.Exists(previousFilePath)) {
				Open(previousFilePath);
			}
			else {
				New();
			}
		}
		
		#endregion
		
		#region Methods
		
		/// <summary>
		/// Open a new task collection.
		/// </summary>
		public void New()
		{
			CloseCurrentFile();
			TaskCollection tasks = new TaskCollection();
			Open(tasks);
			UpdateTitleBar();
		}
		
		
		/// <summary>
		/// Open a task collection.
		/// </summary>
		/// <param name="tasks">The collection of tasks to open</param>
		private void Open(TaskCollection tasks)
		{
			if (tasks == null) {
				throw new ArgumentNullException();
			}			
			
			// Track any changes to these tasks:
			foreach (Task task in tasks) {
				task.PropertyChanged += MakeDirty;			
			}
			tasks.CollectionChanged += MakeDirtyWhenTaskCollectionChanges;
			
			// Open the task collection in the TaskPad:
			pad.Open(tasks);
			
			MyTasksPreferences.Instance.ActiveFilePath = null;
			Dirty = false;
		}

		
		private void MakeDirtyWhenTaskCollectionChanges(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action) {
				case NotifyCollectionChangedAction.Add:
					MakeDirty();
					break;
				case NotifyCollectionChangedAction.Move:
					MakeDirty();
					break;
				case NotifyCollectionChangedAction.Remove:
					MakeDirty();
					break;
				default:
					DebugLog.Write("Something unusual happened to the Tasks collection: (" + e.Action + ") " + e.ToString());
					break;
			}
		}
		
		
		/// <summary>
		/// Open a task collection at a specified path.
		/// </summary>
		/// <param name="path">The location of a valid task collection file</param>
		public void Open(string path)
		{
			try {
				object obj = Serialization.Deserialize(path,typeof(TaskCollection));
				TaskCollection tasks = (TaskCollection)obj;
				
				Open(tasks);
				
				MyTasksPreferences.Instance.ActiveFilePath = path;
				UpdateTitleBar();
			}
			catch (Exception e) {
				Say.Error("Failed to open My Tasks file at path " + path + ".",e);
			}
		}
		
		
		/// <summary>
		/// Save the current task collection to the active file path.
		/// </summary>
		public void Save()
		{
			Save(MyTasksPreferences.Instance.ActiveFilePath);			
		}
		
		
		/// <summary>
		/// Save the current task collection to the specified path.
		/// </summary>
		/// <param name="path">The path to save to</param>
		public void Save(string path)
		{
			try {				
				Serialization.Serialize(path,pad.CurrentTaskCollection);
				Dirty = false;
				UpdateTitleBar();
			}
			catch (Exception e) {
				Say.Error("Failed to save tasks file.",e);
			}
		}
		
		
		public void CloseCurrentFile()
		{
			pad.Clear();
			
			// Remember the last open file:
			if (MyTasksPreferences.Instance.ActiveFilePath != null) {
				MyTasksPreferences.Instance.PreviousFilePath = MyTasksPreferences.Instance.ActiveFilePath;
				MyTasksPreferences.Instance.ActiveFilePath = null;
			}
			
			Dirty = false;
			UpdateTitleBar();
		}
		
		
		/// <summary>
		/// Bring up a new file dialog.
		/// </summary>
		private void NewDialog()
		{
			New();
		}
    	
    	
    	/// <summary>
    	/// Bring up an open file dialog.
    	/// </summary>
    	/// <returns>False if the user cancelled the dialog; true otherwise. Note that returning true
    	/// does not indicate that the Open operation was successful.</returns>
    	private bool OpenDialog()
    	{    				
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.ValidateNames = true;
    		openFileDialog.DefaultExt = Filters.MYTASKSFILES_ALL;
    		openFileDialog.Filter = Filters.MYTASKSFILES_ALL;
			openFileDialog.Title = "Select a My Tasks file to open";
			openFileDialog.Multiselect = false;
			openFileDialog.RestoreDirectory = false;
			if (Directory.Exists(MyTasksPreferences.Instance.UserFilesDirectory)) {
				openFileDialog.InitialDirectory = MyTasksPreferences.Instance.UserFilesDirectory;
			}	
			else { // if you give it an invalid InitialDirectory, the dialog will (silently) refuse to load
				openFileDialog.InitialDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
			}
				
  			bool ok = (bool)openFileDialog.ShowDialog();  				
  			if (ok) {
  				Open(openFileDialog.FileName);
  			}
  			return ok;
    	}
    	    	
    	
    	private bool SaveDialog() 
    	{    		
    		try {
	    		// get a filename to save to if there isn't one already:
	    		string filename = MyTasksPreferences.Instance.ActiveFilePath;
	    		if (filename == null || filename == String.Empty) {
	    			return SaveAsDialog();
	    		}
	    		else {
		    		Save(filename);
		    		return true;
	    		}
    		}
	    	catch (Exception e) {
	    		Say.Error("Failed to save tasks file.",e);
	    		return false;
	    	}
    	}  
    	
    	
    	private bool SaveAsDialog()
    	{
    		SaveFileDialog saveFileDialog = new SaveFileDialog();
    		saveFileDialog.AddExtension = true;
    		saveFileDialog.CheckPathExists = true;
    		saveFileDialog.DefaultExt = Filters.MYTASKSFILES_ALL;
    		saveFileDialog.Filter = Filters.MYTASKSFILES_ALL;
  			saveFileDialog.ValidateNames = true;
  			saveFileDialog.Title = "Select location to save tasks file to";
			if (Directory.Exists(MyTasksPreferences.Instance.UserFilesDirectory)) {
				saveFileDialog.InitialDirectory = MyTasksPreferences.Instance.UserFilesDirectory;
			}	
  			
  			bool ok = (bool)saveFileDialog.ShowDialog();  				
  			if (ok) {
  				MyTasksPreferences.Instance.ActiveFilePath = saveFileDialog.FileName;
  				try {
		  			Save();
		  			return true;
  				}
  				catch (Exception e) {
	    			Say.Error("Failed to save tasks file.",e);
	    			MyTasksPreferences.Instance.ActiveFilePath = null;
	    			return false;
  				}
  			}
  			else {
  				return false;
  			}
    	}  
    	    	
    	
    	/// <summary>
    	/// Bring up a dialog to export a file to text.
    	/// </summary>
    	private void ExportDialog()
    	{    	
    	    SaveFileDialog saveFileDialog = new SaveFileDialog();
    		saveFileDialog.AddExtension = true;
    		saveFileDialog.CheckPathExists = true;
    		saveFileDialog.DefaultExt = Filters.MYTASKSFILES_ALL;
    		saveFileDialog.Filter = Filters.MYTASKSFILES_ALL;
  			saveFileDialog.ValidateNames = true;
  			saveFileDialog.Title = "Select location to export to";  	  			
  			saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
			
	  		// try to get the default filename from the filename:
	  		string activeFilePath = MyTasksPreferences.Instance.ActiveFilePath;
	  		string suggestedFileName;
	  		if (activeFilePath == null || activeFilePath == String.Empty) {
	  			suggestedFileName = "Untitled.txt";
	  		}
	  		else {
	  			try {
	  				suggestedFileName = Path.GetFileNameWithoutExtension(activeFilePath) + ".txt";
		  		}
		  		catch (Exception e) {
		  			Say.Debug("Failed to get a suggested filename for Comment Card - " + e);
		  			suggestedFileName = "Untitled.txt";
		  		};
	  		}
	  		saveFileDialog.FileName = Path.Combine(saveFileDialog.InitialDirectory,suggestedFileName);
  			
  			bool ok = (bool)saveFileDialog.ShowDialog();  				
  			if (ok) {
  				string exportFilename = saveFileDialog.FileName;
  				Log.WriteAction(LogAction.exported,"mytasksfile",Path.GetFileName(exportFilename));  			
  				throw new NotImplementedException("Exporting My Tasks files not implemented.");
  			}
    	}
    	
    	
    	/// <summary>
    	/// Bring up a close file dialog.
    	/// </summary>
    	/// <returns></returns>
    	private bool CloseDialog()
    	{
    		if (dirty) {
    			MessageBoxResult result = 
    				MessageBox.Show("Save changes?","Save changes?",MessageBoxButton.YesNoCancel);
    			switch (result) {
    				case MessageBoxResult.Cancel:
    					return false;
    				case MessageBoxResult.Yes:
    					bool cancelled = !SaveDialog(); // user can save, proceed without saving or cancel
    					if (cancelled) {
    						return false;
    					}
    					break;
    				default:
    					break;
    			}    			
    		}
    		
    		CloseCurrentFile();
    		return true;
    	}
    	
    	
    	/// <summary>
    	/// Update the window title to reflect the current filename and whether or not it has changed from disk.
    	/// </summary>
    	private void UpdateTitleBar()
    	{	
    		string newTitle;
    		if (MyTasksPreferences.Instance.ActiveFilePath == null || MyTasksPreferences.Instance.ActiveFilePath == String.Empty) {
    			newTitle = APPLICATION_NAME;
    		}
    		else {
    			string filename = Path.GetFileName(MyTasksPreferences.Instance.ActiveFilePath);
    			if (Dirty) {
    				newTitle = filename + "* - " + APPLICATION_NAME;
    			}
    			else {
    				newTitle = filename + " - " + APPLICATION_NAME;
    			}
    		}
    		
    		if (Title != newTitle) {
    			Title = newTitle;
    		}
    	}
    	
    	
    	/// <summary>
    	/// Add a task to the current task collection. Using this method ensures that the necessary
    	/// event handlers are added to the task - do not add directly to the collection.
    	/// </summary>
    	/// <param name="task"></param>
    	public void AddTask(Task task)
    	{
    		task.PropertyChanged += MakeDirty;
    		pad.Add(task);
    	}
    	
    	
    	/// <summary>
    	/// Indicate that the current task collection has been changed in some way.
    	/// </summary>
    	public void MakeDirty()
    	{
    		if (!Dirty) {
    			Dirty = true;	
    		}
    	}
		
		#endregion
		
		#region Event handlers
		
		/// <summary>
		/// Tidy up before the application closes.
		/// </summary>
		private void HandleApplicationClosing(object sender, CancelEventArgs e)
		{
			if (!CloseDialog()) { // give the user a chance to cancel
				e.Cancel = true;
				return;
			}
			
			// Automatically save preferences on closing:
    		try {
	    		// Serialize the user's preferences:
	    		Serialization.Serialize(MyTasksPreferences.DefaultPreferencesPath,
	    		                        MyTasksPreferences.Instance);
    		}
    		catch (Exception ex) {
    			Say.Error("Something went wrong when trying to save your preferences - the choices " +
    				      "you have made may not have been saved.",ex);
    		}
		}
		

		/// <summary>
		/// Respond to changes in user preferences.
		/// </summary>
		private void HandlePreferencesChanged(object sender, PropertyChangedEventArgs e)
		{
			
		}
		

		/// <summary>
		/// Update the title bar when appropriate due to a change made.
		/// </summary>
		private void UpdateTitleBar(object sender, EventArgs e)
		{
			UpdateTitleBar();			
		}


		/// <summary>
		/// Set the Dirty flag to true when any change is made.
		/// </summary>
		private void MakeDirty(object sender, EventArgs e)
		{
			if (!Dirty) {
				Dirty = true;	
			}					
		}
    	    	
    	
    	private void OnClick_New(object sender, EventArgs e)
    	{
    		NewDialog();
    	}
    	    	
    	
    	private void OnClick_Open(object sender, EventArgs e)
    	{
    		OpenDialog();
    	}
    	    	
    	
    	private void OnClick_Save(object sender, EventArgs e)
    	{
    		SaveDialog();
    	}
    	    	
    	
    	private void OnClick_SaveAs(object sender, EventArgs e)
    	{
    		SaveAsDialog();
    	}
    	    	
    	
    	private void OnClick_Close(object sender, EventArgs e)
    	{
    		CloseDialog();
    	}
    	    	
    	
    	private void OnClick_NewTask(object sender, EventArgs e)
    	{
    		//NewTaskDialog();
    		Task task = new Task();
    		AddTask(task);
    	}
    	
    	
    	private void OnClick_ListTags(object sender, EventArgs e)
    	{
    		foreach (Task task in pad.CurrentTaskCollection) {
    			foreach (string tag in task.Tags) {
    				Title += " " + tag;
    			}
    		}
    	}
		
		#endregion
	
		
	}
}