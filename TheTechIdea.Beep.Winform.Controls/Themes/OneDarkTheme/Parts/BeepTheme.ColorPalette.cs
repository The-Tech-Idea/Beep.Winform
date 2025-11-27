using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Themes.ThemeContrastUtilities;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class OneDarkTheme
    {
        private void ApplyColorPalette()
        {
            // One Dark theme - popular dark theme for Atom/VSCode
            // Aligned with FormStyle.OneDark: Background #282C34, Text warm grey #ABB2BF
            this.ForeColor = Color.FromArgb(171, 178, 191);  // Warm grey (#ABB2BF)
            this.BackColor = Color.FromArgb(40, 44, 52);  // Dark background (#282C34)
            this.PanelBackColor = Color.FromArgb(40, 44, 52);
            this.PanelGradiantStartColor = Color.FromArgb(33, 37, 43);
            this.PanelGradiantEndColor = Color.FromArgb(33, 37, 43);
            this.PanelGradiantMiddleColor = Color.FromArgb(40, 44, 52);
            this.PanelGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            
            // One Dark borders
            this.BorderColor = Color.FromArgb(60, 66, 82);  // #3C4252
            this.ActiveBorderColor = Color.FromArgb(97, 175, 239);  // One Dark blue
            this.InactiveBorderColor = Color.FromArgb(92, 99, 112);  // #5C6370
            
            // One Dark palette - warm colors
            this.PrimaryColor = Color.FromArgb(97, 175, 239);  // One Dark blue (#61AFEF)
            this.SecondaryColor = Color.FromArgb(86, 182, 194);  // Cyan (#56B6C2)
            this.AccentColor = Color.FromArgb(198, 120, 221);  // Purple (#C678DD)
            this.BackgroundColor = Color.FromArgb(40, 44, 52);  // #282C34
            this.SurfaceColor = Color.FromArgb(33, 37, 43);  // #21252B
            
            // Status colors
            this.ErrorColor = Color.FromArgb(224, 108, 117);  // Red (#E06C75)
            this.WarningColor = Color.FromArgb(229, 192, 123);  // Orange (#E5C07B)
            this.SuccessColor = Color.FromArgb(152, 195, 121);  // Green (#98C379)
            
            // On-colors for readability
            this.OnPrimaryColor = Color.FromArgb(40, 44, 52);  // Dark on blue
            this.OnBackgroundColor = Color.FromArgb(171, 178, 191);  // Warm grey on dark
            this.FocusIndicatorColor = Color.FromArgb(97, 175, 239);  // One Dark blue focus
            ThemeContrastHelper.ValidateTheme(this, targetRatio: 4.5, autofix: true);
        }
    }
}