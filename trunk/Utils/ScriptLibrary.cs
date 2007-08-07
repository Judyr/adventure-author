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
using System.IO;
using AdventureAuthor.Core;
using NWN2Toolset.NWN2.Data;
using OEIShared.IO;
using OEIShared.Utils;

namespace AdventureAuthor.Utils
{
	/// <summary>
	/// Description of ScriptLibrary
	/// </summary>
	public static class ScriptLibrary
	{		
		/// <summary>
		/// Retrieves a compiled (.NCS) script of a given name/resref.
		/// </summary>
		/// <param name="name">The name/resref of the script, e.g. ga_attack (no file extension)</param>
		/// <returns>Returns the compiled script, or null if no script was found.</returns>
		public static NWN2GameScript GetScript(string name)
		{
			try {
				string scriptsPath = Path.Combine(ResourceManager.Instance.BaseDirectory,@"Data\Scripts.zip");
				ResourceRepository scripts = (ResourceRepository)ResourceManager.Instance.GetRepositoryByName(scriptsPath);
				ushort ncs1 = BWResourceTypes.GetResourceType("ncs");
				OEIResRef resRef = new OEIResRef(name);
				IResourceEntry entry = scripts.FindResource(resRef,ncs1);
				NWN2GameScript scrp = new NWN2GameScript(entry);			
				scrp.Demand();
				return scrp;
			}
			catch (NullReferenceException e) {
				Say.Error("Was unable to retrieve script named '" + name + "'.",e);
				return null;
			}
		}
	}
}


