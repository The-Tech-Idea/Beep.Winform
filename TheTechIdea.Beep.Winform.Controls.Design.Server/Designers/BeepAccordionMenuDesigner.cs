using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Winform.Controls.AccordionMenus;
using TheTechIdea.Beep.Winform.Controls.AccordionMenus.Helpers;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    public class BeepAccordionMenuDesigner : BaseBeepControlDesigner
    {
        public BeepAccordionMenu? AccordionMenu => Component as BeepAccordionMenu;

        private bool _showEmptyHint = true;
        private bool _showPreviewHint = true;

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
            if (AccordionMenu != null)
            {
                AccordionMenu.ListItems.ListChanged += AccordionMenu_ListChanged;
                UpdateEmptyState();
            }
        }

        private void AccordionMenu_ListChanged(object? sender, ListChangedEventArgs e)
        {
            UpdateEmptyState();
        }

        private void UpdateEmptyState()
        {
            _showEmptyHint = AccordionMenu == null || AccordionMenu.ListItems.Count == 0;
        }

        protected override void Dispose(bool disposing)
        {
            if (AccordionMenu != null)
            {
                AccordionMenu.ListItems.ListChanged -= AccordionMenu_ListChanged;
            }
            base.Dispose(disposing);
        }

        protected override void OnPaintAdornments(PaintEventArgs pe)
        {
            base.OnPaintAdornments(pe);

            if (_showEmptyHint)
            {
                DrawEmptyStateHint(pe.Graphics);
            }

            if (_showPreviewHint)
            {
                DrawPreviewHint(pe.Graphics);
            }
        }

        private void DrawEmptyStateHint(Graphics g)
        {
            var bounds = Control.Bounds;
            var hintRect = new Rectangle(10, bounds.Height / 2 - 30, bounds.Width - 20, 60);

            using var brush = new SolidBrush(Color.FromArgb(15, 0, 0, 0));
            g.FillRectangle(brush, hintRect);

            using var pen = new Pen(Color.FromArgb(100, 128, 128, 128), 1)
            {
                DashStyle = DashStyle.Dash
            };
            g.DrawRectangle(pen, hintRect);

            var format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            using var font = new Font("Segoe UI", 9, FontStyle.Italic);
            using var textBrush = new SolidBrush(Color.FromArgb(150, 128, 128, 128));
            g.DrawString("Click 'Add Sample Items' to get started\nor edit ListItems in the Properties window",
                font, textBrush, hintRect, format);
        }

        private void DrawPreviewHint(Graphics g)
        {
            if (AccordionMenu == null || AccordionMenu.ListItems.Count == 0) return;

            var bounds = Control.Bounds;
            var hintRect = new Rectangle(4, bounds.Height - 22, bounds.Width - 8, 18);

            using var font = new Font("Segoe UI", 7, FontStyle.Regular);
            using var textBrush = new SolidBrush(Color.FromArgb(100, 100, 100, 100));
            var format = new StringFormat
            {
                Alignment = StringAlignment.Far,
                LineAlignment = StringAlignment.Center
            };

            string hint = $"Items: {AccordionMenu.ListItems.Count} | Style: {AccordionMenu.AccordionStyle}";
            g.DrawString(hint, font, textBrush, hintRect, format);
        }

        public override System.ComponentModel.Design.DesignerVerbCollection Verbs
        {
            get
            {
                var verbs = new System.ComponentModel.Design.DesignerVerbCollection();

                verbs.Add(new DesignerVerb("Add Sample Items", OnAddSampleItems));
                verbs.Add(new DesignerVerb("Toggle Expand/Collapse", OnToggleExpand));
                verbs.Add(new DesignerVerb("Expand All", OnExpandAll));
                verbs.Add(new DesignerVerb("Collapse All", OnCollapseAll));
                verbs.Add(new DesignerVerb("Apply Recommended Sizes", OnApplyRecommendedSizes));
                verbs.Add(new DesignerVerb("Reset to Defaults", OnResetToDefaults));

                return verbs;
            }
        }

        private void OnAddSampleItems(object? sender, EventArgs e)
        {
            if (AccordionMenu == null) return;

            var transactionService = GetService(typeof(IMenuCommandService)) as IMenuCommandService;
            var changeService = GetService(typeof(IComponentChangeService)) as IComponentChangeService;

            changeService?.OnComponentChanging(AccordionMenu, TypeDescriptor.GetProperties(AccordionMenu)["ListItems"]);

            AccordionMenu.ListItems.Clear();

            var sampleItems = new List<SimpleItem>
            {
                new SimpleItem { Text = "Dashboard", ImagePath = SvgsUI.Dashboard, Tag = "dashboard" },
                new SimpleItem { Text = "Reports", ImagePath = SvgsUI.ReportAnalytics, Tag = "reports" },
                new SimpleItem { Text = "Settings", ImagePath = SvgsUI.Settings, Tag = "settings" }
            };

            sampleItems[1].Children.Add(new SimpleItem { Text = "Monthly Report", ImagePath = SvgsUI.ChartPie, Tag = "monthly" });
            sampleItems[1].Children.Add(new SimpleItem { Text = "Annual Report", ImagePath = SvgsUI.ChartArea, Tag = "annual" });
            sampleItems[1].Children.Add(new SimpleItem { Text = "Custom Report", ImagePath = SvgsUI.ChartDots, Tag = "custom" });

            sampleItems[2].Children.Add(new SimpleItem { Text = "General", ImagePath = SvgsUI.Adjustments, Tag = "general" });
            sampleItems[2].Children.Add(new SimpleItem { Text = "Security", ImagePath = SvgsUI.Lock, Tag = "security" });
            sampleItems[2].Children.Add(new SimpleItem { Text = "Notifications", ImagePath = SvgsUI.Bell, Tag = "notifications" });

            foreach (var item in sampleItems)
            {
                AccordionMenu.ListItems.Add(item);
            }

            changeService?.OnComponentChanged(AccordionMenu, TypeDescriptor.GetProperties(AccordionMenu)["ListItems"], null, AccordionMenu.ListItems);

            UpdateEmptyState();
            Control.Invalidate();
        }

        private void OnToggleExpand(object? sender, EventArgs e)
        {
            if (AccordionMenu == null) return;

            var changeService = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
            var prop = TypeDescriptor.GetProperties(AccordionMenu)["Title"];

            changeService?.OnComponentChanging(AccordionMenu, prop);

            bool anyExpanded = false;
            foreach (var item in AccordionMenu.ListItems)
            {
                if (AccordionMenu.IsItemExpanded(item))
                {
                    anyExpanded = true;
                    break;
                }
            }

            foreach (var item in AccordionMenu.ListItems)
            {
                AccordionMenu.SetItemExpanded(item, !anyExpanded);
            }

            changeService?.OnComponentChanged(AccordionMenu, prop, !anyExpanded, anyExpanded);
            Control.Invalidate();
        }

        private void OnExpandAll(object? sender, EventArgs e)
        {
            if (AccordionMenu == null) return;

            var changeService = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
            var prop = TypeDescriptor.GetProperties(AccordionMenu)["Title"];

            changeService?.OnComponentChanging(AccordionMenu, prop);

            foreach (var item in AccordionMenu.ListItems)
            {
                AccordionMenu.SetItemExpanded(item, true);
            }

            changeService?.OnComponentChanged(AccordionMenu, prop, null, null);
            Control.Invalidate();
        }

        private void OnCollapseAll(object? sender, EventArgs e)
        {
            if (AccordionMenu == null) return;

            var changeService = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
            var prop = TypeDescriptor.GetProperties(AccordionMenu)["Title"];

            changeService?.OnComponentChanging(AccordionMenu, prop);

            foreach (var item in AccordionMenu.ListItems)
            {
                AccordionMenu.SetItemExpanded(item, false);
            }

            changeService?.OnComponentChanged(AccordionMenu, prop, null, null);
            Control.Invalidate();
        }

        private void OnApplyRecommendedSizes(object? sender, EventArgs e)
        {
            if (AccordionMenu == null) return;

            var changeService = GetService(typeof(IComponentChangeService)) as IComponentChangeService;

            var itemHeightProp = TypeDescriptor.GetProperties(AccordionMenu)["ItemHeight"];
            var childHeightProp = TypeDescriptor.GetProperties(AccordionMenu)["ChildItemHeight"];

            changeService?.OnComponentChanging(AccordionMenu, itemHeightProp);
            SetProperty("ItemHeight", AccordionStyleHelpers.GetRecommendedItemHeight(AccordionMenu.AccordionStyle));
            changeService?.OnComponentChanged(AccordionMenu, itemHeightProp, null, AccordionMenu.ItemHeight);

            changeService?.OnComponentChanging(AccordionMenu, childHeightProp);
            SetProperty("ChildItemHeight", AccordionStyleHelpers.GetRecommendedChildItemHeight(AccordionMenu.AccordionStyle));
            changeService?.OnComponentChanged(AccordionMenu, childHeightProp, null, AccordionMenu.ChildItemHeight);

            Control.Invalidate();
        }

        private void OnResetToDefaults(object? sender, EventArgs e)
        {
            if (AccordionMenu == null) return;

            var changeService = GetService(typeof(IComponentChangeService)) as IComponentChangeService;

            SetProperty("AccordionStyle", AccordionStyle.Material3);
            SetProperty("ItemHeight", 40);
            SetProperty("ChildItemHeight", 30);
            SetProperty("ExpandedWidth", 200);
            SetProperty("CollapsedWidth", 64);
            SetProperty("Title", "Accordion");
            SetProperty("AutoScrollEnabled", true);
            SetProperty("AllowItemDragDrop", true);
            SetProperty("HasItemIcons", true);

            Control.Invalidate();
        }

        protected override void PreFilterProperties(System.Collections.IDictionary properties)
        {
            base.PreFilterProperties(properties);

            string[] propsToHide = { "Margin", "ImeMode", "AccessibleDescription", "AccessibleName", "AccessibleRole" };
            foreach (var prop in propsToHide)
            {
                if (properties.Contains(prop))
                {
                    properties[prop] = TypeDescriptor.CreateProperty(
                        typeof(BeepAccordionMenu),
                        (PropertyDescriptor)properties[prop]!,
                        new BrowsableAttribute(false));
                }
            }
        }

        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            var lists = new DesignerActionListCollection();
            lists.Add(new BeepAccordionMenuActionList(this));
            return lists;
        }
    }

    public class BeepAccordionMenuActionList : DesignerActionList
    {
        private readonly BeepAccordionMenuDesigner _designer;

        public BeepAccordionMenuActionList(BeepAccordionMenuDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        protected BeepAccordionMenu? AccordionMenu => Component as BeepAccordionMenu;

        #region Properties

        [Category("Appearance")]
        public AccordionStyle AccordionStyle
        {
            get => _designer.GetProperty<AccordionStyle>("AccordionStyle");
            set => _designer.SetProperty("AccordionStyle", value);
        }

        [Category("Appearance")]
        public string Title
        {
            get => _designer.GetProperty<string>("Title");
            set => _designer.SetProperty("Title", value);
        }

        [Category("Layout")]
        public int ItemHeight
        {
            get => _designer.GetProperty<int>("ItemHeight");
            set => _designer.SetProperty("ItemHeight", value);
        }

        [Category("Layout")]
        public int ChildItemHeight
        {
            get => _designer.GetProperty<int>("ChildItemHeight");
            set => _designer.SetProperty("ChildItemHeight", value);
        }

        [Category("Layout")]
        public int ExpandedWidth
        {
            get => _designer.GetProperty<int>("ExpandedWidth");
            set => _designer.SetProperty("ExpandedWidth", value);
        }

        [Category("Layout")]
        public int CollapsedWidth
        {
            get => _designer.GetProperty<int>("CollapsedWidth");
            set => _designer.SetProperty("CollapsedWidth", value);
        }

        [Category("Behavior")]
        public bool AutoScrollEnabled
        {
            get => _designer.GetProperty<bool>("AutoScrollEnabled");
            set => _designer.SetProperty("AutoScrollEnabled", value);
        }

        [Category("Behavior")]
        public bool AutoFitHeight
        {
            get => _designer.GetProperty<bool>("AutoFitHeight");
            set => _designer.SetProperty("AutoFitHeight", value);
        }

        [Category("Behavior")]
        public bool AllowItemDragDrop
        {
            get => _designer.GetProperty<bool>("AllowItemDragDrop");
            set => _designer.SetProperty("AllowItemDragDrop", value);
        }

        [Category("Behavior")]
        public bool HasItemIcons
        {
            get => _designer.GetProperty<bool>("HasItemIcons");
            set => _designer.SetProperty("HasItemIcons", value);
        }

        [Category("Animation")]
        public int AnimationStep
        {
            get => _designer.GetProperty<int>("AnimationStep");
            set => _designer.SetProperty("AnimationStep", value);
        }

        [Category("Animation")]
        public int AnimationDelay
        {
            get => _designer.GetProperty<int>("AnimationDelay");
            set => _designer.SetProperty("AnimationDelay", value);
        }

        #endregion

        #region Style Presets

        public void ApplyMaterial3Style() => AccordionStyle = AccordionStyle.Material3;
        public void ApplyModernStyle() => AccordionStyle = AccordionStyle.Modern;
        public void ApplyClassicStyle() => AccordionStyle = AccordionStyle.Classic;
        public void ApplyMinimalStyle() => AccordionStyle = AccordionStyle.Minimal;
        public void ApplyiOSStyle() => AccordionStyle = AccordionStyle.iOS;
        public void ApplyFluent2Style() => AccordionStyle = AccordionStyle.Fluent2;

        public void SetRecommendedItemHeight()
        {
            if (AccordionMenu != null)
            {
                ItemHeight = AccordionStyleHelpers.GetRecommendedItemHeight(AccordionMenu.AccordionStyle);
                ChildItemHeight = AccordionStyleHelpers.GetRecommendedChildItemHeight(AccordionMenu.AccordionStyle);
            }
        }

        public void AddSampleItems()
        {
            if (AccordionMenu == null) return;

            AccordionMenu.ListItems.Clear();

            var item1 = new SimpleItem { Text = "Dashboard", ImagePath = SvgsUI.Dashboard, Tag = "dashboard" };
            var item2 = new SimpleItem { Text = "Reports", ImagePath = SvgsUI.ReportAnalytics, Tag = "reports" };
            var item3 = new SimpleItem { Text = "Settings", ImagePath = SvgsUI.Settings, Tag = "settings" };

            item2.Children.Add(new SimpleItem { Text = "Monthly Report", ImagePath = SvgsUI.ChartPie });
            item2.Children.Add(new SimpleItem { Text = "Annual Report", ImagePath = SvgsUI.ChartArea });
            item2.Children.Add(new SimpleItem { Text = "Custom Report", ImagePath = SvgsUI.ChartDots });

            item3.Children.Add(new SimpleItem { Text = "General", ImagePath = SvgsUI.Adjustments });
            item3.Children.Add(new SimpleItem { Text = "Security", ImagePath = SvgsUI.Lock });

            AccordionMenu.ListItems.Add(item1);
            AccordionMenu.ListItems.Add(item2);
            AccordionMenu.ListItems.Add(item3);
        }

        public void ExpandAll()
        {
            if (AccordionMenu == null) return;
            foreach (var item in AccordionMenu.ListItems)
                AccordionMenu.SetItemExpanded(item, true);
        }

        public void CollapseAll()
        {
            if (AccordionMenu == null) return;
            foreach (var item in AccordionMenu.ListItems)
                AccordionMenu.SetItemExpanded(item, false);
        }

        #endregion

        #region DesignerActionItemCollection

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Style"));
            items.Add(new DesignerActionPropertyItem("AccordionStyle", "Accordion Style", "Style", "Visual style"));

            items.Add(new DesignerActionHeaderItem("Style Presets"));
            items.Add(new DesignerActionMethodItem(this, "ApplyMaterial3Style", "Material3", "Style Presets", "Material Design 3"));
            items.Add(new DesignerActionMethodItem(this, "ApplyModernStyle", "Modern", "Style Presets", "Modern flat"));
            items.Add(new DesignerActionMethodItem(this, "ApplyClassicStyle", "Classic", "Style Presets", "Classic bordered"));
            items.Add(new DesignerActionMethodItem(this, "ApplyMinimalStyle", "Minimal", "Style Presets", "Minimal clean"));
            items.Add(new DesignerActionMethodItem(this, "ApplyiOSStyle", "iOS", "Style Presets", "iOS rounded"));
            items.Add(new DesignerActionMethodItem(this, "ApplyFluent2Style", "Fluent2", "Style Presets", "Fluent Design 2"));

            items.Add(new DesignerActionHeaderItem("Appearance"));
            items.Add(new DesignerActionPropertyItem("Title", "Title", "Appearance", "Header title"));
            items.Add(new DesignerActionPropertyItem("HasItemIcons", "Show Icons", "Appearance", "Enable item icons"));

            items.Add(new DesignerActionHeaderItem("Layout"));
            items.Add(new DesignerActionPropertyItem("ItemHeight", "Item Height", "Layout", "Header item height"));
            items.Add(new DesignerActionPropertyItem("ChildItemHeight", "Child Height", "Layout", "Child item height"));
            items.Add(new DesignerActionPropertyItem("ExpandedWidth", "Expanded Width", "Layout", "Width when expanded"));
            items.Add(new DesignerActionPropertyItem("CollapsedWidth", "Collapsed Width", "Layout", "Width when collapsed"));

            items.Add(new DesignerActionHeaderItem("Behavior"));
            items.Add(new DesignerActionPropertyItem("AutoScrollEnabled", "Auto Scroll", "Behavior", "Enable scrolling"));
            items.Add(new DesignerActionPropertyItem("AutoFitHeight", "Auto Fit Height", "Behavior", "Auto-adjust height"));
            items.Add(new DesignerActionPropertyItem("AllowItemDragDrop", "Allow Drag-Drop", "Behavior", "Enable reordering"));

            items.Add(new DesignerActionHeaderItem("Animation"));
            items.Add(new DesignerActionPropertyItem("AnimationStep", "Animation Step", "Animation", "Animation speed"));
            items.Add(new DesignerActionPropertyItem("AnimationDelay", "Animation Delay", "Animation", "Animation delay"));

            items.Add(new DesignerActionHeaderItem("Items"));
            items.Add(new DesignerActionMethodItem(this, "AddSampleItems", "Add Sample Items", "Items", "Add demo items"));
            items.Add(new DesignerActionMethodItem(this, "ExpandAll", "Expand All", "Items", "Expand all sections"));
            items.Add(new DesignerActionMethodItem(this, "CollapseAll", "Collapse All", "Items", "Collapse all sections"));

            items.Add(new DesignerActionHeaderItem("Quick Actions"));
            items.Add(new DesignerActionMethodItem(this, "SetRecommendedItemHeight", "Apply Recommended Sizes", "Quick Actions", "Set sizes for current style"));

            return items;
        }

        #endregion
    }
}
