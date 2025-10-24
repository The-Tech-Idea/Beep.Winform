using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeoMorphismTheme
    {
        private void ApplyColorPalette()
        {
            this.ForeColor = Color.FromArgb(58,66,86);
            this.BackColor = Color.FromArgb(236,240,243);
            this.PanelBackColor = Color.FromArgb(236,240,243);
            this.PanelGradiantStartColor = ThemeUtil.Lighten(BackgroundColor, 0.06);
            this.PanelGradiantEndColor = ThemeUtil.Darken(BackgroundColor, 0.07);
            this.PanelGradiantMiddleColor = ThemeUtil.Lighten(BackgroundColor, 0.03);
            this.PanelGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
            this.BorderColor = Color.FromArgb(221,228,235);
            this.ActiveBorderColor = Color.FromArgb(76,110,245);
            this.InactiveBorderColor = Color.FromArgb(196,203,210);
            this.PrimaryColor = Color.FromArgb(76,110,245);
            this.SecondaryColor = Color.FromArgb(129,140,248);
            this.AccentColor = Color.FromArgb(255,173,94);
            this.BackgroundColor = Color.FromArgb(236,240,243);
            this.SurfaceColor = Color.FromArgb(244,247,250);
            this.ErrorColor = Color.FromArgb(231,76,60);
            this.WarningColor = Color.FromArgb(255,159,67);
            this.SuccessColor = Color.FromArgb(46,204,113);
            this.OnPrimaryColor = Color.FromArgb(76,110,245);
            this.OnBackgroundColor = Color.FromArgb(58,66,86);
            this.FocusIndicatorColor = Color.FromArgb(76,110,245);
        }
    }
}