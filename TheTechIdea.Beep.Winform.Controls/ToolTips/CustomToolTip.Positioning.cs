using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ToolTips.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips
{
    public partial class CustomToolTip
    {
        #region Positioning Methods

        /// <summary>
        /// Calculate optimal placement based on available screen space
        /// Uses ToolTipPositioningHelpers for smart collision detection
        /// </summary>
        private ToolTipPlacement CalculatePlacement(Point targetPosition)
        {
            if (_config.Placement != ToolTipPlacement.Auto)
            {
                // Still validate that preferred placement fits
                var targetRect = new Rectangle(targetPosition, new Size(1, 1));
                var tooltipSize = Size;
                var placement = ToolTipPositioningHelpers.CalculateOptimalPlacement(
                    targetRect, tooltipSize, _config.Placement, _config.Offset);
                return placement;
            }

            // Use smart positioning helper
            var targetRectForCalc = new Rectangle(targetPosition, new Size(1, 1));
            return ToolTipPositioningHelpers.CalculateOptimalPlacement(
                targetRectForCalc, Size, ToolTipPlacement.Auto, _config.Offset);
        }

        /// <summary>
        /// Adjust position based on placement
        /// Uses ToolTipStyleHelpers for recommended offset
        /// </summary>
        private Point AdjustPositionForPlacement(Point targetPosition, ToolTipPlacement placement)
        {
            int arrowSize = _config.ShowArrow 
                ? ToolTipStyleHelpers.GetRecommendedArrowSize(_config.Style) 
                : 0;
            int offset = _config.Offset > 0 
                ? _config.Offset 
                : ToolTipStyleHelpers.GetRecommendedOffset(_config.Style);

            return placement switch
            {
                ToolTipPlacement.Top => new Point(
                    targetPosition.X - Width / 2,
                    targetPosition.Y - Height - arrowSize - offset
                ),
                ToolTipPlacement.Bottom => new Point(
                    targetPosition.X - Width / 2,
                    targetPosition.Y + arrowSize + offset
                ),
                ToolTipPlacement.Left => new Point(
                    targetPosition.X - Width - arrowSize - offset,
                    targetPosition.Y - Height / 2
                ),
                ToolTipPlacement.Right => new Point(
                    targetPosition.X + arrowSize + offset,
                    targetPosition.Y - Height / 2
                ),
                ToolTipPlacement.TopStart => new Point(
                    targetPosition.X,
                    targetPosition.Y - Height - arrowSize - offset
                ),
                ToolTipPlacement.TopEnd => new Point(
                    targetPosition.X - Width,
                    targetPosition.Y - Height - arrowSize - offset
                ),
                ToolTipPlacement.BottomStart => new Point(
                    targetPosition.X,
                    targetPosition.Y + arrowSize + offset
                ),
                ToolTipPlacement.BottomEnd => new Point(
                    targetPosition.X - Width,
                    targetPosition.Y + arrowSize + offset
                ),
                _ => new Point(targetPosition.X - Width / 2, targetPosition.Y + arrowSize + offset)
            };
        }

        /// <summary>
        /// Constrain tooltip position to stay within screen bounds
        /// Uses ToolTipPositioningHelpers for better edge handling
        /// </summary>
        private Point ConstrainToScreen(Point position)
        {
            var tooltipBounds = new Rectangle(position, Size);
            return ToolTipPositioningHelpers.AdjustForScreenEdges(tooltipBounds, position);
        }

        #endregion
    }
}
