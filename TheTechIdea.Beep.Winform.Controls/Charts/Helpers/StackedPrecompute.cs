using System.Collections.Generic;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Charts.Helpers
{
    /// <summary>
    /// Shared stacked-chart precomputation used by Line, Bar, and Area
    /// series painters.  Eliminates the ~40-line copy-pasted block
    /// of Stack100 totals and cumulative-height tracking.
    /// </summary>
    internal sealed class StackedPrecompute
    {
        /// <summary>
        /// Per-index percentage totals for Stack100 mode.
        /// Each entry is the sum of all visible series' Y values at
        /// that index.
        /// </summary>
        public float[] Totals { get; }

        /// <summary>
        /// Running cumulative height for standard Stacked mode.
        /// Initialised to all-zero; callers advance it per-series.
        /// </summary>
        public float[] Cumulative { get; }

        private StackedPrecompute(float[] totals, float[] cumulative)
        {
            Totals = totals;
            Cumulative = cumulative;
        }

        /// <summary>
        /// Computes the stacked pre-amble for the given series list.
        /// Returns null when neither Stack nor Stack100 is active,
        /// so painters can branch with a simple null check.
        /// </summary>
        public static StackedPrecompute? Calculate(
            List<ChartDataSeries> data,
            StackedMode mode,
            Func<ChartDataPoint, object> toY)
        {
            if (mode == StackedMode.None || data == null || data.Count == 0)
                return null;

            int n = 0;
            foreach (var s in data)
                if (s.Points != null && s.Points.Count > n)
                    n = s.Points.Count;
            if (n == 0) return null;

            bool stack100 = mode == StackedMode.Stack100;
            var totals = new float[n];
            if (stack100)
            {
                for (int i = 0; i < n; i++)
                {
                    float sum = 0f;
                    foreach (var s in data)
                    {
                        if (!s.Visible || s.Points == null || i >= s.Points.Count) continue;
                        sum += toY(s.Points[i]) is float yf ? yf : 0f;
                    }
                    totals[i] = sum == 0 ? 1f : sum;
                }
            }

            return new StackedPrecompute(totals, new float[n]);
        }
    }
}
