using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Filtering;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Painters
{
    /// <summary>
    /// Base implementation for modern filter toolbar painters.
    /// Displays a unified filter bar with active filter chips (not per-column inputs).
    /// Pattern: [Filter Icon] [Filters label] [Active Filter Chips...] [Clear All]
    /// </summary>
    public abstract class BaseFilterPanelPainter : IGridFilterPanelPainter
    {
        public const int ClearAllActionKey = -1;
        public const int SearchActionKey = -2;
        public const int AdvancedFilterActionKey = -3;

        public abstract navigationStyle Style { get; }
        public abstract string StyleName { get; }

        // Layout constants (base values before DPI scaling)
        protected const int FilterIconSize = 16;
        protected const int ChipHeight = 24;
        protected const int ChipSpacing = 6;
        protected const int SectionPadding = 12;

        /// <summary>
        /// DPI-scaled layout values for filter panel painting
        /// </summary>
        protected class ScaledLayoutValues
        {
            public int FilterIconSize { get; set; }
            public int ChipHeight { get; set; }
            public int ChipSpacing { get; set; }
            public int SectionPadding { get; set; }
            public int ClearIconSize { get; set; }
            public int SeparatorInset { get; set; }
            public int TextPadding { get; set; }
        }

        /// <summary>
        /// Create DPI-scaled layout values for painting
        /// </summary>
        protected ScaledLayoutValues GetScaledLayout(BeepGridPro grid)
        {
            float dpiScale = DpiScalingHelper.GetDpiScaleFactor(grid);
            return new ScaledLayoutValues
            {
                FilterIconSize = DpiScalingHelper.ScaleValue(BaseFilterPanelPainter.FilterIconSize, dpiScale),
                ChipHeight = DpiScalingHelper.ScaleValue(BaseFilterPanelPainter.ChipHeight, dpiScale),
                ChipSpacing = DpiScalingHelper.ScaleValue(BaseFilterPanelPainter.ChipSpacing, dpiScale),
                SectionPadding = DpiScalingHelper.ScaleValue(BaseFilterPanelPainter.SectionPadding, dpiScale),
                ClearIconSize = DpiScalingHelper.ScaleValue(12, dpiScale),
                SeparatorInset = DpiScalingHelper.ScaleValue(8, dpiScale)
            };
        }

        /// <summary>
        /// Calculate filter panel height with DPI awareness and minimum size for all components.
        /// Ensures enough space for: grid title, filter chips, icons, and proper padding.
        /// </summary>
        public virtual int CalculateFilterPanelHeight(BeepGridPro grid)
        {
            if (grid == null) return 36;

            float dpiScale = DpiScalingHelper.GetDpiScaleFactor(grid);
            
            // Get theme for font calculations
            var theme = grid.Theme != null ? BeepThemesManager.GetTheme(grid.Theme) : BeepThemesManager.GetDefaultTheme();
            
            // Calculate heights of all components that need to fit
            int titleFontHeight = 0;
            int regularFontHeight = 0;
            
            try
            {
                // Measure title font height (bold, larger)
                var titleFont = GetTitleFont(theme, grid);
                if (titleFont != null)
                {
                    titleFontHeight = FontListHelper.GetFontHeightSafe(titleFont, grid);
                }
                
                // Measure regular font height (for filter labels)
                var regularFont = GetFont(theme);
                if (regularFont != null)
                {
                    regularFontHeight = FontListHelper.GetFontHeightSafe(regularFont, grid);
                }
            }
            catch
            {
                // Fallback to system font if measurement fails
                titleFontHeight = 16;
                regularFontHeight = 14;
            }
            
            // Component heights (base values before DPI scaling)
            int chipHeight = ChipHeight;           // 24px - filter chip height
            int iconHeight = FilterIconSize;       // 16px - filter icon height
            
            // Find the tallest component (title, chip, or icon)
            int maxComponentHeight = Math.Max(Math.Max(titleFontHeight, chipHeight), Math.Max(regularFontHeight, iconHeight));
            
            // Add padding: top + bottom (SectionPadding = 12px each side)
            int topBottomPadding = SectionPadding * 2; // 24px total vertical padding
            
            // Add extra spacing for visual comfort (4px)
            int extraSpacing = 4;
            
            // Calculate total minimum height
            int baseHeight = maxComponentHeight + topBottomPadding + extraSpacing;
            
            // Ensure absolute minimum (don't go below 36px base even with small fonts)
            baseHeight = Math.Max(baseHeight, 36);
            
            // Apply DPI scaling to the calculated height
            return DpiScalingHelper.ScaleValue(baseHeight, dpiScale);
        }

        protected class ModernToolbarOptions
        {
            public int LeftPadding { get; set; } = 12;
            public int RightPadding { get; set; } = 12;
            public int SeparatorInset { get; set; } = 8;
            public int Spacing { get; set; } = 8;
            public int ControlHeight { get; set; } = 24;
            public int CornerRadius { get; set; } = 6;
            public int TitleMinWidth { get; set; } = 96;
            public int TitleMaxWidthRatioDivisor { get; set; } = 3;
            public int SearchMinWidth { get; set; } = 140;
            public int SearchMaxWidth { get; set; } = 260;
            public int ClearWidth { get; set; } = 84;
            public int CountWidth { get; set; } = 88;
            public int FilterWidth { get; set; } = 74;
            public string SearchPlaceholder { get; set; } = "Search all columns";
            public string FilterText { get; set; } = "Filter";
            public string ClearText { get; set; } = "Clear";
            public string CountFormat { get; set; } = "{0} active";
            public bool FlatControls { get; set; } = false;
        }

        protected ModernToolbarOptions GetScaledModernToolbarOptions(BeepGridPro grid)
        {
            float dpiScale = DpiScalingHelper.GetDpiScaleFactor(grid);
            return new ModernToolbarOptions
            {
                LeftPadding = DpiScalingHelper.ScaleValue(12, dpiScale),
                RightPadding = DpiScalingHelper.ScaleValue(12, dpiScale),
                SeparatorInset = DpiScalingHelper.ScaleValue(8, dpiScale),
                Spacing = DpiScalingHelper.ScaleValue(8, dpiScale),
                ControlHeight = DpiScalingHelper.ScaleValue(24, dpiScale),
                CornerRadius = DpiScalingHelper.ScaleValue(6, dpiScale),
                TitleMinWidth = DpiScalingHelper.ScaleValue(96, dpiScale),
                TitleMaxWidthRatioDivisor = 3, // Ratio, not a pixel value
                SearchMinWidth = DpiScalingHelper.ScaleValue(140, dpiScale),
                SearchMaxWidth = DpiScalingHelper.ScaleValue(260, dpiScale),
                ClearWidth = DpiScalingHelper.ScaleValue(84, dpiScale),
                CountWidth = DpiScalingHelper.ScaleValue(88, dpiScale),
                FilterWidth = DpiScalingHelper.ScaleValue(74, dpiScale),
                SearchPlaceholder = "Search all columns",
                FilterText = "Filter",
                ClearText = "Clear",
                CountFormat = "{0} active",
                FlatControls = false
            };
        }

        protected ModernToolbarOptions ScaleModernToolbarOptions(ModernToolbarOptions options, BeepGridPro grid)
        {
            float dpiScale = DpiScalingHelper.GetDpiScaleFactor(grid);
            return new ModernToolbarOptions
            {
                LeftPadding = DpiScalingHelper.ScaleValue(options.LeftPadding, dpiScale),
                RightPadding = DpiScalingHelper.ScaleValue(options.RightPadding, dpiScale),
                SeparatorInset = DpiScalingHelper.ScaleValue(options.SeparatorInset, dpiScale),
                Spacing = DpiScalingHelper.ScaleValue(options.Spacing, dpiScale),
                ControlHeight = DpiScalingHelper.ScaleValue(options.ControlHeight, dpiScale),
                CornerRadius = DpiScalingHelper.ScaleValue(options.CornerRadius, dpiScale),
                TitleMinWidth = DpiScalingHelper.ScaleValue(options.TitleMinWidth, dpiScale),
                TitleMaxWidthRatioDivisor = options.TitleMaxWidthRatioDivisor, // Ratio, not a pixel value
                SearchMinWidth = DpiScalingHelper.ScaleValue(options.SearchMinWidth, dpiScale),
                SearchMaxWidth = DpiScalingHelper.ScaleValue(options.SearchMaxWidth, dpiScale),
                ClearWidth = DpiScalingHelper.ScaleValue(options.ClearWidth, dpiScale),
                CountWidth = DpiScalingHelper.ScaleValue(options.CountWidth, dpiScale),
                FilterWidth = DpiScalingHelper.ScaleValue(options.FilterWidth, dpiScale),
                SearchPlaceholder = options.SearchPlaceholder,
                FilterText = options.FilterText,
                ClearText = options.ClearText,
                CountFormat = options.CountFormat,
                FlatControls = options.FlatControls
            };
        }

        public virtual void PaintFilterPanel(
            Graphics g,
            Rectangle panelRect,
            BeepGridPro grid,
            IBeepTheme? theme,
            Dictionary<int, Rectangle> filterCellRects,
            Dictionary<int, Rectangle> clearIconRects)
        {
            if (g == null || grid?.Layout == null || panelRect.Width <= 0 || panelRect.Height <= 0)
            {
                return;
            }

            // Clear old hit rects
            filterCellRects.Clear();
            clearIconRects.Clear();

            // Get DPI-scaled layout values
            var layout = GetScaledLayout(grid);

            var tokens = CreateStyleTokens(theme);
            var oldMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            try
            {
                // Draw panel background
                DrawPanelBackground(g, panelRect, tokens);

                // Get active filters
                var activeFilters = GetActiveFilters(grid);

                int x = panelRect.Left + layout.SectionPadding;
                int centerY = panelRect.Top + (panelRect.Height - layout.ChipHeight) / 2;
                var font = GetFont(theme);

                // Draw grid title
                string title = GetGridTitle(grid);
                if (!string.IsNullOrWhiteSpace(title))
                {
                    var titleFont = GetTitleFont(theme, grid);
                    int maxTitleWidth = Math.Max(0, panelRect.Width / 3);
                    var titleSize = TextRenderer.MeasureText(title, titleFont);
                    int titleWidth = maxTitleWidth > 0 ? Math.Min(titleSize.Width, maxTitleWidth) : titleSize.Width;
                    var titleRect = new Rectangle(x, panelRect.Top + (panelRect.Height - titleSize.Height) / 2, titleWidth, titleSize.Height);

                    TextRenderer.DrawText(g, title, titleFont, titleRect, tokens.ActiveTextColor,
                        TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

                    x = titleRect.Right + layout.SectionPadding;

                    using var titleSeparatorPen = new Pen(tokens.SeparatorColor);
                    g.DrawLine(titleSeparatorPen, x, panelRect.Top + layout.SeparatorInset, x, panelRect.Bottom - layout.SeparatorInset);
                    x += layout.SectionPadding;
                }

                // Draw filter icon
                var filterIconRect = new Rectangle(x, panelRect.Top + (panelRect.Height - layout.FilterIconSize) / 2, layout.FilterIconSize, layout.FilterIconSize);
                DrawFilterIcon(g, filterIconRect, tokens, activeFilters.Count > 0);
                x += layout.FilterIconSize + layout.SeparatorInset;

                // Draw "Filters" label
                string filtersLabel = activeFilters.Count > 0 ? $"Filters ({activeFilters.Count})" : "Filters";
                var labelSize = TextRenderer.MeasureText(filtersLabel, font);
                var labelRect = new Rectangle(x, panelRect.Top + (panelRect.Height - labelSize.Height) / 2, labelSize.Width, labelSize.Height);

                TextRenderer.DrawText(g, filtersLabel, font, labelRect, 
                    activeFilters.Count > 0 ? tokens.ActiveTextColor : tokens.InactiveTextColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
                x += labelSize.Width + layout.SectionPadding;

                // Draw separator line
                if (activeFilters.Count > 0)
                {
                    using var separatorPen = new Pen(tokens.SeparatorColor);
                    g.DrawLine(separatorPen, x, panelRect.Top + layout.SeparatorInset, x, panelRect.Bottom - layout.SeparatorInset);
                    x += layout.SectionPadding;
                }

                // Draw active filter chips
                int maxChipWidth = panelRect.Width - x - 100; // Reserve space for clear all button
                foreach (var filter in activeFilters)
                {
                    if (x >= panelRect.Right - 100) break; // Stop if no space

                    var chipRect = DrawFilterChip(g, x, centerY, filter, tokens, font, maxChipWidth, layout);

                    // Store hit rect for the chip (clicking clears this filter)
                    filterCellRects[filter.ColumnIndex] = chipRect;

                    // Store clear icon rect (X button on chip)
                    var clearRect = new Rectangle(chipRect.Right - (layout.ClearIconSize + 6), chipRect.Top + (chipRect.Height - layout.ClearIconSize) / 2, layout.ClearIconSize, layout.ClearIconSize);
                    clearIconRects[filter.ColumnIndex] = clearRect;

                    x = chipRect.Right + layout.ChipSpacing;
                }

                // Draw "Clear All" button if there are active filters
                if (activeFilters.Count > 0)
                {
                    DrawClearAllButton(g, panelRect, tokens, font, clearIconRects, layout);
                }
            }
            finally
            {
                g.SmoothingMode = oldMode;
            }
        }

        protected virtual void PaintModernToolbar(
            Graphics g,
            Rectangle panelRect,
            BeepGridPro grid,
            IBeepTheme? theme,
            Dictionary<int, Rectangle> filterCellRects,
            Dictionary<int, Rectangle> clearIconRects,
            ModernToolbarOptions options)
        {
            if (g == null || grid == null || panelRect.Width <= 0 || panelRect.Height <= 0)
            {
                return;
            }

            filterCellRects.Clear();
            clearIconRects.Clear();

            var tokens = CreateStyleTokens(theme);
            var activeFilters = GetActiveFilters(grid);
            var oldMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            try
            {
                DrawPanelBackground(g, panelRect, tokens);

                var font = GetFont(theme);
                var titleFont = GetTitleFont(theme, grid);

                int x = panelRect.Left + options.LeftPadding;
                int centerY = panelRect.Top + panelRect.Height / 2;

                string title = GetGridTitle(grid);
                int titleMaxWidth = Math.Max(options.TitleMinWidth, panelRect.Width / Math.Max(2, options.TitleMaxWidthRatioDivisor));
                var titleRect = new Rectangle(x, panelRect.Top + 4, titleMaxWidth, panelRect.Height - 8);
                TextRenderer.DrawText(g, title, titleFont, titleRect, tokens.ActiveTextColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
                x = titleRect.Right + options.Spacing;

                using (var sepPen = new Pen(tokens.SeparatorColor))
                {
                    g.DrawLine(sepPen, x, panelRect.Top + options.SeparatorInset, x, panelRect.Bottom - options.SeparatorInset);
                }
                x += options.Spacing;

                int right = panelRect.Right - options.RightPadding;

                if (activeFilters.Count > 0 || grid.IsFiltered)
                {
                    var clearRect = new Rectangle(right - options.ClearWidth, centerY - options.ControlHeight / 2, options.ClearWidth, options.ControlHeight);
                    DrawModernActionButton(g, clearRect, options.ClearText, tokens.ClearAllColor, Alpha(tokens.ClearAllColor, 24), options.CornerRadius, font, options.FlatControls);
                    clearIconRects[ClearAllActionKey] = clearRect;
                    right = clearRect.Left - options.Spacing;
                }

                string countText = string.Format(options.CountFormat, activeFilters.Count);
                var countRect = new Rectangle(right - options.CountWidth, centerY - (options.ControlHeight - 2) / 2, options.CountWidth, options.ControlHeight - 2);
                DrawModernBadge(g, countRect, countText, tokens, font, options.CornerRadius, options.FlatControls);
                right = countRect.Left - options.Spacing;

                var filterRect = new Rectangle(right - options.FilterWidth, centerY - options.ControlHeight / 2, options.FilterWidth, options.ControlHeight);
                DrawModernActionButton(g, filterRect, options.FilterText, tokens.ActiveGlyphColor, tokens.ButtonBackColor, options.CornerRadius, font, options.FlatControls);
                clearIconRects[AdvancedFilterActionKey] = filterRect;
                right = filterRect.Left - options.Spacing;

                int searchWidth = Math.Min(options.SearchMaxWidth, Math.Max(options.SearchMinWidth, right - x));
                var searchRect = new Rectangle(right - searchWidth, centerY - options.ControlHeight / 2, searchWidth, options.ControlHeight);
                DrawModernSearchBox(g, searchRect, options.SearchPlaceholder, tokens, font, options.CornerRadius, options.FlatControls);
                filterCellRects[SearchActionKey] = searchRect;
            }
            finally
            {
                g.SmoothingMode = oldMode;
            }
        }

        protected virtual void DrawModernActionButton(Graphics g, Rectangle rect, string text, Color borderColor, Color backColor, int radius, Font font, bool flat)
        {
            if (flat)
            {
                using var flatBrush = new SolidBrush(backColor);
                using var flatPen = new Pen(borderColor, 1f);
                g.FillRectangle(flatBrush, rect);
                g.DrawRectangle(flatPen, rect);
            }
            else
            {
                using var path = CreateRoundedRectangle(rect, radius);
                using var brush = new SolidBrush(backColor);
                using var pen = new Pen(borderColor, 1f);
                g.FillPath(brush, path);
                g.DrawPath(pen, path);
            }

            TextRenderer.DrawText(g, text, font, rect, borderColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        protected virtual void DrawModernBadge(Graphics g, Rectangle rect, string text, FilterPanelStyleTokens tokens, Font font, int radius, bool flat)
        {
            if (flat)
            {
                using var flatBrush = new SolidBrush(tokens.BadgeBackColor);
                using var flatPen = new Pen(tokens.PanelBorderColor, 1f);
                g.FillRectangle(flatBrush, rect);
                g.DrawRectangle(flatPen, rect);
            }
            else
            {
                using var path = CreateRoundedRectangle(rect, Math.Max(4, radius));
                using var brush = new SolidBrush(tokens.BadgeBackColor);
                using var pen = new Pen(tokens.PanelBorderColor, 1f);
                g.FillPath(brush, path);
                g.DrawPath(pen, path);
            }

            TextRenderer.DrawText(g, text, font, rect, tokens.InactiveTextColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        protected virtual void DrawModernSearchBox(Graphics g, Rectangle rect, string placeholder, FilterPanelStyleTokens tokens, Font font, int radius, bool flat)
        {
            if (flat)
            {
                using var flatBrush = new SolidBrush(tokens.SearchBackColor);
                using var flatPen = new Pen(tokens.PanelBorderColor, 1f);
                g.FillRectangle(flatBrush, rect);
                g.DrawRectangle(flatPen, rect);
            }
            else
            {
                using var path = CreateRoundedRectangle(rect, Math.Max(3, radius - 2));
                using var brush = new SolidBrush(tokens.SearchBackColor);
                using var pen = new Pen(Alpha(tokens.PanelBorderColor, 180), 1f);
                g.FillPath(brush, path);
                g.DrawPath(pen, path);
            }

            var iconRect = new Rectangle(rect.Left + 6, rect.Top + Math.Max(3, (rect.Height - 14) / 2), 14, 14);
            DrawModernSearchGlyph(g, iconRect, tokens.InactiveTextColor);

            var textRect = new Rectangle(rect.Left + 24, rect.Top, rect.Width - 28, rect.Height);
            TextRenderer.DrawText(g, placeholder, font, textRect, tokens.InactiveTextColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        protected virtual void DrawModernSearchGlyph(Graphics g, Rectangle rect, Color color)
        {
            using var pen = new Pen(color, 1.2f);
            var lens = new Rectangle(rect.Left, rect.Top, Math.Max(6, rect.Width - 4), Math.Max(6, rect.Height - 4));
            g.DrawEllipse(pen, lens);
            g.DrawLine(pen, lens.Right, lens.Bottom, rect.Right, rect.Bottom);
        }

        protected virtual void DrawPanelBackground(Graphics g, Rectangle panelRect, FilterPanelStyleTokens tokens)
        {
            using var panelBrush = new SolidBrush(tokens.PanelBackColor);
            g.FillRectangle(panelBrush, panelRect);

            // Draw bottom border only (subtle)
            using var borderPen = new Pen(tokens.PanelBorderColor);
            g.DrawLine(borderPen, panelRect.Left, panelRect.Bottom - 1, panelRect.Right, panelRect.Bottom - 1);
        }

        protected virtual void DrawFilterIcon(Graphics g, Rectangle rect, FilterPanelStyleTokens tokens, bool hasActiveFilters)
        {
            Color color = hasActiveFilters ? tokens.ActiveGlyphColor : tokens.InactiveGlyphColor;
            float penWidth = hasActiveFilters ? 1.8f : 1.4f;

            using var pen = new Pen(color, penWidth);
            using var brush = new SolidBrush(Color.FromArgb(hasActiveFilters ? 100 : 60, color));

            // Draw funnel shape
            int cx = rect.X + rect.Width / 2;
            int padding = 2;
            int halfWidth = (rect.Width - padding * 2) / 2;
            int stemWidth = Math.Max(2, rect.Width / 5);

            Point[] funnel = {
                new Point(cx - halfWidth, rect.Y + padding),
                new Point(cx + halfWidth, rect.Y + padding),
                new Point(cx + stemWidth, rect.Y + rect.Height / 2),
                new Point(cx + stemWidth, rect.Bottom - padding),
                new Point(cx - stemWidth, rect.Bottom - padding),
                new Point(cx - stemWidth, rect.Y + rect.Height / 2)
            };

            g.FillPolygon(brush, funnel);
            g.DrawPolygon(pen, funnel);
        }

        protected virtual Rectangle DrawFilterChip(Graphics g, int x, int y, ActiveFilter filter, 
            FilterPanelStyleTokens tokens, Font font, int maxWidth, ScaledLayoutValues layout)
        {
            // Format chip text: "ColumnName: Value"
            string chipText = $"{filter.ColumnCaption}: {filter.FilterValue}";
            var textSize = TextRenderer.MeasureText(chipText, font);

            int chipWidth = Math.Min(textSize.Width + 28, maxWidth); // 28 = padding + clear icon
            int chipHeight = layout.ChipHeight; // Use scaled height

            var chipRect = new Rectangle(x, y, chipWidth, chipHeight);

            // Draw chip background with rounded corners
            using var path = CreateRoundedRectangle(chipRect, tokens.ChipCornerRadius);
            using var chipBrush = new SolidBrush(tokens.ChipBackColor);
            using var chipPen = new Pen(tokens.ChipBorderColor, 1f);

            g.FillPath(chipBrush, path);
            g.DrawPath(chipPen, path);

            // Draw chip text
            int textPadding = layout.TextPadding;
            var textRect = new Rectangle(chipRect.X + textPadding, chipRect.Y, chipRect.Width - (textPadding * 3), chipRect.Height);
            TextRenderer.DrawText(g, chipText, font, textRect, tokens.ChipTextColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            // Draw clear (X) icon - size already calculated in clearRect in caller
            var clearRect = new Rectangle(chipRect.Right - (layout.ClearIconSize + 6), chipRect.Top + (chipRect.Height - layout.ClearIconSize) / 2, layout.ClearIconSize, layout.ClearIconSize);
            DrawClearIcon(g, clearRect, tokens.ChipClearIconColor);

            return chipRect;
        }

        protected virtual void DrawClearIcon(Graphics g, Rectangle rect, Color color)
        {
            using var pen = new Pen(color, 1.5f);
            int padding = 3;
            g.DrawLine(pen, rect.Left + padding, rect.Top + padding, rect.Right - padding, rect.Bottom - padding);
            g.DrawLine(pen, rect.Right - padding, rect.Top + padding, rect.Left + padding, rect.Bottom - padding);
        }

        protected virtual void DrawClearAllButton(Graphics g, Rectangle panelRect, FilterPanelStyleTokens tokens, 
            Font font, Dictionary<int, Rectangle> clearIconRects, ScaledLayoutValues layout)
        {
            string clearAllText = "Clear All";
            var textSize = TextRenderer.MeasureText(clearAllText, font);

            int buttonWidth = textSize.Width + 16;
            int buttonHeight = layout.ChipHeight; // Use scaled height
            int x = panelRect.Right - buttonWidth - layout.SectionPadding;
            int y = panelRect.Top + (panelRect.Height - buttonHeight) / 2;

            var buttonRect = new Rectangle(x, y, buttonWidth, buttonHeight);

            // Draw button with hover-style background
            using var path = CreateRoundedRectangle(buttonRect, tokens.ChipCornerRadius);
            using var brush = new SolidBrush(Color.FromArgb(40, tokens.ClearAllColor));
            using var pen = new Pen(tokens.ClearAllColor, 1f);

            g.FillPath(brush, path);
            g.DrawPath(pen, path);

            // Draw text
            TextRenderer.DrawText(g, clearAllText, font, buttonRect, tokens.ClearAllColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

            // Store the clear all button rect with a special index (-1)
            clearIconRects[-1] = buttonRect;
        }

        protected List<ActiveFilter> GetActiveFilters(BeepGridPro grid)
        {
            var filters = new List<ActiveFilter>();

            if (grid?.Data?.Columns == null) return filters;

            // Advanced filter criteria (from GridX/Filtering)
            if (grid.ActiveFilter?.Criteria != null)
            {
                int advancedFallbackIndex = -100;
                foreach (var criterion in grid.ActiveFilter.Criteria.Where(c => c != null && c.IsEnabled))
                {
                    var matchingIndex = grid.Data.Columns.FindIndex(c =>
                        string.Equals(c.ColumnName, criterion.ColumnName, StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(c.ColumnCaption, criterion.ColumnName, StringComparison.OrdinalIgnoreCase));

                    var matchingColumn = matchingIndex >= 0 && matchingIndex < grid.Data.Columns.Count
                        ? grid.Data.Columns[matchingIndex]
                        : null;

                    var columnCaption = matchingColumn?.ColumnCaption
                        ?? matchingColumn?.ColumnName
                        ?? criterion.ColumnName
                        ?? "Filter";

                    var valueText = BuildCriterionValueText(criterion);
                    filters.Add(new ActiveFilter
                    {
                        ColumnIndex = matchingIndex >= 0 ? matchingIndex : advancedFallbackIndex--,
                        ColumnName = matchingColumn?.ColumnName ?? criterion.ColumnName ?? string.Empty,
                        ColumnCaption = columnCaption,
                        FilterValue = valueText
                    });
                }
            }

            for (int i = 0; i < grid.Data.Columns.Count; i++)
            {
                var column = grid.Data.Columns[i];
                if (column.IsFiltered && !string.IsNullOrWhiteSpace(column.Filter))
                {
                    bool exists = filters.Any(f =>
                        (f.ColumnIndex == i) ||
                        string.Equals(f.ColumnName, column.ColumnName, StringComparison.OrdinalIgnoreCase));
                    if (exists)
                    {
                        continue;
                    }

                    filters.Add(new ActiveFilter
                    {
                        ColumnIndex = i,
                        ColumnName = column.ColumnName ?? $"Column {i}",
                        ColumnCaption = column.ColumnCaption ?? column.ColumnName ?? $"Column {i}",
                        FilterValue = column.Filter.Trim()
                    });
                }
            }

            return filters;
        }

        protected virtual string BuildCriterionValueText(FilterCriteria criterion)
        {
            if (criterion == null)
            {
                return string.Empty;
            }

            string op = criterion.Operator.GetSymbol();
            string value1 = criterion.Value?.ToString() ?? "∅";

            if (criterion.Operator == FilterOperator.Between || criterion.Operator == FilterOperator.NotBetween)
            {
                string value2 = criterion.Value2?.ToString() ?? "∅";
                return $"{op} {value1} … {value2}";
            }

            if (criterion.Operator == FilterOperator.IsNull || criterion.Operator == FilterOperator.IsNotNull)
            {
                return op;
            }

            return $"{op} {value1}";
        }

        protected virtual string GetGridTitle(BeepGridPro grid)
        {
            if (grid == null) return "Grid";
            if (!string.IsNullOrWhiteSpace(grid.GridTitle)) return grid.GridTitle.Trim();
            if (!string.IsNullOrWhiteSpace(grid.Name)) return grid.Name.Trim();
            return "Grid";
        }

        protected Font GetFont(IBeepTheme? theme)
        {
            if (theme?.GridHeaderFont != null)
            {
                var themedFont = BeepFontManager.ToFont(theme.GridHeaderFont);
                if (themedFont != null)
                {
                    return themedFont;
                }
            }

            return SystemFonts.DefaultFont;
        }

        protected Font GetTitleFont(IBeepTheme? theme, BeepGridPro grid)
        {
            if (theme?.GridHeaderFont != null && grid != null)
            {
                var style = theme.GridHeaderFont;
                var fontStyle = style.FontWeight >= FontWeight.Bold ? FontStyle.Bold : FontStyle.Bold;

                var scaledFont = BeepFontManager.GetFontForControl(
                    style.FontFamily,
                    style.FontSize,
                    grid,
                    fontStyle);

                if (scaledFont != null)
                {
                    return scaledFont;
                }
            }

            var baseFont = GetFont(theme);
            return BeepFontManager.GetFontForControl(baseFont.FontFamily.Name, baseFont.Size, grid, FontStyle.Bold)
                ?? baseFont;
        }

        protected virtual FilterPanelStyleTokens CreateStyleTokens(IBeepTheme? theme)
        {
            Color headerBack = theme?.GridHeaderBackColor ?? SystemColors.Control;
            Color gridBack = theme?.GridBackColor ?? SystemColors.Window;
            Color headerFore = theme?.GridHeaderForeColor ?? SystemColors.ControlText;
            Color gridLine = theme?.GridLineColor ?? SystemColors.ControlDark;
            Color accent = (theme?.FocusIndicatorColor ?? Color.Empty) != Color.Empty
                ? theme!.FocusIndicatorColor
                : Color.FromArgb(59, 130, 246); // Default blue

            var tokens = new FilterPanelStyleTokens
            {
                PanelBackColor = gridBack,
                PanelBorderColor = gridLine,
                SeparatorColor = Color.FromArgb(100, gridLine),

                InactiveTextColor = Color.FromArgb(150, headerFore),
                ActiveTextColor = headerFore,
                InactiveGlyphColor = Color.FromArgb(120, headerFore),
                ActiveGlyphColor = accent,

                ChipBackColor = Color.FromArgb(25, accent),
                ChipBorderColor = Color.FromArgb(80, accent),
                ChipTextColor = headerFore,
                ChipClearIconColor = Color.FromArgb(150, headerFore),
                ChipCornerRadius = 4,

                ClearAllColor = Color.FromArgb(220, 53, 69), // Red for clear all
                SearchBackColor = Color.FromArgb(245, gridBack),
                BadgeBackColor = Color.FromArgb(230, gridBack),
                ButtonBackColor = Color.FromArgb(25, accent)
            };

            // Apply style-specific customizations
            ApplyStyleTokens(tokens, theme);

            return tokens;
        }

        protected virtual void ApplyStyleTokens(FilterPanelStyleTokens tokens, IBeepTheme? theme)
        {
            // Override in derived classes for style-specific customizations
            switch (Style)
            {
                case navigationStyle.Material:
                    tokens.ChipCornerRadius = 16; // Pill shape
                    break;
                case navigationStyle.Bootstrap:
                    tokens.ChipCornerRadius = 4;
                    tokens.ClearAllColor = Color.FromArgb(108, 117, 125); // Bootstrap secondary
                    break;
                case navigationStyle.Fluent:
                    tokens.ChipCornerRadius = 2;
                    break;
                case navigationStyle.Tailwind:
                    tokens.ChipCornerRadius = 6;
                    break;
                case navigationStyle.Card:
                    tokens.ChipCornerRadius = 8;
                    break;
            }
        }

        protected static GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            if (rect.Width <= 1 || rect.Height <= 1 || radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            radius = Math.Min(radius, Math.Min(rect.Width, rect.Height) / 2);
            int d = radius * 2;

            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();

            return path;
        }

        protected static Color Blend(Color baseColor, Color blendColor, float amount)
        {
            amount = Math.Max(0f, Math.Min(1f, amount));
            int r = (int)(baseColor.R + ((blendColor.R - baseColor.R) * amount));
            int g = (int)(baseColor.G + ((blendColor.G - baseColor.G) * amount));
            int b = (int)(baseColor.B + ((blendColor.B - baseColor.B) * amount));
            return Color.FromArgb(255, r, g, b);
        }

        protected static Color Alpha(Color color, int alpha)
        {
            return Color.FromArgb(Math.Max(0, Math.Min(255, alpha)), color);
        }

        protected static Color ResolveAccent(IBeepTheme? theme, Color fallback)
        {
            if ((theme?.FocusIndicatorColor ?? Color.Empty) != Color.Empty)
            {
                return theme!.FocusIndicatorColor;
            }

            return fallback;
        }

        protected class ActiveFilter
        {
            public int ColumnIndex { get; set; }
            public string ColumnName { get; set; } = string.Empty;
            public string ColumnCaption { get; set; } = string.Empty;
            public string FilterValue { get; set; } = string.Empty;
        }

        protected class FilterPanelStyleTokens
        {
            public Color PanelBackColor { get; set; }
            public Color PanelBorderColor { get; set; }
            public Color SeparatorColor { get; set; }

            public Color InactiveTextColor { get; set; }
            public Color ActiveTextColor { get; set; }
            public Color InactiveGlyphColor { get; set; }
            public Color ActiveGlyphColor { get; set; }

            public Color ChipBackColor { get; set; }
            public Color ChipBorderColor { get; set; }
            public Color ChipTextColor { get; set; }
            public Color ChipClearIconColor { get; set; }
            public int ChipCornerRadius { get; set; }

            public Color ClearAllColor { get; set; }

        
            public Color InactiveChipBackColor { get; set; }
            public Color ActiveChipBackColor { get; set; }
            public Color InactiveChipBorderColor { get; set; }
            public Color ActiveChipBorderColor { get; set; }
          
            public Color ClearGlyphColor { get; set; }
            public int OuterPadding { get; set; }
            public int CornerRadius { get; set; }
            public float BorderWidthInactive { get; set; }
            public float BorderWidthActive { get; set; }

            public Color SearchBackColor { get; set; }
            public Color BadgeBackColor { get; set; }
            public Color ButtonBackColor { get; set; }
        }
    }
}
