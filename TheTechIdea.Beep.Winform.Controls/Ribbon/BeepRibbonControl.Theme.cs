using TheTechIdea.Beep.Winform.Controls.Gallery;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;
using TheTechIdea.Beep.Winform.Controls.Tokens;
using TheTechIdea.Beep.Winform.Controls.Tokens;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepRibbonControl
    {
        private void ApplyTheme()
        {
            BackColor = _theme.Background;
            ForeColor = _theme.Text;
            Font = BeepThemesManager.ToFont(_theme.CommandTypography);

            _tabs.BackColor = _theme.Background;
            _tabs.ForeColor = _theme.Text;
            _tabs.Font = BeepThemesManager.ToFont(_theme.TabTypography);

            _quickAccess.BackColor = _theme.QuickAccessBack;
            _quickAccess.ForeColor = _theme.Text;
            _quickAccess.Font = BeepThemesManager.ToFont(_theme.CommandTypography);
            _quickAccess.Padding = new Padding(_theme.ItemSpacing, 2, _theme.ItemSpacing, 2);
            _quickAccess.Renderer = new BeepRibbonToolStripRenderer(this);
            _superTooltip.ApplyTheme(_theme);
            _minimizedTabPopup.Font = BeepThemesManager.ToFont(_theme.CommandTypography);
            _minimizedTabPopup.ForeColor = _theme.Text;
            _minimizedTabPopup.BackColor = _theme.GroupBack;
            _minimizedTabPopup.Renderer = new BeepRibbonToolStripRenderer(this);
            _searchBox.BackColor = _theme.TabActiveBack;
            _searchBox.ForeColor = _theme.Text;
            _searchBox.Font = BeepThemesManager.ToFont(_theme.CommandTypography);
            _searchResultsButton.ForeColor = _theme.Text;
            _searchResultsButton.BackColor = _theme.QuickAccessBack;
            _searchResultsButton.Font = BeepThemesManager.ToFont(_theme.CommandTypography);
            _keyTipToolTip.BackColor = _theme.TabActiveBack;
            _keyTipToolTip.ForeColor = _theme.Text;

            _contextHeader.BackColor = _theme.Background;
            _contextHeader.ForeColor = _theme.Text;
            _contextHeader.Font = BeepThemesManager.ToFont(_theme.ContextHeaderTypography);

            _backstagePanelContent.BackColor = _theme.Background;
            _backstageSplit.BackColor = _theme.Background;
            _backstageNavList.BackColor = _theme.GroupBack;
            _backstageNavList.ForeColor = _theme.Text;
            _backstageNavList.Font = BeepThemesManager.ToFont(_theme.GroupTypography);
            _backstageContentHost.BackColor = _theme.Background;
            _backstageTitle.BackColor = _theme.GroupBack;
            _backstageTitle.ForeColor = _theme.Text;
            _backstageTitle.Font = BeepThemesManager.ToFont(_theme.TabTypography);
            _backstageActions.BackColor = _theme.Background;
            _backstageFooter.BackColor = _theme.Background;
            foreach (var control in _backstageActions.Controls.OfType<Button>())
            {
                control.BackColor = _theme.TabActiveBack;
                control.ForeColor = _theme.Text;
                control.Font = BeepThemesManager.ToFont(_theme.CommandTypography);
                control.FlatAppearance.BorderColor = _theme.GroupBorder;
                control.FlatAppearance.MouseDownBackColor = _theme.GroupBack;
                control.FlatAppearance.MouseOverBackColor = ColorUtils.ShiftLuminance(_theme.GroupBack, .1f);
            }
            foreach (var control in _backstageFooter.Controls.OfType<Button>())
            {
                control.BackColor = _theme.TabActiveBack;
                control.ForeColor = _theme.Text;
                control.Font = BeepThemesManager.ToFont(_theme.CommandTypography);
                control.FlatAppearance.BorderColor = _theme.GroupBorder;
                control.FlatAppearance.MouseDownBackColor = _theme.GroupBack;
                control.FlatAppearance.MouseOverBackColor = ColorUtils.ShiftLuminance(_theme.GroupBack, .1f);
            }

            foreach (TabPage page in _tabs.TabPages)
            {
                page.BackColor = _theme.TabActiveBack;
                page.ForeColor = _theme.Text;

                foreach (var group in page.Controls.OfType<BeepRibbonGroup>())
                {
                    group.Renderer = new BeepRibbonToolStripRenderer(this);
                    group.Density = _density;
                    group.ApplyTheme(_theme);

                    foreach (var host in group.Items.OfType<ToolStripControlHost>())
                    {
                        if (host.Control is BeepRibbonGallery gallery)
                        {
                            gallery.ApplyTheme(_theme, _density);
                        }
                    }
                }
            }

            ApplyRightToLeftLayout();
            Invalidate();
        }

        private void ApplyRightToLeftLayout()
        {
            var rtl = _ribbonRightToLeft ? RightToLeft.Yes : RightToLeft.No;
            RightToLeft = rtl;
            _tabs.RightToLeft = rtl;
            _quickAccess.RightToLeft = rtl;
            _contextHeader.RightToLeft = rtl;
            _backstagePanelContent.RightToLeft = rtl;
            _backstageSplit.RightToLeft = rtl;
            _backstageNavList.RightToLeft = rtl;
            _backstageContentHost.RightToLeft = rtl;
            _backstageTitle.RightToLeft = rtl;
            _backstageActions.RightToLeft = rtl;
            _backstageFooter.RightToLeft = rtl;
            _backstageFooter.FlowDirection = _ribbonRightToLeft ? FlowDirection.LeftToRight : FlowDirection.RightToLeft;
            ApplyRightToLeftToRibbonPages(rtl);
            ApplyRightToLeftRecursive(_backstageActions, rtl);
            ApplyRightToLeftRecursive(_backstageFooter, rtl);
            ApplySearchAccessibility();
            ApplyPaneTabOrder();
        }

        private void ApplyRightToLeftToRibbonPages(RightToLeft rtl)
        {
            foreach (TabPage page in _tabs.TabPages)
            {
                page.RightToLeft = rtl;
                ApplyRightToLeftRecursive(page, rtl);
                foreach (var group in page.Controls.OfType<BeepRibbonGroup>())
                {
                    group.RightToLeft = rtl;
                    foreach (var host in group.Items.OfType<ToolStripControlHost>())
                    {
                        if (host.Control != null)
                        {
                            host.Control.RightToLeft = rtl;
                        }
                    }
                }
            }
        }

        private static void ApplyRightToLeftRecursive(Control root, RightToLeft rtl)
        {
            if (root == null)
            {
                return;
            }

            root.RightToLeft = rtl;
            foreach (Control child in root.Controls)
            {
                ApplyRightToLeftRecursive(child, rtl);
            }
        }

        public void ApplyThemeFromBeep(IBeepTheme? theme)
        {
            FormStyle style = _followGlobalFormStyle ? BeepThemesManager.CurrentStyle : _ribbonFormStyle;
            ApplyThemeFromBeep(theme, style);
        }

        public void ApplyThemeFromBeep(IBeepTheme? theme, FormStyle formStyle)
        {
            _ribbonFormStyle = formStyle;
            _resolvedStylePreset = RibbonThemeMapper.GetPreset(_ribbonFormStyle, _darkMode);
            Theme = RibbonThemeMapper.Map(theme, _darkMode, _ribbonFormStyle);
        }

        public void SyncWithGlobalThemeAndStyle()
        {
            _ribbonFormStyle = BeepThemesManager.CurrentStyle;
            _resolvedStylePreset = RibbonThemeMapper.GetPreset(_ribbonFormStyle, _darkMode);
            ApplyThemeFromBeep(BeepThemesManager.CurrentTheme, _ribbonFormStyle);
        }

        public bool LoadThemeFromTokenFile(string filePath, string mode = "light")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                {
                    return false;
                }

                var result = RibbonTokenImporter.ImportWithDiagnosticsFromFile(filePath, mode, _theme);
                _lastTokenImportDiagnostics.Clear();
                _lastTokenImportDiagnostics.AddRange(result.Diagnostics);
                Theme = result.Theme;
                return true;
            }
            catch
            {
                _lastTokenImportDiagnostics.Clear();
                _lastTokenImportDiagnostics.Add("Failed to load token file.");
                return false;
            }
        }
    }
}
