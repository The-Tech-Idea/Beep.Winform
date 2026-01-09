using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.GridX.Painters;
 

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    /// <summary>
    /// Helper class responsible for rendering the navigation/paging area at the bottom of BeepGridPro.
    /// Supports both legacy button-based navigation and modern painter-based styles.
    /// </summary>
    public sealed class GridNavigationPainterHelper
    {
        private readonly BeepGridPro _grid;

        // Navigator button controls (legacy mode)
        private BeepButton? _btnInsert, _btnDelete, _btnSave, _btnCancel;
        private BeepButton? _btnFirst, _btnPrev, _btnNext, _btnLast;
        private BeepButton? _btnQuery, _btnFilter, _btnPrint;
        private BeepLabel? _lblPageInfo;

        // Painter-based navigation
        private INavigationPainter? _currentPainter;
        private navigationStyle _navigationStyle = navigationStyle.Material;

        // Configuration properties
        public bool ShowGridLines { get; set; } = true;
        public DashStyle GridLineStyle { get; set; } = DashStyle.Solid;
        public bool UsePainterNavigation { get; set; } = true; // Default to new painter system

        private IBeepTheme? Theme => _grid?._currentTheme;

        /// <summary>
        /// Gets or sets the navigation Style when using painter-based navigation
        /// </summary>
        public navigationStyle NavigationStyle
        {
            get => _navigationStyle;
            set
            {
                if (_navigationStyle != value)
                {
                    _navigationStyle = value;
                    _currentPainter = null; // Force recreation on next draw
                    
                    // Force immediate recreation of painter
                    if (value != navigationStyle.None)
                    {
                        _currentPainter = NavigationPainterFactory.CreatePainter(value);
                    }
                }
            }
        }

        public GridNavigationPainterHelper(BeepGridPro grid)
        {
            _grid = grid ?? throw new ArgumentNullException(nameof(grid));
        }

        /// <summary>
        /// Main entry point to draw the navigator area.
        /// </summary>
        public void DrawNavigatorArea(Graphics g)
        {
            var navRect = _grid.Layout.NavigatorRect;
            
            if (navRect.IsEmpty) return;

            // Use painter-based navigation if enabled
            if (UsePainterNavigation)
            {
                DrawPainterNavigation(g, navRect);
                return;
            }

            // Legacy button-based navigation
            DrawLegacyNavigation(g, navRect);
        }

        /// <summary>
        /// Draws navigation using the modern painter system
        /// </summary>
        private void DrawPainterNavigation(Graphics g, Rectangle navRect)
        {
            try
            {
                // Handle None Style - paint blank background only
                if (_navigationStyle == navigationStyle.None)
                {
                    using (var brush = new SolidBrush(Theme?.GridBackColor ?? SystemColors.Window))
                    {
                        g.FillRectangle(brush, navRect);
                    }
                    return;
                }
                
                // Ensure we have a theme
                if (_grid._currentTheme == null)
                {
                    _grid._currentTheme = BeepThemesManager.GetTheme(_grid.Theme) ?? BeepThemesManager.GetDefaultTheme();
                }
                
                // ALWAYS create painter to ensure it exists
                if (_currentPainter == null || _currentPainter.Style != _navigationStyle)
                {
                    _currentPainter = NavigationPainterFactory.CreatePainter(_navigationStyle);
                }

                if (_currentPainter == null)
                {
                    // Fallback - paint background using theme color
                    using (var brush = new SolidBrush(Theme?.GridBackColor ?? SystemColors.Window))
                    {
                        g.FillRectangle(brush, navRect);
                    }
                    return;
                }

                // Delegate to painter with guaranteed theme
                IBeepTheme theme = _grid._currentTheme;
                _currentPainter.PaintNavigation(g, navRect, _grid, theme);
            }
            catch (Exception ex)
            {
                // Silently handle navigation drawing errors
                // Paint error indicator
                using (var brush = new SolidBrush(Color.Orange))
                {
                    g.FillRectangle(brush, navRect);
                }
                using (var font = new Font("Arial", 8))
                using (var textBrush = new SolidBrush(Color.Black))
                {
                    g.DrawString($"Error: {ex.Message}", font, textBrush, navRect);
                }
            }
        }

        /// <summary>
        /// Draws navigation using the legacy button system
        /// </summary>
        private void DrawLegacyNavigation(Graphics g, Rectangle navRect)
        {
            // Clear existing navigator hit tests
            _grid.ClearHitList();

            // Fill navigator background
            using (var brush = new SolidBrush(Theme?.GridHeaderBackColor ?? SystemColors.Control))
            {
                g.FillRectangle(brush, navRect);
            }

            // Top border
            if (ShowGridLines)
            {
                using (var pen = new Pen(Theme?.GridLineColor ?? SystemColors.ControlDark))
                {
                    pen.DashStyle = GridLineStyle;
                    g.DrawLine(pen, navRect.Left, navRect.Top, navRect.Right, navRect.Top);
                }
            }

            // Ensure buttons exist and are themed
            EnsureNavigatorButtons();
            SyncButtonThemes();

            // Layout constants
            const int buttonWidth = 28;
            const int buttonHeight = 24;
            const int padding = 8;
            const int spacing = 6;
            const int sectionSpacing = 16;

            int y = navRect.Top + (navRect.Height - buttonHeight) / 2;

            // Determine layout mode based on GridStyle
            int minRequiredWidth = (buttonWidth * 10) + (spacing * 9) + (padding * 2) + (sectionSpacing * 2) + 200;
            bool compactMode = navRect.Width < minRequiredWidth;

            bool forceCompact = false;
            bool hideUtilities = false;
            bool hidePageInfo = false;

            switch (_grid.GridStyle)
            {
                case BeepGridStyle.Compact:
                    forceCompact = true;
                    break;
                case BeepGridStyle.Minimal:
                case BeepGridStyle.Borderless:
                    forceCompact = true;
                    hideUtilities = true;
                    hidePageInfo = true;
                    break;
            }

            compactMode = forceCompact || compactMode;

            if (compactMode)
            {
                DrawCompactNavigator(g, navRect, buttonWidth, buttonHeight, padding, spacing, y, !hideUtilities);
            }
            else
            {
                DrawFullNavigator(g, navRect, buttonWidth, buttonHeight, padding, spacing, sectionSpacing, y, !hideUtilities, !hidePageInfo);
            }
        }

        /// <summary>
        /// Gets the recommended height for the current navigation Style
        /// </summary>
        public int GetRecommendedNavigatorHeight()
        {
            if (UsePainterNavigation)
            {
                return NavigationPainterFactory.GetRecommendedHeight(_navigationStyle);
            }
            return 32; // Legacy default height
        }

        private void EnsureNavigatorButtons()
        {
            if (_btnFirst != null) return;

            // CRUD buttons
            _btnInsert = new BeepButton { ImagePath = Svgs.NavPlus, Theme = _grid.Theme };
            _btnDelete = new BeepButton { ImagePath = Svgs.NavMinus, Theme = _grid.Theme };
            _btnSave = new BeepButton { ImagePath = Svgs.FloppyDisk, Theme = _grid.Theme };
            _btnCancel = new BeepButton { ImagePath = Svgs.NavBackArrow, Theme = _grid.Theme };

            // Navigation buttons
            _btnFirst = new BeepButton { ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.angle-double-small-left.svg", Theme = _grid.Theme };
            _btnPrev = new BeepButton { ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.angle-small-left.svg", Theme = _grid.Theme };
            _btnNext = new BeepButton { ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.angle-small-right.svg", Theme = _grid.Theme };
            _btnLast = new BeepButton { ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.angle-double-small-right.svg", Theme = _grid.Theme };

            // Page info label
            _lblPageInfo = new BeepLabel { Theme = _grid.Theme, Text = "Page 1 of 1 — 0 records" };

            // Utility buttons
            _btnQuery = new BeepButton { ImagePath = Svgs.NavSearch, Theme = _grid.Theme };
            _btnFilter = new BeepButton { ImagePath = Svgs.NavWaving, Theme = _grid.Theme };
            _btnPrint = new BeepButton { ImagePath = Svgs.NavPrinter, Theme = _grid.Theme };

            foreach (var btn in new[] { _btnInsert, _btnDelete, _btnSave, _btnCancel, _btnFirst, _btnPrev, _btnNext, _btnLast,
                                       _btnQuery, _btnFilter, _btnPrint })
            {
                if (btn != null)
                {
                    btn.Text = "";
                    btn.UseThemeFont = true;
                    btn.AutoSize = false;
                }
            }

            if (_lblPageInfo != null)
            {
                _lblPageInfo.IsChild = true;
                _lblPageInfo.UseThemeFont = true;
                _lblPageInfo.AutoSize = true;
            }
        }

        private void SyncButtonThemes()
        {
            foreach (var btn in new[] { _btnFirst, _btnPrev, _btnNext, _btnLast, _btnInsert, _btnDelete, _btnSave, _btnCancel,
                                       _btnQuery, _btnFilter, _btnPrint })
            {
                if (btn != null)
                {
                    btn.Theme = _grid.Theme;
                    btn.Text = "";
                    btn.UseThemeFont = true;
                    btn.AutoSize = false;
                }
            }

            if (_lblPageInfo != null)
            {
                _lblPageInfo.Theme = _grid.Theme;
            }
        }

        private void DrawCompactNavigator(Graphics g, Rectangle navRect, int buttonWidth, int buttonHeight, int padding, int spacing, int y, bool showUtilities)
        {
            // Left: CRUD buttons
            int leftX = navRect.Left + padding;
            var insertRect = new Rectangle(leftX, y, buttonWidth, buttonHeight); leftX += buttonWidth + spacing;
            var deleteRect = new Rectangle(leftX, y, buttonWidth, buttonHeight); leftX += buttonWidth + spacing;
            var saveRect = new Rectangle(leftX, y, buttonWidth, buttonHeight); leftX += buttonWidth + spacing;
            var cancelRect = new Rectangle(leftX, y, buttonWidth, buttonHeight);

            RegisterAndDrawButton(g, "Insert", insertRect, _btnInsert, () => _grid.InsertNew());
            RegisterAndDrawButton(g, "Delete", deleteRect, _btnDelete, () => _grid.DeleteCurrent());
            RegisterAndDrawButton(g, "Save", saveRect, _btnSave, () => _grid.Save());
            RegisterAndDrawButton(g, "Cancel", cancelRect, _btnCancel, () => _grid.Cancel());

            // Center: Navigation + record counter
            string recordCounter = (_grid.Rows.Count > 0 && _grid.Selection?.RowIndex >= 0)
                ? $"{_grid.Selection.RowIndex + 1} - {_grid.Rows.Count}"
                : "0 - 0";
            var headerFont = BeepThemesManager.ToFont(_grid._currentTheme.GridCellFont) ?? SystemFonts.DefaultFont;
            SizeF textSizeF = TextUtils.MeasureText(recordCounter, headerFont);
            Size textSize = new Size((int)textSizeF.Width, (int)textSizeF.Height);

            int compactCenterTotal = buttonWidth * 4 + textSize.Width + spacing * 4;
            int centerStart = (navRect.Left + navRect.Right) / 2 - compactCenterTotal / 2;

            int leftOccupied = cancelRect.Right + spacing;
            int rightX = navRect.Right - padding;
            int rightOccupiedStart = rightX - (buttonWidth * 2 + spacing);
            int minCenter = leftOccupied + spacing;
            int maxCenter = Math.Max(minCenter, rightOccupiedStart - compactCenterTotal - spacing);
            centerStart = Math.Min(Math.Max(centerStart, minCenter), maxCenter);

            var firstRect = new Rectangle(centerStart, y, buttonWidth, buttonHeight);
            var prevRect = new Rectangle(firstRect.Right + spacing, y, buttonWidth, buttonHeight);
            var counterRect = new Rectangle(prevRect.Right + spacing, y, textSize.Width, buttonHeight);
            var nextRect = new Rectangle(counterRect.Right + spacing, y, buttonWidth, buttonHeight);
            var lastRect = new Rectangle(nextRect.Right + spacing, y, buttonWidth, buttonHeight);

            RegisterAndDrawButton(g, "First", firstRect, _btnFirst, () => _grid.MoveFirst());
            RegisterAndDrawButton(g, "Prev", prevRect, _btnPrev, () => _grid.MovePrevious());
            RegisterAndDrawButton(g, "Next", nextRect, _btnNext, () => _grid.MoveNext());
            RegisterAndDrawButton(g, "Last", lastRect, _btnLast, () => _grid.MoveLast());

            // Draw counter
            TextRenderer.DrawText(g, recordCounter, headerFont, counterRect,
                Theme?.GridHeaderForeColor ?? SystemColors.ControlText,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Left);

            // Right: Utility buttons
            if (showUtilities)
            {
                rightX = navRect.Right - padding;
                var filterRect = new Rectangle(rightX - buttonWidth, y, buttonWidth, buttonHeight); rightX = filterRect.Left - spacing;
                var printRect = new Rectangle(rightX - buttonWidth, y, buttonWidth, buttonHeight);

                RegisterAndDrawButton(g, "Filter", filterRect, _btnFilter, null);
                RegisterAndDrawButton(g, "Print", printRect, _btnPrint, null);
            }
        }

        private void DrawFullNavigator(Graphics g, Rectangle navRect, int buttonWidth, int buttonHeight, int padding, int spacing, int sectionSpacing, int y, bool showUtilities, bool showPageInfo)
        {
            // Left: CRUD buttons
            int leftX = navRect.Left + padding;
            var insertRect = new Rectangle(leftX, y, buttonWidth, buttonHeight); leftX += buttonWidth + spacing;
            var deleteRect = new Rectangle(leftX, y, buttonWidth, buttonHeight); leftX += buttonWidth + spacing;
            var saveRect = new Rectangle(leftX, y, buttonWidth, buttonHeight); leftX += buttonWidth + spacing;
            var cancelRect = new Rectangle(leftX, y, buttonWidth, buttonHeight);

            RegisterAndDrawButton(g, "Insert", insertRect, _btnInsert, () => _grid.InsertNew());
            RegisterAndDrawButton(g, "Delete", deleteRect, _btnDelete, () => _grid.DeleteCurrent());
            RegisterAndDrawButton(g, "Save", saveRect, _btnSave, () => _grid.Save());
            RegisterAndDrawButton(g, "Cancel", cancelRect, _btnCancel, () => _grid.Cancel());

            // Center: Navigation + counter
            string recordCounter = (_grid.Rows.Count > 0 && _grid.Selection?.RowIndex >= 0)
                ? $"{_grid.Selection.RowIndex + 1} - {_grid.Rows.Count}"
                : "0 - 0";
            var headerFont = BeepThemesManager.ToFont(_grid._currentTheme.GridCellFont) ?? SystemFonts.DefaultFont;
            SizeF textSizeF = TextUtils.MeasureText(recordCounter, headerFont);
            Size textSize = new Size((int)textSizeF.Width, (int)textSizeF.Height);

            int leftOccupied = cancelRect.Right + spacing;

            int rightTemp = navRect.Right - padding;
            if (showPageInfo && _lblPageInfo != null)
            {
                SizeF pageInfoSizeF = TextUtils.MeasureText(_lblPageInfo.Text, headerFont);
                Size pageInfoSize = new Size((int)pageInfoSizeF.Width, (int)pageInfoSizeF.Height);
                rightTemp -= pageInfoSize.Width + sectionSpacing;
            }
            rightTemp -= (buttonWidth * 3 + spacing * 2);

            int rightOccupiedStart = rightTemp;
            int totalCenterWidth = buttonWidth * 4 + textSize.Width + spacing * 4;

            int availableWidth = Math.Max(0, rightOccupiedStart - leftOccupied - sectionSpacing * 2);
            int centerStart;
            if (availableWidth >= totalCenterWidth)
            {
                centerStart = leftOccupied + sectionSpacing + (availableWidth - totalCenterWidth) / 2;
            }
            else
            {
                centerStart = (int)(navRect.Left + (navRect.Width - totalCenterWidth) / 2.0);
            }

            int minCenter = leftOccupied + sectionSpacing;
            int maxCenter = Math.Max(minCenter, rightOccupiedStart - totalCenterWidth - sectionSpacing);
            centerStart = Math.Min(Math.Max(centerStart, minCenter), maxCenter);

            var firstRect = new Rectangle(centerStart, y, buttonWidth, buttonHeight);
            var prevRect = new Rectangle(firstRect.Right + spacing, y, buttonWidth, buttonHeight);
            var counterRect = new Rectangle(prevRect.Right + spacing, y, textSize.Width, buttonHeight);
            var nextRect = new Rectangle(counterRect.Right + spacing, y, buttonWidth, buttonHeight);
            var lastRect = new Rectangle(nextRect.Right + spacing, y, buttonWidth, buttonHeight);

            RegisterAndDrawButton(g, "First", firstRect, _btnFirst, () => _grid.MoveFirst());
            RegisterAndDrawButton(g, "Prev", prevRect, _btnPrev, () => _grid.MovePrevious());
            RegisterAndDrawButton(g, "Next", nextRect, _btnNext, () => _grid.MoveNext());
            RegisterAndDrawButton(g, "Last", lastRect, _btnLast, () => _grid.MoveLast());

            // Draw counter
            TextRenderer.DrawText(g, recordCounter, headerFont, counterRect,
                Theme?.GridHeaderForeColor ?? SystemColors.ControlText,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Left);

            // Right: Page info and utilities
            int rightX = navRect.Right - padding;

            if (showPageInfo && _lblPageInfo != null)
            {
                SizeF pageInfoSizeF = TextUtils.MeasureText(_lblPageInfo.Text, headerFont);
                Size pageInfoSize = new Size((int)pageInfoSizeF.Width, (int)pageInfoSizeF.Height);
                var pageInfoRect = new Rectangle(rightX - pageInfoSize.Width, y, pageInfoSize.Width, buttonHeight);
                _grid.AddHitArea("PageInfo", pageInfoRect, _lblPageInfo);
                _lblPageInfo.Draw(g, pageInfoRect);
                rightX = pageInfoRect.Left - sectionSpacing;
            }

            if (showUtilities)
            {
                var queryRect = new Rectangle(rightX - buttonWidth, y, buttonWidth, buttonHeight); rightX -= buttonWidth + spacing;
                var filterRect = new Rectangle(rightX - buttonWidth, y, buttonWidth, buttonHeight); rightX -= buttonWidth + spacing;
                var printRect = new Rectangle(rightX - buttonWidth, y, buttonWidth, buttonHeight);

                RegisterAndDrawButton(g, "Query", queryRect, _btnQuery, null);
                RegisterAndDrawButton(g, "Filter", filterRect, _btnFilter, null);
                RegisterAndDrawButton(g, "Print", printRect, _btnPrint, null);
            }
        }

        private void RegisterAndDrawButton(Graphics g, string name, Rectangle rect, BeepButton? btn, Action? action)
        {
            if (btn == null) return;

            btn.Size = rect.Size;
            btn.MaxImageSize = new Size(rect.Width - 4, rect.Height - 4);

            if (action != null)
            {
                _grid.AddHitArea(name, rect, btn, action);
            }
            else
            {
                _grid.AddHitArea(name, rect, btn);
            }

            btn.Draw(g, rect);
        }

        // Public API for paging support
        public void UpdatePageInfo(int currentPage, int totalPages, int totalRecords)
        {
            if (_lblPageInfo != null)
            {
                if (totalRecords <= 0)
                    _lblPageInfo.Text = "No records";
                else
                    _lblPageInfo.Text = $"Page {currentPage} of {totalPages} — {totalRecords} records";
            }
        }

        public void EnablePagingControls(bool enable)
        {
            if (_btnFirst != null) _btnFirst.Enabled = enable;
            if (_btnPrev != null) _btnPrev.Enabled = enable;
            if (_btnNext != null) _btnNext.Enabled = enable;
            if (_btnLast != null) _btnLast.Enabled = enable;
        }
    }
}
