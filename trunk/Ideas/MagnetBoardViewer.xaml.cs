using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel;
using AdventureAuthor.Core;
using AdventureAuthor.Setup;
using AdventureAuthor.Utils;
using Microsoft.Win32;
using Microsoft.Samples.CustomControls;

namespace AdventureAuthor.Ideas
{
    /// <summary>
    /// Interaction logic for MagnetBoardViewer.xaml
    /// </summary>

    public partial class MagnetBoardViewer : Window
    {    
    	#region Fields  
    	
    	/// <summary>
    	/// The single instance of the magnets window.
    	/// <remarks>Pseudo-Singleton pattern, but I haven't really implemented this.</remarks>
    	/// </summary>
    	private static MagnetBoardViewer instance;    	
    	
		public static MagnetBoardViewer Instance {
			get { 
    			if (instance == null) {
    				instance = new MagnetBoardViewer();
    			}
    			return instance; 
    		}
		}   
    	    	
   
    	public MagnetBoardControl ActiveBoard {
    		get {
    			return magneticSurface;
    		}
    	}
    	
    	
    	private MagnetControl selectedMagnet;     	
		public MagnetControl SelectedMagnet {
			get { return selectedMagnet; }
			internal set {
	    		if (selectedMagnet != value) {
					if (selectedMagnet != null) {
		    			selectedMagnet.DeselectFX();
		    			selectedMagnet.Rotated -= invalidateUponRotateHandler;
					}
		    		selectedMagnet = value;
		    		if (selectedMagnet != null) {
		    			selectedMagnet.SelectFX();
		    			selectedMagnet.Rotated += invalidateUponRotateHandler;
		    		}
	    		}
			}
    	}
    	
    	
    	private EventHandler magnetControl_RequestRemoveHandler;
    	private MouseButtonEventHandler magnetControl_PreviewMouseLeftButtonDownHandler;
    	private MouseEventHandler magnetControl_PreviewMouseMoveHandler;
        private RoutedEventHandler magnetGotFocusHandler;
        private RoutedEventHandler magnetLostFocusHandler;
        private EventHandler invalidateUponRotateHandler;
    	
    	#endregion
    	
    	#region Constructors
    	
        public MagnetBoardViewer()
        {
        	InitializeComponent();
                                                
            // Set up 'Show/Hide idea category' menu:
            RoutedEventHandler showHideChangedHandler = new RoutedEventHandler(CategoryElementIsCheckedChanged);
            foreach (IdeaCategory ideaCategory in Idea.IDEA_CATEGORIES) {
            	ShowHideCategoryMenuItem menuItem = new ShowHideCategoryMenuItem(ideaCategory);
            	menuItem.IsChecked = true;
            	menuItem.Checked += showHideChangedHandler;
            	menuItem.Unchecked += showHideChangedHandler;
            	ShowHideCategoriesMenu.Items.Add(menuItem);
            	
            	ShowHideCategoryCheckBox checkBox = new ShowHideCategoryCheckBox(ideaCategory);
            	checkBox.IsChecked = true;
            	checkBox.Checked += showHideChangedHandler;
            	checkBox.Unchecked += showHideChangedHandler;
            	magnetList.showHideCategoriesPanel.Children.Add(checkBox);
            }     
            
    		// Ideally user should save ideas boards to User/Adventure Author/Magnet boards:
			try {
				if (!Directory.Exists(ModuleHelper.MagnetBoardsDirectory)) {
					Directory.CreateDirectory(ModuleHelper.MagnetBoardsDirectory);
				}
			}
			catch (Exception e) {
    			Say.Debug("Failed to create a Magnets board directory for user:\n"+e);
			}    		
    		
    		SetupEventHandlers();
    		
    		magnetList.Open(ModuleHelper.IdeasBoxFilename); // previously was outside constructor
    		    		
    		//added:
    		Toolset.Plugin.SessionWindows.Add(this);
    		
	    	UpdateMagnetBoxAppearsAtSide(); // wait till magnet box is actually open - handled here
	    	// rather than in magnet list cos the layout of this window's grid also changes (would be 
	    	// better to use a dockpanel but couldn't get this to work properly)
			
			ElementHost.EnableModelessKeyboardInterop(this);
        }
        
        
        private static bool addedEventHandlersThatPersistAcrossInstances = false;
        
