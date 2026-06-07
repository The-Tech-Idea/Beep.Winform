using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.RadioGroup.Helpers;
using TheTechIdea.Beep.Winform.Controls.RadioGroup.Models;

namespace TheTechIdea.Beep.Winform.Controls.RadioGroup.Renderers
{
    /// <summary>
    /// Large touch-friendly tile renderer. Distinct visual identity: 48dp icon
    /// above a title + subtitle, 4px corner, 24dp selection check in top-right.
    /// </summary>
    public class TileRadioRenderer : BaseRadioRenderer
    {
        private const int MinTileWidth = 120;
        private const int MinTileHeight = 100;
        private const int TilePadding = 16;
        private const int ComponentSpacing = 8;
        private const int CornerRadius = 4;
        private const int SelectionIndicatorSize = 24;
        private int IconSize => Math.Min(MaxImageSize.Width, MaxImageSize.Height);

        public override string StyleName => "Tile";
        public override string DisplayName => "Large Tiles";
        public override bool SupportsMultipleSelection => true;
        public override BeepControlStyle ControlStyle { get; set; } = BeepControlStyle.Metro;

        public override void RenderItem(Graphics graphics, SimpleItem item, Rectangle rectangle, RadioItemState state)
        {
            if (graphics == null || item == null) return;
            ResolveTokens();
            var t = _tokens;

            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var tileRect = GetTileRectangle(rectangle);

            DrawTileBackground(graphics, tileRect, state, t);
            DrawTileBorder(graphics, tileRect, state, t);

            if (state.IsSelected) DrawSelectionIndicator(graphics, tileRect, t, state);
            DrawContent(graphics, item, tileRect, state, t);

            if (state.IsError) DrawErrorOverlay(graphics, tileRect, CornerRadius);
        }

        private Rectangle GetTileRectangle(Rectangle itemRectangle) => itemRectangle;

        private void DrawTileBackground(Graphics graphics, Rectangle tileRect, RadioItemState state, RadioGroupColorTokens t)
        {
            Color backgroundColor;
            float anim = Math.Clamp(state.AnimationProgress, 0f, 1f);
            if (!state.IsEnabled) backgroundColor = t.DisabledContainer;
            else if (state.IsSelected) backgroundColor = t.SurfaceContainer;
            else if (state.IsPressed) backgroundColor = t.PressStateLayer;
            else if (state.IsHovered) backgroundColor = t.HoverStateLayer;
            else backgroundColor = t.Surface;

            // Hover/press state layers fade in over AnimationProgress so the hover-in
            // is smooth rather than popping in.
            if (state.IsEnabled && (state.IsHovered || state.IsPressed) && !state.IsSelected)
            {
                byte alpha = (byte)Math.Min(255, backgroundColor.A * anim);
                backgroundColor = Color.FromArgb(alpha, backgroundColor);
            }

            using (var brush = new SolidBrush(backgroundColor))
            using (var path = CreateRoundedRectanglePath(tileRect, S(CornerRadius)))
            {
                graphics.FillPath(brush, path);
            }
        }

        private void DrawTileBorder(Graphics graphics, Rectangle tileRect, RadioItemState state, RadioGroupColorTokens t)
        {
            Color borderColor;
            float borderWidth;
            if (!state.IsEnabled) { borderColor = t.OutlineVariant; borderWidth = SF(1f); }
            else if (state.IsSelected) { borderColor = t.Primary; borderWidth = SF(2f); }
            else if (state.IsHovered) { borderColor = t.Outline; borderWidth = SF(1.5f); }
            else { borderColor = t.OutlineVariant; borderWidth = SF(1f); }

            using (var pen = new Pen(borderColor, borderWidth))
            using (var path = CreateRoundedRectanglePath(tileRect, S(CornerRadius)))
            {
                graphics.DrawPath(pen, path);
            }
        }

        private void DrawSelectionIndicator(Graphics graphics, Rectangle tileRect, RadioGroupColorTokens t, RadioItemState state)
        {
            // Circle scales 0 → 1 over AnimationProgress so the selection appears
            // as a deliberate "pop" rather than a flash.  The checkmark then draws
            // segment by segment on top of the fully-grown circle.
            float anim = Math.Clamp(state.AnimationProgress, 0f, 1f);
            int size = S(SelectionIndicatorSize);
            var indicatorRect = new Rectangle(
                tileRect.Right - S(TilePadding) - size,
                tileRect.Y + S(TilePadding),
                size, size);
            int radius = (int)Math.Round(size * 0.5f * anim);
            if (radius <= 0) return;
            int cx = indicatorRect.X + indicatorRect.Width / 2;
            int cy = indicatorRect.Y + indicatorRect.Height / 2;
            var scaledRect = new Rectangle(cx - radius, cy - radius, radius * 2, radius * 2);
            using (var brush = new SolidBrush(t.Primary))
            {
                graphics.FillEllipse(brush, scaledRect);
            }
            if (anim < 0.5f) return; // hold off the checkmark until the circle is at least half-grown
            float checkAnim = (anim - 0.5f) / 0.5f; // re-normalize 0..1 for the stroke
            using (var pen = new Pen(t.OnPrimary, SF(2.5f)))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                var p1 = new PointF(indicatorRect.X + indicatorRect.Width * 0.25f, indicatorRect.Y + indicatorRect.Height * 0.5f);
                var p2 = new PointF(indicatorRect.X + indicatorRect.Width * 0.45f, indicatorRect.Y + indicatorRect.Height * 0.7f);
                var p3 = new PointF(indicatorRect.X + indicatorRect.Width * 0.78f, indicatorRect.Y + indicatorRect.Height * 0.3f);
                if (checkAnim <= 0.5f)
                {
                    var end = new PointF(p1.X + (p2.X - p1.X) * (checkAnim / 0.5f), p1.Y + (p2.Y - p1.Y) * (checkAnim / 0.5f));
                    graphics.DrawLine(pen, p1, end);
                }
                else
                {
                    graphics.DrawLine(pen, p1, p2);
                    var t2 = (checkAnim - 0.5f) / 0.5f;
                    var end = new PointF(p2.X + (p3.X - p2.X) * t2, p2.Y + (p3.Y - p2.Y) * t2);
                    graphics.DrawLine(pen, p2, end);
                }
            }
        }

