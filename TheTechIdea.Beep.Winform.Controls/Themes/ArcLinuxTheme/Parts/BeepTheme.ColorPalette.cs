using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ArcLinuxTheme
    {
        private void ApplyColorPalette()
        {
            this.ForeColor = Color.FromArgb(43,45,48);
            this.BackColor = Color.FromArgb(245,246,247);
            this.PanelBackColor = Color.FromArgb(245,246,247);
            this.PanelGradiantStartColor = ThemeUtil.Lighten(BackgroundColor, 0.02);
            this.PanelGradiantEndColor = ThemeUtil.Darken(BackgroundColor, 0.04);
            this.PanelGradiantMiddleColor = ThemeUtil.Lighten(BackgroundColor, 0.01);
            this.PanelGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.BorderColor = Color.FromArgb(220,223,230);
            this.ActiveBorderColor = Color.FromArgb(82,148,226);
            this.InactiveBorderColor = Color.FromArgb(200,205,210);
            this.PrimaryColor = Color.FromArgb(82,148,226);
            this.SecondaryColor = Color.FromArgb(27,106,203);
            this.AccentColor = Color.FromArgb(76,201,240);
            this.BackgroundColor = Color.FromArgb(245,246,247);
            this.SurfaceColor = Color.FromArgb(255,255,255);
            this.ErrorColor = Color.FromArgb(244,67,54);
            this.WarningColor = Color.FromArgb(255,193,7);
            this.SuccessColor = Color.FromArgb(76,175,80);
            this.OnPrimaryColor = Color.FromArgb(82,148,226);
            this.OnBackgroundColor = ForeColor;
            this.FocusIndicatorColor = Color.FromArgb(82,148,226);
        }
    }
}