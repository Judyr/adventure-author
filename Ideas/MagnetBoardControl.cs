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
using System.Xml.Serialization;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Ideas
{
    /// <summary>
    /// Interaction logic for MagnetBoard.xaml
    /// </summary>
    public partial class MagnetBoardControl : Board
    {
    	#region Fields
    	
    	private string filename;    	
		public string Filename {
			get { return filename; }
			set { 
				filename = value;
				//UpdateTitleBar();
			}
		}    
    	
    	
    	private MagnetControl selectedMagnet;     	
		public MagnetControl SelectedMagnet {
			get { return selectedMagnet; }
			set {
	    		if (selectedMagnet != value) {
					if (selectedMagnet != null) {
		    			selectedMagnet.Deselect();
					}
		    		selectedMagnet = value;
		    		selectedMagnet.Select();
	    		}
			}
		}
    	
    	
    	private Random random = new Random();
    	
    	#endregion
    	
    	#region Constructors
    	
    	public MagnetBoardControl()
    	{
    		InitializeComponent();
    	}
    	
    	public MagnetBoardControl(MagnetBoardInfo boardInfo) : this()
    	{
    		foreach (MagnetInfo magnetInfo in boardInfo.Magnets) {
    			MagnetControl magnetControl = new MagnetControl(magnetInfo);
    			mainCanvas.Children.Add(magnetControl);
    		}
    	}
        
        #endregion
        
        #region Methods
        
        public void Open(MagnetBoardInfo boardInfo)
        {
        	foreach (MagnetInfo magnetInfo in boardInfo.Magnets) {
        		MagnetControl magnetControl = (MagnetControl)magnetInfo.GetControl();
        		AddMagnet(magnetControl,false);
	    	}
        }
        
        
        public void AddMagnet(MagnetControl magnet)
        {
        	AddMagnet(magnet,false);
        }
        
        
        /// <summary>
        /// Add a magnet to the board.
        /// </summary>
        /// <param name="magnet">The magnet to add</param>
        /// <param name="randomPosition">True to add magnet at a random location and angle; false
        /// to add magnet according to its preset location and angle</param>
        public void AddMagnet(MagnetControl magnet, bool randomPosition)
        {
        	if (magnet == null) {
        		throw new ArgumentNullException("Tried to add a null object to the board.");
        	}

			if (randomPosition) {
	        	Point location = GetRandomLocation();
	        	double angle = GetRandomAngle();
	        	magnet.X = location.X;
	        	magnet.Y = location.Y;
	        	magnet.Angle = angle;
			}        	
	
			magnet.Margin = new Thickness(0); // margin may have been added if the magnet was in the magnetlist
			BringInsideBoard(magnet);
			
			magnet.MouseDown += new MouseButtonEventHandler(MagnetControl_MouseDown);
			
			mainCanvas.Children.Add(magnet);
        }
        
        
        private double GetRandomAngle()
        {
        	double angle = (30 * random.NextDouble());    
        	if (DateTime.Now.Millisecond % 2 == 0) {
        		angle = 360 - angle; // randomly angle to the left instead of the right
        	}
        	return angle;
        }
        
        
        private Point GetRandomLocation()
        {
        	double x = random.NextDouble() * (ActualWidth - MagnetControl.MAGNET_MAX_WIDTH);
        	double y = random.NextDouble() * ActualHeight;
        	return new Point(x,y);
        }
        
        
        /// <summary>
        /// Changes a magnet's position so that it doesn't extend over the edge of this board.
        /// </summary>
        /// <param name="magnet">The magnet to change the position of</param>
        private void BringInsideBoard(MagnetControl magnet)
        {
        	double rightmost = Canvas.GetLeft(magnet) + magnet.Width;
        	double bottommost = Canvas.GetTop(magnet) + magnet.Height;
        	
        	if (rightmost > mainCanvas.Width) {
        		Canvas.SetLeft(magnet,mainCanvas.Width - MagnetControl.MAGNET_MAX_WIDTH);
        	}
        	if (bottommost > mainCanvas.Height) {
        		Canvas.SetTop(magnet,mainCanvas.Height - 50);
        	}
        }
                 
        
        public void Clear()
        {
        	mainCanvas.Children.Clear();
        }
    	
        
    	public void Save()
    	{
    		if (filename == null) {
    			throw new InvalidOperationException("Save failed: Should not have called Save without first setting a filename.");
    		}
    		else {
	    		AdventureAuthor.Utils.Serialization.Serialize(Filename,this.GetSerializable());
//	    		if (Dirty) {
//	    			Dirty = false; TODO
//	    		}
    		}
    	}
    	
    	
    	public bool HasMagnet(MagnetControl magnet)
    	{
    		return mainCanvas.Children.Contains(magnet);
    	}
        
        
    	public override ISerializableData GetSerializable()
    	{
    		return new MagnetBoardInfo(this);
    	}
        
        #endregion
        
        #region Event handlers
        
        private void MagnetControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
        	MagnetControl magnet = (MagnetControl)e.Source;
        	if (HasMagnet(magnet)) {
        		SelectedMagnet = magnet;
        	}
        }
        
        #endregion
    }
}