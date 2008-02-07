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


namespace AdventureAuthor.Ideas
{
    /// <summary>
    /// Interaction logic for MagnetBoardViewer.xaml
    /// </summary>

    public partial class MagnetBoardViewer : Window
    {
        public MagnetBoardViewer()
        {
            InitializeComponent();
        }

        
        private void OnClick_AddMagnet(object sender, RoutedEventArgs e)
        {
        	Random random = new Random();
        	double x = random.NextDouble() * 800;
        	double y = random.NextDouble() * 600;
        	double angle = (90 * random.NextDouble());    
        	if (DateTime.Now.Millisecond % 2 == 0) {
        		angle = 360 - angle; // randomly angle to the left instead of the right
        	}
        	
        	Magnet magnet = new Magnet(x,y,angle,MagneticSurface);
        	magnet.IdeaLabel.Text = IdeaEntryBox.Text;
        	this.MagneticSurface.Add(magnet);
        }
    }
}