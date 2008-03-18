using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using AdventureAuthor.Core;
using AdventureAuthor.Utils;
using System.IO;
using System.Collections.Generic;

namespace AdventureAuthor.Ideas
{
    /// <summary>
    /// Interaction logic for MagnetBoard.xaml
    /// </summary>
    public partial class MagnetBoardControl : Board
    {
    	#region Constants
    	
    	/// <summary>
    	/// The maximum number of degrees a magnet may deviate from an angle of 0 (in either direction.)
    	/// </summary>  
    	/// <remarks>Note that this constant is repeated in MagnetBoardControl and MagnetListControl
    	/// with different values</remarks>  	
    	public const double MAXIMUM_ANGLE_IN_EITHER_DIRECTION = 30;
    	
    	#endregion
    	
    	#region Fields
    	
    	private string filename;    	
		public string Filename {
			get { return filename; }
			set { 
				filename = value;
			}
		}    
    	
    	
		public Color SurfaceColour {
			get { 
    			return ((LinearGradientBrush)Resources["fridgeBrush"]).GradientStops[0].Color;
    		}
			set { 
    			((LinearGradientBrush)Resources["fridgeBrush"]).GradientStops[0].Color = value; 
    			MakeDirty();
    		}
		}
    	
    	
    	private Color defaultSurfaceColour;
		public Color DefaultSurfaceColour {
			get { 
    			return (Color)Resources["defaultSurfaceColour"];
    		}
			set { 
    			Resources["defaultSurfaceColour"] = value; 
    		}
		}
    	
    	
    	/// <summary>
    	/// True if the magnet board has changed since the last save; false otherwise.
    	/// </summary>
    	private bool dirty = false;    	
		internal bool Dirty {
			get { return dirty; }
			set {
				dirty = value;
				OnChanged(new EventArgs());
			}
		}
    	
    	
    	private Random random = new Random();        
    	
    	
        private DragEventHandler magnetControl_DropHandler;
        private EventHandler magnetControl_SendToBackHandler;
        private EventHandler magnetControl_BringToFrontHandler;
        private EventHandler magnetControl_EditedHandler;
    	
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
		
    	
    	public event EventHandler<MagnetEventArgs> MagnetDeleted;    	
		protected virtual void OnMagnetDeleted(MagnetEventArgs e)
		{
			EventHandler<MagnetEventArgs> handler = MagnetDeleted;
			if (handler != null) {
				handler(this, e);
			}
		}
		
    	
    	public event EventHandler<MagnetEventArgs> MagnetMoved;    	
		protected virtual void OnMagnetMoved(MagnetEventArgs e)
		{
			EventHandler<MagnetEventArgs> handler = MagnetMoved;
			if (handler != null) {
				handler(this, e);
			}
		}
    	
    	
    	public event EventHandler<MagnetEventArgs> MagnetTransferredOut;    	
    	protected virtual void OnMagnetTransferredOut(MagnetEventArgs e)
    	{
    		EventHandler<MagnetEventArgs> handler = MagnetTransferredOut;
    		if (handler != null) {
    			handler(this,e);
    		}
    	}
		
    	
    	public event EventHandler Opened;    	
		protected virtual void OnOpened(EventArgs e)
		{
			EventHandler handler = Opened;
			if (handler != null) {
				handler(this, e);
			}
		}
		
    	
    	public event EventHandler Closed;    	
		protected virtual void OnClosed(EventArgs e)
		{
			EventHandler handler = Closed;
			if (handler != null) {
				handler(this, e);
			}
		}
		
    	
    	public event EventHandler Changed;    	
		protected virtual void OnChanged(EventArgs e)
		{
			EventHandler handler = Changed;
			if (handler != null) {
				handler(this, e);
			}
		}
				
    	#endregion
    	
    	#region Constructors
    	    	
    	public MagnetBoardControl()
    	{
    		magnetControl_DropHandler = new DragEventHandler(magnetControl_Drop);
    		magnetControl_BringToFrontHandler = new EventHandler(magnetControl_BringToFront);
    		magnetControl_SendToBackHandler = new EventHandler(magnetControl_SendToBack);
    		magnetControl_EditedHandler = new EventHandler(magnetBoard_Changed);
    		Drop += new DragEventHandler(magnetBoard_Drop);
    		
    		MagnetAdded += delegate { MakeDirty(); };
    		MagnetDeleted += delegate { MakeDirty(); };
    		MagnetMoved += delegate { MakeDirty(); };
    		  		    		
    		InitializeComponent();    		
    	}
    	
    	
    	public MagnetBoardControl(MagnetBoardInfo boardInfo) : this()
    	{
    		Open(boardInfo);
    	}
    	        
