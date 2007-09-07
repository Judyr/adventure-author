﻿/*
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
using AdventureAuthor.Scripts;
using AdventureAuthor.UI.Windows;
using AdventureAuthor.Utils;
using NWN2Toolset.NWN2.Data;

namespace AdventureAuthor.UI.Controls
{
    public partial class LineControl : UserControl
    {    	
        #region Add an Action event handlers        
    	    	    	    	
    	private void OnClick_AdvanceTime(object sender, EventArgs ea)
    	{
    		object[] parameters = new object[2];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref parameters, "Advance time");
    		window.AddIntegerQuestion("Advance time by how many hours?",0,null);
    		window.AddIntegerQuestion("...and how many minutes?",0,59);
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ScriptFunctor action = Actions.AdvanceTimeBy((int)parameters[0],(int)parameters[1],0,0);
	    		nwn2Line.Actions.Add(action);
	    		ConversationWriterWindow.Instance.RefreshDisplay(false);
    		}
    	}
    	
    	private void OnClick_Attack(object sender, EventArgs ea)
    	{    		
    		object[] parameters = new object[1];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref parameters, "Attack the player");
    		window.AddTagQuestion("Who should attack the player?",TagHelper.TagType.Creature);
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ScriptFunctor action = Actions.Attack((string)parameters[0]);
	    		nwn2Line.Actions.Add(action);
	    		ConversationWriterWindow.Instance.RefreshDisplay(false);
    		}
    	}
        
    	private void OnClick_AddHenchmanForPlayer(object sender, EventArgs ea)
    	{    	
    		object[] parameters = new object[2];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref parameters,"Give the player a henchman");
    		window.AddTagQuestion("Who should become the player's henchman?",TagHelper.TagType.Creature);
    		window.AddBooleanQuestion("Should they follow the player around and fight for him?");
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ScriptFunctor action = Actions.AddHenchman((string)parameters[0],String.Empty,(int)parameters[1]);
	    		nwn2Line.Actions.Add(action);
	    		ConversationWriterWindow.Instance.RefreshDisplay(false);
    		}
    	}  
    	    	
    	private void OnClick_AddHenchmanForCreature(object sender, EventArgs ea)
    	{    	
    		object[] parameters = new object[3];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref parameters,"Give someone else a henchman");
    		window.AddTagQuestion("Who should become the henchman?",TagHelper.TagType.Creature);
    		window.AddTagQuestion("Whose henchman should they become?",TagHelper.TagType.Creature);
    		window.AddBooleanQuestion("Should they follow their master around and fight for them?");
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    		
    			NWN2ScriptFunctor action = Actions.AddHenchman((string)parameters[0],(string)parameters[1],(int)parameters[2]);
	    		nwn2Line.Actions.Add(action);
	    		ConversationWriterWindow.Instance.RefreshDisplay(false);
    		}
    	}    
    	    	
    	private void OnClick_CreateObject(object sender, EventArgs ea)
    	{    	
    		NWN2ScriptFunctor action = Actions.CreateObject(ScriptHelper.ObjectType.Placeable,
    		                                                "plc_bc_templee01",
    		                                                "buildingwaypoint",
    		                                                1,
    		                                                "ghostlybuilding",
    		                                                5.0f);
    		nwn2Line.Actions.Add(action);
    		ConversationWriterWindow.Instance.RefreshDisplay(false);
    		
    		
//    		object[] parameters = new object[6];
//    		ScriptParametersWindow window = new ScriptParametersWindow(ref parameters);
//    		window.AddTagQuestion("Who should become the henchman?",TagHelper.TagType.Creature);
//    		window.AddTagQuestion("Whose henchman should they become?",TagHelper.TagType.Creature);
//    		window.AddBooleanQuestion("Should they follow their master around and fight for them?");
//    		bool? result = window.ShowDialog();
//    		if (result == null || !(bool)result) { // cancelled or failed
//    			return;
//    		}
//    		else {    		
//    			NWN2ScriptFunctor action = Actions.AddHenchman((string)parameters[0],(string)parameters[1],(int)parameters[2]);
//	    		nwn2Line.Actions.Add(action);
//	    		ConversationWriterWindow.Instance.RefreshDisplay(false);
//    		}
    	}    	
    	
    	private void OnClick_DestroyObject(object sender, EventArgs ea)
    	{    		
    		object[] parameters = new object[2];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref parameters,"Destroy an object");
    		window.AddTagQuestion("What object should be destroyed?",TagHelper.TagType.Any);
    		window.AddIntegerQuestion("How many seconds should pass before doing this (if any)?",0,null);
    		
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    		
    			NWN2ScriptFunctor action = Actions.Destroy((string)parameters[0],(float)(int)parameters[1]); // unbox first
	    		nwn2Line.Actions.Add(action);
	    		ConversationWriterWindow.Instance.RefreshDisplay(false);
    		}
    	}
    	
    	
    	// TODO: add AddEnumQuestion
    	
    	private void OnClick_EndGame(object sender, EventArgs ea)
    	{
    		object[] parameters = new object[1];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref parameters,"End the game");
    		window.AddStringQuestion("What movie should play when the game ends? (Leave blank for nothing.)");
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ScriptFunctor action = Actions.EndGame((string)parameters[0]);
	    		nwn2Line.Actions.Add(action);
	    		ConversationWriterWindow.Instance.RefreshDisplay(false);
    		}
    	}
    	
    	private void OnClick_FadeToBlack(object sender, EventArgs ea)
    	{
    		NWN2ScriptFunctor action = Actions.FadeOut(3.0f,8.0f,ScriptHelper.FadeColour.Black);
    		nwn2Line.Actions.Add(action);
    		ConversationWriterWindow.Instance.RefreshDisplay(false);    		
    	}
    	
    	private void OnClick_GiveGold(object sender, EventArgs ea)
    	{
    		object[] parameters = new object[1];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref parameters,"Give the player gold");
    		window.AddIntegerQuestion("How much gold does the player receive?",0,null);
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ScriptFunctor action = Actions.GiveGold((int)parameters[0]);
	    		nwn2Line.Actions.Add(action);
	    		ConversationWriterWindow.Instance.RefreshDisplay(false);
    		}
    	}   
    	
    	private void OnClick_GiveItem(object sender, EventArgs ea)
    	{
    		object[] parameters = new object[2];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref parameters,"Give the player an item");
    		window.AddTagQuestion("Which item should the player receive?",TagHelper.TagType.Item);
    		window.AddIntegerQuestion("How many copies of the item should the player receive?",0,null); // TODO split into GiveItem, GiveSeveralItems
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ScriptFunctor action = Actions.GiveItem((string)parameters[0],(int)parameters[1]);
	    		nwn2Line.Actions.Add(action);
	    		ConversationWriterWindow.Instance.RefreshDisplay(false);
    		}
    	}
    	
    	private void OnClick_HealPlayer(object sender, EventArgs ea)
    	{
        	object[] parameters = new object[1];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref parameters,"Heal the player");
    		window.AddIntegerQuestion("What percentage healing should the player receive? (enter 0 for no healing, 100 for full healing)",0,100);
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ScriptFunctor action = Actions.HealPC((int)parameters[0]);
	    		nwn2Line.Actions.Add(action);
	    		ConversationWriterWindow.Instance.RefreshDisplay(false);
    		}
    	}
    	
    	private void OnClick_Kill(object sender, EventArgs ea)
    	{
    		object[] parameters = new object[1];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref parameters,"Kill a creature");
    		window.AddTagQuestion("Which creature should be killed?",TagHelper.TagType.Creature);
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ScriptFunctor action = Actions.Kill((string)parameters[0]);
	    		nwn2Line.Actions.Add(action);
	    		ConversationWriterWindow.Instance.RefreshDisplay(false);
    		}
    	}
    	
    	private void OnClick_OpenStore(object sender, EventArgs ea)
    	{
    		object[] parameters = new object[3];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref parameters,"Open a store");
    		window.AddTagQuestion("Which store should be opened?",TagHelper.TagType.Store);
    		window.AddIntegerQuestion("How much more expensive than normal should it be? (0-100%)",0,100);
    		window.AddIntegerQuestion("Or - how much cheaper than normal should it be? (0-100%)",0,100);
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ScriptFunctor action = Actions.OpenStore((string)parameters[0],(int)parameters[1],(int)parameters[2]);
	    		nwn2Line.Actions.Add(action);
	    		ConversationWriterWindow.Instance.RefreshDisplay(false);
    		}
    	}
    	
    	private void OnClick_PlayAnimation(object sender, EventArgs ea)
    	{
    		object[] parameters = new object[3];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref parameters,"Play an animation");
    		window.AddTagQuestion("Who should perform the animation?",TagHelper.TagType.Creature);
    		window.AddEnumQuestion("What animation should they perform?",typeof(ScriptHelper.OneTimeAnimation));
    		window.AddIntegerQuestion("How long should they wait before performing it (in seconds)?",0,null);
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ScriptFunctor action = Actions.PlayAnimationOnce((string)parameters[0],(ScriptHelper.OneTimeAnimation)parameters[1]
    			                                                     ,1.0f,(float)(int)parameters[2]);
	    		nwn2Line.Actions.Add(action);
	    		ConversationWriterWindow.Instance.RefreshDisplay(false);
    		}
    	}
    	
    	private void OnClick_ShiftAlignmentTowardsGood(object sender, EventArgs ea)
    	{
    		NWN2ScriptFunctor action = Actions.PlayerBecomesMoreGoodOrMoreEvil(3);
    		nwn2Line.Actions.Add(action);
    		ConversationWriterWindow.Instance.RefreshDisplay(false);
    	}
    	
    	private void OnClick_ShiftAlignmentTowardsEvil(object sender, EventArgs ea)
    	{
    		NWN2ScriptFunctor action = Actions.PlayerBecomesMoreGoodOrMoreEvil(-3);
    		nwn2Line.Actions.Add(action);
    		ConversationWriterWindow.Instance.RefreshDisplay(false);
    	}
    	
    	private void OnClick_ShiftAlignmentTowardsLaw(object sender, EventArgs ea)
    	{
    		NWN2ScriptFunctor action = Actions.PlayerBecomesMoreLawfulOrMoreChaotic(3);
    		nwn2Line.Actions.Add(action);
    		ConversationWriterWindow.Instance.RefreshDisplay(false);
    	}
    	
    	private void OnClick_ShiftAlignmentTowardsChaos(object sender, EventArgs ea)
    	{
    		NWN2ScriptFunctor action = Actions.PlayerBecomesMoreLawfulOrMoreChaotic(-3);
    		nwn2Line.Actions.Add(action);
    		ConversationWriterWindow.Instance.RefreshDisplay(false);
    	}
    	
    	private void OnClick_SetIntVariable(object sender, EventArgs ea)
    	{
    		NWN2ScriptFunctor action = Actions.SetInt("numberofchancesleft","3");
    		nwn2Line.Actions.Add(action);
    		ConversationWriterWindow.Instance.RefreshDisplay(false);
    	}
    	
    	private void OnClick_SetStringVariable(object sender, EventArgs ea)
    	{
    		NWN2ScriptFunctor action = Actions.SetString("hasplayerfoundamulet","yes");
    		nwn2Line.Actions.Add(action);
    		ConversationWriterWindow.Instance.RefreshDisplay(false);
    	}
    	
		#endregion   
		
		#region Add a Condition event handlers
		
		private void OnClick_CreatureIsDead(object sender, EventArgs ea)
		{
			NWN2ConditionalFunctor condition = Conditions.CreatureIsDead("wolf");
			nwn2Line.Conditions.Add(condition);
    		ConversationWriterWindow.Instance.RefreshDisplay(false);
		}
		
		private void OnClick_EnemyIsNearPlayer(object sender, EventArgs ea)
		{
			NWN2ConditionalFunctor condition = Conditions.EnemyIsNearPlayer(20.0f);
			nwn2Line.Conditions.Add(condition);
    		ConversationWriterWindow.Instance.RefreshDisplay(false);
		}
				
		private void OnClick_IntHasValue(object sender, EventArgs ea)
		{
			NWN2ConditionalFunctor condition = Conditions.IntHasValue("numberofchancesleft",">0");
			nwn2Line.Conditions.Add(condition);
    		ConversationWriterWindow.Instance.RefreshDisplay(false);
		}
				
		private void OnClick_StringHasValue(object sender, EventArgs ea)
		{
			NWN2ConditionalFunctor condition = Conditions.StringHasValue("hasplayerfoundamulet","yes");
			nwn2Line.Conditions.Add(condition);
    		ConversationWriterWindow.Instance.RefreshDisplay(false);
		}
				
		private void OnClick_ObjectIsNearPlayer(object sender, EventArgs ea)
		{
			NWN2ConditionalFunctor condition = Conditions.ObjectIsNearPlayer("treasure","<5.0");
			nwn2Line.Conditions.Add(condition);
    		ConversationWriterWindow.Instance.RefreshDisplay(false);
		}		
		
		private void OnClick_PlayerHasGold(object sender, EventArgs ea)
		{
			NWN2ConditionalFunctor condition = Conditions.PlayerHasGold(120);
			nwn2Line.Conditions.Add(condition);
    		ConversationWriterWindow.Instance.RefreshDisplay(false);
		}
		
		private void OnClick_PlayerIsMale(object sender, EventArgs ea)
		{
			NWN2ConditionalFunctor condition = Conditions.PlayerIsMale();
			nwn2Line.Conditions.Add(condition);
			ConversationWriterWindow.Instance.RefreshDisplay(false);
		}
				
		private void OnClick_PlayerIsFemale(object sender, EventArgs ea)
		{
			NWN2ConditionalFunctor condition = Conditions.PlayerIsFemale();
			nwn2Line.Conditions.Add(condition);
			ConversationWriterWindow.Instance.RefreshDisplay(false);
		}
				
		private void OnClick_PlayerIsGood(object sender, EventArgs ea)
		{
			NWN2ConditionalFunctor condition = Conditions.PlayerIsGood();
			nwn2Line.Conditions.Add(condition);
			ConversationWriterWindow.Instance.RefreshDisplay(false);
		}
			
		private void OnClick_PlayerIsEvil(object sender, EventArgs ea)
		{
			NWN2ConditionalFunctor condition = Conditions.PlayerIsEvil();
			nwn2Line.Conditions.Add(condition);
			ConversationWriterWindow.Instance.RefreshDisplay(false);
		}		
		
		private void OnClick_PlayerIsLawful(object sender, EventArgs ea)
		{
			NWN2ConditionalFunctor condition = Conditions.PlayerIsLawful();
			nwn2Line.Conditions.Add(condition);
			ConversationWriterWindow.Instance.RefreshDisplay(false);
		}
				
		private void OnClick_PlayerIsChaotic(object sender, EventArgs ea)
		{
			NWN2ConditionalFunctor condition = Conditions.PlayerIsChaotic();
			nwn2Line.Conditions.Add(condition);
			ConversationWriterWindow.Instance.RefreshDisplay(false);
		}			
				
		private void OnClick_PlayerHasNumberOfItems(object sender, EventArgs ea)
		{
			NWN2ConditionalFunctor condition = Conditions.PlayerHasNumberOfItems("gemstone",">2");
			nwn2Line.Conditions.Add(condition);
			ConversationWriterWindow.Instance.RefreshDisplay(false);
		}
				
		#endregion		
    }
}
