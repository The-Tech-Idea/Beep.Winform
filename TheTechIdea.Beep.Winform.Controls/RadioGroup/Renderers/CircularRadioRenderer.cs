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
    /// Traditional circular radio renderer. Distinct visual identity: 20dp circle,
    /// 2px outline ring, no rounded background, classic dot indicator.
    /// Single-selection only by design (per W3C / WCAG radio-group semantics).
    /// </summary>
    public sealed class CircularRadioRenderer : BaseRadioRenderer
    {
        private const int RadioSize = 20;
        private const int MinItemHeight = 40;
        private const int ItemPadding = 12;
        private const int ComponentSpacing = 12;
        private int IconSize => Math.Min(MaxImageSize.Width, MaxImageSize.Height);

        public override string StyleName => "Circular";
        public override string DisplayName => "Traditional Circular";
        public override bool SupportsMultipleSelection => false;
        public override BeepControlStyle ControlStyle { get; set; } = BeepControlStyle.Minimal;

        public override void RenderItem(Graphics graphics, SimpleItem item, Rectangle rectangle, RadioItemState state)
        {
            if (graphics == null || item == null) return;

            ResolveTokens();
            var t = _tokens;

            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            if (state.IsEnabled && state.IsHovered && t.HoverStateLayer.A > 0)
            {
                using (var brush = new SolidBrush(t.HoverStateLayer))
                {
                    graphics.FillRectangle(brush, rectangle);
                }
            }

            var contentArea = GetContentArea(rectangle);
            var radioArea = GetSelectorArea(rectangle);

            DrawRadio(graphics, radioArea, state, t);
            DrawContent(graphics, item, contentArea, radioArea, state, t);

            if (state.IsError) DrawErrorOverlay(graphics, rectangle, 0);
        }

        private void DrawRadio(Graphics graphics, Rectangle radioArea, RadioItemState state, RadioGroupColorTokens t)
        {
            var center = new Point(radioArea.X + radioArea.Width / 2, radioArea.Y + radioArea.Height / 2);
            int radius = S(RadioSize) / 2;

            var borderColor = !state.IsEnabled
                ? t.Disabled
                : (state.IsSelected ? t.Primary : t.Outline);
            var outerRect = new Rectangle(center.X - radius, center.Y - radius, radius * 2, radius * 2);

            using (var pen = new Pen(borderColor, SF(2f)))
            {
                graphics.DrawEllipse(pen, outerRect);
            }

            if (state.IsSelected)
            {
                // Scale the inner dot in via AnimationProgress (0 = hidden, 1 = full size).
                int targetRadius = Math.Max(2, radius - S(6));
                float anim = Math.Clamp(state.AnimationProgress, 0f, 1f);
                int dotRadius = (int)Math.Round(targetRadius * anim);
                if (dotRadius > 0)
                {
                    var dotRect = new Rectangle(center.X - dotRadius, center.Y - dotRadius, dotRadius * 2, dotRadius * 2);
                    using (var brush = new SolidBrush(state.IsEnabled ? t.Primary : t.Disabled))
                    {
                        graphics.FillEllipse(brush, dotRect);
                    }
                }
            }
        }

        private void DrawContent(Graphics graphics, SimpleItem item, Rectangle contentArea, Rectangle radioArea, RadioItemState state, RadioGroupColorTokens t)
        {
            int currentX = radioArea.Right + ComponentSpacing;
            currentX = Math.Max(currentX, contentArea.Left);

            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                int sz = Math.Min(IconSize, Math.Max(12, contentArea.Height - 4));
                var iconRect = new Rectangle(currentX, contentArea.Y + (contentArea.Height - sz) / 2, sz, sz);
                var iconPath = RadioGroupIconHelpers.GetItemIconPath(item.ImagePath);
                var iconColor = state.IsEnabled ? t.OnSurfaceVariant : t.Disabled;
                RadioGroupIconHelpers.PaintIcon(graphics, iconRect, iconPath, iconColor, _theme, UseThemeColors, ControlStyle);
                currentX += sz + ComponentSpacing;
            }

            bool hasSubtitle = !string.IsNullOrEmpty(item.SubText);
            if (!string.IsNullOrEmpty(item.Text))
            {
                int textWidth = Math.Max(0, contentArea.Right - currentX);
                int titleHeight = contentArea.Height;
                if (hasSubtitle)
                {
                    var titleSize = TextUtils.MeasureText(graphics, item.Text, _textFont);
                    titleHeight = Math.Max(1, Math.Min(contentArea.Height - 1, (int)Math.Ceiling(titleSize.Height)));
                }

                var textRect = new Rectangle(currentX, contentArea.Y, textWidth, titleHeight);
                using (var brush = new SolidBrush(state.IsEnabled ? t.OnSurface : t.Disabled))
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

            if (hasSubtitle)
            {
                using var subtitleFont = GetSubtextFont();
                int textWidth = Math.Max(0, contentArea.Right - currentX);
                var titleSize = TextUtils.MeasureText(graphics, item.Text ?? string.Empty, _textFont);
                int titleHeight = Math.Max(1, Math.Min(contentArea.Height - 1, (int)Math.Ceiling(titleSize.Height)));
                int subtitleHeight = Math.Max(0, contentArea.Height - titleHeight);
                var subtitleRect = new Rectangle(currentX, contentArea.Y + titleHeight, textWidth, subtitleHeight);
                using var brush = new SolidBrush(t.OnSurfaceVariant);
                var fmt = new System.Drawing.StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter
                };
                graphics.DrawString(item.SubText, subtitleFont, brush, subtitleRect, fmt);
            }
        }

        public override Size MeasureItem(SimpleItem item, Graphics graphics)
        {
            if (item == null) return new Size(120, MinItemHeight);

            int width = S(ItemPadding);
            int height = S(MinItemHeight);
            width += S(RadioSize) + S(ComponentSpacing);

            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                width += IconSize + ComponentSpacing;
                height = Math.Max(height, IconSize + S(ItemPadding));
            }

            if (!string.IsNullOrEmpty(item.Text))
            {
                var textSize = TextUtils.MeasureText(graphics, item.Text, _textFont);
                width += (int)Math.Ceiling(textSize.Width);
                height = Math.Max(height, (int)Math.Ceiling(textSize.Height) + S(ItemPadding));
            }

            if (!string.IsNullOrEmpty(item.SubText))
            {
                using var subFont = GetSubtextFont();
                var subSize = TextUtils.MeasureText(graphics, item.SubText, subFont);
                height = Math.Max(height, (int)Math.Ceiling(subSize.Height) + S(ItemPadding));
            }

            width += S(ItemPadding);
            return new Size(width, height);
        }

        public override Rectangle GetContentArea(Rectangle itemRectangle)
        {
            return new Rectangle(
                itemRectangle.X + S(ItemPadding) / 2,
                itemRectangle.Y + S(ItemPadding) / 2,
                Math.Max(0, itemRectangle.Width - S(ItemPadding)),
                Math.Max(0, itemRectangle.Height - S(ItemPadding)));
        }

        public override Rectangle GetSelectorArea(Rectangle itemRectangle)
        {
            var contentArea = GetContentArea(itemRectangle);
            return new Rectangle(
                contentArea.X,
                contentArea.Y + (contentArea.Height - S(RadioSize)) / 2,
                S(RadioSize), S(RadioSize));
        }
    }
}
