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
			set { 
				DataContext = value;
				
				// Disable/enable the filter control panel based on whether there are any tasks to filter:
				if (value != null) {
					value.CollectionChanged += delegate 
					{  
						try {
							// Only necessary if you've just added your only task, or just deleted your only task:
							if (value.Count < 2) {
								BindingExpression be = BindingOperations.GetBindingExpression(filterControlsPanel,
		            		                                       							  StackPanel.IsEnabledProperty);
		            			be.UpdateTarget();
							}
						}
						catch (Exception e) {
							System.Diagnostics.Debug.WriteLine("Couldn't update filter panel IsEnabled binding: " + e);
						}
					};
				}
			}
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
			
			// Access to the CollectionViewSource allows filters to be added/removed/refreshed:
			cvs = (CollectionViewSource)Resources["tasksCollectionViewSource"];
			
			// Set up button images:
            string imagePath;
            imagePath = Path.Combine(MyTasksPreferences.Instance.InstallDirectory,"delete.png");
            Tools.SetXAMLButtonImage(DeleteTaskButton,imagePath,"delete");
            imagePath = Path.Combine(MyTasksPreferences.Instance.InstallDirectory,"07.png");
            Tools.SetXAMLButtonImage(MoveTaskDownButton,imagePath,"down");
            imagePath = Path.Combine(MyTasksPreferences.Instance.InstallDirectory,"08.png");
            Tools.SetXAMLButtonImage(MoveTaskUpButton,imagePath,"up");
            imagePath = Path.Combine(MyTasksPreferences.Instance.InstallDirectory,"add.png");
            Tools.SetXAMLButtonImage(AddTaskButton,imagePath,"add"); 
            
            // Select the first item in the tag filter combo box:
            try {
            	 tagFilterComboBox.SelectedIndex = 0;
            }
            catch (Exception) {
            	System.Diagnostics.Debug.WriteLine("Tried and failed to select the first tag.");
            }
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
			if (!Tasks.Contains(task)) {
				throw new InvalidOperationException("Task '" + task + "' is not	a member of Tasks.");
			}
			try {
				 Tasks.Remove(task);
			}
			catch (Exception x) {
				Say.Error(x);
			}
			
			UpdateEmptyTaskListMessage();
		}
		
		
		/// <summary>
		/// Check whether the task list is currently being filtered by any criteria.
		/// </summary>
		/// <returns>True if any filters are active; false otherwise</returns>
		public bool IsFiltered()
		{
			if (!(bool)showAllTasksRadioButton.IsChecked) {
				return true;
			}
			if ((bool)activateTagFilterCheckBox.IsChecked && tagFilterComboBox.SelectedItem != null) {
				return true;
			}
			if (searchStringTextBox.Text.Length > 0) {
				return true;
			}
			return false;
		}
		
		#endregion
		
		#region Event handlers  	
		
		/// <summary>
		/// If the user is typing a description that requires an extra line to display
		/// in the task list, scroll the (now larger) task into view.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ScrollFullyIntoView(object sender, EventArgs e)
		{
			if (taskListBox.SelectedItem != null) {
				taskListBox.ScrollIntoView(taskListBox.SelectedItem);
			}
		}
		
		
		/// <summary>
		/// If the user hits delete while a task is selected, offer
		/// to delete that task.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
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
			
//			System.Diagnostics.Debug.AutoFlush = true;
//			foreach (Task t in taskListBox.ItemsSource) {
//				System.Diagnostics.Debug.WriteLine(t.Description.Substring(0,15));
//			}
//			System.Diagnostics.Debug.WriteLine("");
//			
//			taskListBox.InvalidateVisual();
//			taskListBox.UpdateLayout();
//			System.Diagnostics.Debug.WriteLine(BindingOperations.GetBindingExpressionBase(taskListBox,ListBox.ItemsSourceProperty).ToString());
//			System.Diagnostics.Debug.WriteLine(BindingOperations.GetBindingExpressionBase(taskListBox,ListBox.ItemsSourceProperty).Status.ToString());
			taskListBox.ScrollIntoView(task);
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
			
