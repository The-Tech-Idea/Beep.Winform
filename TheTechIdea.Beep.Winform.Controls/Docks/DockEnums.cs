using System;

namespace TheTechIdea.Beep.Winform.Controls.Docks
{
    /// <summary>
    /// Visual style for the dock
    /// Each style has a corresponding painter implementation
    /// </summary>
    public enum DockStyle
    {
        /// <summary>
        /// macOS-style dock with magnification effect and glass background
        /// </summary>
        AppleDock = 0,

        /// <summary>
        /// Windows 11 taskbar style with centered icons and rounded container
        /// </summary>
        Windows11Dock = 1,

        /// <summary>
        /// Material Design 3 dock with elevation shadows and color fills
        /// </summary>
        Material3Dock = 2,

        /// <summary>
        /// Minimal flat design with subtle hover effects
        /// </summary>
        MinimalDock = 3,

        /// <summary>
        /// Glassmorphism style with blur, transparency, and frosted glass effect
        /// </summary>
        GlassmorphismDock = 4,

        /// <summary>
        /// Neumorphism soft UI with inset/outset shadows
        /// </summary>
        NeumorphismDock = 5,

        /// <summary>
        /// Floating pill-shaped dock with rounded ends
        /// </summary>
        PillDock = 6,

        /// <summary>
        /// iOS-style dock with app icon badges and bounce animations
        /// </summary>
        iOSDock = 7,

        /// <summary>
        /// Ubuntu/GNOME dock style with vertical orientation support
        /// </summary>
        GNOMEDock = 8,

        /// <summary>
        /// KDE Plasma-style panel with extensive customization
        /// </summary>
        PlasmaPanel = 9,

        /// <summary>
        /// Elementary OS Plank-style minimalist dock
        /// </summary>
        PlankDock = 10,

        /// <summary>
        /// Neon glow effect with vibrant colors
        /// </summary>
        NeonDock = 11,

        /// <summary>
        /// Retro-futuristic cyberpunk style with angular shapes
        /// </summary>
        CyberpunkDock = 12,

        /// <summary>
        /// Terminal/console themed dock with monospace aesthetic
        /// </summary>
        TerminalDock = 13,

        /// <summary>
        /// Floating bubble-style dock with circular items
        /// </summary>
        BubbleDock = 14,

        /// <summary>
        /// Arc Linux/i3wm style minimal status bar
        /// </summary>
        ArcDock = 15,

        /// <summary>
        /// Dracula theme inspired dark dock
        /// </summary>
        DraculaDock = 16,

        /// <summary>
        /// Nord theme inspired cool-toned dock
        /// </summary>
        NordDock = 17,

        /// <summary>
        /// Custom style using DockConfig properties
        /// </summary>
        Custom = 99
    }

    /// <summary>
    /// Position of the dock on the screen
    /// </summary>
    public enum DockPosition
    {
        /// <summary>
        /// Positioned at the top of the container
        /// </summary>
        Top,

        /// <summary>
        /// Positioned at the bottom of the container
        /// </summary>
        Bottom,

        /// <summary>
        /// Positioned on the left side of the container
        /// </summary>
        Left,

        /// <summary>
        /// Positioned on the right side of the container
        /// </summary>
        Right,

        /// <summary>
        /// Centered in the container
        /// </summary>
        Center,

        /// <summary>
        /// Floating position that can be dragged
        /// </summary>
        Floating
    }

    /// <summary>
    /// Orientation of dock items
    /// </summary>
    public enum DockOrientation
    {
        /// <summary>
        /// Items arranged horizontally (left to right)
        /// </summary>
        Horizontal,

        /// <summary>
        /// Items arranged vertically (top to bottom)
        /// </summary>
        Vertical
    }

    /// <summary>
    /// Alignment of dock items within the dock
    /// </summary>
    public enum DockAlignment
    {
        /// <summary>
        /// Items start from the beginning (left/top)
        /// </summary>
        Start,

        /// <summary>
        /// Items centered within dock
        /// </summary>
        Center,

        /// <summary>
        /// Items aligned to end (right/bottom)
        /// </summary>
        End,

        /// <summary>
        /// Items spread with space between
        /// </summary>
        SpaceBetween,

        /// <summary>
        /// Items spread with space around
        /// </summary>
        SpaceAround,

        /// <summary>
        /// Items spread evenly
        /// </summary>
        SpaceEvenly
    }

    /// <summary>
    /// Animation style for dock interactions
    /// </summary>
    public enum DockAnimationStyle
    {
        /// <summary>
        /// No animation
        /// </summary>
        None,

        /// <summary>
        /// macOS-style spring magnification
        /// </summary>
        Spring,

        /// <summary>
        /// Smooth scale transition
        /// </summary>
        Scale,

        /// <summary>
        /// Bounce effect on hover
        /// </summary>
        Bounce,

        /// <summary>
        /// Elastic animation
        /// </summary>
        Elastic,

        /// <summary>
        /// Fade in/out
        /// </summary>
        Fade,

        /// <summary>
        /// Slide from position
        /// </summary>
        Slide,

        /// <summary>
        /// Pulse effect
        /// </summary>
        Pulse,

        /// <summary>
        /// Rotate on hover
        /// </summary>
        Rotate
    }

    /// <summary>
    /// Icon display mode
    /// </summary>
    public enum DockIconMode
    {
        /// <summary>
        /// Icon only
        /// </summary>
        IconOnly,

        /// <summary>
        /// Icon with label below
        /// </summary>
        IconWithLabel,

        /// <summary>
        /// Icon with label on hover
        /// </summary>
        IconWithHoverLabel,

        /// <summary>
        /// Large icon with detailed info
        /// </summary>
        DetailedIcon
    }

    /// <summary>
    /// Background blur intensity for glassmorphism effects
    /// </summary>
    public enum DockBlurIntensity
    {
        None = 0,
        Low = 5,
        Medium = 10,
        High = 20,
        Maximum = 30
    }

    /// <summary>
    /// Separator style between dock items
    /// </summary>
    public enum DockSeparatorStyle
    {
        None,
        Line,
        Dot,
        Space,
        Custom
    }

    /// <summary>
    /// Indicator style for active/selected items
    /// </summary>
    public enum DockIndicatorStyle
    {
        /// <summary>
        /// No indicator
        /// </summary>
        None,

        /// <summary>
        /// Dot indicator below/beside item
        /// </summary>
        Dot,

        /// <summary>
        /// Line indicator below/beside item
        /// </summary>
        Line,

        /// <summary>
        /// Glow effect around item
        /// </summary>
        Glow,

        /// <summary>
        /// Highlight background
        /// </summary>
        Background,

        /// <summary>
        /// Border around item
        /// </summary>
        Border,

        /// <summary>
        /// Badge on corner
        /// </summary>
        Badge
    }
}
