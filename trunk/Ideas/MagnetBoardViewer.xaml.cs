using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Markup;
using System.Windows.Media.Effects;
using System.IO;
using AdventureAuthor.Utils;
using AdventureAuthor.Setup;
using Microsoft.Win32;

namespace AdventureAuthor.Ideas
{
    /// <summary>
    /// Interaction logic for MagnetBoardViewer.xaml
    /// </summary>

    public partial class MagnetBoardViewer : Window
    {    
    	#region Fields  
   
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
    	
    	
    	private EventHandler<MagnetEventArgs> magnetAddedHandler;
    	
    	#endregion
    	
    	#region Events
    	
    	
    	#endregion
	
    	#region Constructors
    	
        public MagnetBoardViewer()
        {        	
            InitializeComponent();  
            IdeaEntryBox.MaxLength = Idea.MAX_IDEA_LENGTH;
            
            // Set up 'Add idea' box and 'Show/Hide idea category' menu:
            IdeaCategoryComboBox.ItemsSource = Idea.IDEA_CATEGORIES;            
            foreach (IdeaCategory ideaCategory in Idea.IDEA_CATEGORIES) {
            	RoutedEventHandler showHideChangedHandler = new RoutedEventHandler(CategoryElementIsCheckedChanged);
            	string category = ideaCategory.ToString();
            	string header = "Show " + category + " ideas";
            	
            	ShowHideCategoryMenuItem menuItem = new ShowHideCategoryMenuItem(ideaCategory);
            	menuItem.Header = header;
            	menuItem.IsCheckable = true;
            	menuItem.IsChecked = true;
            	menuItem.Checked += showHideChangedHandler;
            	menuItem.Unchecked += showHideChangedHandler;
            	ShowHideCategoriesMenu.Items.Add(menuItem);
            	
            	ShowHideCategoryCheckBox checkBox = new ShowHideCategoryCheckBox(ideaCategory);
            	checkBox.Content = header;
            	checkBox.IsChecked = true;
            	checkBox.Checked += showHideChangedHandler;
            	checkBox.Unchecked += showHideChangedHandler;
            	magnetList.showHideCategoriesPanel.Children.Add(checkBox);
            }          
            
            // Listen for events:
            
            magnetAddedHandler = new EventHandler<MagnetEventArgs>(magnetAdded);
            
            AddMagnetBoardHandlers(ActiveBoard); // this gets its own method in case we later have multiple boards
            
            magnetList.VisibleMagnetsChanged += new EventHandler(VisibleMagnetsChanged);              
            magnetList.MagnetSelected += new EventHandler<MagnetEventArgs>(MagnetSelected);
            magnetList.MagnetRemoved += new EventHandler<MagnetRemovedEventArgs>(magnetList_MagnetRemoved); 
            magnetList.Drop += new DragEventHandler(magnetList_MagnetDropped);  
            magnetList.MagnetAdded += magnetAddedHandler;
            
            Toolset.IdeaSubmitted += new EventHandler<IdeaEventArgs>(Toolset_IdeaSubmitted);
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

        
        private void magnetAdded(object sender, MagnetEventArgs e)
        {           
	        e.Magnet.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(DragSource_PreviewMouseLeftButtonDown);
	        e.Magnet.PreviewMouseMove += new MouseEventHandler(DragSource_PreviewMouseMove);
        }
        
        
        private void magnetBoardControl_MagnetRemoved(object sender, MagnetRemovedEventArgs e)
        {
        	if (e.Transfer) {
				if (!magnetList.HasMagnet(e.Magnet)) {
	         		magnetList.AddMagnet(e.Magnet,false);	         		
	         	}
        	}
        }

        
        private void magnetList_MagnetRemoved(object sender, MagnetRemovedEventArgs e)
        {
        	if (e.Transfer) {
				if (!ActiveBoard.HasMagnet(e.Magnet)) {
	         		ActiveBoard.AddMagnet(e.Magnet,true);	         		
	         	}
        	}
        }
    
        
	    private void magnetBoardControl_MagnetDropped(object sender, DragEventArgs e)
	    {
	         IDataObject data = e.Data;
	
	         if (data.GetDataPresent(typeof(MagnetControlDataObject))) {
	         	MagnetControlDataObject dataObject = (MagnetControlDataObject)data.GetData(typeof(MagnetControlDataObject));
	         	
	         	// If this magnet is being transferred from the magnet list, remove it from there first:
	         	if (magnetList.HasMagnet(dataObject.Magnet)) {
	         		magnetList.RemoveMagnet(dataObject.Magnet);
	         	}
	         	
	         	// Set the magnet's new position - offset accounts for the position of the mouse when
	         	// clicking on the magnet, and the angle of the magnet's rotation
	         	Point drop = e.GetPosition(this);
	         	drop.X -= dataObject.Magnet.ActualWidth / 2;
	         	drop.Y -= dataObject.Magnet.ActualHeight / 2;
	         	dataObject.Magnet.X = drop.X;
	         	dataObject.Magnet.Y = drop.Y;
	         	
	         	if (!ActiveBoard.HasMagnet(dataObject.Magnet)) {
	         		ActiveBoard.AddMagnet(dataObject.Magnet,false);
	         		ActiveBoard.BringToFront(dataObject.Magnet);
	         	}
	    	}	
	    } 
        

        private void magnetList_MagnetDropped(object sender, DragEventArgs e)
        {
	         IDataObject data = e.Data;
	
	         if (data.GetDataPresent(typeof(MagnetControlDataObject))) {
	         	MagnetControlDataObject dataObject = (MagnetControlDataObject)data.GetData(typeof(MagnetControlDataObject));
	         	
	         	// If this magnet is being transferred from the magnet board, remove it from there first:
	         	if (ActiveBoard.HasMagnet(dataObject.Magnet)) {
	         		ActiveBoard.RemoveMagnet(dataObject.Magnet);
	         	}
	         	
	         	if (!magnetList.HasMagnet(dataObject.Magnet)) {
	         		magnetList.AddMagnet(dataObject.Magnet,false);
	         	}
	         }	
        }

            	
    	#endregion
    	
    	#region Methods
    	
    	public void Open(string filename)
    	{
    		if (!File.Exists(filename)) {
    			Say.Error(filename + " could not be found.");
    			return;
    		}
    			
    		try {
	    		object o = AdventureAuthor.Utils.Serialization.Deserialize(filename,typeof(MagnetBoardInfo));
	    		MagnetBoardInfo magnetBoardInfo = (MagnetBoardInfo)o;	
	    		
	    		ActiveBoard.Open(magnetBoardInfo);
    		}
    		catch (Exception e) {
    			Say.Error("Was unable to open magnet board.",e);
    			ActiveBoard.Clear();
    		}
    	}
    	
    	
    	private void Save()
    	{
    		ActiveBoard.Save();
    		
    		//TODO magnetList.Save();
    		
    		// foreach MagnetBoardControl boardControl in boards {
    		// }
    	}
    	    	
    	
    	private void SaveDialog() 
    	{    		
    		// TODO check which board control is active and only call SaveD
    		if (ActiveBoard.Filename == null) { // get a filename to save to if there isn't one already
    			SaveAsDialog();
    		}
    		else {
	    		try {
	    			Save();
	    		}
	    		catch (Exception e) {
	    			Say.Error("Failed to save magnet board.",e);
	    		}
    		}
    	}
    	
    	
    	private void SaveAsDialog()
    	{
    		SaveFileDialog saveFileDialog = new SaveFileDialog();
    		saveFileDialog.AddExtension = true;
    		saveFileDialog.CheckPathExists = true;
    		saveFileDialog.DefaultExt = Filters.XML;
    		saveFileDialog.Filter = Filters.XML;
  			saveFileDialog.ValidateNames = true;
  			saveFileDialog.Title = "Select location to save magnet board to";
  			bool ok = (bool)saveFileDialog.ShowDialog();  				
  			if (ok) {
  				ActiveBoard.Filename = saveFileDialog.FileName;
  				try {
		  			Save();
  				}
  				catch (Exception e) {
	    			Say.Error("Failed to save magnet board.",e);
  				}
  			}
    	}
                    
            
        private void AddMagnetBoardHandlers(MagnetBoardControl board)
        {
            ActiveBoard.Drop += new DragEventHandler(magnetBoardControl_MagnetDropped);
            ActiveBoard.MagnetRemoved += new EventHandler<MagnetRemovedEventArgs>(magnetBoardControl_MagnetRemoved);        	
            ActiveBoard.MagnetSelected += new EventHandler<MagnetEventArgs>(MagnetSelected);
            ActiveBoard.MagnetAdded += magnetAddedHandler;
        }
    	
    	#endregion
        
    	#region Event handlers
    	 
    	private void OnClick_Open(object sender, RoutedEventArgs e)
        {		
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.ValidateNames = true;
    		openFileDialog.DefaultExt = Filters.XML;
    		openFileDialog.Filter = Filters.XML;
			openFileDialog.Title = "Select a magnet board file to open";
			openFileDialog.Multiselect = false;
			openFileDialog.RestoreDirectory = false;	
			
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
	        	idea = new Idea(IdeaEntryBox.Text,category);
	        	magnetList.AddIdea(idea);
        	}
        }
        
        
        private void OnClick_Scatter(object sender, RoutedEventArgs e)
        {
        	magnetList.Scatter();
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
        			if (SelectedMagnet != null) {
        				if (magnetList.HasMagnet(SelectedMagnet)) {
        					MessageBoxResult result = MessageBox.Show("Permanently delete this idea from the idea box?",
		        					                  "Delete?", 
		        					                  MessageBoxButton.OKCancel,
		        					                  MessageBoxImage.Question,
		        					                  MessageBoxResult.Cancel,
		        					                  MessageBoxOptions.None);
        					if (result == MessageBoxResult.OK) {
        						magnetList.DeleteMagnet(SelectedMagnet);
        						SelectedMagnet = null;
        					}
        				}
        				else if (ActiveBoard.HasMagnet(SelectedMagnet)) {
        					ActiveBoard.RemoveMagnet(SelectedMagnet);
        				}
        			}
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
        				SelectedMagnet.Move(-1,0);
        			}
        			break;
        		case Key.Right:
        			if (SelectedMagnet != null && ActiveBoard.HasMagnet(SelectedMagnet)) {
        				SelectedMagnet.Move(1,0);
        			}
        			break;
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
        
        
        private void OnClick_ClearBoard(object sender, EventArgs e)
        {
			MessageBoxResult result = MessageBox.Show("Clear entire board?",
		        					                  "Clear?", 
		        					                  MessageBoxButton.YesNo,
		        					                  MessageBoxImage.Question,
		        					                  MessageBoxResult.No,
		        					                  MessageBoxOptions.None);
    		if (result == MessageBoxResult.Yes) {
        		List<MagnetControl> magnets = new List<MagnetControl>(ActiveBoard.mainCanvas.Children.Count);
        		foreach (MagnetControl magnet in ActiveBoard.mainCanvas.Children) {
        			magnets.Add(magnet);
        		}        		        		
        		foreach (MagnetControl magnet in magnets) { // TODO in ActiveBoard.GetMagnets()
        			ActiveBoard.RemoveMagnet(magnet);
        		}
    		}
        }
                
        
        private void OnClick_RotateLeft(object sender, EventArgs e) 
        {
        	if (SelectedMagnet != null) {
        		SelectedMagnet.Angle -= 2;
        	}
        }
        
        
        private void OnClick_RotateRight(object sender, EventArgs e) 
        {
        	if (SelectedMagnet != null) {
        		SelectedMagnet.Angle += 2;
        	}
        }
        
