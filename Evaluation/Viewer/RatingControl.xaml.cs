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
using AdventureAuthor.Evaluation.Viewer;

namespace AdventureAuthor.Evaluation.Viewer
{
    /// <summary>
    /// Interaction logic for RatingControl.xaml
    /// </summary>

    public partial class RatingControl : OptionalWorksheetPartControl
    {    		
    	#region Fields
    	
    	private int maxStars;    	
		public int MaxStars {
			get { return maxStars; }
		}
    	
    	#endregion
                
    	
    	#region Constructors
        
        
    	public RatingControl(Rating rating, bool designerMode)
        {
    		if (rating == null) {
    			rating = new Rating();
    		}
    		
            InitializeComponent();
            LoadStars(rating.Max);
            
            if (designerMode) { // show 'Active?' control, and assume that control is Active to begin with
            	ActivateCheckBox.Visibility = Visibility.Visible;
	    		if (rating.Include) {
	    			Activate();
	    		}
	    		else {
	    			Deactivate(false);
	    		}
            }
            else { // hide 'Active?' control, and set status to Not Applicable
            	Enable();
            }
        	try {
	        	int ratingValue = int.Parse(rating.Value);
	        	SelectedStars = ratingValue;
        	}
			catch (Exception) {
    			Say.Debug(rating.Value + " is not a valid numerical rating.");
				SelectedStars = 0;
        	}
        }
    	
    	
    	#endregion
        
        
        private void LoadStars(int stars)
        {
        	maxStars = stars;
        	for (int i = 0; i < stars; i++) {
        		CheckBox button = new CheckBox();
        		button.Template = (ControlTemplate)Resources["StarTemplate"];
        		button.Click += new RoutedEventHandler(OnClick);
        		StarsPanel.Children.Add(button);
        	}
        	Width = maxStars * 40;
        }
        
        
        /// <summary>
        /// Highlight all the stars prior to and including the one that's been clicked
        /// </summary>
        private void OnClick(object sender, RoutedEventArgs e)
        {
        	UIElement element = (UIElement)sender;
	        SelectedStars = StarsPanel.Children.IndexOf(element) + 1;
        }
        
        
        private void OnChecked(object sender, EventArgs e)
        {
        	Activate();
        }
        
        
        private void OnUnchecked(object sender, EventArgs e)
        {
        	Deactivate(false);
        }
           
        
        public int SelectedStars
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
        		
        		OnChanged(new EventArgs());
        	}
        }
        
    	
    	protected override void PerformEnable()
    	{    		
    		ActivatableControl.EnableElement(StarsPanel);
    	}
    	
    	
    	protected override void PerformActivate()
    	{	
    		ActivatableControl.ActivateElement(StarsPanel);
    		ActivatableControl.EnableElement(ActivateCheckBox);
    		if ((bool)!ActivateCheckBox.IsChecked) {
    			ActivateCheckBox.IsChecked = true;
    		}
    	}
    	    	    	
        
    	protected override void PerformDeactivate(bool preventReactivation)
    	{
    		ActivatableControl.DeactivateElement(StarsPanel);
    		if ((bool)ActivateCheckBox.IsChecked) {
    			ActivateCheckBox.IsChecked = false;
    		}
    		if (preventReactivation) {
    			ActivatableControl.DeactivateElement(ActivateCheckBox);
    		}
    	}
    	
        
        protected override OptionalWorksheetPart GetWorksheetPartObject()
        {
        	return new Rating(maxStars,SelectedStars);
		}
        
        
//        protected override List<Control> GetActivationControls()
//        {
//        	List<Control> activationControls = new List<Control>(1);
//        	activationControls.Add(ActivateCheckBox);
//        	return activationControls;
//        }
    }
}