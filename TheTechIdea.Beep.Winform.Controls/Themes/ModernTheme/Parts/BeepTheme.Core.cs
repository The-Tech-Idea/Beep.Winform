using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ModernTheme
    {
        private void ApplyCore()
        {
            this.ThemeGuid = Guid.NewGuid().ToString();
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.PanelGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.CardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.ChartDefaultSeriesColors = new System.Collections.Generic.List<System.Drawing.Color> { Color.FromArgb(100,150,230), Color.FromArgb(100,200,100), Color.FromArgb(180,140,200), Color.FromArgb(230,90,90) };
            this.DashboardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.FontName = "Segoe UI Variable";
            this.FontSize = 12.0f;
            this.GradientDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.FontFamily = string.Empty;
            this.FontSizeBlockHeader = 12.0f;
            this.FontSizeBlockText = 12.0f;
            this.FontSizeQuestion = 12.0f;
            this.FontSizeAnswer = 12.0f;
            this.FontSizeCaption = 12.0f;
            this.FontSizeButton = 12.0f;
            this.FontStyleRegular = System.Drawing.FontStyle.Regular;
            this.FontStyleBold = System.Drawing.FontStyle.Regular;
            this.FontStyleItalic = System.Drawing.FontStyle.Regular;
            this.PaddingSmall = 0;
            this.PaddingMedium = 0;
            this.PaddingLarge = 0;
            this.BorderRadius = 8;
            this.BorderSize = 1;
            this.IconSet = string.Empty;
            this.ApplyThemeToIcons = false;
            this.ShadowOpacity = 0.10f;
            this.AnimationDurationShort = 0f;
            this.AnimationDurationMedium = 0f;
            this.AnimationDurationLong = 0f;
            this.AnimationEasingFunction = string.Empty;
            this.HighContrastMode = false;
            this.IsDarkTheme = false;
            this.SideMenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.BlockquoteBorderWidth = 0f;
            this.BlockquotePadding = 0f;
            this.InlineCodePadding = 0f;
            this.CodeBlockBorderWidth = 0f;
            this.CodeBlockPadding = 0f;
            this.ListItemSpacing = 0f;
            this.ListIndentation = 0f;
            this.LinkIsUnderline = false;
        }
    }
}