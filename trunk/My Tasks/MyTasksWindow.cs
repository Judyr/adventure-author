using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

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
    	
    	
    	private ObservableCollection<string> allTags = new ObservableCollection<string>();
		public ObservableCollection<string> AllTags {
			get { return allTags; }
			set { allTags = value; }
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
			pad.AddTaskButton.Click += AddAndSelectNewTask;
			Closing += HandleApplicationClosing;
			MyTasksPreferences.Instance.PropertyChanged += HandlePreferencesChanged;
			Changed += UpdateTitleBar;
			
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
			CloseFile();
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
				task.Tags.CollectionChanged += UpdateTagCollection;
				task.Tags.CollectionChanged += MakeDirty;
			}
			tasks.CollectionChanged += MakeDirtyWhenTaskCollectionChanges;
			
			// Open the task collection in the TaskPad:
			pad.Open(tasks);
			
			MyTasksPreferences.Instance.ActiveFilePath = null;
			Dirty = false;
			UpdateTagCollection();
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
					Say.Debug("Something unusual happened to the Tasks collection: (" + e.Action + ") " + e.ToString());
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
				Serialization.Serialize(path,pad.Tasks);
				Dirty = false;
				UpdateTitleBar();
			}
			catch (Exception e) {
				Say.Error("Failed to save tasks file.",e);
			}
		}
				
		
		private void CloseFile()
		{
			pad.Clear();
			
			// Remember the last open file:
			if (MyTasksPreferences.Instance.ActiveFilePath != null) {
				MyTasksPreferences.Instance.PreviousFilePath = MyTasksPreferences.Instance.ActiveFilePath;
				MyTasksPreferences.Instance.ActiveFilePath = null;
			}
			
			Dirty = false;
			UpdateTitleBar();
			UpdateTagCollection();
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
    		
    		New();
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
    	/// Collect the set of available tags from the pre-defined set saved in
		/// preferences, and from any tags in the currently open task collection:
    	/// </summary>
    	private void UpdateTagCollection(object sender, NotifyCollectionChangedEventArgs e)
    	{
    		UpdateTagCollection();
    	}
    	
    	
    	/// <summary>
    	/// Collect the set of available tags from the pre-defined set saved in
		/// preferences, and from any tags in the currently open task collection:
    	/// </summary>
    	private void UpdateTagCollection()
    	{
    		allTags.Clear();
			foreach (string tag in MyTasksPreferences.Instance.PreDefinedTags) {
				if (!allTags.Contains(tag)) {
					allTags.Add(tag);
				}
			}
			if (pad.Tasks != null) {
				foreach (Task task in pad.Tasks) {
					foreach (string tag in task.Tags) {
						if (!allTags.Contains(tag)) {
							allTags.Add(tag);
						}
					}
				}
			}
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
    		New();
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
		
		
		private void MakeEditable(object sender, RoutedEventArgs e)
		{
			EditableTextBox editableTextBox = (EditableTextBox)sender;
			editableTextBox.IsEditable = true;
		}
		
		
		private void MakeUneditable(object sender, RoutedEventArgs e)
		{
			EditableTextBox editableTextBox = (EditableTextBox)sender;
			editableTextBox.IsEditable = false;
		}
		
		
		private void FinishEditingWhenUserHitsReturn(object sender, KeyEventArgs e)
		{
			EditableTextBox etb = (EditableTextBox)sender;
			if (e.Key == Key.Return) {
				etb.IsEditable = false;
				TagSelectionComboBox.Focus();
			}
		}  	 
		
		
		private void AddSelectedTagToCurrentTask(object sender, RoutedEventArgs e)
		{
			if (pad.taskListBox.SelectedItem == null) {
				throw new InvalidOperationException("No task is currently selected.");
			}
			
			if (TagSelectionComboBox.SelectedItem != null) {
				Task task = (Task)pad.taskListBox.SelectedItem;
				string tag = (string)TagSelectionComboBox.SelectedItem;
				if (!task.Tags.Contains(tag)) {
					task.Tags.Add(tag);
				}
			}
		}
    	
		
    	private void AddAndSelectNewTask(object sender, EventArgs e)
    	{
    		pad.ClearAllFilters(); //TODO: This is a bit clumsy
    		Task task = new Task("Enter your task here...");
    		pad.AddAfterSelectedTask(task);
    		pad.taskListBox.SelectedItem = task;
    		taskDescriptionBox.Focus();
    		taskDescriptionBox.SelectAll();
    	}
    	
		
    	/// <summary>
    	/// Remove a tag when its delete control is clicked.
    	/// </summary>
		private void RemoveTagWhenDeleteButtonIsClicked(object sender, RoutedEventArgs e)
		{
			if (e.OriginalSource is Button) {
				Button button = (Button)e.OriginalSource;
				if (button.DataContext is string) {
					// Check that the user wants to delete the tag:
					string deletingTag = (string)button.DataContext;
					MessageBoxResult result = MessageBox.Show("Remove '" + deletingTag + "' label from " +
					                                           "this task?",
					                                           "Remove label?",
					                                           MessageBoxButton.OKCancel,
					                                           MessageBoxImage.Question,
					                                           MessageBoxResult.OK);
					
					if (result == MessageBoxResult.OK) {
						// Keep track of the tag we are filtering by, if there is one:
						string filteredTag = null;
						if (((bool)pad.activateTagFilterCheckBox.IsChecked) && pad.tagFilterComboBox.SelectedItem != null) {
							filteredTag = (string)pad.tagFilterComboBox.SelectedItem;
						}
						
						// Delete the clicked tag:
						Task task = (Task)pad.taskListBox.SelectedItem;
						if (task.Tags.Contains(deletingTag)) {
							task.Tags.Remove(deletingTag);
						}
						
						// Removing a tag will cause the AllTags list to refresh, which
						// will clear the filter - if a tag was selected, select it again:
						if (filteredTag != null) {
							if (pad.tagFilterComboBox.Items.Contains(filteredTag)) {
								pad.tagFilterComboBox.SelectedItem = filteredTag;
							}
							else {
								// If the tag we removed was the last instance of the filtered tag,
								// the list will no longer be filtered by that tag - but, confusingly,
								// the previously selected task will still be selected. Clear it:
								pad.taskListBox.SelectedItem = null;
							}
						}					
					}
				}
			}
		}
		
		
		/// <summary>
		/// When the complete/uncomplete task button is clicked,
		/// change the task's status and update the filters that
		/// show/hide tasks based on that status.
		/// </summary>
		private void ChangeCompletionStatusOfTask(object sender, RoutedEventArgs e)
		{
			Task task = (Task)pad.taskListBox.SelectedItem;
			
			MessageBoxResult result;
			if (task.State == TaskState.Completed) {
				result = MessageBox.Show("Mark this task as 'uncompleted' so you can work on it some more?",
				                                          "Uncomplete task?",
				                                          MessageBoxButton.OKCancel);
			}
			else {
				result = MessageBox.Show("Mark this task as 'completed' and cross it out?",
				                                          "Complete task?",
				                                          MessageBoxButton.OKCancel);
			}
			
			if (result == MessageBoxResult.OK) {
				if (task.State == TaskState.Completed) {
					task.State = TaskState.NotCompleted;	
				}
				else {
					if (task.State != TaskState.NotCompleted) {
						System.Diagnostics.Debug.WriteLine("Task state was '" + task.State + "'? How'd that happen?");
					}
					task.State = TaskState.Completed;
				}
				
				pad.RefreshTaskCompletedFilter();
			}
		}
		
		#endregion
	}
}