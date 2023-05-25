// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Microsoft.Toolkit.Uwp.Helpers;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// Defines an area within which child objects are laid out in an ellipse.
    /// </summary>
    public class EllipticalPanel : Panel
    {
        private static readonly double DefaultArcStart = -Math.PI / 2;
        private static readonly double DefaultArcEnd = (2 * Math.PI) + DefaultArcStart;

        /// <summary>
        /// Gets the <see cref="ArcStart"/> XAML dependency property.
        /// </summary>
        public static DependencyProperty ArcStartProperty { get; } = DependencyProperty.Register(
            nameof(ArcStart),
            typeof(double),
            typeof(EllipticalPanel),
            new(DefaultArcStart, HandleMeasure));

        /// <summary>
        /// Gets the <see cref="ArcEnd"/> XAML dependency property.
        /// </summary>
        public static DependencyProperty ArcEndProperty { get; } = DependencyProperty.Register(
            nameof(ArcEnd),
            typeof(double),
            typeof(EllipticalPanel),
            new(DefaultArcEnd, HandleMeasure));

        /// <summary>
        /// Gets the <see cref="ArcStartIncluded"/> XAML dependency property.
        /// </summary>
        public static DependencyProperty ArcStartIncludedProperty { get; } = DependencyProperty.Register(
            nameof(ArcStartIncluded),
            typeof(bool),
            typeof(EllipticalPanel),
            new(true, HandleMeasure));

        /// <summary>
        /// Gets the <see cref="ArcEndIncluded"/> XAML dependency property.
        /// </summary>
        public static DependencyProperty ArcEndIncludedProperty { get; } = DependencyProperty.Register(
            nameof(ArcEndIncluded),
            typeof(bool),
            typeof(EllipticalPanel),
            new(false, HandleMeasure));

        /// <summary>
        /// Gets the <see cref="IncludeFullLayoutEllipse"/> XAML dependency property.
        /// </summary>
        public static DependencyProperty IncludeFullLayoutEllipseProperty { get; } = DependencyProperty.Register(
            nameof(IncludeFullLayoutEllipse),
            typeof(bool),
            typeof(EllipticalPanel),
            new(true, HandleMeasure));

        /// <summary>
        /// Gets the <see cref="HorizontalContentAlignment"/> XAML dependency property.
        /// </summary>
        public static DependencyProperty HorizontalContentAlignmentProperty { get; } = DependencyProperty.Register(
            nameof(HorizontalContentAlignment),
            typeof(HorizontalAlignment),
            typeof(EllipticalPanel),
            new(HorizontalAlignment.Center, HandleMeasure));

        /// <summary>
        /// Gets the <see cref="VerticalContentAlignment"/> XAML dependency property.
        /// </summary>
        public static DependencyProperty VerticalContentAlignmentProperty { get; } = DependencyProperty.Register(
            nameof(VerticalContentAlignment),
            typeof(VerticalAlignment),
            typeof(EllipticalPanel),
            new(VerticalAlignment.Center, HandleMeasure));

        private static void HandleMeasure(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ellipticalPanel = (EllipticalPanel)d;
            ellipticalPanel.InvalidateMeasure();
            ellipticalPanel.InvalidateArrange();
        }

        /// <summary>
        /// Gets or sets the angle from which childern are laid out in a circle.
        /// </summary>
        /// <value>
        /// The angle is denoted in radians. The reference direction is to the right, and
        /// increasing angles result in a rotation couter-clockwise. The default value is -Pi/2.
        /// </value>
        public double ArcStart
        {
            get { return (double)GetValue(ArcStartProperty); }
            set { SetValue(ArcStartProperty, value); }
        }

        /// <summary>
        /// Gets or sets the angle upto which childern are laid out in a circle.
        /// </summary>
        /// <value>
        /// The angle is denoted in radians. The reference direction is to the right, and
        /// increasing angles result in a rotation couter-clockwise. The default value is 3Pi/2.
        /// </value>
        public double ArcEnd
        {
            get { return (double)GetValue(ArcEndProperty); }
            set { SetValue(ArcEndProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the first child is located at <see cref="ArcStart"/>.
        /// </summary>
        /// <value>
        /// true if the first child is located at <see cref="ArcStart"/>; otherwise the first child is
        /// located at one angular step after <see cref="ArcStart"/>. The default value is true.
        /// </value>
        public bool ArcStartIncluded
        {
            get { return (bool)GetValue(ArcStartIncludedProperty); }
            set { SetValue(ArcStartIncludedProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the last child is located at <see cref="ArcEnd"/>.
        /// </summary>
        /// <value>
        /// true if the last child is located at <see cref="ArcEnd"/>; otherwise the last child is
        /// located at one angular step before <see cref="ArcEnd"/>. The default value is false.
        /// </value>
        public bool ArcEndIncluded
        {
            get { return (bool)GetValue(ArcEndIncludedProperty); }
            set { SetValue(ArcEndIncludedProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the full layout circle is contained within the
        /// bounds of the <see cref="EllipticalPanel"/>.
        /// </summary>
        /// <value>
        /// true if the <see cref="EllipticalPanel"/> contains the full layout circle within its bounds;
        /// otherwise only the arc containing child elements is displayed. The default value is true.
        /// </value>
        public bool IncludeFullLayoutEllipse
        {
            get { return (bool)GetValue(IncludeFullLayoutEllipseProperty); }
            set { SetValue(IncludeFullLayoutEllipseProperty, value); }
        }

        /// <summary>
        /// Gets or sets the horizontal alignment of the panel's content.
        /// </summary>
        /// <value>
        /// One of the HorizontalAlignment values. The default is <see cref="HorizontalAlignment.Center"/>.
        /// </value>
        public HorizontalAlignment HorizontalContentAlignment
        {
            get { return (HorizontalAlignment)GetValue(HorizontalContentAlignmentProperty); }
            set { SetValue(HorizontalContentAlignmentProperty, value); }
        }

        /// <summary>
        /// Gets or sets the vertical alignment of the panel's content.
        /// </summary>
        /// <value>
        /// One of the VerticalAlignment values. The default is <see cref="VerticalAlignment.Center"/>.
        /// </value>
        public VerticalAlignment VerticalContentAlignment
        {
            get { return (VerticalAlignment)GetValue(VerticalContentAlignmentProperty); }
            set { SetValue(VerticalContentAlignmentProperty, value); }
        }

        private double DesiredRadiusX { get; set; } = double.NaN;

        private double DesiredRadiusY { get; set; } = double.NaN;

        private Rect DesiredContentBoundary { get; set; } = Rect.Empty;

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
                this.DesiredRadiusX = double.NaN;
                this.DesiredRadiusY = double.NaN;
                this.DesiredContentBoundary = new(0, 0, 0, 0);
                return new(0, 0);
            }

            if (this.Children.Count == 1)
            {
                var child = this.Children[0];
                child.Measure(availableSize);

                this.DesiredRadiusX = 0;
                this.DesiredRadiusY = 0;
                this.DesiredContentBoundary = new(
                    -child.DesiredSize.Width / 2,
                    -child.DesiredSize.Height / 2,
                    child.DesiredSize.Width,
                    child.DesiredSize.Height);
                return new(child.DesiredSize.Width, child.DesiredSize.Height);
            }

            if (this.ArcStart == this.ArcEnd)
            {
                double maxChildWidth = 0;
                double maxChildHeight = 0;
                foreach (var child in this.Children)
                {
                    child.Measure(availableSize);
                    maxChildWidth = Math.Max(child.DesiredSize.Width, maxChildWidth);
                    maxChildHeight = Math.Max(child.DesiredSize.Height, maxChildHeight);
                }

                this.DesiredRadiusX = 0;
                this.DesiredRadiusY = 0;
                this.DesiredContentBoundary = new(
                    -maxChildWidth / 2,
                    -maxChildHeight / 2,
                    maxChildWidth,
                    maxChildHeight);
                return new(maxChildWidth, maxChildHeight);
            }

            var numberOfSegments = this.Children.Count - 1
                + (this.ArcStartIncluded ? 0 : 1)
                + (this.ArcEndIncluded ? 0 : 1);

            var arc = this.ArcEnd - this.ArcStart;
            var segmentArc = arc / numberOfSegments;

            var θ = this.ArcStart + (this.ArcStartIncluded ? 0 : segmentArc);
            List<RadialLocation> radialLocations = new(this.Children.Count);
            foreach (var child in this.Children)
            {
                child.Measure(availableSize);

                // If the child size exceeds the available bounds do not consider further it for measure purposes.
                if ((child.DesiredSize.Width <= availableSize.Width) &&
                    (child.DesiredSize.Height <= availableSize.Height))
                {
                    radialLocations.Add(new(θ, child.DesiredSize));
                }

                θ += segmentArc;
            }

            if (double.IsInfinity(availableSize.Width) && double.IsInfinity(availableSize.Height))
            {
                if (this.IncludeFullLayoutEllipse)
                {
                    return Measure_FullLayout_Unbounded(radialLocations);
                }
                else
                {
                    return Measure_PartialLayout_Unbounded(radialLocations);
                }
            }
            else
            {
                if (this.IncludeFullLayoutEllipse)
                {
                    return Measure_FullLayout_Bounded(availableSize, radialLocations);
                }
                else
                {
                    return Measure_PartialLayout_Bounded(availableSize, radialLocations);
                }
            }
        }

        private Size Measure_FullLayout_Unbounded(IReadOnlyList<RadialLocation> radialLocations)
        {
            Measure_Unbounded(radialLocations, out double rX, out double rY, out Rect totalChildrenBoundary);

            this.DesiredRadiusX = rX;
            this.DesiredRadiusY = rY;
            this.DesiredContentBoundary = CalculateFullLayoutContentBoundary(totalChildrenBoundary);
            return this.DesiredContentBoundary.ToSize();
        }

        private Size Measure_PartialLayout_Unbounded(IReadOnlyList<RadialLocation> radialLocations)
        {
            Measure_Unbounded(radialLocations, out double rX, out double rY, out Rect totalChildrenBoundary);

            this.DesiredRadiusX = rX;
            this.DesiredRadiusY = rY;
            this.DesiredContentBoundary = totalChildrenBoundary;
            return totalChildrenBoundary.ToSize();
        }

        private void Measure_Unbounded(IReadOnlyList<RadialLocation> radialLocations, out double rX, out double rY, out Rect totalChildrenBoundary)
        {
            rX = 0;
            rY = 0;
            for (int i = 1; i < radialLocations.Count; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    var (rX_ij, rY_ij) = RadialLocation.CalculateMinR_NoBoundsOverlap(radialLocations[i], radialLocations[j]);
                    EnforceStretchContentAlignment(ref rX_ij, ref rY_ij);

                    rX = Math.Max(rX, rX_ij);
                    rY = Math.Max(rY, rY_ij);
                }
            }

            totalChildrenBoundary = new();
            foreach (var radialLocation in radialLocations)
            {
                var childBoundary = radialLocation.Boundary(rX, rY);

                totalChildrenBoundary.Union(childBoundary);
            }
        }

        private Size Measure_FullLayout_Bounded(Size availableSize, IReadOnlyList<RadialLocation> radialLocations)
        {
            double rX = double.PositiveInfinity;
            double rY = double.PositiveInfinity;

            foreach (var radialLocation in radialLocations)
            {
                var w = (availableSize.Width / 2) - (radialLocation.Size.Width / 2);
                var h = (availableSize.Height / 2) - (radialLocation.Size.Height / 2);

                var childRX = Math.Abs(w / radialLocation.Cosθ);
                var childRY = Math.Abs(h / radialLocation.Sinθ);

                EnforceStretchContentAlignment(ref childRX, ref childRY);

                rX = Math.Min(childRX, rX);
                rY = Math.Min(childRY, rY);
            }

            Rect totalChildrenBoundary = new();
            foreach (var radialLocation in radialLocations)
            {
                var childBoundary = radialLocation.Boundary(rX, rY);

                totalChildrenBoundary.Union(childBoundary);
            }

            this.DesiredRadiusX = rX;
            this.DesiredRadiusY = rY;
            this.DesiredContentBoundary = CalculateFullLayoutContentBoundary(totalChildrenBoundary);

            if (double.IsInfinity(availableSize.Width) || double.IsInfinity(availableSize.Height))
            {
                double desiredWidth = double.IsInfinity(availableSize.Width)
                    ? totalChildrenBoundary.Width
                    : availableSize.Width;

                double desiredHeight = double.IsInfinity(availableSize.Height)
                    ? totalChildrenBoundary.Height
                    : availableSize.Height;

                Size desiredSize = new Size(desiredWidth, desiredHeight);
                return desiredSize;
            }
            else
            {
                Size desiredSize = availableSize;
                return desiredSize;
            }
        }

        private Size Measure_PartialLayout_Bounded(Size availableSize, IReadOnlyList<RadialLocation> radialLocations)
        {
            var (rX, rY) = RadialLocation.CalculateMaxR_WithinBounds(radialLocations[0], radialLocations[1], availableSize);
            EnforceStretchContentAlignment(ref rX, ref rY);

            var child0Boundary = radialLocations[0].Boundary(rX, rY);
            var child1Boundary = radialLocations[1].Boundary(rX, rY);

            Rect totalChildrenBoundary = child0Boundary;
            totalChildrenBoundary.Union(child1Boundary);

            for (int i = 2; i < radialLocations.Count; i++)
            {
                RadialLocation radialLocation = radialLocations[i];
                var childBoundary = radialLocation.Boundary(rX, rY);

                Rect requestedBoundary = totalChildrenBoundary;
                requestedBoundary.Union(childBoundary);

                if ((requestedBoundary.Width > availableSize.Width) ||
                    (requestedBoundary.Height > availableSize.Height))
                {
                    // The current radii are too large to accomodate the resulting childern layouts.
                    // Find a new value for rX and rY which will accomodate all children.
                    for (int j = 0; j < i; j++)
                    {
                        RadialLocation prevRadialLocation = radialLocations[j];
                        var (rX_ij, rY_ij) = RadialLocation.CalculateMaxR_WithinBounds(radialLocation, prevRadialLocation, availableSize);

                        EnforceStretchContentAlignment(ref rX_ij, ref rY_ij);

                        rX = Math.Min(rX_ij, rX);
                        rY = Math.Min(rY_ij, rY);
                    }

                    // Using the new radii, recalculate the desire boundary.
                    Rect newBoundary = radialLocation.Boundary(rX, rY);
                    for (int j = 0; j < i; j++)
                    {
                        RadialLocation jRadialLocation = radialLocations[j];
                        var jChildBoundary = jRadialLocation.Boundary(rX, rY);
                        newBoundary.Union(jChildBoundary);
                    }

                    totalChildrenBoundary = newBoundary;
                }
                else
                {
                    totalChildrenBoundary = requestedBoundary;
                }
            }

            this.DesiredRadiusX = rX;
            this.DesiredRadiusY = rY;
            this.DesiredContentBoundary = totalChildrenBoundary;
            return totalChildrenBoundary.ToSize();
        }

        private void EnforceStretchContentAlignment(ref double rX, ref double rY)
        {
            var r = Math.Min(rX, rY);

            if (this.HorizontalContentAlignment != HorizontalAlignment.Stretch)
            {
                rX = r;
            }

            if (this.VerticalContentAlignment != VerticalAlignment.Stretch)
            {
                rY = r;
            }
        }

        private Rect CalculateFullLayoutContentBoundary(Rect totalChildrenBoundary)
        {
            double halfWidth = Math.Max(
                Math.Abs(totalChildrenBoundary.Left),
                Math.Abs(totalChildrenBoundary.Right));

            double halfHeight = Math.Max(
                Math.Abs(totalChildrenBoundary.Top),
                Math.Abs(totalChildrenBoundary.Bottom));

            return new(-halfWidth, -halfHeight, halfWidth * 2, halfHeight * 2);
        }

        private (double, double) CalculateTranslationVector(Size finalSize, Rect contentBoundary)
        {
            double translateX;
            switch (this.HorizontalContentAlignment)
            {
                case HorizontalAlignment.Left:
                    translateX = -contentBoundary.Left;
                    break;
                case HorizontalAlignment.Right:
                    translateX = finalSize.Width - contentBoundary.Right;
                    break;
                case HorizontalAlignment.Center:
                case HorizontalAlignment.Stretch:
                    translateX = (finalSize.Width / 2) - (contentBoundary.Left + (contentBoundary.Width / 2));
                    break;
                default:
                    throw new InvalidOperationException("Invalid value set for HorizontalContentAlignment.");
            }

            double translateY;
            switch (this.VerticalContentAlignment)
            {
                case VerticalAlignment.Top:
                    translateY = -contentBoundary.Top;
                    break;
                case VerticalAlignment.Bottom:
                    translateY = finalSize.Height - contentBoundary.Bottom;
                    break;
                case VerticalAlignment.Center:
                case VerticalAlignment.Stretch:
                    translateY = (finalSize.Height / 2) - (contentBoundary.Top + (contentBoundary.Height / 2));
                    break;
                default:
                    throw new InvalidOperationException("Invalid value set for VerticalContentAlignment.");
            }

            return (translateX, translateY);
        }

        /// <summary>
        /// Arranges the content of the <see cref="EllipticalPanel"/>.
        /// </summary>
        /// <param name="finalSize">
        /// The final area within the parent that this element should use to arrange itself and its children.
        /// </param>
        /// <returns>
        /// The actual size used by the <see cref="EllipticalPanel"/>.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            var (translateX, translateY) = CalculateTranslationVector(finalSize, this.DesiredContentBoundary);

            var numberOfSegments = this.Children.Count - 1
                + (this.ArcStartIncluded ? 0 : 1)
                + (this.ArcEndIncluded ? 0 : 1);

            var arc = this.ArcEnd - this.ArcStart;
            var segmentArc = arc / numberOfSegments;

            var θ = this.ArcStart + (this.ArcStartIncluded ? 0 : segmentArc);
            foreach (var child in this.Children)
            {
                RadialLocation radialLocation = new(θ, child.DesiredSize);
                var childBoundary = radialLocation.Boundary(this.DesiredRadiusX, this.DesiredRadiusY);
                childBoundary = new(
                    childBoundary.X + translateX,
                    childBoundary.Y + translateY,
                    childBoundary.Width,
                    childBoundary.Height);

                child.Arrange(childBoundary);

                θ += segmentArc;
            }

            return finalSize;
        }

        private readonly struct RadialLocation
        {
            /// <summary>
            /// Calculate the maximum arrangement radii which will accomodate the radial locations rl1 and rl2 within
            /// the bounds of the given size.
            /// </summary>
            public static (double, double) CalculateMaxR_WithinBounds(RadialLocation rl1, RadialLocation rl2, Size availableSize)
            {
                /* ************************************************************** *\
                 * Child's left:   l = (r * cos(θ)) - (w/2)
                 * Child's right:  r = (r * cos(θ)) + (w/2)
                 * Child's top:    t = (r * -sin(θ)) - (h/2)
                 * Child's bottom: b = (r * -sin(θ)) + (h/2)
                 *
                 * The bounds of r are given by:
                 *
                 * child1.Left - child2.Right <= availableSize.Width (W)
                 * (r * cos(θ1)) - (w1/2)  -  (r * cos(θ2)) + (w2/2) <= W
                 * r * (cos(θ1) - cos(θ2)) - w1/2 - w2/2 <= W
                 * r * (cos(θ1) - cos(θ2)) <= W + (w1+w2/2))
                 * r <= (W + (w1+w2/2)) / (cos(θ1) - cos(θ2))
                 *
                 * Similarly:
                 * child1.Top - child2.Bottom <= availableSize.Height (H)
                 * r <= (H + (h1+h2)/2) / (sin(θ1) - sin(θ2))
                \* ************************************************************** */

                var widthDividend = availableSize.Width - ((rl1.Size.Width + rl2.Size.Width) / 2.0);
                var heightDividend = availableSize.Height - ((rl1.Size.Height + rl2.Size.Height) / 2.0);

                var r_Width = widthDividend / Math.Abs(rl1.Cosθ - rl2.Cosθ);
                var r_Height = heightDividend / Math.Abs(rl1.Sinθ - rl2.Sinθ);

                return (r_Width, r_Height);
            }

            /// <summary>
            /// Calculate the minimum arrangement radii which will accomodate the radial locations rl1 and rl2
            /// without overlapping the children controls.
            /// </summary>
            public static (double, double) CalculateMinR_NoBoundsOverlap(RadialLocation rl1, RadialLocation rl2)
            {
                // If two children at at the same radial location, then ignore for purposes of spacing
                // Error bound chosen as a small number at random (no particular meaning)
                if (DoubleHelper.EqualsWithinError(rl1.Cosθ, rl2.Cosθ, 1e-5))
                {
                    return (0, 0);
                }

                /* ************************************************************** *\
                 * Child's left:   l = (r * cos(θ)) - (w/2)
                 * Child's right:  r = (r * cos(θ)) + (w/2)
                 * Child's top:    t = (r * -sin(θ)) - (h/2)
                 * Child's bottom: b = (r * -sin(θ)) + (h/2)
                 *
                 * For no overlap:
                 * child1.Left > child2.Right, OR
                 * child1.Right < child2.Left, OR
                 * child1.Top > child2.Bottom, OR
                 * child1.Bottom < child2.Top
                 *
                 * child1.Left > child2.Right
                 * (r * cos(θ1)) - (w1/2) > (r * cos(θ2)) + (w2/2)
                 * (r * cos(θ1)) - (r * cos(θ2)) > w2/2 + w1/2
                 * r (cos(θ1) - cos(θ2)) > (w2+w1)/2)
                 * r > (w1+w2) / 2(cos(θ1) - cos(θ2))
                 *
                 * Similarly
                 * child1.Right < child2.Left
                 * r > (w1+w2) / 2(cos(θ2) - cos(θ1))
                 *
                 * These only differ by the sign of the result, thus these can be combined into:
                 *  r > (w1+w2) / 2(ABS(cos(θ1) - cos(θ2)))
                 *
                 * Similarly, by considering the vertical boundaries:
                 *  r > (h1+h2) / 2(ABS(sin(θ1) - sin(θ2)))
                \* ************************************************************** */

                double rX = (rl1.Size.Width + rl2.Size.Width) / (2 * Math.Abs(rl1.Cosθ - rl2.Cosθ));
                double rY = (rl1.Size.Height + rl2.Size.Height) / (2 * Math.Abs(rl1.Sinθ - rl2.Sinθ));

                return (rX, rY);
            }

            public RadialLocation(double θ, Size size)
            {
                this.Sinθ = Math.Sin(θ);
                this.Cosθ = Math.Cos(θ);
                this.Size = size;
            }

            public double Sinθ { get; }

            public double Cosθ { get; }

            public Size Size { get; }

            public Rect Boundary(double rX, double rY) => new(
                (rX * Cosθ) - (Size.Width / 2),
                (rY * -Sinθ) - (Size.Height / 2), // Negative y to 'switch' from Cartisean co-ordinates (where increasing y is traditionally "up") to panel co-ordinates (where increasing y is "down")
                Size.Width,
                Size.Height);
        }
    }
}
