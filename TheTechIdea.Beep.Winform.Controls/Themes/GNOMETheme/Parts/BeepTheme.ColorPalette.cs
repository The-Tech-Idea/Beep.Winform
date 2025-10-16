using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GNOMETheme
    {
        private void ApplyColorPalette()
        {
            this.ForeColor = Color.FromArgb(46,52,54);
            this.BackColor = Color.FromArgb(246,245,244);
            this.PanelBackColor = Color.FromArgb(246,245,244);
            this.PanelGradiantStartColor = Color.FromArgb(246,245,244);
            this.PanelGradiantEndColor = Color.FromArgb(246,245,244);
            this.PanelGradiantMiddleColor = Color.FromArgb(246,245,244);
            this.PanelGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.BorderColor = Color.FromArgb(205,207,212);
            this.ActiveBorderColor = Color.FromArgb(53,132,228);
            this.InactiveBorderColor = Color.FromArgb(186,189,182);
            this.PrimaryColor = Color.FromArgb(53,132,228);
            this.SecondaryColor = Color.FromArgb(98,160,234);
            this.AccentColor = Color.FromArgb(87,227,137);
            this.BackgroundColor = Color.FromArgb(246,245,244);
            this.SurfaceColor = Color.FromArgb(255,255,255);
            this.ErrorColor = Color.FromArgb(224,27,36);
            this.WarningColor = Color.FromArgb(246,211,45);
            this.SuccessColor = Color.FromArgb(51,209,122);
            this.OnPrimaryColor = Color.FromArgb(53,132,228);
            this.OnBackgroundColor = ForeColor;
            this.FocusIndicatorColor = Color.FromArgb(246,245,244);
        }
    }
}