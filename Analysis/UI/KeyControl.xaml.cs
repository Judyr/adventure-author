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

namespace AdventureAuthor.Analysis.UI
{
    /// <summary>
    /// Interaction logic for KeyControl.xaml
    /// </summary>

    public partial class KeyControl : UserControl
    {
        public KeyControl()
        {
            InitializeComponent();
        }

        
        public void AddSymbol(Shape symbol, string identifier)
        {
        	StackPanel entry = new StackPanel();
        	entry.Orientation = Orientation.Horizontal;
        	entry.Margin = new Thickness(10);
        	entry.Children.Add(symbol);
        	
        	TextBlock text = new TextBlock();
        	text.Text = identifier;
        	text.FontFamily = new FontFamily("Times New Roman");
        	text.FontSize = 18;
        	entry.Children.Add(text);
        	
        	EntriesPanel.Children.Add(entry);
        }
    }
}