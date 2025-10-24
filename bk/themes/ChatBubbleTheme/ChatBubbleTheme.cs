
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ChatBubbleTheme : DefaultBeepTheme
    {
        public ChatBubbleTheme()
        {
            ThemeName = "ChatBubbleTheme";
            ThemeGuid = Guid.NewGuid().ToString();
            IsDarkTheme = false;
            FontName = "Nunito";
            FontSize = 12f;
            ApplyAppBar();
            ApplyBadge();
            ApplyMiscellaneous();
            ApplyTypography();
            ApplyColorPalette();
            ApplyButtons();
            ApplyCard();
            ApplyCalendar();
            ApplyChart();
            ApplyCheckBox();
            ApplyComboBox();
            ApplyCompany();
            ApplyDashboard();
            ApplyDialog();
            ApplyGradient();
            ApplyGrid();
            ApplyLabels();
            ApplyLink();
            ApplyList();
            ApplyLogin();
            ApplyMenu();
            ApplyCore();
            ApplyIconography();
            ApplyNavigation();
            ApplyProgressBar();
            ApplyRadioButton();
            ApplySideMenu();
            ApplyStatsCard();
            ApplyStatusBar();
            ApplyStepper();
            ApplySwitch();
            ApplyTab();
            ApplyTaskCard();
            ApplyTextBox();
            ApplyToolTip();
            ApplyTree();}
    }
}