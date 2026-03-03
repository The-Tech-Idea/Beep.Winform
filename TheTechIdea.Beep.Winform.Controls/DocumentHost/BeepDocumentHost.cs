using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.DocumentHost.Tokens;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;
using TheTechIdea.Beep.Winform.Controls.Themes;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    // ─────────────────────────────────────────────────────────────────────────
    // BeepDocumentHost
    // Orchestrates a tab strip + multiple content panels in a simulated MDI
    // ("Scenario C") layout.  Does NOT use BaseControl to avoid ContainerControl
    // focus management and WM_ERASEBKGND conflicts with BeepiFormPro.
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Top-level simulated document host.  Drop this Panel-derived control onto
    /// a <see cref="BeepiFormPro"/> (or any container) to get a full tabbed-document
    /// interface.
    /// </summary>
    [ToolboxItem(true)]
    [Description("Beep tabbed document host — simulated MDI Scenario C.")]
    public partial class BeepDocumentHost : Panel
    {
        // ── Fields ────────────────────────────────────────────────────────────

        private IBeepTheme _theme;
        private string _themeName = string.Empty;

        private readonly BeepDocumentTabStrip _tabStrip;
        private readonly Panel _contentArea;

        // Document registry: documentId → panel
        private readonly Dictionary<string, BeepDocumentPanel> _panels
            = new Dictionary<string, BeepDocumentPanel>(StringComparer.Ordinal);

        // Floated documents: documentId → float window
        private readonly Dictionary<string, BeepDocumentFloatWindow> _floatWindows
            = new Dictionary<string, BeepDocumentFloatWindow>(StringComparer.Ordinal);

        private string? _activeDocumentId;

        // Dock overlay (lazily created, shared across all float windows)
        private BeepDocumentDockOverlay? _dockOverlay;

        // ── Document Groups — split view (feature 2.1) ─────────────────────
        // _primaryGroup wraps the original _tabStrip + _contentArea and is always _groups[0].
        // Secondary groups each own their own strip + content panel.
        private readonly List<BeepDocumentGroup> _groups  = new List<BeepDocumentGroup>();
        private BeepDocumentGroup                _primaryGroup = null!; // set in ctor
        private BeepDocumentGroup                _activeGroup  = null!; // set in ctor
        private int  _maxGroups        = 4;
        private bool _splitHorizontal  = true;   // true = side-by-side, false = top+bottom
        private float _splitRatio      = 0.5f;   // fraction of host width/height for group[0]
        private Panel? _splitterBar;             // thin control drawn between groups
        // Map: documentId → groupId  (for fast lookup)
        private readonly Dictionary<string, string> _docGroupMap
            = new Dictionary<string, string>(StringComparer.Ordinal);

        // ── Performance: batch-add mode ───────────────────────────────────────
        // While true AddDocument skips RecalculateLayout; one flush happens in EndBatchAddDocuments.
        private bool _batchAdding;
        // While true RecalculateLayout is a no-op until ResumeHostLayout is called.
        private bool _layoutSuspended;

        // ── Group-collapse animation (Sprint 15.2) ────────────────────────────
        private System.Windows.Forms.Timer? _collapseAnimTimer;
        private float _collapseAnimFrom;
        private long  _collapseAnimStartMs;
        private BeepDocumentGroup? _collapseAnimGroup;

        // (Properties and events → BeepDocumentHost.Properties.cs)

        public BeepDocumentHost()
        {
            // Double buffering — safe pre-handle
            typeof(Panel).GetProperty("DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance)
                ?.SetValue(this, true);

            // Build child controls with safe pre-handle defaults
            _tabStrip = new BeepDocumentTabStrip
            {
                Dock          = DockStyle.None,
                ShowAddButton = _showAddButton,
                CloseMode     = _closeMode
            };

            _contentArea = new Panel
            {
                Dock      = DockStyle.None,
                BackColor = System.Drawing.SystemColors.Window
            };
            typeof(Panel).GetProperty("DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance)
                ?.SetValue(_contentArea, true);

            // Wire tab strip events
            _tabStrip.TabSelected           += OnTabSelected;
            _tabStrip.TabCloseRequested     += OnTabCloseRequested;
            _tabStrip.TabClosing            += OnTabClosing;
            _tabStrip.TabFloatRequested     += OnTabFloatRequested;
            _tabStrip.TabPinToggled         += OnTabPinToggled;
            _tabStrip.AddButtonClicked      += OnAddButtonClicked;
            _tabStrip.TabReordered          += OnTabReordered;

            Controls.Add(_contentArea);
            Controls.Add(_tabStrip);

            _contentArea.Resize += (s, ea) => SyncPanelBounds();
            _contentArea.Paint  += OnContentAreaPaint;

            // Primary group wraps the already-created strip + content
            _primaryGroup = new BeepDocumentGroup(
                System.Guid.NewGuid().ToString(), _tabStrip, _contentArea);
            _groups.Add(_primaryGroup);
            _activeGroup = _primaryGroup;
        }

        // ── Session Restore + deferred theme init (like BeepButton/BaseControl) ──

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            // Load theme now that we have a real window handle
            try
            {
                _theme     = BeepThemesManager.GetTheme(BeepThemesManager.CurrentThemeName)
                             ?? BeepThemesManager.GetDefaultTheme();
                _themeName = BeepThemesManager.CurrentThemeName ?? string.Empty;
            }
            catch { /* stay null */ }

            _theme ??= new DefaultBeepTheme();
            _themeName ??= string.Empty;

            // Propagate theme to child strip (safe — handle exists now)
            _tabStrip.ThemeName = _themeName;

            // Subscribe once
            BeepThemesManager.ThemeChanged -= OnGlobalThemeChanged;
            BeepThemesManager.ThemeChanged += OnGlobalThemeChanged;

            ApplyThemeColors();

            // Session restore
            if (_autoSaveLayout && !string.IsNullOrEmpty(_sessionFile))
                RestoreLayoutFromFile();
            else
                ApplyDesignTimeDocuments();   // seed from design-time collection if no session file

            // Cross-host drag registration (runtime only)
            BeepDocumentDragManager.Register(this);
        }



        // ── Theme wiring ──────────────────────────────────────────────────────

        private void OnGlobalThemeChanged(object? sender, ThemeChangeEventArgs e)
        {
            ThemeName = e.NewThemeName;
        }

        private void ApplyThemeColors()
        {
            if (_theme == null) return;
            BackColor = _theme.PanelBackColor;
            ForeColor = _theme.ForeColor;

            if (_contentArea != null)
            {
                _contentArea.BackColor = _theme.BackgroundColor;

                // Apply border based on ControlStyle property
                _contentArea.BorderStyle = _controlStyle switch
                {
                    DocumentHostStyle.Thin   => System.Windows.Forms.BorderStyle.FixedSingle,
                    DocumentHostStyle.Raised => System.Windows.Forms.BorderStyle.Fixed3D,
                    _                        => System.Windows.Forms.BorderStyle.None
                };
            }
        }

        private void PropagateTheme(string themeName)
        {
            ApplyThemeColors();
            _tabStrip.ThemeName = themeName;
            foreach (var panel in _panels.Values)
                panel.ThemeName = themeName;
            // Cascade to secondary groups
            foreach (var grp in _groups)
                if (!grp.IsPrimary)
                    grp.ApplyTheme(themeName, _theme);
            // Splitter bar colour
            if (_splitterBar != null)
                _splitterBar.BackColor = _theme?.BorderColor
                    ?? System.Drawing.SystemColors.ControlDark;
        }

        // (Layout → BeepDocumentHost.Layout.cs)

        // ─────────────────────────────────────────────────────────────────────
        // Empty-state painting (Sprint 15.4)
        // ─────────────────────────────────────────────────────────────────────

        private void OnContentAreaPaint(object? sender, System.Windows.Forms.PaintEventArgs e)
        {
            if (!_showEmptyState || DocumentCount > 0) return;

            var g    = e.Graphics;
            var area = _contentArea.ClientRectangle;
            g.SmoothingMode     = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            Color iconColor = _theme?.SecondaryTextColor
                              ?? Color.FromArgb(120, 140, 160);
            Color textColor = _theme?.ForeColor
                              ?? Color.FromArgb(180, 185, 200);
            Color subColor  = Color.FromArgb(140, textColor.R, textColor.G, textColor.B);

            // Centre point
            int cx = area.Width  / 2;
            int cy = area.Height / 2;

            // 64 × 64 document outline icon
            const int IconSize = 64;
            const int FoldSize = 14;
            int ix = cx - IconSize / 2;
            int iy = cy - IconSize / 2 - 30;

            using (var pen = new System.Drawing.Pen(iconColor, 2f))
            {
                // Document body
                var docBody = new System.Drawing.PointF[]
                {
                    new(ix,                    iy + FoldSize),
                    new(ix,                    iy + IconSize),
                    new(ix + IconSize,         iy + IconSize),
                    new(ix + IconSize,         iy),
                    new(ix + IconSize - FoldSize, iy),
                    new(ix,                    iy + FoldSize)
                };
                g.DrawLines(pen, docBody);

                // Fold crease
                g.DrawLine(pen,
                    ix, iy + FoldSize,
                    ix + FoldSize, iy + FoldSize);
                g.DrawLine(pen,
                    ix + FoldSize, iy + FoldSize,
                    ix + FoldSize, iy);

                // Horizontal rules (lines on document)
                float lx1 = ix + 10, lx2 = ix + IconSize - 10;
                g.DrawLine(pen, lx1, iy + FoldSize + 12, lx2, iy + FoldSize + 12);
                g.DrawLine(pen, lx1, iy + FoldSize + 22, lx2, iy + FoldSize + 22);
                g.DrawLine(pen, lx1, iy + FoldSize + 32, (lx1 + lx2) / 2, iy + FoldSize + 32);
            }

            // Primary text
            using (var titleFont = new Font(Font.FontFamily, 13f, FontStyle.Regular, GraphicsUnit.Point))
            using (var titleBrush = new SolidBrush(textColor))
            {
                var titleRect = new RectangleF(area.Left + 20, iy + IconSize + 16,
                                               area.Width - 40, 28);
                var fmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString("No open documents", titleFont, titleBrush, titleRect, fmt);
                fmt.Dispose();
            }

            // Sub text
            using (var subFont  = new Font(Font.FontFamily, 9f, FontStyle.Regular, GraphicsUnit.Point))
            using (var subBrush = new SolidBrush(subColor))
            {
                var subRect = new RectangleF(area.Left + 20, iy + IconSize + 46,
                                             area.Width - 40, 22);
                var fmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString("Press Ctrl+N or click  +  to start", subFont, subBrush, subRect, fmt);
                fmt.Dispose();
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // (Document management and tab event handlers → BeepDocumentHost.Documents.cs / Events.cs)

        // ── Dispose ───────────────────────────────────────────────────────────

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                BeepThemesManager.ThemeChanged -= OnGlobalThemeChanged;
                _tabStrip?.Dispose();
                foreach (var fw in _floatWindows.Values) fw?.Dispose();
                foreach (var p in _panels.Values) p?.Dispose();
                // Auto-save layout if configured (5.2)
                if (_autoSaveLayout && !string.IsNullOrEmpty(_sessionFile))
                    SaveLayoutToFile();

                _dockOverlay?.Dispose();
                _splitterBar?.Dispose();
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

    // (BeepDocumentFloatWindow → BeepDocumentHost.Events.cs)
}
