/*
 *   This file is part of Adventure Author.
 *
 *   Adventure Author is copyright Heriot-Watt University 2006-2008.
 *
 *   This copyright and licence apply to all source code, compiled code,
 *   documentation, graphics and auxiliary files, except where otherwise stated.
 *
 *   Adventure Author is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 *   Adventure Author is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 * 
 *   Adventure Author is a plugin for Atari's Neverwinter Nights 2, a COMMERCIAL
 *   product. Permission is given to link this GPL-covered plug-in with the 
 *   non-free main program. 
 *
 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;

namespace AdventureAuthor.Tasks
{
	/// <summary>
	/// Create a criterion which represents some aspect of what an ITaskGenerator
	/// is checking for when generating tasks.
	/// </summary>
	public class Criterion
	{
		#region Properties and fields
		
		/// <summary>
		/// The title of this criterion.
		/// </summary>
		/// <remarks>An example would be 'Check for broken area transitions'.</remarks>
		private string title;
		public string Title {
			get { return title; }
			set { title = value; }
		}
		
		
		/// <summary>
		/// A description of this criterion.
		/// </summary>
		/// <remarks>An example would be 'Check whether any area transitions have been set up
		/// which lead nowhere or which have the wrong exit type.'</remarks>
		private string description;
		public string Description {
			get { return description; }
			set { description = value; }
		}
		
		
		/// <summary>
		/// True to consider this criterion when generating tasks; false
		/// if this criterion should be ignored when generating tasks.
		/// </summary>
		private bool include;
		public bool Include {
			get { return include; }
			set { include = value; }
		}
		
		
		/// <summary>
		/// Options which relate to the running of this criterion.
		/// </summary>
		/// <remarks>If you want to set options for a particular criterion, create a new class which
		/// has those options as properties - the field can then be represented as a PropertyGrid
		/// to allow the user to set options.
		/// An example of an option would be a list of game areas to ignore when running
		/// this criterion.</remarks>
		private object options;
		public object Options {
			get { return options; }
			set { options = value; }
		}
		
		#endregion
		
		#region Constructors
		
		/// <summary>
		/// For serialization purposes only.
		/// </summary>
		private Criterion() : this(null,null)
		{
		}
		
		
		/// <summary>
		/// Create a criterion which represents some aspect of what an ITaskGenerator
		/// is checking for when generating tasks.
		/// </summary>
		/// <param name="title">The title of this criterion</param>
		/// <param name="description">A description of this criterion</param>
		public Criterion(string title, string description) : this(title,description,false,null)
		{
		}
		
		
		/// <summary>
		/// Create a criterion which represents some aspect of what an ITaskGenerator
		/// is checking for when generating tasks.
		/// </summary>
		/// <param name="title">The title of this criterion</param>
		/// <param name="description">A description of criterion check</param>
		/// <param name="include">True to consider this criterion when generating tasks; false
		/// if this criterion should be ignored when generating tasks.</param>
		/// <param name="options">Options which relate to the running of this criterion.</param>
		public Criterion(string title, string description, bool include, object options)
		{
			this.title = title;
			this.description = description;
			this.include = include;
			this.options = options;
		}
		
		#endregion
	}
}
