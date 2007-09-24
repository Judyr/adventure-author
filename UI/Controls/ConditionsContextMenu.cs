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
    	private bool HasCondition()
    	{
    		return nwn2Line.Conditions.Count > 0;
    	}
    	
		private void OnClick_CreatureIsDead(object sender, EventArgs ea)
		{
			if (HasCondition()) {
				Say.Information("Sorry - you can only add one condition to a line.");
				return;
			}
    		object[] prms = new object[1];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Check that a hostile creature is within range of the player");
    		window.AddFloatQuestion("How close does a hostile creature need to be (in metres)?",0,null);
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ConditionalFunctor condition = Conditions.CreatureIsDead((string)prms[0]);
				nwn2Line.Conditions.Add(condition);
				ConversationWriterWindow.Instance.RefreshDisplay(false);
    		}
		}
		
		private void OnClick_EnemyIsNearPlayer(object sender, EventArgs ea)
		{
			if (HasCondition()) {
				Say.Information("Sorry - you can only add one condition to a line.");
				return;
			}
    		object[] prms = new object[1];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Check that a hostile creature is within range of the player");
    		window.AddFloatQuestion("How close does a hostile creature need to be (in metres)?",0,null);
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ConditionalFunctor condition = Conditions.EnemyIsNearPlayer((int)prms[0]);
				nwn2Line.Conditions.Add(condition);
				ConversationWriterWindow.Instance.RefreshDisplay(false);
    		}
		}
				
		private void OnClick_IntHasValue(object sender, EventArgs ea)
		{
			if (HasCondition()) {
				Say.Information("Sorry - you can only add one condition to a line.");
				return;
			}
    		object[] prms = new object[2];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Check that an integer variable has a certain value");
    		window.AddVariableQuestion("Which variable do you want to check?", NWN2ScriptVariableType.Int);
    		window.AddStringQuestion("What value does the variable need to have?");// Either enter the value, " +
    		                          //"e.g. '5', or an operation, e.g. '-4' or '+1'."); // TODO add check
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ConditionalFunctor condition = Conditions.IntHasValue((string)prms[0],(string)prms[1]);
				nwn2Line.Conditions.Add(condition);
				ConversationWriterWindow.Instance.RefreshDisplay(false);
    		}
		}
				
		private void OnClick_StringHasValue(object sender, EventArgs ea)
		{
			if (HasCondition()) {
				Say.Information("Sorry - you can only add one condition to a line.");
				return;
			}
    		object[] prms = new object[2];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Check that a string variable has a certain value");
    		window.AddVariableQuestion("Which variable do you want to check?", NWN2ScriptVariableType.String);
    		window.AddStringQuestion("What value does the variable need to have?");
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ConditionalFunctor condition = Conditions.StringHasValue((string)prms[0],(string)prms[1]);
				nwn2Line.Conditions.Add(condition);
				ConversationWriterWindow.Instance.RefreshDisplay(false);
    		}
		}
				
		private void OnClick_ObjectIsNearPlayer(object sender, EventArgs ea)
		{
			if (HasCondition()) {
				Say.Information("Sorry - you can only add one condition to a line.");
				return;
			}
    		object[] prms = new object[2];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Check that an object is within range of the player");
    		window.AddTagQuestion("Which object needs to be within range?",ScriptHelper.ObjectType.Any);
    		window.AddStringQuestion("How close does the object need to be (in metres)?");// Either enter the value, " +
    		                          //"e.g. '5', or an operation, e.g. '-4' or '+1'."); // TODO add check
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ConditionalFunctor condition = Conditions.ObjectIsNearPlayer((string)prms[0],(string)prms[1]);
				nwn2Line.Conditions.Add(condition);
				ConversationWriterWindow.Instance.RefreshDisplay(false);
    		}
		}		
		
		private void OnClick_PlayerHasGold(object sender, EventArgs ea)
		{
			if (HasCondition()) {
				Say.Information("Sorry - you can only add one condition to a line.");
				return;
			}
    		object[] prms = new object[1];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Check that the player has a certain amount of gold");
    		window.AddIntegerQuestion("How much gold does the player need to have?",0,null);
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ConditionalFunctor condition = Conditions.PlayerHasGold((int)prms[0]);
				nwn2Line.Conditions.Add(condition);
				ConversationWriterWindow.Instance.RefreshDisplay(false);
    		}
		}
		
		private void OnClick_PlayerIsMale(object sender, EventArgs ea)
		{
			if (HasCondition()) {
				Say.Information("Sorry - you can only add one condition to a line.");
				return;
			}
			NWN2ConditionalFunctor condition = Conditions.PlayerIsMale();
			nwn2Line.Conditions.Add(condition);
			ConversationWriterWindow.Instance.RefreshDisplay(false);
		}
				
		private void OnClick_PlayerIsFemale(object sender, EventArgs ea)
		{
			if (HasCondition()) {
				Say.Information("Sorry - you can only add one condition to a line.");
				return;
			}
			NWN2ConditionalFunctor condition = Conditions.PlayerIsFemale();
			nwn2Line.Conditions.Add(condition);
			ConversationWriterWindow.Instance.RefreshDisplay(false);
		}
				
		private void OnClick_PlayerIsGood(object sender, EventArgs ea)
		{
			if (HasCondition()) {
				Say.Information("Sorry - you can only add one condition to a line.");
				return;
			}
			NWN2ConditionalFunctor condition = Conditions.PlayerIsGood();
			nwn2Line.Conditions.Add(condition);
			ConversationWriterWindow.Instance.RefreshDisplay(false);
		}
			
		private void OnClick_PlayerIsEvil(object sender, EventArgs ea)
		{
			if (HasCondition()) {
				Say.Information("Sorry - you can only add one condition to a line.");
				return;
			}
			NWN2ConditionalFunctor condition = Conditions.PlayerIsEvil();
			nwn2Line.Conditions.Add(condition);
			ConversationWriterWindow.Instance.RefreshDisplay(false);
		}		
		
		private void OnClick_PlayerIsLawful(object sender, EventArgs ea)
		{
			if (HasCondition()) {
				Say.Information("Sorry - you can only add one condition to a line.");
				return;
			}
			NWN2ConditionalFunctor condition = Conditions.PlayerIsLawful();
			nwn2Line.Conditions.Add(condition);
			ConversationWriterWindow.Instance.RefreshDisplay(false);
		}
				
		private void OnClick_PlayerIsChaotic(object sender, EventArgs ea)
		{
			if (HasCondition()) {
				Say.Information("Sorry - you can only add one condition to a line.");
				return;
			}
			NWN2ConditionalFunctor condition = Conditions.PlayerIsChaotic();
			nwn2Line.Conditions.Add(condition);
			ConversationWriterWindow.Instance.RefreshDisplay(false);
		}			
				
		private void OnClick_PlayerHasNumberOfItems(object sender, EventArgs ea)
		{		
			if (HasCondition()) {
				Say.Information("Sorry - you can only add one condition to a line.");
				return;
			}	
			// TODO, pass in a 'remark' that will extend the size of the parameter panel to give extra info,
			// then add it in everywehre it needs it (e.g. make the stuff about '-4', '>6' into its own remark.
			
			// TODO, add a bool parameter to AddStringQuestion to parse its value for being a 'fake integer', i.e.
			// if the string must be 4, -4, >0, !=5 etc.
			
			object[] prms = new object[2];
    		ScriptParametersWindow window = new ScriptParametersWindow(ref prms,"Check that the player has a number of items");
    		window.AddTagQuestion("Which item should the player have?",ScriptHelper.ObjectType.Item);
    		window.AddStringQuestion("How many items with this tag should the player have?");// Either enter the value, " +
    		                          //"e.g. '5', or an operation, e.g. '-4' or '+1'."); // TODO add check
    		bool? result = window.ShowDialog();
    		if (result == null || !(bool)result) { // cancelled or failed
    			return;
    		}
    		else {    			
    			NWN2ConditionalFunctor condition = Conditions.PlayerHasNumberOfItems((string)prms[0],(string)prms[1]);
				nwn2Line.Conditions.Add(condition);
				ConversationWriterWindow.Instance.RefreshDisplay(false);
    		}
		}
    }
}