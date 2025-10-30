using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ChatBubbleTheme
    {
        private void ApplyButtons()
        {
            // ChatBubble buttons - soft blue theme
            // Default: Cyan background with black text
            this.ButtonBackColor = Color.FromArgb(230, 250, 255);  // Light cyan
            this.ButtonForeColor = Color.FromArgb(0, 0, 0);  // Black
            this.ButtonBorderColor = Color.FromArgb(200, 200, 200);  // Light gray
            
            // Hover: Slightly darker blue
            this.ButtonHoverBackColor = Color.FromArgb(210, 240, 250);
            this.ButtonHoverForeColor = Color.FromArgb(0, 0, 0);
            this.ButtonHoverBorderColor = Color.FromArgb(74, 158, 255);  // Blue border
            
            // Selected: Medium blue
            this.ButtonSelectedBackColor = Color.FromArgb(180, 230, 250);
            this.ButtonSelectedForeColor = Color.FromArgb(0, 0, 0);
            this.ButtonSelectedBorderColor = Color.FromArgb(74, 158, 255);
            
            // Selected hover: Darker blue
            this.ButtonSelectedHoverBackColor = Color.FromArgb(160, 220, 250);
            this.ButtonSelectedHoverForeColor = Color.FromArgb(0, 0, 0);
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(74, 158, 255);
            
            // Pressed: Lighter
            this.ButtonPressedBackColor = Color.FromArgb(250, 255, 255);
            this.ButtonPressedForeColor = Color.FromArgb(0, 0, 0);
            this.ButtonPressedBorderColor = Color.FromArgb(200, 200, 200);
            
            // Error button: Red background with white text
            this.ButtonErrorBackColor = Color.FromArgb(245, 80, 100);
            this.ButtonErrorForeColor = Color.FromArgb(255, 255, 255);  // White text on red
            this.ButtonErrorBorderColor = Color.FromArgb(200, 0, 0);
        }
    }
}