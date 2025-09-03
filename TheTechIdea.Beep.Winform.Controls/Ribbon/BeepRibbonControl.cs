using System.ComponentModel;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepRibbonControl : Control
    {
        private TabControl _tabs = new TabControl { Dock = DockStyle.Fill };
        private ToolStrip _quickAccess = new ToolStrip { Dock = DockStyle.Top, GripStyle = ToolStripGripStyle.Hidden, RenderMode = ToolStripRenderMode.System };
        private Panel _contextHeader = new Panel { Dock = DockStyle.Top, Height = 18 }; // draws contextual group header bands
        private RibbonTheme _theme = new RibbonTheme();
        private bool _darkMode;

        // Backstage
        private ToolStripDropDownButton _backstageButton;
        private ToolStripDropDown _backstageDropDown;
        private Panel _backstagePanelContent = new Panel { BackColor = SystemColors.ControlLightLight, Size = new Size(600, 400) };

        // Contextual groups
        private class ContextualGroup
        {
            public string Name = string.Empty;
            public Color Color = Color.LightBlue;
            public bool Visible = false;
            public readonly List<TabPage> Pages = new();
        }
        private readonly List<ContextualGroup> _contextGroups = new();
        private readonly Dictionary<TabPage, ContextualGroup> _pageToGroup = new();

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TabControl Tabs => _tabs;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ToolStrip QuickAccess => _quickAccess;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public RibbonTheme Theme
        {
            get => _theme;
            set { _theme = value ?? new RibbonTheme(); Invalidate(true); ApplyTheme(); }
        }

        [DefaultValue(false)]
        public bool DarkMode
        {
            get => _darkMode;
            set { _darkMode = value; ApplyTheme(); }
        }

        // Backstage content surface
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Panel BackstageContent => _backstagePanelContent;

        public BeepRibbonControl()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            Controls.Add(_tabs);
            Controls.Add(_contextHeader);
            Controls.Add(_quickAccess);
            Height = 120;

            // Backstage setup
            _backstageButton = new ToolStripDropDownButton("File") { ShowDropDownArrow = true, AutoToolTip = false };
            _backstageDropDown = new ToolStripDropDown { Padding = Padding.Empty, AutoClose = true, AutoSize = false };            
            var host = new ToolStripControlHost(_backstagePanelContent) { AutoSize = false, Size = _backstagePanelContent.Size, Margin = Padding.Empty, Padding = Padding.Empty };
            _backstageDropDown.Items.Add(host);
            _backstageButton.DropDown = _backstageDropDown;
            _quickAccess.Items.Insert(0, _backstageButton);

            _quickAccess.Renderer = new BeepRibbonToolStripRenderer(this);
            _contextHeader.Paint += ContextHeader_Paint;
            _tabs.ControlAdded += (_, __) => _contextHeader.Invalidate();
            _tabs.ControlRemoved += (_, __) => _contextHeader.Invalidate();
            _tabs.SelectedIndexChanged += (_, __) => _contextHeader.Invalidate();

            ApplyTheme();
        }

        private void ContextHeader_Paint(object? sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(_theme.Background);
            // Draw a colored band above the span of tabs for each visible contextual group
            for (int gi = 0; gi < _contextGroups.Count; gi++)
            {
                var grp = _contextGroups[gi];
                if (!grp.Visible || grp.Pages.Count == 0) continue;
                // find leftmost and rightmost tab rect among group's visible tabs
                Rectangle? left = null, right = null;
                for (int i = 0; i < _tabs.TabCount; i++)
                {
                    var page = _tabs.TabPages[i];
                    if (!_pageToGroup.TryGetValue(page, out var gref) || gref != grp) continue;
                    var r = _tabs.GetTabRect(i);
                    if (!left.HasValue || r.Left < left.Value.Left) left = r;
                    if (!right.HasValue || r.Right > right.Value.Right) right = r;
                }
                if (left.HasValue && right.HasValue)
                {
                    var band = new Rectangle(left.Value.Left, 0, right.Value.Right - left.Value.Left, _contextHeader.Height - 1);
                    using var b = new SolidBrush(Color.FromArgb(120, grp.Color));
                    using var p = new Pen(grp.Color);
                    g.FillRectangle(b, band);
                    g.DrawRectangle(p, band);
                    using var textBrush = new SolidBrush(_theme.Text);
                    g.DrawString(grp.Name, SystemFonts.MenuFont, textBrush, new PointF(band.Left + 6, 2));
                }
            }
        }

        public int AddContextualGroup(string name, Color color)
        {
            var grp = new ContextualGroup { Name = name, Color = color, Visible = false };
            _contextGroups.Add(grp);
            return _contextGroups.Count - 1;
        }

        public TabPage AddContextualTab(int groupId, string title)
        {
            if (groupId < 0 || groupId >= _contextGroups.Count) throw new ArgumentOutOfRangeException(nameof(groupId));
            var grp = _contextGroups[groupId];
            var page = new TabPage(title) { BackColor = _theme.TabActiveBack };
            grp.Pages.Add(page);
            _pageToGroup[page] = grp;
            if (grp.Visible)
            {
                _tabs.TabPages.Add(page);
            }
            _contextHeader.Invalidate();
            return page;
        }

        public void SetContextualGroupVisible(int groupId, bool visible)
        {
            if (groupId < 0 || groupId >= _contextGroups.Count) return;
            var grp = _contextGroups[groupId];
            if (grp.Visible == visible) return;
            grp.Visible = visible;
            if (visible)
            {
                foreach (var p in grp.Pages)
                {
                    if (!_tabs.TabPages.Contains(p)) _tabs.TabPages.Add(p);
                }
            }
            else
            {
                foreach (var p in grp.Pages)
                {
                    if (_tabs.TabPages.Contains(p)) _tabs.TabPages.Remove(p);
                }
            }
            _contextHeader.Invalidate();
        }

        public void ApplyThemeFromBeep(IBeepTheme? theme)
        {
            Theme = RibbonThemeMapper.Map(theme, _darkMode);
        }

        private void ApplyTheme()
        {
            BackColor = _theme.Background;
            _quickAccess.BackColor = _theme.QuickAccessBack;
            _quickAccess.ForeColor = _theme.Text;
            _contextHeader.BackColor = _theme.Background;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            using var back = new SolidBrush(_theme.Background);
            g.FillRectangle(back, ClientRectangle);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            _tabs.Invalidate();
            _contextHeader.Invalidate();
        }

        public TabPage AddPage(string title)
        {
            var page = new TabPage(title);
            page.BackColor = _theme.TabActiveBack;
            _tabs.TabPages.Add(page);
            return page;
        }

        public BeepRibbonGroup AddGroup(TabPage page, string title)
        {
            var group = new BeepRibbonGroup { Text = title, Renderer = new BeepRibbonToolStripRenderer(this) };
            page.Controls.Add(group);
            page.Controls.SetChildIndex(group, 0);
            return group;
        }

        public void SaveQuickAccessTo(string file)
        {
            try
            {
                var items = _quickAccess.Items.Cast<ToolStripItem>().Select(i => i.Text).ToArray();
                File.WriteAllLines(file, items);
            }
            catch { }
        }

        public void LoadQuickAccessFrom(string file)
        {
            try
            {
                if (!File.Exists(file)) return;
                var lines = File.ReadAllLines(file);
                _quickAccess.Items.Clear();
                foreach (var l in lines)
                {
                    _quickAccess.Items.Add(new ToolStripButton(l));
                }
            }
            catch { }
        }

        private class BeepRibbonToolStripRenderer : ToolStripProfessionalRenderer
        {
            private readonly BeepRibbonControl _owner;
            public BeepRibbonToolStripRenderer(BeepRibbonControl owner) : base(new ProfessionalColorTable()) { _owner = owner; }
            protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
            {
                using var back = new SolidBrush(_owner._theme.GroupBack);
                e.Graphics.FillRectangle(back, e.AffectedBounds);
                using var border = new Pen(_owner._theme.GroupBorder);
                e.Graphics.DrawRectangle(border, new Rectangle(Point.Empty, e.ToolStrip.Size - new Size(1,1)));
            }
            protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
            {
                base.OnRenderButtonBackground(e);
            }
        }
    }
}
