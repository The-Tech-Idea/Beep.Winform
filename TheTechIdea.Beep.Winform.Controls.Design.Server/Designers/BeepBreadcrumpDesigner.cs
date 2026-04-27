using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Design-time support for BeepBreadcrump control
    /// </summary>
    public class BeepBreadcrumpDesigner : BaseBeepControlDesigner
    {
        public BeepBreadcrump? Breadcrump => Component as BeepBreadcrump;

        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            var lists = new DesignerActionListCollection();
            lists.Add(new BeepBreadcrumpActionList(this));
            return lists;
        }
    }

    public class BeepBreadcrumpActionList : DesignerActionList
    {
        private readonly BeepBreadcrumpDesigner _designer;

        public BeepBreadcrumpActionList(BeepBreadcrumpDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        #region Properties

        [Category("Appearance")]
        public string Separator
        {
            get => _designer.GetProperty<string>("SeparatorText") ?? ">";
            set => _designer.SetProperty("SeparatorText", value);
        }

        [Category("Behavior")]
        public bool ShowHomeIcon
        {
            get => _designer.GetProperty<bool>("ShowHomeIcon");
            set => _designer.SetProperty("ShowHomeIcon", value);
        }

        [Category("Appearance")]
        public BreadcrumbStyle BreadcrumbStyle
        {
            get => _designer.GetProperty<BreadcrumbStyle>("BreadcrumbStyle");
            set => _designer.SetProperty("BreadcrumbStyle", value);
        }

        #endregion

        #region Style Presets

        public void SetClassicStyle() => BreadcrumbStyle = BreadcrumbStyle.Classic;
        public void SetModernStyle() => BreadcrumbStyle = BreadcrumbStyle.Modern;
        public void SetPillStyle() => BreadcrumbStyle = BreadcrumbStyle.Pill;
        public void SetFlatStyle() => BreadcrumbStyle = BreadcrumbStyle.Flat;
        public void SetChevronStyle() => BreadcrumbStyle = BreadcrumbStyle.Chevron;

        #endregion

        #region Separator Presets

        public void UseSlashSeparator() => Separator = "/";
        public void UseChevronSeparator() => Separator = ">";
        public void UseArrowSeparator() => Separator = "→";
        public void UseDotSeparator() => Separator = "•";

        #endregion

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Appearance"));
            items.Add(new DesignerActionPropertyItem("BreadcrumbStyle", "Style", "Appearance"));
            items.Add(new DesignerActionPropertyItem("Separator", "Separator", "Appearance"));
            items.Add(new DesignerActionPropertyItem("ShowHomeIcon", "Show Home Icon", "Appearance"));

            items.Add(new DesignerActionHeaderItem("Style Presets"));
            items.Add(new DesignerActionMethodItem(this, "SetClassicStyle", "Classic", "Style Presets", false));
            items.Add(new DesignerActionMethodItem(this, "SetModernStyle", "Modern (Default)", "Style Presets", false));
            items.Add(new DesignerActionMethodItem(this, "SetPillStyle", "Pill", "Style Presets", false));
            items.Add(new DesignerActionMethodItem(this, "SetFlatStyle", "Flat", "Style Presets", false));
            items.Add(new DesignerActionMethodItem(this, "SetChevronStyle", "Chevron", "Style Presets", false));

            items.Add(new DesignerActionHeaderItem("Separator Presets"));
            items.Add(new DesignerActionMethodItem(this, "UseSlashSeparator", "/ Slash", "Separator Presets", false));
            items.Add(new DesignerActionMethodItem(this, "UseChevronSeparator", "> Chevron", "Separator Presets", false));
            items.Add(new DesignerActionMethodItem(this, "UseArrowSeparator", "→ Arrow", "Separator Presets", false));
            items.Add(new DesignerActionMethodItem(this, "UseDotSeparator", "• Dot", "Separator Presets", false));

            return items;
        }
    }
}

