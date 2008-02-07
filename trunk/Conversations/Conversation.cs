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
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Media;
using AdventureAuthor.Conversations.UI;
using AdventureAuthor.Conversations.UI.Controls;
using AdventureAuthor.Core;
using AdventureAuthor.Scripts;
using AdventureAuthor.Utils;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.ConversationData;
using OEIShared.Utils;
using OEIShared.IO;
using form = NWN2Toolset.NWN2ToolsetMainForm;

namespace AdventureAuthor.Conversations
{
	/// <summary>
	/// Conversation wraps a NWN2GameConversation with some additional information for the Conversation Writer.
	/// </summary>
	public partial class Conversation
	{
		#region Classes
		
		public struct DataFromConversation
		{
			public int words;
			public int lines;
			public int pages;
			
			public DataFromConversation(int words, int lines, int pages)
			{
				this.words = words;
				this.lines = lines;
				this.pages = pages;
			}
		}
		
		#endregion Classes
				
		#region Constants
		
		public const byte MAX_NUMBER_OF_SPEAKERS_IN_CONVERSATION = 3;
		public static readonly SolidColorBrush PLAYER_COLOUR = Brushes.Blue;
		public static readonly SolidColorBrush BRANCH_COLOUR = Brushes.Black;
		public const string PLAYER_NAME = "Player";
				
		/// <summary>
		/// Set the comment field on a line to this value to identify it as a non-filler line.
		/// </summary>
		private const string IDENTIFY_AS_NOT_FILLER = "notfiller";
				
		/// <summary>
		/// Set the comment field on a line to this value to identify it as a filler line.
		/// </summary>
		private const string IDENTIFY_AS_FILLER = "filler";
		
		#endregion Constants
				
		#region Events	
		
		public event EventHandler<ConversationChangedEventArgs> Changed;
		public event EventHandler<EventArgs> Saved;
		public event EventHandler<SpeakerAddedEventArgs> SpeakerAdded;
		
		protected virtual void OnChanged(ConversationChangedEventArgs e)
		{
			Say.Debug("Conversation.Changed event raised.");
			EventHandler<ConversationChangedEventArgs> handler = Changed;
			if (handler != null) {
				handler(this,e);
			}
		}		
		
		protected virtual void OnSaved(EventArgs e)
		{
			Say.Debug("Conversation.Saved event raised.");
			EventHandler<EventArgs> handler = Saved;
			if (handler != null) {
				handler(this,e);
			}
		}		
		
		protected virtual void OnSpeakerAdded(SpeakerAddedEventArgs e)
		{
			Say.Debug("Conversation.SpeakersAdded event raised.");
			EventHandler<SpeakerAddedEventArgs> handler = SpeakerAdded;
			if (handler != null) {
				handler(this,e);
			}
		}		
				
		/// <summary>
		/// Save the working copy to disk when a change is made. Must fire before any other event handlers.
		/// </summary>
		private void Conversation_OnChanged(object sender, ConversationChangedEventArgs e)
		{
			Say.Debug("Conversation_OnChanged()");
			SaveToWorkingCopy();
		}		
				
		#endregion
		
		#region Fields
		
		private static Conversation currentConversation;		
		public static Conversation CurrentConversation {
			get { return currentConversation; }
			set { currentConversation = value; }
		}
		
		private static object padlock = new object();
		
		/// <summary>
		/// The game conversation.
		/// </summary>
		private NWN2GameConversation nwnConv;		
		public NWN2GameConversation NwnConv {
			get { return nwnConv; }
		}
		
		/// <summary>
		/// Characters who can speak in this conversation. Always includes the player.
		/// </summary>
		private List<Speaker> speakers;	
		public List<Speaker> Speakers {
			get { return speakers; }
		}
		
		/// <summary>
		/// Whether or not a change has been made to the conversation since the last save.
		/// </summary>
		private bool isDirty;		
		public bool IsDirty {
			get { return isDirty; }
		}		
		
		/// <summary>
		/// Colours to assign to speakers as they are added.
		/// </summary>
		private List<Brush> unassignedColours;
		
		/// <summary>
		/// For the purposes of exporting a conversation file into text form.
		/// </summary>
		private StreamWriter sw = null;
		
		#endregion Fields
		
		#region Constructors 
		
		/// <summary>
		/// Create a new conversation wrapped around a game conversation object.
		/// </summary>
		/// <remarks>The game conversation object is assumed to be retrieved from disk prior to calling this constructor</remarks>
		/// <param name="conv">The game conversation object.</param>
		public Conversation(NWN2GameConversation conv)
		{
			this.nwnConv = conv;
			this.speakers = new List<Speaker>(Conversation.MAX_NUMBER_OF_SPEAKERS_IN_CONVERSATION);
			this.unassignedColours = new List<Brush>(Conversation.MAX_NUMBER_OF_SPEAKERS_IN_CONVERSATION);
			this.unassignedColours.Add(Brushes.Red); 
			this.unassignedColours.Add(Brushes.DarkGreen); 
			this.unassignedColours.Add(Brushes.Brown);
			isDirty = false;
			this.Changed += new EventHandler<ConversationChangedEventArgs>(Conversation_OnChanged);
		}
		
		#endregion Constructors
		
		#region Methods
		
		#region Speakers
		
		/// <summary>
		/// Add an object with a given tag as a speaker in this conversation.
		/// </summary>
		/// <param name="tag">The tag of the object (usually a creature) who will be a speaker, or String.Empty to add the player.</param>
		/// <returns>A speaker object, representing that speaker's tag and assigned colour.</returns>
		public Speaker AddSpeaker(string tag)
		{
			Speaker existingSpeaker = GetSpeaker(tag);
			if (existingSpeaker == null) {	
				if (tag == String.Empty) {
					Log.WriteAction(Log.Action.added,"speaker","player");
				}
				else {
					Log.WriteAction(Log.Action.added,"speaker",tag);
				}
				Log.WriteAction(Log.Action.added,"speaker",tag);
				Speaker speaker = new Speaker(tag);
				speakers.Add(speaker);
				AssignColour(speaker);
				OnSpeakerAdded(new SpeakerAddedEventArgs(speaker));
				return speaker;
			}
			else {
				Say.Error("Tried to add a speaker that already exists.");
				return existingSpeaker;
			}
		}
		
		
		/// <summary>
		/// Get the speaker with a given tag, if one exists.
		/// </summary>
		/// <param name="tag">The tag the returned speaker must have.</param>
		/// <returns>The speaker with the given tag, or null if no such speaker exists.</returns>
		public Speaker GetSpeaker(string tag)
		{
			foreach (Speaker speaker in speakers) {
				if (speaker.Tag.ToUpper() == tag.ToUpper()) {
					return speaker;
				}
			}
			return null;
		}
		
		
		/// <summary>
		/// Assign a unique UI colour to a speaker, or black if all unique colours have been assigned.
		/// </summary>
		/// <param name="speaker"></param>
		private void AssignColour(Speaker speaker)
		{
			if (speaker.Tag == String.Empty) { // player
				speaker.Colour = Conversation.PLAYER_COLOUR;
			}
			else if (unassignedColours.Count > 0) {
				Brush colour = unassignedColours[0];
				unassignedColours.Remove(colour);
				speaker.Colour = colour;
			}
			else {
				speaker.Colour = Brushes.Black;
			}
		}
		
