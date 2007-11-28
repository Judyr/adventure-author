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
using System.Diagnostics;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Xml.Serialization;
using System.Reflection;
using AdventureAuthor.Conversations;
using AdventureAuthor.Setup;
using AdventureAuthor.Scripts;
using AdventureAuthor.Utils;
using NWN2Toolset;
using NWN2Toolset.Data;
using NWN2Toolset.NWN2.Data.Factions;
using NWN2Toolset.NWN2.Data.Journal;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.IO;
using NWN2Toolset.NWN2.Views;
using OEIShared.UI;
using OEIShared.Utils;
using form = NWN2Toolset.NWN2ToolsetMainForm;

namespace AdventureAuthor.Core
{
	/// <summary>
	/// Description of Adventure.
	/// </summary>	
	[Serializable]
	[XmlRoot]
	public static class ModuleHelper
	{		
		#region Constants
		
		public const int MAX_RESOURCE_NAME_LENGTH = 32; // hard-coded into toolset: do not change				
		public const int MIN_AREA_LENGTH = 8;  // equates to 4 red gridbox lengths on the map grid
		public const int MAX_AREA_LENGTH = 16; // equates to 8 red gridbox lengths on the map grid
		public const string NAME_OF_SCRATCHPAD_AREA = "Scratchpad";
		public static readonly Font ADVENTURE_AUTHOR_FONT = new Font("Arial",10.0F);
		
		#endregion Constants
			
		#region Global variables
			
		private static bool beQuiet = false;	
		private static bool debug = true;
		private static bool doBackups = true;	
				
		public static bool BeQuiet {
			get	{ return beQuiet; }
			set	{ beQuiet = value; } 
		}
		
		public static bool Debug {
			get	{ return debug; }
			set	{ debug = value; } 
		}
		
		public static bool DoBackups {
			get { return doBackups; }
			set { doBackups = value; }
		}
		
		public static string AdventureAuthorDir {			
			get {
				return Path.Combine(System.Environment.CurrentDirectory,"AdventureAuthor");
			}
		}	
		
		public static string BackupDir {			
			get {
				return Path.Combine(AdventureAuthorDir,"backups");
			}
		}	
		
		public static string DebugDir {			
			get {
				return Path.Combine(AdventureAuthorDir,"debugs");
			}
		}	
				
		public static string ImagesDir {			
			get {
				return Path.Combine(AdventureAuthorDir,"images");
			}
		}	
		
		public static string LogDir {			
			get {
				if (!Directory.Exists(@"C:\adventureauthorlogs")) {
				    	
				}
				
				return @"C:\adventureauthorlogs";
				
				
//				try {
//				StreamReader sr = new StreamReader(System.IO.File.Open(Path.Combine(System.Environment.CurrentDirectory,"logdirectory.txt"), FileMode.Open));
//				string logdir = sr.ReadLine();
//				if (logdir != null && logdir != String.Empty) {
//					return Path.Combine(logdir,"");
//				}
//				else {
//					Say.Error("No destination found for log files - in the file logdirectory.txt in the main NWN2 folder, " + 
//					          "please type in a valid path e.g. C:\adventureauthorlogs");
//					return Path.Combine(AdventureAuthorDir,"logs");
//				}
//				//return Path.Combine(AdventureAuthorDir,"logs");
//				}
//				catch (FileNotFoundException e) {
//					Say.Error("Could not find log directory.",e);
//					return Path.Combine(AdventureAuthorDir,"logs");
//				}
			}
		}	
				
		
		#endregion Global variables
					
		#region Events
		
		public static event EventHandler<EventArgs> ModuleChanged;
		
		internal static void OnModuleChanged(EventArgs e)
		{
			EventHandler<EventArgs> handler = ModuleChanged;
			if (handler != null) {
				handler(null,e);
			}
		}		
		
		#endregion
		
