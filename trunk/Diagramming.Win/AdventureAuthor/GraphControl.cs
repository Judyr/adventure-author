using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using Netron.Diagramming.Core;
using Netron.Diagramming.Core.AdventureAuthor;
using Netron.Diagramming.Win;
using Netron.Diagramming.Win.AdventureAuthor;

namespace Netron.Diagramming.Win.AdventureAuthor
{
    /// <summary>
    /// The toolbox diagram control (aka surface control).
    /// </summary>
    [
    ToolboxBitmap(typeof(DiagramControl), "DiagramControl.bmp"),
    ToolboxItem(true),
    Description("Generic diagramming control for .Net"),
    Designer(typeof(Netron.Diagramming.Win.DiagramControlDesigner)),
    DefaultProperty("Name"),
    DefaultEvent("OnMouseDown")]
    public sealed class GraphControl : DiagramControlBase
    {
        #region Constants
                
        /// <summary>
        /// Minimum zoom level (no smaller than 14% of actual size)
        /// </summary>
        public const int MIN_ZOOM = 14;
        
        /// <summary>
        /// Maximum zoom level (no bigger than 250% of actual size)0
        /// </summary>
        public const int MAX_ZOOM = 250; 
        
		/// <summary>
		/// The amount to offset the graph control by to hide the space left by the rulers.
		/// </summary>
		public const int RULER_OFFSET = 30;
		        
		/// <summary>
		/// Paint style for the currently selected node.
		/// </summary>
		public static readonly IPaintStyle PAINT_SELECTED = new SolidPaintStyle(Color.PaleGreen);
		
		/// <summary>
		/// Paint style for a node which is part of the route leading to the currently selected node.
		/// </summary>
		public static readonly IPaintStyle PAINT_ON_ROUTE = new SolidPaintStyle(Color.MistyRose);

		/// <summary>
		/// Paint style for a node that is neither the selected node, nor part of the route leading to the selected node.
		/// </summary>
		public static readonly IPaintStyle PAINT_NOT_ON_ROUTE = new SolidPaintStyle(Color.AliceBlue);
		
		/// <summary>
		/// Pen style for an edge which is part of the route leading to the currently selected node.
		/// </summary>
		public static readonly IPenStyle PEN_ON_ROUTE = new PenStyle(Color.DarkRed,DashStyle.Solid,1);
        
        /// <summary>
        /// Pen style for an edge which is not part of the route leading to the currently selected node.
        /// </summary>
       	public static readonly IPenStyle PEN_NOT_ON_ROUTE = new PenStyle(Color.Gray,DashStyle.Solid,1);
        			
        #endregion

        #region Fields
        
        /// <summary>
        /// True if the graph is being panned, false otherwise.
        /// </summary>
        private bool panning = false;        
        
        /// <summary>
        /// The last known position of the mouse on the control during panning.
        /// </summary>
        private Point lastPosition;
        
        /// <summary>
        /// The tooltip for this control - used to simulate a tooltip for individuals nodes, since these are not actually controls.
        /// </summary>
//        private ToolTip tooltip;
        
        #endregion

        #region Constructor      
        
        /// <summary>
        /// Create a new instance of the GraphControl class.
        /// </summary>
        public GraphControl() : base()
        {
            #region double-buffering
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            #endregion
            if(!DesignMode)
            {
            	Initialise();
            }
        }        
        
        
        private void Initialise()
        {
        	try {
	            // Initialise the MVC, see the Visio diagram for an overview of the instantiation process:
	            Controller = new GraphController(this);
	            
	            // Create the view. This is not done in the base class because the view and the controller depend on the medium (web/win...):	            	            
	            View = new View(this);
	            
	            // The diagram document is the total serializable package and contains in particular 
	            // the model (which will be instantiated in the following line):
	            Document = new Document();
	            AttachToDocument(Document);
	            Controller.View = View;
	            
	            // Necessary to update the view matrix, otherwise you get a null reference exception:
	            View.Origin = View.Origin; // ugh
	            	                
	            View.OnCursorChange += new EventHandler<CursorEventArgs>(mView_OnCursorChange);
	            View.OnBackColorChange += new EventHandler<ColorEventArgs>(View_OnBackColorChange);
	                         		
		        
		        // Graph control settings:
	            this.AllowDrop = false;
	            this.AutoScroll = true;
	            this.BackgroundType = CanvasBackgroundTypes.Gradient; 
	            this.EnableAddConnection = false;
	           	Netron.Diagramming.Core.Layout.LayoutSettings.TreeLayout.TreeOrientation = TreeOrientation.TopBottom;	        
				this.ShowRulers = false;
				this.ShowPage = false;
				
				// Setup tooltip:
//				tooltip = new ToolTip();
////				tooltip.Active = false;
//				tooltip.AutoPopDelay = 300000;				
//				tooltip.BackColor = Color.FloralWhite;
//				tooltip.InitialDelay = 1000;
//				tooltip.IsBalloon = false;
//				tooltip.ToolTipIcon = ToolTipIcon.None;
//				tooltip.ToolTipTitle = String.Empty;
//				tooltip.UseAnimation = true;
//				tooltip.UseFading = false;
//				tooltip.Draw += delegate { tooltipIsShown = true; };
        	}
        	catch (Exception e) {
        		MessageBox.Show("Error in Initialise\n\n" + e.ToString());
        	}
        }
        