        private void SetupEventHandlers()
        {
            // Listen for events:                	
        	magnetControl_RequestRemoveHandler = new EventHandler(magnetControl_RequestRemove);
    		magnetControl_PreviewMouseLeftButtonDownHandler = new MouseButtonEventHandler(DragSource_PreviewMouseLeftButtonDown);
    		magnetControl_PreviewMouseMoveHandler = new MouseEventHandler(DragSource_PreviewMouseMove);
    		
            EventHandler titleChangedHandler = new EventHandler(UpdateTitleBar);
            EventHandler<MagnetEventArgs> magnetAddedHandler = new EventHandler<MagnetEventArgs>(magnetAdded); 
            
            ActiveBoard.MagnetAdded += magnetAddedHandler;
            ActiveBoard.Drop += new DragEventHandler(magnetBoardControl_MagnetDropped);   	
            ActiveBoard.Changed += titleChangedHandler; // board became dirty
            ActiveBoard.Opened += titleChangedHandler;
            ActiveBoard.Closed += titleChangedHandler;
            ActiveBoard.MagnetRemoved += new EventHandler<MagnetEventArgs>(magnetBoardMagnetRemoved);
            
            magnetList.MagnetAdded += magnetAddedHandler;
            EventHandler<MagnetCategoryEventArgs> magnetListVisibleMagnetsChangedHandler =
     			new EventHandler<MagnetCategoryEventArgs>(magnetListVisibleMagnetsChanged);     
            magnetList.HidCategory += magnetListVisibleMagnetsChangedHandler;
            magnetList.ShowedCategory += magnetListVisibleMagnetsChangedHandler;   
            magnetList.MagnetTransferredOut += new EventHandler<MagnetEventArgs>(magnetList_MagnetPassedToBoard); 
            magnetList.Drop += new DragEventHandler(magnetList_MagnetDropped);  
            magnetList.Scattered += new EventHandler(magnetList_Scattered);
            
            magnetGotFocusHandler = new RoutedEventHandler(magnetGotFocus);
            magnetLostFocusHandler = new RoutedEventHandler(magnetLostFocus);
            EventHandler<MagnetEventArgs> trackFocusedMagnetHandler = new EventHandler<MagnetEventArgs>(trackFocusedMagnet);
            ActiveBoard.MagnetAdded += trackFocusedMagnetHandler;
            magnetList.MagnetAdded += trackFocusedMagnetHandler;
            
            invalidateUponRotateHandler = new EventHandler(invalidateUponRotate);
                                    
            if (!addedEventHandlersThatPersistAcrossInstances) {
            	Toolset.MagnetSubmitted += toolset_MagnetSubmitted;
            	addedEventHandlersThatPersistAcrossInstances = true;
            }
            
            Loaded += delegate { Log.WriteAction(LogAction.launched,"magnets"); };
            Closing += new CancelEventHandler(magnetBoardViewer_Closing);
            
            Toolset.Plugin.Options.PropertyChanged += new PropertyChangedEventHandler(userPreferencesPropertyChanged);
                 
            Closed += delegate { 
            	instance = null;
            };
        }
            	
    	#endregion
    	
    	#region Methods
    	
