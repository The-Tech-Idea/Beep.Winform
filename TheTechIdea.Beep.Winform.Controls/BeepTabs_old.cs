using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Converters;
using TheTechIdea.Beep.Winform.Controls.Design;
using TheTechIdea.Beep.Winform.Controls.Editors;

namespace TheTechIdea.Beep.Winform.Controls
{
    //[Designer(typeof(BeepTabsDesigner))]
    public class BeepTabs_old : BeepControl
    {
        public FlowLayoutPanel _headerPanel { get; set; }
        public BeepPanel HeaderPanel { get; set; }

        private int _buttonheight = 20;
        private bool IsDrawmenuMode = false;

        private BindingList<BeepPanel> _tabPanels = new BindingList<BeepPanel>();
        [Browsable(true)]
        public BindingList<BeepPanel> TabPanels => _tabPanels;

        private SimpleItemCollection _menuitems = new SimpleItemCollection();
        [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
        [Editor(typeof(MenuItemCollectionEditor), typeof(UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SimpleItemCollection TabPages
        {
            get => _menuitems;
            set => _menuitems = value;
        }

        private BeepPanel _selectedTab;
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [TypeConverter(typeof(BeepPanelConverter))]
        public BeepPanel SelectedTab
        {
            get => _selectedTab;
            set
            {
                if (value != null && _tabPanels.Contains(value))
                {
                    _selectedTab = value;
                    SelectTab(value);
                }
            }
        }

        public event EventHandler<ContainerEvents> AddinAdded;
        public event EventHandler<ContainerEvents> AddinRemoved;
        public event EventHandler<ContainerEvents> AddinMoved;
        public event EventHandler<ContainerEvents> AddinChanging;
        public event EventHandler<ContainerEvents> AddinChanged;
        public event EventHandler<IPassedArgs> PreCallModule;
        public event EventHandler<IPassedArgs> PreShowItem;
        public event EventHandler<KeyCombination> KeyPressed;

        public int Topy = 30;

        private BindingList<BeepButton> _tabbuttons = new BindingList<BeepButton>();

        private BeepPanel _selectedPanel;

        public BeepTabs_old()
        {
            this.ShowAllBorders = true;
            this.ShowShadow = false;

            _menuitems.ListChanged += MenuItems_ListChanged;

            if (Width <= 0 || Height <= 0)
            {
                Width = 200;
                Height = 200;
            }

            ApplyThemeToChilds = false;
            this.DragDrop += Panel_DragDrop;
            this.DragEnter += Panel_DragEnter;
            this.DragOver += Panel_DragOver;
            InitTabs();
        }

        // No OnControlAdded override needed for tab synchronization.
        // Panels are created and destroyed based on TabPages changes, not by adding controls directly.

        private void AddControlToPanel(Control control, BeepPanel panel, Point dropPoint)
        {
            if (panel == null || control == null) return;

            try
            {
                var host = (IDesignerHost)GetService(typeof(IDesignerHost));
                var changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));

                if (DesignMode && host != null && changeService != null)
                {
                    using (var transaction = host.CreateTransaction("Add Control to BeepPanel"))
                    {
                        try
                        {
                            changeService.OnComponentChanging(panel.Controls, null);
                            var createdControl = (Control)host.CreateComponent(control.GetType());
                            if (createdControl != null)
                            {
                                createdControl.Location = panel.PointToClient(dropPoint);
                                createdControl.Name = GetUniqueControlName(panel, createdControl.GetType());
                                panel.Controls.Add(createdControl);
                                changeService.OnComponentChanged(panel.Controls, null, null, null);
                                Console.WriteLine($"Control added to {panel.TitleText} at {createdControl.Location}.");
                            }
                            else
                            {
                                Console.WriteLine("Error: Failed to create control via DesignerHost.");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error during transaction: {ex.Message}");
                        }
                        finally
                        {
                            transaction.Commit();
                        }
                    }
                }
                else
                {
                    panel.Controls.Add(control);
                    control.Location = panel.PointToClient(dropPoint);
                    Console.WriteLine($"Control added to {panel.TitleText} at {control.Location}.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding control: {ex.Message}");
            }
        }

        private string GetUniqueControlName(Control parent, Type controlType)
        {
            string baseName = controlType.Name.ToLower();
            int index = 1;

            while (parent.Controls.Find($"{baseName}{index}", true).Length > 0)
            {
                index++;
            }

            return $"{baseName}{index}";
        }

        private void InitTabs()
        {
            IsDrawmenuMode = true;
            try
            {
                if (HeaderPanel == null)
                {
                    HeaderPanel = new BeepPanel
                    {
                        Height = Topy,
                        Location = new Point(0, 0),
                        Width = Width,
                        Theme = this.Theme,
                        Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
                    };
                    Controls.Add(HeaderPanel);
                }

                if (_headerPanel == null)
                {
                    _headerPanel = CreateFlowLayoutPanel();
                    HeaderPanel.Controls.Add(_headerPanel);
                }

                // Do NOT create or remove panels here. Just redraw headers.
                RedrawHeaderButtons();
            }
            finally
            {
                IsDrawmenuMode = false;
            }
        }

        private BeepPanel AddPanelForMenuItem(SimpleItem item)
        {
            if (item == null) return null;
            // Ensure item.ReferenceID is set
            if (string.IsNullOrEmpty(item.ReferenceID))
            {
                item.ReferenceID = Guid.NewGuid().ToString();
            }

            var existingPanel = _tabPanels.FirstOrDefault(p => p.Tag?.ToString() == item.ReferenceID);
            if (existingPanel != null)
            {
                // Panel already exists
                Console.WriteLine($"Panel already exists for item: {item.Text}");
                return existingPanel;
            }

            BeepPanel panel = null;
            var host = (IDesignerHost)GetService(typeof(IDesignerHost));
            var changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));

            if (DesignMode && host != null && changeService != null)
            {
                using (var transaction = host.CreateTransaction("Add BeepPanel"))
                {
                    try
                    {
                        changeService.OnComponentChanging(this, null);
                        panel = (BeepPanel)host.CreateComponent(typeof(BeepPanel));
                        panel.TitleText = string.IsNullOrEmpty(item.Text) ? "Tab" : item.Text;
                        panel.Name = GetUniqueControlName(this, typeof(BeepPanel));
                        panel.Visible = false;
                        panel.Dock = DockStyle.Fill;
                        panel.ShowTitle = false;
                        panel.ShowTitleLine = false;
                        panel.ShowAllBorders = true;
                        panel.ShowShadow = false;
                        panel.AllowDrop = true;
                        panel.IsRounded = false;
                        panel.IsBorderAffectedByTheme = false;
                        panel.IsShadowAffectedByTheme = false;
                        panel.Size = new Size(DrawingRect.Width, DrawingRect.Height - _headerPanel.Height - 1);
                        panel.Location = new Point(DrawingRect.X, DrawingRect.Top + _headerPanel.Height + 1);
                        panel.Theme = Theme;
                        panel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
                        panel.DragEnter += Panel_DragEnter;
                        panel.DragOver += Panel_DragOver;
                        panel.DragDrop += Panel_DragDrop;

                        // Link panel to item via ReferenceID
                        panel.Tag = item.ReferenceID;

                        _tabPanels.Add(panel);
                        Controls.Add(panel);

                        changeService.OnComponentChanged(this, null, null, null);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error adding panel: {ex.Message}");
                    }
                    finally
                    {
                        transaction.Commit();
                    }
                }
            }
            else
            {
                // Runtime
                panel = new BeepPanel
                {
                    TitleText = string.IsNullOrEmpty(item.Text) ? "Tab" : item.Text,
                    Visible = false,
                    Dock = DockStyle.Fill,
                    ShowTitle = false,
                    ShowTitleLine = false,
                    ShowAllBorders = true,
                    ShowShadow = false,
                    AllowDrop = true,
                    IsRounded = false,
                    IsBorderAffectedByTheme = false,
                    IsShadowAffectedByTheme = false,
                    Size = new Size(DrawingRect.Width, DrawingRect.Height - _headerPanel.Height - 1),
                    Location = new Point(DrawingRect.X, DrawingRect.Top + _headerPanel.Height + 1),
                    Theme = Theme,
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
                };

                panel.DragEnter += Panel_DragEnter;
                panel.DragOver += Panel_DragOver;
                panel.DragDrop += Panel_DragDrop;

                panel.Tag = item.ReferenceID;
                _tabPanels.Add(panel);
                Controls.Add(panel);
            }

            // Notify Designer about TabPages property changing
            if (DesignMode && changeService != null)
            {
                PropertyDescriptor tabPagesProperty = TypeDescriptor.GetProperties(this)["TabPages"];
                changeService.OnComponentChanging(this, tabPagesProperty);
                changeService.OnComponentChanged(this, tabPagesProperty, null, _menuitems);
            }

            Console.WriteLine($"Added panel for item: {item.Text}");
            return panel;
        }

        private void RemovePanelForMenuItem(int index)
        {
            if (index < 0 || index >= _tabPanels.Count || index >= _menuitems.Count) return;

            var itemToRemove = _menuitems[index];
            var panel = _tabPanels.FirstOrDefault(p => p.Tag?.ToString() == itemToRemove.ReferenceID);
            if (panel == null) return;

            Console.WriteLine($"Removing panel: {panel.TitleText}");

            var host = (IDesignerHost)GetService(typeof(IDesignerHost));
            var changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));

            try
            {
                if (DesignMode && host != null && changeService != null)
                {
                    using (var transaction = host.CreateTransaction("Remove BeepPanel and MenuItem"))
                    {
                        PropertyDescriptor controlsProperty = TypeDescriptor.GetProperties(this)["Controls"];
                        changeService.OnComponentChanging(this, controlsProperty);

                        PropertyDescriptor tabPagesProperty = TypeDescriptor.GetProperties(this)["TabPages"];
                        changeService.OnComponentChanging(this, tabPagesProperty);

                        Controls.Remove(panel);
                        _tabPanels.Remove(panel);
                        _menuitems.Remove(itemToRemove);

                        host.DestroyComponent(panel);

                        changeService.OnComponentChanged(this, controlsProperty, null, Controls);
                        changeService.OnComponentChanged(this, tabPagesProperty, null, _menuitems);

                        Console.WriteLine("Panel and menu item removed, and Designer.cs updated.");
                        transaction.Commit();
                    }
                }
                else
                {
                    // Runtime
                    Controls.Remove(panel);
                    _tabPanels.Remove(panel);
                    _menuitems.Remove(itemToRemove);
                    Console.WriteLine($"Removed panel and menu item at runtime: {panel.TitleText}, {itemToRemove.Text}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing panel and menu item: {ex.Message}");
            }
        }

        private void UpdateItemProperties(int index)
        {
            if (index < 0 || index >= _menuitems.Count || index >= _tabPanels.Count)
                return;

            var item = _menuitems[index];
            var panel = _tabPanels[index];

            try
            {
                // Update visual properties only
                panel.TitleText = item.Text;
                panel.Name = item.Text;
                // Do not change item.ReferenceID or panel.Tag here.
                panel.Theme = Theme;

                if (DesignMode)
                {
                    var changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
                    changeService?.OnComponentChanging(panel, null);
                    changeService?.OnComponentChanged(panel, null, null, null);
                    changeService?.OnComponentChanging(_menuitems, null);
                    changeService?.OnComponentChanged(_menuitems, null, null, null);
                }

                Console.WriteLine($"Updated item and panel at index {index}: {item.Text}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating item and panel: {ex.Message}");
            }
        }

        private void HandleItemChanged(int index)
        {
            UpdateItemProperties(index);
            RedrawHeaderButtons();
        }

        private void HandleItemDeleted(int index)
        {
            if (index >= 0 && index < _menuitems.Count)
            {
                IsDrawmenuMode = true;
                RemovePanelForMenuItem(index);
                IsDrawmenuMode = false;
                Console.WriteLine($"Removed tab at index {index}");
                RedrawHeaderButtons();
            }
        }

        private void HandleItemAdded(int index)
        {
            if (index >= 0 && index < _menuitems.Count)
            {
                IsDrawmenuMode = true;
                var newItem = _menuitems[index];
                // Ensure ReferenceID is stable and set once
                if (string.IsNullOrEmpty(newItem.ReferenceID))
                {
                    newItem.ReferenceID = Guid.NewGuid().ToString();
                }

                // If panel already exists for this ReferenceID, skip
                if (!_tabPanels.Any(p => p.Tag?.ToString() == newItem.ReferenceID))
                {
                    AddPanelForMenuItem(newItem);
                }
                IsDrawmenuMode = false;
                Console.WriteLine($"Added new tab: {newItem.Text}");
                RedrawHeaderButtons();
            }
        }

        private void MenuItems_ListChanged(object sender, ListChangedEventArgs e)
        {
            IsDrawmenuMode = true;
            try
            {
                switch (e.ListChangedType)
                {
                    case ListChangedType.ItemAdded:
                        HandleItemAdded(e.NewIndex);
                        break;

                    case ListChangedType.ItemDeleted:
                        HandleItemDeleted(e.OldIndex);
                        break;

                    case ListChangedType.ItemChanged:
                        HandleItemChanged(e.NewIndex);
                        break;

                    case ListChangedType.Reset:
                        // Redraw headers if reset
                        SynchronizePanelsWithTabPages();
                        RedrawHeaderButtons();
                        break;

                    default:
                        Console.WriteLine($"Unhandled ListChangedType: {e.ListChangedType}");
                        break;
                }
            }
            finally
            {
                IsDrawmenuMode = false;
            }
        }

        protected FlowLayoutPanel CreateFlowLayoutPanel()
        {
            var x = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                Dock = DockStyle.Fill,
                BackColor = _currentTheme.BackgroundColor,
                ForeColor = _currentTheme.LatestForColor
            };
            return x;
        }

