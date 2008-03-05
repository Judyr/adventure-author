using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Media.Animation;
using NWN2Toolset.NWN2.Data.Instances;
using AdventureAuthor.Core;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Analysis.UI
{
    /// <summary>
    /// Interaction logic for CreatureMarker.xaml
    /// </summary>

   	public enum DangerLevel {
   		Friendly,
   		Weak,
    	Medium,
    	Strong,
    	Impossible
    }
    
    
    public partial class CreatureMarker : UserControl
    {    	
    	#region Constants
    	
    	internal static readonly Brush DEFAULT_BRUSH = Brushes.Silver;
    	internal static readonly Brush FRIENDLY_BRUSH = Brushes.WhiteSmoke;
    	internal static readonly Brush WEAK_BRUSH = Brushes.Lime;
    	internal static readonly Brush MEDIUM_BRUSH = Brushes.Yellow;
    	internal static readonly Brush STRONG_BRUSH = Brushes.Red;
    	internal static readonly Brush IMPOSSIBLE_BRUSH = Brushes.Purple;
    	
    	internal const float WEAK_RATING = 5;
    	internal const float MEDIUM_RATING = 15;
    	internal const float STRONG_RATING = 35;
    	
    	
    	#endregion
    	
    	
    	private NWN2CreatureInstance creature;    	
		public NWN2CreatureInstance Creature {
			get { return creature; }
		}
    	
    	
    	private float challengeRating;    	
		public float ChallengeRating {
			get { return challengeRating; }
		}
    	
    	
    	private DangerLevel danger;    	
		public DangerLevel Danger {
			get { return danger; }
			set { 
				danger = value;				
		       	Brush brush;
				switch (danger) {
		       		case DangerLevel.Friendly:
		        		brush = FRIENDLY_BRUSH;
		        		break;
		        	case DangerLevel.Weak:
		        		brush = WEAK_BRUSH;
		        		break;
		        	case DangerLevel.Medium:
		        		brush = MEDIUM_BRUSH;
		        		break;
		        	case DangerLevel.Strong:
		        		brush = STRONG_BRUSH;
		        		break;
		        	case DangerLevel.Impossible:
		        		brush = IMPOSSIBLE_BRUSH;
		        		break;
		        	default:
		        		brush = DEFAULT_BRUSH;
		        		break;
		       	}
		        MarkerBrush = brush;
			}
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
        
        
        
        /// <summary>
        /// For creating a functionless visual marker, usable in keys. Not for public consumption.
        /// </summary>
        /// <param name="danger">The danger level of the creature represented, from Weak to Impossible, or alternatively Friendly.</param>
        internal CreatureMarker(DangerLevel danger)
        {
        	Radius = 5;
        	MarkerBrush = DEFAULT_BRUSH;
        	this.creature = null;
        	this.danger = danger;
        	InitializeComponent();
        }
        
        
        public CreatureMarker(NWN2CreatureInstance creature)
        {
        	MarkerBrush = DEFAULT_BRUSH;
        	this.creature = creature;      
        	// Base marker size on the intended size of creature. Could be multiplying this by 
        	// Scale here, but the creature size seems to be very general - e.g. trolls are
        	// same size as dragons even though the dragon is about 10 times as big as the troll
        	// on the map - so I think this would just confuse things (e.g. a triple-sized human,
        	// on the area map much smaller than a dragon, would be bigger than a dragon on the
        	// map view.)
        	Radius = creature.LookupCreatureSize();
        	UpdateChallengeRating();
            InitializeComponent();
            
            // TODO - attaches to everything, despite conversation check. Also pic doesn't look great.
            if (creature.Conversation != null && creature.Conversation.FullName != String.Empty) {   
            	Content = ResourceHelper.GetImage("speechbubblesblue.png");
            }
        }

                
        private void UpdateChallengeRating()
        {
        	if (creature != null) {
	        	if (creature.FactionID != Combat.HOSTILE) {
	        		Danger = DangerLevel.Friendly;
	        	}
	        	else {
		        	challengeRating = Combat.GetChallengeRating(creature);
		        	if (challengeRating <= WEAK_RATING) {
	        			Danger = DangerLevel.Weak;
		        	}
		        	else if (challengeRating <= MEDIUM_RATING) {
	        			Danger = DangerLevel.Medium;
		        	}
		        	else if (challengeRating <= STRONG_RATING) {
	        			Danger = DangerLevel.Strong;
		        	}
		        	else {
	        			Danger = DangerLevel.Impossible;
		        	}
	        	}
        	}
        }
                
        
        public override string ToString()
        {
        	if (creature == null) {
        		return GetType().ToString();
        	}
        	else {
        		return creature.Name + " (" + creature.Tag + ")";
        	}
        }
    }
}