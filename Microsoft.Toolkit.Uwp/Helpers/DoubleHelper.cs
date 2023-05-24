// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Microsoft.Toolkit.Uwp.Helpers
{
    /// <summary>
    /// This class provides static helper methods for <see cref="double"/> objects.
    /// </summary>
    public static class DoubleHelper
    {
        /// <summary>
        /// Returns a value indicating whether two double values are considered equal within ome boundary of error.
        /// </summary>
        /// <param name="value1">The first value to compare.</param>
        /// <param name="value2">The second value to compare.</param>
        /// <param name="errorBound">The margin of error for which <paramref name="value1"/> and <paramref name="value2"/> are considered equal.</param>
        /// <returns>
        /// true if <paramref name="value1"/> and <paramref name="value2"/> lie within <paramref name="errorBound"/> of each other; otherwise, false.
        /// </returns>
        public static bool EqualsWithinError(double value1, double value2, double errorBound)
        {
            if (Math.Abs(value1 - value2) < Math.Abs(errorBound))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
