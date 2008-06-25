/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 08/03/2008
 * Time: 12:15
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace AdventureAuthor.Utils
{
	/// <summary>
	/// Different types of action a user an take when interacting with the 
	/// interface. For the purposes of logging.
	/// </summary>
	public enum LogAction {
		// applications or mini-applications e.g. conversationwriter, expandedgraph:
		launched,
		exited,
		
		// documents etc.:
		opened,
		closed,
		exported,
		
		// other objects:
		added,
		edited,
		deleted,
		saved,
		set,
		moved,
		viewed,		
		
		// entered a mode (e.g. toolset_SelectObjects, toolset_PaintTerrain, commentcard_Design):
		mode,
		
		// selected just means giving it focus:
		selected,
			
		// deliberately non-specific, for when no further information has been gathered, e.g. when the user
		// clicks something in the terrain editor window or area contents window 
		clicked,
		
		// relating to Comment Cards designer:
		activated,
		deactivated,
		
		// relating to magnets:
		placed,
		removed,
		hid,
		showed,
		starred,
		unstarred
	}
}
