using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Enums;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models
{
    public sealed class AdvancedButtonLayoutSlices
    {
        public Rectangle ContentBounds { get; init; }
        public Rectangle PrimaryIconBounds { get; init; }
        public Rectangle SecondaryIconBounds { get; init; }
        public Rectangle TextBounds { get; init; }
    }

    public sealed class AdvancedButtonPaintTokens
    {
        public int BorderRadius { get; init; }
        public int BorderThickness { get; init; }
        public int FocusRingThickness { get; init; }
        public int FocusRingOffset { get; init; }
        public int FocusRingRadiusDelta { get; init; }
        public int IconSize { get; init; }
        public int HorizontalPadding { get; init; }
        public int VerticalPadding { get; init; }
        public int IconGap { get; init; }
        public AdvancedButtonMetrics Metrics { get; init; } = new();
    }

    public static class AdvancedButtonPaintContract
    {
        public static AdvancedButtonPaintTokens CreateTokens(AdvancedButtonPaintContext context)
        {
            var metrics = AdvancedButtonMetrics.GetMetrics(context.ButtonSize, context.OwnerControl);
            return new AdvancedButtonPaintTokens
            {
                Metrics = metrics,
                BorderRadius = ResolveRadius(context, metrics.BorderRadius),
                BorderThickness = Math.Max(1, context.BorderThickness),
                FocusRingThickness = Math.Max(1, Scale(context.OwnerControl, context.FocusRingThickness)),
                FocusRingOffset = Math.Max(0, Scale(context.OwnerControl, context.FocusRingOffset)),
                FocusRingRadiusDelta = Math.Max(0, Scale(context.OwnerControl, context.FocusRingRadiusDelta)),
                IconSize = metrics.IconSize,
                HorizontalPadding = metrics.PaddingHorizontal,
                VerticalPadding = metrics.PaddingVertical,
                IconGap = metrics.IconTextGap
            };
        }

        public static AdvancedButtonLayoutSlices CreateSlices(AdvancedButtonPaintContext context, AdvancedButtonPaintTokens tokens)
        {
            Rectangle bounds = context.Bounds;
            if (bounds.Width <= 0 || bounds.Height <= 0)
            {
                return new AdvancedButtonLayoutSlices();
            }

            int insetX = Math.Max(0, tokens.HorizontalPadding);
            int insetY = Math.Max(0, tokens.VerticalPadding / 2);
            var content = Rectangle.Inflate(bounds, -insetX, -insetY);
            if (content.Width <= 0 || content.Height <= 0)
            {
                content = bounds;
            }

            var primaryIcon = Rectangle.Empty;
            var secondaryIcon = Rectangle.Empty;
            var textBounds = content;

            bool hasLeftIcon = !string.IsNullOrWhiteSpace(context.ImagePath) || !string.IsNullOrWhiteSpace(context.IconLeft);
            bool hasRightIcon = !string.IsNullOrWhiteSpace(context.IconRight);

            if (hasLeftIcon)
            {
                primaryIcon = new Rectangle(content.Left, content.Top + (content.Height - tokens.IconSize) / 2, tokens.IconSize, tokens.IconSize);
                textBounds.X += tokens.IconSize + tokens.IconGap;
                textBounds.Width = Math.Max(0, textBounds.Width - (tokens.IconSize + tokens.IconGap));
            }

            if (hasRightIcon)
            {
                secondaryIcon = new Rectangle(content.Right - tokens.IconSize, content.Top + (content.Height - tokens.IconSize) / 2, tokens.IconSize, tokens.IconSize);
                textBounds.Width = Math.Max(0, textBounds.Width - (tokens.IconSize + tokens.IconGap));
            }

            return new AdvancedButtonLayoutSlices
            {
                ContentBounds = content,
                PrimaryIconBounds = primaryIcon,
                SecondaryIconBounds = secondaryIcon,
                TextBounds = textBounds
            };
        }

        private static int ResolveRadius(AdvancedButtonPaintContext context, int fallback)
        {
            if (context.Shape == ButtonShape.Pill)
            {
                return Math.Max(fallback, context.Bounds.Height / 2);
            }

            if (context.Shape == ButtonShape.Circle)
            {
                return Math.Max(fallback, Math.Min(context.Bounds.Width, context.Bounds.Height) / 2);
            }

            return context.BorderRadius > 0 ? context.BorderRadius : fallback;
        }

        private static int Scale(Control? owner, int value)
        {
            if (owner == null)
            {
                return value;
            }

            return DpiScalingHelper.ScaleValue(value, owner);
        }
    }
}
