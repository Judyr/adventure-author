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
using Netron.Diagramming.Core;

namespace Netron.Diagramming.Win.AdventureAuthor
{
	public interface IPageNode : IShape
	{		
		/// <summary>
		/// The text to display on a tool tip if the mouse is hovering over this node.
		/// <remarks>This is necessary because the class does not derive from Control - rather
		/// it tells the parent control what to display on its tooltip.</remarks>
		/// </summary>
		string ToolTipText { get; }
		
		/// <summary>
		/// Get the route between the selected node and the root, in the form of a list of nodes and edges.
		/// </summary>
		/// <returns>A list of INode objects representing the route between the selected node and the root. Empty if called on root.</returns>
		List<IDiagramEntity> GetRoute();
	}
}