        protected BeepButton CreateButton(SimpleItem item, string referenceID)
        {
            var tabButton = new BeepButton
            {
                Text = item.Text,
                Margin = new Padding(1, 0, 1, 0),
                Tag = item,
                ShowAllBorders = true,
                ShowShadow = false,
                Height = _buttonheight,
                OverrideFontSize = TypeStyleFontSize.Small,
                ImageAlign = ContentAlignment.MiddleRight,
                TextAlign = ContentAlignment.MiddleLeft,
                MaxImageSize = new Size(10, 10),
                SavedGuidID = referenceID, // Store ReferenceID here
                Theme = Theme
            };

            Size textSize = TextRenderer.MeasureText(item.Text, tabButton.Font);
            tabButton.Size = textSize;
            tabButton.MaxImageSize = new Size(16, 16);
            tabButton.Click += TabButton_Click;
            return tabButton;
        }

        public bool RemoveControlByGuidTag(string guidid)
        {
            var panelToRemove = _tabPanels.ToList().Find(p => p.Tag?.ToString() == guidid);
            if (panelToRemove != null)
            {
                Controls.Remove(panelToRemove);
                _tabPanels.Remove(panelToRemove);
                UpdateTabHeaders();
                AddinRemoved?.Invoke(this, new ContainerEvents { Guidid = guidid });
                return true;
            }
            return false;
        }

