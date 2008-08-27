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
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Tasks
{
	/// <summary>
	/// A collection of tasks for a task manager application. 
	/// </summary>
	[Serializable]
	public class TaskCollection : ObservableCollection<Task>
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
		/// Create a new task collection.
		/// </summary>
		public TaskCollection() : this(User.GetCurrentUserName(),DateTime.Now)
		{
		}
		
		
		/// <summary>
		/// Create a new task collection.
		/// </summary>
		public TaskCollection(string creator, DateTime created) : base()
		{
			this.creator = creator;
			this.created = created;
		}		
	}
}
