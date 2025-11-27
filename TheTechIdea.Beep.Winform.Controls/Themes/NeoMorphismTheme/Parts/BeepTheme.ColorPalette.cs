using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Themes.ThemeContrastUtilities;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeoMorphismTheme
    {
        private void ApplyColorPalette()
        {
            // NeoMorphism theme - soft shadows and depth
            // Aligned with FormStyle.NeoMorphism: Background #F0F0F5, Text #32323C
            this.ForeColor = Color.FromArgb(50, 50, 60);  // Dark gray-blue (#32323C)
            this.BackColor = Color.FromArgb(240, 240, 245);  // Light gray-blue (#F0F0F5)
            this.BackgroundColor = Color.FromArgb(240, 240, 245);
            this.SurfaceColor = Color.FromArgb(244, 247, 250);  // Slightly lighter
            this.PanelBackColor = Color.FromArgb(240, 240, 245);
            this.PanelGradiantStartColor = ThemeUtil.Lighten(BackgroundColor, 0.06);
            this.PanelGradiantEndColor = ThemeUtil.Darken(BackgroundColor, 0.07);
            this.PanelGradiantMiddleColor = ThemeUtil.Lighten(BackgroundColor, 0.03);
            this.PanelGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
            
            // NeoMorphism borders - soft gray
            this.BorderColor = Color.FromArgb(220, 220, 225);  // Soft gray
            this.ActiveBorderColor = Color.FromArgb(76, 110, 245);  // Blue accent
            this.InactiveBorderColor = Color.FromArgb(196, 203, 210);  // Light gray
            
            // NeoMorphism palette
            this.PrimaryColor = Color.FromArgb(76, 110, 245);  // Blue
            this.SecondaryColor = Color.FromArgb(129, 140, 248);  // Lighter blue
            this.AccentColor = Color.FromArgb(255, 173, 94);  // Orange
            
            // Status colors
            this.ErrorColor = Color.FromArgb(231, 76, 60);  // Red
            this.WarningColor = Color.FromArgb(255, 159, 67);  // Orange
            this.SuccessColor = Color.FromArgb(46, 204, 113);  // Green
            
            // On-colors for readability
            this.OnPrimaryColor = Color.FromArgb(255, 255, 255);  // White on blue
            this.OnBackgroundColor = Color.FromArgb(50, 50, 60);  // Dark on light
            this.FocusIndicatorColor = Color.FromArgb(76, 110, 245);  // Blue focus
            ThemeContrastHelper.ValidateTheme(this, targetRatio: 4.5, autofix: true);
        }
    }
}
