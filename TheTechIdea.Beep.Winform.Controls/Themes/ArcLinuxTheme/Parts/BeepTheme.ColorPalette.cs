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
            this.ForeColor = Color.FromArgb(211, 218, 227);
            this.BackColor = Color.FromArgb(64, 69, 82);
            this.PanelBackColor = BackColor;
            this.PanelGradiantStartColor = ThemeUtil.Lighten(BackColor, 0.05);
            this.PanelGradiantEndColor = ThemeUtil.Darken(BackColor, 0.05);
            this.PanelGradiantMiddleColor = ThemeUtil.Lighten(BackColor, 0.02);
            this.PanelGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.BorderColor = Color.FromArgb(64, 69, 82);
            this.ActiveBorderColor = Color.FromArgb(82, 148, 226);
            this.InactiveBorderColor = Color.FromArgb(56, 60, 74);
            this.PrimaryColor = Color.FromArgb(82,148,226);
            this.SecondaryColor = Color.FromArgb(56, 60, 74);
            this.AccentColor = Color.FromArgb(118, 182, 248);
            this.BackgroundColor = BackColor;
            this.SurfaceColor = Color.FromArgb(56, 60, 74);
            this.ErrorColor = Color.FromArgb(244,67,54);
            this.WarningColor = Color.FromArgb(255,193,7);
            this.SuccessColor = Color.FromArgb(76,175,80);
            this.OnPrimaryColor = ForeColor;
            this.OnBackgroundColor = ForeColor;
            this.FocusIndicatorColor = PrimaryColor;
        }
    }
}
