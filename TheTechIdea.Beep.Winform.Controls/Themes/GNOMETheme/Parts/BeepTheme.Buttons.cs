using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GNOMETheme
    {
        private void ApplyButtons()
        {
            this.ButtonHoverBackColor = Color.FromArgb(246,245,244);
            this.ButtonHoverForeColor = Color.FromArgb(46,52,54);
            this.ButtonHoverBorderColor = Color.FromArgb(205,207,212);
            this.ButtonSelectedBorderColor = Color.FromArgb(205,207,212);
            this.ButtonSelectedBackColor = Color.FromArgb(246,245,244);
            this.ButtonSelectedForeColor = Color.FromArgb(46,52,54);
            this.ButtonSelectedHoverBackColor = Color.FromArgb(246,245,244);
            this.ButtonSelectedHoverForeColor = Color.FromArgb(46,52,54);
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(205,207,212);
            this.ButtonBackColor = Color.FromArgb(246,245,244);
            this.ButtonForeColor = Color.FromArgb(46,52,54);
            this.ButtonBorderColor = Color.FromArgb(205,207,212);
            this.ButtonErrorBackColor = Color.FromArgb(224,27,36);
            this.ButtonErrorForeColor = Color.FromArgb(46,52,54);
            this.ButtonErrorBorderColor = Color.FromArgb(205,207,212);
            this.ButtonPressedBackColor = Color.FromArgb(246,245,244);
            this.ButtonPressedForeColor = Color.FromArgb(46,52,54);
            this.ButtonPressedBorderColor = Color.FromArgb(205,207,212);
        }
    }
}