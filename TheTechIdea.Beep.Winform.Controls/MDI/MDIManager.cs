using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.MDI
{
    [ToolboxItem(true)]
    [DisplayName("Beep MDI Manager")]
    [Description("Modern themed MDI tabbed document manager with extended features.")]
    [Category("Beep Controls")]
    public class MDIManager : BeepControl
    {
        #region Fields
        private readonly List<MDIDocument> _documents = new();
        private readonly Dictionary<string, MDIDocument> _documentsById = new();
        private MDIDocument _activeDocument;
        private MDIDocument _hoveredDocument;
        private MDIDocument _dragDocument;
        private Point _dragStartPoint;
        private bool _isDragging;
        private int _dragOriginalIndex;

        private System.Windows.Forms.Timer _animationTimer;
        private MDIAnimationHelper _animationHelper;
        private MDILayoutHelper _layoutHelper;
        private MDIPaintHelper _paintHelper;

        // Layout
        private Rectangle _headerRect, _contentRect;
        private Rectangle _scrollLeftRect, _scrollRightRect, _newDocRect;
        private int _scrollOffset = 0;
        private bool _needsScrolling = false;

        // Settings
        private int _tabHeaderHeight = 34;
        private int _tabMinWidth = 80;
        private int _tabMaxWidth = 220;
        private bool _showCloseButtons = true;
        private bool _enableAnimations = true;
        private bool _allowTabReorder = true;
        private bool _middleClickClose = true;
        private bool _allowPinning = true;

        // Theme colors
        private Color _inactiveTabBack, _inactiveTabFore, _activeTabBack, _activeTabFore,
            _hoverTabBack, _borderColor, _headerBackColor;

        private readonly ContextMenuStrip _tabContextMenu;
        private MDIDocument _contextDoc;
        #endregion

        #region Properties
        [Category("Appearance"), DefaultValue(34)] public int TabHeaderHeight { get => _tabHeaderHeight; set { if (value < 24) value = 24; if (_tabHeaderHeight != value) { _tabHeaderHeight = value; RecalculateLayout(); Invalidate(); } } }
        [Category("Behavior"), DefaultValue(true)] public bool ShowCloseButtons { get => _showCloseButtons; set { _showCloseButtons = value; Invalidate(); } }
        [Category("Behavior"), DefaultValue(true)] public bool EnableAnimations { get => _enableAnimations; set { _enableAnimations = value; if (!value) { foreach (var d in _documents) d.AnimationProgress = d.TargetAnimationProgress = 0; Invalidate(); } } }
        [Category("Behavior"), DefaultValue(true)] public bool AllowTabReorder { get => _allowTabReorder; set => _allowTabReorder = value; }
        [Category("Behavior"), DefaultValue(true)] public bool MiddleClickClose { get => _middleClickClose; set => _middleClickClose = value; }
        [Category("Behavior"), DefaultValue(true)] public bool AllowPinning { get => _allowPinning; set { _allowPinning = value; Invalidate(); } }
        [Category("Layout"), DefaultValue(80)] public int TabMinWidth { get => _tabMinWidth; set { _tabMinWidth = Math.Max(40, value); RecalculateLayout(); Invalidate(); } }
        [Category("Layout"), DefaultValue(220)] public int TabMaxWidth { get => _tabMaxWidth; set { _tabMaxWidth = Math.Max(_tabMinWidth, value); RecalculateLayout(); Invalidate(); } }
        [Browsable(false)] public IReadOnlyList<MDIDocument> Documents => _documents;
        [Browsable(false)] public MDIDocument ActiveDocument => _activeDocument;
        #endregion

        #region Events
        public event EventHandler<MDIDocumentEventArgs> DocumentAdded;
        public event EventHandler<MDIDocumentEventArgs> DocumentRemoved;
        public event EventHandler<MDIDocumentEventArgs> ActiveDocumentChanging;
        public event EventHandler<MDIDocumentEventArgs> ActiveDocumentChanged;
        public event EventHandler<MDIDocumentReorderEventArgs> DocumentReordered;
        public event EventHandler<MDIDocumentEventArgs> DocumentPinnedChanged;
        public event EventHandler<MDIDocumentEventArgs> DocumentDirtyStateChanged;
        #endregion

        #region Ctor
        public MDIManager()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
            UpdateStyles();

            _layoutHelper = new MDILayoutHelper();
            _paintHelper = new MDIPaintHelper(_currentTheme);
            _animationHelper = new MDIAnimationHelper(Invalidate);
            _animationTimer = new System.Windows.Forms.Timer { Interval = 16 };
            _animationTimer.Tick += (s, e) => _animationHelper.Tick(_documents, _enableAnimations);
            _animationTimer.Start();

            _tabContextMenu = BuildContextMenu();
            ApplyTheme();
            RecalculateLayout();
        }
        #endregion

        #region Context Menu
        private ContextMenuStrip BuildContextMenu()
        {
            var cms = new ContextMenuStrip();
            cms.Items.Add("Activate", null, (s, e) => { if (_contextDoc != null) ActivateInternal(_contextDoc); });
            cms.Items.Add("Close", null, (s, e) => { if (_contextDoc != null) CloseInternal(_contextDoc); });
            cms.Items.Add("Close Others", null, (s, e) => CloseOthers());
            cms.Items.Add("Close All", null, (s, e) => CloseAll());
            cms.Items.Add(new ToolStripSeparator());
            cms.Items.Add("Pin/Unpin", null, (s, e) => TogglePin(_contextDoc));
            cms.Opening += (s, e) => { if (_contextDoc == null) e.Cancel = true; };
            return cms;
        }
        #endregion

        #region Theme
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            _paintHelper = new MDIPaintHelper(_currentTheme);
            if (_currentTheme != null)
            {
                _inactiveTabBack = _currentTheme.PanelBackColor;
                _inactiveTabFore = _currentTheme.ForeColor;
                _activeTabBack = _currentTheme.ButtonBackColor;
                _activeTabFore = _currentTheme.ButtonForeColor;
                _hoverTabBack = ControlPaint.Light(_currentTheme.ButtonBackColor, 0.3f);
                _borderColor = _currentTheme.BorderColor;
                _headerBackColor = _currentTheme.PanelBackColor;
                BackColor = _currentTheme.BackColor;
            }
            else
            {
                _inactiveTabBack = Color.FromArgb(245, 245, 245);
                _inactiveTabFore = Color.FromArgb(60, 60, 60);
                _activeTabBack = Color.White;
                _activeTabFore = Color.Black;
                _hoverTabBack = Color.FromArgb(230, 230, 230);
                _borderColor = Color.FromArgb(200, 200, 200);
                _headerBackColor = Color.FromArgb(240, 240, 240);
            }
            Invalidate();
        }
        #endregion

        #region Public API
        public MDIDocument AddDocument(Control control, string title, bool canClose = true, Image icon = null, bool activate = true)
        {
            if (control == null) throw new ArgumentNullException(nameof(control));
            var doc = new MDIDocument
            {
                Id = Guid.NewGuid().ToString(),
                Title = title ?? control.Name ?? "Document",
                Content = control,
                CanClose = canClose,
                Icon = icon,
                IsVisible = true
            };
            _documents.Add(doc);
            _documentsById[doc.Id] = doc;
            Controls.Add(control);
            control.Visible = false;
            RecalculateLayout();
            OnDocumentAdded(new MDIDocumentEventArgs(doc));
            if (activate)
                ActivateDocument(doc.Id);
            return doc;
        }

        public bool CloseDocument(string id) => id != null && _documentsById.TryGetValue(id, out var d) && CloseInternal(d);
        public bool CloseDocument(MDIDocument doc) => CloseInternal(doc);
        public bool CloseDocument(Control c) => _documents.FirstOrDefault(d => d.Content == c) is { } d && CloseInternal(d);
        public void CloseOthers()
        {
            var keep = _activeDocument;
            foreach (var d in _documents.ToList()) if (d != keep) CloseInternal(d);
        }
        public void CloseAll() { foreach (var d in _documents.ToList()) CloseInternal(d); }
        public bool ActivateDocument(string id) => id != null && _documentsById.TryGetValue(id, out var d) && ActivateInternal(d);
        public bool ActivateDocument(MDIDocument doc) => ActivateInternal(doc);
        public void SetDirty(MDIDocument doc, bool dirty)
        {
            if (doc != null && doc.IsDirty != dirty)
            {
                doc.IsDirty = dirty; OnDocumentDirtyStateChanged(new MDIDocumentEventArgs(doc)); Invalidate();
            }
        }
        public void TogglePin(MDIDocument doc)
        {
            if (!_allowPinning || doc == null) return;
            doc.IsPinned = !doc.IsPinned;
            OnDocumentPinnedChanged(new MDIDocumentEventArgs(doc));
            SortPinnedFirst();
            RecalculateLayout();
            Invalidate();
        }
        #endregion

        #region Internal Logic
        private void SortPinnedFirst()
        {
            var ordered = _documents.OrderByDescending(d => d.IsPinned).ThenBy(d => d.Title).ToList();
            _documents.Clear();
            _documents.AddRange(ordered);
        }

        private bool CloseInternal(MDIDocument doc)
        {
            if (doc == null || !_documents.Contains(doc) || !doc.CanClose) return false;
            bool wasActive = doc == _activeDocument;
            _documents.Remove(doc);
            _documentsById.Remove(doc.Id);
            if (doc.Content != null) { Controls.Remove(doc.Content); doc.Content.Dispose(); }
            OnDocumentRemoved(new MDIDocumentEventArgs(doc));
            if (wasActive)
            {
                _activeDocument = _documents.FirstOrDefault(d => d.IsPinned) ?? _documents.FirstOrDefault();
                if (_activeDocument != null)
                    ActivateInternal(_activeDocument, raiseChanging: false);
            }
            RecalculateLayout();
            Invalidate();
            return true;
        }

        private bool ActivateInternal(MDIDocument doc, bool raiseChanging = true)
        {
            if (doc == null || doc == _activeDocument) return false;
            var args = new MDIDocumentEventArgs(doc);
            if (raiseChanging)
            {
                OnActiveDocumentChanging(args);
                if (args.Cancel) return false;
            }
            if (_activeDocument?.Content != null) _activeDocument.Content.Visible = false;
            _activeDocument = doc;
            if (doc.Content != null)
            {
                doc.Content.Bounds = _contentRect;
                doc.Content.Visible = true;
                doc.Content.BringToFront();
            }
            OnActiveDocumentChanged(new MDIDocumentEventArgs(doc));
            Invalidate();
            return true;
        }
        #endregion

        #region Layout
        private void RecalculateLayout()
        {
            if (Width <= 0 || Height <= 0) return;
            _headerRect = new Rectangle(0, 0, Width, _tabHeaderHeight);
            _contentRect = new Rectangle(0, _tabHeaderHeight, Width, Height - _tabHeaderHeight);
            if (_activeDocument?.Content != null) _activeDocument.Content.Bounds = _contentRect;
            CalculateTabLayout();
        }
        private void CalculateTabLayout()
        {
            if (_documents.Count == 0) { _needsScrolling = false; return; }
            var result = _layoutHelper.CalculateLayout(_documents, _headerRect, _tabMinWidth, _tabMaxWidth, _scrollOffset, Font);
            _needsScrolling = result.NeedsScrolling;
            _scrollLeftRect = result.ScrollLeftRect; _scrollRightRect = result.ScrollRightRect; _newDocRect = result.NewDocumentRect;
        }
        #endregion

        #region Painting
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);
            DrawHeader(g);
            DrawTabs(g);
            DrawUtilityButtons(g);
            DrawDragIndicator(g);
        }

        private void DrawHeader(Graphics g)
        {
            using var b = new SolidBrush(_headerBackColor);
            g.FillRectangle(b, _headerRect);
            using var pen = new Pen(_borderColor);
            g.DrawLine(pen, _headerRect.Left, _headerRect.Bottom - 1, _headerRect.Right, _headerRect.Bottom - 1);
        }

        private void DrawTabs(Graphics g)
        {
            foreach (var doc in _documents.Where(d => d.IsVisible))
            {
                bool isActive = doc == _activeDocument;
                bool isHovered = doc == _hoveredDocument;
                _paintHelper.DrawTab(g, doc, doc.TabBounds, doc.Title, Font, isActive, isHovered, _showCloseButtons && doc.CanClose, doc.IsCloseHovered, doc.AnimationProgress,
                    _inactiveTabBack, _inactiveTabFore, _activeTabBack, _activeTabFore, _hoverTabBack, _borderColor);
            }
        }

        private void DrawUtilityButtons(Graphics g)
        {
            if (!_needsScrolling) return;
            _paintHelper.DrawScrollButton(g, _scrollLeftRect, ScrollButtonType.Left, _inactiveTabBack, _inactiveTabFore, _borderColor);
            _paintHelper.DrawScrollButton(g, _scrollRightRect, ScrollButtonType.Right, _inactiveTabBack, _inactiveTabFore, _borderColor);
            _paintHelper.DrawNewDocumentButton(g, _newDocRect, _inactiveTabBack, _inactiveTabFore, _borderColor);
        }

        private void DrawDragIndicator(Graphics g)
        {
            if (_isDragging && _dragDocument != null)
            {
                using var pen = new Pen(Color.FromArgb(120, _activeTabFore), 2) { DashStyle = DashStyle.Dash };
                g.DrawRectangle(pen, _dragDocument.TabBounds);
            }
        }
        #endregion

        #region Mouse & Interaction
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            var hit = _documents.FirstOrDefault(d => d.TabBounds.Contains(e.Location));
            if (hit != _hoveredDocument)
            {
                if (_hoveredDocument != null) { _hoveredDocument.IsCloseHovered = false; StartAnimation(_hoveredDocument, 0f); }
                _hoveredDocument = hit; if (_hoveredDocument != null) StartAnimation(_hoveredDocument, 1f); Invalidate();
            }
            if (_hoveredDocument != null && _showCloseButtons && _hoveredDocument.CanClose)
            {
                var closeRect = _layoutHelper.GetCloseButtonRect(_hoveredDocument.TabBounds);
                bool ch = closeRect.Contains(e.Location);
                if (ch != _hoveredDocument.IsCloseHovered) { _hoveredDocument.IsCloseHovered = ch; Cursor = ch ? Cursors.Hand : Cursors.Default; Invalidate(); }
            }
            else Cursor = Cursors.Default;

            if (_isDragging && _dragDocument != null && _allowTabReorder)
            {
                var currentIndex = _documents.IndexOf(_dragDocument);
                var target = _documents.FirstOrDefault(d => d != _dragDocument && d.TabBounds.Contains(e.Location));
                if (target != null)
                {
                    int targetIndex = _documents.IndexOf(target);
                    if (targetIndex != currentIndex)
                    {
                        _documents.Remove(_dragDocument);
                        _documents.Insert(targetIndex, _dragDocument);
                        RecalculateLayout();
                        Invalidate();
                    }
                }
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (_hoveredDocument != null) { _hoveredDocument.IsCloseHovered = false; StartAnimation(_hoveredDocument, 0f); _hoveredDocument = null; Invalidate(); }
            Cursor = Cursors.Default;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                var hit = _documents.FirstOrDefault(d => d.TabBounds.Contains(e.Location));
                if (hit != null)
                {
                    var closeRect = _layoutHelper.GetCloseButtonRect(hit.TabBounds);
                    if (closeRect.Contains(e.Location) && _showCloseButtons && hit.CanClose)
                    {
                        CloseInternal(hit); return;
                    }
                    // start drag possible
                    _dragDocument = hit; _dragStartPoint = e.Location; _dragOriginalIndex = _documents.IndexOf(hit); _isDragging = true;
                }
            }
            else if (e.Button == MouseButtons.Middle && _middleClickClose)
            {
                var hit = _documents.FirstOrDefault(d => d.TabBounds.Contains(e.Location));
                if (hit != null && hit.CanClose) CloseInternal(hit);
            }
            else if (e.Button == MouseButtons.Right)
            {
                _contextDoc = _documents.FirstOrDefault(d => d.TabBounds.Contains(e.Location));
                if (_contextDoc != null) _tabContextMenu.Show(this, e.Location);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (_isDragging && _dragDocument != null)
            {
                int newIndex = _documents.IndexOf(_dragDocument);
                if (newIndex != _dragOriginalIndex)
                    OnDocumentReordered(new MDIDocumentReorderEventArgs(_dragDocument, _dragOriginalIndex, newIndex));
            }
            _isDragging = false; _dragDocument = null; Invalidate();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (e.Button != MouseButtons.Left) return;

            if (_needsScrolling)
            {
                if (_scrollLeftRect.Contains(e.Location)) { ScrollTabs(-1); return; }
                if (_scrollRightRect.Contains(e.Location)) { ScrollTabs(1); return; }
                if (_newDocRect.Contains(e.Location)) { OnNewDocumentRequested(); return; }
            }
            var hit = _documents.FirstOrDefault(d => d.TabBounds.Contains(e.Location));
            if (hit != null) ActivateInternal(hit);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            if (_allowPinning)
            {
                var hit = _documents.FirstOrDefault(d => d.TabBounds.Contains(e.Location));
                if (hit != null) TogglePin(hit);
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (_needsScrolling)
            {
                ScrollTabs(e.Delta > 0 ? -1 : 1);
            }
        }
        #endregion

        #region Keyboard
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.F4) && _activeDocument != null && _activeDocument.CanClose)
            {
                CloseInternal(_activeDocument); return true;
            }
            if (keyData == (Keys.Control | Keys.Tab)) { CycleDocument(1); return true; }
            if (keyData == (Keys.Control | Keys.Shift | Keys.Tab)) { CycleDocument(-1); return true; }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void CycleDocument(int dir)
        {
            if (_documents.Count == 0) return;
            int idx = _documents.IndexOf(_activeDocument);
            if (idx < 0) idx = 0;
            idx = (idx + dir + _documents.Count) % _documents.Count;
            ActivateInternal(_documents[idx]);
        }
        #endregion

        #region Utility
        private void ScrollTabs(int delta)
        {
            _scrollOffset += delta;
            if (_scrollOffset < 0) _scrollOffset = 0;
            if (_scrollOffset > _documents.Count - 1) _scrollOffset = _documents.Count - 1;
            CalculateTabLayout(); Invalidate();
        }
        protected virtual void OnNewDocumentRequested() { }
        private void StartAnimation(MDIDocument doc, float target) { if (_enableAnimations) _animationHelper.Start(doc, target); }
        #endregion

        #region Event Raisers
        protected virtual void OnDocumentAdded(MDIDocumentEventArgs e) => DocumentAdded?.Invoke(this, e);
        protected virtual void OnDocumentRemoved(MDIDocumentEventArgs e) => DocumentRemoved?.Invoke(this, e);
        protected virtual void OnActiveDocumentChanging(MDIDocumentEventArgs e) => ActiveDocumentChanging?.Invoke(this, e);
        protected virtual void OnActiveDocumentChanged(MDIDocumentEventArgs e) => ActiveDocumentChanged?.Invoke(this, e);
        protected virtual void OnDocumentReordered(MDIDocumentReorderEventArgs e) => DocumentReordered?.Invoke(this, e);
        protected virtual void OnDocumentPinnedChanged(MDIDocumentEventArgs e) => DocumentPinnedChanged?.Invoke(this, e);
        protected virtual void OnDocumentDirtyStateChanged(MDIDocumentEventArgs e) => DocumentDirtyStateChanged?.Invoke(this, e);
        #endregion

        #region Overrides
        protected override void OnResize(EventArgs e) { base.OnResize(e); RecalculateLayout(); Invalidate(); }
        #endregion

        #region Dispose
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _animationTimer?.Stop(); _animationTimer?.Dispose();
                foreach (var d in _documents.ToList()) if (d.Content != null && !d.Content.IsDisposed) d.Content.Dispose();
                _documents.Clear(); _documentsById.Clear();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
