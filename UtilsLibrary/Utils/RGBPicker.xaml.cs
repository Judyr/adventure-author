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
using AdventureAuthor.Utils;

namespace AdventureAuthor.Utils
{
    /// <summary>
    /// Interaction logic for RGBPicker.xaml
    /// </summary>

    public partial class RGBPicker : Window
    {
        public RGBPicker()
        {
            InitializeComponent();
            Red.Text = "0";
            Green.Text = "0";
            Blue.Text = "0";
        }

        
        private void OnTextChanged(object sender, EventArgs e)
        {
        	try {
        		SolidColorBrush brush = new SolidColorBrush(Color.FromRgb(byte.Parse(Red.Text),
        		                                                          byte.Parse(Green.Text),
        		                                                          byte.Parse(Blue.Text)));
	        	SetColour(brush);
        	}
        	catch (Exception) {
        		//Say.Error("No!",ex);
        	}
        }
        
        
        private void OnClickRandom(object sender, EventArgs e)
        {
        	byte[] rgbarray = new byte[3];
        	Random r = new Random();
        	r.NextBytes(rgbarray);
        	try {
        		SolidColorBrush brush = new SolidColorBrush(Color.FromRgb(rgbarray[0],rgbarray[1],rgbarray[2]));
        		Red.Text = rgbarray[0].ToString();
        		Green.Text = rgbarray[1].ToString();
        		Blue.Text = rgbarray[2].ToString();
        		SetColour(brush);
        	}
        	catch (Exception) {
        		//Say.Error("No!",ex);
        	}
        }
        
        
        private void SetColour(Brush brush)
        {
        	foreach (RadioButton rb in ButtonsPanel.Children) {
        		if ((bool)rb.IsChecked) {
        			Label l = (Label)rb.Content;
        			l.Background = brush;
        			return;
        		}
        	}
        }
    }
}