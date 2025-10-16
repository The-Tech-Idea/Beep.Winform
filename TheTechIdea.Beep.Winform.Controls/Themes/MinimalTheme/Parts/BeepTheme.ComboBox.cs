using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MinimalTheme
    {
        private void ApplyComboBox()
        {
            this.ComboBoxBackColor = Color.FromArgb(255,255,255);
            this.ComboBoxForeColor = Color.FromArgb(31,41,55);
            this.ComboBoxBorderColor = Color.FromArgb(209,213,219);
            this.ComboBoxHoverBackColor = Color.FromArgb(255,255,255);
            this.ComboBoxHoverForeColor = Color.FromArgb(31,41,55);
            this.ComboBoxHoverBorderColor = Color.FromArgb(209,213,219);
            this.ComboBoxSelectedBackColor = Color.FromArgb(255,255,255);
            this.ComboBoxSelectedForeColor = Color.FromArgb(31,41,55);
            this.ComboBoxSelectedBorderColor = Color.FromArgb(209,213,219);
            this.ComboBoxErrorBackColor = Color.FromArgb(239,68,68);
            this.ComboBoxErrorForeColor = Color.FromArgb(31,41,55);
        }
    }
}