    	public void New()
    	{
    		CloseDialog();
    	}
    	
    	
    	public void Open(string filename)
    	{
    		try {
    			ActiveBoard.Open(filename);
    		}
    		catch (ArgumentException e) {
    			Say.Error(filename + " is not a valid magnet board file.",e);
    		}
    		catch (Exception e) {
    			Say.Error("Was unable to open magnet board.",e);
    		}
    	}
    	
    	
    	public void Save()
    	{
    		ActiveBoard.Save();
    		Log.WriteAction(LogAction.saved,"magnetboard",Path.GetFileNameWithoutExtension(ActiveBoard.Filename));
    	}
    	    	
    	
    	private bool SaveDialog() 
    	{    		
    		try {
	    		// get a filename to save to if there isn't one already:
	    		if (ActiveBoard.Filename == null || ActiveBoard.Filename == String.Empty) { 
	    			return SaveAsDialog();
	    		}
	    		else {
		    		Save();
		    		return true;
	    		}
    		}
	    	catch (Exception e) {
	    		Say.Error("Failed to save magnet board.",e);
	    		return false;
	    	}
    	}  
    	
    	
    	private bool SaveAsDialog()
    	{
    		SaveFileDialog saveFileDialog = new SaveFileDialog();
    		saveFileDialog.AddExtension = true;
    		saveFileDialog.CheckPathExists = true;
    		saveFileDialog.DefaultExt = Filters.XML;
    		saveFileDialog.Filter = Filters.XML;
  			saveFileDialog.ValidateNames = true;
  			saveFileDialog.Title = "Select location to save magnet board to";
			if (Directory.Exists(ModuleHelper.MagnetBoardsDirectory)) {
				saveFileDialog.InitialDirectory = ModuleHelper.MagnetBoardsDirectory;
			}	
  			
  			bool ok = (bool)saveFileDialog.ShowDialog();  				
  			if (ok) {
  				ActiveBoard.Filename = saveFileDialog.FileName;
  				UpdateTitleBar(null,null);
  				try {
		  			Save();
		  			return true;
  				}
  				catch (Exception e) {
	    			Say.Error("Failed to save magnet board.",e);
	    			ActiveBoard.Filename = null;
	    			return false;
  				}
  			}
  			else {
  				return false;
  			}
    	}  
    	
    	
    	/// <summary>
    	/// Close the current board, displaying dialogs offering to save/save as the board first if 
    	/// changes have been made.
    	/// </summary>
    	/// <returns>Returns true if the board was closed; false if it was cancelled</returns>
    	private bool CloseDialog()
    	{
    		if (ActiveBoard != null && ActiveBoard.Dirty) {
    			MessageBoxResult result = 
    				MessageBox.Show("Save changes to this board?","Save changes?",MessageBoxButton.YesNoCancel);
    			switch (result) {
    				case MessageBoxResult.Cancel:
    					return false;
    				case MessageBoxResult.Yes:
    					bool cancelled = !SaveDialog(); // user can save, proceed without saving or cancel
    					if (cancelled) {
    						return false;
    					}
    					break;
    				default:
    					break;
    			}    			
    		}
    		ActiveBoard.CloseBoard();
    		return true;
    	}
    	
    	
    	private void UpdateTitleBar(object sender, EventArgs e)
    	{
    		string title;
    		if (ActiveBoard.Filename == null || ActiveBoard.Filename == String.Empty) {
    			if (ActiveBoard.Dirty) {
    				title = "Untitled* - Ideas";
    			}
    			else {
    				title = "Untitled - Ideas";
    			}
    		}
    		else {
    			string filename = Path.GetFileName(ActiveBoard.Filename);
    			if (ActiveBoard.Dirty) {
    				title = filename + "* - Ideas";
    			}
    			else {
    				title = filename + " - Ideas";
    			}
    		}
    		
    		if (Title != title) {
    			Title = title;
    		}
    	}  
        
        
        public void Scatter(bool unusedOnly)
        {
        	if (ActiveBoard != null) {
        		List<MagnetControl> visibleMagnets = magnetList.GetMagnets(true);
	        	foreach (MagnetControl magnet in visibleMagnets) {
        			if (!ActiveBoard.HasEquivalentMagnet(magnet)) {
	        			MagnetControl clone = (MagnetControl)magnet.Clone();
	        			ActiveBoard.AddMagnet(clone,true);
        			}
	        	}        
        	}
        }
        
        
        private void DeleteMagnet(MagnetControl magnet)
        {
        	if (magnet != null) {
        		if (magnetList.HasMagnet(magnet)) {
        			MessageBoxResult result = MessageBox.Show("Permanently delete this idea from the idea box?",
		        			                  "Delete?", 
		        			                  MessageBoxButton.OKCancel,
		        			                  MessageBoxImage.Question,
		        			                  MessageBoxResult.OK,
		        			                  MessageBoxOptions.None);
        			if (result == MessageBoxResult.OK) {
        				magnetList.DeleteMagnet(magnet);
        				if (SelectedMagnet == magnet) {
        					SelectedMagnet = null;
        				}
        			}
        		}
        		else {
        			throw new InvalidOperationException("Cannot delete magnets from anywhere other than " +
        			                                    "the magnet list itself.");
        		}
        	}
        }
        
        
        private void RemoveMagnet(MagnetControl magnet)
        {
        	if (magnet != null) {
        		if (ActiveBoard.HasMagnet(magnet)) {
	         		ActiveBoard.RemoveMagnet(magnet);				        
        			if (SelectedMagnet == magnet) {
        				SelectedMagnet = null;
        			}
    				Log.WriteAction(LogAction.removed,"idea",magnet.ToString());
        		}
        	}        	
        }
        			
    	
    	#endregion
        
    	#region Event handlers
    	 
