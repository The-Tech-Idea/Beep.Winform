using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CartoonTheme
    {
        private void ApplyComboBox()
        {
            this.ComboBoxBackColor = Color.FromArgb(255,251,235);
            this.ComboBoxForeColor = Color.FromArgb(33,37,41);
            this.ComboBoxBorderColor = Color.FromArgb(247,208,136);
            this.ComboBoxHoverBackColor = Color.FromArgb(255,251,235);
            this.ComboBoxHoverForeColor = Color.FromArgb(33,37,41);
            this.ComboBoxHoverBorderColor = Color.FromArgb(247,208,136);
            this.ComboBoxSelectedBackColor = Color.FromArgb(255,251,235);
            this.ComboBoxSelectedForeColor = Color.FromArgb(33,37,41);
            this.ComboBoxSelectedBorderColor = Color.FromArgb(247,208,136);
            this.ComboBoxErrorBackColor = Color.FromArgb(255,82,82);
            this.ComboBoxErrorForeColor = Color.FromArgb(33,37,41);
        }
    }
}