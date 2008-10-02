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
	/// Description of Check.
	/// </summary>
	public class Check
	{
		/// <summary>
		/// The title of this check.
		/// </summary>
		/// <remarks>An example would be 'Check for broken area transitions'.</remarks>
		private string title;
		public string Title {
			get { return title; }
			set { title = value; }
		}
		
		
		/// <summary>
		/// A description of this check.
		/// </summary>
		/// <remarks>An example would be 'Check whether any area transitions have been set up
		/// which lead nowhere or which have the wrong exit type.'</remarks>
		private string description;
		public string Description {
			get { return description; }
			set { description = value; }
		}
		
		
		/// <summary>
		/// True if the ITaskGenerator which owns this object should consider
		/// the factors represented by this Check when it is asked to generate tasks.
		/// </summary>
		private bool shouldRun;
		public bool ShouldRun {
			get { return shouldRun; }
			set { shouldRun = value; }
		}
		
		
		/// <summary>
		/// Options which relate to the running of this Check.
		/// </summary>
		/// <remarks>If you want to set options for a particular check, create a new class which
		/// has those options as properties - the field can then be represented as a PropertyGrid
		/// to allow the user to set options.
		/// An example of an option would be a list of game areas to ignore when running
		/// this Check.</remarks>
		private object options;
		public object Options {
			get { return options; }
			set { options = value; }
		}
		
		
		private Check() : this(null,null)
		{
		}
		
		
		public Check(string title, string description)
		{
			this.title = title;
			this.description = description;
			this.shouldRun = false;
			this.options = null;
		}
	}
}
