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
    	
    	#endregion
    	
    	#region Events
    	
    	
    	#endregion
	
    	#region Constructors
    	
        public MagnetBoardViewer()
        {
            InitializeComponent();          
            
            // Set up 'Add idea' box and 'Show/Hide idea category' menu:
            IdeaCategoryComboBox.ItemsSource = Idea.IDEA_CATEGORIES;            
            foreach (IdeaCategory ideaCategory in Idea.IDEA_CATEGORIES) {
            	string category = ideaCategory.ToString();
            	MenuItem menuItem = new MenuItem();
            	menuItem.Name = category;
            	menuItem.Header = "Show " + category + " ideas?";
            	menuItem.IsCheckable = true;
            	menuItem.IsChecked = true;
            	menuItem.Checked += new RoutedEventHandler(menuItem_CheckedChanged);
            	menuItem.Unchecked += new RoutedEventHandler(menuItem_CheckedChanged);
            	ShowHideCategoriesMenu.Items.Add(menuItem);
            }          
            
            // Listen for events:
            AddMagnetBoardHandlers(ActiveBoard); // this gets its own method in case we later have multiple boards
            
            magnetList.VisibleMagnetsChanged += new EventHandler(VisibleMagnetsChanged);              
            magnetList.MagnetSelected += new EventHandler<MagnetEventArgs>(MagnetSelected);
            magnetList.MagnetRemoved += new EventHandler<MagnetRemovedEventArgs>(magnetList_MagnetRemoved); 
            magnetList.Drop += new DragEventHandler(magnetList_MagnetDropped);  
            
            Toolset.IdeaSubmitted += new EventHandler<IdeaEventArgs>(Toolset_IdeaSubmitted);                                 
        } 

        
        private void AddMagnetBoardHandlers(MagnetBoardControl board)
        {
            ActiveBoard.Drop += new DragEventHandler(magnetBoardControl_MagnetDropped);
            ActiveBoard.MagnetRemoved += new EventHandler<MagnetRemovedEventArgs>(magnetBoardControl_MagnetRemoved);        	
            ActiveBoard.MagnetSelected += new EventHandler<MagnetEventArgs>(MagnetSelected);
        }
        
        
        private void magnetBoardControl_MagnetRemoved(object sender, MagnetRemovedEventArgs e)
        {
        	if (e.Transfer) {
				if (!magnetList.HasMagnet(e.Magnet)) {
	         		magnetList.AddMagnet(e.Magnet,true);	         		
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
	         	Point p = e.GetPosition(this);
	         	Vector drop = p - dataObject.Offset;	         		         	
	         	dataObject.Magnet.X = drop.X;
	         	dataObject.Magnet.Y = drop.Y;
	         	
	         	if (!ActiveBoard.HasMagnet(dataObject.Magnet)) {
	         		ActiveBoard.AddMagnet(dataObject.Magnet,false);	         		
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
	    		
	    		// TODO Launch a new MagnetBoardControl tab, and operate on that for the rest of this method
	    		// ..until then, just use the single default MagnetBoardControl
	    		
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
        
        
        private void OnClick_Scatter(object sender, RoutedEventArgs e)
        {
        	magnetList.Scatter();
        }
        
        
        private void OnClick_Sort(object sender, RoutedEventArgs e)
        {
        	magnetList.Sort();
        }

        
        /// <summary>
        /// Show or hide the idea category represented by this menu item (show if it has been checked,
        /// hide if it has been unchecked.)
        /// </summary>
        private void menuItem_CheckedChanged(object sender, RoutedEventArgs ea)
        {
        	MenuItem menuItem = (MenuItem)ea.Source;
        	try {
        		IdeaCategory category = (IdeaCategory)Enum.Parse(typeof(IdeaCategory),menuItem.Name,true);
        		if (menuItem.IsChecked && !magnetList.CategoryIsVisible(category)) {
        			magnetList.ShowCategory(category);
        		}
        		else if (!menuItem.IsChecked && magnetList.CategoryIsVisible(category)) {
        			magnetList.HideCategory(category);
        		}
        	}
        	catch (ArgumentException e) {
        		Say.Error("Could not find an idea category to show/hide it.",e);
        	}
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
        /// When an idea category is shown/hidden, update the menu which allows you to show/hide categories.
        /// Also check the selected magnet has not been hidden, and nullify it if it has.
        /// </summary>
        private void VisibleMagnetsChanged(object sender, EventArgs ea)
        {        	
        	if (SelectedMagnet != null && !SelectedMagnet.IsVisible) {
        		SelectedMagnet = null;
        	}
        	
        	foreach (MenuItem menuItem in ShowHideCategoriesMenu.Items) {
        		try {
        			IdeaCategory category = (IdeaCategory)Enum.Parse(typeof(IdeaCategory),menuItem.Name,true);
	        		bool categoryIsVisible = magnetList.CategoryIsVisible(category);
	        		if (menuItem.IsChecked != categoryIsVisible) {
	        			menuItem.IsChecked = categoryIsVisible;
	        		}
        		}
        		catch (ArgumentException e) {
        			Say.Error("Menu item did not appear to correspond to an ideas category.",e);
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
        
        
        private void OnClick_EditSelected(object sender, EventArgs e)
        {
        	if (SelectedMagnet != null) {
        		if (SelectedMagnet.IdeaTextBox.IsEditable) {
        			SelectedMagnet.IdeaTextBox.IsEditable = false;
        		}
        		else {
        			SelectedMagnet.IdeaTextBox.IsEditable = true;
        		}
        	}
        }
        
        #endregion
    }
}