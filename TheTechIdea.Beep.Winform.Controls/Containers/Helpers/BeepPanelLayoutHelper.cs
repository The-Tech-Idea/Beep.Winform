using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Containers.Models;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Containers.Helpers
{
    internal static class BeepPanelLayoutHelper
    {
        public static BeepPanelLayoutContext BuildLayout(BeepPanel owner, Rectangle bounds, BeepPanelState state, Font titleFont)
        {
            var context = new BeepPanelLayoutContext
            {
                OuterBounds = bounds,
                BorderBounds = bounds,
                ContentBounds = bounds
            };

            bool hasTitle = state.ShowTitle && !string.IsNullOrWhiteSpace(state.TitleText) && titleFont != null;
            context.HasTitle = hasTitle;
            if (!hasTitle)
            {
                context.ContentBounds = Rectangle.FromLTRB(
                    bounds.Left + state.Padding.Left,
                    bounds.Top + state.Padding.Top,
                    bounds.Right - state.Padding.Right,
                    bounds.Bottom - state.Padding.Bottom);
                return context;
            }

            Size titleSize = TextRenderer.MeasureText(state.TitleText, titleFont);
            context.TitleSize = titleSize;
            int gap = DpiScalingHelper.ScaleValue(state.TitleGap, owner);
            int titleHeight = titleSize.Height;
            int thickness = DpiScalingHelper.ScaleValue(state.TitleLineThickness, owner);
            bool hasIcon = state.ShowTitleIcon && !string.IsNullOrWhiteSpace(state.ResolvedIconPath);
            context.HasIcon = hasIcon;
            int iconSize = hasIcon ? DpiScalingHelper.ScaleValue(state.TitleIconSize, owner) : 0;
            int iconGap = hasIcon ? DpiScalingHelper.ScaleValue(state.TitleIconGap, owner) : 0;
            int clusterWidth = titleSize.Width + (hasIcon ? iconSize + iconGap : 0);
            int clusterHeight = System.Math.Max(titleHeight, iconSize);
            int headerHeight = clusterHeight + gap;
            int tabHeaderHeight = clusterHeight + (gap * 2); // tab-style header band with padding above/below
            // Allow title line in all horizontal title modes (including GroupBox).
            context.HasTitleLine = state.TitleStyle == PanelTitleStyle.Divider ||
                                   (state.ShowTitleLine &&
                                    state.TitleStyle != PanelTitleStyle.Left &&
                                    state.TitleStyle != PanelTitleStyle.Right);

            int titleX = bounds.Left + gap;
            int clusterX = titleX;
            switch (state.TitleAlignment)
            {
                case ContentAlignment.TopCenter:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.BottomCenter:
                    clusterX = bounds.Left + (bounds.Width - clusterWidth) / 2;
                    break;
                case ContentAlignment.TopRight:
                case ContentAlignment.MiddleRight:
                case ContentAlignment.BottomRight:
                    clusterX = bounds.Right - clusterWidth - gap;
                    break;
            }

            if (state.TitleStyle == PanelTitleStyle.Divider)
            {
                // Divider headings are always centered, like CSS "heading with rules".
                clusterX = bounds.Left + (bounds.Width - clusterWidth) / 2;
            }

            int clusterY = bounds.Top + DpiScalingHelper.ScaleValue(2, owner);
            if (state.TitleStyle == PanelTitleStyle.GroupBox)
            {
                // Keep title fully inside the control top band (fixes design-time clipping at top edge)
                clusterY = bounds.Top + DpiScalingHelper.ScaleValue(1, owner);
            }
            else if (state.TitleStyle == PanelTitleStyle.Below)
            {
                clusterY = bounds.Bottom - clusterHeight - gap;
            }
            else if (state.TitleStyle == PanelTitleStyle.Overlay || state.TitleStyle == PanelTitleStyle.Above ||
                     state.TitleStyle == PanelTitleStyle.Divider || state.TitleStyle == PanelTitleStyle.Badge)
            {
                clusterY = bounds.Top + gap / 2;
            }
            else if (state.TitleStyle == PanelTitleStyle.TopHeader)
            {
                // Tab-style header: title vertically centered in the header band.
                clusterY = bounds.Top + (tabHeaderHeight - clusterHeight) / 2;
            }
            else if (state.TitleStyle == PanelTitleStyle.Left || state.TitleStyle == PanelTitleStyle.Right)
            {
                clusterY = bounds.Top + (bounds.Height - clusterHeight) / 2;
            }

            titleX = clusterX + (hasIcon ? iconSize + iconGap : 0);
            int titleY = clusterY + ((clusterHeight - titleHeight) / 2);
            context.TitleBounds = new Rectangle(titleX, titleY, titleSize.Width, titleSize.Height);
            context.TitleClusterBounds = new Rectangle(clusterX, clusterY, clusterWidth, clusterHeight);
            if (hasIcon)
            {
                context.IconBounds = new Rectangle(clusterX, clusterY + ((clusterHeight - iconSize) / 2), iconSize, iconSize);
            }
            if (state.TitleStyle == PanelTitleStyle.Badge)
            {
                int badgePadH = gap;
                int badgePadV = DpiScalingHelper.ScaleValue(2, owner);
                context.BadgeBounds = new Rectangle(
                    clusterX - badgePadH,
                    clusterY - badgePadV,
                    clusterWidth + (badgePadH * 2),
                    clusterHeight + (badgePadV * 2));
            }
            context.TitleGapBounds = new Rectangle(
                context.TitleClusterBounds.Left - gap,
                bounds.Top,
                context.TitleClusterBounds.Width + (gap * 2),
                System.Math.Max(context.TitleClusterBounds.Height, DpiScalingHelper.ScaleValue(System.Math.Max(2, state.BorderThickness + 2), owner)));

            if (context.HasTitleLine)
            {
                if (state.TitleStyle == PanelTitleStyle.Divider)
                {
                    int ruleY = clusterY + clusterHeight / 2;
                    int sidePad = DpiScalingHelper.ScaleValue(8, owner);
                    context.TitleLineBounds = new Rectangle(
                        bounds.Left + sidePad,
                        ruleY,
                        System.Math.Max(1, clusterX - gap - (bounds.Left + sidePad)),
                        System.Math.Max(1, thickness));
                    context.TitleLineBoundsRight = new Rectangle(
                        clusterX + clusterWidth + gap,
                        ruleY,
                        System.Math.Max(1, (bounds.Right - sidePad) - (clusterX + clusterWidth + gap)),
                        System.Math.Max(1, thickness));
                }
                else if (state.TitleStyle == PanelTitleStyle.TopHeader)
                {
                    // Divider at the bottom edge of the header band, full width.
                    int lineY = bounds.Top + tabHeaderHeight - System.Math.Max(1, thickness);
                    int lineX = bounds.Left + state.BorderThickness;
                    context.TitleLineStartX = lineX;
                    context.TitleLineBounds = new Rectangle(
                        lineX,
                        lineY,
                        System.Math.Max(1, bounds.Width - (state.BorderThickness * 2)),
                        System.Math.Max(1, thickness));
                }
                else
                {
                    int lineY = context.TitleClusterBounds.Bottom + DpiScalingHelper.ScaleValue(2, owner);
                    int lineX = state.ShowTitleLineFullWidth ? bounds.Left + state.BorderThickness : context.TitleClusterBounds.Left;
                    int lineWidth = state.ShowTitleLineFullWidth
                        ? bounds.Width - (state.BorderThickness * 2)
                        : context.TitleClusterBounds.Width;
                    lineWidth = System.Math.Max(1, lineWidth);
                    context.TitleLineStartX = lineX;
                    context.TitleLineBounds = new Rectangle(lineX, lineY, lineWidth, System.Math.Max(1, thickness));
                }
            }

            context.HeaderBounds = state.TitleStyle switch
            {
                PanelTitleStyle.GroupBox => new Rectangle(bounds.Left, bounds.Top, bounds.Width, clusterHeight + DpiScalingHelper.ScaleValue(2, owner)),
                PanelTitleStyle.Below => new Rectangle(bounds.Left, bounds.Bottom - headerHeight, bounds.Width, headerHeight),
                PanelTitleStyle.Left => new Rectangle(bounds.Left, bounds.Top, clusterWidth + (gap * 2), bounds.Height),
                PanelTitleStyle.Right => new Rectangle(bounds.Right - (clusterWidth + (gap * 2)), bounds.Top, clusterWidth + (gap * 2), bounds.Height),
                PanelTitleStyle.Divider => new Rectangle(bounds.Left, bounds.Top, bounds.Width, headerHeight),
                PanelTitleStyle.Badge => new Rectangle(bounds.Left, bounds.Top, bounds.Width, headerHeight + gap),
                PanelTitleStyle.TopHeader => new Rectangle(bounds.Left, bounds.Top, bounds.Width, tabHeaderHeight),
                _ => new Rectangle(bounds.Left, bounds.Top, bounds.Width, headerHeight + (context.HasTitleLine ? thickness : 0))
            };

            int contentTopInset = state.Padding.Top;
            int contentLeftInset = state.Padding.Left;
            int contentRightInset = state.Padding.Right;
            int contentBottomInset = state.Padding.Bottom;
            if (state.TitleStyle == PanelTitleStyle.TopHeader)
            {
                contentTopInset += tabHeaderHeight + (context.HasTitleLine ? thickness : 0);
            }
            else if (state.TitleStyle == PanelTitleStyle.GroupBox || state.TitleStyle == PanelTitleStyle.Above || state.TitleStyle == PanelTitleStyle.Overlay ||
                state.TitleStyle == PanelTitleStyle.Divider || state.TitleStyle == PanelTitleStyle.Badge)
            {
                contentTopInset += headerHeight + (context.HasTitleLine ? thickness : 0);
            }
            else if (state.TitleStyle == PanelTitleStyle.Below)
            {
                contentBottomInset += headerHeight + (context.HasTitleLine ? thickness : 0);
            }
            else if (state.TitleStyle == PanelTitleStyle.Left)
            {
                contentLeftInset += context.HeaderBounds.Width;
            }
            else if (state.TitleStyle == PanelTitleStyle.Right)
            {
                contentRightInset += context.HeaderBounds.Width;
            }

            context.ContentBounds = Rectangle.FromLTRB(
                bounds.Left + contentLeftInset,
                bounds.Top + contentTopInset,
                bounds.Right - contentRightInset,
                bounds.Bottom - contentBottomInset);

            if (context.ContentBounds.Width < 0 || context.ContentBounds.Height < 0)
            {
                context.ContentBounds = Rectangle.Empty;
            }

            return context;
        }
    }
}
