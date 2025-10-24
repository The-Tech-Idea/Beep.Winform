using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordicTheme
    {
        private void ApplyColorPalette()
        {
            this.ForeColor = Color.FromArgb(31,41,55);
            this.BackColor = Color.FromArgb(250,250,251);
            this.PanelBackColor = Color.FromArgb(250,250,251);
            this.PanelGradiantStartColor = ThemeUtil.Lighten(BackgroundColor, 0.03);
            this.PanelGradiantEndColor = ThemeUtil.Darken(BackgroundColor, 0.04);
            this.PanelGradiantMiddleColor = ThemeUtil.Lighten(BackgroundColor, 0.01);
            this.PanelGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.BorderColor = Color.FromArgb(229,231,235);
            this.ActiveBorderColor = Color.FromArgb(46,106,149);
            this.InactiveBorderColor = Color.FromArgb(203,213,225);
            this.PrimaryColor = Color.FromArgb(46,106,149);
            this.SecondaryColor = Color.FromArgb(107,140,110);
            this.AccentColor = Color.FromArgb(216,122,52);
            this.BackgroundColor = Color.FromArgb(250,250,251);
            this.SurfaceColor = Color.FromArgb(255,255,255);
            this.ErrorColor = Color.FromArgb(220,38,38);
            this.WarningColor = Color.FromArgb(245,158,11);
            this.SuccessColor = Color.FromArgb(16,157,108);
            this.OnPrimaryColor = Color.FromArgb(46,106,149);
            this.OnBackgroundColor = ForeColor;
            this.FocusIndicatorColor = Color.FromArgb(46,106,149);
        }
    }
}