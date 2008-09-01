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
    	public event EventHandler ClickedDelete;
		protected virtual void OnClickedDelete(EventArgs e) {
    		EventHandler handler = ClickedDelete;
			if (handler != null) {
				handler(this, e);
			}
		}
    	
    	
        public TagControl()
        {
            InitializeComponent();
        }

        
        private void DeleteTagButtonClick(object sender, RoutedEventArgs e)
        {
        	OnClickedDelete(new EventArgs());
        }
    }
}