using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Desktop.Common;
using TheTechIdea.Beep.Winform.Controls.Converters;

namespace TheTechIdea.Beep.Winform.Controls
{
   [Designer(typeof(TheTechIdea.Beep.Winform.Controls.Design.DynamicTabControlDesigner))]
    public class BeepDynamicTabControl : BeepPanel
    {
        static int count = 0;

        // Panels mapped directly by SimpleItem.GuidId (string)
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Dictionary<string, Panel> Panels = new(StringComparer.OrdinalIgnoreCase);


        private BindingList<SimpleItem> _tabs = new BindingList<SimpleItem>();
        private SimpleItem _selectedTab;
        private HeaderLocation _headerLocation = HeaderLocation.Top;

        // Events
        public event EventHandler<DynamicTabEventArgs> TabSelected;
        public event EventHandler<DynamicTabEventArgs> TabAdded;
        public event EventHandler<DynamicTabEventArgs> TabRemoved;
        public event EventHandler<DynamicTabEventArgs> TabUpdated;
        public event EventHandler<DynamicTabEventArgs> TabButtonClicked;

        // Header and Content Panels
        [Browsable(true)]
        [Localizable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public FlowLayoutPanel HeaderPanel { get; } = new FlowLayoutPanel
        {
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight,
            BackColor = SystemColors.Control,
            WrapContents = false,
            Dock = DockStyle.Top
        };

        [Browsable(true)]
        [Localizable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Panel ContentPanel { get; } = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White
        };

        public BeepDynamicTabControl()
        {
            AllowDrop = true; // Enable drag-and-drop for the control itself
            ShowTitle = false;

            // Attach drag-and-drop event handlers
            this.DragEnter += BeepDynamicTabControl_DragEnter;
            this.DragDrop += BeepDynamicTabControl_DragDrop;

            // Initialize HeaderPanel and ContentPanel
            if (!Controls.Contains(HeaderPanel))
            {
                Controls.Add(HeaderPanel);
            }

            if (!Controls.Contains(ContentPanel))
            {
                Controls.Add(ContentPanel);
                ContentPanel.BringToFront();
            }

            // Initialize Tabs
            _tabs = new BindingList<SimpleItem>();
            _tabs.ListChanged += OnTabsChanged;
        }

        #region Properties

        [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public BindingList<SimpleItem> Tabs
        {
            get => _tabs;
            set
            {
               // Debug.WriteLine("Set Tabs");
                if (_tabs != null)
                {
                    _tabs.ListChanged -= OnTabsChanged;
                }

                _tabs = value ?? new BindingList<SimpleItem>();
                _tabs.ListChanged += OnTabsChanged;
                SynchronizeTabs();
               // Debug.WriteLine("Tabs synchronized.");
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [TypeConverter(typeof(TabItemConverter))]
        public SimpleItem SelectedTab
        {
            get => _selectedTab;
            set
            {
                if (_selectedTab != value)
                {
                    _selectedTab = value;
                    UpdateSelectedTab();
                    // Raise the TabSelected event
                    TabSelected?.Invoke(this, new DynamicTabEventArgs(GetSelectedPanel(), SelectedIndex));
                }
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int SelectedIndex => _tabs?.IndexOf(_selectedTab) ?? -1;

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
                    UpdateLayout();
                }
            }
        }

        #endregion

        #region Designer Support

        // Ensure proper serialization and designer support

        #endregion

        #region Tab Management

        private void OnTabsChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    AddTab(_tabs[e.NewIndex]);
                    TabAdded?.Invoke(this, new DynamicTabEventArgs(Panels[_tabs[e.NewIndex].GuidId], e.NewIndex));
                    break;

                case ListChangedType.ItemDeleted:
                    RemoveTabByIndex(e.NewIndex);
                    break;

                case ListChangedType.ItemChanged:
                    UpdateTab(_tabs[e.NewIndex]);
                    TabUpdated?.Invoke(this, new DynamicTabEventArgs(Panels[_tabs[e.NewIndex].GuidId], e.NewIndex));
                    break;

                case ListChangedType.Reset:
                    SynchronizeTabs();
                    break;
            }
        }

        private void SynchronizeTabs()
        {
           // Debug.WriteLine("SynchronizeTabs");
            if (_tabs == null) return;

            ClearTabs();

            for (int i = 0; i < _tabs.Count; i++)
            {
               // Debug.WriteLine($"SynchronizeTabs: Adding tab {_tabs[i].Text}");
                AddTab(_tabs[i]);
                TabAdded?.Invoke(this, new DynamicTabEventArgs(Panels[_tabs[i].GuidId], i));
            }
        }

        private void ClearTabs()
        {
            HeaderPanel.Controls.Clear();
            ContentPanel.Controls.Clear();
            Panels.Clear();
        }

        private void AddTab(SimpleItem item)
        {
           // Debug.WriteLine($"AddTab: {item.Text}");
            if (Panels.ContainsKey(item.GuidId)) return;

            // Create content panel for the tab
            count++;
            var panel = new Panel
            {
                Name = $"Panel{count}",
                Dock = DockStyle.Fill,
                Visible = false,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                AllowDrop = true // Enable drag-and-drop
            };

           // Debug.WriteLine($"AddTab: Creating drag-and-drop handlers for {panel.Name}");
            // Attach drag-and-drop event handlers
            panel.DragEnter += Panel_DragEnter;
            panel.DragDrop += Panel_DragDrop;

           // Debug.WriteLine($"AddTab: Adding panel {panel.Name} to ContentPanel");
            AddControlToContentPanel(ContentPanel, panel);

           // Debug.WriteLine($"AddTab: Creating button for tab {item.Text}");
            // Create header button for the tab
            var button = CreateTabButton(item, panel);
           // Debug.WriteLine($"AddTab: Adding button {button.Name} to HeaderPanel");
            AddControlToContentPanel(HeaderPanel, button);

            // Map the tab's GuidId to its Panel
            Panels[item.GuidId] = panel;
           // Debug.WriteLine($"AddTab: Added panel {panel.Name} with GuidId {item.GuidId}");

            // Automatically select the first tab
            if (Panels.Count == 1)
            {
                SelectedTab = item;
            }
        }

        private void RemoveTabByIndex(int index)
        {
            if (index < 0 || index >= _tabs.Count) return;

            var item = _tabs[index];
            RemoveTab(item);
            TabRemoved?.Invoke(this, new DynamicTabEventArgs(Panels[item.GuidId], index));
        }

        private void RemoveTab(SimpleItem item)
        {
            if (!Panels.TryGetValue(item.GuidId, out var panel)) return;

            // Remove header button
            var button = HeaderPanel.Controls.OfType<Button>().FirstOrDefault(b => Equals(b.Tag, item));
            if (button != null)
            {
                HeaderPanel.Controls.Remove(button);
            }

            // Remove panel from ContentPanel
            RemoveControlFromContentPanel(ContentPanel, panel);

            // Remove from Panels dictionary
            Panels.Remove(item.GuidId);

            // Update selected tab if necessary
            if (_selectedTab == item)
            {
                SelectedTab = _tabs.FirstOrDefault();
            }

           // Debug.WriteLine($"RemoveTab: Removed tab {item.Text} and panel {panel.Name}");
        }

        private void UpdateTab(SimpleItem item)
        {
            if (!Panels.TryGetValue(item.GuidId, out var panel)) return;

            // Update header button text
            var button = HeaderPanel.Controls.OfType<Button>().FirstOrDefault(b => Equals(b.Tag, item));
            if (button != null)
            {
                button.Text = item.Text;
               // Debug.WriteLine($"UpdateTab: Updated button text to {item.Text}");
            }
        }

        #endregion

        #region Button Creation

        private BeepButton CreateTabButton(SimpleItem item, Panel panel)
        {
           // Debug.WriteLine($"CreateTabButton: Creating button for {item.Name}");
            var button = new BeepButton
            {
                Name = $"HeaderButton{count}",
                Text = string.IsNullOrEmpty(item.Text) ? Name : item.Text,
                AutoSize = true,
                Margin = new Padding(2),
                Tag = item,
                SelectedItem = item,
                GuidID = item.GuidId
            };
           // Debug.WriteLine($"CreateTabButton: Created button {button.Name}");

            // Drag-and-Drop for reordering tabs
            button.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    DoDragDrop(button, DragDropEffects.Move);
                }
            };

