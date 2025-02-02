
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing.Design;
using TheTechIdea.Beep.Desktop.Common;
using TheTechIdea.Beep.Winform.Controls.Converters;
using TheTechIdea.Beep.Winform.Controls.Design;

namespace TheTechIdea.Beep.Winform.Controls
{
  [Designer(typeof(TheTechIdea.Beep.Winform.Controls.Design.DynamicTabControlDesigner))]
    public class BeepDynamicTabControl : BeepPanel
    {
        public BeepDynamicTabControl()
        {
            AllowDrop = true; // Enable drag-and-drop for the control itself
            ShowTitle = false;

            // Attach drag-and-drop event handlers
            //this.DragEnter += BeepDynamicTabControl_DragEnter;
            //this.DragDrop += BeepDynamicTabControl_DragDrop;

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
            // Tabs = new BindingList<SimpleItem>();
            Tabs.CollectionChanged += Tabs_CollectionChanged;
        }

        // Events
        public event EventHandler<DynamicTabEventArgs> TabSelected;
        public event EventHandler<DynamicTabEventArgs> TabAdded;
        public event EventHandler<DynamicTabEventArgs> TabRemoved;
        public event EventHandler<DynamicTabEventArgs> TabUpdated;
        public event EventHandler<DynamicTabEventArgs> TabButtonClicked;

        #region Properties
        static int count = 0;
        private bool _isResetting = false; // ✅ Prevent multiple resets
        [Browsable(true)]
        [Localizable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Dictionary<string, BeepPanel> Panels { get; }
          = new(StringComparer.OrdinalIgnoreCase);


        //   private BindingList<SimpleItem> Tabs= new BindingList<SimpleItem>();
        private SimpleItem _selectedTab;
        private HeaderLocation _headerLocation = HeaderLocation.Top;
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


        private bool _synchronizingTabs = false; // Prevent recursive calls
        [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
        [Editor(typeof(SimpleItemCollectionEditor), typeof(UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public FullySuppressedObservableCollection<SimpleItem> Tabs { get; } = new FullySuppressedObservableCollection<SimpleItem>();
       

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
                    // Optionally disable the collection-changed handler here
                    _synchronizingTabs = true;
                    _selectedTab = value;
                    UpdateSelectedTab();
                    _synchronizingTabs = false;

                    TabSelected?.Invoke(this, new DynamicTabEventArgs(GetSelectedPanel(), SelectedIndex));
                }
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int SelectedIndex => Tabs?.IndexOf(_selectedTab) ?? -1;

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

        #region Tab Management
        private void Tabs_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (SimpleItem newItem in e.NewItems)
                    {
                        if (!Panels.ContainsKey(newItem.GuidId))
                        {
                            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] 🔹 Add Triggered! ...");
                            AddTab(newItem);
                            TabAdded?.Invoke(this, new DynamicTabEventArgs(Panels[newItem.GuidId], Tabs.IndexOf(newItem)));
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (SimpleItem removedItem in e.OldItems)
                    {
                        RemoveTab(removedItem);
                        TabRemoved?.Invoke(this, new DynamicTabEventArgs(null, -1));
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (SimpleItem newItem in e.NewItems)
                    {
                        UpdateTab(newItem);
                        TabUpdated?.Invoke(this, new DynamicTabEventArgs(Panels[newItem.GuidId], Tabs.IndexOf(newItem)));
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] 🚨 Reset Triggered! Checking Stack Trace...");
                    SynchronizeTabs(); // ✅ Only update missing items
                    break;
            }
        }

        private void OnTabsChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.Reset)
            {
                if (_isResetting) return; // ✅ Ignore duplicate resets
                _isResetting = true;

                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] 🚨 Reset Triggered! Checking Stack Trace...");
                Console.WriteLine(new System.Diagnostics.StackTrace()); // ✅ Helps debugging

                // ✅ Instead of resetting everything, only update missing items
              //  SynchronizeTabs();

                _isResetting = false;
                return;
            }

            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] 🔹 ItemAdded -> {Tabs[e.NewIndex].GuidId}");
                    AddTab(Tabs[e.NewIndex]);
                    TabAdded?.Invoke(this, new DynamicTabEventArgs(Panels[Tabs[e.NewIndex].GuidId], e.NewIndex));
                    break;

                case ListChangedType.ItemDeleted:
                    RemoveTabByIndex(e.NewIndex);
                    break;

                case ListChangedType.ItemChanged:
                    UpdateTab(Tabs[e.NewIndex]);
                    TabUpdated?.Invoke(this, new DynamicTabEventArgs(Panels[Tabs[e.NewIndex].GuidId], e.NewIndex));
                    break;
            }
        }


