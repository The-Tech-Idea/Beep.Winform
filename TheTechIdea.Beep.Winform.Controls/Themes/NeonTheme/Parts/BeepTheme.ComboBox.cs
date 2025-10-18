using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeonTheme
    {
        private void ApplyComboBox()
        {
            this.ComboBoxBackColor = Color.FromArgb(10,12,20);
            this.ComboBoxForeColor = Color.FromArgb(235,245,255);
            this.ComboBoxBorderColor = Color.FromArgb(60,70,100);
            this.ComboBoxHoverBackColor = Color.FromArgb(10,12,20);
            this.ComboBoxHoverForeColor = Color.FromArgb(235,245,255);
            this.ComboBoxHoverBorderColor = Color.FromArgb(60,70,100);
            this.ComboBoxSelectedBackColor = Color.FromArgb(10,12,20);
            this.ComboBoxSelectedForeColor = Color.FromArgb(235,245,255);
            this.ComboBoxSelectedBorderColor = Color.FromArgb(60,70,100);
            this.ComboBoxErrorBackColor = Color.FromArgb(10,12,20);
            this.ComboBoxErrorForeColor = Color.FromArgb(235,245,255);
        }
    }
}