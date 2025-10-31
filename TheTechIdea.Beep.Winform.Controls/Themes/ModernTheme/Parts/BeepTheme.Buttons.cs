using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ModernTheme
    {
        private void ApplyButtons()
        {
            // Modern buttons - clean, modern aesthetic
            // Default: White background with dark text
            this.ButtonBackColor = Color.FromArgb(255, 255, 255);  // White
            this.ButtonForeColor = Color.FromArgb(17, 24, 39);  // Dark gray text
            this.ButtonBorderColor = Color.FromArgb(203, 213, 225);  // Light gray border
            
            // Hover: Light gray background
            this.ButtonHoverBackColor = Color.FromArgb(240, 245, 250);
            this.ButtonHoverForeColor = Color.FromArgb(17, 24, 39);
            this.ButtonHoverBorderColor = Color.FromArgb(99, 102, 241);  // Indigo border
            
            // Selected: Indigo background
            this.ButtonSelectedBackColor = Color.FromArgb(99, 102, 241);  // Indigo
            this.ButtonSelectedForeColor = Color.FromArgb(255, 255, 255);  // White text
            this.ButtonSelectedBorderColor = Color.FromArgb(99, 102, 241);
            
            // Selected hover: Lighter indigo
            this.ButtonSelectedHoverBackColor = Color.FromArgb(120, 122, 255);
            this.ButtonSelectedHoverForeColor = Color.FromArgb(255, 255, 255);
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(120, 122, 255);
            
            // Pressed: Darker gray
            this.ButtonPressedBackColor = Color.FromArgb(248, 250, 252);
            this.ButtonPressedForeColor = Color.FromArgb(17, 24, 39);
            this.ButtonPressedBorderColor = Color.FromArgb(203, 213, 225);
            
            // Error button: Red background with white text
            this.ButtonErrorBackColor = Color.FromArgb(239, 68, 68);  // Red
            this.ButtonErrorForeColor = Color.FromArgb(255, 255, 255);  // White text on red
            this.ButtonErrorBorderColor = Color.FromArgb(180, 0, 0);
        }
    }
}