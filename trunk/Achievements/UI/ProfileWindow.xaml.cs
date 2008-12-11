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
using System.Reflection;
using System.Resources;
using AdventureAuthor.Achievements;
using AdventureAuthor.Setup;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Achievements.UI
{
    /// <summary>
    /// A screen which displays the user's profile information and awards.
    /// </summary>
    public partial class ProfileWindow : Window
    {
    	#region Properties and fields
    	
    	/// <summary>
    	/// The single instance of the My Achievements window.
    	/// </summary>
    	private static ProfileWindow instance;    	
		public static ProfileWindow Instance {
			get { return instance; }
			set { instance = value; }
		}   
    	
    	#endregion

    	#region Constructors
    	
    	/// <summary>
    	/// A screen which displays the user's profile information and awards.
    	/// </summary>
        public ProfileWindow()
        {
            InitializeComponent();
            awardsCase.DataContext = Toolset.Plugin.Profile.Awards;
            
            // Put a placeholder picture in the user rank image box:
            ResourceManager manager = new ResourceManager("AdventureAuthor.Utils.images",
                                                          Assembly.GetAssembly(typeof(EditableTextBox)));
           
            System.Drawing.Bitmap bitmap = (System.Drawing.Bitmap)manager.GetObject("aalogo");
           	BitmapSource source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
																		 bitmap.GetHbitmap(),
					                                                     IntPtr.Zero,
			                                                             Int32Rect.Empty,
			                                                             BitmapSizeOptions.FromEmptyOptions());	
            rankImage.Source = source;            
        }
        
        #endregion
        
        #region Event handlers
        
        /// <summary>
        /// Open a dialog window that allows the user to create
        /// their own custom award and add it to the awards case.
        /// </summary>
        private void AddCustomAward(object sender, RoutedEventArgs e)
        {
        	if (User.IdentifyTeacherOrDemandPassword()) {
        		new CreateCustomAwardDialog().ShowDialog();
        	}
        }
        
        #endregion
    }
}