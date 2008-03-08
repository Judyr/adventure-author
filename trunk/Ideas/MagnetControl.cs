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
	public partial class MagnetControl : BoardObject, ICloneable
    {       
    	#region Constants
    	
    	public const double MAGNET_MAX_WIDTH = 150;
		public const double DEGREES_TO_ROTATE = 2;
    	
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
    	
    	/// <summary>
    	/// The X co-ordinate of this magnet.
    	/// </summary>
		public override double X {
    		get {
    			return Canvas.GetLeft(this);    				
    		}
    		set {
    			Canvas.SetLeft(this,value);
    		}
		}
		
    	
    	/// <summary>
    	/// The Y co-ordinate of this magnet.
    	/// </summary>
		public override double Y {
    		get {
    			return Canvas.GetTop(this);    				
    		}
    		set {
    			Canvas.SetTop(this,value);
    		}
		}
		
		
    	/// <summary>
    	/// The idea that this magnet represents.
    	/// </summary>
    	/// <remarks>Do not use this reference to update fields on the idea - use
    	/// the Text, Category, Author properties on the magnet instead</remarks>
		protected Idea idea;		
		public Idea Idea {
			get { return idea; }
			set {
				idea = value;
				Author = value.Author;
				Text = value.Text;
				Category = value.Category;
				Created = value.Created;
			}
		}
		
		
		public string Author {
			get {
				return idea.Author;
			}
			set {
				if (idea == null) {
					throw new InvalidOperationException("This magnet has no idea.");
				}
				idea.Author = value;
			}
		}
		
			
		public string Text {
			get {
				return idea.Text;
			}
			set { 
				if (idea == null) {
					throw new InvalidOperationException("This magnet has no idea.");
				}
				idea.Text = value;
				IdeaTextBox.Text = idea.Text;
			}
		}
		
			
		public IdeaCategory Category {
			get {
				return idea.Category;
			}
			set { 
				if (idea == null) {
					throw new InvalidOperationException("This magnet has no idea.");
				}
				idea.Category = value;
				Background = GetColourForCategory(idea.Category);
			}
		}
		
			
		public DateTime Created {
			get {
				return idea.Created;
			}
			set { 
				if (idea == null) {
					throw new InvalidOperationException("This magnet has no idea.");
				}
				idea.Created = value;
			}
		}
		
		
    	/// <summary>
    	/// The collection of bitmap effects to apply to the magnet. This usually includes
    	/// bevelling (always on, to give the magnet a 3D appearance) and a glow effect
    	/// when the magnet is selected.
    	/// </summary>
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
		
		
		/// <summary>
		/// The angle of rotation of the magnet from 0.
		/// </summary>
		public double Angle {
    		get {     			
    			return ((RotateTransform)RenderTransform).Angle;
    		}
			set { 
    			((RotateTransform)RenderTransform).Angle = value;
			}
		}    	
		
		
		/// <summary>
		/// A random number generator, to help with generating random locations/angles for magnets.
		/// </summary>
		private static Random random = new Random();
    	
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
    	
    	
    	public EventHandler<MagnetEventArgs> Edited;    	
    	protected virtual void OnEdited(MagnetEventArgs e)
    	{
    		EventHandler<MagnetEventArgs> handler = Edited;
    		if (handler != null) {
    			handler(this,e);
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
		
		
		internal event EventHandler<MagnetEventArgs> RequestRemove;		
		protected virtual void OnRequestRemove(MagnetEventArgs e)
		{
			EventHandler<MagnetEventArgs> hander = RequestRemove;
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
            
            // Stop the control expanding to fill the space available in a StackPanel:
            this.VerticalAlignment = VerticalAlignment.Center;
            this.HorizontalAlignment = HorizontalAlignment.Center;
            
            this.BitmapEffect = fxGroup;
    		
            MaxWidth = MAGNET_MAX_WIDTH;
            RenderTransform = new RotateTransform(0,ActualWidth/2,ActualHeight/2);
			SizeChanged += new SizeChangedEventHandler(UpdateCentrePointOfRotation);
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
    	
    	
    	public static Brush GetColourForCategory(IdeaCategory category)
    	{
    		Brush brush;
			switch (category) {
				case IdeaCategory.Characters:
					brush = CHARACTERS_BRUSH;
					break;
				case IdeaCategory.Items:
					brush = ITEMS_BRUSH;
					break;
				case IdeaCategory.Plot:
					brush = PLOT_BRUSH;
					break;
				case IdeaCategory.Quests:
					brush = QUESTS_BRUSH;
					break;
				case IdeaCategory.Setting:
					brush = SETTING_BRUSH;
					break;
				case IdeaCategory.Other:
					brush = OTHER_BRUSH;
					break;
				case IdeaCategory.Toolset:
					brush = RESOURCES_BRUSH;
					break;
				default:
					brush = OTHER_BRUSH;
					break;
			}
    		return brush;
    	}
    	    	
    	
    	internal void SelectFX()
    	{
    		fxGroup.Children.Add(glow);
    	}    	  
    	
    	
    	internal void DeselectFX()
    	{
    		fxGroup.Children.Remove(glow);
    	}
		
		
		public void Move(double right, double down)
		{
			Canvas.SetTop(this,Canvas.GetTop(this) + down);
			Canvas.SetLeft(this,Canvas.GetLeft(this) + right);
		}
		
		
		public void Rotate(double angle)
		{
			Angle += angle;
		}
		
		
		public void RotateLeft()
		{
			Rotate(-DEGREES_TO_ROTATE);
		}
		
		
		public void RotateRight()
		{			
			Rotate(DEGREES_TO_ROTATE);
		}
		
		
		public void RandomiseAngle(double maxDeviationFromZero)
		{
			bool angleToLeft = DateTime.Now.Millisecond % 2 == 0;
			RandomiseAngle(maxDeviationFromZero,angleToLeft);
		}
        
        
        public void RandomiseAngle(double maxDeviationFromZero, bool angleToLeft)
        {
        	double angle = (maxDeviationFromZero * random.NextDouble());
        	if (((int)angle) == 0) { // straight magnets stand out too sharply from the rest
        		angle = DEGREES_TO_ROTATE;
        	}
        	if (angleToLeft) {
        		angle = 360 - angle;
        	}
        	angle -= (angle % DEGREES_TO_ROTATE); // ensure angle is divisible by DEGREES_TO_ROTATE (2)
        	Angle = angle;
        }
        
        
		public object Clone()
		{
			MagnetInfo info = (MagnetInfo)GetSerializable();
			info.Idea = (Idea)info.Idea.Clone();
			return new MagnetControl(info);
		}
    	
		
		public int CompareTo(object obj)
		{
			MagnetControl magnet = (MagnetControl)obj;
			return this.idea.CompareTo(magnet.idea);
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
    	
    	
    	private void OnClick_Edit(object sender, EventArgs e)
    	{
    		EditMagnetWindow window = new EditMagnetWindow(this);
    		window.MagnetEdited += delegate(object s, MagnetEventArgs ea) { OnEdited(ea); };
    		window.ShowDialog();
    	}
    	
    	
    	private void OnClick_RemoveDelete(object sender, EventArgs e)
    	{
    		OnRequestRemove(new MagnetEventArgs(this));
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