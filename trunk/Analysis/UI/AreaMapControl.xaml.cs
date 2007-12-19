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
    	/// <summary>
    	/// Amount to magnify the overall map by. 
    	/// <remarks>Note that there is also a zoom on the map, but this method stops the map markers
    	/// from getting too small, as they are already only a few pixels wide.</remarks>
    	/// </summary>
    	private const double SIZE_MULTIPLIER = 2;
    	
    	
    	/// <summary>
    	/// The amount to multiply the height and width of tiles so that their relative size compared to the map is correct.
    	/// </summary>
    	private const double TILE_SIZE_MULTIPLER = 9 * SIZE_MULTIPLIER;
    	   	    	
    	
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
        	// Draw the map area:
        	AreaMapCanvas.Children.Clear();
            this.area = area;
            BoundingBox3 boundingBox = area.GetBoundsOfArea();
            this.Width = boundingBox.Length * SIZE_MULTIPLIER;
            this.Height = boundingBox.Height * SIZE_MULTIPLIER;
            
            // If the area is a tiled interior, draw the tiles:
            if (!area.HasTerrain) {
            	foreach (NWN2GameAreaTileData tile in area.Tiles) {
            		if (tile.HasBeenPlaced) {
            			Rectangle rect = new Rectangle();
            			rect.Width = tile.TileWidth * TILE_SIZE_MULTIPLER;
            			rect.Height = tile.TileHeight * TILE_SIZE_MULTIPLER;
            			Canvas.SetLeft(rect,tile.TileXPosition * TILE_SIZE_MULTIPLER);
            			Canvas.SetBottom(rect,tile.TileYPosition * TILE_SIZE_MULTIPLER);
            			Canvas.SetZIndex(rect,1); // behind markers
            			rect.Fill = Brushes.DarkGray;
            			rect.Stroke = Brushes.Black;
            			//rect.StrokeThickness = 1;
	        			AreaMapCanvas.Children.Add(rect);
            		}
            	}
            }
            
            Populate();
        }

        
        /// <summary>
        /// Populate the map with creature markers.
        /// </summary>
        public void Populate()
        {
        	foreach (NWN2CreatureInstance creature in area.Creatures) {
        		try {
        			CreatureMarker marker = new CreatureMarker(creature);   
        			double fromLeft = creature.Position.X * SIZE_MULTIPLIER - (marker.Radius / 2);
        			double fromBottom = creature.Position.Y * SIZE_MULTIPLIER - (marker.Radius / 2);
        			if (area.HasTerrain) {
        				fromLeft -= OUT_OF_BOUNDS_WIDTH;
        				fromBottom -= OUT_OF_BOUNDS_WIDTH;
        			}
        			Canvas.SetLeft(marker,fromLeft);
        			Canvas.SetBottom(marker,fromBottom);
        			Canvas.SetZIndex(marker,2); // in front of tiles
	        		AreaMapCanvas.Children.Add(marker);
        		}
        		catch (Exception e) {
        			Say.Error(e);
        		}
        	}
        }
    }
}