		#endregion Speakers
		
		#region Editing lines
		
		/// <summary>
		/// Add an action to a line of dialogue.
		/// </summary>
		/// <param name="line">The line of dialogue</param>
		/// <param name="action">The action to add</param>
		public void AddAction(NWN2ConversationConnector line, NWN2ScriptFunctor action)
		{
			if (line == null) {
				Say.Error("Can't operate on a null line.");
			}
			else if (action == null) {
				Say.Error("Can't add a null action.");
			}
			else {
				Log.WriteAction(Log.Action.added,"action",ScriptHelper.GetDescriptionForAction(action));
				line.Actions.Add(action);
				OnChanged(new ConversationChangedEventArgs(false));
			}
		}			
		
		
		/// <summary>
		/// Add a condition to a line of dialogue.
		/// </summary>
		/// <param name="line">The line of dialogue</param>
		/// <param name="action">The action to add</param>
		public void AddCondition(NWN2ConversationConnector line, NWN2ConditionalFunctor condition)
		{
			if (line == null) {
				Say.Error("Can't operate on a null line.");
			}
			else if (line.Conditions.Count != 0) {
				Say.Error("Can't add more than one condition to a line.");
			}
			else if (condition == null) {
				Say.Error("Can't add a null condition.");
			}
			else {
				Log.WriteAction(Log.Action.added,"condition",ScriptHelper.GetDescriptionForCondition(condition));
				line.Conditions.Add(condition);
				OnChanged(new ConversationChangedEventArgs(false));
			}
		}	
		
		
		/// <summary>
		/// Delete an action from a line of dialogue.
		/// </summary>
		/// <param name="line">The line of dialogue</param>
		/// <param name="action">The action to delete</param>
		public void DeleteAction(NWN2ConversationConnector line, NWN2ScriptFunctor action)
		{
			if (line == null) {
				Say.Error("Can't operate on a null line.");
			}
			else if (action == null) {
				Say.Error("Can't delete a null action.");
			}
			else if (!line.Actions.Contains(action)) {
				Say.Error("Action " + action.Script.FullName + " does not exist on line + " + line.ToString());
			}
			else {
				Log.WriteAction(Log.Action.deleted,"action",ScriptHelper.GetDescriptionForAction(action));
				line.Actions.Remove(action);
				OnChanged(new ConversationChangedEventArgs(false));
			}
		}		
		
				
		/// <summary>
		/// Delete all actions on a line of dialogue.
		/// </summary>
		/// <param name="line">The line of dialogue</param>
		public void DeleteAllActions(NWN2ConversationConnector line)
		{
			if (line == null) {
				Say.Error("Can't operate on a null line.");
			}
			else {
				Log.WriteMessage("deleted all actions on line");
				line.Actions.Clear();
				OnChanged(new ConversationChangedEventArgs(false));
			}
		}
				
		
		/// <summary>
		/// Delete all conditions on a line of dialogue.
		/// </summary>
		/// <param name="line">The line of dialogue</param>
		public void DeleteAllConditions(NWN2ConversationConnector line)
		{
			if (line == null) {
				Say.Error("Can't operate on a null line.");
			}
			else {
				Log.WriteMessage("deleted all conditions on line");
				line.Conditions.Clear();
				OnChanged(new ConversationChangedEventArgs(false));
			}
		}
		
		
		/// <summary>
		/// Delete a condition from a line of dialogue.
		/// </summary>
		/// <param name="line">The line of dialogue</param>
		/// <param name="condition">The condition to delete</param>
		public void DeleteCondition(NWN2ConversationConnector line, NWN2ConditionalFunctor condition)
		{
			if (line == null) {
				Say.Error("Can't operate on a null line.");
			}
			else if (condition == null) {
				Say.Error("Can't delete a null condition.");
			}
			else if (!line.Conditions.Contains(condition)) {
				Say.Error("Condition " + condition.Script.FullName + " does not exist on line + " + line.ToString());
			}
			else {
				Log.WriteAction(Log.Action.deleted,"condition",ScriptHelper.GetDescriptionForAction(condition));
				line.Conditions.Remove(condition);
				OnChanged(new ConversationChangedEventArgs(false));
			}
		}
			
		
		// TODO
		public void SetCameraAngle(NWN2ConversationConnector line)
		{
			Log.WriteAction(Log.Action.set,"camera");
			throw new NotImplementedException();
//			OnChanged(new ConversationChangedEventArgs(false));
		}		
		

