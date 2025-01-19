using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Desktop.Common;
using TheTechIdea.Beep.Winform.Controls.Converters;
using TheTechIdea.Beep.Winform.Controls.Design;



namespace TheTechIdea.Beep.Winform.Controls
{
   
    public class BeepDynamicTabControl : BeepPanel
    {
        static int count = 0;
        private readonly FlowLayoutPanel _headerPanel;
        public  Panel _contentPanel { get; private set; }
        private readonly Dictionary<SimpleItem, Panel> _panelMap = new();
        private BindingList<SimpleItem> _tabs = new BindingList<SimpleItem>();
        private SimpleItem _selectedTab;
        private HeaderLocation _headerLocation = HeaderLocation.Top;

        public event EventHandler<DynamicTabEventArgs> TabSelected;
        public event EventHandler<DynamicTabEventArgs> TabAdded;
        public event EventHandler<DynamicTabEventArgs> TabRemoved;
        public event EventHandler<DynamicTabEventArgs> TabUpdated;
        public event EventHandler<DynamicTabEventArgs> TabButtonClicked;

      
        [Browsable(false)]
        [Localizable(true)]
        [MergableProperty(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BindingList<Panel> TabPanels
        {
            get
            {
                return new BindingList<Panel>(_panelMap.Values.ToList());
            }
            set
            {
                if (value == null) return;
                // Clear existing tabs
                ClearTabs();
                // Add new tabs
                foreach (var panel in value)
                {
                    var item = new SimpleItem { Text = panel.Name };
                    //_panelMap[item] = panel;
                   // AddTab(item);
                }
            }
        }

        public BeepDynamicTabControl()
        {
            AllowDrop = true; // Enable drag-and-drop for the control itself
            ShowTitle = false;
            DragEnter += BeepDynamicTabControl_DragEnter;
            DragDrop += BeepDynamicTabControl_DragDrop;
            // Header panel for tab buttons
            _headerPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = SystemColors.Control,
                WrapContents = false
            };

            // Content panel for tab contents
            _contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };
           // RegisterControl(_contentPanel);
            //NotifyDesigner(this, true);
            //NotifyDesigner(_contentPanel, true);
            // Register TabPanels as a child of this control
            RegisterControlAsChildForParentControl(this, _contentPanel);
            NotifyDesigner(this, true);
            //NotifyDesigner(_contentPanel, true);
            Controls.Add(_headerPanel);
            UpdateLayout();
            _tabs = new BindingList<SimpleItem>();
            _tabs.ListChanged += OnTabsChanged;
        }

        [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public BindingList<SimpleItem> Tabs
        {
            get => _tabs;
            set
            {
               
                Console.WriteLine("Set Tabs");
                _tabs = value;
                Console.WriteLine("Set Tabs 1");
                // Notify the designer about the change
               // NotifyDesigner(this, true);
                Console.WriteLine("Set Tabs 2");
                SynchronizeTabs();
                Console.WriteLine("Set Tabs 3");

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
                    TabSelected?.Invoke(this, new DynamicTabEventArgs(_panelMap[_selectedTab], SelectedIndex));
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

                    // Notify the designer about the change
                    NotifyDesigner(this, true);
                    UpdateLayout();
                }
            }
        }
        #region "Designer Support"

        public void UnregisterControlAsChildForParentControl(Control parentControl, Control childControl)
        {
            if (parentControl == null || childControl == null)
                throw new ArgumentNullException("Parent and child controls cannot be null.");

            if (parentControl.Site?.GetService(typeof(IComponentChangeService)) is IComponentChangeService changeService)
            {
                // Notify the designer about the upcoming removal
                changeService.OnComponentChanging(parentControl, null);

                // Remove the child control from the designer's container
                var container = parentControl.Site.Container;
                if (container != null)
                {
                    // Find the component explicitly since ComponentCollection doesn't have Contains()
                    var componentToRemove = container.Components.Cast<IComponent>().FirstOrDefault(component => component == childControl);
                    if (componentToRemove != null)
                    {
                        container.Remove(componentToRemove);
                    }
                }

                // Remove the child control from the parent
                if (parentControl.Controls.Contains(childControl))
                {
                    parentControl.Controls.Remove(childControl);
                }

                // Notify the designer that the removal is complete
                changeService.OnComponentChanged(parentControl, null, null, null);
            }
            else
            {
                // Runtime fallback
                if (parentControl.Controls.Contains(childControl))
                {
                    parentControl.Controls.Remove(childControl);
                }
            }
        }

