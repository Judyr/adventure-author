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
    /// Interaction logic for TagControl.xaml
    /// </summary>
    public partial class TagControl : UserControl
    {
    	public event EventHandler Deleting;
		protected virtual void OnDeleting(EventArgs e) {
    		EventHandler handler = Deleting;
			if (handler != null) {
				handler(this, e);
			}
		}
    	
    	
        public TagControl()
        {
            InitializeComponent();
        }
    }
}