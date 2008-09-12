using System;
using System.IO;
using System.Collections.Specialized;
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
			taskListBox.ScrollIntoView(task);
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
		
		private void HandleKeyPresses(object sender, KeyEventArgs e)
		{			
			if (e.Key == Key.Delete && taskListBox.SelectedItem != null) {
				DeleteSelectedTask();
			}			
		}
		
		
		/// <summary>
		/// Delete the currently selected task.
		/// </summary>
		private void DeleteSelectedTask(object sender, RoutedEventArgs e)
		{
			DeleteSelectedTask();
		}
		
		
		/// <summary>
		/// Delete the currently selected task.
		/// </summary>
		private void DeleteSelectedTask()
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
		
		
		/// <summary>
		/// Move the currently selected task up one space in the list.
		/// </summary>
		private void MoveSelectedTaskUp(object sender, RoutedEventArgs e)
		{
			if (taskListBox.SelectedItem != null) {
				Task task = (Task)taskListBox.SelectedItem;
				MoveTaskUp(task);
			}
		}
		
		
		/// <summary>
		/// Move the currently selected task down one space in the list.
		/// </summary>
		private void MoveSelectedTaskDown(object sender, RoutedEventArgs e)
		{
			if (taskListBox.SelectedItem != null) {
				Task task = (Task)taskListBox.SelectedItem;
				MoveTaskDown(task);
			}
		}
		
		
		/// <summary>
		/// Move a given task up one space in the list.
		/// </summary>
		/// <param name="task">The task to move</param>
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
		
		
		/// <summary>
		/// Move a given task down one space in the list.
		/// </summary>
		/// <param name="task">The task to move</param>
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
		
		
		/// <summary>
		/// Apply a filter that will hide any tasks not containing 
		/// a user-entered search string.
		/// </summary>
		private void ShowOnlyTasksContainingSearchString()
		{
			if (cvs != null) {
				cvs.Filter -= new FilterEventHandler(SearchFilter);
				cvs.Filter += new FilterEventHandler(SearchFilter);
			}
		}
		
		
		/// <summary>
		/// Apply a filter that will hide any tasks not
		/// tagged with a user-specified tag.
		/// </summary>
		private void ShowOnlyTasksWithGivenTag()
		{
			if (cvs != null) {
				cvs.Filter -= new FilterEventHandler(ShowOnlySelectedTagFilter);
				cvs.Filter += new FilterEventHandler(ShowOnlySelectedTagFilter);
			}
		}
		
		
		/// <summary>
		/// Apply filters to show both completed and uncompleted tasks.
		/// </summary> 
		private void ShowBothCompletedAndUncompletedTasks(object sender, RoutedEventArgs e)
		{
			if (cvs != null) {
				cvs.Filter -= new FilterEventHandler(HideUncompletedTasksFilter);
				cvs.Filter -= new FilterEventHandler(HideCompletedTasksFilter);
			}
		}
		
		
		/// <summary>
		/// Apply filters to hide completed tasks.
		/// </summary>
		private void ShowOnlyUncompletedTasks(object sender, RoutedEventArgs e)
		{
			if (cvs != null) {
				cvs.Filter -= new FilterEventHandler(HideUncompletedTasksFilter);
				cvs.Filter -= new FilterEventHandler(HideCompletedTasksFilter);
				cvs.Filter += new FilterEventHandler(HideCompletedTasksFilter);
			}
		}
		
		
		/// <summary>
		/// Apply filters to show only completed tasks.
		/// </summary>
		private void ShowOnlyCompletedTasks(object sender, RoutedEventArgs e)
		{
			if (cvs != null) {
				cvs.Filter -= new FilterEventHandler(HideUncompletedTasksFilter);
				cvs.Filter += new FilterEventHandler(HideUncompletedTasksFilter);
				cvs.Filter -= new FilterEventHandler(HideCompletedTasksFilter);
			}
		}
		
		
		/// <summary>
		/// When a task is completed/uncompleted, update the filters that
		/// show/hide tasks based on their completion status.
		/// </summary>
		internal void RefreshTaskCompletedFilter()
		{
			//TODO: This stops you from selecting completed tasks???
			#region Horrible
//			foreach (RadioButton button in taskCompletedFilterRadioButtonPanel.Children) {
//				if ((bool)button.IsChecked) {
//					button.IsChecked = false;
//					button.IsChecked = true;
//				}
//			}
			#endregion
		}
		
		
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
		
		
		/// <summary>
		/// When the search text changes, update all the view filters.
		/// </summary>
		private void ShowOnlyTasksContainingSearchString(object sender, RoutedEventArgs e)
		{
			ShowOnlyTasksContainingSearchString();
		}
		
		
		/// <summary>
		/// When the user selects a particular tag, apply a filter to hide
		/// any task which does not have that tag.
		/// </summary>
		private void OnlyShowTasksWithSelectedTag(object sender, SelectionChangedEventArgs e)
		{
			ShowOnlyTasksWithGivenTag();
		}
		
		
		/// <summary>
		/// If the task list is currently only displaying tasks 
		/// with the selected tag, deselect that tag in order to stop filtering.
		/// </summary>
		private void ClearTagFilter(object sender, RoutedEventArgs e)
		{
			if (tagFilterListView.SelectedItem != null) {
				tagFilterListView.SelectedItem = null;
			}
		}
		
		
		/// <summary>
		/// Stop filtering by tag, search string, state, or anything else.
		/// </summary>
		private void ClearAllFilters(object sender, RoutedEventArgs e)
		{
			ClearAllFilters();
		}
		
		
		public void ClearAllFilters()
		{
			if (tagFilterListView.SelectedItem != null) {
				tagFilterListView.SelectedItem = null;
			}
			if (searchStringTextBox.Text.Length > 0) {
				searchStringTextBox.Text = String.Empty;	
			}
			if (!(bool)showAllTasksRadioButton.IsChecked) {
				showAllTasksRadioButton.IsChecked = true;
			}
		}
		
		
		private bool showFilterControls = true;
		public bool ShowFilterControls
		{
			get { return showFilterControls; }
			set { 
				showFilterControls = value;
			}
		}
		
		private void ShowOrHideFilteringControls(object sender, RoutedEventArgs e)
		{
			ShowFilterControls = !ShowFilterControls;
		}
		
		#endregion
		
		#region Filters		
		
		/// <summary>
		/// If a task has not been completed, filter it out.
		/// </summary>
		private void HideUncompletedTasksFilter(object sender, FilterEventArgs e)
		{
			Task task = (Task)e.Item;
			if (task.State != TaskState.Completed) {
				e.Accepted = false;
			}
			// Never set e.Accepted to true, or you may override the results of another filter.
		}
		
				
		/// <summary>
		/// If a task has not been completed, filter it out.
		/// </summary>
		private void HideCompletedTasksFilter(object sender, FilterEventArgs e)
		{
			Task task = (Task)e.Item;
			if (task.State == TaskState.Completed) {
				e.Accepted = false;
			}
			// Never set e.Accepted to true, or you may override the results of another filter.
		}
		
		
		private void SearchFilter(object sender, FilterEventArgs e)
		{
			if (searchStringTextBox.Text.Length > 0) {
				Task task = (Task)e.Item;
				if (!task.ContainsString(searchStringTextBox.Text)) {
					e.Accepted = false;
				}
			}
			// Never set e.Accepted to true, or you may override the results of another filter.
		}
		
		
		private void ShowOnlySelectedTagFilter(object sender, FilterEventArgs e)
		{
			if (tagFilterListView.SelectedItem != null) {
				string tag = (string)tagFilterListView.SelectedItem;
				Task task = (Task)e.Item;
				if (!task.Tags.Contains(tag)) {
					e.Accepted = false;
				}
			}
		}
		
		#endregion
    }
}