/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 11/12/2007
 * Time: 21:22
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Text;
using System.Collections.Generic;
using NWN2Toolset.NWN2.Data.Instances;

namespace AdventureAuthor.Analysis
{
	/// <summary>
	/// A group of one or more creatures who will engage the player in combat.
	/// </summary>
	public class HostileGroup
	{
		/// <summary>
		/// Creatures in this group, sorted by ascending(?) order of challenge rating
		/// </summary>
		private SortedList<float,NWN2CreatureInstance> members; 	
		
		
		/// <summary>
		/// The creature with the highest (or joint highest) challenge rating
		/// </summary>
		public NWN2CreatureInstance Strongest {
			get {
				return members.Values[members.Count-1]; 
			}
		}
		
		
		/// <summary>
		/// Return the total challenge rating of this group of creatures
		/// </summary>
		/// <remarks>Currently this simply calculates the total challenge rating over all creatures within the group</remarks>
		public float TotalChallengeRating {
			get {
				float difficulty = 0;
				foreach (float creatureDifficulty in members.Keys) {
					difficulty += creatureDifficulty;
				}
				return difficulty;
			}
		}
		
		
		/// <summary>
		/// Return the average challenge rating of a creature from this group
		/// </summary>
		public float AverageChallengeRating {
			get {
				return TotalChallengeRating / members.Count;
			}
		}
				
		
		/// <summary>
		/// Create a new collection of hostile creatures.
		/// </summary>
		/// <param name="creature">The first creature to be added to the group</param>
		public HostileGroup(NWN2CreatureInstance creature)
		{
			if (creature == null) {
				throw new ArgumentNullException("A hostile group must contain at least one creature.");
			}
			
			members = new SortedList<float,NWN2CreatureInstance>(1);
			AddCreature(creature);
		}
		
		
		/// <summary>
		/// Add a creature to the group.
		/// </summary>
		/// <param name="creature">The creature to add</param>
		public void AddCreature(NWN2CreatureInstance creature)
		{
			//float difficulty = Combat.ChallengeRatingCalculator.calculateChallengeRating(creature,100);
			//members.Add(7,creature);
		}
	}
}
