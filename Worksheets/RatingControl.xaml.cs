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
using AdventureAuthor.Evaluation;

namespace AdventureAuthor.Evaluation
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
    	
    	
    	private string previousValue;
    	
    	#endregion                
    	
    	#region Constructors
        
        
    	public RatingControl(Rating rating)
        {
        	properName = "StarRating";        	
        	
    		if (rating == null) {
    			rating = new Rating();
    		}
        	
        	previousValue = rating.Value;
    		
    		InitializeComponent();
            LoadStars(rating.Max);
            SetInitialActiveStatus(rating);
            
        	try {
	        	int ratingValue = int.Parse(rating.Value);
	        	SelectedStars = ratingValue;
        	}
			catch (Exception) {
    			Say.Debug(rating.Value + " is not a valid numerical rating.");
				SelectedStars = 0;
        	}
            
            ToolTip = "How much do you agree with the question?\n" +
            		  "1 star - I do not agree at all.\n" +
            		  "5 stars - I agree completely.";
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
        	int width = maxStars * 40;
        	if (WorksheetViewer.Instance.EvaluationMode == Mode.Design) {
        		width += 30; // leave space for the activation checkbox
        	}
        	Width = width;
        }
        
        
        /// <summary>
        /// Highlight all the stars prior to and including the one that's been clicked
        /// </summary>
        private void OnClick(object sender, RoutedEventArgs e)
        {
        	UIElement element = (UIElement)sender;
	        SelectedStars = StarsPanel.Children.IndexOf(element) + 1;
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
        		if (value.ToString() != previousValue) {
        			Log.WriteAction(LogAction.edited,"starrating",value + " stars");
        			previousValue = value.ToString();
        		}        		
        		
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
    		ActivateCheckBox.ToolTip = "Click to deactivate this answer field\n(will not appear in worksheet)";
    	}
    	    	    	
        
    	protected override void PerformDeactivate(bool parentIsDeactivated)
    	{
    		ActivatableControl.DeactivateElement(StarsPanel);
    		if ((bool)ActivateCheckBox.IsChecked) {
    			ActivateCheckBox.IsChecked = false;
    		}
    		if (parentIsDeactivated) {
    			ActivatableControl.DeactivateElement(ActivateCheckBox);
    		}
    		ActivateCheckBox.ToolTip = "Click to activate this answer field\n(will appear in worksheet)";
    	}
    	
		public override void ShowActivationControls()
		{
			ActivateCheckBox.Visibility = Visibility.Visible;
		}
		
		public override void HideActivationControls()
		{
			ActivateCheckBox.Visibility = Visibility.Collapsed;
		}
    	
        
        protected override OptionalWorksheetPart GetWorksheetPartObject()
        {
        	return new Rating(maxStars,SelectedStars);
		}
    }
}