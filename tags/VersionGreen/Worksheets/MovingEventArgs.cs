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
using AdventureAuthor.Evaluation;

namespace AdventureAuthor.Evaluation
{
	/// <summary>
	/// Arguments to accompany the event of a request for a Comment Card part to be moved to a different position.
	/// </summary>
	public class MovingEventArgs : CardPartControlEventArgs
	{
		/// <summary>
		/// True if the control is requesting a move up; false if the control is requesting a move down.
		/// </summary>
		private bool moveUp;		
		public bool MoveUp {
			get { return moveUp; }
		}
		
		
		/// <summary>
		/// Create a new MovingEventArgs.
		/// </summary>
		/// <param name="movingControl">The control to be moved</param>
		/// <param name="moveUp">True if the control is requesting a move up; false if it's requesting a move down</param>
		/// <param name="parentControl">The owner of the control to be moved</param>
		public MovingEventArgs(CardPartControl control, 
		                       bool moveUp) : base(control)
		{
			this.moveUp = moveUp;
		}
	}
}