        public bool RemoveControlByName(string name)
        {
            var panelToRemove = _tabPanels.ToList().Find(p => p.Text == name);
            if (panelToRemove != null)
            {
                Controls.Remove(panelToRemove);
                _tabPanels.Remove(panelToRemove);
                UpdateTabHeaders();
                AddinRemoved?.Invoke(this, new ContainerEvents { TitleText = name });
                return true;
            }
            return false;
        }

        public bool ShowControl(string title, IDM_Addin control)
        {
            var panelToShow = _tabPanels.ToList().Find(p => p.Controls.Contains((Control)control));
            if (panelToShow != null)
            {
                SelectTab(panelToShow);
                AddinAdded?.Invoke(this, new ContainerEvents { TitleText = title, Control = control, Guidid = panelToShow.Tag.ToString() });
                return true;
            }
            return false;
        }

        public bool IsControlExit(IDM_Addin control)
        {
            foreach (BeepPanel panel in _tabPanels)
            {
                if (panel.Controls.Contains((Control)control))
                    return true;
            }
            return false;
        }

        public void Clear()
        {
            _tabPanels.Clear();
            Controls.Clear();
            Controls.Add(_headerPanel);
            AddinRemoved?.Invoke(this, null);
        }

        private void UpdateTabHeaders()
        {
            _headerPanel.Controls.Clear();
            _headerPanel.FlowDirection = FlowDirection.LeftToRight;
            _headerPanel.WrapContents = true;

            if (_menuitems != null)
            {
                foreach (var item in _menuitems)
                {
                    var referenceID = item.ReferenceID;
                    // Create button for each tab
                    _headerPanel.Controls.Add(CreateButton(item, referenceID));
                    AddinAdded?.Invoke(this, new ContainerEvents { TitleText = item.Text, Control = null, Guidid = referenceID });
                }
            }
        }

