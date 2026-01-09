using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls.AccordionMenus;
using TheTechIdea.Beep.Winform.Controls.AccordionMenus.Helpers;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Design-time support for BeepAccordionMenu control
    /// Provides smart tags for accordion configuration and styling
    /// </summary>
    public class BeepAccordionMenuDesigner : BaseBeepControlDesigner
    {
        public BeepAccordionMenu? AccordionMenu => Component as BeepAccordionMenu;

        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            var lists = new DesignerActionListCollection();
            lists.Add(new BeepAccordionMenuActionList(this));
            return lists;
        }
    }

    /// <summary>
    /// Action list for BeepAccordionMenu smart tags
    /// Provides quick configuration presets and common property access
    /// </summary>
    public class BeepAccordionMenuActionList : DesignerActionList
    {
        private readonly BeepAccordionMenuDesigner _designer;

        public BeepAccordionMenuActionList(BeepAccordionMenuDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        protected BeepAccordionMenu? AccordionMenu => Component as BeepAccordionMenu;

        #region Properties (for smart tags)

        [Category("Appearance")]
        [Description("Accordion visual style")]
        public AccordionStyle AccordionStyle
        {
            get => _designer.GetProperty<AccordionStyle>("AccordionStyle");
            set => _designer.SetProperty("AccordionStyle", value);
        }

        [Category("Appearance")]
        [Description("Title text displayed in the header")]
        public string Title
        {
            get => _designer.GetProperty<string>("Title");
            set => _designer.SetProperty("Title", value);
        }

        [Category("Layout")]
        [Description("Height of menu items in pixels")]
        public int ItemHeight
        {
            get => _designer.GetProperty<int>("ItemHeight");
            set => _designer.SetProperty("ItemHeight", value);
        }

        [Category("Layout")]
        [Description("Height of child menu items in pixels")]
        public int ChildItemHeight
        {
            get => _designer.GetProperty<int>("ChildItemHeight");
            set => _designer.SetProperty("ChildItemHeight", value);
        }

        [Category("Layout")]
        [Description("Width when expanded")]
        public int ExpandedWidth
        {
            get => _designer.GetProperty<int>("ExpandedWidth");
            set => _designer.SetProperty("ExpandedWidth", value);
        }

        [Category("Layout")]
        [Description("Width when collapsed")]
        public int CollapsedWidth
        {
            get => _designer.GetProperty<int>("CollapsedWidth");
            set => _designer.SetProperty("CollapsedWidth", value);
        }

        #endregion

        #region Quick Configuration Actions

        /// <summary>
        /// Apply Material3 style preset
        /// </summary>
        public void ApplyMaterial3Style()
        {
            AccordionStyle = AccordionStyle.Material3;
        }

        /// <summary>
        /// Apply Modern style preset
        /// </summary>
        public void ApplyModernStyle()
        {
            AccordionStyle = AccordionStyle.Modern;
        }

        /// <summary>
        /// Apply Classic style preset
        /// </summary>
        public void ApplyClassicStyle()
        {
            AccordionStyle = AccordionStyle.Classic;
        }

        /// <summary>
        /// Apply Minimal style preset
        /// </summary>
        public void ApplyMinimalStyle()
        {
            AccordionStyle = AccordionStyle.Minimal;
        }

        /// <summary>
        /// Apply iOS style preset
        /// </summary>
        public void ApplyiOSStyle()
        {
            AccordionStyle = AccordionStyle.iOS;
        }

        /// <summary>
        /// Apply Fluent2 style preset
        /// </summary>
        public void ApplyFluent2Style()
        {
            AccordionStyle = AccordionStyle.Fluent2;
        }

        /// <summary>
        /// Set recommended item height for current style
        /// </summary>
        public void SetRecommendedItemHeight()
        {
            if (AccordionMenu != null)
            {
                ItemHeight = AccordionStyleHelpers.GetRecommendedItemHeight(AccordionMenu.AccordionStyle);
                ChildItemHeight = AccordionStyleHelpers.GetRecommendedChildItemHeight(AccordionMenu.AccordionStyle);
            }
        }

        #endregion

        #region DesignerActionItemCollection

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            // Header
            items.Add(new DesignerActionHeaderItem("Style"));
            items.Add(new DesignerActionPropertyItem("AccordionStyle", "Accordion Style", "Style", "Visual style of the accordion"));

            // Style presets
            items.Add(new DesignerActionHeaderItem("Style Presets"));
            items.Add(new DesignerActionMethodItem(this, "ApplyMaterial3Style", "Material3 Style", "Style Presets", "Apply Material Design 3 style"));
            items.Add(new DesignerActionMethodItem(this, "ApplyModernStyle", "Modern Style", "Style Presets", "Apply modern flat design style"));
            items.Add(new DesignerActionMethodItem(this, "ApplyClassicStyle", "Classic Style", "Style Presets", "Apply classic bordered style"));
            items.Add(new DesignerActionMethodItem(this, "ApplyMinimalStyle", "Minimal Style", "Style Presets", "Apply minimal clean style"));
            items.Add(new DesignerActionMethodItem(this, "ApplyiOSStyle", "iOS Style", "Style Presets", "Apply iOS-style rounded design"));
            items.Add(new DesignerActionMethodItem(this, "ApplyFluent2Style", "Fluent2 Style", "Style Presets", "Apply Fluent Design 2 style"));

            // Appearance
            items.Add(new DesignerActionHeaderItem("Appearance"));
            items.Add(new DesignerActionPropertyItem("Title", "Title", "Appearance", "Header title text"));

            // Layout
            items.Add(new DesignerActionHeaderItem("Layout"));
            items.Add(new DesignerActionPropertyItem("ItemHeight", "Item Height", "Layout", "Height of menu items"));
            items.Add(new DesignerActionPropertyItem("ChildItemHeight", "Child Item Height", "Layout", "Height of child menu items"));
            items.Add(new DesignerActionPropertyItem("ExpandedWidth", "Expanded Width", "Layout", "Width when expanded"));
            items.Add(new DesignerActionPropertyItem("CollapsedWidth", "Collapsed Width", "Layout", "Width when collapsed"));

            // Quick actions
            items.Add(new DesignerActionHeaderItem("Quick Actions"));
            items.Add(new DesignerActionMethodItem(this, "SetRecommendedItemHeight", "Set Recommended Item Height", "Quick Actions", "Set item heights based on current style"));

            return items;
        }

        #endregion
    }
}
