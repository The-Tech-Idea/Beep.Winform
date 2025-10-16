using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CyberpunkTheme
    {
        private void ApplyComboBox()
        {
            this.ComboBoxBackColor = Color.FromArgb(10,8,20);
            this.ComboBoxForeColor = Color.FromArgb(228,244,255);
            this.ComboBoxBorderColor = Color.FromArgb(90,20,110);
            this.ComboBoxHoverBackColor = Color.FromArgb(10,8,20);
            this.ComboBoxHoverForeColor = Color.FromArgb(228,244,255);
            this.ComboBoxHoverBorderColor = Color.FromArgb(90,20,110);
            this.ComboBoxSelectedBackColor = Color.FromArgb(10,8,20);
            this.ComboBoxSelectedForeColor = Color.FromArgb(228,244,255);
            this.ComboBoxSelectedBorderColor = Color.FromArgb(90,20,110);
            this.ComboBoxErrorBackColor = Color.FromArgb(10,8,20);
            this.ComboBoxErrorForeColor = Color.FromArgb(228,244,255);
        }
    }
}