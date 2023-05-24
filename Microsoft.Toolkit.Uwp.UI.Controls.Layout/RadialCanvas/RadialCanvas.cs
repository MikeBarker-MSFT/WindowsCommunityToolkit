// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// Defines an area within which you can explicitly position child objects, using
    /// polar coordinates that are relative to the <see cref="RadialCanvas"/> area.
    /// </summary>
    /// <remarks>
    /// The reference-point of the <see cref="RadialCanvas"/> coordinate system is located
    /// at the centre of the layout control.
    /// </remarks>
    public class RadialCanvas : Panel
    {
        /// <summary>
        /// Gets the RadialCanvas.Radius XAML attached property.
        /// </summary>
        public static DependencyProperty RadiusProperty { get; } = DependencyProperty.RegisterAttached("Radius", typeof(double), typeof(RadialCanvas), new(0.0, AttachedPropertyChanged));

        /// <summary>
        /// Gets the RadialCanvas.Angle XAML attached property.
        /// </summary>
        /// <remarks>
        /// The angle is denoted in radians. The reference direction is to the right, and
        /// increasing angles result in a rotation couter-clockwise.
        /// </remarks>
        public static DependencyProperty AngleProperty { get; } = DependencyProperty.RegisterAttached("Angle", typeof(double), typeof(RadialCanvas), new(0.0, AttachedPropertyChanged));

        /// <summary>
        /// Gets the value of the RadialCanvas.Radius XAML attached property for the target element.
        /// </summary>
        /// <param name="element">The object from which the property value is read.</param>
        /// <returns>The RadialCanvas.Radius XAML attached property value of the specified object.</returns>
        /// <seealso cref="RadiusProperty"/>
        public static double GetRadius(UIElement element)
        {
            return (double)element.GetValue(RadiusProperty);
        }

        /// <summary>
        /// Sets the value of the RadialCanvas.Radius XAML attached property for a target element.
        /// </summary>
        /// <param name="element">The object to which the property value is written.</param>
        /// <param name="value">The value to set.</param>
        /// <seealso cref="RadiusProperty"/>
        public static void SetRadius(UIElement element, double value)
        {
            element.SetValue(RadiusProperty, value);
        }

        /// <summary>
        /// Gets the value of the RadialCanvas.Angle XAML attached property for the target element.
        /// </summary>
        /// <param name="element">The object from which the property value is read.</param>
        /// <returns>The RadialCanvas.Angle XAML attached property value of the specified object.</returns>
        /// <seealso cref="AngleProperty"/>
        public static double GetAngle(UIElement element)
        {
            return (double)element.GetValue(AngleProperty);
        }

        /// <summary>
        /// Sets the value of the RadialCanvas.Angle XAML attached property for a target element.
        /// </summary>
        /// <param name="element">The object to which the property value is written.</param>
        /// <param name="value">The value to set.</param>
        /// <seealso cref="AngleProperty"/>
        public static void SetAngle(UIElement element, double value)
        {
            element.SetValue(AngleProperty, value);
        }

        private static void AttachedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement element)
            {
                if (element.Parent is RadialCanvas radialCanvas)
                {
                    // BUG: In WinUI3 UpdateLayout does not appear to invalidate to cause a "redraw cycle".
                    //      Calling InvalidateMeasure and InvalidateArrange has the desired effect.
                    radialCanvas.InvalidateMeasure();
                    radialCanvas.InvalidateArrange();
                }
            }
        }

        /// <summary>
        /// Provides the behavior for the "Measure" pass of the layout cycle.
        /// </summary>
        /// <param name="availableSize">
        /// The available size that this object can give to child objects. Infinity can be
        /// specified as a value to indicate that the object will size to whatever content
        /// is available.
        /// </param>
        /// <returns>
        /// The size that this object determines it needs during layout, based on its calculations
        /// of the allocated sizes for child objects or based on other considerations such
        /// as a fixed container size.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.Children.Count == 0)
            {
                return new(0, 0);
            }

            Rect totalChildrenBoundary = Rect.Empty;
            foreach (var child in this.Children)
            {
                child.Measure(availableSize);

                var childBoundary = GetChildBoundary(child);

                totalChildrenBoundary.Union(childBoundary);
            }

            double desiredWidth = double.IsInfinity(availableSize.Width)
                ? totalChildrenBoundary.Width
                : availableSize.Width;

            double desiredHeight = double.IsInfinity(availableSize.Height)
                ? totalChildrenBoundary.Height
                : availableSize.Height;

            Size desiredSize = new(desiredWidth, desiredHeight);
            return desiredSize;
        }

        /// <summary>
        /// Arranges the content of the <see cref="RadialCanvas"/>.
        /// </summary>
        /// <param name="finalSize">
        /// The final area within the parent that this element should use to arrange itself and its children.
        /// </param>
        /// <returns>
        /// The actual size used by the <see cref="RadialCanvas"/>.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            var centreX = finalSize.Width / 2;
            var centreY = finalSize.Height / 2;

            foreach (var child in this.Children)
            {
                var childBoundary = GetChildBoundary(child);

                // Translate to the centre of the canvas.
                childBoundary.X += centreX;
                childBoundary.Y += centreY;

                child.Arrange(childBoundary);
            }

            return finalSize;
        }

        private static Rect GetChildBoundary(UIElement child)
        {
            var radius = GetRadius(child);
            var angle = GetAngle(child);
            var sin = Math.Sin(angle);
            var cos = Math.Cos(angle);

            // Negative y to transform from Cartisean co-ordinates (where increasing y is traditionally "up")
            // to panel co-ordinates (where increasing y is "down").
            Point childLocation = new(radius * cos, radius * -sin);
            Size childSize = child.DesiredSize;

            return new Rect(childLocation, childSize);
        }
    }
}