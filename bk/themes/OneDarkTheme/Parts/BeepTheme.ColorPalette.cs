using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class OneDarkTheme
    {
        private void ApplyColorPalette()
        {
            this.ForeColor = Color.FromArgb(171,178,191);
            this.BackColor = Color.FromArgb(40,44,52);
            this.PanelBackColor = Color.FromArgb(40,44,52);
            this.PanelGradiantStartColor = Color.FromArgb(33,37,43);
            this.PanelGradiantEndColor = Color.FromArgb(33,37,43);
            this.PanelGradiantMiddleColor = Color.FromArgb(40,44,52);
            this.PanelGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.BorderColor = Color.FromArgb(92,99,112);
            this.ActiveBorderColor = Color.FromArgb(92,99,112);
            this.InactiveBorderColor = Color.FromArgb(92,99,112);
            this.PrimaryColor = Color.FromArgb(97,175,239);
            this.SecondaryColor = Color.FromArgb(86,182,194);
            this.AccentColor = Color.FromArgb(198,120,221);
            this.BackgroundColor = Color.FromArgb(40,44,52);
            this.SurfaceColor = Color.FromArgb(33,37,43);
            this.ErrorColor = Color.FromArgb(224,108,117);
            this.WarningColor = Color.FromArgb(229,192,123);
            this.SuccessColor = Color.FromArgb(152,195,121);
            this.OnPrimaryColor = Color.FromArgb(40, 44, 52);
            this.OnBackgroundColor = Color.FromArgb(171,178,191);
            this.FocusIndicatorColor = Color.FromArgb(97,175,239);
        }
    }
}