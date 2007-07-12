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
using NWN2Toolset.NWN2.IO;

namespace AdventureAuthor
{
	/// <summary>
	/// Thrown when the files containing serialized Adventure/Chapter etc. data cannot be found.
	/// Note that this will happen with NWN2 modules that have not been made using Adventure Author.
	/// </summary>
	public class MissingAdventureSerializedDataException: Exception
	{
		public MissingAdventureSerializedDataException() {}
		
		// Note: Could just use a FileNotFoundException, but it should be made clear that this is something 
		// *Adventure Author* is missing: any NWN2 module made with the standard toolset would throw this exception,
		// as well as ill-formed Adventures.
	}
	
	/// <summary>
	/// Thrown when a module cannot be found.
	/// </summary>
	public class AdventureIsMissingModuleDirectoryException: Exception
	{
		public AdventureIsMissingModuleDirectoryException() {}
	}
	
	/// <summary>
	/// Thrown when an invalid resource name is used (was invalid length or contained invalid characters.)
	/// </summary>
	public class InvalidNameException : Exception
	{
		string name;		
		public InvalidNameException(string name) {
			this.name = name;
		}
	}
	
	/// <summary>
	/// Thrown when a module has a ModuleLocationType other than Directory, i.e. Temporary or File.
	/// Note that this will happen with NWN2 modules that have not been made using Adventure Author.
	/// </summary>
	public class InvalidModuleLocationTypeException : Exception
	{
		ModuleLocationType moduleLocationType;		
		public InvalidModuleLocationTypeException(ModuleLocationType mlt) {
			this.moduleLocationType = mlt;
		}
	}
	
	/// <summary>
	/// Thrown when a module is found to be missing a resource (i.e. area, conversation or script) that it should have.
	/// </summary>
	public class MissingModuleResourceException : Exception
	{
		public MissingModuleResourceException() {}
	}
			
	/// <summary>
	/// Thrown if the toolset fails to open the Scratchpad due to the max number of area viewers being reached.
	/// This should never happen.
	/// </summary>
	public class ScratchpadCouldNotBeOpenedException : Exception
	{
		public ScratchpadCouldNotBeOpenedException() {}
	}
	
	/// <summary>
	/// Thrown when a call to Chapter.Open() or Scratchpad.Open() is made, when their owning Adventure is not
	/// currently open in the toolset.
	/// </summary>
	public class TriedToOpenChapterInClosedAdventure : Exception
	{
		public TriedToOpenChapterInClosedAdventure() {}
	}
	
	public class DuplicatedTagException : Exception
	{
		public DuplicatedTagException() {}
	}
	
	/// <summary>
	/// Thrown when a line that is marked as filler contains text (and will therefore be displayed in-game)
	/// </summary>
	public class BadFillerLineException : Exception
	{
		string message;
			
		public BadFillerLineException(string message)
		{
			this.message = message;
		}		
	}
	
	public class BadBranchException : Exception
	{
		string message;
		
		public BadBranchException(string message)
		{
			this.message = message;
		}
	}
}
