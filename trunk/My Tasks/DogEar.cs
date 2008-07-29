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

namespace AdventureAuthor.Tasks
{
    /// <summary>
    /// Interaction logic for DogEar.xaml
    /// </summary>

    public partial class DogEar: UserControl
    {
    	#region Fields
    	
    	/// <summary>
    	/// The corner of the page to dog-ear: TopLeft, TopRight, BottomLeft or BottomRight.
    	/// Default is BottomRight.
    	/// </summary>
    	private Corner corner;
		public Corner Corner {
			get { return corner; }
			set { 
				corner = value; 
				int angle;
				double centerX;
				double centerY;
				switch (corner) {
					case Corner.BottomRight :
						angle = 0;
						centerX = 40;
						centerY = 40;
						HorizontalAlignment = HorizontalAlignment.Right;
						VerticalAlignment = VerticalAlignment.Bottom;
						break;
					case Corner.BottomLeft :
						angle = 90;
						centerX = 0;
						centerY = 40;
						HorizontalAlignment = HorizontalAlignment.Left;
						VerticalAlignment = VerticalAlignment.Bottom;
						break;
					case Corner.TopLeft :
						angle = 180;
						centerX = 0;
						centerY = 0;
						HorizontalAlignment = HorizontalAlignment.Left;
						VerticalAlignment = VerticalAlignment.Top;
						break;
					case Corner.TopRight :
						angle = 270;
						centerX = 40;
						centerY = 0;
						HorizontalAlignment = HorizontalAlignment.Right;
						VerticalAlignment = VerticalAlignment.Top;
						break;
					default:
						angle = 0;
						centerX = 40;
						centerY = 40;
						break;
				}
				rotateTransform.Angle = angle;
				scaleTransform.CenterX = centerX;
				scaleTransform.CenterY = centerY;
			}
		}
    	
    	
    	/// <summary>
    	/// The collection of transforms applied to this control.
    	/// </summary>
		private TransformGroup transformGroup = new TransformGroup();
    	
    	
    	/// <summary>
    	/// A transform allowing the dog-ear to be 'pulled back' when the mouse is hovered.
    	/// </summary>
    	private ScaleTransform scaleTransform = new ScaleTransform(1.3,1.3,40,40);   
    	
    	
    	/// <summary>
    	/// A transform allowing the dog-ear to be attached to any corner of a rectangle.
    	/// </summary>
    	private RotateTransform rotateTransform = new RotateTransform(0,20,20);
    	
    	#endregion
    	
    	#region Constructors
    	
        public DogEar(Corner corner)
        {
            InitializeComponent();
            
            Corner = corner;
            
            transformGroup.Children.Add(rotateTransform);
            RenderTransform = transformGroup;
						
			MouseEnter += delegate 
			{	
				Pull();
			};
						
			MouseLeave += delegate 
			{
				Release();
			};
        }

        
        public DogEar() : this(Corner.BottomRight)
        {
        }
        
        #endregion
        
        #region Methods
        
        /// <summary>
        /// Pull the dog-eared corner back a little further.
        /// </summary>
        public void Pull()
        {
			if (!transformGroup.Children.Contains(scaleTransform)) {
        		transformGroup.Children.Add(scaleTransform);
			}			
        }
        
        
        /// <summary>
        /// If the dog-eared corner has been pulled back, release it to its original position.
        /// </summary>
        public void Release()
        {
			if (transformGroup.Children.Contains(scaleTransform)) {
				transformGroup.Children.Remove(scaleTransform);
			}			
        }
        
        #endregion
    }
}