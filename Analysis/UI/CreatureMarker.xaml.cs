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
using System.Windows.Media.Animation;
using NWN2Toolset.NWN2.Data.Instances;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Analysis.UI
{
    /// <summary>
    /// Interaction logic for CreatureMarker.xaml
    /// </summary>

    public partial class CreatureMarker : UserControl
    {    	
    	#region Constants
    	
    	internal readonly Brush FRIENDLY_BRUSH = Brushes.LightBlue;
    	internal readonly Brush WEAK_BRUSH = Brushes.LightGreen;
    	internal readonly Brush MEDIUM_BRUSH = Brushes.Yellow;
    	internal readonly Brush STRONG_BRUSH = Brushes.Red;
    	
    	private const float WEAK_RATING = 5;
    	private const float MEDIUM_RATING = 15;
    	
    	#endregion
    	
    	
    	private NWN2CreatureInstance creature;    	
		public NWN2CreatureInstance Creature {
			get { return creature; }
		}
    	
    	
    	private float challengeRating;    	
		public float ChallengeRating {
			get { return challengeRating; }
		}
    	    	
    	
    	public Brush MarkerBrush {
    		get {
    			return (Brush)Resources["MarkerBrush"];
    		}
    		set {
    			try {
    				Resources["MarkerBrush"] = value;
    			}
    			catch (KeyNotFoundException) {
    				Resources.Add("MarkerBrush",value);
    			}
    		}
    	}
    	
    	
    	public double Radius {
    		get {
    			return (double)Resources["Radius"];
    		}
    		set {
    			try {
    				Resources["Radius"] = value;
    			}
    			catch (KeyNotFoundException) {
    				Resources.Add("Radius",value);
    			}
    		}
    	}
    	
    	
    	
    	//private static ColorAnimation pulseAnimation = new ColorAnimation(Colors.AliceBlue,Colors.White,Duration.Forever);
        //pulseAnimation.AutoReverse = true;  
        //this.BeginAnimation(ellipse.Fill,pulseAnimation);
    	
        
        
        public CreatureMarker(NWN2CreatureInstance creature)
        {
        	Radius = 5;
        	this.creature = creature;        	
        	UpdateChallengeRating();
            InitializeComponent();
        }

                
        private void UpdateChallengeRating()
        {
        	Brush brush;
        	if (creature.FactionID != Combat.HOSTILE) {
        		brush = FRIENDLY_BRUSH;
        	}
        	else {
	        	challengeRating = Combat.GetChallengeRating(creature);
	        	if (challengeRating <= WEAK_RATING) {
	        		brush = WEAK_BRUSH;
	        	}
	        	else if (challengeRating <= MEDIUM_RATING) {
	        		brush = MEDIUM_BRUSH;
	        	}
	        	else {
	        		brush = STRONG_BRUSH;
	        	}
        	}
        	MarkerBrush = brush;
        }
        
        
        public override string ToString()
        {
        	return creature.Name + " (" + creature.Tag + ")";
        }
    }
}