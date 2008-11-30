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
    	
        public ProfileWindow()
        {
            InitializeComponent();
            awardsCase.DataContext = Toolset.Plugin.Profile.Awards;
            
            // Put a placeholder picture in the user rank image box:
            ResourceManager manager = new ResourceManager("AdventureAuthor.Utils.images",
                                                          Assembly.GetAssembly(typeof(EditableTextBox)));
           
            System.Drawing.Bitmap bitmap = (System.Drawing.Bitmap)manager.GetObject("nwn2_beholder");
           	BitmapSource source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
																		 bitmap.GetHbitmap(),
					                                                     IntPtr.Zero,
			                                                             Int32Rect.Empty,
			                                                             BitmapSizeOptions.FromEmptyOptions());	
            rankImage.Source = source;
        }
        
        #endregion
        
        #region Methods
        
        
        #endregion
    }
}