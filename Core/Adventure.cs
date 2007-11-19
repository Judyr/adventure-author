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
		private static User currentUser = new User("Anon");		
		private static bool beQuiet = false;	
		private static bool debug = true;
		private static bool doBackups = true;	
		
		public static Adventure CurrentAdventure {
			get { return currentAdventure; }
		}
		
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
				
				
				try {
				StreamReader sr = new StreamReader(System.IO.File.Open(Path.Combine(System.Environment.CurrentDirectory,"logdirectory.txt"), FileMode.Open));
				string logdir = sr.ReadLine();
				if (logdir != null && logdir != String.Empty) {
					return Path.Combine(logdir,"");
				}
				else {
					Say.Error("No destination found for log files - in the file logdirectory.txt in the main NWN2 folder, " + 
					          "please type in a valid path e.g. C:\adventureauthorlogs");
					return Path.Combine(AdventureAuthorDir,"logs");
				}
				//return Path.Combine(AdventureAuthorDir,"logs");
				}
				catch (FileNotFoundException e) {
					Say.Error("Could not find log directory.",e);
					return Path.Combine(AdventureAuthorDir,"logs");
				}
			}
		}	
		
								
		public static User CurrentUser {
			get { return currentUser; }
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
			set 
			{ 
				if (value.LocationType != ModuleLocationType.Directory) {
					throw new InvalidOperationException("An Adventure's module must be directory-based - tried to assign a module " +
				                                    	"of type " + value.LocationType.ToString() + ".");
				}
				module = value; 
				LogChanges();
			}
		}
		
		internal void LogChanges()
		{
			// this seems to get called 6 times every time you set the start location (??):
			module.ModuleInfo.EntryPointChanged += delegate
			{ 
				Log.WriteAction(Log.Action.set,"startlocation"); 
			};
			module.ModuleExpanded += delegate 
			{
				Log.WriteMessage("module was 'expanded' (??)");
			};
			module.NameChanged += delegate(object sender, NameChangedEventArgs e) 
			{  
				if (e.OldName != e.NewName) { // ignore if this has been raised on a save
					Log.WriteAction(Log.Action.renamed,"module","'" + e.NewName + "' from '" + e.OldName + "'");
				}
			};
			
			module.Areas.Inserted += delegate(OEIDictionaryWithEvents cDictionary, object key, object value) 
			{  
				Log.WriteAction(Log.Action.added,"area",key.ToString());
			};
			module.Areas.Removed += delegate(OEIDictionaryWithEvents cDictionary, object key, object value) 
			{  
				Log.WriteAction(Log.Action.deleted,"area",key.ToString());
			};
				
			module.Scripts.Inserted += delegate(OEIDictionaryWithEvents cDictionary, object key, object value) 
			{  
				Log.WriteAction(Log.Action.added,"script",key.ToString());
			};
			module.Scripts.Removed += delegate(OEIDictionaryWithEvents cDictionary, object key, object value) 
			{  
				Log.WriteAction(Log.Action.deleted,"script",key.ToString());
			};
			
			module.FactionData.Factions.Inserted += delegate(OEICollectionWithEvents cList, int index, object value)
			{  
				Log.WriteAction(Log.Action.added,"faction",((NWN2Faction)value).Name);
			};				
			module.FactionData.Factions.Removed += delegate(OEICollectionWithEvents cList, int index, object value)
			{  
				Log.WriteAction(Log.Action.deleted,"faction",((NWN2Faction)value).Name);
			};	
			
			module.Journal.Categories.Inserted += delegate(OEICollectionWithEvents cList, int index, object value)
			{  
				Log.WriteAction(Log.Action.added,"journalcategory");
			};				
			module.Journal.Categories.Removed += delegate(OEICollectionWithEvents cList, int index, object value)
			{  
				Log.WriteAction(Log.Action.deleted,"journalcategory");
			};	
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
			Log.WriteAction(Log.Action.added,"module",name);
			
			NWN2GameModule mod = new NWN2GameModule();
			mod.Name = name;
			mod.LocationType = ModuleLocationType.Directory;
			mod.FileName = Path.Combine(form.ModulesDirectory,mod.Name);	
			mod.ModuleInfo.XPScale = 0; // player will gain no experience
			
			this.Module = mod;
			this.name = Module.Name;
			this.modulePath = Module.FileName;
			this.owningUser = Adventure.CurrentUser;
			this.Chapters = new SerializableDictionary<string,Chapter>();
			this.scratch = new Scratchpad(this);
			
			try {
				ScriptHelper.ApplyDefaultScripts(this.Module);
			}
			catch (IOException e) {
				Say.Error("Could not find Adventure Author logging scripts to assign to this resource.",e);
			}
				
			this.Serialize();
		}	
		
		
		/// <summary>
		/// If a module is missing adventure data, construct a new adventure with the module object.
		/// </summary>
		/// <param name="module">The module which is missing adventure data</param>
		public Adventure(NWN2GameModule mod)
		{
			if (mod.LocationType != ModuleLocationType.Directory) {
				throw new ArgumentException("Cannot create an adventure with a module in directory format.");
			}
			
			if (mod.ModuleInfo.XPScale != 0) {
				mod.ModuleInfo.XPScale = 0;
			}
			
			this.Module = mod;
			this.name = Module.Name;
			this.modulePath = Module.FileName;
			this.owningUser = Adventure.CurrentUser;
			this.Chapters = new SerializableDictionary<string,Chapter>();
						
			foreach (NWN2GameArea area in this.Module.Areas) {
				if (area.Name == NAME_OF_SCRATCHPAD_AREA) {
					if (this.scratch != null) {
						throw new ArgumentException("Cannot have two " + NAME_OF_SCRATCHPAD_AREA + " areas!");
					}
					else {
						this.scratch = new Scratchpad(this,area);
					}
				}
				else {
					this.AddChapter(area);
				}
			}
			
			if (this.scratch == null) {
				this.scratch = new Scratchpad(this);
			}
			
			try {
				ScriptHelper.ApplyDefaultScripts(this.Module);
			}
			catch (IOException e) {
				Say.Error("Could not find Adventure Author logging scripts to assign to this resource.",e);
			}
				
			this.Serialize();
		}
		
		#endregion Constructors
		
		#region Open and delete adventures (static)
				
		//TODO: Returns a bool, not an Adventure
		/// <summary>
		/// Open an Adventure in the toolset.
		/// </summary>
		/// <param name="name">Name of the Adventure to open.</param>
		public static bool Open(string name)
		{
			Log.WriteAction(Log.Action.opened,"module",name);
			
			if (currentAdventure != null) {
				currentAdventure.Close();
			}
			else if (form.App.Module != null) { // in case opening an adventure has failed previously, but a module is still open
				CloseModule();
			}
			
			try {
				Adventure adventure = new Adventure();
				adventure.Deserialize(name);
				currentAdventure = adventure;
				Toolset.UpdateTitleBar();
				Toolset.UpdateChapterList();
				return true;
			}
			catch (ArgumentException e) {
				Say.Error("Failed to open module.",e);
				return false;
			}
			catch (DirectoryNotFoundException e) {
				Say.Error("Failed to open module.",e);
				return false;
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
					
			Log.WriteAction(Log.Action.saved,"module");
			
			// Save the Adventure data:
		    FileInfo f = new FileInfo(Path.Combine(Adventure.CurrentAdventure.ModulePath,module.FileName+".xml"));
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
				File.Copy(Path.Combine(Adventure.CurrentAdventure.ModulePath,serializedDataFilename),
				          Path.Combine(backupPath,serializedDataFilename));
				
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
				Say.Error("Problem with writing to disk when trying to back up an Adventure.",e);
			}
		}
		
		private string GetDirectoryPathForBackup()
		{
			string path = Path.Combine(Adventure.BackupDir,this.Name+"_"+UsefulTools.GetDateStamp()+"___");			
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
			string path = Path.Combine(Path.Combine(form.ModulesDirectory,module.Name),module.Name+".xml");
			Say.Debug(path);
			FileInfo f = new FileInfo(path);
			Stream s = f.Open(FileMode.Create);	
			XmlSerializer xml = new XmlSerializer(typeof(Adventure));
			xml.Serialize(s,this);				
			s.Close();
		}			
				
		
		/// <summary>
		/// Used to retrieve an Adventure from disk.
		/// </summary>
		/// <exception cref="FileNotFoundException">Thrown if an area was found in this module which did not form part of either a Chapter or a Scratchpad.</exception>
		private bool crapDeserialize(string nameOfAdventure)
		{									
			// Deserialize the module data and open the module in the toolset:
			ThreadedOpenHelper toh = null;
			try {
				toh = new ThreadedOpenHelper(form.App,this.name,ModuleLocationType.Directory);
				toh.Go();
				form.App.SetupHandlersForGameResourceContainer(form.App.Module);								
				Toolset.UpdateTitleBar();					
			}
			catch (DirectoryNotFoundException e) {
				throw new DirectoryNotFoundException("Directory-based module '" + nameOfAdventure + "' could not be found.",e);
			}
			finally {
				toh = null;
			}
			
			
			// Deserialize the Adventure data:
			Stream s = null;
			Adventure adventure = null;
			try	{
				string moduleDirectory = Path.Combine(form.ModulesDirectory,nameOfAdventure);
				FileInfo f = new FileInfo(Path.Combine(moduleDirectory,nameOfAdventure+".xml"));
				s = f.Open(FileMode.Open); // throws an exception if serialized data is missing			
				XmlSerializer xml = new XmlSerializer(typeof(Adventure));			
				adventure = (Adventure)xml.Deserialize(s);					
				this.blurb = adventure.Blurb;
				this.Chapters = adventure.Chapters;
				this.modulePath = adventure.ModulePath;
				this.name = adventure.Name;
				this.scratch = adventure.Scratch;
				this.owningUser = adventure.OwningUser;	
							
				// Set up the Adventure data and module to refer to each other:
				form.App.Module.LocationType = ModuleLocationType.Directory;	
				this.Module = form.App.Module;
								
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
			}
			catch (FileNotFoundException) {
				// Construct a new adventure from scratch using this module:				
				Say.Warning("Adventure author data for module '" + nameOfAdventure + "' could not be found - rebuilding.");
				adventure = new Adventure(form.App.Module); // may throw an ArgumentException, which is not handled here
			}
			finally {
				if (s != null) {
					s.Close();
				}
			}
							    
			return true;
		}					
		
		
		
