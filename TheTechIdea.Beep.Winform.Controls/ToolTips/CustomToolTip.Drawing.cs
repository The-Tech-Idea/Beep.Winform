using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.ToolTips.Painters;
using TheTechIdea.Beep.Winform.Controls.ToolTips.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips
{
    /// <summary>
    /// Drawing partial class for CustomToolTip
    /// </summary>
    public partial class CustomToolTip
    {
        #region Paint Override

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_config == null || _painter == null)
                return;

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Get the actual painting bounds (exclude transparency key area)
            var bounds = new Rectangle(0, 0, ClientSize.Width, ClientSize.Height);

            // Use painter to render the tooltip
            _painter.Paint(g, bounds, _config, _actualPlacement, _theme);

            // Apply animation opacity
            if (_isAnimatingIn || _isAnimatingOut)
            {
                Opacity = _animationProgress;
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Fill with transparency key color for rounded corners
            using (var brush = new SolidBrush(TransparencyKey))
            {
                e.Graphics.FillRectangle(brush, ClientRectangle);
            }
        }

        #endregion

        #region Manual Drawing Methods (Fallback)

        /// <summary>
        /// Draw shadow effect manually (fallback if painter doesn't support)
        /// </summary>
        private void DrawShadow(Graphics g, Rectangle bounds)
        {
            if (!_config.ShowShadow)
                return;

            _painter?.PaintShadow(g, bounds, _config);
        }

        /// <summary>
        /// Draw background manually (fallback if painter doesn't support)
        /// </summary>
        private void DrawBackground(Graphics g, Rectangle bounds)
        {
            _painter?.PaintBackground(g, bounds, _config, _theme);
        }

        /// <summary>
        /// Draw arrow manually (fallback if painter doesn't support)
        /// </summary>
        private void DrawArrow(Graphics g, Rectangle bounds)
        {
            if (!_config.ShowArrow)
                return;

            var arrowPos = ToolTipHelpers.CalculateArrowPosition(bounds, _actualPlacement, DefaultArrowSize);
            _painter?.PaintArrow(g, arrowPos, _actualPlacement, _config, _theme);
        }

        /// <summary>
        /// Draw content manually (fallback if painter doesn't support)
        /// </summary>
        private void DrawContent(Graphics g, Rectangle bounds)
        {
            _painter?.PaintContent(g, bounds, _config, _theme);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Create rounded rectangle path
        /// </summary>
        private GraphicsPath CreateRoundedPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int d = Math.Min(radius * 2, Math.Min(rect.Width, rect.Height));

            if (d <= 1)
            {
                path.AddRectangle(rect);
                return path;
            }

            var arc = new Rectangle(rect.X, rect.Y, d, d);
            path.AddArc(arc, 180, 90);
            arc.X = rect.Right - d;
            path.AddArc(arc, 270, 90);
            arc.Y = rect.Bottom - d;
            path.AddArc(arc, 0, 90);
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();

            return path;
        }

        #endregion
    }
}
