using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using TheTechIdea.Beep.Desktop.Common;
using static System.Windows.Forms.TabControl;

namespace TheTechIdea.Beep.Winform.Controls
{
    [DefaultProperty("TabPages")]
    [ToolboxItem(true)]
    [DisplayName("Beep Tabs")]
    [Category("Beep Controls")]
    public class BeepTabs : BeepControl
    {
        private FlowLayoutPanel _headerPanel; // Designer-managed
        private TabControl _tabControl;       // Designer-managed
        private HeaderLocation _headerLocation = HeaderLocation.Top;
        public event EventHandler<TabEventArgs> TabSelected;
        public event EventHandler<TabEventArgs> TabButtonClicked;

        private BindingList<SimpleItem> _tabs ;
        [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BindingList<SimpleItem> Tabs
        {
            get => _tabs;
            set
            {
                _tabs = value;

                SyncTabPages();
                RefreshHeaders();
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public HeaderLocation HeaderLocation
        {
            get => _headerLocation;
            set
            {
                if (_headerLocation != value)
                {
                    _headerLocation = value;
                    PerformLayout();
                }
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int HeaderButtonHeight
        {
            get => _headerButtonHeight;
            set
            {
                _headerButtonHeight = value;
                if (!DesignMode)
                    RefreshHeaders();
            }
        }
        private int _headerButtonHeight = 30;

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int HeaderButtonWidth
        {
            get => _headerButtonWidth;
            set
            {
                _headerButtonWidth = value;
                if (!DesignMode)
                    RefreshHeaders();
            }
        }
        private int _headerButtonWidth = 60;

        public int SelectedIndex
        {
            get => _tabControl?.SelectedIndex ?? -1;
            set
            {
                if (_tabControl != null && value >= 0 && value < _tabControl.TabPages.Count)
                {
                    _tabControl.SelectedIndex = value;
                    HighlightButtonAt(value);
                    OnTabSelected(_tabControl.TabPages[value], value);
                }
            }
        }

        public TabPage SelectedTab
        {
            get => _tabControl?.SelectedTab;
            set
            {
                if (_tabControl != null && value != null && _tabControl.TabPages.Contains(value))
                {
                    _tabControl.SelectedTab = value;
                    HighlightButtonAt(_tabControl.TabPages.IndexOf(value));
                    OnTabSelected(value, _tabControl.TabPages.IndexOf(value));
                }
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TabPageCollection TabPages => _tabControl?.TabPages;

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TabControl TabControl
        {
            get => _tabControl;
            set
            {
                if (_tabControl != value)
                {
                    if (_tabControl != null)
                    {
                        _tabControl.SelectedIndexChanged -= TabControl_SelectedIndexChanged;
                        Controls.Remove(_tabControl);
                    }

                    _tabControl = value;

                    if (_tabControl != null)
                    {
                        _tabControl.SelectedIndexChanged += TabControl_SelectedIndexChanged;
                        Controls.Add(_tabControl);
                    }

                    if (!DesignMode)
                    {
                        SyncTabPages();
                        RefreshHeaders();
                    }
                    PerformLayout();
                }
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public FlowLayoutPanel HeaderPanel
        {
            get => _headerPanel;
            set
            {
                if (_headerPanel != value)
                {
                    if (_headerPanel != null)
                        Controls.Remove(_headerPanel);

                    _headerPanel = value;

                    if (_headerPanel != null)
                        Controls.Add(_headerPanel);

                    if (!DesignMode)
                        RefreshHeaders();
                    PerformLayout();
                }
            }
        }

        public BeepTabs()
        {
            if (_tabs == null)
            {
                _tabs = new BindingList<SimpleItem>();
            }
            // Initialize controls for both runtime and design-time fallback
            _headerPanel = new FlowLayoutPanel
            {
                AutoSize = false,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = SystemColors.Control,
                Height = 50
            };
            if (_tabControl == null)
            {
                _tabControl = new TabControl
                {
                    Dock = DockStyle.Fill
                };

                _tabControl.SelectedIndexChanged += TabControl_SelectedIndexChanged;

                Controls.Add(_tabControl);
            }
         
            Controls.Add(_headerPanel);
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            if (_headerPanel == null || _tabControl == null)
                return;

            switch (_headerLocation)
            {
                case HeaderLocation.Top:
                    _headerPanel.Dock = DockStyle.Top;
                    _tabControl.Dock = DockStyle.Fill;
                    _headerPanel.BringToFront();
                    break;
                case HeaderLocation.Bottom:
                    _headerPanel.Dock = DockStyle.Bottom;
                    _tabControl.Dock = DockStyle.Fill;
                    _headerPanel.BringToFront();
                    break;
                case HeaderLocation.Left:
                    _headerPanel.Dock = DockStyle.Left;
                    _tabControl.Dock = DockStyle.Fill;
                    _headerPanel.BringToFront();
                    break;
                case HeaderLocation.Right:
                    _headerPanel.Dock = DockStyle.Right;
                    _tabControl.Dock = DockStyle.Fill;
                    _headerPanel.BringToFront();
                    break;
            }
        }

        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            HighlightButtonAt(_tabControl.SelectedIndex);
            OnTabSelected(_tabControl.SelectedTab, _tabControl.SelectedIndex);
        }

        private void SyncTabPages()
        {
            if (TabControl == null) return;

            // Remove old pages, add new ones
            var existingTabs = TabControl.TabPages.Cast<TabPage>().ToList();
            var existingGuids = existingTabs.Select(tp => tp.Tag as string).ToList();
            var newGuids = _tabs.Select(t => t.GuidId).ToList();

            // Remove tabs that no longer exist in _tabs
            foreach (var tp in existingTabs.Where(tp => !newGuids.Contains(tp.Tag as string)).ToList())
            {
                TabControl.TabPages.Remove(tp);
            }

            // Add new tabs from _tabs
            foreach (var si in _tabs.Where(t => !existingGuids.Contains(t.GuidId)))
            {
                var page = new TabPage(si.Name)
                {
                    Tag = si.GuidId
                };
                TabControl.TabPages.Add(page);
            }
        }


        public void RefreshHeaders()
        {
            if (_headerPanel == null || _tabControl == null)
                return;

            _headerPanel.Controls.Clear();
            for (int i = 0; i < _tabControl.TabPages.Count; i++)
            {
                var tabPage = _tabControl.TabPages[i];
                var btn = CreateTabButton(tabPage, i);
                _headerPanel.Controls.Add(btn);
            }
            HighlightButtonAt(_tabControl.SelectedIndex);
        }

        private BeepButton CreateTabButton(TabPage tabPage, int index)
        {
            var btn = new BeepButton
            {
                Text = tabPage.Text,
                Margin = new Padding(2),
                Id = index,
                Size = new Size(_headerButtonWidth, _headerButtonHeight),
                TextAlign = ContentAlignment.MiddleCenter,
                Tag = tabPage.Tag
            };
            btn.Click += (s, e) => SelectTabByIndex(index);
            return btn;
        }

        public void SelectTabByIndex(int index)
        {
            if (_tabControl == null || index < 0 || index >= _tabControl.TabPages.Count)
                return;

            _tabControl.SelectedIndex = index;
            HighlightButtonAt(index);
            OnTabSelected(_tabControl.TabPages[index], index);
        }

        public void SelectTabByGuid(string guidId)
        {
            if (_tabControl == null)
                return;

            var tabIndex = _tabControl.TabPages.Cast<TabPage>().ToList().FindIndex(tp => tp.Tag as string == guidId);
            if (tabIndex >= 0)
            {
                _tabControl.SelectedIndex = tabIndex;
                HighlightButtonAt(tabIndex);
                OnTabSelected(_tabControl.TabPages[tabIndex], tabIndex);
            }
        }

        private void HighlightButtonAt(int index)
        {
            if (_headerPanel == null)
                return;

            foreach (Control ctrl in _headerPanel.Controls)
            {
                if (ctrl is BeepButton btn)
                    btn.IsSelected = btn.Id == index;
            }
        }

        protected virtual void OnTabSelected(TabPage selectedTab, int selectedIndex)
        {
            TabSelected?.Invoke(this, new TabEventArgs(selectedTab, selectedIndex));
        }
    }

    public enum HeaderLocation
    {
        Top,
        Bottom,
        Left,
        Right
    }

    public class TabEventArgs : EventArgs
    {
        public TabEventArgs(TabPage tabPage, int tabIndex)
        {
            TabPage = tabPage;
            TabIndex = tabIndex;
        }

        public TabPage TabPage { get; }
        public int TabIndex { get; }
    }
}