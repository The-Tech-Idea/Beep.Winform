using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Tabs.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    public class BeepTabsDesigner : ParentControlDesigner
    {
        private DesignerActionListCollection? _actionLists;
        private DesignerVerbCollection? _verbs;
        private IComponentChangeService? _changeService;

        private BeepTabs? Tabs => Component as BeepTabs;

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
            _changeService = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
        }

        public override DesignerActionListCollection ActionLists
            => _actionLists ??= new DesignerActionListCollection
            {
                new BeepTabsActionList(this)
            };

        public override DesignerVerbCollection Verbs
            => _verbs ??= new DesignerVerbCollection
            {
                new DesignerVerb("Add Tab Page", (_, _) => AddTabPage()),
                new DesignerVerb("Remove Selected Tab", (_, _) => RemoveSelectedTabPage()),
                new DesignerVerb("Move Selected Tab Left", (_, _) => MoveSelectedTab(-1)),
                new DesignerVerb("Move Selected Tab Right", (_, _) => MoveSelectedTab(1))
            };

        public T? GetProperty<T>(string propertyName)
        {
            if (Component == null)
            {
                return default;
            }

            PropertyDescriptor? property = TypeDescriptor.GetProperties(Component)[propertyName];
            if (property == null)
            {
                return default;
            }

            object? value = property.GetValue(Component);
            return value is T typedValue ? typedValue : default;
        }

        public void SetProperty(string propertyName, object value)
        {
            if (Component == null)
            {
                return;
            }

            PropertyDescriptor? property = TypeDescriptor.GetProperties(Component)[propertyName];
            if (property == null || property.IsReadOnly)
            {
                return;
            }

            object? currentValue = property.GetValue(Component);
            if (Equals(currentValue, value))
            {
                return;
            }

            _changeService?.OnComponentChanging(Component, property);
            property.SetValue(Component, value);
            _changeService?.OnComponentChanged(Component, property, currentValue, value);
        }

        public string GetSelectedTabTitle()
            => Tabs?.SelectedTab?.Text ?? string.Empty;

        public void SetSelectedTabTitle(string value)
        {
            TabPage? selectedTab = Tabs?.SelectedTab;
            if (selectedTab == null)
            {
                return;
            }

            SetComponentProperty(selectedTab, nameof(TabPage.Text), value);
        }

        public void AddTabPage()
        {
            if (Tabs == null)
            {
                return;
            }

            TabPage? createdPage = null;

            ExecuteTabsAction("Add Tab Page", tabs =>
            {
                int tabNumber = tabs.TabPages.Count + 1;
                string componentName = GetUniqueTabPageName(tabNumber);
                createdPage = CreateTabPage(componentName, $"Tab {tabNumber}");

                tabs.TabPages.Add(createdPage);
                tabs.SelectedTab = createdPage;
                tabs.Invalidate();
            });

            SyncDesignerSelection((object?)createdPage ?? Tabs);
        }

        public void RemoveSelectedTabPage()
        {
            if (Tabs?.SelectedTab == null)
            {
                return;
            }

            TabPage selectedTab = Tabs.SelectedTab;
            object selectionTarget = Tabs;
            ExecuteTabsAction($"Remove Tab Page '{selectedTab.Text}'", tabs =>
            {
                tabs.TabPages.Remove(selectedTab);
                selectionTarget = (object?)tabs.SelectedTab ?? tabs;

                IDesignerHost? designerHost = GetDesignerHost();
                if (designerHost != null && selectedTab.Site != null)
                {
                    designerHost.DestroyComponent(selectedTab);
                }
                else
                {
                    selectedTab.Dispose();
                }

                tabs.Invalidate();
            });

            SyncDesignerSelection(selectionTarget);
        }

        public void ClearAllTabPages()
        {
            if (Tabs == null || Tabs.TabPages.Count == 0)
            {
                return;
            }

            ExecuteTabsAction("Clear Tab Pages", tabs =>
            {
                IDesignerHost? designerHost = GetDesignerHost();
                while (tabs.TabPages.Count > 0)
                {
                    TabPage page = tabs.TabPages[tabs.TabPages.Count - 1];
                    tabs.TabPages.Remove(page);

                    if (designerHost != null && page.Site != null)
                    {
                        designerHost.DestroyComponent(page);
                    }
                    else
                    {
                        page.Dispose();
                    }
                }

                tabs.Invalidate();
            });

            SyncDesignerSelection(Tabs);
        }

        public void MoveSelectedTab(int offset)
        {
            if (Tabs == null || Tabs.SelectedIndex < 0)
            {
                return;
            }

            int currentIndex = Tabs.SelectedIndex;
            int newIndex = Math.Max(0, Math.Min(Tabs.TabPages.Count - 1, currentIndex + offset));
            if (newIndex == currentIndex)
            {
                return;
            }

            ExecuteTabsAction($"Move Tab Page to position {newIndex + 1}", tabs =>
            {
                TabPage page = tabs.TabPages[currentIndex];
                tabs.TabPages.RemoveAt(currentIndex);
                tabs.TabPages.Insert(newIndex, page);
                tabs.SelectedIndex = newIndex;
                tabs.Invalidate();
            });

            SyncDesignerSelection((object?)Tabs.SelectedTab ?? Tabs);
        }

        public void SelectNextTabPage()
        {
            if (Tabs == null || Tabs.TabCount == 0)
            {
                return;
            }

            Tabs.SelectedIndex = Math.Min(Tabs.TabCount - 1, Tabs.SelectedIndex + 1);
            Tabs.Invalidate();
            SyncDesignerSelection((object?)Tabs.SelectedTab ?? Tabs);
        }

        public void SelectPreviousTabPage()
        {
            if (Tabs == null || Tabs.TabCount == 0)
            {
                return;
            }

            Tabs.SelectedIndex = Math.Max(0, Tabs.SelectedIndex - 1);
            Tabs.Invalidate();
            SyncDesignerSelection((object?)Tabs.SelectedTab ?? Tabs);
        }

        private void SetComponentProperty(object component, string propertyName, object value)
        {
            PropertyDescriptor? property = TypeDescriptor.GetProperties(component)[propertyName];
            if (property == null || property.IsReadOnly)
            {
                return;
            }

            object? currentValue = property.GetValue(component);
            if (Equals(currentValue, value))
            {
                return;
            }

            _changeService?.OnComponentChanging(component, property);
            property.SetValue(component, value);
            _changeService?.OnComponentChanged(component, property, currentValue, value);
        }

        private TabPage CreateTabPage(string componentName, string text)
        {
            IDesignerHost? designerHost = GetDesignerHost();
            TabPage page = designerHost?.CreateComponent(typeof(TabPage), componentName) as TabPage ?? new TabPage();
            page.Name = componentName;
            page.Text = text;
            return page;
        }

        private string GetUniqueTabPageName(int suggestedIndex)
        {
            IContainer? container = GetDesignerHost()?.Container;
            int index = Math.Max(1, suggestedIndex);

            string name;
            do
            {
                name = $"tabPage{index}";
                index++;
            }
            while (container?.Components[name] != null);

            return name;
        }

        private IDesignerHost? GetDesignerHost()
            => GetService(typeof(IDesignerHost)) as IDesignerHost;

        private ISelectionService? GetSelectionService()
            => GetService(typeof(ISelectionService)) as ISelectionService;

        private void SyncDesignerSelection(object? selectionTarget)
        {
            if (selectionTarget == null)
            {
                return;
            }

            ISelectionService? selectionService = GetSelectionService();
            selectionService?.SetSelectedComponents(new object[] { selectionTarget }, SelectionTypes.Replace);
        }

        private void ExecuteTabsAction(string description, Action<BeepTabs> action)
        {
            if (Tabs == null)
            {
                return;
            }

            IDesignerHost? designerHost = GetDesignerHost();
            DesignerTransaction? transaction = null;

            try
            {
                transaction = designerHost?.CreateTransaction(description);
                _changeService?.OnComponentChanging(Tabs, null);

                action(Tabs);

                _changeService?.OnComponentChanged(Tabs, null, null, null);
                transaction?.Commit();
            }
            catch
            {
                transaction?.Cancel();
                throw;
            }
        }
    }

    public class BeepTabsActionList : DesignerActionList
    {
        private readonly BeepTabsDesigner _designer;

        public BeepTabsActionList(BeepTabsDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        protected BeepTabs? Tabs => Component as BeepTabs;

        [Category("Tabs")]
        [Description("Visual style of the tabs")]
        public TabStyle TabStyle
        {
            get => _designer.GetProperty<TabStyle>(nameof(BeepTabs.TabStyle));
            set => _designer.SetProperty(nameof(BeepTabs.TabStyle), value);
        }

        [Category("Appearance")]
        [Description("Header height in pixels")]
        public int HeaderHeight
        {
            get => _designer.GetProperty<int>(nameof(BeepTabs.HeaderHeight));
            set => _designer.SetProperty(nameof(BeepTabs.HeaderHeight), value);
        }

        [Category("Appearance")]
        [Description("Minimum touch target width for tabs in pixels")]
        public int MinTouchTargetWidth
        {
            get => _designer.GetProperty<int>(nameof(BeepTabs.MinTouchTargetWidth));
            set => _designer.SetProperty(nameof(BeepTabs.MinTouchTargetWidth), value);
        }

        [Category("Appearance")]
        [Description("Controls when tab text labels are visible")]
        public TabLabelVisibility TabTextVisibility
        {
            get => _designer.GetProperty<TabLabelVisibility>(nameof(BeepTabs.TabTextVisibility));
            set => _designer.SetProperty(nameof(BeepTabs.TabTextVisibility), value);
        }

        [Category("Appearance")]
        [Description("Position of tab headers")]
        public TabHeaderPosition HeaderPosition
        {
            get => _designer.GetProperty<TabHeaderPosition>(nameof(BeepTabs.HeaderPosition));
            set => _designer.SetProperty(nameof(BeepTabs.HeaderPosition), value);
        }

        [Category("Behavior")]
        [Description("Show close buttons on tabs")]
        public bool ShowCloseButtons
        {
            get => _designer.GetProperty<bool>(nameof(BeepTabs.ShowCloseButtons));
            set => _designer.SetProperty(nameof(BeepTabs.ShowCloseButtons), value);
        }

        [Category("Appearance")]
        [Description("Theme name")]
        public string Theme
        {
            get => _designer.GetProperty<string>(nameof(BeepTabs.Theme)) ?? string.Empty;
            set => _designer.SetProperty(nameof(BeepTabs.Theme), value);
        }

        [Category("Tab Pages")]
        [Description("Caption of the currently selected tab page")]
        public string SelectedTabTitle
        {
            get => _designer.GetSelectedTabTitle();
            set => _designer.SetSelectedTabTitle(value);
        }

        public void ApplyClassicStyle() => TabStyle = TabStyle.Classic;
        public void ApplyUnderlineStyle() => TabStyle = TabStyle.Underline;
        public void ApplyCapsuleStyle() => TabStyle = TabStyle.Capsule;
        public void ApplyCardStyle() => TabStyle = TabStyle.Card;
        public void ApplyMinimalStyle() => TabStyle = TabStyle.Minimal;

        public void UseRecommendedHeaderHeight()
        {
            if (Tabs != null)
            {
                HeaderHeight = TabStyleHelpers.GetRecommendedHeaderHeight(Tabs.TabStyle);
            }
        }

        public void AddTabPage() => _designer.AddTabPage();
        public void RemoveSelectedTabPage() => _designer.RemoveSelectedTabPage();
        public void MoveSelectedTabLeft() => _designer.MoveSelectedTab(-1);
        public void MoveSelectedTabRight() => _designer.MoveSelectedTab(1);
        public void SelectPreviousTabPage() => _designer.SelectPreviousTabPage();
        public void SelectNextTabPage() => _designer.SelectNextTabPage();
        public void ClearAllTabPages() => _designer.ClearAllTabPages();

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Tabs"));
            items.Add(new DesignerActionPropertyItem(nameof(TabStyle), "Tab Style", "Tabs"));
            items.Add(new DesignerActionPropertyItem(nameof(ShowCloseButtons), "Show Close Buttons", "Tabs"));

            items.Add(new DesignerActionHeaderItem("Appearance"));
            items.Add(new DesignerActionPropertyItem(nameof(HeaderHeight), "Header Height", "Appearance"));
            items.Add(new DesignerActionPropertyItem(nameof(MinTouchTargetWidth), "Min Touch Target Width", "Appearance"));
            items.Add(new DesignerActionPropertyItem(nameof(TabTextVisibility), "Tab Text Visibility", "Appearance"));
            items.Add(new DesignerActionPropertyItem(nameof(HeaderPosition), "Header Position", "Appearance"));
            items.Add(new DesignerActionPropertyItem(nameof(Theme), "Theme", "Appearance"));

            items.Add(new DesignerActionHeaderItem("Tab Pages"));
            items.Add(new DesignerActionPropertyItem(nameof(SelectedTabTitle), "Selected Tab Title", "Tab Pages", "Edit the caption of the currently selected tab page."));
            items.Add(new DesignerActionMethodItem(this, nameof(AddTabPage), "Add Tab Page", "Tab Pages", true));
            items.Add(new DesignerActionMethodItem(this, nameof(RemoveSelectedTabPage), "Remove Selected Tab", "Tab Pages", true));
            items.Add(new DesignerActionMethodItem(this, nameof(MoveSelectedTabLeft), "Move Selected Tab Left", "Tab Pages", true));
            items.Add(new DesignerActionMethodItem(this, nameof(MoveSelectedTabRight), "Move Selected Tab Right", "Tab Pages", true));
            items.Add(new DesignerActionMethodItem(this, nameof(SelectPreviousTabPage), "Select Previous Tab", "Tab Pages", true));
            items.Add(new DesignerActionMethodItem(this, nameof(SelectNextTabPage), "Select Next Tab", "Tab Pages", true));
            items.Add(new DesignerActionMethodItem(this, nameof(ClearAllTabPages), "Clear All Tabs", "Tab Pages", true));

            items.Add(new DesignerActionHeaderItem("Style Presets"));
            items.Add(new DesignerActionMethodItem(this, nameof(ApplyClassicStyle), "Classic Style", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(ApplyUnderlineStyle), "Underline Style", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(ApplyCapsuleStyle), "Capsule Style", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(ApplyCardStyle), "Card Style", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(ApplyMinimalStyle), "Minimal Style", "Style Presets", true));

            items.Add(new DesignerActionHeaderItem("Layout"));
            items.Add(new DesignerActionMethodItem(this, nameof(UseRecommendedHeaderHeight), "Use Recommended Header Height", "Layout", true));

            return items;
        }
    }
}
