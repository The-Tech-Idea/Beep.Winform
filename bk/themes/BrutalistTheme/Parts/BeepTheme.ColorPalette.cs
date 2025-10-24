using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class BrutalistTheme
    {
        private void ApplyColorPalette()
        {
            this.ForeColor = Color.FromArgb(20,20,20);
            this.BackColor = Color.FromArgb(250,250,250);
            this.PanelBackColor = Color.FromArgb(250,250,250);
            this.PanelGradiantStartColor = Color.FromArgb(255,255,255);
            this.PanelGradiantEndColor = Color.FromArgb(255,255,255);
            this.PanelGradiantMiddleColor = Color.FromArgb(245,245,245);
            this.PanelGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.BorderColor = Color.FromArgb(0,0,0);
            this.ActiveBorderColor = Color.FromArgb(0,0,0);
            this.InactiveBorderColor = Color.FromArgb(0,0,0);
            this.PrimaryColor = Color.FromArgb(255,208,0);
            this.SecondaryColor = Color.FromArgb(0,102,255);
            this.AccentColor = Color.FromArgb(255,0,102);
            this.BackgroundColor = Color.FromArgb(250,250,250);
            this.SurfaceColor = Color.FromArgb(255,255,255);
            this.ErrorColor = Color.FromArgb(220,0,0);
            this.WarningColor = Color.FromArgb(255,140,0);
            this.SuccessColor = Color.FromArgb(0,200,70);
            this.OnPrimaryColor = Color.FromArgb(255,208,0);
            this.OnBackgroundColor = Color.FromArgb(20,20,20);
            this.FocusIndicatorColor = Color.FromArgb(0,0,0);
        }
    }
}