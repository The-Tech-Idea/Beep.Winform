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
    /// Material Design 3 radio group renderer. Distinct visual identity:
    /// 40dp state layer with 8% hover / 12% focus / 12% press overlays,
    /// 20dp radio / 18dp checkbox indicator, 1px outline ring.
    /// </summary>
    public sealed class MaterialRadioRenderer : BaseRadioRenderer
    {
        private const int RadioSize = 20;
        private const int MinItemHeight = 40;
        private const int ItemPadding = 16;
        private const int ComponentSpacing = 12;
        private const int StateLayerSize = 40;
        private int IconSize => Math.Min(MaxImageSize.Width, MaxImageSize.Height);

        public override string StyleName => "Material";
        public override string DisplayName => "Material Design 3";
        public override bool SupportsMultipleSelection => true;
        public override BeepControlStyle ControlStyle { get; set; } = BeepControlStyle.Material3;

        public override void RenderItem(Graphics graphics, SimpleItem item, Rectangle rectangle, RadioItemState state)
        {
            if (graphics == null || item == null) return;

            ResolveTokens();
            var t = _tokens;

            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // State layer for hover / focus. Alpha fades in via AnimationProgress
            // so the hover-in feels smooth rather than popping in.
            if (state.IsEnabled && (state.IsHovered || state.IsFocused))
            {
                Color layerColor = state.IsFocused ? t.FocusStateLayer : t.HoverStateLayer;
                float anim = Math.Clamp(state.AnimationProgress, 0f, 1f);
                byte alpha = (byte)Math.Min(255, layerColor.A * anim);
                var fadeColor = Color.FromArgb(alpha, layerColor);
                using (var brush = new SolidBrush(fadeColor))
                using (var path = CreateRoundedRectanglePath(rectangle, S(4)))
                {
                    graphics.FillPath(brush, path);
                }
            }

            var contentArea = GetContentArea(rectangle);
            var radioArea = GetSelectorArea(rectangle);

            DrawIndicator(graphics, radioArea, state, t);
            DrawContent(graphics, item, contentArea, radioArea, state, t);

            if (state.IsEnabled && state.IsPressed)
            {
                // Press-state layer fades in over AnimationProgress (0 → 1 over ~140ms).
                byte alpha = (byte)(t.PressStateLayer.A * Math.Clamp(state.AnimationProgress, 0f, 1f));
                var fadeColor = Color.FromArgb(alpha, t.PressStateLayer);
                using (var brush = new SolidBrush(fadeColor))
                using (var path = CreateRoundedRectanglePath(rectangle, S(StateLayerSize)))
                {
                    graphics.FillPath(brush, path);
                }
            }

            if (state.IsError) DrawErrorOverlay(graphics, rectangle);
        }

        private void DrawIndicator(Graphics graphics, Rectangle radioArea, RadioItemState state, RadioGroupColorTokens t)
        {
            if (AllowMultipleSelection || state.Tag as string == "checkbox")
            {
                DrawCheckbox(graphics, radioArea, state, t);
            }
            else
            {
                DrawRadio(graphics, radioArea, state, t);
            }
        }

        private void DrawRadio(Graphics graphics, Rectangle radioArea, RadioItemState state, RadioGroupColorTokens t)
        {
            var center = new Point(radioArea.X + radioArea.Width / 2, radioArea.Y + radioArea.Height / 2);
            int outerRadius = S(RadioSize) / 2;
            int innerRadius = Math.Max(2, outerRadius - S(2));

            var borderColor = state.IsEnabled
                ? (state.IsSelected ? t.Primary : t.Outline)
                : t.Disabled;

            using (var pen = new Pen(borderColor, SF(2f)))
            {
                var outerRect = new Rectangle(center.X - outerRadius, center.Y - outerRadius, outerRadius * 2, outerRadius * 2);
                graphics.DrawEllipse(pen, outerRect);
            }

            if (state.IsSelected)
            {
                // Inner dot scales from 0 → full radius over AnimationProgress so the
                // selection state fades in smoothly.  The fill color is also a fade-in.
                float anim = Math.Clamp(state.AnimationProgress, 0f, 1f);
                int fillRadius = Math.Max(1, (int)Math.Round((innerRadius - S(4)) * anim));
                if (fillRadius > 0)
                {
                    var fillColor = state.IsEnabled ? t.Primary : t.Disabled;
                    using (var brush = new SolidBrush(fillColor))
                    {
                        var innerRect = new Rectangle(center.X - fillRadius, center.Y - fillRadius, fillRadius * 2, fillRadius * 2);
                        graphics.FillEllipse(brush, innerRect);
                    }
                }
            }
        }

        private void DrawCheckbox(Graphics graphics, Rectangle checkboxArea, RadioItemState state, RadioGroupColorTokens t)
        {
            var checkboxRect = new Rectangle(
                checkboxArea.X + S(2), checkboxArea.Y + S(2),
                checkboxArea.Width - S(4), checkboxArea.Height - S(4));

            if (state.IsSelected)
            {
                var fillColor = state.IsEnabled ? t.Primary : t.Disabled;
                using (var brush = new SolidBrush(fillColor))
                {
                    graphics.FillRectangle(brush, checkboxRect);
                }
                // Checkmark grows from a 0-length stroke to its full extent over
                // AnimationProgress so the check-in feels intentional.
                float anim = Math.Clamp(state.AnimationProgress, 0f, 1f);
                if (anim > 0.05f)
                {
                    DrawCheckmark(graphics, checkboxRect, t.OnPrimary, anim);
                }
            }
            else
            {
                using (var pen = new Pen(t.Outline, SF(2f)))
                {
                    graphics.DrawRectangle(pen, checkboxRect);
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
                // Two stroke segments: down-stroke (point 0 → 1) and up-stroke (point 1 → 2).
                // Split the animation budget 0.5 / 0.5 between the two segments.
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

        private void DrawContent(Graphics graphics, SimpleItem item, Rectangle contentArea, Rectangle radioArea, RadioItemState state, RadioGroupColorTokens t)
        {
            int currentX = radioArea.Right + ComponentSpacing;
            currentX = Math.Max(currentX, contentArea.Left);

            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                int sz = Math.Min(IconSize, Math.Max(12, contentArea.Height - 4));
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
            width += S(StateLayerSize) + S(ComponentSpacing);

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
                itemRectangle.X + Math.Max(S(4), S(ItemPadding) / 2),
                itemRectangle.Y + Math.Max(S(2), S(ItemPadding) / 2),
                Math.Max(0, itemRectangle.Width - Math.Max(S(8), S(ItemPadding))),
                Math.Max(0, itemRectangle.Height - Math.Max(S(4), S(ItemPadding))));
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
