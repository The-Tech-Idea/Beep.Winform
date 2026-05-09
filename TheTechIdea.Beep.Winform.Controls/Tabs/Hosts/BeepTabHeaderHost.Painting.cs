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

        private static void PaintCloseButtonFeedback(Graphics graphics, BeepTabHeaderItemLayout itemLayout)
        {
            if (!itemLayout.HasCloseButton)
            {
                return;
            }

            if (!itemLayout.Item.IsCloseButtonHovered && !itemLayout.Item.IsCloseButtonPressed)
            {
                return;
            }

            Color overlayColor = itemLayout.Item.IsCloseButtonPressed
                ? Color.FromArgb(72, Color.Black)
                : Color.FromArgb(36, Color.Black);

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

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (LayoutSnapshot == null)
            {
                return;
            }

            // High-contrast mode: skip theme painters and use system-colour pass.
            if (IsHighContrast)
            {
                PaintHighContrast(e.Graphics);

                bool hcMode = true;
                foreach (BeepTabHeaderItemLayout itemLayout in LayoutSnapshot.Items)
                {
                    BeepTabFocusVisualHelper.DrawFocusRing(e.Graphics, itemLayout.Item, itemLayout.Bounds, hcMode);
                }

                PaintHeaderActions(e.Graphics);
                return;
            }

            using Pen borderPen = new Pen(Color.FromArgb(120, ForeColor));
            using SolidBrush textBrush = new SolidBrush(ForeColor);
            using SolidBrush hoverBrush = new SolidBrush(Color.FromArgb(24, ForeColor));
            using SolidBrush pressedBrush = new SolidBrush(Color.FromArgb(40, ForeColor));

            if (!LayoutSnapshot.HeaderBounds.IsEmpty)
            {
                e.Graphics.DrawRectangle(borderPen, LayoutSnapshot.HeaderBounds);
            }

            foreach (BeepTabHeaderItemLayout itemLayout in LayoutSnapshot.Items)
            {
                if (itemLayout.Item.IsPressed)
                {
                    e.Graphics.FillRectangle(pressedBrush, itemLayout.Bounds);
                }
                else if (itemLayout.Item.IsHovered)
                {
                    e.Graphics.FillRectangle(hoverBrush, itemLayout.Bounds);
                }

                TextRenderer.DrawText(
                    e.Graphics,
                    itemLayout.Item.Title ?? string.Empty,
                    Font,
                    itemLayout.TextBounds,
                    textBrush.Color,
                    TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter | TextFormatFlags.Left);

                if (itemLayout.HasCloseButton)
                {
                    if (itemLayout.Item.IsCloseButtonPressed)
                    {
                        e.Graphics.FillEllipse(pressedBrush, itemLayout.CloseButtonBounds);
                    }
                    else if (itemLayout.Item.IsCloseButtonHovered)
                    {
                        e.Graphics.FillEllipse(hoverBrush, itemLayout.CloseButtonBounds);
                    }

                    e.Graphics.DrawRectangle(borderPen, itemLayout.CloseButtonBounds);
                }
            }

            PaintHeaderActions(e.Graphics);
            PaintDragFeedback(e.Graphics);
        }
    }
}