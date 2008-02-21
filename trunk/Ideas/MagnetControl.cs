using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Ideas
{
    public partial class MagnetControl : BoardObject
    {       
    	#region Constants
    	
    	public const double MAGNET_MAX_WIDTH = 150;
    	
    	protected static readonly Brush CHARACTERS_BRUSH = Brushes.LightCoral;
    	protected static readonly Brush ITEMS_BRUSH = Brushes.Wheat;
    	protected static readonly Brush PLOT_BRUSH = Brushes.LightGreen;
    	protected static readonly Brush QUESTS_BRUSH = Brushes.LightBlue;
    	protected static readonly Brush SETTING_BRUSH = Brushes.BurlyWood;
    	protected static readonly Brush OTHER_BRUSH = Brushes.LightGray;
    	protected static readonly Brush RESOURCES_BRUSH = Brushes.White;
    	
    	protected static readonly OuterGlowBitmapEffect glow = new OuterGlowBitmapEffect();
    	protected static readonly BevelBitmapEffect bevel = new BevelBitmapEffect();
    	protected static readonly DropShadowBitmapEffect shadow = new DropShadowBitmapEffect();
    	
    	#endregion    	
    	
    	#region Fields
    	
		public override double X {
    		get {
    			return Canvas.GetLeft(this);    				
    		}
    		set {
    			Canvas.SetLeft(this,value);
    		}
		}
		
    	
		public override double Y {
    		get {
    			return Canvas.GetTop(this);    				
    		}
    		set {
    			Canvas.SetTop(this,value);
    		}
		}
		
		
		protected Idea idea;		
		public Idea Idea {
			get { return idea; }
			set {
				idea = value;
				IdeaTextBox.Text = idea.Text;
				SetColourByCategory();
			}
		}
		
		
    	
    	protected BitmapEffectGroup fxGroup = new BitmapEffectGroup();
		
		
		/// <summary>
		/// Sort according to the text of the idea, in ascending alphabetical order.
		/// </summary>
		public static IComparer SortAlphabeticallyAscending {
			get {
				return new sortAlphabeticallyAscendingComparer();
			}
		}
		
		
		/// <summary>
		/// Sort according to the text of the idea, in descending alphabetical order.
		/// </summary>
		public static IComparer SortAlphabeticallyDescending {
			get {
				return new sortAlphabeticallyDescendingComparer();
			}
		}
		
		
		public double Angle {
    		get {     			
    			return ((RotateTransform)RenderTransform).Angle;
    		}
			set { 
    			((RotateTransform)RenderTransform).Angle = value;
			}
		}    	
    	
    	#endregion    	
    	
    	#region Events
    	
    	public event EventHandler<MagnetEventArgs> Selected;    	
		protected virtual void OnSelected(MagnetEventArgs e)
		{
			EventHandler<MagnetEventArgs> handler = Selected;
			if (handler != null) {
				handler(this, e);
			}
		}
    	
    	#endregion
    	
    	#region Constructors
    	    	
    	public MagnetControl()
    	{
    		// Set up visual effects:
    		glow.GlowColor = Colors.Blue;
    		glow.GlowSize = 15;
    		glow.Opacity = 0.3;
    		
    		bevel.EdgeProfile = EdgeProfile.Linear;
    		bevel.Relief = 0.3;
    		
    		fxGroup.Children.Add(bevel);
    		
            InitializeComponent();
            
            this.BitmapEffect = fxGroup;
    		
            MaxWidth = MAGNET_MAX_WIDTH;
            IdeaTextBox.IsEditable = false;
            RenderTransform = new RotateTransform();

            
            // Set up drag-drop:
            this.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(MagnetBoardViewer_PreviewMouseLeftButtonDown);
            this.PreviewMouseMove += new MouseEventHandler(MagnetBoardViewer_PreviewMouseMove);
            
            
    	}
    	
    	        
        Point _startPoint;
        bool IsDragging = false;
        
        
        private void MagnetBoardViewer_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {        	
        	_startPoint = e.GetPosition(null);
        }

        
        private void MagnetBoardViewer_PreviewMouseMove(object sender, MouseEventArgs e)
        {        	
	            if (e.LeftButton == MouseButtonState.Pressed && !IsDragging)
	            {
	                Point position = e.GetPosition(null);
	
	                if (Math.Abs(position.X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
	                    Math.Abs(position.Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
	                {
	                	StartDrag(e); 
	                }
	            }  
        	
        }
        
        
        private void StartDrag(MouseEventArgs e)
        {
        	IsDragging = true;
        	Hide();       
        	
        	MagnetControlDataObject data = new MagnetControlDataObject();
        	data.Magnet = this; 	
        	
        	// Temporarily undo the rotation on this magnet, in order to correctly measure the
        	// offset between the magnet's location (at its top-left corner) and the mouse click position:
        	if (Angle == 0) {	        	
	        	data.Offset = e.GetPosition(this);
        	}
        	else {
	        	double origAngle = Angle;
	        	Angle = 0;        	
	        	data.Offset = e.GetPosition(this);
	        	Angle = origAngle;
        	}
        	
        	DataObject dataObject = new DataObject(typeof(MagnetControlDataObject),data);
        	DragDropEffects effects = DragDrop.DoDragDrop(this,dataObject,DragDropEffects.Move);
        	
        	Show();
        	IsDragging = false;
        }
    	
    	
    	
    	
    	
    	public MagnetControl(MagnetInfo magnetInfo) : this()
    	{
    		X = magnetInfo.X;
    		Y = magnetInfo.Y;
    		Angle = magnetInfo.Angle;
    		Idea = magnetInfo.Idea;
    	}
    	
    	
    	public MagnetControl(Idea idea) : this()
    	{
            Idea = idea;
    	}
    	
    	
    	public MagnetControl(Idea idea, double x, double y) : this(idea)
        {
            X = x;
            Y = y;
        }
    	
    	
    	public MagnetControl(Idea idea, Point location) : this(idea,location.X,location.Y)
    	{
    		
    	}
        
        #endregion        
        
        #region Methods
            	
		/// <summary>
		/// Get a serializable object representing serializable data in an unserializable control.
		/// </summary>
		/// <returns>A serializable object</returns>
    	public override ISerializableData GetSerializable()
    	{
    		return new MagnetInfo(this);
    	}
    	
    	
    	/// <summary>
    	/// Hide this magnet if it is visible.
    	/// </summary>
    	public void Hide()
    	{
    		if (Visibility == Visibility.Visible) {
    			Visibility = Visibility.Collapsed;
    		}
    	}
    	
    	
    	/// <summary>
    	/// Reveal this magnet if it is hidden.
    	/// </summary>
    	public void Show()
    	{
    		if (Visibility != Visibility.Visible) {
    			Visibility = Visibility.Visible;
    		}
    	}
    	
    	
		private void SetColourByCategory()
		{
			switch (idea.Category) {
				case IdeaCategory.Characters:
					Background = CHARACTERS_BRUSH;
					break;
				case IdeaCategory.Items:
					Background = ITEMS_BRUSH;
					break;
				case IdeaCategory.Plot:
					Background = PLOT_BRUSH;
					break;
				case IdeaCategory.Quests:
					Background = QUESTS_BRUSH;
					break;
				case IdeaCategory.Setting:
					Background = SETTING_BRUSH;
					break;
				case IdeaCategory.Other:
					Background = OTHER_BRUSH;
					break;
				case IdeaCategory.Resources:
					Background = RESOURCES_BRUSH;
					break;
			}
		}
    	
    	
    	internal void SelectFX()
    	{
    		fxGroup.Children.Add(glow);
    	}    	  
    	
    	
    	internal void DeselectFX()
    	{
    		fxGroup.Children.Remove(glow);
    	}
    	
    	
    	internal void LiftedFX()
    	{
    		fxGroup.Children.Add(shadow);
    	}
    	
    	
    	internal void DroppedFX()
    	{
    		fxGroup.Children.Remove(shadow);
    	}
		
		
		public void Move(double right, double down)
		{
			Canvas.SetTop(this,Canvas.GetTop(this) + down);
			Canvas.SetLeft(this,Canvas.GetLeft(this) + right);
		}
    	
		
		public int CompareTo(object obj)
		{
			return String.Compare(idea.Text,((MagnetControl)obj).idea.Text);
		}
		
    	
		public override string ToString()
		{
			return idea.Text;
		}	
        
        #endregion
        
        #region Event handlers   
        
        private void MagnetControl_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
        	OnSelected(new MagnetEventArgs(this));
        }
        
        #endregion
   

		
		#region IComparer classes
		
		/// <summary>
		/// Sort according to the text of the idea, in ascending alphabetical order.
		/// </summary>
		private class sortAlphabeticallyAscendingComparer : IComparer
		{			
			public int Compare(object x, object y)
			{
				Idea idea1 = ((MagnetControl)x).idea;
				Idea idea2 = ((MagnetControl)y).idea;		
				return String.Compare(idea1.Text,idea2.Text);
			}
		}
		
		
		/// <summary>
		/// Sort according to the text of the idea, in descending alphabetical order.
		/// </summary>
		private class sortAlphabeticallyDescendingComparer : IComparer
		{			
			public int Compare(object x, object y)
			{
				Idea idea1 = ((MagnetControl)x).idea;
				Idea idea2 = ((MagnetControl)y).idea;				
				return String.Compare(idea2.Text,idea1.Text);
			}
		}
		
		#endregion
    }
}