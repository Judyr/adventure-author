using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
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
			
		private void HandleTaskButtonClicks(object sender, RoutedEventArgs e)
		{
			if (e.OriginalSource is Button) {
				Button button = (Button)e.OriginalSource;
				if (button.DataContext is Task) {
					Task task = (Task)button.DataContext;				
					if (button.Name == "DeleteTaskButton") {					
						DeleteTaskDialog(task);
					}
					else if (button.Name == "AddTagButton") {
						AddTagDialog(task);
					}
					else if (button.Name == "MoveTaskUpButton") {
						MoveTaskUp(task);
					}
					else if (button.Name == "MoveTaskDownButton") {
						MoveTaskDown(task);
					}
				}
			}			
		}
		
		
		private void DeleteTaskDialog(Task task)
		{
			MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this task?",
										              "Delete task?",
										              MessageBoxButton.OKCancel,
										              MessageBoxImage.Question,
										              MessageBoxResult.OK);
			if (result == MessageBoxResult.OK) {
				Delete(task);
			}	
		}
		
		
		private void AddTagDialog(Task task)
		{
			task.Tags.Add(String.Empty);
		}
		
		
		private void MoveTaskUp(Task task)
		{
			int index = CurrentTaskCollection.IndexOf(task);
			if (index == -1) {
				throw new InvalidOperationException("The selected task was not found in the current task collection.");
			}
			int newIndex = index - 1;
			if (newIndex >= 0) {
				CurrentTaskCollection.Move(index,newIndex);
			}	
		}
		
		
		private void MoveTaskDown(Task task)
		{
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
		
		
		private void MakeDescriptionBoxEditable(object sender, RoutedEventArgs e)
		{
			EditableTextBox editableTextBox = (EditableTextBox)sender;
			editableTextBox.IsEditable = true;
		}
		
		
		private void MakeDescriptionBoxUneditable(object sender, RoutedEventArgs e)
		{
			EditableTextBox editableTextBox = (EditableTextBox)sender;
			editableTextBox.IsEditable = false;
		}
		
		
		private void HideCompletedTasksChanged(object sender, RoutedEventArgs e)
		{	
			UpdateVisibleTasks();
		}
		
		
		/// <summary>
		/// If completed tasks are to be hidden and a task has just been completed,
		/// refresh the view of visible tasks.
		/// </summary>
		private void TaskCompleted(object sender, RoutedEventArgs e)
		{
			if ((bool)hideCompletedTasksCheckbox.IsChecked) {
				CheckBox checkBox = (CheckBox)sender;
				if ((bool)checkBox.IsChecked) {
					UpdateVisibleTasks();
				}
			}
		}
		
		
		private void UpdateVisibleTasks()
		{
			if (Resources.Contains("tasksCollectionViewSource")) {
				CollectionViewSource cvs = (CollectionViewSource)Resources["tasksCollectionViewSource"];
				if (cvs != null && cvs.View != null) {	
					if ((bool)hideCompletedTasksCheckbox.IsChecked) {
						cvs.View.Filter = new Predicate<object>(HideCompletedTasksFilter); 
					}
					else {
						cvs.View.Filter = null;
					}
				}	
			}
		}
				
  
		private bool HideCompletedTasksFilter(object obj)
		{			
			Task task = (Task)obj;
			return (task.State != TaskState.Completed);
		}
		
				
		private void ShowNextPage(object sender, MouseButtonEventArgs e)
		{
		}
		
		
		private void ShowPreviousPage(object sender, MouseButtonEventArgs e)
		{
		}
		
		#endregion
    }
}