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
using System.Windows.Controls;
using AdventureAuthor.Core;
using AdventureAuthor.Scripts;
using AdventureAuthor.Scripts.UI;
using AdventureAuthor.Utils;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.Journal;
using form = NWN2Toolset.NWN2ToolsetMainForm;

namespace AdventureAuthor.Conversations.UI.Controls
{
    public partial class LineControl : UserControl
    {    	
    	private void OnClick_AdvanceTime(object sender, EventArgs ea)
    	{
    		object[] prms = new object[1];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms, "Move the time of day forwards");
    		window.AddIntegerQuestion("Advance time by how many hours?",0,null);
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ScriptFunctor action = Actions.AdvanceTimeBy((int)prms[0]);
    			Conversation.CurrentConversation.AddAction(nwn2Line,action);
    		}
    	}
    	
    	private void OnClick_Attack(object sender, EventArgs ea)
    	{    		
    		object[] prms = new object[1];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms, "Attack the player");
    		window.AddTagQuestion("Who should attack the player?",ScriptHelper.TaggedType.Creature);
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ScriptFunctor action = Actions.Attack((string)prms[0]);
    			Conversation.CurrentConversation.AddAction(nwn2Line,action);
    		}
    	}
    	
    	private void OnClick_AttackTarget(object sender, EventArgs ea)
    	{    		
    		object[] prms = new object[2];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms, "Make a creature attack another creature");
    		window.AddTagQuestion("Who should be the attacker?",ScriptHelper.TaggedType.Creature);
    		window.AddTagQuestion("Who should be the victim?",ScriptHelper.TaggedType.Creature);
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ScriptFunctor action = Actions.AttackTarget((string)prms[0],(string)prms[1]);
    			Conversation.CurrentConversation.AddAction(nwn2Line,action);
    		}
    	}
        
    	private void OnClick_AddHenchmanForPlayer(object sender, EventArgs ea)
    	{    	
    		object[] prms = new object[2];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Give the player an ally");
    		window.AddTagQuestion("Who should become the player's ally?",ScriptHelper.TaggedType.Creature);
    		window.AddBooleanQuestion("Should they follow the player around and fight for him?");
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ScriptFunctor action = Actions.AddHenchmanForPlayer((string)prms[0],(int)prms[1]);
    			Conversation.CurrentConversation.AddAction(nwn2Line,action);
    		}
    	}  
    	    	
    	private void OnClick_AddHenchmanForCreature(object sender, EventArgs ea)
    	{    	
    		object[] prms = new object[3];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Give someone else an ally");
    		window.AddTagQuestion("Who should become the ally?",ScriptHelper.TaggedType.Creature);
    		window.AddTagQuestion("Whose ally should they become?",ScriptHelper.TaggedType.Creature);
    		window.AddBooleanQuestion("Should they follow their master around and fight for them?");
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    		
    			NWN2ScriptFunctor action = Actions.AddHenchmanForCreature((string)prms[0],(string)prms[1],(int)prms[2]);
    			Conversation.CurrentConversation.AddAction(nwn2Line,action);
    		}
    	}    
    	    	
    	private void OnClick_CloseDoor(object sender, EventArgs ea)
    	{    	
    		object[] prms = new object[1];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Close a door");
    		window.AddTagQuestion("Which door should be closed?",ScriptHelper.TaggedType.Door);
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    		
    			NWN2ScriptFunctor action = Actions.CloseDoor((string)prms[0]);
    			Conversation.CurrentConversation.AddAction(nwn2Line,action);
    		}
    	}    
    	
    	private void OnClick_CreateCreature(object sender, EventArgs ea)
    	{    	
    		object[] prms = new object[5];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Create a creature");
    		window.AddResRefQuestion("What template should the creature be created from?",ScriptHelper.TaggedType.Creature); //0th
    		window.AddStringQuestion("What tag should the new creature be given?"); //3rd
    		window.AddTagQuestion("What waypoint should the creature appear at?",ScriptHelper.TaggedType.Waypoint); //1st
    		window.AddBooleanQuestion("Should an 'appearing' animation be played when it is created?"); //2nd
    		window.AddFloatQuestion("How many seconds should pass before doing this (if any)?",0,null); //4th
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    		
    			NWN2ScriptFunctor action = Actions.CreateCreature((string)prms[0],(string)prms[2],(int)prms[3],(string)prms[1],(float)prms[4]);
    			Conversation.CurrentConversation.AddAction(nwn2Line,action);
    		}
    	}    	
    	    	    	
    	private void OnClick_CreateItem(object sender, EventArgs ea)
    	{    	
    		object[] prms = new object[5];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Create an item");
    		window.AddResRefQuestion("What template should the item be created from?",ScriptHelper.TaggedType.Item); //0th
    		window.AddStringQuestion("What tag should the new item be given?"); //3rd
    		window.AddTagQuestion("What waypoint should the item appear at?",ScriptHelper.TaggedType.Waypoint); //1st
    		window.AddBooleanQuestion("Should an 'appearing' animation be played when it is created?"); //2nd
    		window.AddFloatQuestion("How many seconds should pass before doing this (if any)?",0,null); //4th
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    		
    			NWN2ScriptFunctor action = Actions.CreateItem((string)prms[0],(string)prms[2],(int)prms[3],(string)prms[1],(float)prms[4]);
    			Conversation.CurrentConversation.AddAction(nwn2Line,action);
    		}
    	}    	
    	    	    	
    	private void OnClick_CreatePlaceable(object sender, EventArgs ea)
    	{    	
    		object[] prms = new object[5];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Create a placeable");
    		window.AddResRefQuestion("What template should the placeable be created from?",ScriptHelper.TaggedType.Placeable); //0th
    		window.AddStringQuestion("What tag should the new placeable be given?"); //3rd
    		window.AddTagQuestion("What waypoint should the placeable appear at?",ScriptHelper.TaggedType.Waypoint); //1st
    		window.AddBooleanQuestion("Should an 'appearing' animation be played when it is created?"); //2nd
    		window.AddFloatQuestion("How many seconds should pass before doing this (if any)?",0,null); //4th
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    		
    			NWN2ScriptFunctor action = Actions.CreatePlaceable((string)prms[0],(string)prms[2],(int)prms[3],(string)prms[1],(float)prms[4]);
    			Conversation.CurrentConversation.AddAction(nwn2Line,action);
    		}
    	}    		
    	    	    	
    	private void OnClick_CreateStore(object sender, EventArgs ea)
    	{    	
    		object[] prms = new object[5];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Create a placeable");
    		window.AddResRefQuestion("What template should the store be created from?",ScriptHelper.TaggedType.Store); //0th
    		window.AddStringQuestion("What tag should the new store be given?"); //3rd
    		window.AddTagQuestion("What waypoint should the store appear at?",ScriptHelper.TaggedType.Waypoint); //1st
    		window.AddBooleanQuestion("Should an 'appearing' animation be played when it is created?"); //2nd
    		window.AddFloatQuestion("How many seconds should pass before doing this (if any)?",0,null); //4th
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    		
    			NWN2ScriptFunctor action = Actions.CreateStore((string)prms[0],(string)prms[2],(int)prms[3],(string)prms[1],(float)prms[4]);
    			Conversation.CurrentConversation.AddAction(nwn2Line,action);
    		}
    	}    		
    	    	    	
    	private void OnClick_CreateWaypoint(object sender, EventArgs ea)
    	{    	
    		// NB: Note that this appears useless since you have to nominate a waypoint for the new waypoint to appear at -
    		// however it is possible to e.g. create a permanent waypoint at the current location of a player or monster,
    		// which may be useful in future. Currently however the interface just takes a waypoint tag, so this is of no use.
    		object[] prms = new object[5];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Create a waypoint");
    		window.AddResRefQuestion("What template should the waypoint be created from?",ScriptHelper.TaggedType.Waypoint); //0th
    		window.AddStringQuestion("What tag should the new waypoint be given?"); //3rd
    		window.AddTagQuestion("What waypoint should the waypoint appear at?",ScriptHelper.TaggedType.Waypoint); //1st
    		window.AddBooleanQuestion("Should an 'appearing' animation be played when it is created?"); //2nd
    		window.AddFloatQuestion("How many seconds should pass before doing this (if any)?",0,null); //4th
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    		
    			NWN2ScriptFunctor action = Actions.CreateWaypoint((string)prms[0],(string)prms[2],(int)prms[3],(string)prms[1],(float)prms[4]);
    			Conversation.CurrentConversation.AddAction(nwn2Line,action);
    		}
    	}    
    	    	
    	
    	private void OnClick_CreatureMoves(object sender, EventArgs ea)
    	{    	
    		object[] prms = new object[4];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Tell a creature to move somewhere");
    		window.AddTagQuestion("Which creature should move?",ScriptHelper.TaggedType.Creature);
    		window.AddTagQuestion("Which waypoint should they move to?",ScriptHelper.TaggedType.Waypoint);
    		window.AddBooleanQuestion("Should the creature walk or run?","Run","Walk");
    		window.AddBooleanQuestion("Should the creature vanish once it gets there?");
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    		
    			NWN2ScriptFunctor action = Actions.CreatureMoves((string)prms[0],(string)prms[1],(int)prms[2],(int)prms[3]);
    			Conversation.CurrentConversation.AddAction(nwn2Line,action);
    		}
    	}  
    	
    	
    	    	
    	private void OnClick_DestroyObject(object sender, EventArgs ea)
    	{    		
    		object[] prms = new object[2];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Destroy an object");
    		window.AddTagQuestion("What object should be destroyed?",ScriptHelper.TaggedType.AnyObject);
    		window.AddFloatQuestion("How many seconds should pass before doing this (if any)?",0,null);    		
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    		
    			NWN2ScriptFunctor action = Actions.Destroy((string)prms[0],(float)prms[1]); 
    			Conversation.CurrentConversation.AddAction(nwn2Line,action);
    		}
    	}    
    	    	    	
    	private void OnClick_DisplayMessage(object sender, EventArgs ea)
    	{    		
    		object[] prms = new object[1];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Display a message");
    		window.AddStringQuestion("What message should be displayed?");
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    		
    			NWN2ScriptFunctor action = Actions.DisplayMessage((string)prms[0]);
    			Conversation.CurrentConversation.AddAction(nwn2Line,action);
    		}
    	}      	
    	
    	private void OnClick_DestroyAllHenchmen(object sender, EventArgs ea)
    	{    		
    		NWN2ScriptFunctor action = Actions.DestroyAllHenchmen();
    		Conversation.CurrentConversation.AddAction(nwn2Line,action);
    	}  
    	    	    	
    	private void OnClick_EndGame(object sender, EventArgs ea)
    	{
    		object[] prms = new object[1];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"End the game");
    		window.AddEnumQuestion("What movie should play when the game ends?",typeof(ScriptHelper.Movie));
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ScriptFunctor action = Actions.EndGame(((ScriptHelper.Movie)prms[0]).ToString());
    			Conversation.CurrentConversation.AddAction(nwn2Line,action);
    		}
    	}
    	
    	private void OnClick_FadeOut(object sender, EventArgs ea)
    	{
    		object[] prms = new object[3];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Fade out");
    		window.AddEnumQuestion("What colour should the screen fade to?",typeof(ScriptHelper.FadeColour));//2nd
    		window.AddFloatQuestion("How many seconds should it take to fade out?",0,60);//0th
    		window.AddFloatQuestion("How many seconds should pass before fading in again?",0,60);//1st
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ScriptFunctor action = Actions.FadeOut((float)prms[1],(float)prms[2],(ScriptHelper.FadeColour)prms[0]);
    			Conversation.CurrentConversation.AddAction(nwn2Line,action);
    		}
    	}
    	
    	private void OnClick_FadeIn(object sender, EventArgs ea)
    	{
    		object[] prms = new object[1];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Fade in");
    		window.AddFloatQuestion("How many seconds should it take to fade in completely?",0,60);
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ScriptFunctor action = Actions.FadeIn((float)prms[0]);
    			Conversation.CurrentConversation.AddAction(nwn2Line,action);
    		}
    	}
    	
    	private void OnClick_GivePlayerFeat(object sender, EventArgs ea)
    	{
    		object[] prms = new object[1];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Give the player a special ability");
    		window.AddEnumQuestion("Which special ability should the player learn?",typeof(ScriptHelper.Feat));
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ScriptFunctor action = Actions.GivePlayerFeat((ScriptHelper.Feat)prms[0]);
    			Conversation.CurrentConversation.AddAction(nwn2Line,action);
    		}
    	}   
    	
    	private void OnClick_GiveCreatureFeat(object sender, EventArgs ea)
    	{
    		object[] prms = new object[1];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Give a creature a special ability");
    		window.AddTagQuestion("Which creature should learn the special ability?",ScriptHelper.TaggedType.Creature);
    		window.AddEnumQuestion("Which special ability should the creature learn?",typeof(ScriptHelper.Feat));
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ScriptFunctor action = Actions.GiveCreatureFeat((string)prms[0],(ScriptHelper.Feat)prms[1]);
    			Conversation.CurrentConversation.AddAction(nwn2Line,action);
    		}
    	}   
    	      	
    	private void OnClick_GiveGold(object sender, EventArgs ea)
    	{
    		object[] prms = new object[1];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Give the player gold");
    		window.AddIntegerQuestion("How much gold should the player receive?",0,null);
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ScriptFunctor action = Actions.GiveGold((int)prms[0]);
    			Conversation.CurrentConversation.AddAction(nwn2Line,action);
    		}
    	}   
    	    	
    	private void OnClick_GiveItem(object sender, EventArgs ea)
    	{
    		object[] prms = new object[2];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Give the player an item");
    		window.AddResRefQuestion("What kind of item should the player receive?",ScriptHelper.TaggedType.Item);
    		window.AddIntegerQuestion("How many copies of the item should the player receive?",0,null);
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ScriptFunctor action = Actions.GiveItem((string)prms[0],(int)prms[1]);
    			Conversation.CurrentConversation.AddAction(nwn2Line,action);
    		}
    	}	
    	
    	private void OnClick_GiveExperience(object sender, EventArgs ea)
    	{
    		object[] prms = new object[2];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Give the player experience points");
    		window.AddIntegerQuestion("How many experience points should the player receive?",0,1000000);
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ScriptFunctor action = Actions.GiveExperience((int)prms[0]);
    			Conversation.CurrentConversation.AddAction(nwn2Line,action);
    		}
    	}
    	
    	private void OnClick_HealPlayer(object sender, EventArgs ea)
    	{
        	object[] prms = new object[1];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Heal the player");
    		window.AddIntegerQuestion("How much should the player be healed? (0-100%)",0,100);
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ScriptFunctor action = Actions.HealPC((int)prms[0]);
    			Conversation.CurrentConversation.AddAction(nwn2Line,action);
    		}
    	}
    	
    	private void OnClick_CreatureJoinsCommonerFaction(object sender, EventArgs ea)
    	{
        	object[] prms = new object[1];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Make a creature become a commoner");
    		window.AddTagQuestion("Which creature should become a commoner?",ScriptHelper.TaggedType.Creature);
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ScriptFunctor action = Actions.CreatureJoinsCommonerFaction((string)prms[0]);
    			Conversation.CurrentConversation.AddAction(nwn2Line,action);
    		}
    	}
    	    	
    	private void OnClick_CreatureJoinsDefenderFaction(object sender, EventArgs ea)
    	{
        	object[] prms = new object[1];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Make a creature become a defender");
    		window.AddTagQuestion("Which creature should become a defender?",ScriptHelper.TaggedType.Creature);
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ScriptFunctor action = Actions.CreatureJoinsDefenderFaction((string)prms[0]);
    			Conversation.CurrentConversation.AddAction(nwn2Line,action);
    		}
    	}
    	
    	private void OnClick_CreatureJoinsHostileFaction(object sender, EventArgs ea)
    	{
        	object[] prms = new object[1];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Make a creature become a hostile");
    		window.AddTagQuestion("Which creature should become a hostile?",ScriptHelper.TaggedType.Creature);
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ScriptFunctor action = Actions.CreatureJoinsHostileFaction((string)prms[0]);
    			Conversation.CurrentConversation.AddAction(nwn2Line,action);
    		}
    	}    		
    	    	
    	private void OnClick_TeleportCreature(object sender, EventArgs ea)
    	{
        	object[] prms = new object[3];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Move a creature instantly to another location");
    		window.AddTagQuestion("Which creature should be teleported?",ScriptHelper.TaggedType.Creature); // out of sequence deliberately
    		window.AddTagQuestion("Which waypoint should they appear at?",ScriptHelper.TaggedType.Waypoint); // out of sequence deliberately
    		window.AddFloatQuestion("How many seconds should pass before doing this (if any)?",0,null);    
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ScriptFunctor action = Actions.TeleportCreature((string)prms[1],(string)prms[0],(float)prms[2]);
    			Conversation.CurrentConversation.AddAction(nwn2Line,action);
    		}
    	}
    	    	
    	private void OnClick_TeleportObject(object sender, EventArgs ea)
    	{
        	object[] prms = new object[3];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Move an object instantly to another location");
    		window.AddTagQuestion("Which object should be teleported?",ScriptHelper.TaggedType.AnyObject); // out of sequence deliberately
    		window.AddTagQuestion("Which waypoint should they appear at?",ScriptHelper.TaggedType.Waypoint); // out of sequence deliberately
    		window.AddFloatQuestion("How many seconds should pass before doing this (if any)?",0,null);    
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ScriptFunctor action = Actions.TeleportObject((string)prms[1],(string)prms[0],(float)prms[2]);
    			Conversation.CurrentConversation.AddAction(nwn2Line,action);
    		}
    	}
    	    	
    	private void OnClick_TeleportPlayer(object sender, EventArgs ea)
    	{
        	object[] prms = new object[1];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Move the player instantly to another location");
    		window.AddTagQuestion("Which waypoint should the player appear at?",ScriptHelper.TaggedType.Waypoint);
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ScriptFunctor action = Actions.TeleportPlayer((string)prms[0]);
    			Conversation.CurrentConversation.AddAction(nwn2Line,action);
    		}
    	}
    	
    	
    	private void OnClick_Kill(object sender, EventArgs ea)
    	{
    		object[] prms = new object[1];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Kill a creature");
    		window.AddTagQuestion("Which creature should be killed?",ScriptHelper.TaggedType.Creature);
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ScriptFunctor action = Actions.Kill((string)prms[0]);
    			Conversation.CurrentConversation.AddAction(nwn2Line,action);
    		}
    	}
    	  
    	    	
    	private void OnClick_OpenDoor(object sender, EventArgs ea)
    	{    	
    		object[] prms = new object[1];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Open a door");
    		window.AddTagQuestion("Which door should be opened?",ScriptHelper.TaggedType.Door);
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    		
    			NWN2ScriptFunctor action = Actions.OpenDoor((string)prms[0]);
    			Conversation.CurrentConversation.AddAction(nwn2Line,action);
    		}
    	} 
    	
    	
    	private void OnClick_OpenStore(object sender, EventArgs ea)
    	{
    		object[] prms = new object[3];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Open a store");
    		window.AddTagQuestion("Which store should be opened?",ScriptHelper.TaggedType.Store);
    		window.AddIntegerQuestion("How much more expensive than normal should it be? (0-100%)",0,100);
    		window.AddIntegerQuestion("Or - how much cheaper than normal should it be? (0-100%)",0,100);
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ScriptFunctor action = Actions.OpenStore((string)prms[0],(int)prms[1],(int)prms[2]);
    			Conversation.CurrentConversation.AddAction(nwn2Line,action);
    		}
    	}
    	
    	private void OnClick_PlayerAnimation(object sender, EventArgs ea)
    	{
    		object[] prms = new object[2];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Player plays an animation");
    		window.AddEnumQuestion("Which animation should they perform?",typeof(ScriptHelper.Animation));
    		window.AddFloatQuestion("How long should they wait before performing it (in seconds)?",0,null);
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ScriptFunctor action = Actions.PlayerAnimation((ScriptHelper.Animation)prms[0],(float)prms[1]);
    			Conversation.CurrentConversation.AddAction(nwn2Line,action);
    		}
    	}
    	
    	private void OnClick_CreatureAnimation(object sender, EventArgs ea)
    	{
    		// Add a remark: "Note that many animations only work on humanoid creatures."
    		object[] prms = new object[3];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Creature plays an animation");
    		window.AddTagQuestion("Who should perform the animation?",ScriptHelper.TaggedType.Creature);
    		window.AddEnumQuestion("Which animation should they perform?",typeof(ScriptHelper.Animation));
    		window.AddFloatQuestion("How long should they wait before performing it (in seconds)?",0,null);
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ScriptFunctor action = Actions.CreatureAnimation((string)prms[0],(ScriptHelper.Animation)prms[1],(float)prms[2]);
    			Conversation.CurrentConversation.AddAction(nwn2Line,action);
    		}
    	}
    	    	
    	private void OnClick_RemovePlayerFeat(object sender, EventArgs ea)
    	{
    		object[] prms = new object[1];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Take a special ability away from the player");
    		window.AddEnumQuestion("Which special ability should they lose?",typeof(ScriptHelper.Feat));
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ScriptFunctor action = Actions.RemovePlayerFeat((ScriptHelper.Feat)prms[0]);
    			Conversation.CurrentConversation.AddAction(nwn2Line,action);
    		}
    	}    
    	    	
    	private void OnClick_RemoveCreatureFeat(object sender, EventArgs ea)
    	{
    		object[] prms = new object[2];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Take a special ability away from a creature");
    		window.AddTagQuestion("Who should lose a special ability?",ScriptHelper.TaggedType.Creature);
    		window.AddEnumQuestion("Which special ability should they lose?",typeof(ScriptHelper.Feat));
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ScriptFunctor action = Actions.RemoveCreatureFeat((string)prms[0],(ScriptHelper.Feat)prms[1]);
    			Conversation.CurrentConversation.AddAction(nwn2Line,action);
    		}
    	}   
    	    	
    	private void OnClick_RemoveHenchman(object sender, EventArgs ea)
    	{
    		object[] prms = new object[1];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Make one of the player's allies leave him");
    		window.AddTagQuestion("Which creature should stop being the player's ally?",ScriptHelper.TaggedType.Creature);
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ScriptFunctor action = Actions.RemoveHenchman((string)prms[0]);
    			Conversation.CurrentConversation.AddAction(nwn2Line,action);
    		}
    	}   
    	    	
    	private void OnClick_RemoveItem(object sender, EventArgs ea)
    	{
    		object[] prms = new object[1];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Take an item away from the player");
    		window.AddTagQuestion("Which item should the player lose?",ScriptHelper.TaggedType.Item);
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ScriptFunctor action = Actions.RemoveItem((string)prms[0]);
    			Conversation.CurrentConversation.AddAction(nwn2Line,action);
    		}
    	}   
    	
    	    	
    	private void OnClick_LockDoor(object sender, EventArgs ea)
    	{    	
    		object[] prms = new object[1];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Lock a door");
    		window.AddTagQuestion("Which door should be locked?",ScriptHelper.TaggedType.Door);
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    		
    			NWN2ScriptFunctor action = Actions.LockDoor((string)prms[0]);
    			Conversation.CurrentConversation.AddAction(nwn2Line,action);
    		}
    	}    
    	    	
    	private void OnClick_UnlockDoor(object sender, EventArgs ea)
    	{    	
    		object[] prms = new object[1];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Unlock a door");
    		window.AddTagQuestion("Which door should be unlocked?",ScriptHelper.TaggedType.Door);
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    		
    			NWN2ScriptFunctor action = Actions.UnlockDoor((string)prms[0]);
    			Conversation.CurrentConversation.AddAction(nwn2Line,action);
    		}
    	}    
    	
    	private void OnClick_SetFloatVariable(object sender, EventArgs ea)
    	{
    		object[] prms = new object[2];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Set a float variable");
    		
    		window.AddVariableQuestion("Which variable do you want to set?", NWN2ScriptVariableType.Float);
    		window.AddStringQuestion("What is the new value of the variable?");// Either enter the value, e.g. '5', or an operation, " +
    		                         //"e.g. '-4' or '+1'."); // TODO add check
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ScriptFunctor action = Actions.SetFloat((string)prms[0],(string)prms[1]);
    			Conversation.CurrentConversation.AddAction(nwn2Line,action);
    		}
    	}
    	    	
    	private void OnClick_SetIntVariable(object sender, EventArgs ea)
    	{
    		object[] prms = new object[2];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Set an integer variable");
    		
    		window.AddVariableQuestion("Which variable do you want to set?", NWN2ScriptVariableType.Int);
    		window.AddStringQuestion("What is the new value of the variable?");// Either enter the value, e.g. '5', or an operation, " +
    		                         //"e.g. '-4' or '+1'."); // TODO add check
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ScriptFunctor action = Actions.SetInt((string)prms[0],(string)prms[1]);
    			Conversation.CurrentConversation.AddAction(nwn2Line,action);
    		}
    	}
    	
    	private void OnClick_SetStringVariable(object sender, EventArgs ea)
    	{
    		object[] prms = new object[2];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Set a string variable");
    		
    		window.AddVariableQuestion("Which variable do you want to set?", NWN2ScriptVariableType.String);
    		window.AddStringQuestion("What is the new value of the variable?");
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ScriptFunctor action = Actions.SetString((string)prms[0],(string)prms[1]);
    			Conversation.CurrentConversation.AddAction(nwn2Line,action);
    		}
    	}
    	    	
    	private void OnClick_CreatureBecomesMortal(object sender, EventArgs ea)
    	{
    		object[] prms = new object[1];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Make a creature killable");
    		window.AddTagQuestion("Which creature should become killable?",ScriptHelper.TaggedType.Creature);
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ScriptFunctor action = Actions.CreatureBecomesMortal((string)prms[0]);
    			Conversation.CurrentConversation.AddAction(nwn2Line,action);
    		}
    	}
    	
    	private void OnClick_CreatureBecomesImmortal(object sender, EventArgs ea)
    	{
    		object[] prms = new object[1];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Make a creature unkillable");
    		window.AddTagQuestion("Which creature should become unkillable?",ScriptHelper.TaggedType.Creature);
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ScriptFunctor action = Actions.CreatureBecomesImmortal((string)prms[0]);
    			Conversation.CurrentConversation.AddAction(nwn2Line,action);
    		}
    	}
    	
    	private void OnClick_SetTime(object sender, EventArgs ea)
    	{
    		object[] prms = new object[1];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Set the time of day");
    		window.AddIntegerQuestion("Which hour of the day should it become? (enter 0-23)",0,23);
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ScriptFunctor action = Actions.SetTime((int)prms[0]);
    			Conversation.CurrentConversation.AddAction(nwn2Line,action);
    		}
    	}
    	
    	private void OnClick_PlayerBecomesMoreGood(object sender, EventArgs ea)
    	{
    		NWN2ScriptFunctor action = Actions.PlayerBecomesMoreGood();
    		Conversation.CurrentConversation.AddAction(nwn2Line,action);
    	}
    	
    	private void OnClick_PlayerBecomesMoreEvil(object sender, EventArgs ea)
    	{
    		NWN2ScriptFunctor action = Actions.PlayerBecomesMoreEvil();
    		Conversation.CurrentConversation.AddAction(nwn2Line,action);
    	}
    	
    	private void OnClick_PlayerBecomesMoreLawful(object sender, EventArgs ea)
    	{
    		NWN2ScriptFunctor action = Actions.PlayerBecomesMoreLawful();
    		Conversation.CurrentConversation.AddAction(nwn2Line,action);
    	}
    	
    	private void OnClick_PlayerBecomesMoreChaotic(object sender, EventArgs ea)
    	{
    		NWN2ScriptFunctor action = Actions.PlayerBecomesMoreChaotic();
    		Conversation.CurrentConversation.AddAction(nwn2Line,action);
    	}
    	    	      	
    	private void OnClick_TakeGold(object sender, EventArgs ea)
    	{
    		object[] prms = new object[1];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Take gold away from the player");
    		window.AddIntegerQuestion("How much gold should the player lose?",0,null);
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ScriptFunctor action = Actions.TakeGold((int)prms[0]);
    			Conversation.CurrentConversation.AddAction(nwn2Line,action);
    		}
    	}   
    	
		
		
		public void SetupVariables(object sender, EventArgs ea)
		{
//			NWN2Journal journal = Adventure.CurrentAdventure.Module.Journal;
//			
//			NWN2JournalCategory category = journal.AddCategory();
//				
//			category.Comment = "This is a comment - can you see it?";
//			category.Name = Conversation.StringToOEIExoLocString("Journey to the Black Lagoon");
//			category.Priority = NWN2JournalPriority.High;
//			
//			
//			NWN2JournalEntry entry1 = category.AddEntry();
//			entry1.Comment = "This is another comment - can you see it?";
//			entry1.Endpoint = false;
//			entry1.Text = Conversation.StringToOEIExoLocString("I must journey to the black lagoon to locate my brother.");
//			
//			NWN2JournalEntry entry2 = category.AddEntry();	
//			entry2.Endpoint = false;
//			entry2.Text = Conversation.StringToOEIExoLocString("I have sunk to the bottom of the black lagoon using a potion. Here I will discover whether my brother still lives.");
//			
//			NWN2JournalEntry entry4 = category.AddEntry();	
//			entry2.Endpoint = true;
//			entry2.Text = Conversation.StringToOEIExoLocString("My brother is dead. My quest is at an end.");
//				
//			category.XP = 800;
//			category.Tag = "blacklagoonquest";	
//			
			
		}
    }
}
