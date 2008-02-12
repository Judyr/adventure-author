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
using AdventureAuthor.Evaluation.Viewer;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Evaluation.Viewer
{
    public partial class SelectModeWindow : Window
    {
        public SelectModeWindow()
        {
            InitializeComponent();
        }

        
        private void OnClick_CompleteMode(object sender, EventArgs e)
        {
        	Launch(Mode.Complete);
        }
        
        
        private void OnClick_DiscussMode(object sender, EventArgs e)
        {
        	Launch(Mode.Discuss);
        }

        
        private void OnClick_DesignMode(object sender, EventArgs e)
        {
        	if (Tools.TeacherHasSignedIn(false)) {
        		Launch(Mode.Design);
        	}
        }
        
        
        private void Launch(Mode evaluationMode)
        {
        	WorksheetViewer viewer = new WorksheetViewer(evaluationMode);
        	viewer.ShowDialog();
        	Close();
        }
    }
}