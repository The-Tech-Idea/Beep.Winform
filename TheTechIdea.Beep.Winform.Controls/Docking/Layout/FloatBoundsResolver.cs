using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Layout
{
    /// <summary>
    /// Decides where a saved floating window should reappear.
    /// </summary>
    /// <remarks>
    /// A pure function of (saved float, available monitors). No <see cref="System.Windows.Forms.Screen"/>
    /// access, no form, no side effects — so "the monitor it was on has been unplugged" is an
    /// argument rather than a hardware configuration, and the outcome can be asserted.
    /// <para>
    /// The rule is the one Visual Studio and Rider both use: prefer the display the window was
    /// actually on, fall back to whichever display it most overlaps, and finally to the primary —
    /// then clamp, because a window the user cannot reach is worse than one in the wrong place.
    /// </para>
    /// </remarks>
    public static class FloatBoundsResolver
    {
        /// <summary>
        /// Height of the strip that must remain on-screen. A window is draggable only by its
        /// caption, so this is the part that cannot be allowed off the working area.
        /// </summary>
        public const int MinimumVisibleCaption = 24;

        /// <summary>Smallest float this will produce, whatever was saved.</summary>
        public const int MinimumFloatWidth = 120;
        public const int MinimumFloatHeight = 80;

        /// <summary>Why the resolver chose the monitor it did. Reported, not inferred.</summary>
        public enum MatchKind
        {
            /// <summary>The saved device name is still present.</summary>
            DeviceName,

            /// <summary>The device is gone; the display the saved bounds most overlap was used.</summary>
            GeometryOverlap,

            /// <summary>Nothing overlapped; the primary display was used.</summary>
            Primary,

            /// <summary>No monitors were supplied; the bounds were returned unchanged.</summary>
            NoMonitors
        }

        public readonly struct Resolution
        {
            public Resolution(Rectangle bounds, MonitorInfo monitor, MatchKind match, bool clamped)
            {
                Bounds = bounds;
                Monitor = monitor;
                Match = match;
                Clamped = clamped;
            }

            /// <summary>Bounds to restore the float to.</summary>
            public Rectangle Bounds { get; }

            /// <summary>Display the float landed on.</summary>
            public MonitorInfo Monitor { get; }

            public MatchKind Match { get; }

            /// <summary>True when the saved bounds had to be moved or resized to fit.</summary>
            public bool Clamped { get; }

            public override string ToString()
                => $"{Bounds} on {Monitor.DeviceName} via {Match}{(Clamped ? " (clamped)" : "")}";
        }

        /// <summary>
        /// Resolves where <paramref name="saved"/> should be restored, given the displays that exist
        /// now.
        /// </summary>
        public static Resolution Resolve(FloatingPanelInfo saved, IReadOnlyList<MonitorInfo> monitors)
        {
            if (saved == null)
                throw new ArgumentNullException(nameof(saved));

            if (monitors == null || monitors.Count == 0)
                return new Resolution(saved.Bounds, default, MatchKind.NoMonitors, false);

            var (monitor, match) = ChooseMonitor(saved, monitors);
            var clamped = ClampInto(saved.Bounds, monitor.WorkingArea);

            return new Resolution(clamped, monitor, match, clamped != saved.Bounds);
        }

        private static (MonitorInfo monitor, MatchKind match) ChooseMonitor(
            FloatingPanelInfo saved, IReadOnlyList<MonitorInfo> monitors)
        {
            // 1. The display it was on, if it is still here. Matching by name rather than geometry
            //    keeps the float in place when displays are rearranged - a second monitor moved from
            //    the left of the primary to the right changes every coordinate but not its identity.
            if (!string.IsNullOrEmpty(saved.DeviceName))
            {
                foreach (var m in monitors)
                {
                    if (string.Equals(m.DeviceName, saved.DeviceName, StringComparison.Ordinal))
                        return (m, MatchKind.DeviceName);
                }
            }

            // 2. The display the saved bounds most overlap. This is what handles a layout written
            //    before device names were recorded, and a display replaced by a different one in
            //    the same position.
            MonitorInfo best = default;
            long bestArea = 0;
            foreach (var m in monitors)
            {
                var overlap = Rectangle.Intersect(saved.Bounds, m.WorkingArea);
                long area = (long)Math.Max(0, overlap.Width) * Math.Max(0, overlap.Height);
                if (area > bestArea)
                {
                    bestArea = area;
                    best = m;
                }
            }

            if (bestArea > 0)
                return (best, MatchKind.GeometryOverlap);

            // 3. Nothing overlaps: the saved position is off every display.
            return (monitors.FirstOrDefault(m => m.IsPrimary) is { DeviceName: not null } p && p.DeviceName.Length > 0
                        ? p
                        : monitors[0],
                    MatchKind.Primary);
        }

        /// <summary>
        /// Fits <paramref name="bounds"/> inside <paramref name="workingArea"/>, shrinking only when
        /// the window is genuinely larger than the display.
        /// </summary>
        /// <remarks>
        /// The window is moved before it is resized: a float larger than it needs to be is a much
        /// smaller problem than one whose caption sits above the top of the screen, where there is
        /// nothing to grab.
        /// </remarks>
        public static Rectangle ClampInto(Rectangle bounds, Rectangle workingArea)
        {
            if (workingArea.Width <= 0 || workingArea.Height <= 0)
                return bounds;

            int width = Math.Max(MinimumFloatWidth, bounds.Width);
            int height = Math.Max(MinimumFloatHeight, bounds.Height);

            width = Math.Min(width, workingArea.Width);
            height = Math.Min(height, workingArea.Height);

            int x = bounds.X;
            int y = bounds.Y;

            // Right/bottom first, then left/top, so a window wider than the display ends up aligned
            // to the left edge rather than the right.
            if (x + width > workingArea.Right) x = workingArea.Right - width;
            if (y + height > workingArea.Bottom) y = workingArea.Bottom - height;
            if (x < workingArea.Left) x = workingArea.Left;
            if (y < workingArea.Top) y = workingArea.Top;

            return new Rectangle(x, y, width, height);
        }

        /// <summary>
        /// True when at least <see cref="MinimumVisibleCaption"/> of the caption strip falls inside
        /// some display's working area — i.e. the user can still grab the window.
        /// </summary>
        public static bool IsCaptionReachable(Rectangle bounds, IReadOnlyList<MonitorInfo> monitors)
        {
            if (monitors == null || monitors.Count == 0)
                return false;

            var caption = new Rectangle(bounds.X, bounds.Y, bounds.Width,
                                        Math.Min(MinimumVisibleCaption, Math.Max(1, bounds.Height)));

            foreach (var m in monitors)
            {
                var overlap = Rectangle.Intersect(caption, m.WorkingArea);
                if (overlap.Width > 0 && overlap.Height > 0)
                    return true;
            }
            return false;
        }
    }
}