            button.DragEnter += (s, e) =>
            {
                if (e.Data.GetData(typeof(Button)) is Button)
                {
                    e.Effect = DragDropEffects.Move;
                }
            };

            button.DragDrop += (s, e) =>
            {
                if (e.Data.GetData(typeof(Button)) is Button draggedButton)
                {
                    var sourceIndex = HeaderPanel.Controls.GetChildIndex(draggedButton);
                    var targetIndex = HeaderPanel.Controls.GetChildIndex(button);

                    // Swap buttons
                    HeaderPanel.Controls.SetChildIndex(draggedButton, targetIndex);
                    HeaderPanel.Controls.SetChildIndex(button, sourceIndex);

                    // Update BindingList
                    var temp = _tabs[sourceIndex];
                    _tabs[sourceIndex] = _tabs[targetIndex];
                    _tabs[targetIndex] = temp;

                   // Debug.WriteLine($"CreateTabButton: Swapped tabs at index {sourceIndex} and {targetIndex}");
                }
            };

            // Click event to select the tab
            button.Click += (s, e) =>
            {
                BeepButton btn = (BeepButton)s;
                TabButtonClicked?.Invoke(this, new DynamicTabEventArgs(panel, _tabs.IndexOf(btn.SelectedItem)));
                SelectedTab = btn.SelectedItem;
                TabSelected?.Invoke(this, new DynamicTabEventArgs(panel, _tabs.IndexOf(btn.SelectedItem)));
               // Debug.WriteLine($"CreateTabButton: Tab {item.Text} clicked and selected.");
            };

