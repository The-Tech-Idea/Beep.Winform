using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordTheme
    {
        private void ApplyColorPalette()
        {
            this.ForeColor = Color.FromArgb(216,222,233);
            this.BackColor = Color.FromArgb(46,52,64);
            this.PanelBackColor = Color.FromArgb(46,52,64);
            this.PanelGradiantStartColor = Color.FromArgb(59,66,82);
            this.PanelGradiantEndColor = Color.FromArgb(59,66,82);
            this.PanelGradiantMiddleColor = Color.FromArgb(46,52,64);
            this.PanelGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.BorderColor = Color.FromArgb(76,86,106);
            this.ActiveBorderColor = Color.FromArgb(76,86,106);
            this.InactiveBorderColor = Color.FromArgb(76,86,106);
            this.PrimaryColor = Color.FromArgb(136,192,208);
            this.SecondaryColor = Color.FromArgb(94,129,172);
            this.AccentColor = Color.FromArgb(235,203,139);
            this.BackgroundColor = Color.FromArgb(46,52,64);
            this.SurfaceColor = Color.FromArgb(59,66,82);
            this.ErrorColor = Color.FromArgb(191,97,106);
            this.WarningColor = Color.FromArgb(235,203,139);
            this.SuccessColor = Color.FromArgb(163,190,140);
            this.OnPrimaryColor = Color.FromArgb(46, 52, 64);
            this.OnBackgroundColor = Color.FromArgb(216,222,233);
            this.FocusIndicatorColor = Color.FromArgb(136,192,208);
        }
    }
}