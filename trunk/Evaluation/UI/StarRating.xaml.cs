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
using AdventureAuthor.Utils;
using AdventureAuthor.Scripts.UI;

namespace AdventureAuthor.Evaluation.UI
{
    /// <summary>
    /// Interaction logic for StarRating.xaml
    /// </summary>

    public partial class StarRating : UserControl, IQuestionPanel
    {
    	public StarRating() : this(5)
    	{
        }
        
        
        public StarRating(int stars)
        {
            InitializeComponent();
        	for (int i = 0; i < stars; i++) {
        		CheckBox button = new CheckBox();
        		button.Template = (ControlTemplate)Resources["StarTemplate"];
        		button.Click += new RoutedEventHandler(OnClick);
        		StarsPanel.Children.Add(button);
        	}
        }
        
        
        public StarRating(Rating rating) : this(rating.Max)
        {
        	try {
	        	int ratingValue = int.Parse(rating.Value);
	        	Rating = ratingValue;
        	}
			catch (Exception) {
				throw new ArgumentException(rating.Value + " is not a valid numerical rating.");
        	}
        }
        
        
        /// <summary>
        /// Highlight all the stars prior to and including the one that's been clicked
        /// </summary>
        private void OnClick(object sender, RoutedEventArgs e)
        {
        	bool shouldCheck = true;
        	foreach (UIElement uielement in StarsPanel.Children) {
        		if (uielement is CheckBox) {        		
	        		CheckBox star = (CheckBox)uielement;
	        		star.IsChecked = shouldCheck;	        			
		        	if (shouldCheck && star == sender) {
		        		shouldCheck = false;
		        	}
        		}
        	}
        }
           
        
        public int Rating
        {
        	get {    
        		int rating = 0;        		
	        	foreach (UIElement uielement in StarsPanel.Children) {
	        		if (uielement is CheckBox) {        		
		        		CheckBox star = (CheckBox)uielement;
		        		if ((bool)star.IsChecked) {
		        			rating++;
		        		}
	        		}
	        	}        		
        		return rating;
        	}
        	set {
        		int starsChecked = 0;
        		bool shouldCheck = true;
        		
	        	foreach (UIElement uielement in StarsPanel.Children) {
	        		if (uielement is CheckBox) {
	        			if (shouldCheck && starsChecked == value) {
	        				shouldCheck = false;
	        			}     		
		        		CheckBox star = (CheckBox)uielement;
		        		star.IsChecked = shouldCheck;
		        		if ((bool)star.IsChecked) {
		        			starsChecked++;
		        		}
	        		}
	        	}
        	}
        }
    	
        
		public object Answer {
			get {
				return Rating;
			}
		}
    }
}