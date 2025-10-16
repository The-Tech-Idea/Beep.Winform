using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MinimalTheme
    {
        private void ApplyColorPalette()
        {
            this.ForeColor = Color.FromArgb(31,41,55);
            this.BackColor = Color.FromArgb(255,255,255);
            this.PanelBackColor = Color.FromArgb(255,255,255);
            this.PanelGradiantStartColor = Color.FromArgb(255,255,255);
            this.PanelGradiantEndColor = Color.FromArgb(255,255,255);
            this.PanelGradiantMiddleColor = Color.FromArgb(255,255,255);
            this.PanelGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.BorderColor = Color.FromArgb(209,213,219);
            this.ActiveBorderColor = Color.FromArgb(59,130,246);
            this.InactiveBorderColor = Color.FromArgb(156,163,175);
            this.PrimaryColor = Color.FromArgb(59,130,246);
            this.SecondaryColor = Color.FromArgb(107,114,128);
            this.AccentColor = Color.FromArgb(234,179,8);
            this.BackgroundColor = Color.FromArgb(255,255,255);
            this.SurfaceColor = Color.FromArgb(249,250,251);
            this.ErrorColor = Color.FromArgb(239,68,68);
            this.WarningColor = Color.FromArgb(245,158,11);
            this.SuccessColor = Color.FromArgb(16,185,129);
            this.OnPrimaryColor = Color.FromArgb(59,130,246);
            this.OnBackgroundColor = ForeColor;
            this.FocusIndicatorColor = Color.FromArgb(255,255,255);
        }
    }
}