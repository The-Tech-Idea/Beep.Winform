using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes;
using TheTechIdea.Beep.Winform.Controls.GridX.Painters;
 
 

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    /// <summary>
    /// Helper class responsible for rendering column headers in BeepGridPro.
    /// Handles header backgrounds, text, sort indicators, filter icons, and sticky columns.
    /// </summary>
    public sealed class GridColumnHeadersPainterHelper
    {
        private readonly BeepGridPro _grid;
        private readonly Dictionary<int, Rectangle> _headerSortIconRects = new();
        private readonly Dictionary<int, Rectangle> _headerFilterIconRects = new();
        private BeepCheckBoxBool? _selectAllCheckbox;
        private IPaintGridHeader? _currentHeaderPainter;

        // Configuration properties
        public bool ShowGridLines { get; set; } = true;
        public DashStyle GridLineStyle { get; set; } = DashStyle.Solid;
        public bool UseHeaderGradient { get; set; } = false;
        public bool UseHeaderHoverEffects { get; set; } = true;
        public bool UseElevation { get; set; } = false;
        public bool UseBoldHeaderText { get; set; } = true;
        public bool ShowSortIndicators { get; set; } = true;
        public int HeaderCellPadding { get; set; } = 6;

        private IBeepTheme? Theme => _grid?._currentTheme;

        public GridColumnHeadersPainterHelper(BeepGridPro grid)
        {
            _grid = grid ?? throw new ArgumentNullException(nameof(grid));
        }

        public Dictionary<int, Rectangle> HeaderSortIconRects => _headerSortIconRects;
        public Dictionary<int, Rectangle> HeaderFilterIconRects => _headerFilterIconRects;

        /// <summary>
        /// Calculate the recommended header height based on the current navigation style and font
        /// </summary>
        public int CalculateHeaderHeight()
        {
            if (_grid == null) return 28; // Default fallback

            // Get or create header painter matching navigation style
            if (_currentHeaderPainter == null || _currentHeaderPainter.Style != _grid.NavigationStyle)
            {
                _currentHeaderPainter = HeaderPainterFactory.CreateHeaderPainter(_grid.NavigationStyle);
            }

            // Use painter's calculation which accounts for font size
            return _currentHeaderPainter?.CalculateHeaderHeight(_grid) ?? 28;
        }

        /// <summary>
        /// Main entry point to draw all column headers with sticky column support.
        /// Uses header painters for consistent styling with navigation bar.
        /// </summary>
        public void DrawColumnHeaders(Graphics g)
        {
            if (g == null || _grid?.Layout == null) return;

            var headerRect = _grid.Layout.ColumnsHeaderRect;
            if (headerRect.Height <= 0 || headerRect.Width <= 0) return;

            // Get or create header painter matching navigation style
            if (_currentHeaderPainter == null || _currentHeaderPainter.Style != _grid.NavigationStyle)
            {
                _currentHeaderPainter = HeaderPainterFactory.CreateHeaderPainter(_grid.NavigationStyle);
            }

            // Draw select-all checkbox if enabled (before painter draws headers)
            DrawSelectAllCheckbox(g, headerRect);

            // Use painter to draw all headers
            _currentHeaderPainter?.PaintHeaders(g, headerRect, _grid, Theme);

            // Register hit areas for interactions
            _currentHeaderPainter?.RegisterHeaderHitAreas(_grid);
        }

        private void DrawSelectAllCheckbox(Graphics g, Rectangle headerRect)
        {
            var selColumn = _grid.Data.Columns.FirstOrDefault(c => c.IsSelectionCheckBox);
            if (_grid.ShowCheckBox && selColumn != null && _grid.Layout.SelectAllCheckRect != Rectangle.Empty)
            {
                bool allSelected = _grid.Rows.Count > 0 && _grid.Rows.All(r => r.IsSelected);
                _selectAllCheckbox ??= new BeepCheckBoxBool { IsChild = true, GridMode = true, HideText = true, Theme = _grid.Theme };
                _selectAllCheckbox.CurrentValue = allSelected;
                _selectAllCheckbox.Draw(g, _grid.Layout.SelectAllCheckRect);
            }
        }

        private void DrawHeaderCell(Graphics g, BeepColumnConfig column, Rectangle cellRect, int columnIndex)
        {
            if (g == null || column == null || cellRect.Width <= 0 || cellRect.Height <= 0)
                return;

            bool isHovered = UseHeaderHoverEffects && _grid.Layout.HoveredHeaderColumnIndex == columnIndex;

            // Background with gradient or solid fill
            DrawHeaderBackground(g, cellRect, isHovered);

            // Elevation effect
            if (UseElevation)
            {
                using (var shadowPen = new Pen(Color.FromArgb(40, 0, 0, 0), 1))
                {
                    g.DrawLine(shadowPen, cellRect.Left + 1, cellRect.Bottom, cellRect.Right - 1, cellRect.Bottom);
                    g.DrawLine(shadowPen, cellRect.Right, cellRect.Top + 1, cellRect.Right, cellRect.Bottom - 1);
                }
            }

            // Calculate layout areas
            bool hasHeaderImage = !string.IsNullOrEmpty(column.ImagePath);
            Rectangle imageRect = Rectangle.Empty;
            
            if (hasHeaderImage)
            {
                int maxImageHeight = Math.Min(column.MaxImageHeight, cellRect.Height - HeaderCellPadding * 3 - 20);
                int maxImageWidth = Math.Min(column.MaxImageWidth, cellRect.Width - HeaderCellPadding * 2);
                int imageSize = Math.Min(maxImageHeight, maxImageWidth);
                
                imageRect = new Rectangle(
                    cellRect.X + (cellRect.Width - imageSize) / 2,
                    cellRect.Y + HeaderCellPadding,
                    imageSize,
                    imageSize
                );
            }

            int sortIconSize = ShowSortIndicators ? Math.Min(cellRect.Height - HeaderCellPadding * 2, 16) : 0;
            int filterIconSize = Math.Min(cellRect.Height - HeaderCellPadding * 2, 18);

            int textTop = hasHeaderImage ? imageRect.Bottom + HeaderCellPadding : cellRect.Y + HeaderCellPadding;
            int textHeight = cellRect.Bottom - HeaderCellPadding - textTop;
            
            int rightX = cellRect.Right - HeaderCellPadding;

            // Filter icon area
            Rectangle filterIconRect = new Rectangle(
                rightX - filterIconSize,
                textTop,
                filterIconSize,
                Math.Min(filterIconSize, textHeight));
            rightX -= filterIconSize + HeaderCellPadding;

            // Sort icon area
            Rectangle sortIconRect = Rectangle.Empty;
            if (ShowSortIndicators && column.IsSorted && column.ShowSortIcon)
            {
                sortIconRect = new Rectangle(
                    rightX - sortIconSize,
                    textTop,
                    sortIconSize,
                    Math.Min(sortIconSize, textHeight));
                rightX -= sortIconSize + HeaderCellPadding;
            }

            // Text area
            var textRect = new Rectangle(
                cellRect.X + HeaderCellPadding,
                textTop,
                Math.Max(1, rightX - cellRect.X - HeaderCellPadding),
                Math.Max(1, textHeight)
            );

            // Draw image if present
            if (hasHeaderImage && imageRect.Width > 0 && imageRect.Height > 0)
            {
                try
                {
                    Styling.ImagePainters.StyledImagePainter.Paint(g, imageRect, column.ImagePath, BeepControlStyle.Material3);
                }
                catch { /* Silently ignore image loading errors */ }
            }

            // Draw text
            DrawHeaderText(g, textRect, column);

            // Draw sort indicator
            if (ShowSortIndicators && column.IsSorted && column.ShowSortIcon)
            {
                DrawSortIndicator(g, sortIconRect, column.SortDirection);
                _headerSortIconRects[columnIndex] = sortIconRect;
            }
            else
            {
                _headerSortIconRects.Remove(columnIndex);
            }

            // Draw filter icon on hover
            bool showFilterIcon = column.ShowFilterIcon && _grid.Layout.HoveredHeaderColumnIndex == columnIndex;
            if (showFilterIcon)
            {
                DrawFilterIcon(g, filterIconRect, column.IsFiltered);
                _headerFilterIconRects[columnIndex] = filterIconRect;
            }
            else
            {
                _headerFilterIconRects.Remove(columnIndex);
            }

            // Border
            if (ShowGridLines)
            {
                using (var pen = new Pen(Theme?.GridLineColor ?? SystemColors.ControlDark))
                {
                    pen.DashStyle = GridLineStyle;
                    g.DrawRectangle(pen, cellRect);
                }
            }
        }

        private void DrawHeaderBackground(Graphics g, Rectangle cellRect, bool isHovered)
        {
            if (UseHeaderGradient)
            {
                var baseColor = Theme?.GridHeaderBackColor ?? SystemColors.Control;
                var lightColor = Color.FromArgb(
                    Math.Min(255, baseColor.R + 20),
                    Math.Min(255, baseColor.G + 20),
                    Math.Min(255, baseColor.B + 20)
                );

                using (var brush = new LinearGradientBrush(
                    cellRect,
                    isHovered ? lightColor : baseColor,
                    isHovered ? baseColor : lightColor,
                    LinearGradientMode.Vertical))
                {
                    g.FillRectangle(brush, cellRect);
                }
            }
            else
            {
                var backColor = Theme?.GridHeaderBackColor ?? SystemColors.Control;
                if (isHovered && UseHeaderHoverEffects)
                {
                    backColor = Color.FromArgb(
                        Math.Min(255, backColor.R + 15),
                        Math.Min(255, backColor.G + 15),
                        Math.Min(255, backColor.B + 15)
                    );
                }

                using (var brush = new SolidBrush(backColor))
                {
                    g.FillRectangle(brush, cellRect);
                }
            }
        }

        private void DrawHeaderText(Graphics g, Rectangle textRect, BeepColumnConfig column)
        {
            var textColor = Theme?.GridHeaderForeColor ?? SystemColors.ControlText;
            string text = column.ColumnCaption ?? column.ColumnName ?? string.Empty;
            
            if (!string.IsNullOrEmpty(text))
            {
                var baseFont = BeepThemesManager.ToFont(_grid._currentTheme.GridHeaderFont) ?? SystemFonts.DefaultFont;
                var font = UseBoldHeaderText ?
                    new Font(baseFont.FontFamily, baseFont.Size, FontStyle.Bold) :
                    baseFont;

                var headerAlign = column.HeaderTextAlignment;
                var flags = GetTextFormatFlagsForAlignment(headerAlign, true);
                TextRenderer.DrawText(g, text, font, textRect, textColor, flags);

                if (UseBoldHeaderText)
                {
                    font.Dispose();
                }
            }
        }

        private void DrawFilterIcon(Graphics g, Rectangle rect, bool active)
        {
            Color iconColor = active ? Color.DodgerBlue : (Theme?.GridHeaderForeColor ?? SystemColors.ControlText);
            using (var pen = new Pen(iconColor, 2))
            {
                Point[] funnel = new[]
                {
                    new Point(rect.Left + rect.Width / 8, rect.Top + rect.Height / 4),
                    new Point(rect.Right - rect.Width / 8, rect.Top + rect.Height / 4),
                    new Point(rect.Left + rect.Width / 2, rect.Bottom - rect.Height / 8)
                };
                g.DrawLines(pen, funnel);
                g.DrawLine(pen,
                    rect.Left + rect.Width / 2,
                    rect.Bottom - rect.Height / 8,
                    rect.Left + rect.Width / 2,
                    rect.Bottom - rect.Height / 4);
            }
        }

        private void DrawSortIndicator(Graphics g, Rectangle rect, SortDirection sortDirection)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;

            var color = Theme?.GridHeaderForeColor ?? SystemColors.ControlText;

            using (var pen = new Pen(color, 2f))
            {
                int centerX = rect.Left + rect.Width / 2;

                if (sortDirection == SortDirection.Ascending)
                {
                    var points = new Point[]
                    {
                        new Point(centerX, rect.Top + 2),
                        new Point(centerX - 4, rect.Bottom - 2),
                        new Point(centerX + 4, rect.Bottom - 2)
                    };
                    g.DrawPolygon(pen, points);
                }
                else if (sortDirection == SortDirection.Descending)
                {
                    var points = new Point[]
                    {
                        new Point(centerX, rect.Bottom - 2),
                        new Point(centerX - 4, rect.Top + 2),
                        new Point(centerX + 4, rect.Top + 2)
                    };
                    g.DrawPolygon(pen, points);
                }
            }
        }

        private static TextFormatFlags GetTextFormatFlagsForAlignment(ContentAlignment alignment, bool endEllipsis)
        {
            TextFormatFlags flags = TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix | TextFormatFlags.PreserveGraphicsClipping;
            if (endEllipsis) flags |= TextFormatFlags.EndEllipsis;

            switch (alignment)
            {
                case ContentAlignment.TopLeft:
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.BottomLeft:
                    flags |= TextFormatFlags.Left;
                    break;
                case ContentAlignment.TopCenter:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.BottomCenter:
                    flags |= TextFormatFlags.HorizontalCenter;
                    break;
                case ContentAlignment.TopRight:
                case ContentAlignment.MiddleRight:
                case ContentAlignment.BottomRight:
                    flags |= TextFormatFlags.Right;
                    break;
            }
            return flags;
        }
    }
}
