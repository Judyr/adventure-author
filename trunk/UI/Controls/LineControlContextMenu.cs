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
using AdventureAuthor.UI.Windows;
using AdventureAuthor.Utils;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.Journal;
using form = NWN2Toolset.NWN2ToolsetMainForm;

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
    		window.AddTagQuestion("Who should attack the player?",ScriptHelper.ObjectType.Creature);
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
    		window.AddTagQuestion("Who should become the player's henchman?",ScriptHelper.ObjectType.Creature);
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
    		window.AddTagQuestion("Who should become the henchman?",ScriptHelper.ObjectType.Creature);
    		window.AddTagQuestion("Whose henchman should they become?",ScriptHelper.ObjectType.Creature);
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
    		window.AddResRefQuestion("What template should the creature be created from?",ScriptHelper.ObjectType.Creature); //0th
    		window.AddStringQuestion("What tag should the new creature be given?"); //3rd
    		window.AddTagQuestion("What waypoint should the creature appear at?",ScriptHelper.ObjectType.Waypoint); //1st
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
    		window.AddResRefQuestion("What template should the item be created from?",ScriptHelper.ObjectType.Item); //0th
    		window.AddStringQuestion("What tag should the new item be given?"); //3rd
    		window.AddTagQuestion("What waypoint should the item appear at?",ScriptHelper.ObjectType.Waypoint); //1st
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
    		window.AddResRefQuestion("What template should the placeable be created from?",ScriptHelper.ObjectType.Placeable); //0th
    		window.AddStringQuestion("What tag should the new placeable be given?"); //3rd
    		window.AddTagQuestion("What waypoint should the placeable appear at?",ScriptHelper.ObjectType.Waypoint); //1st
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
    		window.AddTagQuestion("What object should be destroyed?",ScriptHelper.ObjectType.Any);
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
    		window.AddResRefQuestion("What kind of item should the player receive?",ScriptHelper.ObjectType.Item);
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
    		window.AddTagQuestion("Which creature should be killed?",ScriptHelper.ObjectType.Creature);
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
    		window.AddTagQuestion("Which store should be opened?",ScriptHelper.ObjectType.Store);
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
    		window.AddTagQuestion("Who should perform the animation?",ScriptHelper.ObjectType.Creature);
    		window.AddEnumQuestion("What animation should they perform?",typeof(ScriptHelper.OneTimeAnimation));
    		window.AddFloatQuestion("How long should they wait before performing it (in seconds)?",0,null);
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ScriptFunctor action = Actions.PlayAnimationOnce((string)prms[0],(ScriptHelper.OneTimeAnimation)prms[1],
    			                                                     1.0f,(float)prms[2]);
	    		nwn2Line.Actions.Add(action);
	    		ConversationWriterWindow.Instance.RefreshDisplay(false);
    		}
    	}
    	
    	private void OnClick_ShiftGoodEvilAlignment(object sender, EventArgs ea)
    	{
    		object[] prms = new object[2];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Change good/evil alignment");
    		window.AddBooleanQuestion("Should the player become more good, or more evil?","Good","Evil");
    		window.AddIntegerQuestion("By how much? (1-3, 1 meaning a bit more good/evil, 3 meaning a lot more good/evil",1,3);    		
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {   
    			int shift;
    			if ((int)prms[0] == 1) { // good
    				shift = (int)prms[1];
    			}
    			else { // evil
    				shift = 0 - (int)prms[1];
    			}
    			NWN2ScriptFunctor action = Actions.PlayerBecomesMoreGoodOrMoreEvil(shift);
	    		nwn2Line.Actions.Add(action);
	    		ConversationWriterWindow.Instance.RefreshDisplay(false);
    		}
    	}
    	    	
    	private void OnClick_ShiftLawChaosAlignment(object sender, EventArgs ea)
    	{
    		object[] prms = new object[2];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Change law/chaos alignment");
    		window.AddBooleanQuestion("Should the player become more lawful, or more chaotic?","Lawful","Chaotic");
    		window.AddIntegerQuestion("By how much? (enter 1-3, 1 meaning change a bit, 3 meaning change a lot",1,3);    		
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {   
    			int shift;
    			if ((int)prms[0] == 1) { // lawful
    				shift = (int)prms[1];
    			}
    			else { // chaotic
    				shift = 0 - (int)prms[1];
    			}
    			NWN2ScriptFunctor action = Actions.PlayerBecomesMoreLawfulOrMoreChaotic(shift);
	    		nwn2Line.Actions.Add(action);
	    		ConversationWriterWindow.Instance.RefreshDisplay(false);
    		}
    	}
    	    	
    	private void OnClick_SetIntVariable(object sender, EventArgs ea)
    	{
    		object[] prms = new object[2];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Set an integer variable");
    		
    		// crashes without message:
//    		foreach (NWN2GameArea area in NWN2Toolset.NWN2ToolsetMainForm.App.Module.Areas) {
//    			foreach (NWN2ScriptVariable var in area.Variables) {
//    				Say.Information(var.Name + " (" + var.VariableType.ToString() + ")");
//    			}
//    		}
    		
    		// TODO: In future this should become an AddEnumQuestion, and the user should have to create a variable in a separate
    		// VariableManager window (giving the name, type, and what it will be used for), and then be able to select what
    		// variable they want to check here.
    		
    		window.AddStringQuestion("What is the name of the variable you want to set?");
    		window.AddStringQuestion("What is the new value of the variable? Either enter the value, e.g. '5', or an operation, " +
    		                         "e.g. '-4' or '+1'."); // TODO add check
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ScriptFunctor action = Actions.SetInt((string)prms[0],(string)prms[1]);
	    		nwn2Line.Actions.Add(action);
	    		ConversationWriterWindow.Instance.RefreshDisplay(false);
    		}
    	}
    	
    	private void OnClick_SetStringVariable(object sender, EventArgs ea)
    	{
    		object[] prms = new object[2];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Set a string variable");
    		
    		// TODO: In future this should become an AddEnumQuestion, and the user should have to create a variable in a separate
    		// VariableManager window (giving the name, type, and what it will be used for), and then be able to select what
    		// variable they want to check here.
    		
    		window.AddStringQuestion("What is the name of the variable you want to set?");
    		window.AddStringQuestion("What is the new value of the variable?");
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ScriptFunctor action = Actions.SetInt((string)prms[0],(string)prms[1]);
	    		nwn2Line.Actions.Add(action);
	    		ConversationWriterWindow.Instance.RefreshDisplay(false);
    		}
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
		
		
		
		
		public void JournalTest(object sender, EventArgs ea)
		{
			/* Journal stuff
			 * 
			 * All seems very simple. 
			 * 
			 * 
			 * 
			 */
			
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
			
			/*
			 * Variable stuff
			 * 
			 */
							NWN2ScriptVariable variable = new NWN2ScriptVariable();
				variable.Name = "Playerhasfoundamulet";
				variable.VariableType = NWN2ScriptVariableType.String;
				variable.ValueString = "yes";
				NWN2ScriptVariable variable2 = new NWN2ScriptVariable();
				variable2.Name = "Player has killed the wolf";
				variable2.VariableType = NWN2ScriptVariableType.String;
				variable2.ValueString = "no";
				form.App.Module.Areas["Scratchpad"].Variables.Add(variable);
				form.App.Module.Areas["Scratchpad"].Variables.Add(variable2);
			
			
			foreach (NWN2GameArea area in form.App.Module.Areas.Values) {
				

				
				
				//area.Variables.Add(
				
				
				Say.Information(area.Name);
				if (area.Variables != null) {
					Say.Information("Has a variables table.");
					foreach (NWN2ScriptVariable var in area.Variables) {
						Say.Information(var.ToString());
					}
				}
			}
		}
    }
}
