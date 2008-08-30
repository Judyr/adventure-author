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
		[XmlArray]
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
		private string origin;
		[XmlElement]
		public string Origin {
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
		                                                   TaskOrigin.UserCreated.ToString())
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
		                                                          TaskOrigin.UserCreated.ToString())
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
		public Task(string description, string tag, string origin) : this(description,
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
		public Task(string description, ObservableCollection<string> tags, string origin) : this(description,
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
		public Task(string description, string tag, string origin, TaskState state) : this(description,
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
		public Task(string description, ObservableCollection<string> tags, string origin, TaskState state) : this(description,
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
		public Task(string description, string tag, string origin, 
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
		public Task(string description, ObservableCollection<string> tags, string origin, TaskState state, string creator, DateTime created)
		{
			this.description = description;
			this.tags = tags;
			this.origin = origin;
			this.state = state;
			this.creator = creator;
			this.created = created;
		}
		
		#endregion

		#region Methods
		
		/// <summary>
		/// Check whether a particular string occurs within the description or tags
		/// of this task.
		/// </summary>
		/// <param name="searchString">The string to search for</param>
		/// <returns>True if the string occurs; false otherwise</returns>
		public bool ContainsString(string searchString)
		{
			if (Description.Contains(searchString) && Description != String.Empty) {
				return true;
			}
			else {
				foreach (string tag in Tags) {
					if (tag.Contains(searchString) && tag != String.Empty) {
						return true;
					}
				}
			}
			return false;
		}
		
		#endregion
	}
}