		/// <summary>
		/// Replace an existing action on a line of dialogue with a new action.
		/// </summary>
		/// <param name="line">The line of dialogue</param>
		/// <param name="originalAction">The action to replace</param>
		/// <param name="newAction">The new action</param>
		/// <remarks>Used to update an action with a new set of parameters</remarks>
		public void ReplaceAction(NWN2ConversationConnector line, NWN2ScriptFunctor originalAction, NWN2ScriptFunctor newAction)
		{
			if (line == null) {
				Say.Error("Can't operate on a null line.");
			}
			else if (originalAction == null || newAction == null) {
				Say.Error("Can't operate on a null action.");
			}
			else if (!line.Actions.Contains(originalAction)) {
				Say.Error("Action " + originalAction.Script.FullName + " does not exist on line + " + line.ToString());
			}
			else {
				Log.WriteAction(Log.Action.edited,"action",ScriptHelper.GetDescriptionForAction(newAction));
				line.Actions.Remove(originalAction);
				line.Actions.Add(newAction);
				OnChanged(new ConversationChangedEventArgs(false));
			}
		}	
		
		
		/// <summary>
		/// Replace an existing condition on a line of dialogue with a new condition.
		/// </summary>
		/// <param name="line">The line of dialogue</param>
		/// <param name="originalCondition">The condition to replace</param>
		/// <param name="newCondition">The new condition</param>
		/// <remarks>Used to update a condition with a new set of parameters</remarks>
		public void ReplaceCondition(NWN2ConversationConnector line, NWN2ConditionalFunctor originalCondition, NWN2ConditionalFunctor newCondition)
		{
			if (line == null) {
				Say.Error("Can't operate on a null line.");
			}
			else if (originalCondition == null || newCondition == null) {
				Say.Error("Can't operate on a null condition.");
			}
			else if (!line.Conditions.Contains(originalCondition)) {
				Say.Error("Condition " + originalCondition.Script.FullName + " does not exist on line + " + line.ToString());
			}
			else {
				Log.WriteAction(Log.Action.edited,"condition",ScriptHelper.GetDescriptionForAction(newCondition));
				line.Conditions.Remove(originalCondition);
				line.Conditions.Add(newCondition);
				OnChanged(new ConversationChangedEventArgs(false));
			}
		}	
		
		
		/// <summary>
		/// Set the text of a line of dialogue.
		/// </summary>
		/// <param name="line">The line of dialogue</param>
		/// <param name="newText">The new value to assign to the line text</param>
		public void SetText(NWN2ConversationConnector line, string newText)
		{
			if (line == null) {
				Say.Error("Cannot operate on a null line.");
			}
			else if (newText == null) {
				Say.Error("Cannot assign a null string to this line.");
			}
			else {
				Log.WriteAction(Log.Action.edited,"line");
				line.Line.Text = GetOEIStringFromString(newText);
				
				// If this is the first line of a non-root node, the node labels on the graph will need to be refreshed:
				if (line.Parent != null && line.Parent.Line.Children.Count > 1) { 
					OnChanged(new ConversationChangedEventArgs(true));
				}
				else {
					OnChanged(new ConversationChangedEventArgs(false));
				}
			}
		}
		
		
		/// <summary>
		/// Set the sound file to be played on a line of dialogue.
		/// </summary>
		/// <param name="line">The line of dialogue</param>
		/// <param name="sound">The sound file to play when this line is spoken; null to play no sound</param>
        public void SetSound(NWN2ConversationConnector line, IResourceEntry sound)
        {
			if (line == null) {
				Say.Error("Can't operate on a null line.");
        	}
			else {
        		if (sound == null) {
        			Log.WriteAction(Log.Action.deleted,"sound");
        		}
        		else {
        			Log.WriteAction(Log.Action.set,"sound");
        		}
				line.Sound = sound; // valid for this to be null
				OnChanged(new ConversationChangedEventArgs(false));
			}
        }
	        
		#endregion
		
		#region Adding lines
		
		/// <summary>
		/// Add a new blank line to an existing choice.
		/// </summary>
		/// <param name="parent">The parent line of the choice</param>
		/// <returns>The newly created branch</returns>
		public NWN2ConversationConnector AddLineToChoice(NWN2ConversationConnector parent)
		{
			Log.WriteAction(Log.Action.added,"branch");
			NWN2ConversationConnector createdLine = _AddLineToChoice(parent);
			OnChanged(new ConversationChangedEventArgs(true));
			return createdLine;
		}
		
		
		/// <summary>
		/// Add a new blank line to an existing choice.
		/// </summary>
		/// <param name="parent">The parent line of the choice</param>
		/// <returns>The newly created branch</returns>
		private NWN2ConversationConnector _AddLineToChoice(NWN2ConversationConnector parent)
		{
			string speaker = null;
			NWN2ConversationConnectorCollection children = GetChildren(parent);
			
			foreach(NWN2ConversationConnector option in children) {
				if (speaker == null) {
					speaker = option.Speaker;
				}
				else if (speaker != option.Speaker) {
					throw new ArgumentException("Found different speakers in the same branch.");
				}
			}
			
			NWN2ConversationConnector createdLine = CreateNewLine(parent,speaker,false);
			return createdLine;
		}
		
				
		/// <summary>
		/// Add a new blank line underneath the specified line.
		/// </summary>
		/// <remarks>To add a line to a choice, call AddLineToChoice instead.</remarks>
		/// <param name="preceding">The existing line which the new line should follow. Pass null to add to root.
		/// Note that this is the visible preceding line, rather than the parent line - filler lines do not
		/// need to be accounted for.</param>
		/// <param name="speaker">The tag of the line's speaker</param>
		/// <returns>The newly created line</returns>
		public NWN2ConversationConnector AddLine(NWN2ConversationConnector preceding, string speaker)
		{
			Log.WriteAction(Log.Action.added,"line",speaker);
			NWN2ConversationConnector createdLine = _AddLine(preceding,speaker);
			OnChanged(new ConversationChangedEventArgs(false));
			return createdLine;
		}
		
				
		/// <summary>
		/// Add a new blank line underneath the specified line.
		/// </summary>
		/// <remarks>To add a line to a choice, call AddLineToChoice instead.</remarks>
		/// <param name="preceding">The existing line which the new line should follow. Pass null to add to root.
		/// Note that this is the visible preceding line, rather than the parent line - filler lines do not
		/// need to be accounted for.</param>
		/// <param name="speaker">The tag of the line's speaker</param>
		/// <returns>The newly created line</returns>
		private NWN2ConversationConnector _AddLine(NWN2ConversationConnector preceding, string speaker)
		{
			NWN2ConversationConnector createdLine = CreateNewLine(preceding,speaker,true);
			return createdLine;
		}
			
		#endregion
		
		#region Deleting lines
		
