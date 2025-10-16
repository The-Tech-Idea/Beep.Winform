using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordicTheme
    {
        private void ApplyStatusBar()
        {
            this.StatusBarBackColor = Color.FromArgb(250,250,251);
            this.StatusBarForeColor = Color.FromArgb(31,41,55);
            this.StatusBarBorderColor = Color.FromArgb(229,231,235);
            this.StatusBarHoverBackColor = Color.FromArgb(250,250,251);
            this.StatusBarHoverForeColor = Color.FromArgb(31,41,55);
            this.StatusBarHoverBorderColor = Color.FromArgb(229,231,235);
        }
    }
}