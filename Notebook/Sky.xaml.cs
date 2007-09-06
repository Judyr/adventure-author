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

namespace AdventureAuthor.Notebook
{
    public partial class Sky : UserControl
    {				
		private string author;
		public string Author {
			get { return author; }
		}
		
		public Sky()
		{
            InitializeComponent();
            this.author = String.Empty;
		}
		    	
		public Sky(List<Balloon> balloons) : this()
        {           
			Ellipse e = new Ellipse();
			
			
			Random random = new Random();
            foreach (Balloon b in balloons) {
            	Canvas.SetLeft(b,random.Next(100,(int)SkyCanvas.Width-100));
            	Canvas.SetTop(b,random.Next(100,(int)SkyCanvas.Height-100));
            	this.SkyCanvas.Children.Add(b);
            }
        }
    }
}
