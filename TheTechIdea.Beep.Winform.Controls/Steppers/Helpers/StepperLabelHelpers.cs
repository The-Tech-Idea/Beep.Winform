using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Steppers.Models;

namespace TheTechIdea.Beep.Winform.Controls.Steppers.Helpers
{
    /// <summary>
    /// One authority for drawing a step's title (and optional subtitle) beside its node.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="StepModel.Text"/> <b>is</b> the title — there is no separate title property — and
    /// every painter is expected to show it. Before this helper each painter rolled its own:
    /// <c>SquareDashed</c> and <c>Dots</c> drew no text at all, and of the twelve that did, only
    /// one measured against the space it had and only one applied
    /// <see cref="TextFormatFlags.EndEllipsis"/>. A title longer than its slot therefore ran into
    /// its neighbour rather than being cut.
    /// </para>
    /// <para>
    /// Placement follows the orientation: under the node when horizontal, to its right when
    /// vertical. The width is clamped to the space the step actually owns, so the text ellipsises
    /// instead of colliding.
    /// </para>
    /// <para>
    /// No font is created here. Allocating a <see cref="Font"/> inside a paint path churns GDI
    /// handles on every repaint.
    /// </para>
    /// </remarks>
    public static class StepperLabelHelpers
    {
        /// <summary>Gap between a node and its title, at 96 dpi.</summary>
        public const int TitleGap = 6;

        /// <summary>
        /// Draws <paramref name="step"/>'s title, and its subtitle when it has one.
        /// </summary>
        /// <param name="slotWidth">
        /// How much horizontal room this step owns — the pitch between neighbours for a horizontal
        /// bar, or the remaining width for a vertical one. The title is ellipsised to fit it.
        /// </param>
        /// <returns>The rectangle the text occupied, or <see cref="Rectangle.Empty"/> if none was drawn.</returns>
        public static Rectangle DrawStepTitle(
            Graphics g,
            StepModel step,
            Rectangle stepRect,
            Orientation orientation,
            Font font,
            Color ink,
            int slotWidth,
            Control owner = null)
        {
            if (g == null || step == null || font == null) return Rectangle.Empty;
            if (string.IsNullOrWhiteSpace(step.Text)) return Rectangle.Empty;

            int gap = DpiScalingHelper.ScaleValue(TitleGap, owner);
            var flags = TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine;

            // Measure BOTH lines and size the box to the wider of them.
            //
            // Sizing from the title alone clipped the subtitle whenever the subtitle was longer:
            // "Profile" + "Step 2" rendered as "Profile" / "Ste...", while "Account" + "Step 1"
            // was fine, because the box inherited each title's own width.
            Size measured = TextRenderer.MeasureText(g, step.Text, font);
            int lineH = measured.Height;
            if (!string.IsNullOrWhiteSpace(step.Subtitle))
            {
                Size sub = TextRenderer.MeasureText(g, step.Subtitle, font);
                measured.Width = Math.Max(measured.Width, sub.Width);
            }

            Rectangle titleRect;
            if (orientation == Orientation.Horizontal)
            {
                // Centred under the node, never wider than the step's own slot - that clamp is
                // what stops a long title running into its neighbour.
                int width = Math.Max(stepRect.Width, Math.Min(measured.Width, Math.Max(1, slotWidth)));
                titleRect = new Rectangle(
                    stepRect.Left + ((stepRect.Width - width) / 2),
                    stepRect.Bottom + gap,
                    width,
                    lineH);
                flags |= TextFormatFlags.HorizontalCenter;
            }
            else
            {
                int left = stepRect.Right + DpiScalingHelper.ScaleValue(10, owner);
                int width = Math.Max(1, slotWidth - (left - stepRect.Left));
                titleRect = new Rectangle(left, stepRect.Top + ((stepRect.Height - lineH) / 2), width, lineH);
                flags |= TextFormatFlags.Left;
            }

            TextRenderer.DrawText(g, step.Text, font, titleRect, ink, flags);

            if (string.IsNullOrWhiteSpace(step.Subtitle)) return titleRect;

            // The same font, dimmed - deliberately not a second Font instance.
            var subRect = new Rectangle(titleRect.X, titleRect.Bottom, titleRect.Width, lineH);
            TextRenderer.DrawText(g, step.Subtitle, font, subRect, Color.FromArgb(150, ink), flags);
            return Rectangle.Union(titleRect, subRect);
        }

        /// <summary>
        /// The horizontal room a step owns, from the distance to its neighbour.
        /// </summary>
        /// <remarks>
        /// Falls back to the node's own width when there is only one step, so a lone step never
        /// gets a zero-width slot and vanishes.
        /// </remarks>
        public static int SlotWidth(StepPainterContext context, int stepIndex, Rectangle stepRect)
        {
            // A node is usually far narrower than the room the step owns, so falling back to the
            // node's width alone ellipsised captions that had space to spare. Give a lone step the
            // whole content rect instead.
            if (context == null || context.StepRects == null || context.StepRects.Count < 2)
                return Math.Max(context?.DrawingRect.Width ?? stepRect.Width, stepRect.Width);

            int pitch = int.MaxValue;
            for (int i = 0; i < context.StepRects.Count; i++)
            {
                if (i == stepIndex) continue;
                int d = Math.Abs(context.StepRects[i].Left - stepRect.Left);
                if (d > 0 && d < pitch) pitch = d;
            }
            return pitch == int.MaxValue ? Math.Max(stepRect.Width, 1) : pitch;
        }

    }
}
