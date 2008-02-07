﻿/*
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
using AdventureAuthor.Evaluation.Viewer;

namespace AdventureAuthor.Evaluation.Viewer
{
	/// <summary>
	/// Arguments to accompany the event of a request for a worksheet part to be deleted.
	/// </summary>
	public class DeletingEventArgs : EventArgs
	{
		/// <summary>
		/// The control to be deleted.
		/// </summary>
		private OptionalWorksheetPartControl deletingControl;			
		public OptionalWorksheetPartControl DeletingControl {
			get { return deletingControl; }
		}
		
		
		/// <summary>
		/// The owner of the control to be deleted.
		/// </summary>
		private OptionalWorksheetPartControl parentControl;		
		public OptionalWorksheetPartControl ParentControl {
			get { return parentControl; }
		}
		
		
		/// <summary>
		/// Create a new DeletingEventArgs.
		/// </summary>
		/// <param name="deletingControl">The control to be deleted</param>
		public DeletingEventArgs(OptionalWorksheetPartControl deletingControl) : this(deletingControl,null)
		{
		}
		
		
		/// <summary>
		/// Create a new DeletingEventArgs.
		/// </summary>
		/// <param name="deletingControl">The control to be deleted</param>
		/// <param name="parentControl">The owner of the control to be deleted</param>
		public DeletingEventArgs(OptionalWorksheetPartControl deletingControl, OptionalWorksheetPartControl parentControl)
		{
			this.deletingControl = deletingControl;
			this.parentControl = parentControl;
		}
	}
}
