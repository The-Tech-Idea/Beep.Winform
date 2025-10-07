using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    /// <summary>
    /// Helper class responsible for rendering the header toolbar area above column headers in BeepGridPro.
    /// This is the area painted by IPaintGridHeader implementations (search, filters, actions, etc.).
    /// </summary>
    public sealed class GridHeaderPainterHelper
    {
        private readonly BeepGridPro _grid;

        // Configuration properties
        public bool ShowGridLines { get; set; } = true;
        public DashStyle GridLineStyle { get; set; } = DashStyle.Solid;

        private IBeepTheme? Theme => _grid?._currentTheme;

        public GridHeaderPainterHelper(BeepGridPro grid)
        {
            _grid = grid ?? throw new ArgumentNullException(nameof(grid));
        }

        /// <summary>
        /// Main entry point to draw the header toolbar area (above column headers).
        /// Delegates to the assigned IPaintGridHeader painter or uses default rendering.
        /// </summary>
        public void DrawHeaderToolbar(Graphics g)
        {
            if (g == null || _grid?.Layout == null) return;

            var headerRect = _grid.Layout.HeaderRect;
            if (headerRect.Height <= 0 || headerRect.Width <= 0) return;

            // If a custom painter is assigned, delegate to it
            //if (_grid.HeaderPainter != null)
            //{
            //    try
            //    {
            //        _grid.HeaderPainter.Paint(g, headerRect, _grid);
            //    }
            //    catch (Exception ex)
            //    {
            //        // Fallback to default rendering if painter fails
            //        System.Diagnostics.Debug.WriteLine($"HeaderPainter failed: {ex.Message}");
            //        DrawDefaultHeaderToolbar(g, headerRect);
            //    }
            //}
            //else
            //{
            //    // No painter assigned, use default rendering
            //    DrawDefaultHeaderToolbar(g, headerRect);
            //}
        }

        /// <summary>
        /// Default header toolbar rendering when no IPaintGridHeader painter is assigned.
        /// Fills background and draws border.
        /// </summary>
        private void DrawDefaultHeaderToolbar(Graphics g, Rectangle headerRect)
        {
            // Fill background using BeepStyling if available
            if (_grid.UseThemeColors && BeepStyling.CurrentTheme != null)
            {
                BeepStyling.CurrentTheme = (IBeepTheme)Theme;
                BeepStyling.UseThemeColors = _grid.UseThemeColors;
                BeepStyling.PaintStyleBackground(g, headerRect, _grid.ControlStyle);
            }
            else
            {
                using (var brush = new SolidBrush(Theme?.GridHeaderBackColor ?? SystemColors.Control))
                {
                    g.FillRectangle(brush, headerRect);
                }
            }

            // Draw bottom border if grid lines enabled
            if (ShowGridLines)
            {
                using (var pen = new Pen(Theme?.GridLineColor ?? SystemColors.ControlDark))
                {
                    pen.DashStyle = GridLineStyle;
                    g.DrawLine(pen, headerRect.Left, headerRect.Bottom, headerRect.Right, headerRect.Bottom);
                }
            }

            // Optional: Draw placeholder text indicating no painter is assigned
            if (_grid.IsAncestorSiteInDesignMode)
            {
                DrawDesignModeHint(g, headerRect);
            }
        }

        private void DrawDesignModeHint(Graphics g, Rectangle headerRect)
        {
            string hint = "Header Toolbar (Assign HeaderPainter to customize)";
            var font = SystemFonts.DefaultFont;
            var color = Color.FromArgb(150, SystemColors.ControlText);
            var format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            using (var brush = new SolidBrush(color))
            {
                g.DrawString(hint, font, brush, headerRect, format);
            }
        }

        /// <summary>
        /// Helper method to test if a point is within the header toolbar area.
        /// Used for hit testing and event routing.
        /// </summary>
        public bool ContainsPoint(Point pt)
        {
            if (_grid?.Layout == null) return false;
            return _grid.Layout.HeaderRect.Contains(pt);
        }

        /// <summary>
        /// Get the current header toolbar rectangle bounds.
        /// </summary>
        public Rectangle GetBounds()
        {
            return _grid?.Layout?.HeaderRect ?? Rectangle.Empty;
        }
    }
}