    	private void OnClick_New(object sender, RoutedEventArgs e)
    	{
    		New();
    		Log.WriteAction(LogAction.added,"magnetboard");
    	}
    	
    	
    	private void OnClick_Open(object sender, RoutedEventArgs e)
        {		
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.ValidateNames = true;
    		openFileDialog.DefaultExt = Filters.XML;
    		openFileDialog.Filter = Filters.XML;
			openFileDialog.Title = "Select a magnet board file to open";
			openFileDialog.Multiselect = false;
			openFileDialog.RestoreDirectory = false;
			if (Directory.Exists(ModuleHelper.MagnetBoardsDirectory)) {
				openFileDialog.InitialDirectory = ModuleHelper.MagnetBoardsDirectory;
			}
			
  			bool ok = (bool)openFileDialog.ShowDialog();  				
  			if (ok) {
  				Open(openFileDialog.FileName);
  			}
    	}
    	
    	
    	private void OnClick_Save(object sender, EventArgs e)
    	{
    		SaveDialog();
    	}
    	
    	
    	private void OnClick_SaveAs(object sender, EventArgs e)
    	{
    		SaveAsDialog();
    	}		
    	 
    	
    	private void OnClick_Close(object sender, RoutedEventArgs e)
    	{
    		CloseDialog();
    	}    		
    	 
    	
    	private void OnClick_Exit(object sender, RoutedEventArgs e)
    	{
    		Close();
    	}   
    	
    	
    	private void OnClick_CreateMagnet(object sender, RoutedEventArgs e)
        {   
    		magnetList.OnClick_CreateMagnet(sender,e);
    	}
        
        
        private void OnClick_Scatter(object sender, RoutedEventArgs e)
        {
        	magnetList.OnClick_Scatter(sender,e);
        }
        
        
        private void OnClick_Sort(object sender, RoutedEventArgs e)
        {
        	magnetList.Sort();
        }
        
        
        private void previewKeyDown(object sender, KeyEventArgs e)
        {
        	//Say.Debug(e.Key.ToString() + " is down");
        	if (SelectedMagnet != null) {
        		if (e.Key == Key.Delete) {
        			if (ActiveBoard.HasMagnet(SelectedMagnet)) {
        				RemoveMagnet(SelectedMagnet);
        				e.Handled = true;
        			}
        			else if (magnetList.HasMagnet(SelectedMagnet)) {
        				DeleteMagnet(SelectedMagnet);
        				e.Handled = true;
        			}
        		}
        		else if (ActiveBoard.HasMagnet(SelectedMagnet)) { // only perform these operations on board magnets     
        			//Say.Debug("..so do stuff");
        			bool shift = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
        			bool down = Keyboard.IsKeyDown(Key.Down);
        			bool up = Keyboard.IsKeyDown(Key.Up);
        			bool left = Keyboard.IsKeyDown(Key.Left);
        			bool right = Keyboard.IsKeyDown(Key.Right);          			
        			
        			double xMovement = 0; // if both down & up are pressed this will amount to 0
        			double yMovement = 0; // if both right & left are pressed this will amount to 0
        			
        			double step = 1;
        			
        			if (up) {
        				yMovement -= step;
        				//Say.Debug("up: yMovement: " + yMovement);
        			}
        			if (down) {
        				yMovement += step;
        				//Say.Debug("dn: yMovement: " + yMovement);
        			}
        			if (left) {
        				xMovement -= step;
        				//Say.Debug("lt: xMovement: " + xMovement);
        			}
        			if (right) {
        				xMovement += step;
        				//Say.Debug("rt: xMovement: " + xMovement);
        			}
        			
        			if (shift) { 
        				SelectedMagnet.RotateBy(xMovement * MagnetControl.DEGREES_TO_ROTATE,
		        				                MagnetBoardControl.MAXIMUM_ANGLE_IN_EITHER_DIRECTION);
        			}
        			else {
        				SelectedMagnet.Move(xMovement,yMovement);
        			}
        			e.Handled = true;        			
        		}
        	}    	
        }
        
        
        private void OnClick_ClearBoard(object sender, EventArgs e)
        {
			MessageBoxResult result = MessageBox.Show("Clear entire board?",
		        					                  "Clear?", 
		        					                  MessageBoxButton.YesNo,
		        					                  MessageBoxImage.Question,
		        					                  MessageBoxResult.No,
		        					                  MessageBoxOptions.None);
    		if (result == MessageBoxResult.Yes) {
        		ActiveBoard.ClearBoard();
    		}
        }

        
        private void CategoryElementIsCheckedChanged(object sender, RoutedEventArgs e)
        {
        	IdeaCategory category;
        	bool isChecked;
        	if (e.Source is ShowHideCategoryMenuItem) {
        		ShowHideCategoryMenuItem menuItem = (ShowHideCategoryMenuItem)e.Source;
        		category = menuItem.Category;
        		isChecked = menuItem.IsChecked;
        	}
        	else if (e.Source is ShowHideCategoryCheckBox) {
        		ShowHideCategoryCheckBox checkBox = (ShowHideCategoryCheckBox)e.Source;
        		category = checkBox.Category;
        		isChecked = (bool)checkBox.IsChecked;
        	}
        	else {
        		throw new ArgumentException("Did not recognise show/hide category element (" + e.Source + ")");
        	}
        	
        	if (magnetList.CategoryIsVisible(category) && !isChecked) {
        		magnetList.HideCategory(category,false);
        	}
        	else if (!magnetList.CategoryIsVisible(category) && isChecked) {
        		magnetList.ShowCategory(category,false);
        	}
        }
        
        
        private void useWonkyMagnetsCheckedOrUnchecked(object sender, RoutedEventArgs e)
        {
        	Toolset.Plugin.Options.UseWonkyMagnets = wonkyMagnetsMenuItem.IsChecked;
        }
        
        
        private void appearsAtSideCheckedOrUnchecked(object sender, EventArgs e)
        {
        	Toolset.Plugin.Options.MagnetBoxAppearsAtSide = appearsAtSideMenuItem.IsChecked;
        }       

        
        /// <summary>
        /// Add handlers to respond to drag-drop attempts, and to deal with magnets requesting removal/deletion.
        /// </summary>
        private void magnetAdded(object sender, MagnetEventArgs e)
        {           
	        // If this magnet was created on the list, added to the board, edited on the board,
	        // and dropped back into the list, this will be the second time it has been 'added'.
	        // To avoid duplicate handlers being attached, remove and then add the handler object.
	        // (If the handler isn't already attached, no exception is raised, so you always end up with 1 handler.)
	        e.Magnet.PreviewMouseLeftButtonDown -= magnetControl_PreviewMouseLeftButtonDownHandler;
	        e.Magnet.PreviewMouseLeftButtonDown += magnetControl_PreviewMouseLeftButtonDownHandler;
	        e.Magnet.PreviewMouseMove -= magnetControl_PreviewMouseMoveHandler;
	        e.Magnet.PreviewMouseMove += magnetControl_PreviewMouseMoveHandler;
	        e.Magnet.RequestRemove -= magnetControl_RequestRemoveHandler;
	        e.Magnet.RequestRemove += magnetControl_RequestRemoveHandler;
        }

        
        /// <summary>
        /// When the magnet list indicates that a magnet should be removed from it and transferred
        /// to the active magnet board, create a clone of that magnet and add the clone to the 
        /// magnet board.
        /// </summary>
        private void magnetList_MagnetPassedToBoard(object sender, MagnetEventArgs e)
        {
        	MagnetControl magnet = (MagnetControl)e.Magnet.Clone();
        	ActiveBoard.AddMagnet(magnet,true);	
        	Log.WriteAction(LogAction.placed,"idea",e.Magnet.ToString());
        }
        
        
        /// <summary>
        /// When a magnet is removed from the board, and an equivalent magnet does
        /// not appear on the magnet list, add the removed magnet to the magnet list.
        /// If the transferred magnet was the selected magnet, deselect it.
        /// </summary>
        private void magnetBoardMagnetRemoved(object sender, MagnetEventArgs e)
        {
        	if (!magnetList.HasEquivalentMagnet(e.Magnet)) {
        		magnetList.AddMagnet(e.Magnet,true);	
        	}
        	if (SelectedMagnet == e.Magnet) {
        		SelectedMagnet = null;
        	}
        }
        
        
        /// <summary>
        /// Ensure that every magnet that's added to the board is watched for focus events,
        /// so that we always know which magnet is currently selected. By removing and then
        /// adding the appropriate handlers, we ensure they end up with exactly one.
        /// </summary>
        private void trackFocusedMagnet(object sender, MagnetEventArgs e)
        {
           e.Magnet.GotFocus -= magnetGotFocusHandler;
           e.Magnet.GotFocus += magnetGotFocusHandler;
           e.Magnet.LostFocus -= magnetLostFocusHandler;
           e.Magnet.LostFocus += magnetLostFocusHandler;            
        }
                