        private void SynchronizeTabs()
        {
            if (Tabs == null) return;
            if (_isResetting) return; // ✅ Prevent duplicate reset calls
            _isResetting = true;      // 🚨 Lock reset to avoid recursion

            // ✅ Step 1: Add missing tabs
            foreach (var item in Tabs)
            {
                if (!Panels.ContainsKey(item.GuidId.Trim())) // 🚀 Only add if missing
                {
                    AddTab(item);
                    TabAdded?.Invoke(this, new DynamicTabEventArgs(Panels[item.GuidId], Tabs.IndexOf(item)));
                }
            }

            // ✅ Step 2: Remove tabs that no longer exist in `Tabs`
            var existingKeys = Panels.Keys.ToList(); // 🚀 Avoid modifying collection while iterating
            foreach (var key in existingKeys)
            {
                if (!Tabs.Any(t => t.GuidId.Trim() == key))
                {
                    RemoveTab(key); // 🚀 Only remove if not in `Tabs`
                    Panels.Remove(key);
                }
            }

            // ✅ Step 3: Update existing tabs if their text has changed
            foreach (var item in Tabs)
            {
                UpdateTab(item); // 🚀 Only update without removing/re-adding
            }

            _isResetting = false; // ✅ Unlock reset
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
            if (Panels.ContainsKey(item.GuidId.Trim())) return;

            // Create content panel for the tab
            count++;
            var panel = new BeepPanel
            {
                Name = $"Panel{count}",
                TitleText = Name,
                Dock = DockStyle.Fill,
                Visible = false,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                GuidID = item.GuidId.Trim(),
                AllowDrop = true // Enable drag-and-drop
               
            };

           // Debug.WriteLine($"AddTab: Creating drag-and-drop handlers for {panel.Name}");
            // Attach drag-and-drop event handlers
            panel.DragEnter += Panel_DragEnter;
            panel.DragDrop += Panel_DragDrop;

           //Debug.WriteLine($"AddTab: Adding panel {panel.Name} to ContentPanel");
            AddControlToContentPanel(ContentPanel, panel);

         //  Debug.WriteLine($"AddTab: Creating button for tab {item.Text}");
            // Create header button for the tab
            var button = CreateTabButton(item, panel);
          //  Debug.WriteLine($"AddTab: Adding button {button.Name} to HeaderPanel");
            AddControlToContentPanel(HeaderPanel, button);

            // Map the tab's GuidId to its Panel
            Panels[item.GuidId.Trim()] = panel;
            Debug.WriteLine($"AddTab: Added panel {panel.Name} with GuidId {item.GuidId}");
            
            // Automatically select the first tab
            if (Panels.Count == 1)
            {
                SelectedTab = item;
            }
        }
        private BeepPanel GetPanelByGuid(string guid)
        {
            if (string.IsNullOrWhiteSpace(guid))
                return null; // Handle null/empty input

            guid = guid.Trim(); // Ensure no leading/trailing spaces

            // Try to get the panel with case-insensitive lookup
            if (Panels.TryGetValue(guid, out var panel))
            {
                return panel;
            }

            return null; // Return null if not found
        }

