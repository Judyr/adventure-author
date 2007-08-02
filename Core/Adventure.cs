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
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using AdventureAuthor.UI;
using AdventureAuthor.Utils;
using NWN2Toolset;
using NWN2Toolset.Data;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.IO;
using NWN2Toolset.NWN2.UI;
using NWN2Toolset.NWN2.Views;
using OEIShared.UI;
using form = NWN2Toolset.NWN2ToolsetMainForm;

namespace AdventureAuthor.Core
{
	/// <summary>
	/// Description of Adventure.
	/// </summary>	
	[Serializable]
	[XmlRoot]
	public class Adventure
	{		
		#region Constants
		
		public const int MAX_RESOURCE_NAME_LENGTH = 32; // hard-coded into toolset: do not change				
		public const int MIN_AREA_LENGTH = 8;  // equates to 4 red gridbox lengths on the map grid
		public const int MAX_AREA_LENGTH = 16; // equates to 8 red gridbox lengths on the map grid
		public const int MAX_NUMBER_OF_OPEN_AREAS = 3; // hard-coded into toolset: do not change
		public const string NAME_OF_SCRATCHPAD_AREA = "Scratchpad";
		public static readonly Font ADVENTURE_AUTHOR_FONT = new Font("Arial",10.0F);
		
		#endregion Constants
			
		#region Global variables
		
		private static Adventure currentAdventure = null;
		private static User currentUser = new User("Anonymous");		
		private static bool beQuiet = false;
		private static bool doBackups = true;
		private static string adventureAuthorDir = @"C:\AdventureAuthor\";
		private static string backupDir = Path.Combine(adventureAuthorDir,"backups");
		private static string serializedDir = Path.Combine(adventureAuthorDir,"serialized");
		private static string logDir = Path.Combine(adventureAuthorDir,"logs");
		
		public static Adventure CurrentAdventure {
			get { return currentAdventure; }
		}
		
		public static bool BeQuiet {
			get	{ return beQuiet; }
			set	{ beQuiet = value; } 
		}
		
		public static bool DoBackups {
			get { return doBackups; }
			set { doBackups = value; }
		}
		
		public static string AdventureAuthorDir {
			get { return adventureAuthorDir; }
		}	
		
		public static string BackupDir {
			get { return backupDir; }
		}					
				
		public static string SerializedDir {
			get { return serializedDir; }
		}	
				
		public static User CurrentUser {
			get { return currentUser; }
		}				
		
		public static string LogDir {
			get { return logDir; }
		}
		
		#endregion Global variables
				
		#region Fields
		
		private string name;
		private string blurb;
		private string modulePath;
		private User owningUser;
		private NWN2GameModule module;	
		private Scratchpad scratch;
		
		[XmlElement]
		public Scratchpad Scratch {
			get { return scratch; }
			set { scratch = value; }
		}
				
		[XmlElement]		
		public SerializableDictionary<string,Chapter> Chapters;
				
		[XmlElement]
		public string Name {
			get { return name; }
			set { name = value; }
		}
		
		[XmlElement]
		public string Blurb {
			get { return blurb; }
			set { blurb = value; }
		}
		
		[XmlElement]
		public string ModulePath {
			get { return modulePath; }
			set { modulePath = value; }
		}		
		
		[XmlElement]
		public User OwningUser {
			get { return owningUser; }
		}
				
		[XmlIgnore]
		public NWN2GameModule Module {
			get { return module; }
			set { 
				if (value.LocationType != ModuleLocationType.Directory) {
					throw new InvalidOperationException("An Adventure's module must be directory-based - tried to assign a module " +
					                                    "of type " + value.LocationType.ToString() + ".");
				}
				else {				
					module = value; 
				}
			}
		}				
		
		#endregion Fields
		
		#region Constructors
		
		/// <summary>
		/// Parameterless constructor, for serialization purposes only.
		/// </summary>
		private Adventure()
		{
		}
		
