using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GruvBoxTheme
    {
        private void ApplyButtons()
        {
            this.ButtonHoverBackColor = Color.FromArgb(40,40,40);
            this.ButtonHoverForeColor = Color.FromArgb(235,219,178);
            this.ButtonHoverBorderColor = Color.FromArgb(168,153,132);
            this.ButtonSelectedBorderColor = Color.FromArgb(168,153,132);
            this.ButtonSelectedBackColor = Color.FromArgb(40,40,40);
            this.ButtonSelectedForeColor = Color.FromArgb(235,219,178);
            this.ButtonSelectedHoverBackColor = Color.FromArgb(40,40,40);
            this.ButtonSelectedHoverForeColor = Color.FromArgb(235,219,178);
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(168,153,132);
            this.ButtonBackColor = Color.FromArgb(40,40,40);
            this.ButtonForeColor = Color.FromArgb(235,219,178);
            this.ButtonBorderColor = Color.FromArgb(168,153,132);
            this.ButtonErrorBackColor = Color.FromArgb(40,40,40);
            this.ButtonErrorForeColor = Color.FromArgb(235,219,178);
            this.ButtonErrorBorderColor = Color.FromArgb(168,153,132);
            this.ButtonPressedBackColor = Color.FromArgb(40,40,40);
            this.ButtonPressedForeColor = Color.FromArgb(235,219,178);
            this.ButtonPressedBorderColor = Color.FromArgb(168,153,132);
        }
    }
}