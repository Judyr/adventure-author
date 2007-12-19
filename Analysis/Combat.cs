/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 12/12/2007
 * Time: 12:00
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.TypedCollections;
using NWN2Toolset.NWN2.Data.Instances;
using NWN2Toolset.NWN2.Data.Templates;
using NWN2PluginToolsLibrary;
using AdventureAuthor.Analysis;

namespace AdventureAuthor.Analysis
{
	/// <summary>
	/// Provides methods for analysing various aspects of combat in a module.
	/// </summary>
	public static class Combat
	{
		#region Constants
		
		public const ushort HOSTILE = 1;
		public const ushort COMMONER = 2;
		
		#endregion
		
		/// <summary>
		/// Calculates the challenge rating of a creature object/blueprint.
		/// </summary>
		/// <remarks>From GrinningFool's NWN2PluginToolsLibrary</remarks>
		private static CreatureCRCalculator challengeRatingCalculator = new CreatureCRCalculator();		
				
		
		/// <summary>
		/// Get the challenge rating of a creature.
		/// </summary>
		/// <remarks>Can pass either an object or a blueprint</remarks>
		public static float GetChallengeRating(NWN2CreatureTemplate creature)
		{			
			return challengeRatingCalculator.calculateChallengeRating(creature,100);
		}
		
		
		/// <summary>
		/// Returns a list of hostile groups (containing one or more enemies in close proximity) in this area
		/// </summary>
		/// <param name="area">The area containing enemies to be collected into hostile groups</param>
		/// <returns>A list of hostile groups in this area</returns>
		/// <remarks>Currently only accounts for creatures who are hostile when the module begins - further things to
		/// consider include hostile encounters, creatures who turn hostile or attack, and spawned hostile creatures.</remarks>
		public static List<HostileGroup> CollectHostileGroups(NWN2GameArea area)
		{
			List<HostileGroup> groups = new List<HostileGroup>(15);
			
			// Find and add groups of creatures who are set to hostile:
			
			
			// Add a group representing every hostile encounter:
			foreach (NWN2EncounterInstance encounter in area.Encounters) {
				
			}
			
			// Add a group representing every hostile spawn-in:
			
			
			// Add a group representing every non-hostile creature who MUST turn hostile from a conversation:
			
			
			return groups;
		}		
		
		
		public static string GetFactionName(ushort factionID)
		{
			switch (factionID) {
				case 0:
					return "?0?";
				case 1:
					return "Hostile";
				case 2:
					return "Commoner";
				case 3:
					return "?3?";
				default:
					return "Faction ID: " + factionID.ToString();
			}
		}
	}
}
