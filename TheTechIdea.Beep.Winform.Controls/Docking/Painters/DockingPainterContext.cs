using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Painters
{
    /// <summary>
    /// Immutable render context passed to every <see cref="IDockingElementRenderer"/>.
    /// Carries the theme, resolved docking palette, control style, interaction state, and
    /// geometry so renderers remain stateless — they only paint the resolved state. All
    /// mouse/hit-test logic lives in the layout managers, never in renderers.
    /// </summary>
    internal sealed class DockingPainterContext
    {
        /// <summary>The active Beep theme (source of fonts and base colors).</summary>
        public IBeepTheme Theme { get; init; }

        /// <summary>Resolved docking palette (header/tab/splitter/strip colors).</summary>
        public DockingThemeColors Colors { get; init; } = DockingThemeColors.Default;

        /// <summary>Control style used to drive the background/border/shadow factories.</summary>
        public BeepControlStyle Style { get; init; } = BeepControlStyle.Material3;

        /// <summary>When true, chrome reads colors from <see cref="Theme"/>; otherwise the fallback palette.</summary>
        public bool UseThemeColors { get; init; } = true;

        /// <summary>The element bounds being painted (control-relative).</summary>
        public Rectangle Bounds { get; init; }

        /// <summary>DPI scale factor (1.0 = 96 DPI).</summary>
        public float DpiScale { get; init; } = 1f;

        // ── Interaction state (resolved by the layout manager) ───────────────────────
        /// <summary>The element is active/selected (e.g. active panel or selected tab).</summary>
        public bool IsActive { get; init; }

        /// <summary>The pointer is hovering the element.</summary>
        public bool IsHover { get; init; }

        /// <summary>The element is pressed.</summary>
        public bool IsPressed { get; init; }

        /// <summary>The element (or its owner) has focus.</summary>
        public bool IsFocused { get; init; }

        /// <summary>A drag operation involving this element is in progress.</summary>
        public bool IsDragging { get; init; }

        /// <summary>True when hosted in the Visual Studio designer.</summary>
        public bool IsDesignTime { get; init; }

        // ── Caption button visibility ────────────────────────────────────────────────
        /// <summary>Show the close button.</summary>
        public bool CanClose { get; init; }

        /// <summary>Show the float/restore button.</summary>
        public bool CanFloat { get; init; }

        /// <summary>Show the auto-hide button.</summary>
        public bool CanAutoHide { get; init; }

        /// <summary>Show the pin button (auto-hidden state toggle).</summary>
        public bool CanPin { get; init; }

        /// <summary>Per-style chrome tuning (corner radii, accent width, grip style). Defaults to <see cref="DockingStyleFlavor.Default"/>.</summary>
        public DockingStyleFlavor Flavor { get; init; } = DockingStyleFlavor.Default;

        /// <summary>Returns a copy of this context with new bounds (renderers paint sub-regions).</summary>
        public DockingPainterContext WithBounds(Rectangle bounds) => new DockingPainterContext
        {
            Theme = Theme,
            Colors = Colors,
            Style = Style,
            UseThemeColors = UseThemeColors,
            Bounds = bounds,
            DpiScale = DpiScale,
            IsActive = IsActive,
            IsHover = IsHover,
            IsPressed = IsPressed,
            IsFocused = IsFocused,
            IsDragging = IsDragging,
            IsDesignTime = IsDesignTime,
            CanClose = CanClose,
            CanFloat = CanFloat,
            CanAutoHide = CanAutoHide,
            CanPin = CanPin,
            Flavor = Flavor
        };
    }
}
