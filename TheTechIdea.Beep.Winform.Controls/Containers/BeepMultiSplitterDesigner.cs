using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Custom designer for BeepMultiSplitter that allows design-time editing of the TableLayoutPanel
    /// </summary>
    internal class BeepMultiSplitterDesigner : ParentControlDesigner
    {
        private BeepMultiSplitter _control;

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
            _control = component as BeepMultiSplitter;
            
            if (_control != null && _control.TableLayoutPanel != null)
            {
                // Enable design-time editing of the TableLayoutPanel
                EnableDesignMode(_control.TableLayoutPanel, "TableLayoutPanel");
            }
        }

        protected override void PreFilterProperties(System.Collections.IDictionary properties)
        {
            base.PreFilterProperties(properties);

            // Expose TableLayoutPanel properties directly on the control
            if (_control?.TableLayoutPanel != null)
            {
                // Add ColumnCount property
                PropertyDescriptor columnCountProp = TypeDescriptor.CreateProperty(
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

                // Add RowCount property
                PropertyDescriptor rowCountProp = TypeDescriptor.CreateProperty(
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

                // Add CellBorderStyle property
                PropertyDescriptor cellBorderStyleProp = TypeDescriptor.CreateProperty(
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

        public override DesignerActionListCollection ActionLists
        {
            get
            {
                DesignerActionListCollection actionLists = new DesignerActionListCollection();
                actionLists.Add(new BeepMultiSplitterActionList(_control));
                return actionLists;
            }
        }
    }

    /// <summary>
    /// Smart tag action list for BeepMultiSplitter
    /// </summary>
    internal class BeepMultiSplitterActionList : DesignerActionList
    {
        private BeepMultiSplitter _control;
        private DesignerActionUIService _designerActionUISvc;

        public BeepMultiSplitterActionList(BeepMultiSplitter control)
            : base(control)
        {
            _control = control;
            _designerActionUISvc = GetService(typeof(DesignerActionUIService)) as DesignerActionUIService;
        }

        public int ColumnCount
        {
            get => _control?.TableLayoutPanel?.ColumnCount ?? 1;
            set
            {
                if (_control?.TableLayoutPanel != null)
                {
                    _control.TableLayoutPanel.ColumnCount = value;
                    _control.Invalidate();
                }
            }
        }

        public int RowCount
        {
            get => _control?.TableLayoutPanel?.RowCount ?? 1;
            set
            {
                if (_control?.TableLayoutPanel != null)
                {
                    _control.TableLayoutPanel.RowCount = value;
                    _control.Invalidate();
                }
            }
        }

        public TableLayoutPanelCellBorderStyle CellBorderStyle
        {
            get => _control?.TableLayoutPanel?.CellBorderStyle ?? TableLayoutPanelCellBorderStyle.None;
            set
            {
                if (_control?.TableLayoutPanel != null)
                {
                    _control.TableLayoutPanel.CellBorderStyle = value;
                    _control.Invalidate();
                }
            }
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            DesignerActionItemCollection items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Layout"));
            items.Add(new DesignerActionPropertyItem("ColumnCount", "Column Count", "Layout", "Number of columns"));
            items.Add(new DesignerActionPropertyItem("RowCount", "Row Count", "Layout", "Number of rows"));
            items.Add(new DesignerActionHeaderItem("Appearance"));
            items.Add(new DesignerActionPropertyItem("CellBorderStyle", "Cell Border Style", "Appearance", "Border style for cells"));

            return items;
        }
    }

}

