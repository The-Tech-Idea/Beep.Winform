using System.Collections.Concurrent;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips
{
    /// <summary>
    /// Modern ToolTip Manager inspired by DevExpress, Material-UI, and Ant Design
    /// Provides centralized tooltip management with rich content, animations, positioning, theming, and accessibility
    /// 
    /// Architecture:
    /// - Static API for easy access throughout application
    /// - Instance-based lifecycle management
    /// - Painter pattern for flexible rendering
    /// - Theme integration for consistent styling
    /// - Automatic cleanup and resource management
    /// 
    /// Design Patterns Used:
    /// - Singleton (static manager)
    /// - Factory (creates tooltip instances)
    /// - Observer (event-driven control interactions)
    /// - Strategy (painter pattern for rendering)
    /// </summary>
    public static partial class ToolTipManager
    {
        #region Fields

        /// <summary>
        /// Active tooltip instances tracked by unique keys
        /// Thread-safe for concurrent access
        /// </summary>
        internal static readonly ConcurrentDictionary<string, ToolTipInstance> _activeTooltips = new();

        /// <summary>
        /// Mapping of controls to their tooltip keys
        /// Allows efficient lookup when controls are interacted with
        /// </summary>
        internal static readonly ConcurrentDictionary<Control, string> _controlTooltips = new();

        /// <summary>
        /// Cleanup timer to remove expired tooltip instances
        /// Runs every 5 seconds to prevent memory leaks
        /// </summary>
        private static readonly System.Threading.Timer _cleanupTimer;

        #endregion

        #region Static Constructor

        static ToolTipManager()
        {
            // Initialize periodic cleanup timer (every 5 seconds)
            _cleanupTimer = new System.Threading.Timer(OnCleanupTimer, null, 5000, 5000);
        }

        #endregion

        #region Global Settings

        /// <summary>
        /// Default theme for all tooltips (can be overridden per tooltip)
        /// </summary>
        public static ToolTipType DefaultType { get; set; } = ToolTipType.Default;

        /// <summary>
        /// Default visual style for all tooltips (can be overridden per tooltip)
        /// </summary>
        public static BeepControlStyle DefaultStyle { get; set; } = BeepControlStyle.Material3;

        /// <summary>
        /// Default BeepControlStyle for all tooltips (takes precedence over DefaultStyle if set)
        /// </summary>
        public static BeepControlStyle? DefaultControlStyle { get; set; } = null;

        /// <summary>
        /// Use BeepStyling theme colors by default
        /// </summary>
        public static bool DefaultUseThemeColors { get; set; } = true;

        /// <summary>
        /// Default delay before showing tooltip in milliseconds
        /// </summary>
        public static int DefaultShowDelay { get; set; } = 500;

        /// <summary>
        /// Default duration tooltip remains visible in milliseconds (0 = indefinite)
        /// </summary>
        public static int DefaultHideDelay { get; set; } = 3000;

        /// <summary>
        /// Default fade-in animation duration in milliseconds
        /// </summary>
        public static int DefaultFadeInDuration { get; set; } = 150;

        /// <summary>
        /// Default fade-out animation duration in milliseconds
        /// </summary>
        public static int DefaultFadeOutDuration { get; set; } = 100;

        /// <summary>
        /// Default placement strategy when not specified
        /// </summary>
        public static ToolTipPlacement DefaultPlacement { get; set; } = ToolTipPlacement.Auto;

        /// <summary>
        /// Enable/disable animations globally (useful for accessibility or performance)
        /// </summary>
        public static bool EnableAnimations { get; set; } = true;

        /// <summary>
        /// Enable/disable accessibility features (ARIA attributes, keyboard navigation)
        /// </summary>
        public static bool EnableAccessibility { get; set; } = true;

        #endregion

        #region State Properties

        /// <summary>
        /// Get count of currently active tooltips
        /// </summary>
        public static int ActiveTooltipCount => _activeTooltips.Count;

        /// <summary>
        /// Get count of controls with registered tooltips
        /// </summary>
        public static int RegisteredControlCount => _controlTooltips.Count;

        #endregion
    }
}
