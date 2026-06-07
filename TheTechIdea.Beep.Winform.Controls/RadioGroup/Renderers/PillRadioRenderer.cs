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
    /// Pill-shaped capsule renderer. Distinct visual identity: full radius
    /// (height/2), 16dp horizontal padding, brand-colored fill on selection.
    /// </summary>
    public class PillRadioRenderer : BaseRadioRenderer
    {
        private const int MinPillHeight = 36;
        private const int MinPillWidth = 80;
        private const int PillPadding = 16;
        private const int ComponentSpacing = 8;
        private int IconSize => Math.Min(MaxImageSize.Width, MaxImageSize.Height);

        public override string StyleName => "Pill";
        public override string DisplayName => "Pill Buttons";
        public override bool SupportsMultipleSelection => true;
        public override BeepControlStyle ControlStyle { get; set; } = BeepControlStyle.PillRail;

        public override void RenderItem(Graphics graphics, SimpleItem item, Rectangle rectangle, RadioItemState state)
        {
            if (graphics == null || item == null) return;
            ResolveTokens();
            var t = _tokens;

            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var pillRect = GetPillRectangle(rectangle);
            DrawPillBackground(graphics, pillRect, state, t);
            DrawPillBorder(graphics, pillRect, state, t);
            DrawContent(graphics, item, pillRect, state, t);

            if (state.IsError) DrawErrorOverlay(graphics, pillRect, pillRect.Height / 2);
            if (state.IsPressed) DrawPressedOverlay(graphics, pillRect, pillRect.Height / 2);
        }

        private Rectangle GetPillRectangle(Rectangle itemRectangle)
        {
            int h = Math.Max(S(MinPillHeight), itemRectangle.Height);
            int w = Math.Max(S(MinPillWidth), itemRectangle.Width);
            int y = itemRectangle.Y + (itemRectangle.Height - h) / 2;
            return new Rectangle(itemRectangle.X, y, w, h);
        }

        private void DrawPillBackground(Graphics graphics, Rectangle pillRect, RadioItemState state, RadioGroupColorTokens t)
        {
            Color backgroundColor;
            if (!state.IsEnabled) backgroundColor = t.DisabledContainer;
            else if (state.IsSelected) backgroundColor = t.Primary;
            else if (state.IsPressed) backgroundColor = t.PressStateLayer;
            else if (state.IsHovered) backgroundColor = t.HoverStateLayer;
            else backgroundColor = t.Surface;

            // Selected transition: fill sweeps in from the left over AnimationProgress.
            if (state.IsSelected && state.AnimationProgress < 1f)
            {
                int revealWidth = (int)(pillRect.Width * Math.Clamp(state.AnimationProgress, 0f, 1f));
                graphics.SetClip(pillRect);
                using (var brush = new SolidBrush(backgroundColor))
                {
                    graphics.FillRectangle(brush, pillRect.X, pillRect.Y, revealWidth, pillRect.Height);
                }
                graphics.ResetClip();
                return;
            }

            // Hover/press state layer alpha fades in over AnimationProgress.
            if (state.IsEnabled && (state.IsHovered || state.IsPressed) && !state.IsSelected)
            {
                float anim = Math.Clamp(state.AnimationProgress, 0f, 1f);
                byte alpha = (byte)Math.Min(255, backgroundColor.A * anim);
                backgroundColor = Color.FromArgb(alpha, backgroundColor);
            }

            int radius = pillRect.Height / 2;
            using (var brush = new SolidBrush(backgroundColor))
            using (var path = CreateRoundedRectanglePath(pillRect, radius))
            {
                graphics.FillPath(brush, path);
            }
        }

        private void DrawPillBorder(Graphics graphics, Rectangle pillRect, RadioItemState state, RadioGroupColorTokens t)
        {
            Color borderColor;
            float borderWidth;
            if (!state.IsEnabled) { borderColor = t.OutlineVariant; borderWidth = SF(1f); }
            else if (state.IsSelected) { borderColor = t.Primary; borderWidth = SF(1.5f); }
            else if (state.IsHovered) { borderColor = t.Outline; borderWidth = SF(1.5f); }
            else { borderColor = t.Outline; borderWidth = SF(1f); }

            int radius = pillRect.Height / 2;
            using (var pen = new Pen(borderColor, borderWidth))
            using (var path = CreateRoundedRectanglePath(pillRect, radius))
            {
                graphics.DrawPath(pen, path);
            }
        }

        private void DrawContent(Graphics graphics, SimpleItem item, Rectangle pillRect, RadioItemState state, RadioGroupColorTokens t)
        {
            int currentX = pillRect.X + S(PillPadding);
            int rightCap = pillRect.Right - S(PillPadding);

            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                int sz = Math.Min(IconSize, Math.Max(10, pillRect.Height - 8));
                var iconRect = new Rectangle(currentX, pillRect.Y + (pillRect.Height - sz) / 2, sz, sz);
                var iconPath = RadioGroupIconHelpers.GetItemIconPath(item.ImagePath);
                var iconColor = state.IsEnabled
                    ? (state.IsSelected ? t.OnPrimary : t.OnSurfaceVariant)
                    : t.Disabled;
                RadioGroupIconHelpers.PaintIcon(graphics, iconRect, iconPath, iconColor, _theme, UseThemeColors, ControlStyle);
                currentX += sz + S(ComponentSpacing);
            }

            if (!string.IsNullOrEmpty(item.Text))
            {
                var textRect = new Rectangle(currentX, pillRect.Y, Math.Max(0, rightCap - currentX), pillRect.Height);
                using (var brush = new SolidBrush(state.IsEnabled
                    ? (state.IsSelected ? t.OnPrimary : t.OnSurface)
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
                int titleHeight = Math.Max(1, Math.Min(pillRect.Height - 1, (int)Math.Ceiling(titleSize.Height)));
                int subtitleHeight = Math.Max(0, pillRect.Height - titleHeight);
                var subRect = new Rectangle(currentX, pillRect.Y + titleHeight, textWidth, subtitleHeight);
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
            if (item == null) return new Size(MinPillWidth, MinPillHeight);
            int width = S(PillPadding) * 2;
            int height = S(MinPillHeight);

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
            return new Size(Math.Max(S(MinPillWidth), width), height);
        }

        public override Rectangle GetContentArea(Rectangle itemRectangle)
        {
            var pillRect = GetPillRectangle(itemRectangle);
            return new Rectangle(pillRect.X + S(PillPadding), pillRect.Y,
                Math.Max(0, pillRect.Width - S(PillPadding) * 2), pillRect.Height);
        }

        public override Rectangle GetSelectorArea(Rectangle itemRectangle)
        {
            return GetPillRectangle(itemRectangle);
        }
    }
}
