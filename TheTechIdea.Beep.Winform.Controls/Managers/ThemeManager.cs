using TheTechIdea.Beep.Vis.Modules;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Grid;
using TheTechIdea.Beep.Winform.Controls.Template;

namespace TheTechIdea.Beep.Winform.Controls.Managers
{
    public static class ThemeManager
    {
        public static Panel MenuPanel { get; set; }
        public static Panel StatusPanel { get; set; }
        private static BeepTheme _theme = BeepThemesManager.LightTheme;
        private static string _themeName = "LightTheme";
        private static EnumBeepThemes _themeEnum = EnumBeepThemes.LightTheme;

        [TypeConverter(typeof(ThemeConverter))]
        public static EnumBeepThemes Theme
        {
            get => _themeEnum;
            set
            {
                _themeEnum = value;
                _theme = BeepThemesManager.GetTheme(value);  // Get the theme by enum value
                _themeName = BeepThemesManager.GetThemeName(value);  // Get the theme name
                ApplyTheme();  // Apply the selected theme when it changes
            }
        }

        public static void InitializeTheme()
        {
            // Set default theme if not already set
            if (_theme == null)
            {
                _theme = BeepThemesManager.LightTheme;
            }
        }

        public static bool ShouldSerializeTheme()
        {
            // Only serialize the theme if it's not the default theme
            return _theme != BeepThemesManager.LightTheme;
        }

        // Method to apply theme to a main panel and all of its child controls
        public static void ApplyThemeToAllComponents(Panel panel)
        {
            MenuPanel = panel;
            MenuPanel.BackColor = _theme.PanelBackColor;

            // Loop through each control in the panel and apply the appropriate theme properties
            foreach (Control control in MenuPanel.Controls)
            {
                ApplyThemeToControl(control);
            }
        }

        // Core theme application method for each control
        public static void ApplyThemeToControl(Control control)
        {
            switch (control)
            {
                case Button button:
                    button.BackColor = _theme.ButtonBackColor;
                    button.ForeColor = _theme.ButtonForeColor;
                    button.FlatAppearance.BorderColor = _theme.BorderColor;
                    button.MouseEnter += (s, e) => button.BackColor = _theme.ButtonHoverBackColor;
                    button.MouseLeave += (s, e) => button.BackColor = _theme.ButtonBackColor;
                    break;

                case Label label:
                    label.BackColor = _theme.LabelBackColor;
                    label.ForeColor = _theme.LabelForeColor;
                    break;

                case TextBox textBox:
                    textBox.BackColor = _theme.TextBoxBackColor;
                    textBox.ForeColor = _theme.TextBoxForeColor;
                    break;

                case ComboBox comboBox:
                    comboBox.BackColor = _theme.ComboBoxBackColor;
                    comboBox.ForeColor = _theme.ComboBoxForeColor;
                    break;

                case CheckBox checkBox:
                    checkBox.BackColor = _theme.CheckBoxBackColor;
                    checkBox.ForeColor = _theme.CheckBoxForeColor;
                    break;

                case RadioButton radioButton:
                    radioButton.BackColor = _theme.RadioButtonBackColor;
                    radioButton.ForeColor = _theme.RadioButtonForeColor;
                    break;

                case ListBox listBox:
                    listBox.BackColor = _theme.PanelBackColor;
                    listBox.ForeColor = _theme.RowForeColor;
                    break;

                case TreeView treeView:
                    treeView.BackColor = _theme.PanelBackColor;
                    treeView.ForeColor = _theme.RowForeColor;
                    break;

                case DataGridView grid:
                    grid.BackgroundColor = _theme.PanelBackColor;
                    grid.ForeColor = _theme.RowForeColor;
                    grid.GridColor = _theme.GridLineColor;
                    grid.AlternatingRowsDefaultCellStyle.BackColor = _theme.AltRowBackColor;
                    grid.DefaultCellStyle.BackColor = _theme.RowBackColor;
                    grid.DefaultCellStyle.ForeColor = _theme.RowForeColor;
                    grid.ColumnHeadersDefaultCellStyle.BackColor = _theme.HeaderBackColor;
                    grid.ColumnHeadersDefaultCellStyle.ForeColor = _theme.HeaderForeColor;
                    break;

                case BeepSimpleGrid beepGrid:
                    beepGrid.BackColor = _theme.PanelBackColor;
                    beepGrid.ForeColor = _theme.RowForeColor;
                    //beepGrid.GridView.GridColor = _theme.GridLineColor;
                    //beepGrid.GridView.AlternatingRowsDefaultCellStyle.BackColor = _theme.AltRowBackColor;
                    //beepGrid.GridView.DefaultCellStyle.BackColor = _theme.RowBackColor;
                    //beepGrid.GridView.DefaultCellStyle.ForeColor = _theme.RowForeColor;
                    //beepGrid.GridView.ColumnHeadersDefaultCellStyle.BackColor = _theme.HeaderBackColor;
                    //beepGrid.GridView.ColumnHeadersDefaultCellStyle.ForeColor = _theme.HeaderForeColor;
                    break;

                case Panel subPanel:
                    subPanel.BackColor = _theme.PanelBackColor;
                    break;

                    // Add additional cases for any other control types as necessary
            }
        }

        // Apply the theme to the menu panel and status panel (if applicable)
        public static void ApplyTheme()
        {
            if (MenuPanel != null)
            {
                MenuPanel.BackColor = _theme.PanelBackColor;

                // Loop through controls and apply the theme
                foreach (Control control in MenuPanel.Controls)
                {
                    ApplyThemeToControl(control);
                }
            }

            if (StatusPanel != null)
            {
                StatusPanel.BackColor = _theme.PanelBackColor;
            }

            // Additional logic for status bars, headers, etc.
        }

        // Method to manually set and apply a theme
        public static void SetTheme(BeepTheme theme)
        {
            _theme = theme;
            ApplyTheme();
        }
    }
}
