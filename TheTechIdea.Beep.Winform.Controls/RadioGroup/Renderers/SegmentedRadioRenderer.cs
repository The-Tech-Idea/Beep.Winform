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
    /// iOS-style segmented control renderer. Distinct visual identity: connected
    /// segments inside a rounded track, 1px dividers, 2px inset selected pill with
    /// shadow. Single-selection only (segmented controls are exclusive by definition).
    /// </summary>
    public class SegmentedRadioRenderer : BaseRadioRenderer
    {
        private const int MinSegmentHeight = 36;
        private const int MinSegmentWidth = 60;
        private const int SegmentPadding = 12;
        private const int ComponentSpacing = 6;
        private const int CornerRadius = 8;
        private int IconSize => Math.Min(MaxImageSize.Width, MaxImageSize.Height);

        public override string StyleName => "Segmented";
        public override string DisplayName => "iOS Segmented Control";
        public override bool SupportsMultipleSelection => false;
        public override BeepControlStyle ControlStyle { get; set; } = BeepControlStyle.iOS15;

        public override void RenderItem(Graphics graphics, SimpleItem item, Rectangle rectangle, RadioItemState state)
        {
            if (graphics == null || item == null) return;
            ResolveTokens();
            var t = _tokens;

            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            if (!state.IsEnabled) DrawDisabledSegment(graphics, rectangle, t);
            else if (state.IsSelected) DrawSelectedSegment(graphics, rectangle, t, state);
            else if (state.IsHovered) DrawHoveredSegment(graphics, rectangle, t, state);

            DrawContent(graphics, item, rectangle, state, t);

            if (state.IsError) DrawErrorOverlay(graphics, rectangle, CornerRadius);
        }

        private void DrawSelectedSegment(Graphics graphics, Rectangle rect, RadioGroupColorTokens t, RadioItemState state)
        {
            // AnimationProgress grows the selected pill (initially a thin sliver at rect.X,
            // ending at the full inset rect). Combined with a fading shadow, this produces
            // a smooth slide-in transition when an item becomes selected.
            float anim = Math.Clamp(state.AnimationProgress, 0f, 1f);
            int inset = 2;
            int fullWidth = Math.Max(0, rect.Width - inset * 2);
            int animatedWidth = Math.Max(1, (int)Math.Round(fullWidth * anim));
            var segmentRect = new Rectangle(rect.X + inset, rect.Y + inset, animatedWidth, Math.Max(0, rect.Height - inset * 2));

            // Shadow fades in from 0 to 30/255 over the animation.
            int shadowAlpha = (int)Math.Round(30 * anim);
            using (var shadowPath = CreateRoundedRectanglePath(
                new Rectangle(segmentRect.X + 1, segmentRect.Y + 1, segmentRect.Width, segmentRect.Height), S(CornerRadius - 2)))
            using (var shadowBrush = new SolidBrush(Color.FromArgb(shadowAlpha, 0, 0, 0)))
            {
                graphics.FillPath(shadowBrush, shadowPath);
            }

            using (var path = CreateRoundedRectanglePath(segmentRect, S(CornerRadius - 2)))
            using (var brush = new SolidBrush(t.SurfaceContainer))
            {
                graphics.FillPath(brush, path);
            }
        }

        private void DrawHoveredSegment(Graphics graphics, Rectangle rect, RadioGroupColorTokens t, RadioItemState state)
        {
            // Hover state layer alpha fades in over AnimationProgress so the hover-in
            // feels smooth instead of snapping.
            float anim = Math.Clamp(state.AnimationProgress, 0f, 1f);
            byte alpha = (byte)Math.Min(255, t.HoverStateLayer.A * anim);
            using (var brush = new SolidBrush(Color.FromArgb(alpha, t.HoverStateLayer)))
            {
                graphics.FillRectangle(brush, rect);
            }
        }

        private void DrawDisabledSegment(Graphics graphics, Rectangle rect, RadioGroupColorTokens t)
        {
            using (var brush = new SolidBrush(t.DisabledContainer))
            {
                graphics.FillRectangle(brush, rect);
            }
        }

        private void DrawContent(Graphics graphics, SimpleItem item, Rectangle rect, RadioItemState state, RadioGroupColorTokens t)
        {
            int currentX = rect.X + S(SegmentPadding);
            int rightCap = rect.Right - S(SegmentPadding);

            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                int sz = Math.Min(IconSize, Math.Max(10, rect.Height - 12));
                var iconRect = new Rectangle(currentX, rect.Y + (rect.Height - sz) / 2, sz, sz);
                var iconPath = RadioGroupIconHelpers.GetItemIconPath(item.ImagePath);
                var iconColor = state.IsEnabled ? (state.IsSelected ? t.Primary : t.OnSurface) : t.Disabled;
                RadioGroupIconHelpers.PaintIcon(graphics, iconRect, iconPath, iconColor, _theme, UseThemeColors, ControlStyle);
                currentX += sz + S(ComponentSpacing);
            }

            if (!string.IsNullOrEmpty(item.Text))
            {
                var textRect = new Rectangle(currentX, rect.Y, Math.Max(0, rightCap - currentX), rect.Height);
                using (var brush = new SolidBrush(state.IsEnabled
                    ? (state.IsSelected ? t.OnSurface : t.OnSurfaceVariant)
                    : t.Disabled))
                {
                    var fmt = new System.Drawing.StringFormat
                    {
                        Alignment = StringAlignment.Center,
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
                int titleHeight = Math.Max(1, Math.Min(rect.Height - 1, (int)Math.Ceiling(titleSize.Height)));
                int subtitleHeight = Math.Max(0, rect.Height - titleHeight);
                var subRect = new Rectangle(currentX, rect.Y + titleHeight, textWidth, subtitleHeight);
                using var subBrush = new SolidBrush(t.OnSurfaceVariant);
                var fmt = new System.Drawing.StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter
                };
                graphics.DrawString(item.SubText, subtitleFont, subBrush, subRect, fmt);
            }
        }

        public override Size MeasureItem(SimpleItem item, Graphics graphics)
        {
            if (item == null) return new Size(MinSegmentWidth, MinSegmentHeight);
            int width = S(SegmentPadding) * 2;
            int height = S(MinSegmentHeight);

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
            return new Size(Math.Max(S(MinSegmentWidth), width), height);
        }

        public override Rectangle GetContentArea(Rectangle itemRectangle) => itemRectangle;
        public override Rectangle GetSelectorArea(Rectangle itemRectangle) => itemRectangle;
    }
}
