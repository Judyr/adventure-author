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
        private EventHandler<MagnetEventArgs> magnetControl_SendToBackHandler;
        private EventHandler<MagnetEventArgs> magnetControl_BringToFrontHandler;
    	
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
    		magnetControl_BringToFrontHandler = new EventHandler<MagnetEventArgs>(magnetControl_BringToFront);
    		magnetControl_SendToBackHandler = new EventHandler<MagnetEventArgs>(magnetControl_SendToBack);
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
        	magnet.bringToFrontMenuItem.IsEnabled = true;
        	magnet.sendToBackMenuItem.IsEnabled = true;
			magnet.Show();
			BringInsideBoard(magnet);			
			
			mainCanvas.Children.Add(magnet);
			BringToFront(magnet);
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
        		magnet.RequestBringToFront += magnetControl_BringToFrontHandler;
        		magnet.RequestSendToBack += magnetControl_SendToBackHandler;
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
        		magnet.RequestBringToFront -= magnetControl_BringToFrontHandler;
        		magnet.RequestSendToBack -= magnetControl_SendToBackHandler;
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
        	angle -= (angle % 2); // ensure angle is divisible by 2
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
    	
    	
    	public void BringToFront(MagnetControl magnet)
    	{
    		if (HasMagnet(magnet)) {
    			UpdateZOrder(magnet,true);
    		}
    	}
    	
    	
    	public void SendToBack(MagnetControl magnet)
    	{
    		if (HasMagnet(magnet)) {
    			UpdateZOrder(magnet,false);
    		}
    	}
    	
    	
    	/// <summary>
		/// Helper method used by the BringToFront and SendToBack methods.
		/// </summary>
		/// <param name="element">
		/// The element to bring to the front or send to the back.
		/// </param>
		/// <param name="bringToFront">
		/// Pass true if calling from BringToFront, else false.
		/// </param>
		/// <remarks>Josh Smith code from 'Dragging Elements in a Canvas' on CodeProject</remarks>
		private void UpdateZOrder( UIElement element, bool bringToFront )
		{
			#region Safety Check

			if( element == null )
				throw new ArgumentNullException( "element" );

			if( !mainCanvas.Children.Contains( element ) )
				throw new ArgumentException( "Must be a child element of the Canvas.", "element" );

			#endregion // Safety Check

			#region Calculate Z-Indici And Offset

			// Determine the Z-Index for the target UIElement.
			int elementNewZIndex = -1;
			if( bringToFront )
			{
				foreach( UIElement elem in mainCanvas.Children )
					if( elem.Visibility != Visibility.Collapsed )
						++elementNewZIndex;
			}
			else
			{
				elementNewZIndex = 0;
			}

			// Determine if the other UIElements' Z-Index 
			// should be raised or lowered by one. 
			int offset = (elementNewZIndex == 0) ? +1 : -1;

			int elementCurrentZIndex = Canvas.GetZIndex( element );

			#endregion // Calculate Z-Indici And Offset

			#region Update Z-Indici

			// Update the Z-Index of every UIElement in the Canvas.
			foreach( UIElement childElement in mainCanvas.Children )
			{
				if( childElement == element )
					Canvas.SetZIndex( element, elementNewZIndex );
				else
				{
					int zIndex = Canvas.GetZIndex( childElement );

					// Only modify the z-index of an element if it is  
					// in between the target element's old and new z-index.
					if( bringToFront && elementCurrentZIndex < zIndex ||
						!bringToFront && zIndex < elementCurrentZIndex )
					{
						Canvas.SetZIndex( childElement, zIndex + offset );
					}
				}
			}

			#endregion // Update Z-Indici
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
        	e.Handled = false;
        	OnDrop(e);
        }          
        
        
        private void magnetControl_BringToFront(object sender, MagnetEventArgs e)
        {
        	BringToFront(e.Magnet);
        }       
        
        
        private void magnetControl_SendToBack(object sender, MagnetEventArgs e)
        {
        	SendToBack(e.Magnet);
        }
        
        #endregion
    }
}