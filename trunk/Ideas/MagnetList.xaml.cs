using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using AdventureAuthor.Utils;
using AdventureAuthor.Core;
using AdventureAuthor.Setup;
using Microsoft.Win32;

namespace AdventureAuthor.Ideas
{
    /// <summary>
    /// Interaction logic for MagnetList.xaml
    /// </summary>

    public partial class MagnetList : UnserializableControl
    {
    	#region Constants
    	
    	/// <summary>
    	/// The maximum number of degrees a magnet may deviate from an angle of 0 (in either direction.)
    	/// </summary>  
    	/// <remarks>Note that this constant is repeated in MagnetBoardControl and MagnetListControl
    	/// with different values</remarks>  	
    	public const double MAXIMUM_ANGLE_IN_EITHER_DIRECTION = 6;
    	
    	#endregion
    	
    	#region Fields  
    	
    	private string filename;    	
		public string Filename {
			get { return filename; }
			set { 
				filename = value;
			}
		}     	
    	
    	
    	private List<IdeaCategory> visibleCategories;
    	
    	
    	private SortCriterion sortBy = SortCriterion.Alphabetically;
		public SortCriterion SortBy {
			get { return sortBy; }
			set { 
				sortBy = value;
			}
		}
    	
    	
    	/// <summary>
    	/// True to save automatically whenever a magnet is added or deleted.
    	/// </summary>
    	/// <remarks>Should only set this to true if a filename has been provided,
    	/// otherwise a crash may occur</remarks>
    	private bool saveAutomatically = false;    	
		public bool SaveAutomatically {
			get { return saveAutomatically; }
			set { 
				saveAutomatically = value; 
			}
		}
    	
    	
    	private Orientation orientation;
		public Orientation Orientation {
			get { return orientation; }
			set { 
				orientation = value;
				SetOrientation(orientation);
    			OnOrientationChanged(new EventArgs());
			}
		}
        
        
        private MouseButtonEventHandler magnetControl_MouseDoubleClickHandler;        
        private EventHandler<MagnetEventArgs> magnetControl_SelectedHandler;        
        private DragEventHandler magnetControl_DropHandler;
        private EventHandler magnetListChangedHandler;
    	
        
        private object padlock = new object();
        
        
        internal MenuItem appearsAtSideContextMenuItem = null; // since you can't easily name these (weird XAML thing)
        internal MenuItem wonkyMagnetsContextMenuItem = null;
        
    	#endregion
    	
    	#region Events
    	
    	public event EventHandler<MagnetEventArgs> MagnetAdded;    	
    	protected virtual void OnMagnetAdded(MagnetEventArgs e)
    	{
    		EventHandler<MagnetEventArgs> handler = MagnetAdded;
    		if (handler != null) {
    			handler(this,e);
    		}
    	}
    	
    	
    	public event EventHandler<MagnetEventArgs> MagnetDeleted;    	
    	protected virtual void OnMagnetDeleted(MagnetEventArgs e)
    	{
    		EventHandler<MagnetEventArgs> handler = MagnetDeleted;
    		if (handler != null) {
    			handler(this,e);
    		}
    	}
    	
    	
    	public event EventHandler<MagnetEventArgs> MagnetTransferredOut;    	
    	protected virtual void OnMagnetTransferredOut(MagnetEventArgs e)
    	{
    		EventHandler<MagnetEventArgs> handler = MagnetTransferredOut;
    		if (handler != null) {
    			handler(this,e);
    		}
    	}
		
    	
    	public event EventHandler<MagnetCategoryEventArgs> HidCategory;   	
		protected virtual void OnHidCategory(MagnetCategoryEventArgs e)
		{
			EventHandler<MagnetCategoryEventArgs> handler = HidCategory;
			if (handler != null) {
				handler(this, e);
			}
		}
		
    	
    	public event EventHandler<MagnetCategoryEventArgs> ShowedCategory;   	
		protected virtual void OnShowedCategory(MagnetCategoryEventArgs e)
		{
			EventHandler<MagnetCategoryEventArgs> handler = ShowedCategory;
			if (handler != null) {
				handler(this, e);
			}
		}
		
    	
    	public event EventHandler Scattered;   	
		protected virtual void OnScattered(EventArgs e)
		{
			EventHandler handler = Scattered;
			if (handler != null) {
				handler(this, e);
			}
		}
		
    	
    	public event EventHandler OrientationChanged;   	
		protected virtual void OnOrientationChanged(EventArgs e)
		{
			EventHandler handler = OrientationChanged;
			if (handler != null) {
				handler(this, e);
			}
		}
    	
