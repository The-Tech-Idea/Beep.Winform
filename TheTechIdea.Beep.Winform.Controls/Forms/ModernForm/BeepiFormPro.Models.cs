using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm
{
    public enum RegionDock { Caption, Bottom, Left, Right, ContentOverlay }
    public enum HitAreaType
    {
        Label, Button, TextBox, Drag, Icon,Caption,Custom
    }

    public sealed class FormRegion
    {
        public string Id { get; set; }
        public RegionDock Dock { get; set; }
        public Rectangle Bounds { get; set; }
        public Action<Graphics, Rectangle> OnPaint { get; set; }
        public object Tag { get; set; }
        public bool IsInteractive { get; set; } = false;
        public bool IsEnabled { get; set; } = true;
        public string AccessibleName { get; set; }
        public string AccessibleDescription { get; set; }
        public string AccessibleDefaultActionDescription { get; set; }
        public AccessibleRole AccessibleRole { get; set; } = AccessibleRole.PushButton;
    }

    public sealed class HitArea
    {
        public string Name { get; set; }
        public Rectangle Bounds { get; set; }
        public object Data { get; set; }
    }

    public static class FormHitAreaNames
    {
        public const string Caption = "caption";
        public const string Title = "title";
        public const string Icon = "icon";
        public const string Close = "close";
        public const string Maximize = "maximize";
        public const string Minimize = "minimize";
        public const string Theme = "theme";
        public const string Style = "Style";
        public const string CustomAction = "customAction";
        public const string Search = "search";
        public const string Profile = "profile";
    }

    public class RegionEventArgs : EventArgs
    {
        public string RegionName { get; }
        public FormRegion Region { get; }
        public Rectangle Bounds { get; }

        public RegionEventArgs(string regionName, FormRegion region, Rectangle bounds)
        {
            RegionName = regionName;
            Region = region;
            Bounds = bounds;
        }
    }

    // Advanced modern enums for the best modern form experience

    /// <summary>
    /// Backdrop effects for modern appearance (Windows 11 Style)
    /// </summary>
    public enum BackdropEffect
    {
        None,
        Mica,
        Acrylic,
        MicaAlt,
        Blur
    }

    /// <summary>
    /// Focus indicator styles for accessibility
    /// </summary>
    public enum FocusIndicatorStyle
    {
        None,
        Subtle,
        Prominent,
        HighContrast
    }

    /// <summary>
    /// Adaptive layout modes for responsive design
    /// </summary>
    public enum AdaptiveLayoutMode
    {
        Auto,
        Compact,
        Comfortable,
        Spacious
    }

    /// <summary>
    /// Rendering quality presets for performance optimization
    /// </summary>
    public enum RenderingQuality
    {
        Auto,
        Performance,
        Balanced,
        Quality,
        Ultra
    }
}