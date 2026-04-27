using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers.Actions;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers.Ribbon
{
    public class BeepRibbonControlDesigner : BaseBeepControlDesigner
    {
        private DesignerVerbCollection? _verbs;

        public global::TheTechIdea.Beep.Winform.Controls.BeepRibbonControl? Ribbon => Component as global::TheTechIdea.Beep.Winform.Controls.BeepRibbonControl;

        public DesignerVerbCollection CustomVerbs
        {
            get
            {
                if (_verbs == null)
                {
                    _verbs = new DesignerVerbCollection
                    {
                        new DesignerVerb("Toggle Minimize", OnToggleMinimize),
                        new DesignerVerb("Load Standard Backstage", OnLoadBackstage),
                        new DesignerVerb("Add Sample Tab", OnAddSampleTab),
                        new DesignerVerb("Customize Ribbon", OnCustomizeRibbon)
                    };
                }
                return _verbs;
            }
        }

        public override DesignerVerbCollection Verbs => CustomVerbs;

        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            var lists = new DesignerActionListCollection();
            lists.Add(new BeepRibbonControlActionList(this));
            return lists;
        }

        private void OnToggleMinimize(object? sender, EventArgs e) => ToggleMinimize();
        private void OnLoadBackstage(object? sender, EventArgs e) => LoadStandardBackstage();
        private void OnAddSampleTab(object? sender, EventArgs e) => AddSampleTab();
        private void OnCustomizeRibbon(object? sender, EventArgs e) => CustomizeRibbon();

        public void ToggleMinimize()
        {
            if (Ribbon == null) return;
            Ribbon.ToggleMinimized();
        }

        public void LoadStandardBackstage()
        {
            if (Ribbon == null) return;
            Ribbon.LoadStandardBackstageTemplate("Application");
        }

        public void AddSampleTab()
        {
            if (Ribbon == null) return;
            var tabPage = new TabPage("Sample Tab");
            Ribbon.Tabs.TabPages.Add(tabPage);
        }

        public void CustomizeRibbon()
        {
            if (Ribbon == null) return;
            var menuItem = Ribbon.CreateCustomizeRibbonMenuItem();
            menuItem.PerformClick();
        }
    }

    public class BeepRibbonControlActionList : DesignerActionList
    {
        private readonly BeepRibbonControlDesigner _designer;

        public BeepRibbonControlActionList(BeepRibbonControlDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        protected global::TheTechIdea.Beep.Winform.Controls.BeepRibbonControl? Ribbon => Component as global::TheTechIdea.Beep.Winform.Controls.BeepRibbonControl;

        #region Properties

        [Category("Appearance")]
        [Description("Use dark mode theme")]
        public bool DarkMode
        {
            get => _designer.GetProperty<bool>("DarkMode");
            set => _designer.SetProperty("DarkMode", value);
        }

        [Category("Appearance")]
        [Description("Follow global form style")]
        public bool FollowGlobalFormStyle
        {
            get => _designer.GetProperty<bool>("FollowGlobalFormStyle");
            set => _designer.SetProperty("FollowGlobalFormStyle", value);
        }

        [Category("Layout")]
        [Description("Ribbon layout mode")]
        public global::TheTechIdea.Beep.Winform.Controls.RibbonLayoutMode LayoutMode
        {
            get => _designer.GetProperty<global::TheTechIdea.Beep.Winform.Controls.RibbonLayoutMode>("LayoutMode");
            set => _designer.SetProperty("LayoutMode", value);
        }

        [Category("Layout")]
        [Description("Ribbon density profile")]
        public global::TheTechIdea.Beep.Winform.Controls.RibbonDensity Density
        {
            get => _designer.GetProperty<global::TheTechIdea.Beep.Winform.Controls.RibbonDensity>("Density");
            set => _designer.SetProperty("Density", value);
        }

        [Category("Behavior")]
        [Description("Allow ribbon minimize")]
        public bool AllowMinimize
        {
            get => _designer.GetProperty<bool>("AllowMinimize");
            set => _designer.SetProperty("AllowMinimize", value);
        }

        [Category("Behavior")]
        [Description("Quick access toolbar above ribbon")]
        public bool QuickAccessAboveRibbon
        {
            get => _designer.GetProperty<bool>("QuickAccessAboveRibbon");
            set => _designer.SetProperty("QuickAccessAboveRibbon", value);
        }

        [Category("Behavior")]
        [Description("Enable key tips")]
        public bool EnableKeyTips
        {
            get => _designer.GetProperty<bool>("EnableKeyTips");
            set => _designer.SetProperty("EnableKeyTips", value);
        }

        [Category("Behavior")]
        [Description("Use super tooltips")]
        public bool UseSuperToolTips
        {
            get => _designer.GetProperty<bool>("UseSuperToolTips");
            set => _designer.SetProperty("UseSuperToolTips", value);
        }

        [Category("Search")]
        [Description("Ribbon search mode")]
        public global::TheTechIdea.Beep.Winform.Controls.RibbonSearchMode SearchMode
        {
            get => _designer.GetProperty<global::TheTechIdea.Beep.Winform.Controls.RibbonSearchMode>("SearchMode");
            set => _designer.SetProperty("SearchMode", value);
        }

        [Category("Accessibility")]
        [Description("Respect system reduced motion")]
        public bool RespectSystemReducedMotion
        {
            get => _designer.GetProperty<bool>("RespectSystemReducedMotion");
            set => _designer.SetProperty("RespectSystemReducedMotion", value);
        }

        #endregion

        #region Actions

        public void ToggleMinimize() => _designer.ToggleMinimize();
        public void LoadStandardBackstage() => _designer.LoadStandardBackstage();
        public void AddSampleTab() => _designer.AddSampleTab();
        public void CustomizeRibbon() => _designer.CustomizeRibbon();

        public void SetLayoutClassic() => LayoutMode = global::TheTechIdea.Beep.Winform.Controls.RibbonLayoutMode.Classic;
        public void SetLayoutSimplified() => LayoutMode = global::TheTechIdea.Beep.Winform.Controls.RibbonLayoutMode.Simplified;

        public void SetDensityComfortable() => Density = global::TheTechIdea.Beep.Winform.Controls.RibbonDensity.Comfortable;
        public void SetDensityCompact() => Density = global::TheTechIdea.Beep.Winform.Controls.RibbonDensity.Compact;
        public void SetDensityTouch() => Density = global::TheTechIdea.Beep.Winform.Controls.RibbonDensity.Touch;

        public void SetSearchOff() => SearchMode = global::TheTechIdea.Beep.Winform.Controls.RibbonSearchMode.Off;
        public void SetSearchLocal() => SearchMode = global::TheTechIdea.Beep.Winform.Controls.RibbonSearchMode.Local;

        public void EnableDarkMode() => DarkMode = true;
        public void EnableLightMode() => DarkMode = false;

        #endregion

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Appearance"));
            items.Add(new DesignerActionPropertyItem("DarkMode", "Dark Mode:", "Appearance"));
            items.Add(new DesignerActionPropertyItem("FollowGlobalFormStyle", "Follow Global Style:", "Appearance"));

            items.Add(new DesignerActionHeaderItem("Layout"));
            items.Add(new DesignerActionPropertyItem("LayoutMode", "Layout Mode:", "Layout"));
            items.Add(new DesignerActionPropertyItem("Density", "Density:", "Layout"));
            items.Add(new DesignerActionPropertyItem("QuickAccessAboveRibbon", "QAT Above Ribbon:", "Layout"));

            items.Add(new DesignerActionHeaderItem("Behavior"));
            items.Add(new DesignerActionPropertyItem("AllowMinimize", "Allow Minimize:", "Behavior"));
            items.Add(new DesignerActionPropertyItem("EnableKeyTips", "Enable KeyTips:", "Behavior"));
            items.Add(new DesignerActionPropertyItem("UseSuperToolTips", "Super ToolTips:", "Behavior"));

            items.Add(new DesignerActionHeaderItem("Search"));
            items.Add(new DesignerActionPropertyItem("SearchMode", "Search Mode:", "Search"));

            items.Add(new DesignerActionHeaderItem("Accessibility"));
            items.Add(new DesignerActionPropertyItem("RespectSystemReducedMotion", "Reduced Motion:", "Accessibility"));

            items.Add(new DesignerActionHeaderItem("Layout Presets"));
            items.Add(new DesignerActionMethodItem(this, "SetLayoutClassic", "Classic Layout", "Layout Presets", true));
            items.Add(new DesignerActionMethodItem(this, "SetLayoutSimplified", "Simplified Layout", "Layout Presets", false));

            items.Add(new DesignerActionHeaderItem("Density Presets"));
            items.Add(new DesignerActionMethodItem(this, "SetDensityComfortable", "Comfortable", "Density Presets", true));
            items.Add(new DesignerActionMethodItem(this, "SetDensityCompact", "Compact", "Density Presets", false));
            items.Add(new DesignerActionMethodItem(this, "SetDensityTouch", "Touch", "Density Presets", false));

            items.Add(new DesignerActionHeaderItem("Theme Presets"));
            items.Add(new DesignerActionMethodItem(this, "EnableDarkMode", "Dark Theme", "Theme Presets", false));
            items.Add(new DesignerActionMethodItem(this, "EnableLightMode", "Light Theme", "Theme Presets", true));

            items.Add(new DesignerActionHeaderItem("Actions"));
            items.Add(new DesignerActionMethodItem(this, "ToggleMinimize", "Toggle Minimize", "Actions", true));
            items.Add(new DesignerActionMethodItem(this, "LoadStandardBackstage", "Load Backstage Template", "Actions", false));
            items.Add(new DesignerActionMethodItem(this, "AddSampleTab", "Add Sample Tab", "Actions", false));
            items.Add(new DesignerActionMethodItem(this, "CustomizeRibbon", "Customize Ribbon", "Actions", false));

            return items;
        }
    }
}
