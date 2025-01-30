using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using TheTechIdea.Beep.Desktop.Common;
using TheTechIdea.Beep.Winform.Controls.Converters;




namespace TheTechIdea.Beep.Winform.Controls
{
   // [Designer(typeof(DynamicTabControlDesigner))]
    public class BeepDynamicTabControl : BeepPanel
    {
        static int count = 0;
      
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
     //   public  Panel _contentPanel { get; private set; }
        public readonly Dictionary<SimpleItem, Panel> _panelMap = new();
        private BindingList<SimpleItem> _tabs = new BindingList<SimpleItem>();
        private SimpleItem _selectedTab;
        private HeaderLocation _headerLocation = HeaderLocation.Top;

        public event EventHandler<DynamicTabEventArgs> TabSelected;
        public event EventHandler<DynamicTabEventArgs> TabAdded;
        public event EventHandler<DynamicTabEventArgs> TabRemoved;
        public event EventHandler<DynamicTabEventArgs> TabUpdated;
        public event EventHandler<DynamicTabEventArgs> TabButtonClicked;
        [Browsable(true)]
        [Localizable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public FlowLayoutPanel HeaderPanel { get; } = new()
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
        public Panel ContentPanel { get; } = new()
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White
        };


        public BeepDynamicTabControl()
        {
            AllowDrop = true; // Enable drag-and-drop for the control itself
            ShowTitle = false;
            DragEnter += BeepDynamicTabControl_DragEnter;
            DragDrop += BeepDynamicTabControl_DragDrop;
            //// Header panel for tab buttons

            //NotifyContentPanelControlsPropertyDesigner(this, true);
            //UpdateLayout();
            if (!Controls.Contains(HeaderPanel))
            {
                Controls.Add(HeaderPanel);
            }

            if (!Controls.Contains(ContentPanel))
            {
                Controls.Add(ContentPanel);
                ContentPanel.BringToFront();
            }
            _tabs = new BindingList<SimpleItem>();
            _tabs.ListChanged+= OnTabsChanged;
           
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
                  //  NotifyDesigner(this, true);
                    UpdateLayout();
                }
            }
        }
        #region "Designer Support"
        public void RemoveAllControlsFromContentPanel(Control contentPanel)
        {
            if (contentPanel == null)
                throw new ArgumentNullException(nameof(contentPanel));

            // Iterate through all child controls and remove them
            foreach (Control childControl in contentPanel.Controls.Cast<Control>().ToList())
            {
                // Remove the child control from the ContentPanel
                contentPanel.Controls.Remove(childControl);

                // Notify the designer
                NotifyContentPanelControlsPropertyDesigner(contentPanel, false);

                // Unregister the control from the designer
                if (Site?.Container is IContainer container)
                {
                    var componentToRemove = container.Components.Cast<IComponent>().FirstOrDefault(c => c == childControl);
                    if (componentToRemove != null)
                    {
                        container.Remove(componentToRemove);
                    }
                }

                // Dispose the control to free resources
                childControl.Dispose();
            }
        }

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
        private void NotifyContentPanelControlsPropertyDesigner(Control contentPanel, bool adding)
        {
            if (contentPanel == null)
                throw new ArgumentNullException(nameof(contentPanel));

            if (Site?.GetService(typeof(IComponentChangeService)) is IComponentChangeService changeService)
            {
                Console.WriteLine("Design-time services available.");
                // Get the 'Controls' property descriptor
                var controlsProperty = TypeDescriptor.GetProperties(contentPanel)["Controls"];

                if (controlsProperty == null)
                    throw new InvalidOperationException("The 'Controls' property could not be found on the ContentPanel.");

                // Notify the designer about the change
                if (adding)
                { 
                    Console.WriteLine("Add Design-time services available. Notifying the designer.");
                    changeService.OnComponentChanging(contentPanel, controlsProperty);
                    changeService.OnComponentChanged(contentPanel, controlsProperty, null, null);
                }
                else
                {
                    Console.WriteLine("Remove Design-time services available. Notifying the designer.");
                    changeService.OnComponentChanging(contentPanel, controlsProperty);
                    changeService.OnComponentChanged(contentPanel, controlsProperty, null, null);
                }
            }
            else
            {
                // Fallback if no design-time services are available
                Console.WriteLine("Design-time services not available. Runtime-only change.");
            }
        }
        public void AddControlToContentPanel(Control contentPanel, Control childControl)
        {
            if (contentPanel == null || childControl == null)
                throw new ArgumentNullException();

            // Check if the childControl already exists in the contentPanel
            //if (contentPanel.Controls.Contains(childControl))
            //{
            //    Console.WriteLine($"The control '{childControl.Name}' already exists in the ContentPanel. Skipping addition.");
            //    return; // Exit if the control already exists
            //}
            // Add the child control to the ContentPanel
            contentPanel.Controls.Add(childControl);

            // Notify the designer
            NotifyContentPanelControlsPropertyDesigner(contentPanel, true);

            // Register the control with the designer
            if (Site?.Container is IContainer container)
            {
                if (!container.Components.Cast<IComponent>().Contains(childControl))
                {
                    container.Add(childControl, childControl.Name);
                }
            }
        }
        public void RemoveControlFromContentPanel(Control contentPanel, Control childControl)
        {
            if (contentPanel == null || childControl == null)
                throw new ArgumentNullException();

            // Remove the child control from the ContentPanel
            contentPanel.Controls.Remove(childControl);

            // Notify the designer
            NotifyContentPanelControlsPropertyDesigner(contentPanel, false);

            // Unregister the control from the designer
            if (Site?.Container is IContainer container)
            {
                var componentToRemove = container.Components.Cast<IComponent>().FirstOrDefault(c => c == childControl);
                if (componentToRemove != null)
                {
                    container.Remove(componentToRemove);
                }
            }
        }


        private void NotifyDesignerVisibilityChange(Control control, bool isVisible)
        {
            if (control == null)
                return;

            if (Site?.GetService(typeof(IComponentChangeService)) is IComponentChangeService changeService)
            {
                // Notify the designer that the control is changing
                changeService.OnComponentChanging(control, null);

                // Change the property
                control.Visible = isVisible;

                // Notify the designer that the change is complete
                changeService.OnComponentChanged(control, null, null, null);
            }
            else
            {
                // Fallback for runtime-only changes
                control.Visible = isVisible;
            }
        }
        private void UpdatePanelVisibility(Control panel, bool isVisible)
        {
            if (panel == null)
                return;

            if (Site?.GetService(typeof(IComponentChangeService)) is IComponentChangeService changeService)
            {
                // Notify the designer about the upcoming change
                changeService.OnComponentChanging(panel, TypeDescriptor.GetProperties(panel)["Visible"]);

                // Update the property
                panel.Visible = isVisible;
                if (isVisible)
                {
                    panel.Dock = DockStyle.Fill; // Ensure the panel fills the content area
                    panel.BringToFront(); // Bring the selected panel to the front
                }
                else
                {
                    panel.SendToBack(); // Send non-selected panels to the back
                }

                // Notify the designer that the change is complete
                changeService.OnComponentChanged(panel, TypeDescriptor.GetProperties(panel)["Visible"], !isVisible, isVisible);
            }
            else
            {
                // Fallback for runtime-only changes
                panel.Visible = isVisible;
                if (isVisible)
                {
                    panel.Dock = DockStyle.Fill;
                    panel.BringToFront();
                }
                else
                {
                    panel.SendToBack();
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
         //   ClearTabs();
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
            HeaderPanel.Controls.Clear();
             ContentPanel.Controls.Clear();
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
            panel.AllowDrop= true;
            EnableDesignMode(panel, panel.Name);
            Console.WriteLine($"notify designer");
            // Register the panel with the designer
           
            Console.WriteLine($"Add panel to TabPanels");
            // Add panel to TabPanels (content panel's controls collection)
        //    _contentPanel.Controls.Add(panel);
            AddControlToContentPanel(ContentPanel, panel);

            Console.WriteLine($"Create Button to panelMap");
            // Create header button for the tab
            var button = CreateTabButton(item, panel);
            Console.WriteLine($"Add button to headerPanel");
            //HeaderPanel.Controls.Add(button);
            AddControlToContentPanel(HeaderPanel, button);
            Console.WriteLine($"Add to panelMap");
            _panelMap[item] = panel;
            Console.WriteLine($"Add to panelMap Done");
            // Notify the designer about TabPanels change
            // NotifyDesigner(panel, true);
       //     NotifyContentPanelControlsPropertyDesigner(ContentPanel, true);

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
            
           
            // Remove header button
            var button = HeaderPanel.Controls.OfType<Button>().FirstOrDefault(b => Equals(b.Tag, item));
            if (button != null)
            {
                HeaderPanel.Controls.Remove(button);
            }
          //  _contentPanel.Controls.Remove(panel);
            RemoveControlFromContentPanel(ContentPanel, panel);

          //  NotifyContentPanelControlsPropertyDesigner(ContentPanel, false);
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
            var button = HeaderPanel.Controls.OfType<Button>().FirstOrDefault(b => Equals(b.Tag, item));
            if (button != null)
            {
                button.Text = item.Text;
            }
        }

        private Button CreateTabButton(SimpleItem item, Panel panel)
        {
            Console.WriteLine($"CreateTabButton {item.Name}");
            var button = new Button
            {
                Name= $"HeaderButton{count}",
                Text = item.Text==null? Name : item.Text,
                AutoSize = true,
                Margin = new Padding(2),
                Tag = item
            };
            Console.WriteLine($"CreateTabButton {item.Name} Done");
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
            if (_selectedTab == null)
                return;

            Console.WriteLine($"UpdateSelectedTab {_selectedTab.Text}");

            foreach (Control control in ContentPanel.Controls)
            {
                if (control is Panel panel)
                {
                    if (_panelMap.TryGetValue(_selectedTab, out var selectedPanel) && panel == selectedPanel)
                    {
                        Console.WriteLine($"Showing Panel {panel.Name}");
                        UpdatePanelVisibility(panel, true); // Config the selected panel
                    }
                    else
                    {
                        Console.WriteLine($"Hiding Panel {panel.Name}");
                        UpdatePanelVisibility(panel, false); // Hide all other panels
                    }
                }
            }

            HighlightButtonAt(SelectedIndex); // Highlight the corresponding button
        }



        private void HighlightButtonAt(int index)
        {
            for (int i = 0; i < HeaderPanel.Controls.Count; i++)
            {
                if (HeaderPanel.Controls[i] is Button btn)
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
                // Unregister dynamically created panels and buttons
                //foreach (var panel in _panelMap.Values)
                //{
                //    RemoveControlFromContentPanel(ContentPanel, panel);
                //}

                // Clear panel map
                _panelMap.Clear();

                // Unregister HeaderPanel and ContentPanel
                UnregisterControl(HeaderPanel);
                UnregisterControl(ContentPanel);
            }

            base.Dispose(disposing);
        }
       

        #region "Drag and Drop"

        //// Drag and Drop should go to Selected Panel and if no panel is selected it should go to the last panel. If no panel exists, stop drop
        //protected override void OnDragEnter(DragEventArgs e)
        //{
        //    base.OnDragEnter(e);
        //    BeepDynamicTabControl_DragEnter(this, e);
        //}

        //protected override void OnDragDrop(DragEventArgs e)
        //{
        //    base.OnDragDrop(e);
        //    BeepDynamicTabControl_DragDrop(this, e);
        //}

        private void Panel_DragEnter(object sender, DragEventArgs e)
        {
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
            if (e.Data.GetData(typeof(ToolboxItem)) is Control   )
            {
                var control = e.Data.GetData(typeof(ToolboxItem)) as Control;
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
