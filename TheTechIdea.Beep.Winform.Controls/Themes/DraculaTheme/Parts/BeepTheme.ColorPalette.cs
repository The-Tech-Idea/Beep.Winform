using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class DraculaTheme
    {
        private void ApplyColorPalette()
        {
            this.ForeColor = Color.FromArgb(248,248,242);
            this.BackColor = Color.FromArgb(40,42,54);
            this.PanelBackColor = Color.FromArgb(40,42,54);
            this.PanelGradiantStartColor = Color.FromArgb(64,67,87);
            this.PanelGradiantEndColor = Color.FromArgb(64,67,87);
            this.PanelGradiantMiddleColor = Color.FromArgb(52,55,72);
            this.PanelGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
            this.BorderColor = Color.FromArgb(98,114,164);
            this.ActiveBorderColor = Color.FromArgb(98,114,164);
            this.InactiveBorderColor = Color.FromArgb(98,114,164);
            this.PrimaryColor = Color.FromArgb(189,147,249);
            this.SecondaryColor = Color.FromArgb(139,233,253);
            this.AccentColor = Color.FromArgb(255,121,198);
            this.BackgroundColor = Color.FromArgb(40,42,54);
            this.SurfaceColor = Color.FromArgb(68,71,90);
            this.ErrorColor = Color.FromArgb(255,85,85);
            this.WarningColor = Color.FromArgb(241,250,140);
            this.SuccessColor = Color.FromArgb(80,250,123);
            this.OnPrimaryColor = Color.FromArgb(40, 42, 54);
            this.OnBackgroundColor = Color.FromArgb(248,248,242);
            this.FocusIndicatorColor = Color.FromArgb(139,233,253);
        }
    }
}