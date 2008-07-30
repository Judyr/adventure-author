using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Tasks
{
	/// <summary>
	/// The main user interface window for the My Tasks Application.
	/// </summary>
	public partial class MyTasksWindow : Window
	{
		#region Constants
		
		private const string DEFAULTPATH = @"C:\Users\Kirn\Desktop\tasks.tsk";
		
		#endregion
		
		#region Constructors
		
		/// <summary>
		/// Create the main user interface window for the My Tasks application.
		/// </summary>
		public MyTasksWindow()
		{
			InitializeComponent();
			Open(DEFAULTPATH);
		}
		
		#endregion
		
		#region Methods
		
		/// <summary>
		/// Open a task collection.
		/// </summary>
		/// <param name="tasks">The collection of tasks to open</param>
		public void Open(TaskCollection tasks)
		{
			DataContext = tasks;
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
			}
			catch (Exception e) {
				Say.Error("Failed to open My Tasks file at path " + path + ".",e);
			}
		}
		
		
		/// <summary>
		/// Generate a sample task collection file at the path specified.
		/// </summary>
		/// <param name="path">The path to create the task collection file at</param>
		private void GenerateSampleTasksCollection(string path)
		{
			TaskCollection tasks = new TaskCollection();
			tasks.Tasks.Add(new Task("Finish reading Freakonomics",
			                         "Errands",
			                         TaskOrigin.UserCreated.ToString(),
			                         TaskState.InProgress));
			tasks.Tasks.Add(new Task("Take Freakonomics back to the library",
			                         "Errands",
			                         TaskOrigin.UserCreated.ToString()));
			tasks.Tasks.Add(new Task("Email Other Judy about installation package",
			                         "Work",
			                         TaskOrigin.Imported.ToString()));
			Serialization.Serialize(path,tasks);
		}
		
		#endregion
	}
}