using System.ComponentModel;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Customization;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepRibbonControl
    {
        // Event declarations
        public event EventHandler<RibbonCommandInvokedEventArgs>? CommandInvoked;
        public event EventHandler<BackstageCommandInvokedEventArgs>? BackstageCommandInvoked;
        public event EventHandler<BackstageSectionChangedEventArgs>? BackstageSectionChanged;
        public event EventHandler<RibbonMergedEventArgs>? RibbonMerged;
        public event EventHandler? RibbonMinimizedChanged;
        public event EventHandler? RibbonCustomizationRequested;
        public event EventHandler<RibbonCustomizationAppliedEventArgs>? RibbonCustomizationApplied;
        public event EventHandler? RibbonCustomizationReset;
        public event EventHandler? RibbonCustomizationCanceled;
        public event EventHandler<RibbonSearchExecutedEventArgs>? SearchExecuted;
        public event EventHandler<RibbonTooltipActionRequestedEventArgs>? TooltipActionRequested;

        // Theme manager subscription handlers
        private void TrySubscribeThemeManager()
        {
            if (_subscribedToThemeManager) return;
            try
            {
                BeepThemesManager.ThemeChanged -= OnGlobalThemeChanged;
                BeepThemesManager.ThemeChanged += OnGlobalThemeChanged;
                BeepThemesManager.FormStyleChanged -= OnGlobalFormStyleChanged;
                BeepThemesManager.FormStyleChanged += OnGlobalFormStyleChanged;
                _subscribedToThemeManager = true;
            }
            catch
            {
                // best effort only
            }
        }

        private void OnGlobalThemeChanged(object? sender, ThemeChangeEventArgs e)
        {
            if (IsDisposed || !_followGlobalFormStyle) return;
            try
            {
                var nextTheme = e?.NewTheme ?? BeepThemesManager.CurrentTheme;
                ApplyThemeFromBeep(nextTheme, _ribbonFormStyle);
            }
            catch
            {
                // keep ribbon stable if theme manager fails
            }
        }

        private void OnGlobalFormStyleChanged(object? sender, StyleChangeEventArgs e)
        {
            if (IsDisposed || !_followGlobalFormStyle) return;
            _ribbonFormStyle = e.NewStyle;
            _resolvedStylePreset = RibbonThemeMapper.GetPreset(_ribbonFormStyle, _darkMode);
            // FormStyleChanged fires before BeepThemesManager.CurrentThemeName is updated.
            // ThemeChanged will fire immediately after with the correct new theme.
        }

        private void UnsubscribeThemeManager()
        {
            if (!_subscribedToThemeManager) return;
            try
            {
                BeepThemesManager.ThemeChanged -= OnGlobalThemeChanged;
                BeepThemesManager.FormStyleChanged -= OnGlobalFormStyleChanged;
            }
            catch
            {
                // no-op
            }
            _subscribedToThemeManager = false;
        }

        // List changed handlers
        private void CommandItems_ListChanged(object? sender, ListChangedEventArgs e)
        {
            if (_suspendCommandRebuild) return;
            if (DesignMode)
            {
                try
                {
                    if (_commandItems.Count > 0)
                        BuildFromSimpleItems();
                    else
                        ShowPlaceholder();
                }
                catch { ShowPlaceholder(); }
                return;
            }
            BuildFromSimpleItems();
        }

        private void ShowPlaceholder()
        {
            _tabStrip.Clear();
            _ribbonContentHost.Controls.Clear();
            var lbl = new Label
            {
                Text = "Beep Ribbon Control\n\nAdd tabs, groups, and buttons via the CommandItems collection\nor use the smart-tag \"Add Sample Tabs\" action.",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = SystemColors.GrayText
            };
            _ribbonContentHost.Controls.Add(lbl);
        }

        private void BackstageItems_ListChanged(object? sender, ListChangedEventArgs e)
        {
            BuildBackstageFromSimpleItems();
        }

        private void BackstageFooterItems_ListChanged(object? sender, ListChangedEventArgs e)
        {
            BuildBackstageFooterActions();
        }

        // Tab context menu and customization handlers
        private void ShowTabHeaderContextMenu(Point location)
        {
            var menu = new ContextMenuStrip();
            menu.Font = BeepThemesManager.ToFont(_theme.CommandTypography);

            string minimizeText = _isMinimized ? "Show the Ribbon" : "Minimize the Ribbon";
            menu.Items.Add(minimizeText, null, (_, __) =>
            {
                if (_allowMinimize)
                {
                    IsMinimized = !_isMinimized;
                }
            });

            string qatPlacementText = _quickAccessAboveRibbon
                ? "Show Quick Access Toolbar Below the Ribbon"
                : "Show Quick Access Toolbar Above the Ribbon";
            menu.Items.Add(qatPlacementText, null, (_, __) => QuickAccessAboveRibbon = !_quickAccessAboveRibbon);

            var layoutMenu = new ToolStripMenuItem("Ribbon Layout");
            AddCheckedMenuItem(layoutMenu, "Classic", _layoutMode == RibbonLayoutMode.Classic, () => LayoutMode = RibbonLayoutMode.Classic);
            AddCheckedMenuItem(layoutMenu, "Simplified", _layoutMode == RibbonLayoutMode.Simplified, () => LayoutMode = RibbonLayoutMode.Simplified);
            menu.Items.Add(layoutMenu);

            var densityMenu = new ToolStripMenuItem("Ribbon Density");
            AddCheckedMenuItem(densityMenu, "Comfortable", _density == RibbonDensity.Comfortable, () => Density = RibbonDensity.Comfortable);
            AddCheckedMenuItem(densityMenu, "Compact", _density == RibbonDensity.Compact, () => Density = RibbonDensity.Compact);
            AddCheckedMenuItem(densityMenu, "Touch", _density == RibbonDensity.Touch, () => Density = RibbonDensity.Touch);
            menu.Items.Add(densityMenu);

            menu.Items.Add(new ToolStripSeparator());
            AddCheckedMenuItem(menu.Items, "Dark Mode", _darkMode, () => DarkMode = !_darkMode);
            AddCheckedMenuItem(menu.Items, "Search Includes Backstage", _searchIncludeBackstage, () => SearchIncludeBackstage = !_searchIncludeBackstage);
            AddCheckedMenuItem(menu.Items, "Reduced Motion", _reducedMotion, () => ReducedMotion = !_reducedMotion);

            if ((_personalizationOptions & (RibbonPersonalizationOptions.RibbonTabs | RibbonPersonalizationOptions.RibbonGroups | RibbonPersonalizationOptions.CommandOrder)) != 0)
            {
                menu.Items.Add(new ToolStripSeparator());
                menu.Items.Add(CreateCustomizeRibbonMenuItem());
            }

            menu.Closed += (_, __) => menu.Dispose();
            menu.Show(_tabStrip, location);
        }

        private void RequestRibbonCustomization()
        {
            if (RibbonCustomizationRequested != null)
            {
                RibbonCustomizationRequested.Invoke(this, EventArgs.Empty);
                return;
            }

            ShowCustomizeRibbonDialog(FindForm());
        }

        public ToolStripMenuItem CreateCustomizeRibbonMenuItem(string text = "Customize Ribbon...")
        {
            var item = new ToolStripMenuItem(string.IsNullOrWhiteSpace(text) ? "Customize Ribbon..." : text)
            {
                AccessibleName = "Customize ribbon",
                AccessibleDescription = "Open ribbon customization for tabs, groups, and quick access commands."
            };
            item.Click += (_, __) => RequestRibbonCustomization();
            return item;
        }

        private static void AddCheckedMenuItem(ToolStripItemCollection items, string text, bool isChecked, Action onClick)
        {
            var item = new ToolStripMenuItem(text)
            {
                Checked = isChecked,
                CheckOnClick = false
            };
            item.Click += (_, __) => onClick();
            items.Add(item);
        }

        private static void AddCheckedMenuItem(ToolStripMenuItem parent, string text, bool isChecked, Action onClick)
        {
            AddCheckedMenuItem(parent.DropDownItems, text, isChecked, onClick);
        }

        private void Tabs_MouseDoubleClick(object? sender, MouseEventArgs e)
        {
            if (!_allowMinimize) return;
            for (int i = 0; i < Tabs.Tabs.Count; i++)
            {
                var rect = _tabStrip.GetTabRect(i);
                if (rect.Contains(e.Location))
                {
                    ToggleMinimized();
                    break;
                }
            }
        }

        private int GetTabIndexAt(Point location)
        {
            for (int i = 0; i < Tabs.Tabs.Count; i++)
            {
                if (_tabStrip.GetTabRect(i).Contains(location))
                    return i;
            }
            return -1;
        }

        private void Tabs_MouseUp(object? sender, MouseEventArgs e)
        {
            int tabIndex = GetTabIndexAt(e.Location);
            if (e.Button == MouseButtons.Right)
            {
                ShowTabHeaderContextMenu(e.Location);
                return;
            }
            if (e.Button != MouseButtons.Left || !_isMinimized || !_showMinimizedPopupOnTabClick || tabIndex < 0)
                return;
            ShowMinimizedPopupForTabIndex(tabIndex);
        }
    }
}
