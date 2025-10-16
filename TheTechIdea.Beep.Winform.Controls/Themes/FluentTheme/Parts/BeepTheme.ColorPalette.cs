using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class FluentTheme
    {
        private void ApplyColorPalette()
        {
            this.ForeColor = Color.FromArgb(32,32,32);
            this.BackColor = Color.FromArgb(245,246,248);
            this.PanelBackColor = Color.FromArgb(245,246,248);
            this.PanelGradiantStartColor = Color.FromArgb(245,246,248);
            this.PanelGradiantEndColor = Color.FromArgb(245,246,248);
            this.PanelGradiantMiddleColor = Color.FromArgb(245,246,248);
            this.PanelGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.BorderColor = Color.FromArgb(218,223,230);
            this.ActiveBorderColor = Color.FromArgb(0,120,215);
            this.InactiveBorderColor = Color.FromArgb(180,186,196);
            this.PrimaryColor = Color.FromArgb(0,120,215);
            this.SecondaryColor = Color.FromArgb(0,153,188);
            this.AccentColor = Color.FromArgb(255,185,0);
            this.BackgroundColor = Color.FromArgb(245,246,248);
            this.SurfaceColor = Color.FromArgb(255,255,255);
            this.ErrorColor = Color.FromArgb(196,30,58);
            this.WarningColor = Color.FromArgb(255,185,0);
            this.SuccessColor = Color.FromArgb(16,124,16);
            this.OnPrimaryColor = Color.FromArgb(0,120,215);
            this.OnBackgroundColor = ForeColor;
            this.FocusIndicatorColor = Color.FromArgb(245,246,248);
        }
    }
}