using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls.AppBars;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    public class BeepWebHeaderAppBarDesigner : BaseBeepControlDesigner
    {
        public BeepWebHeaderAppBar? AppBar => Component as BeepWebHeaderAppBar;

        private bool _showEmptyHint = true;

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
            if (AppBar != null)
            {
                AppBar.Tabs.ListChanged += Tabs_ListChanged;
                UpdateEmptyState();
            }
        }

        private void Tabs_ListChanged(object? sender, ListChangedEventArgs e)
        {
            UpdateEmptyState();
        }

        private void UpdateEmptyState()
        {
            _showEmptyHint = AppBar == null || AppBar.Tabs.Count == 0;
        }

        protected override void Dispose(bool disposing)
        {
            if (AppBar != null)
                AppBar.Tabs.ListChanged -= Tabs_ListChanged;
            base.Dispose(disposing);
        }

        protected override void OnPaintAdornments(PaintEventArgs pe)
        {
            base.OnPaintAdornments(pe);

            if (_showEmptyHint)
                DrawEmptyStateHint(pe.Graphics);

            if (AppBar != null && AppBar.Tabs.Count > 0)
                DrawPreviewHint(pe.Graphics);
        }

        private void DrawEmptyStateHint(Graphics g)
        {
            var bounds = Control.Bounds;
            var hintRect = new Rectangle(10, bounds.Height / 2 - 30, bounds.Width - 20, 60);

            using var brush = new SolidBrush(Color.FromArgb(15, 0, 0, 0));
            g.FillRectangle(brush, hintRect);

            using var pen = new Pen(Color.FromArgb(100, 128, 128, 128), 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash };
            g.DrawRectangle(pen, hintRect);

            var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            using var font = new Font("Segoe UI", 9, FontStyle.Italic);
            using var textBrush = new SolidBrush(Color.FromArgb(150, 128, 128, 128));
            g.DrawString("Click 'Add Sample Tabs' to get started\nor edit Tabs in the Properties window",
                font, textBrush, hintRect, format);
        }

        private void DrawPreviewHint(Graphics g)
        {
            if (AppBar == null) return;

            var bounds = Control.Bounds;
            var hintRect = new Rectangle(4, bounds.Height - 20, bounds.Width - 8, 16);

            using var font = new Font("Segoe UI", 7, FontStyle.Regular);
            using var textBrush = new SolidBrush(Color.FromArgb(100, 100, 100, 100));
            var format = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };

            string hint = $"Tabs: {AppBar.Tabs.Count} | Buttons: {AppBar.ActionButtons.Count} | Style: {AppBar.HeaderStyle}";
            g.DrawString(hint, font, textBrush, hintRect, format);
        }

        public override DesignerVerbCollection Verbs
        {
            get
            {
                var verbs = new DesignerVerbCollection();
                verbs.Add(new DesignerVerb("Add Sample Tabs", OnAddSampleTabs));
                verbs.Add(new DesignerVerb("Add Sample Buttons", OnAddSampleButtons));
                verbs.Add(new DesignerVerb("Select First Tab", OnSelectFirstTab));
                verbs.Add(new DesignerVerb("Select Last Tab", OnSelectLastTab));
                verbs.Add(new DesignerVerb("Toggle Indicator Style", OnToggleIndicatorStyle));
                verbs.Add(new DesignerVerb("Reset to Defaults", OnResetToDefaults));
                return verbs;
            }
        }

        private void OnAddSampleTabs(object? sender, EventArgs e)
        {
            if (AppBar == null) return;
            var changeService = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
            var prop = TypeDescriptor.GetProperties(AppBar)["Tabs"];

            changeService?.OnComponentChanging(AppBar, prop);

            AppBar.Tabs.Clear();
            AppBar.AddTab("Home");
            AppBar.AddTab("Products");
            AppBar.AddTab("Services");
            AppBar.AddTab("About");
            AppBar.AddTab("Contact");

            changeService?.OnComponentChanged(AppBar, prop, null, AppBar.Tabs);
            UpdateEmptyState();
            Control.Invalidate();
        }

        private void OnAddSampleButtons(object? sender, EventArgs e)
        {
            if (AppBar == null) return;
            var changeService = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
            var prop = TypeDescriptor.GetProperties(AppBar)["ActionButtons"];

            changeService?.OnComponentChanging(AppBar, prop);

            AppBar.ActionButtons.Clear();
            AppBar.AddActionButton("Sign In");
            AppBar.AddActionButton("Get Started");

            changeService?.OnComponentChanged(AppBar, prop, null, AppBar.ActionButtons);
            Control.Invalidate();
        }

        private void OnSelectFirstTab(object? sender, EventArgs e)
        {
            if (AppBar != null && AppBar.Tabs.Count > 0)
                SetProperty("SelectedTabIndex", 0);
        }

        private void OnSelectLastTab(object? sender, EventArgs e)
        {
            if (AppBar != null && AppBar.Tabs.Count > 0)
                SetProperty("SelectedTabIndex", AppBar.Tabs.Count - 1);
        }

        private void OnToggleIndicatorStyle(object? sender, EventArgs e)
        {
            if (AppBar == null) return;
            var current = AppBar.IndicatorStyle;
            var next = current switch
            {
                TabIndicatorStyle.UnderlineSimple => TabIndicatorStyle.UnderlineFull,
                TabIndicatorStyle.UnderlineFull => TabIndicatorStyle.PillBackground,
                TabIndicatorStyle.PillBackground => TabIndicatorStyle.SlidingUnderline,
                TabIndicatorStyle.SlidingUnderline => TabIndicatorStyle.UnderlineSimple,
                _ => TabIndicatorStyle.UnderlineSimple
            };
            SetProperty("IndicatorStyle", next);
        }

        private void OnResetToDefaults(object? sender, EventArgs e)
        {
            if (AppBar == null) return;
            SetProperty("HeaderStyle", WebHeaderStyle.ShoppyStore1);
            SetProperty("IndicatorStyle", TabIndicatorStyle.UnderlineSimple);
            SetProperty("HeaderHeight", 60);
            SetProperty("LogoWidth", 40);
            SetProperty("ShowLogo", true);
            SetProperty("ShowSearchBox", true);
            Control.Invalidate();
        }

        protected override void PreFilterProperties(System.Collections.IDictionary properties)
        {
            base.PreFilterProperties(properties);

            string[] propsToHide = { "Margin", "ImeMode", "AccessibleDescription", "AccessibleName", "AccessibleRole", "Padding" };
            foreach (var prop in propsToHide)
            {
                if (properties.Contains(prop))
                {
                    properties[prop] = TypeDescriptor.CreateProperty(
                        typeof(BeepWebHeaderAppBar),
                        (PropertyDescriptor)properties[prop]!,
                        new BrowsableAttribute(false));
                }
            }
        }

        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            var lists = new DesignerActionListCollection();
            lists.Add(new BeepWebHeaderAppBarActionList(this));
            return lists;
        }
    }

    public class BeepWebHeaderAppBarActionList : DesignerActionList
    {
        private readonly BeepWebHeaderAppBarDesigner _designer;

        public BeepWebHeaderAppBarActionList(BeepWebHeaderAppBarDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        protected BeepWebHeaderAppBar? AppBar => Component as BeepWebHeaderAppBar;

        #region Properties

        [Category("Appearance")]
        public WebHeaderStyle HeaderStyle
        {
            get => _designer.GetProperty<WebHeaderStyle>("HeaderStyle");
            set => _designer.SetProperty("HeaderStyle", value);
        }

        [Category("Appearance")]
        public TabIndicatorStyle IndicatorStyle
        {
            get => _designer.GetProperty<TabIndicatorStyle>("IndicatorStyle");
            set => _designer.SetProperty("IndicatorStyle", value);
        }

        [Category("Appearance")]
        public string LogoText
        {
            get => _designer.GetProperty<string>("LogoText");
            set => _designer.SetProperty("LogoText", value);
        }

        [Category("Appearance")]
        public bool ShowLogo
        {
            get => _designer.GetProperty<bool>("ShowLogo");
            set => _designer.SetProperty("ShowLogo", value);
        }

        [Category("Appearance")]
        public bool ShowSearchBox
        {
            get => _designer.GetProperty<bool>("ShowSearchBox");
            set => _designer.SetProperty("ShowSearchBox", value);
        }

        [Category("Layout")]
        public int HeaderHeight
        {
            get => _designer.GetProperty<int>("HeaderHeight");
            set => _designer.SetProperty("HeaderHeight", value);
        }

        [Category("Layout")]
        public int LogoWidth
        {
            get => _designer.GetProperty<int>("LogoWidth");
            set => _designer.SetProperty("LogoWidth", value);
        }

        [Category("Behavior")]
        public bool AllowTabScroll
        {
            get => _designer.GetProperty<bool>("AllowTabScroll");
            set => _designer.SetProperty("AllowTabScroll", value);
        }

        #endregion

        #region Style Presets

        public void ApplyShoppyStore1() => HeaderStyle = WebHeaderStyle.ShoppyStore1;
        public void ApplyShoppyStore2() => HeaderStyle = WebHeaderStyle.ShoppyStore2;
        public void ApplyTrendModern() => HeaderStyle = WebHeaderStyle.TrendModern;
        public void ApplyStudiofokMinimal() => HeaderStyle = WebHeaderStyle.StudiofokMinimal;
        public void ApplyEcommerceDark() => HeaderStyle = WebHeaderStyle.EcommerceDark;
        public void ApplySaaSProfessional() => HeaderStyle = WebHeaderStyle.SaaSProfessional;
        public void ApplyCreativeAgency() => HeaderStyle = WebHeaderStyle.CreativeAgency;
        public void ApplyCorporateMinimal() => HeaderStyle = WebHeaderStyle.CorporateMinimal;
        public void ApplyMaterialDesign3() => HeaderStyle = WebHeaderStyle.MaterialDesign3;
        public void ApplyMinimalClean() => HeaderStyle = WebHeaderStyle.MinimalClean;
        public void ApplyStartupHero() => HeaderStyle = WebHeaderStyle.StartupHero;
        public void ApplyPortfolioMinimal() => HeaderStyle = WebHeaderStyle.PortfolioMinimal;
        public void ApplyEcommerceModern() => HeaderStyle = WebHeaderStyle.EcommerceModern;

        #endregion

        #region Actions

        public void AddSampleTabs()
        {
            if (AppBar == null) return;
            AppBar.Tabs.Clear();
            AppBar.AddTab("Home");
            AppBar.AddTab("Products");
            AppBar.AddTab("Services");
            AppBar.AddTab("About");
            AppBar.AddTab("Contact");
        }

        public void AddSampleButtons()
        {
            if (AppBar == null) return;
            AppBar.ActionButtons.Clear();
            AppBar.AddActionButton("Sign In");
            AppBar.AddActionButton("Get Started");
        }

        public void ClearAll()
        {
            if (AppBar != null)
            {
                AppBar.ClearTabs();
                AppBar.ClearActionButtons();
            }
        }

        public void CycleIndicatorStyle()
        {
            if (AppBar == null) return;
            var current = AppBar.IndicatorStyle;
            var next = current switch
            {
                TabIndicatorStyle.UnderlineSimple => TabIndicatorStyle.UnderlineFull,
                TabIndicatorStyle.UnderlineFull => TabIndicatorStyle.PillBackground,
                TabIndicatorStyle.PillBackground => TabIndicatorStyle.SlidingUnderline,
                TabIndicatorStyle.SlidingUnderline => TabIndicatorStyle.UnderlineSimple,
                _ => TabIndicatorStyle.UnderlineSimple
            };
            IndicatorStyle = next;
        }

        #endregion

        #region DesignerActionItemCollection

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Style"));
            items.Add(new DesignerActionPropertyItem("HeaderStyle", "Header Style", "Style", "Visual style variant"));
            items.Add(new DesignerActionPropertyItem("IndicatorStyle", "Indicator Style", "Style", "Tab indicator type"));

            items.Add(new DesignerActionHeaderItem("Style Presets"));
            items.Add(new DesignerActionMethodItem(this, "ApplyShoppyStore1", "Shoppy Store 1", "Style Presets", "E-commerce minimal"));
            items.Add(new DesignerActionMethodItem(this, "ApplyShoppyStore2", "Shoppy Store 2", "Style Presets", "E-commerce centered"));
            items.Add(new DesignerActionMethodItem(this, "ApplyTrendModern", "Trend Modern", "Style Presets", "Bold vibrant"));
            items.Add(new DesignerActionMethodItem(this, "ApplyStudiofokMinimal", "Studiofok Minimal", "Style Presets", "Clean professional"));
            items.Add(new DesignerActionMethodItem(this, "ApplyEcommerceDark", "E-commerce Dark", "Style Presets", "Sleek dark"));
            items.Add(new DesignerActionMethodItem(this, "ApplySaaSProfessional", "SaaS Professional", "Style Presets", "Dashboard style"));
            items.Add(new DesignerActionMethodItem(this, "ApplyCreativeAgency", "Creative Agency", "Style Presets", "Bold typography"));
            items.Add(new DesignerActionMethodItem(this, "ApplyCorporateMinimal", "Corporate Minimal", "Style Presets", "Clean white"));
            items.Add(new DesignerActionMethodItem(this, "ApplyMaterialDesign3", "Material Design 3", "Style Presets", "Google MD3"));
            items.Add(new DesignerActionMethodItem(this, "ApplyMinimalClean", "Minimal Clean", "Style Presets", "Ultra-minimal"));
            items.Add(new DesignerActionMethodItem(this, "ApplyStartupHero", "Startup Hero", "Style Presets", "Modern startup"));
            items.Add(new DesignerActionMethodItem(this, "ApplyPortfolioMinimal", "Portfolio Minimal", "Style Presets", "Personal portfolio"));
            items.Add(new DesignerActionMethodItem(this, "ApplyEcommerceModern", "E-commerce Modern", "Style Presets", "Modern e-commerce"));

            items.Add(new DesignerActionHeaderItem("Appearance"));
            items.Add(new DesignerActionPropertyItem("LogoText", "Logo Text", "Appearance", "Logo text display"));
            items.Add(new DesignerActionPropertyItem("ShowLogo", "Show Logo", "Appearance", "Toggle logo visibility"));
            items.Add(new DesignerActionPropertyItem("ShowSearchBox", "Show Search", "Appearance", "Toggle search box"));

            items.Add(new DesignerActionHeaderItem("Layout"));
            items.Add(new DesignerActionPropertyItem("HeaderHeight", "Header Height", "Layout", "Control height"));
            items.Add(new DesignerActionPropertyItem("LogoWidth", "Logo Width", "Layout", "Logo area width"));
            items.Add(new DesignerActionPropertyItem("AllowTabScroll", "Allow Tab Scroll", "Layout", "Enable overflow scrolling"));

            items.Add(new DesignerActionHeaderItem("Tabs"));
            items.Add(new DesignerActionMethodItem(this, "AddSampleTabs", "Add Sample Tabs", "Tabs", "Add 5 demo tabs"));
            items.Add(new DesignerActionMethodItem(this, "CycleIndicatorStyle", "Cycle Indicator Style", "Tabs", "Rotate indicator type"));

            items.Add(new DesignerActionHeaderItem("Actions"));
            items.Add(new DesignerActionMethodItem(this, "AddSampleButtons", "Add Sample Buttons", "Actions", "Add 2 demo buttons"));
            items.Add(new DesignerActionMethodItem(this, "ClearAll", "Clear All", "Actions", "Remove all tabs and buttons"));

            return items;
        }

        #endregion
    }
}
