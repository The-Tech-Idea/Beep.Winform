using System;
using System.ComponentModel;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Design.Server.ActionLists;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Design-time support for BeepMultiSplitter, enabling TableLayoutPanel editing
    /// and providing smart-tag actions for row/column management.
    /// </summary>
    internal sealed class BeepMultiSplitterDesigner : BaseBeepParentControlDesigner
    {
        public override void Initialize(IComponent component)
        {
            base.Initialize(component);

            if (Component is BeepMultiSplitter splitter && splitter.TableLayoutPanel != null)
            {
                EnableDesignMode(splitter.TableLayoutPanel, "TableLayoutPanel");
            }
        }

        protected override void PreFilterProperties(System.Collections.IDictionary properties)
        {
            base.PreFilterProperties(properties);

            if (Component is BeepMultiSplitter splitter && splitter.TableLayoutPanel != null)
            {
                var columnCountProp = TypeDescriptor.CreateProperty(
                    typeof(BeepMultiSplitter),
                    "ColumnCount",
                    typeof(int),
                    new Attribute[]
                    {
                        new CategoryAttribute("Layout"),
                        new DescriptionAttribute("The number of columns in the table layout."),
                        new DefaultValueAttribute(1)
                    });
                properties["ColumnCount"] = columnCountProp;

                var rowCountProp = TypeDescriptor.CreateProperty(
                    typeof(BeepMultiSplitter),
                    "RowCount",
                    typeof(int),
                    new Attribute[]
                    {
                        new CategoryAttribute("Layout"),
                        new DescriptionAttribute("The number of rows in the table layout."),
                        new DefaultValueAttribute(1)
                    });
                properties["RowCount"] = rowCountProp;

                var cellBorderStyleProp = TypeDescriptor.CreateProperty(
                    typeof(BeepMultiSplitter),
                    "CellBorderStyle",
                    typeof(TableLayoutPanelCellBorderStyle),
                    new Attribute[]
                    {
                        new CategoryAttribute("Appearance"),
                        new DescriptionAttribute("The style of the cell borders."),
                        new DefaultValueAttribute(TableLayoutPanelCellBorderStyle.Single)
                    });
                properties["CellBorderStyle"] = cellBorderStyleProp;
            }
        }

        protected override DesignerActionListCollection GetControlSpecificActionLists()
            => new DesignerActionListCollection
            {
                new BeepMultiSplitterActionList(this)
            };
    }
}