    	#endregion
    	
    	#region Constructors
    	
        public MagnetList()
        {            
        	magnetControl_MouseDoubleClickHandler = new MouseButtonEventHandler(magnetControl_MouseDoubleClick);
        	magnetControl_DropHandler = new DragEventHandler(magnetControl_Drop);
        	magnetListChangedHandler = new EventHandler(MagnetListChanged_SaveAutomatically);
        		
            InitializeComponent();
            
            visibleCategories = new List<IdeaCategory>(Idea.IDEA_CATEGORIES.Length);
            foreach (IdeaCategory category in Idea.IDEA_CATEGORIES) {
            	visibleCategories.Add(category);
            }
            
            // All changes in the Magnet Box should be serialized automatically:
            MagnetAdded += delegate(object sender, MagnetEventArgs e) {
            	automaticSave(); 
            	UpdateMagnetCount();
            };
            MagnetDeleted += delegate(object sender, MagnetEventArgs e) {
            	automaticSave(); 
            	UpdateMagnetCount();
            	Log.WriteAction(LogAction.deleted,"idea",e.Magnet.ToString());
            };
            Scattered += delegate {
            	Log.WriteMessage("scattered " + GetMagnets(true).Count + " magnets");
            };
            HidCategory += delegate(object sender, MagnetCategoryEventArgs e) { 
            	Log.WriteAction(LogAction.hid,"ideacategory",e.Category.ToString());
            };
            ShowedCategory += delegate(object sender, MagnetCategoryEventArgs e) { 
            	Log.WriteAction(LogAction.showed,"ideacategory",e.Category.ToString());
            };
            
           	Toolset.Plugin.Options.PropertyChanged += new PropertyChangedEventHandler(userPreferencesPropertyChanged);
           	UpdateUseWonkyMagnets();
        }
        
        
        public MagnetList(MagnetListInfo magnetListInfo) : this()
        {        	
        	Open(magnetListInfo);
        }
        
        #endregion
        
        #region Methods
        