        private void RedrawHeaderButtons()
        {
            _headerPanel.Controls.Clear();
            _tabbuttons.Clear();

            foreach (var item in _menuitems)
            {
                var panel = _tabPanels.FirstOrDefault(p => p.Tag?.ToString() == item.ReferenceID);
                if (panel != null)
                {
                    var button = CreateButton(item, item.ReferenceID);
                    button.Click += TabButton_Click;
                    _tabbuttons.Add(button);
                    _headerPanel.Controls.Add(button);
                }
            }
        }

        private void CloseClicked(object sender, BeepEventDataArgs e)
        {
            if (sender is BeepButton button && button.SavedGuidID is string refID)
            {
                // Implement close logic if needed
            }
        }

        private void TabButton_Click(object sender, EventArgs e)
        {
            if (sender is BeepButton button && button.SavedGuidID is string refID)
                SelectTab(refID);
        }

        public void SelectTab(string referenceID)
        {
            BeepPanel matchedPanel = null;
            foreach (BeepPanel panel in _tabPanels)
            {
                bool isSelected = (panel.Tag?.ToString() == referenceID);
                panel.Visible = isSelected;
                if (isSelected)
                {
                    panel.BringToFront();
                    matchedPanel = panel;
                }
            }

            if (matchedPanel != null)
            {
                _selectedPanel = matchedPanel;
                _selectedTab = matchedPanel;
                HilightButtonAfterSelectTab(matchedPanel);
            }
        }

