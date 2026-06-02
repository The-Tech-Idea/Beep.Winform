using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Painters
{
    /// <summary>
    /// Per-style chrome tuning applied to the shared docking element renderers. Each
    /// <see cref="BeepControlStyle"/> resolves to a flavor that tweaks the look of the same
    /// underlying renderers (caption, splitter, auto-hide strip) — e.g. Fluent2 uses a 4 px tab
    /// radius and a thin accent bar, Material3 uses 8 px rounded corners with a tonal
    /// elevation tint, MacOSBigSur uses a translucent header and pill-shaped tabs.
    /// </summary>
    /// <remarks>
    /// Flavour is resolved once per style and cached by <see cref="DockingPainterFactory"/>.
    /// Renderers accept a flavor as a Paint-context extension so a single renderer instance
    /// paints any style without per-style subclasses.
    /// </remarks>
    public sealed class DockingStyleFlavor
    {
        /// <summary>Tab corner radius (px). 0 = square.</summary>
        public int TabCornerRadius { get; init; }

        /// <summary>Header / caption strip corner radius (px). 0 = square.</summary>
        public int HeaderCornerRadius { get; init; }

        /// <summary>Active-tab accent line width drawn along the active tab's bottom edge (px). 0 = none.</summary>
        public int ActiveTabAccentWidth { get; init; }

        /// <summary>Splitter grip dot size in pixels.</summary>
        public int SplitterGripSize { get; init; } = 2;

        /// <summary>Splitter grip dot spacing in pixels.</summary>
        public int SplitterGripSpacing { get; init; } = 4;

        /// <summary>Whether the active tab gets a tonal elevation tint applied on top of the accent color.</summary>
        public bool UseTonalElevation { get; init; }

        /// <summary>When true, the splitter bar is painted with a translucent overlay (macOS frosted look).</summary>
        public bool UseTranslucentSplitter { get; init; }

        /// <summary>Header background blend (0..1) toward a slightly lighter color to mimic elevation.</summary>
        public float HeaderElevationBlend { get; init; }

        /// <summary>Tab border style.</summary>
        public DockingTabBorderStyle TabBorderStyle { get; init; } = DockingTabBorderStyle.Rectangle;

        /// <summary>Resolves a flavor for the given control style. Falls back to <see cref="Default"/>.</summary>
        public static DockingStyleFlavor ForStyle(BeepControlStyle style) => style switch
        {
            BeepControlStyle.Fluent2           => Fluent2,
            BeepControlStyle.Windows11Mica     => Fluent2,             // Mica reuses Fluent2 chrome
            BeepControlStyle.Material3         => Material3,
            BeepControlStyle.MaterialYou       => Material3,          // MaterialYou is a Material3 variant
            BeepControlStyle.MacOSBigSur       => MacOSBigSur,
            BeepControlStyle.iOS15             => iOS15,
            BeepControlStyle.AntDesign         => AntDesign,
            BeepControlStyle.ChakraUI          => ChakraUI,
            BeepControlStyle.TailwindCard      => TailwindCard,
            _                                  => Default
        };

        /// <summary>Default flat look — no rounded corners, no accent, no elevation.</summary>
        public static DockingStyleFlavor Default { get; } = new DockingStyleFlavor
        {
            TabCornerRadius = 0,
            HeaderCornerRadius = 0,
            ActiveTabAccentWidth = 0,
            UseTonalElevation = false,
            UseTranslucentSplitter = false,
            HeaderElevationBlend = 0f,
            TabBorderStyle = DockingTabBorderStyle.Rectangle
        };

        /// <summary>Microsoft Fluent 2 — 4 px rounded tabs, 2 px accent bar, subtle header elevation.</summary>
        public static DockingStyleFlavor Fluent2 { get; } = new DockingStyleFlavor
        {
            TabCornerRadius = 4,
            HeaderCornerRadius = 4,
            ActiveTabAccentWidth = 2,
            UseTonalElevation = false,
            UseTranslucentSplitter = false,
            HeaderElevationBlend = 0.06f,
            TabBorderStyle = DockingTabBorderStyle.Rectangle
        };

        /// <summary>Google Material 3 — 8 px rounded corners with tonal elevation on active tab.</summary>
        public static DockingStyleFlavor Material3 { get; } = new DockingStyleFlavor
        {
            TabCornerRadius = 8,
            HeaderCornerRadius = 8,
            ActiveTabAccentWidth = 0,
            UseTonalElevation = true,
            UseTranslucentSplitter = false,
            HeaderElevationBlend = 0.10f,
            TabBorderStyle = DockingTabBorderStyle.Rectangle
        };

        /// <summary>macOS Big Sur — translucent chrome, pill-shaped tabs, 6 px radius.</summary>
        public static DockingStyleFlavor MacOSBigSur { get; } = new DockingStyleFlavor
        {
            TabCornerRadius = 6,
            HeaderCornerRadius = 6,
            ActiveTabAccentWidth = 0,
            UseTonalElevation = true,
            UseTranslucentSplitter = true,
            HeaderElevationBlend = 0.05f,
            TabBorderStyle = DockingTabBorderStyle.Rectangle
        };

        /// <summary>iOS 15 — pill tabs, 10 px radius, no header elevation.</summary>
        public static DockingStyleFlavor iOS15 { get; } = new DockingStyleFlavor
        {
            TabCornerRadius = 10,
            HeaderCornerRadius = 0,
            ActiveTabAccentWidth = 0,
            UseTonalElevation = true,
            UseTranslucentSplitter = false,
            HeaderElevationBlend = 0f,
            TabBorderStyle = DockingTabBorderStyle.Rectangle
        };

        /// <summary>Ant Design — 2 px radius, primary-color accent bar.</summary>
        public static DockingStyleFlavor AntDesign { get; } = new DockingStyleFlavor
        {
            TabCornerRadius = 2,
            HeaderCornerRadius = 2,
            ActiveTabAccentWidth = 1,
            UseTonalElevation = false,
            UseTranslucentSplitter = false,
            HeaderElevationBlend = 0.04f,
            TabBorderStyle = DockingTabBorderStyle.Rectangle
        };

        /// <summary>Chakra UI — 4 px radius, top-anchored accent bar.</summary>
        public static DockingStyleFlavor ChakraUI { get; } = new DockingStyleFlavor
        {
            TabCornerRadius = 4,
            HeaderCornerRadius = 4,
            ActiveTabAccentWidth = 2,
            UseTonalElevation = false,
            UseTranslucentSplitter = false,
            HeaderElevationBlend = 0.04f,
            TabBorderStyle = DockingTabBorderStyle.Rectangle
        };

        /// <summary>Tailwind card — 6 px radius, no border, soft header elevation.</summary>
        public static DockingStyleFlavor TailwindCard { get; } = new DockingStyleFlavor
        {
            TabCornerRadius = 6,
            HeaderCornerRadius = 6,
            ActiveTabAccentWidth = 0,
            UseTonalElevation = true,
            UseTranslucentSplitter = false,
            HeaderElevationBlend = 0.08f,
            TabBorderStyle = DockingTabBorderStyle.Rectangle
        };
    }

    /// <summary>How tab borders are drawn.</summary>
    public enum DockingTabBorderStyle
    {
        /// <summary>Solid 1 px rectangle border.</summary>
        Rectangle = 0,

        /// <summary>No border; rely on background differentiation.</summary>
        None = 1
    }
}
