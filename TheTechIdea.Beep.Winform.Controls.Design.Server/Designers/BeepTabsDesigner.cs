using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Tabs.Helpers;
using TheTechIdea.Beep.Winform.Controls.Tabs.Models;
using TheTechIdea.Beep.Winform.Controls.Design.Server.ActionLists;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    public enum BeepTabCloseVisibilityOption
    {
        Inherit,
        Show,
        Hide
    }

    public class BeepTabsDesigner : ParentControlDesigner, IBeepDesignerActionHost
    {
        private DesignerActionListCollection? _actionLists;
        private DesignerVerbCollection? _verbs;
        private IComponentChangeService? _changeService;

        private BeepTabs? Tabs => Component as BeepTabs;

        private const int WM_LBUTTONDOWN = 0x0201;

        public override void Initialize(IComponent component)
        {
            try
            {
                base.Initialize(component);
                _changeService = GetService(typeof(IComponentChangeService)) as IComponentChangeService;

                if (component is BeepTabs tabs)
                {
                    // Subscribe so the smart-tag panel refreshes when the active page changes
                    tabs.SelectedIndexChanged += OnTabsSelectedIndexChanged;
                    // Subscribe to ControlAdded so each new BeepTabPage gets EnableDesignMode called.
                    // ParentControlDesigner in the Design.Server SDK does not expose OnControlAdded
                    // as an overridable method, so we hook the control event directly.
                    tabs.ControlAdded += OnTabsControlAdded;
                    tabs.ControlRemoved += OnTabsControlRemoved;

                    // Enable design mode for any BeepTabPages already known to the Beep-owned
                    // page model when the designer loads an existing form. This is required so
                    // controls dropped onto the active page are parented into the page, not into
                    // BeepTabs itself.
                    int pageIndex = 0;
                    foreach (BeepTabPage existingPage in EnumerateDesignablePages(tabs))
                    {
                        string pageName = !string.IsNullOrEmpty(existingPage.Name)
                            ? existingPage.Name
                            : $"beepTabPage{++pageIndex}";
                        EnablePageDesignMode(existingPage, pageName);
                    }

                }
            }
            catch (Exception ex)
            {
                ReportDesignerError("BeepTabs designer failed to initialize.", ex);
            }
        }

        public override void InitializeNewComponent(IDictionary defaultValues)
        {
            base.InitializeNewComponent(defaultValues);

            if (Tabs == null || Tabs.TabCount > 0)
            {
                return;
            }

            ExecuteTabsAction("Add Default Page", tabs =>
            {
                BeepTabPage page = CreateDesignerPage(GetUniquePageName(1), "Page 1");
                tabs.AddPage(page, select: true);
                tabs.Invalidate();
                SyncDesignerSelection(page);
            });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && Tabs != null)
            {
                Tabs.SelectedIndexChanged -= OnTabsSelectedIndexChanged;
                Tabs.ControlAdded -= OnTabsControlAdded;
                Tabs.ControlRemoved -= OnTabsControlRemoved;
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Enables design mode for each BeepTabPage added to BeepTabs at design time so that
        /// controls dropped onto the active page are parented into the selected page.
        /// ParentControlDesigner in the Design.Server SDK has no OnControlAdded override,
        /// so this handler is wired to tabs.ControlAdded in Initialize.
        /// </summary>
        private void OnTabsControlAdded(object? sender, ControlEventArgs e)
        {
            if (e.Control is BeepTabPage page)
            {
                string pageName = !string.IsNullOrEmpty(page.Name) ? page.Name : page.Text;
                if (!string.IsNullOrEmpty(pageName))
                {
                    EnablePageDesignMode(page, pageName);
                }
            }
        }

        private void OnTabsControlRemoved(object? sender, ControlEventArgs e)
        {
            if (e.Control is not BeepTabPage || Tabs == null)
            {
                return;
            }

            Tabs.Invalidate();
            SyncDesignerSelection((object?)Tabs.SelectedPage ?? Tabs);
            RefreshDesignerActionUI();
        }

        private IEnumerable<BeepTabPage> EnumerateDesignablePages(BeepTabs tabs)
        {
            HashSet<BeepTabPage> seenPages = new HashSet<BeepTabPage>();

            foreach (BeepTabPage page in tabs.Pages)
            {
                if (seenPages.Add(page))
                {
                    yield return page;
                }
            }

            foreach (Control child in tabs.Controls)
            {
                if (child is BeepTabPage page && seenPages.Add(page))
                {
                    yield return page;
                }
            }
        }

        private void EnablePageDesignMode(BeepTabPage page, string pageName)
        {
            try { EnableDesignMode(page, pageName); }
            catch { /* ignore if already enabled or host unavailable */ }
        }

        private void OnTabsSelectedIndexChanged(object? sender, EventArgs e)
        {
            // Invalidate the designer surface and refresh smart-tag items so
            // SelectedPageTitle and page actions stay in sync with the active page.
            if (Component is Control ctrl)
            {
                ctrl.Invalidate();
            }

            RefreshDesignerActionUI();
        }

        // ── Design-time tab switching via mouse ───────────────────────────────

        /// <summary>
        /// Intercepts left-button clicks on the tab header strip so clicking a tab
        /// in the designer switches the active page — same as DevExpress / Telerik.
        /// The header check runs BEFORE base so we can return early and prevent the
        /// base designer from misrouting the click to a child.
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_LBUTTONDOWN && Tabs != null)
            {
                int lParam = m.LParam.ToInt32();
                Point clientPt = new Point(lParam & 0xFFFF, (lParam >> 16) & 0xFFFF);

                if (IsInHeaderArea(clientPt))
                {
                    int hitIndex = HitTestTabHeader(clientPt);
                    if (hitIndex >= 0 && hitIndex != Tabs.SelectedIndex)
                    {
                        ExecuteTabsAction($"Select Page {hitIndex + 1}", tabs =>
                        {
                            tabs.SelectedIndex = hitIndex;
                            tabs.Invalidate();
                        });
                        SyncDesignerSelection((object?)Tabs.SelectedPage ?? Tabs);
                        return; // Handled — do not let base misroute the header click
                    }
                }
            }

            base.WndProc(ref m);
        }

        /// <summary>
        /// Returns <see langword="true"/> for points inside the tab header strip so the
        /// designer framework routes those clicks through <see cref="WndProc"/> (for tab
        /// switching) instead of forwarding them to child controls.  Clicks in the
        /// content area return <see langword="false"/> so they reach the active Beep-owned page.
        /// </summary>
        protected override bool GetHitTest(Point point)
        {
            if (Tabs == null) return false;
            Point clientPoint = Tabs.PointToClient(point);
            return GetHeaderRect().Contains(clientPoint);
        }

        /// <summary>Returns the rectangle of the tab header strip based on HeaderPosition.</summary>
        private Rectangle GetHeaderRect()
        {
            if (Tabs == null) return Rectangle.Empty;
            Rectangle display = Tabs.DisplayRectangle;
            return Tabs.HeaderPosition switch
            {
                TabHeaderPosition.Top    => new Rectangle(0, 0, Tabs.Width, display.Y),
                TabHeaderPosition.Bottom => new Rectangle(0, display.Bottom, Tabs.Width, Tabs.Height - display.Bottom),
                TabHeaderPosition.Left   => new Rectangle(0, 0, display.X, Tabs.Height),
                TabHeaderPosition.Right  => new Rectangle(display.Right, 0, Tabs.Width - display.Right, Tabs.Height),
                _                        => Rectangle.Empty
            };
        }

        private bool IsInHeaderArea(Point clientPt) => GetHeaderRect().Contains(clientPt);

        /// <summary>Returns the zero-based index of the tab whose header bounds contain <paramref name="clientPt"/>, or -1.</summary>
        private int HitTestTabHeader(Point clientPt)
        {
            if (Tabs == null) return -1;
            for (int i = 0; i < Tabs.TabCount; i++)
            {
                if (Tabs.GetTabRect(i).Contains(clientPt))
                    return i;
            }
            return -1;
        }

        public override DesignerActionListCollection ActionLists
        {
            get
            {
                if (_actionLists != null) return _actionLists;
                _actionLists = new DesignerActionListCollection
                {
                    new CommonBeepControlActionList(this),
                    new BeepTabsActionList(this)
                };
                return _actionLists;
            }
        }

        public override DesignerVerbCollection Verbs
            => _verbs ??= new DesignerVerbCollection
            {
                new DesignerVerb("Add Page", (_, _) => AddPage()),
                new DesignerVerb("Remove Selected Page", (_, _) => RemoveSelectedPage()),
                new DesignerVerb("Move Selected Page Left", (_, _) => MoveSelectedPage(-1)),
                new DesignerVerb("Move Selected Page Right", (_, _) => MoveSelectedPage(1))
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

        public void ApplyTheme() => Tabs?.ApplyTheme();

        /// <summary>
        /// Routes controls dropped onto BeepTabs in the designer into the currently
        /// selected <see cref="BeepTabPage"/> so they are children of the active page,
        /// matching behaviour of DevExpress / Telerik tab controls.
        /// </summary>
        protected override Control GetParentForComponent(IComponent component)
        {
            BeepTabPage? targetPage = EnsureSelectedPageForDesignerContent();
            if (targetPage != null)
            {
                return targetPage;
            }

            return base.GetParentForComponent(component);
        }

        private BeepTabPage? EnsureSelectedPageForDesignerContent()
        {
            if (Tabs == null)
            {
                return null;
            }

            if (Tabs.SelectedPage != null)
            {
                return Tabs.SelectedPage;
            }

            BeepTabPage? createdPage = null;
            ExecuteTabsAction("Create Page For Dropped Control", tabs =>
            {
                if (tabs.SelectedPage != null)
                {
                    createdPage = tabs.SelectedPage;
                    return;
                }

                int pageNumber = Math.Max(1, tabs.TabCount + 1);
                string componentName = GetUniquePageName(pageNumber);
                createdPage = CreateDesignerPage(componentName, $"Page {pageNumber}");
                tabs.AddPage(createdPage, select: true);
                tabs.Invalidate();
            });

            return Tabs.SelectedPage ?? createdPage;
        }

        /// <summary>
        /// Prevents internal infrastructure controls from being dropped directly
        /// onto BeepTabs from the toolbox; tab pages are managed via verbs only.
        /// </summary>
        public override bool CanParent(Control control)
        {
            if (control?.GetType().Name is nameof(BeepTabPage) or "BeepTabHeaderHost" or "BeepTabContentHost")
            {
                return false;
            }

            return base.CanParent(control);
        }

        public string GetSelectedPageTitle()
            => Tabs?.SelectedPage?.Text ?? string.Empty;

        public void SetSelectedPageTitle(string value)
        {
            BeepTabPage? selectedPage = Tabs?.SelectedPage;
            if (selectedPage == null)
            {
                return;
            }

            SetComponentProperty(selectedPage, nameof(BeepTabPage.Text), value);
        }

        public string GetSelectedPageIconPath()
            => Tabs?.SelectedPage?.TabIconPath ?? string.Empty;

        public void SetSelectedPageIconPath(string value)
            => SetSelectedPageProperty(nameof(BeepTabPage.TabIconPath), value ?? string.Empty, "Set Selected Page Icon");

        public string GetSelectedPageSubText()
            => Tabs?.SelectedPage?.TabSubText ?? string.Empty;

        public void SetSelectedPageSubText(string value)
            => SetSelectedPageProperty(nameof(BeepTabPage.TabSubText), value ?? string.Empty, "Set Selected Page Sub Text");

        public BeepTabBadgeKind GetSelectedPageBadgeKind()
            => Tabs?.SelectedPage?.TabBadgeKind ?? BeepTabBadgeKind.None;

        public void SetSelectedPageBadgeKind(BeepTabBadgeKind value)
            => SetSelectedPageProperty(nameof(BeepTabPage.TabBadgeKind), value, "Set Selected Page Badge Kind");

        public string GetSelectedPageBadgeText()
            => Tabs?.SelectedPage?.TabBadgeText ?? string.Empty;

        public void SetSelectedPageBadgeText(string value)
            => SetSelectedPageProperty(nameof(BeepTabPage.TabBadgeText), value ?? string.Empty, "Set Selected Page Badge Text");

        public bool GetSelectedPageCanClose()
            => Tabs?.SelectedPage?.TabCanClose ?? true;

        public void SetSelectedPageCanClose(bool value)
            => SetSelectedPageProperty(nameof(BeepTabPage.TabCanClose), value, "Set Selected Page Can Close");

        public bool GetSelectedPageCanReorder()
            => Tabs?.SelectedPage?.TabCanReorder ?? true;

        public void SetSelectedPageCanReorder(bool value)
            => SetSelectedPageProperty(nameof(BeepTabPage.TabCanReorder), value, "Set Selected Page Can Reorder");

        public bool GetSelectedPageCanSelect()
            => Tabs?.SelectedPage?.TabCanSelect ?? true;

        public void SetSelectedPageCanSelect(bool value)
            => SetSelectedPageProperty(nameof(BeepTabPage.TabCanSelect), value, "Set Selected Page Can Select");

        public bool GetSelectedPageIsBusy()
            => Tabs?.SelectedPage?.TabIsBusy ?? false;

        public void SetSelectedPageIsBusy(bool value)
            => SetSelectedPageProperty(nameof(BeepTabPage.TabIsBusy), value, "Set Selected Page Busy");

        public BeepTabCloseVisibilityOption GetSelectedPageCloseButtonVisibility()
            => Tabs?.SelectedPage?.TabCloseVisible switch
            {
                true => BeepTabCloseVisibilityOption.Show,
                false => BeepTabCloseVisibilityOption.Hide,
                _ => BeepTabCloseVisibilityOption.Inherit
            };

        public void SetSelectedPageCloseButtonVisibility(BeepTabCloseVisibilityOption value)
            => SetSelectedPageProperty(
                nameof(BeepTabPage.TabCloseVisible),
                value switch
                {
                    BeepTabCloseVisibilityOption.Show => true,
                    BeepTabCloseVisibilityOption.Hide => false,
                    _ => (bool?)null
                },
                "Set Selected Page Close Button Visibility");

        public bool GetSelectedPageIsPinned()
            => Tabs?.SelectedPage?.TabIsPinned ?? false;

        public void SetSelectedPageIsPinned(bool value)
            => SetSelectedPageProperty(nameof(BeepTabPage.TabIsPinned), value, "Set Selected Page Pinned");

        public string GetSelectedPageGroupKey()
            => Tabs?.SelectedPage?.TabGroupKey ?? string.Empty;

        public void SetSelectedPageGroupKey(string value)
            => SetSelectedPageProperty(nameof(BeepTabPage.TabGroupKey), value ?? string.Empty, "Set Selected Page Group Key");

        public bool GetSelectedPageReusePreviewSlot()
            => Tabs?.SelectedPage?.TabReusePreviewSlot ?? true;

        public void SetSelectedPageReusePreviewSlot(bool value)
            => SetSelectedPageProperty(nameof(BeepTabPage.TabReusePreviewSlot), value, "Set Selected Page Reuse Preview Slot");

        public bool GetSelectedPageIsPreview()
            => Tabs?.SelectedPage?.TabIsPreview ?? false;

        public void SetSelectedPageIsPreview(bool value)
            => SetSelectedPageProperty(nameof(BeepTabPage.TabIsPreview), value, "Set Selected Page Preview");

        public bool GetSelectedPageIsDirty()
            => Tabs?.SelectedPage?.TabIsDirty ?? false;

        public void SetSelectedPageIsDirty(bool value)
            => SetSelectedPageProperty(nameof(BeepTabPage.TabIsDirty), value, "Set Selected Page Dirty");

        public void AddPage()
        {
            if (Tabs == null)
            {
                return;
            }

            BeepTabPage? createdPage = null;

            ExecuteTabsAction("Add Page", tabs =>
            {
                int pageNumber = tabs.TabCount + 1;
                string componentName = GetUniquePageName(pageNumber);
                createdPage = CreateDesignerPage(componentName, $"Page {pageNumber}");

                tabs.AddPage(createdPage, select: true);
                tabs.Invalidate();
            });

            SyncDesignerSelection((object?)createdPage ?? Tabs);
        }

        public void RemoveSelectedPage()
        {
            if (Tabs?.SelectedPage == null)
            {
                return;
            }

            BeepTabPage selectedPage = Tabs.SelectedPage;
            object selectionTarget = Tabs;
            ExecuteTabsAction($"Remove Page '{selectedPage.Text}'", tabs =>
            {
                tabs.RemovePage(selectedPage);
                selectionTarget = (object?)tabs.SelectedPage ?? tabs;

                IDesignerHost? designerHost = GetDesignerHost();
                if (designerHost != null && selectedPage.Site != null)
                {
                    designerHost.DestroyComponent(selectedPage);
                }
                else
                {
                    selectedPage.Dispose();
                }

                tabs.Invalidate();
            });

            SyncDesignerSelection(selectionTarget);
        }

        public void ClearAllPages()
        {
            if (Tabs == null || Tabs.TabCount == 0)
            {
                return;
            }

            ExecuteTabsAction("Clear Pages", tabs =>
            {
                IDesignerHost? designerHost = GetDesignerHost();
                while (tabs.TabCount > 0)
                {
                    BeepTabPage? page = tabs.GetPageAt(tabs.TabCount - 1);
                    if (page == null) break;
                    tabs.RemovePage(page);

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

        public void ResetSelectedPageMetadata()
        {
            if (Tabs == null || Tabs.SelectedIndex < 0)
            {
                return;
            }

            ExecuteTabsAction("Reset Selected Page Metadata", tabs =>
            {
                BeepTabPage? selectedPage = tabs.SelectedPage;
                if (selectedPage == null)
                {
                    return;
                }

                SetComponentProperty(selectedPage, nameof(BeepTabPage.TabIconPath), string.Empty);
                SetComponentProperty(selectedPage, nameof(BeepTabPage.TabSubText), string.Empty);
                SetComponentProperty(selectedPage, nameof(BeepTabPage.TabBadgeText), string.Empty);
                SetComponentProperty(selectedPage, nameof(BeepTabPage.TabBadgeKind), BeepTabBadgeKind.None);
                SetComponentProperty(selectedPage, nameof(BeepTabPage.TabCanClose), true);
                SetComponentProperty(selectedPage, nameof(BeepTabPage.TabCanSelect), true);
                SetComponentProperty(selectedPage, nameof(BeepTabPage.TabCanReorder), true);
                SetComponentProperty(selectedPage, nameof(BeepTabPage.TabIsBusy), false);
                SetComponentProperty(selectedPage, nameof(BeepTabPage.TabCloseVisible), null);
                SetComponentProperty(selectedPage, nameof(BeepTabPage.TabIsPinned), false);
                SetComponentProperty(selectedPage, nameof(BeepTabPage.TabIsPreview), false);
                SetComponentProperty(selectedPage, nameof(BeepTabPage.TabIsDirty), false);
                SetComponentProperty(selectedPage, nameof(BeepTabPage.TabReusePreviewSlot), true);
                SetComponentProperty(selectedPage, nameof(BeepTabPage.TabGroupKey), string.Empty);
                tabs.Invalidate();
            });

            RefreshDesignerActionUI();
        }

        public void MoveSelectedPage(int offset)
        {
            if (Tabs == null || Tabs.SelectedIndex < 0)
            {
                return;
            }

            int currentIndex = Tabs.SelectedIndex;
            int newIndex = Math.Max(0, Math.Min(Tabs.TabCount - 1, currentIndex + offset));
            if (newIndex == currentIndex)
            {
                return;
            }

            ExecuteTabsAction($"Move Page to position {newIndex + 1}", tabs =>
            {
                BeepTabPage? page = tabs.GetPageAt(currentIndex);
                if (page == null) return;
                tabs.MovePage(page, newIndex);
                tabs.Invalidate();
            });

            SyncDesignerSelection((object?)Tabs.SelectedPage ?? Tabs);
        }

        public void SelectNextPage()
        {
            if (Tabs == null || Tabs.TabCount == 0)
            {
                return;
            }

            Tabs.SelectedIndex = Math.Min(Tabs.TabCount - 1, Tabs.SelectedIndex + 1);
            Tabs.Invalidate();
            SyncDesignerSelection((object?)Tabs.SelectedPage ?? Tabs);
        }

        public void SelectPreviousPage()
        {
            if (Tabs == null || Tabs.TabCount == 0)
            {
                return;
            }

            Tabs.SelectedIndex = Math.Max(0, Tabs.SelectedIndex - 1);
            Tabs.Invalidate();
            SyncDesignerSelection((object?)Tabs.SelectedPage ?? Tabs);
        }

        private void SetComponentProperty(object component, string propertyName, object? value)
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

        private void SetSelectedPageProperty(string propertyName, object? value, string description)
        {
            if (Tabs?.SelectedPage == null)
            {
                return;
            }

            ExecuteTabsAction(description, tabs =>
            {
                BeepTabPage? selectedPage = tabs.SelectedPage;
                if (selectedPage == null)
                {
                    return;
                }

                SetComponentProperty(selectedPage, propertyName, value!);
                tabs.Invalidate();
            });

            RefreshDesignerActionUI();
        }

        private BeepTabPage CreateDesignerPage(string componentName, string text)
        {
            IDesignerHost? designerHost = GetDesignerHost();
            BeepTabPage page = designerHost?.CreateComponent(typeof(BeepTabPage), componentName) as BeepTabPage ?? new BeepTabPage();
            SetComponentProperty(page, nameof(Control.Name), componentName);
            SetComponentProperty(page, nameof(BeepTabPage.Text), text);
            return page;
        }

        private string GetUniquePageName(int suggestedIndex)
        {
            IContainer? container = GetDesignerHost()?.Container;
            int index = Math.Max(1, suggestedIndex);

            string name;
            do
            {
                name = $"beepTabPage{index}";
                index++;
            }
            while (container?.Components[name] != null);

            return name;
        }

        private IDesignerHost? GetDesignerHost()
            => GetService(typeof(IDesignerHost)) as IDesignerHost;

        private ISelectionService? GetSelectionService()
            => GetService(typeof(ISelectionService)) as ISelectionService;

        private DesignerActionUIService? GetDesignerActionUiService()
            => GetService(typeof(DesignerActionUIService)) as DesignerActionUIService;

        private void RefreshDesignerActionUI()
        {
            if (Component == null)
            {
                return;
            }

            GetDesignerActionUiService()?.Refresh(Component);
        }

        public void RefreshSmartTagPreview()
        {
            RefreshDesignerActionUI();
        }

        private void SyncDesignerSelection(object? selectionTarget)
        {
            if (selectionTarget == null)
            {
                return;
            }

            ISelectionService? selectionService = GetSelectionService();
            selectionService?.SetSelectedComponents(new object[] { selectionTarget }, SelectionTypes.Replace);
            RefreshDesignerActionUI();
        }

        private void ExecuteTabsAction(string description, Action<BeepTabs> action)
        {
            if (Tabs == null)
            {
                ReportDesignerError($"BeepTabs designer: cannot execute '{description}' — Tabs is null.", null);
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
            catch (Exception ex)
            {
                transaction?.Cancel();
                ReportDesignerError($"BeepTabs designer action '{description}' failed.", ex);
            }
        }

        /// <summary>
        /// Displays a design-time error in a MessageBox so failures are never silently swallowed.
        /// </summary>
        private static void ReportDesignerError(string context, Exception? ex)
        {
            string message = ex == null
                ? context
                : $"{context}\n\n{ex.GetType().Name}: {ex.Message}\n\nStack trace:\n{ex.StackTrace}";

            MessageBox.Show(message, "BeepTabs Designer Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // ── Property-grid filter ────────────────────────────────────────────────

        /// <summary>
        /// Hides low-value inherited properties from the Properties window so the
        /// designer surface only surfaces BeepTabs-specific configuration.
        /// </summary>
        protected override void PreFilterProperties(IDictionary properties)
        {
            base.PreFilterProperties(properties);

            string[] hidden =
            {
                "AutoScroll", "AutoScrollMargin", "AutoScrollMinSize",
                "AutoScrollOffset", "AutoScrollPosition",
                "HorizontalScroll", "VerticalScroll",
                "BackgroundImage", "BackgroundImageLayout",
            };

            foreach (string name in hidden)
            {
                if (properties[name] is PropertyDescriptor pd)
                    properties[name] = TypeDescriptor.CreateProperty(
                        pd.ComponentType, pd, new BrowsableAttribute(false));
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

        [Category("Behavior")]
        [Description("Functional mode: Navigation (static tabs), Documents (closable), Workspace (pin/preview/dirty guard).")]
        public BeepTabMode TabMode
        {
            get => _designer.GetProperty<BeepTabMode>(nameof(BeepTabs.TabMode));
            set => _designer.SetProperty(nameof(BeepTabs.TabMode), value);
        }

        [Category("Behavior")]
        [Description("Overflow policy when the tab strip is too narrow to show all tabs.")]
        public BeepTabOverflowPolicy HeaderOverflowPolicy
        {
            get => _designer.GetProperty<BeepTabOverflowPolicy>(nameof(BeepTabs.HeaderOverflowPolicy));
            set => _designer.SetProperty(nameof(BeepTabs.HeaderOverflowPolicy), value);
        }

        [Category("Behavior")]
        [Description("Maximum number of recently closed tabs remembered for Ctrl+Shift+T reopen (Documents/Workspace mode).")]
        public int MaxClosedHistory
        {
            get => _designer.GetProperty<int>(nameof(BeepTabs.MaxClosedHistory));
            set => _designer.SetProperty(nameof(BeepTabs.MaxClosedHistory), value);
        }

        [Category("Behavior")]
        [Description("Shows a close-current-tab action button in the tab strip header when a tab is selected.")]
        public bool ShowHeaderCloseCurrentAction
        {
            get => _designer.GetProperty<bool>(nameof(BeepTabs.ShowHeaderCloseCurrentAction));
            set => _designer.SetProperty(nameof(BeepTabs.ShowHeaderCloseCurrentAction), value);
        }

        [Category("Pages")]
        [Description("Caption of the currently selected page")]
        public string SelectedPageTitle
        {
            get => _designer.GetSelectedPageTitle();
            set => _designer.SetSelectedPageTitle(value);
        }

        [Category("Page Metadata")]
        [Description("Path or resource key for the selected page icon.")]
        public string SelectedPageIconPath
        {
            get => _designer.GetSelectedPageIconPath();
            set => _designer.SetSelectedPageIconPath(value);
        }

        [Category("Page Metadata")]
        [Description("Optional secondary text displayed with the selected page title.")]
        public string SelectedPageSubText
        {
            get => _designer.GetSelectedPageSubText();
            set => _designer.SetSelectedPageSubText(value);
        }

        [Category("Page Metadata")]
        [Description("Visual badge kind shown on the selected page header.")]
        public BeepTabBadgeKind SelectedPageBadgeKind
        {
            get => _designer.GetSelectedPageBadgeKind();
            set => _designer.SetSelectedPageBadgeKind(value);
        }

        [Category("Page Metadata")]
        [Description("Badge text shown on the selected page header.")]
        public string SelectedPageBadgeText
        {
            get => _designer.GetSelectedPageBadgeText();
            set => _designer.SetSelectedPageBadgeText(value);
        }

        [Category("Page Metadata")]
        [Description("Determines whether the selected page can be closed.")]
        public bool SelectedPageCanClose
        {
            get => _designer.GetSelectedPageCanClose();
            set => _designer.SetSelectedPageCanClose(value);
        }

        [Category("Page Metadata")]
        [Description("Determines whether the selected page can be reordered.")]
        public bool SelectedPageCanReorder
        {
            get => _designer.GetSelectedPageCanReorder();
            set => _designer.SetSelectedPageCanReorder(value);
        }

        [Category("Page Metadata")]
        [Description("Determines whether the selected page can be selected from the header.")]
        public bool SelectedPageCanSelect
        {
            get => _designer.GetSelectedPageCanSelect();
            set => _designer.SetSelectedPageCanSelect(value);
        }

        [Category("Page Metadata")]
        [Description("Marks the selected page as busy so the header renders its activity indicator.")]
        public bool SelectedPageIsBusy
        {
            get => _designer.GetSelectedPageIsBusy();
            set => _designer.SetSelectedPageIsBusy(value);
        }

        [Category("Page Metadata")]
        [Description("Overrides close-button visibility for the selected page: inherit the host setting, force show, or force hide.")]
        public BeepTabCloseVisibilityOption SelectedPageCloseButtonVisibility
        {
            get => _designer.GetSelectedPageCloseButtonVisibility();
            set => _designer.SetSelectedPageCloseButtonVisibility(value);
        }

        [Category("Workspace State")]
        [Description("Marks the selected page as pinned in document/workspace modes.")]
        public bool SelectedPageIsPinned
        {
            get => _designer.GetSelectedPageIsPinned();
            set => _designer.SetSelectedPageIsPinned(value);
        }

        [Category("Workspace State")]
        [Description("Workspace/document grouping key for the selected page.")]
        public string SelectedPageGroupKey
        {
            get => _designer.GetSelectedPageGroupKey();
            set => _designer.SetSelectedPageGroupKey(value);
        }

        [Category("Workspace State")]
        [Description("Controls whether the selected preview page can be reused as the workspace preview slot.")]
        public bool SelectedPageReusePreviewSlot
        {
            get => _designer.GetSelectedPageReusePreviewSlot();
            set => _designer.SetSelectedPageReusePreviewSlot(value);
        }

        [Category("Workspace State")]
        [Description("Marks the selected page as the preview slot in workspace mode.")]
        public bool SelectedPageIsPreview
        {
            get => _designer.GetSelectedPageIsPreview();
            set => _designer.SetSelectedPageIsPreview(value);
        }

        [Category("Workspace State")]
        [Description("Marks the selected page as dirty so the header renders its modified state.")]
        public bool SelectedPageIsDirty
        {
            get => _designer.GetSelectedPageIsDirty();
            set => _designer.SetSelectedPageIsDirty(value);
        }

        public void ApplyClassicStyle() => TabStyle = TabStyle.Classic;
        public void ApplyUnderlineStyle() => TabStyle = TabStyle.Underline;
        public void ApplyCapsuleStyle() => TabStyle = TabStyle.Capsule;
        public void ApplyCardStyle() => TabStyle = TabStyle.Card;
        public void ApplyMinimalStyle() => TabStyle = TabStyle.Minimal;

        /// <summary>Configure the tab control for static navigation (no close buttons, Navigation mode).</summary>
        public void ApplyNavigationMode()
        {
            TabMode = BeepTabMode.Navigation;
            _designer.SetProperty(nameof(BeepTabs.ShowCloseButtons), false);
            _designer.SetProperty(nameof(BeepTabs.HeaderOverflowPolicy), BeepTabOverflowPolicy.None);
        }

        /// <summary>Configure the tab control for document management (closable tabs, overflow menu, Documents mode).</summary>
        public void ApplyDocumentsMode()
        {
            TabMode = BeepTabMode.Documents;
            _designer.SetProperty(nameof(BeepTabs.ShowCloseButtons), true);
            _designer.SetProperty(nameof(BeepTabs.HeaderOverflowPolicy), BeepTabOverflowPolicy.OverflowMenu);
        }

        /// <summary>Configure the tab control for a full workspace (pin, preview, dirty guard, overflow menu, Workspace mode).</summary>
        public void ApplyWorkspaceMode()
        {
            TabMode = BeepTabMode.Workspace;
            _designer.SetProperty(nameof(BeepTabs.ShowCloseButtons), true);
            _designer.SetProperty(nameof(BeepTabs.HeaderOverflowPolicy), BeepTabOverflowPolicy.OverflowMenu);
        }

        public void UseRecommendedHeaderHeight()
        {
            if (Tabs != null)
            {
                HeaderHeight = TabStyleHelpers.GetRecommendedHeaderHeight(Tabs.TabStyle);
            }
        }

        public void AddPage() => _designer.AddPage();
        public void RemoveSelectedPage() => _designer.RemoveSelectedPage();
        public void MoveSelectedPageLeft() => _designer.MoveSelectedPage(-1);
        public void MoveSelectedPageRight() => _designer.MoveSelectedPage(1);
        public void SelectPreviousPage() => _designer.SelectPreviousPage();
        public void SelectNextPage() => _designer.SelectNextPage();
        public void ClearAllPages() => _designer.ClearAllPages();
        public void ResetSelectedPageMetadata() => _designer.ResetSelectedPageMetadata();
        public void RefreshPreview() => _designer.RefreshSmartTagPreview();

        private string GetHeaderActionsPreviewText()
        {
            if (Tabs == null)
            {
                return "Header actions: unavailable.";
            }

            object? result = InvokeNonPublicInstanceMethod(Tabs, "GetHeaderActionSlots");
            if (result is not BeepTabHeaderAction[] actions || actions.Length == 0)
            {
                return "Header actions: none.";
            }

            List<string> visibleActions = new List<string>();
            foreach (BeepTabHeaderAction action in actions)
            {
                if (!action.IsActionSlot || !action.IsVisible)
                {
                    continue;
                }

                visibleActions.Add(FormatHeaderActionLabel(action.ActionKind));
            }

            return visibleActions.Count == 0
                ? "Header actions: none."
                : $"Header actions: {string.Join(", ", visibleActions)}.";
        }

        private string GetOverflowPreviewText()
        {
            if (Tabs == null)
            {
                return "Overflow preview: unavailable.";
            }

            if (Tabs.HeaderOverflowPolicy == BeepTabOverflowPolicy.None)
            {
                return "Overflow preview: disabled by policy.";
            }

            object? overflowState = InvokeNonPublicInstanceMethod(Tabs, "GetHeaderOverflowState");
            if (!TryGetPropertyValue(overflowState, "HasOverflow", out bool hasOverflow) || !hasOverflow)
            {
                return $"Overflow preview: no hidden pages ({Tabs.HeaderOverflowPolicy}).";
            }

            if (!TryGetPropertyValue(overflowState, "OverflowItemCount", out int overflowItemCount))
            {
                overflowItemCount = 0;
            }

            object? overflowItemsResult = InvokeNonPublicInstanceMethod(Tabs, "GetOverflowedItems");
            if (overflowItemsResult is BeepTabItem[] overflowItems && overflowItems.Length > 0)
            {
                int previewCount = Math.Min(3, overflowItems.Length);
                List<string> titles = new List<string>(previewCount);
                for (int index = 0; index < previewCount; index++)
                {
                    string title = string.IsNullOrWhiteSpace(overflowItems[index].Title)
                        ? overflowItems[index].Name
                        : overflowItems[index].Title;
                    titles.Add(title);
                }

                string suffix = overflowItems.Length > previewCount ? ", ..." : string.Empty;
                return $"Overflow preview: {Math.Max(overflowItemCount, overflowItems.Length)} hidden page(s) [{string.Join(", ", titles)}{suffix}].";
            }

            return $"Overflow preview: {overflowItemCount} hidden page(s).";
        }

        private static string FormatHeaderActionLabel(BeepTabHeaderActionKind actionKind)
        {
            return actionKind switch
            {
                BeepTabHeaderActionKind.CloseCurrent => "Close current",
                BeepTabHeaderActionKind.Overflow => "Overflow menu",
                BeepTabHeaderActionKind.AddTab => "Add page",
                BeepTabHeaderActionKind.ScrollBackward => "Scroll backward",
                BeepTabHeaderActionKind.ScrollForward => "Scroll forward",
                _ => actionKind.ToString()
            };
        }

        private static object? InvokeNonPublicInstanceMethod(object? target, string methodName)
        {
            if (target == null)
            {
                return null;
            }

            MethodInfo? method = target.GetType().GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.NonPublic,
                binder: null,
                types: Type.EmptyTypes,
                modifiers: null);

            if (method == null)
            {
                return null;
            }

            try
            {
                return method.Invoke(target, Array.Empty<object>());
            }
            catch
            {
                return null;
            }
        }

        private static bool TryGetPropertyValue<T>(object? target, string propertyName, out T value)
        {
            value = default!;

            if (target == null)
            {
                return false;
            }

            PropertyInfo? property = target.GetType().GetProperty(
                propertyName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (property == null)
            {
                return false;
            }

            object? rawValue = property.GetValue(target);
            if (rawValue is not T typedValue)
            {
                return false;
            }

            value = typedValue;
            return true;
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Tabs"));
            items.Add(new DesignerActionPropertyItem(nameof(TabStyle), "Tab Style", "Tabs"));
            items.Add(new DesignerActionPropertyItem(nameof(ShowCloseButtons), "Show Close Buttons", "Tabs"));

            items.Add(new DesignerActionHeaderItem("Behavior"));
            items.Add(new DesignerActionPropertyItem(nameof(TabMode), "Tab Mode", "Behavior"));
            items.Add(new DesignerActionPropertyItem(nameof(HeaderOverflowPolicy), "Overflow Policy", "Behavior"));
            items.Add(new DesignerActionPropertyItem(nameof(MaxClosedHistory), "Max Closed History", "Behavior"));
            items.Add(new DesignerActionPropertyItem(nameof(ShowHeaderCloseCurrentAction), "Show Header Close Action", "Behavior"));

            items.Add(new DesignerActionHeaderItem("Appearance"));
            items.Add(new DesignerActionPropertyItem(nameof(HeaderHeight), "Header Height", "Appearance"));
            items.Add(new DesignerActionPropertyItem(nameof(MinTouchTargetWidth), "Min Touch Target Width", "Appearance"));
            items.Add(new DesignerActionPropertyItem(nameof(TabTextVisibility), "Tab Text Visibility", "Appearance"));
            items.Add(new DesignerActionPropertyItem(nameof(HeaderPosition), "Header Position", "Appearance"));
            items.Add(new DesignerActionPropertyItem(nameof(Theme), "Theme", "Appearance"));

            items.Add(new DesignerActionHeaderItem("Pages"));
            items.Add(new DesignerActionPropertyItem(nameof(SelectedPageTitle), "Selected Page Title", "Pages", "Edit the caption of the currently selected page."));
            items.Add(new DesignerActionMethodItem(this, nameof(AddPage), "Add Page", "Pages", true));
            items.Add(new DesignerActionMethodItem(this, nameof(RemoveSelectedPage), "Remove Selected Page", "Pages", true));
            items.Add(new DesignerActionMethodItem(this, nameof(MoveSelectedPageLeft), "Move Selected Page Left", "Pages", true));
            items.Add(new DesignerActionMethodItem(this, nameof(MoveSelectedPageRight), "Move Selected Page Right", "Pages", true));
            items.Add(new DesignerActionMethodItem(this, nameof(SelectPreviousPage), "Select Previous Page", "Pages", true));
            items.Add(new DesignerActionMethodItem(this, nameof(SelectNextPage), "Select Next Page", "Pages", true));
            items.Add(new DesignerActionMethodItem(this, nameof(ClearAllPages), "Clear All Pages", "Pages", true));

            items.Add(new DesignerActionHeaderItem("Page Metadata"));
            items.Add(new DesignerActionPropertyItem(nameof(SelectedPageIconPath), "Selected Page Icon Path", "Page Metadata", "Set the icon path or resource key for the selected page."));
            items.Add(new DesignerActionPropertyItem(nameof(SelectedPageSubText), "Selected Page Sub Text", "Page Metadata", "Set the secondary text shown for the selected page."));
            items.Add(new DesignerActionPropertyItem(nameof(SelectedPageBadgeKind), "Selected Page Badge Kind", "Page Metadata", "Choose the badge style shown on the selected page."));
            items.Add(new DesignerActionPropertyItem(nameof(SelectedPageBadgeText), "Selected Page Badge Text", "Page Metadata", "Set the badge text shown on the selected page."));
            items.Add(new DesignerActionPropertyItem(nameof(SelectedPageCanClose), "Selected Page Can Close", "Page Metadata", "Control whether the selected page can be closed."));
            items.Add(new DesignerActionPropertyItem(nameof(SelectedPageCanReorder), "Selected Page Can Reorder", "Page Metadata", "Control whether the selected page can be reordered."));
            items.Add(new DesignerActionPropertyItem(nameof(SelectedPageCanSelect), "Selected Page Can Select", "Page Metadata", "Control whether the selected page can be selected from the header."));
            items.Add(new DesignerActionPropertyItem(nameof(SelectedPageIsBusy), "Selected Page Is Busy", "Page Metadata", "Mark the selected page as busy so the header shows its activity indicator."));
            items.Add(new DesignerActionPropertyItem(nameof(SelectedPageCloseButtonVisibility), "Selected Page Close Button Visibility", "Page Metadata", "Override close-button visibility for the selected page."));
            items.Add(new DesignerActionMethodItem(this, nameof(ResetSelectedPageMetadata), "Reset Selected Page Metadata", "Page Metadata", true));

            items.Add(new DesignerActionHeaderItem("Workspace State"));
            items.Add(new DesignerActionPropertyItem(nameof(SelectedPageIsPinned), "Selected Page Is Pinned", "Workspace State", "Mark the selected page as pinned in document/workspace modes."));
            items.Add(new DesignerActionPropertyItem(nameof(SelectedPageGroupKey), "Selected Page Group Key", "Workspace State", "Set the workspace/document grouping key for the selected page."));
            items.Add(new DesignerActionPropertyItem(nameof(SelectedPageIsPreview), "Selected Page Is Preview", "Workspace State", "Mark the selected page as the preview slot in workspace mode."));
            items.Add(new DesignerActionPropertyItem(nameof(SelectedPageReusePreviewSlot), "Selected Page Reuse Preview Slot", "Workspace State", "Control whether the selected preview page can be reused as the workspace preview slot."));
            items.Add(new DesignerActionPropertyItem(nameof(SelectedPageIsDirty), "Selected Page Is Dirty", "Workspace State", "Mark the selected page as dirty so the header shows modified state."));

            items.Add(new DesignerActionHeaderItem("Preview"));
            items.Add(new DesignerActionTextItem(GetHeaderActionsPreviewText(), "Preview"));
            items.Add(new DesignerActionTextItem(GetOverflowPreviewText(), "Preview"));
            items.Add(new DesignerActionMethodItem(this, nameof(RefreshPreview), "Refresh Preview", "Preview", true));

            items.Add(new DesignerActionHeaderItem("Mode Presets"));
            items.Add(new DesignerActionMethodItem(this, nameof(ApplyNavigationMode), "Navigation Mode", "Mode Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(ApplyDocumentsMode), "Documents Mode", "Mode Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(ApplyWorkspaceMode), "Workspace Mode", "Mode Presets", true));

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
