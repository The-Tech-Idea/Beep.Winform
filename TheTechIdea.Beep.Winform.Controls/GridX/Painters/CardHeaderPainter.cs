using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Painters
{
    /// <summary>
    /// Card-style header painter with rounded corners, shadows, and spaced design
    /// </summary>
    public class CardHeaderPainter : BaseHeaderPainter
    {
        /// <summary>
        /// Gets the navigation style
        /// </summary>
        public override navigationStyle Style => navigationStyle.Card;
        
        /// <summary>
        /// Gets the style name
        /// </summary>
        public override string StyleName => "Card";

        /// <summary>
        /// Calculate padding for Card headers
        /// </summary>
        public override int CalculateHeaderPadding() => 14;

        /// <summary>
        /// Calculate header height with extra space for card spacing
        /// </summary>
        public override int CalculateHeaderHeight(BeepGridPro grid)
        {
            // Use base font-aware calculation and add extra space for card shadows/spacing
            int baseHeight = base.CalculateHeaderHeight(grid);
            return baseHeight + 8; // Add 8px for card spacing and shadows
        }

        /// <summary>
        /// Paint all column headers
        /// </summary>
        public override void PaintHeaders(Graphics g, Rectangle headerRect, BeepGridPro grid, IBeepTheme? theme)
        {
            if (g == null || grid?.Layout == null || headerRect.IsEmpty) return;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Card style uses light background - no global header fill needed
            // Individual cells will have card appearance

            // Calculate sticky regions for proper layering
            var stickyColumns = grid.Data.Columns.Where(c => c.Sticked && c.Visible).ToList();
            int stickyWidth = stickyColumns.Sum(c => Math.Max(20, c.Width));
            stickyWidth = Math.Min(stickyWidth, headerRect.Width);

            Rectangle stickyRegion = new Rectangle(headerRect.Left, headerRect.Top, stickyWidth, headerRect.Height);
            Rectangle scrollingRegion = new Rectangle(headerRect.Left + stickyWidth, headerRect.Top,
                Math.Max(0, headerRect.Width - stickyWidth), headerRect.Height);

            // Draw scrolling (non-sticky) headers first
            var state1 = g.Save();
            g.SetClip(scrollingRegion);
            for (int i = 0; i < grid.Data.Columns.Count; i++)
            {
                var col = grid.Data.Columns[i];
                if (!col.Visible || col.Sticked) continue;
                if (i < grid.Layout.HeaderCellRects.Length)
                {
                    var cellRect = grid.Layout.HeaderCellRects[i];
                    if (!cellRect.IsEmpty && cellRect.Width > 0 && cellRect.Height > 0)
                        PaintHeaderCell(g, cellRect, col, i, grid, theme);
                }
            }
            g.Restore(state1);

            // Draw sticky headers on top
            var state2 = g.Save();
            g.SetClip(stickyRegion);
            for (int i = 0; i < grid.Data.Columns.Count; i++)
            {
                var col = grid.Data.Columns[i];
                if (!col.Visible || !col.Sticked) continue;
                if (i < grid.Layout.HeaderCellRects.Length)
                {
                    var cellRect = grid.Layout.HeaderCellRects[i];
                    if (!cellRect.IsEmpty && cellRect.Width > 0 && cellRect.Height > 0)
                        PaintHeaderCell(g, cellRect, col, i, grid, theme);
                }
            }
            g.Restore(state2);

            g.SmoothingMode = SmoothingMode.Default;
        }

        /// <summary>
        /// Paint a single header cell as a card
        /// </summary>
        public override void PaintHeaderCell(Graphics g, Rectangle cellRect, BeepColumnConfig column,
            int columnIndex, BeepGridPro grid, IBeepTheme? theme)
        {
            if (g == null || column == null || cellRect.IsEmpty) return;

            bool isHovered = grid.Layout.HoveredHeaderColumnIndex == columnIndex;
            bool isSorted = column.IsSorted;

            // Create card bounds with spacing
            int cardSpacing = 4;
            var cardBounds = new Rectangle(
                cellRect.X + cardSpacing / 2,
                cellRect.Y + cardSpacing,
                cellRect.Width - cardSpacing,
                cellRect.Height - cardSpacing * 2
            );

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Create rounded rectangle for card using base class helper
            int cornerRadius = 8;
            using (var cardPath = CreateRoundedRectangle(cardBounds, cornerRadius))
            {
                // Draw card shadow
                var shadowBounds = cardBounds;
                shadowBounds.Offset(0, 2);
                using (var shadowPath = CreateRoundedRectangle(shadowBounds, cornerRadius))
                using (var shadowBrush = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
                {
                    g.FillPath(shadowBrush, shadowPath);
                }

                // Card background color
                Color cardBgColor = Color.White;
                if (isHovered)
                {
                    cardBgColor = Color.FromArgb(248, 249, 250); // Light gray on hover
                }
                else if (isSorted)
                {
                    cardBgColor = Color.FromArgb(240, 248, 255); // Light blue for sorted
                }

                using (var cardBrush = new SolidBrush(cardBgColor))
                {
                    g.FillPath(cardBrush, cardPath);
                }

                // Card border
                Color borderColor = isHovered 
                    ? Color.FromArgb(206, 212, 218) 
                    : Color.FromArgb(222, 226, 230);
                
                if (isSorted)
                {
                    borderColor = Color.FromArgb(100, 149, 237); // Cornflower blue
                }

                using (var borderPen = new Pen(borderColor, 1.5f))
                {
                    g.DrawPath(borderPen, cardPath);
                }
            }

            g.SmoothingMode = SmoothingMode.Default;

            // Calculate layout areas
            int padding = CalculateHeaderPadding();
            int sortIconSize = column.IsSorted && column.ShowSortIcon ? 16 : 0;
            int filterIconSize = column.ShowFilterIcon && isHovered ? 18 : 0;

            int rightX = cardBounds.Right - padding;

            // Filter icon area (rightmost)
            Rectangle filterIconRect = Rectangle.Empty;
            if (filterIconSize > 0)
            {
                filterIconRect = new Rectangle(
                    rightX - filterIconSize,
                    cardBounds.Y + (cardBounds.Height - filterIconSize) / 2,
                    filterIconSize,
                    filterIconSize);
                rightX -= filterIconSize + padding;
            }

            // Sort icon area
            Rectangle sortIconRect = Rectangle.Empty;
            if (sortIconSize > 0)
            {
                sortIconRect = new Rectangle(
                    rightX - sortIconSize,
                    cardBounds.Y + (cardBounds.Height - sortIconSize) / 2,
                    sortIconSize,
                    sortIconSize);
                rightX -= sortIconSize + padding;
            }

            // Text area
            var textRect = new Rectangle(
                cardBounds.X + padding,
                cardBounds.Y + padding,
                Math.Max(1, rightX - cardBounds.X - padding),
                Math.Max(1, cardBounds.Height - padding * 2)
            );

            // Draw header text
            Color textColor = Color.FromArgb(52, 58, 64); // Dark gray
            var font = new Font("Segoe UI", 9.5f, FontStyle.Regular);
            string text = column.ColumnCaption ?? column.ColumnName ?? string.Empty;
            PaintHeaderText(g, textRect, text, font, column.HeaderTextAlignment, theme);

            // Draw sort indicator if needed
            if (sortIconSize > 0)
            {
                PaintCardSortIndicator(g, sortIconRect, column.SortDirection);
            }

            // Draw filter icon if needed
            if (filterIconSize > 0)
            {
                PaintCardFilterIcon(g, filterIconRect);
            }
        }

        private void PaintCardSortIndicator(Graphics g, Rectangle bounds, SortDirection direction)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw circular badge background
            Color badgeBgColor = Color.FromArgb(100, 149, 237); // Cornflower blue
            using (var badgeBrush = new SolidBrush(badgeBgColor))
            {
                g.FillEllipse(badgeBrush, bounds);
            }

            // Draw arrow in white
            Color arrowColor = Color.White;
            using (var arrowBrush = new SolidBrush(arrowColor))
            {
                var centerX = bounds.X + bounds.Width / 2f;
                var centerY = bounds.Y + bounds.Height / 2f;

                if (direction == SortDirection.Ascending)
                {
                    // Upward arrow
                    PointF[] points = new PointF[]
                    {
                        new PointF(centerX, centerY - 3),           // Top
                        new PointF(centerX - 3, centerY + 2),       // Bottom left
                        new PointF(centerX + 3, centerY + 2)        // Bottom right
                    };
                    g.FillPolygon(arrowBrush, points);
                }
                else
                {
                    // Downward arrow
                    PointF[] points = new PointF[]
                    {
                        new PointF(centerX - 3, centerY - 2),       // Top left
                        new PointF(centerX + 3, centerY - 2),       // Top right
                        new PointF(centerX, centerY + 3)            // Bottom
                    };
                    g.FillPolygon(arrowBrush, points);
                }
            }

            g.SmoothingMode = SmoothingMode.Default;
        }

        private void PaintCardFilterIcon(Graphics g, Rectangle bounds)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw circular outline
            Color circleColor = Color.FromArgb(173, 181, 189); // Gray
            using (var circlePen = new Pen(circleColor, 1.5f))
            {
                g.DrawEllipse(circlePen, bounds);
            }

            // Draw funnel inside
            Color filterColor = Color.FromArgb(108, 117, 125); // Darker gray
            using (var filterPen = new Pen(filterColor, 1.5f))
            {
                var centerX = bounds.X + bounds.Width / 2f;
                var inset = 4;

                PointF[] funnelPoints = new PointF[]
                {
                    new PointF(bounds.X + inset, bounds.Y + inset),          // Top left
                    new PointF(bounds.Right - inset, bounds.Y + inset),      // Top right
                    new PointF(centerX + 2, bounds.Y + bounds.Height / 2),   // Middle right
                    new PointF(centerX + 2, bounds.Bottom - inset),          // Bottom right
                    new PointF(centerX - 2, bounds.Bottom - inset),          // Bottom left
                    new PointF(centerX - 2, bounds.Y + bounds.Height / 2),   // Middle left
                    new PointF(bounds.X + inset, bounds.Y + inset)           // Back to start
                };

                g.DrawLines(filterPen, funnelPoints);
            }

            g.SmoothingMode = SmoothingMode.Default;
        }
    }
}