		/// <summary>
		/// Creates a new Adventure with a given name.
		/// </summary>
		/// <param name="name">The Adventure name</param>
		public Adventure(string name)
		{
			NWN2GameModule mod = new NWN2GameModule();
			mod.Name = name;
			mod.LocationType = ModuleLocationType.Directory;
			mod.FileName = Path.Combine(form.ModulesDirectory,mod.Name);	
			mod.ModuleInfo.XPScale = 0; // player will gain no experience
			
			this.module = mod;
			this.name = mod.Name;
			this.modulePath = mod.FileName;
			this.owningUser = Adventure.CurrentUser;
			this.Chapters = new SerializableDictionary<string,Chapter>();
			this.scratch = new Scratchpad(this);
				
			this.Serialize();
		}	
		
		#endregion Constructors
		
		#region Open and delete adventures (static)
				
		//TODO: Returns a bool, not an Adventure
		/// <summary>
		/// Open an Adventure in the toolset.
		/// </summary>
		/// <param name="name">Name of the Adventure to open.</param>
		public static Adventure Open(string name)
		{
			if (currentAdventure != null) {
				currentAdventure.Close();
			}
			try {
				Adventure a = new Adventure();
				bool deserialized = a.Deserialize(name);				
				if (!deserialized) {
					return null;
				}
				else {
					currentAdventure = a;	
					Toolset.UpdateTitleBar();
					Toolset.UpdateChapterList();
					return a;
				}
			}
			catch (FileNotFoundException e) {
				Say.Error("Adventure '" + name + "' contained an area which did not form part of a Chapter or Scratchpad.",e);
				return null;
			}				
		}			
		
		//TODO: IMPLEMENT
		public static bool Delete(string adventureName)
		{	
			throw new NotImplementedException();
			
			//TODO: 
			
//			if (module.LocationType == NWN2Toolset.NWN2.IO.ModuleLocationType.Temporary) {
//				Say.Error("Working on a temporary module; nothing to delete.");
//				return false;
//			}
//			
//			try	{
//				// Check that we really want to delete the module, exit if we don't:
//				if (!Adventure.BeQuiet) {
//					if (MessageBox.Show("Delete " + module.FileName + "?",
//			             			 	"Delete module?", 
//				                 		 MessageBoxButtons.YesNo, 
//				                 	     MessageBoxIcon.Question, 
//				                	     MessageBoxDefaultButton.Button2) == DialogResult.No) { return false; }
//				}
//							
//				if (Adventure.DoBackups)	{
//					System.IO.Directory.Move(module.FileName,GameArea.GetBackupDirname(module.Name)); // move module to backup directory
//				}
//				else {
//					DirectoryInfo d = new DirectoryInfo(module.FileName);				
//					if (d.Exists) // module could be either directory or file, although should only be a directory
//					{
//						OEIShared.Utils.CommonUtils.DeleteDirectory(module.FileName,true);
//					}
//					else {
//						System.IO.File.Delete(module.FileName);
//					}	
//				}
//						
//				Adventure.Close();
//				Say.Information("Module deleted.");
//				return true;
//			}		
//			catch (Exception e)	{
//				throw e;
//			}
		}		
		 
		#endregion Open and delete adventures
						
		#region Save
		
