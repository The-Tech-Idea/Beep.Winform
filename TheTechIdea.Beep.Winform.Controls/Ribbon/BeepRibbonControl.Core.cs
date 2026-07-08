using System.ComponentModel;
using TheTechIdea.Beep.Winform.Controls.Accessibility;
using TheTechIdea.Beep.Winform.Controls.Backstage;
using TheTechIdea.Beep.Winform.Controls.Rendering;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepRibbonControl
    {
        public BeepRibbonControl()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            Controls.Add(_ribbonContentHost);
            Controls.Add(_tabStrip);
            Controls.Add(_contextHeader);
            Controls.Add(_quickAccess);
            _contextHeader.Height = ContextHeaderHeight;
            Height = DpiScalingHelper.ScaleValue(130, this);

            _commandItems.ListChanged += CommandItems_ListChanged;
            _backstageItems.ListChanged += BackstageItems_ListChanged;
            _backstageRecentItems.ListChanged += BackstageItems_ListChanged;
            _backstagePinnedItems.ListChanged += BackstageItems_ListChanged;
            _backstageFooterItems.ListChanged += BackstageFooterItems_ListChanged;

            _backstageButton = new ToolStripDropDownButton("File") { ShowDropDownArrow = true, AutoToolTip = false };
            _backstageDropDown = new ToolStripDropDown { Padding = Padding.Empty, AutoClose = true, AutoSize = false };
            InitializeBackstageLayout();
            _backstageHost = new ToolStripControlHost(_backstagePanelContent) { AutoSize = false, Size = _backstagePanelContent.Size, Margin = Padding.Empty, Padding = Padding.Empty };
            _backstageDropDown.Items.Add(_backstageHost);
            _backstageButton.DropDownOpening += BackstageButton_DropDownOpening;
            _backstageDropDown.Closed += BackstageDropDown_Closed;
            _backstageButton.DropDown = _backstageDropDown;
            _quickAccess.Items.Insert(0, _backstageButton);
            InitializeSearchControls();

            _quickAccess.Renderer = new BeepRibbonToolStripRenderer(this);
            _contextHeader.Paint += ContextHeader_Paint;

            _tabStrip.SelectedIndexChanged += (_, __) => ShowActiveContentPanel();
            _tabStrip.MouseDoubleClick += Tabs_MouseDoubleClick;
            _tabStrip.MouseUp += Tabs_MouseUp;

            _keyboardMap.Register(Keys.F6, () => FocusRibbonPane(1));
            _keyboardMap.Register(Keys.Shift | Keys.F6, () => FocusRibbonPane(-1));
            _contextTransitionTimer.Tick += ContextTransitionTimer_Tick;
            _backstageTransitionTimer.Tick += BackstageTransitionTimer_Tick;

            _tabStrip.ApplyTheme(_theme);
            ApplyRightToLeftLayout();
            ApplyQuickAccessPlacement();
            ApplySearchAccessibility();
            ApplyPaneTabOrder();
            TrySubscribeThemeManager();
            if (_followGlobalFormStyle) SyncWithGlobalThemeAndStyle();
            else ApplyTheme();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (!DesignMode && RibbonTabs.Count > 0 && _commandItems.Count == 0)
            {
                foreach (var tab in RibbonTabs)
                    if (tab.Visible) _commandItems.Add(tab.ToSimpleItem());
                BuildFromSimpleItems();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _commandItems.ListChanged -= CommandItems_ListChanged;
                _backstageItems.ListChanged -= BackstageItems_ListChanged;
                _backstageRecentItems.ListChanged -= BackstageItems_ListChanged;
                _backstagePinnedItems.ListChanged -= BackstageItems_ListChanged;
                _backstageFooterItems.ListChanged -= BackstageFooterItems_ListChanged;
                _backstageNavList.SelectedItemChanged -= BackstageNavList_SelectedItemChanged;
                _backstageActions.SizeChanged -= BackstageActions_SizeChanged;
                _backstageButton.DropDownOpening -= BackstageButton_DropDownOpening;
                _backstageDropDown.Closed -= BackstageDropDown_Closed;
                _tabStrip.MouseDoubleClick -= Tabs_MouseDoubleClick;
                _tabStrip.MouseUp -= Tabs_MouseUp;
                UnsubscribeThemeManager();
                _contextTransitionTimer.Stop(); _contextTransitionTimer.Tick -= ContextTransitionTimer_Tick; _contextTransitionTimer.Dispose();
                _backstageTransitionTimer.Stop(); _backstageTransitionTimer.Tick -= BackstageTransitionTimer_Tick; _backstageTransitionTimer.Dispose();
                _keyboardMap.Clear();
                HideKeyTips(); HideMinimizedPopup();
                _minimizedTabPopup.Closed -= MinimizedTabPopup_Closed; _minimizedTabPopup.Dispose();
                _superTooltip.Dispose();
                ClearBackstageActions(); ClearBackstageFooterActions();
                DisposeMinimizedImages(); DisposeGeneratedImages();
            }
            base.Dispose(disposing);
        }

        private void ShowActiveContentPanel()
        {
            foreach (var t in _tabStrip.Tabs)
                if (t.ContentPanel != null) t.ContentPanel.Visible = false;
            var sel = _tabStrip.SelectedTab;
            if (sel?.ContentPanel != null)
                sel.ContentPanel.Visible = true;
        }

        private void RefreshCommandView()
        {
            if (_suspendCommandRebuild) return;
            BuildFromSimpleItems();
            if (_isMinimized) ApplyMinimizedState();
        }

        private void EnsureSearchControls()
        {
            _searchBox.Visible = _searchMode != RibbonSearchMode.Off;
            _searchResultsButton.Visible = _searchMode != RibbonSearchMode.Off;
            ApplySearchAccessibility();
        }

        private void FocusRibbonPane(int direction)
        {
            int count = _tabStrip.Tabs.Count;
            if (count > 0)
            {
                int next = (_tabStrip.SelectedIndex + direction) % count;
                if (next < 0) next += count;
                _tabStrip.SelectedIndex = next;
                _tabStrip.Focus();
            }
        }

        private void ApplyPaneTabOrder()
        {
            _quickAccess.TabIndex = 0;
            _tabStrip.TabIndex = 1;
            _contextHeader.TabIndex = 2;
        }

        private void ContextHeader_Paint(object? sender, PaintEventArgs e)
        {
            if (_theme == null || _contextGroups.Count == 0) return;
            var g = e.Graphics;
            g.Clear(_theme.Background);

            for (int gi = 0; gi < _contextGroups.Count; gi++)
            {
                var grp = _contextGroups[gi];
                if (!grp.Visible || grp.Pages.Count == 0) continue;

                Rectangle? left = null, right = null;
                for (int i = 0; i < _tabStrip.Tabs.Count; i++)
                {
                    var tab = _tabStrip.Tabs[i];
                    if (!_pageToGroup.TryGetValue(tab, out var gref) || gref != grp) continue;
                    var r = _tabStrip.GetTabRect(i);
                    if (!left.HasValue || r.Left < left.Value.Left) left = r;
                    if (!right.HasValue || r.Right > right.Value.Right) right = r;
                }

                if (!left.HasValue || !right.HasValue) continue;
                var band = new Rectangle(left.Value.Left, 0, right.Value.Right - left.Value.Left, _contextHeader.Height - 1);
                int alpha = Math.Clamp((int)(120 * _contextTransitionProgress), 30, 180);
                using var b = new SolidBrush(Color.FromArgb(alpha, grp.Color));
                g.FillRectangle(b, band);
                using var p = new Pen(grp.Color);
                g.DrawLine(p, band.Left, band.Bottom, band.Right, band.Bottom);
            }
        }

        private void ContextTransitionTimer_Tick(object? sender, EventArgs e)
        {
            _contextTransitionProgress += _contextTransitionTimer.Interval / (float)Math.Max(1, _contextTransitionEffectiveDurationMs);
            if (_contextTransitionProgress >= 1f)
            {
                _contextTransitionProgress = 1f;
                _contextTransitionTimer.Stop();
            }
            _contextHeader.Invalidate();
        }

        private void StartContextTransition()
        {
            if (!ShouldAnimateTransitions() || !_enableContextTransitions)
            {
                _contextTransitionProgress = 1f;
                _contextHeader.Invalidate();
                return;
            }
            _contextTransitionEffectiveDurationMs = GetEffectiveTransitionDurationMs(_contextTransitionDurationMs, forBackstage: false);
            _contextTransitionTimer.Interval = Math.Clamp(_contextTransitionEffectiveDurationMs / 12, 10, 24);
            _contextTransitionProgress = 0f;
            _contextTransitionTimer.Stop();
            _contextTransitionTimer.Start();
        }

        private bool ShouldAnimateTransitions() => _enableContextTransitions && !_reducedMotion;
        private int GetEffectiveTransitionDurationMs(int configuredDurationMs, bool forBackstage)
        {
            int baseDuration = Math.Max(50, configuredDurationMs);
            if (!_adaptiveTransitionTiming) return baseDuration;
            float df = _density switch { RibbonDensity.Compact => 0.86f, RibbonDensity.Touch => 1.16f, _ => 1f };
            return Math.Max(50, (int)(baseDuration * df));
        }
    }
}
