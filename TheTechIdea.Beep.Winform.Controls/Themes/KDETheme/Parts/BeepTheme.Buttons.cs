using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class KDETheme
    {
        private void ApplyButtons()
        {
            // KDE buttons - clean Linux aesthetic
            // Default: Light background with dark text
            this.ButtonBackColor = BackgroundColor;  // Light gray
            this.ButtonForeColor = ForeColor;  // Dark gray
            this.ButtonBorderColor = BorderColor;  // Medium gray
            
            // Hover: Darker background
            this.ButtonHoverBackColor = Color.FromArgb(230, 235, 240);
            this.ButtonHoverForeColor = ForeColor;
            this.ButtonHoverBorderColor = ActiveBorderColor;  // KDE blue
            
            // Selected: KDE blue background
            this.ButtonSelectedBackColor = PrimaryColor;  // KDE blue
            this.ButtonSelectedForeColor = OnPrimaryColor;  // White text
            this.ButtonSelectedBorderColor = PrimaryColor;
            
            // Selected hover: Lighter blue
            this.ButtonSelectedHoverBackColor = Color.FromArgb(81, 194, 253);
            this.ButtonSelectedHoverForeColor = OnPrimaryColor;
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(81, 194, 253);
            
            // Pressed: Lighter gray
            this.ButtonPressedBackColor = Color.FromArgb(250, 252, 255);
            this.ButtonPressedForeColor = ForeColor;
            this.ButtonPressedBorderColor = BorderColor;
            
            // Error button: Red background with white text
            this.ButtonErrorBackColor = ErrorColor;  // Bootstrap red
            this.ButtonErrorForeColor = OnPrimaryColor;  // White text on red
            this.ButtonErrorBorderColor = ErrorColor;
        }
    }
}
