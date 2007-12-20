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
using OEIShared.Utils;
using Microsoft.DirectX;
using NWN2Toolset.NWN2.Data.Instances;

namespace AdventureAuthor.Analysis.UI
{
    /// <summary>
    /// Interaction logic for TriggerMarker.xaml
    /// </summary>

    public partial class TriggerMarker : UserControl
    {
    	private const double TRIGGER_SIZE_INCREASE = 2;
    	
    	
        public TriggerMarker(NWN2TriggerInstance trigger)
        { 
        	PointCollection triggerPoints = new PointCollection();
            foreach (Vector3 v in trigger.Geometry) {
            	Point p = new Point(v.X * TRIGGER_SIZE_INCREASE,-v.Y * TRIGGER_SIZE_INCREASE);
            	triggerPoints.Add(p);
            }
        	Resources.Add("TriggerPoints",triggerPoints);
        	
            InitializeComponent();
        }

    }
}