        #endregion
        
        #region Event handlers

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseWheel"></see> event.
        /// <list type="bullet">
        /// </list>
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"></see> that contains the event data.</param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {  
//            Point p = Origin;
//            int newValue=0;

            #region Zooming
            
            SizeF s = Magnification;
            float alpha = e.Delta > 0 ? 1.1F : 0.9F;   
            
            SizeF newMagnification = new SizeF(s.Width * alpha, s.Height * alpha);
            if (newMagnification.Height > MAX_ZOOM || newMagnification.Height < MIN_ZOOM) { // set zoom limits
            	return;
            }
            
            float differenceInPixels = newMagnification.Height - Magnification.Height;
//            MessageBox.Show("Original magnification height: "  + Magnification.Height + "\n\n" +
//                            "New magnification height: " + newMagnification.Height + "\n\n" +
//                            Magnification.Height + " - " + newMagnification.Height + " = " + differenceInPixels);
            
            Magnification = newMagnification;
            
            float w = (float)AutoScrollPosition.X / (float)AutoScrollMinSize.Width;
            float h = (float)AutoScrollPosition.Y / (float)AutoScrollMinSize.Height;
            
            // Resize the scrollbars proportionally to keep the actual canvas constant:
            s = new SizeF(AutoScrollMinSize.Width * alpha, AutoScrollMinSize.Height * alpha);
            AutoScrollMinSize = Size.Round(s);

            // Zoom out from the centre of the graph control:
//			            Point v = e.Location;
//			            Point newOrigin = Origin;
//			            newOrigin.Offset(Convert.ToInt32((alpha - 1) * v.X), Convert.ToInt32((alpha - 1) * v.Y));
//			            Origin = newOrigin;
			
			//            Point v = e.Location;
			//            Point newOrigin = Origin;
			//            newOrigin.Offset(Convert.ToInt32((alpha - 1) * v.X), Convert.ToInt32((alpha - 1) * v.Y));
			//            Origin = newOrigin;
			
			// Zooming in: adjust up and left. Zooming out: adjust down and right.

//			Point newOrigin = Origin;
//			int diff = (int)differenceInPixels/2;
//			newOrigin.Offset(-diff,diff);
//			MessageBox.Show("Original origin: ( " + Origin.X + " , " + Origin.Y + " )\n\n" +
//			                "New origin: ( " + newOrigin.X + " , " + newOrigin.Y + " )");
//			Origin = newOrigin;
//			if (alpha < 1.0F) { // zoom out - down and right
//				newOrigin = Origin;
//				newOrigin.Offset(3,3);
//				Origin = newOrigin;
//			}
//			else  if (alpha > 1.0F) { // zoom in - up and left
//				newOrigin = Origin;
//				newOrigin.Offset(-3,-3);
//				Origin = newOrigin;
//			}
			Invalidate();
             
            #endregion   
            
            #region Deprecated
//            if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
//            {   
//				// zooming was originally here
//            }
//            else if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
//            {
//                #region Pan horizontal
//                newValue = p.X - Math.Sign(e.Delta) * 20;
//                if (newValue > 0)
//                    Origin = new Point(newValue, p.Y);
//                else
//                    Origin = new Point(0, Origin.Y);
//                
//                #endregion
//            }
//            else
//            {
//                #region Default vertical scroll
//                newValue = Origin.Y - Math.Sign(e.Delta) * 20;
//                if (newValue > 0)
//                    Origin = new Point(Origin.X, newValue);
//                else
//                    Origin = new Point(Origin.X, 0);
//                #endregion
//            }
			#endregion
//			
//            this.AutoScrollPosition = Origin;
            
