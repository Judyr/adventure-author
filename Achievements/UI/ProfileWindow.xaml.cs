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
using AdventureAuthor.Achievements;
using AdventureAuthor.Setup;

namespace AdventureAuthor.Achievements.UI
{
    /// <summary>
    /// A screen which displays the user's profile information and awards.
    /// </summary>
    public partial class ProfileWindow : Window
    {
    	#region Properties and fields
    	
    	#endregion

    	#region Constructors
    	
        public ProfileWindow()
        {
            InitializeComponent();
            awardsCase.DataContext = Toolset.Plugin.Profile.Awards;
        }
        
        #endregion
        
        #region Methods
        
        
        #endregion
    }
}