		public void DeleteEntireChoice(NWN2ConversationConnector parent)
		{
			NWN2ConversationConnectorCollection children = GetChildren(parent);
			Log.WriteAction(Log.Action.deleted,"choice",ScriptHelper.GetPlayerIfBlank(children[0].Speaker));
			children.Clear();
        	OnChanged(new ConversationChangedEventArgs(true));
		}
		
		
		public NWN2ConversationConnectorCollection DeleteLineFromChoice(NWN2ConversationConnector line)
		{			
			NWN2ConversationConnectorCollection siblings = GetSiblings(line);
			if (siblings.Count < 2) {
				throw new InvalidOperationException("Illegal call to DeleteLineFromChoice - given line was not part " +
				                                   "of a choice. Call DeleteLine instead.");
			}
			if (!(this.Contains(line))) {
				return null;
			}
			
			Log.WriteAction(Log.Action.deleted,"branch");
			
			NWN2ConversationConnectorCollection children = Conversation.CurrentConversation.nwnConv.RemoveNode(line);
			
			// If there is only one branch left, the choice will be removed, so clear any conditions:
			if (siblings.Count == 1) {
				siblings[0].Conditions.Clear();
			}
			
			OnChanged(new ConversationChangedEventArgs(true));
			return children;
		}
			
		
		public void DeleteLine(NWN2ConversationConnector line)
		{			
			Log.WriteAction(Log.Action.deleted,"line");
			_DeleteLine(line);			
			OnChanged(new ConversationChangedEventArgs(false));
		}
		
		
		private void _DeleteLine(NWN2ConversationConnector line)
		{
			Say.Debug("Ran DeleteLine");
			if (GetSiblings(line).Count > 1) {
				throw new InvalidOperationException("Called DeleteLine illegally - call DeleteLineFromChoice if you want to " +
					                                "delete a line which has siblings other than itself.");
			}
			
			if (GetChildren(line).Count == 0) { // if the line has no children, just delete it
				Say.Debug("Line has no children, so just delete it.");
				Conversation.CurrentConversation.NwnConv.RemoveNode(line);
			}
			else { // otherwise, the line has children
				if (line.Parent == null) { // the line must be an NPC starting entry, so just make it a filler line
					Say.Debug("line must be an NPC starting entry, so just make it a filler line");
					SetAsFillerLine(line);
				}				
				else { // TODO refactor to check more explicitly how many children it has/whether single child is filler etc. - bit ratty.
					NWN2ConversationConnector child = line.Line.Children[0];
					if (IsFiller(child)) { // if the child is a filler line, delete both the line and the child:	
						Say.Debug("Line's child is a filler line.");
						if (line.Line.Children.Count > 1) {
							throw new InvalidDataException("A filler line formed part of a choice/check.");
						}
						else {
							Say.Debug("Give all of the line's child's children to the line's parent, and remove the child and the line.");
							NWN2ConversationConnectorCollection linesToBeReparented = new NWN2ConversationConnectorCollection();
							foreach (NWN2ConversationConnector lineToBeReparented in child.Line.Children) {
								linesToBeReparented.Add(lineToBeReparented);
							}
							foreach(NWN2ConversationConnector lineToBeReparented in linesToBeReparented) {						
								Conversation.CurrentConversation.NwnConv.ReparentNode(lineToBeReparented,line.Parent.Line);
							}	
							Conversation.CurrentConversation.NwnConv.RemoveNode(child);
							Conversation.CurrentConversation.NwnConv.RemoveNode(line);
							// NB: Even if both line.Parent and the line's child are filler lines, we always choose to delete the child since
							// the Parent may be a filler starting entry (by leaving it there, we can allow conversations to start with the PC.)
						}
					}
					else { // if there is more than one child, or the single child is not a filler line:
						Say.Debug("Line has more than one child, or the single child is not a filler line.");
						if (IsFiller(line.Parent)) { // if the single child is a real line and the parent is a filler line, give the child to line.Parent.Parent:
							Say.Debug("The child is a non-filler line and the parent is a filler line, so give the child to line.Parent.Parent, and remove the parent and the line.");
							NWN2ConversationLine newParent;
							if (line.Parent.Parent == null) {
								newParent = null; // special case if the grandparent is root
							}
							else {
								newParent = line.Parent.Parent.Line;
							}							
							
							NWN2ConversationConnectorCollection linesToBeReparented = new NWN2ConversationConnectorCollection();
							foreach (NWN2ConversationConnector lineToBeReparented in line.Line.Children) {
								linesToBeReparented.Add(lineToBeReparented);
							}
							foreach(NWN2ConversationConnector lineToBeReparented in linesToBeReparented) {						
								Conversation.CurrentConversation.NwnConv.ReparentNode(lineToBeReparented,newParent);
							}	
														
							Conversation.CurrentConversation.NwnConv.RemoveNode(line);						
							Conversation.CurrentConversation.NwnConv.RemoveNode(line.Parent); // delete the line and its parent
						}
						else { // if the child(ren) and parent are real lines, make line a filler line:
							Say.Debug("Both the children and the parent are real lines, so make the current line into a filler line.");
							SetAsFillerLine(line);	
						}						
					}
				}
			}
		}	
		
		#endregion
		
		#region Moving lines
		
