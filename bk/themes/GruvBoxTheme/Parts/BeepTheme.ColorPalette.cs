using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GruvBoxTheme
    {
        private void ApplyColorPalette()
        {
            this.ForeColor = Color.FromArgb(235,219,178);
            this.BackColor = Color.FromArgb(40,40,40);
            this.PanelBackColor = Color.FromArgb(40,40,40);
            this.PanelGradiantStartColor = Color.FromArgb(60,56,54);
            this.PanelGradiantEndColor = Color.FromArgb(60,56,54);
            this.PanelGradiantMiddleColor = Color.FromArgb(40,40,40);
            this.PanelGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.BorderColor = Color.FromArgb(168,153,132);
            this.ActiveBorderColor = Color.FromArgb(168,153,132);
            this.InactiveBorderColor = Color.FromArgb(168,153,132);
            this.PrimaryColor = Color.FromArgb(250,189,47);
            this.SecondaryColor = Color.FromArgb(142,192,124);
            this.AccentColor = Color.FromArgb(254,128,25);
            this.BackgroundColor = Color.FromArgb(40,40,40);
            this.SurfaceColor = Color.FromArgb(60,56,54);
            this.ErrorColor = Color.FromArgb(251,73,52);
            this.WarningColor = Color.FromArgb(250,189,47);
            this.SuccessColor = Color.FromArgb(184,187,38);
            this.OnPrimaryColor = Color.FromArgb(40, 40, 40);
            this.OnBackgroundColor = Color.FromArgb(235,219,178);
            this.FocusIndicatorColor = Color.FromArgb(250,189,47);
        }
    }
}