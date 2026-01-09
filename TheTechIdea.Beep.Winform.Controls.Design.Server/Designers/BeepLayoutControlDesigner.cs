using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls.Layouts;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Design-time support for BeepLayoutControl
    /// Provides smart tags for template selection and layout configuration
    /// </summary>
    public class BeepLayoutControlDesigner : BaseBeepControlDesigner
    {
        public BeepLayoutControl? LayoutControl => Component as BeepLayoutControl;

        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            var lists = new DesignerActionListCollection();
            lists.Add(new BeepLayoutControlActionList(this));
            return lists;
        }
    }

    /// <summary>
    /// Action list for BeepLayoutControl smart tags
    /// Provides quick template selection and layout configuration
    /// </summary>
    public class BeepLayoutControlActionList : DesignerActionList
    {
        private readonly BeepLayoutControlDesigner _designer;

        public BeepLayoutControlActionList(BeepLayoutControlDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        protected BeepLayoutControl? LayoutControl => Component as BeepLayoutControl;

        #region Properties (for smart tags)

        [Category("Layout")]
        [Description("The layout template type")]
        public BeepLayoutControl.TemplateType Template
        {
            get => _designer.GetProperty<BeepLayoutControl.TemplateType>("Template");
            set => _designer.SetProperty("Template", value);
        }

        [Category("Layout")]
        [Description("Number of rows for Grid template")]
        public int GridRows
        {
            get => _designer.GetProperty<int>("GridRows");
            set => _designer.SetProperty("GridRows", value);
        }

        [Category("Layout")]
        [Description("Number of columns for Grid template")]
        public int GridColumns
        {
            get => _designer.GetProperty<int>("GridColumns");
            set => _designer.SetProperty("GridColumns", value);
        }

        #endregion

        #region Quick Template Selection Actions

        /// <summary>
        /// Switch to Invoice layout template
        /// </summary>
        public void UseInvoiceLayout()
        {
            Template = BeepLayoutControl.TemplateType.Invoice;
        }

        /// <summary>
        /// Switch to Product layout template
        /// </summary>
        public void UseProductLayout()
        {
            Template = BeepLayoutControl.TemplateType.Product;
        }

        /// <summary>
        /// Switch to Profile layout template
        /// </summary>
        public void UseProfileLayout()
        {
            Template = BeepLayoutControl.TemplateType.Profile;
        }

        /// <summary>
        /// Switch to Report layout template
        /// </summary>
        public void UseReportLayout()
        {
            Template = BeepLayoutControl.TemplateType.Report;
        }

        /// <summary>
        /// Switch to Vertical Stack layout template
        /// </summary>
        public void UseVerticalStackLayout()
        {
            Template = BeepLayoutControl.TemplateType.VerticalStack;
        }

        /// <summary>
        /// Switch to Horizontal Stack layout template
        /// </summary>
        public void UseHorizontalStackLayout()
        {
            Template = BeepLayoutControl.TemplateType.HorizontalStack;
        }

        /// <summary>
        /// Switch to Grid layout template
        /// </summary>
        public void UseGridLayout()
        {
            Template = BeepLayoutControl.TemplateType.Grid;
            GridRows = 3;
            GridColumns = 3;
        }

        /// <summary>
        /// Switch to Split Container layout template
        /// </summary>
        public void UseSplitContainerLayout()
        {
            Template = BeepLayoutControl.TemplateType.SplitContainer;
        }

        /// <summary>
        /// Switch to Dock layout template
        /// </summary>
        public void UseDockLayout()
        {
            Template = BeepLayoutControl.TemplateType.Dock;
        }

        /// <summary>
        /// Rebuild the current layout
        /// </summary>
        public void RebuildLayout()
        {
            var layoutControl = LayoutControl;
            if (layoutControl != null)
            {
                layoutControl.RebuildLayout();
            }
        }

        #endregion

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            // Quick template selection
            items.Add(new DesignerActionHeaderItem("Layout Templates"));
            items.Add(new DesignerActionMethodItem(this, "UseInvoiceLayout", "Invoice Layout", "Layout Templates", true));
            items.Add(new DesignerActionMethodItem(this, "UseProductLayout", "Product Layout", "Layout Templates", true));
            items.Add(new DesignerActionMethodItem(this, "UseProfileLayout", "Profile Layout", "Layout Templates", true));
            items.Add(new DesignerActionMethodItem(this, "UseReportLayout", "Report Layout", "Layout Templates", true));
            items.Add(new DesignerActionMethodItem(this, "UseVerticalStackLayout", "Vertical Stack", "Layout Templates", true));
            items.Add(new DesignerActionMethodItem(this, "UseHorizontalStackLayout", "Horizontal Stack", "Layout Templates", true));
            items.Add(new DesignerActionMethodItem(this, "UseGridLayout", "Grid Layout", "Layout Templates", true));
            items.Add(new DesignerActionMethodItem(this, "UseSplitContainerLayout", "Split Container", "Layout Templates", true));
            items.Add(new DesignerActionMethodItem(this, "UseDockLayout", "Dock Layout", "Layout Templates", true));

            // Layout actions
            items.Add(new DesignerActionHeaderItem("Layout Actions"));
            items.Add(new DesignerActionMethodItem(this, "RebuildLayout", "Rebuild Layout", "Layout Actions", true));

            // Layout properties
            items.Add(new DesignerActionHeaderItem("Layout Properties"));
            items.Add(new DesignerActionPropertyItem("Template", "Template", "Layout Properties"));
            items.Add(new DesignerActionPropertyItem("GridRows", "Grid Rows", "Layout Properties"));
            items.Add(new DesignerActionPropertyItem("GridColumns", "Grid Columns", "Layout Properties"));

            return items;
        }
    }
}