		/// <summary>
		/// Copy the fields of one line to another. Necessary because we may wish to move a line to another location,
		/// but 'deleting' it at its original location actually involves making it into a filler line, so we 
		/// can't use the same object.
		/// </summary>
		/// <param name="original">The line to create a copy of</param>
		/// <param name="copy">The (existing) line which will become a copy of the original</param>
		private void Copy(NWN2ConversationConnector original, ref NWN2ConversationConnector copy)
		{
			PropertyInfo[] fields = typeof(NWN2ConversationConnector).GetProperties(BindingFlags.Public | 
			                                                                 	 	BindingFlags.Instance);
						
			foreach (PropertyInfo f in fields) {
				if (f.Name == "Comment" || f.Name == "ConnectorID" || f.Name == "Line" || 
				    f.Name == "Parent" || f.Name == "TreeNode" || f.Name == "Type" ||
				    f.Name == "Actions" || f.Name == "Conditions" || f.Name == "Text") {
					Say.Debug("Skipped " + f.Name);
				}	
				else {
					f.SetValue(copy,f.GetValue(original,null),null);
					Say.Debug("Shallow copied " + f.Name);
				}
			}
			
			// Need deep-copy:
			foreach (NWN2ScriptFunctor action in original.Actions) {
				copy.Actions.Add(action);
			}
			foreach (NWN2ConditionalFunctor condition in original.Conditions) {
				copy.Conditions.Add(condition);
			}
			copy.Text = (OEIExoLocString)original.Text.Clone();
			// TODO - does CameraInfo require a deep copy as well?
		}
		
		
		public void MoveLineIntoChoice(NWN2ConversationConnector line, NWN2ConversationConnector choiceParent)
		{
			Log.WriteAction(Log.Action.dragdropped,"line","into choice, creating a new branch");
			
			NWN2ConversationConnectorCollection existingBranches = GetChildren(choiceParent);
			
			if (line == null) {			
				throw new ArgumentNullException("line","Cannot operate on a null line.");
			}
			if (choiceParent == null) {
				throw new ArgumentNullException("choiceParent","Cannot operate on a null line.");
			}
			if (!Conversation.CurrentConversation.Contains(line)) {
				throw new ArgumentException("line","Passed line does not exist in this conversation.");
			}
			if (existingBranches.Count < 2) {
			    throw new InvalidOperationException("Illegal call to MoveLineIntoChoice - call MoveLine " +
			    	                                "if you just want to move a line to a different (non-choice) position.");
			}
			if (GetSiblings(line).Count > 1) {
				throw new InvalidOperationException("Cannot move an existing branch line into another choice. " + 
				                                    "Call MoveLineWithinChoice instead.");
			}
			if (line.Parent == choiceParent) {
				throw new InvalidOperationException("Tried to move a line into a choice it was already a part of.");
			}
			
			if (line.Speaker != existingBranches[0].Speaker) {
				Say.Warning("You can only add branches by the same speaker (" + 
				            ScriptHelper.GetPlayerIfBlank(existingBranches[0].Speaker).ToLower() + ").");
				return;
			}
							
			bool? result = Say.Question("You are about to make this line into a new branch of the choice at the bottom of the page. " +
			             "This cannot be reversed. Continue?","Confirm",System.Windows.Forms.MessageBoxButtons.OKCancel);
			if (result != null && (bool)result) {
				// Create a new branch, copy the properties of the original and then delete it:
				NWN2ConversationConnector copy = Conversation.CurrentConversation._AddLineToChoice(choiceParent);
				Copy(line,ref copy);
				_DeleteLine(line);
				OnChanged(new ConversationChangedEventArgs(true));
			}
		}
		
		
		public void MoveLineWithinChoice(NWN2ConversationConnector line, NWN2ConversationConnector newPrecedingBranch)
		{
			Log.WriteAction(Log.Action.dragdropped,"line","within choice, re-ordering existing branches");
			                
			if (line == null) {			
				throw new ArgumentNullException("line","Cannot operate on a null line.");
			}
			if (newPrecedingBranch == null) {
				throw new ArgumentNullException("newPrecedingBranch","Cannot operate on a null line.");
			}
			if (line.Parent != newPrecedingBranch.Parent) {
				throw new InvalidOperationException("The two lines were not part of the same choice.");
			}
			if (!Conversation.CurrentConversation.Contains(line)) {
				throw new ArgumentException("line","Passed line does not exist in this conversation.");
			}
			if (!Conversation.CurrentConversation.Contains(newPrecedingBranch)) {
				throw new ArgumentException("newPrecedingBranch","Passed line does not exist in this conversation.");
			}
			
			NWN2ConversationConnectorCollection children = GetChildren(line.Parent);
			children.Remove(line);
			int index = children.IndexOf(newPrecedingBranch) + 1;
			
			if (index > (children.Count - 1)) { // if outside index bounds
				children.Add(line);
			}
			else {
				children.Insert(index,line);
			}
			OnChanged(new ConversationChangedEventArgs(true));
		}				
		
		      
		/// <summary>
		/// Move a Line to another position on the same page.
		/// </summary>
		/// <remarks>Call MoveLine to rearrange the lines on a page, MoveLineWithinChoice to rearrange the branches
		/// of a choice, and MoveLineIntoChoice to remove a line from the page and make it into a new branch on an 
		/// existing choice.</remarks>
		/// <param name="line">The line to move</param>
		/// <param name="newPrecedingLine">The line that should now come before it (not necessarily the parent)</param>
		public void MoveLine(NWN2ConversationConnector line, NWN2ConversationConnector newPrecedingLine)
		{
			Log.WriteAction(Log.Action.dragdropped,"line");
			
			if (line == null) {			
				Say.Debug("line == null");
				throw new ArgumentNullException("line","Cannot move a null line.");
			}
			if (!Conversation.CurrentConversation.Contains(line)) {
				Say.Debug("!Conversation.CurrentConversation.Contains(line)");
				throw new ArgumentException("line","Passed line does not exist in this conversation.");
			}
			if (newPrecedingLine == line) {
				Say.Debug("Drag-dropped line over itself - do nothing.");
				return; // dragged over self
			}
			// TODO if trying to drop over the line's original preceding line, return as well
					
			// Create a new line, copy the properties of the original and then delete it:
			NWN2ConversationConnector copy = _AddLine(newPrecedingLine,line.Speaker);
			Copy(line,ref copy);
			_DeleteLine(line);
			OnChanged(new ConversationChangedEventArgs(false));
		}
		
		#endregion
		
		#region Saving
		
		/// <summary>
		/// Save the current working copy as the master copy.
		/// </summary>
		public void SaveToOriginal()
		{
			if (this != CurrentConversation) {
				throw new InvalidOperationException("Tried to operate on a closed Conversation.");
			}
			
			Log.WriteAction(Log.Action.saved,"conversation");
			
			// Changes to a line's text are not saved immediately, so save changes before going any further:
			if (WriterWindow.Instance.SelectedLineControl != null && !IsFiller(WriterWindow.Instance.SelectedLineControl.Nwn2Line)) {
				WriterWindow.Instance.SelectedLineControl.SaveChangesToText();
			}
			
			lock (padlock) {				
				NwnConv.OEISerialize(false);
				string originalPath = Path.Combine(form.App.Module.Repository.DirectoryName,WriterWindow.Instance.OriginalFilename+".dlg");
				string workingPath = Path.Combine(form.App.Module.Repository.DirectoryName,WriterWindow.Instance.WorkingFilename+".dlg");
				File.Copy(workingPath,originalPath,true);
				isDirty = false;
				OnSaved(new EventArgs());
			}
		}
		
		
		/// <summary>
		/// This should be called anytime a change is made to the conversation.
		/// </summary>
		private void SaveToWorkingCopy() 
		{
			if (this != CurrentConversation) {
				throw new InvalidOperationException("Tried to operate on a closed Conversation.");
			}
			
			try {
				lock (padlock) {
					NwnConv.OEISerialize(false); // TODO can still throw an error on OEISerialize: launch in separate thread ?				
					isDirty = true;
					Say.Debug("Saved to working copy.");
				}
			}
			catch (Exception e) {
				Say.Error("Error on serializing.",e);
			}
		}
		
		
		/// <summary>
		/// Messy - refactor?
		/// </summary>
		internal void Serialize()
		{
			lock (padlock) {
				NwnConv.OEISerialize(false);
			}
		}
		
		#endregion
					
		#region General methods
		