        /// <summary>
        /// To lock the assignment of SelectedMagnet when you need to check
        /// the value of SelectedMagnet first.
        /// </summary>
        private object padlock = new object();
        
        /// <summary>
        /// If any magnet gets focus, it becomes the selected magnet.
        /// </summary>
        private void magnetGotFocus(object sender, EventArgs e)
        {
        	SelectedMagnet = (MagnetControl)sender;
        }
        
        
        /// <summary>
        /// If the selected magnet loses focus, it is no longer the selected magnet.
        /// </summary>
        private void magnetLostFocus(object sender, EventArgs e)
        {
        	lock (padlock) { // check that a newly focused magnet has not assigned SelectedMagnet already
        		if (SelectedMagnet == (MagnetControl)sender) {
        			SelectedMagnet = null;
        		}
        	}
        }    
        
        
        /// <summary>
        /// Rotate the selected magnet with the mouse wheel, if one exists.
        /// </summary>
        private void magnetBoardViewerMouseWheel(object sender, MouseWheelEventArgs e)
        {
        	double offset = -(e.Delta / 60);
        	if (SelectedMagnet != null && ActiveBoard.HasMagnet(SelectedMagnet)) {
        		SelectedMagnet.RotateBy(offset,MagnetBoardControl.MAXIMUM_ANGLE_IN_EITHER_DIRECTION);
        	}
        } 
        
        
        /// <summary>
        /// Invalidate the visuals after a magnet has been rotated, to account for the 
        /// dashed selection lines not updating. 
        /// </summary>
        /// <remarks>This is only necessary for the selected magnet, and only then
        /// if it was selected using tab.</remarks>
        private void invalidateUponRotate(object sender, EventArgs e)
        {
        	if (SelectedMagnet == (MagnetControl)sender) {
        		((MagnetControl)sender).InvalidateArrange();        		
        	}
        }
        
                
	    private void magnetBoardControl_MagnetDropped(object sender, DragEventArgs e)
	    {
	         IDataObject data = e.Data;
	
	         if (data.GetDataPresent(typeof(MagnetControlDataObject))) {
	         	MagnetControlDataObject dataObject = (MagnetControlDataObject)data.GetData(typeof(MagnetControlDataObject));
	         	MagnetControl magnet = dataObject.Magnet;
	         	
	         	bool fromMagnetList = !ActiveBoard.HasMagnet(magnet);
	         	
//	         	if (fromMagnetList && ActiveBoard.HasEquivalentMagnet(magnet)) {
//	         		Say.Information("Adding another copy of this magnet to the board.");
//	         	}
	         		         	
	         	if (fromMagnetList) { // copy the magnet from the magnetlist
	         		magnet = (MagnetControl)magnet.Clone();
	         	}
	         	
	         	// Set the magnet's new position:
	         	Point drop = e.GetPosition(ActiveBoard);
	         	Size size = dataObject.ActualRotatedSize;
	         	drop.X -= size.Width / 2;  // this info is not otherwise available from the magnet
		        drop.Y -= size.Height / 2; // once it is being dragged, as it has no size
//	         	drop.X -= dataObject.MagnetWidth / 2;  // this info is not otherwise available from the magnet
//		        drop.Y -= dataObject.MagnetHeight / 2; // once it is being dragged, as it has no size
	         	magnet.X = drop.X;	  				   // when not placed on the interface.
	         	magnet.Y = drop.Y;
	         	
	         	if (fromMagnetList) {
	         		ActiveBoard.AddMagnet(magnet,false);
	         		magnet.Focus(); // select the clone rather than the original
	         		Log.WriteAction(LogAction.placed,"idea",magnet.ToString());
	         	}
	         	else {
	         		Log.WriteAction(LogAction.moved,"idea",magnet.ToString());
	         	}
	         		
	         	ActiveBoard.BringToFront(magnet);
	    	}	
	    } 
        

