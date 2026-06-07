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
    /// Chip / pill renderer. Distinct visual identity: fully rounded pill
    /// (16px radius), outline or fill, small close icon when in single-select.
    /// </summary>
    public class ChipRadioRenderer : BaseRadioRenderer
    {
        private const int MinItemHeight = 36;
        private const int ItemPadding = 12;
        private const int ComponentSpacing = 8;
        private const int ChipRadius = 16;
        private int IconSize => Math.Min(MaxImageSize.Width, MaxImageSize.Height);

        public override string StyleName => "Chip";
        public override string DisplayName => "Chip";
        public override bool SupportsMultipleSelection => true;
        public override BeepControlStyle ControlStyle { get; set; } = BeepControlStyle.PillRail;

        public override void RenderItem(Graphics graphics, SimpleItem item, Rectangle rectangle, RadioItemState state)
        {
            if (graphics == null || item == null) return;
            ResolveTokens();
            var t = _tokens;

            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var chipRect = GetChipRectangle(rectangle);

            DrawChipBackground(graphics, chipRect, state, t);
            DrawChipBorder(graphics, chipRect, state, t);
            DrawChipContent(graphics, item, chipRect, state, t);

            if (state.IsError) DrawErrorOverlay(graphics, chipRect, ChipRadius);
            if (state.IsPressed) DrawPressedOverlay(graphics, chipRect, ChipRadius);

            // Pressed scale animation: shrink to 95% and back to 100% over the animation.
            if (state.IsPressed && state.AnimationProgress < 1f)
            {
                float dip = state.AnimationProgress * 2f; // 0→1→0 over the lifetime
                float scale = 1f - (0.05f * (1f - Math.Abs(dip - 0.5f) * 2f));
                int dx = (int)(chipRect.Width * (1f - scale) / 2f);
                int dy = (int)(chipRect.Height * (1f - scale) / 2f);
                var scaled = new Rectangle(chipRect.X + dx, chipRect.Y + dy,
                    chipRect.Width - dx * 2, chipRect.Height - dy * 2);
                using (var brush = new SolidBrush(Color.FromArgb(40, t.OnSurface)))
                using (var path = CreateRoundedRectanglePath(scaled, S(ChipRadius)))
                {
                    graphics.FillPath(brush, path);
                }
            }
        }

        private Rectangle GetChipRectangle(Rectangle itemRectangle)
        {
            int vPad = Math.Max(0, (itemRectangle.Height - S(MinItemHeight)) / 2);
            return new Rectangle(itemRectangle.X, itemRectangle.Y + vPad, itemRectangle.Width, itemRectangle.Height - vPad * 2);
        }

        private void DrawChipBackground(Graphics graphics, Rectangle chipRect, RadioItemState state, RadioGroupColorTokens t)
        {
            Color backgroundColor;
            if (!state.IsEnabled) backgroundColor = t.DisabledContainer;
            else if (state.IsSelected) backgroundColor = t.PrimaryContainer;
            else if (state.IsPressed) backgroundColor = t.PressStateLayer;
            else if (state.IsHovered) backgroundColor = t.HoverStateLayer;
            else backgroundColor = t.Surface;

            using (var brush = new SolidBrush(backgroundColor))
            using (var path = CreateRoundedRectanglePath(chipRect, S(ChipRadius)))
            {
                graphics.FillPath(brush, path);
            }
        }

        private void DrawChipBorder(Graphics graphics, Rectangle chipRect, RadioItemState state, RadioGroupColorTokens t)
        {
            Color borderColor;
            float borderWidth;
            if (!state.IsEnabled) { borderColor = t.OutlineVariant; borderWidth = SF(1f); }
            else if (state.IsSelected) { borderColor = t.Primary; borderWidth = SF(1.5f); }
            else if (state.IsFocused) { borderColor = t.Primary; borderWidth = SF(1.5f); }
            else if (state.IsHovered) { borderColor = t.Outline; borderWidth = SF(1f); }
            else { borderColor = t.OutlineVariant; borderWidth = SF(1f); }

            // Animate border width: 1px resting → 1.5px selected (interpolated by AnimationProgress).
            float anim = Math.Clamp(state.AnimationProgress, 0f, 1f);
            float animatedWidth = (state.IsSelected || state.IsFocused)
                ? SF(1f) + (SF(1.5f) - SF(1f)) * anim
                : SF(1f);

            using (var pen = new Pen(borderColor, animatedWidth))
            using (var path = CreateRoundedRectanglePath(chipRect, S(ChipRadius)))
            {
                graphics.DrawPath(pen, path);
            }
        }

        private void DrawChipContent(Graphics graphics, SimpleItem item, Rectangle chipRect, RadioItemState state, RadioGroupColorTokens t)
        {
            int currentX = chipRect.X + S(ItemPadding);
            int rightCap = chipRect.Right - S(ItemPadding);

            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                int sz = Math.Min(IconSize, Math.Max(10, chipRect.Height - 8));
                var iconRect = new Rectangle(currentX, chipRect.Y + (chipRect.Height - sz) / 2, sz, sz);
                var iconPath = RadioGroupIconHelpers.GetItemIconPath(item.ImagePath);
                var iconColor = state.IsEnabled ? (state.IsSelected ? t.OnPrimaryContainer : t.OnSurfaceVariant) : t.Disabled;
                RadioGroupIconHelpers.PaintIcon(graphics, iconRect, iconPath, iconColor, _theme, UseThemeColors, ControlStyle);
                currentX += sz + S(ComponentSpacing);
            }

            if (!string.IsNullOrEmpty(item.Text))
            {
                var textRect = new Rectangle(currentX, chipRect.Y, Math.Max(0, rightCap - currentX), chipRect.Height);
                using (var brush = new SolidBrush(state.IsEnabled
                    ? (state.IsSelected ? t.OnPrimaryContainer : t.OnSurface)
                    : t.Disabled))
                {
                    var fmt = new System.Drawing.StringFormat
                    {
                        Alignment = StringAlignment.Near,
                        LineAlignment = StringAlignment.Center,
                        Trimming = StringTrimming.EllipsisCharacter
                    };
                    graphics.DrawString(item.Text, _textFont, brush, textRect, fmt);
                }
            }

            if (!string.IsNullOrEmpty(item.SubText))
            {
                using var subtitleFont = GetSubtextFont();
                int textWidth = Math.Max(0, rightCap - currentX);
                var titleSize = TextUtils.MeasureText(graphics, item.Text ?? string.Empty, _textFont);
                int titleHeight = Math.Max(1, Math.Min(chipRect.Height - 1, (int)Math.Ceiling(titleSize.Height)));
                int subtitleHeight = Math.Max(0, chipRect.Height - titleHeight);
                var subRect = new Rectangle(currentX, chipRect.Y + titleHeight, textWidth, subtitleHeight);
                using var subBrush = new SolidBrush(t.OnSurfaceVariant);
                var fmt = new System.Drawing.StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter
                };
                graphics.DrawString(item.SubText, subtitleFont, subBrush, subRect, fmt);
            }
        }

        public override Size MeasureItem(SimpleItem item, Graphics graphics)
        {
            if (item == null) return new Size(120, MinItemHeight);
            int width = S(ItemPadding) * 2;
            int height = S(MinItemHeight);

            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                width += IconSize + S(ComponentSpacing);
                height = Math.Max(height, IconSize + 8);
            }
            if (!string.IsNullOrEmpty(item.Text))
            {
                var textSize = TextUtils.MeasureText(graphics, item.Text, _textFont);
                width += (int)Math.Ceiling(textSize.Width) + S(ComponentSpacing);
                height = Math.Max(height, (int)Math.Ceiling(textSize.Height) + 8);
            }
            if (!string.IsNullOrEmpty(item.SubText))
            {
                using var subFont = GetSubtextFont();
                var subSize = TextUtils.MeasureText(graphics, item.SubText, subFont);
                height = Math.Max(height, (int)Math.Ceiling(subSize.Height) + 12);
            }
            return new Size(width, height);
        }

        public override Rectangle GetContentArea(Rectangle itemRectangle)
        {
            return new Rectangle(
                itemRectangle.X + S(ItemPadding),
                itemRectangle.Y,
                Math.Max(0, itemRectangle.Width - S(ItemPadding) * 2),
                itemRectangle.Height);
        }

        public override Rectangle GetSelectorArea(Rectangle itemRectangle)
        {
            return itemRectangle;
        }
    }
}
