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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AdventureAuthor.Tasks
{
    /// <summary>
    /// Interaction logic for DogEarControl.xaml
    /// </summary>

    public partial class DogEarControl : UserControl
    {
        public DogEarControl()
        {
            InitializeComponent();
//            System.Timers.Timer timer = new System.Timers.Timer(100);
//            int movement = 1;
//            timer.Elapsed += delegate { 
//            	Int32 extent = (Int32)Resources["Extent"];
//            	if (extent >= 150 || extent <= 0) {
//            		movement *= -1;
//            	}
//            	Resources["Extent"] = extent + movement;
//            };
//            timer.Start();
        }

    }
}