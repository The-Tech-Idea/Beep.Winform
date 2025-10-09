using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Menus.Helpers;
using TheTechIdea.Beep.Utilities;

namespace TheTechIdea.Beep.Winform.Controls.Menus.Painters
{
    /// <summary>
    /// Card Layout menu bar painter - menu items displayed as cards with icons, titles, and descriptions
    /// </summary>
    public sealed class CardLayoutMenuBarPainter : MenuBarPainterBase
    {
        #region Fields
        private List<Rectangle> _cardRects = new List<Rectangle>();
        #endregion

        #region MenuBarPainterBase Implementation
        public override MenuBarContext AdjustLayout(Rectangle drawingRect, MenuBarContext ctx)
        {
            if (drawingRect.IsEmpty || ctx == null)
                return ctx ?? new MenuBarContext();

            UpdateContextColors(ctx);
            ctx.DrawingRect = drawingRect;

            int padding = ScaleValue(16);
            ctx.ContentRect = Rectangle.Inflate(drawingRect, -padding, -padding);
            ctx.MenuItemsRect = ctx.ContentRect;

            // Calculate card grid layout
            CalculateCardRects(ctx);

            return ctx;
        }

        public override void DrawBackground(Graphics g, MenuBarContext ctx)
        {
            if (g == null || ctx.DrawingRect.IsEmpty) return;

            using var bgBrush = new SolidBrush(GetBackgroundColor());
            g.FillRectangle(bgBrush, ctx.DrawingRect);
        }

        public override void DrawContent(Graphics g, MenuBarContext ctx)
        {
            if (g == null || ctx == null || ctx.MenuItems == null) return;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw each card
            for (int i = 0; i < _cardRects.Count && i < ctx.MenuItems.Count; i++)
            {
                var item = ctx.MenuItems[i];
                var rect = _cardRects[i];
                DrawCard(g, rect, item, ctx, i);
            }
        }

        public override void DrawForegroundAccents(Graphics g, MenuBarContext ctx)
        {
            if (g == null || ctx == null) return;

            // Draw hover effects on cards
            for (int i = 0; i < _cardRects.Count; i++)
            {
                if (IsAreaHovered($"Card_{i}"))
                {
                    var rect = _cardRects[i];
                    DrawCardHoverEffect(g, rect);
                }
            }

            // Draw selection indicator
            if (ctx.SelectedIndex >= 0 && ctx.SelectedIndex < _cardRects.Count)
            {
                var rect = _cardRects[ctx.SelectedIndex];
                DrawCardSelectionEffect(g, rect, ctx);
            }
        }

        public override void UpdateHitAreas(BaseControl owner, MenuBarContext ctx, Action<string, Rectangle> notifyAreaHit)
        {
            if (owner == null || ctx == null) return;

            ClearOwnerHitAreas();

            // Add hit areas for each card
            for (int i = 0; i < _cardRects.Count && i < ctx.MenuItems.Count; i++)
            {
                var item = ctx.MenuItems[i];
                int cardIndex = i;

                AddHitAreaToOwner($"Card_{i}", _cardRects[i], () =>
                {
                    if (item.IsEnabled)
                    {
                        ctx.SelectedIndex = cardIndex;
                        notifyAreaHit?.Invoke($"Card_{cardIndex}", _cardRects[cardIndex]);
                        SafeInvalidate();
                    }
                });
            }
        }
        #endregion

        #region Private Methods
        private void CalculateCardRects(MenuBarContext ctx)
        {
            _cardRects.Clear();

            if (ctx.MenuItems == null || ctx.ContentRect.IsEmpty) return;

            int x = ctx.ContentRect.X;
            int y = ctx.ContentRect.Y;
            int availableWidth = ctx.ContentRect.Width;

            int cardWidth = ScaleValue(180);
            int cardHeight = ScaleValue(120);
            int cardSpacing = ScaleValue(12);

            // Calculate how many cards fit per row
            int cardsPerRow = Math.Max(1, (availableWidth + cardSpacing) / (cardWidth + cardSpacing));

            int currentRow = 0;
            int currentCol = 0;

            foreach (var item in ctx.MenuItems)
            {
                int cardX = x + currentCol * (cardWidth + cardSpacing);
                int cardY = y + currentRow * (cardHeight + cardSpacing);

                var cardRect = new Rectangle(cardX, cardY, cardWidth, cardHeight);
                _cardRects.Add(cardRect);

                currentCol++;
                if (currentCol >= cardsPerRow)
                {
                    currentCol = 0;
                    currentRow++;
                }
            }
        }