            //Origin = new Point(Origin.X, Origin.Y - 10);
            //this.AutoScrollPosition = Origin;
        }

        
		protected override void OnMouseDown(MouseEventArgs e)
		{
			lastPosition = e.Location;
			base.OnMouseDown(e);
			
			if (!panning) {
				panning = true;
				this.Cursor = Cursors.SizeAll;
			}
		}
		
		
		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (panning) {
				panning = false;
				this.Cursor = Cursors.Default;
			}
			base.OnMouseUp(e);
		}
		
		private bool tooltipIsShown = false;
		
		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (panning) {				
				int x = lastPosition.X - e.X;
				int y = lastPosition.Y - e.Y;		
				
				Origin = new Point(Origin.X + x,Origin.Y + y);					
				lastPosition = e.Location;
				this.Invalidate();
			}
			else {
//				if (!tooltipIsShown) {
					foreach (IDiagramEntity entity in this.Controller.Model.Paintables) {
						IPageNode node = entity as IPageNode;
						if (node != null) {			
				        	float alpha = Magnification.Height / 100F;				
				        	Rectangle entityRect = new Rectangle((int)((node.Rectangle.X * alpha) - Origin.X + (RULER_OFFSET/2)),
				        	                                     (int)((node.Rectangle.Y * alpha) - Origin.Y + (RULER_OFFSET/2)),
				        	                                     (int)(node.Rectangle.Width * alpha),
				        	                                     (int)(node.Rectangle.Height * alpha));			        	
				        	
//				        	if (entityRect.Contains(e.Location)) {
//						        tooltip.Show(node.ToolTipText,this);
//						        return;						        
//				        	}						
						}
					}
//				}
//				else {
//					tooltip.Hide(this);
//					tooltipIsShown = false;
//				}
			}
		}       
              


        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.ScrollableControl.Scroll"></see> event.
        /// </summary>
        /// <param name="se">A <see cref="T:System.Windows.Forms.ScrollEventArgs"></see> that contains the event data.</param>
        protected override void OnScroll(ScrollEventArgs se)
        {
            //base.OnScroll(se);
            if (se.ScrollOrientation == ScrollOrientation.HorizontalScroll)
            {
                Origin = new Point(se.NewValue, Origin.Y);
                //System.Diagnostics.Trace.WriteLine(se.NewValue);
            }
            else
            {
                Origin = new Point(Origin.X, se.NewValue);
                //System.Diagnostics.Trace.WriteLine(se.NewValue);
            }
        }


        void mView_OnCursorChange(object sender, CursorEventArgs e)
        {
           this.Cursor = e.Cursor;
        }
       
        
        
        /// <summary>
        /// Called on delete.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void OnDelete(object sender, EventArgs e)
        { 
        
        }

        private void OnProperties(object sender, EventArgs e)
        {
            this.RaiseOnShowCanvasProperties(new SelectionEventArgs(new object[] { this }));
        }
        
        private void OnRecShape(object sender, EventArgs e)
        {
        	
        }
        
        private void OnNewConnection(object sender, EventArgs e)
        {
        	
        }

       
        
        #endregion
                
        #region Conversation graph methods
        
        /// <summary>
        /// Select a node in the graph, highlight the path to it from the root, and optionally centre the graph around it.
        /// </summary>
        /// <param name="node">The node to select</param>
		/// <param name="centreGraph">True to centre the graph around the node representing this page; false otherwise</param>
        public void SelectNode(IPageNode node) 
        {
        	if (!Controller.Model.Paintables.Contains(node)) {
        		throw new ArgumentException(node.ToString() + " is not a part of this graph.");
        	}
        	MarkRoute(node);
        	Invalidate();
        }
				        
        
        /// <summary>
        /// Highlight the route between the root node and the selected node.
        /// </summary>
        /// <param name="selectedNode">The selected node</param>
        private void MarkRoute(IPageNode selectedNode)
        {
        	List<IDiagramEntity> route = selectedNode.GetRoute();
        	        	
        	foreach (IDiagramEntity entity in Controller.Model.Paintables) {
        		IPageNode node = entity as IPageNode;
        		IConnection edge = entity as IConnection;
        		if (node != null) {
        			if (node == selectedNode) {
        				node.PaintStyle = PAINT_SELECTED;
        				node.PenStyle = PEN_ON_ROUTE;
        			}
        			else if (route.Contains(node)) {
        				node.PaintStyle = PAINT_ON_ROUTE;
        				node.PenStyle = PEN_ON_ROUTE;
        			}
        			else {
        				node.PaintStyle = PAINT_NOT_ON_ROUTE;
        				node.PenStyle = PEN_NOT_ON_ROUTE;
        			}
        		}
        		else if (edge != null) {
        			if (route.Contains(edge)) {
        				edge.PenStyle = PEN_ON_ROUTE;
        			}
        			else {
        				edge.PenStyle = PEN_NOT_ON_ROUTE;
        			}
        		}
        	}
        }
        
        
        /// <summary>
        /// Centres the graph control on a particular shape.
        /// </summary>
        /// <param name="shape">The shape to centre the graph view on.</param>
        public void CentreOnShape(IShape shape)
        {
        	if (!Controller.Model.Paintables.Contains(shape)) {
        		MessageBox.Show("Tried to centre view on a shape that was not a part of the graph.");
        		return;
        	}
        	        	
        	// Adjust for the current zoom level of the graph control:
        	float alpha = Magnification.Height / 100F;
        	
        	float xf = (alpha * shape.Location.X) + (alpha * (shape.Width/2)) - this.Parent.Width/2;
        	float yf = (alpha * shape.Location.Y) + (alpha * (shape.Height/2)) - this.Parent.Height/2;
        	        	
        	Origin = new Point((int)xf,(int)yf);
        }
                        
        #endregion
        
        
        
        #region Original API visible members
        
        /// <summary>
        /// Adds a shape to the diagram.
        /// </summary>
        /// <param name="shape">The shape.</param>
        /// <returns></returns>
        public IShape AddShape(IShape shape)
        {
            this.Controller.Model.AddShape(shape);
            return shape;
        }
        
        /// <summary>
        /// Adds a connection to the diagram.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns></returns>
        public IConnection AddConnection(IConnection connection)
        {
            this.Controller.Model.AddConnection(connection);
            return connection;
        }
        
        /// <summary>
        /// Adds a connection to the diagram.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <returns></returns>
        public IConnection AddConnection(IConnector from, IConnector to)
        {

            Connection cn = new Connection(Point.Empty, Point.Empty);
            this.AddConnection(cn);
            from.AttachConnector(cn.From);
            to.AttachConnector(cn.To);
            return cn;
        }

        /// <summary>
        ///  Changes the paint style of the selected entities.
        /// </summary>
        /// <param name="paintStyle">The paint style.</param>
        public void ChangeStyle(IPaintStyle paintStyle)
        {
            this.Controller.ChangeStyle(paintStyle);
        }
        
        /// <summary>
        /// Changes the pen style of the selected entities.
        /// </summary>
        /// <param name="penStyle"></param>
        public void ChangeStyle(IPenStyle penStyle)
        {
            this.Controller.ChangeStyle(penStyle);
        }

        public new void Layout(LayoutType type)
        {
            if (this.Controller.Model.CurrentPage.Shapes.Count == 0)
                throw new InconsistencyException("There are no shapes on the canvas; there's nothing to lay out.");

            switch (type)
            {
                case LayoutType.ForceDirected:
                    RunActivity("ForceDirected Layout");
                    break;
                case LayoutType.FruchtermanRheingold:
                    RunActivity("FruchtermanReingold Layout");
                    break;
                case LayoutType.RadialTree:
                    SetLayoutRoot();
                    RunActivity("Radial TreeLayout");
                    break;
                case LayoutType.Balloon:
                    SetLayoutRoot();
                    RunActivity("Balloon TreeLayout");
                    break;
                case LayoutType.ClassicTree:
                    SetLayoutRoot();
                    RunActivity("Standard TreeLayout");
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Sets the layout root.
        /// </summary>
        private void SetLayoutRoot()
        {
            //this layout needs a root, you should assign one before calling this method
            if (this.Controller.Model.LayoutRoot == null)
            {
                //if a shape is selected we'll take that one as the root for the layout
                if (this.SelectedItems.Count > 0)
                    this.Controller.Model.LayoutRoot = this.SelectedItems[0] as IShape;
                else //use the zero-th shape
                    this.Controller.Model.LayoutRoot = this.Controller.Model.CurrentPage.Shapes[0];
            }
        }

        public void Unwrap(IBundle bundle)
        {
            if (bundle != null)
            {
                #region Unwrap the bundle
                Anchors.Clear();
                this.Controller.Model.Unwrap(bundle.Entities);
                Rectangle rec = Utils.BoundingRectangle(bundle.Entities);
                rec.Inflate(30, 30);
                this.Controller.View.Invalidate(rec);
                #endregion
            }
        }
        
        #endregion

   
    }
}
