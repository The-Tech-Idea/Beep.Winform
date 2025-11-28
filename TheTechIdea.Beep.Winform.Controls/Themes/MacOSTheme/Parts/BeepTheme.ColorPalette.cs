using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Themes.ThemeContrastUtilities;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MacOSTheme
    {
        private void ApplyColorPalette()
        {
            this.ForeColor = Color.FromArgb(28,28,30);
            this.BackColor = Color.FromArgb(250,250,252);
            this.PanelBackColor = Color.FromArgb(250,250,252);
            this.PanelGradiantStartColor = Color.FromArgb(250,250,252);
            this.PanelGradiantEndColor = Color.FromArgb(250,250,252);
            this.PanelGradiantMiddleColor = Color.FromArgb(250,250,252);
            this.PanelGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.BorderColor = Color.FromArgb(229,229,234);
            this.ActiveBorderColor = Color.FromArgb(0,122,255);
            this.InactiveBorderColor = Color.FromArgb(199,199,204);
            this.PrimaryColor = Color.FromArgb(0,122,255);
            this.SecondaryColor = Color.FromArgb(88,86,214);
            this.AccentColor = Color.FromArgb(52,199,89);
            this.BackgroundColor = Color.FromArgb(250,250,252);
            this.SurfaceColor = Color.FromArgb(255,255,255);
            this.ErrorColor = Color.FromArgb(255,69,58);
            this.WarningColor = Color.FromArgb(255,159,10);
            this.SuccessColor = Color.FromArgb(48,209,88);
            this.OnPrimaryColor = Color.FromArgb(255,255,255);  // White text on blue primary
            this.OnBackgroundColor = ForeColor;
            this.FocusIndicatorColor = Color.FromArgb(250,250,252);
            ThemeContrastHelper.ValidateTheme(this, targetRatio: 4.5, autofix: true);
        }
       
    }
}
