/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 24/07/2008
 * Time: 17:15
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;
using System.Windows.Data;
using System.Xml.Serialization;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Tasks
{
	/// <summary>
	/// A task for a task manager application.
	/// </summary>
	[Serializable]
	public class Task
	{
		#region Properties and fields
		
		/// <summary>
		/// A textual description of what this task involves.
		/// </summary>
		private string description;
		[XmlElement]
		public string Description {
			get { return description; }
			set { description = value; }
		}
		
		
		/// <summary>
		/// Tags associated with this task, usually indicating the type(s) of work this task relates to. 
		/// For example, 'Bugs' or 'World building', or both. 
		/// </summary>
		private List<string> tags;
		[XmlArray]
		public List<string> Tags {
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
			set { origin = value; }
		}
		
				
		/// <summary>
		/// The various states a task can be in - completed, not yet completed,
		/// currently in progress, or deleted. 
		/// </summary>
		private TaskState state;
		[XmlElement]
		public TaskState State {
			get { return state; }
			set { state = value; }
		}
		
		
		/// <summary>
		/// The user who created this task.
		/// </summary>
		private string creator;
		[XmlAttribute("Creator")]
		public string Creator {
			get { return creator; }
			set { creator = value; }
		}
		
		
		/// <summary>
		/// The date and time this task was created.
		/// </summary>
		private DateTime created;
		[XmlAttribute("Created")]
		public DateTime Created {
			get { return created; }
			set { created = value; }
		}		
		
		#endregion
		
		#region Constructors
		
		/// <summary>
		/// For serialization.
		/// </summary>
		private Task() {}
						
		
		/// <summary>
		/// Create a new task.
		/// </summary>
		/// <param name="description">A textual description of what this task involves</param>
		public Task(string description) : this(description,
		                                       new List<string>())
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
		public Task(string description, List<string> tags) : this(description,
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
		public Task(string description, List<string> tags, string origin) : this(description,
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
		public Task(string description, List<string> tags, string origin, TaskState state) : this(description,
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
		                                                          			  new List<string>(1),
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
		public Task(string description, List<string> tags, string origin, TaskState state, string creator, DateTime created)
		{
			this.description = description;
			this.tags = tags;
			this.origin = origin;
			this.state = state;
			this.creator = creator;
			this.created = created;
		}
		
		#endregion
	}
}
