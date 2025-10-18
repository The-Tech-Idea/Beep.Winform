using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class HolographicTheme
    {
        private void ApplyColorPalette()
        {
            this.ForeColor = Color.FromArgb(245,247,255);
            this.BackColor = Color.FromArgb(15,16,32);
            this.PanelBackColor = Color.FromArgb(15,16,32);
            this.PanelGradiantStartColor = Color.FromArgb(255, 22, 24, 46);
            this.PanelGradiantEndColor = Color.FromArgb(255, 22, 24, 46);
            this.PanelGradiantMiddleColor = Color.FromArgb(255, 32, 28, 58);
            this.PanelGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
            this.BorderColor = Color.FromArgb(74,79,123);
            this.ActiveBorderColor = Color.FromArgb(74,79,123);
            this.InactiveBorderColor = Color.FromArgb(74,79,123);
            this.PrimaryColor = Color.FromArgb(255,122,217);
            this.SecondaryColor = Color.FromArgb(122,252,255);
            this.AccentColor = Color.FromArgb(176,141,255);
            this.BackgroundColor = Color.FromArgb(15,16,32);
            this.SurfaceColor = Color.FromArgb(22,24,46);
            this.ErrorColor = Color.FromArgb(255,138,167);
            this.WarningColor = Color.FromArgb(255,212,126);
            this.SuccessColor = Color.FromArgb(159,255,169);
            this.OnPrimaryColor = Color.FromArgb(15, 16, 32);
            this.OnBackgroundColor = Color.FromArgb(245,247,255);
            this.FocusIndicatorColor = Color.FromArgb(122,252,255);
        }
    }
}