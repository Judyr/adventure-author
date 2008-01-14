/*
 *   This file is part of Adventure Author.
 *
 *   Adventure Author is copyright Heriot-Watt University 2006-2008.
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
using System.IO;
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
using form = NWN2Toolset.NWN2ToolsetMainForm;

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
			string startingValueMessage = null;
			switch (variable.VariableType) {
				case NWN2ScriptVariableType.String:
					if (variable.ValueString != null) {
						startingValueMessage = " (starting value: " + variable.ValueString + ")";
					}
					break;
				case NWN2ScriptVariableType.Int:
					startingValueMessage = " (starting value: " + variable.ValueInt + ")";
					break;
				default:
					throw new ArgumentException("Variable of type " + variable.VariableType.ToString() + " is not supported.");
			}
			if (startingValueMessage != null) {
				Log.WriteAction(Log.Action.added,"variable","'" + variable.Name + "'" + startingValueMessage);
			}
			else {
				Log.WriteAction(Log.Action.added,"variable","'" + variable.Name + "'");
			}
	        form.App.Module.ModuleInfo.Variables.Add(variable);
			if (VariablesWindow.Instance != null) {
				VariablesWindow.Instance.RefreshVariablesList();
			}
		}
		
		
		/// <summary>
		/// Delete a variable, and optionally remove all references to it
		/// </summary>
		/// <param name="variable">The variable to delete</param>
		/// <param name="removeReferences">True to also remove all references in conversations and scripts, false to only delete the variable</param>
		public static void Delete(NWN2ScriptVariable variable, bool removeReferences)
		{			
			Log.WriteAction(Log.Action.deleted,"variable",variable.Name);
			
			if (removeReferences) {
				RemoveReferences(variable);
			}
			
			form.App.Module.ModuleInfo.Variables.Remove(variable);
			ModuleHelper.Save();
			if (VariablesWindow.Instance != null) {
				VariablesWindow.Instance.RefreshVariablesList();
			}
		}
		
			
		/// <summary>
		/// Remove all references in conversation scripts and OnEvent scripts to a given variable
		/// </summary>
		/// <param name="variable">The variable to remove references for</param>
		private static void RemoveReferences(NWN2ScriptVariable variable)
		{			
			// Remove all references to this variable in the module's conversations:
        	IResourceRepository moduleRepos = ResourceManager.Instance.GetRepositoryByName(form.App.Module.Repository.Name); // was modulepath
            if (moduleRepos == null) {
            	Say.Error("Couldn't locate the module as a resource repository.");
            	return;
            }      
        	
        	ushort DLG = BWResourceTypes.GetResourceType("dlg");
			OEIGenericCollectionWithEvents<IResourceEntry> DLGfiles = moduleRepos.FindResourcesByType(DLG);

			foreach (IResourceEntry resource in DLGfiles) {
				Conversation conversation;
								
				// Deal with the conversation currently open in the Conversation Writer (if there is one): 				
				if (WriterWindow.Instance != null && 
				    Conversation.CurrentConversation != null &&
				    resource.FullName == WriterWindow.Instance.WorkingFilename + ".DLG") {
					
					conversation = Conversation.CurrentConversation;
				}
				else {
					NWN2GameConversation conv = new NWN2GameConversation(resource);
					conv.Demand();
					conversation = new Conversation(conv);
				}
				
	        	foreach (NWN2ConversationConnector connector in conversation.NwnConv.AllConnectors) {		
					
					List<NWN2ScriptFunctor> defunctActions = new List<NWN2ScriptFunctor>();
					List<NWN2ConditionalFunctor> defunctConditions = new List<NWN2ConditionalFunctor>();
					
					for (int i = 0; i < connector.Actions.Count; i++) { 
						if (DependsOnVariable(connector.Actions[i],variable)) {
							defunctActions.Add(connector.Actions[i]);
						}
					}
					
					for (int i = 0; i < connector.Conditions.Count; i++) { 
						if (DependsOnVariable(connector.Conditions[i],variable)) {
							defunctConditions.Add(connector.Conditions[i]);
						}
					}
					
					
					foreach (NWN2ScriptFunctor removable in defunctActions) {
						Conversation.CurrentConversation.DeleteAction(connector,removable);
					}
					
					foreach (NWN2ConditionalFunctor removable in defunctConditions) {
						Conversation.CurrentConversation.DeleteCondition(connector,removable);
					}
	       		}
				
				if (conversation != Conversation.CurrentConversation) {
					conversation.Serialize();
					conversation.NwnConv.Release();
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
        
        
        /// <summary>
        /// Check whether this variable name is taken.
        /// </summary>
        /// <param name="variableName">The variable name to check for</param>
        /// <returns>True if the name is available, false if there is already a variable by that name</returns>
        public static bool NameIsAvailable(string variableName)
        {
        	return form.App.Module.ModuleInfo.Variables.GetVariable(variableName) == null;
        }
        
        
        /// <summary>
        /// Check whether this is a valid name for a variable, based on length and the absence of invalid characters.
        /// </summary>
        /// <param name="variableName">The variable name to check</param>
        /// <returns>True if this is a valid name, false otherwise</returns>
        public static bool NameIsValid(string variableName)
        {
        	foreach (char c in Path.GetInvalidFileNameChars()) {
        		if (variableName.Contains(c.ToString())) {
        			return false;
        		}
        	}
        	return variableName.Length <= ModuleHelper.MAX_RESOURCE_NAME_LENGTH;
        }
	}
}