        private void magnetList_MagnetDropped(object sender, DragEventArgs e)
        {
	         IDataObject data = e.Data;
	
	         if (data.GetDataPresent(typeof(MagnetControlDataObject))) {
	         	MagnetControlDataObject dataObject = (MagnetControlDataObject)data.GetData(typeof(MagnetControlDataObject));
	         	
	         	if (ActiveBoard.HasMagnet(dataObject.Magnet)) {
	         		ActiveBoard.RemoveMagnet(dataObject.Magnet);
    				Log.WriteAction(LogAction.removed,"idea",dataObject.Magnet.ToString());
	         	}
	         }	
        }
        

        private void toolset_MagnetSubmitted(object sender, MagnetEventArgs e)
        {
			try {
        		// full reference to Instance required, or you don't see magnets
        		// that are sent from the outside the magnet viewer until it's
        		// closed and opened again:
				MagnetBoardViewer.Instance.magnetList.AddMagnet(e.Magnet,true);			   
			}
			catch (Exception ex) {
				Say.Error(ex.ToString());
			}   	
        }
        
        
        /// <summary>
        /// When an idea category is shown/hidden, update the UI elements which allows you to show/hide categories.
        /// Also check the selected magnet has not been hidden, and nullify it if it has.
        /// </summary>
        private void magnetListVisibleMagnetsChanged(object sender, MagnetCategoryEventArgs ea)
        {        	
        	// Check the selected magnet has not been hidden, and nullify it if it has:
        	lock (padlock) {
	        	if (SelectedMagnet != null && !SelectedMagnet.IsVisible) {
	        		SelectedMagnet = null;
	        	}
        	}
        	
        	foreach (ShowHideCategoryMenuItem menuItem in ShowHideCategoriesMenu.Items) {
        		if (menuItem.IsChecked != magnetList.CategoryIsVisible(menuItem.Category)) {
        			menuItem.IsChecked = magnetList.CategoryIsVisible(menuItem.Category);
        		}
        	}
        	foreach (ShowHideCategoryCheckBox checkBox in magnetList.showHideCategoriesPanel.Children) {
        		if (checkBox.IsChecked != magnetList.CategoryIsVisible(checkBox.Category)) {
        			checkBox.IsChecked = magnetList.CategoryIsVisible(checkBox.Category);
        		}
        	}
        }        
        