        private void DrawCard(Graphics g, Rectangle rect, SimpleItem item, MenuBarContext ctx, int index)
        {
            if (g == null || item == null) return;

            int cardPadding = ScaleValue(12);
            int cornerRadius = ScaleValue(8);

            // Draw card shadow
            DrawCardShadow(g, rect, cornerRadius);

            // Draw card background with rounded corners
            var bgColor = (index == ctx.SelectedIndex) ? GetSelectedBackgroundColor() : GetItemBackgroundColor();
            using (var path = CreateRoundedPath(rect, cornerRadius))
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillPath(brush, path);
            }

            // Draw card border
            using (var path = CreateRoundedPath(rect, cornerRadius))
            using (var pen = new Pen(GetBorderColor(), ScaleValue(1)))
            {
                g.DrawPath(pen, path);
            }

            // Draw icon at top
            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                int iconSize = ScaleValue(48);
                var iconRect = new Rectangle(
                    rect.X + (rect.Width - iconSize) / 2,
                    rect.Y + cardPadding,
                    iconSize, iconSize);
                var iconColor = item.IsEnabled ? GetItemForegroundColor() : GetDisabledForegroundColor();
                MenuBarRenderingHelpers.DrawMenuItemIcon(g, iconRect, item.ImagePath, iconColor);
            }

            // Draw title text
            var titleColor = item.IsEnabled ? GetItemForegroundColor() : GetDisabledForegroundColor();
            using (var brush = new SolidBrush(titleColor))
            using (var titleFont = new Font(ctx.TextFont.FontFamily, ctx.TextFont.Size + 1, FontStyle.Bold))
            {
                var titleRect = new Rectangle(
                    rect.X + cardPadding,
                    rect.Y + ScaleValue(70),
                    rect.Width - cardPadding * 2,
                    ScaleValue(20));
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };
                g.DrawString(item.Text ?? "", titleFont, brush, titleRect, sf);
            }

            // Draw description if available (using Category as description placeholder)
            if (item.Category != DatasourceCategory.NONE)
            {
                var descColor = Color.FromArgb(128, titleColor);
                using (var brush = new SolidBrush(descColor))
                using (var descFont = new Font(ctx.TextFont.FontFamily, ctx.TextFont.Size - 1))
                {
                    int descY = ScaleValue(92);
                    var descRect = new Rectangle(
                        rect.X + cardPadding,
                        rect.Y + descY,
                        rect.Width - cardPadding * 2,
                        rect.Height - descY - cardPadding);
                    var sf = new StringFormat 
                    { 
                        Alignment = StringAlignment.Center, 
                        LineAlignment = StringAlignment.Near,
                        Trimming = StringTrimming.EllipsisCharacter
                    };
                    g.DrawString(item.Category.ToString(), descFont, brush, descRect, sf);
                }
            }
        }

        private void DrawCardShadow(Graphics g, Rectangle rect, int cornerRadius)
        {
            var shadowRect = new Rectangle(rect.X + ScaleValue(2), rect.Y + ScaleValue(2), rect.Width, rect.Height);
            using (var path = CreateRoundedPath(shadowRect, cornerRadius))
            using (var brush = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
            {
                g.FillPath(brush, path);
            }
        }

        private void DrawCardHoverEffect(Graphics g, Rectangle rect)
        {
            using (var path = CreateRoundedPath(rect, ScaleValue(8)))
            using (var brush = new SolidBrush(Color.FromArgb(30, GetAccentColor())))
            {
                g.FillPath(brush, path);
            }
        }

        private void DrawCardSelectionEffect(Graphics g, Rectangle rect, MenuBarContext ctx)
        {
            using (var path = CreateRoundedPath(rect, ScaleValue(8)))
            using (var pen = new Pen(GetAccentColor(), ScaleValue(3)))
            {
                g.DrawPath(pen, path);
            }
        }
        #endregion
    }
}
