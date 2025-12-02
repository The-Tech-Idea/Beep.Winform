
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class FluentTheme : DefaultBeepTheme
    {
        Color InfoColor = Color.FromArgb(0, 122, 204);  // Blue
        Color OnInfoColor = Color.White;  // White on blue
        Color OnDisabledColor = Color.FromArgb(130, 130, 130);  // Greyed out
        Color DisabledColor = Color.FromArgb(200, 200, 200);  // Light grey
        Color OnWarningColor = Color.White;  // White on orange
        Color PlaceHolderColor = Color.FromArgb(160, 160, 160);  // Light grey
        Color ShadowOpacityColor = Color.FromArgb(0, 0, 0, 50);  // Semi-transparent black
        public FluentTheme()
        {
            ThemeName = "FluentTheme";
            ThemeGuid = Guid.NewGuid().ToString();
            IsDarkTheme = false;
            FontName = "Segoe UI";
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
            ApplyMiscellaneous();
            
            // Final validation after all components are configured
            ThemeContrastUtilities.ThemeContrastHelper.ValidateTheme(this, targetRatio: 4.5, autofix: true);
        }
    }
}