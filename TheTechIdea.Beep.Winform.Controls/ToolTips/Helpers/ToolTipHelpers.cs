using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips.Helpers
{
    /// <summary>
    /// Helper utilities for tooltip positioning, sizing, and common operations
    /// </summary>
    public static class ToolTipHelpers
    {

        /// <summary>
        /// Calculate arrow position based on placement
        /// </summary>
        public static Point CalculateArrowPosition(Rectangle tooltipBounds, ToolTipPlacement placement, int arrowSize)
        {
            return placement switch
            {
                ToolTipPlacement.Top or ToolTipPlacement.TopStart or ToolTipPlacement.TopEnd =>
                    new Point(tooltipBounds.Left + tooltipBounds.Width / 2, tooltipBounds.Bottom),
                
                ToolTipPlacement.Bottom or ToolTipPlacement.BottomStart or ToolTipPlacement.BottomEnd =>
                    new Point(tooltipBounds.Left + tooltipBounds.Width / 2, tooltipBounds.Top),
                
                ToolTipPlacement.Left or ToolTipPlacement.LeftStart or ToolTipPlacement.LeftEnd =>
                    new Point(tooltipBounds.Right, tooltipBounds.Top + tooltipBounds.Height / 2),
                
                ToolTipPlacement.Right or ToolTipPlacement.RightStart or ToolTipPlacement.RightEnd =>
                    new Point(tooltipBounds.Left, tooltipBounds.Top + tooltipBounds.Height / 2),
                
                _ => new Point(tooltipBounds.Left + tooltipBounds.Width / 2, tooltipBounds.Bottom)
            };
        }

        /// <summary>
        /// Create arrow path for tooltip
        /// </summary>
        public static GraphicsPath CreateArrowPath(Point position, ToolTipPlacement placement, int arrowSize)
        {
            var path = new GraphicsPath();
            
            switch (placement)
            {
                case ToolTipPlacement.Top:
                case ToolTipPlacement.TopStart:
                case ToolTipPlacement.TopEnd:
                    // Arrow pointing down
                    path.AddPolygon(new[]
                    {
                        new Point(position.X - arrowSize, position.Y),
                        new Point(position.X, position.Y + arrowSize),
                        new Point(position.X + arrowSize, position.Y)
                    });
                    break;
                
                case ToolTipPlacement.Bottom:
                case ToolTipPlacement.BottomStart:
                case ToolTipPlacement.BottomEnd:
                    // Arrow pointing up
                    path.AddPolygon(new[]
                    {
                        new Point(position.X - arrowSize, position.Y),
                        new Point(position.X, position.Y - arrowSize),
                        new Point(position.X + arrowSize, position.Y)
                    });
                    break;
                
                case ToolTipPlacement.Left:
                case ToolTipPlacement.LeftStart:
                case ToolTipPlacement.LeftEnd:
                    // Arrow pointing right
                    path.AddPolygon(new[]
                    {
                        new Point(position.X, position.Y - arrowSize),
                        new Point(position.X + arrowSize, position.Y),
                        new Point(position.X, position.Y + arrowSize)
                    });
                    break;
                
                case ToolTipPlacement.Right:
                case ToolTipPlacement.RightStart:
                case ToolTipPlacement.RightEnd:
                    // Arrow pointing left
                    path.AddPolygon(new[]
                    {
                        new Point(position.X, position.Y - arrowSize),
                        new Point(position.X - arrowSize, position.Y),
                        new Point(position.X, position.Y + arrowSize)
                    });
                    break;
            }
            
            return path;
        }


        /// <summary>
        /// Apply easing function for animations
        /// </summary>
        public static double EaseOutCubic(double t)
        {
            return 1 - Math.Pow(1 - t, 3);
        }

        public static double EaseInOutCubic(double t)
        {
            return t < 0.5 ? 4 * t * t * t : 1 - Math.Pow(-2 * t + 2, 3) / 2;
        }

        public static double EaseBounce(double t)
        {
            const double n1 = 7.5625;
            const double d1 = 2.75;

            if (t < 1 / d1)
                return n1 * t * t;
            else if (t < 2 / d1)
                return n1 * (t -= 1.5 / d1) * t + 0.75;
            else if (t < 2.5 / d1)
                return n1 * (t -= 2.25 / d1) * t + 0.9375;
            else
                return n1 * (t -= 2.625 / d1) * t + 0.984375;
        }

        // CalculateOptimalPosition / CalculatePositionForPlacement / MeasureContentSize were
        // removed: they formed a THIRD implementation of tooltip placement, alongside
        // ToolTipPositioningHelpers and the old CustomToolTip.Positioning. None of the three
        // agreed, and these had no callers. Placement lives in
        // ToolTipPositioningHelpers.Resolve. What remains here is arrow geometry, which is used.
    }
}
