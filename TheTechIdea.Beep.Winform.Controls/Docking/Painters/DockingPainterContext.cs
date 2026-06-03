using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Painters
{
    /// <summary>
    /// Mutable render context passed to every <see cref="IDockingElementRenderer"/>.
    /// Carries the theme, resolved docking palette, control style, interaction state, and
    /// geometry. Values are updated in-place before each paint pass so the same instance
    /// can be reused across frames — no per-paint allocation.
    /// </summary>
    internal sealed class DockingPainterContext
    {
        public IBeepTheme Theme { get; set; }
        public DockingThemeColors Colors { get; set; } = DockingThemeColors.Default;
        public BeepControlStyle Style { get; set; } = BeepControlStyle.Material3;
        public bool UseThemeColors { get; set; } = true;
        public Rectangle Bounds { get; set; }
        public float DpiScale { get; set; } = 1f;

        public bool IsActive { get; set; }
        public bool IsHover { get; set; }
        public bool IsPressed { get; set; }
        public bool IsFocused { get; set; }
        public bool IsDragging { get; set; }
        public bool IsDesignTime { get; set; }

        public bool CanClose { get; set; }
        public bool CanFloat { get; set; }
        public bool CanAutoHide { get; set; }
        public bool CanPin { get; set; }

        /// <summary>Per-style chrome tuning (corner radii, accent width, grip style).</summary>
        public DockingStyleFlavor Flavor { get; set; } = DockingStyleFlavor.Default;

        /// <summary>
        /// Updates the context in-place with the most common paint-time values. No allocation.
        /// </summary>
        internal void Update(DockingThemeColors colors, BeepControlStyle style, Rectangle bounds,
            bool isDesignTime = false)
        {
            Colors = colors;
            Style = style;
            Bounds = bounds;
            IsDesignTime = isDesignTime;
            Flavor = DockingPainterFactory.ResolveFlavor(style);
            IsActive = false;
            IsHover = false;
            IsPressed = false;
            IsFocused = false;
            IsDragging = false;
        }
    }
}

