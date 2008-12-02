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
    	
    	public List<Bitmap> GetAllAwardImages()
    	{
//    		ResourceManager manager = new ResourceManager("AdventureAuthor.Achievements.UI.awardimages",
//    		                                              Assembly.GetExecutingAssembly());
    		throw new NotImplementedException();
    	}
    	
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
        
        #endregion
    }
}