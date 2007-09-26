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

namespace AdventureAuthor.Notebook.MyIdeas.UI
{
    /// <summary>
    /// Interaction logic for MyIdeasWindow.xaml
    /// </summary>

    public partial class MyIdeasWindow : Window
    {
        public MyIdeasWindow()
        {
            InitializeComponent();
            
            List<Balloon> balloons = new List<Balloon>(5);
            balloons.Add(new Balloon(new Idea()));
            balloons.Add(new Balloon(new Idea()));
            balloons.Add(new Balloon(new Idea()));
            balloons.Add(new Balloon(new Idea()));
            balloons.Add(new Balloon(new Idea()));
            Sky sky = new Sky(balloons);
            MainCanvas.Children.Add(sky);
        }

    }
}
