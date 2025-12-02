
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GlassTheme : DefaultBeepTheme
    {
        public GlassTheme()
        {
            ThemeName = "GlassTheme";
            ThemeGuid = Guid.NewGuid().ToString();
            IsDarkTheme = false;
            FontName = "Segoe UI";
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
            ApplyTree();
            
            // Final validation after all components are configured
            ThemeContrastUtilities.ThemeContrastHelper.ValidateTheme(this, targetRatio: 4.5, autofix: true);
        }
    }
}