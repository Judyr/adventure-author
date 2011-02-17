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
using System.Windows.Markup;

namespace AdventureAuthor.Utils
{
    /// <summary>
    /// Interaction logic for IconButton.xaml
    /// </summary>

    [ContentPropertyAttribute("Picture")]
    public partial class PictureButton : Button
    {
    	public Image Picture {
    		get { return image; }
    		set { image = value; }
    	}
    	
    	
    	public string Text {
    		get { return textBlock.Text; }
    		set { textBlock.Text = value; }
    	}
    	
    	
        public PictureButton()
        {
            InitializeComponent();
        }
        
        
        public PictureButton(Image picture, string text)
        {
        	Picture = picture;
        	Text = text;
        }
    }
}