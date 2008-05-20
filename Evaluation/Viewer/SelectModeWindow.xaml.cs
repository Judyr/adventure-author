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
using AdventureAuthor.Evaluation;
using AdventureAuthor.Utils;
using AdventureAuthor.Setup;
using System.Windows.Forms.Integration;

namespace AdventureAuthor.Evaluation
{
    public partial class SelectModeWindow : Window
    {
        public SelectModeWindow()
        {        	
            InitializeComponent();
            
            try {
            	Tools.EnsureDirectoryExists(WorksheetPreferences.Instance.SavedWorksheetsDirectory);
            }
            catch (Exception e) {
            	Say.Error("A problem was encountered when trying to create a folder for saved worksheets.",e);
            }            
        }

        
        private void OnClick_CompleteMode(object sender, EventArgs e)
        {
        	Launch(Mode.Complete);
        	Close();
        }

        
        private void OnClick_DesignMode(object sender, EventArgs e)
        {
        	if (User.IdentifyTeacherOrDemandPassword()) {
        		Launch(Mode.Design);
        		Close();
        	}
        }
        
        
        /// <summary>
		/// Bring up the Worksheets window.
		/// </summary>
		public static void Launch(Mode mode)
		{
			try {
				if (WorksheetViewer.Instance == null || !WorksheetViewer.Instance.IsLoaded) {
					WorksheetViewer.Instance = new WorksheetViewer(mode);
				}
				ElementHost.EnableModelessKeyboardInterop(WorksheetViewer.Instance);
				WorksheetViewer.Instance.Show();
			}
			catch (Exception e) {
				Say.Error("A problem was encounted when trying to open the Worksheets application.",e);
			}
		}	
    }
}