        public void SelectTab(BeepPanel selectedPanel)
        {
            if (selectedPanel == null) return;

            foreach (BeepPanel panel in _tabPanels)
            {
                panel.Visible = (panel == selectedPanel);
                if (panel.Visible)
                    panel.BringToFront();
            }

            HilightButtonAfterSelectTab(selectedPanel);
            _selectedTab = selectedPanel;

            if (DesignMode)
            {
                var designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
                var changeService = designerHost?.Container?.Components
                    .OfType<IComponentChangeService>()
                    .FirstOrDefault();
                changeService?.OnComponentChanged(_selectedTab, null, null, null);
            }
        }

        public void SelectTab(int index)
        {
            if (index >= 0 && index < _tabPanels.Count)
            {
                SelectTab(_tabPanels[index]);
            }
        }

        private void HilightButtonAfterSelectTab(BeepPanel panel)
        {
            foreach (BeepButton button in _headerPanel.Controls)
            {
                button.IsHovered = (button.SavedGuidID == panel.Tag?.ToString());
            }
        }

        public IErrorsInfo PressKey(KeyCombination keyCombination)
        {
            KeyPressed?.Invoke(this, keyCombination);
            return null;
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);

            if (DesignMode && Parent is BeepTabs_old)
            {
                Console.WriteLine("Parent changed: Reparenting control.");
                if (_selectedTab != null)
                {
                    _selectedTab.Controls.Add(this);
                }
            }
        }

        public void Add(BeepPanel panel)
        {
            panel.Size = new Size(Width - (BorderRadius * 2) - 10, Height - _headerPanel.Height - (BorderRadius * 2) - 10);
            panel.Location = new Point(BorderRadius + 10, _headerPanel.Height + BorderRadius + 10);
            panel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            Controls.Add(panel);
            panel.StaticNotMoving = true;
            UpdateTabHeaders();
            Invalidate();
        }

        public override void ApplyTheme()
        {
            if (_headerPanel == null) return;

            _headerPanel.BackColor = _currentTheme.BackgroundColor;
            _headerPanel.ForeColor = _currentTheme.ButtonForeColor;
            HeaderPanel.Theme = Theme;

            try
            {
                for (int i = 0; i < _headerPanel.Controls.Count; i++)
                {
                    if (_headerPanel.Controls[i] is BeepButton btn)
                    {
                        btn.Theme = Theme;
                        btn.Size = btn.GetSuitableSize();
                    }
                }
            }
            catch (Exception)
            {
                // Handle theme application exceptions
            }

            foreach (BeepPanel panel in _tabPanels)
            {
                panel.Theme = Theme;
            }
        }

        #region "Drag and Drop"
        private void Panel_DragEnter(object sender, DragEventArgs e)
        {
            base.OnDragEnter(e);
            Console.WriteLine("Drag Enter");
            if (e.Data.GetDataPresent(typeof(BeepPanel)))
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void Panel_DragOver(object sender, DragEventArgs e)
        {
            base.OnDragOver(e);
            if (e.Data.GetDataPresent(typeof(BeepPanel)))
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            foreach (Control control in _headerPanel.Controls)
            {
                if (control is BeepButton button && button.Bounds.Contains(e.Location))
                {
                    SelectTab(button.SavedGuidID);
                    break;
                }
            }
            if (DesignMode)
            {
                var hitControl = this.GetChildAtPoint(e.Location);
                if (hitControl is BeepPanel panel)
                {
                    DoDragDrop(panel, DragDropEffects.Move);
                }
            }
        }

        private void Panel_DragDrop(object sender, DragEventArgs e)
        {
            if (sender is BeepPanel panel && e.Data.GetDataPresent(typeof(ToolboxItem)))
            {
                var toolboxItem = (ToolboxItem)e.Data.GetData(typeof(ToolboxItem));
                var host = (IDesignerHost)GetService(typeof(IDesignerHost));

                if (host != null)
                {
                    var transaction = host.CreateTransaction("Add Control to Tab");
                    try
                    {
                        var control = (Control)toolboxItem.CreateComponents(host)[0];
                        Point dropPoint = new Point(e.X, e.Y);
                        AddControlToPanel(control, panel, dropPoint);
                    }
                    finally
                    {
                        transaction.Commit();
                    }
                }
            }
        }
        private void SynchronizePanelsWithTabPages()
        {
            var host = (IDesignerHost)GetService(typeof(IDesignerHost));
            var changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));

