using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls.BottomNavBars;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Design-time support for BottomBar control
    /// Provides smart tags for navigation bar configuration and styling
    /// </summary>
    public class BottomBarDesigner : BaseBeepControlDesigner
    {
        public BottomBar? BottomBar => Component as BottomBar;

        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            var lists = new DesignerActionListCollection();
            lists.Add(new BottomBarActionList(this));
            return lists;
        }
    }

    /// <summary>
    /// Action list for BottomBar smart tags
    /// Provides quick configuration presets and common property access
    /// </summary>
    public class BottomBarActionList : DesignerActionList
    {
        private readonly BottomBarDesigner _designer;

        public BottomBarActionList(BottomBarDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        protected BottomBar? BottomBar => Component as BottomBar;

        #region Properties (for smart tags)

        [Category("Appearance")]
        [Description("Visual style of the bottom navigation bar")]
        public BottomBarStyle BarStyle
        {
            get => _designer.GetProperty<BottomBarStyle>("BarStyle");
            set => _designer.SetProperty("BarStyle", value);
        }

        [Category("Appearance")]
        [Description("Height of the bottom bar in pixels")]
        public int BarHeight
        {
            get => _designer.GetProperty<int>("BarHeight");
            set => _designer.SetProperty("BarHeight", value);
        }

        [Category("Appearance")]
        [Description("Accent color for selected items and indicators")]
        public System.Drawing.Color AccentColor
        {
            get => _designer.GetProperty<System.Drawing.Color>("AccentColor");
            set => _designer.SetProperty("AccentColor", value);
        }

        [Category("Behavior")]
        [Description("Animation duration in milliseconds")]
        public int AnimationDuration
        {
            get => _designer.GetProperty<int>("AnimationDuration");
            set => _designer.SetProperty("AnimationDuration", value);
        }

        [Category("Behavior")]
        [Description("Index of the centered CTA (Call-to-Action) item, or -1 for none")]
        public int CTAIndex
        {
            get => _designer.GetProperty<int>("CTAIndex");
            set => _designer.SetProperty("CTAIndex", value);
        }

        [Category("Behavior")]
        [Description("Show shadow effect on CTA button")]
        public bool ShowCTAShadow
        {
            get => _designer.GetProperty<bool>("ShowCTAShadow");
            set => _designer.SetProperty("ShowCTAShadow", value);
        }

        [Category("Behavior")]
        [Description("Width factor for CTA button (1.0 = normal size)")]
        public float CTAWidthFactor
        {
            get => _designer.GetProperty<float>("CTAWidthFactor");
            set => _designer.SetProperty("CTAWidthFactor", value);
        }

        [Category("Behavior")]
        [Description("Width factor for selected item (1.0 = normal size)")]
        public float SelectedWidthFactor
        {
            get => _designer.GetProperty<float>("SelectedWidthFactor");
            set => _designer.SetProperty("SelectedWidthFactor", value);
        }

        [Category("Appearance")]
        [Description("Opacity for glass acrylic style (0.0 to 1.0)")]
        public float GlassAcrylicOpacity
        {
            get => _designer.GetProperty<float>("GlassAcrylicOpacity");
            set => _designer.SetProperty("GlassAcrylicOpacity", value);
        }

        #endregion

        #region Quick Configuration Actions

        /// <summary>
        /// Configure as Classic bottom bar style
        /// </summary>
        public void ConfigureAsClassic()
        {
            BarStyle = BottomBarStyle.Classic;
            BarHeight = 72;
            ShowCTAShadow = true;
        }

        /// <summary>
        /// Configure as Floating CTA bottom bar style
        /// </summary>
        public void ConfigureAsFloatingCTA()
        {
            BarStyle = BottomBarStyle.FloatingCTA;
            BarHeight = 72;
            CTAIndex = 2; // Center item
            ShowCTAShadow = true;
            CTAWidthFactor = 1.6f;
        }

        /// <summary>
        /// Configure as Bubble bottom bar style
        /// </summary>
        public void ConfigureAsBubble()
        {
            BarStyle = BottomBarStyle.Bubble;
            BarHeight = 72;
            ShowCTAShadow = false;
        }

        /// <summary>
        /// Configure as Pill bottom bar style
        /// </summary>
        public void ConfigureAsPill()
        {
            BarStyle = BottomBarStyle.Pill;
            BarHeight = 64;
            ShowCTAShadow = false;
        }

        /// <summary>
        /// Configure as Notion Minimal bottom bar style
        /// </summary>
        public void ConfigureAsNotionMinimal()
        {
            BarStyle = BottomBarStyle.NotionMinimal;
            BarHeight = 56;
            ShowCTAShadow = false;
        }

        /// <summary>
        /// Configure as Glass Acrylic bottom bar style
        /// </summary>
        public void ConfigureAsGlassAcrylic()
        {
            BarStyle = BottomBarStyle.GlassAcrylic;
            BarHeight = 72;
            GlassAcrylicOpacity = 0.6f;
            ShowCTAShadow = false;
        }

        /// <summary>
        /// Set standard bar height (72px)
        /// </summary>
        public void SetStandardHeight()
        {
            BarHeight = 72;
        }

        /// <summary>
        /// Set compact bar height (56px)
        /// </summary>
        public void SetCompactHeight()
        {
            BarHeight = 56;
        }

        /// <summary>
        /// Set comfortable bar height (80px)
        /// </summary>
        public void SetComfortableHeight()
        {
            BarHeight = 80;
        }

        /// <summary>
        /// Enable fast animations (120ms)
        /// </summary>
        public void EnableFastAnimations()
        {
            AnimationDuration = 120;
        }

        /// <summary>
        /// Enable smooth animations (300ms)
        /// </summary>
        public void EnableSmoothAnimations()
        {
            AnimationDuration = 300;
        }

        #endregion

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            // Quick configuration presets
            items.Add(new DesignerActionHeaderItem("Style Presets"));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsClassic", "Classic Style", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsFloatingCTA", "Floating CTA Style", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsBubble", "Bubble Style", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsPill", "Pill Style", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsNotionMinimal", "Notion Minimal Style", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsGlassAcrylic", "Glass Acrylic Style", "Style Presets", true));

            // Height presets
            items.Add(new DesignerActionHeaderItem("Height Presets"));
            items.Add(new DesignerActionMethodItem(this, "SetStandardHeight", "Standard (72px)", "Height Presets", true));
            items.Add(new DesignerActionMethodItem(this, "SetCompactHeight", "Compact (56px)", "Height Presets", true));
            items.Add(new DesignerActionMethodItem(this, "SetComfortableHeight", "Comfortable (80px)", "Height Presets", true));

            // Animation presets
            items.Add(new DesignerActionHeaderItem("Animation Presets"));
            items.Add(new DesignerActionMethodItem(this, "EnableFastAnimations", "Fast (120ms)", "Animation Presets", true));
            items.Add(new DesignerActionMethodItem(this, "EnableSmoothAnimations", "Smooth (300ms)", "Animation Presets", true));

            // Appearance properties
            items.Add(new DesignerActionHeaderItem("Appearance Properties"));
            items.Add(new DesignerActionPropertyItem("BarStyle", "Bar Style", "Appearance Properties"));
            items.Add(new DesignerActionPropertyItem("BarHeight", "Bar Height", "Appearance Properties"));
            items.Add(new DesignerActionPropertyItem("AccentColor", "Accent Color", "Appearance Properties"));
            items.Add(new DesignerActionPropertyItem("GlassAcrylicOpacity", "Glass Acrylic Opacity", "Appearance Properties"));

            // Behavior properties
            items.Add(new DesignerActionHeaderItem("Behavior Properties"));
            items.Add(new DesignerActionPropertyItem("AnimationDuration", "Animation Duration", "Behavior Properties"));
            items.Add(new DesignerActionPropertyItem("CTAIndex", "CTA Index", "Behavior Properties"));
            items.Add(new DesignerActionPropertyItem("ShowCTAShadow", "Show CTA Shadow", "Behavior Properties"));
            items.Add(new DesignerActionPropertyItem("CTAWidthFactor", "CTA Width Factor", "Behavior Properties"));
            items.Add(new DesignerActionPropertyItem("SelectedWidthFactor", "Selected Width Factor", "Behavior Properties"));

            return items;
        }
    }
}
