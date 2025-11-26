using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CyberpunkTheme
    {
        private void ApplyButtons()
        {
            // Cyberpunk buttons - neon aesthetic
            // Default: Dark with cyan borders
            this.ButtonBackColor = BackgroundColor;  // Dark background
            this.ButtonForeColor = ForeColor;  // Cyan text
            this.ButtonBorderColor = BorderColor;  // Cyan border
            
            // Hover: Glowing effect
            this.ButtonHoverBackColor = Color.FromArgb(20, 40, 40);  // Dark cyan tint
            this.ButtonHoverForeColor = Color.FromArgb(100, 255, 255);  // Bright cyan
            this.ButtonHoverBorderColor = ActiveBorderColor;  // Bright border
            
            // Selected: Visible state
            this.ButtonSelectedBackColor = Color.FromArgb(30, 60, 60);  // Medium cyan
            this.ButtonSelectedForeColor = ForeColor;
            this.ButtonSelectedBorderColor = BorderColor;
            
            // Selected hover: Brighter
            this.ButtonSelectedHoverBackColor = Color.FromArgb(40, 80, 80);
            this.ButtonSelectedHoverForeColor = Color.FromArgb(100, 255, 255);
            this.ButtonSelectedHoverBorderColor = ActiveBorderColor;
            
            // Pressed: Lighter
            this.ButtonPressedBackColor = Color.FromArgb(20, 30, 40);
            this.ButtonPressedForeColor = ForeColor;
            this.ButtonPressedBorderColor = BorderColor;
            
            // Error button: Red neon
            this.ButtonErrorBackColor = ErrorColor;
            this.ButtonErrorForeColor = OnPrimaryColor;  // Light on red
            this.ButtonErrorBorderColor = ErrorColor;
        }
    }
}
