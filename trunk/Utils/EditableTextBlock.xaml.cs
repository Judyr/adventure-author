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

namespace AdventureAuthor.Utils
{
    /// <summary>
    /// Interaction logic for EditableTextBlock.xaml
    /// </summary>

    public partial class EditableTextBlock : UserControl
    {
		public string Text
		{
			get { return this.textBox.Text; }
			set { this.textBox.Text = value; }
		}
    	
		
		private bool isEditable;
		public bool IsEditable {
			get { return isEditable; }
			set { 
				isEditable = value;
				if (isEditable) {
					textBlock.Visibility = Visibility.Hidden;
					textBlock.Focusable = false;
					this.textBox.Visibility = Visibility.Visible;
					this.textBox.Focusable = true;
				}
				else {
					textBlock.Visibility = Visibility.Visible;
					textBlock.Focusable = false;
					this.textBox.Visibility = Visibility.Hidden;
					this.textBox.Focusable = false;
				}
			}
		}
		
		
		
		
		
		
		
		
    	
        public EditableTextBlock(bool isEditable)
        {
            InitializeComponent();
            IsEditable = isEditable;
            
        }

        
        public EditableTextBlock() : this(false)
        {
        	
        }
    }
}