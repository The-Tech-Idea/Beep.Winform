using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Design-time support for BeepMarquee control
    /// Provides smart tags for marquee configuration and animation settings
    /// </summary>
    public class BeepMarqueeDesigner : BaseBeepControlDesigner
    {
        public BeepMarquee? Marquee => Component as BeepMarquee;

        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            var lists = new DesignerActionListCollection();
            lists.Add(new BeepMarqueeActionList(this));
            return lists;
        }
    }

    /// <summary>
    /// Action list for BeepMarquee smart tags
    /// Provides quick configuration presets and common property access
    /// </summary>
    public class BeepMarqueeActionList : DesignerActionList
    {
        private readonly BeepMarqueeDesigner _designer;

        public BeepMarqueeActionList(BeepMarqueeDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        protected BeepMarquee? Marquee => Component as BeepMarquee;

        #region Properties (for smart tags)

        [Category("Behavior")]
        [Description("Scroll direction: true for left, false for right")]
        public bool ScrollLeft
        {
            get => _designer.GetProperty<bool>("ScrollLeft");
            set => _designer.SetProperty("ScrollLeft", value);
        }

        [Category("Behavior")]
        [Description("Spacing between consecutive components in pixels")]
        public int ComponentSpacing
        {
            get => _designer.GetProperty<int>("ComponentSpacing");
            set => _designer.SetProperty("ComponentSpacing", value);
        }

        [Category("Behavior")]
        [Description("Scroll interval in milliseconds (refresh rate)")]
        public int ScrollInterval
        {
            get => _designer.GetProperty<int>("ScrollInterval");
            set => _designer.SetProperty("ScrollInterval", value);
        }

        [Category("Behavior")]
        [Description("Scroll speed in pixels per tick")]
        public float ScrollSpeed
        {
            get => _designer.GetProperty<float>("ScrollSpeed");
            set => _designer.SetProperty("ScrollSpeed", value);
        }

        #endregion

        #region Quick Configuration Actions

        /// <summary>
        /// Configure for slow scrolling
        /// </summary>
        public void ConfigureSlowScrolling()
        {
            ScrollSpeed = 1.0f;
            ScrollInterval = 50;
        }

        /// <summary>
        /// Configure for normal scrolling
        /// </summary>
        public void ConfigureNormalScrolling()
        {
            ScrollSpeed = 2.0f;
            ScrollInterval = 30;
        }

        /// <summary>
        /// Configure for fast scrolling
        /// </summary>
        public void ConfigureFastScrolling()
        {
            ScrollSpeed = 4.0f;
            ScrollInterval = 20;
        }

        /// <summary>
        /// Set scroll direction to left
        /// </summary>
        public void ScrollToLeft()
        {
            ScrollLeft = true;
        }

        /// <summary>
        /// Set scroll direction to right
        /// </summary>
        public void ScrollToRight()
        {
            ScrollLeft = false;
        }

        /// <summary>
        /// Set tight spacing (10px)
        /// </summary>
        public void SetTightSpacing()
        {
            ComponentSpacing = 10;
        }

        /// <summary>
        /// Set normal spacing (20px)
        /// </summary>
        public void SetNormalSpacing()
        {
            ComponentSpacing = 20;
        }

        /// <summary>
        /// Set loose spacing (40px)
        /// </summary>
        public void SetLooseSpacing()
        {
            ComponentSpacing = 40;
        }

        #endregion

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            // Speed presets
            items.Add(new DesignerActionHeaderItem("Speed Presets"));
            items.Add(new DesignerActionMethodItem(this, "ConfigureSlowScrolling", "Slow Scrolling", "Speed Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureNormalScrolling", "Normal Scrolling", "Speed Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureFastScrolling", "Fast Scrolling", "Speed Presets", true));

            // Direction presets
            items.Add(new DesignerActionHeaderItem("Direction Presets"));
            items.Add(new DesignerActionMethodItem(this, "ScrollToLeft", "Scroll Left", "Direction Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ScrollToRight", "Scroll Right", "Direction Presets", true));

            // Spacing presets
            items.Add(new DesignerActionHeaderItem("Spacing Presets"));
            items.Add(new DesignerActionMethodItem(this, "SetTightSpacing", "Tight (10px)", "Spacing Presets", true));
            items.Add(new DesignerActionMethodItem(this, "SetNormalSpacing", "Normal (20px)", "Spacing Presets", true));
            items.Add(new DesignerActionMethodItem(this, "SetLooseSpacing", "Loose (40px)", "Spacing Presets", true));

            // Behavior properties
            items.Add(new DesignerActionHeaderItem("Behavior Properties"));
            items.Add(new DesignerActionPropertyItem("ScrollLeft", "Scroll Left", "Behavior Properties"));
            items.Add(new DesignerActionPropertyItem("ScrollSpeed", "Scroll Speed", "Behavior Properties"));
            items.Add(new DesignerActionPropertyItem("ScrollInterval", "Scroll Interval", "Behavior Properties"));
            items.Add(new DesignerActionPropertyItem("ComponentSpacing", "Component Spacing", "Behavior Properties"));

            return items;
        }
    }
}