		public void Save()
		{
			if (this.module.LocationType != ModuleLocationType.Directory) {
				throw new InvalidOperationException("An Adventure's module must be directory-based - tried to save a module " +
					                                "of type " + module.LocationType.ToString() + ".");
			}
			else if (this != currentAdventure) {
				throw new InvalidOperationException("Tried to operate on a closed Adventure.");
			}	
					     
			// Save the Adventure data:
		    FileInfo f = new FileInfo(Path.Combine(Adventure.SerializedDir,module.FileName+".xml"));
			Stream s = f.Open(FileMode.Create);	
			XmlSerializer xml = new XmlSerializer(typeof(Adventure));
			xml.Serialize(s,this);				
			s.Close();			
			
			// Save the module data:
		    form.App.WaitForPanelsToSave();  	        
		    if (form.VersionControlManager.OnModuleSaving()) {
		      	ThreadedSaveHelper save = new ThreadedSaveHelper(form.App,
		       	                                                 module.FileName,
		       	                                                 ModuleLocationType.Directory);
		       	ThreadedProgressDialog progress = new ThreadedProgressDialog();
		       	progress.Text = "Saving";
		       	progress.Message = "Saving '" + module.FileName + "'";
		       	progress.WorkerThread = new ThreadedProgressDialog.WorkerThreadDelegate(save.Go);
		       	progress.ShowDialog(form.App);
		    }
		        
		    // Update the title bar:
		    Toolset.UpdateTitleBar();
		}
					
		public void SaveAs(string newName)
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
		public void Backup(string reason)
		{
			string backupPath = GetDirectoryPathForBackup();
			string originalPath = Path.Combine(form.ModulesDirectory,this.module.FileName);
			
			try {
				// Copy the module files to the backup directory:
				DirectoryInfo di = new DirectoryInfo(backupPath);
				di.Create();
				CopyFolders(originalPath,backupPath);	
				
				// Copy the Adventure file to the backup directory:
				string serializedDataFilename = this.name + ".xml";
				File.Copy(Path.Combine(Adventure.SerializedDir,serializedDataFilename),
				          Path.Combine(backupPath,serializedDataFilename));
				
				// If given a justification for the backup, write it to a file in the backup directory:
				if (reason != string.Empty) {
					FileInfo reasonForBackup = new FileInfo(Path.Combine(backupPath,"Reason for backup.txt"));
					StreamWriter sw = new StreamWriter(reasonForBackup.OpenWrite());					
					sw.WriteLine("Reason for backup:  " + reason);
					sw.WriteLine();					
					sw.WriteLine("Created " + Log.GetDate());	
					sw.Flush();
					sw.Close();
				}
			}
			catch (IOException e) {
				Say.Error("Problem with writing to disk when trying to back up an Adventure.",e);
			}
		}
		