		internal static void LogChanges()
		{
			// this seems to get called 6 times every time you set the start location (??):
			form.App.Module.ModuleInfo.EntryPointChanged += delegate
			{ 
				Log.WriteAction(Log.Action.set,"startlocation"); 
			};
			form.App.Module.ModuleExpanded += delegate 
			{
				Log.WriteMessage("module was 'expanded' (??)");
			};
			form.App.Module.NameChanged += delegate(object sender, NameChangedEventArgs e) 
			{  
				if (e.OldName != e.NewName) { // ignore if this has been raised on a save
					Log.WriteAction(Log.Action.renamed,"module","'" + e.NewName + "' from '" + e.OldName + "'");
				}
			};
			
			form.App.Module.Areas.Inserted += delegate(OEIDictionaryWithEvents cDictionary, object key, object value) 
			{  
				Log.WriteAction(Log.Action.added,"area",key.ToString());
			};
			form.App.Module.Areas.Removed += delegate(OEIDictionaryWithEvents cDictionary, object key, object value) 
			{  
				Log.WriteAction(Log.Action.deleted,"area",key.ToString());
			};
				
			form.App.Module.Scripts.Inserted += delegate(OEIDictionaryWithEvents cDictionary, object key, object value) 
			{  
				Log.WriteAction(Log.Action.added,"script",key.ToString());
			};
			form.App.Module.Scripts.Removed += delegate(OEIDictionaryWithEvents cDictionary, object key, object value) 
			{  
				Log.WriteAction(Log.Action.deleted,"script",key.ToString());
			};
			
			form.App.Module.FactionData.Factions.Inserted += delegate(OEICollectionWithEvents cList, int index, object value)
			{  
				Log.WriteAction(Log.Action.added,"faction",((NWN2Faction)value).Name);
			};				
			form.App.Module.FactionData.Factions.Removed += delegate(OEICollectionWithEvents cList, int index, object value)
			{  
				Log.WriteAction(Log.Action.deleted,"faction",((NWN2Faction)value).Name);
			};	
			
			form.App.Module.Journal.Categories.Inserted += delegate(OEICollectionWithEvents cList, int index, object value)
			{  
				Log.WriteAction(Log.Action.added,"journalcategory");
			};				
			form.App.Module.Journal.Categories.Removed += delegate(OEICollectionWithEvents cList, int index, object value)
			{  
				Log.WriteAction(Log.Action.deleted,"journalcategory");
			};	
		}	
		
				
		public static NWN2GameModule CreateAndOpenModule(string name)
		{			
			Log.WriteAction(Log.Action.added,"module",name);
			
			// Create the module object:
			NWN2GameModule mod = new NWN2GameModule();
			mod.Name = name;
			mod.LocationType = ModuleLocationType.Directory;
			mod.FileName = Path.Combine(form.ModulesDirectory,mod.Name);	
			mod.ModuleInfo.XPScale = 0; // player will gain no experience
			
			// Apply default scripts to the module for automatically logging certain actions in-game:
			try {
				ScriptHelper.ApplyDefaultScripts(mod);
			}
			catch (IOException e) {
				Say.Error("Could not find Adventure Author logging scripts to assign to this resource.",e);
			}		
			
			// Write the module to disk and open it:
			Serialize(mod);
			Open(name);
			
			// Create a scratchpad area and open it:
			AreaHelper.CreateArea(NAME_OF_SCRATCHPAD_AREA,true,12,12);
			AreaHelper.Open(NAME_OF_SCRATCHPAD_AREA);

			return mod;
		}	
		
		
		private static void Serialize(NWN2GameModule module)
		{
			module.OEISerialize(module.Name);
//			foreach (NWN2GameArea area in module.Areas) {
//				area.OEISerialize(area.Name);
//			}
		}
		
						
		
