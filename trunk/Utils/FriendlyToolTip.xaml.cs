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

namespace AdventureAuthor.Setup.UI
{
    public partial class FriendlyToolTip : ToolTip
    {
    	public object TipTitle {
    		get {
    			return TitleLabel.Content;
    		}
    		set {
    			TitleLabel.Content = value;
    		}
    	}
    	
    	
    	public string TipMessage {
    		get {
    			return MainText.Text;
    		}
    		set {
    			MainText.Text = value;
    		}
    	}
    	
    	
    	public FriendlyToolTip()
    	{
            InitializeComponent();
    	}
    	
    	
        public FriendlyToolTip(string title, string message)
        {
            InitializeComponent();
            TitleLabel.Content = title;
            MainText.Text = message;
            SeparatorLine.Visibility = Visibility.Collapsed;
            HintPanel.Visibility = Visibility.Collapsed;
        }
        
        
        public FriendlyToolTip(string title, string message, string hint)
        {
            InitializeComponent();
            TitleLabel.Content = title;
            MainText.Text = message;
            HintText.Text = hint;
        }
    }
}