        public void RegisterControlAsChildForParentControl(Control parentControl, Control childControl, string childCollectionProperty = "Controls")
        {
            if (parentControl == null || childControl == null)
                throw new ArgumentNullException("Parent and child controls cannot be null.");

            // Get the property dynamically
            var propertyInfo = parentControl.GetType().GetProperty(childCollectionProperty);
            if (propertyInfo == null)
                throw new InvalidOperationException($"The parent control does not have a property named '{childCollectionProperty}'.");

            // Validate the property is a ControlCollection
            if (propertyInfo.GetValue(parentControl) is not Control.ControlCollection collection)
                throw new InvalidOperationException($"The property '{childCollectionProperty}' is not a valid ControlCollection.");

            if (parentControl.Site?.GetService(typeof(IComponentChangeService)) is IComponentChangeService changeService)
            {
                // Notify the designer about the change
                changeService.OnComponentChanging(parentControl, null);

                // Add the control to the collection
                if (!collection.Contains(childControl))
                {
                    collection.Add(childControl);

                    // Ensure the child control is serialized into Designer.cs
                    var designerHost = parentControl.Site.GetService(typeof(IDesignerHost)) as IDesignerHost;
                    if (designerHost != null)
                    {
                        designerHost.Container.Add(childControl, childControl.Name);
                    }
                }

                // Notify the designer that the change has been completed
                changeService.OnComponentChanged(parentControl, null, null, null);
            }
            else
            {
                // Runtime fallback
                if (!collection.Contains(childControl))
                {
                    collection.Add(childControl);
                }
            }
        }



        private void NotifyDesigner(Control control, bool adding)
        {
            if (Site?.Container is IContainer container && container is IComponentChangeService changeService)
            {
                if (adding)
                {
                    changeService.OnComponentChanging(this, null);
                    changeService.OnComponentChanged(this, null, null, null);
                }
                else
                {
                    changeService.OnComponentChanging(this, null);
                    changeService.OnComponentChanged(this, null, null, null);
                }
            }
        }
        private void RegisterControl(Control control)
        {
            if (Site?.GetService(typeof(IComponentChangeService)) is IComponentChangeService changeService)
            {
                // Notify the designer that a control is being added
                changeService.OnComponentChanging(this, null);

                // Check if the control is already in the container
                bool exists = false;
                if (Site.Container != null)
                {
                    foreach (IComponent component in Site.Container.Components)
                    {
                        if (component == control)
                        {
                            exists = true;
                            break;
                        }
                    }

                    // Add the control if it doesn't exist
                    if (!exists)
                    {
                        Site.Container.Add(control, control.Name);
                    }
                }

                // Notify the designer that the change is complete
                changeService.OnComponentChanged(this, null, null, null);
            }
        }

        private void UnregisterControl(Control control)
        {
            if (Site?.GetService(typeof(IComponentChangeService)) is IComponentChangeService changeService)
            {
                // Notify the designer about the upcoming removal
                changeService.OnComponentChanging(this, null);

                // Remove the control from the container
                if (Site.Container != null)
                {
                    foreach (IComponent component in Site.Container.Components)
                    {
                        if (component == control)
                        {
                            Site.Container.Remove(control);
                            break;
                        }
                    }
                }

                // Notify the designer that the removal is complete
                changeService.OnComponentChanged(this, null, null, null);
            }
            else
            {
                // Fallback for runtime or invalid design-time environment
                Controls.Remove(control);
            }
        }

        #endregion "Designer Support"




        private void OnTabsChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:

                    AddTab(_tabs[e.NewIndex]);
                    TabAdded?.Invoke(this, new DynamicTabEventArgs(_panelMap[_tabs[e.NewIndex]], e.NewIndex));
                    break;

                case ListChangedType.ItemDeleted:
                    RemoveTabByIndex(e.NewIndex);
                    break;

                case ListChangedType.ItemChanged:
                    UpdateTab(_tabs[e.NewIndex]);
                    TabUpdated?.Invoke(this, new DynamicTabEventArgs(_panelMap[_tabs[e.NewIndex]], e.NewIndex));
                    break;

