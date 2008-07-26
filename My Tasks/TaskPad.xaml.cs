using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AdventureAuthor.Tasks
{
    /// <summary>
    /// Interaction logic for TaskPad.xaml
    /// </summary>

    public partial class TaskPad : UserControl
    {
		private Color[] allColors;
		private Random random;
		
		
		public TaskPad()
		{
			Type colorsType = typeof(Colors);
			PropertyInfo[] properties = colorsType.GetProperties();
			allColors = new Color[properties.Length];
			for (int i = 0; i < properties.Length; i++) {
				PropertyInfo property = properties[i];
				Color color = (Color)property.GetValue(colorsType,null);
				allColors[i] = color;
			}
			random = new Random();
				
			InitializeComponent();
		}
		
		
		private void SetBackgroundColourRandomly()
		{
			int randomNumber = random.Next(0,allColors.Length-1);
			Background = new SolidColorBrush(allColors[randomNumber]);
		}
    }
}