using System.Drawing;
using System.Drawing.Text;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.TextPainters
{
    /// <summary>
    /// Microsoft Design text painter using FontManagement system
    /// Supports: Fluent2, Windows11Mica, Office styles
    /// </summary>
    public class MicrosoftDesignTextPainter : DesignSystemTextPainter
    {
        #region Design System Properties
        protected override string PrimaryFontFamily => "Segoe UI Variable";
        protected override string FallbackFontFamily => "Segoe UI";

        protected override float GetFontSize(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.Fluent2 => 14f,
                BeepControlStyle.Windows11Mica => 14f,
                _ => 14f
            };
        }

        protected override float GetLetterSpacing(BeepControlStyle style)
        {
            // Microsoft uses minimal letter spacing
            return style switch
            {
                BeepControlStyle.Fluent2 => 0f,         // No tracking for Fluent
                BeepControlStyle.Windows11Mica => 0f,   // No tracking for Mica
                _ => 0f
            };
        }

        protected override TextRenderingHint GetRenderingHint()
        {
            return TextRenderingHint.ClearTypeGridFit;
        }

        protected override Color GetTextColor(BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            return GetColor(style, StyleColors.GetForeground, "Foreground", theme, useThemeColors);
        }
        #endregion

        #region Microsoft Design Font Management
        /// <summary>
        /// Get Microsoft Design font with Variable font support
        /// </summary>
        protected override Font GetFont(BeepControlStyle style, bool isFocused)
        {
            float fontSize = GetFontSize(style);
            FontStyle fontStyle = isFocused ? FontStyle.Bold : FontStyle.Regular;
            
            // Try Segoe UI Variable first (Windows 11)
            Font variableFont = FontListHelper.GetFont("Segoe UI Variable", fontSize, fontStyle);
            if (variableFont != null)
                return variableFont;
            
            // Fallback to regular Segoe UI
            return FontListHelper.GetFontWithFallback("Segoe UI", "Arial", fontSize, fontStyle);
        }
        #endregion

        #region Microsoft Design Typography Variants
        /// <summary>
        /// Paint Microsoft Design title text (for headers, titles)
        /// </summary>
        public static void PaintTitle(Graphics g, Rectangle bounds, string text,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            var painter = new MicrosoftDesignTextPainter();
            
            using (Font titleFont = painter.GetTitleFont(style))
            using (SolidBrush brush = new SolidBrush(painter.GetTextColor(style, theme, useThemeColors)))
            {
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                
                StringFormat format = new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter
                };
                
                g.DrawString(text, titleFont, brush, bounds, format);
            }
        }

        /// <summary>
        /// Paint Microsoft Design body text
        /// </summary>
        public static void PaintBody(Graphics g, Rectangle bounds, string text, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            var painter = new MicrosoftDesignTextPainter();
            ((DesignSystemTextPainter)painter).Paint(g, bounds, text, isFocused, style, theme, useThemeColors);
        }

        /// <summary>
        /// Paint Microsoft Design subtitle text
        /// </summary>
        public static void PaintSubtitle(Graphics g, Rectangle bounds, string text,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            var painter = new MicrosoftDesignTextPainter();
            
            using (Font subtitleFont = painter.GetSubtitleFont(style))
            using (SolidBrush brush = new SolidBrush(painter.GetSubtleColor(style, theme, useThemeColors)))
            {
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                g.DrawString(text, subtitleFont, brush, bounds, painter.GetStringFormat());
            }
        }

        private Font GetTitleFont(BeepControlStyle style)
        {
            float titleSize = GetFontSize(style) + 4f;  // Slightly larger for titles
            return FontListHelper.GetFontWithFallback("Segoe UI Variable", "Segoe UI", titleSize, FontStyle.Bold);
        }

        private Font GetSubtitleFont(BeepControlStyle style)
        {
            float subtitleSize = GetFontSize(style) - 1f;  // Slightly smaller for subtitles
            return FontListHelper.GetFontWithFallback("Segoe UI Variable", "Segoe UI", subtitleSize, FontStyle.Regular);
        }

        private Color GetSubtleColor(BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            Color baseColor = GetTextColor(style, theme, useThemeColors);
            // Make subtitle text more subtle (75% opacity)
            return Color.FromArgb(191, baseColor.R, baseColor.G, baseColor.B);
        }
        #endregion

        #region Microsoft Letter Spacing (None)
        /// <summary>
        /// Microsoft design doesn't use letter spacing, so we disable it
        /// </summary>
        protected override bool ShouldUseLetterSpacing(BeepControlStyle style)
        {
            return false; // Microsoft Design uses default character spacing
        }
        #endregion

        #region Static Factory Methods
        /// <summary>
        /// Paint Microsoft Design text with enhanced FontManagement
        /// </summary>
        public static void Paint(Graphics g, Rectangle bounds, string text, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors)
        {
            var painter = new MicrosoftDesignTextPainter();
            ((DesignSystemTextPainter)painter).Paint(g, bounds, text, isFocused, style, theme, useThemeColors);
        }

        /// <summary>
        /// Paint text for Office-style controls
        /// </summary>
        public static void PaintOfficeStyle(Graphics g, Rectangle bounds, string text, bool isFocused,
            IBeepTheme theme, bool useThemeColors)
        {
            Paint(g, bounds, text, isFocused, BeepControlStyle.Fluent2, theme, useThemeColors);
        }

        /// <summary>
        /// Paint text for Metro-style controls
        /// </summary>
        public static void PaintMetroStyle(Graphics g, Rectangle bounds, string text, bool isFocused,
            IBeepTheme theme, bool useThemeColors)
        {
            Paint(g, bounds, text, isFocused, BeepControlStyle.Fluent2, theme, useThemeColors);
        }
        #endregion
    }
}