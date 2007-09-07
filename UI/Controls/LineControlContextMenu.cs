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
    		object[] prms = new object[2];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms, "Advance time");
    		window.AddIntegerQuestion("Advance time by how many hours?",0,null);
    		window.AddIntegerQuestion("...and how many minutes?",0,59);
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ScriptFunctor action = Actions.AdvanceTimeBy((int)prms[0],(int)prms[1],0,0);
	    		nwn2Line.Actions.Add(action);
	    		ConversationWriterWindow.Instance.RefreshDisplay(false);
    		}
    	}
    	
    	private void OnClick_Attack(object sender, EventArgs ea)
    	{    		
    		object[] prms = new object[1];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms, "Attack the player");
    		window.AddTagQuestion("Who should attack the player?",TagHelper.ObjectType.Creature);
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ScriptFunctor action = Actions.Attack((string)prms[0]);
	    		nwn2Line.Actions.Add(action);
	    		ConversationWriterWindow.Instance.RefreshDisplay(false);
    		}
    	}
        
    	private void OnClick_AddHenchmanForPlayer(object sender, EventArgs ea)
    	{    	
    		object[] prms = new object[2];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Give the player a henchman");
    		window.AddTagQuestion("Who should become the player's henchman?",TagHelper.ObjectType.Creature);
    		window.AddBooleanQuestion("Should they follow the player around and fight for him?");
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ScriptFunctor action = Actions.AddHenchman((string)prms[0],String.Empty,(int)prms[1]);
	    		nwn2Line.Actions.Add(action);
	    		ConversationWriterWindow.Instance.RefreshDisplay(false);
    		}
    	}  
    	    	
    	private void OnClick_AddHenchmanForCreature(object sender, EventArgs ea)
    	{    	
    		object[] prms = new object[3];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Give someone else a henchman");
    		window.AddTagQuestion("Who should become the henchman?",TagHelper.ObjectType.Creature);
    		window.AddTagQuestion("Whose henchman should they become?",TagHelper.ObjectType.Creature);
    		window.AddBooleanQuestion("Should they follow their master around and fight for them?");
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    		
    			NWN2ScriptFunctor action = Actions.AddHenchman((string)prms[0],(string)prms[1],(int)prms[2]);
	    		nwn2Line.Actions.Add(action);
	    		ConversationWriterWindow.Instance.RefreshDisplay(false);
    		}
    	}    
    	    	
    	private void OnClick_CreateCreature(object sender, EventArgs ea)
    	{    	
    		object[] prms = new object[5];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Create a creature");
    		window.AddResRefQuestion("What template should the creature be created from?",TagHelper.ObjectType.Creature); //0th
    		window.AddStringQuestion("What tag should the new creature be given?"); //3rd
    		window.AddTagQuestion("What waypoint should the creature appear at?",TagHelper.ObjectType.Waypoint); //1st
    		window.AddBooleanQuestion("Should an 'appearing' animation be played when it is created?"); //2nd
    		window.AddFloatQuestion("How many seconds should pass before doing this (if any)?",0,null); //4th
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    		
    			NWN2ScriptFunctor action = Actions.CreateCreature((string)prms[0],(string)prms[2],(int)prms[3],(string)prms[1],(float)prms[4]);
	    		nwn2Line.Actions.Add(action);
	    		ConversationWriterWindow.Instance.RefreshDisplay(false);
    		}
    	}    	
    	    	    	
    	private void OnClick_CreateItem(object sender, EventArgs ea)
    	{    	
    		object[] prms = new object[5];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Create an item");
    		window.AddResRefQuestion("What template should the item be created from?",TagHelper.ObjectType.Item); //0th
    		window.AddStringQuestion("What tag should the new item be given?"); //3rd
    		window.AddTagQuestion("What waypoint should the item appear at?",TagHelper.ObjectType.Waypoint); //1st
    		window.AddBooleanQuestion("Should an 'appearing' animation be played when it is created?"); //2nd
    		window.AddFloatQuestion("How many seconds should pass before doing this (if any)?",0,null); //4th
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    		
    			NWN2ScriptFunctor action = Actions.CreateItem((string)prms[0],(string)prms[2],(int)prms[3],(string)prms[1],(float)prms[4]);
	    		nwn2Line.Actions.Add(action);
	    		ConversationWriterWindow.Instance.RefreshDisplay(false);
    		}
    	}    	
    	    	    	
    	private void OnClick_CreatePlaceable(object sender, EventArgs ea)
    	{    	
    		object[] prms = new object[5];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Create a placeable");
    		window.AddResRefQuestion("What template should the placeable be created from?",TagHelper.ObjectType.Placeable); //0th
    		window.AddStringQuestion("What tag should the new placeable be given?"); //3rd
    		window.AddTagQuestion("What waypoint should the placeable appear at?",TagHelper.ObjectType.Waypoint); //1st
    		window.AddBooleanQuestion("Should an 'appearing' animation be played when it is created?"); //2nd
    		window.AddFloatQuestion("How many seconds should pass before doing this (if any)?",0,null); //4th
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    		
    			NWN2ScriptFunctor action = Actions.CreatePlaceable((string)prms[0],(string)prms[2],(int)prms[3],(string)prms[1],(float)prms[4]);
	    		nwn2Line.Actions.Add(action);
	    		ConversationWriterWindow.Instance.RefreshDisplay(false);
    		}
    	}    	
    	    	
    	private void OnClick_DestroyObject(object sender, EventArgs ea)
    	{    		
    		object[] prms = new object[2];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Destroy an object");
    		window.AddTagQuestion("What object should be destroyed?",TagHelper.ObjectType.Any);
    		window.AddFloatQuestion("How many seconds should pass before doing this (if any)?",0,null);    		
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    		
    			NWN2ScriptFunctor action = Actions.Destroy((string)prms[0],(float)prms[1]); // unbox first
	    		nwn2Line.Actions.Add(action);
	    		ConversationWriterWindow.Instance.RefreshDisplay(false);
    		}
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
	    		nwn2Line.Actions.Add(action);
	    		ConversationWriterWindow.Instance.RefreshDisplay(false);
    		}
    	}
    	
    	private void OnClick_FadeToBlack(object sender, EventArgs ea)
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
	    		nwn2Line.Actions.Add(action);
	    		ConversationWriterWindow.Instance.RefreshDisplay(false);
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
	    		nwn2Line.Actions.Add(action);
	    		ConversationWriterWindow.Instance.RefreshDisplay(false);
    		}
    	}   
    	
    	// TODO add default value to method call
    	
    	private void OnClick_GiveItem(object sender, EventArgs ea)
    	{
    		object[] prms = new object[2];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Give the player an item");
    		window.AddResRefQuestion("What kind of item should the player receive?",TagHelper.ObjectType.Item);
    		window.AddIntegerQuestion("How many copies of the item should the player receive?",0,null);
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ScriptFunctor action = Actions.GiveItem((string)prms[0],(int)prms[1]);
	    		nwn2Line.Actions.Add(action);
	    		ConversationWriterWindow.Instance.RefreshDisplay(false);
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
	    		nwn2Line.Actions.Add(action);
	    		ConversationWriterWindow.Instance.RefreshDisplay(false);
    		}
    	}
    	
    	private void OnClick_Kill(object sender, EventArgs ea)
    	{
    		object[] prms = new object[1];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Kill a creature");
    		window.AddTagQuestion("Which creature should be killed?",TagHelper.ObjectType.Creature);
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ScriptFunctor action = Actions.Kill((string)prms[0]);
	    		nwn2Line.Actions.Add(action);
	    		ConversationWriterWindow.Instance.RefreshDisplay(false);
    		}
    	}
    	
    	private void OnClick_OpenStore(object sender, EventArgs ea)
    	{
    		object[] prms = new object[3];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Open a store");
    		window.AddTagQuestion("Which store should be opened?",TagHelper.ObjectType.Store);
    		window.AddIntegerQuestion("How much more expensive than normal should it be? (0-100%)",0,100);
    		window.AddIntegerQuestion("Or - how much cheaper than normal should it be? (0-100%)",0,100);
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ScriptFunctor action = Actions.OpenStore((string)prms[0],(int)prms[1],(int)prms[2]);
	    		nwn2Line.Actions.Add(action);
	    		ConversationWriterWindow.Instance.RefreshDisplay(false);
    		}
    	}
    	
    	private void OnClick_PlayAnimation(object sender, EventArgs ea)
    	{
    		object[] prms = new object[3];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Play an animation");
    		window.AddTagQuestion("Who should perform the animation?",TagHelper.ObjectType.Creature);
    		window.AddEnumQuestion("What animation should they perform?",typeof(ScriptHelper.OneTimeAnimation));
    		window.AddFloatQuestion("How long should they wait before performing it (in seconds)?",0,null);
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ScriptFunctor action = Actions.PlayAnimationOnce((string)prms[0],(ScriptHelper.OneTimeAnimation)prms[1]
    			                                                     ,1.0f,(float)prms[2]);
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