		/// <summary>
		/// Open a module in the toolset.
		/// </summary>
		/// <param name="name">Name of the module to open.</param>
		public static bool Open(string name)
		{
			Log.WriteAction(Log.Action.opened,"module",name);
			
			if (ModuleIsOpen()) {
				CloseModule();
			}
			
			try {
				Deserialize(name);				
				foreach (NWN2GameArea area in form.App.Module.Areas.Values) {
					AreaHelper.ApplyLogging(area);
				}
				
				try {
					AreaHelper.Open(NAME_OF_SCRATCHPAD_AREA);
				}
				catch (FileNotFoundException) { 
					Say.Debug("Tried to open " + NAME_OF_SCRATCHPAD_AREA  + " in module '" + name + "', but there was no such area.");
				}
				
				OnModuleChanged(new EventArgs());
				return true;
			}
			catch (DirectoryNotFoundException e) {
				Say.Error("Failed to open module.",e);
				CloseModule();
				return false;
			}
		}			
		
		
		public static void Save()
		{
			if (form.App.Module == null) {
				return;
			}
			if (form.App.Module.LocationType != ModuleLocationType.Directory) {
				Say.Error("Can't save a module that is not stored as a directory."); // shouldn't happen
			}
					
			Log.WriteAction(Log.Action.saved,"module");
						
		    form.App.WaitForPanelsToSave();  	        
		    if (form.VersionControlManager.OnModuleSaving()) {
		      	ThreadedSaveHelper save = new ThreadedSaveHelper(form.App,
		       	                                                 form.App.Module.FileName,
		       	                                                 ModuleLocationType.Directory);
		       	ThreadedProgressDialog progress = new ThreadedProgressDialog();
		       	progress.Text = "Saving";
		       	progress.Message = "Saving '" + form.App.Module.FileName + "'";
		       	progress.WorkerThread = new ThreadedProgressDialog.WorkerThreadDelegate(save.Go);
		       	progress.ShowDialog(form.App);
		    }
		        
		    OnModuleChanged(new EventArgs());
		}
					
		
		public static void SaveAs(string newName)
		{	 	
			try {
				throw new NotImplementedException();
			}
			catch (NotImplementedException) {
				Say.Error("Not implemented yet.");
			}
			
			// TODO: Implement
			// ..... Simply replacing module.Filename with newName in the two places it's used in Save
			// doesn't work, as the existing module is now saved as newName, with nothing under the original
			// name of module.Filename. 
			// If we clone the existing module to the new location and then open it, that would work,
			// except that 
			
			
			// Clone the existing module to a new location - this will
		}
				
		
		/// <summary>
		/// Create a date-stamped backup copy of this Adventure.
		/// </summary>
		/// <param name="reason">The reason that the backup was created.</param>
		public static void Backup(string reason)
		{
			string backupPath = GetDirectoryPathForBackup();
			string originalPath = Path.Combine(form.ModulesDirectory,form.App.Module.FileName);
			
			try {
				// Copy the module files to the backup directory:
				DirectoryInfo di = new DirectoryInfo(backupPath);
				di.Create();
				CopyFolders(originalPath,backupPath);	
								
				// If given a justification for the backup, write it to a file in the backup directory:
				if (reason != string.Empty) {
					FileInfo reasonForBackup = new FileInfo(Path.Combine(backupPath,"Reason for backup.txt"));
					StreamWriter sw = new StreamWriter(reasonForBackup.OpenWrite());					
					sw.WriteLine("Reason for backup:  " + reason);
					sw.WriteLine();					
					sw.WriteLine("Created " + UsefulTools.GetDateStamp());	
					sw.Flush();
					sw.Close();
				}
			}
			catch (IOException e) {
				Say.Error("Problem with writing to disk when trying to back up a module.",e);
			}
		}
		
		
		private static string GetDirectoryPathForBackup()
		{
			string path = Path.Combine(ModuleHelper.BackupDir,form.App.Module.Name+"_"+UsefulTools.GetDateStamp()+"___");			
			int count = 1;	
			string newpath = path;
			while (Directory.Exists(newpath)) { // keep trying directory names until one is not taken
				count++;
				newpath = path + count.ToString();
			}			
			return newpath;
		}		
		
		
		private static void CopyFolders(string source, string destination)
		{
	    	DirectoryInfo di = new DirectoryInfo(source);
	    	CopyFiles(source, destination);
	    	foreach (DirectoryInfo d in di.GetDirectories()) {
	       		string newDir = Path.Combine(destination, d.Name);
	       		Directory.CreateDirectory(newDir);
	       		CopyFolders(d.FullName, newDir);
	   		}
		}
		
		
		private static void CopyFiles(string source, string destination)
		{
	    	DirectoryInfo di = new DirectoryInfo(source);
	    	FileInfo[] files = di.GetFiles();
		    foreach (FileInfo f in files) {
	       		string sourceFile = f.FullName;
	       		string destFile = Path.Combine(destination, f.Name);
	       		File.Copy(sourceFile, destFile);
	    	}
		}	
						
		
		/// <summary>
		/// Used to retrieve a module from disk.
		/// </summary>
		private static void Deserialize(string name)
		{									
			// Deserialize the module data and open the module in the toolset:
			ThreadedOpenHelper toh = null;
			try {
				toh = new ThreadedOpenHelper(form.App,name,ModuleLocationType.Directory);
				toh.Go();
				form.App.SetupHandlersForGameResourceContainer(form.App.Module);
			}
			catch (DirectoryNotFoundException e) {
				throw new DirectoryNotFoundException("Directory-based module '" + name + "' could not be found.",e);
			}
			finally {
				toh = null;
			}		
		}					
		
		
		/// <summary>
		/// Close the currently open module.
		/// </summary>
		public static void Close()
		{
			if (form.App.Module == null) {
				Say.Warning("No module was open to be closed.");
				return;
			}
			
			Log.WriteAction(Log.Action.closed,"module",form.App.Module.Name);
			
			CloseModule();
			OnModuleChanged(new EventArgs());
		}
		
		
		/// <summary>
		/// Close the currently open module (based on reflected code). Preferentially call Adventure.Close() instead.
		/// </summary>
		private static void CloseModule()
		{
			if (NWN2ToolsetMainForm.VersionControlManager.OnModuleClosing()) {				
				OEIShared.Actions.ActionManager.Manager.Clear(); // ??	
				form.App.Module.CloseModule();            
		        form.VersionControlManager.OnModuleClosed();            
		        form.App.ClearHandlersForGameResourceContainer(form.App.Module);	                  
		        if (form.App.Module != null) {
		        	form.App.Module.Dispose();
		        }	
		        form.App.DoNewModule(true);	
			}
		}
				
		
		/// <summary>
		/// Run the currently open Adventure.
		/// </summary>
		/// <param name="waypoint">The tag of the waypoint to run from - if empty, run from the player start.</param>
		/// <param name="debugOn">If true, the debug window is accessible during gameplay. Currently does nothing.</param>
		/// <param name="playerIsInvincible">If true, the player cannot be harmed. Currently does nothing.</param>
		public static void Run(string waypoint, bool debugOn, bool playerIsInvincible)
		{
			if (waypoint != string.Empty) {				
				//TODO:check waypoint is good, including exists, valid name, and isn't reused in another area
			}
			else {
				//TODO: check that player start location is good? is this done for us anyway?
			}
			
			// Apply options:
			if (debugOn) {
				// TODO: Make the debug window accessible in-game
			}
			if (playerIsInvincible) {
				// TODO: Add 'SetImmortal(GetFirstPC(),TRUE)' to the module's
				// OnEnter script (or the other script which adds user-defined
				// bits to the OnEnter script automatically)
			}		
						
			RunNWN2();
		}
		
		
		/// <summary>
		/// Create a copy of the log from the previous game session, and then run Neverwinter Nights 2.
		/// </summary>
		private static void RunNWN2()
		{
			// create a copy of the log from the last game (otherwise it will be over-written):
			ProcessStartInfo copylogStartInfo = new ProcessStartInfo(Path.Combine(System.Environment.CurrentDirectory,"copylog.bat"));
			Process.Start(copylogStartInfo);
			
			// start the game:
			Log.WriteAction(Log.Action.launched,"game");
			form.App.RunModule(String.Empty,false,false,false);
			
//			ProcessStartInfo nwn2mainStartInfo = new ProcessStartInfo(Path.Combine(System.Environment.CurrentDirectory,"nwn2main.exe"));
//			nwn2mainStartInfo.RedirectStandardOutput = true;
//			nwn2mainStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
//			nwn2mainStartInfo.UseShellExecute = false;
//			Process nwn2main = new Process();
//			nwn2main.StartInfo = nwn2mainStartInfo;
//			nwn2main.Disposed += delegate { Log.WriteAction(Log.Action.exited,"game",CurrentAdventure.Name); }; doesn't seem to work
//			nwn2main.Start();
		}
		
		
		public static void Bake()
		{			
			Log.WriteMessage("baking module");
			form.App.DoBakeAll(false,false);
			Log.WriteMessage("baked module");
		}
		
							
		//TODO: IMPLEMENT
		/// <summary>
		/// Delete a resource i.e. an area, script or conversation
		/// </summary>
		/// <param name="resource">The resource to delete</param>
		public static void DeleteResource(object resource)
		{		
			throw new NotImplementedException();
			
			// TODO: if resource is an area then return, should use DeleteChapter
			
			
//			NWN2GameArea area = resource as NWN2GameArea;			
//			if (area != null) {
//				//DeleteArea(area);
//				return;
//			}
//			
//			NWN2GameModule mod;
//			string name;
//			string filename;			
//						
//			NWN2GameConversation conversation = (NWN2GameConversation)resource;			
//			NWN2GameScript script = (NWN2GameScript)resource;
//			
//			if (conversation != null) {
//				mod = (NWN2GameModule)conversation.Module;
//				name = conversation.Name;
//				filename = conversation.Resource.FullName;
//			}			
//			else if (script != null) {
//				mod = (NWN2GameModule)script.Module;
//				name = script.Name;
//				filename = script.Resource.FullName;
//			}
//			else {
//				//throw InvalidResource exception
//				return;
//			}
//			
//			if (NWN2ToolsetMainForm.App.Module.LocationType == NWN2Toolset.NWN2.IO.ModuleLocationType.Temporary) {
//				MessageBox.Show("Working on a temporary module; nothing to delete.");
//				return;
//			}
//			if (NWN2ToolsetMainForm.App.Module.LocationType == NWN2Toolset.NWN2.IO.ModuleLocationType.File)	{
//				MessageBox.Show("Can't delete from a file - sorry. Modules should be in the form of directories!");
//				return;
//			}
//			
//			try	{
//				// Check that we really want to delete the resource, exit if we don't:
//				if (MessageBox.Show("Delete " + name + "?",
//			             			  "Delete?", 
//			                 		  MessageBoxButtons.YesNo, 
//			                 	     MessageBoxIcon.Question, 
//			                	     MessageBoxDefaultButton.Button2) == DialogResult.No) { return; }
//										
//				if (Adventure.DoBackups) // move the resource file to backup directory before removing the resources
//				{					
//					string sourcePath = Path.Combine(Path.Combine(NWN2ToolsetMainForm.ModulesDirectory,mod.FileName),filename);
//					
//					//TODO: Don't just backup the file: run Settings.CurrentAdventure.Backup()
//					//System.IO.Directory.Move(sourcePath,AdventureAuthorArea.GetBackupFilename(filename));
//				}
//				
//				mod.RemoveResource(resource); // removes resource from the module's resources, and deletes the file if it can be found
//								
//				INWN2Viewer viewer = NWN2ToolsetMainForm.App.GetViewerForResource(resource);
//				if (viewer != null)	{
//					viewer.Close(false);
//				}
//			}			
//			catch (System.Security.SecurityException se) {
//				MessageBox.Show("The caller does not have the required directory permission.\n\n"+se.ToString());
//			}
//			catch (Exception e)	{
//				MessageBox.Show(e.ToString());
//			}	
		}
			
		
		
