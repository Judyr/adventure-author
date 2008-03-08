/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 08/03/2008
 * Time: 08:31
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace AdventureAuthor.Scripts
{
	/// <summary>
	/// Cutscene movies included with Neverwinter Nights 2, which could be re-used
	/// via scripting (mainly for the purpose of making the game seem more
	/// 'professional', rather than for storytelling). 
	/// </summary>
	/// <remarks>All are stored as .bik files in Neverwinter Nights 2/Movies</remarks>
	public enum Movie { 
		AtariLogo,
		Credits,
		Intro,
		Legal,
		NvidiaLogo,
		OEIlogo,
		WOTCLogo
	}
}
