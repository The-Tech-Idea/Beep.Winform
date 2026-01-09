using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ToolTips.Painters;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips
{
    public partial class CustomToolTip
    {
        #region Drawing Methods

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            // Use _currentTheme if available (from ApplyTheme()), otherwise use _theme
            var activeTheme = _currentTheme ?? _theme;

            if (_config == null || _painter == null)
                return;

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Get the actual painting bounds
            var bounds = new Rectangle(0, 0, ClientSize.Width, ClientSize.Height);

            // Use painter to render the tooltip with active theme (from ApplyTheme() if available)
            _painter.Paint(g, bounds, _config, _actualPlacement, activeTheme);

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

        #region Cleanup

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _animationTimer?.Stop();
                _animationTimer?.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
