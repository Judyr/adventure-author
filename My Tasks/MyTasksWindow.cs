using System;
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

namespace AdventureAuthor.Tasks
{
	/// <summary>
	/// The main user interface window for the My Tasks Application.
	/// </summary>
	public partial class MyTasksWindow : Window
	{
		#region Properties and fields
		
		public TaskCollection Tasks {
			get { return (TaskCollection)DataContext; }
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
			
			// If you can find the last file that was opened, attempt to open it. Otherwise, open a blank task list.
			string previousFilePath = MyTasksPreferences.Instance.PreviousFilePath;
			if (previousFilePath != null && File.Exists(previousFilePath)) {
				Open(previousFilePath);
			}
			else {
				New();
			}
			
			Closing += AutomaticallySaveOnClosing;
		}
		
		#endregion
		
		#region Methods
		
		/// <summary>
		/// Open a new task collection.
		/// </summary>
		public void New()
		{
			TaskCollection tasks = new TaskCollection();
			tasks.Tasks.Add(new Task("This is a task","gamebuilding"));
			tasks.Tasks.Add(new Task("This is a good task",new List<string>{"gamebuilding","bugs"}));
			Open(tasks);
		}
		
		
		/// <summary>
		/// Open a task collection.
		/// </summary>
		/// <param name="tasks">The collection of tasks to open</param>
		public void Open(TaskCollection taskCollection)
		{
			DataContext = taskCollection;
			MyTasksPreferences.Instance.ActiveFilePath = null;
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
			}
			catch (Exception e) {
				Say.Error("Failed to open My Tasks file at path " + path + ".",e);
			}
		}
		
		
		public void CloseTaskList()
		{
			// TODO: before saving, ensure that the value of a textbox that the user is
			// currently editing is bound back to the source object - this seems to 
			// get missed by default.
			
			// Automatically save the current Task List on closing:
			if (MyTasksPreferences.Instance.ActiveFilePath == null) {
				// TODO: Force the user to save, cancel, or lose their work. 
				// Remember in doing so to update ActiveFilePath, so that
				// PreviousFilePath can get its new value further down.
				throw new InvalidOperationException("No filename to save to.");
			}
			Serialization.Serialize(MyTasksPreferences.Instance.ActiveFilePath,Tasks);
			
			MyTasksPreferences.Instance.PreviousFilePath = MyTasksPreferences.Instance.ActiveFilePath;
			MyTasksPreferences.Instance.ActiveFilePath = null;
			
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
		
		#endregion
		
		#region Event handlers
		
		private void AutomaticallySaveOnClosing(object sender, CancelEventArgs e)
		{
			CloseTaskList();
		}
		
		#endregion
	}
}