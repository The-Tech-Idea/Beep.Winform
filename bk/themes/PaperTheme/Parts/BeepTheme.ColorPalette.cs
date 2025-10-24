using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class PaperTheme
    {
        private void ApplyColorPalette()
        {
            this.ForeColor = Color.FromArgb(33,33,33);
            this.BackColor = Color.FromArgb(250,250,250);
            this.PanelBackColor = Color.FromArgb(250,250,250);
            this.PanelGradiantStartColor = ThemeUtil.Lighten(BackgroundColor, 0.02);
            this.PanelGradiantEndColor = ThemeUtil.Darken(BackgroundColor, 0.03);
            this.PanelGradiantMiddleColor = ThemeUtil.Lighten(BackgroundColor, 0.01);
            this.PanelGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.BorderColor = Color.FromArgb(224,224,224);
            this.ActiveBorderColor = Color.FromArgb(224,224,224);
            this.InactiveBorderColor = Color.FromArgb(224,224,224);
            this.PrimaryColor = Color.FromArgb(33,150,243);
            this.SecondaryColor = Color.FromArgb(0,150,136);
            this.AccentColor = Color.FromArgb(255,193,7);
            this.BackgroundColor = Color.FromArgb(250,250,250);
            this.SurfaceColor = Color.FromArgb(255,255,255);
            this.ErrorColor = Color.FromArgb(244,67,54);
            this.WarningColor = Color.FromArgb(255,160,0);
            this.SuccessColor = Color.FromArgb(76,175,80);
            this.OnPrimaryColor = Color.White;
            this.OnBackgroundColor = Color.FromArgb(33,33,33);
            this.FocusIndicatorColor = Color.FromArgb(33,150,243);
        }
    }
}