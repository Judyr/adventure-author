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
    	#region Constants
    	
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
    	
    	
    	private const int ZINDEX_CREATURE = 3;
    	private const int ZINDEX_TRIGGER = 2;
    	private const int ZINDEX_TILE = 1;
    	
    	#endregion
    	
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
            			Canvas.SetZIndex(rect,ZINDEX_TILE); // behind markers
            			rect.Fill = GetTileFill(tile);
            			rect.StrokeThickness = 0;
            			AreaMapCanvas.Children.Add(rect); 
            		}
            	}
            }
            
            PopulateCreatures();
            PopulateTriggers();
        }
        
        
        /// <summary>
        /// Get the fill colour of this tile, so tiles are never adjacent to another tile of the same colour.
        /// </summary>
        /// <param name="tile">The tile to return the fill colour for</param>
        /// <returns>The brush to fill this tile</returns>
        private Brush GetTileFill(NWN2GameAreaTileData tile) 
        {
        	Brush a = Brushes.DarkGray;
        	Brush b = Brushes.Gray;
        	
        	if (tile.TileXPosition % 2 == 0) {
        		if (tile.TileYPosition % 2 == 0) {
        			return a;
        		}
        		else {
        			return b;
        		}
        	}
        	else {
        		if (tile.TileYPosition % 2 == 0) {
        			return b;
        		}
        		else {
        			return a;
        		}
        	}
        }

        
        /// <summary>
        /// Populate the map with creature markers.
        /// </summary>
        public void PopulateCreatures()
        {
        	try {
        		foreach (NWN2CreatureInstance creature in area.Creatures) {
        			Mark(creature);
        		}
        	}
        	catch (Exception e) {
        		Say.Error(e);
        	}
        }

                
        /// <summary>
        /// Populate the map with trigger markers.
        /// </summary>
        public void PopulateTriggers()
        {
        	try {
        		foreach (NWN2TriggerInstance trigger in area.Triggers) {
        			Mark(trigger);
        		}
        	}
        	catch (Exception e) {
        		Say.Error(e);
        	}
        }        
        
        
        private void Mark(NWN2CreatureInstance creature) 
        {
        	CreatureMarker marker = new CreatureMarker(creature);
        	double offset = marker.Radius / 2;
        	Place(marker,creature.Position.X,creature.Position.Y,offset);
        }
        
        
        private void Mark(NWN2TriggerInstance trigger) 
        {
        	TriggerMarker marker = new TriggerMarker(trigger);
        	double offset = 0;
        	Place(marker,trigger.Position.X,trigger.Position.Y,offset);
        }
        
        
        private void Place(Control marker, double x, double y, double offset)
        {
        	double fromLeft, fromBottom;
        	fromLeft = x * SIZE_MULTIPLIER - offset;
        	fromBottom = y * SIZE_MULTIPLIER - offset;
        	if (area.HasTerrain) {
        		fromLeft -= OUT_OF_BOUNDS_WIDTH;
        		fromBottom -= OUT_OF_BOUNDS_WIDTH;
        	}
        	Canvas.SetLeft(marker,fromLeft);
        	Canvas.SetBottom(marker,fromBottom);
        	Canvas.SetZIndex(marker,GetZIndex(marker));
	        AreaMapCanvas.Children.Add(marker);
        }
        
        
        private int GetZIndex(Control c) 
        {
        	if (c is CreatureMarker) {
        		return ZINDEX_CREATURE;
        	}
        	else if (c is TriggerMarker) {
        		return ZINDEX_TRIGGER;
        	}
        	else {
        		return ZINDEX_TILE;
        	}
//        	else {
//        		deal with tile type	TODO
//        	}
        }
        
        
        // NB: Different Canvas layers with transparent Backgrounds, e.g. TriggerLayer, CreatureLayer,
        // that can then be made visible or invisible according to map preferences. Or might be better to
        // bind visibility of individual markers to the preference properties? Not sure which would be more
        // efficient.
    }
}