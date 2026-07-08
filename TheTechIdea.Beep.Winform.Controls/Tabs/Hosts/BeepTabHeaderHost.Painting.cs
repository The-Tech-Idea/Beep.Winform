using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Tabs.Helpers;
using TheTechIdea.Beep.Winform.Controls.Tabs.Models;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Hosts
{
    public partial class BeepTabHeaderHost
    {
        public void RenderLegacyHeader(Graphics graphics, BeepTabHeaderRenderRequest renderRequest)
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
                PaintLegacyTab(graphics, itemLayout, renderRequest);
                PaintCloseButtonFeedback(graphics, itemLayout);
            }

            PaintHeaderActions(graphics);
            PaintDragFeedback(graphics);

            // Focus rings are drawn last so they appear above all tab content.
            bool highContrast = SystemInformation.HighContrast;
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
            Color baseOverlay = TabsOwner?.CurrentTheme?.ForeColor ?? SystemColors.ControlText;
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

            using Pen markerPen = new Pen(Color.Black, 2f);
            graphics.DrawLine(markerPen, DragFeedback.MarkerStart, DragFeedback.MarkerEnd);
        }

        private static void PaintLegacyTab(Graphics graphics, BeepTabHeaderItemLayout itemLayout, BeepTabHeaderRenderRequest renderRequest)
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