        private void DrawContent(Graphics graphics, SimpleItem item, Rectangle tileRect, RadioItemState state, RadioGroupColorTokens t)
        {
            int currentX = tileRect.X + S(TilePadding);
            int topY = tileRect.Y + S(TilePadding);
            int rightCap = tileRect.Right - S(TilePadding) - S(SelectionIndicatorSize) - S(ComponentSpacing);

            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                int sz = Math.Min(IconSize, Math.Max(16, tileRect.Height / 3));
                var iconRect = new Rectangle(currentX, topY + (sz - sz) / 2, sz, sz);
                if (state.IsSelected)
                {
                    iconRect = new Rectangle(currentX, topY, sz, sz);
                }
                var iconPath = RadioGroupIconHelpers.GetItemIconPath(item.ImagePath);
                var iconColor = state.IsEnabled ? (state.IsSelected ? t.Primary : t.OnSurfaceVariant) : t.Disabled;
                RadioGroupIconHelpers.PaintIcon(graphics, iconRect, iconPath, iconColor, _theme, UseThemeColors, ControlStyle);
            }

            if (!string.IsNullOrEmpty(item.Text))
            {
                using var titleFont = GetLabelFont();
                int titleY = topY + IconSize + S(ComponentSpacing);
                int titleHeight = (int)Math.Ceiling(TextUtils.MeasureText(graphics, item.Text, titleFont).Height);
                var textRect = new Rectangle(currentX, titleY, Math.Max(0, rightCap - currentX), titleHeight);
                using (var brush = new SolidBrush(state.IsEnabled ? t.OnSurface : t.Disabled))
                {
                    var fmt = new System.Drawing.StringFormat
                    {
                        Alignment = StringAlignment.Near,
                        LineAlignment = StringAlignment.Near,
                        Trimming = StringTrimming.EllipsisCharacter
                    };
                    graphics.DrawString(item.Text, titleFont, brush, textRect, fmt);
                }
            }

            if (!string.IsNullOrEmpty(item.SubText))
            {
                using var subFont = GetSubtextFont();
                using var titleFont = GetLabelFont();
                int titleY = topY + IconSize + S(ComponentSpacing);
                int titleHeight = (int)Math.Ceiling(TextUtils.MeasureText(graphics, item.Text ?? string.Empty, titleFont).Height);
                int subY = titleY + titleHeight;
                int subHeight = Math.Max(0, tileRect.Bottom - S(TilePadding) - subY);
                var subRect = new Rectangle(currentX, subY, Math.Max(0, rightCap - currentX), subHeight);
                using var subBrush = new SolidBrush(t.OnSurfaceVariant);
                var fmt = new System.Drawing.StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Near,
                    Trimming = StringTrimming.EllipsisCharacter
                };
                graphics.DrawString(item.SubText, subFont, subBrush, subRect, fmt);
            }
        }

        public override Size MeasureItem(SimpleItem item, Graphics graphics)
        {
            if (item == null) return new Size(MinTileWidth, MinTileHeight);
            int width = S(MinTileWidth);
            int height = S(MinTileHeight);

            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                width = Math.Max(width, IconSize + S(TilePadding) * 2 + S(SelectionIndicatorSize));
            }
            if (!string.IsNullOrEmpty(item.Text))
            {
                using var titleFont = GetLabelFont();
                var textSize = TextUtils.MeasureText(graphics, item.Text, titleFont);
                width = Math.Max(width, (int)Math.Ceiling(textSize.Width) + S(TilePadding) * 2 + S(SelectionIndicatorSize));
            }
            if (!string.IsNullOrEmpty(item.SubText))
            {
                using var subFont = GetSubtextFont();
                var subSize = TextUtils.MeasureText(graphics, item.SubText, subFont);
                width = Math.Max(width, (int)Math.Ceiling(subSize.Width) + S(TilePadding) * 2 + S(SelectionIndicatorSize));
            }
            return new Size(width, height);
        }

        public override Rectangle GetContentArea(Rectangle itemRectangle) => itemRectangle;
        public override Rectangle GetSelectorArea(Rectangle itemRectangle) => itemRectangle;
    }
}
