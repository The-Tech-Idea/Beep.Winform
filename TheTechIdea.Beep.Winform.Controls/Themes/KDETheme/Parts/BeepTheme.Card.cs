using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class KDETheme
    {
        private void ApplyCard()
        {
            this.CardTextForeColor = Color.FromArgb(33,37,41);
            this.CardBackColor = Color.FromArgb(248,249,250);
            this.CardTitleForeColor = Color.FromArgb(33,37,41);
            this.CardSubTitleForeColor = Color.FromArgb(33,37,41);
            this.CardrGradiantStartColor = Color.FromArgb(248,249,250);
            this.CardGradiantEndColor = Color.FromArgb(248,249,250);
            this.CardGradiantMiddleColor = Color.FromArgb(248,249,250);
            this.CardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}