using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;
using System.Resources;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AdventureAuthor.Utils;
using Microsoft.Win32;

namespace AdventureAuthor.Achievements.UI
{
    /// <summary>
    /// A dialog window that allows the user to define
    /// and grant their own custom award.
    /// </summary>
    public partial class CreateCustomAwardDialog : Window
    {
    	#region Constructors
    	
    	/// <summary>
	    /// A dialog window that allows the user to define
	    /// and grant their own custom award.
    	/// </summary>
        public CreateCustomAwardDialog()
        {
            InitializeComponent();
        }
        
    	#endregion

    	#region Methods
    	    	
    	#endregion
    	
        #region Event handlers
        
        /// <summary>
        /// Create the custom award and give it to the user.
        /// </summary>
        private void GiveUserCustomAward(object sender, RoutedEventArgs e)
        {
        	try {
	        	CustomAward award = new CustomAward(awardNameTextBox.Text,
	        	                                    awardDescriptionTextBox.Text,
	        	                                    Award.DEFAULTDESIGNERPOINTSVALUE,
	        	                                    imageLocationTextBox.Text);
        		AchievementMonitor.GiveAward(award);
        		Close();
        	}
        	catch (FileNotFoundException) {
        		Say.Error("No file was found at location '" + imageLocationTextBox.Text + "'.");
        	}
        	catch (IOException) {
        		Say.Error("The file at location '" + imageLocationTextBox.Text + "' is not a valid image file.");
        	}
        }
        
        
        /// <summary>
        /// Close the window.
        /// </summary>
        private void CloseWindow(object sender, RoutedEventArgs e)
        {
        	Close();
        }
        
        
        /// <summary>
        /// Launch an 'open file' dialog to allow the user to browse
        /// to an image file.
        /// </summary>
        private void LaunchOpenFileDialog(object sender, RoutedEventArgs e)
        {
        	OpenImageBrowseDialog();
        }
        
        #endregion
        
        #region Methods
        
        /// <summary>
        /// Launch an 'open file' dialog to allow the user to browse
        /// to an image file.
        /// </summary>
		private void OpenImageBrowseDialog()
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.ValidateNames = true;
    		openFileDialog.DefaultExt = Filters.PICTURES_ALL;
    		openFileDialog.Filter = Filters.PICTURES_ALL;
			openFileDialog.Title = "Select an image file";
			openFileDialog.Multiselect = false;
			openFileDialog.RestoreDirectory = false;
						
  			bool ok = (bool)openFileDialog.ShowDialog();  				
  			if (ok) {
  				imageLocationTextBox.Text = openFileDialog.FileName;
  			}
		}
        
        #endregion
    }
}