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
using System.Windows.Shapes;
using AdventureAuthor.Analysis;
using AdventureAuthor.Utils;
using form = NWN2Toolset.NWN2ToolsetMainForm;
using NWN2Toolset.NWN2.Views;
using NWN2Toolset.NWN2.Data;
using AdventureAuthor.Analysis.UI;

namespace AdventureAuthor.Analysis
{
    /// <summary>
    /// Interaction logic for CombatMap.xaml
    /// </summary>
    public partial class CombatMap : Window
    {
    	#region Fields
    	
    	private AreaMapControl map;    	
		public AreaMapControl Map {
			get { return map; }
		}
    	    	
    	#endregion
    	
    	
    	#region Constructors
    	
        public CombatMap()
        {
            InitializeComponent();
            
            map = new AreaMapControl();
            MapContainer.Children.Add(map);
            INWN2Viewer viewer = form.App.GetActiveViewer();
            if (viewer is NWN2AreaViewer) {
            	NWN2AreaViewer areaViewer = (NWN2AreaViewer)viewer;
            	Open(areaViewer.Area);
	           	this.Activated += delegate {
	           		Refresh();
	           	};
            }
            else {
            	Title = "No area selected";
            }
        }
        
        #endregion Constructors
        
        
        #region Methods
        
        public void Open(NWN2GameArea area)
        {
        	map.LoadArea(area);
        	
        	foreach (UIElement element in map.AreaMapCanvas.Children) {
        		Log.WriteMessage("child");
        		if (element is CreatureMarker) {
        			Log.WriteMessage("child is creaturemarker");
        			CreatureMarker marker = (CreatureMarker)element;
        			marker.MouseDown += delegate { Say.Information("RAR!"); };
        			marker.MouseEnter += new MouseEventHandler(OnMouseEnter_Marker);
        			marker.MouseLeave += new MouseEventHandler(OnMouseLeave_Marker);
        			Log.WriteMessage("attached handlers");
        		}
        	}
        	
        	Title = area.Name;
        }
        
        
        private void Refresh()
        {
        	map.LoadArea(map.Area);
        }
        
        #endregion Methods        
        
        
        #region Event handlers
                        
        private void OnMouseEnter_Marker(object sender, MouseEventArgs e)
        {
        	Log.WriteMessage("start enter");
        	CreatureInfoText.Text = sender.ToString();
        	this.Background = ((CreatureMarker)sender).Background;
        	Log.WriteMessage("finish enter");
        }
        
        
        private void OnMouseLeave_Marker(object sender, MouseEventArgs e)
        {
        	Log.WriteMessage("start leave");
        	if (CreatureInfoText.Text == sender.ToString()) {
        		CreatureInfoText.Text = String.Empty;
        	}
        	Log.WriteMessage("finish enter");
        }
        
        #endregion
    }
}