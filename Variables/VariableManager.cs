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
using System.Collections.Generic;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.ConversationData;
using NWN2Toolset.NWN2.Data.TypedCollections;
using AdventureAuthor.Conversations;
using AdventureAuthor.Conversations.UI;
using AdventureAuthor.Core;
using AdventureAuthor.Utils;
using AdventureAuthor.Variables.UI;
using OEIShared.IO;
using OEIShared.Utils;

namespace AdventureAuthor.Variables
{
	public static class VariableManager
	{        
		/// <summary>
		/// Add a variable to the local variables on the module object (used as a 'global' variable system)
		/// </summary>
		/// <param name="variable">The variable to add</param>
		public static void Add(NWN2ScriptVariable variable)
		{
	        Adventure.CurrentAdventure.Module.ModuleInfo.Variables.Add(variable);
		}
		
		
		/// <summary>
		/// Delete a variable, and optionally remove all references to it
		/// </summary>
		/// <param name="variable">The variable to delete</param>
		/// <param name="removeReferences">True to also remove all references in conversations and scripts, false to only delete the variable</param>
		public static void Delete(NWN2ScriptVariable variable, bool removeReferences)
		{
			Say.Debug("Called Delete, removeReferences was " + removeReferences.ToString());
			
			if (removeReferences) {
				RemoveReferences(variable);
			}
			
			Say.Debug("Past removeReferences.");
			
			Adventure.CurrentAdventure.Module.ModuleInfo.Variables.Remove(variable);
			Adventure.CurrentAdventure.Save();
			if (VariablesWindow.Instance != null) {
				VariablesWindow.Instance.Refresh();
			}
		}
		
		
		/// <summary>
		/// Remove all references in conversation scripts and OnEvent scripts to a given variable
		/// </summary>
		/// <param name="variable">The variable to remove references for</param>
		private static void RemoveReferences(NWN2ScriptVariable variable)
		{			
			// Remove all references to this variable in the module's conversations:
        	IResourceRepository moduleRepos = ResourceManager.Instance.GetRepositoryByName(Adventure.CurrentAdventure.ModulePath);
            if (moduleRepos == null) {
            	Say.Error("Couldn't locate the module as a resource repository.");
            	return;
            }      
        	
        	ushort DLG = BWResourceTypes.GetResourceType("dlg");
			OEIGenericCollectionWithEvents<IResourceEntry> DLGfiles = moduleRepos.FindResourcesByType(DLG);

			foreach (IResourceEntry resource in DLGfiles) {
				Conversation conversation;
				
				// Deal with the conversation currently open in the Conversation Writer (if there is one): 				
				if (ConversationWriterWindow.Instance != null && 
				    Conversation.CurrentConversation != null &&
				    resource.FullName == ConversationWriterWindow.Instance.WorkingFilename) {
					
					conversation = Conversation.CurrentConversation;
				}
				else {
					NWN2GameConversation conv = new NWN2GameConversation(resource);
					conv.Demand();
					conversation = new Conversation(conv);
				}
				
				Say.Debug("Dealing with " + conversation.NwnConv.Name);
				
	        	foreach (NWN2ConversationConnector connector in conversation.NwnConv.AllConnectors) {		
					
					for (int i = 0; i < connector.Actions.Count; i++) { 
						if (DependsOnVariable(connector.Actions[i],variable)) {
							Say.Debug(connector.Actions.Count.ToString() + " actions.");
							Say.Debug("Removing Action " + connector.Actions[i].ToString());
							connector.Actions.Remove(connector.Actions[i]);
							Say.Debug(connector.Actions.Count.ToString() + " actions.");
						}
					}
					
					for (int i = 0; i < connector.Conditions.Count; i++) { 
						if (DependsOnVariable(connector.Conditions[i],variable)) {
							Say.Debug(connector.Conditions.Count.ToString() + " conditions.");
							Say.Debug("Removing Condition " + connector.Conditions[i].ToString());
							connector.Conditions.Remove(connector.Conditions[i]);
							Say.Debug(connector.Conditions.Count.ToString() + " conditions.");
						}
					}
	       		}
				
				if (conversation != Conversation.CurrentConversation) {
					conversation.Serialize();
					conversation.NwnConv.Release();
				}
				else {
					ConversationWriterWindow.Instance.RefreshDisplay(false); // SaveToWorkingCopy() called through this.
				}
			}
			
			
			// Remove all references to this variable in the module's OnEvent scripts:
			
			// TODO once the code for OnEvent stuff has been done
		}
        
        
        /// <summary>
        /// Check whether a particular script functor uses the variable represented by this control
        /// </summary>
        /// <param name="functor">The script functor to check</param>
        /// <param name="variable">The variable to check for</param>
        /// <returns>True if the script functor is dependent upon this variable, false otherwise</returns>
        private static bool DependsOnVariable(NWN2ScriptFunctor functor, NWN2ScriptVariable variable)
        {
			foreach (NWN2ScriptParameter parameter in functor.Parameters) {
				if (parameter.ValueString == variable.Name) {
	        		return true;
				}
			}	
        	return false;
        }
	}
}
