using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class FluentTheme
    {
        private void ApplyButtons()
        {
            // Fluent buttons - clean light theme
            // Default: Light background with dark text
            this.ButtonBackColor = BackgroundColor;  // Light gray-blue
            this.ButtonForeColor = ForeColor;  // Dark gray
            this.ButtonBorderColor = BorderColor;  // Light gray border
            
            // Hover: Darker background
            this.ButtonHoverBackColor = Color.FromArgb(230, 235, 240);
            this.ButtonHoverForeColor = ForeColor;
            this.ButtonHoverBorderColor = ActiveBorderColor;  // Blue border
            
            // Selected: Light blue background
            this.ButtonSelectedBackColor = Color.FromArgb(220, 230, 245);
            this.ButtonSelectedForeColor = PrimaryColor;  // Darker blue
            this.ButtonSelectedBorderColor = ActiveBorderColor;
            
            // Selected hover: Medium blue
            this.ButtonSelectedHoverBackColor = Color.FromArgb(200, 220, 240);
            this.ButtonSelectedHoverForeColor = PrimaryColor;
            this.ButtonSelectedHoverBorderColor = ActiveBorderColor;
            
            // Pressed: Lighter
            this.ButtonPressedBackColor = Color.FromArgb(250, 251, 253);
            this.ButtonPressedForeColor = ForeColor;
            this.ButtonPressedBorderColor = BorderColor;
            
            // Error button: Red background with white text
            this.ButtonErrorBackColor = ErrorColor;  // Fluent red
            this.ButtonErrorForeColor = OnPrimaryColor;  // White text on red
            this.ButtonErrorBorderColor = ErrorColor;
        }
    }
}
