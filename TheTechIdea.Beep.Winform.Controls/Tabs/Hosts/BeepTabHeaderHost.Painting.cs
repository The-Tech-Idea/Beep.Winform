using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Tabs.Helpers;
using TheTechIdea.Beep.Winform.Controls.Tabs.Models;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Hosts
{
    public partial class BeepTabHeaderHost
    {
        /// <summary>
        /// Renders the whole tab header: header background, every tab, header actions, drag
        /// feedback, then focus rings on top.
        /// </summary>
        /// <remarks>
        /// This was called <c>RenderLegacyHeader</c>, which was simply wrong - it is the only
        /// render entry point there is, called from <c>BeepTabs.Drawing</c>. Nothing replaced it and
        /// nothing was scheduled to. The name cost real time during this review, because a method
        /// named "legacy" invites a maintainer to delete it or to go looking for the newer one.
        /// </remarks>
        public void RenderHeader(Graphics graphics, BeepTabHeaderRenderRequest renderRequest)
        {
            if (graphics == null)
            {
                throw new ArgumentNullException(nameof(graphics));
            }

            if (renderRequest == null)
            {
                throw new ArgumentNullException(nameof(renderRequest));
            }

            if (LayoutSnapshot == null || LayoutSnapshot.Items.Count == 0)
            {
                return;
            }

            ApplyItemState();

            if (!LayoutSnapshot.HeaderBounds.IsEmpty)
            {
                renderRequest.PrimaryPainter.PaintHeaderBackground(graphics, LayoutSnapshot.HeaderBounds);
            }

            foreach (BeepTabHeaderItemLayout itemLayout in LayoutSnapshot.Items)
            {
                PaintTabItemClipped(graphics, itemLayout, renderRequest);
                PaintCloseButtonFeedback(graphics, itemLayout);
            }

            PaintHeaderActions(graphics);
            PaintDragFeedback(graphics);

            // Focus rings are drawn last so they appear above all tab content.
            bool highContrast = TabThemeHelpers.IsHighContrast;
            foreach (BeepTabHeaderItemLayout itemLayout in LayoutSnapshot.Items)
            {
                BeepTabFocusVisualHelper.DrawFocusRing(graphics, itemLayout.Item, itemLayout.Bounds, highContrast);
            }
        }

        private void PaintCloseButtonFeedback(Graphics graphics, BeepTabHeaderItemLayout itemLayout)
        {
            if (!itemLayout.HasCloseButton)
            {
                return;
            }

            if (!itemLayout.Item.IsCloseButtonHovered && !itemLayout.Item.IsCloseButtonPressed)
            {
                return;
            }

            // BT-03: Use theme-derived overlay colors
            Color baseOverlay = TabThemeHelpers.GetTabTextColor(TabsOwner?.CurrentTheme);
            Color overlayColor = itemLayout.Item.IsCloseButtonPressed
                ? Color.FromArgb(72, baseOverlay)
                : Color.FromArgb(36, baseOverlay);

            Rectangle overlayRect = itemLayout.CloseButtonBounds;
            overlayRect.Inflate(-2, -2);
            if (overlayRect.Width <= 0 || overlayRect.Height <= 0)
            {
                overlayRect = itemLayout.CloseButtonBounds;
            }

            using SolidBrush overlayBrush = new SolidBrush(overlayColor);
            graphics.FillEllipse(overlayBrush, overlayRect);
        }

        private void PaintDragFeedback(Graphics graphics)
        {
            if (!DragFeedback.HasMarker)
            {
                return;
            }

            using Pen markerPen = new Pen(TabThemeHelpers.GetTabIndicatorColor(TabsOwner?.CurrentTheme), 2f);
            graphics.DrawLine(markerPen, DragFeedback.MarkerStart, DragFeedback.MarkerEnd);
        }

        /// <summary>
        /// Paints one tab through the active painter, clipped to its own bounds, cross-fading
        /// between the outgoing and incoming painters while a style transition is running.
        /// </summary>
        private static void PaintTabItemClipped(Graphics graphics, BeepTabHeaderItemLayout itemLayout, BeepTabHeaderRenderRequest renderRequest)
        {
            graphics.SetClip(itemLayout.Bounds, System.Drawing.Drawing2D.CombineMode.Replace);
            try
            {
                if (renderRequest.HasTransition)
                {
                    renderRequest.TransitionFromPainter!.PaintTabItem(graphics, itemLayout, 1f - renderRequest.TransitionProgress);
                    renderRequest.TransitionToPainter!.PaintTabItem(graphics, itemLayout, renderRequest.TransitionProgress);

                    return;
                }

                renderRequest.PrimaryPainter.PaintTabItem(graphics, itemLayout, 1f);
            }
            finally
            {
                graphics.ResetClip();
            }
        }

    }
}