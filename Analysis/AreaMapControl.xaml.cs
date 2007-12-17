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
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.Instances;
using NWN2Toolset.NWN2.Data.TypedCollections;
using OEIShared.OEIMath;
using AdventureAuthor.Utils;
using AdventureAuthor.Analysis;
using AdventureAuthor.Analysis.UI;

namespace AdventureAuthor.Analysis
{
    public partial class AreaMapControl : UserControl
    {    	
    	private const double SIZE_MULTIPLIER = 2;
    	   	    	
    	
    	/// <summary>
    	/// The width of the portion of the area that is outside the 'white box', and is therefore not fully usable.
    	/// </summary>
    	private const double OUT_OF_BOUNDS_WIDTH = 80 * SIZE_MULTIPLIER;
    	
    	
    	/// <summary>
    	/// The area this map represents.
    	/// </summary>
    	private NWN2GameArea area;    	
		public NWN2GameArea Area {
			get { return area; }
		}
    	    	
    	
    	/// <summary>
    	/// Create a map control that represents a game area.
    	/// </summary>
        public AreaMapControl()
        {
            InitializeComponent();
        }
        
        
        /// <summary>
        /// Create a map control that represents a game area.
        /// </summary>
        /// <param name="area">The area to represent</param>
        public AreaMapControl(NWN2GameArea area) : this()
        {
        	LoadArea(area);
        }
        
        
        /// <summary>
        /// Display a map based on a given game area.
        /// </summary>
        /// <param name="area">The area to represent</param>
        public void LoadArea(NWN2GameArea area)
        {
        	AreaMapCanvas.Children.Clear();
            this.area = area;
            BoundingBox3 boundingBox = area.GetBoundsOfArea();
            this.Width = boundingBox.Length * SIZE_MULTIPLIER;
            this.Height = boundingBox.Height * SIZE_MULTIPLIER;           
            Populate();
        }

        
        public void Populate()
        {
        	foreach (NWN2CreatureInstance creature in area.Creatures) {
        		try {
        			CreatureMarker marker = new CreatureMarker(creature);   
        			double fromLeft = creature.Position.X * SIZE_MULTIPLIER;
        			double fromBottom = creature.Position.Y * SIZE_MULTIPLIER;
        			if (area.HasTerrain) {
        				fromLeft -= OUT_OF_BOUNDS_WIDTH;
        				fromBottom -= OUT_OF_BOUNDS_WIDTH;
        			}
        			Canvas.SetLeft(marker,fromLeft);
        			Canvas.SetBottom(marker,fromBottom);
	        		AreaMapCanvas.Children.Add(marker);
        		}
        		catch (Exception e) {
        			Say.Error(e);
        		}
        	}
        }
    }
}