		public void ExportToTextFile(string filename)
		{
			FileInfo fi = new FileInfo(filename);
			if (sw != null) {
				sw.Dispose();
			}
			sw = fi.CreateText();
			sw.AutoFlush = false;
			
			foreach (NWN2ConversationConnector line in nwnConv.StartingList) {
				if (line.Conditions.Count > 0) {
					sw.WriteLine(ScriptHelper.GetDescriptionForCondition(line.Conditions));			
				}
				WriteToFile(line,String.Empty);
			}
			
			sw.Flush();
			sw.Close();
			sw.Dispose();
			Say.Information("Exported conversation to " + filename);
		}		
		
		
		private void WriteToFile(NWN2ConversationConnector line, string indent)
		{
			if (line == null) {
				throw new ArgumentNullException("Tried to write a null line to file.");
			}
			
			string newIndent = indent;
					
			if (!IsFiller(line)) {	
				
				string speakerName;
				if (line.Type == NWN2ConversationConnectorType.Reply) {
					speakerName = "PLAYER";
				}
				else if (line.Speaker == null || line.Speaker == String.Empty) {
					speakerName = "[OWNER]";
				}
				else {
					speakerName = line.Speaker.ToUpper();
				}
				sw.WriteLine(indent + speakerName + "\t\t" + GetStringFromOEIString(line.Text));
				
				foreach (NWN2ScriptFunctor action in line.Actions) {
					sw.WriteLine(indent + ScriptHelper.GetDescriptionForAction(action));
				}
				
				newIndent = "   " + newIndent;
			}
			
			foreach (NWN2ConversationConnector child in line.Line.Children) {
				if (child.Conditions.Count > 0) {
					sw.WriteLine(indent + ScriptHelper.GetDescriptionForCondition(child.Conditions));			
				}
				WriteToFile(child,newIndent);
			}
		}
		
			
		/// <summary>
		/// Get the number of words in a sentence.
		/// </summary>
		/// <param name="toCount">The sentence to count the words in.</param>
		/// <returns>The number of words in the sentence</returns>
		public static int WordCount(string toCount)
		{ 
			return toCount.Split(' ').Length;
		}
		
		
		/// <summary>
		/// Get the word, line and page count from either the whole conversation, or the conversation from a particular point.
		/// </summary>
		/// <param name="parent">The line to start counting from - pass null to start from the root.</param>
		/// <returns>The total word, line and page count of the conversation (or conversation segment).</returns>
		public DataFromConversation GetWordLinePageCounts(NWN2ConversationConnector parent)
		{			
			// Get the counts from the parent line before continuing:
			int words = 0;
			int lines = 0;
			int pages = 0;
			NWN2ConversationConnectorCollection children = GetChildren(parent);
			
			if (parent == null) {
				pages += children.Count; // if we have reached a branch point, increment the number of pages
			}
			else {				
				string text = GetStringFromOEIString(parent.Line.Text);
				if (text.Length > 0) {
					words += WordCount(text);
					lines++; // only count lines that have something written in them, not filler lines or blank new lines	
				}
				if (children != null && children.Count > 1) {
					pages += children.Count; // if we have reached a branch point, increment the number of pages
				}
			}		
			
			DataFromConversation childrenCounts = GetWordLinePageCounts(children);
			DataFromConversation totalCounts = new DataFromConversation(childrenCounts.words + words, childrenCounts.lines + lines, childrenCounts.pages + pages);
				
			return totalCounts;
		}
			
		
		public static OEIExoLocString GetOEIStringFromString(string text)
		{			
			OEIExoLocString str = new OEIExoLocString();
			OEIExoLocSubString substr = new OEIExoLocSubString();
			substr.Value = text;
			str.Strings.Add(substr);
			return str;
		}
		
		
		public static string GetStringFromOEIString(OEIExoLocString text)
		{
			if (text.Strings.Count == 0) {
				return String.Empty;
			}
			else {
				return text.Strings[0].Value;
			}
		}
		
		
		public static bool AreSameSpeakerType(NWN2ConversationConnectorType a, NWN2ConversationConnectorType b)
		{
			if (a == NWN2ConversationConnectorType.Reply) {
				return b == NWN2ConversationConnectorType.Reply;
			}
			else {
				return b != NWN2ConversationConnectorType.Reply;
			}
		}
		
		
		private static bool CanFollow(NWN2ConversationConnector line, NWN2ConversationConnectorType typeOfFollowingLine)
		{
			if (line == null) {
				return typeOfFollowingLine == NWN2ConversationConnectorType.StartingEntry;
			}
			else if (line.Type != NWN2ConversationConnectorType.Reply) { // only if following an existing NPC line can you add a PC line
				return typeOfFollowingLine == NWN2ConversationConnectorType.Reply;
			}
			else {
				return typeOfFollowingLine != NWN2ConversationConnectorType.Reply;
			}
		}
		
		
		/// <summary>
		/// Check whether a given line is contained within this conversation.
		/// </summary>
		/// <param name="line">The line to check for</param>
		/// <returns>True if the line exists in this conversation; false otherwise</returns>
		public bool Contains(NWN2ConversationConnector line)
		{
			return this.nwnConv.Entries.Contains(line.Line) || 
				this.nwnConv.Replies.Contains(line.Line) || 
				this.nwnConv.StartingList.Contains(line);
		}
						
		
		/// <summary>
		/// Returns the siblings of a given line, AND the line itself. 
		/// </summary>
		/// <param name="line">The line to return the siblings of.</param>
		/// <returns>The siblings of the line, AND the line itself (i.e. all of the line's parent's children)</returns>
		public NWN2ConversationConnectorCollection GetSiblings(NWN2ConversationConnector line)
		{
			if (line == null) {
				throw new ArgumentNullException("line","Can't operate on a null line.");
			}
			
			return GetChildren(line.Parent);
		}
		
		
		/// <summary>
		/// Returns the children of a given line. 
		/// </summary>
		/// <param name="line">The line to return the children of. Null may be passed (as root) to get the starting list.</param>
		/// <returns>The children of the line</returns>
		public NWN2ConversationConnectorCollection GetChildren(NWN2ConversationConnector line)
		{
			if (line == null) {
				return Conversation.CurrentConversation.NwnConv.StartingList;
			}
			else {
				return line.Line.Children;
			}
		}
		
		
		public DataFromConversation GetWordLinePageCounts(NWN2ConversationConnectorCollection parents)
		{			
			int words = 0;
			int lines = 0;
			int pages = 0;
			
			foreach (NWN2ConversationConnector parent in parents) {
				string text = GetStringFromOEIString(parent.Line.Text);
				if (text.Length > 0) {
					words += WordCount(text);
					lines++; // only count lines that have something written in them, not filler lines or blank new lines	
				}
				if (parent.Line.Children != null && parent.Line.Children.Count > 1) {
					pages += parent.Line.Children.Count; // if we have reached a branch point, increment the number of pages
				}
				DataFromConversation dataFromChildren = GetWordLinePageCounts(parent.Line.Children);
				words += dataFromChildren.words;
				lines += dataFromChildren.lines;
				pages += dataFromChildren.pages;
			}
			
			return new DataFromConversation(words,lines,pages);
		}
		
		public override string ToString()
		{
			StringBuilder s = new StringBuilder();
			if (this.nwnConv.Name != null) {
				s.Append(this.nwnConv.Name + " (" + speakers[0].ToString());
				for (int i = 1; i < speakers.Count; i++) {
					s.Append(", " + speakers[i].ToString());
				}
				s.Append(")");
			}
			else {
				s.Append("(Null conversation)");
			}
			return s.ToString();
		}
				
		#endregion
		
		// TODO reconcile these methods		
		
		#region Needs reconciled with other adding line methods, and updated with OnConversationChange
		