		private string GetDirectoryPathForBackup()
		{
			string path = Path.Combine(Adventure.BackupDir,this.Name+"_"+Log.GetDate()+"___");			
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
		/// Used to persist changes to the Adventure.
		/// </summary>
		public void Serialize()
		{			
			this.module.OEISerialize(this.name);				
						
			foreach (Chapter c in this.Chapters.Values) {
				c.Area.OEISerialize();
			}			
			this.scratch.Area.OEISerialize();
			
			FileInfo f = new FileInfo(Path.Combine(Adventure.SerializedDir,module.Name+".xml"));
			Stream s = f.Open(FileMode.Create);	
			XmlSerializer xml = new XmlSerializer(typeof(Adventure));
			xml.Serialize(s,this);				
			s.Close();
		}			
				
		/// <summary>
		/// Used to retrieve an Adventure from disk.
		/// </summary>
		/// <exception cref="FileNotFoundException">Thrown if an area was found in this module which did not form part of either a Chapter or a Scratchpad.</exception>
		private bool Deserialize(string nameOfAdventure)
		{
			// Deserialize the Adventure data:
			Stream s = null;
			try	{
				FileInfo f = new FileInfo(Path.Combine(Adventure.SerializedDir,nameOfAdventure+".xml"));
				s = f.Open(FileMode.Open); // throws an exception if serialized data is missing			
				XmlSerializer xml = new XmlSerializer(typeof(Adventure));			
				Adventure adventure = (Adventure)xml.Deserialize(s);

				this.blurb = adventure.Blurb;
				this.Chapters = adventure.Chapters;
				this.modulePath = adventure.ModulePath;
				this.name = adventure.Name;
				this.scratch = adventure.Scratch;
				this.owningUser = adventure.OwningUser;				
			}
			catch (FileNotFoundException e) {
				Say.Error("AdventureAuthor data for '" + nameOfAdventure + "' could not be found.",e);
				return false;
			}
			finally {
				if (s != null) {
					s.Close();
				}
			}
			
			// Deserialize the module data and open the module in the toolset:
			ThreadedOpenHelper toh = null;
			try {
				toh = new ThreadedOpenHelper(form.App,this.name,ModuleLocationType.Directory);
				toh.Go();
				form.App.SetupHandlersForGameResourceContainer(form.App.Module);								
				Toolset.UpdateTitleBar();					
			}
			catch (DirectoryNotFoundException e) {
				Say.Error("Directory-based module '" + this.name + "' could not be found.",e);
				return false;
			}
			finally {
				toh = null;
			}
							
			// Set up the Adventure and module to refer to each other again:
			this.module = form.App.Module;
			this.module.LocationType = ModuleLocationType.Directory;	
							
			foreach (NWN2GameArea area in this.module.Areas.Values) {	
				if (Chapters.ContainsKey(area.Name)) {
					Chapter chapter = Chapters[area.Name];
					area.Demand(); // TODO: Do I need to do this?
					chapter.Area = area;
					chapter.OwningAdventure = this;
				}
				else if (area.Name == scratch.Name) {
					area.Demand(); // TODO: Do I need to do this?
					scratch.Area = area;
					scratch.OwningAdventure = this;
				}
				else {
					throw new FileNotFoundException("Area '" + area.Name + "' in this Adventure is not part of a Chapter or Scratchpad.");
				}
			}
							    
			return true;
		}					
		
		#endregion Save
		
		#region Run, close, add and delete chapters and delete resources
		
		public void Close()
		{
			if (this != currentAdventure) {
				throw new InvalidOperationException("Tried to operate on a closed Adventure.");
			}
			
			if (NWN2ToolsetMainForm.VersionControlManager.OnModuleClosing()) {				
				OEIShared.Actions.ActionManager.Manager.Clear(); // ??	
				form.App.Module.CloseModule();            
		        form.VersionControlManager.OnModuleClosed();            
		        form.App.ClearHandlersForGameResourceContainer(form.App.Module);	                  
		        if (form.App.Module != null) {
		        	form.App.Module.Dispose();
		        }	
		        form.App.DoNewModule(true);		            
		        currentAdventure = null;		           
		        Toolset.Clear();
			}	
		}
				
		/// <summary>
		/// Run the currently open Adventure.
		/// </summary>
		/// <param name="waypoint">The tag of the waypoint to run from - if empty, run from the player start.</param>
		/// <param name="debugOn">If true, the debug window is accessible during gameplay. Currently does nothing.</param>
		/// <param name="playerIsInvincible">If true, the player cannot be harmed. Currently does nothing.</param>
		public void Run(string waypoint, bool debugOn, bool playerIsInvincible)
		{
			if (this != currentAdventure) {
				throw new InvalidOperationException("Tried to operate on a closed Adventure.");
			}
			
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
					
			// Save, bake and run the module:	
			Save();			
			form.App.DoBakeAll(false,false);
			form.App.RunModule(waypoint,false,false,false);
		}			
							
		//TODO: IMPLEMENT
		/// <summary>
		/// Delete a resource i.e. an area, script or conversation
		/// </summary>
		/// <param name="resource">The resource to delete</param>
		public void DeleteResource(object resource)
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
						
		/// <summary>
		/// Add a new chapter to the adventure
		/// </summary>
		/// <param name="name">The name to give the new chapter</param>
		/// <returns>The chapter which has just been created, or null if creation failed</returns>
		public Chapter AddChapter(string name)
		{	
			return AddChapter(name,String.Empty,true,Adventure.MIN_AREA_LENGTH,Adventure.MIN_AREA_LENGTH);
		}
		
		/// <summary>
		/// Add a new chapter to the adventure
		/// </summary>
		/// <param name="name">The chapter name</param>
		/// <param name="introduction">The chapter introduction, displayed to the player in-game</param>
		/// <param name="exterior">Whether or not the chapter takes place outdoors</param>
		/// <param name="size">Size of the area in terms of width and height, which must both be between 2 and 8</param>
		/// <returns>The chapter which has just been created, or null if creation failed</returns>
		public Chapter AddChapter(string name, string introduction, bool exterior, int width, int height)
		{		
			if (this != currentAdventure) {
				throw new InvalidOperationException("Tried to operate on a closed Adventure.");
			}
			
			if (name.ToLower() == NAME_OF_SCRATCHPAD_AREA) {
				Say.Error("Can not add a chapter named '" + NAME_OF_SCRATCHPAD_AREA + ".");
				return null;
			}		
			else if (!IsValidName(name)){
				throw new ArgumentException("'" + name + "' is an invalid chapter name - the name you enter must be between " + 
				                            "1 and 32 characters long, and must not contain the following characters:  < > : \\ \" / | .");
			}								
			else if (module.IsNameOccupied(ModuleResourceType.Area,name)) {
				Say.Error("Could not add chapter named '" + name + "' - that name is already taken.");
				return null;
			}
			
			Size validSize = new Size(width,height);
			
			Chapter c = new Chapter(this,name,introduction,exterior,validSize);
			module.AddResource(c.Area);
			Chapters.Add(c.Name,c);
			Serialize();
			Toolset.UpdateChapterList();
			return c;
		}
		
		public bool DeleteChapter(Chapter chapter)
		{
			if (this != currentAdventure) {
				throw new InvalidOperationException("Tried to operate on a closed Adventure.");
			}
			
			// If the Chapter is currently open, close it silently:
			INWN2Viewer viewer = form.App.GetViewerForResource(chapter.Area);			
			if (viewer != null) {
				viewer.Close(false);
			}
			
			// If requested, backup the entire Adventure to the AdventureAuthor backup directory:
			if (Adventure.DoBackups) {
				Backup("Deleted Chapter '" + chapter + "'.");
			}
			
			// Delete the area:
			module.RemoveResource(chapter.Area);
			string path = Path.Combine(form.ModulesDirectory,module.FileName);
			string[] areaFiles = Directory.GetFiles(path,chapter.Area.Name+"*");
			foreach (string filename in areaFiles) {
				Directory.Delete(filename);
			}
			
			// Delete the chapter:
			Chapters.Remove(chapter.Name);
			Serialize(); // remove references to chapter from serialized data
			
			// Refresh user interface:
			if (form.App.AreaContents.Area == chapter.Area) {
				form.App.AreaContents.Area = null;
				form.App.AreaContents.Refresh();
			}			
			Toolset.UpdateChapterList();
			return true;
		}		
		
		#endregion Run, close, add and delete chapters and delete resources
		
		#region Name checks
		
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
		public static bool IsAvailableAdventureName(string name)
		{
			DirectoryInfo moduleDir = new DirectoryInfo(Path.Combine(form.ModulesDirectory,name));			
			string serializedDataPath = Path.Combine(SerializedDir,name+".xml");
			if (moduleDir.Exists) {
				if (File.Exists(serializedDataPath)) {
					return false;
				}
				else {
					throw new IOException("A module exists with the name '" + name + "', but there is no AdventureAuthor data associated " +
					                      "with it - i.e. a standard Neverwinter Nights 2 module was found.");
				}
			}
			else {
				if (File.Exists(serializedDataPath)) {
					throw new IOException("No module exists with the name '" + name + "', but there is AdventureAuthor data associated " +
					                      "with an Adventure of that name at " + serializedDataPath + ".");				
				}
				else {
					return true;
				}
			}
		}
		
		#endregion Name checks
		
		public override string ToString()
		{			
			return this.name;
		}
	}
}
