using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CyberpunkTheme
    {
        private void ApplyColorPalette()
        {
            this.ForeColor = Color.FromArgb(228,244,255);
            this.BackColor = Color.FromArgb(10,8,20);
            this.PanelBackColor = Color.FromArgb(10,8,20);
            this.PanelGradiantStartColor = Color.FromArgb(32,12,48);
            this.PanelGradiantEndColor = Color.FromArgb(32,12,48);
            this.PanelGradiantMiddleColor = Color.FromArgb(20,24,40);
            this.PanelGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
            this.BorderColor = Color.FromArgb(90,20,110);
            this.ActiveBorderColor = Color.FromArgb(90,20,110);
            this.InactiveBorderColor = Color.FromArgb(90,20,110);
            this.PrimaryColor = Color.FromArgb(255,0,128);
            this.SecondaryColor = Color.FromArgb(0,255,230);
            this.AccentColor = Color.FromArgb(255,230,0);
            this.BackgroundColor = Color.FromArgb(10,8,20);
            this.SurfaceColor = Color.FromArgb(22,20,40);
            this.ErrorColor = Color.FromArgb(255,64,64);
            this.WarningColor = Color.FromArgb(255,191,0);
            this.SuccessColor = Color.FromArgb(0,255,140);
            this.OnPrimaryColor = Color.Black;
            this.OnBackgroundColor = Color.FromArgb(228,244,255);
            this.FocusIndicatorColor = Color.FromArgb(0,255,230);
        }
    }
}