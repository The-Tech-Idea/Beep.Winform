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
    /// Toggle-switch renderer. Distinct visual identity: track + thumb on the right edge,
    /// 40×20dp track with rounded ends, thumb slide animation.
    /// </summary>
    public sealed class ToggleRadioRenderer : BaseRadioRenderer
    {
        private const int HorizontalPadding = 12;
        private const int VerticalPadding = 6;
        private const int ComponentSpacing = 8;
        private const int TrackWidth = 40;
        private const int TrackHeight = 20;
        private int IconSize => Math.Min(MaxImageSize.Width, MaxImageSize.Height);

        public override string StyleName => "Toggle";
        public override string DisplayName => "Toggle Switch";
        public override bool SupportsMultipleSelection => true;
        public override BeepControlStyle ControlStyle { get; set; } = BeepControlStyle.iOS15;

        public override void RenderItem(Graphics g, SimpleItem item, Rectangle rect, RadioItemState state)
        {
            ResolveTokens();
            var t = _tokens;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var content = GetContentArea(rect);
            var toggle = GetSelectorArea(rect);

            int reserveRight = toggle.Width + ComponentSpacing;
            int availableRight = Math.Max(content.X, content.Right - reserveRight);
            int currentX = content.X;

            if (!string.IsNullOrEmpty(item.ImagePath) && currentX < availableRight)
            {
                int sz = Math.Min(IconSize, Math.Max(10, content.Height - 4));
                sz = Math.Min(sz, Math.Max(0, availableRight - currentX));
                if (sz > 0)
                {
                    var iconRect = new Rectangle(currentX, content.Y + (content.Height - sz) / 2, sz, sz);
                    var iconPath = RadioGroupIconHelpers.GetItemIconPath(item.ImagePath);
                    var iconColor = state.IsEnabled ? (state.IsSelected ? t.Primary : t.OnSurfaceVariant) : t.Disabled;
                    RadioGroupIconHelpers.PaintIcon(g, iconRect, iconPath, iconColor, _theme, UseThemeColors, ControlStyle);
                    currentX += sz + ComponentSpacing;
                }
            }

            if (!string.IsNullOrEmpty(item.Text) && currentX < availableRight)
            {
                using var br = new SolidBrush(state.IsEnabled ? t.OnSurface : t.Disabled);
                var fmt = new System.Drawing.StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter
                };
                g.DrawString(item.Text, _textFont, br, new Rectangle(currentX, content.Y, availableRight - currentX, content.Height), fmt);
            }

            if (!string.IsNullOrEmpty(item.SubText) && currentX < availableRight)
            {
                using var subFont = GetSubtextFont();
                using var subBr = new SolidBrush(t.OnSurfaceVariant);
                var fmt = new System.Drawing.StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Far,
                    Trimming = StringTrimming.EllipsisCharacter
                };
                g.DrawString(item.SubText, subFont, subBr, new Rectangle(currentX, content.Y, availableRight - currentX, content.Height), fmt);
            }

            int actualTrackWidth = Math.Max(8, Math.Min(TrackWidth, Math.Max(0, toggle.Width)));
            int actualTrackHeight = Math.Max(8, Math.Min(TrackHeight, Math.Max(0, toggle.Height)));
            var trackRect = new Rectangle(toggle.Right - actualTrackWidth, toggle.Y + (toggle.Height - actualTrackHeight) / 2, actualTrackWidth, actualTrackHeight);
            int radius = actualTrackHeight / 2;

            Color trackColor = !state.IsEnabled
                ? t.DisabledContainer
                : (state.IsSelected ? t.Primary : t.SurfaceVariant);

            // Lerp the track color from SurfaceVariant → Primary over AnimationProgress
            // so the track fills in alongside the thumb slide.
            if (state.IsEnabled && state.AnimationProgress > 0f && state.AnimationProgress < 1f)
            {
                float lerpT = state.IsSelected ? state.AnimationProgress : 1f - state.AnimationProgress;
                trackColor = LerpColor(t.SurfaceVariant, t.Primary, lerpT);
            }

            using (var path = CreateRoundedRectanglePath(trackRect, radius))
            using (var trackBr = new SolidBrush(trackColor))
            {
                g.FillPath(trackBr, path);
            }

            int thumb = Math.Max(4, actualTrackHeight - 4);
            int leftCx = trackRect.Left + (thumb / 2) + 2;
            int rightCx = trackRect.Right - (thumb / 2) - 2;
            int cx;
            // Animate the thumb in BOTH directions.  When AnimationProgress is mid-way
            // the thumb slides between left (unselected) and right (selected) regardless
            // of which target state is current, so deselect transitions are just as
            // smooth as select transitions.
            if (state.AnimationProgress > 0f && state.AnimationProgress < 1f)
            {
                float animT = state.AnimationProgress;
                float slideT = state.IsSelected ? animT : 1f - animT;
                cx = leftCx + (int)((rightCx - leftCx) * slideT);
            }
            else
            {
                cx = state.IsSelected ? rightCx : leftCx;
            }
            var thumbRect = new Rectangle(cx - thumb / 2, trackRect.Y + 2, thumb, thumb);
            using var thumbBr = new SolidBrush(state.IsEnabled ? t.OnPrimary : t.Disabled);
            g.FillEllipse(thumbBr, thumbRect);

            if (state.IsError) DrawErrorOverlay(g, rect, radius + 2);
        }

        public override Size MeasureItem(SimpleItem item, Graphics g)
        {
            int h = Math.Max(36, TrackHeight + VerticalPadding * 2);
            int w = HorizontalPadding * 2 + TrackWidth;

            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                w += IconSize + ComponentSpacing;
                h = Math.Max(h, IconSize + VerticalPadding * 2);
            }

            if (!string.IsNullOrEmpty(item.Text))
            {
                var s = TextUtils.MeasureText(g, item.Text, _textFont);
                w += (int)Math.Ceiling(s.Width) + ComponentSpacing;
                h = Math.Max(h, (int)Math.Ceiling(s.Height) + VerticalPadding * 2);
            }

            if (!string.IsNullOrEmpty(item.SubText))
            {
                using var subFont = GetSubtextFont();
                var s = TextUtils.MeasureText(g, item.SubText, subFont);
                h = Math.Max(h, (int)Math.Ceiling(s.Height) + VerticalPadding * 4);
            }

            return new Size(w, h);
        }

        public override Rectangle GetContentArea(Rectangle itemRectangle)
        {
            return new Rectangle(
                itemRectangle.X + HorizontalPadding,
                itemRectangle.Y + VerticalPadding,
                Math.Max(0, itemRectangle.Width - (HorizontalPadding * 2)),
                Math.Max(0, itemRectangle.Height - (VerticalPadding * 2)));
        }

        public override Rectangle GetSelectorArea(Rectangle itemRectangle)
        {
            var content = GetContentArea(itemRectangle);
            int selectorWidth = TrackWidth + HorizontalPadding;
            return new Rectangle(
                Math.Max(content.X, content.Right - selectorWidth),
                content.Y,
                Math.Min(selectorWidth, content.Width),
                content.Height);
        }
    }
}
