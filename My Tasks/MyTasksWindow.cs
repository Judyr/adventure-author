using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Serialization;
using winforms = System.Windows.Forms;
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
		public const string DEFAULT_TASK_DESCRIPTION = "Enter your task here...";
		public const string MYSUGGESTIONSINFO_GENERAL = "Click here to ask Adventure Author to suggest tasks " +
														"based on your module.\n\n" +
														"You can only do this when you have the toolset running " +
														"and your module open.";
		public const string MYSUGGESTIONSINFO_NOMODULEOPEN = "You need to open a module in the toolset before you " +
															 "can get suggestions about how to improve it.\n\n" +
															 "Once you've opened your module, click here again " +
															 "to get Adventure Author to suggest tasks for you.";
		public const string MYSUGGESTIONSINFO_NOSUGGESTIONS = "your module didn't seem to have any obvious bugs, so " +
															  "Adventure Author has no suggestions for you at the moment.\n\n" +
															  "As you continue to work on your module, Adventure Author may " +
															  "come up with some suggestions about how to improve it. You can " +
															  "click here every so often to find out what it has to say.";
		public const string MYSUGGESTIONSINFO_WAITING = "Waiting for toolset...";
		
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
    	
    	
    	/// <summary>
    	/// The list of all tags available to the user on the main interface.
    	/// </summary>
    	/// <remarks>Drawn from the preferences file (or from default values if no
    	/// preferences file found) together with any tags found in the current tasks file.</remarks>
    	private ObservableCollection<string> allTags = new ObservableCollection<string>();
		public ObservableCollection<string> AllTags {
			get { return allTags; }
			set { allTags = value; }
		}
    	
    	
    	/// <summary>
    	/// The system tray icon for this application.
    	/// </summary>
    	private winforms.NotifyIcon trayIcon;
    	
    			
		/// <summary>
		/// The collection of suggested tasks.
		/// </summary>
		public TaskCollection SuggestedTasks {
			get { return (TaskCollection)mySuggestionsGrid.DataContext; }
			set { mySuggestionsGrid.DataContext = value; }
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
			
			SuggestedTasks = new TaskCollection();
			mySuggestionsInformationTextBlock.Text = MYSUGGESTIONSINFO_GENERAL;
	                		
			try {
				Tools.EnsureDirectoryExists(MyTasksPreferences.LocalAppDataDirectory);
				Tools.EnsureDirectoryExists(MyTasksPreferences.Instance.UserFilesDirectory);
			}
			catch (Exception e) {
	    		Say.Debug("Failed to create directory for user:\n"+e);
			}  
			
			
			// Logging:
			LogWriter.StartRecording("mytasks");					
			Loaded += delegate { Log.WriteAction(LogAction.launched,"mytasks"); };
			
			
			// Set up event handlers:
			pad.AddTaskButton.Click += AddAndSelectNewTask;
			Closing += SavePreferencesWhenClosing;
			StateChanged += HideWhenMinimised;
			MyTasksPreferences.Instance.PropertyChanged += HandlePreferencesChanged;
			Changed += UpdateTitleBar;
			
			
			// If you can find the last file that was opened, attempt to open it. Otherwise, open a blank task list.
			string previousFilePath = MyTasksPreferences.Instance.PreviousFilePath;
			if (previousFilePath != null && File.Exists(previousFilePath)) {
				Open(previousFilePath);
				Log.WriteAction(LogAction.opened,"taskcollection","'"+Path.GetFileName(previousFilePath)+"'. Opened most recent file automatically upon loading.");
			}
			else {
				New();
				//Log.WriteAction(LogAction.added,"taskcollection","Created automatically upon launching application.");
			}	
							
			// Dispose the system tray icon when you're done:
			Closed += delegate { 
				if (trayIcon != null) {
					trayIcon.Visible = false;
					trayIcon.Dispose();
				}
				Log.WriteAction(LogAction.exited,"mytasks");
				LogWriter.StopRecording();
			};
			
			// Launch the application in the system tray:
			Loaded += new RoutedEventHandler(LaunchInSystemTray);
			
			// And listen for messages from the NWN2Toolset:
			Loaded += new RoutedEventHandler(StartListeningForMessages);			
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
			// In case you add a generated task which is already labelled with a new label:
			tasks.CollectionChanged += UpdateTagCollection;
			
			// Open the task collection in the TaskPad:
			pad.Open(tasks);
			
			MyTasksPreferences.Instance.ActiveFilePath = null;
			Dirty = false;
			pad.ClearAllFilters();
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
			try {
				if (pad != null) {				
					pad.Clear();
					pad.ClearAllFilters();
				}
				
				// Remember the last open file:
				if (MyTasksPreferences.Instance.ActiveFilePath != null) {
					MyTasksPreferences.Instance.PreviousFilePath = MyTasksPreferences.Instance.ActiveFilePath;
					MyTasksPreferences.Instance.ActiveFilePath = null;
				}
				
				Dirty = false;
				UpdateTitleBar();
				UpdateTagCollection();
			}
			catch (Exception e) {
				Say.Error("Error on closing the current file.",e);
			}
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
  				Log.WriteAction(LogAction.opened,"taskcollection","'"+Path.GetFileName(openFileDialog.FileName)+
  				                "'. Opened via File->Open.");
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
		    		Log.WriteAction(LogAction.saved,"taskcollection","'" + Path.GetFileName(MyTasksPreferences.Instance.ActiveFilePath) + "'");
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
		  			Log.WriteAction(LogAction.saved,"taskcollection","Saved as '" + Path.GetFileName(MyTasksPreferences.Instance.ActiveFilePath) + "'");
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
		  			Say.Debug("Failed to get a suggested filename for My Tasks file - " + e);
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
    	/// <returns>False if the user cancels the process; true otherwise</returns>
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
    		    		
			Log.WriteAction(LogAction.closed,"taskcollection",MyTasksPreferences.Instance.ActiveFilePath);
    		New();
			//Log.WriteAction(LogAction.added,"taskcollection","Created automatically when previous file was closed.");
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
    		string selectedTag;
    		if (pad != null && pad.tagFilterComboBox != null && pad.tagFilterComboBox.SelectedItem != null) {
    			selectedTag = (string)pad.tagFilterComboBox.SelectedItem;
    		}
    		else {
    			selectedTag = null;
    		}
    		
    		allTags.Clear();
			foreach (string tag in MyTasksPreferences.Instance.PreDefinedTags) {
				if (!allTags.Contains(tag)) {
					allTags.Add(tag);
				}
			}
			if (pad != null && pad.Tasks != null) {
				foreach (Task task in pad.Tasks) {
    				if (task.Tags != null) {
						foreach (string tag in task.Tags) {
							if (!allTags.Contains(tag)) {
								allTags.Add(tag);
							}
						}
    				}
				}
			}
    		
    		if (allTags.Contains(selectedTag)) {
    			pad.tagFilterComboBox.SelectedItem = selectedTag;
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
    	    	
    	    	
    	/// <summary>
    	/// Hide the main window when the user minimises it.
    	/// </summary>
    	private void HideWhenMinimised(object sender, EventArgs e)
    	{
    		if (WindowState == WindowState.Minimized) {
				System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
				                                                          new HideWindowDelegate(HideWindow));
    		}
    	}	
		
		#endregion
		
		#region Event handlers		

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
			Log.WriteAction(LogAction.added,"taskcollection");
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
    	
    	
    	private void OnClick_Exit(object sender, EventArgs e)
    	{
    		Close();
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
		
		
		/// <summary>
		/// When the user hits return, take focus away from the task description
		/// editable text box, making it appear uneditable again.
		/// </summary>
		private void FinishEditingWhenUserHitsReturn(object sender, System.Windows.Input.KeyEventArgs e)
		{
			EditableTextBox etb = (EditableTextBox)sender;
			if (e.Key == System.Windows.Input.Key.Return) {
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
					Log.WriteAction(LogAction.added,"label","'" + tag + "' to " + task);
				}
			}
		}
    	
		
    	private void AddAndSelectNewTask(object sender, EventArgs e)
    	{
    		Task task = new Task(DEFAULT_TASK_DESCRIPTION);
    		AddTask(task,true,true);
    		Log.WriteAction(LogAction.added,"task",task.ToString());
    	}
    	
    	
    	private void AddTask(Task task, bool afterSelectedTask, bool highlightDescription)
    	{
    		pad.Add(task,afterSelectedTask);
    		
	    	// Select the task in the list:
	    	pad.taskListBox.SelectedItem = task;
	    	
    		if (highlightDescription) {	    		
	    		// Allow the user to start typing the task description directly:
	    		taskDescriptionBox.Focus();
	    		taskDescriptionBox.SelectAll();
    		}
    		
    		pad.taskListBox.ScrollIntoView(task);
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
						
						Log.WriteAction(LogAction.deleted,"label","'" + deletingTag + "' from " + task);
						
						if (filteredTag != null && !pad.tagFilterComboBox.Items.Contains(filteredTag)) {
							// If the tag we removed was the last instance of the filtered tag,
							// the list will no longer be filtered by that tag - but, confusingly,
							// the previously selected task will still be selected. Clear it:
							pad.taskListBox.SelectedItem = null;
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
					task.Uncomplete();
					Log.WriteAction(LogAction.uncompleted,"task",task.ToString());
				}
				else {
					if (task.State != TaskState.NotCompleted) {
						System.Diagnostics.Debug.WriteLine("Task state was '" + task.State + "'? How'd that happen?");
					}
					task.Complete();
					Log.WriteAction(LogAction.completed,"task",task.ToString());
				}
				
				pad.RefreshAllFilters();
			}
		}
		
		
		private void LogThatTextHasChanged(object sender, TextEditedEventArgs e)
		{
			EditableTextBox etb = (EditableTextBox)sender;
			Task task = etb.DataContext as Task;
			if (task != null) {
				Log.WriteAction(LogAction.edited,"task",((Task)((EditableTextBox)sender).DataContext).ToString());
			}			
		}
		
		#endregion	
		
		#region Communication    	
		
    	private void StartListeningForMessages(object sender, RoutedEventArgs e)
    	{   
    		ThreadStart threadStart = new ThreadStart(ListenForMessagesFromNWN2Toolset);
    		Thread listenForToolsetThread = new Thread(threadStart);
    		listenForToolsetThread.Name = "Listen for messages thread";
    		listenForToolsetThread.IsBackground = true; // will not prevent the application from closing down
			listenForToolsetThread.Priority = ThreadPriority.BelowNormal;
    		listenForToolsetThread.Start();
    	}
    	
    	
    	public delegate void AddSuggestedTaskDelegate(Task task);
    	public void AddSuggestedTask(Task task)
    	{
    		if (SuggestedTasks != null && !SuggestedTasks.Contains(task)) {
    			SuggestedTasks.Add(task);
    		}
    	}
    	
    	
    	public delegate void AddSuggestedTasksDelegate(List<Task> tasks);
    	public void AddSuggestedTasks(List<Task> tasks)
    	{
    		foreach (Task task in tasks) {
    			AddSuggestedTask(task);
    		}
    	}
    	
    	
    	public delegate void SetSuggestionsPanelMessageDelegate(string message);    	
    	public void SetSuggestionsPanelMessage(string message)
    	{
    		mySuggestionsInformationTextBlock.Text = message;
    	}
    	
    	
    	private void ListenForMessagesFromNWN2Toolset()
    	{
    		using (NamedPipeClientStream client = new NamedPipeClientStream(".",
    		                                                                PipeNames.NWN2TOMYTASKS,
    		                                                                PipeDirection.In))
    		{
    			try {
    				client.Connect();
	    			using (StreamReader reader = new StreamReader(client))
	    			{
	    				string message;
	    				while ((message = reader.ReadToEnd()) != null) {
	    					if (message.Length == 0) {
	    						break;	
	    					}
	    					else if (message == Messages.NOMODULEOPEN) {
	    						Dispatcher.BeginInvoke(DispatcherPriority.Normal,
	    						                       new SetSuggestionsPanelMessageDelegate(SetSuggestionsPanelMessage),
	    						                       MYSUGGESTIONSINFO_NOMODULEOPEN);
	    					}
	    					else {
		    					try {
			    					//Say.Information("Received:\n" + message + "\n(" + message.Length + " chars.)");	    						
			    					
		    						XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Task>));
			    					StringReader stringReader = new StringReader(message);
			    					List<Task> tasks = (List<Task>)xmlSerializer.Deserialize(stringReader);
			    					
			    					if (tasks.Count == 0) {
			    						string noSuggestionsMsg = "When checked at " + DateTime.Now.ToShortTimeString() + ", "
			    							+ MYSUGGESTIONSINFO_NOSUGGESTIONS;
			    						Dispatcher.BeginInvoke(DispatcherPriority.Normal,
			    						                       new SetSuggestionsPanelMessageDelegate(SetSuggestionsPanelMessage),
			    						                       noSuggestionsMsg);
			    					}
			    					else {
				    					Dispatcher.BeginInvoke(DispatcherPriority.Normal,
				    					                       new AddSuggestedTasksDelegate(AddSuggestedTasks),
				    					                       tasks);
			    						// This won't be visible immediately, but should be reset to the 'normal'
			    						// message whenever the 'no module/suggestions' messages don't apply:
			    						Dispatcher.BeginInvoke(DispatcherPriority.Normal,
			    						                       new SetSuggestionsPanelMessageDelegate(SetSuggestionsPanelMessage),
			    						                       MYSUGGESTIONSINFO_GENERAL);
			    					}
		    					}
		    					catch (Exception e) {
		    						Say.Error(e);
		    					}	
	    					}
	    				}
	    			}
    			}
    			catch (Exception e) {
    				Say.Error("Something went wrong when listening for messages from the NWN2 Toolset.",e);
    			}    			
			}    		
			ListenForMessagesFromNWN2Toolset();
    	}
    		
    	    	
		private void ThreadedSendMessage(string message)
		{			
			ParameterizedThreadStart threadStart = new ParameterizedThreadStart(SendMessage);
			Thread sendMsgThread = new Thread(threadStart);
			sendMsgThread.Name = "Send message thread";
			sendMsgThread.IsBackground = true;
			sendMsgThread.Priority = ThreadPriority.BelowNormal;
			mySuggestionsInformationTextBlock.Text = MYSUGGESTIONSINFO_WAITING;
			sendMsgThread.Start(Messages.REQUESTALLTASKS);
		}
		
		
    	private void SendMessage(object obj)
    	{    		
    		try {	    			 
    			string message = (string)obj;
		    	using (NamedPipeServerStream server = new NamedPipeServerStream(PipeNames.MYTASKSTONWN2,
    			                                          PipeDirection.Out,
    			                                          1))
		    	{	    			
		    		server.WaitForConnection();
		    		using (StreamWriter writer = new StreamWriter(server))
		    		{
		    			writer.WriteLine(message);
		    			writer.Flush();
		    		}
		    	}
    		}
    		catch (IOException) {
    			System.Diagnostics.Debug.WriteLine("Tried to send a message to the toolset, but the pipe was busy.");
    		}
    		catch (Exception e) {
    			Say.Error(e);
    		}
    	}
    	
    	
    	private void IgnoreSuggestedTask(object sender, RoutedEventArgs e)
    	{
    		Button button = (Button)sender;
    		Task task = (Task)button.DataContext;
    		if (SuggestedTasks.Contains(task)) {
    			SuggestedTasks.Remove(task);
    		}
    		Log.WriteAction(LogAction.ignored,"task",task.ToString());
    	}
    	
    	
    	private void AddSuggestedTask(object sender, RoutedEventArgs e)
    	{
    		Button button = (Button)sender;
    		Task task = (Task)button.DataContext;
    		if (SuggestedTasks.Contains(task)) {
    			SuggestedTasks.Remove(task);
    		}
    		if (!pad.Tasks.Contains(task)) {
    			AddTask(task,false,false);
    			Log.WriteAction(LogAction.added,"task",task.ToString());
    		}
    	}
    	
    	
    	private void UserClickedOnMySuggestionsMessage(object sender, RoutedEventArgs e)
    	{
    		SuggestedTasks.Clear();
	    	ThreadedSendMessage(Messages.REQUESTALLTASKS);
	    	Log.WriteMessage("User clicked to request suggested tasks from the toolset.");
    	}
    	
    	
    	private void AddAllSuggestedTasks(object sender, RoutedEventArgs e)
    	{
    		if (SuggestedTasks.Count > 1) {
    			MessageBoxResult result = MessageBox.Show("Add all " + SuggestedTasks.Count + " suggested tasks to your task list?",
    			                                          "Add all?",
    			                                          MessageBoxButton.OKCancel,
    			                                          MessageBoxImage.Question,
    			                                          MessageBoxResult.Cancel,
    			                                          MessageBoxOptions.None);
    			if (result == MessageBoxResult.Cancel) {
    				return;	
    			}
    		}
    		
    		Log.WriteMessage("User clicked 'Add all' to add all suggested tasks.");
    		foreach (Task task in SuggestedTasks) {
    			AddTask(task,false,false);
    			Log.WriteAction(LogAction.added,"task",task.ToString());
    		}
    		SuggestedTasks.Clear();
    	}
    	
    	
    	private void IgnoreAllSuggestedTasks(object sender, RoutedEventArgs e)
    	{
    		SuggestedTasks.Clear();
    		Log.WriteMessage("User clicked 'Ignore all' to ignore all suggested tasks.");
    	}
    	
    	
    	private void ChangeWhetherSuggestedTasksListBoxIsVisible(object sender, RoutedEventArgs e)
    	{
    		SuggestedTasksVisible = !SuggestedTasksVisible;
    		if (SuggestedTasksVisible) {
    			Log.WriteMessage("Showed My Suggestions panel.");
    		}
    		else {
    			Log.WriteMessage("Hid My Suggestions panel.");
    		}
    	}
    	
    	
    	public bool SuggestedTasksVisible {
    		get { return (bool)this.GetValue(SuggestedTasksVisibleProperty); }
    		set { this.SetValue(SuggestedTasksVisibleProperty,value); }
    	}
    	
    	
    	public static readonly DependencyProperty SuggestedTasksVisibleProperty
    		= DependencyProperty.Register("SuggestedTasksVisible",
    		                              typeof(bool),
    		                              typeof(MyTasksWindow),
    		                              new PropertyMetadata(true));
    		                              
		#endregion
	}
}