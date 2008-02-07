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

        
        private void OnClick_PupilMode(object sender, EventArgs e)
        {
        	Launch(false);//(WorksheetViewer.Mode.Pupil);
        }
        
        
        private void OnClick_TeacherMode(object sender, EventArgs e)
        {
        	//Launch(WorksheetViewer.Mode.Teacher);
        }

        
        private void OnClick_DesignerMode(object sender, EventArgs e)
        {
        	Launch(true);//(WorksheetViewer.Mode.Constructor);
        }
        
        
        private void Launch(bool designer)//WorksheetViewer.Mode mode)
        {
        	WorksheetViewer viewer = new WorksheetViewer(designer);
        	viewer.ShowDialog();
        	Close();
        }
    }
}