            return button;
        }

        #endregion

        #region Selected Tab Handling

        public void UpdateSelectedTab()
        {
            if (_selectedTab == null)
            {
               // Debug.WriteLine("UpdateSelectedTab: SelectedTab is null.");
                return;
            }

           // Debug.WriteLine($"UpdateSelectedTab: SelectedTab = {_selectedTab.Text}");

            foreach (Control control in ContentPanel.Controls)
            {
                if (control is Panel panel)
                {
                    bool isSelected = Panels.TryGetValue(_selectedTab.GuidId, out var selectedPanel) &&
                                       selectedPanel == panel;
                    panel.Visible = isSelected;
                    if (isSelected)
                    {
                        panel.BringToFront();
                       // Debug.WriteLine($"UpdateSelectedTab: Panel {panel.Name} is now visible.");
                    }
                    else
                    {
                        panel.SendToBack();
                       // Debug.WriteLine($"UpdateSelectedTab: Panel {panel.Name} is now hidden.");
                    }
                }
            }

            HighlightButtonAt(_selectedTab.GuidId);
        }

        private Panel GetSelectedPanel()
        {
            if (_selectedTab != null && Panels.TryGetValue(_selectedTab.GuidId, out var panel))
            {
                return panel;
            }
            return null;
        }

        #endregion

        #region UI Enhancements

        private void HighlightButtonAt(string itemguidid)
        {
          //  Debug.WriteLine($"Expected GUID: '{itemguidid}'");

            for (int i = 0; i < HeaderPanel.Controls.Count; i++)
            {
                if (HeaderPanel.Controls[i] is BeepButton btn)
                {
            //        Debug.WriteLine($"Checking Button: {btn.Name} | Button GUID: '{btn.GuidID}'");

                    bool match = string.Equals(btn.GuidID?.Trim(), itemguidid?.Trim(), StringComparison.OrdinalIgnoreCase);
                    btn.IsSelected = match;

              //      Debug.WriteLine($"HighlightButtonAt: Button {btn.Name} is selected: {btn.IsSelected}");
                }
            }
        }


        private void UpdateLayout()
        {
            switch (_headerLocation)
            {
                case HeaderLocation.Top:
                    HeaderPanel.Dock = DockStyle.Top;
                    HeaderPanel.FlowDirection = FlowDirection.LeftToRight;
                    break;

                case HeaderLocation.Bottom:
                    HeaderPanel.Dock = DockStyle.Bottom;
                    HeaderPanel.FlowDirection = FlowDirection.LeftToRight;
                    break;

                case HeaderLocation.Left:
                    HeaderPanel.Dock = DockStyle.Left;
                    HeaderPanel.FlowDirection = FlowDirection.TopDown;
                    break;

                case HeaderLocation.Right:
                    HeaderPanel.Dock = DockStyle.Right;
                    HeaderPanel.FlowDirection = FlowDirection.TopDown;
                    break;
            }
        }

        #endregion

        #region Drag and Drop

