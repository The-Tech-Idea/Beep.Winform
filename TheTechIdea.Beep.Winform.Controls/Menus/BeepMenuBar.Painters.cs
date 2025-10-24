using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Menus;
using TheTechIdea.Beep.Winform.Controls.Menus.Helpers;
using TheTechIdea.Beep.Winform.Controls.Menus.Painters;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepMenuBar
    {
        #region Painter Fields
        private MenuBarStyle _menuBarStyle = MenuBarStyle.Classic;
        private IMenuBarPainter _painter;
        private MenuBarContext _context;
        #endregion

        #region Painter Properties
      

        /// <summary>
        /// Selects the menu painter Style (designer-visible property).
        /// </summary>
        [Category("Menu Bar")]
        [Description("Selects the menu painter Style.")]
        [DefaultValue(MenuBarStyle.Classic)]
        public MenuBarStyle MenuBarStyle
        {
            get => _menuBarStyle;
            set
            {
                if (_menuBarStyle != value)
                {
                    _menuBarStyle = value;
                    InitializePainter();
                    UpdateMenuBarLayout();
                    Invalidate();
                }
            }
        }


        /// <summary>
        /// Gets the current painter instance
        /// </summary>
        [Browsable(false)]
        public IMenuBarPainter CurrentPainter => _painter;

        /// <summary>
        /// Gets the current menu bar context
        /// </summary>
        [Browsable(false)]
        public MenuBarContext Context => _context;
        #endregion

        #region Painter Initialization
        /// <summary>
        /// Initializes the appropriate painter based on the current Style
        /// </summary>
        private void InitializePainter()
        {
            // Dispose existing painter
            _painter?.Dispose();

            // Create new painter based on Style
            _painter = _menuBarStyle switch
            {
                MenuBarStyle.Classic         => new ClassicMenuBarPainter(),
                MenuBarStyle.Modern          => new ModernMenuBarPainter(),
                MenuBarStyle.Material        => new MaterialMenuBarPainter(),
                MenuBarStyle.Compact         => new CompactMenuBarPainter(),
                MenuBarStyle.Breadcrumb      => new BreadcrumbMenuBarPainter(),
                MenuBarStyle.Tab             => new TabMenuBarPainter(),
                MenuBarStyle.Fluent          => new FluentMenuBarPainter(),
                MenuBarStyle.Bubble          => new BubbleMenuBarPainter(),
                MenuBarStyle.Floating        => new FloatingMenuBarPainter(),
                MenuBarStyle.DropdownCategory => new DropdownCategoryMenuBarPainter(),
                MenuBarStyle.CardLayout      => new CardLayoutMenuBarPainter(),
                MenuBarStyle.IconGrid        => new IconGridMenuBarPainter(),
                MenuBarStyle.MultiRow        => new MultiRowMenuBarPainter(),
                _ => new ClassicMenuBarPainter()
            };

            // Initialize painter with current theme
            _painter?.Initialize(this, _currentTheme);

            // Initialize context if not exists
            if (_context == null)
            {
                _context = new MenuBarContext();
            }

            // Update context with current settings
            UpdateMenuBarContext();
        }

        /// <summary>
        /// Updates the menu bar context with current control settings
        /// </summary>
        private void UpdateMenuBarContext()
        {
            if (_context == null) return;

            // Update basic properties
            _context.MenuItems.Clear();
            if (items != null)
            {
                _context.MenuItems.AddRange(items);
            }

            _context.SelectedIndex = _selectedIndex;
            _context.HoveredItemName = _hoveredMenuItemName ?? "";
            _context.TextFont = _textFont;
            _context.IconSize = new Size(ScaledImageSize, ScaledImageSize);
            _context.ItemHeight = ScaledMenuItemHeight;
            _context.ItemWidth = ScaledMenuItemWidth;
            _context.ShowIcons = true; // Default to showing icons
            _context.ShowText = true; // Default to showing text
            _context.ShowDropdownIndicators = true; // Default to showing dropdown indicators
            _context.IsInteractive = !DesignMode && Enabled;
            _context.CanBeFocused = CanBeFocused;

            // Update colors will be handled by painter during AdjustLayout
        }

        /// <summary>
        /// Updates the menu bar layout using the current painter
        /// </summary>
        private void UpdateMenuBarLayout()
        {
            if (_painter == null || _context == null) return;

            try
            {
                // Update context with current settings
                UpdateMenuBarContext();

                // Let painter adjust layout
                _context = _painter.AdjustLayout(DrawingRect, _context);

                // Update hit areas
                RefreshPainterHitAreas();
            }
            catch (Exception ex)
            {
                // Handle any painter errors gracefully
                System.Diagnostics.Debug.WriteLine($"MenuBar layout update error: {ex.Message}");
            }
        }

        /// <summary>
        /// Refreshes hit areas using the current painter
        /// </summary>
        private void RefreshPainterHitAreas()
        {
            if (_painter == null || _context == null) return;

            try
            {
                _painter.UpdateHitAreas(this, _context, OnPainterAreaHit);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"MenuBar hit area refresh error: {ex.Message}");
            }
        }

        /// <summary>
        /// Handles hit area notifications from the painter
        /// </summary>
        private void OnPainterAreaHit(string areaName, Rectangle rect)
        {
            // Extract menu item index from area name
            if (areaName.StartsWith("MenuItem_") && int.TryParse(areaName.Substring(9), out int index))
            {
                if (index >= 0 && index < items.Count)
                {
                    var item = items[index];
                    HandleMenuItemClick(item, index);
                }
            }
        }
        #endregion

        #region Painter Drawing Integration
        /// <summary>
        /// Draws the menu bar using the current painter
        /// </summary>
        private void DrawWithPainter(Graphics g)
        {
            if (_painter == null || _context == null || g == null) return;

            try
            {
                // Update layout before drawing
                UpdateMenuBarLayout();

                // Let painter handle the drawing
                _painter.DrawBackground(g, _context);
                _painter.DrawContent(g, _context);
                _painter.DrawForegroundAccents(g, _context);
            }
            catch (Exception ex)
            {
                // Fallback to basic drawing on error
                System.Diagnostics.Debug.WriteLine($"MenuBar painter drawing error: {ex.Message}");
                DrawFallback(g);
            }
        }

        /// <summary>
        /// Fallback drawing method when painter fails
        /// </summary>
        private void DrawFallback(Graphics g)
        {
            // Basic fallback drawing
            using var brush = new SolidBrush(BackColor);
            g.FillRectangle(brush, DrawingRect);

            using var textBrush = new SolidBrush(ForeColor);
            g.DrawString("Menu Bar", Font, textBrush, DrawingRect);
        }
        #endregion

        #region Painter Theme Integration
        /// <summary>
        /// Applies theme to the current painter
        /// </summary>
        private void ApplyThemeToPainter()
        {
            if (_painter != null && _currentTheme != null)
            {
                try
                {
                    _painter.ApplyTheme(_currentTheme);
                    UpdateMenuBarLayout();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"MenuBar painter theme application error: {ex.Message}");
                }
            }
        }
        #endregion

        #region Painter Size Calculation
        /// <summary>
        /// Calculates preferred size using the current painter
        /// </summary>
        private Size CalculatePreferredSizeWithPainter(Size proposedSize)
        {
            if (_painter == null || _context == null)
            {
                return DefaultSize;
            }

            try
            {
                // Update context before size calculation
                UpdateMenuBarContext();
                return _painter.CalculatePreferredSize(_context, proposedSize);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"MenuBar size calculation error: {ex.Message}");
                return DefaultSize;
            }
        }
        #endregion

        #region Painter Cleanup
        /// <summary>
        /// Disposes the current painter
        /// </summary>
        private void DisposePainter()
        {
            _painter?.Dispose();
            _painter = null;
        }
        #endregion
    }
}