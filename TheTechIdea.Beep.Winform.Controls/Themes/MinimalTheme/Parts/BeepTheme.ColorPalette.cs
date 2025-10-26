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
            this.ForeColor = Color.FromArgb(30, 30, 30);
            this.BackColor = Color.FromArgb(255,255,255);
            this.PanelBackColor = Color.FromArgb(255,255,255);
            this.PanelGradiantStartColor = Color.FromArgb(255,255,255);
            this.PanelGradiantEndColor = Color.FromArgb(255,255,255);
            this.PanelGradiantMiddleColor = Color.FromArgb(255,255,255);
            this.PanelGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.BorderColor = Color.FromArgb(200, 200, 200);
            this.ActiveBorderColor = Color.FromArgb(45, 45, 45);
            this.InactiveBorderColor = Color.FromArgb(220, 220, 220);
            this.PrimaryColor = Color.FromArgb(20, 20, 20);
            this.SecondaryColor = Color.FromArgb(100, 100, 100);
            this.AccentColor = Color.FromArgb(210, 210, 210);
            this.BackgroundColor = Color.FromArgb(255,255,255);
            this.SurfaceColor = Color.FromArgb(245, 245, 245);
            this.ErrorColor = Color.FromArgb(239,68,68);
            this.WarningColor = Color.FromArgb(245,158,11);
            this.SuccessColor = Color.FromArgb(16,185,129);
            this.OnPrimaryColor = Color.FromArgb(255, 255, 255);
            this.OnBackgroundColor = ForeColor;
            this.FocusIndicatorColor = Color.FromArgb(45, 45, 45);
        }

        private static Color MinimalMinimizeColor => Color.FromArgb(253, 224, 71);
        private static Color MinimalMaximizeColor => Color.FromArgb(134, 239, 172);
        private static Color MinimalCloseColor => Color.FromArgb(248, 113, 113);
        private static Color MinimalMutedTextColor => Color.FromArgb(120, 120, 120);
    }
}
