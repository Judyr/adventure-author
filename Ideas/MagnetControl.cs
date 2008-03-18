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
    	
    	static MagnetControl()
    	{
//    		PLOTCOLOR = Color.FromScRgb(1.0f,0.3915725f,0.8879231f,0.5394795f);// = "#FFA8F2C2";
//			QUESTSCOLOR = Color.FromScRgb(1.0f,0.5332764f,0.6724432f,1.0f);// = "#FFC1D6FE";
//			CHARACTERSCOLOR = Color.FromScRgb(1.0f,1.0f,0.6444797f,0.3324515f);// = "#FFFFD29C";
//			DIALOGUECOLOR = Color.FromScRgb(1.0f,0.8631572f,0.8879231f,0.3915725f);// = "#FFEFF2A8";
//			SETTINGCOLOR = Color.FromScRgb(1.0f,1.0f,0.5775805f,0.5775805f);// = "#FFFFC8C8";
//			ITEMSCOLOR = Color.FromScRgb(1.0f,0.8631572f,0.5647115f,0.9734453f);// = "#FFEFC6FC";
//			OTHERCOLOR = Color.FromScRgb(1.0f,0.9215819f,0.7083758f,0.5457245f);// = "#FFF6DBC3";
//			TOOLSETCOLOR = Color.FromScRgb(1.0f,0.838799f,0.838799f,0.838799f);// = "#FFEBECEC";
			
    		PLOTCOLOR = Color.FromScRgb(1.0f,0.1915725f,0.6879231f,0.3394795f);// = "#FFA8F2C2";
			QUESTSCOLOR = Color.FromScRgb(1.0f,0.3332764f,0.4724432f,1.0f);// = "#FFC1D6FE";
			CHARACTERSCOLOR = Color.FromScRgb(1.0f,1.0f,0.5972018f,0.2541521f);// = "#FFFFD29C";
			DIALOGUECOLOR = Color.FromScRgb(1.0f,1.0f,0.8069522f,0.1559265f);// = "#FFEFF2A8";
			SETTINGCOLOR = Color.FromScRgb(1.0f,1.0f,0.3775805f,0.3775805f);// = "#FFFFC8C8";
			ITEMSCOLOR = Color.FromScRgb(1.0f,0.6631572f,0.3647115f,0.7734453f);// = "#FFEFC6FC";
			OTHERCOLOR = Color.FromScRgb(1.0f,0.7215819f,0.5083758f,0.3457245f);// = "#FFF6DBC3";
			TOOLSETCOLOR = Color.FromScRgb(1.0f,0.638799f,0.638799f,0.638799f);// = "#FFEBECEC";
			    	
	    	PLOTBRUSH = new SolidColorBrush(PLOTCOLOR);
	    	QUESTSBRUSH = new SolidColorBrush(QUESTSCOLOR);
			CHARACTERSBRUSH = new SolidColorBrush(CHARACTERSCOLOR);
	    	DIALOGUEBRUSH = new SolidColorBrush(DIALOGUECOLOR);
	    	SETTINGBRUSH = new SolidColorBrush(SETTINGCOLOR);
	    	ITEMSBRUSH = new SolidColorBrush(ITEMSCOLOR);
	    	OTHERBRUSH = new SolidColorBrush(OTHERCOLOR);
	    	TOOLSETBRUSH = new SolidColorBrush(TOOLSETCOLOR);
    	}
    	
    	public const double MAGNET_MAX_WIDTH = 150;
		public const double DEGREES_TO_ROTATE = 2;		
		
		protected static Color PLOTCOLOR;// = "#FFA8F2C2";
		protected static Color QUESTSCOLOR;// = "#FFC1D6FE";
		protected static Color CHARACTERSCOLOR;// = "#FFFFD29C";
		protected static Color DIALOGUECOLOR;// = "#FFEFF2A8";
		protected static Color SETTINGCOLOR;// = "#FFFFC8C8";
		protected static Color ITEMSCOLOR;// = "#FFEFC6FC";
		protected static Color OTHERCOLOR;// = "#FFF6DBC3";
		protected static Color TOOLSETCOLOR;// = "#FFEBECEC";
    	
    	protected static SolidColorBrush PLOTBRUSH;
    	protected static SolidColorBrush QUESTSBRUSH;
		protected static SolidColorBrush CHARACTERSBRUSH;
    	protected static SolidColorBrush DIALOGUEBRUSH;
    	protected static SolidColorBrush SETTINGBRUSH;
    	protected static SolidColorBrush ITEMSBRUSH;
    	protected static SolidColorBrush OTHERBRUSH;
    	protected static SolidColorBrush TOOLSETBRUSH;
    	
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
				Creator = value.Creator;
				Text = value.Text;
				Category = value.Category;
				Created = value.Created;
			}
		}
		
		
		public string Creator {
			get {
				return idea.Creator;
			}
			set {
				if (idea == null) {
					throw new InvalidOperationException("This magnet has no idea.");
				}
				idea.Creator = value;
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
				Background = GetBrushForCategory(idea.Category);
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
				double angle = ((RotateTransform)RenderTransform).Angle;
				return angle;
    		}
			set { 
    			((RotateTransform)RenderTransform).Angle = value;
				OnRotated(new EventArgs());
			}
		}    	
		
		
		/// <summary>
		/// A random number generator, to help with generating random locations/angles for magnets.
		/// </summary>
		private static Random random = new Random();
    	
    	#endregion    	
    	
    	#region Events
    	
    	public EventHandler Edited;    	
    	protected virtual void OnEdited(EventArgs e)
    	{
    		EventHandler handler = Edited;
    		if (handler != null) {
    			handler(this,e);
    		}
    	}
		
		
		public event EventHandler Rotated;		
		protected virtual void OnRotated(EventArgs e)
		{
			EventHandler handler = Rotated;
			if (handler != null) {
				handler(this, e);
			}
		}
    			
		
		public event EventHandler RequestSendToBack;		
		protected virtual void OnRequestSendToBack(EventArgs e)
		{
			EventHandler handler = RequestSendToBack;
			if (handler != null) {
				handler(this, e);
			}
		}
		
		
		public event EventHandler RequestBringToFront;		
		protected virtual void OnRequestBringToFront(EventArgs e)
		{
			EventHandler handler = RequestBringToFront;
			if (handler != null) {
				handler(this, e);
			}
		}
		
		
		public event EventHandler RequestRemove;		
		protected virtual void OnRequestRemove(EventArgs e)
		{
			EventHandler handler = RequestRemove;
			if (handler != null) {
				handler(this, e);
			}
		}
		
		
		public event EventHandler TaskSubmitted;		
		protected virtual void OnTaskSubmitted(EventArgs e)
		{
			EventHandler handler = TaskSubmitted;
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
    	
    	
    	/// <summary>
    	/// Get the colour that represents a given category of idea.
    	/// </summary>
    	/// <param name="category">A category of idea (e.g. Quests, Plot)</param>
    	/// <returns>The colour representing the given idea category</returns>
    	public static Color GetColourForCategory(IdeaCategory category)
    	{
    		Color color;
			switch (category) {
				case IdeaCategory.Plot:
					color = PLOTCOLOR;
					break;
				case IdeaCategory.Quests:
					color = QUESTSCOLOR;
					break;
				case IdeaCategory.Characters:
					color = CHARACTERSCOLOR;
					break;
				case IdeaCategory.Dialogue:
					color = DIALOGUECOLOR;
					break;
				case IdeaCategory.Setting:
					color = SETTINGCOLOR;
					break;
				case IdeaCategory.Items:
					color = ITEMSCOLOR;
					break;
				case IdeaCategory.Other:
					color = OTHERCOLOR;
					break;
				case IdeaCategory.Toolset:
					color = TOOLSETCOLOR;
					break;
				default:
					color = OTHERCOLOR;
					break;
			}
    		return color;
    	}
    	
    	
    	/// <summary>
    	/// Get the colour brush that represents a given category of idea.
    	/// </summary>
    	/// <param name="category">A category of idea (e.g. Quests, Plot)</param>
    	/// <returns>The colour brush representing the given idea category</returns>
    	public static SolidColorBrush GetBrushForCategory(IdeaCategory category)
    	{
    		SolidColorBrush brush;
			switch (category) {
				case IdeaCategory.Plot:
					brush = PLOTBRUSH;
					break;
				case IdeaCategory.Quests:
					brush = QUESTSBRUSH;
					break;
				case IdeaCategory.Characters:
					brush = CHARACTERSBRUSH;
					break;
				case IdeaCategory.Dialogue:
					brush = DIALOGUEBRUSH;
					break;
				case IdeaCategory.Setting:
					brush = SETTINGBRUSH;
					break;
				case IdeaCategory.Items:
					brush = ITEMSBRUSH;
					break;
				case IdeaCategory.Other:
					brush = OTHERBRUSH;
					break;
				case IdeaCategory.Toolset:
					brush = TOOLSETBRUSH;
					break;
				default:
					brush = OTHERBRUSH;
					break;
			}
    		return brush;
    	}
    	    	
    	
    	/// <summary>
    	/// Apply bitmap effects to indicate that this magnet is currently selected.
    	/// </summary>
    	internal void SelectFX()
    	{
    		fxGroup.Children.Add(glow);
    	}    	  
    	
    	
    	/// <summary>
    	/// Remove bitmap effects which indicated that this magnet was selected.
    	/// </summary>
    	internal void DeselectFX()
    	{
    		fxGroup.Children.Remove(glow);
    	}
		
		
		public void Move(double right, double down)
		{
			Log.WriteMessage("move " + right.ToString() + "," + down);
			Canvas.SetTop(this,Canvas.GetTop(this) + down);
			Canvas.SetLeft(this,Canvas.GetLeft(this) + right);
		}
		
		
		/// <summary>
		/// Rotate this magnet by a certain number of degrees, within a 
		/// given maximum deviation from zero in either direction.
		/// </summary>
		/// <param name="offset">The number of degrees to move the magnet angle by
		/// (positive to move to the right, negative to move to the left)</param>
		public void RotateBy(double offset) 
		{
			RotateBy(offset,180);
		}
		
		
		/// <summary>
		/// Rotate this magnet by a certain number of degrees, within a 
		/// given maximum deviation from zero in either direction.
		/// </summary>
		/// <param name="offset">The number of degrees to move the magnet angle by
		/// (positive to move to the right, negative to move to the left)</param>
		/// <param name="maxDeviationFromZero">The maximum number of degrees
		/// the magnet may deviate from zero (in either direction, so passing
		/// the number 45 will restrict the magnet to an angle of between
		/// -45 degrees and 45 degrees). If the new
		/// angle would be outside this range, the magnet is not rotated</param>
		public void RotateBy(double offset, double maxDeviationFromZero)
		{			                
			double proposedAngle = Angle + offset;
			while (proposedAngle < 0) {
				proposedAngle += 360;
			}
			while (proposedAngle > 360) {
				proposedAngle -= 360;	
			}
			
			if (proposedAngle < maxDeviationFromZero || proposedAngle > 360 - maxDeviationFromZero) {
				Angle = proposedAngle;
			}
		}
		
		
		/// <summary>
		/// Angle the magnet randomly, within a certain deviation from zero.
		/// </summary>
		/// <param name="maxDeviationFromZero">The maximum number of degrees
		/// the magnet may deviate from zero (in either direction, so passing
		/// the number 45 will restrict the magnet to an angle of between
		/// -45 degrees and 45 degrees)</param>
		public void RandomiseAngle(double maxDeviationFromZero)
		{
			bool angleToLeft = DateTime.Now.Millisecond % 2 == 0;
			RandomiseAngle(maxDeviationFromZero,angleToLeft);
		}
        
        	
		/// <summary>
		/// Angle the magnet randomly, within a certain deviation from zero.
		/// </summary>
		/// <param name="maxDeviationFromZero">The maximum number of degrees
		/// the magnet may deviate from zero (in either direction, so passing
		/// the number 45 will restrict the magnet to an angle of between
		/// -45 degrees and 45 degrees)</param>
		/// <param name="angleToLeft">True to angle the magnet a random number of degrees
		/// to the left; false to angle the magnet a random number of degrees
		/// to the right</param>
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
        
        
        /// <summary>
        /// Get an identical copy of this MagnetControl.
        /// </summary>
        /// <returns>An identical copy of this MagnetControl</returns>
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
    	
        /// <summary>
        /// Focus on the magnet if you receive a MouseDown event.
        /// </summary>
        private void magnetControlMouseDown(object sender, MouseButtonEventArgs e)
        {
        	Focus();
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
    	        
    	
    	private void OnClick_AddToTasksDelete(object sender, EventArgs e)
    	{
    		OnTaskSubmitted(new MagnetEventArgs(this));
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