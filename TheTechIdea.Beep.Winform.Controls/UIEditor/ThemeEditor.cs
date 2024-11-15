using TheTechIdea.Beep.Vis.Modules;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace TheTechIdea.Beep.Winform.Controls.UIEditor
{
    public class ThemeEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            // This indicates the editor will use a dropdown.
            return UITypeEditorEditStyle.DropDown;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

            if (editorService != null)
            {
                // Create a dropdown or custom UI to allow theme selection
                ListBox themeListBox = new ListBox();

                // Add predefined themes to the ListBox

                themeListBox.Items.Add(BeepThemesManager.DarkTheme);
                themeListBox.Items.Add(BeepThemesManager.OceanTheme);
                themeListBox.Items.Add(BeepThemesManager.SunsetTheme);
                themeListBox.Items.Add(BeepThemesManager.ForestTheme);
                themeListBox.Items.Add(BeepThemesManager.AutumnTheme);
                themeListBox.Items.Add(BeepThemesManager.WinterTheme);
                themeListBox.Items.Add(BeepThemesManager.CandyTheme);
                themeListBox.Items.Add(BeepThemesManager.ZenTheme);
                themeListBox.Items.Add(BeepThemesManager.RoyalTheme);
                themeListBox.Items.Add(BeepThemesManager.RetroTheme);
                themeListBox.Items.Add(BeepThemesManager.HighlightTheme);
                themeListBox.Items.Add(BeepThemesManager.EarthyTheme); // Add EarthyTheme
                themeListBox.Items.Add(BeepThemesManager.ModernDarkTheme); // Add ModernDarkTheme
                themeListBox.Items.Add(BeepThemesManager.MaterialDesignTheme); // Add MaterialDesignTheme
                themeListBox.Items.Add(BeepThemesManager.NeumorphismTheme); // Add NeumorphismTheme
                themeListBox.Items.Add(BeepThemesManager.GlassmorphismTheme); // Add GlassmorphismTheme
                themeListBox.Items.Add(BeepThemesManager.FlatDesignTheme); // Add FlatDesignTheme
                themeListBox.Items.Add(BeepThemesManager.CyberpunkNeonTheme); // Add CyberpunkNeonTheme
                themeListBox.Items.Add(BeepThemesManager.LuxuryGoldTheme); // Add LuxuryGoldTheme
                themeListBox.Items.Add(BeepThemesManager.GradientBurstTheme); // Add GradientBurstTheme
                themeListBox.Items.Add(BeepThemesManager.HighContrastTheme); // Add HighContrastTheme
                themeListBox.Items.Add(BeepThemesManager.MonochromeTheme); // Add MonochromeTheme

                // Set default selection if any
                if (value != null && themeListBox.Items.Contains(value))
                {
                    themeListBox.SelectedItem = value;
                }

                // Close the dropdown when an item is selected
                themeListBox.SelectedIndexChanged += (s, e) =>
                {
                    editorService.CloseDropDown();
                };

                // Show dropdown
                editorService.DropDownControl(themeListBox);

                // Return the selected theme
                if (themeListBox.SelectedItem != null)
                {
                    value = themeListBox.SelectedItem;
                }
            }

            return value; // Return the selected or current value
        }
    }
}
