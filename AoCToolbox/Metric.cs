using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoCToolbox
{
    public enum Metric
    {
        /// <summary>
        /// Also known as the chessboard metric, equivalent to the max of the individual component magnitudes
        /// </summary>
        Chebyshev,
        /// <summary>
        /// Also known as the Manhattan metric, equivalent to the sum of the individual component magnitudes
        /// </summary>
        Taxicab,
        /// <summary>
        /// Represents the Euclidean distance metric, which calculates the straight-line distance between two points in
        /// Euclidean space.
        /// </summary>
        /// <remarks>The Euclidean metric is commonly used in geometry, machine learning, and clustering
        /// algorithms to measure similarity or proximity between data points. It is defined as the square root of the
        /// sum of the squared differences between corresponding coordinates.</remarks>
        Euclidean
    }
}
