using TheTechIdea.Beep.Winform.Controls.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ToolTips.Painters;
using TheTechIdea.Beep.Winform.Controls.ToolTips.Helpers;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

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
                BeepLog.FallbackOnce($"ToolTip.paint:{_painter.GetType().Name}", this,
                    $"paint with {_painter.GetType().Name}; falling back to BeepStyledToolTipPainter", ex);

                if (_painter is not BeepStyledToolTipPainter)
                {
                    _painter = new BeepStyledToolTipPainter();
                    try { _painter.Paint(g, bounds, _config, _actualPlacement, activeTheme); }
                    catch (Exception fallbackEx)
                    {
                        BeepLog.FailureOnce("ToolTip.paint.fallback", this,
                            "paint with the fallback painter", fallbackEx);
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
            // The card colour, NOT a transparency key: the region already removes everything
            // outside the silhouette, and painting the same colour underneath means the card's
            // antialiased edge blends into itself rather than into a key colour. Filling a
            // contrasting colour here is what draws a light outline around the whole tooltip.
            var theme = _currentTheme ?? _theme ?? BeepThemesManager.CurrentTheme;
            Color surface = _config != null
                ? ToolTipThemeHelpers.GetToolTipBackColor(theme, _config.Type, _config.BackColor)
                : BackColor;

            using (var brush = new SolidBrush(surface))
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
                ThemeManagement.BeepThemesManager.ThemeChanged -= OnGlobalToolTipThemeChanged;
                _animationTimer?.Stop();
                _animationTimer?.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
