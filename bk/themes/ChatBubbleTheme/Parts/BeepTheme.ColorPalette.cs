using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ChatBubbleTheme
    {
        private void ApplyColorPalette()
        {
            this.ForeColor = Color.FromArgb(24,28,35);
            this.BackColor = Color.FromArgb(245,248,255);
            this.PanelBackColor = Color.FromArgb(245,248,255);
            this.PanelGradiantStartColor = Color.FromArgb(245,248,255);
            this.PanelGradiantEndColor = Color.FromArgb(245,248,255);
            this.PanelGradiantMiddleColor = Color.FromArgb(245,248,255);
            this.PanelGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.BorderColor = Color.FromArgb(210,220,235);
            this.ActiveBorderColor = Color.FromArgb(74,158,255);
            this.InactiveBorderColor = Color.FromArgb(180,190,205);
            this.PrimaryColor = Color.FromArgb(74,158,255);
            this.SecondaryColor = Color.FromArgb(155,111,242);
            this.AccentColor = Color.FromArgb(255,100,180);
            this.BackgroundColor = Color.FromArgb(245,248,255);
            this.SurfaceColor = Color.FromArgb(255,255,255);
            this.ErrorColor = Color.FromArgb(245,80,100);
            this.WarningColor = Color.FromArgb(255,177,66);
            this.SuccessColor = Color.FromArgb(72,199,142);
            this.OnPrimaryColor = Color.FromArgb(74,158,255);
            this.OnBackgroundColor = ForeColor;
            this.FocusIndicatorColor = Color.FromArgb(245,248,255);
        }
    }
}