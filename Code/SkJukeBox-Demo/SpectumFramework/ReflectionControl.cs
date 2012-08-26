// Copyright (C) 2011, Jacob Johnston 
//
// Permission is hereby granted, free of charge, to any person obtaining a 
// copy of this software and associated documentation files (the "Software"), 
// to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, 
// and/or sell copies of the Software, and to permit persons to whom the 
// Software is furnished to do so, subject to the following conditions: 
//
// The above copyright notice and this permission notice shall be included in 
// all copies or substantial portions of the Software. 
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.  IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE. 

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BandedSpectrumAnalyzer
{
    class ReflectionControl : Decorator
    {
        private VisualBrush reflection;
        private LinearGradientBrush opacityMask;

        public ReflectionControl()
        {            
            VerticalAlignment = VerticalAlignment.Center;
            HorizontalAlignment = HorizontalAlignment.Center;

            opacityMask = new LinearGradientBrush();
            opacityMask.StartPoint = new Point(0, 0);
            opacityMask.EndPoint = new Point(0, 1);
            opacityMask.GradientStops.Add(new GradientStop(Colors.Black, 0));
            opacityMask.GradientStops.Add(new GradientStop(Colors.Black, 0.5));
            opacityMask.GradientStops.Add(new GradientStop(Colors.Transparent, 0.8));
            opacityMask.GradientStops.Add(new GradientStop(Colors.Transparent, 1));
            reflection = new VisualBrush();
            reflection.AlignmentY = AlignmentY.Bottom;
            reflection.Stretch = Stretch.None;
            reflection.TileMode = TileMode.None;
            reflection.Transform = new ScaleTransform(1, -1);
            reflection.AutoLayoutContent = false;
        }

        protected override Size MeasureOverride(Size constraint)
        {            
            // Control is twice the height of the child control.
            if (Child == null)
            {
                return new Size(0, 0);
            }
            Child.Measure(constraint);
            return new Size(Child.DesiredSize.Width, Child.DesiredSize.Height * 2);
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            // Put actual child in the upper half of the control.
            if (Child == null)
            {
                return new Size(0, 0);
            }
            Child.Arrange(new Rect(0, 0, arrangeBounds.Width, arrangeBounds.Height / 2));
            return arrangeBounds;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            // Draw non-reflection controls
            base.OnRender(drawingContext);

            double halfHeight = ActualHeight / 2;
            
            // Create fading opacity mask
            drawingContext.PushOpacityMask(opacityMask);
            drawingContext.PushOpacity(0.15);

            // Create the reflection mirror transform.
            reflection.Visual = Child;            
            ((ScaleTransform)reflection.Transform).CenterY = ActualHeight * 3/4;
            ((ScaleTransform)reflection.Transform).CenterX = ActualWidth / 2;

            // Draw the reflection visual with opacity mask applied
            drawingContext.DrawRectangle(
                reflection, null,
                new Rect(0, halfHeight, ActualWidth, halfHeight));

            // Remove opacity masks for next drawing
            drawingContext.Pop();
            drawingContext.Pop();
        }
    }
}
