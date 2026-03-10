using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ProgressBars.Helpers;
using TheTechIdea.Beep.Winform.Controls.ProgressBars.Models;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.ProgressBars.Painters
{
    internal sealed class RingCenterImagePainter : IProgressPainter, IProgressPainterV2
    {
        public string Key => nameof(ProgressPainterKind.RingCenterImage);

        public void Paint(Graphics g, Rectangle bounds, IBeepTheme theme, BeepProgressBar owner, IReadOnlyDictionary<string, object> p)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int requestedThickness = ProgressBarDpiHelpers.Scale(owner, ProgressPainterParameterContracts.GetInt(p, "Thickness", Math.Max(6, bounds.Height / 8)));
            int thickness = ProgressRingVisualHelpers.GetClampedThickness(owner, bounds, requestedThickness);
            int pad = thickness / 2 + ProgressBarDpiHelpers.Scale(owner, 2);
            var ringRect = ProgressRingVisualHelpers.GetSquareRingRect(bounds, pad);
            int shadowOffset = ProgressRingVisualHelpers.GetShadowOffset(owner);
            int trackAlpha = ProgressRingVisualHelpers.GetTrackAlpha(p, owner.Enabled, 40, 24, 54, 34);
            using var backPen = new Pen(Color.FromArgb(trackAlpha, theme.CardTextForeColor), thickness) { StartCap = LineCap.Round, EndCap = LineCap.Round };
            var activeColor = theme.PrimaryColor.IsEmpty ? Color.SeaGreen : theme.PrimaryColor;
            if (!owner.Enabled)
            {
                activeColor = Color.FromArgb(ProgressRingVisualHelpers.GetDisabledAccentAlpha(p), activeColor);
            }
            using var forePen = new Pen(activeColor, thickness) { StartCap = LineCap.Round, EndCap = LineCap.Round };
            using var shadowPen = new Pen(Color.FromArgb(ProgressRingVisualHelpers.GetRingShadowAlpha(p, owner.Enabled), Color.Black), thickness) { StartCap = LineCap.Round, EndCap = LineCap.Round };
            if (ringRect.Width > 0 && ringRect.Height > 0)
            {
                g.DrawArc(shadowPen, ringRect.X + shadowOffset, ringRect.Y + shadowOffset, ringRect.Width, ringRect.Height, -90, 360);
            }
            g.DrawArc(backPen, ringRect, -90, 360);
            float pct = owner.DisplayProgressPercentageAccessor;
            g.DrawArc(forePen, ringRect, -90, 360f * pct);

            // center image and optional text
            string iconPath = ProgressPainterParameterContracts.GetString(p, "CenterIconPath", null);
            int iconSize = ProgressBarDpiHelpers.Scale(owner, ProgressPainterParameterContracts.GetInt(p, "CenterIconSize", Math.Max(16, ringRect.Height/3)));
            string txt = ProgressPainterParameterContracts.GetString(p, "CenterText", string.Empty);
            bool hasIcon = !string.IsNullOrEmpty(iconPath);
            bool hasText = !string.IsNullOrEmpty(txt);

            if (!string.IsNullOrEmpty(iconPath))
            {
                int topBandHeight = hasText ? (int)(ringRect.Height * 0.58f) : ringRect.Height;
                iconSize = Math.Min(iconSize, Math.Max(ProgressBarDpiHelpers.Scale(owner, 12), topBandHeight - ProgressBarDpiHelpers.Scale(owner, 4)));
                var iconRect = new Rectangle(
                    ringRect.X + (ringRect.Width - iconSize) / 2,
                    ringRect.Y + (topBandHeight - iconSize) / 2,
                    iconSize,
                    iconSize);
                
                // Use ProgressBarIconHelpers to paint icon with StyledImagePainter
                ProgressBarIconHelpers.PaintIcon(
                    g,
                    iconRect,
                    owner,
                    ProgressPainterKind.RingCenterImage,
                    iconPath,
                    theme,
                    owner.UseThemeColors,
                    owner.Style);
            }
            if (hasText)
            {
                using var f = ProgressBarFontHelpers.GetProgressBarLabelFont(owner, owner.ControlStyle, theme);
                using var br = new SolidBrush(owner.Enabled ? theme.CardTextForeColor : Color.FromArgb(ProgressRingVisualHelpers.GetDisabledTextAlpha(p), theme.CardTextForeColor));
                using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                Rectangle textRect = hasIcon
                    ? new Rectangle(ringRect.X, ringRect.Y + (int)(ringRect.Height * 0.58f), ringRect.Width, (int)(ringRect.Height * 0.42f))
                    : ringRect;
                g.DrawString(txt, f, br, textRect, sf);
            }
        }

        public void UpdateHitAreas(BeepProgressBar owner, Rectangle bounds, IBeepTheme theme, IReadOnlyDictionary<string, object> p, Action<string, Rectangle> register)
        {
            register("Ring", bounds);
        }

        public void Paint(Graphics g, ProgressPainterContext context, BeepProgressBar owner)
        {
            if (context == null)
            {
                return;
            }

            Paint(g, context.Bounds, context.Theme, owner, context.Parameters);
        }

        public void UpdateHitAreas(ProgressPainterContext context, BeepProgressBar owner, Action<string, Rectangle> register)
        {
            if (context == null)
            {
                return;
            }

            UpdateHitAreas(owner, context.Bounds, context.Theme, context.Parameters, register);
        }
    }
}
