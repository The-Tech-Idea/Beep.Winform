
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CartoonTheme : DefaultBeepTheme
    {
        Color InfoColor = Color.FromArgb(0, 122, 204);  // Blue
        Color OnInfoColor = Color.White;  // White on blue
        Color OnDisabledColor = Color.FromArgb(130, 130, 130);  // Greyed out
        Color DisabledColor = Color.FromArgb(200, 200, 200);  // Light grey
        Color OnWarningColor = Color.White;  // White on orange
        Color PlaceHolderColor = Color.FromArgb(160, 160, 160);  // Light grey
        Color ShadowOpacityColor = Color.FromArgb(0, 0, 0, 50);  // Semi-transparent black
        public CartoonTheme()
        {
            ThemeName = "CartoonTheme";
            ThemeGuid = Guid.NewGuid().ToString();
            IsDarkTheme = false;
            FontName = "Comic Sans MS";
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