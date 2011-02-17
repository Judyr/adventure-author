/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 07/03/2008
 * Time: 18:58
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace AdventureAuthor.Scripts
{
	/// <summary>
	/// An animation for a humanoid character to play on a line of dialogue.
	/// </summary>
	/// <remarks>Includes both looping and fire-once animations</remarks>
	public enum Animation { 
		// Looping:
		Pause = 0,
		Pause2 = 1,
		Listen = 2,
		Meditate = 3, 
		Worship = 4, 
		LookFar = 5,
//		SitInChair = 6,
		SitCrossLegged = 7,
		TalkNormal = 8,
		TalkPleading = 9,
		TalkForceful = 10,
		TalkLaughing = 11,
		GetLow = 12,
		GetMid = 13,
		PauseTired = 14,
		PauseDrunk = 15,
		DeadFront = 16,
		DeadBack = 17,
		Conjure1 = 18,
		Conjure2 = 19,
		Spasm = 20,
//		Custom1 = 21,
//		Custom2 = 22,
		Cast1 = 23,
		Prone = 24,
		Kneel = 25,			
		Dance1 = 26, 
		Dance2 = 27, 
		Dance3 = 28,
		PlayGuitar = 29,
		IdleGuitar = 30,
		PlayFlute = 31,
		IdleFlute = 32,
		PlayDrum = 33,
		IdleDrum = 34,
		Cook1 = 35,
		Cook2 = 36,
		Craft = 37,
		Forge = 38,
		BoxCarry = 39,
		BoxIdle = 40,
		BoxHurried = 41,
		Lookown = 42,
		LookUp = 43,
		LookLeft = 44,
		LookRight = 45,
		Shoveling = 46,
		Injured = 47,
		
		// One-time:
		TurnHeadLeft = 100,
		TurnHeadRight = 101,
		PauseScratchHead = 102,
		PauseBored = 103,
		Salute = 104,
		Bow = 105,
		Steal = 106,
		Greeting = 107,
		Taunt = 108,
		Victory1 = 109,
		Victory2 = 110,
		Victory3 = 111,
		Read = 112,
		Drink = 113,
		DodgeToSide = 114,
		Duck = 115,
//		Spasm = 116, // seems to be repeated
		Collapse = 117,
		LieDown = 118,
		StandUp = 119,
		Activate = 120,
		UseItem = 121,
		KneelFidget = 122,
		KneelTalk = 123,
		KneelDamage = 124,
		KneelDeath = 125,
		Sing = 126,
		FidgetWithGuitar = 127,
		FidgetWithFlute = 128,
		FidgetWithDrum = 129,
		Wildshape = 130,
		Search = 131,
		Intimidate = 132,
		Chuckle = 133			
	};
}
