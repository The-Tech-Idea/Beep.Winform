using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using TheTechIdea.Beep.Winform.Controls.BaseImage;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling.Spacing;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Winform.Controls.Styling.Typography;
using TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters;
using TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters;

using TheTechIdea.Beep.Winform.Controls.Styling.SpinnerButtonPainters;
using TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters;
using TheTechIdea.Beep.Winform.Controls.Styling.PathPainters;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.TextPainters;

namespace TheTechIdea.Beep.Winform.Controls.Styling
{
    /// <summary>
    /// Central styling system for Beep WinForm controls
    /// Provides unified painting methods using the helper classes
    /// </summary>
    public static class BeepStyling
    {
        // Cache for ImagePainters to avoid recreating them
        public static Dictionary<string, ImagePainter> ImageCachedPainters = new Dictionary<string, ImagePainter>();
        
        // Current style and theme
        public static BeepControlStyle CurrentControlStyle { get; set; }
        public static IBeepTheme CurrentTheme { get; set; }
        public static bool UseThemeColors { get; set; } = false;  // Global setting to use theme colors
        
        static BeepStyling()
        {
            CurrentControlStyle = BeepControlStyle.Minimal; // Default style
        }
        public static event EventHandler<BeepControlStyle> ControlStyleChanged;
        
        public static void SetControlStyle(BeepControlStyle style)
        {
            if (CurrentControlStyle != style)
            {
                CurrentControlStyle = style;
                ControlStyleChanged?.Invoke(null, style);
            }
        }
        
        public static BeepControlStyle GetControlStyle()
        {
            return CurrentControlStyle;
        }
        
        public static void ToggleControlStyle()
        {
            if (CurrentControlStyle == BeepControlStyle.Minimal)
                SetControlStyle(BeepControlStyle.StripeDashboard);
            else
                SetControlStyle(BeepControlStyle.Minimal);
        }
        
        #region Helper Methods for Common Drawing Operations
        
        /// <summary>
        /// Create rounded rectangle path
        /// </summary>
        private static GraphicsPath CreateRoundedRectangle(Rectangle bounds, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }
            
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            
            // Top left
            path.AddArc(arc, 180, 90);
            // Top right
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            // Bottom right
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            // Bottom left
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);
            
