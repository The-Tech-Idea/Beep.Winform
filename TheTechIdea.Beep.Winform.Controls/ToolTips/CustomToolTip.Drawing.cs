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

            // Use painter to render the tooltip with active theme (from ApplyTheme() if available).
            //
            // Guarded: an exception escaping a painter's Paint leaves WinForms drawing the red-X
            // error box in place of the whole tooltip, with no indication of what failed. Falling
            // back to the default painter keeps the tooltip usable and puts the cause in the
            // debug log instead of on the user's screen.
            try
            {
                _painter.Paint(g, bounds, _config, _actualPlacement, activeTheme);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[CustomToolTip] {_painter.GetType().Name}.Paint failed: {ex}");

                if (_painter is not BeepStyledToolTipPainter)
                {
                    _painter = new BeepStyledToolTipPainter();
                    try { _painter.Paint(g, bounds, _config, _actualPlacement, activeTheme); }
                    catch (Exception fallbackEx)
                    {
                        System.Diagnostics.Debug.WriteLine(
                            $"[CustomToolTip] fallback painter also failed: {fallbackEx.Message}");
                    }
                }
            }

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
