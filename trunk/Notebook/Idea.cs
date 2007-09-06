/*
 *   This file is part of Adventure Author.
 *
 *   Adventure Author is copyright Heriot-Watt University 2006-2007.
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
using System.Collections.Generic;

namespace AdventureAuthor.Notebook
{
	public class Idea
	{
		// private int id;
		public string title;
		public string author;
		public string description;
		public int stars; // a rating of between 0 and 5 stars
		public List<Comment> comments;
		// private List<int> connectedIdeas = new List<int>(); // ids of connected ideas
		
		public Idea()
		{
			this.title = String.Empty;
			this.author = String.Empty;
			this.description = String.Empty;
			this.stars = 0;
			this.comments = new List<Comment>();
		}
		
		public Idea(string title, string author)
		{
			this.title = title;
			this.author = author;
			this.description = String.Empty;
			this.stars = 0;
			this.comments = new List<Comment>();
		}
		
		public Idea(string title, string author, string description)
		{
			this.title = title;
			this.author = author;
			this.description = description;
			this.stars = 0;
			this.comments = new List<Comment>();
		}
	}
}
