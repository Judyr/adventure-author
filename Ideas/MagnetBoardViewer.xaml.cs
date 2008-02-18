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
            magnetList.MagnetDeployed += new EventHandler<MagnetEventArgs>(magnetList_MagnetLeavingList);
            magnetList.MagnetSelected += new EventHandler<MagnetEventArgs>(MagnetSelected);
            magneticSurface.MagnetSelected += new EventHandler<MagnetEventArgs>(MagnetSelected);
            Toolset.IdeaSubmitted += new EventHandler<IdeaEventArgs>(Toolset_IdeaSubmitted);
            
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
            magnetList.VisibleCategoriesChanged += new EventHandler(VisibleCategoriesChanged);
            magneticSurface.MagnetRemoved += new EventHandler<MagnetEventArgs>(MagnetBoard_MagnetRemoved);
        }
        

        private void Toolset_IdeaSubmitted(object sender, IdeaEventArgs e)
        {
        	MagnetControl magnet = new MagnetControl(e.Idea);
        	magnetList.AddNewMagnet(magnet);
        }

        
        /// <summary>
        /// A magnet has been removed from the magnet board - rather than deleting the magnet permanently,
        /// it should return to the magnet list.
        /// </summary>
        private void MagnetBoard_MagnetRemoved(object sender, MagnetEventArgs e)
        {
        	magnetList.TransferMagnetFromBoard(e.Magnet);
        }
        
        
        /// <summary>
        /// When an idea category is shown/hidden, update the menu which allows you to show/hide categories.
        /// </summary>
        private void VisibleCategoriesChanged(object sender, EventArgs ea)
        {
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
	    		
	    		magneticSurface.Open(magnetBoardInfo);
    		}
    		catch (Exception e) {
    			Say.Error("Was unable to open magnet board.",e);
    			magneticSurface.Clear();
    		}
    	}
    	
    	
    	private void Save()
    	{
    		// foreach MagnetBoardControl boardControl in boards {
    			magneticSurface.Save();
    		// }
    	}
    	    	
    	
    	private void SaveDialog() 
    	{    		
    		// TODO check which board control is active and only call SaveD
    		if (magneticSurface.Filename == null) { // get a filename to save to if there isn't one already
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
  				magneticSurface.Filename = saveFileDialog.FileName;
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
        	if (IdeaCategoryComboBox.SelectedItem != null) {
        		idea = new Idea(IdeaEntryBox.Text,(IdeaCategory)IdeaCategoryComboBox.SelectedItem);
        	}
        	else {
        		idea = new Idea(IdeaEntryBox.Text);
        	}
        	magnetList.AddIdea(idea);
        }
        
        
        private void OnClick_Scatter(object sender, RoutedEventArgs e)
        {
        	magnetList.Scatter();
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

        
        private void magnetList_MagnetLeavingList(object sender, MagnetEventArgs e)
        {
        	if (!magneticSurface.HasMagnet(e.Magnet)) {
        		magneticSurface.AddMagnet(e.Magnet,true);
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
        				else if (magneticSurface.HasMagnet(SelectedMagnet)) {
        					magneticSurface.RemoveMagnet(SelectedMagnet);
        					SelectedMagnet = null;
        				}
        			}
        			break;
        		case Key.Up:
        			if (SelectedMagnet != null && magneticSurface.HasMagnet(SelectedMagnet)) {
        				SelectedMagnet.Move(0,-1);
        			}
        			break;
        		case Key.Down:
        			if (SelectedMagnet != null && magneticSurface.HasMagnet(SelectedMagnet)) {
        				SelectedMagnet.Move(0,1);
        			}
        			break;
        		case Key.Left:
        			if (SelectedMagnet != null && magneticSurface.HasMagnet(SelectedMagnet)) {
        				SelectedMagnet.Move(-1,0);
        			}
        			break;
        		case Key.Right:
        			if (SelectedMagnet != null && magneticSurface.HasMagnet(SelectedMagnet)) {
        				SelectedMagnet.Move(1,0);
        			}
        			break;
        	}
        	
        }
        
        #endregion
    }
}