		public void MakeLineIntoChoice(NWN2ConversationConnector memberOfBranch)
		{
			Log.WriteAction(Log.Action.added,"choice",ScriptHelper.GetPlayerIfBlank(memberOfBranch.Speaker) + " (by making choice from an existing line)");
			Conversation.CurrentConversation.InsertNewLineWithoutReparenting(memberOfBranch.Parent,memberOfBranch.Speaker);
			OnChanged(new ConversationChangedEventArgs(true));
		}
		
		
		
		public void AddChoice(string speakerTag)
		{
			if (!WriterWindow.Instance.CurrentPage.IsEndPage) {
				throw new InvalidOperationException("Tried to add a branch at the end of a page that already had one.");
			}
			
			Log.WriteAction(Log.Action.added,"choice",ScriptHelper.GetPlayerIfBlank(speakerTag) + " (by adding new choice)");
			
        	NWN2ConversationConnector parent;
        	if (WriterWindow.Instance.CurrentPage.LineControls.Count > 0) {
        		parent = WriterWindow.Instance.CurrentPage.LineControls[WriterWindow.Instance.CurrentPage.LineControls.Count-1].Nwn2Line;
			}
			else {
        		parent = WriterWindow.Instance.CurrentPage.LeadLine; // LeadInLine may be null i.e. root
			}			
        	NWN2ConversationConnectorCollection children = GetChildren(parent);	
						
        	try {
				if (children.Count > 0) { // shouldn't happen as we are at the end of the page
					NWN2ConversationConnectorCollection childrenToRemove = new NWN2ConversationConnectorCollection();
					foreach (NWN2ConversationConnector child in children) {
						if (!IsFiller(child)) {
							throw new InvalidOperationException("The line at the end of the page had non-filler lines as children.");
						}		
						childrenToRemove.Add(child);
					}
					foreach (NWN2ConversationConnector child in childrenToRemove) {
						Conversation.CurrentConversation.NwnConv.RemoveNode(child);
					}
	        		Say.Debug("The line Adventure Author took to be the end of the page had filler lines as children - this shouldn't have happened.");
//					Say.Error("The line Adventure Author took to be the end of the page had filler lines as children - this shouldn't have happened.");
				}			
				
				// Insert the new lines, including a filler line if necessary:
				NWN2ConversationConnectorType newLineType;
				if (speakerTag == String.Empty) {
					newLineType = NWN2ConversationConnectorType.Reply;
				}
				else if (parent == null) {
					newLineType = NWN2ConversationConnectorType.StartingEntry;
				}
				else {
					newLineType = NWN2ConversationConnectorType.Entry;
				}	
				
				if (Conversation.CanFollow(parent,newLineType)) {
					Conversation.CurrentConversation.InsertNewLineWithoutReparenting(parent,speakerTag);
					Conversation.CurrentConversation.InsertNewLineWithoutReparenting(parent,speakerTag);
				}
				else {					
					NWN2ConversationConnector fillerLine = Conversation.CurrentConversation.InsertFillerLineWithoutReparenting(parent);
					Conversation.CurrentConversation.InsertNewLineWithoutReparenting(fillerLine,speakerTag);
					Conversation.CurrentConversation.InsertNewLineWithoutReparenting(fillerLine,speakerTag);	
				}			
								
				OnChanged(new ConversationChangedEventArgs(true));
        	}
        	catch (InvalidOperationException) {
        		Say.Error("The line at the end of the page had non-filler lines as children - failed to create a branch.");
        	}
		}				
		
					
		private NWN2ConversationConnector InsertNewLineWithoutReparenting(NWN2ConversationConnector parentLine, string speakerTag)
		{
			NWN2ConversationConnector newLine = Conversation.CurrentConversation.nwnConv.InsertChild(parentLine);
			SetAsNotFillerLine(newLine);
			newLine.Speaker = speakerTag;
			newLine.Sound = null;
			return newLine;
		}

		private NWN2ConversationConnector InsertFillerLineWithoutReparenting(NWN2ConversationConnector parentLine)
		{
			NWN2ConversationConnector fillerLine = Conversation.CurrentConversation.nwnConv.InsertChild(parentLine);
			SetAsFillerLine(fillerLine);
			return fillerLine;
		}
		
		#endregion
	
		#region Private methods for adding lines, do not call directly
						
		/// <summary>
		/// Do not call directly.
		/// </summary>
		private NWN2ConversationConnector CreateNewLine(NWN2ConversationConnector parent, string speakerTag, bool reparentChildren)
		{			
			if (this != currentConversation) {
				throw new InvalidOperationException("Tried to operate on a closed Conversation.");
			}
			
			NWN2ConversationConnectorType newLineType;	
			NWN2ConversationConnector newLine = null;
			
			if (speakerTag == String.Empty) {
				newLineType = NWN2ConversationConnectorType.Reply;
			}
			else if (parent == null) {
				newLineType = NWN2ConversationConnectorType.StartingEntry;
			}
			else {
				newLineType = NWN2ConversationConnectorType.Entry;
			}						
			
			if (parent == null) { // adding to root
				if (newLineType == NWN2ConversationConnectorType.Reply) { 	
//					Say.Debug("Adding a player line to root, need to add a filler line first.");
					newLine = CreateNewLine(parent,speakerTag,true,reparentChildren); // adding PC line to root is only possible by adding a filler line first
				}
				else {
//					Say.Debug("Adding an NPC line to root, no need to add a filler line first.");
					newLine = CreateNewLine(parent,speakerTag,false,reparentChildren); // can add an NPC line directly to root without adding a filler line first
				}
			}
			else { // adding to an existing line	
				if (!CanFollow(parent,newLineType)) { // if the new line can't be directly added, put in a filler line first
//					Say.Debug("Adding a " + newLineType.ToString() + " line to a " + parent.Type.ToString() + " line - need a filler line first.");
					newLine = CreateNewLine(parent,speakerTag,true,reparentChildren);
				}
				else { // add the new line to the parent	
//					Say.Debug("Adding a " + newLineType.ToString() + " line to a " + parent.Type.ToString() + " line - don't need a filler line.");
					newLine = CreateNewLine(parent,speakerTag,false,reparentChildren); 
				}
			}
			
//			Say.Debug("Set new line: " + GetStringFromOEIString(newLine.Text) + " ... as a non-filler line.");
			SetAsNotFillerLine(newLine);
			return newLine;
		}			
				