        #endregion
        
        

        void DragSource_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && !IsDragging)
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

        void DragSource_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(null);
        }
        

        

        #region STEP2  BASIC Custom Cursor .. 
        

        void DragSource_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
                e.UseDefaultCursors = false;
                e.Handled = true;



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
        }
        #endregion 


        #region STEP3 ADORNERS
        // 
        DragAdorner _adorner = null;
        AdornerLayer _layer;

        #endregion 




        #region STEP5  -- Use DRAGOVER as a workaround 

        FrameworkElement _dragScope;
        public FrameworkElement DragScope
        {
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
            
            
            MagnetControl magnet = (MagnetControl)e.Source;
            magnet.Opacity = 0.5;
            BitmapEffect fx = magnet.BitmapEffect;
            magnet.BitmapEffect = null;
            
            _adorner = new DragAdorner(DragScope,(UIElement)magnet, true, 1.0);// 0.5);
            //magnet.Hide();
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


            DragDropEffects de = DragDrop.DoDragDrop(this, data, DragDropEffects.Move);
            
             // Clean up our mess :) 
             magnet.BitmapEffect = fx;
             magnet.Opacity = 1.0;
            DragScope.AllowDrop = previousDrop;
            AdornerLayer.GetAdornerLayer(DragScope).Remove(_adorner);
            _adorner = null;

            this.GiveFeedback -= feedbackhandler;
            DragScope.DragLeave -= dragleavehandler;
            DragScope.QueryContinueDrag -= queryhandler;
            DragScope.PreviewDragOver -= draghandler;  

            //magnet.Show();
            IsDragging = false;
        }

        private bool _dragHasLeftScope = false; 
        void DragScope_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (this._dragHasLeftScope)
            {
                e.Action = DragAction.Cancel;
                e.Handled = true; 
            }
            
        }


        void DragScope_DragLeave(object sender, DragEventArgs e)
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




        void Window1_DragOver(object sender, DragEventArgs args)
        {
            if (_adorner != null)
            {
                _adorner.LeftOffset = args.GetPosition(DragScope).X /* - _startPoint.X */ ;
                _adorner.TopOffset = args.GetPosition(DragScope).Y /* - _startPoint.Y */ ;
            }
        }


        
        #endregion 



    

        private Point _startPoint;
        private bool _isDragging;

        public bool IsDragging
        {
            get { return _isDragging; }
            set { _isDragging = value; }
        } 

      

    }
}