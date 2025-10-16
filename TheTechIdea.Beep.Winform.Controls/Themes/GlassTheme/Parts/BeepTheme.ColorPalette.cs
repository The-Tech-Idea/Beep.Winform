using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GlassTheme
    {
        private void ApplyColorPalette()
        {
            this.ForeColor = Color.FromArgb(17,24,39);
            this.BackColor = Color.FromArgb(236,244,255);
            this.PanelBackColor = Color.FromArgb(236,244,255);
            this.PanelGradiantStartColor = Color.FromArgb(160, 255, 255, 255);
            this.PanelGradiantEndColor = Color.FromArgb(160, 255, 255, 255);
            this.PanelGradiantMiddleColor = Color.FromArgb(120, 255, 255, 255);
            this.PanelGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.BorderColor = Color.FromArgb(140, 255, 255, 255);
            this.ActiveBorderColor = Color.FromArgb(99,102,241);
            this.InactiveBorderColor = Color.FromArgb(148,163,184);
            this.PrimaryColor = Color.FromArgb(99,102,241);
            this.SecondaryColor = Color.FromArgb(56,189,248);
            this.AccentColor = Color.FromArgb(16,185,129);
            this.BackgroundColor = Color.FromArgb(236,244,255);
            this.SurfaceColor = Color.FromArgb(255,255,255);
            this.ErrorColor = Color.FromArgb(239,68,68);
            this.WarningColor = Color.FromArgb(245,158,11);
            this.SuccessColor = Color.FromArgb(16,185,129);
            this.OnPrimaryColor = Color.FromArgb(99,102,241);
            this.OnBackgroundColor = Color.FromArgb(17,24,39);
            this.FocusIndicatorColor = Color.FromArgb(236,244,255);
        }
    }
}