                case ListChangedType.Reset:
                    SynchronizeTabs();
                    break;
            }
        }

        private void SynchronizeTabs()
        {
            ClearTabs();
            Console.WriteLine("SynchronizeTabs");
            if (_tabs == null) return;
            Console.WriteLine("SynchronizeTabs 1");
            for (int i = 0; i < _tabs.Count; i++)
            {
                Console.WriteLine("SynchronizeTabs 2");
                AddTab(_tabs[i]);
                TabAdded?.Invoke(this, new DynamicTabEventArgs(_panelMap[_tabs[i]], i));
            }
        }

        private void ClearTabs()
        {
            _headerPanel.Controls.Clear();
            _contentPanel.Controls.Clear();
            _panelMap.Clear();
        }

        private void AddTab(SimpleItem item)
        {
            Console.WriteLine($"AddTab {item.Text}");
            if (_panelMap.ContainsKey(item)) return;
            Console.WriteLine($"Create content panel for the tab");
            count++;
            // Create content panel for the tab
            var panel = new Panel
            {
                Name = $"Panel{count}",
                Dock = DockStyle.Fill,
                Visible = false,
                BackColor = Color.White,
                AllowDrop = true // Enable drag-and-drop
            };
            Console.WriteLine($" Creating  Drag and Drop for Tab {item.Text} {panel.Name}");
            // Attach drag-and-drop event handlers
            panel.DragEnter += Panel_DragEnter;
            panel.DragDrop += Panel_DragDrop;

            Console.WriteLine($"notify designer");
            // Register the panel with the designer
           
            Console.WriteLine($"Add panel to TabPanels");
            // Add panel to TabPanels (content panel's controls collection)
            //TabPanels.Add(panel);

            // Register the panel as a child of TabPanels
            RegisterControlAsChildForParentControl(_contentPanel, panel);
            Console.WriteLine($"Create Button to panelMap");
            // Create header button for the tab
            var button = CreateTabButton(item, panel);
            Console.WriteLine($"Add button to headerPanel");
            _headerPanel.Controls.Add(button);
            Console.WriteLine($"Add to panelMap");
            _panelMap[item] = panel;
            Console.WriteLine($"Add to panelMap Done");
            // Notify the designer about TabPanels change
            NotifyDesigner(panel, true);
            if (_panelMap.Count == 1)
            {
                SelectedTab = item; // Automatically select the first tab
            }
        }



        private void RemoveTabByIndex(int index)
        {
            if (index < 0 || index >= _tabs.Count) return;

            var item = _tabs[index];
            RemoveTab(item);
            TabRemoved?.Invoke(this, new DynamicTabEventArgs(_panelMap[item], index));
        }

        private void RemoveTab(SimpleItem item)
        {
            if (!_panelMap.TryGetValue(item, out var panel)) return;

            // Unregister and remove the panel
            UnregisterControlAsChildForParentControl(_contentPanel, panel);

            // Remove header button
            var button = _headerPanel.Controls.OfType<Button>().FirstOrDefault(b => Equals(b.Tag, item));
            if (button != null)
            {
                _headerPanel.Controls.Remove(button);
            }

            // Remove from map
            _panelMap.Remove(item);

            // Update selected tab if necessary
            if (_selectedTab == item)
            {
                SelectedTab = _tabs.FirstOrDefault();
            }
        }


        private void UpdateTab(SimpleItem item)
        {
            if (!_panelMap.TryGetValue(item, out var panel)) return;

            // Update header button text
            var button = _headerPanel.Controls.OfType<Button>().FirstOrDefault(b => Equals(b.Tag, item));
            if (button != null)
            {
                button.Text = item.Text;
            }
        }

        private Button CreateTabButton(SimpleItem item, Panel panel)
        {
            var button = new Button
            {
                Text = item.Text,
                AutoSize = true,
                Margin = new Padding(2),
                Tag = item
            };

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
                    var sourceIndex = _headerPanel.Controls.GetChildIndex(draggedButton);
                    var targetIndex = _headerPanel.Controls.GetChildIndex(button);

                    // Swap buttons
                    _headerPanel.Controls.SetChildIndex(draggedButton, targetIndex);
                    _headerPanel.Controls.SetChildIndex(button, sourceIndex);

                    // Update BindingList
                    var temp = _tabs[sourceIndex];
                    _tabs[sourceIndex] = _tabs[targetIndex];
                    _tabs[targetIndex] = temp;
                }
            };

            button.Click += (s, e) =>
            {
                TabButtonClicked?.Invoke(this, new DynamicTabEventArgs(panel, _tabs.IndexOf(item)));
                SelectedTab = item;
                TabSelected?.Invoke(this, new DynamicTabEventArgs(panel, _tabs.IndexOf(item)));
            };

            return button;
        }

        private void UpdateSelectedTab()
        {
            Console.WriteLine($"UpdateSelectedTab {_selectedTab.Text}");
            foreach (var kvp in _panelMap)
            {
                Panel panel = kvp.Value;
                SimpleItem item = kvp.Key;
                if (panel != null)
                {
                    if (item != _selectedTab) 
                    { 
                        Console.WriteLine($"Hide Panel {item.Text}");
                        panel.Visible = false;
                        panel.SendToBack();
                    }
                    else
                    {
                        Console.WriteLine($"Showing Panel {item.Text}");
                        panel.Visible = true;
                        panel.BringToFront();
                    }
                }
                
            }

            HighlightButtonAt(SelectedIndex);
        }

        private void HighlightButtonAt(int index)
        {
            for (int i = 0; i < _headerPanel.Controls.Count; i++)
            {
                if (_headerPanel.Controls[i] is Button btn)
                {
                    btn.BackColor = (i == index) ? Color.LightBlue : SystemColors.Control;
                }
            }
        }

        private void UpdateLayout()
        {
            switch (_headerLocation)
            {
                case HeaderLocation.Top:
                    _headerPanel.Dock = DockStyle.Top;
                    _headerPanel.FlowDirection = FlowDirection.LeftToRight;
                    break;

                case HeaderLocation.Bottom:
                    _headerPanel.Dock = DockStyle.Bottom;
                    _headerPanel.FlowDirection = FlowDirection.LeftToRight;
                    break;

                case HeaderLocation.Left:
                    _headerPanel.Dock = DockStyle.Left;
                    _headerPanel.FlowDirection = FlowDirection.TopDown;
                    break;

                case HeaderLocation.Right:
                    _headerPanel.Dock = DockStyle.Right;
                    _headerPanel.FlowDirection = FlowDirection.TopDown;
                    break;
            }
        }

        #region "Drag and Drop"

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

        private void Panel_DragEnter(object sender, DragEventArgs e)
        {
            // Allow drop only if the data is a control
            if (e.Data.GetDataPresent(typeof(Control)))
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
            if (e.Data.GetData(typeof(Control)) is Control control)
            {
                // Get the target panel
                var targetPanel = sender as Panel;

                if (targetPanel != null)
                {
                    // Add the control to the target panel
                    targetPanel.Controls.Add(control);

                    // Position the control at the drop location
                    var dropPoint = targetPanel.PointToClient(new Point(e.X, e.Y));
                    control.Location = dropPoint;

                    // Ensure the control is visible within the panel
                    control.BringToFront();
                }
            }
        }

        private void BeepDynamicTabControl_DragEnter(object sender, DragEventArgs e)
        {
            // Allow drop only if the data is a control
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
            if (e.Data.GetData(typeof(Control)) is Control control)
            {
                Panel targetPanel = null;

                // Use the selected panel if available
                if (_selectedTab != null && _panelMap.TryGetValue(_selectedTab, out var selectedPanel))
                {
                    targetPanel = selectedPanel;
                }
                else if (_panelMap.Values.Any())
                {
                    // Default to the last panel if no panel is selected
                    targetPanel = _panelMap.Values.Last();
                }

                if (targetPanel != null)
                {
                    // Add the control to the target panel
                    targetPanel.Controls.Add(control);

                    // Position the control at the drop location
                    var dropPoint = targetPanel.PointToClient(new Point(e.X, e.Y));
                    control.Location = dropPoint;

                    // Ensure the control is visible within the panel
                    control.BringToFront();
                }
            }
        }

        #endregion "Drag and Drop"
    }
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
