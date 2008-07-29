/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 24/07/2008
 * Time: 17:15
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
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
		#region Constants
		
		
		
		#endregion
		
		#region Properties and fields
		
		/// <summary>
		/// A textual description of what this task involves.
		/// </summary>
		[XmlElement]
		private string description;
		public string Description {
			get { return description; }
			set { description = value; }
		}
		
		
		/// <summary>
		/// The type of work this task relates to. For example, 'Bugs' or 'World building'. 
		/// </summary>
		/// <remarks>The set of categories may differ across applications, and users might wish
		/// to add their own categories: for this reason, the information is stored as a string
		/// rather than an enum, but it's expected that individual user interfaces will generally
		/// restrict users to a suitable set of pre-defined categories.</remarks>
		[XmlElement]
		private string category;
		public string Category {
			get { return category; }
			set { category = value; }
		}
		
		
		/// <summary>
		/// The origin of the task. Tasks will often be original to the user, but may also have
		/// been suggested by the application or by someone else. 
		/// </summary>
		/// <remarks>For Adventure Author, this property can be used to track whether a task
		/// was sent from the Fridge Magnets application, was suggested after the application
		/// checked for errors/warnings in a module, or came from a pre-defined set of useful 
		/// tasks relating to game design.</remarks>
		[XmlAttribute]
		private string origin;
		public string Origin {
			get { return origin; }
		}
		
				
		/// <summary>
		/// The various states a task can be in - completed, not yet completed,
		/// currently in progress, or deleted. 
		/// </summary>
		[XmlAttribute]
		private TaskState state;
		public TaskState State {
			get { return state; }
			set { state = value; }
		}
		
		
		/// <summary>
		/// The user who created this task.
		/// </summary>
		private string creator;
		public string Creator {
			get { return creator; }
		}
		
		
		/// <summary>
		/// The date and time this task was created.
		/// </summary>
		private DateTime created;
		public DateTime Created {
			get { return created; }
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
		public Task(string description)
		{
			this.description = description;
		}
		
		
		/// <summary>
		/// Create a new task.
		/// </summary>
		/// <param name="description">A textual description of what this task involves</param>
		/// <param name="category">The type of work this task relates to. For example, 'Bugs' or 'World building'</param>
		public Task(string description, string category) 
		{
		}
		
		
		/// <summary>
		/// Create a new task.
		/// </summary>
		/// <param name="description">A textual description of what this task involves</param>
		/// <param name="category">The type of work this task relates to. For example, 'Bugs' or 'World building'</param>
		public Task(string description, string category) : this(description,category,
		{
		}
		
		
		/// <summary>
		/// Create a new task.
		/// </summary>
		/// <param name="description">A textual description of what this task involves</param>
		/// <param name="category">The type of work this task relates to. For example, 'Bugs' or 'World building'</param>
		/// <param name="origin">The origin of the task. Tasks will often be original to the user, but may also have
		/// been suggested by the application or by someone else. </param>
		public Task(string description, string category, string origin) : this(description,category,origin,
		                                                                       TaskState.NotCompleted,
		                                                                       User.GetCurrentUserName(),
		                                                                       DateTime.Now)
		{
		}
		
		
		/// <summary>
		/// Create a new task.
		/// </summary>
		/// <param name="description">A textual description of what this task involves</param>
		/// <param name="category">The type of work this task relates to. For example, 'Bugs' or 'World building'</param>
		/// <param name="origin">The origin of the task. Tasks will often be original to the user, but may also have
		/// been suggested by the application or by someone else.</param>
		/// <param name="state">The various states a task can be in - completed, not yet completed,
		/// currently in progress, or deleted.</param>
		/// <param name="user">The user who created this task.</param>
		/// <param name="created">The date and time this task was created.</param>
		public Task(string description, string category, string origin, TaskState state, string user, DateTime created)
		{
			this.description = description;
			this.category = category;
			this.origin = origin;
			this.state = state;
			this.user = user;
			this.created = created;
		}
		
		#endregion
	}
}