        private void magnetList_Scattered(object sender, EventArgs e)
        {
        	Scatter(true);
        }

        
        private void userPreferencesPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        	if (e.PropertyName == "MagnetBoxAppearsAtSide") {
        		UpdateMagnetBoxAppearsAtSide();
        		if (appearsAtSideMenuItem.IsChecked != Toolset.Plugin.Options.MagnetBoxAppearsAtSide) {
        			appearsAtSideMenuItem.IsChecked = Toolset.Plugin.Options.MagnetBoxAppearsAtSide;
        		}
        	} 	
        	if (e.PropertyName == "UseWonkyMagnets") { // magnet list takes care of the actual change
        		if (wonkyMagnetsMenuItem.IsChecked != Toolset.Plugin.Options.UseWonkyMagnets) {
        			wonkyMagnetsMenuItem.IsChecked = Toolset.Plugin.Options.UseWonkyMagnets;
        		}
        	}
        }
        
        
        internal void UpdateMagnetBoxAppearsAtSide()
        {
        	if (Toolset.Plugin.Options.MagnetBoxAppearsAtSide) {
		       	magnetList.Orientation = Orientation.Vertical;	        
			    Grid.SetRowSpan(magneticSurface,2);
			    Grid.SetColumnSpan(magneticSurface,1);
			    Grid.SetRow(magnetList,1);
			    Grid.SetRowSpan(magnetList,2);
			    Grid.SetColumn(magnetList,1);
			    Grid.SetColumnSpan(magnetList,1);
        	}
        	else {
		        magnetList.Orientation = Orientation.Horizontal;	        
			    Grid.SetRowSpan(magneticSurface,1);
			    Grid.SetColumnSpan(magneticSurface,2);
			    Grid.SetRow(magnetList,2);
			    Grid.SetRowSpan(magnetList,1);
			    Grid.SetColumn(magnetList,0);
			    Grid.SetColumnSpan(magnetList,2);
        	}
        }
        
        
        private void OnClick_ChangeBoardColour(object sender, EventArgs e)
        {        	
        	ColorPickerDialog colorPicker = new ColorPickerDialog();
        	colorPicker.StartingColor = ActiveBoard.SurfaceColour;
        	bool ok = (bool)colorPicker.ShowDialog();
        	
        	if (ok) {
        		ActiveBoard.SurfaceColour = colorPicker.SelectedColor;
        	}
        }
        
        
        private void magnetBoardViewer_Closing(object sender, CancelEventArgs e)
        {        	
			if (!CloseDialog()) {
				e.Cancel = true;
			}
        	else {
        		try {
        			Log.WriteAction(LogAction.exited,"magnets");
        		}
        		catch (Exception) {
        			// already disposed because the toolset is closing
        		}
        	}
        }
        
        
        private void magnetControl_RequestRemove(object sender, EventArgs e)
        {
        	MagnetControl magnet = (MagnetControl)sender;
        	if (ActiveBoard.HasMagnet(magnet)) {
        		RemoveMagnet(magnet);
        	}
        	else if (magnetList.HasMagnet(magnet)) {
        		DeleteMagnet(magnet);
        	}
        }
        
        #endregion
        
        
        #region Drag-drop

        private void DragSource_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDownOnMagnet && e.LeftButton == MouseButtonState.Pressed && !IsDragging)
            {
                Point position = e.GetPosition(null);

                if (Math.Abs(position.X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(position.Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    // StartDrag(e);
                   //  StartDragCustomCursor(e);
                   // StartDragWindow(e);
                     StartDragInProcAdorner(e); 

                }
            }   
        }
        
        
        private bool IsDraggable(object o)
        {
        	return o is MagnetControl;
        }
                
        
        bool mouseDownOnMagnet = false;

        
        private void DragSource_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        	_startPoint = e.GetPosition(null); // must move some distance from this point before dragging commences
        	mouseDownOnMagnet = IsDraggable(e.Source);
        }
        

        private void DragSource_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            e.UseDefaultCursors = false;
            e.Handled = true;

            #region Redundant
            
