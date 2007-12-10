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
using AdventureAuthor.Notebook.Worksheets;

namespace AdventureAuthor.Notebook.Worksheets.UI
{
    /// <summary>
    /// Interaction logic for PointControl.xaml
    /// </summary>

    public partial class PointControl : UserControl
    {
        private Point representedPoint;        
		public Point RepresentedPoint {
			get { return representedPoint; }
			set { 
				representedPoint = value; 
				PointTextBox.Text = representedPoint.Message;
	        	switch (representedPoint.Type) {
	        		case PointType.Good:
	        			BePositive();
	        			break;
	        		case PointType.Bad:
	        			BeNegative();
	        			break;
	        		default:
	        			break;
	        	}				
			}
		}
        
        
        public PointControl()
        {
            InitializeComponent();
        }
        
        
        public PointControl(Point point) : this()
        {
        	RepresentedPoint = point;
        }
        
        
        private void BePositive()
        {
        	ProConTextBlock.Text = "Like";
        	ProConTextBlock.Foreground = Brushes.DarkGreen;
        	LineControlGrid.Background = (Brush)Resources["ProBrush"];
        }
        
        
        private void BeNegative()
        {
        	ProConTextBlock.Text = "Dislike";
        	ProConTextBlock.Foreground = Brushes.DarkRed;
        	LineControlGrid.Background = (Brush)Resources["ConBrush"];
        }
        
        
       	protected void OnGotFocus(object sender, RoutedEventArgs e)
        {
        	PointTextBox.Background = Brushes.White;
        	PointTextBox.BorderBrush = Brushes.Black;
        }
        
        
        protected void OnLostFocus(object sender, RoutedEventArgs e)
        {
        	PointTextBox.Background = Brushes.Transparent;
        	PointTextBox.BorderBrush = Brushes.Transparent;
        }
    }
}