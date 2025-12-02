using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Themes.ThemeContrastUtilities;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class SolarizedTheme
    {
        private void ApplyColorPalette()
        {
            // Solarized LIGHT theme - scientifically crafted color palette
            // Aligned with StyleColors.Solarized: Light beige background (253,246,227)
            // FIXED: Using Solarized LIGHT palette to match BeepStyling
            
            // Base colors - Solarized LIGHT mode
            this.ForeColor = Color.FromArgb(88, 110, 117);  // Dark gray-blue (#586E75) on light
            this.BackColor = Color.FromArgb(253, 246, 227);  // Light beige (#FDF6E3)
            this.PanelBackColor = Color.FromArgb(253, 246, 227);  // Light beige
            this.PanelGradiantStartColor = Color.FromArgb(253, 246, 227);
            this.PanelGradiantEndColor = Color.FromArgb(238, 232, 213);  // Slightly darker beige
            this.PanelGradiantMiddleColor = Color.FromArgb(245, 239, 220);
            this.PanelGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            
            // Solarized borders
            this.BorderColor = Color.FromArgb(147, 161, 161);  // Medium gray (#93A1A1)
            this.ActiveBorderColor = Color.FromArgb(203, 75, 22);  // Orange (#CB4B16)
            this.InactiveBorderColor = Color.FromArgb(188, 198, 198);  // Light gray (#BCC6C6)
            
            // Solarized accent palette (same in light/dark)
            this.PrimaryColor = Color.FromArgb(38, 139, 210);  // Blue (#268BD2)
            this.SecondaryColor = Color.FromArgb(42, 161, 152);  // Cyan (#2AA198)
            this.AccentColor = Color.FromArgb(203, 75, 22);  // Orange (#CB4B16)
            this.BackgroundColor = Color.FromArgb(253, 246, 227);  // Light beige (#FDF6E3)
            this.SurfaceColor = Color.FromArgb(238, 232, 213);  // Slightly darker beige (#EEE8D5)
            
            // Status colors (same in light/dark)
            this.ErrorColor = Color.FromArgb(220, 50, 47);  // Red (#DC322F)
            this.WarningColor = Color.FromArgb(181, 137, 0);  // Yellow (#B58900)
            this.SuccessColor = Color.FromArgb(133, 153, 0);  // Green (#859900)
            
            // On-colors for readability - LIGHT MODE
            this.OnPrimaryColor = Color.FromArgb(253, 246, 227);  // Light beige on blue/colored backgrounds
            this.OnBackgroundColor = Color.FromArgb(88, 110, 117);  // Dark text on light background
            this.FocusIndicatorColor = Color.FromArgb(38, 139, 210);  // Blue focus
            
            ThemeContrastHelper.ValidateTheme(this, targetRatio: 4.5, autofix: true);
        }
    }
}