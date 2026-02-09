using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Painters
{
    /// <summary>
    /// Ant Design inspired navigation - clean, professional, minimal
    /// </summary>
    public class AntDesignNavigationPainter : BaseNavigationPainter
    {
        public override navigationStyle Style => navigationStyle.AntDesign;
        public override int RecommendedHeight => 48;
        public override int RecommendedMinWidth => 550;

        private const int BUTTON_SIZE = 32;
        private const int SPACING = 8;

        public override void PaintNavigation(Graphics g, Rectangle bounds, BeepGridPro grid, IBeepTheme theme)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            grid.ClearHitList();

            // Clean background using theme
            using (var brush = new SolidBrush(theme.GridHeaderBackColor))
            {
                g.FillRectangle(brush, bounds);
            }

            // Top border using theme border color
            using (var pen = new Pen(theme.GridHeaderBorderColor, 1))
            {
                g.DrawLine(pen, bounds.X, bounds.Y, bounds.Right, bounds.Y);
            }

            var layout = CalculateLayout(bounds, grid.Data.Rows.Count, true);

            // Left section - CRUD
            PaintAntButton(g, grid, layout.AddNewButtonRect, "+ New", () => grid.InsertNew(), theme, false);
            PaintAntButton(g, grid, layout.DeleteButtonRect, "Delete", () => grid.DeleteCurrent(), theme, false);
            PaintAntButton(g, grid, layout.SaveButtonRect, "Save", () => grid.Save(), theme, true);

            // Right section - Navigation
            PaintAntButton(g, grid, layout.FirstButtonRect, "⟪", () => grid.Render.GoToFirstPage(grid), theme, false);
            PaintAntButton(g, grid, layout.PreviousButtonRect, "‹", () => grid.Render.GoToPreviousPage(grid), theme, false);
            
            PaintAntPageNumbers(g, grid, layout, theme);
            
            PaintAntButton(g, grid, layout.NextButtonRect, "›", () => grid.Render.GoToNextPage(grid), theme, false);
            PaintAntButton(g, grid, layout.LastButtonRect, "⟫", () => grid.Render.GoToLastPage(grid), theme, false);

            // Position info
            PaintAntInfo(g, layout.PositionIndicatorRect, grid.Selection.RowIndex + 1, grid.Data.Rows.Count, theme);
        }

        private void PaintAntButton(Graphics g, BeepGridPro grid, Rectangle bounds, 
            string text, Action action, IBeepTheme theme, bool isPrimary)
        {
            if (bounds.IsEmpty) return;

            grid.AddHitArea(text, bounds, null, action);

            // Button background using theme
            Color bgColor = isPrimary ? theme.AccentColor : theme.GridHeaderBackColor;
            using (var brush = new SolidBrush(bgColor))
            using (var path = CreateRoundedRectangle(bounds, 2))
            {
                g.FillPath(brush, path);
            }

            // Border using theme colors
            Color borderColor = isPrimary ? theme.AccentColor : theme.GridHeaderBorderColor;
            using (var pen = new Pen(borderColor, 1))
            using (var path = CreateRoundedRectangle(bounds, 2))
            {
                g.DrawPath(pen, path);
            }

            // Text
            Color textColor = isPrimary ? theme.GridHeaderBackColor : theme.GridHeaderForeColor;
            using (var font = new Font("Segoe UI", 9, FontStyle.Regular))
            {
                DrawCenteredText(g, text, font, textColor, bounds);
            }
        }

        private void PaintAntPageNumbers(Graphics g, BeepGridPro grid, NavigationLayout layout, IBeepTheme theme)
        {
            // Draw numbered pages (1, 2, 3, ...) using dynamic pagination
            int totalRecords = grid.Data.Rows.Count;
            int currentPage = grid.Render.GetCurrentPage(grid);
            var (startPage, endPage) = grid.Render.GetVisiblePageRange(grid, 3);

            // Start position: right after the Previous button
            int x = layout.PreviousButtonRect.Right + SPACING;
            int y = layout.PreviousButtonRect.Y;

            for (int page = startPage; page <= endPage; page++)
            {
                Rectangle pageRect = new Rectangle(x, y, BUTTON_SIZE, BUTTON_SIZE);
                bool isActive = page == currentPage;

                int targetPage = page;
                grid.AddHitArea($"Page{page}", pageRect, null, () => grid.Render.GoToPage(grid, targetPage));

                if (isActive)
                {
                    // Active page - using theme accent color
                    using (var brush = new SolidBrush(theme.AccentColor))
                    using (var path = CreateRoundedRectangle(pageRect, 2))
                    {
                        g.FillPath(brush, path);
                    }
                    using (var font = new Font("Segoe UI", 9, FontStyle.Regular))
                    {
                        DrawCenteredText(g, page.ToString(), font, theme.GridHeaderBackColor, pageRect);
                    }
                }
                else
                {
                    // Inactive page border using theme border color
                    using (var pen = new Pen(theme.GridHeaderBorderColor, 1))
                    using (var path = CreateRoundedRectangle(pageRect, 2))
                    {
                        g.DrawPath(pen, path);
                    }
                    using (var font = new Font("Segoe UI", 9, FontStyle.Regular))
                    {
                        DrawCenteredText(g, page.ToString(), font, theme.GridHeaderForeColor, pageRect);
                    }
                }

                x += BUTTON_SIZE + SPACING; // Move left to right
            }
        }

        private void PaintAntInfo(Graphics g, Rectangle bounds, int current, int total, IBeepTheme theme)
        {
            if (bounds.IsEmpty) return;

            string text = $"{current} / {total}";
            using (var font = new Font("Segoe UI", 9, FontStyle.Regular))
            {
                DrawCenteredText(g, text, font, theme.GridHeaderForeColor, bounds);
            }
        }

        public override void PaintButton(Graphics g, Rectangle bounds, NavigationButtonType buttonType, 
            NavigationButtonState state, IBeepUIComponent component, IBeepTheme theme)
        {
            // Handled in PaintAntButton
        }

        public override void PaintPositionIndicator(Graphics g, Rectangle bounds, int currentPosition, 
            int totalRecords, IBeepTheme theme)
        {
            // Handled in PaintAntInfo
        }

        public override NavigationLayout CalculateLayout(Rectangle availableBounds, 
            int totalRecords, bool showCrudButtons)
        {
            var layout = new NavigationLayout
            {
                IsCompact = false,
                IconOnly = false,
                ButtonSize = new Size(BUTTON_SIZE, BUTTON_SIZE),
                ButtonSpacing = SPACING,
                TotalHeight = RecommendedHeight
            };

            int y = availableBounds.Y + (availableBounds.Height - BUTTON_SIZE) / 2;

            // Left section - CRUD
            int x = availableBounds.X + 16;
            layout.AddNewButtonRect = new Rectangle(x, y, 70, BUTTON_SIZE);
            x += 70 + SPACING;

            layout.DeleteButtonRect = new Rectangle(x, y, 60, BUTTON_SIZE);
            x += 60 + SPACING;

            layout.SaveButtonRect = new Rectangle(x, y, 60, BUTTON_SIZE);

            // Right section - Navigation (calculate from right to left)
            x = availableBounds.Right - 16 - BUTTON_SIZE;
            layout.LastButtonRect = new Rectangle(x, y, BUTTON_SIZE, BUTTON_SIZE);
            
            x -= BUTTON_SIZE + SPACING;
            layout.NextButtonRect = new Rectangle(x, y, BUTTON_SIZE, BUTTON_SIZE);
            
            // Leave space for 3 page numbers (BUTTON_SIZE * 3 + SPACING * 2)
            x -= (BUTTON_SIZE * 3 + SPACING * 2) + SPACING;
            
            layout.PreviousButtonRect = new Rectangle(x, y, BUTTON_SIZE, BUTTON_SIZE);
            
            x -= BUTTON_SIZE + SPACING;
            layout.FirstButtonRect = new Rectangle(x, y, BUTTON_SIZE, BUTTON_SIZE);

            // Position info (between CRUD and navigation)
            int infoWidth = 80;
            int centerX = (layout.SaveButtonRect.Right + layout.FirstButtonRect.Left) / 2;
            layout.PositionIndicatorRect = new Rectangle(centerX - infoWidth / 2, y, infoWidth, BUTTON_SIZE);

            layout.TotalWidth = availableBounds.Width;

            return layout;
        }
    }
}
