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
 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.IO;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.Instances;
using NWN2Toolset.NWN2.Data.ConversationData;
using NWN2Toolset.NWN2.Data.TypedCollections;
using OEIShared.Utils;
using AdventureAuthor;
using AdventureAuthor.Core;
using AdventureAuthor.ConversationWriter;
using AdventureAuthor.UI.Windows;
using AdventureAuthor.UI.Controls;
using AdventureAuthor.Utils;
using System.Windows.Controls;
using Microsoft.Win32;
using form = NWN2Toolset.NWN2ToolsetMainForm;

namespace AdventureAuthor.ConversationWriter
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
		public static readonly SolidColorBrush ACTION_COLOUR = Brushes.Orange;
		public const string PLAYER_NAME = "Player";
		public const string NOT_FILLER = "notfiller";
		public const string FILLER = "filler";
		public const int MINIMUM_WIDTH_BETWEEN_CHILDREN = 25; // pixels
    	public const int HEIGHT_BETWEEN_LEVELS = 100;
    	public const int HEIGHT_FROM_TOP_OF_WINDOW = 50;
    	public const double SELECTED_NODE_OUTER_DIAMETER = 44.0;
    	public const double SELECTED_NODE_INNER_DIAMETER = 39.0;
    	public const double UNSELECTED_NODE_OUTER_DIAMETER = 40.0;
    	public const double UNSELECTED_NODE_INNER_DIAMETER = 35.0;		
		
		#endregion Constants
		
		#region Fields
		
		private static Conversation currentConversation;		
		public static Conversation CurrentConversation {
			get { return currentConversation; }
			set { currentConversation = value; }
		}
		private static object padlock = new object();
		
		private NWN2GameConversation nwnConv;		
		public NWN2GameConversation NwnConv {
			get { return nwnConv; }
		}
		
		private List<Speaker> speakers; // speakers in the conversation other than the player		
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
		
		
		//TODO: Currently no way to store this information below. Use NwnConv.Comments somehow?
		//TODO: If this gets reinstated, use the Chapter.ContainsObject, Chapter.ContainsSpeakingObject stuff etc. Otherwise use them when attaching convo to an instance.
		// Remember that this is the INSTANCE that owns this conversation (i.e. different instances of a Character will have different conversations!)
