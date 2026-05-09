// BeepTabFocusVisualHelper.cs
// Focus-ring rendering for keyboard-navigated tabs.
// Follows WCAG 2.2 §2.4.11 (Focus Appearance) guidance:
//   - minimum 2 px outline
//   - contrasting offset so the ring is visible on any tab background
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Tabs.Models;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Helpers
{
    /// <summary>
    /// Draws and measures the keyboard focus ring for a single tab header item.
    /// Use <see cref="DrawFocusRing"/> in a paint pass after all tab bodies have
    /// been rendered so the ring always appears on top.
    /// </summary>
    public static class BeepTabFocusVisualHelper
    {
        // ── Public API ────────────────────────────────────────────────────────

        /// <summary>
        /// Draws a focus ring inside <paramref name="tabBounds"/> for the given
        /// <paramref name="item"/>.  Does nothing when
        /// <see cref="BeepTabItem.IsFocused"/> is <see langword="false"/>.
        /// </summary>
        /// <param name="graphics">Graphics from the owning control's paint handler.</param>
        /// <param name="item">Tab model carrying the focused state.</param>
        /// <param name="tabBounds">Client-coordinate bounds of the tab body.</param>
        /// <param name="isHighContrast">
        /// Pass <see langword="true"/> when the system is running in a high-contrast theme
        /// to switch from a dotted focus indicator to a solid high-visibility ring.
        /// </param>
        public static void DrawFocusRing(
            Graphics graphics,
            BeepTabItem item,
            Rectangle tabBounds,
            bool isHighContrast = false)
        {
            if (graphics == null) throw new ArgumentNullException(nameof(graphics));
            if (item == null)     throw new ArgumentNullException(nameof(item));

            if (!item.IsFocused || tabBounds.IsEmpty)
            {
                return;
            }

            Rectangle ringRect = Inflate(tabBounds, -FocusInset);
            if (ringRect.Width < 4 || ringRect.Height < 4)
            {
                ringRect = tabBounds;
            }

            if (isHighContrast)
            {
                DrawHighContrastRing(graphics, ringRect);
            }
            else
            {
                DrawStandardRing(graphics, ringRect);
            }
        }

        /// <summary>
        /// Returns the inflated bounds that <see cref="DrawFocusRing"/> will use.
        /// Useful for hit-test or accessible-bounds calculations.
        /// </summary>
        public static Rectangle GetFocusRingBounds(Rectangle tabBounds)
        {
            if (tabBounds.IsEmpty) return tabBounds;

            Rectangle r = Inflate(tabBounds, -FocusInset);
            return (r.Width >= 4 && r.Height >= 4) ? r : tabBounds;
        }

        // ── Constants ─────────────────────────────────────────────────────────

        /// <summary>
        /// Pixels the ring is inset from the outer tab edge.
        /// Must be positive so the ring is drawn inside the tab body.
        /// </summary>
        public const int FocusInset       = 2;

        /// <summary>Focus ring outline thickness in pixels.</summary>
        public const float FocusThickness = 2f;

        // ── Private helpers ───────────────────────────────────────────────────

        /// <summary>
        /// Standard (non-high-contrast) ring: dotted dark outline offset
        /// with a white inner outline to ensure contrast on any background.
        /// </summary>
        private static void DrawStandardRing(Graphics graphics, Rectangle rect)
        {
            GraphicsState savedState = graphics.Save();
            try
            {
                graphics.SmoothingMode = SmoothingMode.None;

                // White inner shadow for contrast on dark backgrounds.
                using (Pen whitePen = new Pen(Color.White, FocusThickness + 1f))
                {
                    whitePen.DashStyle = DashStyle.Dot;
                    graphics.DrawRectangle(whitePen, rect.X, rect.Y, rect.Width, rect.Height);
                }

                // Dotted dark outline (Windows classic focus ring style).
                using (Pen darkPen = new Pen(SystemColors.ControlText, FocusThickness))
                {
                    darkPen.DashStyle = DashStyle.Dot;
                    graphics.DrawRectangle(darkPen, rect.X, rect.Y, rect.Width, rect.Height);
                }
            }
            finally
            {
                graphics.Restore(savedState);
            }
        }

        /// <summary>
        /// High-contrast ring: solid thick outline in <see cref="SystemColors.Highlight"/>
        /// without transparency or compositing.
        /// </summary>
        private static void DrawHighContrastRing(Graphics graphics, Rectangle rect)
        {
            GraphicsState savedState = graphics.Save();
            try
            {
                graphics.SmoothingMode = SmoothingMode.None;

                using Pen focusPen = new Pen(SystemColors.Highlight, FocusThickness + 1f);
                graphics.DrawRectangle(focusPen, rect.X, rect.Y, rect.Width, rect.Height);
            }
            finally
            {
                graphics.Restore(savedState);
            }
        }

        private static Rectangle Inflate(Rectangle r, int amount)
        {
            return new Rectangle(
                r.X       - amount,
                r.Y       - amount,
                r.Width   + amount * 2,
                r.Height  + amount * 2);
        }
    }
}
