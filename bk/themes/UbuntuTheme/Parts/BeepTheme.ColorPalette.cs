using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class UbuntuTheme
    {
        private void ApplyColorPalette()
        {
            this.ForeColor = Color.FromArgb(44,44,44);
            this.BackColor = Color.FromArgb(242,242,245);
            this.PanelBackColor = Color.FromArgb(242,242,245);
            this.PanelGradiantStartColor = ThemeUtil.Lighten(BackgroundColor, 0.02);
            this.PanelGradiantEndColor = ThemeUtil.Darken(BackgroundColor, 0.04);
            this.PanelGradiantMiddleColor = ThemeUtil.Lighten(BackgroundColor, 0.01);
            this.PanelGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.BorderColor = Color.FromArgb(218,218,222);
            this.ActiveBorderColor = Color.FromArgb(233,84,32);
            this.InactiveBorderColor = Color.FromArgb(192,192,196);
            this.PrimaryColor = Color.FromArgb(233,84,32);
            this.SecondaryColor = Color.FromArgb(119,33,111);
            this.AccentColor = Color.FromArgb(244,197,66);
            this.BackgroundColor = Color.FromArgb(242,242,245);
            this.SurfaceColor = Color.FromArgb(255,255,255);
            this.ErrorColor = Color.FromArgb(192,28,40);
            this.WarningColor = Color.FromArgb(229,165,10);
            this.SuccessColor = Color.FromArgb(42,168,118);
            this.OnPrimaryColor = Color.White;
            this.OnBackgroundColor = ForeColor;
            this.FocusIndicatorColor = Color.FromArgb(233,84,32);
        }
    }
}