		/// <summary>
		/// Do not call directly.
		/// </summary>
		private NWN2ConversationConnector CreateNewLine(NWN2ConversationConnector parent, string speakerTag, bool addFillerBeforeNewLine, bool reparentChildren)
		{
			NWN2ConversationConnector newLine = null;
			NWN2ConversationConnector fillerLine = null;
			NWN2ConversationConnectorCollection children = GetChildren(parent);
		
			if (addFillerBeforeNewLine) {
				fillerLine = Conversation.CurrentConversation.NwnConv.InsertChild(parent);
//				if (parent ==  null) {
//					Say.Debug("Added " + GetStringFromOEIString(fillerLine.Text) + " as a filler line, child of root.");				
//				}
//				else {
//					Say.Debug("Added " + GetStringFromOEIString(fillerLine.Text) + " as a filler line, child of " + GetStringFromOEIString(parent.Text) + ".");
//				}
				SetAsFillerLine(fillerLine);
//				Say.Debug("Set it to be a filler line.");
				newLine = Conversation.CurrentConversation.NwnConv.InsertChild(fillerLine);	
//				Say.Debug("Added " + GetStringFromOEIString(newLine.Text) + " as the new line, child of filler line.");
			}
			else {
				newLine = Conversation.CurrentConversation.NwnConv.InsertChild(parent); // again, parent may be null for ROOT

//					if (parent == null) {
//						Say.Debug("Added " + GetStringFromOEIString(newLine.Text) + " as a newline, child of root.");
//					}
//					else {
//						Say.Debug("Added " + GetStringFromOEIString(newLine.Text) + " as a newline, child of " + GetStringFromOEIString(parent.Text) + ".");
//					}
			}
			newLine.Speaker = speakerTag;
			newLine.Sound = null;
			
			if (reparentChildren && children.Count > 1) { // reparent children if we're just adding a line; don't if we're adding a branch
//				Say.Debug("Reparent the children.");
				ReparentChildren(newLine,fillerLine,children);
			}
			
			return newLine;
		}					
		
		/// <summary>
		/// Do not call directly.
		/// </summary>
		private void ReparentChildren(NWN2ConversationConnector newLine, NWN2ConversationConnector fillerLine, NWN2ConversationConnectorCollection children)
		{						
			NWN2ConversationConnectorCollection displacedChildren = new NWN2ConversationConnectorCollection();
			NWN2ConversationConnector newParent;
			NWN2ConversationConnector childToIgnore;
			
			if (fillerLine == null) { // create a fillerLine between newLine and newLine's children
				fillerLine = Conversation.CurrentConversation.NwnConv.InsertChild(newLine);
				SetAsFillerLine(fillerLine);
				newParent = fillerLine;
				childToIgnore = newLine; // edited - because surely fillerLine must be the child to ignore?
//				Say.Debug("Added a filler line between the new line and its children since one didn't exist. The filler line is their new parent.");
			}
			else { // a fillerLine has already been created, between newLine and newLine's parent
				newParent = newLine;
				childToIgnore = fillerLine;
//				Say.Debug("a fillerLine has already been created, between newLine and newLine's parent.");
			}
				
			foreach (NWN2ConversationConnector child in children) { // split over two foreach statements to avoid an enumeration exception
				if (child != childToIgnore) {
					displacedChildren.Add(child);
				}
			}
			foreach (NWN2ConversationConnector displacedChild in displacedChildren) {
				Conversation.CurrentConversation.NwnConv.ReparentNode(displacedChild,newParent.Line);
			}	
			
//			Say.Debug("Reparented children.");
		}
		
		#endregion
			
		#region Filler lines
						
		/// <summary>
		/// Check whether a given line is marked as a filler line.
		/// </summary>		
		/// <remarks>Invisible filler lines are necessary to overcome the NWN2 restriction that a player or NPC
		/// have to take turns to speak (rather than being able to say two lines in a row).</remarks>
		/// <param name="line">The line of dialogue to check</param>
		/// <returns>True if this line has been identified as a filler line; false otherwise.</returns>
		/// <exception cref="InvalidDataException">Thrown if a filler line illegally contains text, or if
		/// the identifier is invalid.</exception>
		public static bool IsFiller(NWN2ConversationConnector line)
		{
			if (line.Comment == Conversation.IDENTIFY_AS_FILLER) {
				string text = GetStringFromOEIString(line.Text);
				if (text.Length > 0) {
					throw new InvalidDataException("Line is marked as 'filler', but contains text - '" 
					                               + text + "' - and will therefore be displayed.");
				}
				else {
					return true;
				}
			}
			else if (line.Comment == null || line.Comment == String.Empty || line.Comment == Conversation.IDENTIFY_AS_NOT_FILLER) {
				return false;
			}
			else {
				throw new InvalidDataException(line.ToString() + " has an invalid comment field: " + line.Comment.ToString() + "." +
				                               "Comment field is reserved to identify lines as filler or non-filler.");
			}
		}
		
		
		/// <summary>
		/// Identify the given line as a filler line, so that it is invisible to the user.
		/// </summary>
		/// <remarks>Invisible filler lines are necessary to overcome the NWN2 restriction that a player or NPC
		/// have to take turns to speak (rather than being able to say two lines in a row).</remarks>
		/// <param name="line">The line of dialogue to set as a filler line</param>
		private void SetAsFillerLine(NWN2ConversationConnector line)
		{		
			if (line == null) {
				throw new ArgumentNullException("Can't operate on a null line.");
			}
			
//			Say.Debug("Initial value of line: " + GetStringFromOEIString(line.Text));
			line.Comment = IDENTIFY_AS_FILLER;
			line.Line.Text = GetOEIStringFromString(String.Empty); // don't use SetText as this will fire an OnChanged event, and we're not finished
//			Say.Debug("New value of line: " + GetStringFromOEIString(line.Text));
			
			if (line.Speaker != String.Empty) {
				line.Speaker = String.Empty;
			}
			if (line.Sound != null) {
				line.Sound = null;
			}
			if (line.Actions.Count > 0) {
				line.Actions.Clear();
			}
			if (line.Conditions.Count > 0) {
				line.Conditions.Clear();
			}
			// TODO - also set camera info
		}
				
		
		/// <summary>
		/// Identify the given line as a non-filler line, so that it is visible to the user. 
		/// </summary>
		/// <remarks>This is necessary to make the distinction between a blank line that the player has added
		/// (and thus wants to interact with) and a blank line that the software has added to deal with
		/// restrictions in conversation structure (which should be invisible).</remarks>
		/// <param name="line">The line of dialogue to set as a non-filler line</param>
		private void SetAsNotFillerLine(NWN2ConversationConnector line)
		{
			if (line == null) {
				throw new ArgumentNullException("Can't operate on a null line.");
			}
			
			line.Comment = IDENTIFY_AS_NOT_FILLER;
		}
		
		#endregion
		
		
		#endregion
	}
}
