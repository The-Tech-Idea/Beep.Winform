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
    /// Modern flat design renderer. Distinct visual identity: 1.5px ring, 6px corner radius,
    /// transparent idle / tinted selected background, simple check / dot indicators.
    /// </summary>
    public sealed class FlatRadioRenderer : BaseRadioRenderer
    {
        private const int SelectorSize = 20;
        private const int MinItemHeight = 40;
        private const int ItemPadding = 12;
        private const int ComponentSpacing = 12;
        private const int CornerRadius = 6;
        private int IconSize => Math.Min(MaxImageSize.Width, MaxImageSize.Height);

        public override string StyleName => "Flat";
        public override string DisplayName => "Modern Flat Design";
        public override bool SupportsMultipleSelection => true;
        public override BeepControlStyle ControlStyle { get; set; } = BeepControlStyle.Minimal;

        public override void RenderItem(Graphics graphics, SimpleItem item, Rectangle rectangle, RadioItemState state)
        {
            if (graphics == null || item == null) return;

            ResolveTokens();
            var t = _tokens;

            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            DrawBackground(graphics, rectangle, state, t);

            var contentArea = GetContentArea(rectangle);
            var selectorArea = GetSelectorArea(rectangle);

            DrawSelector(graphics, selectorArea, state, t);
            DrawContent(graphics, item, contentArea, selectorArea, state, t);

            if (state.IsError) DrawErrorOverlay(graphics, rectangle, CornerRadius);
            if (state.IsPressed) DrawPressedOverlay(graphics, rectangle, CornerRadius);
        }

        private void DrawBackground(Graphics graphics, Rectangle rectangle, RadioItemState state, RadioGroupColorTokens t)
        {
            Color backgroundColor = Color.Transparent;

            if (!state.IsEnabled)
            {
                backgroundColor = t.DisabledContainer;
            }
            else if (state.IsSelected)
            {
                backgroundColor = t.PrimaryContainer;
            }
            else if (state.IsPressed)
            {
                backgroundColor = t.PressStateLayer;
            }
            else if (state.IsHovered)
            {
                backgroundColor = t.HoverStateLayer;
            }

            if (backgroundColor != Color.Transparent)
            {
                // Animate press-state alpha so the layer fades in over AnimationProgress.
                if (state.IsPressed && state.AnimationProgress < 1f)
                {
                    byte alpha = (byte)(backgroundColor.A * Math.Clamp(state.AnimationProgress, 0f, 1f));
                    backgroundColor = Color.FromArgb(alpha, backgroundColor);
                }
                using (var brush = new SolidBrush(backgroundColor))
                using (var path = CreateRoundedRectanglePath(rectangle, S(CornerRadius)))
                {
                    graphics.FillPath(brush, path);
                }
            }
        }

        private void DrawSelector(Graphics graphics, Rectangle selectorArea, RadioItemState state, RadioGroupColorTokens t)
        {
            var center = new Point(selectorArea.X + selectorArea.Width / 2, selectorArea.Y + selectorArea.Height / 2);

            if (AllowMultipleSelection)
            {
                DrawCheckbox(graphics, selectorArea, state, t);
            }
            else
            {
                DrawRadio(graphics, center, state, t);
            }
        }

        private void DrawRadio(Graphics graphics, Point center, RadioItemState state, RadioGroupColorTokens t)
        {
            int radius = Math.Max(2, S(SelectorSize) / 2 - S(1));

            var borderColor = !state.IsEnabled
                ? t.Disabled
                : (state.IsSelected ? t.Primary : t.Outline);
            var fillColor = !state.IsEnabled
                ? t.DisabledContainer
                : (state.IsSelected ? t.Primary : Color.Transparent);

            var outerRect = new Rectangle(center.X - radius, center.Y - radius, radius * 2, radius * 2);

            if (state.IsSelected)
            {
                using (var brush = new SolidBrush(fillColor))
                {
                    graphics.FillEllipse(brush, outerRect);
                }
            }

            using (var pen = new Pen(borderColor, state.IsSelected ? SF(2f) : SF(1.5f)))
            {
                graphics.DrawEllipse(pen, outerRect);
            }

            if (state.IsSelected)
            {
                int innerRadius = Math.Max(2, radius - S(6));
                var innerRect = new Rectangle(center.X - innerRadius, center.Y - innerRadius, innerRadius * 2, innerRadius * 2);
                using (var brush = new SolidBrush(state.IsEnabled ? t.OnPrimary : t.Disabled))
                {
                    graphics.FillEllipse(brush, innerRect);
                }
            }
        }

        private void DrawCheckbox(Graphics graphics, Rectangle selectorArea, RadioItemState state, RadioGroupColorTokens t)
        {
            var checkboxRect = new Rectangle(
                selectorArea.X + S(2), selectorArea.Y + S(2),
                selectorArea.Width - S(4), selectorArea.Height - S(4));

            if (state.IsSelected)
            {
                using (var brush = new SolidBrush(state.IsEnabled ? t.Primary : t.Disabled))
                using (var path = CreateRoundedRectanglePath(checkboxRect, S(3)))
                {
                    graphics.FillPath(brush, path);
                }
                DrawCheckmark(graphics, checkboxRect, t.OnPrimary);
            }
            else
            {
                using (var pen = new Pen(state.IsEnabled ? t.Outline : t.Disabled, SF(1.5f)))
                using (var path = CreateRoundedRectanglePath(checkboxRect, S(3)))
                {
                    graphics.DrawPath(pen, path);
                }
            }
        }

        private static void DrawCheckmark(Graphics graphics, Rectangle rect, Color color)
        {
            using (var pen = new Pen(color, 2f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                var points = new[]
                {
                    new PointF(rect.X + 4, rect.Y + rect.Height / 2),
                    new PointF(rect.X + rect.Width / 2 - 1, rect.Y + rect.Height - 6),
                    new PointF(rect.X + rect.Width - 4, rect.Y + 4)
                };
                graphics.DrawLines(pen, points);
            }
        }

        private void DrawContent(Graphics graphics, SimpleItem item, Rectangle contentArea, Rectangle selectorArea, RadioItemState state, RadioGroupColorTokens t)
        {
            int currentX = Math.Max(selectorArea.Right + ComponentSpacing, contentArea.Left);

            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                int sz = Math.Min(IconSize, Math.Max(12, contentArea.Height - 6));
                var iconRect = new Rectangle(currentX, contentArea.Y + (contentArea.Height - sz) / 2, sz, sz);
                var iconPath = RadioGroupIconHelpers.GetItemIconPath(item.ImagePath);
                var iconColor = state.IsEnabled ? (state.IsSelected ? t.Primary : t.OnSurfaceVariant) : t.Disabled;
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
                using var brush = new SolidBrush(state.IsEnabled ? t.OnSurface : t.Disabled);
                var fmt = new System.Drawing.StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter
                };
                graphics.DrawString(item.Text, _textFont, brush, textRect, fmt);
            }

            if (hasSubtitle)
            {
                using var subtitleFont = GetSubtextFont();
                int textWidth = Math.Max(0, contentArea.Right - currentX);
                var titleSize = TextUtils.MeasureText(graphics, item.Text ?? string.Empty, _textFont);
                int titleHeight = Math.Max(1, Math.Min(contentArea.Height - 1, (int)Math.Ceiling(titleSize.Height)));
                int subtitleHeight = Math.Max(0, contentArea.Height - titleHeight);
                var subtitleRect = new Rectangle(currentX, contentArea.Y + titleHeight, textWidth, subtitleHeight);
                using var brush = new SolidBrush(state.IsEnabled ? t.OnSurfaceVariant : t.Disabled);
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
            width += S(SelectorSize) + S(ComponentSpacing);

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
                var titleSize = TextUtils.MeasureText(graphics, item.Text ?? string.Empty, _textFont);
                height = Math.Max(height, (int)Math.Ceiling(titleSize.Height + subSize.Height) + S(ItemPadding));
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
            int selectorSize = Math.Min(S(SelectorSize), Math.Max(0, contentArea.Height));
            return new Rectangle(
                contentArea.X,
                contentArea.Y + (contentArea.Height - selectorSize) / 2,
                selectorSize, selectorSize);
        }
    }
}
