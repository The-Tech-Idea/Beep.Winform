using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ArcLinuxTheme
    {
        private void ApplyCard()
        {
            this.CardTextForeColor = ForeColor;
            this.CardBackColor = SurfaceColor;
            this.CardTitleForeColor = ForeColor;
            this.CardSubTitleForeColor = ThemeUtil.Lighten(ForeColor, 0.08);
            this.CardrGradiantStartColor = ThemeUtil.Darken(SurfaceColor, 0.04);
            this.CardGradiantEndColor = SurfaceColor;
            this.CardGradiantMiddleColor = ThemeUtil.Lighten(SurfaceColor, 0.03);
            this.CardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
