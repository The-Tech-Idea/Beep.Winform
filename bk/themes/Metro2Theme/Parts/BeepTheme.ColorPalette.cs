using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class Metro2Theme
    {
        private void ApplyColorPalette()
        {
            this.ForeColor = Color.FromArgb(32,31,30);
            this.BackColor = Color.FromArgb(243,242,241);
            this.PanelBackColor = Color.FromArgb(243,242,241);
            this.PanelGradiantStartColor = Color.FromArgb(243,242,241);
            this.PanelGradiantEndColor = Color.FromArgb(243,242,241);
            this.PanelGradiantMiddleColor = Color.FromArgb(243,242,241);
            this.PanelGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.BorderColor = Color.FromArgb(220,220,220);
            this.ActiveBorderColor = Color.FromArgb(0,120,215);
            this.InactiveBorderColor = Color.FromArgb(196,196,196);
            this.PrimaryColor = Color.FromArgb(0,120,215);
            this.SecondaryColor = Color.FromArgb(230,0,126);
            this.AccentColor = Color.FromArgb(140,190,40);
            this.BackgroundColor = Color.FromArgb(243,242,241);
            this.SurfaceColor = Color.FromArgb(255,255,255);
            this.ErrorColor = Color.FromArgb(232,17,35);
            this.WarningColor = Color.FromArgb(255,185,0);
            this.SuccessColor = Color.FromArgb(0,153,51);
            this.OnPrimaryColor = Color.FromArgb(0,120,215);
            this.OnBackgroundColor = ForeColor;
            this.FocusIndicatorColor = Color.FromArgb(243,242,241);
        }
    }
}