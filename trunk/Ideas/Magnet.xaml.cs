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

namespace AdventureAuthor.Ideas
{
    public partial class Magnet : BoardObject
    {       
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
		
    	private double angle;    	
		public override double Angle {
			get { return angle; }
			set { 
				angle = value; 
				SetAngle(angle);
			}
		}
    	
		private IDragBehaviour dragBehaviour;		
		public override IDragBehaviour DragBehaviour {
			get { return dragBehaviour; }
			set { dragBehaviour = value; }
		}
		
		private IDropBehaviour dropBehaviour;		
		public override IDropBehaviour DropBehaviour {
			get { return dropBehaviour; }
			set { dropBehaviour = value; }
		}
    	
    	#endregion
    	
    	
        public Magnet(double x, double y, double angle, UserControl parentcontrol)
        {
            InitializeComponent();
            X = x;
            Y = y;
            SetAngle(angle);
            
            this.MouseDown += new MouseButtonEventHandler(Magnet_MouseDown);
            this.MouseMove += new MouseEventHandler(Magnet_MouseMove);
            this.MouseUp += new MouseButtonEventHandler(Magnet_MouseUp);
            this.MouseLeave += new MouseEventHandler(Magnet_MouseLeave);
            this.parentcontrol = parentcontrol;
        }	

        void Magnet_MouseLeave(object sender, MouseEventArgs e)
        {
        	if (dragging) {
        		Point currentMouseLocation = e.GetPosition(this.parentcontrol);
        		X = currentMouseLocation.X - (this.ActualWidth/2);
        		Y = currentMouseLocation.Y - (this.ActualHeight/2);
        	}
        }

        void Magnet_MouseUp(object sender, MouseButtonEventArgs e)
        {
        	if (dragging) {
        		dragging = false;
        		//lastMouseLocation = null;
        	}
        }
        
        private UserControl parentcontrol;

        void Magnet_MouseMove(object sender, MouseEventArgs e)
        {
        	if (dragging) {
        		Point currentMouseLocation = e.GetPosition(this.parentcontrol);
        		X = currentMouseLocation.X - (this.ActualWidth/2);
        		Y = currentMouseLocation.Y - (this.ActualHeight/2);
        	}
        }
        
        private bool dragging = false;

        void Magnet_MouseDown(object sender, MouseButtonEventArgs e)
        {
        	if (!dragging) {
        		dragging = true;
        		//lastMouseLocation = e.GetPosition(this.Parent);
        	}        	
        }
        
        //private Point lastMouseLocation = null;
        
        
        #region Methods
        
        public void SetAngle(double angle)
        {
        	RenderTransform = new RotateTransform(angle);        	
        }
        
        #endregion
    }
}