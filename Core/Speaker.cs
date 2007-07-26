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
 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace AdventureAuthor.Core
{
	/// <summary>
	/// Description of Speaker.
	/// </summary>
	public class Speaker
	{		
		private string tag;		
		public string Tag {
			get { return tag; }
			set { tag = value; }
		}	
		
		private string displayName;		
		public string DisplayName {
			get { return displayName; }
			set { displayName = value; }
		}
		
		private Brush colour;				
		public Brush Colour {
			get { return colour; }
			set { colour = value; }
		}
				
		public Speaker(string tag, string name)
		{
			this.tag = tag;
			this.displayName = name;
			this.colour = Brushes.Black;
		}
		
		public override string ToString()
		{
			return displayName.ToUpper();
		}
	}
}
