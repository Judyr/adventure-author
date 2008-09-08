﻿using System;
using System.IO;
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
		public TaskCollection Tasks {
			get { return (TaskCollection)DataContext; }
			set { DataContext = value; }
		}   	
		
		
		/// <summary>
		/// The CollectionViewSource providing access to the task collection.
		/// </summary>
		private CollectionViewSource cvs;
    	
    	#endregion
    	
    	#region Constructors
    	
    	public ListBox TaskListBox {
    		get {
    			return taskListBox;
    		}
    	}
    	
    	
		public TaskPad()
		{
			InitializeComponent();
			cvs = (CollectionViewSource)Resources["tasksCollectionViewSource"];
			// If 'Hide completed tasks' is ticked at launch, the Checked event fires
			// before there is a CollectionViewSource reference to add search filters to.
			// Deal with this possibility by explicitly refreshing the filters here:
			//RefreshFilters();			
			
            string imagePath;
            imagePath = Path.Combine(MyTasksPreferences.Instance.InstallDirectory,"delete.png");
            Tools.SetXAMLButtonImage(DeleteTaskButton,imagePath,"delete");
            imagePath = Path.Combine(MyTasksPreferences.Instance.InstallDirectory,"07.png");
            Tools.SetXAMLButtonImage(MoveTaskDownButton,imagePath,"down");
            imagePath = Path.Combine(MyTasksPreferences.Instance.InstallDirectory,"08.png");
            Tools.SetXAMLButtonImage(MoveTaskUpButton,imagePath,"up");
            imagePath = Path.Combine(MyTasksPreferences.Instance.InstallDirectory,"add.png");
            Tools.SetXAMLButtonImage(AddTaskButton,imagePath,"add");
		}
		
		#endregion
		
		#region Methods
			
		/// <summary>
		/// Open a task collection.
		/// </summary>
		public void Open(TaskCollection tasks)
		{
			Tasks = tasks;
		}
		
		
		/// <summary>
		/// Clear the current set of tasks.
		/// </summary>
		public void Clear()
		{
			Tasks = null;
		}
		
		
		/// <summary>
		/// Add a task to the current collection.
		/// </summary>
		/// <param name="task">The task to add</param>
		public void Add(Task task)
		{
			if (Tasks == null) {
				throw new InvalidOperationException("No task collection is currently open.");
			}
			Tasks.Add(task);
		}
		
		
		/// <summary>
		/// Add a task to the current collection, placed after the currently selected task.
		/// </summary>
		/// <param name="task">The task to add</param>
		public void AddAfterSelectedTask(Task task)
		{
			if (Tasks == null) {
				throw new InvalidOperationException("No task collection is currently open.");
			}
			if (taskListBox.SelectedItem == null) {
				Tasks.Add(task);
			}
			else {
				int index = Tasks.IndexOf((Task)taskListBox.SelectedItem) + 1;
				if (index == Tasks.Count) {
					Tasks.Add(task);
				}
				else {
					Tasks.Insert(index,task);
				}
			}		
		}
		
		
		/// <summary>
		/// Delete a task from the current collection.
		/// </summary>
		/// <param name="task">The task to delete</param>
		public void Delete(Task task)
		{
			if (Tasks == null) {
				throw new InvalidOperationException("No task collection is currently open.");
			}
			Tasks.Remove(task);
		}
		
		#endregion
		
		#region Event handlers  
		
		private void HandleDeleteTagClicks(object sender, RoutedEventArgs e)
		{
			TagControl tagControl = (TagControl)e.OriginalSource;
			Say.Information(tagControl.DataContext.GetType().ToString() + "\n\n" + tagControl.DataContext);
		}
		
		
		private void HandleReturnKeyPressForTagEntry(object sender, KeyEventArgs e)
		{
			if (e.OriginalSource is TextBox) {
				TextBox textBox = (TextBox)e.OriginalSource;
				if (textBox.Name == "AddTagTextBox" && e.Key == Key.Return) {
					if (textBox.Text.Length > 0) {					
						Task task = (Task)textBox.DataContext;
						if (!task.Tags.Contains(textBox.Text)) {
							task.Tags.Add(textBox.Text);
						}
						textBox.Text = String.Empty;
					}				
				}
			}			
		}
		
		
		private void DeleteSelectedTask(object sender, RoutedEventArgs e)
		{
			if (taskListBox.SelectedItem != null) {		
				Task task = (Task)taskListBox.SelectedItem;
				MessageBoxResult result = MessageBox.Show("Are you sure you want to delete the selected task?",
											              "Delete task?",
											              MessageBoxButton.OKCancel,
											              MessageBoxImage.Question,
											              MessageBoxResult.OK);
				if (result == MessageBoxResult.OK) {
					Delete(task);
				}	
			}
		}
		
		
		private void MoveSelectedTaskUp(object sender, RoutedEventArgs e)
		{
			if (taskListBox.SelectedItem != null) {
				Task task = (Task)taskListBox.SelectedItem;
				MoveTaskUp(task);
			}
		}
		
		
		private void MoveSelectedTaskDown(object sender, RoutedEventArgs e)
		{
			if (taskListBox.SelectedItem != null) {
				Task task = (Task)taskListBox.SelectedItem;
				MoveTaskDown(task);
			}
		}
		
		
		private void MoveTaskUp(Task task)
		{
			int index = Tasks.IndexOf(task);
			if (index == -1) {
				throw new InvalidOperationException("The selected task was not found in the current task collection.");
			}
			int newIndex = index - 1;
			if (newIndex >= 0) {
				Tasks.Move(index,newIndex);
			}	
		}
		
		
		private void MoveTaskDown(Task task)
		{
			int index = Tasks.IndexOf(task);
			if (index == -1) {
				throw new InvalidOperationException("The selected task was not found in the current task collection.");
			}
			int newIndex = index + 1;
			int maxIndex = Tasks.Count - 1;
			if (newIndex <= maxIndex) {
				Tasks.Move(index,newIndex);
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
		
		
//		private void AddHideCompletedTasksFilter(object sender, RoutedEventArgs e)
//		{
//			if (cvs != null) {
//				cvs.Filter += new FilterEventHandler(CompletedTasksFilter);
//			}
//		}
//		
//		
//		private void RemoveHideCompletedTasksFilter(object sender, RoutedEventArgs e)
//		{
//			if (cvs != null) {
//				cvs.Filter -= new FilterEventHandler(CompletedTasksFilter);
//			}
//		}
//		
//		
//		private void RefreshFilters()
//		{
//			if ((bool)hideCompletedTasksCheckbox.IsChecked) {
//				cvs.Filter -= new FilterEventHandler(CompletedTasksFilter);
//				cvs.Filter += new FilterEventHandler(CompletedTasksFilter);
//			}
//			
//			cvs.Filter -= new FilterEventHandler(SearchFilter);
//			cvs.Filter += new FilterEventHandler(SearchFilter);
//		}
//		
//		
//		private void CompletedTasksFilter(object sender, FilterEventArgs e)
//		{
//			Task task = (Task)e.Item;
//			if (task.State == TaskState.Completed) {
//				e.Accepted = false;
//			}
//			// Never set e.Accepted to true, or you may override the results of another filter.
//		}
//		
//		
//		private void SearchFilter(object sender, FilterEventArgs e)
//		{
//			if (searchStringTextBox.Text.Length > 0) {
//				Task task = (Task)e.Item;
//				if (!task.ContainsString(searchStringTextBox.Text)) {
//					e.Accepted = false;
//				}
//			}
//			// Never set e.Accepted to true, or you may override the results of another filter.
//		}
//		
//		
//		/// <summary>
//		/// If completed tasks are to be hidden and a task has just been completed,
//		/// refresh the view of visible tasks.
//		/// </summary>
//		private void TaskCompleted(object sender, RoutedEventArgs e)
//		{
//			if ((bool)hideCompletedTasksCheckbox.IsChecked) {
//				CheckBox checkBox = (CheckBox)sender;
//				if ((bool)checkBox.IsChecked) {					
//					RefreshFilters();
//				}
//			}
//		}
//		
//		
//		/// <summary>
//		/// When the search text changes, update all the view filters.
//		/// </summary>
//		private void SearchTextChanged(object sender, RoutedEventArgs e)
//		{
//			RefreshFilters();
//		}
		
				
		private void ShowNextPage(object sender, MouseButtonEventArgs e)
		{			
		}
		
		
		private void ShowPreviousPage(object sender, MouseButtonEventArgs e)
		{
		}
		
		#endregion
    }
}