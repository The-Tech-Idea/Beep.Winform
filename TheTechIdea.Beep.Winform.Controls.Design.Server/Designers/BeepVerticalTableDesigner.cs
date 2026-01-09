using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls.VerticalTables;
using TheTechIdea.Beep.Winform.Controls.VerticalTables.Painters;
using TheTechIdea.Beep.Winform.Controls.VerticalTables.Helpers;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Design-time support for BeepVerticalTable control
    /// </summary>
    public class BeepVerticalTableDesigner : BaseBeepControlDesigner
    {
        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            var lists = new DesignerActionListCollection();
            lists.Add(new BeepVerticalTableActionList(this));
            return lists;
        }
    }

    /// <summary>
    /// Action list for BeepVerticalTable smart tags
    /// Provides quick access to common table properties and style presets
    /// </summary>
    public class BeepVerticalTableActionList : DesignerActionList
    {
        private readonly BeepVerticalTableDesigner _designer;

        public BeepVerticalTableActionList(BeepVerticalTableDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        protected BeepVerticalTable? Table => Component as BeepVerticalTable;

        #region Properties

        [Category("Table")]
        [Description("Visual style of the vertical table")]
        public VerticalTablePainterStyle TableStyle
        {
            get => _designer.GetProperty<VerticalTablePainterStyle>("TableStyle");
            set => _designer.SetProperty("TableStyle", value);
        }

        [Category("Appearance")]
        [Description("Header height in pixels")]
        public int HeaderHeight
        {
            get => _designer.GetProperty<int>("HeaderHeight");
            set => _designer.SetProperty("HeaderHeight", value);
        }

        [Category("Appearance")]
        [Description("Row height in pixels")]
        public int RowHeight
        {
            get => _designer.GetProperty<int>("RowHeight");
            set => _designer.SetProperty("RowHeight", value);
        }

        [Category("Appearance")]
        [Description("Column width in pixels")]
        public int ColumnWidth
        {
            get => _designer.GetProperty<int>("ColumnWidth");
            set => _designer.SetProperty("ColumnWidth", value);
        }

        [Category("Behavior")]
        [Description("Show image for each column header")]
        public bool ShowImage
        {
            get => _designer.GetProperty<bool>("ShowImage");
            set => _designer.SetProperty("ShowImage", value);
        }

        #endregion

        #region Actions

        public void ApplyStyle1()
        {
            TableStyle = VerticalTablePainterStyle.Style1;
        }

        public void ApplyStyle2()
        {
            TableStyle = VerticalTablePainterStyle.Style2;
        }

        public void ApplyStyle3()
        {
            TableStyle = VerticalTablePainterStyle.Style3;
        }

        public void ApplyStyle6()
        {
            TableStyle = VerticalTablePainterStyle.Style6;
        }

        public void UseRecommendedSizes()
        {
            if (Table != null)
            {
                var style = Table.TableStyle;
                HeaderHeight = VerticalTableStyleHelpers.GetRecommendedHeaderHeight(style);
                RowHeight = VerticalTableStyleHelpers.GetRecommendedRowHeight(style);
                ColumnWidth = VerticalTableStyleHelpers.GetRecommendedColumnWidth(style);
            }
        }

        #endregion

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Table"));
            items.Add(new DesignerActionPropertyItem("TableStyle", "Style:", "Table"));

            items.Add(new DesignerActionHeaderItem("Appearance"));
            items.Add(new DesignerActionPropertyItem("HeaderHeight", "Header Height:", "Appearance"));
            items.Add(new DesignerActionPropertyItem("RowHeight", "Row Height:", "Appearance"));
            items.Add(new DesignerActionPropertyItem("ColumnWidth", "Column Width:", "Appearance"));
            items.Add(new DesignerActionPropertyItem("ShowImage", "Show Image:", "Appearance"));

            items.Add(new DesignerActionHeaderItem("Style Presets"));
            items.Add(new DesignerActionMethodItem(this, "ApplyStyle1", "Card Style (Style 1)", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ApplyStyle2", "Grid Comparison (Style 2)", "Style Presets", false));
            items.Add(new DesignerActionMethodItem(this, "ApplyStyle3", "Minimal Cards (Style 3)", "Style Presets", false));
            items.Add(new DesignerActionMethodItem(this, "ApplyStyle6", "Classic Table (Style 6)", "Style Presets", false));

            items.Add(new DesignerActionHeaderItem("Layout"));
            items.Add(new DesignerActionMethodItem(this, "UseRecommendedSizes", "Use Recommended Sizes", "Layout", true));

            return items;
        }
    }
}
