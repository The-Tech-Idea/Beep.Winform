using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Docks
{
    /// <summary>
    /// Configuration for dock appearance and behavior
    /// </summary>
    public class DockConfig
    {
        // Style and Theme
        public DockStyle Style { get; set; } = DockStyle.AppleDock;
        public DockPosition Position { get; set; } = DockPosition.Bottom;
        public DockOrientation Orientation { get; set; } = DockOrientation.Horizontal;
        public DockAlignment Alignment { get; set; } = DockAlignment.Center;

        /// <summary>
        /// Device pixels per logical pixel. Written by <c>BeepDock</c>; 1.0 until it knows better.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Painters receive a <see cref="System.Drawing.Graphics"/> and no <see cref="System.Windows.Forms.Control"/>,
        /// so the only DPI source available to them was <c>DpiScalingHelper.GetDpiScaleFactor(Graphics)</c> -
        /// the overload that helper explicitly warns against ("Graphics.DpiX can return incorrect
        /// values... Prefer GetDpiScaleFactor(Control) when available"). In a WinForms paint handler
        /// it commonly reports 96 whatever the monitor is doing, because WinForms scales the control's
        /// bounds instead. So the two painters that appeared to be DPI-aware were reading a number
        /// that is right in an offscreen bitmap and wrong on screen.
        /// </para>
        /// <para>
        /// The authoritative value is <c>Control.DeviceDpi</c>, and this carries it to the painters
        /// the same way <see cref="UseThemeColors"/> does, for the same reason: painters are shared
        /// singletons and cannot hold per-control state.
        /// </para>
        /// </remarks>
        public float DpiScale { get; set; } = 1.0f;

        private int ScaleUp(int logical) =>
            DpiScale > 0f && Math.Abs(DpiScale - 1.0f) > 0.001f
                ? (int)Math.Round(logical * DpiScale)
                : logical;

        // Dimensions
        //
        // Each of these holds only what the user actually set. Unset, they resolve from the single
        // per-style table in DockPainterMetrics, so a style change moves them without anything having
        // to write them - which is what BeepDock's style setter used to do, discarding whatever the
        // user had chosen (set ItemSize = 40, change style, get 44).
        //
        // The getters return DEVICE pixels: they are read by the layout helper and by the painters,
        // and those two must agree or the chrome will not line up with the geometry. Scaling here is
        // the one boundary where that consistency is guaranteed for all 19 painters at once, without
        // each of them having to remember. The *Logical properties expose the unscaled value for the
        // designer, which must round-trip what the user typed rather than what this monitor renders.

        private int? _itemSize;
        public int ItemSize
        {
            get => ScaleUp(ItemSizeLogical);
            set => _itemSize = value;
        }
        public int ItemSizeLogical => _itemSize ?? DockPainterMetrics.DimensionsFor(Style).ItemSize;

        private int? _dockHeight;
        public int DockHeight
        {
            get => ScaleUp(DockHeightLogical);
            set => _dockHeight = value;
        }
        public int DockHeightLogical => _dockHeight ?? DockPainterMetrics.DimensionsFor(Style).DockHeight;

        private int? _spacing;
        public int Spacing
        {
            get => ScaleUp(SpacingLogical);
            set => _spacing = value;
        }
        public int SpacingLogical => _spacing ?? DockPainterMetrics.DimensionsFor(Style).Spacing;

        private int? _padding;
        public int Padding
        {
            get => ScaleUp(PaddingLogical);
            set => _padding = value;
        }
        public int PaddingLogical => _padding ?? DockPainterMetrics.DimensionsFor(Style).Padding;

        private int? _cornerRadius;
        public int CornerRadius
        {
            get => ScaleUp(CornerRadiusLogical);
            set => _cornerRadius = value;
        }
        public int CornerRadiusLogical => _cornerRadius ?? DockPainterMetrics.DimensionsFor(Style).CornerRadius;

        /// <summary>Returns a dimension to following the style, undoing an explicit assignment.</summary>
        public void ResetDimensions()
        {
            _itemSize = null;
            _dockHeight = null;
            _spacing = null;
            _padding = null;
            _cornerRadius = null;
            _maxScale = null;
            _showShadow = null;
            _backgroundOpacity = null;
        }

        // Animation
        public DockAnimationStyle AnimationStyle { get; set; } = DockAnimationStyle.Spring;

        /// <summary>
        /// How long a scale animation takes, in seconds.
        /// </summary>
        /// <remarks>
        /// This is <see cref="AnimationSpeed"/> under a name that says what it is. It was a per-tick
        /// lerp fraction, which is why no easing curve could be applied to it - a curve needs
        /// progress through a duration, and a fraction-per-tick has neither.
        /// </remarks>
        public float AnimationDuration { get; set; } = 0.2f;

        /// <summary>
        /// Obsolete alias for <see cref="AnimationDuration"/>, kept so the published control property
        /// and any saved designer state keep working. Same value, honest name.
        /// </summary>
        public float AnimationSpeed
        {
            get => AnimationDuration;
            set => AnimationDuration = value;
        }
        private float? _maxScale;
        public float MaxScale
        {
            get => _maxScale ?? DockPainterMetrics.DimensionsFor(Style).MaxScale;
            set => _maxScale = value;
        }
        public float SelectedScale { get; set; } = 1.1f;
        /// <summary>
        /// Pixels the hovered item lifts off the dock. 0 = no lift.
        /// </summary>
        /// <remarks>
        /// Defaulted to 20 while nothing read it. Now that the layout honours it, the default is 0 so
        /// that implementing the property does not restyle every dock that never asked for a lift.
        /// </remarks>
        public int HoverOffset { get; set; } = 0;
        public int HoverEnterDelay { get; set; } = 120;
        public float PressedScale { get; set; } = 0.95f;
        public int DragHysteresis { get; set; } = 8;

        // Visual Effects
        private bool? _showShadow;
        public bool ShowShadow
        {
            get => _showShadow ?? DockPainterMetrics.DimensionsFor(Style).ShowShadow;
            set => _showShadow = value;
        }
        // ShowGlow lived here with zero readers. DockPainterMetrics.ShowGlow is the live one, set
        // per style and read by the painters that use metrics - two identically named flags, one
        // real, one decorative. The dead one is gone.
        public bool ShowBackground { get; set; } = true;
        public bool ShowBorder { get; set; } = true;
        private float? _backgroundOpacity;
        public float BackgroundOpacity
        {
            get => _backgroundOpacity ?? DockPainterMetrics.DimensionsFor(Style).BackgroundOpacity;
            set => _backgroundOpacity = value;
        }
        // BlurIntensity had no reader anywhere and was not even published - it existed only to be
        // assigned. Glassmorphism already blurs via DockPainterMetrics.BackgroundBlur. Removed
        // rather than left as a setting that looks like it does something.

        // Icon Display
        public DockIconMode IconMode { get; set; } = DockIconMode.IconOnly;
        public bool ApplyThemeToIcons { get; set; } = true;
        public bool ShowBadges { get; set; } = false;
        public bool ShowTooltips { get; set; } = true;

        // Indicators
        public DockIndicatorStyle IndicatorStyle { get; set; } = DockIndicatorStyle.Dot;
        private Color? _indicatorColor;

        /// <summary>
        /// Accent for indicators. Unset, it follows the style.
        /// </summary>
        /// <remarks>
        /// This was a non-nullable <see cref="Color"/> with a default, so there was no value meaning
        /// "the user did not choose one" - and every resolver that forwarded it therefore treated the
        /// default iOS blue as a deliberate choice, which meant a style's own accent could never win.
        /// Same nullable-backing as the dimensions in stage 03, for the same reason.
        /// </remarks>
        public Color IndicatorColor
        {
            get => _indicatorColor ?? DockPainterMetrics.AccentFor(Style);
            set => _indicatorColor = value;
        }

        /// <summary>The accent the user actually set, or null. Used by the painters' resolvers.</summary>
        public Color? IndicatorColorOrNull => _indicatorColor;
        public bool ShowRunningIndicator { get; set; } = true;

        // Separators
        public DockSeparatorStyle SeparatorStyle { get; set; } = DockSeparatorStyle.None;
        public Color SeparatorColor { get; set; } = Color.FromArgb(100, 255, 255, 255);

        // Behavior
        public bool EnableDrag { get; set; } = false;
        public bool EnableReorder { get; set; } = false;
        public bool EnableContextMenu { get; set; } = true;
        public bool AutoHide { get; set; } = false;
        public int AutoHideDelay { get; set; } = 2000;
        public bool EnableOverflow { get; set; } = true;

        /// <summary>
        /// Mirrors <c>BaseControl.UseThemeColors</c> so painters can honour it.
        /// </summary>
        /// <remarks>
        /// Painters are shared singletons held in <c>DockPainterFactory</c>'s static dictionary, so
        /// this cannot be a property on the painter - per-control state on a shared painter is the
        /// bug class this stage exists to remove. It rides here instead, and <c>BeepDock.ApplyTheme</c>
        /// is the single place that writes it. Before this existed, no painter could see the flag:
        /// <c>DockPainterBase</c> guessed it as "a theme was supplied" and the folder's whole theme
        /// layer, <c>DockThemeHelpers</c>, had no painter calling it.
        /// </remarks>
        public bool UseThemeColors { get; set; } = true;

        // Colors (nullable for theme override)
        public Color? BackgroundColor { get; set; }
        public Color? BorderColor { get; set; }
        public Color? ForegroundColor { get; set; }
        public Color? HoverColor { get; set; }
        public Color? SelectedColor { get; set; }

        // Custom Properties
        public object Tag { get; set; }
    }

    /// <summary>
    /// State information for a single dock item
    /// </summary>
    public class DockItemState
    {
        public SimpleItem Item { get; set; }
        public float CurrentScale { get; set; } = 1.0f;
        public float TargetScale { get; set; } = 1.0f;

        // An easing curve needs a progress value, and the old animation had none: it approached the
        // target by a fixed fraction per tick, so there was no `t` to hand to EaseOutBounce. These
        // three give each item an animation with a beginning, so the curve has something to evaluate.

        /// <summary>Scale this item started its current animation from.</summary>
        public float AnimationFromScale { get; set; } = 1.0f;

        /// <summary>Target the running animation is heading to; used to notice a new target.</summary>
        public float AnimationToScale { get; set; } = 1.0f;

        /// <summary>Seconds elapsed in the current animation.</summary>
        public float AnimationElapsed { get; set; }

        /// <summary>0..1 position in the pulse cycle, for <see cref="DockAnimationStyle.Pulse"/>.</summary>
        public float PulsePhase { get; set; }
        public float CurrentRotation { get; set; } = 0f;
        public float CurrentOpacity { get; set; } = 1.0f;
        public bool IsHovered { get; set; }
        public bool IsPressed { get; set; }
        public bool IsFocused { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsSelected { get; set; }
        public bool IsRunning { get; set; }
        public bool IsDragging { get; set; }
        public Rectangle Bounds { get; set; }
        public Rectangle HitBounds { get; set; }
        public int Index { get; set; }
        public int BadgeCount { get; set; }
    }
}
