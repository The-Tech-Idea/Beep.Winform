using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Painters
{
    /// <summary>
    /// Adapts Beep's theme painter system to the docking UI.
    /// Extracts colors and fonts from the active Beep theme and applies them to docking elements.
    /// </summary>
    public class DockingPainterAdapter : IDockingPainter
    {
        private Color _backgroundColor = Color.FromArgb(240, 240, 240);
        private Color _foregroundColor = Color.FromArgb(33, 33, 33);
        private Color _borderColor = Color.FromArgb(200, 200, 200);
        private Color _hoverColor = Color.FromArgb(245, 245, 245);
        private Color _selectedColor = Color.FromArgb(0, 122, 255);
        private Color _disabledColor = Color.FromArgb(150, 150, 150);

        private Font _uiFont = new Font("Segoe UI", 9f, FontStyle.Regular);
        private Font _tabFont = new Font("Segoe UI", 9f, FontStyle.Regular);

        private int _tabStripHeight = 30;
        private int _splitterWidth = 5;

        private bool _disposed = false;

        /// <summary>
        /// Creates an adapter with default colors and fonts.
        /// Call UpdateFromTheme() to apply actual Beep theme colors.
        /// </summary>
        public DockingPainterAdapter()
        {
            // Initialize with reasonable defaults
            // These will be overridden by UpdateFromTheme() if Beep theme is available
        }

        /// <summary>
        /// Updates the adapter's colors and fonts from the active Beep theme.
        /// This method should be called:
        /// - When the adapter is created (to get current theme)
        /// - When BeepThemesManager.ThemeChanged event fires
        /// </summary>
        public void UpdateFromTheme()
        {
            UpdateFromTheme(BeepThemesManager.CurrentTheme ?? BeepThemesManager.GetDefaultTheme());
        }

        /// <summary>
        /// Updates adapter colours and fonts from a specific Beep theme.
        /// </summary>
        public void UpdateFromTheme(IBeepTheme theme)
        {
            var colors = DockingThemeColors.FromTheme(theme, useThemeColors: true);
            _backgroundColor = colors.PanelBackColor;
            _foregroundColor = colors.PanelForeColor;
            _borderColor = colors.BorderColor;
            _hoverColor = colors.HoverBackColor;
            _selectedColor = colors.ActiveTabBackColor;
            _disabledColor = theme?.DisabledForeColor ?? _disabledColor;

            if (theme != null)
            {
                UIFont = BeepThemesManager.ToFont(theme.BodyMedium) ?? _uiFont ?? new Font("Segoe UI", 9f, FontStyle.Regular);
                TabFont = BeepThemesManager.ToFont(theme.TabFont) ?? _tabFont ?? new Font("Segoe UI", 9f, FontStyle.Regular);
            }
            else
            {
                _uiFont = _uiFont ?? new Font("Segoe UI", 9f, FontStyle.Regular);
                _tabFont = _tabFont ?? new Font("Segoe UI", 9f, FontStyle.Regular);
            }

            ThemeChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Applies explicit theme colours to the adapter.
        /// Called by <see cref="BeepDockingManager.ApplyTheme"/> when the host application's
        /// theme changes.  Follows Krypton's IPalette update-and-invalidate pattern.
        /// </summary>
        /// <param name="background">Panel / strip background colour.</param>
        /// <param name="foreground">Title / tab text colour.</param>
        /// <param name="border">Panel border / splitter colour.</param>
        /// <param name="hover">Mouse-hover highlight colour.</param>
        /// <param name="accent">Active-tab / active-caption accent colour.</param>
        public void ApplyTheme(Color background, Color foreground, Color border,
                               Color hover, Color accent)
        {
            _backgroundColor = background;
            _foregroundColor = foreground;
            _borderColor     = border;
            _hoverColor      = hover;
            _selectedColor   = accent;

            ThemeChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raised whenever <see cref="ApplyTheme"/> or <see cref="UpdateFromTheme"/> change colours.
        /// Docking controls subscribe and call Invalidate() to repaint.
        /// </summary>
        public event EventHandler ThemeChanged;

        #region Color Properties

        public Color BackgroundColor
        {
            get => _backgroundColor;
            set => _backgroundColor = value;
        }

        public Color ForegroundColor
        {
            get => _foregroundColor;
            set => _foregroundColor = value;
        }

        public Color BorderColor
        {
            get => _borderColor;
            set => _borderColor = value;
        }

        public Color HoverColor
        {
            get => _hoverColor;
            set => _hoverColor = value;
        }

        public Color SelectedColor
        {
            get => _selectedColor;
            set => _selectedColor = value;
        }

        public Color DisabledColor
        {
            get => _disabledColor;
            set => _disabledColor = value;
        }

        #endregion

        #region Font Properties

        public Font UIFont
        {
            get => _uiFont ?? new Font("Segoe UI", 9f);
            set
            {
                _uiFont?.Dispose();
                _uiFont = value ?? new Font("Segoe UI", 9f);
            }
        }

        public Font TabFont
        {
            get => _tabFont ?? new Font("Segoe UI", 9f);
            set
            {
                _tabFont?.Dispose();
                _tabFont = value ?? new Font("Segoe UI", 9f);
            }
        }

        #endregion

        #region Layout Properties

        public int TabStripHeight
        {
            get => _tabStripHeight;
            set => _tabStripHeight = Math.Max(20, value);
        }

        public int SplitterWidth
        {
            get => _splitterWidth;
            set => _splitterWidth = Math.Max(3, value);
        }

        #endregion

        #region Drawing Methods

        public void DrawTabStrip(Graphics graphics, Rectangle bounds, TabInfo[] tabs, int activeTabIndex, Action<int> onTabClicked)
        {
            if (graphics == null || tabs == null || tabs.Length == 0)
                return;

            // Draw background
            using (var brush = new SolidBrush(BackgroundColor))
            {
                graphics.FillRectangle(brush, bounds);
            }

            // Draw border
            using (var pen = new Pen(BorderColor))
            {
                graphics.DrawRectangle(pen, bounds);
            }

            // Calculate tab widths
            int tabWidth = (tabs.Length > 0) ? bounds.Width / tabs.Length : bounds.Width;
            int x = bounds.X;

            // Draw each tab
            for (int i = 0; i < tabs.Length; i++)
            {
                var tabRect = new Rectangle(x, bounds.Y, tabWidth, bounds.Height);
                bool isActive = (i == activeTabIndex);
                bool isHovered = false;  // TODO: Track mouse position for hover state

                DrawTab(graphics, tabRect, tabs[i], isActive, isHovered);

                x += tabWidth;
            }
        }

        public void DrawTab(Graphics graphics, Rectangle bounds, TabInfo tab, bool isActive, bool isHovered)
        {
            if (graphics == null)
                return;

            // Draw tab background
            Color tabColor = isActive ? SelectedColor : (isHovered ? HoverColor : BackgroundColor);
            using (var brush = new SolidBrush(tabColor))
            {
                graphics.FillRectangle(brush, bounds);
            }

            // Draw tab border
            using (var pen = new Pen(BorderColor, 1f))
            {
                graphics.DrawRectangle(pen, bounds);
            }

            // Draw tab text
            var textRect = new Rectangle(
                bounds.X + 8,
                bounds.Y + 4,
                bounds.Width - 16,
                bounds.Height - 8);

            var format = new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter
            };

            using (var brush = new SolidBrush(ForegroundColor))
            {
                graphics.DrawString(tab.Title, TabFont, brush, textRect, format);
            }

            // Draw dirty indicator if needed
            if (tab.IsDirty)
            {
                var dotRect = new Rectangle(bounds.Right - 8, bounds.Y + 4, 4, 4);
                using (var brush = new SolidBrush(SelectedColor))
                {
                    graphics.FillEllipse(brush, dotRect);
                }
            }

            // Draw close button if allowed
            if (tab.CanClose)
            {
                var closeRect = new Rectangle(bounds.Right - 20, bounds.Y + 4, 12, bounds.Height - 8);
                using (var pen = new Pen(ForegroundColor, 1.5f))
                {
                    // Draw X
                    graphics.DrawLine(pen, closeRect.X + 2, closeRect.Y + 2, closeRect.X + 10, closeRect.Y + 10);
                    graphics.DrawLine(pen, closeRect.X + 10, closeRect.Y + 2, closeRect.X + 2, closeRect.Y + 10);
                }
            }

            format.Dispose();
        }

        public void DrawPanelChrome(Graphics graphics, Rectangle bounds, string title, Image icon, bool isDirty)
        {
            if (graphics == null)
                return;

            // Draw title bar background
            var titleBarRect = new Rectangle(bounds.X, bounds.Y, bounds.Width, 24);
            using (var brush = new SolidBrush(SelectedColor))
            {
                graphics.FillRectangle(brush, titleBarRect);
            }

            // Draw title text
            var textRect = new Rectangle(titleBarRect.X + 8, titleBarRect.Y, titleBarRect.Width - 16, titleBarRect.Height);
            using (var brush = new SolidBrush(Color.White))
            {
                graphics.DrawString(title ?? "Panel", UIFont, brush, textRect, StringFormat.GenericDefault);
            }

            // Draw border
            using (var pen = new Pen(BorderColor))
            {
                graphics.DrawRectangle(pen, bounds);
            }
        }

        public void DrawSplitter(Graphics graphics, Rectangle bounds, SplitterOrientation orientation, bool isHovered)
        {
            if (graphics == null)
                return;

            Color color = isHovered ? HoverColor : BorderColor;
            using (var brush = new SolidBrush(color))
            {
                graphics.FillRectangle(brush, bounds);
            }

            using (var pen = new Pen(BorderColor))
            {
                graphics.DrawRectangle(pen, bounds);
            }
        }

        public void DrawDockingGuide(Graphics graphics, Rectangle bounds, DockPosition position)
        {
            if (graphics == null)
                return;

            // Draw semi-transparent overlay for docking guide
            Color guideColor = Color.FromArgb(100, SelectedColor);
            using (var brush = new SolidBrush(guideColor))
            {
                graphics.FillRectangle(brush, bounds);
            }

            // Draw colored border based on position
            using (var pen = new Pen(SelectedColor, 3f))
            {
                graphics.DrawRectangle(pen, bounds);
            }
        }

        #endregion

        #region Layout Calculation

        public Size GetTabStripPreferredSize(TabInfo[] tabs, int availableWidth)
        {
            if (tabs == null || tabs.Length == 0)
                return new Size(0, TabStripHeight);

            // For now, use equal width distribution
            return new Size(availableWidth, TabStripHeight);
        }

        public int GetTabAtPoint(Point point, Rectangle bounds, TabInfo[] tabs)
        {
            if (tabs == null || tabs.Length == 0)
                return -1;

            if (!bounds.Contains(point))
                return -1;

            int tabWidth = bounds.Width / tabs.Length;
            int tabIndex = (point.X - bounds.X) / tabWidth;

            return (tabIndex >= 0 && tabIndex < tabs.Length) ? tabIndex : -1;
        }

        public Rectangle GetTabCloseButtonRect(Rectangle tabBounds, TabInfo tab)
        {
            if (!tab.CanClose)
                return Rectangle.Empty;

            return new Rectangle(
                tabBounds.Right - 20,
                tabBounds.Y + 4,
                12,
                tabBounds.Height - 8);
        }

        #endregion

        #region Cache Management

        public void InvalidateCache()
        {
            // Repaint from scratch, but keep color scheme
            // Typically called when theme changes
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (_disposed)
                return;

            _uiFont?.Dispose();
            _tabFont?.Dispose();

            _disposed = true;
        }

        #endregion
    }
}
