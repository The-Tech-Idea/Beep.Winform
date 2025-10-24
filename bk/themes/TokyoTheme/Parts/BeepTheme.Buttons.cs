using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class TokyoTheme
    {
        private void ApplyButtons()
        {
            this.ButtonHoverBackColor = Color.FromArgb(26,27,38);
            this.ButtonHoverForeColor = Color.FromArgb(192,202,245);
            this.ButtonHoverBorderColor = Color.FromArgb(86,95,137);
            this.ButtonSelectedBorderColor = Color.FromArgb(86,95,137);
            this.ButtonSelectedBackColor = Color.FromArgb(26,27,38);
            this.ButtonSelectedForeColor = Color.FromArgb(192,202,245);
            this.ButtonSelectedHoverBackColor = Color.FromArgb(26,27,38);
            this.ButtonSelectedHoverForeColor = Color.FromArgb(192,202,245);
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(86,95,137);
            this.ButtonBackColor = Color.FromArgb(26,27,38);
            this.ButtonForeColor = Color.FromArgb(192,202,245);
            this.ButtonBorderColor = Color.FromArgb(86,95,137);
            this.ButtonErrorBackColor = Color.FromArgb(26,27,38);
            this.ButtonErrorForeColor = Color.FromArgb(192,202,245);
            this.ButtonErrorBorderColor = Color.FromArgb(86,95,137);
            this.ButtonPressedBackColor = Color.FromArgb(26,27,38);
            this.ButtonPressedForeColor = Color.FromArgb(192,202,245);
            this.ButtonPressedBorderColor = Color.FromArgb(86,95,137);
        }
    }
}