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
    /// Defines an area within which you can explicitly position child objects, using
    /// polar coordinates that are relative to the <see cref="CircularPanel"/> area.
    /// </summary>
    public class CircularPanel : Panel
    {
        internal static readonly double DefaultArcStart = -Math.PI / 2;
        internal static readonly double DefaultArcEnd = (2 * Math.PI) + DefaultArcStart;

        /// <summary>
        /// Gets the <see cref="ArcStart"/> XAML dependency property.
        /// </summary>
        public static DependencyProperty ArcStartProperty { get; } = DependencyProperty.Register(
            nameof(ArcStart),
            typeof(double),
            typeof(CircularPanel),
            new(DefaultArcStart, HandleMeasure));

        /// <summary>
        /// Gets the <see cref="ArcEnd"/> XAML dependency property.
        /// </summary>
        public static DependencyProperty ArcEndProperty { get; } = DependencyProperty.Register(
            nameof(ArcEnd),
            typeof(double),
            typeof(CircularPanel),
            new(DefaultArcEnd, HandleMeasure));

        /// <summary>
        /// Gets the <see cref="ArcStartIncluded"/> XAML dependency property.
        /// </summary>
        public static DependencyProperty ArcStartIncludedProperty { get; } = DependencyProperty.Register(
            nameof(ArcStartIncluded),
            typeof(bool),
            typeof(CircularPanel),
            new(true, HandleMeasure));

        /// <summary>
        /// Gets the <see cref="ArcEndIncluded"/> XAML dependency property.
        /// </summary>
        public static DependencyProperty ArcEndIncludedProperty { get; } = DependencyProperty.Register(
            nameof(ArcEndIncluded),
            typeof(bool),
            typeof(CircularPanel),
            new(false, HandleMeasure));

        /// <summary>
        /// Gets the <see cref="IsOriginAtCentre"/> XAML dependency property.
        /// </summary>
        public static DependencyProperty IsOriginAtCentreProperty { get; } = DependencyProperty.Register(
            nameof(IsOriginAtCentre),
            typeof(bool),
            typeof(CircularPanel),
            new(true, HandleMeasure));

        private static void HandleMeasure(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var circularPanel = (CircularPanel)d;
            circularPanel.InvalidateMeasure();
            circularPanel.InvalidateArrange();
        }

        /// <summary>
        /// Gets or sets the angle from which childern are laid out in a circle.
        /// </summary>
        /// <remarks>
        /// The angle is denoted in radians. The reference direction is to the right, and
        /// increasing angles result in a rotation couter-clockwise.
        /// </remarks>
        public double ArcStart
        {
            get { return (double)GetValue(ArcStartProperty); }
            set { SetValue(ArcStartProperty, value); }
        }

        /// <summary>
        /// Gets or sets the angle upto which childern are laid out in a circle.
        /// </summary>
        /// <remarks>
        /// The angle is denoted in radians. The reference direction is to the right, and
        /// increasing angles result in a rotation couter-clockwise.
        /// </remarks>
        public double ArcEnd
        {
            get { return (double)GetValue(ArcEndProperty); }
            set { SetValue(ArcEndProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the first child is located at <see cref="ArcStart"/>.
        /// </summary>
        /// <remarks>
        /// If true the first child is located at <see cref="ArcStart"/>; otherwise the first child is
        /// located at one angular step after <see cref="ArcStart"/>.
        /// </remarks>
        public bool ArcStartIncluded
        {
            get { return (bool)GetValue(ArcStartIncludedProperty); }
            set { SetValue(ArcStartIncludedProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the last child is located at <see cref="ArcEnd"/>.
        /// </summary>
        /// <remarks>
        /// If true the last child is located at <see cref="ArcEnd"/>; otherwise the last child is
        /// located at one angular step before <see cref="ArcEnd"/>.
        /// </remarks>
        public bool ArcEndIncluded
        {
            get { return (bool)GetValue(ArcEndIncludedProperty); }
            set { SetValue(ArcEndIncludedProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the centre of the layout circle (on which the children are located)
        /// is at the centre of the <see cref="CircularPanel"/>.
        /// </summary>
        public bool IsOriginAtCentre
        {
            get { return (bool)GetValue(IsOriginAtCentreProperty); }
            set { SetValue(IsOriginAtCentreProperty, value); }
        }

        private double DesiredRadius { get; set; } = double.NaN;

        private Point? DesiredTranslationVector { get; set; } = default;

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
                this.DesiredRadius = double.NaN;
                this.DesiredTranslationVector = default;
                return new(0, 0);
            }

            if (this.Children.Count == 1)
            {
                var child = this.Children[0];
                child.Measure(availableSize);

                this.DesiredRadius = 0;
                this.DesiredTranslationVector = new(
                    availableSize.Width / 2,
                    availableSize.Height / 2);
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

                this.DesiredRadius = 0;
                if (double.IsInfinity(availableSize.Width) && double.IsInfinity(availableSize.Height))
                {
                    this.DesiredTranslationVector = new(
                        maxChildWidth / 2,
                        maxChildHeight / 2);
                }
                else
                {
                    this.DesiredTranslationVector = new(
                        availableSize.Width / 2,
                        availableSize.Height / 2);
                }

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

                // TODO: Handle DesiredSize is > AvailableSize
                radialLocations.Add(new(θ, child.DesiredSize));

                θ += segmentArc;
            }

            if (double.IsInfinity(availableSize.Width) && double.IsInfinity(availableSize.Height))
            {
                if (this.IsOriginAtCentre)
                {
                    return Measure_Centred_Unbounded(radialLocations);
                }
                else
                {
                    return Measure_Uncentred_Unbounded(radialLocations);
                }
            }
            else
            {
                if (this.IsOriginAtCentre)
                {
                    return Measure_Centred_Bounded(availableSize, radialLocations);
                }
                else
                {
                    return Measure_Uncentred_Bounded(availableSize, radialLocations);
                }
            }
        }

        private Size Measure_Centred_Unbounded(IReadOnlyList<RadialLocation> radialLocations)
        {
            Measure_Unbounded(radialLocations, out double r, out Rect totalChildrenBoundary);

            double halfWidth = Math.Max(
                Math.Abs(totalChildrenBoundary.Left),
                Math.Abs(totalChildrenBoundary.Right));

            double halfHeight = Math.Max(
                Math.Abs(totalChildrenBoundary.Top),
                Math.Abs(totalChildrenBoundary.Bottom));

            this.DesiredRadius = r;
            this.DesiredTranslationVector = null; // Align to the centre of the finalSize
            return new(halfWidth * 2, halfHeight * 2);
        }

        private Size Measure_Uncentred_Unbounded(IReadOnlyList<RadialLocation> radialLocations)
        {
            Measure_Unbounded(radialLocations, out double r, out Rect totalChildrenBoundary);

            this.DesiredRadius = r;
            this.DesiredTranslationVector = new(
                -totalChildrenBoundary.Left,
                -totalChildrenBoundary.Top);
            return totalChildrenBoundary.ToSize();
        }

        private void Measure_Unbounded(IReadOnlyList<RadialLocation> radialLocations, out double r, out Rect totalChildrenBoundary)
        {
            r = 0;
            for (int i = 1; i < radialLocations.Count; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    var r_ij = RadialLocation.CalculateMinR_NoBoundsOverlap(radialLocations[i], radialLocations[j]);

                    r = Math.Max(r, r_ij);
                }
            }

            totalChildrenBoundary = new();
            foreach (var radialLocation in radialLocations)
            {
                var childBoundary = radialLocation.Boundary(r);

                totalChildrenBoundary.Union(childBoundary);
            }
        }

        private Size Measure_Centred_Bounded(Size availableSize, IReadOnlyList<RadialLocation> radialLocations)
        {
            double r = double.PositiveInfinity;

            foreach (var radialLocation in radialLocations)
            {
                var w = (availableSize.Width / 2) - (radialLocation.Size.Width / 2);
                var h = (availableSize.Height / 2) - (radialLocation.Size.Height / 2);

                var childR = Math.Min(Math.Abs(w / radialLocation.Cosθ), Math.Abs(h / radialLocation.Sinθ));
                r = Math.Min(childR, r);
            }

            this.DesiredRadius = r;

            if (double.IsInfinity(availableSize.Width) || double.IsInfinity(availableSize.Height))
            {
                Rect totalChildrenBoundary = new();
                foreach (var radialLocation in radialLocations)
                {
                    var childBoundary = radialLocation.Boundary(r);

                    totalChildrenBoundary.Union(childBoundary);
                }

                double desiredWidth = double.IsInfinity(availableSize.Width)
                    ? totalChildrenBoundary.Width
                    : availableSize.Width;

                double desiredHeight = double.IsInfinity(availableSize.Height)
                    ? totalChildrenBoundary.Height
                    : availableSize.Height;

                Size desiredSize = new Size(desiredWidth, desiredHeight);
                this.DesiredTranslationVector = null; // Align to the centre of the finalSize
                return desiredSize;
            }
            else
            {
                Size desiredSize = availableSize;
                this.DesiredTranslationVector = null; // Align to the centre of the finalSize
                return desiredSize;
            }
        }

        private Size Measure_Uncentred_Bounded(Size availableSize, IReadOnlyList<RadialLocation> radialLocations)
        {
            var r = RadialLocation.CalculateMaxR_WithinBounds(radialLocations[0], radialLocations[1], availableSize);

            var child0Boundary = radialLocations[0].Boundary(r);
            var child1Boundary = radialLocations[1].Boundary(r);

            Rect panelDesiredBoundary = child0Boundary;
            panelDesiredBoundary.Union(child1Boundary);

            for (int i = 2; i < radialLocations.Count; i++)
            {
                RadialLocation radialLocation = radialLocations[i];
                var childBoundary = radialLocation.Boundary(r);

                Rect requestedBoundary = panelDesiredBoundary;
                requestedBoundary.Union(childBoundary);

                if ((requestedBoundary.Width > availableSize.Width) ||
                    (requestedBoundary.Height > availableSize.Height))
                {
                    // The current r is too large to accomodate the resulting childern layouts.
                    // Find a new value for r which will accomodate all children.
                    for (int j = 0; j < i; j++)
                    {
                        RadialLocation prevRadialLocation = radialLocations[j];
                        var r_ij = RadialLocation.CalculateMaxR_WithinBounds(radialLocation, prevRadialLocation, availableSize);
                        r = Math.Min(r, r_ij);
                    }

                    // Using the new r, recalculate the desire boundary.
                    Rect newBoundary = radialLocation.Boundary(r);
                    for (int j = 0; j < i; j++)
                    {
                        RadialLocation jRadialLocation = radialLocations[j];
                        var jChildBoundary = jRadialLocation.Boundary(r);
                        newBoundary.Union(jChildBoundary);
                    }

                    panelDesiredBoundary = newBoundary;
                }
                else
                {
                    panelDesiredBoundary = requestedBoundary;
                }
            }

            this.DesiredRadius = r;
            double translationX = double.IsInfinity(availableSize.Width)
                ? -panelDesiredBoundary.Left
                : -panelDesiredBoundary.Left + ((availableSize.Width - panelDesiredBoundary.Width) / 2);

            double translationY = double.IsInfinity(availableSize.Height)
                ? -panelDesiredBoundary.Top
                : -panelDesiredBoundary.Top + ((availableSize.Height - panelDesiredBoundary.Height) / 2);

            this.DesiredTranslationVector = new(translationX, translationY);
            Size desiredSize = panelDesiredBoundary.ToSize();
            return desiredSize;
        }

        /// <summary>
        /// Arranges the content of the <see cref="CircularPanel"/>.
        /// </summary>
        /// <param name="finalSize">
        /// The final area within the parent that this element should use to arrange itself and its children.
        /// </param>
        /// <returns>
        /// The actual size used by the <see cref="CircularPanel"/>.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            double translationX = this.DesiredTranslationVector?.X ?? (finalSize.Width / 2);
            double translationY = this.DesiredTranslationVector?.Y ?? (finalSize.Height / 2);

            var numberOfSegments = this.Children.Count - 1
                + (this.ArcStartIncluded ? 0 : 1)
                + (this.ArcEndIncluded ? 0 : 1);

            var arc = this.ArcEnd - this.ArcStart;
            var segmentArc = arc / numberOfSegments;

            var θ = this.ArcStart + (this.ArcStartIncluded ? 0 : segmentArc);
            foreach (var child in this.Children)
            {
                RadialLocation radialLocation = new(θ, child.DesiredSize);
                var childBoundary = radialLocation.Boundary(this.DesiredRadius);
                childBoundary = new(
                    childBoundary.X + translationX,
                    childBoundary.Y + translationY,
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
            /// Calculate the maximum arrangement radius (r) which will accomodate the radial locations rl1 and rl2 within
            /// the bounds of the given size.
            /// </summary>
            public static double CalculateMaxR_WithinBounds(RadialLocation rl1, RadialLocation rl2, Size availableSize)
            {
                /* ************************************************************** *\
                 * Child's left:   l = (r * cos(θ)) - (w/2)
                 * Child's right:  r = (r * cos(θ)) + (w/2)
                 * Child's top:    t = (r * -sin(θ)) - (h/2)
                 * Child's bottom: b = (r * -sin(θ)) + (h/2)

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

                var r = Math.Min(r_Width, r_Height);
                return r;
            }

            /// <summary>
            /// Calculate the minimum arrangement radius (r) which will accomodate the radial locations rl1 and rl2
            /// without overlapping the children controls.
            /// </summary>
            public static double CalculateMinR_NoBoundsOverlap(RadialLocation rl1, RadialLocation rl2)
            {
                // If two children at at the same radial location, then ignore for purposes of spacing
                // Error bound chosen as a small number at random (no particular meaning)
                if (DoubleHelper.EqualsWithinError(rl1.Cosθ, rl2.Cosθ, 1e-5))
                {
                    return 0;
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

                double r_Width = (rl1.Size.Width + rl2.Size.Width) / (2 * Math.Abs(rl1.Cosθ - rl2.Cosθ));
                double r_Height = (rl1.Size.Height + rl2.Size.Height) / (2 * Math.Abs(rl1.Sinθ - rl2.Sinθ));

                var r = Math.Min(r_Width, r_Height);
                return r;
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

            public Rect Boundary(double r) => new(
                (r * Cosθ) - (Size.Width / 2),
                (r * -Sinθ) - (Size.Height / 2), // Negative y to 'switch' from Cartisean co-ordinates (where increasing y is traditionally "up") to panel co-ordinates (where increasing y is "down")
                Size.Width,
                Size.Height);
        }
    }
}
