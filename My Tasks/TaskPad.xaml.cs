using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Tasks
{
	/// <summary>
	/// A control resembling a notepad, which is used to display a list of tasks ('Things to do').
	/// </summary>
    public partial class TaskPad : UserControl
    {
    	#region Properties and fields
    			
		/// <summary>
		/// The task collection that is currently open.
		/// </summary>
		public TaskCollection CurrentTaskCollection {
			get { return (TaskCollection)DataContext; }
			set { DataContext = value; }
		}   	
    	
    	#endregion
    	
    	#region Constructors
    	
		public TaskPad()
		{
			InitializeComponent();
		}
		
		#endregion
		
		#region Methods
			
		/// <summary>
		/// Open a task collection.
		/// </summary>
		public void Open(TaskCollection tasks)
		{
			CurrentTaskCollection = tasks;
		}
		
		
		/// <summary>
		/// Clear the current set of tasks.
		/// </summary>
		public void Clear()
		{
			CurrentTaskCollection = null;
		}
		
		
		/// <summary>
		/// Add a task to the current collection.
		/// </summary>
		/// <param name="task">The task to add</param>
		public void Add(Task task)
		{
			if (CurrentTaskCollection == null) {
				throw new InvalidOperationException("No task collection is currently open.");
			}
			CurrentTaskCollection.Add(task);
		}
		
		
		/// <summary>
		/// Delete a task from the current collection.
		/// </summary>
		/// <param name="task">The task to delete</param>
		public void Delete(Task task)
		{
			if (CurrentTaskCollection == null) {
				throw new InvalidOperationException("No task collection is currently open.");
			}
			CurrentTaskCollection.Remove(task);
		}
		
		#endregion
		
		#region Event handlers
		
		/// <summary>
		/// Delete a user-specified task after receiving confirmation.
		/// </summary>
		private void OnClick_DeleteTask(object sender, RoutedEventArgs e)
		{
			Task task = taskListBox.SelectedItem as Task;
			if (task == null) {
				throw new InvalidOperationException("No task is currently selected.");
			}
			
			MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this task?",
										              "Delete task?",
										              MessageBoxButton.OKCancel,
										              MessageBoxImage.Question,
										              MessageBoxResult.OK);
			if (result == MessageBoxResult.OK) {
				Delete(task);
			}			
		}
		
		
		private void OnClick_EditTask(object sender, RoutedEventArgs e)
		{
		}
		
		
		private void OnClick_ChangeCategoryOfTask(object sender, RoutedEventArgs e)
		{
			Say.Information("Change category");
		}
		
		
		/// <summary>
		/// Move a user-specified task up in the list.
		/// </summary>
		private void OnClick_MoveUp(object sender, RoutedEventArgs e)
		{
			Task task = taskListBox.SelectedItem as Task;
			if (task == null) {
				throw new InvalidOperationException("No task is currently selected.");
			}
			
			int index = CurrentTaskCollection.IndexOf(task);
			if (index == -1) {
				throw new InvalidOperationException("The selected task was not found in the current task collection.");
			}
			int newIndex = index - 1;
			if (newIndex >= 0) {
				CurrentTaskCollection.Move(index,newIndex);
			}			
		}
		
		
		/// <summary>
		/// Move a user-specified task down in the list.
		/// </summary>
		private void OnClick_MoveDown(object sender, RoutedEventArgs e)
		{
			Task task = taskListBox.SelectedItem as Task;
			if (task == null) {
				throw new InvalidOperationException("No task is currently selected.");
			}
			
			int index = CurrentTaskCollection.IndexOf(task);
			if (index == -1) {
				throw new InvalidOperationException("The selected task was not found in the current task collection.");
			}
			int newIndex = index + 1;
			int maxIndex = CurrentTaskCollection.Count - 1;
			if (newIndex <= maxIndex) {
				CurrentTaskCollection.Move(index,newIndex);
			}		
		}
		
		#endregion
    }
}