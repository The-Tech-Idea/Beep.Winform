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
using TheTechIdea.Beep.Winform.Controls.DocumentHost.Tokens;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;
using TheTechIdea.Beep.Winform.Controls.Themes;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
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

        private readonly List<BeepDocumentGroup> _groups = new List<BeepDocumentGroup>();
        private BeepDocumentGroup _primaryGroup = null!;
        private BeepDocumentGroup _activeGroup = null!;
        private int _maxGroups = 4;
        private bool _splitHorizontal = true;
        private float _splitRatio = 0.5f;
        private BeepDocumentSplitterBar? _splitterBar;
        private readonly Dictionary<string, string> _docGroupMap
            = new Dictionary<string, string>(StringComparer.Ordinal);

        private bool _batchAdding;
        private bool _layoutSuspended;

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
            _tabStrip.AddButtonClicked += OnAddButtonClicked;
            _tabStrip.TabReordered += OnTabReordered;
            _tabStrip.TabSplitHorizontalRequested += (s, e) => SplitDocumentHorizontal(e.Tab.Id);
            _tabStrip.TabSplitVerticalRequested   += (s, e) => SplitDocumentVertical(e.Tab.Id);
            // 5.8 — Record a paint frame for FPS tracking whenever the tab strip repaints
            _tabStrip.Paint += (s, e) => _profiler?.RecordFrame();

            Controls.Add(_contentArea);
            Controls.Add(_tabStrip);

            _contentArea.Resize += (s, ea) => SyncPanelBounds();
            _contentArea.Paint += OnContentAreaPaint;

            _primaryGroup = new BeepDocumentGroup(
                System.Guid.NewGuid().ToString(), _tabStrip, _contentArea);
            _groups.Add(_primaryGroup);
            _activeGroup = _primaryGroup;
        }

        protected override bool IsContainerControl => true;
        protected override bool AllowBaseControlClear => true;

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

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
                panel.Theme = themeName;
            foreach (var grp in _groups)
                if (!grp.IsPrimary)
                    grp.ApplyTheme(themeName, _currentTheme);
            if (_splitterBar != null)
                _splitterBar.ApplyTheme(_currentTheme);
            foreach (var bar in _extraSplitters)
                bar.ApplyTheme(_currentTheme);
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
                var fmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString("No open documents", titleFont, titleBrush, titleRect, fmt);
                fmt.Dispose();
            }

            var subFont = BeepFontManager.GetCachedFont("Segoe UI", 9f, FontStyle.Regular);
            using (var subBrush = new SolidBrush(subColor))
            {
                var subRect = new RectangleF(
                    area.Left + DpiScalingHelper.ScaleValue(20, this),
                    iy + iconSize + DpiScalingHelper.ScaleValue(46, this),
                    area.Width - DpiScalingHelper.ScaleValue(40, this),
                    DpiScalingHelper.ScaleValue(22, this));
                var fmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString("Press Ctrl+N or click  +  to start", subFont, subBrush, subRect, fmt);
                fmt.Dispose();
            }
        }

        private void OnContentAreaPaint(object? sender, PaintEventArgs e)
        {
            if (!_showEmptyState || DocumentCount > 0) return;
            PaintEmptyState(e.Graphics);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                BeepThemesManager.ThemeChanged -= OnGlobalThemeChanged;
                _tabStrip?.Dispose();
                foreach (var fw in _floatWindows.Values) fw?.Dispose();
                foreach (var p in _panels.Values) p?.Dispose();
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
                BeepDocumentDragManager.Unregister(this);
            }
            base.Dispose(disposing);
        }
    }
}
