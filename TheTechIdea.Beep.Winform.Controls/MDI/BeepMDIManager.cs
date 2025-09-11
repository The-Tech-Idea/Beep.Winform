using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Forms.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.MDI
{
    /// <summary>
    /// BeepMdiManager
    /// Drop-on component that enables DevExpress-like MDI (Tabbed MDI, child tracking, menu merge)
    /// on top of BeepiForm's borderless chrome.
    /// </summary>
    [DesignerCategory("Component")]
    [ToolboxItem(true)]
    [Designer("TheTechIdea.Beep.Winform.Controls.MDI.Designers.BeepMdiManagerDesigner, TheTechIdea.Beep.Winform.Controls.Design.Server")]
    public sealed partial class BeepMdiManager : Component
    {
        // Documents design-time collection -------------------------------------------------
        private readonly BeepMdiDocumentCollection _documents = new();
        [Category("Data"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("Design-time documents. When AutoCreateDocumentsOnLoad is true and FormType is set they will be instantiated on Load.")]
        public BeepMdiDocumentCollection Documents => _documents;

        [Category("Behavior"), DefaultValue(false)]
        [Description("Automatically instantiate documents (with FormType) on HostForm.Load.")]
        public bool AutoCreateDocumentsOnLoad { get; set; } = false;

        // Hook HostForm.Load for auto creation
        private void HostForm_Load(object sender, EventArgs e)
        {
            if (!AutoCreateDocumentsOnLoad) return;
            try
            {
                foreach (var doc in _documents.ToList())
                {
                    if (doc?.FormType == null) continue;
                    try
                    {
                        if (Activator.CreateInstance(doc.FormType) is Form f)
                        {
                            f.MdiParent = _hostForm;
                            f.Text = string.IsNullOrWhiteSpace(doc.Text) ? doc.Name : doc.Text;
                            if (doc.StartMaximized) f.WindowState = FormWindowState.Maximized;
                            f.Show();
                        }
                    }
                    catch { /* ignore single doc failure */ }
                }
            }
            catch { }
        }

        // -----------------------------
        // Public API
        // -----------------------------
        private Form _hostForm;
        private Panel _documentHostPanel;
        private MdiTabStrip _tabStrip;
        private MdiClient _mdiClient;
        private readonly Dictionary<Form, TabPage> _tabByChild = new();
        private readonly Dictionary<TabPage, Form> _childByTab = new();
        private MenuStrip _mergeTarget;
        private MenuStrip _lastMergedChildMenu;

        [Browsable(true), Category("Behavior")]
        [Description("Enable/disable Tabbed MDI. MDI container management stays on.")]
        public bool EnableTabbedMdi { get; set; } = true;

        [Browsable(true), Category("Behavior")]
        [Description("Hide system caption from MDI children so the parent chrome owns the UI.")]
        public bool HideChildCaptions { get; set; } = true;

        [Browsable(true), Category("Behavior")]
        [Description("Allow dragging tabs to reorder.")]
        public bool AllowTabReorder { get; set; } = true;

        [Browsable(true), Category("Appearance")]
        [Description("Preferred height of the tab strip.")]
        public int TabHeight { get; set; } = 30;

        [Browsable(true), Category("Behavior")]
        [Description("Target MenuStrip on the parent to receive merged child menus (optional).")]
        public MenuStrip MergeTargetMenuStrip
        {
            get => _mergeTarget;
            set
            {
                _mergeTarget = value;
                // Re-merge with the active child if any.
                ReapplyMenuMerge();
            }
        }

        [Browsable(true), Category("Behavior")]
        [Description("Enable/disable automatic ToolStrip menu merge for active child.")]
        public bool EnableMenuMerge { get; set; } = false;

        /// <summary>
        /// Host form (usually your BeepiForm). If left null, the manager will try to use the component's parent form at runtime.
        /// </summary>
        [Browsable(false)]
        public Form HostForm
        {
            get => _hostForm;
            set
            {
                if (_hostForm == value) return;
                DetachFromForm();
                _hostForm = value;
                if (_hostForm?.IsHandleCreated == true)
                    AttachToForm();
                else
                    _hostForm.HandleCreated += HostForm_HandleCreated;
            }
        }

        // Raised when the active MDI child changes.
        public event EventHandler<Form> ActiveDocumentChanged;

        // -----------------------------
        // Construction & design-time
        // -----------------------------
        public BeepMdiManager() { }
        public BeepMdiManager(IContainer container)
        {
            container?.Add(this);
        }

        public override ISite Site
        {
            get => base.Site;
            set
            {
                base.Site = value;
                // Try to infer host at design time without forcing anything at runtime.
                if (DesignMode && value?.Container is IContainer)
                {
                    // Nothing to do: host will be available at runtime.
                }
            }
        }

        // -----------------------------
        // Initialization wiring
        // -----------------------------
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing) DetachFromForm();
        }

        private void HostForm_HandleCreated(object sender, EventArgs e)
        {
            _hostForm.HandleCreated -= HostForm_HandleCreated;
            AttachToForm();
        }

        private void AttachToForm()
        {
            if (_hostForm == null) return;

            // Ensure MDI container
            _hostForm.IsMdiContainer = true;

            // Find or create MdiClient
            _mdiClient = _hostForm.Controls.OfType<MdiClient>().FirstOrDefault();
            if (_mdiClient == null)
            {
                // Setting IsMdiContainer usually adds an MdiClient automatically. But just in case:
                _hostForm.IsMdiContainer = false;
                _hostForm.IsMdiContainer = true;
                _mdiClient = _hostForm.Controls.OfType<MdiClient>().FirstOrDefault();
            }

            if (_mdiClient == null)
                throw new InvalidOperationException("MDI client area could not be created.");

            // Create the document host panel (fills client area)
            _documentHostPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = _hostForm.BackColor
            };

            // Create the tab strip (top)
            _tabStrip = new MdiTabStrip
            {
                Dock = DockStyle.Top,
                Height = TabHeight,
                Visible = EnableTabbedMdi,
                AllowReorder = AllowTabReorder
            };
            _tabStrip.TabClosing += TabStrip_TabClosing;
            _tabStrip.SelectedTabChanged += TabStrip_SelectedTabChanged;
            _tabStrip.TabReordered += TabStrip_TabReordered;
            _tabStrip.DoubleClickMaximizeRequested += TabStrip_DoubleClickMaximizeRequested;

            // Reparent: put everything under document host panel
            _hostForm.SuspendLayout();

            // Move mdi client into our panel
            _hostForm.Controls.Remove(_mdiClient);
            _documentHostPanel.Controls.Add(_mdiClient);

            // Insert tab strip on top
            _documentHostPanel.Controls.Add(_tabStrip);

            // Add the managed panel to the form
            _hostForm.Controls.Add(_documentHostPanel);
            _documentHostPanel.BringToFront();

            // Hook lifecycle
            _hostForm.MdiChildActivate += HostForm_MdiChildActivate;
            _hostForm.Load -= HostForm_Load; // avoid multiple
            _hostForm.Load += HostForm_Load;

            // Sync any already-open children
            SyncTabsWithChildren();

            _hostForm.ResumeLayout();

            // Respect BeepiForm caption padding automatically (the caption helper already adjusts Padding on the host).
            // Nothing else required here: our tab strip docks below that padding.
        }

        private void DetachFromForm()
        {
            if (_hostForm == null) return;

            _hostForm.MdiChildActivate -= HostForm_MdiChildActivate;
            _hostForm.Load -= HostForm_Load;

            foreach (var child in _tabByChild.Keys.ToList())
            {
                child.FormClosed -= Child_FormClosed;
                child.TextChanged -= Child_TextChanged;
            }
            _tabByChild.Clear();
            _childByTab.Clear();

            if (_documentHostPanel != null)
            {
                if (_mdiClient != null)
                {
                    _documentHostPanel.Controls.Remove(_mdiClient);
                    _hostForm.Controls.Add(_mdiClient);
                    _mdiClient.Dock = DockStyle.Fill;
                }
                _hostForm.Controls.Remove(_documentHostPanel);
                _documentHostPanel.Dispose();
                _documentHostPanel = null;
            }

            if (_tabStrip != null)
            {
                _tabStrip.TabClosing -= TabStrip_TabClosing;
                _tabStrip.SelectedTabChanged -= TabStrip_SelectedTabChanged;
                _tabStrip.TabReordered -= TabStrip_TabReordered;
                _tabStrip.DoubleClickMaximizeRequested -= TabStrip_DoubleClickMaximizeRequested;
                _tabStrip.Dispose();
                _tabStrip = null;
            }

            _hostForm = null;
        }

        // -----------------------------
        // Event wiring & syncing
        // -----------------------------
        private void HostForm_MdiChildActivate(object sender, EventArgs e)
        {
            var child = _hostForm.ActiveMdiChild;
            EnsureTabsForChildren();

            if (child != null)
            {
                if (!_tabByChild.TryGetValue(child, out var tab))
                {
                    tab = AddTabForChild(child);
                }

                _tabStrip.SelectTab(tab);
                ApplyChildChromePolicy(child);
                ApplyMenuMergeFor(child);
                ActiveDocumentChanged?.Invoke(this, child);
            }
            else
            {
                RevertMenuMerge();
            }
        }

        private void Child_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (sender is Form child && _tabByChild.TryGetValue(child, out var tab))
            {
                _tabStrip.RemoveTab(tab);
                _childByTab.Remove(tab);
                _tabByChild.Remove(child);
            }

            if (_hostForm?.ActiveMdiChild != null)
                ApplyMenuMergeFor(_hostForm.ActiveMdiChild);
            else
                RevertMenuMerge();
        }

        private void Child_TextChanged(object sender, EventArgs e)
        {
            if (sender is Form child && _tabByChild.TryGetValue(child, out var tab))
            {
                tab.Text = child.Text;
                _tabStrip.Invalidate(); // refresh header text
            }
        }

        private void TabStrip_SelectedTabChanged(object sender, TabPage e)
        {
            if (e != null && _childByTab.TryGetValue(e, out var child))
            {
                try { child.Activate(); } catch { /* ignore */ }
            }
        }

        private void TabStrip_TabClosing(object sender, TabPage e)
        {
            if (e != null && _childByTab.TryGetValue(e, out var child))
            {
                try { child.Close(); } catch { /* ignore */ }
            }
        }

        private void TabStrip_TabReordered(object sender, (int oldIndex, int newIndex) e)
        {
            // Visual only; MDI z-order stays managed by activation.
        }

        private void TabStrip_DoubleClickMaximizeRequested(object sender, TabPage e)
        {
            if (e != null && _childByTab.TryGetValue(e, out var child))
            {
                ToggleChildMaximize(child);
            }
        }

        // -----------------------------
        // Helpers
        // -----------------------------
        private void SyncTabsWithChildren()
        {
            EnsureTabsForChildren();

            // Select active
            var active = _hostForm.ActiveMdiChild;
            if (active != null && _tabByChild.TryGetValue(active, out var tab))
            {
                _tabStrip.SelectTab(tab);
            }
        }

        private void EnsureTabsForChildren()
        {
            if (!EnableTabbedMdi)
            {
                _tabStrip.Visible = false;
                return;
            }

            _tabStrip.Visible = true;

            foreach (var child in _hostForm.MdiChildren)
            {
                if (!_tabByChild.ContainsKey(child))
                {
                    AddTabForChild(child);
                    ApplyChildChromePolicy(child);
                    child.FormClosed += Child_FormClosed;
                    child.TextChanged += Child_TextChanged;
                }
            }

            // Remove tabs for closed children (safety)
            foreach (var kvp in _tabByChild.ToList())
            {
                if (kvp.Key.IsDisposed)
                {
                    _tabStrip.RemoveTab(kvp.Value);
                    _childByTab.Remove(kvp.Value);
                    _tabByChild.Remove(kvp.Key);
                }
            }
        }

        private TabPage AddTabForChild(Form child)
        {
            var tab = new TabPage(child.Text) { Tag = child };
            _tabByChild[child] = tab;
            _childByTab[tab] = child;
            _tabStrip.AddTab(tab);
            return tab;
        }

        private void ApplyChildChromePolicy(Form child)
        {
            if (!HideChildCaptions) return;

            try
            {
                // If child is BeepiForm, its caption is already handled by the parent; ensure borderless feel.
                // For standard Forms, we also remove their system caption.
                child.ControlBox = false;
                child.MinimizeBox = false;
                child.MaximizeBox = false;
                child.FormBorderStyle = FormBorderStyle.None;
            }
            catch { /* ignore */ }
        }

        private static void ToggleChildMaximize(Form child)
        {
            try
            {
                child.WindowState = child.WindowState == FormWindowState.Maximized
                    ? FormWindowState.Normal
                    : FormWindowState.Maximized;
            }
            catch { /* ignore */ }
        }

        // -----------------------------
        // Menu merge (optional)
        // -----------------------------
        private void ApplyMenuMergeFor(Form child)
        {
            if (!EnableMenuMerge || _mergeTarget == null) return;

            RevertMenuMerge();

            var childMenu = child.Controls.OfType<MenuStrip>().FirstOrDefault();
            if (childMenu != null)
            {
                try
                {
                    ToolStripManager.Merge(childMenu, _mergeTarget);
                    _lastMergedChildMenu = childMenu;
                }
                catch { /* ignore */ }
            }
        }

        private void ReapplyMenuMerge()
        {
            if (!EnableMenuMerge || _mergeTarget == null) return;

            if (_hostForm?.ActiveMdiChild != null)
            {
                ApplyMenuMergeFor(_hostForm.ActiveMdiChild);
            }
        }

        private void RevertMenuMerge()
        {
            if (!EnableMenuMerge || _mergeTarget == null || _lastMergedChildMenu == null) return;

            try
            {
                ToolStripManager.RevertMerge(_mergeTarget);
            }
            catch { /* ignore */ }
            finally
            {
                _lastMergedChildMenu = null;
            }
        }

        // -----------------------------
        // Public helpers (API)
        // -----------------------------
        /// <summary>Open (show) an MDI child and create/select its tab.</summary>
        public void OpenDocument(Form child)
        {
            if (child == null) return;
            child.MdiParent = _hostForm ?? throw new InvalidOperationException("HostForm is not set.");
            child.Show();
            child.Activate();
        }

        /// <summary>Close the active MDI child.</summary>
        public void CloseActiveDocument()
        {
            var child = _hostForm?.ActiveMdiChild;
            child?.Close();
        }

        // ====================================================================
        // Internal lightweight tab strip
        // ====================================================================
        private sealed class MdiTabStrip : Control
        {
            private readonly List<TabPage> _tabs = new();
            private int _selectedIndex = -1;
            private const int CloseGlyphSize = 12;
            private const int TabPaddingH = 14;
            private const int TabMinWidth = 80;

            private bool _mouseDown;
            private int _dragIndex = -1;
            private Point _dragStart;
            private Rectangle _dragRect;

            public bool AllowReorder { get; set; } = true;

            public event EventHandler<TabPage> SelectedTabChanged;
            public event EventHandler<TabPage> TabClosing;
            public event EventHandler<(int oldIndex, int newIndex)> TabReordered;
            public event EventHandler<TabPage> DoubleClickMaximizeRequested;

            public MdiTabStrip()
            {
                SetStyle(ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.OptimizedDoubleBuffer |
                         ControlStyles.ResizeRedraw |
                         ControlStyles.UserPaint, true);
                Height = 30;
                BackColor = SystemColors.ControlLightLight;
            }

            public void AddTab(TabPage page)
            {
                if (page == null || _tabs.Contains(page)) return;
                _tabs.Add(page);
                if (_selectedIndex == -1) _selectedIndex = 0;
                Invalidate();
            }

            public void RemoveTab(TabPage page)
            {
                if (page == null) return;
                var idx = _tabs.IndexOf(page);
                if (idx < 0) return;

                _tabs.RemoveAt(idx);
                if (_selectedIndex >= _tabs.Count) _selectedIndex = _tabs.Count - 1;
                Invalidate();
                SelectedTabChanged?.Invoke(this, SelectedTab);
            }

            public void SelectTab(TabPage page)
            {
                var idx = _tabs.IndexOf(page);
                if (idx >= 0 && idx != _selectedIndex)
                {
                    _selectedIndex = idx;
                    Invalidate();
                    SelectedTabChanged?.Invoke(this, page);
                }
            }

            public TabPage SelectedTab => _selectedIndex >= 0 && _selectedIndex < _tabs.Count ? _tabs[_selectedIndex] : null;

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                var g = e.Graphics;
                g.Clear(BackColor);

                if (_tabs.Count == 0) return;

                var x = 8;
                var h = Height - 1;

                for (int i = 0; i < _tabs.Count; i++)
                {
                    var page = _tabs[i];
                    var text = page.Text ?? "Document";
                    var size = TextRenderer.MeasureText(text, Font);
                    var tabW = Math.Max(TabMinWidth, size.Width + 2 * TabPaddingH + CloseGlyphSize + 8);
                    var rect = new Rectangle(x, 0, tabW, h);

                    var isSel = i == _selectedIndex;
                    using (var bg = new SolidBrush(isSel ? Color.White : ControlPaint.Light(BackColor)))
                    using (var br = new SolidBrush(Color.Black))
                    using (var pen = new Pen(Color.Silver))
                    {
                        g.FillRectangle(bg, rect);
                        g.DrawRectangle(pen, rect.Left, rect.Top, rect.Width - 1, rect.Height - 1);
                        var textRect = new Rectangle(rect.Left + TabPaddingH, rect.Top, rect.Width - TabPaddingH - CloseGlyphSize - 10, rect.Height);
                        TextRenderer.DrawText(g, text, Font, textRect, br.Color, TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);

                        // Close glyph
                        var cx = rect.Right - CloseGlyphSize - 6;
                        var cy = rect.Top + (rect.Height - CloseGlyphSize) / 2;
                        var closeRect = new Rectangle(cx, cy, CloseGlyphSize, CloseGlyphSize);
                        DrawCloseGlyph(g, closeRect, isSel ? Color.Black : Color.DimGray);
                        page.Bounds = rect;
                        page.Tag = closeRect; // store close rect in Tag for hit testing
                    }

                    x += tabW + 4;
                }
            }

            private static void DrawCloseGlyph(Graphics g, Rectangle r, Color color)
            {
                using var pen = new Pen(color, 1.5f);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.DrawLine(pen, r.Left, r.Top, r.Right, r.Bottom);
                g.DrawLine(pen, r.Left, r.Bottom, r.Right, r.Top);
            }

            protected override void OnMouseDown(MouseEventArgs e)
            {
                base.OnMouseDown(e);
                _mouseDown = true;
                _dragStart = e.Location;
                _dragRect = new Rectangle(e.X - SystemInformation.DragSize.Width / 2,
                                          e.Y - SystemInformation.DragSize.Height / 2,
                                          SystemInformation.DragSize.Width,
                                          SystemInformation.DragSize.Height);

                var hitIndex = HitTestTab(e.Location, out bool onClose);
                if (hitIndex >= 0)
                {
                    if (onClose && e.Button == MouseButtons.Left)
                    {
                        TabClosing?.Invoke(this, _tabs[hitIndex]);
                        return;
                    }

                    _selectedIndex = hitIndex;
                    SelectedTabChanged?.Invoke(this, _tabs[hitIndex]);
                    Invalidate();

                    if (AllowReorder && e.Button == MouseButtons.Left)
                    {
                        _dragIndex = hitIndex;
                    }
                }
            }

            protected override void OnMouseUp(MouseEventArgs e)
            {
                base.OnMouseUp(e);
                _mouseDown = false;
                _dragIndex = -1;

                // Middle-click close
                var hitIndex = HitTestTab(e.Location, out bool onCloseArea);
                if (hitIndex >= 0 && e.Button == MouseButtons.Middle)
                {
                    TabClosing?.Invoke(this, _tabs[hitIndex]);
                }
            }

            protected override void OnMouseMove(MouseEventArgs e)
            {
                base.OnMouseMove(e);

                if (!_mouseDown || _dragIndex < 0 || !AllowReorder) return;

                if (!_dragRect.Contains(e.Location))
                {
                    // Begin reorder by swapping with neighbor based on X
                    int overIndex = HitTestTab(e.Location, out _);
                    if (overIndex >= 0 && overIndex != _dragIndex)
                    {
                        var tmp = _tabs[_dragIndex];
                        _tabs[_dragIndex] = _tabs[overIndex];
                        _tabs[overIndex] = tmp;
                        TabReordered?.Invoke(this, (_dragIndex, overIndex));
                        _dragIndex = overIndex;
                        _selectedIndex = overIndex;
                        Invalidate();
                    }
                }
            }

            protected override void OnDoubleClick(EventArgs e)
            {
                base.OnDoubleClick(e);
                var me = e as MouseEventArgs ?? new MouseEventArgs(MouseButtons.Left, 2, PointToClient(MousePosition).X, PointToClient(MousePosition).Y, 0);
                var hitIndex = HitTestTab(me.Location, out _);
                if (hitIndex >= 0)
                {
                    DoubleClickMaximizeRequested?.Invoke(this, _tabs[hitIndex]);
                }
            }

            private int HitTestTab(Point p, out bool onCloseGlyph)
            {
                onCloseGlyph = false;
                for (int i = 0; i < _tabs.Count; i++)
                {
                    var rect = _tabs[i].Bounds;
                    if (rect.Contains(p))
                    {
                        var closeRect = _tabs[i].Tag as Rectangle? ?? Rectangle.Empty;
                        onCloseGlyph = closeRect.Contains(p);
                        return i;
                    }
                }
                return -1;
            }
        }

        // Simple carrier class (no real Control children used, just painter/header logic)
        private sealed class TabPage
        {
            public TabPage(string text) { Text = text; }
            public string Text { get; set; }
            public Rectangle Bounds { get; set; }
            public object Tag { get; set; } // holds close glyph rect
        }
    }
}