		public static bool DeleteArea(NWN2GameArea area)
		{
			if (form.App.Module == null) {
				throw new InvalidOperationException("Tried to operate on a closed Adventure.");
			}
			
			// If the area is currently open, close it silently:
			AreaHelper.Close(area);
			
			// If requested, backup the entire module:
			if (ModuleHelper.DoBackups) {
				Backup("Deleted area '" + area.Name + "'.");
			}
			
			// Delete the area:
			form.App.Module.RemoveResource(area);
			string path = Path.Combine(form.ModulesDirectory,form.App.Module.FileName);
			string[] areaFiles = Directory.GetFiles(path,area.Name+"*");
			foreach (string filename in areaFiles) {
				Directory.Delete(filename);
			}
					
			return true;
		}		
		
				
		/// <summary>
		/// Checks that a given resource or module name is valid
		/// </summary>
		/// <param name="name">The name to check for validity</param>
		/// <returns>Returns false if too short, too long or contains invalid characters, true otherwise</returns>						
		public static bool IsValidName(string name)
		{
			if (name.Length < 1 || name.Length > 32) {
				return false;
			}
			
			string[] invalidChars = {"<" , ">" , ":" , "\\" , "\"" , "/" , "|", "."};
			
			for (int i = 0; i < invalidChars.Length; i++) {
				if (name.Contains(invalidChars[i]))	{
					return false;
				}
			}
			
			return true;
		}
		
		/// <summary>
		/// Checks if an adventure, conversation or scripts with the given name already exists
		/// </summary>
		/// <param name="type">The type of resource to check - adventure, conversation or script</param>
		/// <param name="name">The name to check for</param>
		/// <exception cref="IOException">Thrown if AdventureAuthor data is found without a module, or vice versa.</exception>
		/// <returns>Returns false if a resource of the given type already exists with this name, true otherwise</returns>
		public static bool IsAvailableModuleName(string name)
		{
			string modulePath = Path.Combine(form.ModulesDirectory,name);
			if (Directory.Exists(modulePath)) {
				return false;
			}
			else {
				return true;
			}
		}
		
		
		public static bool ModuleIsOpen()
		{
			if (form.App.Module != null) { 
				switch (form.App.Module.LocationType) {
					case ModuleLocationType.Directory:
						return true;
					case ModuleLocationType.Temporary:
						return false;
					case ModuleLocationType.File:
						throw new InvalidDataException("Module is stored as a file.");
				}
				return true;
			}
			else {
				return false;
			}
		}
	}
}
