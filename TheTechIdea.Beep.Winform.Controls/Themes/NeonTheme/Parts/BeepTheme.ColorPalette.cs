using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeonTheme
    {
        private void ApplyColorPalette()
        {
            this.ForeColor = Color.FromArgb(235,245,255);
            this.BackColor = Color.FromArgb(10,12,20);
            this.PanelBackColor = Color.FromArgb(10,12,20);
            this.PanelGradiantStartColor = Color.FromArgb(20,24,38);
            this.PanelGradiantEndColor = Color.FromArgb(20,24,38);
            this.PanelGradiantMiddleColor = Color.FromArgb(10,12,20);
            this.PanelGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.BorderColor = Color.FromArgb(60,70,100);
            this.ActiveBorderColor = Color.FromArgb(60,70,100);
            this.InactiveBorderColor = Color.FromArgb(60,70,100);
            this.PrimaryColor = Color.FromArgb(255,64,160);
            this.SecondaryColor = Color.FromArgb(64,224,255);
            this.AccentColor = Color.FromArgb(180,86,255);
            this.BackgroundColor = Color.FromArgb(10,12,20);
            this.SurfaceColor = Color.FromArgb(20,24,38);
            this.ErrorColor = Color.FromArgb(255,77,109);
            this.WarningColor = Color.FromArgb(255,208,0);
            this.SuccessColor = Color.FromArgb(57,255,20);
            this.OnPrimaryColor = Color.FromArgb(10, 12, 20);
            this.OnBackgroundColor = Color.FromArgb(235,245,255);
            this.FocusIndicatorColor = Color.FromArgb(64,224,255);
        }
    }
}