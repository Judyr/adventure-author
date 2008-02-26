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
		
		
		internal event EventHandler<MagnetEventArgs> RequestSendToBack;		
		protected virtual void OnRequestSendToBack(MagnetEventArgs e)
		{
			EventHandler<MagnetEventArgs> hander = RequestSendToBack;
			if (hander != null) {
				hander(this, e);
			}
		}
		
		
		internal event EventHandler<MagnetEventArgs> RequestBringToFront;		
		protected virtual void OnRequestBringToFront(MagnetEventArgs e)
		{
			EventHandler<MagnetEventArgs> hander = RequestBringToFront;
			if (hander != null) {
				hander(this, e);
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
            RenderTransform = new RotateTransform(0,ActualWidth/2,ActualHeight/2);
			SizeChanged += new SizeChangedEventHandler(UpdateCentrePointOfRotation);
				
            
            this.GotFocus += delegate { Log.WriteMessage(this.ToString() + " got focus"); };
            this.LostFocus += delegate { Log.WriteMessage(this.ToString() + " lost focus"); };
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
    	
    	
    	internal void Lifted()
    	{
    	}
    	
    	
    	internal void Dropped()
    	{
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

    	
        /// <summary>
        /// When the size changes, we need to update the RotateTransform's centre of rotation,
        /// otherwise it will no longer rotate evenly around the centre - this will screw up
        /// working out where to drop magnets (since you need to account for rotation.)
        /// </summary>
    	private void UpdateCentrePointOfRotation(object sender, SizeChangedEventArgs e)
    	{
			RotateTransform rotate = (RotateTransform)RenderTransform;
			RenderTransform = new RotateTransform(rotate.Angle,e.NewSize.Width/2,e.NewSize.Height/2);
    	}
    	
    	
    	private void OnClick_BringToFront(object sender, EventArgs e)
    	{
    		OnRequestBringToFront(new MagnetEventArgs(this));
    	}
    	
    	
    	private void OnClick_SendToBack(object sender, EventArgs e)
    	{
    		OnRequestSendToBack(new MagnetEventArgs(this));
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