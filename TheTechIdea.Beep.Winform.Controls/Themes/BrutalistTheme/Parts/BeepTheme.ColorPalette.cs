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
            this.BackColor = Color.FromArgb(255,255,255);
            this.PanelBackColor = BackColor;
            this.PanelGradiantStartColor = BackColor;
            this.PanelGradiantEndColor = BackColor;
            this.PanelGradiantMiddleColor = BackColor;
            this.PanelGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.BorderColor = Color.FromArgb(0,0,0);
            this.ActiveBorderColor = Color.FromArgb(0,0,0);
            this.InactiveBorderColor = Color.FromArgb(0,0,0);
            this.PrimaryColor = Color.FromArgb(0,0,0);
            this.SecondaryColor = Color.FromArgb(80,80,80);
            this.AccentColor = Color.FromArgb(255,208,0);
            this.BackgroundColor = BackColor;
            this.SurfaceColor = Color.FromArgb(255,255,255);
            this.ErrorColor = Color.FromArgb(220,0,0);
            this.WarningColor = Color.FromArgb(255,140,0);
            this.SuccessColor = Color.FromArgb(0,200,70);
            this.OnPrimaryColor = Color.FromArgb(255,255,255);
            this.OnBackgroundColor = ForeColor;
            this.FocusIndicatorColor = Color.FromArgb(0,0,0);
        }
    }
}
