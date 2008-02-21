using System;
using System.Windows;
using System.Windows.Controls;
using AdventureAuthor.Core;
using AdventureAuthor.Utils;
using System.IO;

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
    	
    	private Random random = new Random();
        
    	
        private EventHandler<MagnetEventArgs> magnetControl_SelectedHandler;     
        private DragEventHandler magnetControl_DropHandler;
    	
    	#endregion
    	
    	#region Events
    	
    	public event EventHandler<MagnetEventArgs> MagnetAdded;    	
		protected virtual void OnMagnetAdded(MagnetEventArgs e)
		{
			EventHandler<MagnetEventArgs> handler = MagnetAdded;
			if (handler != null) {
				handler(this, e);
			}
		}
		
    	
    	public event EventHandler<MagnetRemovedEventArgs> MagnetRemoved;    	
		protected virtual void OnMagnetRemoved(MagnetRemovedEventArgs e)
		{
			EventHandler<MagnetRemovedEventArgs> handler = MagnetRemoved;
			if (handler != null) {
				handler(this, e);
			}
		}
		
    	
    	public event EventHandler<MagnetEventArgs> MagnetSelected;    	
		protected virtual void OnMagnetSelected(MagnetEventArgs e)
		{
			EventHandler<MagnetEventArgs> handler = MagnetSelected;
			if (handler != null) {
				handler(this, e);
			}
		}
				
    	#endregion
    	
    	#region Constructors
    	
    	public MagnetBoardControl()
    	{
    		magnetControl_SelectedHandler = new EventHandler<MagnetEventArgs>(magnetControl_Selected); 
    		magnetControl_DropHandler = new DragEventHandler(magnetControl_Drop);
    		InitializeComponent();    		
    	}
    	
    	
    	public MagnetBoardControl(MagnetBoardInfo boardInfo) : this()
    	{
    		Open(boardInfo);
    	}
        
        #endregion
        
        #region Methods    
        
        public void Open(MagnetBoardInfo boardInfo)
        {
        	Clear();
    		foreach (MagnetInfo magnetInfo in boardInfo.Magnets) {
    			MagnetControl magnet = (MagnetControl)magnetInfo.GetControl();
    			AddMagnet(magnet,false);
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
	
        	AddHandlers(magnet);
        	
			magnet.Margin = new Thickness(0); // margin may have been added if the magnet was in the magnetlist
			magnet.Show();
			BringInsideBoard(magnet);
			
			mainCanvas.Children.Add(magnet);
			OnMagnetAdded(new MagnetEventArgs(magnet));
        }
        
        
        public void RemoveMagnet(MagnetControl magnet)
        {
        	mainCanvas.Children.Remove(magnet);
        	RemoveHandlers(magnet);	        	
	        OnMagnetRemoved(new MagnetRemovedEventArgs(magnet,true));
        }
        
        
        private void AddHandlers(MagnetControl magnet)
        {
        	try {
        		magnet.Selected += magnetControl_Selected;
        		magnet.Drop += magnetControl_Drop;
        	}
        	catch (Exception e) {
        		Say.Error("Failed to add handlers to magnet.",e);
        	}
        }
        
        
        private void RemoveHandlers(MagnetControl magnet)
        {
        	try {
        		magnet.Selected -= magnetControl_Selected;
        		magnet.Drop -= magnetControl_Drop;
        	}
        	catch (Exception e) {
        		Say.Error("Failed to remove handlers from magnet.",e);
        	}
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
    		        
    	private void magnetControl_Selected(object sender, MagnetEventArgs e)
    	{
    		OnMagnetSelected(e);
    	} 
    		
        
        /// <summary>
        /// Treat drop events on magnets as you would drop events on the list itself.
        /// </summary>
        private void magnetControl_Drop(object sender, DragEventArgs e)
        {
        	OnDrop(e);
        }
        
        #endregion
    }
}