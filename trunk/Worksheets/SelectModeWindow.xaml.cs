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
using System.Windows.Forms.Integration;
using System.IO;
using AdventureAuthor.Evaluation;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Evaluation
{
    public partial class SelectModeWindow : Window
    {
        public SelectModeWindow()
        {        	
            InitializeComponent();
            
            try {       	
    			Tools.EnsureDirectoryExists(EvaluationPreferences.LocalAppDataDirectory);
            	Tools.EnsureDirectoryExists(EvaluationPreferences.Instance.SavedCommentCardsDirectory);     
            }
            catch (Exception e) {
            	Say.Error("A problem was encountered when trying to create a folder for saved Comment Cards.",e);
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
		/// Bring up the Comment Card window.
		/// </summary>
		public static void Launch(Mode mode)
		{
			try {
				if (CardViewer.Instance == null || !CardViewer.Instance.IsLoaded) {
					CardViewer.Instance = new CardViewer(mode);
				}
				ElementHost.EnableModelessKeyboardInterop(CardViewer.Instance);
				CardViewer.Instance.Show();
			}
			catch (Exception e) {
				Say.Error("A problem was encounted when trying to open the Comment Cards application.",e);
			}
		}	
    }
}