//            System.Diagnostics.Debug.WriteLine("DragSource_GiveFeedback " + e.Effects.ToString());
//
//            if (this.DragScope == null)
//            {
//                try
//                {
//                    //This loads the cursor from a stream .. 
//                    if (_allOpsCursor == null)
//                    {                        
//                        using (Stream cursorStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("SimplestDragDrop.DDIcon.cur"))
//                        {
//                            _allOpsCursor = new Cursor(cursorStream);
//                        } 
//                    }
//                    Mouse.SetCursor(_allOpsCursor);
//
//                    e.UseDefaultCursors = false;
//                    e.Handled = true;
//                }
//                finally { }
//            }
//            else  // This code is called when we are using a custom cursor  (either a window or adorner ) 
//            {
//                 
//                e.UseDefaultCursors = false;
//                e.Handled = true;
//            }

			#endregion
        }


        private DragAdorner _adorner = null;
        private AdornerLayer _layer;



        private FrameworkElement _dragScope;
        public FrameworkElement DragScope {
            get { return _dragScope; }
            set { _dragScope = value; }
        }


        private void StartDragInProcAdorner(MouseEventArgs e)
        {
        	if (!(e.Source is MagnetControl)) {
        		Say.Error("e.Source of wrong type, returning: " + e.Source.GetType().ToString());
        		return;
        	}
        	        	
            // Let's define our DragScope .. In this case it is every thing inside our main window .. 
            //DragScope = Application.Current.MainWindow.Content as FrameworkElement;
            DragScope = this.Content as FrameworkElement;
            System.Diagnostics.Debug.Assert(DragScope != null);

            // We enable Drag & Drop in our scope ...  We are not implementing Drop, so it is OK, but this allows us to get DragOver 
            bool previousDrop = DragScope.AllowDrop;
            DragScope.AllowDrop = true;            

            // Let's wire our usual events.. 
            // GiveFeedback just tells it to use no standard cursors..  

            GiveFeedbackEventHandler feedbackhandler = new GiveFeedbackEventHandler(DragSource_GiveFeedback);
            //this.DragSource.GiveFeedback += feedbackhandler;
            this.GiveFeedback += feedbackhandler;

            // The DragOver event ... 
            DragEventHandler draghandler = new DragEventHandler(Window1_DragOver);
            DragScope.PreviewDragOver += draghandler; 

            // Drag Leave is optional, but write up explains why I like it .. 
            DragEventHandler dragleavehandler = new DragEventHandler(DragScope_DragLeave);
            DragScope.DragLeave += dragleavehandler; 

            // QueryContinue Drag goes with drag leave... 
            QueryContinueDragEventHandler queryhandler = new QueryContinueDragEventHandler(DragScope_QueryContinueDrag);
            DragScope.QueryContinueDrag += queryhandler; 

            //Here we create our adorner..
            
            // Cancel the effects when moving, otherwise it's too slow. Note that this means you lose
            // the bevelling effect, but it's too sluggish otherwise.
            MagnetControl magnet = (MagnetControl)e.Source;
            BitmapEffect fx = magnet.BitmapEffect;
            magnet.BitmapEffect = null;
            
            _adorner = new DragAdorner(DragScope, magnet, true, 0.5);
            _layer = AdornerLayer.GetAdornerLayer(DragScope as Visual);
            _layer.Add(_adorner);


            IsDragging = true;
            _dragHasLeftScope = false; 
            //Finally lets drag drop 
            
        	MagnetControlDataObject data = new MagnetControlDataObject();
        	data.Magnet = magnet;
        	data.MagnetHeight = magnet.ActualHeight;
        	data.MagnetWidth = magnet.ActualWidth;
        	data.ActualRotatedSize = magnet.GetRotatedSize();
        	       	
        	
        	
        	DataObject dataObject = new DataObject(typeof(MagnetControlDataObject),data);


            DragDropEffects de = DragDrop.DoDragDrop(this, dataObject, DragDropEffects.Move);
                        
            
             // Clean up our mess :) 
             magnet.BitmapEffect = fx;
            DragScope.AllowDrop = previousDrop;
            AdornerLayer.GetAdornerLayer(DragScope).Remove(_adorner);
            _adorner = null;

            this.GiveFeedback -= feedbackhandler;
            DragScope.DragLeave -= dragleavehandler;
            DragScope.QueryContinueDrag -= queryhandler;
            DragScope.PreviewDragOver -= draghandler;  

            mouseDownOnMagnet = false;
            
            IsDragging = false;
        }
        

        private bool _dragHasLeftScope = false; 
        private void DragScope_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (this._dragHasLeftScope)
            {
                e.Action = DragAction.Cancel;
                e.Handled = true; 
            }
            
        }


        private void DragScope_DragLeave(object sender, DragEventArgs e)
        {
            if (e.OriginalSource == DragScope)
            {
                Point p = e.GetPosition(DragScope);
                Rect r = VisualTreeHelper.GetContentBounds(DragScope);
                if (!r.Contains(p))
                {
                    this._dragHasLeftScope = true;
                    e.Handled = true;
                }
            }

        } 




        private void Window1_DragOver(object sender, DragEventArgs args)
        {
            if (_adorner != null)
            {
                _adorner.LeftOffset = args.GetPosition(DragScope).X /* - _startPoint.X */ ;
                _adorner.TopOffset = args.GetPosition(DragScope).Y /* - _startPoint.Y */ ;
            }
        }

    

        private Point _startPoint;
        private bool _isDragging;

        public bool IsDragging
        {
            get { return _isDragging; }
            set { _isDragging = value; }
        } 

        
        #endregion 

    }
}