﻿using System;
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
    	protected static readonly Brush ITEMS_BRUSH = Brushes.LightYellow;
    	protected static readonly Brush PLOT_BRUSH = Brushes.LightGreen;
    	protected static readonly Brush QUESTS_BRUSH = Brushes.LightBlue;
    	protected static readonly Brush SETTING_BRUSH = Brushes.BurlyWood;
    	protected static readonly Brush OTHER_BRUSH = Brushes.DimGray;
    	protected static readonly Brush UNASSIGNED_BRUSH = Brushes.White;
    	protected static readonly Brush USERDEFINED_BRUSH = Brushes.Chocolate;
    	
    	protected static readonly OuterGlowBitmapEffect glow = new OuterGlowBitmapEffect();
    	
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
				case IdeaCategory.Unassigned:
					Background = UNASSIGNED_BRUSH;
					break;
				case IdeaCategory.UserDefined:
					Background = USERDEFINED_BRUSH;
					break;
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
    	
    	#region Constructors
    	    	
    	public MagnetControl()
    	{
    		glow.GlowColor = Colors.Blue;
    		glow.GlowSize = 15;
    		glow.Opacity = 0.3;
            InitializeComponent();    		
    		
            MaxWidth = MAGNET_MAX_WIDTH;
            IdeaTextBox.IsEditable = false; //Tools.PreventEditingOfTextBox(IdeaTextBox);
            RenderTransform = new RotateTransform();
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
    	
    	
    	internal void Select()
    	{
//    		OuterGlowBitmapEffect glow = new OuterGlowBitmapEffect();
//    		glow.GlowColor = Colors.Blue;
//    		glow.GlowSize = 10;
    		BitmapEffect = glow;
    	}    	  
    	
    	
    	internal void Deselect()
    	{
    		BitmapEffect = null;
    	}
    	
    	
		public override string ToString()
		{
			return idea.Text;
		}		
        
        #endregion
        
        #region Event handlers   
        
        #endregion
    }
}