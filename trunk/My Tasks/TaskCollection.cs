/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 29/07/2008
 * Time: 12:30
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Tasks
{
	/// <summary>
	/// A collection of tasks for a task manager application. 
	/// </summary>
	[Serializable]
	public class TaskCollection
	{	
		/// <summary>
		/// The user who created this task collection.
		/// </summary>
		private string creator;
		[XmlAttribute("Creator")]
		public string Creator {
			get { return creator; }
			set { creator = value; }
		}
		
		
		/// <summary>
		/// The time this task collection was created.
		/// </summary>
		private DateTime created;
		[XmlAttribute("Created")]
		public DateTime Created {
			get { return created; }
			set { created = value; }
		}
		
		
		/// <summary>
		/// A list of every task in this collection. 
		/// </summary>
		private List<Task> tasks;
		[XmlArray]
		public List<Task> Tasks {
			get { return tasks; }
			set { tasks = value; } //to enable TwoWay data binding
		}
		
		
		/// <summary>
		/// Create a new task collection.
		/// </summary>
		public TaskCollection() : this(new List<Task>())
		{
		}
		
		
		/// <summary>
		/// Create a new task collection.
		/// </summary>
		public TaskCollection(List<Task> tasks) : this(tasks,User.GetCurrentUserName(),DateTime.Now)
		{
		}
		
		
		/// <summary>
		/// Create a new task collection.
		/// </summary>
		public TaskCollection(List<Task> tasks, string creator, DateTime created)
		{
			this.tasks = tasks;
			this.creator = creator;
			this.created = created;
		}
	}
}
