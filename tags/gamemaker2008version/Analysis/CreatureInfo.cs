/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 18/12/2007
 * Time: 20:49
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using AdventureAuthor.Analysis;
using NWN2Toolset.NWN2.Data.Instances;

namespace AdventureAuthor.Analysis
{
	/// <summary>
	/// Struct containing basic information about a creature for interface display.
	/// </summary>
	public struct CreatureInfo
	{
		public string Name;
		public string Tag;
		public string FactionName;
		public float ChallengeRating;
		
		public CreatureInfo(NWN2CreatureInstance creature)
		{
			Name = creature.FirstName.ToString();
			Tag = creature.Tag;
			FactionName = Combat.GetFactionName(creature.FactionID);
			ChallengeRating = Combat.GetChallengeRating(creature);			
		}
	}
}