        /// <summary>
        /// Attempt to open a Magnet Box at the given filename.
        /// </summary>
        /// <param name="filename">The location of a Magnet Box, or a valid location
        /// where a new one can be created</param>
        /// <remarks>If there is no file at that location (but the location is valid) a new
        /// Magnet Box will be saved to it. If the location is invalid, or the existing file
        /// is corrupted or in the wrong format, tries to delete the corrupted file (giving the 
        /// user the option of backing it up) and create a new one. If anything further goes
        /// wrong, or the user declines to deal with the corrupted file, the Magnet Box will
        /// remain functional but not save any ideas.</remarks>
        public void Open(string filename)
        {   
	        if (!File.Exists(filename)) { // as long as a Magnet Box can be created at this filename, save to it
	            Say.Debug("Couldn't find a Magnet Box at the expected location (" + filename + ")" +
	        		     " - will create a new one when required.");
	          	Filename = filename;
	          	SaveAutomatically = true;
	        }
	        else {
	          	try { // try to open the Magnet Box, and if successful, save to it        		
		    		object o = AdventureAuthor.Utils.Serialization.Deserialize(filename,typeof(MagnetListInfo));
			    	MagnetListInfo magnetListInfo = (MagnetListInfo)o;
			    	Open(magnetListInfo);
			    	Filename = filename;
			    	SaveAutomatically = true;
	           	} 
	           	catch (Exception e) {
	           		if (e.InnerException is InvalidOperationException) {
        				if (ModuleHelper.BeQuiet) { // don't show dialog if in quiet (automated) mode
        					AbortOpen();
        				}
		           		MessageBoxResult result = 
		           			MessageBox.Show( // this will appear before toolset has finished loading
		           				  "There was a problem when setting up the Adventure Author fridge magnet application.\n\n" +
		           				  filename + " is not a valid Magnet Box file. It may " +
		           	    	      "be corrupted, or the wrong type of file.\n\n" + 
		           	    	      "This file must be replaced to continue. Do you want to back up the " +
		           	    	      "corrupted file in case it can be fixed?",
		           	    	      "Back up corrupted ideas file?",
		           	    	      MessageBoxButton.YesNoCancel);
		           		switch (result) {
		           			case MessageBoxResult.Cancel: // user didn't want to deal with corrupted file
		           				AbortOpen();
		           				break;
		           			case MessageBoxResult.Yes: // user wants to back up the corrupted file before deleting it
		           				SaveFileDialog saveFileDialog = new SaveFileDialog();
					    		saveFileDialog.AddExtension = true;
					    		saveFileDialog.CheckPathExists = true;
					    		saveFileDialog.DefaultExt = Filters.XML;
					    		saveFileDialog.Filter = Filters.XML;
					  			saveFileDialog.ValidateNames = true;
					  			saveFileDialog.OverwritePrompt = true;
					  			saveFileDialog.Title = "Select location to save copy of corrupted Magnet Box";
					  			
					  			bool ok = (bool)saveFileDialog.ShowDialog();  				
					  			if (ok) {	
					  				try {
					  					File.Copy(filename,saveFileDialog.FileName);
		           						File.Delete(filename);
		           						Filename = filename;
		           						SaveAutomatically = true;
					  				} 
					  				catch (Exception ex) {
					  					Say.Error("Could not create a backup copy of the corrupted file.",ex);
					  					AbortOpen();
					  				}
					  			}
					  			else { // user changed their mind, and didn't want to deal with corrupted file
					  				AbortOpen();
					  			}
		           				break;
		           			case MessageBoxResult.No: // user just wants to delete the corrupted file
					  			try {
		           					File.Delete(filename);
		           					Filename = filename;
		           					SaveAutomatically = true;
					  			} 
					  			catch (Exception ex) {
					  				Say.Error("Could not delete the corrupted file.",ex);
					  				AbortOpen();
					  			}
		           				break;
		           		}
	           		}
		           	else { // something non-specific went wrong
        				Say.Error("Something went wrong when trying to open the Magnet Box.",e);
        				AbortOpen();
		           	}
	           	}
	        }        	
        }      
        
        
        private void AbortOpen()
        {
			Say.Warning("Any ideas you create in the fridge magnets application will not be saved during this session.");
		    Filename = null;
			SaveAutomatically = false;
			Clear();
        }
            	
    	
    	/// <summary>
    	/// Open a Magnet Box.
    	/// </summary>
    	/// <param name="magnetListInfo">Serialized data to represent</param>
    	/// <remarks>Changes will not be saved unless you set SaveAutomatically to true following
    	/// this call, and provide a valid filename</remarks>
    	private void Open(MagnetListInfo magnetListInfo)
    	{
	        Clear();
	    	ShowAllCategories(); // set all categories to be shown before adding magnets (faster)
	    	
	    	foreach (MagnetControlInfo magnetInfo in magnetListInfo.Magnets) {
	    		MagnetControl magnet = (MagnetControl)magnetInfo.GetControl();
	    		AddMagnet(magnet,false);
	        }
	        SaveAutomatically = false;
	        
	        UpdateMagnetCount();
    	}
    	
    	
    	private void UpdateMagnetCount()
    	{
    		this.magnetCountTextBlock.GetBindingExpression(TextBlock.TextProperty).UpdateTarget();
    		//this.numberOfMagnetsTextBlock.GetBindingExpression(TextBlock.TextProperty).UpdateTarget();
    	}
        
        
        /// <summary>
        /// Clear the Magnet Box.
        /// </summary>
        /// <remarks>This is intended for UI functions, rather than to delete all magnets in a Magnet Box,
        /// so no events are fired</remarks>
        private void Clear()
        {
        	magnetsPanel.Children.Clear();
        }
        
        
    	public void Save()
    	{
    		if (filename == null) {
    			throw new InvalidOperationException("Save failed: Should not have called Save without first setting a filename.");
    		}
    		else {
    			try {
    				lock (padlock) {
    					AdventureAuthor.Utils.Serialization.Serialize(Filename,this.GetSerializable());
    				}
    			} 
    			catch (Exception e) {
    				Say.Error("Changes to the Magnet Box could not be saved.",e);
    			}
    		}
    	} 
        
        
        /// <summary>
        /// Add an idea to the list.
        /// </summary>
        /// <param name="idea">The idea to add</param>
        public void AddIdea(Idea idea)
        {
        	MagnetControl magnet = new MagnetControl(idea);
        	AddMagnet(magnet,true);
        }
        
        
        /// <summary>
        /// Add a range of ideas to the list.
        /// </summary>
        /// <param name="ideas">The range of ideas to add</param>
        public void AddIdeas(List<Idea> ideas)
        {
        	foreach (Idea idea in ideas) {
        		AddIdea(idea);
        	}
        }
        
        
        /// <summary>
        /// Add a magnet representing an idea to the list
        /// </summary>
        /// <param name="magnet">The magnet to add</param>
        /// <param name="forceShow">True to force this magnet's category to be shown; false otherwise</param>
        /// <remarks>All Add methods ultimately call this method</remarks>
        public void AddMagnet(MagnetControl magnet, bool forceShow)
        {
        	AddHandlers(magnet);
        	
        	magnet.Margin = new Thickness(5);
        	
        	// update context menu:
        	magnet.bringToFrontMenuItem.IsEnabled = false;
        	magnet.sendToBackMenuItem.IsEnabled = false;
        	magnet.removeDeleteMagnetMenuItem.Header = "Delete"; // more permanent than removing from a magnet board
        	
        	magnetsPanel.Children.Add(magnet);
        	
        	// Angle the magnet consistently with current policy:
        	if (Toolset.Plugin.Options.UseWonkyMagnets) {
	        	bool angleToLeft = magnetsPanel.Children.IndexOf(magnet) % 2 == 0;
	        	magnet.RandomiseAngle(MAXIMUM_ANGLE_IN_EITHER_DIRECTION,angleToLeft);
        	}
        	else {
        		magnet.Angle = 0;
        	}
        	
        	magnet.UpdateStarVisibility();
        	        	
        	// If the magnet is newly created, ensure that it is shown, even if it is in a category which
        	// is currently hidden. If it's been transferred back to the Magnet Box after being deleted
        	// from a board, then it can retain the same shown/hidden state as its category.
        	bool categoryIsVisible = CategoryIsVisible(magnet.Idea.Category);
        	if (forceShow) {
	        	if (!categoryIsVisible) {
	        		ShowCategory(magnet.Idea.Category);
	        	}
	        	magnet.BringIntoView();
        	}
        	else {
        		if (!categoryIsVisible) {
        			magnet.Hide();
        		}
        	}
        	    		
        	OnMagnetAdded(new MagnetEventArgs(magnet));        	
        }
        
        
        /// <summary>
        /// Add Magnet Box event handlers when a magnet is added
        /// </summary>
        /// <param name="magnet">The magnet to add handlers for</param>
        private void AddHandlers(MagnetControl magnet)
        {
        	try {
        		magnet.MouseDoubleClick += magnetControl_MouseDoubleClickHandler;
        		magnet.Drop += magnetControl_DropHandler;
        		magnet.Edited += magnetListChangedHandler;
        		magnet.Starred += magnetListChangedHandler;
        		magnet.Unstarred += magnetListChangedHandler;
        		magnet.Rotated += magnetListChangedHandler;
        	}
        	catch (Exception e) {
        		Say.Error("Failed to add handlers to magnet.",e);
        	}
        }
        
        
        /// <summary>
        /// Remove Magnet Box event handlers when a magnet is removed (as it may be going elsewhere)
        /// </summary>
        /// <param name="magnet">The magnent to remove handlers from</param>
        private void RemoveHandlers(MagnetControl magnet)
        {
        	try {
        		magnet.MouseDoubleClick -= magnetControl_MouseDoubleClickHandler;
        		magnet.Drop -= magnetControl_DropHandler;
        		magnet.Edited -= magnetListChangedHandler;
        		magnet.Starred -= magnetListChangedHandler;
        		magnet.Unstarred -= magnetListChangedHandler;
        		magnet.Rotated -= magnetListChangedHandler;
        	}
        	catch (Exception e) {
        		Say.Error("Failed to remove handlers from magnet.",e);
        	}
        }
        
        
        /// <summary>
        /// Indicate that a magnet should be transferred to the magnet board
        /// </summary>
        /// <param name="magnet">The magnet to be transferred</param>
        public void TransferMagnet(MagnetControl magnet)
        {       	        	
	    	OnMagnetTransferredOut(new MagnetEventArgs(magnet));
        }
                
        
        /// <summary>
        /// Delete a magnet from this list
        /// </summary>
        /// <param name="magnet">The magnet to remove</param>
        public void DeleteMagnet(MagnetControl magnet)
        {
        	magnetsPanel.Children.Remove(magnet);        	
        	RemoveHandlers(magnet);	        	
	    	OnMagnetDeleted(new MagnetEventArgs(magnet));
        }
        
        
        private void UpdateUseWonkyMagnets()
        {
        	if (Toolset.Plugin.Options.UseWonkyMagnets) {
        		AngleMagnets(MAXIMUM_ANGLE_IN_EITHER_DIRECTION);
        	}
        	else {
        		StraightenMagnets();
        	}
        }
                        
        
        /// <summary>
        /// Show ideas belonging to a particular category.
        /// </summary>
        /// <param name="category">The category of ideas to show</param>
        public void ShowCategory(IdeaCategory category)
        {
        	ShowCategory(category,false);
        }
        
        
        /// <summary>
        /// Show ideas belonging to a particular category.
        /// </summary>
        /// <param name="category">The category of ideas to show</param>
        /// <param name="only">True to show only the given category, and hide everything else; 
        /// false to show the given category and ignore everything else</param>
        public void ShowCategory(IdeaCategory category, bool only)
        {
        	// Update list of visible categories:
        	if (only) {
        		foreach (IdeaCategory visCat in visibleCategories) {        			
        			if (visCat != category) {
        				OnHidCategory(new MagnetCategoryEventArgs(visCat));
        			}
        		}        		
        		visibleCategories.Clear();
        	}
        	
        	//if (!visibleCategories.Contains(category)) {
        		visibleCategories.Add(category);
        		
	        	foreach (MagnetControl magnet in magnetsPanel.Children) {
	        		if (magnet.Idea.Category == category) {
	        			magnet.Show();
	        		}
	        		else if (only) {
	        			magnet.Hide();
	        		}
	        	}	        	
	        	OnShowedCategory(new MagnetCategoryEventArgs(category));
        	//}
        }
                
        
        /// <summary>
        /// Show all idea categories.
        /// </summary>
        public void ShowAllCategories()
        {
        	// Update list of visible categories:        	
        	foreach (IdeaCategory category in Idea.IDEA_CATEGORIES) {
        		if (!visibleCategories.Contains(category)) {
        			visibleCategories.Add(category);
        			OnShowedCategory(new MagnetCategoryEventArgs(category));
        		}
        	}
        	foreach (MagnetControl magnet in magnetsPanel.Children) {
        		magnet.Show();
        	}
        }
        
        
        /// <summary>
        /// Hide ideas belonging to a particular category.
        /// </summary>
        /// <param name="category">The category of ideas to hide</param>
        public void HideCategory(IdeaCategory category)
        {
        	HideCategory(category,false);
        }
        
                
        /// <summary>
        /// Hide ideas belonging to a particular category.
        /// </summary>
        /// <param name="category">The category of ideas to hide</param>
        /// <param name="only">True to hide only the given category, and show everything else; 
        /// false to hide the given category and ignore everything else</param>
        public void HideCategory(IdeaCategory category, bool only)
        {
        	// Update list of visible categories:
        	if (visibleCategories.Contains(category)) {
        		visibleCategories.Remove(category);
        		OnHidCategory(new MagnetCategoryEventArgs(category));
        	}
        	
        	if (only) {
        		foreach (IdeaCategory cat in Idea.IDEA_CATEGORIES) {
        			if (cat != category && !visibleCategories.Contains(cat)) {
        				visibleCategories.Add(cat);
        				OnShowedCategory(new MagnetCategoryEventArgs(cat));
        			}
        		}    		
        	}
        	        	
        	foreach (MagnetControl magnet in magnetsPanel.Children) {
        		if (magnet.Idea.Category == category) {
        			magnet.Hide();
        		}
        		else if (only) {
        			magnet.Show();
        		}
        	}
        }
                
        
        /// <summary>
        /// Hide all idea categories.
        /// </summary>
        public void HideAllCategories()
        {
        	// Update list of visible categories:     	
        	foreach (IdeaCategory visCat in visibleCategories) {
        		OnHidCategory(new MagnetCategoryEventArgs(visCat));
        	}
        	Clear();
        }
    	        
        
        public List<MagnetControl> GetMagnets(bool visibleOnly)
        {
        	List<MagnetControl> magnets = new List<MagnetControl>(magnetsPanel.Children.Count);
        	foreach (MagnetControl magnet in magnetsPanel.Children) {
        		if (!visibleOnly || magnet.IsVisible) {
        			magnets.Add(magnet);
        		}
        	}
        	return magnets;
        }     	                
           
        
        /// <summary>
        /// Set the angle of each magnet in the list to 0. 
        /// </summary>
        private void StraightenMagnets()
        {
        	foreach (MagnetControl magnet in magnetsPanel.Children) {
        		magnet.Angle = 0;
        	}
        }  
        
        
        private void AngleMagnets(double maxDeviationFromZero)
        {
        	if (maxDeviationFromZero == 0) {
        		StraightenMagnets();
        		return;
        	}
        	
        	foreach (MagnetControl magnet in magnetsPanel.Children) {
        		bool angleToLeft = magnetsPanel.Children.IndexOf(magnet) % 2 == 0;
        		magnet.RandomiseAngle(maxDeviationFromZero,angleToLeft);
        	}
        }
            	
        
        public MagnetControl[] GetMagnets() 
        {
    		MagnetControl[] magnets = new MagnetControl[magnetsPanel.Children.Count];
    		foreach (MagnetControl magnet in magnetsPanel.Children) {
    			magnets[magnetsPanel.Children.IndexOf(magnet)] = magnet;
    		}
    		return magnets;
    	}    		
        
        
        public void Scatter()
        {
        	OnScattered(new EventArgs());
        }
    	
    	    	
        /// <summary>
    	/// Check whether a given magnet exists in this list.
    	/// </summary>
    	/// <param name="magnet">The magnet to check for</param>
    	/// <returns>True if the magnet object exists within this list; false otherwise</returns>
    	/// <remarks>This checks for the actual magnet object - to check for an equivalent magnet
    	/// (i.e. a magnet with identical field values) call HasEquivalentMagnet instead</remarks>
    	public bool HasMagnet(MagnetControl magnet)
    	{
    		return MagnetList.HasMagnet(magnetsPanel.Children,magnet);
    	}
    	
    	
    	/// <summary>
    	/// Check whether a given magnet exists within a collection of UI elements.
    	/// </summary>
    	/// <param name="elements">The UI element collection to check</param>
    	/// <param name="magnet">The magnet to check for</param>
    	/// <returns>True if the magnet object exists within this collection; false otherwise</returns>
    	/// <remarks>This checks for the actual magnet object - to check for an equivalent magnet
    	/// (i.e. a magnet with identical field values) call HasEquivalentMagnet instead</remarks>
    	public static bool HasMagnet(UIElementCollection elements, MagnetControl magnet)
    	{
    		return elements.Contains(magnet);
    	}
    	
    	        
    	/// <summary>
    	/// Check whether an equivalent magnet to the given magnet exists in this list.
    	/// </summary>
    	/// <param name="magnet">The magnet to check for</param>
    	/// <returns>True if a magnet with identical field values to the given magnet exists
    	/// within this list; false otherwise</returns>
    	/// <remarks>This checks for an equivalent magnet (i.e. a magnet with identical field values) - 
    	/// to check whether the collection contains the actual object, call HasMagnet instead</remarks>
    	public bool HasEquivalentMagnet(MagnetControl magnet)
    	{
    		return HasEquivalentMagnet(magnetsPanel.Children,magnet);
    	}
    	
    	        
    	/// <summary>
    	/// Check whether an equivalent magnet to the given magnet exists in a given element collection.
    	/// </summary>
    	/// <param name="magnet">The magnet to check for</param>
    	/// <param name="elements">The UI element collection to check</param>
    	/// <returns>True if a magnet with identical field values to the given magnet exists
    	/// within this list; false otherwise</returns>
    	/// <remarks>This checks for an equivalent magnet (i.e. a magnet with identical field values) - 
    	/// to check whether the collection contains the actual object, call HasMagnet instead</remarks>
    	public static bool HasEquivalentMagnet(UIElementCollection elements, MagnetControl magnet)
    	{
    		return GetEquivalentMagnet(elements,magnet) != null;
    	}
    	
    	    	
    	/// <summary>
    	/// Get an equivalent magnet to the given magnet, if it exists within this Magnet Box.
    	/// </summary>
    	/// <param name="magnet">The magnet to check for</param>
    	/// <returns>A magnet with identical field values to the given magnet if it exists
    	/// within this collection; null otherwise</returns>
    	/// <remarks>This checks for an equivalent magnet (i.e. a magnet with identical field values) - 
    	/// to check whether the collection contains the actual object, call HasMagnet instead</remarks>
    	public MagnetControl GetEquivalentMagnet(MagnetControl magnet)
    	{
    		return GetEquivalentMagnet(magnetsPanel.Children,magnet);
    	}
    	
    	    	
    	/// <summary>
    	/// Get an equivalent magnet to the given magnet, if it exists within a collection of UI elements.
    	/// </summary>
    	/// <param name="elements">The UI element collection to check</param>
    	/// <param name="magnet">The magnet to check for</param>
    	/// <returns>A magnet with identical field values to the given magnet if it exists
    	/// within this collection; null otherwise</returns>
    	/// <remarks>This checks for an equivalent magnet (i.e. a magnet with identical field values) - 
    	/// to check whether the collection contains the actual object, call HasMagnet instead</remarks>
    	public static MagnetControl GetEquivalentMagnet(UIElementCollection elements, MagnetControl magnet)
    	{
    		foreach (UIElement element in elements) { // return true if an equivalent object is found
    			if (element is MagnetControl) {
    				MagnetControl magnet2 = (MagnetControl)element;
    				
    				if (magnet.Idea.Equals(magnet2.Idea)) {
	    				return magnet2;
	    			}
    			}
    		}
    		return null;
    	}
    	
    	
    	/// <summary>
    	/// Check whether a given category of idea is currently being displayed.
    	/// </summary>
    	/// <param name="category">The category to check for visibility</param>
    	/// <returns>True if the category is currently set to be displayed; false otherwise</returns>
    	public bool CategoryIsVisible(IdeaCategory category)
    	{
    		return visibleCategories.Contains(category);
    	}
    	
    	
    	public void Sort()
    	{
    		MagnetControl[] magnets = GetMagnets();
    		
    		switch (SortBy) {
    			case SortCriterion.Alphabetically:
    				Array.Sort(magnets,MagnetControl.SortAlphabeticallyAscending);
    				magnetsPanel.Children.Clear();
    				foreach (MagnetControl magnet in magnets) {
    					magnetsPanel.Children.Add(magnet);
    				}
    				break;
    			case SortCriterion.Category:
    				break;
    			case SortCriterion.Created:
    				break;
    			case SortCriterion.Stars:
    				break;
    			case SortCriterion.Used:
    				break;    				
    		}
    	}  

        
        private void userPreferencesPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        	if (e.PropertyName == "UseWonkyMagnets") { // magnet board viewer updates the checkable menu item
        		UpdateUseWonkyMagnets();
        	}
        }
    	
    	
    	private void SetOrientation(Orientation orientation)
    	{
    		double magnetListHeight = 250; //220;
    		double magnetListWidth = 180;
    		
    		switch (orientation) {
    			case Orientation.Vertical:
    				Grid.SetRow(controlPanel,0);
    				Grid.SetColumn(controlPanel,1);
    				row0.MinHeight = magnetListHeight;
    				row0.MaxHeight = magnetListHeight;
    				row1.MaxHeight = double.MaxValue;
    				magnetsPanel.MaxHeight = double.MaxValue;
    				column0.MinWidth = 0;
    				column0.MaxWidth = 0;
    				column1.MinWidth = magnetListWidth;
    				column1.MaxWidth = magnetListWidth;
	        		magnetsPanel.Orientation = Orientation.Vertical;
    				scroller.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
    				scroller.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
    				break;
    			case Orientation.Horizontal:
    				Grid.SetRow(controlPanel,1);
    				Grid.SetColumn(controlPanel,0);
    				row0.MinHeight = 0;
    				row0.MaxHeight = 0;
    				row1.MinHeight = magnetListHeight;
    				row1.MaxHeight = magnetListHeight;
    				magnetsPanel.MaxHeight = 195;
    				column0.MinWidth = magnetListWidth;
    				column0.MaxWidth = magnetListWidth;
    				column1.MaxWidth = double.MaxValue;
	        		magnetsPanel.Orientation = Orientation.Horizontal;
    				scroller.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
    				scroller.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
    				break;
    		}
    	}
    	
		
    	public override ISerializableData GetSerializable()
    	{
    		return new MagnetListInfo(this);
    	}	
                
