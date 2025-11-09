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

        // Dimensions
        public int ItemSize { get; set; } = 56;
        public int DockHeight { get; set; } = 72;
        public int Spacing { get; set; } = 8;
        public int Padding { get; set; } = 12;
        public int CornerRadius { get; set; } = 16;

        // Animation
        public DockAnimationStyle AnimationStyle { get; set; } = DockAnimationStyle.Spring;
        public float AnimationSpeed { get; set; } = 0.2f;
        public float MaxScale { get; set; } = 1.5f;
        public float SelectedScale { get; set; } = 1.1f;
        public int HoverOffset { get; set; } = 20;

        // Visual Effects
        public bool ShowShadow { get; set; } = true;
        public bool ShowGlow { get; set; } = false;
        public bool ShowBackground { get; set; } = true;
        public bool ShowBorder { get; set; } = true;
        public float BackgroundOpacity { get; set; } = 0.8f;
        public DockBlurIntensity BlurIntensity { get; set; } = DockBlurIntensity.Medium;

        // Icon Display
        public DockIconMode IconMode { get; set; } = DockIconMode.IconOnly;
        public bool ApplyThemeToIcons { get; set; } = true;
        public bool ShowBadges { get; set; } = false;
        public bool ShowTooltips { get; set; } = true;

        // Indicators
        public DockIndicatorStyle IndicatorStyle { get; set; } = DockIndicatorStyle.Dot;
        public Color IndicatorColor { get; set; } = Color.FromArgb(0, 122, 255);
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
        public float CurrentRotation { get; set; } = 0f;
        public float CurrentOpacity { get; set; } = 1.0f;
        public bool IsHovered { get; set; }
        public bool IsSelected { get; set; }
        public bool IsRunning { get; set; }
        public bool IsDragging { get; set; }
        public Rectangle Bounds { get; set; }
        public Rectangle HitBounds { get; set; }
        public int Index { get; set; }
        public int BadgeCount { get; set; }
    }
}
