using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Documents;
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
			get { return instance; }
			set { instance = value; }
		}   
    	    	
   
    	public MagnetBoardControl ActiveBoard {
    		get {
    			return magneticSurface;
    		}
    	}
    	
    	
    	private MagnetControl selectedMagnet;     	
		public MagnetControl SelectedMagnet {
			get { return selectedMagnet; }
			set {
	    		if (selectedMagnet != value) {
					if (selectedMagnet != null) {
		    			selectedMagnet.DeselectFX();
					}
		    		selectedMagnet = value;
		    		if (selectedMagnet != null) {
		    			selectedMagnet.SelectFX();
		    		}
	    		}
			}
    	}
    	
    	
    	private EventHandler<MagnetEventArgs> magnetControl_RequestRemoveHandler;
    	private MouseButtonEventHandler magnetControl_PreviewMouseLeftButtonDownHandler;
    	private MouseEventHandler magnetControl_PreviewMouseMoveHandler;
    	
    	#endregion
    	
    	#region Constructors
    	
        public MagnetBoardViewer()
        {
        	magnetControl_RequestRemoveHandler = new EventHandler<MagnetEventArgs>(magnetControl_RequestRemove);
    		magnetControl_PreviewMouseLeftButtonDownHandler = new MouseButtonEventHandler(DragSource_PreviewMouseLeftButtonDown);
    		magnetControl_PreviewMouseMoveHandler = new MouseEventHandler(DragSource_PreviewMouseMove);
    	
        	InitializeComponent();
                        
            IdeaEntryBox.MaxLength = Idea.MAX_IDEA_LENGTH;
            
            // Set up 'Add idea' box and 'Show/Hide idea category' menu:
            IdeaCategoryComboBox.ItemsSource = Idea.IDEA_CATEGORIES;            
            RoutedEventHandler showHideChangedHandler = new RoutedEventHandler(CategoryElementIsCheckedChanged);
            
            foreach (IdeaCategory ideaCategory in Idea.IDEA_CATEGORIES) {
            	string category = ideaCategory.ToString();
            	
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
            
            // Listen for events:            
            EventHandler titleChangedHandler = new EventHandler(UpdateTitleBar);
            EventHandler<MagnetEventArgs> magnetAddedHandler 
            	= new EventHandler<MagnetEventArgs>(magnetAdded); 
            
            ActiveBoard.MagnetAdded += magnetAddedHandler;
            ActiveBoard.Drop += new DragEventHandler(magnetBoardControl_MagnetDropped);   	
            ActiveBoard.MagnetSelected += new EventHandler<MagnetEventArgs>(MagnetSelected);
            ActiveBoard.Changed += titleChangedHandler; // board became dirty
            ActiveBoard.Opened += titleChangedHandler;
            ActiveBoard.Closed += titleChangedHandler;
            ActiveBoard.Closed += delegate { 
            	IdeaEntryBox.Text = String.Empty; 
            	IdeaCategoryComboBox.SelectedValue = null;
            };
            ActiveBoard.MagnetTransferredOut += new EventHandler<MagnetEventArgs>(ActiveBoard_MagnetPassedToList);
            
            magnetList.MagnetAdded += magnetAddedHandler;
            magnetList.VisibleMagnetsChanged += new EventHandler(VisibleMagnetsChanged);              
            magnetList.MagnetSelected += new EventHandler<MagnetEventArgs>(MagnetSelected);
            magnetList.MagnetTransferredOut += new EventHandler<MagnetEventArgs>(magnetList_MagnetPassedToBoard); 
            magnetList.Drop += new DragEventHandler(magnetList_MagnetDropped);  
            magnetList.Scattered += new EventHandler(magnetList_Scattered);
            
            Toolset.IdeaSubmitted += new EventHandler<IdeaEventArgs>(Toolset_IdeaSubmitted);
            
            Loaded += delegate { Log.WriteAction(LogAction.launched,"magnets"); };
            Closing += new CancelEventHandler(magnetBoardViewer_Closing);
                       
            wonkyMagnetsMenuItem.IsChecked = false;
                        
    		// Ideally user should save ideas boards to User/Adventure Author/Magnet boards:
			try {
				if (!Directory.Exists(ModuleHelper.MagnetBoardsDirectory)) {
					Directory.CreateDirectory(ModuleHelper.MagnetBoardsDirectory);
				}
			}
			catch (Exception e) {
    			Say.Debug("Failed to create a Magnets board directory for user:\n"+e);
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
	        	foreach (MagnetControl magnet in magnetList.GetMagnets(true)) {
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
        					magnet = null;
        				}
        			}
        		}
        		else if (ActiveBoard.HasMagnet(magnet)) {
        			ActiveBoard.DeleteMagnet(magnet);
        			if (SelectedMagnet == magnet) {
        				magnet = null;
        			}
        		}
        	}        	
        }
        			
    	
    	#endregion
        
    	#region Event handlers
    	 
    	private void OnClick_New(object sender, RoutedEventArgs e)
    	{
    		New();
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
    	
    	
        private void OnClick_AddMagnet(object sender, RoutedEventArgs e)
        {        	
        	if (IdeaEntryBox.Text != String.Empty) {
	        	Idea idea;
	        	IdeaCategory category;
	        	if (IdeaCategoryComboBox.SelectedItem != null) {
	        		category = (IdeaCategory)IdeaCategoryComboBox.SelectedItem;
	        	}
	        	else {
	        		category = IdeaCategory.Other;
	        	}
	        	idea = new Idea(IdeaEntryBox.Text,category,User.GetCurrentUser());
	        	magnetList.AddIdea(idea);
        	}
        }
        
        
        private void OnClick_Scatter(object sender, RoutedEventArgs e)
        {
        	MessageBoxResult result = MessageBox.Show("Tip all your magnets onto the board?",
		        					                  "Scatter magnets?", 
		        					                  MessageBoxButton.OKCancel,
		        					                  MessageBoxImage.Question,
		        					                  MessageBoxResult.Cancel,
		        					                  MessageBoxOptions.None);
        	if (result == MessageBoxResult.OK) {
        		magnetList.Scatter();
        	}
        }
        
        
        private void OnClick_Sort(object sender, RoutedEventArgs e)
        {
        	magnetList.Sort();
        }
        
        
        private void MagnetSelected(object sender, MagnetEventArgs e)
        {
        	SelectedMagnet = e.Magnet;
        }

        
        private void KeyPressed(object sender, KeyEventArgs e)
        {
        	switch (e.Key) {
        		case Key.Delete:
        			DeleteMagnet(SelectedMagnet);
        			break;
        		case Key.Up:
        			if (SelectedMagnet != null && ActiveBoard.HasMagnet(SelectedMagnet)) {
        				SelectedMagnet.Move(0,-1);
        			}
        			break;
        		case Key.Down:
        			if (SelectedMagnet != null && ActiveBoard.HasMagnet(SelectedMagnet)) {
        				SelectedMagnet.Move(0,1);
        			}
        			break;
        		case Key.Left:
        			if (SelectedMagnet != null && ActiveBoard.HasMagnet(SelectedMagnet)) {
        				if (Keyboard.Modifiers == ModifierKeys.Shift) {
        					SelectedMagnet.RotateLeft();
        				}
        				else {
        					SelectedMagnet.Move(-1,0);
        				}
        			}
        			break;
        		case Key.Right:
        			if (SelectedMagnet != null && ActiveBoard.HasMagnet(SelectedMagnet)) {
        				if (Keyboard.Modifiers == ModifierKeys.Shift) {
        					SelectedMagnet.RotateRight();
        				}
        				else {
        					SelectedMagnet.Move(1,0);
        				}
        			}
        			break;
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
        		List<MagnetControl> magnets = ActiveBoard.GetMagnets();
        		foreach (MagnetControl magnet in magnets) {
        			ActiveBoard.DeleteMagnet(magnet);
        		}
    		}
        }
                
        
        private void OnClick_RotateLeft(object sender, EventArgs e) 
        {
        	if (SelectedMagnet != null && ActiveBoard.HasMagnet(SelectedMagnet)) {
        		SelectedMagnet.Angle -= MagnetControl.DEGREES_TO_ROTATE;
        	}
        }
        
        
        private void OnClick_RotateRight(object sender, EventArgs e) 
        {
        	if (SelectedMagnet != null && ActiveBoard.HasMagnet(SelectedMagnet)) {
        		SelectedMagnet.Angle += MagnetControl.DEGREES_TO_ROTATE;
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
        
        
        private void useWonkyMagnetsChecked(object sender, RoutedEventArgs e)
        {
        	magnetList.UseWonkyMagnets = true;
        }
        
        
        private void useWonkyMagnetsUnchecked(object sender, RoutedEventArgs e)
        {
        	magnetList.UseWonkyMagnets = false;
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
        }
        
        
        /// <summary>
        /// When the magnet board indicates that a magnet should be removed from it and transferred
        /// to the magnet list (this would happen where an equivalent magnet did not exist on the 
        /// magnet list already), delete the magnet from the active board and add it to the list.
        /// If the transferred magnet was the selected magnet, deselect it.
        /// </summary>
        private void ActiveBoard_MagnetPassedToList(object sender, MagnetEventArgs e)
        {
        	ActiveBoard.DeleteMagnet(e.Magnet);
        	magnetList.AddMagnet(e.Magnet,true);	
        	if (SelectedMagnet == e.Magnet) {
        		SelectedMagnet = null;
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
	         	
	         	// Set the magnet's new position - account for the position of the mouse when
	         	// clicking on the magnet (and the angle of the magnet's rotation?):
	         	Point drop = e.GetPosition(this);
	         	drop.X -= magnet.DesiredSize.Width / 2;
	         	drop.Y -= magnet.DesiredSize.Height / 2;
	         	magnet.X = drop.X;
	         	magnet.Y = drop.Y;
	         	
	         	if (fromMagnetList) {
	         		ActiveBoard.AddMagnet(magnet,false);
	         		SelectedMagnet = magnet; // select the clone rather than the original
	         	}
	         		
	         	ActiveBoard.BringToFront(magnet);
	    	}	
	    } 
        

        private void magnetList_MagnetDropped(object sender, DragEventArgs e)
        {
	         IDataObject data = e.Data;
	
	         if (data.GetDataPresent(typeof(MagnetControlDataObject))) {
	         	MagnetControlDataObject dataObject = (MagnetControlDataObject)data.GetData(typeof(MagnetControlDataObject));
	         	
	         	// If this magnet is being put back in the idea box, remove it from the board.
		        // If the list doesn't already have this magnet (i.e. it has been edited while on the
		        // board) tell the list to transfer it in.
	         	if (ActiveBoard.HasMagnet(dataObject.Magnet)) {
	         		if (!magnetList.HasEquivalentMagnet(dataObject.Magnet)) {
	         			ActiveBoard.TransferMagnet(dataObject.Magnet);
	         		}	         		
	         		else {
	         			ActiveBoard.DeleteMagnet(dataObject.Magnet);	         			
	         		}
	         	}
	         }	
        }
        

        private void Toolset_IdeaSubmitted(object sender, IdeaEventArgs e)
        {
        	MagnetControl magnet = new MagnetControl(e.Idea);
        	magnetList.AddMagnet(magnet,true);
        }
        
        
        /// <summary>
        /// When an idea category is shown/hidden, update the UI elements which allows you to show/hide categories.
        /// Also check the selected magnet has not been hidden, and nullify it if it has.
        /// </summary>
        private void VisibleMagnetsChanged(object sender, EventArgs ea)
        {        	
        	// Check the selected magnet has not been hidden, and nullify it if it has:
        	if (SelectedMagnet != null && !SelectedMagnet.IsVisible) {
        		SelectedMagnet = null;
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
        
        
        private void magnetControl_RequestRemove(object sender, MagnetEventArgs e)
        {
        	DeleteMagnet(e.Magnet);
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
        	_startPoint = e.GetPosition(null);
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
            
            _adorner = new DragAdorner(DragScope,(UIElement)magnet, true, 0.5);
            _layer = AdornerLayer.GetAdornerLayer(DragScope as Visual);
            _layer.Add(_adorner);


            IsDragging = true;
            _dragHasLeftScope = false; 
            //Finally lets drag drop 
            
        	MagnetControlDataObject data = new MagnetControlDataObject();
        	data.Magnet = magnet;
        	
//        	 Temporarily undo the rotation on this magnet, in order to correctly measure the
//        	 offset between the magnet's location (at its top-left corner) and the mouse click position:
//        	if (magnet.Angle == 0) {	        	
//	        	data.Offset = e.GetPosition(magnet);
//        	}
//        	else {
//	        	double origAngle = magnet.Angle;
//	        	magnet.Angle = 0;        	
//	        	data.Offset = e.GetPosition(magnet);
//	        	magnet.Angle = origAngle;
//        	}
        	
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