        #endregion
        
        #region Event handlers
        
        /// <summary>
        /// Scroll the Magnet Box by scrolling the mouse wheel.
        /// </summary>
        /// <remarks>This is a preview event because scrolling the wheel over the window
        /// should move the list up and down, but not rotate the selected magnet (which is
        /// what usually happens on wheel scroll.) Handling the event will stop
        /// the selected magnet from rotating, if we are currently over the Magnet Box.</remarks>
        private void magnetListPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
	        double offset = e.Delta / 2;
        	if (Orientation == Orientation.Horizontal) {
	        	scroller.ScrollToHorizontalOffset(scroller.HorizontalOffset - offset);
        	}
        	else { // technically unnecessary as it happens anyway, but left for clarity
	        	scroller.ScrollToVerticalOffset(scroller.VerticalOffset - offset);
        	}
	        e.Handled = true;
        }
        
        
        internal void OnClick_Scatter(object sender, RoutedEventArgs e)
        {
        	MessageBoxResult result = MessageBox.Show("Tip all visible magnets onto the board?",
		        					                  "Scatter magnets?", 
		        					                  MessageBoxButton.YesNo,
		        					                  MessageBoxImage.Question,
		        					                  MessageBoxResult.No,
		        					                  MessageBoxOptions.None);
        	if (result == MessageBoxResult.Yes) {
        		Scatter();
        	}
        }
    	
    	
        internal void OnClick_CreateMagnet(object sender, RoutedEventArgs e)
        {        	
        	EditMagnetWindow window = new EditMagnetWindow();
        	window.MagnetCreated += new EventHandler<MagnetEventArgs>(newMagnetCreated);
        	window.ShowDialog();
        }
    	
    	
        internal void OnClick_LuckyDip(object sender, RoutedEventArgs e)
        {        	
        	Window window = new Window();
        	window.SizeToContent = SizeToContent.WidthAndHeight;
        	window.Background = Brushes.DarkBlue;
        	StackPanel sp = new StackPanel();
        	sp.Orientation = Orientation.Horizontal;
        	Image einstein = AdventureAuthor.Utils.ResourceHelper.GetImage("einstein.png");
        	einstein.Height = 200;
        	einstein.Width = 180;
        	TextBlock tb = new TextBlock();
        	tb.Text = "Einstein says: \"Why not try making your own cheese? It is cheap and effective!";
        	tb.Background = Brushes.Transparent;
        	tb.Foreground = Brushes.White;
        	tb.FontSize = 18.0f;
        	sp.Children.Add(einstein);
        	sp.Children.Add(tb);
        	window.Content = sp;
        	window.ShowDialog();
        }

        
        private void newMagnetCreated(object sender, MagnetEventArgs e)
        {
        	AddMagnet(e.Magnet,true);
			Log.WriteAction(LogAction.added,"idea",e.Magnet.Idea.ToString() + " ... added from magnets app");
        }
        
        
        /// <summary>
        /// Transfer a copy of this magnet to the currently active magnet board.
        /// </summary>
    	private void magnetControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    	{
    		MagnetControl magnet = (MagnetControl)e.Source;
    		if (HasMagnet(magnet)) {
    			TransferMagnet(magnet);
    		}
    	}
    		
        
        /// <summary>
        /// Treat drop events on magnets as you would drop events on the list itself.
        /// </summary>
        private void magnetControl_Drop(object sender, DragEventArgs e)
        {
        	OnDrop(e);
        }
        
        
        /// <summary>
        /// Save all changes to the Magnet Box automatically, unless you don't have a valid
        /// filename or something went wrong on a previous attempt to save.
        /// </summary>
        private void MagnetListChanged_SaveAutomatically(object sender, EventArgs e)
        {
        	automaticSave();
        }
        
        
        private void automaticSave() 
        {
        	if (saveAutomatically) {
        		Save();
        	}        	
        }
        
        #endregion
    }
}