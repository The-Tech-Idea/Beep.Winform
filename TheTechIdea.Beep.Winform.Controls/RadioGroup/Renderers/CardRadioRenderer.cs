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
    /// Card-style renderer with elevation. Distinct visual identity: large rounded card
    /// (8px corner), 1–2px border, soft elevation shadow on hover/select, gradient
    /// background when selected.
    /// </summary>
    public class CardRadioRenderer : BaseRadioRenderer
    {
        private const int SelectorSize = 18;
        private const int MinItemHeight = 48;
        private const int ItemPadding = 16;
        private const int ComponentSpacing = 12;
        private const int CornerRadius = 8;
        private int IconSize => Math.Min(MaxImageSize.Width, MaxImageSize.Height);

        public override string StyleName => "Card";
        public override string DisplayName => "Material Card";
        public override bool SupportsMultipleSelection => true;
        public override BeepControlStyle ControlStyle { get; set; } = BeepControlStyle.TailwindCard;

        public override void RenderItem(Graphics graphics, SimpleItem item, Rectangle rectangle, RadioItemState state)
        {
            if (graphics == null || item == null) return;
            ResolveTokens();
            var t = _tokens;

            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            if (state.IsEnabled && (state.IsHovered || state.IsSelected))
            {
                DrawCardShadow(graphics, rectangle, state);
            }

            DrawCardBackground(graphics, rectangle, state, t);
            DrawCardBorder(graphics, rectangle, state, t);
            DrawContent(graphics, item, rectangle, GetSelectorArea(rectangle), state, t);
            DrawCardSelector(graphics, rectangle, state, t);

            if (state.IsError) DrawErrorOverlay(graphics, rectangle, CornerRadius);
            if (state.IsPressed) DrawPressedOverlay(graphics, rectangle, CornerRadius);
        }

        private void DrawCardShadow(Graphics graphics, Rectangle cardRect, RadioItemState state)
        {
            // Selection elevates the card (3 shadow layers, max intensity).
            // AnimationProgress interpolates between the resting shadow and the elevated
            // shadow as the user hovers/selects.
            float anim = Math.Clamp(state.AnimationProgress, 0f, 1f);
            int restingLayers = 1;
            int restingIntensity = 2;
            int elevatedLayers = 3;
            int elevatedIntensity = 8;
            int shadowLayers = (int)Math.Round(restingLayers + (elevatedLayers - restingLayers) * anim);
            int intensity = (int)Math.Round(restingIntensity + (elevatedIntensity - restingIntensity) * anim);

            for (int i = 0; i < shadowLayers; i++)
            {
                var shadowRect = new Rectangle(cardRect.X + i, cardRect.Y + i, cardRect.Width, cardRect.Height);
                using (var brush = new SolidBrush(Color.FromArgb(Math.Max(0, intensity - i * 2), Color.Black)))
                using (var path = CreateRoundedRectanglePath(shadowRect, S(CornerRadius)))
                {
                    graphics.FillPath(brush, path);
                }
            }
        }

        private void DrawCardBackground(Graphics graphics, Rectangle cardRect, RadioItemState state, RadioGroupColorTokens t)
        {
            Color backgroundColor;
            if (!state.IsEnabled) backgroundColor = t.DisabledContainer;
            else if (state.IsSelected) backgroundColor = t.SurfaceContainer;
            else if (state.IsPressed) backgroundColor = t.PressStateLayer;
            else if (state.IsHovered) backgroundColor = t.HoverStateLayer;
            else backgroundColor = t.Surface;

            // Hover/press state layer alpha fades in over AnimationProgress so the
            // hover-in feels smooth rather than popping in.
            if (state.IsEnabled && (state.IsHovered || state.IsPressed) && !state.IsSelected)
            {
                float anim = Math.Clamp(state.AnimationProgress, 0f, 1f);
                byte alpha = (byte)Math.Min(255, backgroundColor.A * anim);
                backgroundColor = Color.FromArgb(alpha, backgroundColor);
            }

            if (state.IsSelected)
            {
                var darkStop = Color.FromArgb(backgroundColor.A,
                    Math.Max(0, backgroundColor.R - 20),
                    Math.Max(0, backgroundColor.G - 20),
                    Math.Max(0, backgroundColor.B - 20));
                using (var brush = new LinearGradientBrush(cardRect, backgroundColor, darkStop, LinearGradientMode.Vertical))
                using (var path = CreateRoundedRectanglePath(cardRect, S(CornerRadius)))
                {
                    graphics.FillPath(brush, path);
                }
            }
            else
            {
                using (var brush = new SolidBrush(backgroundColor))
                using (var path = CreateRoundedRectanglePath(cardRect, S(CornerRadius)))
                {
                    graphics.FillPath(brush, path);
                }
            }
        }

        private void DrawCardBorder(Graphics graphics, Rectangle cardRect, RadioItemState state, RadioGroupColorTokens t)
        {
            Color borderColor;
            float borderWidth;
            if (!state.IsEnabled) { borderColor = t.OutlineVariant; borderWidth = SF(1f); }
            else if (state.IsSelected) { borderColor = t.Primary; borderWidth = SF(2f); }
            else if (state.IsFocused) { borderColor = t.Primary; borderWidth = SF(2f); }
            else if (state.IsHovered) { borderColor = t.Outline; borderWidth = SF(1.5f); }
            else { borderColor = t.OutlineVariant; borderWidth = SF(1f); }

            using (var pen = new Pen(borderColor, borderWidth))
            using (var path = CreateRoundedRectanglePath(cardRect, S(CornerRadius)))
            {
                graphics.DrawPath(pen, path);
            }
        }

        private void DrawContent(Graphics graphics, SimpleItem item, Rectangle contentArea, Rectangle selectorArea, RadioItemState state, RadioGroupColorTokens t)
        {
            int left = contentArea.X + S(ItemPadding);
            int rightCap = Math.Max(left, selectorArea.X - S(ComponentSpacing));
            int currentX = left;

            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                int sz = Math.Min(IconSize, Math.Max(12, contentArea.Height - 6));
                var iconRect = new Rectangle(currentX, contentArea.Y + (contentArea.Height - sz) / 2, sz, sz);
                var iconPath = RadioGroupIconHelpers.GetItemIconPath(item.ImagePath);
                var iconColor = state.IsEnabled ? (state.IsSelected ? t.Primary : t.OnSurfaceVariant) : t.Disabled;
                RadioGroupIconHelpers.PaintIcon(graphics, iconRect, iconPath, iconColor, _theme, UseThemeColors, ControlStyle);
                currentX += sz + S(ComponentSpacing);
            }

            bool hasSubtitle = !string.IsNullOrEmpty(item.SubText);
            if (!string.IsNullOrEmpty(item.Text))
            {
                int titleHeight = contentArea.Height;
                if (hasSubtitle)
                {
                    var titleSize = TextUtils.MeasureText(graphics, item.Text, _textFont);
                    titleHeight = Math.Max(1, Math.Min(contentArea.Height - 1, (int)Math.Ceiling(titleSize.Height)));
                }
                var textRect = new Rectangle(currentX, contentArea.Y, Math.Max(0, rightCap - currentX), titleHeight);
                using var brush = new SolidBrush(state.IsEnabled ? t.OnSurface : t.Disabled);
                var fmt = new System.Drawing.StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter };
                graphics.DrawString(item.Text, _textFont, brush, textRect, fmt);
            }

            if (hasSubtitle)
            {
                using var subtitleFont = GetSubtextFont();
                var titleSize = TextUtils.MeasureText(graphics, item.Text ?? string.Empty, _textFont);
                int titleHeight = Math.Max(1, Math.Min(contentArea.Height - 1, (int)Math.Ceiling(titleSize.Height)));
                int subtitleHeight = Math.Max(0, contentArea.Height - titleHeight);
                var subRect = new Rectangle(currentX, contentArea.Y + titleHeight, Math.Max(0, rightCap - currentX), subtitleHeight);
                using var subBrush = new SolidBrush(t.OnSurfaceVariant);
                var fmt = new System.Drawing.StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter };
                graphics.DrawString(item.SubText, subtitleFont, subBrush, subRect, fmt);
            }
        }

        private void DrawCardSelector(Graphics graphics, Rectangle cardRect, RadioItemState state, RadioGroupColorTokens t)
        {
            int selectorSize = Math.Min(S(SelectorSize), Math.Max(0, cardRect.Height - (S(ItemPadding) * 2)));
            if (selectorSize <= 0) return;

            var selectorRect = new Rectangle(
                Math.Max(cardRect.X + S(ItemPadding), cardRect.Right - S(ItemPadding) - selectorSize),
                cardRect.Y + (cardRect.Height - selectorSize) / 2,
                selectorSize, selectorSize);

            var center = new Point(selectorRect.X + selectorRect.Width / 2, selectorRect.Y + selectorRect.Height / 2);

            if (AllowMultipleSelection) DrawCheckbox(graphics, selectorRect, state, t);
            else DrawRadio(graphics, center, state, t);
        }

        private void DrawRadio(Graphics graphics, Point center, RadioItemState state, RadioGroupColorTokens t)
        {
            int radius = Math.Max(2, S(SelectorSize) / 2 - S(1));
            var borderColor = !state.IsEnabled ? t.Disabled : (state.IsSelected ? t.Primary : t.Outline);
            using (var pen = new Pen(borderColor, SF(2f)))
            {
                var outerRect = new Rectangle(center.X - radius, center.Y - radius, radius * 2, radius * 2);
                graphics.DrawEllipse(pen, outerRect);
            }
            if (state.IsSelected)
            {
                // Inner dot scales 0 → full radius via AnimationProgress.
                float anim = Math.Clamp(state.AnimationProgress, 0f, 1f);
                int fillRadius = Math.Max(1, (int)Math.Round((radius - S(4)) * anim));
                if (fillRadius > 0)
                {
                    var innerRect = new Rectangle(center.X - fillRadius, center.Y - fillRadius, fillRadius * 2, fillRadius * 2);
                    using (var brush = new SolidBrush(state.IsEnabled ? t.Primary : t.Disabled))
                    {
                        graphics.FillEllipse(brush, innerRect);
                    }
                }
            }
        }

        private void DrawCheckbox(Graphics graphics, Rectangle selectorRect, RadioItemState state, RadioGroupColorTokens t)
        {
            var checkboxRect = new Rectangle(selectorRect.X + S(2), selectorRect.Y + S(2), selectorRect.Width - S(4), selectorRect.Height - S(4));
            if (state.IsSelected)
            {
                using (var brush = new SolidBrush(state.IsEnabled ? t.Primary : t.Disabled))
                using (var path = CreateRoundedRectanglePath(checkboxRect, S(3)))
                {
                    graphics.FillPath(brush, path);
                }
                // Checkmark stroke grows segment by segment over AnimationProgress.
                float anim = Math.Clamp(state.AnimationProgress, 0f, 1f);
                if (anim > 0.05f)
                {
                    DrawCheckmark(graphics, checkboxRect, t.OnPrimary, anim);
                }
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

        private static void DrawCheckmark(Graphics graphics, Rectangle rect, Color color, float anim = 1f)
        {
            anim = Math.Clamp(anim, 0f, 1f);
            using (var pen = new Pen(color, 2f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                var p0 = new PointF(rect.X + rect.Width * 0.2f, rect.Y + rect.Height * 0.5f);
                var p1 = new PointF(rect.X + rect.Width * 0.45f, rect.Y + rect.Height * 0.7f);
                var p2 = new PointF(rect.X + rect.Width * 0.8f, rect.Y + rect.Height * 0.3f);
                if (anim <= 0.5f)
                {
                    var end = new PointF(
                        p0.X + (p1.X - p0.X) * (anim / 0.5f),
                        p0.Y + (p1.Y - p0.Y) * (anim / 0.5f));
                    graphics.DrawLine(pen, p0, end);
                }
                else
                {
                    graphics.DrawLine(pen, p0, p1);
                    var t = (anim - 0.5f) / 0.5f;
                    var end = new PointF(
                        p1.X + (p2.X - p1.X) * t,
                        p1.Y + (p2.Y - p1.Y) * t);
                    graphics.DrawLine(pen, p1, end);
                }
            }
        }

        public override Size MeasureItem(SimpleItem item, Graphics graphics)
        {
            if (item == null) return new Size(200, MinItemHeight);
            int width = S(ItemPadding) * 2 + S(SelectorSize) + S(ComponentSpacing);
            int height = S(MinItemHeight);

            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                width += IconSize + S(ComponentSpacing);
                height = Math.Max(height, IconSize + S(ItemPadding) * 2);
            }
            if (!string.IsNullOrEmpty(item.Text))
            {
                var textSize = TextUtils.MeasureText(graphics, item.Text, _textFont);
                width += (int)Math.Ceiling(textSize.Width);
                height = Math.Max(height, (int)Math.Ceiling(textSize.Height) + S(ItemPadding) * 2);
            }
            if (!string.IsNullOrEmpty(item.SubText))
            {
                using var subFont = GetSubtextFont();
                var subSize = TextUtils.MeasureText(graphics, item.SubText, subFont);
                height = Math.Max(height, (int)Math.Ceiling(subSize.Height) + S(ItemPadding) * 3);
            }
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
            int selectorSize = S(SelectorSize);
            return new Rectangle(
                itemRectangle.Right - S(ItemPadding) - selectorSize,
                itemRectangle.Y + (itemRectangle.Height - selectorSize) / 2,
                selectorSize, selectorSize);
        }
    }
}
