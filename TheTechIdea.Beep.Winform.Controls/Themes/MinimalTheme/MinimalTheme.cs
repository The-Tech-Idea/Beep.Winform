
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MinimalTheme : DefaultBeepTheme
    {
        public MinimalTheme()
        {
            ThemeName = "MinimalTheme";
            ThemeGuid = Guid.NewGuid().ToString();
            IsDarkTheme = false;
            FontName = "Inter";
            FontSize = 12f;
            ApplyColorPalette();
            ApplyCore();
            ApplyAppBar();
            ApplyTypography();
            ApplyButtons();
            ApplyLabels();
            ApplyLink();
            ApplyList();
            ApplyGrid();
            ApplyTextBox();
            ApplyComboBox();
            ApplyCheckBox();
            ApplyRadioButton();
            ApplyMenu();
            ApplyNavigation();
            ApplyProgressBar();
            ApplyTab();
            ApplySideMenu();
            ApplyCard();
            ApplyStatsCard();
            ApplyTaskCard();
            ApplyDashboard();
            ApplyDialog();
            ApplyCalendar();
            ApplyChart();
            ApplyTree();
            ApplyStatusBar();
            ApplyBadge();
            ApplyToolTip();
            ApplySwitch();
            ApplyStepper();
            ApplyGradient();
            ApplyIconography();
            ApplyLogin();
            ApplyMiscellaneous();}
    }
}