using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class TokyoTheme
    {
        private void ApplyCard()
        {
            this.CardTextForeColor = Color.FromArgb(192,202,245);
            this.CardBackColor = Color.FromArgb(26,27,38);
            this.CardTitleForeColor = Color.FromArgb(192,202,245);
            this.CardSubTitleForeColor = Color.FromArgb(192,202,245);
            this.CardrGradiantStartColor = Color.FromArgb(26,27,38);
            this.CardGradiantEndColor = Color.FromArgb(26,27,38);
            this.CardGradiantMiddleColor = Color.FromArgb(26,27,38);
            this.CardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}