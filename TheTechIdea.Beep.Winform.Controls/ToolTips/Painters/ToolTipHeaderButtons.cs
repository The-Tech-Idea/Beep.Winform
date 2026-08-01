using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips.Painters
{
    /// <summary>
    /// Positions the small buttons in a tooltip's top-right corner (pin, close).
    /// <para>
    /// One implementation, used by the painter to draw them and by <c>CustomToolTip</c> to hit-test
    /// them. Computing these rectangles independently in two places is how a control ends up
    /// drawing a button somewhere the click handler is not looking — the same class of defect that
    /// this program has already had to fix in placement, fonts and trailing reserves.
    /// </para>
    /// </summary>
    internal static class ToolTipHeaderButtons
    {
        /// <summary>Logical edge length of a header button.</summary>
        public const int ButtonSize = 16;

        /// <summary>Gap between adjacent buttons and from the tooltip edge.</summary>
        public const int Gap = 4;

        /// <summary>
        /// Rectangle of the pin toggle, or <see cref="Rectangle.Empty"/> when the tooltip is not
        /// pinnable. The pin sits left of the close button when both are shown.
        /// </summary>
        public static Rectangle PinRect(Rectangle contentBounds, ToolTipConfig config)
        {
            if (config?.Pinnable != true) return Rectangle.Empty;

            int right = contentBounds.Right - Gap;
            if (config.Closable) right -= ButtonSize + Gap;

            return new Rectangle(right - ButtonSize, contentBounds.Top + Gap, ButtonSize, ButtonSize);
        }

        /// <summary>
        /// Rectangle of the close button, or <see cref="Rectangle.Empty"/> when not closable.
        /// </summary>
        public static Rectangle CloseRect(Rectangle contentBounds, ToolTipConfig config)
        {
            if (config?.Closable != true) return Rectangle.Empty;

            return new Rectangle(
                contentBounds.Right - Gap - ButtonSize,
                contentBounds.Top + Gap,
                ButtonSize, ButtonSize);
        }

        /// <summary>
        /// Total width the header buttons occupy, so content can avoid running underneath them.
        /// </summary>
        public static int ReservedWidth(ToolTipConfig config)
        {
            int count = 0;
            if (config?.Pinnable == true) count++;
            if (config?.Closable == true) count++;
            return count == 0 ? 0 : count * (ButtonSize + Gap) + Gap;
        }
    }
}
