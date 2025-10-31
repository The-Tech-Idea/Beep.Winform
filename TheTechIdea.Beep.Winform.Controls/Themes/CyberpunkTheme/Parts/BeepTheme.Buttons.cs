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
            this.ButtonBackColor = Color.FromArgb(10, 10, 20);  // Dark background
            this.ButtonForeColor = Color.FromArgb(0, 255, 255);  // Cyan text
            this.ButtonBorderColor = Color.FromArgb(0, 255, 255);  // Cyan border
            
            // Hover: Glowing effect
            this.ButtonHoverBackColor = Color.FromArgb(20, 40, 40);  // Dark cyan tint
            this.ButtonHoverForeColor = Color.FromArgb(100, 255, 255);  // Bright cyan
            this.ButtonHoverBorderColor = Color.FromArgb(100, 255, 255);  // Bright border
            
            // Selected: Visible state
            this.ButtonSelectedBackColor = Color.FromArgb(30, 60, 60);  // Medium cyan
            this.ButtonSelectedForeColor = Color.FromArgb(0, 255, 255);
            this.ButtonSelectedBorderColor = Color.FromArgb(0, 255, 255);
            
            // Selected hover: Brighter
            this.ButtonSelectedHoverBackColor = Color.FromArgb(40, 80, 80);
            this.ButtonSelectedHoverForeColor = Color.FromArgb(100, 255, 255);
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(100, 255, 255);
            
            // Pressed: Lighter
            this.ButtonPressedBackColor = Color.FromArgb(20, 30, 40);
            this.ButtonPressedForeColor = Color.FromArgb(0, 255, 255);
            this.ButtonPressedBorderColor = Color.FromArgb(0, 255, 255);
            
            // Error button: Red neon
            this.ButtonErrorBackColor = Color.FromArgb(60, 0, 0);  // Dark red
            this.ButtonErrorForeColor = Color.FromArgb(255, 80, 80);  // Red neon
            this.ButtonErrorBorderColor = Color.FromArgb(255, 80, 80);
        }
    }
}