using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using AdventureAuthor.Conversations.UI.Controls;

namespace AdventureAuthor.Conversations.UI
{
    public class LineControlDragAdorner : Adorner
    {
        protected UIElement _child;
        protected VisualBrush _brush;
        protected UIElement _owner;
        protected double XCenter;
        protected double YCenter;

        public LineControlDragAdorner(UIElement owner) : base(owner) { }

        
        public LineControlDragAdorner(UIElement owner, LineControl lineControl, bool useVisualBrush, double opacity)
            : base(owner)
        {
            System.Diagnostics.Debug.Assert(owner != null);
            System.Diagnostics.Debug.Assert(lineControl != null); 
            _owner = owner;
            if (useVisualBrush)
            {
                VisualBrush _brush = new VisualBrush(lineControl);
                _brush.Opacity = opacity;
                Rectangle r = new Rectangle();
                r.RadiusX = 3;
                r.RadiusY = 3;

                //TODO: questioning DesiredSize vs. Actual 
                r.Width = lineControl.ActualWidth;
                r.Height = lineControl.ActualHeight;
                
                XCenter = lineControl.DesiredSize.Width / 2;
                YCenter = lineControl.DesiredSize.Height / 2;

                r.Fill = _brush;
                _child = r;

            }
            else
                _child = lineControl;


        }


        private double _leftOffset;
        public double LeftOffset
        {
            get { return _leftOffset; }
            set
            {
                _leftOffset = value - XCenter;
                UpdatePosition();
            }
        }

        private double _topOffset;
        public double TopOffset
        {
            get { return _topOffset; }
            set
            {
                _topOffset = value - YCenter;

                UpdatePosition();
            }
        }

        private void UpdatePosition()
        {
            AdornerLayer adorner = (AdornerLayer)this.Parent;
            if (adorner != null)
            {
                adorner.Update(this.AdornedElement);
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            return _child;
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }


        protected override Size MeasureOverride(Size finalSize)
        {
            _child.Measure(finalSize);
            return _child.DesiredSize;
        }
        protected override Size ArrangeOverride(Size finalSize)
        {

            _child.Arrange(new Rect(_child.DesiredSize));
            return finalSize;
        }

        public double scale = 1.0;
        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            GeneralTransformGroup result = new GeneralTransformGroup();

            result.Children.Add(base.GetDesiredTransform(transform));
            result.Children.Add(new TranslateTransform(_leftOffset, _topOffset));
            return result;
        }
    }
}
