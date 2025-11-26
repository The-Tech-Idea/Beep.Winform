using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ArcLinuxTheme
    {
        private void ApplyTextBox()
        {
            this.TextBoxBackColor = SurfaceColor;
            this.TextBoxForeColor = ForeColor;
            this.TextBoxBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
            this.TextBoxHoverBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
            this.TextBoxHoverBackColor = SurfaceColor;
            this.TextBoxHoverForeColor = ForeColor;
            this.TextBoxSelectedBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
            this.TextBoxSelectedBackColor = SurfaceColor;
            this.TextBoxSelectedForeColor = ForeColor;
            this.TextBoxPlaceholderColor = ThemeUtil.Lighten(ForeColor, 0.25); // keep readable on dark surface
            this.TextBoxErrorBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
            this.TextBoxErrorBackColor = ErrorColor;
            this.TextBoxErrorForeColor = ForeColor;
            this.TextBoxErrorTextColor = ForeColor;
            this.TextBoxErrorPlaceholderColor = ErrorColor;
            this.TextBoxErrorTextBoxColor = ErrorColor;
            this.TextBoxErrorTextBoxBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
            this.TextBoxErrorTextBoxHoverColor = ErrorColor;
        }
    }
}
