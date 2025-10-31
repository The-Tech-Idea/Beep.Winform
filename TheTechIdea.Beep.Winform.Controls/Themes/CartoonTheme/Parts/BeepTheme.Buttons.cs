using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CartoonTheme
    {
        private void ApplyButtons()
        {
            // Cartoon buttons - playful with purple accents
            // Default: Light background with purple text
            this.ButtonBackColor = Color.FromArgb(255, 240, 255);  // Pink-tinted background
            this.ButtonForeColor = Color.FromArgb(80, 0, 120);  // Purple text
            this.ButtonBorderColor = Color.FromArgb(150, 100, 200);  // Purple border
            
            // Hover: Slightly darker purple background
            this.ButtonHoverBackColor = Color.FromArgb(240, 220, 255);
            this.ButtonHoverForeColor = Color.FromArgb(80, 0, 120);
            this.ButtonHoverBorderColor = Color.FromArgb(120, 0, 180);
            
            // Selected: Light purple background
            this.ButtonSelectedBackColor = Color.FromArgb(220, 180, 255);
            this.ButtonSelectedForeColor = Color.FromArgb(80, 0, 120);
            this.ButtonSelectedBorderColor = Color.FromArgb(120, 0, 180);
            
            // Selected hover: Medium purple
            this.ButtonSelectedHoverBackColor = Color.FromArgb(200, 160, 255);
            this.ButtonSelectedHoverForeColor = Color.FromArgb(80, 0, 120);
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(120, 0, 180);
            
            // Pressed: Darker for feedback
            this.ButtonPressedBackColor = Color.FromArgb(255, 250, 255);
            this.ButtonPressedForeColor = Color.FromArgb(80, 0, 120);
            this.ButtonPressedBorderColor = Color.FromArgb(150, 100, 200);
            
            // Error button: Pink background with white text
            this.ButtonErrorBackColor = Color.FromArgb(255, 69, 180);
            this.ButtonErrorForeColor = Color.FromArgb(255, 255, 255);  // White text on pink
            this.ButtonErrorBorderColor = Color.FromArgb(200, 0, 150);
        }
    }
}