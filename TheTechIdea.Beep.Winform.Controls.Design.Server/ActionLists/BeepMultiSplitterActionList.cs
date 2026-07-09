using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Designers;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.ActionLists
{
    /// <summary>
    /// Smart tag action list for BeepMultiSplitter providing layout and appearance controls.
    /// </summary>
    internal sealed class BeepMultiSplitterActionList : DesignerActionList
    {
        private readonly IBeepDesignerActionHost _designer;
        private readonly IServiceProvider? _serviceProvider;
        internal BeepMultiSplitter? Splitter => Component as BeepMultiSplitter;
        private TableLayoutPanel? TableLayoutPanel => Splitter?.TableLayoutPanel;
        private IComponentChangeService? ChangeService;

        public BeepMultiSplitterActionList(IBeepDesignerActionHost designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new System.ArgumentNullException(nameof(designer));
            if (designer is IServiceProvider sp)
            {
                _serviceProvider = sp;
                ChangeService = sp.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
            }
        }

        /// <summary>Rebuilds the smart-tag panel (used to cascade the Category -> Template dropdowns).</summary>
        private void RefreshSmartTag()
            => (_serviceProvider?.GetService(typeof(DesignerActionUIService)) as DesignerActionUIService)?.Refresh(Component);

        public int ColumnCount
        {
            get => TableLayoutPanel?.ColumnCount ?? 1;
            set
            {
                if (TableLayoutPanel != null && value >= 1)
                {
                    ChangeService?.OnComponentChanging(TableLayoutPanel, TypeDescriptor.GetProperties(TableLayoutPanel)["ColumnCount"]);
                    TableLayoutPanel.ColumnCount = value;
                    ChangeService?.OnComponentChanged(TableLayoutPanel, TypeDescriptor.GetProperties(TableLayoutPanel)["ColumnCount"], null, null);
                    Splitter?.Invalidate();
                }
            }
        }

        public int RowCount
        {
            get => TableLayoutPanel?.RowCount ?? 1;
            set
            {
                if (TableLayoutPanel != null && value >= 1)
                {
                    ChangeService?.OnComponentChanging(TableLayoutPanel, TypeDescriptor.GetProperties(TableLayoutPanel)["RowCount"]);
                    TableLayoutPanel.RowCount = value;
                    ChangeService?.OnComponentChanged(TableLayoutPanel, TypeDescriptor.GetProperties(TableLayoutPanel)["RowCount"], null, null);
                    Splitter?.Invalidate();
                }
            }
        }

        public TableLayoutPanelCellBorderStyle CellBorderStyle
        {
            get => TableLayoutPanel?.CellBorderStyle ?? TableLayoutPanelCellBorderStyle.Single;
            set
            {
                if (TableLayoutPanel != null)
                {
                    ChangeService?.OnComponentChanging(TableLayoutPanel, TypeDescriptor.GetProperties(TableLayoutPanel)["CellBorderStyle"]);
                    TableLayoutPanel.CellBorderStyle = value;
                    ChangeService?.OnComponentChanged(TableLayoutPanel, TypeDescriptor.GetProperties(TableLayoutPanel)["CellBorderStyle"], null, null);
                    Splitter?.Invalidate();
                }
            }
        }

        /// <summary>Applies a built-in layout preset (reconfigures the grid). Rendered as a dropdown.</summary>
        public MultiSplitterLayout LayoutPreset
        {
            get => Splitter?.Layout ?? MultiSplitterLayout.Custom;
            set
            {
                if (Splitter == null) return;
                var tlp = TableLayoutPanel;
                if (tlp != null) ChangeService?.OnComponentChanging(tlp, null);
                ChangeService?.OnComponentChanging(Splitter, TypeDescriptor.GetProperties(Splitter)["Layout"]);
                Splitter.Layout = value;
                ChangeService?.OnComponentChanged(Splitter, TypeDescriptor.GetProperties(Splitter)["Layout"], null, null);
                if (tlp != null) ChangeService?.OnComponentChanged(tlp, null, null, null);
                Splitter.Invalidate();
            }
        }

        /// <summary>Applies a ready-made business template (invoice, product, POS, ...). Rendered as a dropdown.</summary>
        public BusinessTemplate BusinessTemplate
        {
            get => Splitter?.BusinessTemplate ?? BusinessTemplate.None;
            set
            {
                if (Splitter == null) return;
                var tlp = TableLayoutPanel;
                if (tlp != null) ChangeService?.OnComponentChanging(tlp, null);
                ChangeService?.OnComponentChanging(Splitter, TypeDescriptor.GetProperties(Splitter)["BusinessTemplate"]);
                Splitter.BusinessTemplate = value;
                ChangeService?.OnComponentChanged(Splitter, TypeDescriptor.GetProperties(Splitter)["BusinessTemplate"], null, null);
                if (tlp != null) ChangeService?.OnComponentChanged(tlp, null, null, null);
                Splitter.Invalidate();
            }
        }

        private BusinessTemplateCategory _templateCategory = BusinessTemplateCategory.Billing;

        /// <summary>Selected business-template category. Changing it re-filters the Template dropdown.</summary>
        public BusinessTemplateCategory TemplateCategory
        {
            get
            {
                // Keep the category in sync with the active template so the dropdown reflects reality.
                var current = Splitter?.BusinessTemplate ?? BusinessTemplate.None;
                if (current != BusinessTemplate.None)
                    _templateCategory = BeepMultiSplitter.GetTemplateCategory(current);
                return _templateCategory;
            }
            set
            {
                _templateCategory = value;
                RefreshSmartTag();
            }
        }

        /// <summary>The template within the selected category. Setting it applies the template.</summary>
        [TypeConverter(typeof(TemplateInCategoryConverter))]
        public BusinessTemplate Template
        {
            get => BusinessTemplate;
            set => BusinessTemplate = value;
        }

        /// <summary>The templates available in the currently selected category (used by the converter).</summary>
        internal BusinessTemplate[] TemplatesInCategory => BeepMultiSplitter.GetTemplates(_templateCategory);

        public bool ShowSplitterGrips
        {
            get => Splitter?.ShowSplitterGrips ?? true;
            set
            {
                if (Splitter == null) return;
                ChangeService?.OnComponentChanging(Splitter, TypeDescriptor.GetProperties(Splitter)["ShowSplitterGrips"]);
                Splitter.ShowSplitterGrips = value;
                ChangeService?.OnComponentChanged(Splitter, TypeDescriptor.GetProperties(Splitter)["ShowSplitterGrips"], null, null);
                Splitter.Invalidate();
            }
        }

        public bool ShowCollapseHandles
        {
            get => Splitter?.ShowCollapseHandles ?? true;
            set
            {
                if (Splitter == null) return;
                ChangeService?.OnComponentChanging(Splitter, TypeDescriptor.GetProperties(Splitter)["ShowCollapseHandles"]);
                Splitter.ShowCollapseHandles = value;
                ChangeService?.OnComponentChanged(Splitter, TypeDescriptor.GetProperties(Splitter)["ShowCollapseHandles"], null, null);
                Splitter.Invalidate();
            }
        }

        // One-click preset shortcuts
        public void PresetSidebarLeft() => LayoutPreset = MultiSplitterLayout.SidebarLeft;
        public void PresetHeaderContentFooter() => LayoutPreset = MultiSplitterLayout.HeaderContentFooter;
        public void PresetGrid2x2() => LayoutPreset = MultiSplitterLayout.Grid2x2;

        /// <summary>Scaffolds the current layout preset's named zones as labelled tiles.</summary>
        public void ScaffoldLayoutZones()
        {
            if (Splitter == null || TableLayoutPanel == null || Splitter.Layout == MultiSplitterLayout.Custom) return;
            ChangeService?.OnComponentChanging(TableLayoutPanel, null);
            Splitter.ApplyLayout(Splitter.Layout, addPlaceholders: true);
            ChangeService?.OnComponentChanged(TableLayoutPanel, null, null, null);
            Splitter.Invalidate();
        }

        /// <summary>Scaffolds the current business template's named zones as labelled tiles.</summary>
        public void ScaffoldTemplateZones()
        {
            if (Splitter == null || TableLayoutPanel == null || Splitter.BusinessTemplate == BusinessTemplate.None) return;
            ChangeService?.OnComponentChanging(TableLayoutPanel, null);
            Splitter.ApplyBusinessTemplate(Splitter.BusinessTemplate, addPlaceholders: true);
            ChangeService?.OnComponentChanged(TableLayoutPanel, null, null, null);
            Splitter.Invalidate();
        }

        public void AddRow()
        {
            if (TableLayoutPanel == null) return;

            ChangeService?.OnComponentChanging(TableLayoutPanel, null);
            int rowIndex = TableLayoutPanel.RowCount;
            TableLayoutPanel.RowCount++;
            TableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            for (int i = 0; i < TableLayoutPanel.ColumnCount; i++)
            {
                var label = new Label
                {
                    Text = $"Cell {rowIndex + 1},{i + 1}",
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill
                };
                TableLayoutPanel.Controls.Add(label, i, rowIndex);
            }
            ChangeService?.OnComponentChanged(TableLayoutPanel, null, null, null);
            Splitter?.Invalidate();
        }

        public void AddColumn()
        {
            if (TableLayoutPanel == null) return;

            ChangeService?.OnComponentChanging(TableLayoutPanel, null);
            int colIndex = TableLayoutPanel.ColumnCount;
            TableLayoutPanel.ColumnCount++;
            TableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            for (int i = 0; i < TableLayoutPanel.RowCount; i++)
            {
                var label = new Label
                {
                    Text = $"Cell {i + 1},{colIndex + 1}",
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill
                };
                TableLayoutPanel.Controls.Add(label, colIndex, i);
            }
            ChangeService?.OnComponentChanged(TableLayoutPanel, null, null, null);
            Splitter?.Invalidate();
        }

        public void RemoveLastRow()
        {
            if (TableLayoutPanel == null || TableLayoutPanel.RowCount <= 1) return;

            ChangeService?.OnComponentChanging(TableLayoutPanel, null);
            int rowIndex = TableLayoutPanel.RowCount - 1;

            for (int i = TableLayoutPanel.Controls.Count - 1; i >= 0; i--)
            {
                var control = TableLayoutPanel.Controls[i];
                if (TableLayoutPanel.GetRow(control) == rowIndex)
                {
                    TableLayoutPanel.Controls.Remove(control);
                    control.Dispose();
                }
            }

            TableLayoutPanel.RowStyles.RemoveAt(rowIndex);
            TableLayoutPanel.RowCount--;
            ChangeService?.OnComponentChanged(TableLayoutPanel, null, null, null);
            Splitter?.Invalidate();
        }

        public void RemoveLastColumn()
        {
            if (TableLayoutPanel == null || TableLayoutPanel.ColumnCount <= 1) return;

            ChangeService?.OnComponentChanging(TableLayoutPanel, null);
            int colIndex = TableLayoutPanel.ColumnCount - 1;

            for (int i = TableLayoutPanel.Controls.Count - 1; i >= 0; i--)
            {
                var control = TableLayoutPanel.Controls[i];
                if (TableLayoutPanel.GetColumn(control) == colIndex)
                {
                    TableLayoutPanel.Controls.Remove(control);
                    control.Dispose();
                }
            }

            TableLayoutPanel.ColumnStyles.RemoveAt(colIndex);
            TableLayoutPanel.ColumnCount--;
            ChangeService?.OnComponentChanged(TableLayoutPanel, null, null, null);
            Splitter?.Invalidate();
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Layout Presets"));
            items.Add(new DesignerActionPropertyItem("LayoutPreset", "Layout Preset", "Layout Presets", "Apply a built-in grid layout"));
            items.Add(new DesignerActionMethodItem(this, nameof(PresetSidebarLeft), "Sidebar + Content", "Layout Presets", "Fixed left sidebar with flexible content", true));
            items.Add(new DesignerActionMethodItem(this, nameof(PresetHeaderContentFooter), "Header / Content / Footer", "Layout Presets", "Fixed header and footer with flexible content", true));
            items.Add(new DesignerActionMethodItem(this, nameof(PresetGrid2x2), "2 × 2 Grid", "Layout Presets", "A four-cell equal grid", true));
            items.Add(new DesignerActionMethodItem(this, nameof(ScaffoldLayoutZones), "Scaffold Preset Zones", "Layout Presets", "Fill the current preset's zones with labelled tiles", true));

            items.Add(new DesignerActionHeaderItem("Business Templates"));
            items.Add(new DesignerActionPropertyItem("TemplateCategory", "Category", "Business Templates", "Filter templates by business category"));
            items.Add(new DesignerActionPropertyItem("Template", "Template", "Business Templates", "Apply a ready-made business template"));
            items.Add(new DesignerActionMethodItem(this, nameof(ScaffoldTemplateZones), "Scaffold Template Zones", "Business Templates", "Fill the current template's zones with labelled tiles", true));

            items.Add(new DesignerActionHeaderItem("Layout"));
            items.Add(new DesignerActionPropertyItem("ColumnCount", "Column Count", "Layout", "Number of columns"));
            items.Add(new DesignerActionPropertyItem("RowCount", "Row Count", "Layout", "Number of rows"));
            items.Add(new DesignerActionMethodItem(this, nameof(AddRow), "Add Row", "Layout", "Add a new row", true));
            items.Add(new DesignerActionMethodItem(this, nameof(AddColumn), "Add Column", "Layout", "Add a new column", true));
            items.Add(new DesignerActionMethodItem(this, nameof(RemoveLastRow), "Remove Last Row", "Layout", "Remove the last row", true));
            items.Add(new DesignerActionMethodItem(this, nameof(RemoveLastColumn), "Remove Last Column", "Layout", "Remove the last column", true));

            items.Add(new DesignerActionHeaderItem("Appearance"));
            items.Add(new DesignerActionPropertyItem("CellBorderStyle", "Cell Border Style", "Appearance", "Border style for cells"));
            items.Add(new DesignerActionPropertyItem("ShowSplitterGrips", "Show Splitter Grips", "Appearance", "Draw grip handles on the splitter borders"));
            items.Add(new DesignerActionPropertyItem("ShowCollapseHandles", "Show Collapse Handles", "Appearance", "Show collapse/expand chevrons on fixed panes"));

            return items;
        }

        /// <summary>
        /// TypeConverter that filters the <see cref="BusinessTemplate"/> dropdown to show only the
        /// templates belonging to the action list's currently selected <c>_templateCategory</c>.
        /// Falls back to the full list when the parent action list is unavailable.
        /// </summary>
        private sealed class TemplateInCategoryConverter : EnumConverter
        {
            public TemplateInCategoryConverter() : base(typeof(BusinessTemplate)) { }

            public override StandardValuesCollection? GetStandardValues(ITypeDescriptorContext? context)
            {
                if (context?.Instance is BeepMultiSplitterActionList actionList)
                {
                    var templates = actionList.TemplatesInCategory;
                    return new StandardValuesCollection(templates);
                }
                return base.GetStandardValues(context);
            }

            public override bool GetStandardValuesSupported(ITypeDescriptorContext? context) => true;
            public override bool GetStandardValuesExclusive(ITypeDescriptorContext? context) => true;
        }
    }
}