            // Find panels that are no longer represented in _menuitems
            var panelsToRemove = _tabPanels.Where(p => !_menuitems.Any(i => i.ReferenceID == p.Tag?.ToString())).ToList();

            if (panelsToRemove.Count > 0)
            {
                if (DesignMode && host != null && changeService != null)
                {
                    using (var transaction = host.CreateTransaction("Sync Panels with TabPages"))
                    {
                        PropertyDescriptor controlsProperty = TypeDescriptor.GetProperties(this)["Controls"];
                        PropertyDescriptor tabPagesProperty = TypeDescriptor.GetProperties(this)["TabPages"];

                        changeService.OnComponentChanging(this, controlsProperty);
                        changeService.OnComponentChanging(this, tabPagesProperty);

                        foreach (var panel in panelsToRemove)
                        {
                            Controls.Remove(panel);
                            _tabPanels.Remove(panel);
                            host.DestroyComponent(panel);
                        }

                        // Notify the designer about the changes
                        changeService.OnComponentChanged(this, controlsProperty, null, Controls);
                        changeService.OnComponentChanged(this, tabPagesProperty, null, _menuitems);
                        transaction.Commit();
                    }
                }
                else
                {
                    // Runtime scenario
                    foreach (var panel in panelsToRemove)
                    {
                        Controls.Remove(panel);
                        _tabPanels.Remove(panel);
                    }
                }

                RedrawHeaderButtons();
            }
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            // Forward to the selected tab
            if (SelectedTab != null)
            {
                ForwardDragEnter(SelectedTab, e);
            }
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            // Forward to the selected tab
            if (SelectedTab != null)
            {
                ForwardDragOver(SelectedTab, e);
            }
        }

        protected override void OnDragDrop(DragEventArgs e)
        {
            // Forward to the selected tab
            if (SelectedTab != null)
            {
                ForwardDragDrop(SelectedTab, e);
            }
        }

        private void ForwardDragEnter(BeepPanel panel, DragEventArgs e)
        {
            if (panel.AllowDrop)
            {
                Panel_DragEnter(panel, e);
            }
        }

        private void ForwardDragOver(BeepPanel panel, DragEventArgs e)
        {
            if (panel.AllowDrop)
            {
                Panel_DragOver(panel, e);
            }
        }

        private void ForwardDragDrop(BeepPanel panel, DragEventArgs e)
        {
            if (panel.AllowDrop)
            {
                Point dropPoint = panel.PointToClient(new Point(e.X, e.Y));
                HandleDragDropOnPanel(panel, dropPoint, e);
            }
        }

        private void HandleDragDropOnPanel(BeepPanel panel, Point dropPoint, DragEventArgs e)
        {
            Console.WriteLine("Drag Drop from handle");
            if (e.Data.GetDataPresent(typeof(ToolboxItem)))
            {
                var toolboxItem = (ToolboxItem)e.Data.GetData(typeof(ToolboxItem));
                var host = (IDesignerHost)GetService(typeof(IDesignerHost));
                if (host != null)
                {
                    var transaction = host.CreateTransaction("Add Control to Tab");
                    try
                    {
                        var control = (Control)toolboxItem.CreateComponents(host)[0];
                        panel.Controls.Add(control);
                        control.Location = dropPoint;
                        Console.WriteLine($"Control added to {panel.TitleText} at {control.Location}.");
                    }
                    finally
                    {
                        transaction.Commit();
                    }
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (DesignMode)
            {
                using (var pen = new Pen(Color.Blue, 2))
                {
                    foreach (BeepPanel panel in _tabPanels)
                    {
                        if (panel == SelectedTab)
                        {
                            e.Graphics.DrawRectangle(pen, panel.Bounds);
                        }
                    }
                }
            }
        }
        #endregion
    }
}
