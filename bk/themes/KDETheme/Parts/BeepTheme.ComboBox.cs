using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class KDETheme
    {
        private void ApplyComboBox()
        {
            this.ComboBoxBackColor = Color.FromArgb(248,249,250);
            this.ComboBoxForeColor = Color.FromArgb(33,37,41);
            this.ComboBoxBorderColor = Color.FromArgb(222,226,230);
            this.ComboBoxHoverBackColor = Color.FromArgb(248,249,250);
            this.ComboBoxHoverForeColor = Color.FromArgb(33,37,41);
            this.ComboBoxHoverBorderColor = Color.FromArgb(222,226,230);
            this.ComboBoxSelectedBackColor = Color.FromArgb(248,249,250);
            this.ComboBoxSelectedForeColor = Color.FromArgb(33,37,41);
            this.ComboBoxSelectedBorderColor = Color.FromArgb(222,226,230);
            this.ComboBoxErrorBackColor = Color.FromArgb(220,53,69);
            this.ComboBoxErrorForeColor = Color.FromArgb(33,37,41);
        }
    }
}