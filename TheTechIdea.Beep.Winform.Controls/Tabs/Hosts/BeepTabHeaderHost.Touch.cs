// BeepTabHeaderHost.Touch.cs
// Touch ergonomics enforcement for BeepTabHeaderHost.
// Provides helpers that ensure tab header hit targets meet the minimum
// recommended touch-target size (44 × 44 dip, matching WCAG 2.5.5 and
// Microsoft's Fluent Design guidance).
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Tabs.Models;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Hosts
{
    public partial class BeepTabHeaderHost
    {
        // ── Touch-target constants ────────────────────────────────────────────

        /// <summary>
        /// Minimum recommended touch-target width in device-independent pixels.
        /// Matches the owner control's <c>MinTouchTargetWidth</c> default (44 dp).
        /// </summary>
        public const int DefaultMinTouchTargetWidth  = 44;

        /// <summary>
        /// Minimum recommended touch-target height in device-independent pixels.
        /// </summary>
        public const int DefaultMinTouchTargetHeight = 44;

        // ── Hit-target expansion ─────────────────────────────────────────────

        /// <summary>
        /// Returns an expanded hit-test rectangle for <paramref name="tabBounds"/>
        /// that is at least <paramref name="minWidth"/> × <paramref name="minHeight"/>
        /// pixels.  The expansion is centred on the original bounds so the visual
        /// position does not change.
        /// </summary>
        /// <param name="tabBounds">Original layout bounds of the tab body.</param>
        /// <param name="minWidth">Minimum width in pixels (device units).</param>
        /// <param name="minHeight">Minimum height in pixels (device units).</param>
        public static Rectangle ExpandToMinTouchTarget(
            Rectangle tabBounds,
            int minWidth  = DefaultMinTouchTargetWidth,
            int minHeight = DefaultMinTouchTargetHeight)
        {
            if (tabBounds.IsEmpty) return tabBounds;

            int expandW = Math.Max(0, minWidth  - tabBounds.Width)  / 2;
            int expandH = Math.Max(0, minHeight - tabBounds.Height) / 2;

            return new Rectangle(
                tabBounds.X      - expandW,
                tabBounds.Y      - expandH,
                tabBounds.Width  + expandW * 2,
                tabBounds.Height + expandH * 2);
        }

        /// <summary>
        /// Converts the <paramref name="minWidth"/> value (in device-independent
        /// pixels) to physical pixels using the DPI of the form that owns this host.
        /// Returns the raw value unchanged when the owner form is unavailable.
        /// </summary>
        /// <param name="minWidth">Logical minimum width (dp).</param>
        public int ScaleTouchTarget(int minWidth)
        {
            try
            {
                float dpiScale = DeviceDpi / 96f;
                return (int)Math.Ceiling(minWidth * dpiScale);
            }
            catch
            {
                return minWidth;
            }
        }

        // ── Touch-target validation helper ───────────────────────────────────

        /// <summary>
        /// Returns <see langword="true"/> when the tab body rect in
        /// <paramref name="layout"/> satisfies the minimum touch target size.
        /// Useful for diagnostics and layout auditing during development.
        /// </summary>
        /// <param name="layout">Item layout to inspect.</param>
        /// <param name="minWidth">Minimum width threshold (device pixels).</param>
        /// <param name="minHeight">Minimum height threshold (device pixels).</param>
        public static bool MeetsTouchTarget(
            BeepTabHeaderItemLayout layout,
            int minWidth  = DefaultMinTouchTargetWidth,
            int minHeight = DefaultMinTouchTargetHeight)
        {
            if (layout == null) throw new ArgumentNullException(nameof(layout));

            return layout.Bounds.Width  >= minWidth
                && layout.Bounds.Height >= minHeight;
        }

        // ── Pointer-input expansion at hit-test time ──────────────────────────

        /// <summary>
        /// Checks whether <paramref name="clientPoint"/> falls within the touch-
        /// expanded hit area of any item in the current snapshot.  Returns the
        /// matched tab index or -1.
        /// </summary>
        /// <param name="clientPoint">Pointer position in client (host-control) coordinates.</param>
        /// <param name="scaledMinWidth">Pre-scaled minimum width; use <see cref="ScaleTouchTarget"/>.</param>
        public int TouchHitTestTabIndex(Point clientPoint, int scaledMinWidth)
        {
            if (LayoutSnapshot == null) return -1;

            foreach (BeepTabHeaderItemLayout item in LayoutSnapshot.Items)
            {
                if (!item.Item.IsVisible) continue;

                Rectangle touchRect = ExpandToMinTouchTarget(item.Bounds, scaledMinWidth, DefaultMinTouchTargetHeight);
                if (touchRect.Contains(clientPoint))
                {
                    return item.Item.Index;
                }
            }

            return -1;
        }
    }
}
