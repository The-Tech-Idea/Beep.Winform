using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips
{
    /// <summary>
    /// Tooltip type defining semantic purpose and styling
    /// Based on best practices from Material-UI, Ant Design, Chakra UI, Bootstrap, and DevExpress
    /// </summary>
    public enum ToolTipType
    {
        /// <summary>
        /// Default/standard informational tooltip
        /// Neutral colors, used for general information
        /// </summary>
        Default,

        /// <summary>
        /// Primary brand color theme
        /// Used for primary actions or brand-focused information
        /// </summary>
        Primary,

        /// <summary>
        /// Secondary brand color theme
        /// Used for secondary actions or supporting information
        /// </summary>
        Secondary,

        /// <summary>
        /// Accent/highlight color theme
        /// Used to draw attention to important but non-critical info
        /// </summary>
        Accent,

        /// <summary>
        /// Success/positive action theme (green)
        /// Used for confirmations, completed actions, positive feedback
        /// </summary>
        Success,

        /// <summary>
        /// Warning/caution theme (orange/yellow)
        /// Used for warnings, potential issues, actions requiring attention
        /// </summary>
        Warning,

        /// <summary>
        /// Error/danger theme (red)
        /// Used for errors, critical issues, destructive actions
        /// </summary>
        Error,

        /// <summary>
        /// Informational theme (blue)
        /// Used for helpful information, tips, documentation links
        /// </summary>
        Info,

        /// <summary>
        /// Help/question theme
        /// Used for contextual help, explanations, "what is this?"
        /// </summary>
        Help,

        /// <summary>
        /// Validation feedback theme
        /// Used for form validation messages and field feedback
        /// </summary>
        Validation,

        /// <summary>
        /// Interactive/rich tooltip
        /// Contains interactive elements like buttons, links, or forms
        /// Inspired by Ant Design Popover and Material-UI Popper
        /// </summary>
        Interactive,

        /// <summary>
        /// Descriptive/detailed tooltip
        /// Longer descriptions, documentation, feature explanations
        /// Multi-paragraph content with formatting
        /// </summary>
        Descriptive,

        /// <summary>
        /// Notification/alert style
        /// Similar to toast notifications but anchored to element
        /// Used for real-time updates, status changes
        /// </summary>
        Notification,

        /// <summary>
        /// Tutorial/walkthrough step
        /// Used in guided tours, onboarding flows, feature introductions
        /// Includes step indicators, navigation buttons
        /// </summary>
        Tutorial,

        /// <summary>
        /// Keyboard shortcut indicator
        /// Shows available keyboard shortcuts for actions
        /// Inspired by VSCode and modern IDEs
        /// </summary>
        Shortcut,

        /// <summary>
        /// Badge/label supplementary info
        /// Additional context for badges, labels, status indicators
        /// </summary>
        Badge,

        /// <summary>
        /// Preview/peek tooltip
        /// Shows preview of content (image, file, link preview)
        /// Inspired by GitHub hover cards and link previews
        /// </summary>
        Preview,

        /// <summary>
        /// Context menu tooltip
        /// Explains menu items in context menus
        /// Shows shortcuts and additional details
        /// </summary>
        ContextMenu,

        /// <summary>
        /// Status indicator tooltip
        /// Shows status details (online/offline, health, progress)
        /// </summary>
        Status,

        /// <summary>
        /// Hint/suggestion tooltip
        /// Provides helpful hints, suggestions, auto-complete details
        /// Inspired by IDE IntelliSense tooltips
        /// </summary>
        Hint,

        /// <summary>
        /// Custom colors and styling defined in config
        /// Full control over appearance
        /// </summary>
        Custom
    }

    /// <summary>
    /// Placement position relative to target element
    /// Based on Popper.js and Material-UI positioning system
    /// </summary>
    public enum ToolTipPlacement
    {
        /// <summary>
        /// Automatically determine best placement
        /// </summary>
        Auto,

        /// <summary>
        /// Above target, center aligned
        /// </summary>
        Top,

        /// <summary>
        /// Above target, left aligned
        /// </summary>
        TopStart,

        /// <summary>
        /// Above target, right aligned
        /// </summary>
        TopEnd,

        /// <summary>
        /// Below target, center aligned
        /// </summary>
        Bottom,

        /// <summary>
        /// Below target, left aligned
        /// </summary>
        BottomStart,

        /// <summary>
        /// Below target, right aligned
        /// </summary>
        BottomEnd,

        /// <summary>
        /// Left of target, center aligned
        /// </summary>
        Left,

        /// <summary>
        /// Left of target, top aligned
        /// </summary>
        LeftStart,

        /// <summary>
        /// Left of target, bottom aligned
        /// </summary>
        LeftEnd,

        /// <summary>
        /// Right of target, center aligned
        /// </summary>
        Right,

        /// <summary>
        /// Right of target, top aligned
        /// </summary>
        RightStart,

        /// <summary>
        /// Right of target, bottom aligned
        /// </summary>
        RightEnd
    }

    /// <summary>
    /// Animation type for tooltip show/hide transitions
    /// Inspired by Ant Design and Framer Motion animations
    /// </summary>
    public enum ToolTipAnimation
    {
        /// <summary>
        /// No animation
        /// </summary>
        None,

        /// <summary>
        /// Fade in/out opacity transition
        /// </summary>
        Fade,

        /// <summary>
        /// Scale up/down from center
        /// </summary>
        Scale,

        /// <summary>
        /// Slide in/out from placement direction
        /// </summary>
        Slide,

        /// <summary>
        /// Bounce effect on entry
        /// </summary>
        Bounce
    }
}
