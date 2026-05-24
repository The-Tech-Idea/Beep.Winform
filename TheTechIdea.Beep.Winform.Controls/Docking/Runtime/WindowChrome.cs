using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;
using TheTechIdea.Beep.Winform.Controls.Docking.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Runtime
{
    /// <summary>
    /// Manages rendering and interaction with window chrome (title bar, tabs, borders).
    /// Handles tab strip rendering, tab hit-testing, and close/menu buttons.
    /// </summary>
    public class WindowChrome : IDisposable
    {
        private readonly IDockingPainter _painter;
        private Dictionary<string, TabItem> _tabCache = new Dictionary<string, TabItem>();
        private Dictionary<string, Rectangle> _tabBounds = new Dictionary<string, Rectangle>();
        private int _tabStripHeight = 30;
        private int _chromeHeight = 24;
        private bool _disposed = false;

        public WindowChrome(IDockingPainter painter)
        {
            _painter = painter ?? throw new ArgumentNullException(nameof(painter));
        }

        /// <summary>
        /// Renders the tab strip for a group at the specified bounds.
        /// </summary>
        public void DrawTabStrip(Graphics graphics, Rectangle bounds, DockGroup group)
        {
            if (graphics == null || group == null || group.Panels.Count == 0)
                return;

            try
            {
                // Draw background
                using (var brush = new SolidBrush(Color.FromArgb(240, 240, 240)))
                {
                    graphics.FillRectangle(brush, bounds);
                }

                // Draw border
                using (var pen = new Pen(Color.FromArgb(200, 200, 200)))
                {
                    graphics.DrawRectangle(pen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
                }

                // Draw tabs
                int xOffset = bounds.X + 4;
                foreach (var panel in group.Panels)
                {
                    var tabBounds = new Rectangle(xOffset, bounds.Y, 120, bounds.Height);
                    DrawTab(graphics, tabBounds, panel, panel == group.ActivePanel);
                    _tabBounds[panel.Key] = tabBounds;
                    xOffset += 124;  // Tab width + spacing

                    if (xOffset > bounds.Right)
                        break;  // Stop if we run out of space
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[WindowChrome] Error drawing tab strip: {ex.Message}");
            }
        }

        /// <summary>
        /// Renders a single tab.
        /// </summary>
        private void DrawTab(Graphics graphics, Rectangle bounds, DockPanel panel, bool isActive)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0)
                return;

            try
            {
                // Draw tab background
                Color bgColor = isActive ? Color.White : Color.FromArgb(230, 230, 230);
                using (var brush = new SolidBrush(bgColor))
                {
                    graphics.FillRectangle(brush, bounds);
                }

                // Draw tab border
                Color borderColor = isActive ? Color.FromArgb(100, 100, 100) : Color.FromArgb(180, 180, 180);
                using (var pen = new Pen(borderColor))
                {
                    graphics.DrawRectangle(pen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
                }

                // Draw tab text
                var textRect = new Rectangle(bounds.X + 4, bounds.Y + 2, bounds.Width - 28, bounds.Height - 4);
                var format = new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter
                };

                using (var font = new Font("Segoe UI", 9f))
                using (var brush = new SolidBrush(Color.Black))
                {
                    graphics.DrawString(panel.Title ?? "Panel", font, brush, textRect, format);
                }

                // Draw close button
                var closeButtonRect = new Rectangle(bounds.Right - 20, bounds.Y + 5, 16, 16);
                DrawCloseButton(graphics, closeButtonRect);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[WindowChrome] Error drawing tab: {ex.Message}");
            }
        }

        /// <summary>
        /// Draws a close button (X).
        /// </summary>
        private void DrawCloseButton(Graphics graphics, Rectangle bounds)
        {
            // Draw background circle
            using (var brush = new SolidBrush(Color.FromArgb(240, 240, 240)))
            {
                graphics.FillEllipse(brush, bounds);
            }

            // Draw X
            using (var pen = new Pen(Color.FromArgb(100, 100, 100), 2f))
            {
                int margin = 3;
                graphics.DrawLine(pen, bounds.X + margin, bounds.Y + margin, bounds.Right - margin, bounds.Bottom - margin);
                graphics.DrawLine(pen, bounds.Right - margin, bounds.Y + margin, bounds.X + margin, bounds.Bottom - margin);
            }
        }

        /// <summary>
        /// Hit-tests to find which tab was clicked.
        /// </summary>
        public string HitTestTab(Point point)
        {
            foreach (var kvp in _tabBounds)
            {
                if (kvp.Value.Contains(point))
                {
                    return kvp.Key;
                }
            }
            return null;
        }

        /// <summary>
        /// Hit-tests to find if a close button was clicked.
        /// </summary>
        public string HitTestCloseButton(Point point, Rectangle tabStripBounds)
        {
            if (!tabStripBounds.Contains(point))
                return null;

            foreach (var kvp in _tabBounds)
            {
                var closeButtonRect = new Rectangle(kvp.Value.Right - 20, kvp.Value.Y + 5, 16, 16);
                if (closeButtonRect.Contains(point))
                {
                    return kvp.Key;
                }
            }
            return null;
        }

        /// <summary>
        /// Renders chrome decorations (title bar, borders).
        /// </summary>
        public void DrawChrome(Graphics graphics, Rectangle bounds)
        {
            if (graphics == null)
                return;

            try
            {
                // Draw top border/title bar area
                using (var brush = new SolidBrush(Color.FromArgb(45, 45, 48)))
                {
                    graphics.FillRectangle(brush, bounds.X, bounds.Y, bounds.Width, _chromeHeight);
                }

                // Draw sides and bottom
                using (var pen = new Pen(Color.FromArgb(100, 100, 100)))
                {
                    graphics.DrawRectangle(pen, bounds.X, bounds.Y + _chromeHeight, bounds.Width - 1, bounds.Height - _chromeHeight - 1);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[WindowChrome] Error drawing chrome: {ex.Message}");
            }
        }

        /// <summary>
        /// Draws a splitter bar between panels.
        /// </summary>
        public void DrawSplitter(Graphics graphics, Rectangle bounds, bool isVertical)
        {
            if (graphics == null)
                return;

            try
            {
                // Draw splitter background
                Color splitterColor = Color.FromArgb(200, 200, 200);
                using (var brush = new SolidBrush(splitterColor))
                {
                    graphics.FillRectangle(brush, bounds);
                }

                // Draw center grip
                if (isVertical)
                {
                    // Vertical splitter - draw dots for horizontal drag
                    int centerY = bounds.Y + bounds.Height / 2;
                    using (var brush = new SolidBrush(Color.FromArgb(150, 150, 150)))
                    {
                        graphics.FillEllipse(brush, bounds.X + 2, centerY - 6, 2, 2);
                        graphics.FillEllipse(brush, bounds.X + 2, centerY - 2, 2, 2);
                        graphics.FillEllipse(brush, bounds.X + 2, centerY + 2, 2, 2);
                    }
                }
                else
                {
                    // Horizontal splitter - draw dots for vertical drag
                    int centerX = bounds.X + bounds.Width / 2;
                    using (var brush = new SolidBrush(Color.FromArgb(150, 150, 150)))
                    {
                        graphics.FillEllipse(brush, centerX - 6, bounds.Y + 2, 2, 2);
                        graphics.FillEllipse(brush, centerX - 2, bounds.Y + 2, 2, 2);
                        graphics.FillEllipse(brush, centerX + 2, bounds.Y + 2, 2, 2);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[WindowChrome] Error drawing splitter: {ex.Message}");
            }
        }

        /// <summary>
        /// Draws a docking guide overlay.
        /// </summary>
        public void DrawDockingGuide(Graphics graphics, Rectangle bounds, DockPosition position)
        {
            if (graphics == null)
                return;

            try
            {
                Color guideColor = Color.FromArgb(100, 200, 255);
                int thickness = 4;

                switch (position)
                {
                    case DockPosition.Left:
                        using (var brush = new SolidBrush(guideColor))
                        {
                            graphics.FillRectangle(brush, bounds.X, bounds.Y, thickness, bounds.Height);
                        }
                        break;

                    case DockPosition.Right:
                        using (var brush = new SolidBrush(guideColor))
                        {
                            graphics.FillRectangle(brush, bounds.Right - thickness, bounds.Y, thickness, bounds.Height);
                        }
                        break;

                    case DockPosition.Top:
                        using (var brush = new SolidBrush(guideColor))
                        {
                            graphics.FillRectangle(brush, bounds.X, bounds.Y, bounds.Width, thickness);
                        }
                        break;

                    case DockPosition.Bottom:
                        using (var brush = new SolidBrush(guideColor))
                        {
                            graphics.FillRectangle(brush, bounds.X, bounds.Bottom - thickness, bounds.Width, thickness);
                        }
                        break;

                    case DockPosition.Fill:
                        // Draw crosshair pattern
                        using (var pen = new Pen(guideColor, 2f))
                        {
                            graphics.DrawLine(pen, bounds.X, bounds.Y, bounds.Right, bounds.Bottom);
                            graphics.DrawLine(pen, bounds.Right, bounds.Y, bounds.X, bounds.Bottom);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[WindowChrome] Error drawing docking guide: {ex.Message}");
            }
        }

        /// <summary>
        /// Clears the tab bounds cache.
        /// </summary>
        public void InvalidateCache()
        {
            _tabBounds.Clear();
            _tabCache.Clear();
        }

        /// <summary>
        /// Updates chrome dimensions based on painter metrics.
        /// </summary>
        public void UpdateMetrics(int tabStripHeight, int chromeHeight)
        {
            _tabStripHeight = Math.Max(20, tabStripHeight);
            _chromeHeight = Math.Max(20, chromeHeight);
        }

        /// <summary>
        /// Gets diagnostic information.
        /// </summary>
        public ChromeDiagnostics GetDiagnostics()
        {
            return new ChromeDiagnostics
            {
                TabStripHeight = _tabStripHeight,
                ChromeHeight = _chromeHeight,
                CachedTabs = _tabBounds.Count,
                TabBounds = new Dictionary<string, Rectangle>(_tabBounds)
            };
        }

        /// <summary>
        /// Disposes managed resources.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            InvalidateCache();
            _disposed = true;
        }
    }

    /// <summary>
    /// Represents a single tab in the tab strip.
    /// </summary>
    public class TabItem
    {
        public string PanelKey { get; set; }
        public Rectangle Bounds { get; set; }
        public bool IsActive { get; set; }
        public bool IsHovered { get; set; }
    }

    /// <summary>
    /// Diagnostic information from WindowChrome.
    /// </summary>
    public class ChromeDiagnostics
    {
        public int TabStripHeight { get; set; }
        public int ChromeHeight { get; set; }
        public int CachedTabs { get; set; }
        public Dictionary<string, Rectangle> TabBounds { get; set; } = new Dictionary<string, Rectangle>();
    }
}