        #endregion
        
        #region Methods   
       
        /// <summary>
        /// Open a magnet board from serialized data.
        /// </summary>
        /// <param name="filename">The location of the serialized data file</param>
        public void Open(string filename)
        {
    		if (!File.Exists(filename)) {
    			throw new ArgumentException(filename + " could not be found.");
    		}
        	
        	try {
	    		object o = Serialization.Deserialize(filename,typeof(MagnetBoardInfo));
		    	MagnetBoardInfo boardInfo = (MagnetBoardInfo)o;
        		OpenBoard(boardInfo);
		    	Filename = filename;
	        	OnOpened(new EventArgs());
        	}
        	catch (Exception e) {
        		Say.Error("Failed to open magnet board.",e);
        		CloseBoard();
        	}
        }
        
        
        /// <summary>
        /// Open a magnet board from serialized data, without raising an Opened event.
        /// </summary>
        /// <param name="boardInfo">Serialized data describing the board to be opened</param>
        private void OpenBoard(MagnetBoardInfo boardInfo)
        {
	        CloseBoard();
	        SurfaceColour = boardInfo.SurfaceColour;
	    	foreach (MagnetInfo magnetInfo in boardInfo.Magnets) {
	    		MagnetControl magnet = (MagnetControl)magnetInfo.GetControl();
	    		AddMagnet(magnet,false);
	    	}
	        dirty = false; // cancel effect of adding magnets when opening (and don't raise Changed event)
        }
        
        
        /// <summary>
        /// Open a magnet board from serialized data.
        /// </summary>
        /// <param name="boardInfo">Serialized data describing the board to be opened</param>
        public void Open(MagnetBoardInfo boardInfo)
        {
        	try {
        		OpenBoard(boardInfo);
	        	OnOpened(new EventArgs());
        	}
        	catch (Exception e) {
        		Say.Error("Failed to open magnet board.",e);
        		CloseBoard();
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
	        	magnet.X = location.X;
	        	magnet.Y = location.Y;
	        	magnet.RandomiseAngle(MAXIMUM_ANGLE_IN_EITHER_DIRECTION);
			}        	
	
        	AddHandlers(magnet);
        	
			magnet.Margin = new Thickness(0); // margin may have been added if the magnet was in the magnetlist
        	magnet.bringToFrontMenuItem.IsEnabled = true;
        	magnet.sendToBackMenuItem.IsEnabled = true;
        	magnet.removeDeleteMagnetMenuItem.Header = "Remove"; // less permanent than deleting from the magnet list
			magnet.Show();
			BringInsideBoard(magnet);			
			
			mainCanvas.Children.Add(magnet);
			BringToFront(magnet);
			
			OnMagnetAdded(new MagnetEventArgs(magnet));
        }
        
        
        public void DeleteMagnet(MagnetControl magnet)
        {
        	mainCanvas.Children.Remove(magnet);
        	RemoveHandlers(magnet);	        	
	        OnMagnetDeleted(new MagnetEventArgs(magnet));
        }
        
        
        private void AddHandlers(MagnetControl magnet)
        {
        	try {
        		magnet.Drop += magnetControl_Drop;
        		magnet.RequestBringToFront += magnetControl_BringToFrontHandler;
        		magnet.RequestSendToBack += magnetControl_SendToBackHandler;
        		magnet.Edited += magnetControl_EditedHandler;
        	}
        	catch (Exception e) {
        		Say.Error("Failed to add handlers to magnet.",e);
        	}
        }
        
        
        private void RemoveHandlers(MagnetControl magnet)
        {
        	try {
        		magnet.Drop -= magnetControl_Drop;
        		magnet.RequestBringToFront -= magnetControl_BringToFrontHandler;
        		magnet.RequestSendToBack -= magnetControl_SendToBackHandler;
        		magnet.Edited -= magnetControl_EditedHandler;
        	}
        	catch (Exception e) {
        		Say.Error("Failed to remove handlers from magnet.",e);
        	}
        }
        
        
        /// <summary>
        /// Indicate that a magnet should be transferred to the magnet list
        /// </summary>
        /// <param name="magnet">The magnet to be transferred</param>
        public void TransferMagnet(MagnetControl magnet)
        {       	        	
	    	OnMagnetTransferredOut(new MagnetEventArgs(magnet));
        }
        
        
        /// <summary>
        /// Get a point on this board at random where a magnet could sit.
        /// </summary>
        /// <returns>A point that represents a pair of co-ordinates on the magnet board</returns>
        private Point GetRandomLocation()
        {
        	double x = random.NextDouble() * (ActualWidth - MagnetControl.MAGNET_MAX_WIDTH);
        	double y = random.NextDouble() * ActualHeight - 50;
        	return new Point(x,y);
        }
    	
        
        public List<MagnetControl> GetMagnets()
        {
        	List<MagnetControl> magnets = new List<MagnetControl>(mainCanvas.Children.Count);
        	foreach (MagnetControl magnet in mainCanvas.Children) {
        		magnets.Add(magnet);
        	}
        	return magnets;
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
                 
        
        /// <summary>
        /// Close the current board.
        /// </summary>
        public void CloseBoard()
        {
        	mainCanvas.Children.Clear(); 
        	SurfaceColour = defaultSurfaceColour;
        	Filename = null;
        	Dirty = false;
        	OnClosed(new EventArgs());
        }
    	
        
    	public void Save()
    	{
    		if (Filename == null) {
    			throw new InvalidOperationException("Save failed: Should not have called Save without first setting a filename.");
    		}
    		else {
	    		AdventureAuthor.Utils.Serialization.Serialize(Filename,this.GetSerializable());
	    		if (Dirty) {
	    			Dirty = false;
	    		}
    		}
    	}
    	
    	    	 
    	/// <summary>
    	/// Check whether a given magnet exists on this board.
    	/// </summary>
    	/// <param name="magnet">The magnet to check for</param>
    	/// <returns>True if the magnet object exists on this board; false otherwise</returns>
    	/// <remarks>This checks for the actual magnet object - to check for an equivalent magnet
    	/// (i.e. a magnet with identical field values) call HasEquivalentMagnet instead</remarks>
    	public bool HasMagnet(MagnetControl magnet)
    	{
    		return MagnetList.HasMagnet(mainCanvas.Children,magnet);
    	}
    	
    	    	
    	/// <summary>
    	/// Check whether an equivalent magnet to the given magnet exists on this board.
    	/// </summary>
    	/// <param name="magnet">The magnet to check for</param>
    	/// <returns>True if a magnet with identical field values to the given magnet exists
    	/// on this board; false otherwise</returns>
    	/// <remarks>This checks for an equivalent magnet (i.e. a magnet with identical field values) - 
    	/// to check whether the collection contains the actual object, call HasMagnet instead</remarks>
    	public bool HasEquivalentMagnet(MagnetControl magnet)
    	{
    		return MagnetList.HasEquivalentMagnet(mainCanvas.Children,magnet);
    	}
    	
    	
    	public void BringToFront(MagnetControl magnet)
    	{
    		if (HasMagnet(magnet)) {
    			UpdateZOrder(magnet,true);
    			OnMagnetMoved(new MagnetEventArgs(magnet));
    		}
    	}
    	
    	
    	public void SendToBack(MagnetControl magnet)
    	{
    		if (HasMagnet(magnet)) {
    			UpdateZOrder(magnet,false);
    			OnMagnetMoved(new MagnetEventArgs(magnet));
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
    		           
        /// <summary>
        /// Treat drop events on magnets as you would drop events on the board itself.
        /// </summary>
        private void magnetControl_Drop(object sender, DragEventArgs e)
        {
        	e.Handled = false;
        	OnDrop(e);
        }      

    	
    	/// <summary>
    	/// If a magnet is dropped that is already a child of this board, raise a MagnetMoved event.
    	/// </summary>
    	/// <remarks>If the magnet came from outside the list, a handler in MagnetBoardViewer
    	/// will deal with it, and ultimately the MagnetAdded event will be raised.</remarks>
    	private void magnetBoard_Drop(object sender, DragEventArgs e)
    	{    		
	         IDataObject data = e.Data;
	
	         if (data.GetDataPresent(typeof(MagnetControlDataObject))) {
	         	MagnetControlDataObject dataObject = (MagnetControlDataObject)data.GetData(typeof(MagnetControlDataObject));
	         	MagnetControl magnet = dataObject.Magnet;
	         	
	         	if (HasMagnet(magnet)) {
	         		OnMagnetMoved(new MagnetEventArgs(magnet));
	         	}
	         }
	         
	         e.Handled = false; // necessary?
    	}    
        
        
        private void magnetControl_BringToFront(object sender, EventArgs e)
        {
        	BringToFront((MagnetControl)sender);
        }       
        
        
        private void magnetControl_SendToBack(object sender, EventArgs e)
        {
        	SendToBack((MagnetControl)sender);
        }
        
        
        private void magnetBoard_Changed(object sender, EventArgs e)
        {
        	MakeDirty();
        }
        
        
        private void MakeDirty()
        {
        	if (!Dirty) {
        		Dirty = true;
        	}
        }
        
        #endregion
    }
}