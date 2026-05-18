using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.DocumentHost.Layout;
using TheTechIdea.Beep.Winform.Controls.DocumentHost.Tokens;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;
using TheTechIdea.Beep.Winform.Controls.Themes;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    [ToolboxItem(true)]
    [DefaultEvent(nameof(ActiveDocumentChanged))]
    [DefaultProperty(nameof(DocumentPanels))]
    [Description("Commercial-style tabbed document host with design-time document surfaces, split groups, floating documents, and layout persistence.")]
    [Designer(
        "TheTechIdea.Beep.Winform.Controls.Design.Server.Designers.BeepDocumentHostDesigner, " +
        "TheTechIdea.Beep.Winform.Controls.Design.Server")]
    public partial class BeepDocumentHost : BaseControl
    {
        private readonly BeepDocumentTabStrip _tabStrip;
        private readonly Panel _contentArea;

        private readonly Dictionary<string, BeepDocumentPanel> _panels
            = new Dictionary<string, BeepDocumentPanel>(StringComparer.Ordinal);

        private readonly Dictionary<string, BeepDocumentFloatWindow> _floatWindows
            = new Dictionary<string, BeepDocumentFloatWindow>(StringComparer.Ordinal);

        private string? _activeDocumentId;
        private BeepDocumentDockOverlay? _dockOverlay;

        // ── Layout tree (nested splits) ──────────────────────────────────────
        // The layout tree is the source of truth for runtime layout.
        // _groups is kept as a flat index for backward compatibility and quick lookups.
        private ILayoutNode _layoutRoot = null!;
        private readonly List<BeepDocumentGroup> _groups = new List<BeepDocumentGroup>();
        private readonly Dictionary<string, BeepDocumentGroup> _groupById
            = new Dictionary<string, BeepDocumentGroup>(StringComparer.Ordinal);

        private BeepDocumentGroup _primaryGroup = null!;
        private BeepDocumentGroup _activeGroup = null!;
        private int _maxGroups = 4;
        private bool _splitHorizontal = true;
        private float _splitRatio = 0.5f;
        private BeepDocumentSplitterBar? _splitterBar;
        private readonly Dictionary<string, string> _docGroupMap
            = new Dictionary<string, string>(StringComparer.Ordinal);
        private readonly List<BeepDocumentSplitterBar> _extraSplitters = new();

        private bool _batchAdding;
        private bool _layoutSuspended;
        private bool _isDisposingHost;
        private bool _isDesignerDetaching;
        private bool _disposed;
        private bool _applyingDesignTimeDocuments;
        private int _designTimeDocumentUpdateDepth;
        private readonly HashSet<string> _dockBackClosingIds = new(StringComparer.Ordinal);

        private bool IsDesignTimeHost
            => LicenseManager.UsageMode == LicenseUsageMode.Designtime
               || (Site?.DesignMode ?? false);

        // 5.3 — Layout re-entrancy guard (prevents the ResumeLayout→OnLayout double-pass)
        private bool _inLayoutRecalc;

        // 5.6 — Batch-close state
        private bool _batchClosing;
        private readonly List<string> _pendingCloseIds = new();

        // 5.7 — Memory management
        private int _maxActivePanels;

        // 5.8 — Performance profiler (lazy)
        private BeepDocumentHostProfiler? _profiler;

        private System.Windows.Forms.Timer? _collapseAnimTimer;
        private float _collapseAnimFrom;
        private long _collapseAnimStartMs;
        private BeepDocumentGroup? _collapseAnimGroup;

        public BeepDocumentHost()
        {
            DoubleBuffered = true;
            _designTimeDocuments = new DesignTimeDocumentCollection(this);

            _tabStrip = new BeepDocumentTabStrip
            {
                Dock = DockStyle.None,
                ShowAddButton = _showAddButton,
                CloseMode = _closeMode
            };

            _contentArea = new Panel
            {
                Dock = DockStyle.None,
                BackColor = TheTechIdea.Beep.Winform.Controls.Helpers.ColorUtils.MapSystemColor(SystemColors.Window)
            };
            typeof(Panel).GetProperty("DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance)
                ?.SetValue(_contentArea, true);

            _tabStrip.TabSelected += OnTabSelected;
            _tabStrip.TabCloseRequested += OnTabCloseRequested;
            _tabStrip.TabClosing += OnTabClosing;
            _tabStrip.TabFloatRequested += OnTabFloatRequested;
            _tabStrip.TabPinToggled += OnTabPinToggled;
            _tabStrip.TabAutoHideRequested += OnTabAutoHideRequested;
            _tabStrip.AddButtonClicked += OnAddButtonClicked;
            _tabStrip.TabReordered += OnTabReordered;
            _tabStrip.TabSplitHorizontalRequested += (s, e) => SplitDocumentHorizontal(e.Tab.Id);
            _tabStrip.TabSplitVerticalRequested   += (s, e) => SplitDocumentVertical(e.Tab.Id);
            _tabStrip.GroupSplitHorizontalRequested += (s, e) => { if (_activeDocumentId != null) SplitDocumentHorizontal(_activeDocumentId); };
            _tabStrip.GroupSplitVerticalRequested   += (s, e) => { if (_activeDocumentId != null) SplitDocumentVertical(_activeDocumentId); };
            _tabStrip.GroupCloseAllRequested        += (s, e) => CloseGroupDocuments(_primaryGroup.GroupId);
            // 5.8 — Record a paint frame for FPS tracking whenever the tab strip repaints
            _tabStrip.Paint += (s, e) => _profiler?.RecordFrame();

            Controls.Add(_contentArea);
            Controls.Add(_tabStrip);

            _contentArea.Resize += (s, ea) => { if (!_isDisposingHost && !_isDesignerDetaching) SyncPanelBounds(); };
            _contentArea.Paint += OnContentAreaPaint;

            // Initialize layout tree with a single root group
            _primaryGroup = new BeepDocumentGroup(
                System.Guid.NewGuid().ToString(), _tabStrip, _contentArea);
            _groups.Add(_primaryGroup);
            _groupById[_primaryGroup.GroupId] = _primaryGroup;
            _activeGroup = _primaryGroup;

            _layoutRoot = new GroupLayoutNode(_primaryGroup.GroupId);
        }

        protected override bool IsContainerControl => true;
        protected override bool AllowBaseControlClear => true;

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (IsDesignTimeHost)
            {
                try
                {
                    _currentTheme = BeepThemesManager.GetTheme(BeepThemesManager.CurrentThemeName)
                        ?? BeepThemesManager.GetDefaultTheme();
                }
                catch
                {
                    _currentTheme ??= new DefaultBeepTheme();
                }

                ApplyThemeColors();
                ApplyDesignTimeSeedState();
                return;
            }

            try
            {
                _currentTheme = BeepThemesManager.GetTheme(BeepThemesManager.CurrentThemeName)
                    ?? BeepThemesManager.GetDefaultTheme();
            }
            catch { }

            _currentTheme ??= new DefaultBeepTheme();

            _tabStrip.ThemeName = BeepThemesManager.CurrentThemeName ?? string.Empty;

            BeepThemesManager.ThemeChanged -= OnGlobalThemeChanged;
            BeepThemesManager.ThemeChanged += OnGlobalThemeChanged;

            ApplyThemeColors();

            if (_autoSaveLayout && !string.IsNullOrEmpty(_sessionFile))
                RestoreLayoutFromFile();
            else
                ApplyDesignTimeDocuments();

            BeepDocumentDragManager.Register(this);
            InstallGlobalKeyFilter();
        }

        private void ApplyDesignTimeSeedState()
        {
            try
            {
                ApplyDesignTimeDocuments();

                if (!string.IsNullOrWhiteSpace(_designTimeLayoutJson))
                {
                    TryRestoreLayout(_designTimeLayoutJson, out _);
                }
            }
            catch
            {
                // Design-time restore should never break the VS designer surface.
            }
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);

            if (!IsDesignTimeHost)
            {
                return;
            }

            // During designer remove/reparent operations, parent may become null before dispose.
            if (Parent == null)
            {
                _isDesignerDetaching = true;
                _isDisposingHost = true;

                // Immediately detach hosted document panels from _contentArea so the designer
                // does not pick them up as orphaned controls and try to serialize or display
                // them on the parent form — which crashes the design-tools server.
                try { DetachHostedPanelsForDesignerRemoval(); } catch { }
                return;
            }

            _isDesignerDetaching = false;
            _isDisposingHost = false;
        }

        protected override void OnControlRemoved(ControlEventArgs e)
        {
            base.OnControlRemoved(e);

            if (!IsDesignTimeHost)
            {
                return;
            }

            // Suppress runtime mutation/layout flows only when the designer is tearing down
            // the host's own infrastructure. Normal child-control remove/reparent operations
            // are valid when the host is used as a design-time container.
            if (IsInternalDesignerInfrastructure(e.Control))
            {
                _isDesignerDetaching = true;
                _isDisposingHost = true;
            }
        }

        private bool IsInternalDesignerInfrastructure(Control? control)
        {
            if (control == null)
                return false;

            if (ReferenceEquals(control, _tabStrip)
                || ReferenceEquals(control, _contentArea)
                || ReferenceEquals(control, _splitterBar)
                || ReferenceEquals(control, _ahOverlay)
                || ReferenceEquals(control, _ahLeft)
                || ReferenceEquals(control, _ahRight)
                || ReferenceEquals(control, _ahTop)
                || ReferenceEquals(control, _ahBottom))
            {
                return true;
            }

            foreach (var group in _groups)
            {
                if (ReferenceEquals(control, group.TabStrip)
                    || ReferenceEquals(control, group.ContentArea))
                {
                    return true;
                }
            }

            foreach (var splitter in _extraSplitters)
            {
                if (ReferenceEquals(control, splitter))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Removes all hosted document panels from their parent panels so the designer does not
        /// try to re-parent or serialize them after the DocumentHost itself is removed from the form.
        /// Called only at design time when the host's parent becomes null (designer detach).
        /// </summary>
        private void DetachHostedPanelsForDesignerRemoval()
        {
            // Detach panels sitting in any group's ContentArea
            foreach (var panel in _panels.Values)
            {
                try
                {
                    if (panel != null && panel.Parent != null)
                        panel.Parent.Controls.Remove(panel);
                }
                catch { }
            }

            // Detach panels that were reparented to the host itself (e.g. auto-hide state)
            var hostedControls = Controls.OfType<BeepDocumentPanel>().ToList();
            foreach (var p in hostedControls)
            {
                try { Controls.Remove(p); } catch { }
            }
        }

        private void OnGlobalThemeChanged(object? sender, ThemeChangeEventArgs e)
        {
            ThemeName = e.NewThemeName;
        }

        private void ApplyThemeColors()
        {
            if (_currentTheme == null) return;
            BackColor = _currentTheme.PanelBackColor;
            ForeColor = _currentTheme.ForeColor;

            if (_contentArea != null)
            {
                _contentArea.BackColor = _currentTheme.BackgroundColor;

                _contentArea.BorderStyle = _controlStyle switch
                {
                    DocumentHostStyle.Thin => BorderStyle.FixedSingle,
                    DocumentHostStyle.Raised => BorderStyle.Fixed3D,
                    _ => BorderStyle.None
                };
            }
        }

        private void PropagateTheme(string themeName)
        {
            ApplyThemeColors();
            _tabStrip.ThemeName = themeName;
            foreach (var panel in _panels.Values)
            {
                try { panel.Theme = themeName; }
                catch { /* isolate per-panel theme errors from propagating to siblings */ }
            }
            foreach (var grp in _groups)
            {
                if (!grp.IsPrimary)
                {
                    try { grp.ApplyTheme(themeName, _currentTheme); }
                    catch { /* isolate per-group theme errors */ }
                }
            }
            if (_splitterBar != null)
                _splitterBar.ApplyTheme(_currentTheme);
            foreach (var bar in _extraSplitters)
            {
                try { bar.ApplyTheme(_currentTheme); }
                catch { /* isolate per-splitter theme errors */ }
            }
            if (_statusBar != null)
                ApplyStatusBarTheme(_statusBar);
            ApplyThemeToDockRails();
        }

        public override void ApplyTheme()
        {
            base.ApplyTheme();

            if (_currentTheme == null) return;

            ApplyThemeColors();
            _tabStrip.ApplyTheme();

            foreach (var panel in _panels.Values)
                panel.Theme = ThemeName;

            foreach (var grp in _groups)
                if (!grp.IsPrimary)
                    grp.ApplyTheme(ThemeName, _currentTheme);

            Invalidate();
        }

        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);

            if (_showEmptyState && DocumentCount == 0)
            {
                PaintEmptyState(g);
            }
        }

        private void PaintEmptyState(Graphics g)
        {
            var area = _contentArea.ClientRectangle;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            int scale = DpiScalingHelper.ScaleValue(1, this);
            int iconSize = DpiScalingHelper.ScaleValue(64, this);
            int foldSize = DpiScalingHelper.ScaleValue(14, this);

            Color iconColor = _currentTheme?.SecondaryTextColor ?? Color.FromArgb(120, 140, 160);
            Color textColor = _currentTheme?.ForeColor ?? Color.FromArgb(180, 185, 200);
            Color subColor = Color.FromArgb(140, textColor.R, textColor.G, textColor.B);

            int cx = area.Width / 2;
            int cy = area.Height / 2;
            int ix = cx - iconSize / 2;
            int iy = cy - iconSize / 2 - DpiScalingHelper.ScaleValue(30, this);

            using (var pen = new Pen(iconColor, 2f * scale))
            {
                var docBody = new PointF[]
                {
                    new(ix, iy + foldSize),
                    new(ix, iy + iconSize),
                    new(ix + iconSize, iy + iconSize),
                    new(ix + iconSize, iy),
                    new(ix + iconSize - foldSize, iy),
                    new(ix, iy + foldSize)
                };
                g.DrawLines(pen, docBody);

                g.DrawLine(pen, ix, iy + foldSize, ix + foldSize, iy + foldSize);
                g.DrawLine(pen, ix + foldSize, iy + foldSize, ix + foldSize, iy);

                float lx1 = ix + DpiScalingHelper.ScaleValue(10, this);
                float lx2 = ix + iconSize - DpiScalingHelper.ScaleValue(10, this);
                g.DrawLine(pen, lx1, iy + foldSize + DpiScalingHelper.ScaleValue(12, this), lx2, iy + foldSize + DpiScalingHelper.ScaleValue(12, this));
                g.DrawLine(pen, lx1, iy + foldSize + DpiScalingHelper.ScaleValue(22, this), lx2, iy + foldSize + DpiScalingHelper.ScaleValue(22, this));
                g.DrawLine(pen, lx1, iy + foldSize + DpiScalingHelper.ScaleValue(32, this), (lx1 + lx2) / 2, iy + foldSize + DpiScalingHelper.ScaleValue(32, this));
            }

            var titleFont = BeepFontManager.GetCachedFont("Segoe UI", 13f, FontStyle.Regular);
            using (var titleBrush = new SolidBrush(textColor))
            {
                var titleRect = new RectangleF(
                    area.Left + DpiScalingHelper.ScaleValue(20, this),
                    iy + iconSize + DpiScalingHelper.ScaleValue(16, this),
                    area.Width - DpiScalingHelper.ScaleValue(40, this),
                    DpiScalingHelper.ScaleValue(28, this));
                using var fmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString("No open documents", titleFont, titleBrush, titleRect, fmt);
            }

            var subFont = BeepFontManager.GetCachedFont("Segoe UI", 9f, FontStyle.Regular);
            using (var subBrush = new SolidBrush(subColor))
            {
                var subRect = new RectangleF(
                    area.Left + DpiScalingHelper.ScaleValue(20, this),
                    iy + iconSize + DpiScalingHelper.ScaleValue(46, this),
                    area.Width - DpiScalingHelper.ScaleValue(40, this),
                    DpiScalingHelper.ScaleValue(22, this));
                using var fmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString("Press Ctrl+N or click  +  to start", subFont, subBrush, subRect, fmt);
            }
        }

        private void OnContentAreaPaint(object? sender, PaintEventArgs e)
        {
            if (!_showEmptyState || DocumentCount > 0) return;
            PaintEmptyState(e.Graphics);
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed) return;
            _disposed = true;

            if (disposing)
            {
                _isDisposingHost = true;

                if (IsDesignTimeHost)
                {
                    // ── Design-time teardown: stop every timer FIRST so no ticks fire
                    // after we start releasing sub-controls.
                    try { _chordTimer.Stop(); } catch { }
                    try { _collapseAnimTimer?.Stop(); _collapseAnimTimer?.Dispose(); _collapseAnimTimer = null; } catch { }

                    // DisposeAutoHide handles _ahSlideTimer + strips.
                    try { DisposeAutoHide(); } catch { }

                    // Extra focus / hover timers in the AutoHide partial.
                    try { _ahFocusTimer?.Stop(); _ahFocusTimer?.Dispose(); _ahFocusTimer = null; } catch { }

                    // Snapshot-dispose any panels created by smart-tag "Add Document".
                    var dtPanels = _panels.Values.ToList();
                    _panels.Clear();
                    foreach (var p in dtPanels) { try { p?.Dispose(); } catch { } }

                    // Snapshot-dispose float windows (may have been opened at design time).
                    var dtFloat = _floatWindows.Values.ToList();
                    _floatWindows.Clear();
                    foreach (var fw in dtFloat) { try { fw?.Dispose(); } catch { } }

                    // Non-primary groups may own Controls — dispose them.
                    foreach (var grp in _groups)
                        if (!grp.IsPrimary) { try { grp.Dispose(); } catch { } }

                    try { _tabStrip?.Dispose(); } catch { }
                    try { _dockOverlay?.Dispose(); } catch { }
                    try { _splitterBar?.Dispose(); } catch { }
                    foreach (var bar in _extraSplitters.ToList())
                    {
                        try { bar?.Dispose(); } catch { }
                    }
                    try { BeepDocumentDragManager.Unregister(this); } catch { }
                    base.Dispose(disposing);
                    return;
                }

                BeepThemesManager.ThemeChanged -= OnGlobalThemeChanged;
                _tabStrip?.Dispose();

                // Use snapshots because disposing float windows can trigger FormClosed handlers
                // that mutate these dictionaries.
                var floatWindows = _floatWindows.Values.ToList();
                _floatWindows.Clear();
                foreach (var fw in floatWindows) fw?.Dispose();

                var panels = _panels.Values.ToList();
                _panels.Clear();
                foreach (var p in panels) p?.Dispose();

                if (_autoSaveLayout && !string.IsNullOrEmpty(_sessionFile))
                    SaveLayoutToFile();

                _dockOverlay?.Dispose();
                _splitterBar?.Dispose();
                foreach (var bar in _extraSplitters) bar?.Dispose();
                foreach (var grp in _groups)
                    if (!grp.IsPrimary) grp.Dispose();
                DisposeMvvm();
                DisposePreviewCache();
                DisposeAutoHide();
                DisposeDockRails();
                RemoveGlobalKeyFilter();
                BeepDocumentDragManager.Unregister(this);
            }
            base.Dispose(disposing);
        }
    }
}
