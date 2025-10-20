using System;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm
{
    public enum RegionDock { Caption, Bottom, Left, Right, ContentOverlay }
    public enum HitAreaType
    {
        Label, Button, TextBox, Drag, Icon,Caption,Custom
    }
    public enum FormStyle
    {
        Modern,         // Borderless, custom caption with rounded corners
        Minimal,        // Thin border, minimal caption
        MacOS,          // macOS-style traffic lights (red/yellow/green)
        Fluent,         // Microsoft Fluent Design System
        Material,       // Material Design 3
        Cartoon,        // Playful cartoon-style with exaggerated shapes
        ChatBubble,     // Chat bubble speech balloon style
        Glass,          // Transparent glass/acrylic effect
        Metro,          // Windows 8/10 Metro style
        Metro2,         // Updated Metro with accent colors
        GNOME,          // GNOME/Adwaita style
        
        // New styles
        NeoMorphism,    // Soft UI with shadows and highlights
        Glassmorphism,  // Frosted glass with blur effects
        Brutalist,      // Bold, geometric, high-contrast design
        Retro,          // 80s/90s retro computing aesthetic
        Cyberpunk,      // Neon-lit futuristic style
        Nordic,         // Clean Scandinavian minimalist design
        iOS,            // Apple iOS modern style
        // Windows11,   // REMOVED - hides caption, use regular WinForms for native Windows look
        Ubuntu,         // Ubuntu/Unity style
        KDE,            // KDE Plasma style
        ArcLinux,       // Arc Linux theme style
        Dracula,        // Popular dark theme with purple accents
        Solarized,      // Solarized color scheme style
        OneDark,        // Atom One Dark theme style
        GruvBox,        // Warm retro groove color scheme
        Nord,           // Nordic-inspired color palette
        Tokyo,          // Tokyo Night theme style
        Paper,          // Flat paper material design
        Neon,           // Vibrant neon glow effects
        Holographic,    // Iridescent holographic effects
        
        Custom,          // Fully custom rendering by user
        Terminal
    }

    public sealed class FormRegion
    {
        public string Id { get; set; }
        public RegionDock Dock { get; set; }
        public Rectangle Bounds { get; set; }
        public Action<Graphics, Rectangle> OnPaint { get; set; }
        public object Tag { get; set; }
    }

    public sealed class HitArea
    {
        public string Name { get; set; }
        public Rectangle Bounds { get; set; }
        public object Data { get; set; }
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
    /// Backdrop effects for modern appearance (Windows 11 style)
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