using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using Netron.Diagramming.Core;
using Netron.Diagramming.Win.AdventureAuthor;
namespace Netron.Diagramming.Win
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
    public sealed class MindMapControl : DiagramControlBase
    {
        #region Constants
                
        /// <summary>
        /// Minimum zoom level (no smaller than 14% of actual size)
        /// </summary>
        public int MIN_ZOOM = 14;
        
        /// <summary>
        /// Maximum zoom level (no bigger than 250% of actual size)0
        /// </summary>
        public int MAX_ZOOM = 250; 
        
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

        #region Events
       
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
        /// the context menu of the control
        /// </summary>
        private ContextMenu menu;
        /// <summary>
        /// the tooltip control
        /// </summary>
        public ToolTip toolTip;
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="T:DiagramControl"/> class.
        /// </summary>
        public MindMapControl() : base()
        {
            

            #region double-buffering
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            #endregion
            if(!DesignMode)
            {

                //init the MVC, see the Visio diagram for an overview of the instantiation proces
                Controller = new Controller(this); // TODO MindMapController??
                //Create the view. This is not done in the base class because the view and the controller depend on the medium (web/win...)
                View = new View(this);
                //The diagram document is the total serializable package and contains in particular the model (which will be instantiated in the following line).
                Document = new Document();
                AttachToDocument(Document);
                Controller.View = View;
                MindMapTextEditor.Init(this);
                
                //Selection.Controller = Controller;
                //menu
                menu = new ContextMenu();
                BuildMenu();

                View.OnCursorChange += new EventHandler<CursorEventArgs>(mView_OnCursorChange);
                View.OnBackColorChange += new EventHandler<ColorEventArgs>(View_OnBackColorChange);
                Controller.OnShowContextMenu += new EventHandler<EntityMenuEventArgs>(Controller_OnShowContextMenu);
                this.AllowDrop = true;
	            // Necessary to update the view matrix, otherwise you get a null reference exception:
	            View.Origin = View.Origin; // ugh
               
                
                this.toolTip = new ToolTip();
                toolTip.IsBalloon = true;
                toolTip.UseAnimation = true;
                toolTip.UseFading = true;
                toolTip.ToolTipIcon = ToolTipIcon.Info;
                toolTip.ToolTipTitle = "Info";
                toolTip.Active = false;
                toolTip.BackColor = Color.OrangeRed;
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseWheel"></see> event.
        /// <list type="bullet">
        /// <item><term>Control-key</term><description>the control (CTRL) modifier will zoom/magnify the diagram.</description></item>
        /// <item><term>Shift-key</term><description>the shift (SHFT) modifier will pan/translate the diagram</description></item>
        /// </list>
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"></see> that contains the event data.</param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {  
            SizeF s = Magnification;
            float alpha = e.Delta > 0 ? 1.1F : 0.9F;   
            
            SizeF newMagnification = new SizeF(s.Width * alpha, s.Height * alpha);
            if (newMagnification.Height > MAX_ZOOM || newMagnification.Height < MIN_ZOOM) { // set zoom limits
            	return;
            }
            
            float differenceInPixels = newMagnification.Height - Magnification.Height;
            
            Magnification = newMagnification;
            
            float w = (float)AutoScrollPosition.X / (float)AutoScrollMinSize.Width;
            float h = (float)AutoScrollPosition.Y / (float)AutoScrollMinSize.Height;
            
            // Resize the scrollbars proportionally to keep the actual canvas constant:
            s = new SizeF(AutoScrollMinSize.Width * alpha, AutoScrollMinSize.Height * alpha);
            AutoScrollMinSize = Size.Round(s);
            
            Invalidate();
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
        
        
        /// <summary>
        /// Handles the OnShowContextMenu event of the Controller control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:Netron.Diagramming.Core.EntityMenuEventArgs"/> instance containing the event data.</param>
        void Controller_OnShowContextMenu(object sender, EntityMenuEventArgs e)
        {

            menu.Show(this, e.MouseEventArgs.Location);
        }

         

        

       
        #endregion

        #region Methods

    



       
        void mView_OnCursorChange(object sender, CursorEventArgs e)
        {
           this.Cursor = e.Cursor;
        }
        /// <summary>
        /// Builds the context menu
        /// </summary>
        private void BuildMenu()
        {
            menu.MenuItems.Clear();

            MenuItem mnuDelete = new MenuItem("Delete", new EventHandler(OnDelete));
            menu.MenuItems.Add(mnuDelete);

            MenuItem mnuProps = new MenuItem("Properties", new EventHandler(OnProperties));
            menu.MenuItems.Add(mnuProps);

            MenuItem mnuDash = new MenuItem("-");
            menu.MenuItems.Add(mnuDash);

            if (EnableAddConnection)
            {
                MenuItem mnuNewConnection = new MenuItem("Add connection", new EventHandler(OnNewConnection));
                menu.MenuItems.Add(mnuNewConnection);
            }

            MenuItem mnuShapes = new MenuItem("Shapes");
            menu.MenuItems.Add(mnuShapes);

            MenuItem mnuRecShape = new MenuItem("Rectangular", new EventHandler(OnRecShape));
            mnuShapes.MenuItems.Add(mnuRecShape);
            /*
            MenuItem mnuOvalShape = new MenuItem("Oval", new EventHandler(OnOvalShape));
            mnuShapes.MenuItems.Add(mnuOvalShape);

            MenuItem mnuTLShape = new MenuItem("Text label", new EventHandler(OnTextLabelShape));
            mnuShapes.MenuItems.Add(mnuTLShape);

            MenuItem mnuClassShape = new MenuItem("Class bundle", new EventHandler(OnClassShape));
            mnuShapes.MenuItems.Add(mnuClassShape);

            */

        }
        /// <summary>
        /// Called on delete.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void OnDelete(object sender, EventArgs e)
        { }

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
        
        

        #region API visible members
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

        public void Layout(LayoutType type)
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
