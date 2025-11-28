using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Themes.ThemeContrastUtilities;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class KDETheme
    {
        private void ApplyColorPalette()
        {
            this.ForeColor = Color.FromArgb(33,37,41);
            this.BackColor = Color.FromArgb(248,249,250);
            this.BackgroundColor = Color.FromArgb(248,249,250);
            this.SurfaceColor = Color.FromArgb(255,255,255);
            this.PanelBackColor = Color.FromArgb(248,249,250);
            this.PanelGradiantStartColor = ThemeUtil.Lighten(BackgroundColor, 0.02);
            this.PanelGradiantEndColor = ThemeUtil.Darken(BackgroundColor, 0.04);
            this.PanelGradiantMiddleColor = ThemeUtil.Lighten(BackgroundColor, 0.01);
            this.PanelGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.BorderColor = Color.FromArgb(222,226,230);
            this.ActiveBorderColor = Color.FromArgb(61,174,233);
            this.InactiveBorderColor = Color.FromArgb(200,205,210);
            this.PrimaryColor = Color.FromArgb(61,174,233);
            this.SecondaryColor = Color.FromArgb(41,128,185);
            this.AccentColor = Color.FromArgb(0,188,212);
            this.ErrorColor = Color.FromArgb(220,53,69);
            this.WarningColor = Color.FromArgb(255,193,7);
            this.SuccessColor = Color.FromArgb(46,204,113);
            this.OnPrimaryColor = Color.FromArgb(61,174,233);
            this.ShadowColor = PanelBackColor;
            this.OnBackgroundColor = ForeColor;
            this.FocusIndicatorColor = Color.FromArgb(61,174,233);
            ThemeContrastHelper.ValidateTheme(this, targetRatio: 4.5, autofix: true);
        }
    }
}
