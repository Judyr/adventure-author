/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 29/07/2008
 * Time: 12:30
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;

namespace AdventureAuthor.Tasks
{
	/// <summary>
	/// A collection of tasks for a task manager application. 
	/// </summary>
	public class TaskCollection
	{
		/// <summary>
		/// The user who created this task collection.
		/// </summary>
		private string creator;
		public string Creator {
			get { return creator; }
			set { creator = value; }
		}
		
		
		/// <summary>
		/// The time this task collection was created.
		/// </summary>
		private DateTime created;
		public DateTime Created {
			get { return created; }
		}
		
		
		/// <summary>
		/// A list of every task in this collection. 
		/// </summary>
		private List<Task> tasks;
		public List<Task> Tasks {
			get { return tasks; }
		}
		
		
		public TaskCollection()
		{
		}
	}
}
