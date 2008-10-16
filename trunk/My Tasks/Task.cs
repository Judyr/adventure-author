/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 24/07/2008
 * Time: 17:15
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Windows.Data;
using System.Xml.Serialization;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Tasks
{
	/// <summary>
	/// A task for a task manager application.
	/// </summary>
	[Serializable]
	public class Task : INotifyPropertyChanged
	{
		#region Properties and fields
		
		/// <summary>
		/// A textual description of what this task involves.
		/// </summary>
		private string description;
		[XmlElement]
		public string Description {
			get { return description; }
			set { 
				description = value; 
				OnPropertyChanged(new PropertyChangedEventArgs("Description"));
			}
		}
		
		
		/// <summary>
		/// Tags associated with this task, usually indicating the type(s) of work this task relates to. 
		/// For example, 'Bugs' or 'World building', or both. 
		/// </summary>
		private ObservableCollection<string> tags = new ObservableCollection<string>();
		public ObservableCollection<string> Tags {
			get { return tags; }
		}
		
		
		/// <summary>
		/// The origin of the task. Tasks will often be original to the user, but may also have
		/// been suggested by the application or by someone else. 
		/// </summary>
		/// <remarks>For Adventure Author, this property can be used to track whether a task
		/// was sent from the Fridge Magnets application, was suggested after the application
		/// checked for errors/warnings in a module, or came from a pre-defined set of useful 
		/// tasks relating to game design.</remarks>
		private TaskOrigin origin;
		[XmlElement]
		public TaskOrigin Origin {
			get { return origin; }
			set { 
				origin = value; 
				OnPropertyChanged(new PropertyChangedEventArgs("Origin"));
			}
		}
		
				
		/// <summary>
		/// The various states a task can be in - completed, not yet completed,
		/// currently in progress, or deleted. 
		/// </summary>
		private TaskState state;
		[XmlElement]
		public TaskState State {
			get { return state; }
			set { 
				state = value;
				OnPropertyChanged(new PropertyChangedEventArgs("State"));
			}
		}
		
		
		/// <summary>
		/// The user who created this task.
		/// </summary>
		private string creator;
		[XmlAttribute("Creator")]
		public string Creator {
			get { return creator; }
			set { 
				creator = value; 
				OnPropertyChanged(new PropertyChangedEventArgs("Creator"));
			}
		}
		
		
		/// <summary>
		/// The date and time this task was created.
		/// </summary>
		private DateTime created;
		[XmlAttribute("Created")]
		public DateTime Created {
			get { return created; }
			set { 
				created = value; 
				OnPropertyChanged(new PropertyChangedEventArgs("Created"));
			}
		}		
		
		
		/// <summary>
		/// The date and time this task was (most recently) completed, if it has been completed.
		/// </summary>
		/// <remarks>This property should be set to DateTime.MinValue for uncompleted tasks</remarks>
		[OptionalFieldAttribute()]
		private DateTime completed;
		[XmlAttribute("Completed")]
		public DateTime Completed {
			get { return completed; }
			set { 
				completed = value; 
				OnPropertyChanged(new PropertyChangedEventArgs("Completed"));
			}
		}	
		
		#endregion
	
		#region Events
		
		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) {
				handler(this, e);
			}
		}
		
		#endregion
		
		#region Constructors
		
		/// <summary>
		/// For serialization.
		/// </summary>
		public Task() {}
						
		
		/// <summary>
		/// Create a new task.
		/// </summary>
		/// <param name="description">A textual description of what this task involves</param>
		public Task(string description) : this(description,
		                                       new ObservableCollection<string>())
		{
		}
		
		
		/// <summary>
		/// Create a new task.
		/// </summary>
		/// <param name="description">A textual description of what this task involves</param>
		/// <param name="tags">Tags associated with this task, usually indicating the type(s) of work this 
		/// task relates to.</param>
		public Task(string description, string tag) : this(description,
		                                                   tag,
		                                                   TaskOrigin.User)
		{
		}
		
		
		/// <summary>
		/// Create a new task.
		/// </summary>
		/// <param name="description">A textual description of what this task involves</param>
		/// <param name="tags">Tags associated with this task, usually indicating the type(s) of work this 
		/// task relates to.</param>
		public Task(string description, ObservableCollection<string> tags) : this(description,
						                                                          tags,
						                                                          TaskOrigin.User)
		{
		}
		
		
		/// <summary>
		/// Create a new task.
		/// </summary>
		/// <param name="description">A textual description of what this task involves</param>
		/// <param name="tags">A tag associated with this task, usually indicating the type of work this 
		/// task relates to.</param>
		/// <param name="origin">The origin of the task. Tasks will often be original to the user, but may also have
		/// been suggested by the application or by someone else.</param>
		public Task(string description, string tag, TaskOrigin origin) : this(description,
			                                                                  tag,
			                                                                  origin,
			                                                                  TaskState.NotCompleted)
		{
		}
		
		
		/// <summary>
		/// Create a new task.
		/// </summary>
		/// <param name="description">A textual description of what this task involves</param>
		/// <param name="tags">Tags associated with this task, usually indicating the type(s) of work this 
		/// task relates to.</param>
		/// <param name="origin">The origin of the task. Tasks will often be original to the user, but may also have
		/// been suggested by the application or by someone else.</param>
		public Task(string description, ObservableCollection<string> tags, TaskOrigin origin) : this(description,
							                                                                         tags,
							                                                                         origin,
							                                                                         TaskState.NotCompleted)
		{
		}
		
		
		/// <summary>
		/// Create a new task.
		/// </summary>
		/// <param name="description">A textual description of what this task involves</param>
		/// <param name="tags">A tag associated with this task, usually indicating the type of work this 
		/// task relates to.</param>
		/// <param name="origin">The origin of the task. Tasks will often be original to the user, but may also have
		/// been suggested by the application or by someone else.</param>
		/// <param name="state">The various states a task can be in - completed, not yet completed,
		/// currently in progress, or deleted.</param>
		public Task(string description, string tag, TaskOrigin origin, TaskState state) : this(description,
			                                                                                   tag,
			                                                                                   origin,
			                                                                                   state,
			                                                                                   User.GetCurrentUserName(),
			                                                                                   DateTime.Now)
		{
		}
		
		
		/// <summary>
		/// Create a new task.
		/// </summary>
		/// <param name="description">A textual description of what this task involves</param>
		/// <param name="tags">Tags associated with this task, usually indicating the type(s) of work this 
		/// task relates to.</param>
		/// <param name="origin">The origin of the task. Tasks will often be original to the user, but may also have
		/// been suggested by the application or by someone else.</param>
		/// <param name="state">The various states a task can be in - completed, not yet completed,
		/// currently in progress, or deleted.</param>
		public Task(string description, ObservableCollection<string> tags, TaskOrigin origin, TaskState state) : this(description,
							                                                                                          tags,
							                                                                                          origin,
							                                                                                          state,
							                                                                                          User.GetCurrentUserName(),
							                                                                                          DateTime.Now)
		{
		}
		
		
		/// <summary>
		/// Create a new task.
		/// </summary>
		/// <param name="description">A textual description of what this task involves</param>
		/// <param name="tags">A tag associated with this task, usually indicating the type of work this 
		/// task relates to.</param>
		/// <param name="origin">The origin of the task. Tasks will often be original to the user, but may also have
		/// been suggested by the application or by someone else.</param>
		/// <param name="state">The various states a task can be in - completed, not yet completed,
		/// currently in progress, or deleted.</param>
		/// <param name="creator">The user who created this task.</param>
		/// <param name="created">The date and time this task was created.</param>
		public Task(string description, string tag, TaskOrigin origin, 
		            TaskState state, string creator, DateTime created) : this(description,
		                                                          			  new ObservableCollection<string>(),
		                                                          			  origin,
		                                                          			  state,
		                                                          			  creator,
		                                                          			  created)
		{
			this.tags.Add(tag);
		}
		
		
		/// <summary>
		/// Create a new task.
		/// </summary>
		/// <param name="description">A textual description of what this task involves</param>
		/// <param name="tags">Tags associated with this task, usually indicating the type(s) of work this 
		/// task relates to.</param>
		/// <param name="origin">The origin of the task. Tasks will often be original to the user, but may also have
		/// been suggested by the application or by someone else.</param>
		/// <param name="state">The various states a task can be in - completed, not yet completed,
		/// currently in progress, or deleted.</param>
		/// <param name="creator">The user who created this task.</param>
		/// <param name="created">The date and time this task was created.</param>
		public Task(string description, ObservableCollection<string> tags, TaskOrigin origin, TaskState state, string creator, DateTime created)
		{
			this.description = description;
			this.tags = tags;
			this.origin = origin;
			this.state = state;
			this.creator = creator;
			this.created = created;
			this.completed = DateTime.MinValue;
		}
		
		#endregion

		#region Methods
		
		/// <summary>
		/// Check whether a particular string occurs within the description
		/// of this task.
		/// </summary>
		/// <param name="searchString">The string to search for</param>
		/// <returns>True if the string occurs; false otherwise</returns>
		public bool ContainsString(string searchString)
		{			
			string str = searchString.ToLower();
			if (Description != null && Description.ToLower().Contains(str) && Description != String.Empty) {
				return true;
			}
			// No longer searching for tag text, as this seems confusing when the tags
			// are not visible in the list view.
//			else {
//				foreach (string tag in Tags) {
//					if (tag.ToLower().Contains(str) && tag != String.Empty) {
//						return true;
//					}
//				}
//			}
			return false;
		}
		
		
		/// <summary>
		/// Mark this task as completed and set the completion date to DateTime.Now.
		/// </summary>
		public void Complete()
		{
			State = TaskState.Completed;
			Completed = DateTime.Now;
		}
		
		
		/// <summary>
		/// Mark this task as uncompleted and erase the completion date.
		/// </summary>
		/// <remarks>Sets Completed to DateTime.MinValue, as nullable values can't be serialised with XML.</remarks>
		public void Uncomplete()
		{
			State = TaskState.NotCompleted;
			Completed = DateTime.MinValue;
		}
		
		
		public override string ToString()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder("'" + Description + " (Tags: ");
			if (Tags.Count > 0) {
				foreach (string tag in Tags) {
					sb.Append("'" + tag + "', ");
				}
				sb.Remove(sb.Length-2,2); //remove the last comma and space
			}
			else {
				sb.Append("None");
			}
			sb.Append("; ");
			
			sb.Append("Created: " + Created + "; Origin: " + Origin + "; Status: " + State);
			
			if (Completed != DateTime.MinValue) {
				sb.Append("; Completed: " + Completed);
			}
			
			sb.Append(")'");
			
			return sb.ToString();
		}
		
		#endregion
	}
}
