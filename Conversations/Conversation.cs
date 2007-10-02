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
using System.IO;
using System.Windows;
using System.Windows.Media;
using AdventureAuthor.Conversations.UI;
using AdventureAuthor.Conversations.UI.Controls;
using AdventureAuthor.Core;
using AdventureAuthor.Utils;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.ConversationData;
using OEIShared.Utils;

namespace AdventureAuthor.Conversations
{
	/// <summary>
	/// Conversation wraps a NWN2GameConversation with some additional information for the Adventure Author Conversation Writer.
	/// </summary>
	public class Conversation : DependencyObject
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
		public const string NOT_FILLER = "notfiller";
		public const string FILLER = "filler";
		
		#endregion Constants
		
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
		
		private bool isDirty;		
		public bool IsDirty {
			get { return isDirty; }
		}		
		
//		private DependencyProperty isDirtyProperty = DependencyProperty.Register("IsDirty",typeof(bool),typeof(Conversation));
//		public bool IsDirty {
//			get { return (bool)GetValue(isDirtyProperty); }
//			private set { SetValue(isDirtyProperty,value); }			
//		}
		
		/// <summary>
		/// Colours to assign to speakers as they are added.
		/// </summary>
		private List<Brush> unassignedColours;
		
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
		}
		
		#endregion Constructors
		
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
				Speaker speaker = new Speaker(tag);
				speakers.Add(speaker);
				AssignColour(speaker);
				ConversationWriterWindow.Instance.CreateButtonForSpeaker(speaker);
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
		
		#region Editing the conversation

		internal NWN2ConversationConnector InsertNewLineWithoutReparenting(NWN2ConversationConnector parentLine, string speakerTag)
		{
			NWN2ConversationConnector newLine = Conversation.CurrentConversation.nwnConv.InsertChild(parentLine);
			newLine.Comment = Conversation.NOT_FILLER;
			newLine.Speaker = speakerTag;
			newLine.Sound = null;
			return newLine;
		}

		internal NWN2ConversationConnector InsertFillerLineWithoutReparenting(NWN2ConversationConnector parentLine)
		{
			NWN2ConversationConnector fillerLine = Conversation.CurrentConversation.nwnConv.InsertChild(parentLine);
			fillerLine.Comment = Conversation.FILLER;
			fillerLine.Speaker = String.Empty;
			fillerLine.Sound = null;
			return fillerLine;
		}
		
		public static bool CanFollow(NWN2ConversationConnector line, NWN2ConversationConnectorType typeOfFollowingLine)
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
					
		public NWN2ConversationConnector AddLine(NWN2ConversationConnector parent, string speakerTag, bool reparentChildren)
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
					newLine = AddLine(parent,speakerTag,true,reparentChildren); // adding PC line to root is only possible by adding a filler line first
				}
				else {
					newLine = AddLine(parent,speakerTag,false,reparentChildren); // can add an NPC line directly to root without adding a filler line first
				}
			}
			else { // adding to an existing line	
				if (!CanFollow(parent,newLineType)) { // if the new line can't be directly added, put in a filler line first
					newLine = AddLine(parent,speakerTag,true,reparentChildren);
				}
				else { // add the new line to the parent	
					newLine = AddLine(parent,speakerTag,false,reparentChildren); 
				}
			}
			
			newLine.Comment = Conversation.NOT_FILLER;	
			SaveToWorkingCopy();
			return newLine;
		}			
		
		private NWN2ConversationConnector AddLine(NWN2ConversationConnector parent, string speakerTag, bool addFillerBeforeNewLine, bool reparentChildren)
		{
			NWN2ConversationConnector newLine = null;
			NWN2ConversationConnector fillerLine = null;
			NWN2ConversationConnectorCollection children;
			if (parent == null) { // root
				children = Conversation.CurrentConversation.NwnConv.StartingList;
			}
			else {
				children = parent.Line.Children;
			}
		
			if (addFillerBeforeNewLine) {
				fillerLine = Conversation.CurrentConversation.NwnConv.InsertChild(parent);
				fillerLine.Comment = Conversation.FILLER;
				fillerLine.Sound = null;
				newLine = Conversation.CurrentConversation.NwnConv.InsertChild(fillerLine);						
			}
			else {
				newLine = Conversation.CurrentConversation.NwnConv.InsertChild(parent); // again, parent may be null for ROOT
			}
			newLine.Speaker = speakerTag;
			newLine.Sound = null;
			
			if (reparentChildren && children.Count > 1) { // reparent children if we're just adding a line; don't if we're adding a branch
				ReparentChildren(newLine,fillerLine,children);
			}
			
			return newLine;
		}						
		
		private void ReparentChildren(NWN2ConversationConnector newLine, NWN2ConversationConnector fillerLine, NWN2ConversationConnectorCollection children)
		{						
			NWN2ConversationConnectorCollection displacedChildren = new NWN2ConversationConnectorCollection();
			NWN2ConversationConnector newParent;
			NWN2ConversationConnector childToIgnore;
			
			if (fillerLine == null) { // create a fillerLine between newLine and newLine's children
				fillerLine = Conversation.CurrentConversation.NwnConv.InsertChild(newLine);
				fillerLine.Comment = Conversation.FILLER;
				fillerLine.Sound = null;
				newParent = fillerLine;
				childToIgnore = newLine;
			}
			else { // a fillerLine has already been created, between newLine and newLine's parent
				newParent = newLine;
				childToIgnore = fillerLine;
			}
				
			foreach (NWN2ConversationConnector child in children) { // split over two foreach statements to avoid an enumeration exception
				if (child != childToIgnore) {
					displacedChildren.Add(child);
				}
			}
			foreach (NWN2ConversationConnector displacedChild in displacedChildren) {
				Conversation.CurrentConversation.NwnConv.ReparentNode(displacedChild,newParent.Line);
			}	
		}
		
		public bool Contains(NWN2ConversationConnector line)
		{
			return this.nwnConv.Entries.Contains(line.Line) || this.nwnConv.Replies.Contains(line.Line) || this.nwnConv.StartingList.Contains(line);
		}
		
		public NWN2ConversationConnectorCollection DeleteLineFromBranch(NWN2ConversationConnector Nwn2Line)
		{			
			// Checks:
			if (!(this.Contains(Nwn2Line))) {
				return null;
			}
			else {
				if (Nwn2Line.Parent == null) {
					if (nwnConv.StartingList.Count < 2) {
						throw new InvalidOperationException("Tried to delete a line from a branch that had less than 2 options.");
					}
				}
				else if (Nwn2Line.Parent.Line.Children.Count < 2) {
					throw new InvalidOperationException("Tried to delete a line from a branch that had less than 2 options.");
				}
			}
			
			NWN2ConversationConnectorCollection children = Conversation.CurrentConversation.nwnConv.RemoveNode(Nwn2Line);
			
			// If there is only one line left in the branch (i.e. the branch is removed), clear any conditions from the remaining line:
			if (Nwn2Line.Parent == null) {
        		if (NwnConv.StartingList.Count == 1) {
					NwnConv.StartingList[0].Conditions.Clear();
        		}
        	}
        	else if (Nwn2Line.Parent.Line.Children.Count == 1){
				Nwn2Line.Parent.Line.Children[0].Conditions.Clear();
        	} 
			
			ConversationWriterWindow.Instance.RemoveLineControl(Nwn2Line);
			ConversationWriterWindow.Instance.RefreshDisplay(true);
			SaveToWorkingCopy();
			return children;
		}
		
		public void DeleteLine(NWN2ConversationConnector Nwn2Line)
		{
			if (Nwn2Line.Parent == null) { // root (line is therefore NPC starting entry)
				if (Nwn2Line.Line.Children.Count == 0) { // if the line has no children, just delete it:
					Conversation.CurrentConversation.NwnConv.RemoveNode(Nwn2Line);
				}
				else { // otherwise, if the line has children, make it a filler line, since root must always start with an NPC starting entry:
					Nwn2Line.Text = StringToOEIExoLocString(String.Empty);
					Nwn2Line.Comment = Conversation.FILLER;						
				}				
			}
			else {
				if (Nwn2Line.Line.Children.Count == 0) { // if the line has no children, just delete it:
					Conversation.CurrentConversation.NwnConv.RemoveNode(Nwn2Line);
				}
				else if (Nwn2Line.Line.Children.Count > 0) { // if the line has children:
					NWN2ConversationConnector child = Nwn2Line.Line.Children[0];
					if (IsFiller(child)) { // if the child is a filler line, delete both the line and the child:		
						if (Nwn2Line.Line.Children.Count > 1) {
							throw new InvalidDataException("A filler line formed part of a choice/check.");
						}
						else {
							NWN2ConversationConnectorCollection linesToBeReparented = new NWN2ConversationConnectorCollection();
							foreach (NWN2ConversationConnector lineToBeReparented in child.Line.Children) {
								linesToBeReparented.Add(lineToBeReparented);
							}
							foreach(NWN2ConversationConnector lineToBeReparented in linesToBeReparented) {						
								Conversation.CurrentConversation.NwnConv.ReparentNode(lineToBeReparented,Nwn2Line.Parent.Line);
							}	
							Conversation.CurrentConversation.NwnConv.RemoveNode(child);
							Conversation.CurrentConversation.NwnConv.RemoveNode(Nwn2Line);
							// NB: Even if both line.Parent and the line's child are filler lines, we always choose to delete the child since
							// the Parent may be a filler starting entry (by leaving it there, we can allow conversations to start with the PC.)
						}
					}
					else { // if there is more than one child, or the single child is not a filler line:
						if (IsFiller(Nwn2Line.Parent)) { // if the single child is a real line and the parent is a filler line, give the child to line.Parent.Parent:
							NWN2ConversationLine newParent;
							if (Nwn2Line.Parent.Parent == null) {
								newParent = null; // special case if the grandparent is root
							}
							else {
								newParent = Nwn2Line.Parent.Parent.Line;
							}							
							
							NWN2ConversationConnectorCollection linesToBeReparented = new NWN2ConversationConnectorCollection();
							foreach (NWN2ConversationConnector lineToBeReparented in Nwn2Line.Line.Children) {
								linesToBeReparented.Add(lineToBeReparented);
							}
							foreach(NWN2ConversationConnector lineToBeReparented in linesToBeReparented) {						
								Conversation.CurrentConversation.NwnConv.ReparentNode(lineToBeReparented,newParent);
							}	
														
							Conversation.CurrentConversation.NwnConv.RemoveNode(Nwn2Line);						
							Conversation.CurrentConversation.NwnConv.RemoveNode(Nwn2Line.Parent);		 // delete the line and its parent
						}
						else { // if the child(ren) and parent are real lines, make line a filler line:
							Nwn2Line.Text = StringToOEIExoLocString(String.Empty);
							Nwn2Line.Comment = Conversation.FILLER;	
						}						
					}
				}
			}
			
			// Refresh display:
			ConversationWriterWindow.Instance.RemoveLineControl(Nwn2Line);
			SaveToWorkingCopy(); // still needed ? TODO check
			ConversationWriterWindow.Instance.RefreshDisplay(false);
		}	
		
		#endregion Editing the conversation		
		
		public NWN2ConversationConnector AddLineToBranch(NWN2ConversationConnector branchParent)
		{
			string speakerTag = null;
			NWN2ConversationConnectorCollection children;
			
			if (branchParent == null) { // add line to root
				children = nwnConv.StartingList;
			}
			else {
				children = branchParent.Line.Children;
			}
			
			foreach(NWN2ConversationConnector option in children) {
				if (speakerTag == null) {
					speakerTag = option.Speaker;
				}
				else if (speakerTag != option.Speaker) {
					throw new ArgumentException("Found different speakers in the same branch.");
				}
			}
			return AddLine(branchParent,speakerTag,false);
		}
						
		public static bool IsFiller(NWN2ConversationConnector line)
		{
			if (line.Comment == Conversation.FILLER) {
				if (line.Text.Strings.Count > 0 && line.Text.Strings[0].Value.Length > 0) {
					throw new InvalidDataException("Line is marked as 'filler', but contains text and will therefore be displayed.");
				}
				else {
					return true;
				}
			}
			else {
				return false;
			}
		}
				
		public static OEIExoLocString StringToOEIExoLocString(string text)
		{
			OEIExoLocString str = new OEIExoLocString();
			OEIExoLocSubString substr = new OEIExoLocSubString();
			substr.Value = text;
			str.Strings.Add(substr);
			return str;
		}
		
		public static string OEIExoLocStringToString(OEIExoLocString text)
		{
			if (text.Strings.Count == 0) {
				return String.Empty;
			}
			else {
				return text.Strings[0].Value;
			}
		}
		
		public void SaveToOriginal()
		{
			if (this != CurrentConversation) {
				throw new InvalidOperationException("Tried to operate on a closed Conversation.");
			}
			
			lock (padlock) {
				// Changes to lines are only saved to the working copy when the control loses focus, so make sure you save changes to a currently selected line:
				if (ConversationWriterWindow.Instance.CurrentControl != null) {
					LineControl lc = ConversationWriterWindow.Instance.CurrentControl as LineControl;
					if (lc != null) {
						lc.Nwn2Line.Line.Text = StringToOEIExoLocString(lc.Dialogue.Text);
					}
				}
				
				NwnConv.OEISerialize(false);
				string originalPath = Path.Combine(Adventure.CurrentAdventure.Module.Repository.DirectoryName,ConversationWriterWindow.Instance.OriginalFilename+".dlg");
				string workingPath = Path.Combine(Adventure.CurrentAdventure.Module.Repository.DirectoryName,ConversationWriterWindow.Instance.WorkingFilename+".dlg");
				File.Copy(workingPath,originalPath,true);
				isDirty = false;
			}
		}
				
		public void SaveToWorkingCopy() 
		{
			if (this != CurrentConversation) {
				throw new InvalidOperationException("Tried to operate on a closed Conversation.");
			}
			
			lock (padlock) {
				NwnConv.OEISerialize(false);
				isDirty = true;
				Say.Debug("Saved to working copy.");
			}
		}
		
		internal void Serialize()
		{
			lock (padlock) {
				NwnConv.OEISerialize(false);
			}
		}
		
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
			NWN2ConversationConnectorCollection children;
			
			if (parent == null) {
				children = nwnConv.StartingList;
				pages += children.Count; // if we have reached a branch point, increment the number of pages
			}
			else {
				children = parent.Line.Children;				
				string text = OEIExoLocStringToString(parent.Line.Text);
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
		
		public static DataFromConversation GetWordLinePageCounts(NWN2ConversationConnectorCollection parents)
		{			
			int words = 0;
			int lines = 0;
			int pages = 0;
			
			foreach (NWN2ConversationConnector parent in parents) {
				string text = OEIExoLocStringToString(parent.Line.Text);
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
	}
}
