using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class KDETheme
    {
        private void ApplyButtons()
        {
            this.ButtonHoverBackColor = Color.FromArgb(248,249,250);
            this.ButtonHoverForeColor = Color.FromArgb(33,37,41);
            this.ButtonHoverBorderColor = Color.FromArgb(222,226,230);
            this.ButtonSelectedBorderColor = Color.FromArgb(222,226,230);
            this.ButtonSelectedBackColor = Color.FromArgb(248,249,250);
            this.ButtonSelectedForeColor = Color.FromArgb(33,37,41);
            this.ButtonSelectedHoverBackColor = Color.FromArgb(248,249,250);
            this.ButtonSelectedHoverForeColor = Color.FromArgb(33,37,41);
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(222,226,230);
            this.ButtonBackColor = Color.FromArgb(248,249,250);
            this.ButtonForeColor = Color.FromArgb(33,37,41);
            this.ButtonBorderColor = Color.FromArgb(222,226,230);
            this.ButtonErrorBackColor = Color.FromArgb(220,53,69);
            this.ButtonErrorForeColor = Color.FromArgb(33,37,41);
            this.ButtonErrorBorderColor = Color.FromArgb(222,226,230);
            this.ButtonPressedBackColor = Color.FromArgb(248,249,250);
            this.ButtonPressedForeColor = Color.FromArgb(33,37,41);
            this.ButtonPressedBorderColor = Color.FromArgb(222,226,230);
        }
    }
}