//			System.Diagnostics.Debug.AutoFlush = true;
//			foreach (Task t in taskListBox.ItemsSource) {
//				System.Diagnostics.Debug.WriteLine(t.Description.Substring(0,15));
//			}
//			System.Diagnostics.Debug.WriteLine("");
//			
//			taskListBox.InvalidateVisual();
//			taskListBox.UpdateLayout();
//			System.Diagnostics.Debug.WriteLine(BindingOperations.GetBindingExpressionBase(taskListBox,ListBox.ItemsSourceProperty).ToString());
//			System.Diagnostics.Debug.WriteLine(BindingOperations.GetBindingExpressionBase(taskListBox,ListBox.ItemsSourceProperty).Status.ToString());
			taskListBox.ScrollIntoView(task);
			
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
		
		
		private void OldShowOnlySelectedTagFilter(object sender, FilterEventArgs e)
		{
			if (((bool)activateTagFilterCheckBox.IsChecked) && tagFilterComboBox.SelectedItem != null) {
				string tag = (string)tagFilterComboBox.SelectedItem;
				Task task = (Task)e.Item;
				if (!task.Tags.Contains(tag)) {
					e.Accepted = false;
				}
			}
			// Never set e.Accepted to true, or you may override the results of another filter.
		}
		
		
		private void ShowOnlySelectedTagFilter(object sender, FilterEventArgs e)
		{
			string tag = (string)tagFilterComboBox.SelectedItem;
			Task task = (Task)e.Item;
			if (!task.Tags.Contains(tag)) {
				e.Accepted = false;
			}
			// Never set e.Accepted to true, or you may override the results of another filter.
		}
		
		#endregion
		
		#region Applying and removing filters
		
		/// <summary>
		/// Apply a filter that will hide any tasks not containing 
		/// a user-entered search string.
		/// </summary>
		private void ApplySearchFilter()
		{
			if (cvs != null) {
				cvs.Filter -= new FilterEventHandler(SearchFilter);
				cvs.Filter += new FilterEventHandler(SearchFilter);
			}
		}
				
		
		/// <summary>
		/// Stop filtering by search string.
		/// </summary>
		private void RemoveSearchFilter()
		{
			if (cvs != null) {
				cvs.Filter -= new FilterEventHandler(SearchFilter);
			}
		}
		
		
		/// <summary>
		/// Apply a filter that will hide any tasks not
		/// tagged with a user-specified tag.
		/// </summary>
		private void ApplyTagFilter()
		{
			if (cvs != null) {
				cvs.Filter -= new FilterEventHandler(ShowOnlySelectedTagFilter);
				cvs.Filter += new FilterEventHandler(ShowOnlySelectedTagFilter);
			}
		}
		
		
		/// <summary>
		/// Stop filtering by tag.
		/// </summary>
		private void RemoveTagFilter()
		{
			if (cvs != null) {
				cvs.Filter -= new FilterEventHandler(ShowOnlySelectedTagFilter);
			}
		}
		
		
		/// <summary>
		/// Apply filters to hide completed tasks.
		/// </summary>
		private void ApplyHideCompletedTasksFilter()
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
		private void ApplyHideUncompletedTasksFilter()
		{
			if (cvs != null) {
				cvs.Filter -= new FilterEventHandler(HideCompletedTasksFilter);
				cvs.Filter -= new FilterEventHandler(HideUncompletedTasksFilter);
				cvs.Filter += new FilterEventHandler(HideUncompletedTasksFilter);
			}
		}
		
		
		/// <summary>
		/// Stop filtering by completion state (show both completed and uncompleted tasks).
		/// </summary> 
		private void RemoveCompletionStateFilters()
		{
			if (cvs != null) {
				cvs.Filter -= new FilterEventHandler(HideUncompletedTasksFilter);
				cvs.Filter -= new FilterEventHandler(HideCompletedTasksFilter);
			}
		}
		
		
		/// <summary>
		/// Check whether it's appropriate to filter by search string,
		/// and if so apply the relevant filter.
		/// </summary>
		private void ConditionallyApplySearchFilter(object sender, RoutedEventArgs e)
		{
			// No conditions are necessary in this case.
			ApplySearchFilter();
		}
		
		
		/// <summary>
		/// Check whether it's appropriate to filter by tag,
		/// and if so apply the relevant filter.
		/// </summary>
		private void ConditionallyApplyTagFilter(object sender, RoutedEventArgs e)
		{
			if ((bool)activateTagFilterCheckBox.IsChecked && tagFilterComboBox.SelectedItem != null) {
				ApplyTagFilter();
			}
			else {
				RemoveTagFilter();
			}
		}
				
		
		/// <summary>
		/// Check whether it's appropriate to filter by completion state,
		/// and if so apply the relevant filter.
		/// </summary>
		private void ConditionallyApplyStateFilters(object sender, RoutedEventArgs e)
		{
			if ((bool)showAllTasksRadioButton.IsChecked) {
				RemoveCompletionStateFilters();
			}
			else if ((bool)showCompletedTasksOnlyRadioButton.IsChecked) {
				ApplyHideUncompletedTasksFilter();
			}
			else if ((bool)showUncompletedTasksOnlyRadioButton.IsChecked) {
				ApplyHideCompletedTasksFilter();
			}
		}
		
		
		/// <summary>
		/// Remove and re-apply all active filters to force the filtered task list to refresh.
		/// </summary>
		public void RefreshAllFilters()
		{
			ConditionallyApplySearchFilter(null,null);
			ConditionallyApplyTagFilter(null,null);
			ConditionallyApplyStateFilters(null,null);
			UpdateEmptyTaskListMessage();
		}
				
		
		/// <summary>
		/// Clears all active filters by deselecting them through the user interface.
		/// </summary>
		public void ClearAllFilters()
		{
			if ((bool)activateTagFilterCheckBox.IsChecked) {
				activateTagFilterCheckBox.IsChecked = false;
			}
			if (searchStringTextBox.Text.Length > 0) {
				searchStringTextBox.Text = String.Empty;	
			}
			if (!(bool)showAllTasksRadioButton.IsChecked) {
				showAllTasksRadioButton.IsChecked = true;
			}
		}
		
		
		/// <summary>
		/// Clears all active filters by deselecting them through the user interface.
		/// </summary>
		public void ClearAllFilters(object sender, RoutedEventArgs e)
		{
			ClearAllFilters();
		}
		
		
		private void UpdateEmptyTaskListMessage()
		{
			try {
				MultiBindingExpression mbe = BindingOperations.GetMultiBindingExpression(emptyTaskListMessageTextBlock,
				                                                                         TextBlock.TextProperty);
				mbe.UpdateTarget();
			}
			catch (Exception e) {
				Say.Error(e);
			}
		}
		
		#endregion
    }
}