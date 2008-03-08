/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 07/03/2008
 * Time: 19:00
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace AdventureAuthor.Scripts
{
	/// <summary>
	/// The location of a script resource; either in the scripts repository that comes
	/// with the game, or in the override folder (for Adventure Author scripts)
	/// </summary>
	public enum ScriptOrigin {
		Original, // included with game - located at Neverwinter Nights 2/Data/Scripts.zip on the main drive
		Override // included with Adventure Author - located at Neverwinter Nights 2/Override on the main drive
	}
}
