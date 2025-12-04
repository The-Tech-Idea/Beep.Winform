using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers.Actions;
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
            get => _designer.GetProperty<string>("Separator") ?? "/";
            set => _designer.SetProperty("Separator", value);
        }

        [Category("Behavior")]
        public bool ShowHomeIcon
        {
            get => _designer.GetProperty<bool>("ShowHomeIcon");
            set => _designer.SetProperty("ShowHomeIcon", value);
        }

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
            items.Add(new DesignerActionPropertyItem("Separator", "Separator", "Appearance"));
            items.Add(new DesignerActionPropertyItem("ShowHomeIcon", "Show Home Icon", "Appearance"));

            items.Add(new DesignerActionHeaderItem("Separator Presets"));
            items.Add(new DesignerActionMethodItem(this, "UseSlashSeparator", "/ Slash", "Separator Presets", false));
            items.Add(new DesignerActionMethodItem(this, "UseChevronSeparator", "> Chevron", "Separator Presets", false));
            items.Add(new DesignerActionMethodItem(this, "UseArrowSeparator", "→ Arrow", "Separator Presets", false));
            items.Add(new DesignerActionMethodItem(this, "UseDotSeparator", "• Dot", "Separator Presets", false));

            return items;
        }
    }
}

