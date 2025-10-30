using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MinimalTheme
    {
        private void ApplyButtons()
        {
            // Minimal buttons - clean, minimal aesthetic
            // Default: White background with dark text
            this.ButtonBackColor = BackgroundColor;  // White
            this.ButtonForeColor = ForeColor;  // Dark grey
            this.ButtonBorderColor = BorderColor;  // Light grey border
            
            // Hover: Slightly grey background
            this.ButtonHoverBackColor = Color.FromArgb(245, 245, 245);  // SurfaceColor
            this.ButtonHoverForeColor = ForeColor;
            this.ButtonHoverBorderColor = Color.FromArgb(45, 45, 45);  // Darker border
            
            // Selected: Light grey background
            this.ButtonSelectedBackColor = Color.FromArgb(230, 230, 230);  // Medium grey
            this.ButtonSelectedForeColor = ForeColor;
            this.ButtonSelectedBorderColor = Color.FromArgb(45, 45, 45);
            
            // Selected hover: Slightly darker
            this.ButtonSelectedHoverBackColor = Color.FromArgb(220, 220, 220);
            this.ButtonSelectedHoverForeColor = ForeColor;
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(45, 45, 45);
            
            // Pressed: White
            this.ButtonPressedBackColor = Color.FromArgb(255, 255, 255);
            this.ButtonPressedForeColor = ForeColor;
            this.ButtonPressedBorderColor = BorderColor;
            
            // Error button: Red background with white text
            this.ButtonErrorBackColor = ErrorColor;  // Red
            this.ButtonErrorForeColor = Color.FromArgb(255, 255, 255);  // White text on red
            this.ButtonErrorBorderColor = Color.FromArgb(180, 0, 0);
        }
    }
}