            path.CloseFigure();
            return path;
        }
        
        /// <summary>
        /// Get color based on UseThemeColors setting
        /// </summary>
        private static Color GetColor(BeepControlStyle style, Func<BeepControlStyle, Color> styleColorFunc, string themeColorKey)
        {
            if (UseThemeColors && CurrentTheme != null)
            {
                // Try to get from theme
                var themeColor = GetThemeColor(themeColorKey);
                if (themeColor != Color.Empty)
                    return themeColor;
            }
            return styleColorFunc(style);
        }

        /// <summary>
        /// Get color from theme by property name/key
        /// </summary>
        public static Color GetThemeColor(string themeColorKey)
        {
            if (CurrentTheme == null || string.IsNullOrEmpty(themeColorKey))
                return Color.Empty;

            // Use reflection to get the color property from the theme
            var property = typeof(IBeepTheme).GetProperty(themeColorKey + "Color");
            if (property != null)
            {
                try
                {
                    var value = property.GetValue(CurrentTheme);
                    if (value is Color color)
                        return color;
                }
                catch
                {
                    // Ignore reflection errors
                }
            }

            // Fallback mapping for common keys that don't follow the "XxxColor" pattern
            switch (themeColorKey.ToLowerInvariant())
            {
                case "foreground":
                    return CurrentTheme.ForeColor;
                case "background":
                    return CurrentTheme.BackColor;
                case "secondary":
                    return CurrentTheme.SecondaryColor;
                case "primary":
                    return CurrentTheme.PrimaryColor;
                case "border":
                    return CurrentTheme.BorderColor;
                case "accent":
                    return CurrentTheme.AccentColor;
                case "surface":
                    return CurrentTheme.SurfaceColor;
                case "error":
                    return CurrentTheme.ErrorColor;
                case "warning":
                    return CurrentTheme.WarningColor;
                case "success":
                    return CurrentTheme.SuccessColor;
                default:
                    // Try direct property name match
                    property = typeof(IBeepTheme).GetProperty(themeColorKey);
                    if (property != null && property.PropertyType == typeof(Color))
                    {
                        try
                        {
                            var value = property.GetValue(CurrentTheme);
                            if (value is Color color)
                                return color;
                        }
                        catch
                        {
                            // Ignore reflection errors
                        }
                    }
                    break;
            }

            return Color.Empty;
        }
        
        #endregion
        #region Style Painting Methods
        
        /// <summary>
        /// Paint background for the current style
        /// </summary>
        public static void PaintStyleBackground(Graphics g, Rectangle bounds)
        {
            PaintStyleBackground(g, bounds, CurrentControlStyle);
        }
        
        /// <summary>
        /// Paint background for a specific style
        /// </summary>
        public static void PaintStyleBackground(Graphics g, Rectangle bounds, BeepControlStyle style)
        {
            int radius = StyleBorders.GetRadius(style);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Draw shadow first (behind background)
            if (StyleShadows.HasShadow(style))
            {
                using (var shadowPath = CreateRoundedRectangle(bounds, radius))
                {
                    if (StyleShadows.UsesDualShadows(style))
                        NeumorphismShadowPainter.Paint(g, bounds, radius, style, CurrentTheme, UseThemeColors);
                    else
                        StandardShadowPainter.Paint(g, bounds, style, shadowPath);
                }
            }
            
            using (var path = CreateRoundedRectangle(bounds, radius))
            {
                // Delegate to individual style background painter
                switch (style)
                {
                    case BeepControlStyle.Material3:
                        Material3BackgroundPainter.Paint(g, bounds, path, style, CurrentTheme, UseThemeColors);
                        break;
                    case BeepControlStyle.MaterialYou:
                        MaterialYouBackgroundPainter.Paint(g, bounds, path, style, CurrentTheme, UseThemeColors);
                        break;
                    case BeepControlStyle.iOS15:
                        iOS15BackgroundPainter.Paint(g, bounds, path, style, CurrentTheme, UseThemeColors);
                        break;
                    case BeepControlStyle.MacOSBigSur:
                        MacOSBigSurBackgroundPainter.Paint(g, bounds, path, style, CurrentTheme, UseThemeColors);
                        break;
                    case BeepControlStyle.Fluent2:
                        Fluent2BackgroundPainter.Paint(g, bounds, path, style, CurrentTheme, UseThemeColors);
                        break;
                    case BeepControlStyle.Windows11Mica:
                        Windows11MicaBackgroundPainter.Paint(g, bounds, path, style, CurrentTheme, UseThemeColors);
                        break;
                    case BeepControlStyle.Minimal:
                        MinimalBackgroundPainter.Paint(g, bounds, path, style, CurrentTheme, UseThemeColors);
                        break;
                    case BeepControlStyle.NotionMinimal:
                        NotionMinimalBackgroundPainter.Paint(g, bounds, path, style, CurrentTheme, UseThemeColors);
                        break;
                    case BeepControlStyle.VercelClean:
                        VercelCleanBackgroundPainter.Paint(g, bounds, path, style, CurrentTheme, UseThemeColors);
                        break;
                    case BeepControlStyle.Neumorphism:
                        NeumorphismBackgroundPainter.Paint(g, bounds, path, style, CurrentTheme, UseThemeColors);
                        break;
                    case BeepControlStyle.GlassAcrylic:
                        GlassAcrylicBackgroundPainter.Paint(g, bounds, path, style, CurrentTheme, UseThemeColors);
                        break;
                    case BeepControlStyle.DarkGlow:
                        DarkGlowBackgroundPainter.Paint(g, bounds, path, style, CurrentTheme, UseThemeColors);
                        break;
                    case BeepControlStyle.GradientModern:
                        GradientModernBackgroundPainter.Paint(g, bounds, path, style, CurrentTheme, UseThemeColors);
                        break;
                    case BeepControlStyle.Bootstrap:
                        BootstrapBackgroundPainter.Paint(g, bounds, path, style, CurrentTheme, UseThemeColors);
                        break;
                    case BeepControlStyle.TailwindCard:
                        TailwindCardBackgroundPainter.Paint(g, bounds, path, style, CurrentTheme, UseThemeColors);
                        break;
                    case BeepControlStyle.StripeDashboard:
                        StripeDashboardBackgroundPainter.Paint(g, bounds, path, style, CurrentTheme, UseThemeColors);
                        break;
                    case BeepControlStyle.FigmaCard:
                        FigmaCardBackgroundPainter.Paint(g, bounds, path, style, CurrentTheme, UseThemeColors);
                        break;
                    case BeepControlStyle.DiscordStyle:
                        DiscordStyleBackgroundPainter.Paint(g, bounds, path, style, CurrentTheme, UseThemeColors);
                        break;
                    case BeepControlStyle.AntDesign:
                        AntDesignBackgroundPainter.Paint(g, bounds, path, style, CurrentTheme, UseThemeColors);
                        break;
                    case BeepControlStyle.ChakraUI:
                        ChakraUIBackgroundPainter.Paint(g, bounds, path, style, CurrentTheme, UseThemeColors);
                        break;
                    case BeepControlStyle.PillRail:
                        PillRailBackgroundPainter.Paint(g, bounds, path, style, CurrentTheme, UseThemeColors);
                        break;
                }
            }
        }
        
        #endregion
        
        #region Style Painting Methods - Border, Text, Buttons
        
        /// <summary>
        /// Paint border for the current style
        /// </summary>
        public static void PaintStyleBorder(Graphics g, Rectangle bounds, bool isFocused)
        {
            PaintStyleBorder(g, bounds, isFocused, CurrentControlStyle);
        }
        
        /// <summary>
        /// Paint border for a specific style
        /// </summary>
        public static void PaintStyleBorder(Graphics g, Rectangle bounds, bool isFocused, BeepControlStyle style)
        {
            int radius = StyleBorders.GetRadius(style);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
           
        }

        /// <summary>
        /// Paint text for the current style
        /// </summary>
        public static void PaintStyleText(Graphics g, Rectangle bounds, string text, bool isFocused)
        {
            PaintStyleText(g, bounds, text, isFocused, CurrentControlStyle);
        }
        
        /// <summary>
        /// Paint text for a specific style
        /// </summary>
        public static void PaintStyleText(Graphics g, Rectangle bounds, string text, bool isFocused, BeepControlStyle style)
        {
            if (string.IsNullOrEmpty(text))
                return;
            
            // Delegate to appropriate text painter
            switch (style)
            {
                case BeepControlStyle.Material3:
                case BeepControlStyle.MaterialYou:
                    MaterialTextPainter.Paint(g, bounds, text, isFocused, style, CurrentTheme, UseThemeColors);
                    break;
                
                case BeepControlStyle.iOS15:
                case BeepControlStyle.MacOSBigSur:
                    AppleTextPainter.Paint(g, bounds, text, isFocused, style, CurrentTheme, UseThemeColors);
                    break;
                
                case BeepControlStyle.DarkGlow:
                    MonospaceTextPainter.Paint(g, bounds, text, isFocused, style, CurrentTheme, UseThemeColors);
                    break;
                
                default:
                    // All other styles use standard text painter
                    StandardTextPainter.Paint(g, bounds, text, isFocused, style, CurrentTheme, UseThemeColors);
                    break;
            }
        }
        /// <summary>
        /// Paint buttons (e.g., up/down for numeric controls) for the current style
        /// </summary>
        public static void PaintStyleButtons(Graphics g, Rectangle upButtonRect, Rectangle downButtonRect, bool isFocused)
        {
           // PaintStyleButtons(g, upButtonRect, downButtonRect, isFocused, CurrentControlStyle);
        }
        
        /// <summary>
        /// Paint spinner buttons (up/down arrows for numeric controls) for a specific style
        /// </summary>
        public static void PaintStyleSpinnerButtons(Graphics g, Rectangle upButtonRect, Rectangle downButtonRect, bool isFocused, BeepControlStyle style)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Delegate to individual style button painter
            switch (style)
            {
                case BeepControlStyle.Material3:
                    Material3ButtonPainter.PaintButtons(g, upButtonRect, downButtonRect, isFocused, style, CurrentTheme, UseThemeColors);
                    break;
                case BeepControlStyle.MaterialYou:
                    MaterialYouButtonPainter.PaintButtons(g, upButtonRect, downButtonRect, isFocused, style, CurrentTheme, UseThemeColors);
                    break;
                case BeepControlStyle.iOS15:
                    iOS15ButtonPainter.PaintButtons(g, upButtonRect, downButtonRect, isFocused, style, CurrentTheme, UseThemeColors);
                    break;
                case BeepControlStyle.MacOSBigSur:
                    MacOSBigSurButtonPainter.PaintButtons(g, upButtonRect, downButtonRect, isFocused, style, CurrentTheme, UseThemeColors);
                    break;
                case BeepControlStyle.Fluent2:
                    Fluent2ButtonPainter.PaintButtons(g, upButtonRect, downButtonRect, isFocused, style, CurrentTheme, UseThemeColors);
                    break;
                case BeepControlStyle.Windows11Mica:
                    Windows11MicaButtonPainter.PaintButtons(g, upButtonRect, downButtonRect, isFocused, style, CurrentTheme, UseThemeColors);
                    break;
                case BeepControlStyle.Minimal:
                    MinimalButtonPainter.PaintButtons(g, upButtonRect, downButtonRect, isFocused, style, CurrentTheme, UseThemeColors);
                    break;
                case BeepControlStyle.NotionMinimal:
                    NotionMinimalButtonPainter.PaintButtons(g, upButtonRect, downButtonRect, isFocused, style, CurrentTheme, UseThemeColors);
                    break;
                case BeepControlStyle.VercelClean:
                    VercelCleanButtonPainter.PaintButtons(g, upButtonRect, downButtonRect, isFocused, style, CurrentTheme, UseThemeColors);
                    break;
                case BeepControlStyle.Neumorphism:
                    NeumorphismButtonPainter.PaintButtons(g, upButtonRect, downButtonRect, isFocused, style, CurrentTheme, UseThemeColors);
                    break;
                case BeepControlStyle.GlassAcrylic:
                    GlassAcrylicButtonPainter.PaintButtons(g, upButtonRect, downButtonRect, isFocused, style, CurrentTheme, UseThemeColors);
                    break;
                case BeepControlStyle.DarkGlow:
                    DarkGlowButtonPainter.PaintButtons(g, upButtonRect, downButtonRect, isFocused, style, CurrentTheme, UseThemeColors);
                    break;
                case BeepControlStyle.GradientModern:
                    GradientModernButtonPainter.PaintButtons(g, upButtonRect, downButtonRect, isFocused, style, CurrentTheme, UseThemeColors);
                    break;
                case BeepControlStyle.Bootstrap:
                    BootstrapButtonPainter.PaintButtons(g, upButtonRect, downButtonRect, isFocused, style, CurrentTheme, UseThemeColors);
                    break;
                case BeepControlStyle.TailwindCard:
                    TailwindCardButtonPainter.PaintButtons(g, upButtonRect, downButtonRect, isFocused, style, CurrentTheme, UseThemeColors);
                    break;
                case BeepControlStyle.StripeDashboard:
                    StripeDashboardButtonPainter.PaintButtons(g, upButtonRect, downButtonRect, isFocused, style, CurrentTheme, UseThemeColors);
                    break;
                case BeepControlStyle.FigmaCard:
                    FigmaCardButtonPainter.PaintButtons(g, upButtonRect, downButtonRect, isFocused, style, CurrentTheme, UseThemeColors);
                    break;
                case BeepControlStyle.DiscordStyle:
                    DiscordStyleButtonPainter.PaintButtons(g, upButtonRect, downButtonRect, isFocused, style, CurrentTheme, UseThemeColors);
                    break;
                case BeepControlStyle.AntDesign:
                    AntDesignButtonPainter.PaintButtons(g, upButtonRect, downButtonRect, isFocused, style, CurrentTheme, UseThemeColors);
                    break;
                case BeepControlStyle.ChakraUI:
                    ChakraUIButtonPainter.PaintButtons(g, upButtonRect, downButtonRect, isFocused, style, CurrentTheme, UseThemeColors);
                    break;
                case BeepControlStyle.PillRail:
                    PillRailButtonPainter.PaintButtons(g, upButtonRect, downButtonRect, isFocused, style, CurrentTheme, UseThemeColors);
                    break;
            }
        }
        /// <summary>
        /// Paint value text (e.g., for numeric or date controls) for the current style
        /// </summary>
        public static void PaintStyleValueText(Graphics g, Rectangle textRect, string formattedText, bool isFocused)
        {
            PaintStyleValueText(g, textRect, formattedText, isFocused, CurrentControlStyle);
        }
        
        /// <summary>
        /// Paint value text for a specific style
        /// </summary>
        public static void PaintStyleValueText(Graphics g, Rectangle textRect, string formattedText, bool isFocused, BeepControlStyle style)
        {
            if (string.IsNullOrEmpty(formattedText))
                return;
            
            // Delegate to value text painter
            ValueTextPainter.Paint(g, textRect, formattedText, isFocused, style, CurrentTheme, UseThemeColors);
        }
        /// <summary>
        /// Create graphics path for the current style
        /// </summary>
        public static GraphicsPath CreateStylePath(Rectangle bounds)
        {
            return CreateStylePath(bounds, CurrentControlStyle);
        }
        
        /// <summary>
        /// Create graphics path for a specific style
        /// </summary>
        public static GraphicsPath CreateStylePath(Rectangle bounds, BeepControlStyle style)
        {
            int radius = StyleBorders.GetRadius(style);
            return CreateRoundedRectangle(bounds, radius);
        }
        
        /// <summary>
        /// Paint styled path with fill
        /// </summary>
        public static void PaintStylePath(Graphics g, Rectangle bounds, int radius)
        {
            PaintStylePath(g, bounds, radius, CurrentControlStyle);
        }
        
        /// <summary>
        /// Paint styled path for a specific style
        /// </summary>
        public static void PaintStylePath(Graphics g, Rectangle bounds, int radius, BeepControlStyle style)
        {
            // Delegate to solid path painter
           // SolidPathPainter.Paint(g, bounds, radius, style, CurrentTheme, UseThemeColors);
        }
        
        /// <summary>
        /// Paint styled image with rounded corners using image path and cache
        /// </summary>
        public static void PaintStyleImage(Graphics g, Rectangle bounds, string imagePath)
        {
            PaintStyleImage(g, bounds, imagePath, CurrentControlStyle);
        }
        
        /// <summary>
        /// Paint styled image for a specific style using image path and cache
        /// </summary>
        public static void PaintStyleImage(Graphics g, Rectangle bounds, string imagePath, BeepControlStyle style)
        {
            if (string.IsNullOrEmpty(imagePath))
                return;
            
            // Delegate to styled image painter (uses cache)
            StyledImagePainter.Paint(g, bounds, imagePath, style);
        }
        
        /// <summary>
        /// Clear image painter cache
        /// </summary>
        public static void ClearImageCache()
        {
            StyledImagePainter.ClearCache();
        }
        
        /// <summary>
        /// Remove specific image from cache
        /// </summary>
        public static void RemoveImageFromCache(string imagePath)
        {
            StyledImagePainter.RemoveFromCache(imagePath);
        }
        
        #endregion
    }
}
