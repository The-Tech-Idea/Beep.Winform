using System;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Charts.Helpers
{
    /// <summary>
    /// Performance optimization helper for large dataset rendering.
    /// Provides utilities for point culling, vertex simplification, and batch rendering.
    /// </summary>
    public static class BeepChartPerformanceHelper
    {
        /// <summary>
        /// Culls points outside the viewport bounds with margin.
        /// </summary>
        public static List<ChartDataPoint> CullPointsOutsideViewport(
            List<ChartDataPoint> points,
            float viewportXMin, float viewportXMax,
            float viewportYMin, float viewportYMax,
            Func<ChartDataPoint, object> getXValue,
            Func<ChartDataPoint, object> getYValue,
            float margin = 0.1f)
        {
            if (points.Count <= 2)
                return new List<ChartDataPoint>(points);

            var culled = new List<ChartDataPoint>();
            var xMin = viewportXMin - margin;
            var xMax = viewportXMax + margin;
            var yMin = viewportYMin - margin;
            var yMax = viewportYMax + margin;

            foreach (var point in points)
            {
                var x = getXValue(point);
                var y = getYValue(point);

                if (x is float xVal && y is float yVal)
                {
                    if (xVal >= xMin && xVal <= xMax && yVal >= yMin && yVal <= yMax)
                    {
                        culled.Add(point);
                    }
                }
            }

            // Always keep at least first and last points
            if (culled.Count == 0)
            {
                culled.Add(points[0]);
                if (points.Count > 1)
                    culled.Add(points[points.Count - 1]);
            }

            return culled;
        }

        /// <summary>
        /// Decimates points by taking every Nth point, useful for quick grid line reduction.
        /// </summary>
        public static List<ChartDataPoint> DecimatePoints(List<ChartDataPoint> points, int decimationFactor)
        {
            if (decimationFactor <= 1 || points.Count <= 10)
                return new List<ChartDataPoint>(points);

            var decimated = new List<ChartDataPoint>();
            for (int i = 0; i < points.Count; i += decimationFactor)
            {
                decimated.Add(points[i]);
            }

            // Always include last point
            if (!decimated.Contains(points[points.Count - 1]))
            {
                decimated.Add(points[points.Count - 1]);
            }

            return decimated;
        }

        /// <summary>
        /// Estimates decimation factor based on point count and target performance level.
        /// </summary>
        public static int EstimateDecimationFactor(int totalPointCount, int targetMaxPoints = 200)
        {
            if (totalPointCount <= targetMaxPoints)
                return 1;

            var factor = (int)Math.Ceiling(totalPointCount / (float)targetMaxPoints);
            return Math.Max(1, factor);
        }

        /// <summary>
        /// Optimizes grid line density based on dataset size and zoom level.
        /// Returns recommended grid line interval.
        /// </summary>
        public static int OptimizeGridDensity(int datasetSize, float zoomLevel = 1.0f)
        {
            // Reduce grid density for large datasets
            if (datasetSize > 5000)
                return Math.Max(5, (int)(10 / zoomLevel));
            else if (datasetSize > 1000)
                return Math.Max(3, (int)(5 / zoomLevel));
            else if (datasetSize > 500)
                return Math.Max(2, (int)(3 / zoomLevel));

            return 1;
        }

        /// <summary>
        /// Recommends label rendering interval based on zoom level and point count.
        /// </summary>
        public static int RecommendLabelInterval(int datasetSize, float viewportRangePercent)
        {
            // Show fewer labels when zoomed out or many points visible
            var visiblePointsEstimate = (int)(datasetSize * viewportRangePercent);

            if (visiblePointsEstimate > 500)
                return Math.Max(20, visiblePointsEstimate / 10);
            else if (visiblePointsEstimate > 100)
                return Math.Max(10, visiblePointsEstimate / 8);
            else if (visiblePointsEstimate > 50)
                return 5;

            return 1;
        }

        /// <summary>
        /// Calculates rendering cost estimate (0.0 to 1.0) for UI feedback.
        /// </summary>
        public static float EstimateRenderingCost(int totalPointCount, int screenPixels)
        {
            var costFactor = Math.Min(1.0f, totalPointCount / (float)screenPixels);
            return costFactor;
        }

        /// <summary>
        /// Checks if performance optimization should be enabled.
        /// </summary>
        public static bool ShouldOptimizePerformance(int totalPointCount, int largeDatasetThreshold)
        {
            return totalPointCount > largeDatasetThreshold;
        }
    }
}