        // Drag and Drop should go to Selected Panel and if no panel is selected it should go to the last panel. If no panel exists, stop drop
        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);
            BeepDynamicTabControl_DragEnter(this, e);
        }

        protected override void OnDragDrop(DragEventArgs e)
        {
            base.OnDragDrop(e);
            BeepDynamicTabControl_DragDrop(this, e);
        }

        private void BeepDynamicTabControl_DragEnter(object sender, DragEventArgs e)
        {
            if (IsInDesignMode())
            {
                // Let the designer handle drag-enter
                return;
            }

            // Runtime drag-enter logic
            if (e.Data.GetDataPresent(typeof(Control)))
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void BeepDynamicTabControl_DragDrop(object sender, DragEventArgs e)
        {
            if (IsInDesignMode())
            {
                // Let the designer handle drag-drop
                return;
            }

            // Runtime drag-drop logic
            if (e.Data.GetData(typeof(Control)) is Control control)
            {
                Panel targetPanel = SelectedTab != null && Panels.ContainsKey(SelectedTab.GuidId)
                    ? Panels[SelectedTab.GuidId]
                    : (Panels.Values.Any() ? Panels.Values.Last() : null);

                if (targetPanel != null)
                {
                    targetPanel.Controls.Add(control);
                    control.Location = targetPanel.PointToClient(new Point(e.X, e.Y));
                    control.BringToFront();
                   // Debug.WriteLine($"DragDrop: Control {control.Name} added to {targetPanel.Name} at location {control.Location}");
                }
            }
        }

        private void Panel_DragEnter(object sender, DragEventArgs e)
        {
            if (IsInDesignMode())
            {
                // Let the designer handle drag-enter
                return;
            }

            // Allow drop only if the data is a control
            if (e.Data.GetDataPresent(typeof(Control)) ||
                e.Data.GetDataPresent("System.Windows.Forms.Design.ToolboxItem", false))
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void Panel_DragDrop(object sender, DragEventArgs e)
        {
            if (IsInDesignMode())
            {
                // Let the designer handle drag-drop
                return;
            }

            // Runtime drag-drop logic for individual panels
            if (e.Data.GetData(typeof(Control)) is Control control)
            {
                Panel targetPanel = sender as Panel;

                if (targetPanel != null)
                {
                    targetPanel.Controls.Add(control);
                    control.Location = targetPanel.PointToClient(new Point(e.X, e.Y));
                    control.BringToFront();
                   // Debug.WriteLine($"Panel_DragDrop: Control {control.Name} added to {targetPanel.Name} at location {control.Location}");
                }
            }
        }

        #endregion

        #region Helper Methods

        private void AddControlToContentPanel(Control parent, Control child)
        {
            if (!parent.Controls.Contains(child))
            {
                parent.Controls.Add(child);
            }
        }

        private void RemoveControlFromContentPanel(Control parent, Control child)
        {
            if (parent.Controls.Contains(child))
            {
                parent.Controls.Remove(child);
                child.Dispose();
            }
        }

        #endregion

        #region Design-Time Support

        // The designer class handles design-time features. Ensure it's properly implemented.

        #endregion

        #region Design Mode Check

        public bool IsInDesignMode()
        {
            return LicenseManager.UsageMode == LicenseUsageMode.Designtime ||
                   (Site != null && Site.DesignMode);
        }

        #endregion

        #region Disposal

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose child controls
                foreach (var control in Controls.OfType<Control>().ToList())
                {
                    if (control is IDisposable disposableControl)
                    {
                        disposableControl.Dispose();
                    }

                    Controls.Remove(control);
                }

                RemoveAllControlsFromContentPanel(ContentPanel);
                RemoveAllControlsFromContentPanel(HeaderPanel);

                // Clear Panels
                Panels.Clear();
            }

            base.Dispose(disposing);
        }

        private void RemoveAllControlsFromContentPanel(Control parent)
        {
            foreach (Control child in parent.Controls.OfType<Control>().ToList())
            {
                parent.Controls.Remove(child);
                child.Dispose();
            }
        }

        #endregion
    }

    // Supporting Classes

    public class DynamicTabEventArgs : EventArgs
    {
        public DynamicTabEventArgs(Panel tabPage, int tabIndex)
        {
            TabPage = tabPage;
            TabIndex = tabIndex;
        }

        public Panel TabPage { get; }
        public int TabIndex { get; }
    }
}
