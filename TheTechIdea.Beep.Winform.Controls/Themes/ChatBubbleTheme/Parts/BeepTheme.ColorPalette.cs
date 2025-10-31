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
            // ChatBubble theme - soft blue/cyan aesthetic
            // Aligned with FormStyle.ChatBubble: Background #E6FAFF, Border #C8E8F5
            this.ForeColor = Color.FromArgb(0, 0, 0);  // Black text
            this.BackColor = Color.FromArgb(230, 250, 255);  // Light cyan-blue (#E6FAFF)
            this.PanelBackColor = Color.FromArgb(230, 250, 255);
            this.PanelGradiantStartColor = Color.FromArgb(245, 248, 255);
            this.PanelGradiantEndColor = Color.FromArgb(220, 245, 250);
            this.PanelGradiantMiddleColor = Color.FromArgb(230, 250, 255);
            this.PanelGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            
            // Soft borders - light blue/teal
            this.BorderColor = Color.FromArgb(200, 200, 200);  // Light gray borders
            this.ActiveBorderColor = Color.FromArgb(74, 158, 255);  // Blue accent
            this.InactiveBorderColor = Color.FromArgb(180, 220, 240);  // Light blue
            
            // Blue-themed palette
            this.PrimaryColor = Color.FromArgb(74, 158, 255);  // Blue
            this.SecondaryColor = Color.FromArgb(155, 111, 242);  // Purple
            this.AccentColor = Color.FromArgb(100, 200, 255);  // Cyan
            this.BackgroundColor = Color.FromArgb(230, 250, 255);
            this.SurfaceColor = Color.FromArgb(255, 255, 255);  // White surface
            
            // Status colors
            this.ErrorColor = Color.FromArgb(245, 80, 100);
            this.WarningColor = Color.FromArgb(255, 177, 66);
            this.SuccessColor = Color.FromArgb(72, 199, 142);
            
            // On-colors for readability
            this.OnPrimaryColor = Color.FromArgb(255, 255, 255);  // White on blue
            this.OnBackgroundColor = Color.FromArgb(0, 0, 0);  // Black on white
            this.FocusIndicatorColor = Color.FromArgb(74, 158, 255);  // Blue focus
        }
    }
}