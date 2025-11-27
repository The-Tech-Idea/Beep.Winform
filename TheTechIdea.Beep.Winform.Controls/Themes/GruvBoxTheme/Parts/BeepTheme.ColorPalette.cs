using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Themes.ThemeContrastUtilities;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GruvBoxTheme
    {
        private void ApplyColorPalette()
        {
            // GruvBox theme - retro groove color scheme
            // Aligned with FormStyle.GruvBox: Background #3C3836, warm neutrals
            this.ForeColor = Color.FromArgb(235, 219, 178);  // Light beige (#EBDBB2)
            this.BackColor = Color.FromArgb(60, 56, 54);  // Dark gray-brown (#3C3836)
            this.PanelBackColor = Color.FromArgb(40, 40, 40);  // #282828
            this.PanelGradiantStartColor = Color.FromArgb(60, 56, 54);
            this.PanelGradiantEndColor = Color.FromArgb(50, 48, 47);
            this.PanelGradiantMiddleColor = Color.FromArgb(40, 40, 40);
            this.PanelGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            
            // GruvBox borders - muted tones
            this.BorderColor = Color.FromArgb(80, 73, 69);  // #504945
            this.ActiveBorderColor = Color.FromArgb(251, 73, 52);  // Red accent
            this.InactiveBorderColor = Color.FromArgb(146, 131, 116);  // #928374
            
            // GruvBox palette - warm retro colors
            this.PrimaryColor = Color.FromArgb(251, 73, 52);  // Red (#FB4934)
            this.SecondaryColor = Color.FromArgb(142, 192, 124);  // Green (#8EC07C)
            this.AccentColor = Color.FromArgb(254, 128, 25);  // Orange (#FE8019)
            this.BackgroundColor = Color.FromArgb(60, 56, 54);  // #3C3836
            this.SurfaceColor = Color.FromArgb(40, 40, 40);  // #282828
            
            // Warm status colors
            this.ErrorColor = Color.FromArgb(251, 73, 52);  // Red (#FB4934)
            this.WarningColor = Color.FromArgb(250, 189, 47);  // Yellow (#FABD2F)
            this.SuccessColor = Color.FromArgb(184, 187, 38);  // Yellow-green (#B8BB26)
            
            // On-colors for readability
            this.OnPrimaryColor = Color.FromArgb(40, 40, 40);  // Dark on colored
            this.OnBackgroundColor = Color.FromArgb(235, 219, 178);  // Light on dark
            this.FocusIndicatorColor = Color.FromArgb(250, 189, 47);  // Yellow focus (#FABD2F)
            ThemeContrastHelper.ValidateTheme(this, targetRatio: 4.5, autofix: true);
        }
    }
}
