using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls.Layouts;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    public class BeepLayoutControlDesigner : BaseBeepControlDesigner
    {
        private DesignerVerbCollection? _verbs;

        public BeepLayoutControl? LayoutControl => Component as BeepLayoutControl;

        public override DesignerVerbCollection Verbs
            => _verbs ??= new DesignerVerbCollection
            {
                new DesignerVerb("Rebuild Layout", (_, _) => ExecuteAction("Rebuild Layout", control => control.RebuildLayout())),
                new DesignerVerb("Clear Generated Layout", (_, _) => ExecuteAction("Clear Generated Layout", control => control.Controls.Clear())),
                new DesignerVerb("Use 2 x 2 Grid", (_, _) => ApplyTemplate(BeepLayoutControl.TemplateType.Grid, 2, 2)),
                new DesignerVerb("Use Vertical Stack", (_, _) => ApplyTemplate(BeepLayoutControl.TemplateType.VerticalStack))
            };

        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            return new DesignerActionListCollection
            {
                new BeepLayoutControlActionList(this)
            };
        }

        public void ExecuteAction(string description, Action<BeepLayoutControl> action)
        {
            if (LayoutControl == null)
            {
                return;
            }

            IDesignerHost? designerHost = GetService(typeof(IDesignerHost)) as IDesignerHost;
            DesignerTransaction? transaction = null;

            try
            {
                transaction = designerHost?.CreateTransaction(description);
                ChangeService?.OnComponentChanging(LayoutControl, null);

                action(LayoutControl);

                ChangeService?.OnComponentChanged(LayoutControl, null, null, null);
                transaction?.Commit();
            }
            catch
            {
                transaction?.Cancel();
                throw;
            }
        }

        public void ApplyTemplate(BeepLayoutControl.TemplateType template, int? gridRows = null, int? gridColumns = null)
        {
            SetProperty(nameof(BeepLayoutControl.Template), template);

            if (gridRows.HasValue)
            {
                SetProperty(nameof(BeepLayoutControl.GridRows), gridRows.Value);
            }

            if (gridColumns.HasValue)
            {
                SetProperty(nameof(BeepLayoutControl.GridColumns), gridColumns.Value);
            }

            LayoutControl?.RebuildLayout();
        }
    }

    public class BeepLayoutControlActionList : DesignerActionList
    {
        private readonly BeepLayoutControlDesigner _designer;

        public BeepLayoutControlActionList(BeepLayoutControlDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        protected BeepLayoutControl? LayoutControl => Component as BeepLayoutControl;

        [Category("Layout")]
        [Description("The layout template type")]
        public BeepLayoutControl.TemplateType Template
        {
            get => _designer.GetProperty<BeepLayoutControl.TemplateType>(nameof(BeepLayoutControl.Template));
            set => _designer.SetProperty(nameof(BeepLayoutControl.Template), value);
        }

        [Category("Layout")]
        [Description("Number of rows for Grid template")]
        public int GridRows
        {
            get => _designer.GetProperty<int>(nameof(BeepLayoutControl.GridRows));
            set => _designer.SetProperty(nameof(BeepLayoutControl.GridRows), value);
        }

        [Category("Layout")]
        [Description("Number of columns for Grid template")]
        public int GridColumns
        {
            get => _designer.GetProperty<int>(nameof(BeepLayoutControl.GridColumns));
            set => _designer.SetProperty(nameof(BeepLayoutControl.GridColumns), value);
        }

        public void UseInvoiceLayout() => _designer.ApplyTemplate(BeepLayoutControl.TemplateType.Invoice);
        public void UseProductLayout() => _designer.ApplyTemplate(BeepLayoutControl.TemplateType.Product);
        public void UseProfileLayout() => _designer.ApplyTemplate(BeepLayoutControl.TemplateType.Profile);
        public void UseReportLayout() => _designer.ApplyTemplate(BeepLayoutControl.TemplateType.Report);
        public void UseVerticalStackLayout() => _designer.ApplyTemplate(BeepLayoutControl.TemplateType.VerticalStack);
        public void UseHorizontalStackLayout() => _designer.ApplyTemplate(BeepLayoutControl.TemplateType.HorizontalStack);
        public void UseGridLayout() => _designer.ApplyTemplate(BeepLayoutControl.TemplateType.Grid, 3, 3);
        public void UseSplitContainerLayout() => _designer.ApplyTemplate(BeepLayoutControl.TemplateType.SplitContainer);
        public void UseDockLayout() => _designer.ApplyTemplate(BeepLayoutControl.TemplateType.Dock);
        public void UseGridTwoByTwoLayout() => _designer.ApplyTemplate(BeepLayoutControl.TemplateType.Grid, 2, 2);
        public void UseSplitWorkspaceLayout() => _designer.ApplyTemplate(BeepLayoutControl.TemplateType.SplitContainer);
        public void RestoreTemplateChildren() => _designer.ExecuteAction("Restore Template Children", control => control.RebuildLayout());
        public void ClearGeneratedLayoutChildren() => _designer.ExecuteAction("Clear Generated Layout", control => control.Controls.Clear());

        public void RebuildLayout()
        {
            if (LayoutControl != null)
            {
                _designer.ExecuteAction("Rebuild Layout", control => control.RebuildLayout());
            }
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Layout Templates"));
            items.Add(new DesignerActionMethodItem(this, nameof(UseInvoiceLayout), "Invoice Layout", "Layout Templates", true));
            items.Add(new DesignerActionMethodItem(this, nameof(UseProductLayout), "Product Layout", "Layout Templates", true));
            items.Add(new DesignerActionMethodItem(this, nameof(UseProfileLayout), "Profile Layout", "Layout Templates", true));
            items.Add(new DesignerActionMethodItem(this, nameof(UseReportLayout), "Report Layout", "Layout Templates", true));
            items.Add(new DesignerActionMethodItem(this, nameof(UseVerticalStackLayout), "Vertical Stack", "Layout Templates", true));
            items.Add(new DesignerActionMethodItem(this, nameof(UseHorizontalStackLayout), "Horizontal Stack", "Layout Templates", true));
            items.Add(new DesignerActionMethodItem(this, nameof(UseGridLayout), "Grid Layout", "Layout Templates", true));
            items.Add(new DesignerActionMethodItem(this, nameof(UseSplitContainerLayout), "Split Container", "Layout Templates", true));
            items.Add(new DesignerActionMethodItem(this, nameof(UseDockLayout), "Dock Layout", "Layout Templates", true));

            items.Add(new DesignerActionHeaderItem("Generated Layout"));
            items.Add(new DesignerActionMethodItem(this, nameof(RestoreTemplateChildren), "Restore Template Children", "Generated Layout", true));
            items.Add(new DesignerActionMethodItem(this, nameof(ClearGeneratedLayoutChildren), "Clear Generated Children", "Generated Layout", true));
            items.Add(new DesignerActionMethodItem(this, nameof(UseGridTwoByTwoLayout), "Use 2 x 2 Grid", "Generated Layout", true));
            items.Add(new DesignerActionMethodItem(this, nameof(UseSplitWorkspaceLayout), "Use Split Workspace", "Generated Layout", true));

            items.Add(new DesignerActionHeaderItem("Layout Actions"));
            items.Add(new DesignerActionMethodItem(this, nameof(RebuildLayout), "Rebuild Layout", "Layout Actions", true));

            items.Add(new DesignerActionHeaderItem("Layout Properties"));
            items.Add(new DesignerActionPropertyItem(nameof(Template), "Template", "Layout Properties"));
            items.Add(new DesignerActionPropertyItem(nameof(GridRows), "Grid Rows", "Layout Properties"));
            items.Add(new DesignerActionPropertyItem(nameof(GridColumns), "Grid Columns", "Layout Properties"));

            return items;
        }
    }
}
