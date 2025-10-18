using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class iOSTheme
    {
        private void ApplyColorPalette()
        {
            this.ForeColor = Color.FromArgb(28,28,30);
            this.BackColor = Color.FromArgb(242,242,247);
            this.PanelBackColor = Color.FromArgb(242,242,247);
            this.PanelGradiantStartColor = Color.FromArgb(255,255,255);
            this.PanelGradiantEndColor = Color.FromArgb(255,255,255);
            this.PanelGradiantMiddleColor = Color.FromArgb(252,252,252);
            this.PanelGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.BorderColor = Color.FromArgb(198,198,207);
            this.ActiveBorderColor = Color.FromArgb(198,198,207);
            this.InactiveBorderColor = Color.FromArgb(198,198,207);
            this.PrimaryColor = Color.FromArgb(10,132,255);
            this.SecondaryColor = Color.FromArgb(88,86,214);
            this.AccentColor = Color.FromArgb(255,159,10);
            this.BackgroundColor = Color.FromArgb(242,242,247);
            this.SurfaceColor = Color.FromArgb(255,255,255);
            this.ErrorColor = Color.FromArgb(255,69,58);
            this.WarningColor = Color.FromArgb(255,159,10);
            this.SuccessColor = Color.FromArgb(48,209,88);
            this.OnPrimaryColor = Color.White;
            this.OnBackgroundColor = Color.FromArgb(28,28,30);
            this.FocusIndicatorColor = Color.FromArgb(10,132,255);
        }
    }
}