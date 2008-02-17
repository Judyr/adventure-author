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
using Microsoft.Win32;

namespace AdventureAuthor.Ideas
{
    /// <summary>
    /// Interaction logic for MagnetBoardViewer.xaml
    /// </summary>

    public partial class MagnetBoardViewer : Window
    {    	
    	#region Constructors
    	
        public MagnetBoardViewer()
        {
            InitializeComponent();            
            IdeaCategoryComboBox.ItemsSource = Enum.GetValues(typeof(IdeaCategory));
            magnetList.MagnetLeavingList += new EventHandler<MagnetEventArgs>(magnetList_MagnetLeavingList);
        }

        
        private void magnetList_MagnetLeavingList(object sender, MagnetEventArgs e)
        {
        	if (!magneticSurface.HasMagnet(e.Magnet)) {
        		magneticSurface.AddMagnet(e.Magnet,true);
        	}        	
        }
    	
    	#endregion
    	
    	#region Events
    	
    	
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
        	magnetList.Add(idea);
        }
        
        
        private void OnClick_Scatter(object sender, RoutedEventArgs e)
        {
        	List<MagnetControl> magnets = new List<MagnetControl>(magnetList.magnetsPanel.Children.Count);
        	foreach (MagnetControl magnet in magnetList.magnetsPanel.Children) {
        		magnets.Add(magnet);
        	}
        	foreach (MagnetControl magnet in magnets) {
        		magnetList.MoveMagnetFromList(magnet);
        	}
        }
        
        #endregion
    }
}