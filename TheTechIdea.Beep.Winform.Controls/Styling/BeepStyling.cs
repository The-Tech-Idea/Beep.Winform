using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.BaseImage;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters;
using TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Winform.Controls.Styling.PathPainters;
using TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Winform.Controls.Styling.Spacing;
using TheTechIdea.Beep.Winform.Controls.Styling.SpinnerButtonPainters;
using TheTechIdea.Beep.Winform.Controls.Styling.TextPainters;
using TheTechIdea.Beep.Winform.Controls.Styling.Typography;

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
        /// Paint background for the current style with GraphicsPath
        /// </summary>
        public static void PaintStyleBackground(Graphics g, GraphicsPath path)
        {
            PaintStyleBackground(g, path, CurrentControlStyle);
        }
        
        /// <summary>
        /// Paint background for a specific style with GraphicsPath
        /// </summary>
        public static void PaintStyleBackground(Graphics g, GraphicsPath path, BeepControlStyle style)
        {
            PaintStyleBackground(g, path, style, UseThemeColors);
        }
        
        /// <summary>
        /// Paint background for a specific style with GraphicsPath
        /// </summary>
        public static void PaintStyleBackground(Graphics g, GraphicsPath path, BeepControlStyle style, bool useThemeColors)
        {
            if (path == null) return;

            int radius = StyleBorders.GetRadius(style);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw shadow first (behind background)
            if (StyleShadows.HasShadow(style))
            {
                GraphicsPath shadowResult;
                if (StyleShadows.UsesDualShadows(style))
                    shadowResult = NeumorphismShadowPainter.Paint(g, path, radius, style, CurrentTheme, useThemeColors);
                else
                    shadowResult = StandardShadowPainter.Paint(g, path, style, path);
            }

            // Delegate to individual style background painter
            switch (style)
            {
                case BeepControlStyle.Material3:
                    Material3BackgroundPainter.Paint(g, path, style, CurrentTheme, useThemeColors);
                    break;
                case BeepControlStyle.MaterialYou:
                    MaterialYouBackgroundPainter.Paint(g, path, style, CurrentTheme, useThemeColors);
                    break;
                case BeepControlStyle.iOS15:
                    iOS15BackgroundPainter.Paint(g, path, style, CurrentTheme, useThemeColors);
                    break;
                case BeepControlStyle.MacOSBigSur:
                    MacOSBigSurBackgroundPainter.Paint(g, path, style, CurrentTheme, useThemeColors);
                    break;
                case BeepControlStyle.Fluent2:
                    Fluent2BackgroundPainter.Paint(g, path, style, CurrentTheme, useThemeColors);
                    break;
                case BeepControlStyle.Windows11Mica:
                    Windows11MicaBackgroundPainter.Paint(g, path, style, CurrentTheme, useThemeColors);
                    break;
                case BeepControlStyle.Minimal:
                    MinimalBackgroundPainter.Paint(g, path, style, CurrentTheme, useThemeColors);
                    break;
                case BeepControlStyle.NotionMinimal:
                    NotionMinimalBackgroundPainter.Paint(g, path, style, CurrentTheme, useThemeColors);
                    break;
                case BeepControlStyle.VercelClean:
                    VercelCleanBackgroundPainter.Paint(g, path, style, CurrentTheme, useThemeColors);
                    break;
                case BeepControlStyle.Neumorphism:
                    NeumorphismBackgroundPainter.Paint(g, path, style, CurrentTheme, useThemeColors);
                    break;
                case BeepControlStyle.GlassAcrylic:
                    GlassAcrylicBackgroundPainter.Paint(g, path, style, CurrentTheme, useThemeColors);
                    break;
                case BeepControlStyle.DarkGlow:
                    DarkGlowBackgroundPainter.Paint(g, path, style, CurrentTheme, useThemeColors);
                    break;
                case BeepControlStyle.GradientModern:
                    GradientModernBackgroundPainter.Paint(g, path, style, CurrentTheme, useThemeColors);
                    break;
                case BeepControlStyle.Bootstrap:
                    BootstrapBackgroundPainter.Paint(g, path, style, CurrentTheme, useThemeColors);
                    break;
                case BeepControlStyle.TailwindCard:
                    TailwindCardBackgroundPainter.Paint(g, path, style, CurrentTheme, useThemeColors);
                    break;
                case BeepControlStyle.StripeDashboard:
                    StripeDashboardBackgroundPainter.Paint(g, path, style, CurrentTheme, useThemeColors);
                    break;
                case BeepControlStyle.FigmaCard:
                    FigmaCardBackgroundPainter.Paint(g, path, style, CurrentTheme, useThemeColors);
                    break;
                case BeepControlStyle.DiscordStyle:
                    DiscordStyleBackgroundPainter.Paint(g, path, style, CurrentTheme, useThemeColors);
                    break;
                case BeepControlStyle.AntDesign:
                    AntDesignBackgroundPainter.Paint(g, path, style, CurrentTheme, useThemeColors);
                    break;
                case BeepControlStyle.ChakraUI:
                    ChakraUIBackgroundPainter.Paint(g, path, style, CurrentTheme, useThemeColors);
                    break;
                case BeepControlStyle.PillRail:
                    PillRailBackgroundPainter.Paint(g, path, style, CurrentTheme, useThemeColors);
                    break;
            }
        }
        
        #endregion
        
        #region Style Painting Methods - Border, Text, Buttons
        
        /// <summary>
        /// Paint border for the current style with GraphicsPath
        /// </summary>
        public static void PaintStyleBorder(Graphics g, GraphicsPath path, bool isFocused)
        {
            PaintStyleBorder(g, path, isFocused, CurrentControlStyle);
        }
        
        /// <summary>
        /// Paint border for a specific style with GraphicsPath
        /// </summary>
        public static void PaintStyleBorder(Graphics g, GraphicsPath path, bool isFocused, BeepControlStyle style)
        {
            if (path == null) return;
            
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Border painting logic here if needed
        }

        /// <summary>
        /// Paint text for the current style with GraphicsPath
        /// </summary>
        public static void PaintStyleText(Graphics g, GraphicsPath path, string text, bool isFocused)
        {
            PaintStyleText(g, path, text, isFocused, CurrentControlStyle);
        }
        
        /// <summary>
        /// Paint text for a specific style with GraphicsPath
        /// </summary>
        public static void PaintStyleText(Graphics g, GraphicsPath path, string text, bool isFocused, BeepControlStyle style)
        {
            if (string.IsNullOrEmpty(text) || path == null)
                return;
            
            RectangleF boundsF = path.GetBounds();
            Rectangle bounds = Rectangle.Round(boundsF);
            
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
        public static void PaintStyleButtons(Graphics g, GraphicsPath upButtonPath, GraphicsPath downButtonPath, bool isFocused)
        {
            PaintStyleSpinnerButtons(g, upButtonPath, downButtonPath, isFocused, CurrentControlStyle);
        }
        
        /// <summary>
        /// Paint spinner buttons (up/down arrows for numeric controls) for a specific style
        /// </summary>
        public static void PaintStyleSpinnerButtons(Graphics g, GraphicsPath upButtonPath, GraphicsPath downButtonPath, bool isFocused, BeepControlStyle style)
        {
            if (upButtonPath == null || downButtonPath == null) return;
            
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            RectangleF upBoundsF = upButtonPath.GetBounds();
            RectangleF downBoundsF = downButtonPath.GetBounds();
            Rectangle upButtonRect = Rectangle.Round(upBoundsF);
            Rectangle downButtonRect = Rectangle.Round(downBoundsF);
            
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
        public static void PaintStyleValueText(Graphics g, GraphicsPath textPath, string formattedText, bool isFocused)
        {
            PaintStyleValueText(g, textPath, formattedText, isFocused, CurrentControlStyle);
        }
        
        /// <summary>
        /// Paint value text for a specific style
        /// </summary>
        public static void PaintStyleValueText(Graphics g, GraphicsPath textPath, string formattedText, bool isFocused, BeepControlStyle style)
        {
            if (string.IsNullOrEmpty(formattedText) || textPath == null)
                return;
            
            RectangleF boundsF = textPath.GetBounds();
            Rectangle textRect = Rectangle.Round(boundsF);
            
            // Delegate to value text painter
            ValueTextPainter.Paint(g, textRect, formattedText, isFocused, style, CurrentTheme, UseThemeColors);
        }
        /// <summary>
        /// Create graphics path for the current style from Rectangle
        /// </summary>
        public static GraphicsPath CreateStylePath(Rectangle bounds)
        {
            return CreateStylePath(bounds, CurrentControlStyle);
        }
        
        /// <summary>
        /// Create graphics path for a specific style from Rectangle
        /// </summary>
        public static GraphicsPath CreateStylePath(Rectangle bounds, BeepControlStyle style)
        {
            int radius = StyleBorders.GetRadius(style);
            GraphicsPath path = new GraphicsPath();
            
            if (radius == 0)
            {
                path.AddRectangle(bounds);
            }
            else
            {
                int diameter = radius * 2;
                Size size = new Size(diameter, diameter);
                Rectangle arc = new Rectangle(bounds.Location, size);
                
                path.AddArc(arc, 180, 90);
                arc.X = bounds.Right - diameter;
                path.AddArc(arc, 270, 90);
                arc.Y = bounds.Bottom - diameter;
                path.AddArc(arc, 0, 90);
                arc.X = bounds.Left;
                path.AddArc(arc, 90, 90);
                path.CloseFigure();
            }
            
            return path;
        }
        
        /// <summary>
        /// Paint styled image with rounded corners using GraphicsPath
        /// </summary>
        public static void PaintStyleImage(Graphics g, GraphicsPath path, string imagePath)
        {
            PaintStyleImage(g, path, imagePath, CurrentControlStyle);
        }
        
        /// <summary>
        /// Paint styled image for a specific style using GraphicsPath
        /// </summary>
        public static void PaintStyleImage(Graphics g, GraphicsPath path, string imagePath, BeepControlStyle style)
        {
            if (string.IsNullOrEmpty(imagePath) || path == null)
                return;
            
            // Delegate to styled image painter (uses cache)
            StyledImagePainter.Paint(g, path, imagePath, style);
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

        #region Complete Control Painting with Factories (GraphicsPath Only - Returns Internal Drawing Area)

        /// <summary>
        /// Paint complete control using all three painter systems (Shadow → Background → Border)
        /// Returns the internal drawing area (content area) GraphicsPath after accounting for borders and shadows
        /// </summary>
        /// <param name="g">Graphics context</param>
        /// <param name="controlPath">Control bounds as GraphicsPath</param>
        /// <param name="style">Control style to use</param>
        /// <param name="theme">Theme to apply</param>
        /// <param name="useThemeColors">Whether to use theme colors</param>
        /// <param name="state">Control state (Normal, Hovered, Pressed, etc.)</param>
        /// <returns>GraphicsPath representing the internal content area</returns>
        public static GraphicsPath PaintControl(
            Graphics g,
            GraphicsPath controlPath,
            BeepControlStyle style,
            IBeepTheme theme,
            bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (controlPath == null)
                return null;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            int radius = StyleBorders.GetRadius(style);
            GraphicsPath currentPath = (GraphicsPath)controlPath.Clone();

            // === STEP 1: Apply padding to get content area first ===
            GraphicsPath contentPath = currentPath;
            int padding = StyleSpacing.GetPadding(style);
            if (padding > 0)
            {
                contentPath = currentPath.CreateInsetPath(padding);
                if (contentPath == null)
                    contentPath = currentPath;
            }

            // === STEP 2: Paint Inner Shadow (paints around content area, returns smaller path) ===
            GraphicsPath pathAfterShadow = contentPath;
            if (StyleShadows.HasShadow(style) && state != ControlState.Disabled)
            {
                var shadowPainter = ShadowPainterFactory.CreatePainter(style);
                if (shadowPainter != null)
                {
                    pathAfterShadow = shadowPainter.Paint(g, contentPath, radius, theme);
                    if (pathAfterShadow == null)
                        pathAfterShadow = contentPath;
                }
            }

            // === STEP 3: Paint Border (uses shadow path, returns smaller path after border) ===
            GraphicsPath pathAfterBorder = pathAfterShadow;
            var borderPainter = BorderPainterFactory.CreatePainter(style);
            if (borderPainter != null)
            {
                bool isFocused = (state == ControlState.Focused);
                pathAfterBorder = borderPainter.Paint(g, pathAfterShadow, isFocused, style, theme, useThemeColors, state);
                if (pathAfterBorder == null)
                    pathAfterBorder = pathAfterShadow;
            }

            // === STEP 4: Paint Background (fills the area between border and final content) ===
            var backgroundPainter = BackgroundPainterFactory.CreatePainter(style);
            if (backgroundPainter != null)
            {
                backgroundPainter.Paint(g, pathAfterBorder, style, theme, useThemeColors, state);
            }

            // Cleanup intermediate paths if they're different
            if (contentPath != currentPath && contentPath != pathAfterBorder)
                contentPath.Dispose();
            if (pathAfterShadow != contentPath && pathAfterShadow != pathAfterBorder)
                pathAfterShadow.Dispose();

            return pathAfterBorder; // Return final drawing area (inside border)
        }

        /// <summary>
        /// Paint complete control with current style and theme
        /// </summary>
        public static GraphicsPath PaintControl(
            Graphics g,
            GraphicsPath controlPath,
            ControlState state = ControlState.Normal)
        {
            return PaintControl(g, controlPath, CurrentControlStyle, CurrentTheme, UseThemeColors, state);
        }

        /// <summary>
        /// Paint control with image in content area
        /// Returns the content area path after image is painted
        /// </summary>
        public static GraphicsPath PaintControlWithImage(
            Graphics g,
            GraphicsPath controlPath,
            string imagePath,
            BeepControlStyle style,
            IBeepTheme theme,
            bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Paint control layers and get content path
            GraphicsPath contentPath = PaintControl(g, controlPath, style, theme, useThemeColors, state);
            
            if (contentPath == null)
                return null;

            // Paint image in content area
            if (!string.IsNullOrEmpty(imagePath))
            {
                StyledImagePainter.Paint(g, contentPath, imagePath, style);
            }

            return contentPath; // Caller is responsible for disposing
        }

        /// <summary>
        /// Paint control with text in content area
        /// Returns the content area path after text is painted
        /// </summary>
        public static GraphicsPath PaintControlWithText(
            Graphics g,
            GraphicsPath controlPath,
            string text,
            BeepControlStyle style,
            IBeepTheme theme,
            bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Paint control layers and get content path
            GraphicsPath contentPath = PaintControl(g, controlPath, style, theme, useThemeColors, state);
            
            if (contentPath == null)
                return null;

            // Paint text in content area
            if (!string.IsNullOrEmpty(text))
            {
                RectangleF contentBounds = contentPath.GetBounds();
                bool isFocused = (state == ControlState.Focused);
                
                // Get appropriate font
                Font textFont = StyleTypography.GetFont(style);
                Color textColor = useThemeColors && theme != null 
                    ? theme.ForeColor 
                    : StyleColors.GetForeground(style);

                // Draw text centered in content area
                using (SolidBrush brush = new SolidBrush(textColor))
                {
                    StringFormat format = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center,
                        Trimming = StringTrimming.EllipsisCharacter
                    };

                    g.DrawString(text, textFont, brush, contentBounds, format);
                }
            }

            return contentPath; // Caller is responsible for disposing
        }

        /// <summary>
        /// Helper method to create rounded rectangle path from RectangleF
        /// </summary>
        private static GraphicsPath CreateRoundedRectanglePath(RectangleF bounds, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            
            if (radius <= 0 || radius > bounds.Height / 2 || radius > bounds.Width / 2)
            {
                // No radius or invalid radius - use rectangle
                path.AddRectangle(bounds);
            }
            else
            {
                float diameter = radius * 2;
                SizeF size = new SizeF(diameter, diameter);
                RectangleF arc = new RectangleF(bounds.Location, size);
                
                // Top-left arc
                path.AddArc(arc, 180, 90);
                
                // Top-right arc
                arc.X = bounds.Right - diameter;
                path.AddArc(arc, 270, 90);
                
                // Bottom-right arc
                arc.Y = bounds.Bottom - diameter;
                path.AddArc(arc, 0, 90);
                
                // Bottom-left arc
                arc.X = bounds.Left;
                path.AddArc(arc, 90, 90);
                
                path.CloseFigure();
            }
            
            return path;
        }

        /// <summary>
        /// Paint only inner shadow for a control (useful for pre-rendering shadows)
        /// Shadow is painted INSIDE the control bounds, between border and content area
        /// Returns the shadow depth used
        /// </summary>
        public static int PaintControlInnerShadow(
            Graphics g,
            GraphicsPath controlPath,
            BeepControlStyle style,
            IBeepTheme theme,
            ControlState state = ControlState.Normal)
        {
            if (controlPath == null)
                return 0;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            int radius = StyleBorders.GetRadius(style);

            int shadowDepth = 0;
            if (StyleShadows.HasShadow(style) && state != ControlState.Disabled)
            {
                shadowDepth = Math.Max(2, StyleShadows.GetShadowBlur(style) / 2);
                Color shadowColor = StyleShadows.GetShadowColor(style);
                ShadowPainterHelpers.PaintInnerShadow(g, controlPath, radius, shadowDepth, shadowColor);
            }

            return shadowDepth;
        }

        /// <summary>
        /// Get the internal content path for a control (without actually painting)
        /// Useful for layout calculations
        /// </summary>
        public static GraphicsPath GetContentPath(
            GraphicsPath controlPath,
            BeepControlStyle style)
        {
            if (controlPath == null)
                return null;

            RectangleF bounds = controlPath.GetBounds();
            float borderWidth = StyleBorders.GetBorderWidth(style);
            int padding = StyleSpacing.GetPadding(style);
            int radius = StyleBorders.GetRadius(style);
            
            // Deflate bounds for internal content area
            float inset = borderWidth + padding;
            RectangleF contentBounds = new RectangleF(
                bounds.X + inset,
                bounds.Y + inset,
                bounds.Width - (inset * 2),
                bounds.Height - (inset * 2)
            );

            // Ensure valid bounds
            if (contentBounds.Width <= 0 || contentBounds.Height <= 0)
                return null;

            float contentRadius = Math.Max(0, radius - borderWidth - (padding / 2));
            return CreateRoundedRectanglePath(contentBounds, (int)contentRadius);
        }

        /// <summary>
        /// Paint only background layer (no shadow or border)
        /// </summary>
        public static void PaintControlBackground(
            Graphics g,
            GraphicsPath controlPath,
            BeepControlStyle style,
            IBeepTheme theme,
            bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (controlPath == null) return;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            var backgroundPainter = BackgroundPainterFactory.CreatePainter(style);
            backgroundPainter?.Paint(g, controlPath, style, theme, useThemeColors, state);
        }

        /// <summary>
        /// Paint only border layer (no shadow or background)
        /// </summary>
        public static void PaintControlBorder(
            Graphics g,
            GraphicsPath controlPath,
            BeepControlStyle style,
            IBeepTheme theme,
            bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (controlPath == null) return;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            var borderPainter = BorderPainterFactory.CreatePainter(style);
            if (borderPainter != null)
            {
                bool isFocused = (state == ControlState.Focused);
                borderPainter.Paint(g, controlPath, isFocused, style, theme, useThemeColors, state);
            }
        }

        public static void PaintStyleBackground(Graphics g, Rectangle drawingRect, BeepControlStyle controlStyle)
        {
            if (g == null) throw new ArgumentNullException(nameof(g));
            GraphicsPath path = CreateStylePath(drawingRect, controlStyle);
            PaintStyleBackground(g, path, controlStyle, UseThemeColors);
        }

        #endregion
    }
}
