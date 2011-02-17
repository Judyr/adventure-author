/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 19/02/2008
 * Time: 13:32
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace AdventureAuthor.Conversations
{
	/// <summary>
	/// Format to export text files in. Tree format - a staggered conversation tree, as in the
	/// original NWN1 and NWN2 conversation editors. Page format - a 'Choose Your Own Adventure'
	/// conversation split into pages, as in the Adventure Author conversation editor.
	/// </summary>
	public enum ExportFormat {
		TreeFormat,
		PageFormat
	}
}