//		
//		/// <summary>
//		/// Used to retrieve an Adventure from disk.
//		/// </summary>
//		/// <exception cref="FileNotFoundException">Thrown if an area was found in this module which did not form part of either a Chapter or a Scratchpad.</exception>
//		private bool Deserialize(string nameOfAdventure)
//		{
//			// Deserialize the Adventure data:
//			Stream s = null;
//			try	{
//				FileInfo f = new FileInfo(Path.Combine(Path.Combine(form.ModulesDirectory,nameOfAdventure),nameOfAdventure+".xml"));
//				s = f.Open(FileMode.Open); // throws an exception if serialized data is missing			
//				XmlSerializer xml = new XmlSerializer(typeof(Adventure));			
//				Adventure adventure = (Adventure)xml.Deserialize(s);
//
//				this.blurb = adventure.Blurb;
//				this.Chapters = adventure.Chapters;
//				this.modulePath = adventure.ModulePath;
//				this.name = adventure.Name;
//				this.scratch = adventure.Scratch;
//				this.owningUser = adventure.OwningUser;				
//			}
//			catch (DirectoryNotFoundException e) {
//				throw new DirectoryNotFoundException("Directory-based module '" + nameOfAdventure + "' could not be found.",e);
//			}
//			catch (FileNotFoundException e) {
//				Say.Error("Adventure data for module '" + nameOfAdventure + "' could not be found - attempting to rebuild.",e);
//				this.blurb = String.Empty;
//				this.Chapters 
//			}
//			finally {
//				if (s != null) {
//					s.Close();
//				}
//			}
//			
//			// Deserialize the module data and open the module in the toolset:
//			ThreadedOpenHelper toh = null;
//			try {
//				toh = new ThreadedOpenHelper(form.App,this.name,ModuleLocationType.Directory);
//				toh.Go();
//				form.App.SetupHandlersForGameResourceContainer(form.App.Module);								
//				Toolset.UpdateTitleBar();					
//			}
//			catch (DirectoryNotFoundException e) {
//				Say.Error("Directory-based module '" + this.name + "' could not be found.",e);
//				return false;
//			}
//			finally {
//				toh = null;
//			}
//							
//			// Set up the Adventure and module to refer to each other again:
//			form.App.Module.LocationType = ModuleLocationType.Directory;	
//			this.Module = form.App.Module;
//							
//			foreach (NWN2GameArea area in this.module.Areas.Values) {	
//				if (Chapters.ContainsKey(area.Name)) {
//					Chapter chapter = Chapters[area.Name];
//					area.Demand(); // TODO: Do I need to do this?
//					chapter.Area = area;
//					chapter.OwningAdventure = this;
//				}
//				else if (area.Name == scratch.Name) {
//					area.Demand(); // TODO: Do I need to do this?
//					scratch.Area = area;
//					scratch.OwningAdventure = this;
//				}
//				else {
//					throw new FileNotFoundException("Area '" + area.Name + "' in this Adventure is not part of a Chapter or Scratchpad.");
//				}
//			}
//							    
//			return true;
//		}					
//		
		
		/// <summary>
		/// Used to retrieve an Adventure from disk.
		/// </summary>
		/// <exception cref="FileNotFoundException">Thrown if an area was found in this module which did not form part of either a Chapter or a Scratchpad.</exception>
		private bool Deserialize(string nameOfAdventure)
		{
			// Deserialize the Adventure data:
			Stream s = null;
			try	{
				FileInfo f = new FileInfo(Path.Combine(Path.Combine(form.ModulesDirectory,nameOfAdventure),nameOfAdventure+".xml"));
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
			catch (DirectoryNotFoundException e) {
				Say.Error("Module '" + nameOfAdventure + "' could not be found.",e);
				return false;
			}
			catch (FileNotFoundException e) {
				Say.Error("Serialized data for '" + nameOfAdventure + "' could not be found.",e);
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
			form.App.Module.LocationType = ModuleLocationType.Directory;	
			this.Module = form.App.Module;
							
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
		
		
		/// <summary>
		/// Close the currently open Adventure.
		/// </summary>
		public void Close()
		{
			if (this != currentAdventure) {
				throw new InvalidOperationException("Tried to operate on a closed Adventure.");
			}
			
			Log.WriteAction(Log.Action.closed,"module",CurrentAdventure.Name);
			
			CloseModule();
		    currentAdventure = null;		           
		    Toolset.Clear();
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
					
			Save();						
			Bake();
			
			// create a copy of the log from the last game (otherwise it will be over-written):
			ProcessStartInfo copylogStartInfo = new ProcessStartInfo(Path.Combine(System.Environment.CurrentDirectory,"copylog.bat"));
			Process.Start(copylogStartInfo);
			
			Log.WriteAction(Log.Action.launched,"game",CurrentAdventure.Name);
			form.App.RunModule(waypoint,false,false,false);
		}
		
		
		private static void RunNWN2()
		{
			// if .Exited delegate works, on the process exiting copy the log with an appropriate filename
			
			
			// create a copy of the log from the last game (otherwise it will be over-written):
			ProcessStartInfo copylogStartInfo = new ProcessStartInfo(Path.Combine(System.Environment.CurrentDirectory,"copylog.bat"));
			Process.Start(copylogStartInfo);
			
			// start the game:
			ProcessStartInfo nwn2mainStartInfo = new ProcessStartInfo(Path.Combine(System.Environment.CurrentDirectory,"nwn2main.exe"));
			nwn2mainStartInfo.RedirectStandardOutput = true;
			nwn2mainStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			nwn2mainStartInfo.UseShellExecute = false;
			Log.WriteAction(Log.Action.launched,"game");
			Process nwn2main = new Process();
			nwn2main.StartInfo = nwn2mainStartInfo;
//			nwn2main.Disposed += delegate { Log.WriteAction(Log.Action.exited,"game",CurrentAdventure.Name); };
			nwn2main.Start();
		}
		
		
		public void Bake()
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
		/// For converting a non-AA module into an AA module - should not really be used otherwise.
		/// </summary>
		/// <param name="area">The existing area to form part of the chapter</param>
		/// <returns>The chapter which has just been created, or null if creation failed</returns>
		private Chapter AddChapter(NWN2GameArea area)
		{
			if (area.Name == NAME_OF_SCRATCHPAD_AREA) {
				throw new ArgumentException("Cannot create a chapter named Scratchpad.");
			}
			
			return new Chapter(this,area);
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
			string modulePath = Path.Combine(form.ModulesDirectory,name);
			DirectoryInfo moduleDir = new DirectoryInfo(modulePath);	
			string serializedDataPath = Path.Combine(modulePath,name+".xml");
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
