using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls.Widgets;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    public class BeepNavigationWidgetDesigner : BaseWidgetDesigner
    {
        public BeepNavigationWidget? NavigationWidget => Component as BeepNavigationWidget;

        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            var lists = new DesignerActionListCollection();
            lists.Add(new BeepNavigationWidgetActionList(this));
            return lists;
        }
    }

    public class BeepNavigationWidgetActionList : DesignerActionList
    {
        private readonly BeepNavigationWidgetDesigner _designer;

        public BeepNavigationWidgetActionList(BeepNavigationWidgetDesigner designer) : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        [Category("Navigation")]
        [Description("Visual style of the navigation widget")]
        public NavigationWidgetStyle Style
        {
            get => _designer.GetProperty<NavigationWidgetStyle>("Style");
            set => _designer.SetProperty("Style", value);
        }

        [Category("Navigation")]
        [Description("Title of the navigation")]
        public string Title
        {
            get => _designer.GetProperty<string>("Title") ?? string.Empty;
            set => _designer.SetProperty("Title", value);
        }

        [Category("Navigation")]
        [Description("Show icons for navigation items")]
        public bool ShowIcons
        {
            get => _designer.GetProperty<bool>("ShowIcons");
            set => _designer.SetProperty("ShowIcons", value);
        }

        [Category("Navigation")]
        [Description("Arrange navigation items horizontally or vertically")]
        public bool IsHorizontal
        {
            get => _designer.GetProperty<bool>("IsHorizontal");
            set => _designer.SetProperty("IsHorizontal", value);
        }

        [Category("Navigation")]
        [Description("Current selected item index")]
        public int CurrentIndex
        {
            get => _designer.GetProperty<int>("CurrentIndex");
            set => _designer.SetProperty("CurrentIndex", value);
        }

        public void ConfigureAsSidebarNav()
        {
            Style = NavigationWidgetStyle.SidebarNav;
            IsHorizontal = false;
            ShowIcons = true;
        }

        public void ConfigureAsMenuBar()
        {
            Style = NavigationWidgetStyle.MenuBar;
            IsHorizontal = true;
            ShowIcons = true;
        }

        public void ConfigureAsPagination()
        {
            Style = NavigationWidgetStyle.Pagination;
            IsHorizontal = true;
            ShowIcons = false;
        }

        public void ConfigureAsBreadcrumb()
        {
            Style = NavigationWidgetStyle.Breadcrumb;
            IsHorizontal = true;
            ShowIcons = false;
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();
            items.Add(new DesignerActionHeaderItem("Style Presets"));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsSidebarNav", "Sidebar Nav", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsMenuBar", "Menu Bar", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsPagination", "Pagination", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsBreadcrumb", "Breadcrumb", "Style Presets", true));
            items.Add(new DesignerActionHeaderItem("Properties"));
            items.Add(new DesignerActionPropertyItem("Style", "Style", "Properties"));
            items.Add(new DesignerActionPropertyItem("Title", "Title", "Properties"));
            items.Add(new DesignerActionPropertyItem("ShowIcons", "Show Icons", "Properties"));
            items.Add(new DesignerActionPropertyItem("IsHorizontal", "Is Horizontal", "Properties"));
            items.Add(new DesignerActionPropertyItem("CurrentIndex", "Current Index", "Properties"));
            return items;
        }
    }
}