        private void RemoveTabByIndex(int index)
        {
            if (index < 0 || index >= Tabs.Count) return;

            var item = Tabs[index];
            RemoveTab(item);
            TabRemoved?.Invoke(this, new DynamicTabEventArgs(Panels[item.GuidId], index));
        }
        private void RemoveTab(string guid)
        {
            if (string.IsNullOrWhiteSpace(guid))
                return; // Handle null/empty input
            guid = guid.Trim(); // Ensure no leading/trailing spaces
            SimpleItem item = Tabs.FirstOrDefault(t => t.GuidId == guid);
            var panel = Panels[guid.Trim()];
            if (panel == null) return;

            // Remove header button
            var button = HeaderPanel.Controls.OfType<Button>().FirstOrDefault(b => Equals(b.Tag, item));
            if (button != null)
            {
                HeaderPanel.Controls.Remove(button);
            }

            // Remove panel from ContentPanel
            RemoveControlFromContentPanel(ContentPanel, panel);

            // Remove from Panels dictionary
            Panels.Remove(guid);

            // Update selected tab if necessary

                SelectedTab = Tabs.FirstOrDefault();
            // remove from Tabs
          
           // Tabs.Remove(item);

        }
        private void RemoveTab(SimpleItem item)
        {
            var panel = Panels[item.GuidId.Trim()];
            if (panel == null) return;

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
                SelectedTab = Tabs.FirstOrDefault();
            }
            // remove from Tabs
         //   Tabs.Remove(item);
            // Debug.WriteLine($"RemoveTab: Removed tab {item.Text} and panel {panel.Name}");
        }

        private void UpdateTab(SimpleItem item)
        {
            var panel = Panels[item.GuidId.Trim()];
            if (panel == null) return;

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

        private BeepButton CreateTabButton(SimpleItem item, BeepPanel panel)
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
                    var temp = Tabs[sourceIndex];
                    Tabs[sourceIndex] = Tabs[targetIndex];
                    Tabs[targetIndex] = temp;

                   // Debug.WriteLine($"CreateTabButton: Swapped tabs at index {sourceIndex} and {targetIndex}");
                }
            };

            // Click event to select the tab
            button.Click += (s, e) =>
            {
                BeepButton btn = (BeepButton)s;
                BeepPanel btnpanel = GetPanelByGuid(btn.GuidID);
                TabButtonClicked?.Invoke(this, new DynamicTabEventArgs(btnpanel, Tabs.IndexOf(btn.SelectedItem)));
                SelectedTab = btn.SelectedItem;
                TabSelected?.Invoke(this, new DynamicTabEventArgs(btnpanel, Tabs.IndexOf(btn.SelectedItem)));
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
                if (control is BeepPanel panel)
                {
                   if (panel.GuidID == _selectedTab.GuidId)
                    {
                        panel.Visible = true;
                        panel.BringToFront();
                        // Debug.WriteLine($"UpdateSelectedTab: Panel {panel.Name} is now visible.");
                    }
                    else
                    {
                        panel.Visible = false;
                        panel.SendToBack();
                        // Debug.WriteLine($"UpdateSelectedTab: Panel {panel.Name} is now hidden.");
                    }
                    //panel.Visible = isSelected;
                    //if (isSelected)
                    //{
                    //    panel.BringToFront();
                    //   // Debug.WriteLine($"UpdateSelectedTab: Panel {panel.Name} is now visible.");
                    //}
                    //else
                    //{
                    //    panel.SendToBack();
                    //   // Debug.WriteLine($"UpdateSelectedTab: Panel {panel.Name} is now hidden.");
                    //}
                }
            }

            HighlightButtonAt(_selectedTab.GuidId);
        }

        private BeepPanel GetSelectedPanel()
        {
            if (_selectedTab != null)
            {
                var panel = Panels[_selectedTab.GuidId];
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

        //private void BeepDynamicTabControl_DragEnter(object sender, DragEventArgs e)
        //{
        //    if (IsInDesignMode())
        //    {
        //        // Let the designer handle drag-enter
        //        return;
        //    }

        //    // Runtime drag-enter logic
        //    if (e.Data.GetDataPresent(typeof(Control)))
        //    {
        //        e.Effect = DragDropEffects.Move;
        //    }
        //    else
        //    {
        //        e.Effect = DragDropEffects.None;
        //    }
        //}

        //private void BeepDynamicTabControl_DragDrop(object sender, DragEventArgs e)
        //{
        //    if (IsInDesignMode())
        //    {
        //        // Let the designer handle drag-drop
        //        return;
        //    }

        //    // Runtime drag-drop logic
        //    if (e.Data.GetData(typeof(Control)) is Control control)
        //    {
        //        Panel targetPanel = SelectedTab != null && Panels.ContainsKey(SelectedTab.GuidId)
        //            ? Panels[SelectedTab.GuidId]
        //            : (Panels.Values.Any() ? Panels.Values.Last() : null);

        //        if (targetPanel != null)
        //        {
        //            targetPanel.Controls.Add(control);
        //            control.Location = targetPanel.PointToClient(new Point(e.X, e.Y));
        //            control.BringToFront();
        //           // Debug.WriteLine($"DragDrop: Control {control.Name} added to {targetPanel.Name} at location {control.Location}");
        //        }
        //    }
        //}

        private void Panel_DragEnter(object sender, DragEventArgs e)
        {
            //if (IsInDesignMode())
            //{
            //    // Let the designer handle drag-enter
            //    return;
            //}

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
            //if (IsInDesignMode())
            //{
            //    // Let the designer handle drag-drop
            //    return;
            //}

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


        #endregion

        #region "Designer Support"

        public void RemoveAllControlsFromContentPanel(Control contentPanel)
        {
            if (contentPanel == null)
                throw new ArgumentNullException(nameof(contentPanel));

            // In design mode, do not remove controls that might contain user-dropped controls.
            if (IsInDesignMode())
                return;

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
                    var componentToRemove = container.Components
                        .Cast<IComponent>()
                        .FirstOrDefault(c => c == childControl);
                    if (componentToRemove != null)
                    {
                        container.Remove(componentToRemove);
                    }
                }

                // Dispose the control to free resources (only at runtime)
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
                    var componentToRemove = container.Components
                        .Cast<IComponent>()
                        .FirstOrDefault(component => component == childControl);
                    if (componentToRemove != null)
                    {
                        container.Remove(componentToRemove);
                    }
                }

                // Remove the child control from the parent (always remove from Controls collection)
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

                // Add the control to the collection if it's not already added
                if (!collection.Contains(childControl))
                {
                    collection.Add(childControl);

                    // Ensure the child control is serialized into Designer.cs
                    var designerHost = parentControl.Site.GetService(typeof(IDesignerHost)) as IDesignerHost;
                    if (designerHost != null)
                    {
                        designerHost.Container.Add(childControl, childControl.Name);
                    }

                    // Enable design mode on the child control so that it can be modified in the designer.
                    if (parentControl is Control parent && parent.Site != null)
                    {
                        // This registers the control with the designer surface.
                       // EnableDesignMode(childControl, childControl.Name);
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

            if (!IsInDesignMode()) return; // Only notify in design mode

            if (Site?.GetService(typeof(IComponentChangeService)) is IComponentChangeService changeService)
            {
                var controlsProperty = TypeDescriptor.GetProperties(contentPanel)["Controls"];

                if (controlsProperty == null)
                    throw new InvalidOperationException("The 'Controls' property could not be found on the ContentPanel.");

                // Notify the designer of the change
                changeService.OnComponentChanging(contentPanel, controlsProperty);
                changeService.OnComponentChanged(contentPanel, controlsProperty, null, null);
            }
        }

        public void AddControlToContentPanel(Control contentPanel, Control childControl)
        {
            if (contentPanel == null || childControl == null)
                throw new ArgumentNullException();

            // Prevent duplicate additions
            if (contentPanel.Controls.Contains(childControl))
            {
                Debug.WriteLine($"[INFO] The control '{childControl.Name}' already exists in the ContentPanel. Skipping addition.");
                return;
            }

            // Add control to the panel
            contentPanel.Controls.Add(childControl);

            // Notify the designer if in design mode
            if (IsInDesignMode())
            {
                NotifyContentPanelControlsPropertyDesigner(contentPanel, true);
            }
        }

        public void RemoveControlFromContentPanel(Control contentPanel, Control childControl)
        {
            if (contentPanel == null || childControl == null)
                throw new ArgumentNullException();

            // Remove the child control from the ContentPanel
            contentPanel.Controls.Remove(childControl);

            // In design mode, avoid disposing so that user-added controls persist.
            if (!IsInDesignMode())
            {
                // Unregister the control from the designer if it was added at design-time
                if (Site?.Container is IContainer container)
                {
                    var componentToRemove = container.Components
                        .Cast<IComponent>()
                        .FirstOrDefault(c => c == childControl);
                    if (componentToRemove != null)
                    {
                        container.Remove(componentToRemove);
                        Debug.WriteLine($"[DESIGNER] Unregistered '{childControl.Name}' from the designer.");
                    }
                }

                // Dispose the control (only at runtime)
                childControl.Dispose();
            }
        }

        private void NotifyDesignerVisibilityChange(Control control, bool isVisible)
        {
            if (control == null)
                return;

            if (Site?.GetService(typeof(IComponentChangeService)) is IComponentChangeService changeService)
            {
                changeService.OnComponentChanging(control, null);
                control.Visible = isVisible;
                changeService.OnComponentChanged(control, null, null, null);
            }
            else
            {
                control.Visible = isVisible;
            }
        }

        private void UpdatePanelVisibility(Control panel, bool isVisible)
        {
            if (panel == null)
                return;

            if (Site?.GetService(typeof(IComponentChangeService)) is IComponentChangeService changeService)
            {
                var propDescriptor = TypeDescriptor.GetProperties(panel)["Visible"];
                changeService.OnComponentChanging(panel, propDescriptor);

                panel.Visible = isVisible;
                if (isVisible)
                {
                    panel.Dock = DockStyle.Fill; // Ensure the panel fills the content area
                    panel.BringToFront();
                }
                else
                {
                    panel.SendToBack();
                }

                changeService.OnComponentChanged(panel, propDescriptor, !isVisible, isVisible);
            }
            else
            {
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
            if (Site?.Container is IContainer container &&
                Site?.GetService(typeof(IComponentChangeService)) is IComponentChangeService changeService)
            {
                changeService.OnComponentChanging(this, null);
                changeService.OnComponentChanged(this, null, null, null);
            }
        }

        private void RegisterControl(Control control)
        {
            if (Site?.GetService(typeof(IComponentChangeService)) is IComponentChangeService changeService)
            {
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

                changeService.OnComponentChanged(this, null, null, null);
            }
        }

        private void UnregisterControl(Control control)
        {
            if (Site?.GetService(typeof(IComponentChangeService)) is IComponentChangeService changeService)
            {
                changeService.OnComponentChanging(this, null);

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

                changeService.OnComponentChanged(this, null, null, null);
            }
            else
            {
                Controls.Remove(control);
            }
        }

        #endregion "Designer Support"


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

      

        #endregion
    }

    // Supporting Classes

    public class DynamicTabEventArgs : EventArgs
    {
        public DynamicTabEventArgs(BeepPanel tabPage, int tabIndex)
        {
            TabPage = tabPage;
            TabIndex = tabIndex;
        }

        public BeepPanel TabPage { get; }
        public int TabIndex { get; }
    }
}
