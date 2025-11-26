using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class iOSTheme
    {
        private void ApplyButtons()
        {
            // iOS buttons - clean, modern aesthetic
            // Default: Light gray background with dark text
            this.ButtonBackColor = BackgroundColor;  // Light gray
            this.ButtonForeColor = ForeColor;  // Dark gray
            this.ButtonBorderColor = BorderColor;  // Medium gray
            
            // Hover: Darker background
            this.ButtonHoverBackColor = Color.FromArgb(220, 220, 230);
            this.ButtonHoverForeColor = ForeColor;
            this.ButtonHoverBorderColor = ActiveBorderColor;  // iOS blue
            
            // Selected: iOS blue background
            this.ButtonSelectedBackColor = PrimaryColor;  // iOS blue
            this.ButtonSelectedForeColor = OnPrimaryColor;  // White text
            this.ButtonSelectedBorderColor = PrimaryColor;
            
            // Selected hover: Lighter blue
            this.ButtonSelectedHoverBackColor = Color.FromArgb(20, 142, 255);
            this.ButtonSelectedHoverForeColor = OnPrimaryColor;
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(20, 142, 255);
            
            // Pressed: Lighter gray
            this.ButtonPressedBackColor = Color.FromArgb(250, 250, 255);
            this.ButtonPressedForeColor = ForeColor;
            this.ButtonPressedBorderColor = BorderColor;
            
            // Error button: Red background with white text
            this.ButtonErrorBackColor = ErrorColor;  // iOS red
            this.ButtonErrorForeColor = OnPrimaryColor;  // White text on red
            this.ButtonErrorBorderColor = ErrorColor;
        }
    }
}