//		private INWN2Instance owner; // the creature/door/placeable that owns this conversation			
//		public INWN2Instance Owner {
//			get { return owner; }
//		}
//		
//		private Chapter owningChapter;		
//		public Chapter OwningChapter {
//			get { return owningChapter; }
//		}
				
		// TODO: How does class Speaker relate to class Character? Speaker owns a Character?
		
		private List<Brush> unassignedColours;
		
		#endregion Fields
		
		#region Constructors 
		
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
		
		public Speaker AddSpeaker(string tag)
		{
			return AddSpeaker(tag,tag); // use the tag as the display name if one isn't provided
		}
		
		public Speaker AddSpeaker(string tag, string displayName)
		{
			Speaker existingSpeaker = GetSpeaker(tag);
			if (existingSpeaker == null) {	
				Speaker speaker = new Speaker(tag,displayName);
				speakers.Add(speaker);
				AssignColour(speaker);
				return speaker;
			}
			else {
				Say.Error("Tried to add a speaker that already exists.");
				return existingSpeaker;
			}
		}
		
		public Speaker GetSpeaker(string tag)
		{
			foreach (Speaker speaker in speakers) {
				if (speaker.Tag.ToUpper() == tag.ToUpper()) {
					return speaker;
				}
			}
			return null;
		}
		
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
			SaveToWorkingCopy();
			return newLine;
		}

		internal NWN2ConversationConnector InsertFillerLineWithoutReparenting(NWN2ConversationConnector parentLine)
		{
			NWN2ConversationConnector fillerLine = Conversation.CurrentConversation.nwnConv.InsertChild(parentLine);
			fillerLine.Comment = Conversation.FILLER;
			fillerLine.Speaker = String.Empty;
			SaveToWorkingCopy();
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
				newLine = Conversation.CurrentConversation.NwnConv.InsertChild(fillerLine);						
			}
			else {
				newLine = Conversation.CurrentConversation.NwnConv.InsertChild(parent); // again, parent may be null for ROOT
			}
			newLine.Speaker = speakerTag;
			
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
			
			// Remove and refresh display, and return the children of the deleted line:
			NWN2ConversationConnectorCollection children = Conversation.CurrentConversation.nwnConv.RemoveNode(Nwn2Line);
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
			SaveToWorkingCopy();
			ConversationWriterWindow.Instance.RefreshDisplay(false);
		}	
		
		#endregion Editing the conversation		
		
		public NWN2ConversationConnector AddLineToBranch(NWN2ConversationConnector branchParent)
		{
			string speakerTag = null;
			foreach(NWN2ConversationConnector option in branchParent.Line.Children) {
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
		
		public static string GetDescriptionOfConditions(NWN2ConversationConnector line)
		{
			// TODO: Since we are completely controlling the creation of scripts, we should have our own method of describing them.
			// When constructing a script through a wizard (as should hopefully almost always be the case), it should be relatively easy
			// to generate a decent natural-language representation of the script.
			// When hand-coding a script, we can offer the opportunity to describe what it does in 50(?) characters or less, for
			// use in the descriptive bit of the Conversation Writer. If this isn't found or is unusable, just say ACTION: SPECIAL
			// or IF (SPECIAL) THEN DISPLAY THIS LINE:.
			
			return "IF something AND somethingelse THEN DISPLAY THIS LINE:";
			
//			#region Ramblings
//			string description = String.Empty;
//			string desc2 = String.Empty;
////			Say.Information("line.Actions\n\n***********");
//			foreach (NWN2ScriptFunctor sf in line.Actions) {
//				description += "ScriptFunctor.Parameters{";
//				foreach (NWN2ScriptParameter sp in sf.Parameters) {
//					description += "sp: " + sp.ParameterType.ToString();
//					switch (sp.ParameterType) {
//						case NWN2ScriptParameterType.Float:
//							description += sp.ValueFloat.ToString();
//							break;
//						case NWN2ScriptParameterType.Int:
//							description += sp.ValueInt.ToString();
//							break;
//						case NWN2ScriptParameterType.String:
//							description += sp.ValueString;
//							break;
//						case NWN2ScriptParameterType.Tag:
//							description += sp.ValueTag;
//							break;
//					}
//					description += ". ";
//					//sp.ParameterType - enum {float,int,string,tag}. 
//					//sp has Properties ValueFloat, ValueString, ValueInt, ValueTag and Value (an object).
//				}
//				// also: IResourceEntry sf.Script
////				Say.Information("Script.FullName: " + sf.Script.FullName + "\n\nScript.Repository: " + sf.Script.Repository.Name 
////				           + "\n\nScript.ResourceType: " + sf.Script.ResourceType.ToString() + "\n\nScript.ResRef: " + sf.Script.ResRef.Value);
//			}
//			
////			Say.Information("line.Conditions\n\n*************");
//			foreach (NWN2ConditionalFunctor cf in line.Conditions) {
//				
//				//cf.Condition = NWN2ConditionalType.And, NWN2ConditionalType.Or
//				//cf.Not = true, false
//				foreach (NWN2ScriptParameter sp in cf.Parameters) {					
//					desc2 += "sp: " + sp.ParameterType.ToString();
//					switch (sp.ParameterType) {
//						case NWN2ScriptParameterType.Float:
//							desc2 += sp.ValueFloat.ToString();
//							break;
//						case NWN2ScriptParameterType.Int:
//							desc2 += sp.ValueInt.ToString();
//							break;
//						case NWN2ScriptParameterType.String:
//							desc2 += sp.ValueString;
//							break;
//						case NWN2ScriptParameterType.Tag:
//							desc2 += sp.ValueTag;
//							break;
//					}
//					desc2 += ". ";
//					//sp.ParameterType - enum {float,int,string,tag}. 
//					//sp has Properties ValueFloat, ValueString, ValueInt, ValueTag and Value (an object).
//				}
//				// also: IResourceEntry sf.Script				
////				Say.Information("Script.FullName: " + cf.Script.FullName + "\n\nScript.Repository: " + cf.Script.Repository.Name 
////				           + "\n\nScript.ResourceType: " + cf.Script.ResourceType.ToString() + "\n\nScript.ResRef: " + cf.Script.ResRef.Value);
//				string x = String.Empty;
//				if (cf.Not) x+= "NOT ";
//				x += cf.Condition.ToString();
//			}
//			
//			#endregion Ramblings
		}
		
		public void SaveToOriginal()
		{
			if (this != CurrentConversation) {
				throw new InvalidOperationException("Tried to operate on a closed Conversation.");
			}
			
			lock (padlock) {
				// Changes to lines are only saved to the working copy when the control loses focus, so make sure you save changes to a currently selected line:
				if (ConversationWriterWindow.Instance.CurrentlySelectedControl != null) {
					LineControl lc = ConversationWriterWindow.Instance.CurrentlySelectedControl as LineControl;
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
			}
		}
		
		public static int WordCount(string toCount)
		{ 
			return toCount.Split(' ').Length;
		}

		public static DataFromConversation GetWordLinePageCounts(Conversation conversation, NWN2ConversationConnector parent)
		{			
			// Get the counts from the parent line before continuing:
			int words = 0;
			int lines = 0;
			int pages = 0;
			NWN2ConversationConnectorCollection children;
			
			if (parent == null) {
				children = conversation.nwnConv.StartingList;
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
		
		private static DataFromConversation GetWordLinePageCounts(NWN2ConversationConnectorCollection parents)
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
