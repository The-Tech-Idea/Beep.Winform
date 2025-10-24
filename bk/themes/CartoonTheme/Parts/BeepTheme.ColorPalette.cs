using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CartoonTheme
    {
        private void ApplyColorPalette()
        {
            this.ForeColor = Color.FromArgb(33,37,41);
            this.BackColor = Color.FromArgb(255,251,235);
            this.PanelBackColor = Color.FromArgb(255,251,235);
            this.PanelGradiantStartColor = Color.FromArgb(255,251,235);
            this.PanelGradiantEndColor = Color.FromArgb(255,251,235);
            this.PanelGradiantMiddleColor = Color.FromArgb(255,251,235);
            this.PanelGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
            this.BorderColor = Color.FromArgb(247,208,136);
            this.ActiveBorderColor = Color.FromArgb(255,159,67);
            this.InactiveBorderColor = Color.FromArgb(221,221,221);
            this.PrimaryColor = Color.FromArgb(255,107,107);
            this.SecondaryColor = Color.FromArgb(255,199,0);
            this.AccentColor = Color.FromArgb(72,199,142);
            this.BackgroundColor = Color.FromArgb(255,251,235);
            this.SurfaceColor = Color.FromArgb(255,255,255);
            this.ErrorColor = Color.FromArgb(255,82,82);
            this.WarningColor = Color.FromArgb(255,177,66);
            this.SuccessColor = Color.FromArgb(72,199,142);
            this.OnPrimaryColor = Color.FromArgb(255,107,107);
            this.OnBackgroundColor = ForeColor;
            this.FocusIndicatorColor = Color.FromArgb(255,251,235);
        }
    }
}